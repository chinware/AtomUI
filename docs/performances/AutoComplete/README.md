# AutoComplete Performance Notes

## Scope

本页记录 `AutoComplete` 已完成优化和本轮追加的低风险结构性收敛。

既有主线优化：closed popup content 已按需创建；`AutoCompleteShowCase` repeated mean `74.97ms -> 55.63ms`，alloc `8143KB -> 7664.37KB`。

## Additional Structural Pass

补全路径里需要比较当前输入和候选项前缀是否相等。旧实现对 `Value` 和 `topString` 分别 `Substring(0, minLength)`，再执行 `string.Equals()`；本轮改为 `string.Compare(Value, 0, topString, 0, minLength, StringComparison.CurrentCultureIgnoreCase)`，保留 `CurrentCultureIgnoreCase` 语义，并显式保留两个字符串同为 `null` 时的旧行为。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Completion prefix substring allocations / accepted completion | 2 strings | 0 strings | `(2 - 0) / 2` | 100.00% | 结构收益；直接区间比较 |
| Prefix comparison culture | CurrentCultureIgnoreCase | CurrentCultureIgnoreCase | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `--verify-autocomplete-states` currently fails on both this change and clean HEAD `d35b21a1a` with the same closed-popup / detach cleanup / duplicate close-event expectations, so it is not introduced by this optimization.
