# Card Performance Notes

## Scope

本轮补充 Card 动态集合同步路径：

- `Card.Actions -> CardActionPanel.Actions`
- `CardActionPanel.Actions -> UniformGrid.Children`
- `CardTabsContent.Items -> TabControl.Items`

## Root Cause

- `NotifyCollectionChangedEventArgs.NewItems/OldItems` 原先通过 `OfType<Control>()` 传入 `InsertRange` / `RemoveAll`，每个 add/remove 分支都会创建 LINQ 类型过滤迭代器。
- Avalonia `AvaloniaList.InsertRange` 对 `IList` 有快路径；传入 LINQ iterator 会落到枚举器路径。参考 `.referenceprojects/Avalonia/src/Avalonia.Base/Collections/AvaloniaList.cs:343-427`。
- Avalonia `AvaloniaList.RemoveAll` 会先把传入 enumerable 建成 `HashSet<T>`，再扫描目标集合。参考 `.referenceprojects/Avalonia/src/Avalonia.Base/Collections/AvaloniaList.cs:530-550`。镜像集合 remove 已有 `OldStartingIndex` 和 `OldItems.Count`，按 range 删除更直接，也避免重复元素时的过删风险。
- `CardTabsContent` 的 move 分支原先把 `OldStartingIndex` 当作 `OldItems` 的起始下标使用，`oldIndex > OldItems.Count` 时不会收集/删除任何移动项。

## Changes

- 新增共享 `ControlCollectionChangedUtils`，add 路径先用 `IList.Count/indexer` 收集 `Control` 列表，再交给 `InsertRange` 的 `IList` 快路径。
- `Card` / `CardActionPanel` remove 路径改为 `RemoveRange(OldStartingIndex, OldItems.Count)`。
- `CardTabsContent` add/remove 去掉 `OfType<Control>()`；move 分支改为按 `OldItems.Count` 删除并按 Avalonia `MoveRange` 的新索引语义插回。

## Benefit

这轮是结构性收益：Card 动态 actions / tabs 同步时少走 LINQ 类型过滤和 HashSet remove 扫描；同时修复 tabs 批量 move 的边界错误。不声明本轮页面加载耗时提升。

| Metric | Baseline | Optimized | Formula | Improvement | Conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Card collection sync `OfType<Control>` callsites | 6 | 0 | `(6 - 0) / 6` | 100.00% | 有效；actions、action panel、tabs add/remove 均去掉 LINQ 类型过滤 |
| Mirrored action remove target scans | 2 scans | 0 scans | `(2 - 0) / 2` | 100.00% | 有效；两层 action mirror 均按 `OldStartingIndex` 删除 |
| CardTabsContent remove value lookups | N lookups | 0 lookups | `(N - 0) / N` | 100.00% | 有效；tabs remove 改为固定下标 `RemoveAt` |
| CardTabsContent move stale branch risk | 1 risky branch | 0 risky branches | `(1 - 0) / 1` | 100.00% | 正确性修复；按 `OldItems.Count` 处理移动项 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` passed.
- `--verify-card-states` 当前仍受旧动态模板期望断言影响，不能作为本轮集合同步路径的通过证明。
