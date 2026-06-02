# Badge 性能优化

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

## Preset Color Span Parse

`BadgeColorUtils.CalculateColor()` 和 `AbstractRibbonBadge.SetupRibbonColor()` 的颜色字符串解析旧路径会先 `Trim()` 生成临时字符串；此前已去掉 `ToLower()` 临时字符串，本轮继续改为 span trim，并直接调用 Avalonia `Color.TryParse(ReadOnlySpan<char>)`。preset 颜色和自定义颜色优先级保持不变。

Avalonia source reference：`.referenceprojects/Avalonia/src/Avalonia.Base/Media/Color.cs:208` 暴露 `Color.TryParse(ReadOnlySpan<char>, out Color)`。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Badge preset/custom color trim temp strings / color parse | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；span trim 保持 trim 语义 |
| Ribbon preset/custom color trim temp strings / color parse | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；span trim 保持 trim 语义 |
| Custom color parse string wrappers / color parse | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；直接调用 span overload |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Preset Color Name Table

`BadgeColorUtils.CalculateColor()` 和 `AbstractRibbonBadge.SetupRibbonColor()` 在 span trim 后仍会对每个 preset 调用 `presetColor.Type.ToString()` 做大小写无关匹配；命中 preset 时还会通过 `PresetPrimaryColor.Color()` 重新解析 hex 字符串。

本轮改为共享静态 preset 名称 / RGB 表：Badge 和 Ribbon 共用 `BadgeColorUtils.TryGetPresetColor()`，匹配路径不再做 enum name 转换，命中 preset 也不再每次解析 hex 字符串。自定义颜色仍走 `Color.TryParse(ReadOnlySpan<char>)`，优先级保持 preset 先于 custom parse。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Badge preset enum name conversions / worst-case parse | 14 conversions | 0 conversions | `(14 - 0) / 14` | 100.00% | 结构收益；静态名称表替代 `Type.ToString()` |
| Ribbon preset enum name conversions / worst-case parse | 14 conversions | 0 conversions | `(14 - 0) / 14` | 100.00% | 结构收益；复用同一静态名称表 |
| Preset color hex parses / matched preset parse | 1 parse | 0 parses | `(1 - 0) / 1` | 100.00% | 结构收益；静态 RGB color 直接用于 brush |

---

## Verification

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed.
- `dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-build -- --verify-badge-states` passed.
