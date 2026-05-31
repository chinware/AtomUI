# MessageBox Performance Notes

## Scope

本页记录 MessageBox 后续低风险结构性补充。主线 Dialog / MessageBox 优化记录见 [Dialog](../Dialog/README.md)。

## CustomButtons Sync

`MessageBox.HandleCustomButtonsChanged()` 原先用 `OfType<DialogButton>()` 分别处理 add/remove，再同步到内部 `Dialog.CustomButtons`。本轮改为复用 `DialogButtonCollectionUtils`，按 `NotifyCollectionChangedEventArgs.NewItems/OldItems` 的 `Count/indexer` 显式过滤 `DialogButton`。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| MessageBox CustomButtons sync `OfType<DialogButton>` callsites | 2 | 0 | `(2 - 0) / 2` | 100.00% | 有效；add/remove 均去掉 LINQ 类型过滤 |
| CustomButtons sync helper implementations | 2 paths | 1 shared helper | `(2 - 1) / 2` | 50.00% | 有效；与 Dialog host propagation 复用同一 helper |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## Verification

- `dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj` passed.
