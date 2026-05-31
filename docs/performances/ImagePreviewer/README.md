# ImagePreviewer Performance Notes

## Scope

本轮补充 ImagePreviewer 图片 source 加载路径里的 SVG 扩展名判断。

本轮继续补充封面图 fallback：当 `EffectiveCoverImage` 为空且 `EffectiveSources.Count > 0` 时，旧实现两处使用 `EffectiveSources.First()`。`EffectiveSources` 是可索引集合，且调用点已有 Count 保护；现在改为 `EffectiveSources[0]`，保持取第一个 source 的行为，去掉两处 LINQ enumerator。

## Root Cause

旧实现先 `Path.GetExtension(filePath).ToLowerInvariant()`，再和 `.svg` 比较。这里每次判断都会产生一个 lowercase 临时字符串，而实际需求只是大小写不敏感比较扩展名。

新实现使用 `string.Equals(..., StringComparison.OrdinalIgnoreCase)`，保留大小写不敏感语义，同时去掉 lowercase 临时字符串。

## Benefit

这是 source path 判断路径的结构性收益，不声明页面加载 timing 提升。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| SVG extension lowercase temp strings / source path check | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；直接大小写不敏感比较 |
| Cover fallback first-source LINQ enumerators / source update + loaded | 2 | 0 | `(2 - 0) / 2` | 100.00% | 有效；Count 保护后直接 indexer |
| Case-insensitive SVG semantics | preserved | preserved | n/a | 0.00% | 正确性保持不变 |
| Cover fallback semantics | first source | first source | n/a | 0.00% | 正确性保持不变 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `--verify-imagepreviewer-states` currently fails on both this change and clean HEAD `d35b21a1a` with the same closed-source-list / clear-source expectations, so it is not introduced by this optimization.
