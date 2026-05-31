# Rate Performance Notes

## Scope

本页记录 `Rate` 已完成优化和本轮追加的低风险结构性收敛。

既有主线优化：默认星形、焦点虚线框和全局 input 订阅成本已收敛；`RateShowCase` repeated mean 提升约 `10.96%`。

## Additional Structural Pass

`RateItem` 在把字符串 `Character` 转成 `RateCharacter` 时只需要首个 `char`。旧实现使用 `str.First()`；调用点已有 `str.Length >= 1` 保护，因此改为 `str[0]`，保持取首字符语义不变，并去掉一次 LINQ enumerator。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| String character first lookup LINQ calls / rate item brush rebuild | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；直接读 `string[0]` |
| First character semantics | first `char` | first `char` | n/a | 0.00% | 正确性保持不变 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `--verify-rate-states` currently fails on both this change and clean HEAD `d35b21a1a` with the same tooltip / focus border / RateItem reuse expectations, so it is not introduced by this optimization.
