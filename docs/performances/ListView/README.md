# ListView Performance Notes

## Select-All Hotkey Match

`ListView.OnKeyDown()` 多选模式下原先用 `hotkeys.SelectAll.Any(x => x.Matches(e))` 判断全选快捷键。本轮改为显式遍历，避免键盘事件路径创建 LINQ predicate/iterator。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| ListView select-all hotkey LINQ operators / keydown | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；快捷键匹配改为显式遍历 |
| Hotkey matching behavior | first matching gesture wins | first matching gesture wins | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` passed.

## Selection Snapshot Copy

`ListView` 在 selection model 变更、替换 selection model、抛 `SelectionChangedEventArgs` 时需要保留数组快照语义。旧路径直接调用 `SelectedItems.ToArray()` / `DeselectedItems.ToArray()`；本轮改为 `IReadOnlyList<object?>.Count/indexer` 拷贝，并在空集合时返回 `Array.Empty`。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| ListView selection snapshot `ToArray()` callsites | 6 | 0 | `(6 - 0) / 6` | 100.00% | 有效；选择变更快照不再走 LINQ extension |
| Empty selection snapshot arrays / event | 1 new empty array risk | shared empty array | structural | 更稳定 | 空选择复用 `Array.Empty` |
| SelectionChanged snapshot behavior | array snapshot | array snapshot | n/a | 0.00% | 行为保持 |

说明：这是选择事件路径的结构性收益；未新增 Gallery timing 对比，不声明页面加载速度提升。
