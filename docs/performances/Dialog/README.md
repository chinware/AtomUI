# Dialog 性能优化

> 状态：已完成主线优化；本页记录后续低风险结构性补充。

---

## 1. 追加结构优化：DialogButtonBox 自定义按钮同步去 LINQ

`DialogButtonBox.HandleCustomButtonsChanged()` 在自定义按钮增删时只需要处理 `DialogButton`。旧实现通过 `e.NewItems!.OfType<DialogButton>()` / `e.OldItems!.OfType<DialogButton>()` 再逐个同步到 button group；本轮改为直接遍历 collection changed items 并显式类型判断。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| CustomButtons add filter LINQ operators / add change | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 结构收益；新增自定义按钮同步不再创建 `OfType` iterator |
| CustomButtons remove filter LINQ operators / remove change | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 结构收益；移除自定义按钮同步不再创建 `OfType` iterator |

说明：这是 Dialog runtime 自定义按钮同步路径的结构性收益；未新增 Gallery timing 对比，不声明页面加载速度提升。

## 2. 追加结构优化：Dialog 自定义按钮跨 Host 传播去 LINQ

`Dialog` 打开后，自定义按钮变更会继续同步到 `DialogHost` / `DialogWindowContent` / `OverlayDialogHost` 的内部按钮集合。旧实现四个 host propagation handler 都通过 `OfType<DialogButton>()` 过滤 `NotifyCollectionChangedEventArgs.NewItems/OldItems`，再交给 `AvaloniaList.AddRange()` / `RemoveAll()`。本轮抽出 `DialogButtonCollectionUtils`，按 `IList.Count/indexer` 收集 `DialogButton` 后复用原批量 API，保留 AddRange/RemoveAll 的批量通知语义。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Dialog host CustomButtons add/remove LINQ callsites | 8 `OfType` callsites | 0 `OfType` callsites | `(8 - 0) / 8` | 100.00% | 有效；四个 propagation handler 的 add/remove 路径都不再创建 LINQ 过滤 iterator |
| Dialog host CustomButtons add notification temp lists / add propagation handler | 1 internal notification list | 0 internal notification lists | `(1 - 0) / 1` | 100.00% | 结构收益；先收集为 `List<DialogButton>` 后传给 `AvaloniaList.AddRange()`，避免 AddRange 再建通知列表 |
| Dialog host CustomButtons remove HashSet source count | unknown enumerable count | known `List.Count` | structural | 更稳定 | `RemoveAll()` 构造 HashSet 时可按已收集按钮数预设容量 |

说明：这是自定义按钮运行时增删路径的结构性收益；未新增 Gallery timing 对比，不声明页面加载速度提升。

## 3. 追加结构优化：DialogButtonBox 自定义按钮和标准按钮同步小对象收敛

`DialogButtonBox.HandleCustomButtonsChanged()` 的 `NewItems/OldItems` 来自 collection changed args，类型是 `IList`，本轮从 `foreach` 改为 Count/indexer 遍历，避免自定义按钮增删同步时创建 collection enumerator。`AddButtonToGroup()` / `RemoveButtonFromGroup()` 也从 `ContainsKey()` + indexer 改为 `TryGetValue()`，每个按钮分组同步少一次字典查找；标准按钮列表按 `DialogStandardButtons.Count` 预分配。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| DialogButtonBox CustomButtons add collection enumerators / add change | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；按 `IList.Count/indexer` 遍历新增按钮 |
| DialogButtonBox CustomButtons remove collection enumerators / remove change | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；按 `IList.Count/indexer` 遍历移除按钮 |
| DialogButtonBox button-role dictionary lookups / button group update | 2 lookups | 1 lookup | `(2 - 1) / 2` | 50.00% | 有效；`TryGetValue()` 保留原分组语义 |
| DialogButtonBox standard buttons list growth / AllStandard build | dynamic growth | exact `StandardButtons.Count` capacity | structural | 更稳定 | 结构收益；标准按钮列表一次按 flag 数预分配 |

说明：状态验证补充覆盖 custom button add/remove；未新增 Gallery timing 对比，不声明页面加载速度提升。

## 4. 追加结构优化：MessageBox 自定义按钮同步复用 Dialog helper

`MessageBox.HandleCustomButtonsChanged()` 原先还有一组独立的 `OfType<DialogButton>()` add/remove 路径，用于把 `MessageBox.CustomButtons` 同步到内部 `Dialog.CustomButtons`。本轮改为复用 `DialogButtonCollectionUtils`，与 Dialog host propagation 走同一套 Count/indexer 类型过滤 helper。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| MessageBox CustomButtons sync `OfType<DialogButton>` callsites | 2 | 0 | `(2 - 0) / 2` | 100.00% | 有效；add/remove 均不再创建 LINQ 类型过滤 iterator |
| MessageBox CustomButtons sync helper implementations | 2 paths | 1 shared helper | `(2 - 1) / 2` | 50.00% | 有效；复用 Dialog 自定义按钮同步 helper，减少分叉逻辑 |
| Custom button filter semantics | preserved | preserved | n/a | 0.00% | 行为保持；仍只同步 `DialogButton` |

说明：这是 MessageBox 运行时自定义按钮增删路径的结构性收益；未新增 Gallery timing 对比，不声明页面加载速度提升。
