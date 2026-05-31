# Mentions Performance Notes

## Scope

本页记录 `Mentions` 已完成优化和本轮追加的低风险结构性收敛。

既有主线优化：closed popup content 已按需创建；`MentionsShowCase` `_candidateList 15 -> 0`，repeated mean `110.05ms -> 106.95ms`，alloc `10115.79KB -> 9326.47KB`；OptionsSource cache list 使用已知容量。

## Additional Structural Pass

过滤视图刷新后，候选列表默认选中第一项。旧实现使用 `_view.First()`；`_view` 类型是 `IList<IMentionOption>?`，调用点已有 `_view?.Count > 0` 保护，因此改为 `_view[0]`，保持首项选中语义不变，并去掉一次 LINQ enumerator。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Filtered view first selected lookup LINQ calls / filter refresh | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；直接读 `IList[0]` |
| Candidate default selection semantics | first item | first item | n/a | 0.00% | 正确性保持不变 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `--verify-mentions-states` currently fails on both this change and clean HEAD `d35b21a1a` with the same closed-popup / detach cleanup / loading indicator expectations, so it is not introduced by this optimization.
