# SplitButton 性能与正确性优化

> 状态：本轮已完成正确性与生命周期优化。收益以事件/绑定生命周期收敛为主，页面级速度不声明。

---

## 1. 范围

- `src/AtomUI.Desktop.Controls/Buttons/SplitButton.cs`
- `tools/performances/AtomUI.Performance/Suites/SplitButton/`
- `--suite splitbutton`
- `--verify-splitbutton-states`

## 2. 根因

Avalonia `FlyoutBase` 通过 `Opened` / `Closed` 事件通知打开关闭（`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/FlyoutBase.cs:27-28`），`PopupFlyoutBase.ShowAtCore` 会把 `Target` 设置为传入的 placement target（`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/PopupFlyoutBase.cs:287`）。

SplitButton 的 flyout 由 secondary button 触发，因此真实 `Target` 是 `PART_SecondaryButton`，不是 SplitButton 自身。旧代码只接受 `flyout.Target == this`，导致 secondary 打开 flyout 时 `:flyout-open` 状态不更新。

另一个问题是生命周期重复注册：`Flyout` 属性变化时注册一次，`OnAttachedToVisualTree` 再注册一次，同一个 flyout 的 `Opened/Closed` 会多挂一组 SplitButton 自己的 handler。替换 flyout 时只释放一次，旧 flyout 还残留一组 handler。

## 3. 改动

- 增加 `_registeredFlyout`，让 SplitButton 自己的 flyout 事件和 9 条 flyout property binding 幂等注册。
- SplitButton 自己的 flyout 事件/绑定只在 visual tree attached 后注册；detached 状态只保留 `FlyoutStateHelper` 的基础监听。
- detach / flyout 替换时按 `_registeredFlyout` 精确释放，旧 flyout 不再残留 SplitButton handler。
- `HandleFlyoutOpened/Closed` 判断 target 时接受当前 SplitButton 和 `PART_SecondaryButton`，修复 secondary 打开状态不更新。
- 新增独立 SplitButton suite 和状态验证。

## 4. 收益

| 指标 | baseline | optimized | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| 状态验证失败数 | 7 | 0 | `(7 - 0) / 7` | 100.00% | 正确性问题归零 |
| detached flyout `Opened/Closed` handlers / flyout | 2 | 1 | `(2 - 1) / 2` | 50.00% | 未 attach 时不再注册 SplitButton 自己的 handler |
| attached flyout `Opened/Closed` handlers / flyout | 3 | 2 | `(3 - 2) / 3` | 33.33% | 同一个 flyout 不再重复挂 SplitButton handler |
| 替换 Flyout 后旧 flyout 残留 handlers | 1 | 0 | `(1 - 0) / 1` | 100.00% | 旧 flyout 事件链释放干净 |
| secondary button 打开时 `:flyout-open` 状态失败数 | 1 | 0 | `(1 - 0) / 1` | 100.00% | flyout 打开视觉状态恢复正确 |

## 5. 控件级 smoke 数据

> 单轮 headless `--suite splitbutton --count 80`，只作为 smoke 观察，不作为稳定页面级性能收益证明。

| Scenario | ms/item baseline | ms/item optimized | KB/item baseline | KB/item optimized | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| `SplitButton.Default.Closed` | 6.074 | 5.892 | 930.9 | 917.9 | smoke-only |
| `SplitButton.Primary.Icon.Closed` | 4.636 | 3.774 | 999.0 | 989.2 | smoke-only |
| `SplitButton.NoFlyout.Closed` | 4.696 | 2.609 | 877.4 | 877.4 | smoke-only |
| `SplitButton.Hover.Closed` | 13.921 | 2.493 | 926.3 | 916.6 | smoke-only |
| `SplitButton.Small.Danger.Closed` | 15.626 | 2.773 | 956.8 | 947.1 | smoke-only |
| `SplitButton.CompactSpace.Pair` | 9.837 | 7.144 | 1965.5 | 1946.0 | smoke-only |
| `SplitButton.Batch6.Closed` | 21.635 | 18.283 | 5613.1 | 5558.4 | smoke-only |

## 6. 验证

```
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-splitbutton-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite splitbutton --count 80
```

结果：

- build passed
- `SplitButton state verification passed.`
- SplitButton suite smoke passed

## 7. 不声明的收益

- 本轮未做 Gallery 页面级前后多样本测试，因此不声明 SplitButton 页面加载速度提升。
- ms/item 是单轮 smoke 数据，不能替代稳定多轮收益证明。
