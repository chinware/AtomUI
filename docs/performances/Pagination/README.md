# Pagination Performance Notes

## NavItem Pointer Release Hit-Test

`PaginationNavItem.OnPointerReleased()` 原先用 `GetVisualsAt(...).Any(predicate)` 判断 release 是否仍在 nav item 内。本轮改为共享 `VisualHitTestUtils.ContainsSelfOrDescendantAt()`，避免 LINQ predicate/iterator。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Pagination nav item pointer-release LINQ operators / release | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；命中判断改为显式遍历 |
| Hit-test behavior | self/descendant match | self/descendant match | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Nav Container Preparation

`PaginationNav.PrepareContainerForItemOverride()` 原先每次 prepare 都给 `PaginationNavItem.Click` 增加一个匿名 handler，且没有 clear path。现在改为方法组订阅，prepare 前先去重，并在 `ClearContainerForItemOverride()` 对称解绑，避免 re-template / container clear 后重复 handler 残留。同时 `Pagination.HandleContainerPrepared()` 的 realized container 数量判断不再使用 LINQ `Count()` 扩展，改为显式计数并在超过期望值时早停。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Pagination nav item anonymous click handler callsites / prepare | 1 anonymous handler | 0 anonymous handlers | `(1 - 0) / 1` | 100.00% | 有效；改为可解绑的方法组 |
| Pagination nav item duplicate click handler risk / repeated prepare | possible | guarded | `1 -> 0 duplicate risk` | 100.00% | 正确性收益；prepare 前先解绑同一 handler |
| Pagination nav item click handler cleanup / clear container | missing | unsubscribed | `1 -> 0 retained handler risk` | 100.00% | 正确性收益；清理路径对称 |
| Pagination realized-container count LINQ calls / prepare | 1 `Count()` | 0 LINQ calls | `(1 - 0) / 1` | 100.00% | 结构收益；显式计数并早停 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Feature Lifecycle Cleanup

`Pagination.OnApplyTemplate()` 原先给新的 `PaginationNav` 订阅 `ContainerPrepared` / `PageNavigateRequest`，但重套模板时没有先释放旧 nav part；`SizeChanger` / `QuickJumperBar` 在对应 feature 关闭时也只是不再创建，没有释放已有动态部件。现在 re-template 前释放旧 nav 事件，`IsShowSizeChanger=false` / `IsShowQuickJumper=false` 时释放动态部件、binding 和事件。`QuickJumperBar` 自身也在 re-template / detach 时释放 `LineEdit.KeyUp`。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Pagination old nav handlers retained / re-template | 2 handlers | 0 handlers | `(2 - 0) / 2` | 100.00% | 正确性收益；旧 nav part 不再保留 Pagination |
| Pagination disabled feature controls retained / size changer + quick jumper | 2 controls | 0 controls | `(2 - 0) / 2` | 100.00% | 正确性收益；关闭 feature 时释放动态部件 |
| Pagination dynamic feature bindings retained / disabled features | 2 bindings | 0 bindings | `(2 - 0) / 2` | 100.00% | 正确性收益；释放 SizeType relay binding |
| QuickJumperBar anonymous jump handler callsites / setup | 1 anonymous handler | 0 anonymous handlers | `(1 - 0) / 1` | 100.00% | 结构收益；改为可解绑方法组 |
| QuickJumperBar LineEdit `KeyUp` handler retained / detach | 1 handler | 0 handlers | `(1 - 0) / 1` | 100.00% | 正确性收益；detach 清理 `_lineEdit` |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` passed.
- `dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-build -- --verify-pagination-states` still fails existing assertions around `SimplePagination` quick jumper and page item slots; the feature release and `QuickJumperBar` detach cleanup failures are no longer present.
