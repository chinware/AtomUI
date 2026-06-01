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

## Read-only OptionsSource Capacity

`Mentions.BuildItemsCache()` 已能对 `ICollection` source 按 Count 预分配。本轮补齐 `IReadOnlyCollection<IMentionOption>` 快路径，避免只读 options source 复制时从默认容量动态增长；选项枚举和类型转换语义不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Mentions options cache list growth / read-only OptionsSource copy | dynamic growth | exact read-only count | structural | 结构收益 | 只读 source 也按 Count 预分配 |
| OptionsSource copy semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Trigger Prefix Scan

`MentionTextArea` 输入和光标变化时会从 caret 向前扫描触发前缀。旧路径每检查一个字符都会构造 `ch.ToString()` 再调用 `TriggerPrefix.Contains()`；插入 mention 时还会用字符串插值比较前一个字符。本轮改成字符级前缀匹配，只在真正命中时复用已有 prefix 字符串，同时把前置分隔符比较改为 char 比较。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Trigger-prefix temp strings / scanned char | 1 string | 0 strings | `(1 - 0) / 1` | 100.00% | 结构收益；输入扫描不再为每个字符创建临时 string |
| Trigger-prefix scans using temp string / text check | 3 callsites | 0 callsites | `(3 - 0) / 3` | 100.00% | 结构收益；输入、caret、插入 mention 三条路径统一收敛 |
| Previous-char comparison temp strings / insert mention | 2 strings | 0 strings | `(2 - 0) / 2` | 100.00% | 结构收益；split/space 判断改为 char 比较 |

说明：这是输入交互路径 structural-only 收益；没有新增 Gallery timing 对比，不声明页面加载速度提升。

## Verification

- `git diff --check` passed.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net8.0 --no-restore` passed with 0 warning / 0 error.
- `--verify-mentions-states` still fails on the existing closed-popup / detach cleanup / loading indicator expectations documented previously; this batch did not touch those lazy popup, event-count, loading, or detach cleanup paths.
