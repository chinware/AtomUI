# Carousel Performance Notes

## Pagination Pointer Release Hit-Test

`CarouselPagination.OnPointerReleased()` 触控路径原先通过 `GetVisualsAt(...).Any(predicate)` 判断 release 是否仍在当前 pagination item 内。本轮改为共享 `VisualHitTestUtils.ContainsSelfOrDescendantAt()`，减少 LINQ predicate/iterator。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Carousel pagination pointer-release LINQ operators / release | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；命中判断改为显式遍历 |
| Hit-test behavior | self/descendant match | self/descendant match | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Progress Indicator Animation Cleanup

`CarouselPageIndicator` 的 progress animation 默认 easing 从每个 indicator 一次 `new LinearEasing()` 改为共享只读实例；强制重建动画时同时 dispose 旧 `CancellationTokenSource`，避免开关 progress/重建动画后留下已 cancel 但未释放的 CTS。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Progress animation default easing allocations / indicator | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；默认 easing 复用 |
| Force rebuild retained cancelled CTS / indicator | 1 | 0 | `(1 - 0) / 1` | 100.00% | 生命周期收益；重建前 cancel + dispose |

说明：这是 structural-only / lifecycle 收益；未新增 Gallery timing 对比，不声明页面加载速度提升。

## Verification

- `dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` passed.
