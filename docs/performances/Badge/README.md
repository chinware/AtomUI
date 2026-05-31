# Badge 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md)
> 状态：本轮追加 render / motion structural cleanup；不声明页面级 timing 收益。

---

## Ribbon Corner Brush Cache

`AbstractRibbonBadgeAdorner.Render()` 原先每次 render 都根据 `RibbonColor` 派生暗角颜色并创建一个新的 `SolidColorBrush`。Ribbon badge 出现在列表、卡片或复杂页面时，render 重入会重复分配同色 brush。

本轮把暗角 brush 缓存在 adorner 实例上；当 `RibbonColor.Color` 或 `BadgeRibbonCornerDarkenAmount` 变化时才重建。`BadgeRibbonCornerDarkenAmount` 同时加入 `AffectsRender`，确保运行时变化能触发重绘。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Ribbon corner brush allocations / render | 1 | 0 after first | `(1 - 0) / 1` | 100.00% | 结构收益；同色重复 render 不再分配 brush |
| Runtime darken-amount redraw path | implicit/unstable | explicit `AffectsRender` | n/a | n/a | 正确性补强；视觉输出保持一致 |

说明：这是 render 路径 structural-only 收益；没有新增 BadgeShowCase timing 对比，不声明页面加载速度提升。

---

## Badge Motion Transform Cache

`AbstractCountBadgeAdorner` / `AbstractDotBadgeAdorner` 在 show 前会把 motion actor 缩放到 `0.01`，`BadgeZoomBadgeInMotion` / `BadgeZoomBadgeOutMotion` 的 start/end 也会构建 `0.01` 与 `1.0` 两个固定 scale transform。旧路径每次 show/hide 都重新创建这些 transform。

本轮改为共享 `TransformOperations` scale 值。`TransformOperations` 由 builder 创建后对外只暴露只读 operations 和已计算的 `Value`（`.referenceprojects/Avalonia/src/Avalonia.Base/Media/Transformation/TransformOperations.cs:10-35`, `:168-235`），因此这里不复用可变 `ScaleTransform` 实例。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| CountBadge scale transform allocations / show | 3 | 0 | `(3 - 0) / 3` | 100.00% | 结构收益；pre-seed + motion start/end 均复用 |
| DotBadge scale transform allocations / show | 3 | 0 | `(3 - 0) / 3` | 100.00% | 结构收益；pre-seed + motion start/end 均复用 |
| Badge scale transform allocations / hide | 2 | 0 | `(2 - 0) / 2` | 100.00% | 结构收益；motion start/end 均复用 |

说明：这是 show/hide motion 路径 structural-only 收益；没有新增 BadgeShowCase timing 对比，不声明页面加载速度提升。

---

## Badge Mode Lifecycle Correctness

复测 `--verify-badge-states` 时发现 standalone / target 模式互切存在旧父子关系残留：`CountBadgeAdorner` 会同时保留在 standalone visual subtree 与 target adorner path 中，切回 standalone 时触发重复 visual parent 异常。`DotBadge` 还会复用已套用 standalone 模板的 adorner，target 模式仍残留 `Label` 模板子树。

本轮把 Count / Dot / Ribbon 的模式切换统一为先 detach 旧 child / adorner layer，再 attach 当前模式需要的 child；`DotBadge` 在 standalone / target 模式互切时重建 adorner，避免复用错误模板状态。`CountBadge` 还修复了 hidden zero 不应创建 adorner，以及 `IsZeroVisible=true` 时零值 badge 需要恢复显示。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Hidden zero `CountBadgeAdorner` visuals / hidden CountBadge | 1 | 0 | `(1 - 0) / 1` | 100.00% | 正确性 + 结构收益；隐藏态不再创建 adorner |
| Target `DotBadge` Label visuals / adorner | 1 | 0 | `(1 - 0) / 1` | 100.00% | 正确性 + 结构收益；target 模式不残留 standalone label |
| Badge standalone/target switch verification failures | 1+ / run | 0 / run | n/a | n/a | 正确性修复；不作为 timing 收益 |

说明：这是正确性修复和 visual subtree 结构收益；没有新增 BadgeShowCase timing 对比，不声明页面加载速度提升。

---

## Verification

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed.
- `dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-badge-states` passed.
