# Collapse Performance Notes

## Pointer Release Hit-Test

`Collapse.OnPointerReleased()` 触控路径原先在 `GetVisualsAt()` 后使用 `.Any(predicate)` 判断 release 是否仍在当前 item 内。本轮改为共享 `VisualHitTestUtils.ContainsSelfOrDescendantAt()`，避免每次 pointer release 创建 LINQ predicate/iterator。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Collapse pointer-release hit-test LINQ operators / release | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；命中判断改为显式遍历 |
| Hit-test behavior | self/descendant match | self/descendant match | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Item Motion Easing Cache

`CollapseItem` 展开/折叠内容时仍按当前 duration 创建 `SlideUpInMotion` / `SlideUpOutMotion`，但默认 `CubicEaseOut` / `CubicEaseIn` 不再每次交互重复创建。Avalonia 两个 easing 类只有 `Ease(double)` override，没有实例字段（`.referenceprojects/Avalonia/src/Avalonia.Base/Animation/Easings/CubicEaseOut.cs:7-13`，`.referenceprojects/Avalonia/src/Avalonia.Base/Animation/Easings/CubicEaseIn.cs:7-13`）。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| CollapseItem default easing allocations / expand | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；展开不再新建默认 easing |
| CollapseItem default easing allocations / collapse | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；折叠不再新建默认 easing |
| CollapseItem motion objects / expand-or-collapse | 1 | 1 | `(1 - 1) / 1` | 0.00% | 行为保持；motion 仍按当前 duration 即时创建 |

说明：这是交互 motion 路径 structural-only 收益；没有新增 Gallery timing 对比，不声明页面加载速度提升。

## Verification

- `dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` passed.
