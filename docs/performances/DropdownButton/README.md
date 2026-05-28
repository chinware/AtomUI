# DropdownButton 性能与正确性优化

> 状态：本轮已完成生命周期优化。收益以 detached 状态不提前创建绑定、不残留 `MenuItemClicked` handler 为主；页面级速度不声明。

---

## 1. 范围

- `src/AtomUI.Desktop.Controls/Buttons/DropdownButton.cs`
- `tools/performances/AtomUI.Performance/Suites/DropdownButton/`
- `--suite dropdownbutton`
- `--verify-dropdownbutton-states`

## 2. 根因

旧实现里，`DropdownFlyout` 在对象初始化阶段设置时就会：

- 订阅 `MenuFlyout.MenuItemClicked`
- 创建 8 条 DropdownButton -> MenuFlyout property binding

这两件事发生在控件尚未 attach 到 visual tree 时。更严重的是，`OnDetachedFromVisualTree` 只释放了 binding，没有反订阅 `MenuItemClicked`，因此外部仍持有 `MenuFlyout` 时会通过事件链继续保留 DropdownButton。

## 3. 改动

- 增加 `_registeredDropdownFlyout`，让 flyout event/binding 注册幂等。
- `MenuFlyout.MenuItemClicked` 与 8 条 flyout property binding 只在 visual tree attached 后注册。
- detach 时按 `_registeredDropdownFlyout` 精确释放 `MenuItemClicked` 和 binding。
- `DropdownFlyout` 替换时，旧 flyout 的 handler 立即释放；新 flyout 只在 attached 状态注册。
- 新增独立 DropdownButton suite 和状态验证。

## 4. 收益

| 指标 | baseline | optimized | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| 状态验证失败数 | 3 | 0 | `(3 - 0) / 3` | 100.00% | 生命周期问题归零 |
| detached `MenuItemClicked` handlers / flyout | 1 | 0 | `(1 - 0) / 1` | 100.00% | 未 attach 时不再订阅菜单点击 |
| detached flyout property bindings / button | 8 | 0 | `(8 - 0) / 8` | 100.00% | 未 attach 时不再创建 flyout 绑定 |
| detach 后残留 `MenuItemClicked` handlers / flyout | 1 | 0 | `(1 - 0) / 1` | 100.00% | 移出 visual tree 后事件链释放 |
| menu item forwarding | 1 | 1 | `1 / 1` | 0.00% | 行为保持不变 |

## 5. 控件级 smoke 数据

> 单轮 headless `--suite dropdownbutton --count 80`，只作为 smoke 观察，不作为稳定页面级性能收益证明。

| Scenario | ms/item baseline | ms/item optimized | KB/item baseline | KB/item optimized | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| `DropdownButton.Default.Closed` | 3.013 | 2.710 | 398.5 | 388.0 | smoke-only |
| `DropdownButton.Primary.Icon.Closed` | 3.211 | 3.516 | 470.6 | 461.8 | smoke-only，耗时回归不作为收益 |
| `DropdownButton.NoFlyout.Closed` | 1.333 | 1.299 | 347.0 | 347.0 | smoke-only |
| `DropdownButton.NoIndicator.Closed` | 1.386 | 1.149 | 387.4 | 376.8 | smoke-only |
| `DropdownButton.Hover.Closed` | 1.577 | 1.461 | 400.2 | 389.4 | smoke-only |
| `DropdownButton.PlacementGrid.Closed` | 5.483 | 5.598 | 1609.4 | 1566.0 | smoke-only，耗时回归不作为收益 |
| `DropdownButton.Batch6.Closed` | 8.074 | 7.972 | 2403.7 | 2352.0 | smoke-only |

## 6. 验证

```
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-dropdownbutton-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite dropdownbutton --count 80
```

结果：

- build passed
- `DropdownButton state verification passed.`
- DropdownButton suite smoke passed

## 7. 不声明的收益

- 本轮未做 Gallery 页面级多样本测试，因此不声明 DropdownButton 页面加载速度提升。
- ms/item 是单轮 smoke 数据，且部分场景耗时有噪声回归；本轮收益以结构生命周期和分配下降为准。
