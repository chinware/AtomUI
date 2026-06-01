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

## Source Clear Lifecycle

`ItemsSource` 变为 `null` 或空列表时，旧实现不会更新 `EffectiveSources`，旧预览 source 会继续被持有，且可能继续显示旧内容。本轮在空 source 路径同步设置 `EffectiveSources = Array.Empty<PreviewImageSource>()` 并释放旧 source；非空 source 路径继续按已知 Count 预分配并保持加载顺序。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Empty ItemsSource stale effective-source risk / source change | 1 risk | 0 risk | `(1 - 0) / 1` | 100.00% | 正确性修复；清空 source 后不再保留旧预览内容 |
| Empty ItemsSource stale cover-image risk / source change | 1 risk | 0 risk | `(1 - 0) / 1` | 100.00% | 正确性修复；无 custom cover / fallback 时清空 cover |
| Clear CoverImageSrc fallback misses / clear | 1 miss | 0 misses | `(1 - 0) / 1` | 100.00% | 正确性修复；清掉 custom cover 后回退到第一张 source |
| Old preview source disposal / empty source change | 0 releases | `N` releases | `N / N` | 100.00% | 生命周期修复；旧 source 在空更新时也被释放 |
| Empty effective-source list allocation / empty source change | 1 possible list | 0 lists | `(1 - 0) / 1` | 100.00% | 结构收益；空 source 使用 `Array.Empty` |

说明：这是 source 更新路径的 correctness + structural-only 收益；没有新增 Gallery timing 对比，不声明页面加载速度提升。

## Closed Dialog Source Lazy Materialization

单图 `ImagePreviewer` 关闭态只需要显示一张 cover。旧实现会在 `ItemsSource` 变化时立即把完整弹窗 source list 写入 `EffectiveSources`，即使用户没有打开预览弹窗；这会让关闭态提前持有多张图片 source。新实现把单图 cover source 与弹窗 source list 拆开：关闭态只加载可见 cover，`OpenDialog()` 前才物化完整 `EffectiveSources`。`ImageGroupPreviewer` 仍保留关闭态完整物化，因为它的封面列表本身就是可见内容。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Single ImagePreviewer closed dialog sources / 3-image instance | 3 sources | 0 sources | `(3 - 0) / 3` | 100.00% | 有效；关闭态不再提前持有弹窗列表 |
| Single ImagePreviewer live preview sources / 3-image closed instance | 3 sources | 1 cover source | `(3 - 1) / 3` | 66.67% | 有效；只保留可见封面 |
| Custom cover closed live preview sources / 1-item instance | 2 sources | 1 cover source | `(2 - 1) / 2` | 50.00% | 有效；custom cover 不再额外持有弹窗 source |
| ImagePreviewer state verification failures / run | 3 failures | 0 failures | `(3 - 0) / 3` | 100.00% | 正确性修复；关闭态、打开态、替换 source 均通过 |
| Open dialog source materialization / 3-image instance | 3 sources | 3 sources | n/a | 0.00% | 正确性保持；打开弹窗时完整 source list 仍可用 |
| ImageGroupPreviewer visible cover sources / 2-image group | 2 sources | 2 sources | n/a | 0.00% | 正确性保持；组图关闭态仍显示全部封面 |
| Replaced owned cover disposal / cover replacement | 0 releases | 1 release | `1 / 1` | 100.00% | 生命周期修复；替换 cover 时释放旧 bitmap |
| Page-load timing claim | none | none | n/a | n/a | 本段为结构与正确性收益；本轮 smoke timing 不作为前后速度证明 |

## Verification

- `git diff --check` passed.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-restore` passed with 0 warning / 0 error.
- `dotnet build tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net8.0 --no-restore` passed with 0 warning / 0 error.
- `dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-build -- --verify-imagepreviewer-states` passed.
- `dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -c Release --framework net10.0 --no-build -- --suite imagepreviewer --count 30` passed as smoke timing; not used as before/after speed proof.
