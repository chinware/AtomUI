# Space Performance Notes

## Scope

本轮补充普通 `Space.Children` 同步路径。此前 `CompactSpace` 已去掉 add/remove item filter LINQ，本轮覆盖非 split-template 的普通 `Space` visual/logical children 同步。

## Root Cause

- `Space.HandleChildrenChanged` add/remove 原先分别对 `LogicalChildren` 和 `VisualChildren` 使用 `OfType<Control>()` / `OfType<Visual>()`。
- add 路径传入 LINQ iterator 会避开 Avalonia `InsertRange` 的 `IList` 快路径。参考 `.referenceprojects/Avalonia/src/Avalonia.Base/Collections/AvaloniaList.cs:343-427`。
- remove 路径使用 `RemoveAll` 会创建 `HashSet<T>` 并扫描目标集合；普通 `Space.Children` 与 logical/visual children 是镜像顺序，已有 `OldStartingIndex` / `OldItems.Count` 可以直接 range 删除。参考 `.referenceprojects/Avalonia/src/Avalonia.Base/Collections/AvaloniaList.cs:530-550`。

## Changes

- 普通 `Space` add 路径改为 indexed collect 后分别传入 `LogicalChildren.InsertRange` / `VisualChildren.InsertRange`。
- 普通 `Space` remove 路径改为 `RemoveRange(OldStartingIndex, OldItems.Count)`。
- split-template 路径仍走 `HandleSplitTemplateChanged()`，布局、spacing token、compact-space 行为不变。

## Benefit

这轮是结构性收益：普通 Space 动态 children add/remove 少走 LINQ 类型过滤；remove 从 HashSet + 全集合扫描降为按索引删除。不声明本轮页面加载耗时提升。

| Metric | Baseline | Optimized | Formula | Improvement | Conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Space collection sync LINQ type-filter callsites | 4 | 0 | `(4 - 0) / 4` | 100.00% | 有效；logical/visual add/remove 均去掉 LINQ 类型过滤 |
| Space mirrored remove target scans | 2 scans | 0 scans | `(2 - 0) / 2` | 100.00% | 有效；logical/visual remove 均改为 indexed range remove |
| Split-template behavior changes | 0 | 0 | n/a | 0.00% | 行为保持；split-template 仍整批刷新 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` passed.
- `--verify-space-states` 当前仍受旧 CompactSpace filler / token spacing 断言影响，不能作为本轮普通 Space 集合同步路径的通过证明。

---

## CompactSpaceItem Transform Reuse

`CompactSpaceItem.ArrangeOverride()` 会按相邻边框厚度给 item 设置 offset transform。旧路径在 offset 发生变化时重新创建 `TranslateTransform`；本轮在已有 transform 为 `TranslateTransform` 时直接更新 `X/Y`，相同 offset 仍保持短路。

Avalonia 依据：

- `.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:92-100`, `:644-710`：`Visual.RenderTransform` 会订阅 mutable transform 的 `Changed` 并触发 render invalidation。
- `.referenceprojects/Avalonia/src/Avalonia.Base/Media/TranslateTransform.cs:14-22`, `:70-75`：`TranslateTransform.X/Y` 变化会 raise changed。

| Metric | Baseline | Optimized | Formula | Improvement | Conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| CompactSpaceItem changed-offset transform allocations / arrange after first | 1 | 0 | `(1 - 0) / 1` | 100.00% | 结构收益；复用已有 `TranslateTransform` |
| CompactSpaceItem same-offset transform writes / arrange | 0 | 0 | n/a | 0.00% | 既有短路保留 |

说明：这是 compact layout arrange 路径 structural-only 收益；没有新增页面 timing 对比，不声明页面加载速度提升。
