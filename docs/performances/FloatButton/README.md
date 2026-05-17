# FloatButton Performance

## Scope

本轮针对 `FloatButton` / `BackTopFloatButton` / `FloatButtonGroup` / `FloatButtonHost` 做完整性能优化，原则是：

- 未启用 badge 的按钮不创建 badge 容器和 adorner。
- 未打开的 trigger group 不创建 menu `MotionActor` 和 `FloatButtonItemsControl`。
- 代码创建的 binding、事件订阅、overlay 子控件必须有对称释放路径。
- 保留外观、动画和交互语义；性能优化不能改变 UI 行为。

## Phase 0 Baseline

控件级基线命令：

```bash
dotnet run --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite floatbutton --count 30 --markdown /tmp/atomui-floatbutton-before.md
```

Gallery 基线命令：

```bash
dotnet run --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase floatbutton --label before --warmup 3 --iterations 20 --cold-iterations 3 --markdown /tmp/atomui-floatbutton-gallery-before.md --timeout-ms 15000
```

关键基线结论：

| 场景 | ms/item | KB/item | Visual/root | 主要问题 |
| --- | ---: | ---: | ---: | --- |
| `FloatButtonHost.Default` | 0.870 | 160.8 | 12 | 默认无 badge 仍创建 badge canvas |
| `FloatButtonGroupHost.DefaultCircle` | 3.630 | 611.3 | 44 | 圆形组仍创建隐藏 separator 层 |
| `FloatButtonGroupHost.TriggerClickClosed` | 2.145 | 538.9 | 41 | 关闭态预创建 menu actor 和 items |
| `FloatButtonGroupHost.TriggerHoverClosed` | 1.944 | 539.1 | 41 | 同上 |
| `FloatButton.GalleryShape` | 14.780 | 3561.5 | 265 | Gallery 组合中 closed trigger 与 badge 空槽成本明显 |

## Implemented Changes

| Phase | 改动 | 风险控制 |
| --- | --- | --- |
| Phase 1 | `FloatButtonHost` / `BackTopFloatButtonHost` / `FloatButtonGroupHost` 的 relay binding 全部进入 `CompositeDisposable` | detach 时释放；overlay child 从缓存的 overlay layer 移除，避免 detached 后 `FindLayer` 失败 |
| Phase 2 | badge canvas 改为 `IsBadgeEnabled=True` 时创建；badge adorner 替换/关闭时释放 binding 和 visual parent | `PART_BadgeRoot` 承载按需 canvas；`PART_BadgeLayout` 只在启用 badge 后出现 |
| Phase 3 | trigger group 关闭态只保留 trigger button；首次打开才创建 menu `MotionActor + FloatButtonItemsControl` | close 后复用已创建内容；detach/re-template 时释放 menu binding、templated parent 和 child visual parent |
| Phase 4 | group 子项 Shape / Motion 从长期 binding 改为显式同步 | 避免子项 binding 泄露；remove/detach 时清理 group override 和 embed 状态 |
| Phase 5 | 圆形 `FloatButtonItemsControl` 不再创建隐藏 separator layer | square 仍保留 separator 绘制 |
| Phase 6 | `FloatButtonItemsControl.ArrangeOverride()` 去掉 LINQ `Where().ToList()`，复用临时 buffer | square separator line 仍在 arrange 后生成 |
| Phase 7 | 补充专项 verifier | 覆盖 overlay cleanup、badge lifecycle、trigger menu lazy/reuse、BackTop badge part |

## Control-Level Result

同一台机器 Debug headless，`count=30`。单项 timing 存在波动，因此主要看结构和分配；真实用户体感以后面的 Gallery 数据为准。

| Scenario | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| `FloatButtonHost.Default` visual/root | 12 | 11 | 8.33% less |
| `FloatButtonGroupHost.TriggerClickClosed` visual/root | 41 | 14 | 65.85% less |
| `FloatButtonGroupHost.TriggerClickClosed` KB/item | 538.9KB | 196.4KB | 63.55% less |
| `FloatButtonGroupHost.TriggerClickClosed` ms/item | 2.145ms | 1.003ms | 53.24% faster |
| `FloatButtonGroupHost.TriggerHoverClosed` visual/root | 41 | 14 | 65.85% less |
| `FloatButtonGroupHost.TriggerHoverClosed` KB/item | 539.1KB | 196.8KB | 63.50% less |
| `FloatButtonGroupHost.TriggerHoverClosed` ms/item | 1.944ms | 0.927ms | 52.31% faster |
| `FloatButton.GalleryShape` visual/root | 265 | 197 | 25.66% less |
| `FloatButton.GalleryShape` KB/item | 3561.5KB | 2838.6KB | 20.30% less |

默认 group 的 visual 和 KB 已下降，但 headless timing 波动较大，不把单次 `ms/item` 作为成功指标。后续如果继续压默认 group，应单独建立多进程重复样本，避免被 headless layout 抖动误导。

## Gallery Result

真实 `FloatButtonShowCase.axaml`，`cold-iterations=3`，`warmup=3`，`iterations=20`。

| Metric | Before | After | Improvement |
| --- | ---: | ---: | ---: |
| Cold first navigation mean | 331.83ms | 309.94ms | 6.60% faster |
| Cold first navigation median | 331.73ms | 310.75ms | 6.32% faster |
| Cold first navigation P95 | 337.13ms | 312.10ms | 7.42% faster |
| Repeated navigation mean | 126.71ms | 110.81ms | 12.55% faster |
| Repeated navigation median | 100.11ms | 82.52ms | 17.57% faster |
| Repeated navigation P95 | 226.86ms | 181.88ms | 19.83% faster |
| Repeated alloc mean | 14447.92KB | 12182.94KB | 15.68% less |
| Visual nodes | 949 | 752 | 20.76% less |
| Icon nodes | 50 | 38 | 24.00% less |
| IconPresenter nodes | 51 | 39 | 23.53% less |
| PathIcon nodes | 50 | 38 | 24.00% less |
| MotionActor nodes | 17 | 11 | 35.29% less |

结论：符合本轮优化预期。主要收益来自 closed trigger group 不再预创建菜单内容，以及默认无 badge 不再承担 badge 容器成本。页面仍有固定成本，包括 11 个 ShowCaseItem、39 个 icon provider/source icon，以及 Gallery route/layout 本身。

## Verification

```bash
dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -v:minimal
dotnet run --framework net10.0 --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-floatbutton-states
dotnet run --framework net10.0 --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- --showcase floatbutton --label after --warmup 3 --iterations 20 --cold-iterations 3 --markdown /tmp/atomui-floatbutton-gallery-after.md --timeout-ms 15000
```

Verifier 覆盖：

- host detach 后 overlay child 字段、binding disposables 和 visual parent 清理。
- badge count/dot 切换不累积 adorner。
- badge disabled 后释放 adorner 和 binding。
- BackTopFloatButton 启用 badge 时仍暴露 `PART_BadgeLayout`。
- trigger group closed 初始状态不创建 menu items control / motion actor。
- trigger group open/close/open 复用同一个 menu content，不重复创建。
- default group 仍立即创建 items control 和子按钮。
- group Shape 变化会同步到 embedded 子按钮，detach 后清理 group override。
