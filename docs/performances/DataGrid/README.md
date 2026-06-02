# DataGrid 性能优化

> 状态：T4.1-T4.5 已完成。最后一轮审计确认 `Panel.Children` 遍历基于 AvaloniaList struct enumerator，不保留无收益的 foreach->indexer 改动；剩余 DataGridColumnCollection/DisplayData/GetSelectionInclusive helper 无当前生产调用热点，仅保留兼容/DEBUG 路径。

---

## 0. 结论

本轮优化有效，但不是大幅减少 DataGrid 主视觉树的优化。收益集中在关闭态过滤列头、列/行生命周期、pagination 重套模板生命周期、row details binding、selection reset hot path 和 data-layer hot paths：

- 无过滤项的列头不再创建空 filter flyout shell。
- 有过滤项的列头只保留轻量 flyout shell，菜单/树过滤项延迟到首次打开前创建。
- filter indicator detach 时释放 flyout shell，并补齐 `CollectionView.FilterDescriptions` 订阅切换/释放。
- filter indicator 可见性不再为每个列头创建 3 路 `MultiBinding` + converter，改为列头 DirectProperty + `{TemplateBinding}`。
- filter popup materialized 后，menu/tree presenter 的 reset / ok 按钮不再注册本地 Click handlers，改为 presenter class handler。
- 默认 `FilterOnClose=false` 时，filter flyout 被动关闭不再递归收集 selected values，也不再创建内部 selected-values 事件参数；确认关闭和 `FilterOnClose=true` 仍会收集并发起过滤。
- filter flyout close 时，有 presenter 的路径不再先创建一个马上被丢弃的空 `List<string>`，直接使用 presenter 收集的 selected values。
- menu/tree filter presenter 确认关闭收集 selected values 时，先按已选中的 leaf 数量预分配 `List<string>`；空选保持 0 容量，单选/全选都避免默认扩容。
- menu/tree filter presenter 空选、reset 后空选和未 materialized fallback 空选复用共享零容量 selected-values list，不再为每次空筛选关闭分配新 `List<string>` 对象。
- `DataGridFilterItem.Children` 改为懒创建，leaf filter item 不再默认持有空 `List<DataGridFilterItem>`；menu/tree materialize 通过 `HasChildren` 判断，不会为了检查 leaf 子项而创建空列表。
- filter indicator 的 tree radio group name 不再在每个列头构造时创建，改为首次 materialize tree filter items 时按需创建。
- menu/tree filter items materialize 时不再递归返回临时 `List`，改为直接填充 `ItemCollection`，保持 menu 5 项 / tree 6 项层级计数。
- column header hover 不再为每个 realized `DataGridColumnHeader` 注册本地 `PointerEntered` / `PointerExited` handler，改走 Avalonia 的 virtual override 路径。
- column header press/release/move 不再为每个 realized `DataGridColumnHeader` 注册本地 handler；内部 resize/reorder/sort 逻辑和 `DataGridColumn.HeaderPointerPressed/Released` 转发都改为 class handler。
- column header 普通点击 / 排序释放不再发送无效 `ColumnDraggingOver(null, null)` cleanup；真正列重排释放仍保留一次 null cleanup。
- `DataGridColumnHeader` 在 frozen-column 裁剪路径不再每次 header presenter arrange 新建 clip `RectangleGeometry`，改为 header 级复用；不需要裁剪时仍清空 `Clip`。
- column group header press/release 不再为每个 realized `DataGridColumnGroupHeader` 注册本地 forwarding lambda，改为 class handler 转发到 `DataGridColumnGroupItem.HeaderPointerPressed/Released`。
- column group tree add/remove 收集 leaf columns 时不再为每个 group/column 节点创建递归临时 `List<DataGridColumn>`，改为一次操作共享同一个收集列表。
- column group tree add/remove 现在递归直接同步 `ColumnsInternal`，不再创建操作级 leaf-column 临时列表；remove 时 top-level 和 nested group item 都释放 `OwningGrid`。
- `DataGridHeaderViewItem` 在 column group leaf frozen-column 裁剪路径不再每次 group header presenter arrange 新建 clip `RectangleGeometry`，改为 header view item 级复用；不需要裁剪时仍清空 `Clip`。
- `DataGridGroupColumnHeadersPresenter` 的 measure / arrange 不再通过 `GetVisibleColumns()` 构造 visible-column iterator，改为按 display index 直接遍历并跳过隐藏列。
- row header press 不再为每个 realized `DataGridRowHeader` 注册本地 `PointerPressed` handler，改为 class handler，同时保留 focus / selection / current slot 行为。
- row group header press 不再为每个 realized `DataGridRowGroupHeader` 注册本地 `PointerPressed` lambda，改为 class handler，同时保留 current slot 行为。
- `DataGridRowGroupHeader` 内置模板的 `RootLayout` / `RowHeader` part 现在能被 code-behind 找到，默认 `HeadersVisibility=Column` 时分组行头内 row header 会正确隐藏，并恢复 owner / clip lifecycle。
- `DataGrid` core `KeyDown` / `KeyUp` / `GotFocus` / `LostFocus` 不再为每个 grid 实例注册本地 routed handlers，改为 Avalonia virtual override 路径。
- `DataGrid` 重套模板时，旧 top/bottom `Pagination` 在替换前释放 `CurrentPageChanged` 订阅；新 pagination 仍保持 exactly one handler，detach 后也释放。
- `DataGridRowsPresenter` 不再为每个 presenter 注册本地 `ScrollGesture` handler，改为 class handler，同时保留滚动手势更新 `VerticalOffset` 的行为。
- `DataGridRowsPresenter` 不再在每次 arrange 时新建 clip `RectangleGeometry`，改为 presenter 级复用并在 `Rect` 变化时显式 invalidation。
- DataGrid row 高度估算、横向滚动后重测、插入/删除 slot 修正、displayed rows reset 和 row reorder hit-test 不再通过 `DisplayData.GetScrollingElements()` / `GetScrollingRows()` 构造 iterator，改为按 display index 直接遍历。
- DataGrid column width adjustment 的 star / non-star 列遍历不再通过 `GetDisplayedColumns(predicate)` 构造 filtered iterator；目标宽度选择也不再用每次调用的 `Func<DataGridColumn, double>` delegate。
- DataGrid star column width adjustment 没有可调 star 列时不再创建空临时列表；有可调 star 列时按剩余 displayed column 数预分配 star 列和排序对列表。
- DataGrid column frozen-state 修正和 header pseudo-class 刷新不再通过 displayed / visible column iterator 和 `ToList()` 临时集合，改为 display index 直接遍历。
- DataGrid 水平列坐标和列滚动 offset 计算不再通过 visible/frozen/scrolling column iterator，改为 display index 直接遍历。
- DataGrid 自动列宽完成、ItemsSource header 初始化、列宽属性变更、编辑元素生成、star width coerce、列 resize、剪贴板复制和 column header hit-test 不再通过 visible/displayed column iterator，改为 display index 直接遍历。
- DataGrid 剪贴板复制 header row 和每个 selected row 时，`ClipboardRowContent` list 按当前 visible column count 预分配，避免逐个追加 visible cells 时触发 backing array 扩容。
- DataGrid 剪贴板文本格式化直接追加到总 `StringBuilder`，不再为每个 copied row 创建临时 row `StringBuilder` / row string，也不再为每个 copied cell 创建插值字符串。
- DataGrid 自动生成列收集排序对时按 `DataProperties.Length` 预分配 `columnOrderPairs`，并在一次 pass 内复用同一个属性数组。
- DataGrid 普通列头套模板时不再复制 `ColumnsItemsInternal` 并排序，直接按 `ColumnsInternal.DisplayIndexMap` 插入 header；乱序 `DisplayIndex` 的列头顺序已验证。
- `DataGridRow` 底部分割线不再在每次 arrange 时新建 clip `RectangleGeometry`，改为 row 级复用并在模板重套 / reset 时释放引用。
- `DataGridRow` 非回收隐藏路径不再每次设置隐藏态时新建空 `RectangleGeometry`，改为 row 级复用；恢复显示时清空 `Clip`。
- `DataGridRowGroupHeader` 的非冻结子控件裁剪路径不再每次 arrange 新建 clip `RectangleGeometry`，改为按 child 复用；冻结分组头、重套模板和 detach 都清空旧 child clip。
- `DataGridRowGroupHeader` 的冻结子控件水平偏移不再每次 arrange 新建 `TranslateTransform`，改为按 child 复用；冻结分组头、重套模板和 detach 都清空旧 child transform。
- `DataGridDetailsPresenter` 不再在 non-frozen row details 每次 arrange 时新建 clip `RectangleGeometry`，改为 presenter 级复用；frozen row details 仍清空 `Clip`。
- `DataGridCell` 在 frozen-column 裁剪路径不再每次 cells presenter arrange 新建 clip `RectangleGeometry`，改为 cell 级复用；不需要裁剪时仍清空 `Clip`。
- `DataGridCell` 跟随 column header 的 sort / reorder 状态绑定不再通过 `BindUtils.RelayBind` 创建每 cell 捕获 lambda，改为直接 observable 绑定 + 静态 converter delegate。
- `Columns.Clear()` 现在和单列 remove 一样走 column about-to-detach，释放 special columns 缓存的 grid 事件订阅。
- column reorder dragging-over indicator 不再在每次 `Render()` 时创建 dashed `Pen`，改为按 foreground 缓存。
- column reorder drag / drop-location indicator 不再在每次 clipped arrange 时创建新的 `RectangleGeometry`，改为 presenter 级缓存；替换 indicator 时清空旧控件 `Clip`。
- column reorder 拖拽中目标列已经为 `null` 后，连续 pointer move 不再重复创建 `DataGridColumnDraggingOverEventArgs` / 重复通知，只在 target 从非 null 变为 null 时通知一次。
- `DataGridColumnHeader` 在 resize/reorder 拖拽中被移除时，会按 owner 释放 static drag state、presenter drag state 和 child removal drag indicator；TopLevel detach 只清状态、不改动别的 parent 的 visual children。
- `DataGridRowReorderColumn` 的重复列检查不再通过 `Columns.Count(predicate)` 构造 LINQ predicate/enumerator，改为 indexed loop，并在发现第二个 reorder column 时提前退出。
- `DataGridRowReorderHandle` 现在只有真正进入拖拽后才执行 drop / `RowReordered` / rows presenter arrange invalidation；单击 row reorder handle 不再触发无效重排事件。
- `DataGridRowReorderHandle` 在真实 drop 后清空 rows presenter 的 dragged row state；row unload 会按 owner 释放 ghost row，TopLevel detach 只清 static state，避免 detach 遍历期间改动 rows presenter visual children。
- `SelectionChangedEventArgs` 的 added / removed item list 现在按选择数量变化差值预分配，SelectAll / ClearSelection 这类批量选择不再从 0 容量逐步扩容。
- selected-items index rebuild 现在按旧选中项数量预分配 surviving selected-items cache，ItemsSource reset / data refresh 后保留选中项时不再从 0 容量逐步扩容。
- 默认空选择状态的 `InitializeElements()` reset 不再复制空 selected-items cache；`UpdateIndexes()` 对默认空 cache 不再替换成新的空 list，但对清空后仍持有 backing array 的 cache 保留释放容量路径；非空 selection reset 仍保留原选择。
- selected-items 内部遍历现在可走 `IndexToValueTable` struct index enumerator，`SelectionChanged` diff 和 `SelectedItems` 枚举不再额外创建 selected-slot yield iterator。
- `SelectedItems` 公共枚举不再由 C# `yield return` 生成外层状态机，改为显式枚举器；`IList.GetEnumerator()` 仍需要返回 1 个 `IEnumerator` 对象，收益只限于移除 compiler-generated iterator state machine。
- `GetSelectionInclusive()` 的起始 slot 枚举现在复用 `IndexToValueTable` struct index enumerator，不再为 range selection 内部 slot 遍历创建 `GetIndexes(start)` yield iterator。
- selected-slots table copy 现在按 range count 预分配内部 range list，`SelectionChanged` 保存旧 slots 快照时不再从 0 容量扩到 4。
- 单选模式替换唯一选中行时不再用 `_selectedItems.GetIndexes().First()` 创建 iterator / LINQ 调用，改为直接读取第一个 selected slot。
- `DataGridDetailsPresenter.ContentHeight` 的 `AffectsMeasure` 注册从实例构造函数移到静态构造函数，避免每个 details presenter 重复添加 class-level property observer。
- `DataGridDataConnection.TryGetCount()` 对非 `ICollection` 的 `IEnumerable` fallback 不再通过 `Cast<object>().Count()/Any()` 构造 LINQ iterator，改为直接使用 raw `IEnumerator`。
- `DataGridDataConnection.TryGetCount()` 对 `IReadOnlyCollection<object>` 数据源直接读取 `Count`，不再退回 enumerable count/getAny 探测。
- `DataGridDataConnection.GetDataItem(index)` 对 `IReadOnlyList<object>` 数据源直接用 `Count` + indexer 取项，不再从头枚举到目标行。
- `DataGridDataConnection.IndexOf(item)` 对 `IReadOnlyList<object>` 数据源直接用 `Count` + indexer 扫描，不再创建 enumerable enumerator。
- `DataGridDataConnection.HandleDataSourceCollectionChanged()` 处理 Remove old-items 时改用 `Count + indexer`，不再为批量 removed items 创建 enumerator。
- `TypeHelper.GetItemType()` 对裸 `IEnumerable` 探测代表项类型后会释放枚举器，避免 DataGrid `DataConnection.DataType` 读取这类 ItemsSource 时留下未释放 enumerator。
- `TypeHelper.GetItemType()` 对 item type 只能推断为 `object` 的 `IReadOnlyList<object>` 数据源直接用 `Count + indexer` 获取第一个非空代表项，不再创建 enumerator。
- `TypeHelper.GetItemType()` 对 item type 只能推断为 `object` 的非泛型 `IList` 数据源直接用 `Count + indexer` 获取第一个非空代表项，不再创建 enumerator。
- `DataGridDataConnection.Any()` 在 `DataSource` 已是 `IDataGridCollectionView` 时直接读取 `IsEmpty`，不再为非 `ICollection` collection view 构造 enumerator / 调 `MoveNext()`。
- `DataGridDataConnection.DataProperties` 对当前数据类型的 `PropertyInfo[]` 做缓存，自动生成列等重复读取不再反复 `GetProperties(...)` 分配数组；DataSource 替换和 `ClearDataProperties()` 会刷新缓存。
- `DataGridDataConnection.GetPropertyIsReadOnly()` 在属性没有 `EditableAttribute` 的常见路径上不再调用 `GetCustomAttributes()` 创建空 attribute array；带 `Editable(false/true)` 的路径保持原语义。
- `TypeHelper.GetIsReadOnly()` 在 member 没有 `ReadOnlyAttribute` 的常见路径上不再调用 `GetCustomAttributes()` 创建空 attribute array；DataGrid 编辑只读判断里的 property type / property info 检查都受益，`ReadOnly(true/false)` 和 type-level 语义已验证。
- `TypeHelper.GetDisplayName()` 和 DataGrid 自动生成列的 `DisplayAttribute` 读取在无 attribute 常见路径上不再调用 `GetCustomAttributes()` 创建空 attribute array；`ShortName`、`Name` fallback、`Order` 和 `AutoGenerateField=false` 语义已验证。
- `TypeHelper.SplitPropertyPath()` 对无 nested / indexer / parenthesis 的简单属性路径直接返回容量为 1 的 segment list，不再把单段路径扩到默认 4 个槽；复杂路径保留原解析逻辑。
- `DataGrid.GetAllRows()` 不再通过 `yield return` 为每次已实现行遍历创建 iterator 对象，改为 value-type enumerable/enumerator；列状态刷新、selection column 状态刷新和 row data-context 通知等 realized-row 遍历路径受益。
- RowGroupHeadersTable 的分组 slot 遍历不再通过 `GetIndexes()` / `GetIndexes(start)` 创建 yield iterator，改走已有 `EnumerateIndexes()` value-type enumerable；分组行初始化、插入/删除修正、展开/折叠和 parent group 查找受益。
- `DataGridFilterDescription` 对同一 item type 的 `PropertyPath` 属性类型解析结果做缓存，`PropertyPath` 变更时清空缓存并重新解析；同一 record 的多条件过滤只取一次属性值并只执行一次 `ToString()`。
- `DataGridFilterDescription.FilterConditions` 的默认空 list 改为懒创建；无条件过滤和 object initializer 覆盖条件时，不再先分配一个马上被丢弃的空列表。
- `DataGridColumn.GetSortDescription()` / `GetFilterDescription()` 不再通过 `OfType()` / `FirstOrDefault(predicate)` 构造 LINQ iterator / predicate delegate，改为 indexed loop 并保持 first-match 语义。
- 清空过滤请求不再把空 `List<string>` 再复制成新的空 `List<object>`，改为复用私有零容量空列表；非空过滤请求仍保持独立快照。
- `DataGridColumn` 移除已失效的 `_clipboardContentBinding` 私有字段，`ClipboardContentBinding` 仍保持 auto property；`DataGridBoundColumn` 的 Binding fallback / explicit override / clear fallback 行为已验证。
- `DataGridCollectionView.PrepareLocalArray()` 无 Filter 的 refresh 路径不再对每个 item 重复执行 filter 分支判断，直接复制 source item；有 Filter 的路径仍保持原过滤语义。
- `DataGridCollectionView` 对 `IReadOnlyCollection<object>` 源集合复用 `Count` 做 source copy 预分配和 Reset 空检查，不再退回默认 list 扩容或 empty-check enumerator。
- `DataGridCollectionView` 对 `IReadOnlyList<object>` 源集合用 `Count + indexer` 复制，构造、Refresh 和无过滤 local-array refresh 不再创建 source-copy enumerator。
- `DataGridCollectionView` 对带 Filter 的 `IReadOnlyList<object>` 源集合用 `Count + indexer` 扫描，避免 source enumerator，同时 filtered result 不按全量源 Count 预分配。
- `DataGridCollectionView` 对非泛型 `IList` 源集合用 `Count + indexer` 复制和过滤扫描，构造、Refresh、无过滤 local-array refresh 和 filtered refresh 都不再创建 source enumerator。
- `DataGridCollectionView.ProcessCollectionChanged()` 对 remove/replace 的 `OldItems` 改用 Count/indexer 处理，不再为批量 removed items 创建 enumerator。
- `DataGridCollectionView.AdjustCurrencyForEdit()` 复用第一次 `IndexOf(newCurrentItem)` 结果，不再为同一个 edited current item 做第二次线性查找。
- `DataGridCollectionView` 对 `INotifyCollectionChanged` 源集合不再创建未使用的 polling tracking enumerator；非通知源保留 polling 语义并在 `Dispose()` 释放 tracking enumerator。
- `ValidationUtils.ContainsMemberName()` 对 validation member names 的 read-only list 走 Count/indexer 快路径，空列表目标也只读 Count，不再创建枚举器。
- `ValidationUtils.FindEqualValidationResult()` 比较 validation member names 时，对 read-only list 走 Count/indexer 快路径，不同 Count 直接失败，fallback 枚举器会被释放。
- `ValidationUtils.FindEqualValidationResult()` 对 read-only validation result list 走 Count/indexer 快路径，命中 / 未命中都不再创建 validation-result list enumerator。
- `DataGridCollectionView.SortList()` 仍保留原 LINQ stable sort chain，但最终 sorted result 不再用 `seq.ToList()`，改为按输入 count 预分配并手写 materialize。
- `DataGridCollectionView.GetEnumerator()` 的分页路径现在直接用页范围枚举器读取 `InternalList`，不再为当前页创建临时 `List<object?>` 或复制页内 item。
- `DataGridCollectionView.GetEnumerator()` 在 `PageSize > 0 && PageIndex < 0` 的空分页状态下复用静态空数组枚举器，不再为返回空结果创建临时 `List<object?>`。
- `DataGridCollectionView` 分组插入现在按 `GroupBy.KeysMatch(subgroup.Key, key)` 匹配已有 subgroup，非连续重复 key 不再被错误插入第一个 subgroup。
- `CollectionViewGroupRoot` 对 multi-key grouping 的 `IList` key 集合走 Count/indexer，新增 subgroup 和 remove subgroup 都不再创建 key-list enumerator。
- `DataGridPathGroupDescription` 现在按 owner type 缓存 property type，混合 item type 使用同名但不同类型属性时不再复用过期 property type。
- `DataGridCollectionView` 的已知 `PropertyChanged` 属性名现在复用静态 `PropertyChangedEventArgs`，热路径通知不再每次分配 event args；未知属性名仍保持原来的按次创建行为。
- `DataGridCollectionView` 的无 payload `Reset` collection changed 通知现在复用静态 `NotifyCollectionChangedEventArgs`，刷新 / 分页 / clear 路径不再为纯 Reset 每次分配 event args。
- `DataGridCollectionView` 处理 `ICollection` source 的 Reset 时，空集合判断直接读取 `Count`，不再为了 `MoveNext()` 额外创建一次枚举器。
- `DataGridCollectionViewGroup` / `DataGridCollectionViewGroupInternal` / `DataGridGroupDescription` 的固定 `PropertyChanged` 通知现在复用静态 event args，分组 item count、bottom-level 状态和 group keys 变更不再每次创建事件参数。
- `DataGridSortDescription.FromPath()` 的属性 comparer 不再在每次 `Compare()` 时重新解析；已知属性类型后缓存 comparer，并保持自定义 path value comparer 在 `MergedComparer` 路径中不被默认 comparer 覆盖。
- `DataGridSortDescription` comparer sort 的 identity key selector 和 path sort 的 `GetValue` key selector 现在复用 cached delegate，不再每次 `OrderBy()` / `ThenBy()` 创建短命委托。
- `DataGridRowExpander` 不再为每个 realized row expander 创建 `Binding + Path` 的 `BindUtils.RelayBind` 双向绑定，改为 direct observable binding + checked-state writeback，并在 visual detach 时兜底释放 row 引用。
- `DataGridOperationButtons` 不再为 edit / save / delete / cancel 四个模板 part 分别注册本地 routed handlers，改为 owner class handler，同时避免重套模板重复订阅。
- `DataGridCheckBoxColumn` 的 pointer-triggered edit 不再用 `LayoutUpdated` 等待首个非零 bounds，改为一次性 `BoundsProperty` 观察并在 dispatcher 下一轮处理 click hit-test，保留原来的布局后 toggle 语义。
- 运行时增删 `DataGridColumn.Filters` 会同步刷新 filter indicator 可见性和 flyout shell。
- 修复 `DataGridColumnHeader.IsMiddleVisible` 错误 raise 到 `IsLastVisibleProperty` 的正确性问题。
- 修复分组行头模板中 `DataGridRowHeader` 可能先于 `Owner` 设置完成 `OnApplyTemplate()` 导致空引用的问题。
- 修复分组行头内置模板 part 名称与 code-behind 查找不一致导致 row header visibility / owner / child clip 逻辑不生效的问题。

Avalonia 依据：

- `PopupFlyoutBase.ShowAtCore` 只有在 `Popup.Child == null` 时才调用 `CreatePresenter()`：`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/PopupFlyoutBase.cs:292`。
- `MenuFlyout.CreatePresenter()` 把 `Items` 交给 presenter：`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/MenuFlyout.cs:85-92`。
- `ItemsControl.Items` 变更会进入 logical children / item count 路径：`.referenceprojects/Avalonia/src/Avalonia.Controls/ItemsControl.cs:639-656`。
- `TemplateBinding` 直接订阅 templated parent 的 `PropertyChanged`：`.referenceprojects/Avalonia/src/Avalonia.Base/Data/TemplateBindingExpression.cs:37-43`。
- `MultiBindingExpression` 在子 observable 变更后进入 converter 路径：`.referenceprojects/Avalonia/src/Avalonia.Base/Data/Core/MultiBindingExpression.cs:23-49`、`:86-98`。
- `PointerEntered` / `PointerExited` 是 `Direct` routed event：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:144-155`。
- Avalonia 已通过 class handler 调用 `OnPointerEnteredCore` / `OnPointerExitedCore`，再进入 virtual `OnPointerEntered` / `OnPointerExited`：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:242-243`、`:1019-1035`。
- `PointerMoved` / `PointerPressed` / `PointerReleased` 是 tunnel + bubble routed events：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:158-179`。
- 事件属性 `PointerXxx +=` 会走 `AddHandler`，本地订阅进入 `_eventHandlers` 字典；class handler 通过 `RoutedEvent.Raised` 按类型订阅一次：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:353-389`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/Interactive.cs:176-190`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/RoutedEvent.cs:84-99`。
- Avalonia 已通过 `InputElement` class handlers 把 `GotFocus` / `LostFocus` / `KeyDown` / `KeyUp` 分发到 virtual `OnGotFocus` / `OnLostFocus` / `OnKeyDown` / `OnKeyUp`：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:235-240`、`:596-657`。
- `KeyDown` / `KeyUp` 是 tunnel + bubble routed events，事件属性 `+=` 默认只添加 direct/bubble 本地 handler；`AddClassHandler` 默认也覆盖 direct/bubble route，因此 override 路径与原实例订阅的 bubble 处理口径一致：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:106-118`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/Interactive.cs:30-64`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/RoutedEvent.cs:78-99`。
- `ScrollGestureEvent` 是 bubble routed event，事件属性 `+=` 同样走 `AddHandler` 进入本地 `_eventHandlers`；`AddClassHandler` 默认覆盖 direct/bubble route，因此 rows presenter 的 class handler 与原本地 handler 路由口径一致：`.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.Gestures.cs:65-69`、`:197-200`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/Interactive.cs:30-64`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/RoutedEvent.cs:78-99`。
- `Visual.Clip` 是 `StyledProperty<Geometry?>`，且 `Visual` static constructor 把 `ClipProperty` 注册进 `AffectsRender`；`InvalidateVisual()` 会把 visual 加入 renderer dirty 集合：`.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:52-53`、`:141-149`、`:418-421`。
- `RectangleGeometry.RectProperty` 通过 `AffectsGeometry` 标记 geometry dirty；复用同一个 geometry 时本轮在 `Rect` 变化后显式调用 `InvalidateVisual()`，不依赖 `Clip` 换引用来触发重绘：`.referenceprojects/Avalonia/src/Avalonia.Base/Media/RectangleGeometry.cs:25-33`、`.referenceprojects/Avalonia/src/Avalonia.Base/Media/Geometry.cs:146-172`。
- `Visual.RenderTransform` 是 styled property；Avalonia 在 `RenderTransformProperty.Changed` 时订阅 mutable transform 的 `Changed` 并 invalidate visual，`TranslateTransform.X/Y` 变化会 `RaiseChanged()`：`.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:92-100`、`:150`、`:644-710`、`.referenceprojects/Avalonia/src/Avalonia.Base/Media/TranslateTransform.cs:14-22`、`:70-75`、`.referenceprojects/Avalonia/src/Avalonia.Base/Media/Transform.cs:23-50`。
- `PressedMixin.Attach<T>` 本身通过 class handler 维护 `:pressed`，本轮 column header class handler 注册在 `PressedMixin.Attach<DataGridColumnHeader>()` 之后，避免影响 pressed 状态：`.referenceprojects/Avalonia/src/Avalonia.Controls/Mixins/PressedMixin.cs:14-33`。
- `AvaloniaObjectExtensions.Bind(IObservable<T>)` 对 DirectProperty 进入 `DirectBindingObserver<T>` 路径：`.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaObjectExtensions.cs:188-225`、`.referenceprojects/Avalonia/src/Avalonia.Base/PropertyStore/DirectBindingObserver.cs:7-84`。
- `Pen` 是 mutable `AvaloniaObject`，`Brush` / `Thickness` / `DashStyle` 都是 styled properties：`.referenceprojects/Avalonia/src/Avalonia.Base/Media/Pen.cs:17-40`。
- `Visual.AffectsRender` 会在注册属性变更时 invalidate render：`.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:446-500`。
- `AffectsMeasure<T>` 注释要求在控件 static constructor 中调用，且实现会对 `property.Changed` 订阅 observer：`.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:502-512`。
- `AvaloniaObjectExtensions.GetObservable` 为属性提供直接 observable；`AvaloniaObject.Bind(StyledProperty<T>, IObservable<T>)` 直接走 value store binding，非 `BindingBase` 时不创建 path binding expression：`.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaObjectExtensions.cs:59-77`、`.referenceprojects/Avalonia/src/Avalonia.Base/AvaloniaObject.cs:511-522`。
- `BindUtils.RelayBind` 会创建 `Binding`、设置 `Source` 和 `Path`，再调用 `target.Bind(...)`：`src/AtomUI.Core/Data/BindUtils.cs:8-36`。
- `HyperLinkTextBlock.ClickEvent` 和 `PopupConfirm.ConfirmedEvent` 都是 Bubble routed event，可由 owner class handler 接收：`src/AtomUI.Desktop.Controls/TextBlock/HyperLinkTextBlock.cs:125-128`、`src/AtomUI.Desktop.Controls/PopupConfirm/PopupConfirm.cs:52-53`。
- Avalonia `Button.ClickEvent` 是 Bubble routed event；`RoutedEvent.AddClassHandler<TTarget>()` 默认覆盖 Direct/Bubble route：`.referenceprojects/Avalonia/src/Avalonia.Controls/Button.cs:82-84`、`.referenceprojects/Avalonia/src/Avalonia.Base/Interactivity/RoutedEvent.cs:121-134`。
- `Visual.Bounds` 是 DirectProperty，`Bounds` setter 通过 `SetAndRaise(BoundsProperty, ...)` 通知观察者：`.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:40-42`、`:182-185`。
- `Layoutable.LayoutUpdated` 在首个订阅时转接到 root `LayoutManager.LayoutUpdated`，layout pass 结束后由 `LayoutManager` 统一广播：`.referenceprojects/Avalonia/src/Avalonia.Base/Layout/Layoutable.cs:195-213`、`.referenceprojects/Avalonia/src/Avalonia.Base/Layout/LayoutManager.cs:177-178`。
- `TemplatedControl.ApplyTemplate()` 在新模板 build 前会清空旧 template descendants，随后对新模板调用 `OnApplyTemplate()`：`.referenceprojects/Avalonia/src/Avalonia.Controls/Primitives/TemplatedControl.cs:306-341`。
- `AbstractPagination.CurrentPageChanged` 是普通事件字段；替换 template part 前需要显式从旧 part 退订：`src/AtomUI.Desktop.Controls/Pagination/AbstractPagination.cs:95`。
- `Visual.OnDetachedFromVisualTreeCore()` 会先调用当前 visual 的 detach，再按预先缓存的 `VisualChildren.Count` 遍历子节点；因此 TopLevel detach 过程中不能从别的 parent 的 `VisualChildren` 移除 ghost row：`.referenceprojects/Avalonia/src/Avalonia.Base/Visual.cs:581-611`。
- `VisualTreeAttachmentEventArgs.AttachmentPoint == null` 表示整棵树 attach/detach 到 `PresentationSource`，非 null 表示从某个 parent attach/detach；本轮用它区分单 header 移除和 TopLevel detach：`.referenceprojects/Avalonia/src/Avalonia.Base/VisualTreeAttachmentEventArgs.cs:16-30`。

因此，过滤菜单项属于 popup-open-only 内容，延迟到 `FlyoutAboutToShow` 前创建符合 Popup Lazy Content Rule。

---

## 1. 资格门槛

Gallery source：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml`

| 项 | 数值 |
| --- | ---: |
| `<atom:DataGrid>` | 65 |
| DataGrid column declarations | 210 |
| Columns with filter blocks | 6 |
| `DataGridFilterItem` declarations | 21 |

真实会话操作频率：

- 页面导航：至少 1 次。
- 过滤按钮打开：只有用户操作过滤列时发生，默认关闭态。

结论：DataGrid 页面实例量和列数量都明显超过 SKILL Tier 1 §13 门槛；关闭态过滤内容值得优化。

---

## 2. 收益表

### 2.1 结构收益

| 指标 | baseline | optimized | 改善 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Plain 3-column grid filter flyout shells | 4 | 0 | 100.00% removed | 有效；3 列 + top-left header 都不再创建空 shell |
| Filter 3-column grid closed flyout shells | 4 | 2 | 50.00% removed | 有效；只有 2 个带 filter items 的列保留 shell |
| Closed filter flyout materialized items | 7 | 0 | 100.00% deferred | 有效；首次打开前不创建菜单项 |
| Unopened sibling filter flyout materialized items | n/a | 0 | preserved lazy state | 有效；打开一个 filter 不牵连其他 filter |
| Detached filter indicator flyout shells | retained risk | 0 | release path added | 正确性/泄漏风险修复 |
| Filter indicator visibility bindings per header | 3 bindings + MultiBinding converter | 1 TemplateBinding | binding graph reduced | 有效；DataGridShowCase 210 个列声明受益 |
| Runtime `Filters` collection visibility refresh | binding-path dependent | explicit `CollectionChanged` refresh | correctness path added | 正确性修复；add/clear filter items 均验证 |
| Filter flyout presenter local button handlers | 2 Click handlers per materialized menu/tree presenter | 0 local handlers | 100.00% removed | 结构/生命周期优化；reset / ok 统一 presenter class handler |
| Filter selected-values list capacity for 1 checked nested value | 4 capacity after first add | 1 capacity preallocated | 75.00% capacity reduction | 结构/分配优化；按 selected leaf count 一次到位 |
| Filter selected-values list capacity for 6 checked nested values | 8 capacity after growth | 6 capacity preallocated | 25.00% capacity reduction | 结构/分配优化；menu/tree 嵌套 filter close 均验证 |
| Filter selected-values list Add-time growth for 6 checked nested values | 2 backing-array growths | 0 Add-time growths | 100.00% removed | 结构/分配优化；收集 selected values 时不再边 add 边扩容 |
| Empty filter selected-values list object allocations / empty close | 1 `List<string>` | 0 new lists | 100.00% removed | 结构/分配优化；menu/tree 空选、reset 空选和未 materialized fallback 都复用共享空列表 |
| DataGrid.Filter.*.Closed filter item child lists / grid | 7 lists | 1 list | 85.71% fewer lists | 结构/分配优化；只有真正有 child filters 的 item 创建 Children list |
| DataGrid.Filter.*.Closed leaf filter empty child lists / grid | 6 empty lists | 0 empty lists | 100.00% removed | 结构/分配优化；leaf filter items 保持懒创建 |
| DataGrid.GalleryShape leaf filter empty child lists | 12 empty lists | 0 empty lists | 100.00% removed | 结构/分配优化；GalleryShape 里 menu/tree 两个 filter grids 共 12 个 leaf filters |
| Filter indicator tree radio group name strings / closed indicator | 1 string | 0 strings | 100.00% deferred | 结构/分配优化；只有首次 materialize tree filter items 时创建 |
| DataGrid.GalleryShape closed tree group name strings | 22 strings | 0 strings | 100.00% deferred | 结构/分配优化；按当前 performance GalleryShape 的 header indicator 数估算 |
| Nested 5-item filter materialization temp item lists / first open | 2 lists | 0 lists | 100.00% removed | 结构/分配优化；menu/tree 递归直接填充 `ItemCollection` |
| Filter flyout item hierarchy | menu 5 / tree 6 items | menu 5 / tree 6 items | behavior preserved | 正确性保持；状态验证覆盖 exact count 和 tree group name |
| Column header local hover routed handlers | 2 per realized header | 0 per realized header | 100.00% removed | 结构/分配优化；hover 语义保留在 `OnPointerEntered` / `OnPointerExited` override |
| Column header local press/release/move handlers | 5 per standard header | 0 per standard header | 100.00% removed | 结构/分配优化；internal handler + public column event forwarding 均迁到 class handler |
| Column header click drag-over cleanup events | 1 `ColumnDraggingOver(null, null)` per no-drag click release | 0 events | 100.00% removed | 交互路径优化；普通点击 / 排序不再误发 drag-over cleanup |
| Column header clip geometry allocation | 1 new `RectangleGeometry` per clipped header arrange | 0 new `RectangleGeometry` per clipped header arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；frozen-column header clip instance 复用、Rect 更新和 clear 已验证 |
| Column group header forwarding handlers | 2 per group header | 0 per group header | 100.00% removed | 结构/分配优化；Gallery group header 示例有 4 个 group items |
| Column group tree recursive temp lists per 6-node add/remove | 6 lists | 0 lists | 100.00% removed | 结构/分配优化；整棵列分组树共享一个操作级收集列表 |
| Column group tree operation leaf-column lists per add/remove pair | 2 lists | 0 lists | 100.00% removed | 结构/分配优化；add/remove 递归直接同步 `ColumnsInternal` |
| Removed nested column group owning-grid references | 2 retained group references | 0 retained group references | 100.00% released | 正确性/生命周期修复；top-level + nested group remove 后都释放 owner |
| Group column header view item clip geometry allocation | 1 new `RectangleGeometry` per clipped group leaf header arrange | 0 new `RectangleGeometry` per clipped group leaf header arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；frozen-column group leaf header view item clip instance 复用、Rect 更新和 clear 已验证 |
| GroupColumnHeadersPresenter measure visible-column iterators | 2 iterator traversals per measure pass | 0 iterator traversals | 100.00% removed | 结构/分配优化；normal + star-sizing second pass 都直接 indexed 遍历 |
| GroupColumnHeadersPresenter arrange visible-column iterators | 2 iterator traversals per arrange pass | 0 iterator traversals | 100.00% removed | 结构/分配优化；auto-height + star-sizing second pass 都直接 indexed 遍历 |
| Row header local press handler | 1 per row header | 0 per row header | 100.00% removed | 结构/分配优化；行头点击 selection / current slot 语义已验证 |
| Row group header local press handler | 1 per row group header | 0 per row group header | 100.00% removed | 结构/分配优化；分组头点击 current slot 语义已验证 |
| Row header owner/template ordering | template assumed owner ready | owner-dependent state retries from `Owner` setter | null-ref path removed | 正确性修复；分组行头覆盖验证 |
| DataGrid core input handlers | 4 local routed handlers per grid | 0 local core input handlers per grid | 100.00% removed | 结构/分配优化；KeyDown / GotFocus 行为已验证 |
| Pagination re-template stale page-change handlers | 2 handlers retained on old top/bottom pagination parts after re-template | 0 handlers retained | 100.00% removed | 正确性/生命周期修复；旧 template part 不再保留 DataGrid handler，新 part 保持 exactly one handler |
| Rows presenter scroll gesture handler | 1 local handler per rows presenter | 0 local handlers per rows presenter | 100.00% removed | 结构/分配优化；ScrollGesture 滚动行为已验证 |
| Rows presenter clip geometry allocation | 1 new `RectangleGeometry` per arrange | 0 new `RectangleGeometry` per arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；clip instance 复用和 Rect 更新已验证 |
| Row/core scrolling element iterator callsites | 6 external `GetScrollingElements()` / `GetScrollingRows()` callsites | 0 external callsites | 100.00% removed | 结构/分配优化；row height estimate、slot correction、reset 和 reorder hit-test 直接 indexed 遍历 |
| Column width adjustment target delegates | 3 per decrease path + 3 per increase path + 2 per star adjustment | 0 target delegates | 100.00% removed | 结构/分配优化；目标宽度改为 enum 分派 |
| Column width displayed-column filtered iterators | 3 per star adjustment + 3 per decrease path + 3 per increase path | 0 filtered iterators | 100.00% removed | 结构/分配优化；star / non-star adjustment 直接 indexed 遍历 |
| Star-column width adjustment empty temp lists / no eligible star columns | 3 empty lists | 0 lists | 100.00% removed | 结构/分配优化；没有可调 star 列时 outer + desired/max-min inner 列表都不创建 |
| Star-column width adjustment backing-array growths / 4 eligible star columns | 2 growths | 0 growths | 100.00% removed | 结构/分配优化；outer star list 和 desired pass pair list 按剩余 displayed columns 预分配 |
| Star-column width distribution | all-star + partial star increase | unchanged | behavior preserved | 正确性保持；4 列 all increase 与 displayIndex=2 partial increase 已验证 |
| Column frozen-state displayed-column iterator | 1 iterator per correction pass | 0 iterator | 100.00% removed | 结构/分配优化；left/right frozen 状态按 display index 直接计算 |
| Header pseudo-class visible-column list allocation | 1 iterator + 1 `List<DataGridColumn>` copy per refresh | 0 iterator/list allocation | 100.00% removed | 结构/分配优化；first/last/middle header 状态按 display index 刷新 |
| Horizontal column coordinate visible-column iterators | 4 iterators across compute/display/scroll coordinate paths | 0 iterators | 100.00% removed | 结构/分配优化；frozen width、column X、negative offset 和 scroll offset 直接 indexed 遍历 |
| Column lifecycle / edit / copy / resize hit-test iterator callsites | 11 `GetDisplayedColumns()` / `GetVisibleColumns()` callsites | 0 callsites | 100.00% removed | 结构/分配优化；自动列宽、header 初始化、列宽属性、编辑、star coerce、resize、copy、header hit-test 直接 indexed 遍历 |
| Clipboard row content list capacity / copied row with 8 visible cells | 0 capacity, then grows while adding visible cells | 8 capacity up front | 100.00% Add-time growth removed | 结构/分配优化；header row 和每个 selected row 都按 visible column count 预分配 |
| Clipboard row content Add-time growth for 8 visible cells | 2 backing-array growths (`0 -> 4 -> 8`) | 0 backing-array growths | 100.00% removed | 结构/分配优化；状态验证覆盖 8 次 add 后 capacity 仍为 8 |
| Clipboard content row formatting temporaries / copied row | 1 row `StringBuilder` + 1 row string | 0 row temporaries | 100.00% removed | 结构/分配优化；header row 和每个 selected row 直接追加到总 builder |
| Clipboard content interpolated strings / copied cell | 1 interpolated string | 0 interpolated strings | 100.00% removed | 结构/分配优化；cell 内容改为 append 引号、内容和转义引号 |
| Clipboard quote escaping replacement calls / copied cell | 1 `string.Replace()` call | 0 `Replace()` calls | 100.00% removed | 结构/CPU/分配优化；引号转义语义由状态验证覆盖 |
| Plain DataGrid template-apply column header temp list | 1 `List<DataGridColumn>` copy per grid template apply | 0 lists | 100.00% removed | 结构/分配优化；普通列头直接按 `DisplayIndexMap` 插入 |
| Plain DataGrid template-apply display-index comparer | 1 `DisplayIndexComparer` per grid template apply | 0 comparers | 100.00% removed | 结构/分配优化；不再临时排序列集合 |
| Plain DataGrid template-apply column sort | 1 `List.Sort()` over columns per grid template apply | 0 sort calls | 100.00% removed | 结构/CPU 优化；乱序 DisplayIndex header 顺序已验证 |
| Row bottom grid-line clip geometry allocation | 1 new `RectangleGeometry` per realized row arrange | 0 new `RectangleGeometry` per row arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；gridline clip instance 复用和 horizontal offset Rect 更新已验证 |
| Row hidden clip geometry allocation | 1 new `RectangleGeometry` per hidden non-recyclable row application | 0 new `RectangleGeometry` per hidden application after first | 100.00% repeated hidden-state allocation removed | 结构/分配优化；hidden apply / clear / reapply 复用已验证 |
| Row group header template part lookup | built-in `RootLayout` / `RowHeader` not found by code-behind fallback | current names and legacy PART names both resolved | correctness path restored | 正确性修复；默认隐藏 row headers 时 `DataGrid.RowGroups` visual/root smoke `321 -> 315` |
| Row group header child clip geometry allocation | 1 new `RectangleGeometry` per clipped child arrange | 0 new `RectangleGeometry` per clipped child arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；clip instance 复用、Rect 更新、frozen clear 已验证 |
| Row group header frozen child transform allocation | 1 new `TranslateTransform` per frozen visible child arrange | 0 new `TranslateTransform` per frozen visible child arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；transform instance 复用、X 更新、frozen clear 已验证 |
| Details presenter clip geometry allocation | 1 new `RectangleGeometry` per non-frozen details arrange | 0 new `RectangleGeometry` per non-frozen details arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；clip instance 复用、Rect 更新和 frozen clear 已验证 |
| Cell clip geometry allocation | 1 new `RectangleGeometry` per clipped cell arrange | 0 new `RectangleGeometry` per clipped cell arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；frozen-column cell clip instance 复用、Rect 更新和 clear 已验证 |
| Cell header-state binding converters | 2 captured lambdas per realized data cell | 2 cached static converter delegates | per-cell converter closures removed | 结构/分配优化；sort / reorder header state 语义保留 |
| Special column grid subscriptions after `Columns.Clear()` | retained via Reset path | released | leak path removed | 正确性/生命周期修复；覆盖 detail / reorder / selection / checkbox / operation columns |
| Column reorder drag indicator `Pen` allocations | 1 per render | 1 per foreground value | render allocation removed | 结构优化；只影响列拖拽重排交互 |
| Column reorder indicator clip geometry allocation | 1 new `RectangleGeometry` per clipped drag/drop indicator arrange | 0 new `RectangleGeometry` per clipped arrange after first | 100.00% repeated-arrange allocation removed | 结构优化；覆盖 standard/grouped header presenters、drag/drop-location indicator 和 replacement clear |
| Column reorder repeated null-target drag-over notifications | 2 notifications / 2 consecutive null-target moves | 1 notification / 2 consecutive null-target moves | 50.00% removed in verified case | 结构/交互路径优化；target 变为 null 后后续 move 不再重复通知 |
| Row reorder duplicate check LINQ predicate | 1 `Count(predicate)` callsite | 0 LINQ predicate callsites | 100.00% removed | 结构优化；重复 reorder column 仍抛错 |
| Row reorder handle click no-op drop path | 1 `RowReordered` + 1 drop cleanup + 1 rows presenter arrange invalidation per click | 0 | 100.00% removed | 正确性/交互路径优化；未进入拖拽的 click 不再误发 reorder |
| Row reorder dropped `RowsPresenter.DraggedRowIndex` | 1 stale index after drag release | 0 stale indexes | 100.00% removed | 正确性/生命周期修复；真实 drop 后 presenter state 清空 |
| Row reorder unload ghost row indicator | 1 retained ghost row during active drag unload | 0 retained ghost rows | 100.00% removed | 生命周期修复；owner handle unload 时移除 drag indicator |
| Row reorder detach static drag owner/state | active static drag owner + drag fields | null owner + null drag fields | 100.00% released | 生命周期修复；TopLevel detach 不再保留 static drag state |
| SelectionChanged select-all added list preallocated capacity | 0 / 4 items | 4 / 4 items | 100.00% preallocated | 结构/分配优化；批量选择 added list 一次到位 |
| SelectionChanged clear removed list preallocated capacity | 0 / 4 items | 4 / 4 items | 100.00% preallocated | 结构/分配优化；批量清空 removed list 一次到位 |
| Selected-items index rebuild cache capacity for 5 selected rows | 8 capacity after growth | 5 capacity | 37.50% capacity reduction | 结构/分配优化；data refresh 后保留 selection 的缓存一次到位 |
| Default-empty selected-items reset cache copy / `InitializeElements()` | 1 empty `List<object>` copy | 0 list copies | 100.00% removed | 结构/分配优化；默认无 selection 的 reset 不再复制/替换空 cache |
| Cleared selected-items cache backing capacity | released by replacing empty list | still released when `Capacity > 0` | memory release preserved | 正确性/内存防护；清空 selection 后不滞留 backing array |
| Selected-items index enumeration iterator | 1 selected-slot yield iterator per selection diff / public enumeration | 0 selected-slot yield iterators | 100.00% removed | 结构/分配优化；SelectAll / ClearSelection diff 和 `SelectedItems` 枚举语义已验证 |
| SelectedItems public outer yield state machine | 1 compiler-generated yield state machine per public enumeration | 0 compiler-generated yield state machines | 100.00% removed | 轻量结构优化；`IList.GetEnumerator()` 仍保留 1 个显式 enumerator 对象 |
| Selection-inclusive start-slot iterator | 1 selected-slot `GetIndexes(start)` yield iterator per range selection enumeration | 0 selected-slot yield iterators | 100.00% removed | 结构/分配优化；`GetSelectionInclusive(1,3)` 顺序语义已验证 |
| Selected-slots table copy capacity for one selected range | 4 capacity after first add | 1 capacity | 75.00% capacity reduction | 结构/分配优化；`SelectionChanged` 保存旧 selected slots 快照时 range list 一次到位 |
| Column header child detach static drag owner/state | active static drag owner + drag fields | null owner + null drag fields | 100.00% released | 生命周期修复；owner header 移除时释放 resize/reorder static state |
| Column header child detach presenter drag indicator | 1 retained presenter indicator | 0 retained presenter indicators | 100.00% removed | 生命周期修复；单 header 移除时清理 presenter drag visual |
| Column header TopLevel detach visual-child mutation | possible cross-parent child mutation | 0 verified mutation/crash | crash path avoided | 正确性修复；TopLevel detach 只清 state，不移除别的 parent 的 `VisualChildren` |
| Details presenter measure registration | 1 `property.Changed` subscription per presenter instance | 1 static registration per control type | duplicate class-level subscriptions removed | 结构/生命周期优化；`ContentHeight` 仍触发 measure invalidation |
| Data connection enumerable count LINQ iterator | 1 `Cast<object>()` iterator per non-collection count/any fallback | 0 LINQ iterators | 100.00% removed | 结构/分配优化；raw `IEnumerator` count/getAny 行为已验证 |
| DataConnection Any on `IDataGridCollectionView` | 1 enumerator + 1 `MoveNext()` for non-`ICollection` view | 0 enumerators + 1 `IsEmpty` read | 100.00% enumeration removed | 结构/分配优化；collection-view empty check 走接口语义，且不枚举 view |
| DataConnection editable check no-attribute lookup | 1 `GetCustomAttributes(EditableAttribute)` call per property path segment | 0 calls per no-attribute segment | 100.00% removed on common no-attribute path | 结构/分配优化；`Editable(false/true)` 和 nested path 语义已验证 |
| TypeHelper read-only check no-attribute lookup | 1 `GetCustomAttributes(ReadOnlyAttribute)` call per member | 0 calls per no-attribute member | 100.00% removed on common no-attribute path | 结构/分配优化；`ReadOnly(true/false)` property 和 type-level 语义已验证 |
| DataConnection editable check no-attribute read-only lookups | 2 `GetCustomAttributes(ReadOnlyAttribute)` calls per writable property segment | 0 calls when property type and property info have no `ReadOnlyAttribute` | 100.00% removed on common writable-property path | 结构/分配优化；编辑只读判断不再为空 `ReadOnlyAttribute` 结果创建数组 |
| TypeHelper display-name no-attribute lookup | 1 `GetCustomAttributes(DisplayAttribute)` call per property | 0 calls per no-attribute property | 100.00% removed on common no-attribute path | 结构/分配优化；无 `DisplayAttribute` 显示名解析不再创建空数组 |
| Auto-generated column display attribute lookup | 1 `GetCustomAttributes(DisplayAttribute)` call per data property | 0 calls per no-attribute data property | 100.00% removed on common no-attribute path | 结构/分配优化；自动生成列的 plain property 跳过空数组创建 |
| Auto-generated column order-pair list capacity / 4 data properties | 0 capacity, then grows to 4 while inserting generated columns | 4 capacity up front | 100.00% Add-time growth removed | 结构/分配优化；自动生成列排序临时表按属性数量一次到位 |
| DataProperties property array reads / auto-generate pass | 2 reads (`Length` + `foreach`) | 1 read | 50.00% fewer reads | 结构优化；事件触发前完成 property array snapshot，排序/header/hide 语义保持 |
| DisplayAttribute auto-generate semantics | old behavior only | preserved + verified | correctness coverage added | 正确性防护；`ShortName` / `Name` fallback / `Order` / `AutoGenerateField=false` 均验证 |
| Empty filter request object-list copy | 1 empty `List<object>` per clear-filter request | 0 new lists, shared zero-capacity list | 100.00% removed | 结构/分配优化；清空过滤请求不再二次复制空列表 |
| Default filter description conditions list | 1 empty `List<object>` per constructed filter description | 0 lists until public getter or assigned conditions need storage | 100.00% deferred | 结构/分配优化；空条件 `FilterBy()` 和 object initializer 覆盖路径不再创建默认空 list |
| GetAllRows realized-row traversal iterator | 1 yield iterator object per traversal | 0 iterator objects per traversal | 100.00% removed | 结构/分配优化；basic/grouped realized row 数量与顺序已验证 |
| Row group header slot traversal iterator | 1 yield iterator object per traversal | 0 iterator objects per traversal | 100.00% removed | 结构/分配优化；full/start slot 顺序与旧 yield path 已验证 |
| Row group header slot traversal release callsites | 9 `GetIndexes()` iterator callsites | 0 `GetIndexes()` callsites | 100.00% removed | 结构/分配优化；分组行初始化、插入/删除、展开/折叠和查找路径受益 |
| CollectionView Reset empty check for `ICollection` source | 1 extra enumerator + 1 `MoveNext()` per Reset | 0 extra enumerators + 1 `Count` read | 100.00% empty-check enumeration removed | 结构/分配优化；Reset-to-empty 后 view 清空语义已验证 |
| PrepareLocalArray no-filter per-item filter branch | 1 `Filter == null || PassesFilter(item)` check per item | 0 checks per item | 100.00% removed on no-filter path | 结构/CPU 热路径优化；无过滤 source copy 内容和容量语义已验证 |
| CollectionView sorted result materialization | 1 LINQ `ToList()` callsite after sort chain | 0 LINQ `ToList()` callsites | 100.00% removed | 结构/分配优化；sorted result 按输入 count 预分配，stable order 已验证 |
| CollectionView non-empty paged enumerator page list | 1 `List<object?>` + 1 backing array per paged enumeration | 0 page lists / backing arrays | 100.00% removed | 结构/分配优化；分页枚举直接按页范围读取 internal list |
| CollectionView non-empty paged enumerator item copy | `N` page-item `Add` copies per enumeration | 0 page-item copies | 100.00% removed | 结构/CPU 优化；第 2 页 / 末页 public 枚举顺序保持 |
| Empty paged collection-view enumerator list allocation | 1 empty `List<object?>` per `PageIndex < 0` enumeration | 0 lists per enumeration | 100.00% removed | 结构/分配优化；空分页状态仍枚举 0 项 |
| Comparer sort key selector delegate allocation | 1 delegate per `OrderBy()` / `ThenBy()` call | 0 delegates per call | 100.00% per-call allocation removed | 结构/分配优化；static identity selector 复用且 comparer sort 顺序已验证 |
| Path sort key selector delegate allocation | 1 method-group delegate per `OrderBy()` / `ThenBy()` call | 0 delegates per call after sort description construction | 100.00% per-call allocation removed | 结构/分配优化；path sort 升/降序排序和 selector 复用已验证 |
| CollectionView grouped distinct keys for sequence `[1, 2, 1]` | 1 subgroup | 2 subgroups | correctness gap closed | 正确性修复；非连续重复 key 回到匹配 subgroup |
| Path group description mixed owner type keys | 2 correct keys / 3 calls | 3 correct keys / 3 calls | 33.33% correctness coverage improved | 正确性修复；owner type 变化后刷新 cached property type |
| Column custom sort description lookup LINQ chain | `OfType<DataGridComparerSortDescription>() + FirstOrDefault(predicate)` | indexed `for` loop | 100.00% LINQ callsites removed | 结构/分配优化；custom comparer first-match lookup 已验证 |
| Column path sort description lookup LINQ predicate | `FirstOrDefault(predicate)` | indexed `for` loop | 100.00% LINQ callsites removed | 结构/分配优化；path sort first-match lookup 已验证 |
| Column filter description lookup LINQ predicate | `FirstOrDefault(predicate)` | indexed `for` loop | 100.00% LINQ callsites removed | 结构/分配优化；filter first-match / no-match lookup 已验证 |
| DataGridColumn unused clipboard backing field | 1 unused reference field per column instance | 0 unused fields | 100.00% removed | 结构/内存优化；clipboard binding fallback / override 行为已验证 |
| Row expander details binding | 1 `Binding + Path` TwoWay relay per row expander | 1 direct observable binding + checked-state writeback | path binding removed | 结构/生命周期优化；checked ↔ row details 双向同步和 detach 释放均验证 |
| Operation buttons local routed handlers | 4 per realized operation buttons | 0 per realized operation buttons | 100.00% removed | 结构/生命周期优化；edit/save/delete/cancel 统一 owner class handler，行为已验证 |

### 2.2 Gallery 同参数复测

命令：

```bash
dotnet run -c Debug -f net10.0 --no-build \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase datagrid --cold-iterations 3 --iterations 8 --warmup 3
```

| 指标 | baseline | optimized | 改善 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Cold navigation mean | 4230.37 ms | 3032.43 ms | 28.32% | 正向，但 DataGrid 页面波动较大 |
| Cold navigation median | 4110.94 ms | 3011.75 ms | 26.74% | 正向 |
| Cold navigation P95 | 4481.33 ms | 3102.18 ms | 30.77% | 正向 |
| Cold alloc mean | 182352.31 KB | 182395.39 KB | -0.02% | 基本不变 |
| Repeated navigation mean | 1571.65 ms | 1380.94 ms | 12.13% | 正向 |
| Repeated navigation median | 1562.07 ms | 1371.01 ms | 12.23% | 正向 |
| Repeated navigation P95 | 1649.63 ms | 1434.52 ms | 13.04% | 正向 |
| Repeated alloc mean | 172780.27 KB | 172841.63 KB | -0.04% | 基本不变 |

本轮判断：有效。主收益是关闭态过滤 popup 内容不再提前构造；Gallery timing 正向但不是唯一依据，后续 DataGrid 核心仍应继续拆 row/cell/column header 专项。

### 2.3 Column header filter indicator binding 复测

本轮优化点：把 `DataGridFilterIndicator.IsVisible` 从 XAML 三路 `MultiBinding` 改为 `DataGridColumnHeader.FilterIndicatorVisible` DirectProperty + `{TemplateBinding}`，并删除专用 converter。

控件级命令：baseline 使用临时 worktree `HEAD=9bd85e754`，optimized 使用当前工作区；两边均运行两轮 `count=100` 后取平均。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 100
```

| Scenario | 指标 | baseline | optimized | 改善 | 结论 |
| --- | --- | ---: | ---: | ---: | --- |
| DataGrid.Basic | ms/item | 16.703 | 16.382 | 1.92% | 正向；单轮有波动，均值正向 |
| DataGrid.Basic | KB/item | 3032.6 | 2981.6 | 1.68% | 正向 |
| DataGrid.Filter.Menu.Closed | ms/item | 10.323 | 10.101 | 2.15% | 正向 |
| DataGrid.Filter.Menu.Closed | KB/item | 2652.6 | 2613.4 | 1.48% | 正向 |
| DataGrid.Filter.Tree.Closed | ms/item | 8.998 | 8.716 | 3.13% | 正向 |
| DataGrid.Filter.Tree.Closed | KB/item | 2651.3 | 2611.2 | 1.51% | 正向 |
| DataGrid.GalleryShape | ms/item | 45.936 | 44.477 | 3.18% | 正向 |
| DataGrid.GalleryShape | KB/item | 12778.7 | 12562.9 | 1.69% | 正向 |

真实 Gallery 同参数复测：

```bash
dotnet run -c Debug -f net10.0 --no-build \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase datagrid --cold-iterations 3 --iterations 8 --warmup 3
```

| 指标 | baseline | optimized | 改善 | 结论 |
| --- | ---: | ---: | ---: | --- |
| Cold navigation mean | 3368.91 ms | 3332.93 ms | 1.07% | 小幅正向 |
| Cold navigation median | 3249.93 ms | 3352.99 ms | -3.17% | 3 个 cold 样本下波动，非本轮主判据 |
| Cold navigation P95 | 3682.07 ms | 3434.89 ms | 6.71% | 正向 |
| Cold alloc mean | 182245.07 KB | 179481.42 KB | 1.52% | 正向 |
| Repeated navigation mean | 1475.26 ms | 1382.78 ms | 6.27% | 正向 |
| Repeated navigation median | 1463.74 ms | 1383.66 ms | 5.47% | 正向 |
| Repeated navigation P95 | 1531.90 ms | 1405.09 ms | 8.28% | 正向 |
| Repeated alloc mean | 172426.37 KB | 170235.01 KB | 1.27% | 正向 |

本轮判断：有效。主要收益来自列头模板绑定图收敛；结构 visual/logical 计数不变，分配下降稳定，真实 Gallery repeated 指标正向。

### 2.4 Column header hover handler cleanup 复测

本轮优化点：删除 `DataGridColumnHeader` 构造路径中每个实例的 `PointerEntered +=` / `PointerExited +=`，保留已有 `OnPointerEntered` / `OnPointerExited` override，并在 override 中调用原 hover 状态逻辑。排序 tooltip 逻辑仍先执行，hover pseudo-class 与拖拽/resize 状态更新不改变。

控件级命令：baseline 使用临时 worktree `HEAD=2d869466e`，optimized 使用当前工作区；运行 `count=100` 三轮、`count=300` 两组配对（其中一组反向顺序）。`ms/item` 在本地复测中方向不稳定，不能作为本轮主收益；`KB/item` 每轮稳定下降，因此本轮只按结构/分配收益记录。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 300
```

| Scenario | 指标 | baseline | optimized | 改善 | 结论 |
| --- | --- | ---: | ---: | ---: | --- |
| DataGrid.Basic | KB/item | 3034.3 | 3030.2 | 0.13% | 稳定正向；约 4.1 KB/item |
| DataGrid.Filter.Menu.Closed | KB/item | 2628.5 | 2625.1 | 0.13% | 稳定正向；约 3.4 KB/item |
| DataGrid.Filter.Tree.Closed | KB/item | 2628.1 | 2624.8 | 0.13% | 稳定正向；约 3.3 KB/item |
| DataGrid.GalleryShape | KB/item | 12725.3 | 12707.8 | 0.14% | 稳定正向；约 17.5 KB/item |
| DataGrid.Basic | ms/item | 13.812 | 14.862 | -7.60% | 噪声/热状态内，不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | ms/item | 9.518 | 9.917 | -4.19% | 噪声/热状态内，不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | ms/item | 8.980 | 9.331 | -3.91% | 噪声/热状态内，不作为本轮收益 |
| DataGrid.GalleryShape | ms/item | 45.585 | 43.536 | 4.49% | 正向，但同样不单独作为主判据 |

`count=100` 三轮均值用于交叉确认：Basic/Menu/Tree/GalleryShape 的 allocation 分别下降约 `0.14% / 0.21% / 0.14% / 0.14%`；time 方向混合（Basic 小幅负向，GalleryShape 正向），结论保持为结构/分配优化。

### 2.5 Special column detach lifecycle

本轮修复点：`DataGridColumnCollection.RemoveItemPrivate()` 会调用 `NotifyOwningGridAboutToDetached()` 再把 `OwningGrid` 置空，但 `ClearItems()` 原先直接 `OwningGrid = null`，绕过 about-to-detach。对 `DataGridDetailExpanderColumn`、`DataGridRowReorderColumn`、`DataGridSelectionColumn`、`DataGridCheckBoxColumn`、`DataGridOperationColumn` 这类会缓存 `_owningGrid` 并订阅 grid 事件的列，`Columns.Clear()` 会留下 stale grid 引用和事件链。

修复后：

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| `DataGrid.Columns.Clear()` about-to-detach | skipped | called for every internal column | 与单列 remove 语义对齐 |
| Detail / row reorder / operation column grid row events | retained risk | released | `LoadingRow` / `UnloadingRow` 取消订阅 |
| Selection column grid events | retained risk | released | `LoadingRow` / `SelectionChanged` / `PropertyChanged` 取消订阅 |
| CheckBox column grid events | retained risk | released | `CurrentCellChanged` / `KeyDown` / `LoadingRow` 取消订阅 |
| Row reorder `ItemsSource` guard | retained risk | released | `PropertyChanged` 取消订阅 |

这轮不宣称页面加载提速；`datagrid --count 100` smoke 显示 visual/logical 计数不变。收益是清空列集合、重建列模型、动态切换 special columns 后不保留旧 grid 事件链。

### 2.6 Column reorder drag indicator render resource

本轮优化点：`DataGridColumnDraggingOverIndicator.Render()` 原先每次绘制都执行 `new Pen(TextElement.GetForeground(this), 1.0, DashStyle.Dash)`。列拖拽重排时这个 indicator 会跟随 pointer 移动重绘，`Pen` 是 Avalonia `StyledProperty` carrier，不应在 render loop 中重复创建。

修复后按当前 `TextElement.Foreground` 缓存 dashed `Pen`，并把 `TextElement.ForegroundProperty` 加入 `AffectsRender`；foreground 变化时清空缓存，下次 render 重建。

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| Drag indicator repeated render with same foreground | `new Pen` per render | reuse cached `Pen` | render 分配移除 |
| Drag indicator foreground change | implicit fresh pen per render | cached pen invalidated and rebuilt once | 颜色更新语义保留 |

这轮不影响页面加载和默认 DataGrid visual tree；收益只在用户拖拽重排列时发生。

### 2.7 Cell header-state binding converters

本轮优化点：`DataGridCell.EnsureSortBindings()` 原先对每个 realized cell 使用 `BindUtils.RelayBind` 建立两条转换订阅：

- `DataGridColumnHeader.CurrentSortingState -> DataGridCell.IsSorting`
- `DataGridColumnHeader.HeaderDragMode -> DataGridCell.OwningColumnDragging`

`DataGrid.GalleryShape` 控件级场景有 `115` 个 realized data cells；真实 `DataGridShowCase` 声明 `65` 个 DataGrid、`210` 个 columns，因此 per-cell setup 成本会按页面实例量放大。修复后保留 DirectProperty binding 路径，但直接使用 `headerCell.GetObservable(...)` + `this.Bind(...)`，并把转换函数缓存为静态 delegate，避免每 cell 创建捕获 lambda 和 `BindUtils` 注册检查。

控件级命令：baseline 使用临时 worktree `HEAD=af9e97309`，optimized 使用当前工作区；两边各运行两轮 `count=300` 后取平均。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 300
```

| Scenario | 指标 | baseline | optimized | 改善 | 结论 |
| --- | --- | ---: | ---: | ---: | --- |
| DataGrid.Basic | ms/item | 15.019 | 13.630 | 9.25% | 正向 |
| DataGrid.Basic | KB/item | 3030.3 | 3028.5 | 0.06% | 小幅正向 |
| DataGrid.Filter.Menu.Closed | ms/item | 10.262 | 9.538 | 7.06% | 正向 |
| DataGrid.Filter.Menu.Closed | KB/item | 2625.3 | 2624.0 | 0.05% | 小幅正向 |
| DataGrid.Filter.Tree.Closed | ms/item | 9.600 | 9.445 | 1.61% | 小幅正向 |
| DataGrid.Filter.Tree.Closed | KB/item | 2625.0 | 2623.6 | 0.05% | 小幅正向 |
| DataGrid.GalleryShape | ms/item | 46.846 | 43.783 | 6.54% | 正向 |
| DataGrid.GalleryShape | KB/item | 12708.7 | 12701.2 | 0.06% | 小幅正向 |

本轮判断：保留。主收益仍按结构/分配记录；visual/logical 计数不变，sort / reorder state propagation 由 `--verify-datagrid-states` 覆盖。

### 2.8 Details presenter measure registration

本轮优化点：`DataGridDetailsPresenter` 原先在实例构造函数里调用 `AffectsMeasure<DataGridDetailsPresenter>(ContentHeightProperty)`。Avalonia 的 `AffectsMeasure<T>` 是 class-level 注册：源码注释要求在控件 static constructor 中调用，实现会对 `property.Changed` 订阅 observer。因此每个 realized `DataGridDetailsPresenter` 都会重复注册同一个类型/属性的 observer。

`DataGridRowTheme` 中每个 realized row 都包含一个 `DataGridDetailsPresenter` 槽位，即使 details frame 通过 `IsDetailsVisible` 折叠；Gallery 的 `Expandable Row`、`Order Specific Column`、`Row Header` 等场景会使用 `RowDetailsTemplate`。修复后只保留一次静态注册，所有实例共享同一条 class-level observer。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| `DataGridDetailsPresenter` 构造 | 每实例调用 `AffectsMeasure` | 不调用 | 构造路径去掉 class-level observer 订阅 | 有效；row 越多，重复订阅越多 |
| `ContentHeight` measure invalidation | 依赖重复注册 | 依赖静态注册 | 行为保留 | `--verify-datagrid-states` 覆盖 `ContentHeight` 改变后 `IsMeasureValid=false` |
| 默认 DataGrid visual/logical tree | 不变 | 不变 | 无结构变化 | 本轮不宣称页面导航或 visual count 下降 |

当前工作区 smoke：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 100
```

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 16.477 | 2975.2 | 305.0 | 1.0 | smoke 通过 |
| DataGrid.Filter.Menu.Closed | 9.892 | 2608.5 | 267.0 | 1.0 | smoke 通过 |
| DataGrid.Filter.Tree.Closed | 8.769 | 2606.1 | 267.0 | 1.0 | smoke 通过 |
| DataGrid.GalleryShape | 42.884 | 12536.5 | 1260.0 | 5.0 | smoke 通过 |

### 2.9 Row expander details visibility binding

本轮优化点：`DataGridRowExpander.NotifyLoadingRow()` 原先用 `BindUtils.RelayBind(this, IsCheckedProperty, row, DataGridRow.IsDetailsVisibleProperty, BindingMode.TwoWay)`。这会为每个 realized row expander 创建一个 `Binding`、字符串 `Path` 和完整 binding expression。Gallery `Expandable Row`、`Order Specific Column`、`Hidden Columns` 等详情列场景会在页面加载时创建 row expander；用户展开/折叠详情时还会反复走 `IsChecked` ↔ `IsDetailsVisible` 同步。

修复后：

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| Row expander loading | `BindUtils.RelayBind` TwoWay path binding | row `IsDetailsVisible` direct observable -> expander `IsChecked` | 构造路径更轻 |
| 用户点击 expander | Binding engine 写回 row | `OnPropertyChanged(IsChecked)` 写回 row `IsDetailsVisible` | 双向语义保留 |
| Row details programmatic change | Binding engine 更新 expander | direct observable 更新 expander | 双向语义保留 |
| Row unload / visual detach | 依赖 `UnloadingRow` | `UnloadingRow` + `OnDetachedFromVisualTree` 兜底释放 | 修复 detach-only 路径 stale row 引用风险 |

本轮还给性能工具补了 `DataGrid.RowDetails.Collapsed` 场景，用于覆盖关闭态详情列和 row expander binding setup。当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.RowDetails.Collapsed | 10.266 | 3046.7 | 320.0 | 1.0 | smoke 通过；本轮新增场景，无历史 timing 对比 |

### 2.10 Column header press/release/move handler cleanup

本轮优化点：`DataGridColumnHeader` 构造函数原先为每个 realized header 注册 `PointerPressed` / `PointerReleased` / `PointerMoved` 三个本地 routed handlers；`DataGridColumn.CreateHeader()` 还为 `HeaderPointerPressed` / `HeaderPointerReleased` 转发注册两个 per-header lambda。DataGrid 页面列声明多、列头频繁 realized，这类 handler 会进入每个控件实例的 `_eventHandlers` 字典。

修复后把内部 resize / reorder / sort 逻辑和 column public event 转发迁到 `DataGridColumnHeader` static constructor 的 class handlers：

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| Header press/move/release internal logic | 3 local handlers / realized header | 3 class handlers / control type | per-header `_eventHandlers` 记录移除 |
| `DataGridColumn.HeaderPointerPressed/Released` 转发 | 2 per-header lambda | 2 class handlers / control type | per-header lambda 移除 |
| Routed strategy | event adders 默认 Direct/Bubble | class handler 默认 Direct/Bubble | 与原 `PointerXxx +=` 路由策略一致 |
| `HeaderPointerPressed` 转发 | local lambda | class handler | 状态验证覆盖 exactly once 和 sender 保持为 `DataGridColumn` |

这轮属于结构/分配和生命周期清理，不把 `datagrid --count 100` timing smoke 当成确定加速。主收益是 standard column header 的本地 routed handlers 从 `5/header` 收敛到 `0/header`，并且验证 realized header 不再持有 `PointerPressed` / `PointerReleased` / `PointerMoved` 本地订阅。

同口径复测：baseline 使用临时 worktree `HEAD=ac3db9f2e`，optimized 使用当前工作区；两边均运行 `datagrid --count 100`。提升公式：`(baseline - optimized) / baseline`。

| Scenario | 指标 | baseline | optimized | 提升 |
| --- | --- | ---: | ---: | ---: |
| DataGrid.Basic | ms/item | 17.471 | 14.494 | 17.04% |
| DataGrid.Basic | KB/item | 2975.3 | 2969.5 | 0.19% |
| DataGrid.Filter.Menu.Closed | ms/item | 10.317 | 10.312 | 0.05% |
| DataGrid.Filter.Menu.Closed | KB/item | 2607.7 | 2604.8 | 0.11% |
| DataGrid.Filter.Tree.Closed | ms/item | 9.141 | 8.662 | 5.24% |
| DataGrid.Filter.Tree.Closed | KB/item | 2605.3 | 2600.8 | 0.17% |
| DataGrid.GalleryShape | ms/item | 44.224 | 43.936 | 0.65% |
| DataGrid.GalleryShape | KB/item | 12536.7 | 12512.4 | 0.19% |

`DataGrid.RowDetails.Collapsed` 是本轮新增 scenario，baseline 没有同名场景，不能给 timing 百分比；它对应的收益按 RowExpander details binding 结构项记录。

### 2.11 Column group header pointer forwarding cleanup

本轮优化点：`DataGridColumnGroupItem.CreateHeader()` 原先为每个 group header 注册两个本地 forwarding lambda：

- `PointerPressed -> HeaderPointerPressed`
- `PointerReleased -> HeaderPointerReleased`

Gallery `Grouping table head` 示例声明 4 个 `DataGridColumnGroupItem`，页面导航时会创建对应的 `DataGridColumnGroupHeader`。修复后由 `DataGridColumnGroupHeader` static constructor 注册 class handlers，再通过 `OwningGroupItem` 转发事件；路由策略与原 `PointerXxx +=` 保持一致，都是 Direct/Bubble。

| 场景 | baseline | optimized | 结论 |
| --- | --- | --- | --- |
| Group header pointer forwarding | 2 per-header lambda | 2 class handlers / control type | per-header `_eventHandlers` 记录移除 |
| `HeaderPointerPressed` sender | `DataGridColumnGroupItem` | `DataGridColumnGroupItem` | 状态验证覆盖 exactly once 和 sender 保持 |
| Group header smoke scenario | n/a | `DataGrid.GroupHeaders` | 本轮新增，用于覆盖 group header 初始化路径 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.GroupHeaders | 9.618 | 2595.5 | 266.0 | 1.0 | smoke 通过；本轮新增场景，无历史 timing 对比 |

### 2.12 Row header pointer handler cleanup

本轮优化点：`DataGridRowHeader` 构造函数原先为每个 row header 调用 `AddHandler(PointerPressedEvent, HandlePointerPressed, handledEventsToo: true)`。Row header 是 `DataGridRowTheme` 的模板槽位，Gallery 的 `Row Header`、row details、特殊列等场景会随 realized rows 创建这些 header；即使后续通过 `HeadersVisibility` 控制可见性，本地 handler 仍已经进入每个 header 实例的 `_eventHandlers` 字典。

修复后把 press 逻辑迁到 `DataGridRowHeader` static constructor 的 class handler，并继续使用 `handledEventsToo: true`。保留原有行为：左键点击行头会 focus grid、更新 selection 和 `CurrentSlot`；右键路径仍调用原来的 grid 状态更新。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Row header `PointerPressed` 注册 | 1 local handler / row header | 1 class handler / control type | 100.00% per-row handler removed | 有效；per-instance `_eventHandlers` 记录移除 |
| `handledEventsToo` 语义 | true | true | preserved | 保留子元素已处理事件后的行头选择路径 |
| 左键点击行头 | focus + select row + update current slot | focus + select row + update current slot | 行为保留 | `--verify-datagrid-states` 覆盖 |
| Row header smoke scenario | n/a | `DataGrid.RowHeaders` | 新增覆盖 | 本轮新增，无历史 timing 百分比 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.RowHeaders | 9.838 | 3127.2 | 336.0 | 1.0 | smoke 通过；本轮新增场景，无历史 timing 对比 |

### 2.13 Row group header pointer handler cleanup

本轮优化点：`DataGridRowGroupHeader` 构造函数原先为每个 row group header 注册一个本地 `PointerPressed` lambda：

- `AddHandler(PointerPressedEvent, (s, e) => HandlePointerPressed(e), handledEventsToo: true)`

分组数据网格会按可视分组头创建 `DataGridRowGroupHeader`，并通过回收池复用；handler 注册发生在每个新建 group header 实例上。修复后由 `DataGridRowGroupHeader` static constructor 注册 class handler，保留 `handledEventsToo: true`，左键点击分组头仍更新 `CurrentSlot`。

新增验证同时打出了一个正确性问题：分组头模板内的 `DataGridRowHeader` 可能先完成自己的 `OnApplyTemplate()`，随后父 `DataGridRowGroupHeader` 才设置 `Owner`。原实现直接在 `OnApplyTemplate()` 中读取 `OwningGrid.BorderThickness`，会在这个顺序下空引用。修复后 `DataGridRowHeader.Owner` setter 和 `OnApplyTemplate()` 都走同一个 owner-dependent state 刷新路径；如果模板先到，等 `Owner` 设置后再补齐 separator / pseudo-class 状态。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Row group header `PointerPressed` 注册 | 1 local lambda / row group header | 1 class handler / control type | 100.00% per-header handler removed | 有效；per-instance `_eventHandlers` 记录移除 |
| `handledEventsToo` 语义 | true | true | preserved | 保留子元素已处理事件后的分组头状态更新路径 |
| 左键点击分组头 | update current slot | update current slot | 行为保留 | `--verify-datagrid-states` 覆盖 |
| Row header owner/template order | `OnApplyTemplate()` assumes owner ready | `Owner` setter retries owner-dependent state | null-ref path removed | 正确性修复 |
| Row group smoke scenario | n/a | `DataGrid.RowGroups` | 新增覆盖 | 本轮新增，无历史 timing 百分比 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.RowGroups | 13.517 | 3003.0 | 321.0 | 1.0 | smoke 通过；本轮新增场景，无历史 timing 对比 |

### 2.14 DataGrid core input handler cleanup

本轮优化点：`DataGrid` 构造函数原先为每个 grid 实例注册 4 个本地 routed event handlers：

- `KeyDown += HandleKeyDown`
- `KeyUp += HandleKeyUp`
- `GotFocus += HandleGotFocus`
- `LostFocus += HandleLostFocus`

真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，页面导航时会放大这类每实例 `_eventHandlers` 记录。Avalonia `InputElement` 已经通过 class handlers 把 `KeyDown` / `KeyUp` / `GotFocus` / `LostFocus` 分发到 virtual override；修复后改为 `OnKeyDown` / `OnKeyUp` / `OnGotFocus` / `OnLostFocus` 调用原处理函数，并保留 `!e.Handled` 口径。`DataGridCheckBoxColumn` 等按需附加到 grid 的功能订阅不属于本轮 core constructor cleanup，仍按对应功能生命周期管理。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Core input routed handler registration | 4 local handlers / grid | 0 local handlers / grid | 100.00% removed | 有效；`_eventHandlers` 记录移除 |
| `KeyDown` navigation | constructor instance handler | `OnKeyDown` override | behavior preserved | `Down` key 从 slot `0 -> 1` 覆盖 |
| `GotFocus` state | constructor instance handler | `OnGotFocus` override | behavior preserved | `ContainsFocus` 更新覆盖 |
| `KeyUp` / `LostFocus` dispatch | constructor instance handler | virtual override calls same handler body | handler body preserved | 本地 handler 移除覆盖；交互语义未改 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.101 | 2967.1 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.487 | 2601.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.805 | 2598.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 11.064 | 3125.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 12.351 | 3044.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.580 | 2594.6 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.986 | 3001.7 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.297 | 12490.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.15 Rows presenter scroll gesture handler cleanup

本轮优化点：`DataGridRowsPresenter` 构造函数原先为每个 presenter 注册一个本地 `ScrollGesture` handler：

- `AddHandler(InputElement.ScrollGestureEvent, OnScrollGesture)`

每个 `DataGrid` 模板会创建一个 `DataGridRowsPresenter`，真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，页面导航时会同步创建 rows presenter。修复后由 `DataGridRowsPresenter` static constructor 注册 class handler，继续调用原滚动逻辑 `OwningGrid.UpdateScroll(-e.Delta)`，不改变滚动手势方向和 handled 语义。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Rows presenter `ScrollGesture` 注册 | 1 local handler / rows presenter | 1 class handler / control type | 100.00% per-presenter handler removed | 有效；`_eventHandlers` 记录移除 |
| Scroll gesture handled 状态 | handled when grid scrolls | handled when grid scrolls | preserved | `--verify-datagrid-states` 覆盖 |
| Scroll gesture vertical offset | updates `VerticalOffset` | updates `VerticalOffset` | 行为保留 | 40-row grid 触发滚动覆盖 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.096 | 2966.6 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.287 | 2602.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.460 | 2598.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.872 | 3124.8 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.518 | 3044.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.734 | 2593.5 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.038 | 3001.2 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 45.845 | 12488.7 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.16 Rows presenter clip geometry allocation

本轮优化点：`DataGridRowsPresenter.ArrangeOverride()` 原先每次布局都会创建新的 `RectangleGeometry` 并赋给 `Clip`。每个 `DataGrid` 模板会创建一个 rows presenter，真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`；页面导航、窗口尺寸变化、滚动带来的布局 pass 都会触发 rows presenter arrange。

修复后 presenter 持有一个 `_clipGeometry`，首次 arrange 创建，后续只更新 `Rect`。由于换新 `Clip` 对象会触发 `Visual.ClipProperty` 的 `AffectsRender`，但复用同一个 geometry 后不应依赖 property 引用变化，本轮在 `Rect` 变化时显式 `InvalidateVisual()`，保持裁剪刷新语义。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Rows presenter clip allocation / repeated arrange | 1 new `RectangleGeometry` / arrange | 0 new `RectangleGeometry` / arrange after first | 100.00% repeated allocation removed | 有效；每 presenter 生命周期内从 `N` 次分配降到 `1` 次 |
| 65-grid Gallery shape estimated clip allocation / repeated arrange pass | 65 new `RectangleGeometry` / pass | 0 new `RectangleGeometry` / pass after first | 100.00% repeated allocation removed | 结构收益；按真实 `DataGridShowCase` 声明数量估算 |
| Clip geometry instance | replaced every arrange | reused across repeated arrange and resize | reuse verified | `--verify-datagrid-states` 覆盖 |
| Clip geometry rect | implicit by new geometry | reused geometry updates `Rect` | 行为保留 | 尺寸变化后 `Rect` 更新已验证 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.596 | 2966.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.496 | 2601.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.789 | 2598.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.236 | 3124.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.969 | 3043.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.684 | 2592.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.989 | 3000.6 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.319 | 12486.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.17 Row bottom grid-line clip geometry allocation

本轮优化点：`DataGridRow.ArrangeOverride()` 原先只要模板存在 `PART_BottomGridLine`，每个 realized row 的每次 arrange 都会创建新的 `RectangleGeometry`：

- `new RectangleGeometry()`
- `gridlineClipGeometry.Rect = new Rect(...)`
- `_bottomGridLine.Clip = gridlineClipGeometry`

真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，导航时会同步 realize 多个 row；滚动、水平偏移变化、尺寸变化和重排都会触发 row arrange。修复后每个 row 缓存 `_bottomGridLineClipGeometry`，重复 arrange 只更新 `Rect`，并在 `OnApplyTemplate()` / `ResetGridLine()` 清掉旧引用，避免跨模板持有旧 visual 状态。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Row bottom grid-line clip allocation / repeated arrange | 1 new `RectangleGeometry` / realized row arrange | 0 new `RectangleGeometry` / realized row arrange after first | 100.00% repeated allocation removed | 有效；每 row/template 生命周期内从 `N` 次分配降到 `1` 次 |
| 8-row basic grid repeated arrange pass | 8 new `RectangleGeometry` / pass | 0 new `RectangleGeometry` / pass after first | 100.00% repeated allocation removed | 结构收益；按 `DataGrid.Basic` 默认 row 数估算 |
| Clip geometry instance | replaced every arrange | reused across repeated arrange and horizontal offset change | reuse verified | `--verify-datagrid-states` 覆盖 |
| Clip geometry rect | implicit by new geometry | reused geometry updates `Rect` | 行为保留 | 水平偏移变化后 `Rect` 更新已验证 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.237 | 2966.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.223 | 2600.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.460 | 2597.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.171 | 3124.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 12.117 | 3043.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.429 | 2592.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.279 | 3000.1 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.514 | 12486.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.18 Details presenter clip geometry allocation

本轮优化点：`DataGridDetailsPresenter.ArrangeOverride()` 原先在 row details 非 frozen 模式下每次 arrange 都会创建新的 `RectangleGeometry` 并赋给 `Clip`。row details 可见、水平滚动、窗口尺寸变化、行详情展开/折叠和 grid relayout 都会触发 details presenter arrange；真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，其中 row details 场景会为 realized row 创建 `DataGridDetailsPresenter`。

修复后 details presenter 持有一个 `_clipGeometry`，首次需要裁剪时创建，后续只更新 `Rect`；`IsRowDetailsFrozen=true` 时仍清空 `Clip`，回到非 frozen 后继续复用同一个 cached geometry。由于 `Visual.Clip` 换引用会触发 `AffectsRender`，而复用 geometry 只更新 `RectangleGeometry.Rect`，本轮在 `Rect` 变化时显式 `InvalidateVisual()`，保持裁剪刷新语义。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Details presenter clip allocation / repeated arrange | 1 new `RectangleGeometry` / non-frozen details arrange | 0 new `RectangleGeometry` / non-frozen details arrange after first | 100.00% repeated allocation removed | 有效；每 details presenter 生命周期内从 `N` 次分配降到 `1` 次 |
| 8-row visible details repeated arrange pass | 8 new `RectangleGeometry` / pass | 0 new `RectangleGeometry` / pass after first | 100.00% repeated allocation removed | 结构收益；按验证场景 visible details rows 估算 |
| Clip geometry instance | replaced every arrange | reused across repeated arrange and horizontal offset change | reuse verified | `--verify-datagrid-states` 覆盖 |
| Frozen row details clip | cleared by assigning `Clip = null` | `ClearClipGeometry()` clears `Clip` and keeps cache for reuse | 行为保留 | frozen clear 和恢复后复用均已验证 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.711 | 2966.1 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.631 | 2601.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.381 | 2597.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.931 | 3124.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.321 | 3043.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.066 | 2592.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.951 | 3000.0 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.034 | 12486.4 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.19 Cell clip geometry allocation

本轮优化点：`DataGridCellsPresenter.EnsureCellClip()` 原先只要 scrolling cell 被 frozen columns 或右侧边界裁剪，每次 cells presenter arrange 都会创建新的 `RectangleGeometry` 并赋给 `DataGridCell.Clip`。真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，多个 frozen-column 示例设置 `LeftFrozenColumnCount` / `RightFrozenColumnCount`；水平滚动、窗口尺寸变化、列宽变化、行虚拟化复用和 grid relayout 都会触发 realized row 的 cells presenter arrange。

修复后 `DataGridCell` 持有一个 `_clipGeometry`，首次需要裁剪时创建，后续只更新 `Rect`；裁剪不再需要时仍清空 `Clip`。由于 `Visual.Clip` 换引用会触发 `AffectsRender`，而复用 geometry 只更新 `RectangleGeometry.Rect`，本轮在 `Rect` 变化时显式 `InvalidateVisual()`，保持裁剪刷新语义。`EnsureCellDisplay()` 的 current-cell hidden clip 也改走同一个缓存路径，避免当前格在横向虚拟化隐藏路径重复分配。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Cell clip allocation / repeated arrange | 1 new `RectangleGeometry` / clipped cell arrange | 0 new `RectangleGeometry` / clipped cell arrange after first | 100.00% repeated allocation removed | 有效；每 cell 生命周期内从 `N` 次分配降到 `1` 次 |
| 8-row frozen-column verification pass | 8 new `RectangleGeometry` / clipped column pass | 0 new `RectangleGeometry` / clipped column pass after first | 100.00% repeated allocation removed | 结构收益；按验证场景单个 clipped scrolling column 估算 |
| Clip geometry instance | replaced every arrange | reused across repeated arrange and horizontal offset change | reuse verified | `--verify-datagrid-states` 覆盖 |
| Clip clear path | assign `Clip = null` | `ClearClipGeometry()` clears `Clip` while keeping reusable cache | 行为保留 | frozen clipping 取消后 `Clip` 清空已验证 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.326 | 2964.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.343 | 2600.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.870 | 2596.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.448 | 3123.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.622 | 3042.1 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 10.193 | 2592.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.897 | 2998.9 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 45.449 | 12482.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.20 Column header clip geometry allocation

本轮优化点：`DataGridColumnHeadersPresenter.EnsureColumnHeaderClip()` 原先只要 scrolling column header 被 frozen columns 或右侧边界裁剪，每次 header presenter arrange 都会创建新的 `RectangleGeometry` 并赋给 `DataGridColumnHeader.Clip`。真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`、210 个 column declarations，多个 frozen-column 示例设置 `LeftFrozenColumnCount` / `RightFrozenColumnCount`；水平滚动、窗口尺寸变化、列宽变化和 grid relayout 都会触发 header presenter arrange。

修复后 `DataGridColumnHeader` 持有一个 `_clipGeometry`，首次需要裁剪时创建，后续只更新 `Rect`；裁剪不再需要或列头进入 frozen 区时仍清空 `Clip`。由于 `Visual.Clip` 换引用会触发 `AffectsRender`，而复用 geometry 只更新 `RectangleGeometry.Rect`，本轮在 `Rect` 变化时显式 `InvalidateVisual()`，保持裁剪刷新语义。

| 场景 | baseline | optimized | 改善 | 结论 |
| --- | --- | --- | --- | --- |
| Column header clip allocation / repeated arrange | 1 new `RectangleGeometry` / clipped header arrange | 0 new `RectangleGeometry` / clipped header arrange after first | 100.00% repeated allocation removed | 有效；每 header 生命周期内从 `N` 次分配降到 `1` 次 |
| 6-column frozen-header verification pass | 1 new `RectangleGeometry` / clipped scrolling header pass | 0 new `RectangleGeometry` / clipped scrolling header pass after first | 100.00% repeated allocation removed | 结构收益；按验证场景单个 clipped scrolling header 估算 |
| Clip geometry instance | replaced every arrange | reused across repeated arrange and horizontal offset change | reuse verified | `--verify-datagrid-states` 覆盖 |
| Clip clear path | assign `Clip = null` | `ClearClipGeometry()` clears `Clip` while keeping reusable cache | 行为保留 | frozen clipping 取消后 `Clip` 清空已验证 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 16.045 | 2964.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.194 | 2599.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.336 | 2596.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 11.296 | 3122.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 11.803 | 3041.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.993 | 2592.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 11.546 | 2998.7 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 47.777 | 12479.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.21 Group column header view item clip geometry allocation

本轮优化点：`DataGridGroupColumnHeadersPresenter.EnsureColumnHeaderClip()` 原先只要 grouped leaf `DataGridHeaderViewItem` 被左 frozen columns 裁剪，每次 group header presenter arrange 都会创建新的 `RectangleGeometry` 并赋给 `DataGridHeaderViewItem.Clip`。真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`、210 个 column declarations，分组表头与 frozen columns 同时启用时，水平滚动、窗口尺寸变化、列宽变化和 grid relayout 都会触发 group header presenter arrange。

修复后 `DataGridHeaderViewItem` 持有一个 `_clipGeometry`，首次需要裁剪时创建，后续只更新 `Rect`；裁剪不再需要或 leaf header 进入 frozen 区时仍清空 `Clip`。由于 `Visual.Clip` 换引用会触发 `AffectsRender`，而复用 geometry 只更新 `RectangleGeometry.Rect`，本轮在 `Rect` 变化时显式 `InvalidateVisual()`，保持裁剪刷新语义。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Group column header view item clip allocation / repeated arrange | 1 new `RectangleGeometry` / clipped group leaf header arrange | 0 new `RectangleGeometry` / clipped group leaf header arrange after first | `(1 - 0) / 1` | 100.00% | 有效；每 header view item 生命周期内从 `N` 次分配降到 `1` 次 |
| 6-column grouped frozen-header verification pass | 1 new `RectangleGeometry` / clipped grouped scrolling leaf header pass | 0 new `RectangleGeometry` / clipped grouped scrolling leaf header pass after first | `(1 - 0) / 1` | 100.00% | 结构收益；按验证场景单个 clipped grouped leaf header 估算 |
| Clip geometry instance | replaced every arrange | reused across repeated arrange and horizontal offset change | reuse verified | n/a | `--verify-datagrid-states` 覆盖 |
| Clip clear path | assign `Clip = null` | `ClearClipGeometry()` clears `Clip` while keeping reusable cache | behavior preserved | n/a | frozen clipping 取消后 `Clip` 清空已验证 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.995 | 2964.6 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.570 | 2599.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.152 | 2596.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.814 | 3122.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.248 | 3041.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.810 | 2590.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.872 | 2998.7 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.773 | 12479.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.22 Column reordering indicator clip geometry allocation

本轮优化点：`DataGridColumnHeadersPresenter.EnsureColumnReorderingClip()` 与 `DataGridGroupColumnHeadersPresenter.EnsureColumnReorderingClip()` 原先只要 column reorder drag indicator / drop-location indicator 被 frozen column 区域裁剪，每次 presenter arrange 都会创建新的 `RectangleGeometry` 并赋给 indicator `Clip`。列拖拽重排期间 pointer move、auto-scroll、水平滚动和布局刷新都会反复触发这个路径；真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`、210 个 column declarations，列重排是用户可交互高频路径。

修复后 standard column headers presenter 为当前 `DragIndicator` 缓存一个 clip geometry，group column headers presenter 分别为 `DragIndicator` 与 `DropLocationIndicator` 缓存独立 clip geometry。裁剪边界变化时只更新 `Rect` 并显式 `InvalidateVisual()`；不再需要裁剪时清空 `Clip`；替换 indicator 控件时清空旧控件 `Clip`，避免旧控件继续持有 presenter 缓存 geometry。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Standard column drag indicator clip allocation / repeated arrange | 1 new `RectangleGeometry` / clipped arrange | 0 new `RectangleGeometry` / clipped arrange after first | `(1 - 0) / 1` | 100.00% | 有效；同一 drag indicator 生命周期内从 `N` 次分配降到 `1` 次 |
| Group column drag indicator clip allocation / repeated arrange | 1 new `RectangleGeometry` / clipped arrange | 0 new `RectangleGeometry` / clipped arrange after first | `(1 - 0) / 1` | 100.00% | 有效；group header presenter drag indicator 复用已验证 |
| Group column drop-location indicator clip allocation / repeated arrange | 1 new `RectangleGeometry` / clipped arrange | 0 new `RectangleGeometry` / clipped arrange after first | `(1 - 0) / 1` | 100.00% | 有效；drop-location indicator 使用独立缓存，未与 drag indicator 共享 |
| Replacement clear path | old indicator can retain `Clip` reference | old indicator `Clip` cleared on replacement | behavior/lifecycle preserved | n/a | 旧 drag / drop-location indicator clear 已验证 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.731 | 2964.6 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.638 | 2599.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.284 | 2597.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.150 | 3122.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.007 | 3041.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.193 | 2591.5 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.507 | 2998.7 | 321.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 46.298 | 12479.7 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.23 Row hidden clip geometry allocation

本轮优化点：`DataGrid.RemoveDisplayedElement()` 和 `DataGridDisplayData.ClearElements(recycle: true)` 原先在非回收隐藏 row 路径中每次都执行 `new RectangleGeometry()` 并赋给 `DataGridRow.Clip`。这个路径会在垂直滚动、ItemsSource 变化、删除行和清空 rows 时反复触发；真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，row 虚拟化/移除路径覆盖所有 realized rows。

修复后 `DataGridRow` 持有 `_hiddenClipGeometry`，第一次进入隐藏态时创建，重复隐藏只复用同一个空 clip；恢复显示时仍清空 `Clip`，不改变显示行为。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Hidden row clip allocation / repeated hidden-state application | 1 new `RectangleGeometry` / hidden non-recyclable row application | 0 new `RectangleGeometry` / hidden application after first | `(1 - 0) / 1` | 100.00% | 有效；每 row 生命周期内从 `N` 次隐藏分配降到 `1` 次 |
| Hidden clip clear path | `Clip = null` on redisplay | `ClearHiddenClipGeometry()` clears `Clip` on redisplay | behavior preserved | n/a | 显示恢复语义保留 |
| Cached geometry reapply | replaced on every hidden application | same `RectangleGeometry` reused after clear/reapply | reuse verified | n/a | `--verify-datagrid-states` 覆盖 |

### 2.24 Row group header child clip geometry allocation

本轮优化点：`DataGridRowGroupHeader.ArrangeOverride()` 的非冻结 row group header 子控件裁剪路径原先每次 clipped child arrange 都会创建新的 `RectangleGeometry`。同时内置模板的 root/header part 名称与 code-behind 查找不一致，导致 `_rootElement` / `_headerElement` 为空，默认 `HeadersVisibility=Column` 下分组行头内 row header 未按 grid 设置隐藏，owner 和 child clip lifecycle 也没有生效。

修复后 code-behind 同时兼容 legacy PART 名和当前内置模板名；`DataGridRowGroupHeader` 对每个 clipped child 缓存一个 `RectangleGeometry`，只在边界变化时更新 `Rect` 并显式 `InvalidateVisual()`。分组头整体冻结、重套模板和 detach 会清空旧 child `Clip` 并释放字典引用。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Row group header child clip allocation / repeated arrange | 1 new `RectangleGeometry` / clipped child arrange | 0 new `RectangleGeometry` / clipped child arrange after first | `(1 - 0) / 1` | 100.00% | 有效；每 child 生命周期内从 `N` 次分配降到 `1` 次 |
| Row group header template part lookup | built-in `RootLayout` / `RowHeader` not found by old lookup | current template names and legacy PART names resolved | correctness path restored | n/a | 正确性修复；row header visibility / owner / clip lifecycle 生效 |
| `DataGrid.RowGroups` visual/root smoke | 321.0 | 315.0 | `(321 - 315) / 321` | 1.87% | smoke-only；默认隐藏 row headers 后少 6 个 visual/root |
| `DataGrid.RowGroups` KB/item smoke | 2998.7 KB | 2956.5 KB | `(2998.7 - 2956.5) / 2998.7` | 1.41% | smoke-only；结构修复带来的分配下降，不声明稳定 timing |
| Clip clear lifecycle | old child `Clip` may remain until replacement/detach | frozen clear, re-template and detach clear child `Clip` | lifecycle preserved | n/a | `--verify-datagrid-states` 覆盖 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.286 | 2964.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.246 | 2600.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.234 | 2596.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.797 | 3123.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.129 | 3041.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.790 | 2590.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.666 | 2956.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.742 | 12479.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.25 Row group header frozen child transform allocation

本轮优化点：`DataGridRowGroupHeader.ArrangeOverride()` 原先在 row group header 非整体冻结、但内部 row header 子控件需要随水平滚动冻结时，每次 arrange 都会创建新的 `TranslateTransform` 并赋给 frozen child 的 `RenderTransform`。这个路径与上一轮 child clip 属于同一条 row group header 水平滚动 / relayout 热路径；真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，分组表格和 row headers/frozen offset 组合时会触发。

修复后 `DataGridRowGroupHeader` 对每个 frozen child 缓存一个 `TranslateTransform`，重复 arrange 只更新 `X`；当 row group header 整体 frozen、重套模板或 detach 时清空 child `RenderTransform` 并释放字典引用。由于 `TranslateTransform.X/Y` 变化会 raise transform changed，Avalonia 会对使用该 `RenderTransform` 的 visual invalidate render，本轮不需要替换 transform 引用来刷新位置。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Row group header frozen child transform allocation / repeated arrange | 1 new `TranslateTransform` / frozen visible child arrange | 0 new `TranslateTransform` / frozen visible child arrange after first | `(1 - 0) / 1` | 100.00% | 有效；每 frozen child 生命周期内从 `N` 次分配降到 `1` 次 |
| Transform offset update | replace `RenderTransform` every arrange | reuse same `TranslateTransform`, update `X` | reference reuse verified | n/a | `--verify-datagrid-states` 覆盖 repeated arrange 和 horizontal offset change |
| Transform clear lifecycle | clear through `ClearFrozenStates()` only | clear on fully frozen branch, re-template and detach too | lifecycle tightened | n/a | `--verify-datagrid-states` 覆盖 fully frozen clear |
| `DataGrid.RowGroups` visual/root smoke | 315.0 | 315.0 | `(315 - 315) / 315` | 0.00% | smoke-only；本轮不改变 visual tree |
| `DataGrid.RowGroups` KB/item smoke | 2956.5 KB | 2956.5 KB | `(2956.5 - 2956.5) / 2956.5` | 0.00% | smoke-only；本轮收益是 horizontal-scroll/arrange 分配项 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.488 | 2964.6 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.183 | 2600.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.137 | 2596.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.883 | 3123.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.190 | 3041.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.831 | 2590.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.786 | 2956.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.746 | 12479.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.26 RowsPresenter layout list allocation

本轮优化点：`DataGridRowsPresenter.MeasureOverride()` / `ArrangeOverride()` 原先每次 layout pass 都会通过 `DisplayData.GetScrollingElements()` 构造 iterator，并复制为 `List<Control>` / `ToList()` 后再追加可选 drag indicator。这个路径覆盖 DataGrid 纵向滚动、窗口尺寸变化、ItemsSource 更新、row group 展开/折叠和 row reorder drag 的 rows presenter layout；真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，所有 realized rows 都经过该 presenter。

修复后 `DataGridDisplayData` 暴露按 display index 读取的 accessor，`DataGridRowsPresenter` 按当前 displayed element count 直接遍历 scrolling elements，并单独处理 `_dragIndicator`。保留原有语义：drag indicator 仍参与 measure/header width，measure total height 仍不计入 drag indicator，arrange final height 仍保留旧的追加高度行为。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| RowsPresenter `MeasureOverride` list allocation / layout pass | 1 iterator + 1 `List<Control>` copy | 0 iterator/list allocation | `(2 - 0) / 2` | 100.00% | 有效；displayed rows 直接 indexed 遍历 |
| RowsPresenter `ArrangeOverride` list allocation / layout pass | 1 iterator + 1 `List<Control>` copy | 0 iterator/list allocation | `(2 - 0) / 2` | 100.00% | 有效；drag indicator 仍作为独立尾项处理 |
| `DataGrid.GalleryShape` KB/item smoke | 12479.9 KB | 12476.9 KB | `(12479.9 - 12476.9) / 12479.9` | 0.02% | smoke-only；分配下降很小但方向稳定，不声明页面级 timing 收益 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.402 | 2964.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.358 | 2599.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.078 | 2596.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.056 | 3122.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.975 | 3041.2 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.863 | 2590.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.761 | 2955.4 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.897 | 12476.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.27 Cells/header presenter arrange visible-column allocation

本轮优化点：`DataGridCellsPresenter.ArrangeOverride()` 和 `DataGridColumnHeadersPresenter.ArrangeOverride()` 原先每次 arrange 都会先通过 `GetVisibleColumns()` 构造 iterator 计算右冻结边界，再通过 `GetVisibleColumns().ToList()` 构造第二个 iterator 和 `List<DataGridColumn>`，用于正向和反向遍历。这个路径覆盖每个 realized row 的 cells presenter arrange，以及 column header presenter 的 frozen/scrolling header arrange；真实 Gallery `DataGridShowCase` 声明 65 个 `DataGrid`，其中 frozen-column、row header、row details 和 grouped scenarios 都会反复触发布局。

修复后 `DataGridColumnCollection` 暴露按 display index 读取的 accessor，cells/header presenter 直接按 display index 正向/反向遍历并跳过不可见列。原有语义保持：右冻结列仍在第二段反向遍历中重新 arrange；非右冻结列的 `visibleColumnIndex` 推进规则不变；header 右冻结 shadow 判断沿用旧的 displayed column count 口径。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| CellsPresenter arrange visible-column temporaries / realized row layout pass | 2 iterators + 1 `List<DataGridColumn>` copy | 0 iterator/list allocation | `(3 - 0) / 3` | 100.00% | 有效；每个 realized row 的 arrange 热路径去掉临时集合 |
| ColumnHeadersPresenter arrange visible-column temporaries / header layout pass | 2 iterators + 1 `List<DataGridColumn>` copy | 0 iterator/list allocation | `(3 - 0) / 3` | 100.00% | 有效；header arrange 正反向遍历直接走 display index |
| `DataGrid.GalleryShape` KB/item smoke | 12476.9 KB | 12470.5 KB | `(12476.9 - 12470.5) / 12476.9` | 0.05% | smoke-only；分配继续下降，不声明稳定 timing 收益 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.124 | 2962.5 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.598 | 2598.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.516 | 2594.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.116 | 3120.8 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.486 | 3039.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.794 | 2589.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.580 | 2953.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.453 | 12470.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.28 Cells/header presenter measure visible-column allocation

本轮优化点：上一轮收敛了 cells/header presenter 的 arrange visible-column 临时集合后，`DataGridCellsPresenter.MeasureOverride()` 与 `DataGridColumnHeadersPresenter.MeasureOverride()` 仍然在正常测量和 star sizing 二次测量路径中通过 `GetVisibleColumns()` 构造 iterator。cells presenter measure 是每个 realized row 的高频路径；column header presenter measure 覆盖 header auto-size、star sizing、窗口尺寸变化和列宽变化。

修复后两个 presenter 的 measure 路径复用 `DataGridColumnCollection.GetDisplayedColumnAtDisplayIndex()`，按 display index 正向遍历并跳过不可见列。原有 sizing 语义保持：`EnsureVisibleEdgedColumnsWidth()` 前后调用位置不变；`LastVisibleColumn`、`totalDisplayWidth`、`scrollingLeftEdge`、star sizing 二次测量的 `leftEdge` 推进规则不变。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| CellsPresenter measure visible-column iterators / realized row measure pass | 2 iterator traversals | 0 iterator traversals | `(2 - 0) / 2` | 100.00% | 有效；normal + star-sizing second pass 都直接 indexed 遍历 |
| ColumnHeadersPresenter measure visible-column iterators / header measure pass | 2 iterator traversals | 0 iterator traversals | `(2 - 0) / 2` | 100.00% | 有效；header normal + star-sizing second pass 都直接 indexed 遍历 |
| `DataGrid.GalleryShape` KB/item smoke | 12470.5 KB | 12464.4 KB | `(12470.5 - 12464.4) / 12470.5` | 0.05% | smoke-only；分配继续下降，不声明稳定 timing 收益 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.307 | 2961.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.595 | 2596.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.530 | 2593.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.850 | 3118.8 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.065 | 3037.1 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.014 | 2588.6 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.703 | 2951.9 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.903 | 12464.4 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.29 Group column header presenter visible-column allocation

本轮优化点：`DataGridGroupColumnHeadersPresenter.MeasureOverride()` 和 `ArrangeOverride()` 仍然各有两段 `GetVisibleColumns()` 遍历。grouped header 场景下这些路径覆盖 leaf header 测量、star sizing 二次测量、auto-height 统计和 arrange 前的 leaf height 统计；每次调用都会构造 visible-column iterator。

修复后 group column header presenter 复用 `DataGridColumnCollection.GetDisplayedColumnAtDisplayIndex()`，按 display index 正向遍历并跳过不可见列。原有语义保持：`LastVisibleColumn`、`totalDisplayWidth`、`leftEdge` 推进、`_visibleColumnCount` 的 displayed-count 口径、frozen shadow 判断和 group recursive arrange 路径不变。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| GroupColumnHeadersPresenter measure visible-column iterators / header measure pass | 2 iterator traversals | 0 iterator traversals | `(2 - 0) / 2` | 100.00% | 有效；normal + star-sizing second pass 都直接 indexed 遍历 |
| GroupColumnHeadersPresenter arrange visible-column iterators / header arrange pass | 2 iterator traversals | 0 iterator traversals | `(2 - 0) / 2` | 100.00% | 有效；auto-height + star-sizing second pass 都直接 indexed 遍历 |
| `DataGrid.GroupHeaders` KB/item smoke | 2588.6 KB | 2588.1 KB | `(2588.6 - 2588.1) / 2588.6` | 0.02% | smoke-only；分配有轻微下降，不声明稳定 timing 收益 |
| `DataGrid.GalleryShape` KB/item smoke | 12464.4 KB | 12464.3 KB | `(12464.4 - 12464.3) / 12464.4` | 0.00% | 噪声内；不作为本轮收益 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.909 | 2961.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.510 | 2596.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.919 | 2593.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.482 | 3118.8 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.420 | 3037.1 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.993 | 2588.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.070 | 2951.9 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 46.372 | 12464.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.30 Row scrolling element iterator cleanup

本轮优化点：DataGrid row/core 仍有 6 个外部路径通过 `DisplayData.GetScrollingElements()` / `GetScrollingRows()` 构造 iterator：`EdgedRowsHeightCalculated` 高度估算、`ScrollSlotIntoView()` 横向滚动后的 displayed row 重测、插入/删除 slot 修正、`ResetDisplayedRows()` unload 通知，以及 `DataGridRowReorderHandle` 拖拽 hit-test。这些路径不是每帧渲染，但会被滚动、虚拟化窗口变化、数据插入删除、reset 和行重排交互触发。

修复后这些路径复用 `DisplayData.NumDisplayedScrollingElements` + `GetScrollingElementAtDisplayIndex()`，按当前 display order 直接遍历。原有语义保持：row-only 路径仍只处理 `DataGridRow`；reset 路径仍同时处理 `DataGridRow` 和 `DataGridRowGroupHeader`；row reorder hit-test 仍跳过 group header。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Row height estimate scrolling-element iterator | 1 iterator / getter evaluation | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；高度估算直接 indexed 遍历 |
| Horizontal scroll row remeasure iterator | 1 filtered iterator / call | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；仍只 measure realized data rows |
| Slot correction visible-row iterators | 2 filtered iterators / delete+insert correction paths | 0 iterator | `(2 - 0) / 2` | 100.00% | 有效；删除/插入修正仍跳过 group header |
| Reset displayed rows iterator | 1 iterator / reset with unload handlers | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；row unload 和 row group unload 语义保留 |
| Row reorder hit-test iterator | 1 iterator / drag hit-test | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；拖拽命中仍只返回 data row index |
| `DataGrid.GalleryShape` KB/item smoke | 12464.3 KB | 12466.3 KB | `(12464.3 - 12466.3) / 12464.3` | -0.02% | 噪声内；本轮不声明 KB/timing 收益 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.325 | 2961.5 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.434 | 2597.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.022 | 2593.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.977 | 3119.7 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.314 | 3038.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.079 | 2588.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.920 | 2952.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.962 | 12466.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.31 Column width adjustment iterator/delegate cleanup

本轮优化点：`DecreaseColumnWidths()` / `IncreaseColumnWidths()` 和 star sizing adjustment 仍在列宽调整过程中通过 `GetDisplayedColumns(predicate)` 构造 filtered iterator，并通过 `Func<DataGridColumn, double>` 传入目标宽度。这个路径覆盖窗口宽度变化、star sizing、列宽 resize、列插入删除后的宽度再分配；真实 `DataGridShowCase` 声明 210 个 columns，列宽调整会按 displayed-column 数量放大。

修复后 star / non-star adjustment 直接使用 `GetDisplayedColumnAtDisplayIndex()` 按 display index 遍历，过滤条件保持原样；目标宽度从 `Func<DataGridColumn, double>` 改成 `ColumnWidthTarget` enum。原有语义保持：left-to-right / right-to-left 顺序、`DisplayIndex >= displayIndex`、`ActualCanUserResize`、`IsInitialDesiredWidthDetermined`、star desired/min/max 限制和 star ratio 更新规则不变。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Star sizing displayed-column filtered iterators / star adjustment | 3 iterator traversals | 0 iterator traversals | `(3 - 0) / 3` | 100.00% | 有效；initial + desired + limit pass 都直接 indexed 遍历 |
| Non-star decrease displayed-column filtered iterators / decrease path | 3 iterator traversals | 0 iterator traversals | `(3 - 0) / 3` | 100.00% | 有效；desired + min initialized + min all 三段直接 indexed 遍历 |
| Non-star increase displayed-column filtered iterators / increase path | 3 iterator traversals | 0 iterator traversals | `(3 - 0) / 3` | 100.00% | 有效；desired + max initialized + max all 三段直接 indexed 遍历 |
| Column width target delegates / decrease path | 3 `Func<DataGridColumn, double>` delegates | 0 delegates | `(3 - 0) / 3` | 100.00% | 有效；目标宽度改为 enum |
| Column width target delegates / increase path | 3 `Func<DataGridColumn, double>` delegates | 0 delegates | `(3 - 0) / 3` | 100.00% | 有效；目标宽度改为 enum |
| Star sizing target delegates / star adjustment | 2 `Func<DataGridColumn, double>` delegates | 0 delegates | `(2 - 0) / 2` | 100.00% | 有效；desired 和 min/max pass 都不再分配 delegate |
| `DataGrid.GalleryShape` KB/item smoke | 12466.3 KB | 12466.1 KB | `(12466.3 - 12466.1) / 12466.3` | 0.00% | smoke-only；差值太小，不声明稳定 KB/timing 收益 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 16.290 | 2961.4 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.081 | 2595.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.114 | 2593.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.435 | 3119.7 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 11.196 | 3038.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 10.102 | 2588.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.790 | 2952.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.609 | 12466.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.32 Column state refresh iterator/list cleanup

本轮优化点：`CorrectColumnFrozenStates()` 仍通过 `GetDisplayedColumns()` 构造 displayed-column iterator；`UpdatePseudoClasses()` 仍通过 `GetVisibleColumns().ToList()` 复制 visible columns，再遍历列表设置列头首/尾/中间状态和 sort/filter 能力。这些路径覆盖列冻结数量变化、列集合变化、ItemsSource / CollectionView 变化和 header 状态刷新；真实 `DataGridShowCase` 有 210 个 columns，临时集合会按列数放大。

修复后 `CorrectColumnFrozenStates()` 按 display index 直接遍历；`UpdatePseudoClasses()` 先按 display index 计算 visible count，再按 display index 刷新 visible headers。原有语义保持：left/right frozen 分界仍使用 displayed-column count；header first/last/middle 的分支口径保持旧的 visible index 规则；`CanUserSort`、`CanUserFilter`、`IsSorterTooltipVisible` 同步仍只作用于 visible columns。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `CorrectColumnFrozenStates` displayed-column iterator / correction pass | 1 iterator traversal | 0 iterator traversal | `(1 - 0) / 1` | 100.00% | 有效；按 display index 直接计算 frozen state |
| `UpdatePseudoClasses` visible-column iterator / refresh | 1 iterator traversal | 0 iterator traversal | `(1 - 0) / 1` | 100.00% | 有效；visible count 和 header refresh 都直接 indexed 遍历 |
| `UpdatePseudoClasses` visible-column list copy / refresh | 1 `List<DataGridColumn>` copy | 0 list allocation | `(1 - 0) / 1` | 100.00% | 有效；不再复制 visible columns |
| `DataGrid.GalleryShape` KB/item smoke | 12466.1 KB | 12462.2 KB | `(12466.1 - 12462.2) / 12466.1` | 0.03% | smoke-only；单轮差值小，不声明稳定 KB/timing 收益 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.364 | 2960.6 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.721 | 2595.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.859 | 2592.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.587 | 3118.8 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.941 | 3037.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.641 | 2586.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.887 | 2951.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.602 | 12462.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.33 Horizontal column coordinate iterator cleanup

本轮优化点：水平列坐标和滚动 offset 仍有 4 个 visible-column iterator：`ComputeDisplayedColumns()` 通过 `GetVisibleFrozenColumns()` 统计 frozen columns 宽度，`GetColumnXFromIndex()` 通过 `GetVisibleColumns()` 计算列左边界，`GetNegHorizontalOffsetFromHorizontalOffset()` 和 `ScrollColumns()` 通过 `GetVisibleScrollingColumns()` 计算 scrolling column offset。这些路径覆盖横向滚动、列冻结、列宽变化、窗口宽度变化和 `ScrollIntoView` 类操作。

修复后这些路径统一使用 `DataGridColumnCollection.GetDisplayedColumnAtDisplayIndex()` 按 display index 遍历，并在循环内保留原 filter 条件。原有语义保持：frozen width 仍只统计 visible + frozen columns；column X 仍按 visible columns 累加到目标 index 前；negative horizontal offset 和 scroll column offset 仍只统计 visible + non-frozen columns。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `ComputeDisplayedColumns` frozen-column iterator / compute pass | 1 `GetVisibleFrozenColumns()` iterator | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；frozen columns 直接 indexed 遍历 |
| `GetColumnXFromIndex` visible-column iterator / call | 1 `GetVisibleColumns()` iterator | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；列左边界计算直接 indexed 遍历 |
| `GetNegHorizontalOffsetFromHorizontalOffset` scrolling-column iterator / call | 1 `GetVisibleScrollingColumns()` iterator | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；负 offset 校验直接 indexed 遍历 |
| `ScrollColumns` scrolling-column iterator / scroll operation | 1 `GetVisibleScrollingColumns()` iterator | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；新 horizontal offset 直接 indexed 遍历 |
| `DataGrid.GalleryShape` KB/item smoke | 12462.2 KB | 12460.7 KB | `(12462.2 - 12460.7) / 12462.2` | 0.01% | smoke-only；单轮差值小，不声明稳定 KB/timing 收益 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.623 | 2960.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.183 | 2594.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.322 | 2592.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.985 | 3118.5 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.024 | 3036.8 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.829 | 2586.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.740 | 2951.1 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.298 | 12460.7 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.34 Column lifecycle/edit/copy iterator cleanup

本轮优化点：DataGrid 业务路径里剩余的 `GetDisplayedColumns()` / `GetVisibleColumns()` 调用点集中在自动列宽结束、ItemsSource 换绑后的 header 初始化、列宽属性变更、编辑元素生成、star width coerce、列 resize、剪贴板复制和 column header hit-test。这些路径不是每帧渲染路径，但会在列数量较大的真实 `DataGridShowCase`（210 个 column declarations）里按列数放大 iterator / predicate 分配。

修复后这些路径统一使用 `DataGridColumnCollection.GetDisplayedColumnAtDisplayIndex()` 按 display index 直接遍历，并在循环内保留旧的 `IsVisible` / `IsReadOnly` / `Width.DesiredValue` 过滤条件。原有语义保持：显示顺序不变；hidden/read-only 列仍被跳过；clipboard header 和 item 内容仍只复制 visible columns；column header hit-test 仍用 translated header bounds 判断命中。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| DataGrid business column iterator callsites / batch | 11 `GetDisplayedColumns()` / `GetVisibleColumns()` callsites | 0 callsites | `(11 - 0) / 11` | 100.00% | 有效；业务调用点直接 indexed 遍历 |
| Auto-sizing visible-column iterator / finish pass | 1 iterator | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；只处理 visible columns |
| Header init + width property displayed-column iterators | 4 iterators | 0 iterators | `(4 - 0) / 4` | 100.00% | 有效；ItemsSource header 初始化、ColumnWidth、Min/MaxColumnWidth 变更直接 indexed 遍历 |
| Edit + star coerce filtered iterators / delegates | 2 filtered iterators + 2 predicate delegates | 0 | `(4 - 0) / 4` | 100.00% | 有效；过滤条件内联，避免 iterator 和 predicate delegate |
| Column resize visible-column iterator / resize | 1 iterator | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；resize 总宽和 star count 直接 indexed 遍历 |
| Clipboard copy visible-column iterators / copy operation | `1 + SelectedItems.Count` iterators | 0 iterators | `(n - 0) / n` | 100.00% | 有效；header row 和每个 selected item 都直接 indexed 遍历 |
| Column header hit-test visible-column iterator / drag hit-test | 1 iterator | 0 iterator | `(1 - 0) / 1` | 100.00% | 有效；命中判断保持 header bounds 口径 |
| `DataGrid.GalleryShape` KB/item smoke | 12460.7 KB | 12417.6 KB | `(12460.7 - 12417.6) / 12460.7` | 0.35% | smoke-only；只作异常检查，不声明稳定 KB 收益 |
| `DataGrid.GalleryShape` ms/item smoke | 43.298 ms | 46.154 ms | `(43.298 - 46.154) / 43.298` | -6.60% | smoke-only；本轮路径不覆盖初始化主视觉树，不作为速度收益或回退依据 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.948 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.715 | 2589.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.502 | 2585.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.334 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 11.926 | 3026.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.680 | 2576.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.004 | 2940.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 46.154 | 12417.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.35 Operation buttons class handler cleanup

本轮优化点：`DataGridOperationButtons.OnApplyTemplate()` 原先给 edit / save 两个 `HyperLinkTextBlock` part 注册 `Click` handler，并给 delete / cancel 两个 `PopupConfirm` part 注册 `Confirmed` handler。`HyperLinkTextBlock.ClickEvent` 和 `PopupConfirm.ConfirmedEvent` 都是 Bubble routed event，模板子控件事件可以由 `DataGridOperationButtons` owner 的 class handler 统一处理。

修复后 `DataGridOperationButtons` static constructor 注册 2 个 owner class handlers，`OnApplyTemplate()` 只保留 part lookup，不再给每个 realized operation buttons 写入 `_eventHandlers`。原有语义保持：edit 仍进入 editing 状态；save / cancel 仍退出 editing 状态；delete confirmation 仍按当前 row index 删除数据项。当前 `DataGrid.GalleryShape` smoke 不包含 operation column，因此本轮不声明页面级 timing 收益；收益适用于使用 `DataGridOperationColumn` 的表格。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Operation buttons local routed handlers / realized operation cell | 4 handlers | 0 handlers | `(4 - 0) / 4` | 100.00% | 有效；edit/save/delete/cancel 统一 owner class handler |
| Operation buttons template reapply duplicate subscriptions | 4 new subscriptions / template apply | 0 new subscriptions / template apply | `(4 - 0) / 4` | 100.00% | 正确性/生命周期收益；重套模板不再叠加 part handlers |
| Owner class handler registrations / control type | 0 | 2 static class handlers | structural change | n/a | 只按类型注册一次，不随 realized cell 数量增长 |
| `DataGrid.GalleryShape` KB/item smoke | 12417.6 KB | 12417.7 KB | `(12417.6 - 12417.7) / 12417.6` | -0.00% | smoke-only；该 smoke 不覆盖 operation column，不作为本轮收益 |
| `DataGrid.GalleryShape` ms/item smoke | 46.154 ms | 47.812 ms | `(46.154 - 47.812) / 46.154` | -3.59% | smoke-only；该 smoke 不覆盖 operation column，不作为速度收益或回退依据 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.757 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.677 | 2588.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.294 | 2585.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 11.646 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 12.151 | 3026.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 11.740 | 2576.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 12.478 | 2940.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 47.812 | 12417.7 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.36 Data connection enumerable count cleanup

本轮优化点：`DataGridDataConnection.TryGetCount()` 在 `DataSource` 不是 `ICollection`、但仍是 `IEnumerable` 时，通过 `enumerable.Cast<object>().Count()` 或 `enumerable.Cast<object>().Any()` 做 fallback。这个路径覆盖非集合型 ItemsSource、lazy enumerable、custom enumerable 等 data layer 场景；`RowsPresenter.MeasureOverride()` 也会走 `TryGetCount(false, true, out count)` 探测是否有数据。

修复后 fallback 直接使用 raw `IEnumerator`：slow count 路径完整遍历并保留 `checked` 溢出语义；getAny 路径只调用一次 `MoveNext()`；两条路径都会 dispose enumerator。当前标准 `DataGrid.GalleryShape` smoke 多数走 `ICollection` 快路径，因此本轮不声明页面级 timing / KB 收益，主收益是非集合 ItemsSource 的结构分配减少。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Non-collection count fallback LINQ iterators | 1 `Cast<object>()` iterator / count | 0 LINQ iterators | `(1 - 0) / 1` | 100.00% | 有效；raw `IEnumerator` 直接计数 |
| Non-collection getAny fallback LINQ iterators | 1 `Cast<object>()` iterator / any | 0 LINQ iterators | `(1 - 0) / 1` | 100.00% | 有效；只调用一次 `MoveNext()` |
| Non-collection getAny enumeration length | up to first item via LINQ `Any()` | first item via raw `IEnumerator` | equivalent | 0.00% | 语义保持；验证 MoveNext count = 1 |
| Enumerator disposal / fallback path | LINQ iterator owns disposal | explicit dispose | behavior preserved | n/a | 正确性验证；count/getAny 都 dispose raw enumerator |
| `DataGrid.GalleryShape` KB/item smoke | 12417.7 KB | 12417.7 KB | `(12417.7 - 12417.7) / 12417.7` | 0.00% | smoke-only；标准场景不覆盖非集合 fallback |
| `DataGrid.GalleryShape` ms/item smoke | 47.812 ms | 45.405 ms | `(47.812 - 45.405) / 47.812` | 5.03% | smoke-only；不作为本轮速度收益证明 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.914 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.641 | 2588.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.695 | 2585.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.320 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.440 | 3026.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.621 | 2576.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.226 | 2940.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 45.405 | 12417.7 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.37 Filter flyout presenter button class handler cleanup

本轮优化点：`DataGridMenuFilterFlyoutPresenter` 和 `DataGridTreeFilterFlyoutPresenter` 在 materialized popup content 的 `OnApplyTemplate()` 中给 reset / ok 两个 `Button` part 注册本地 `Click` handler。filter popup 首次打开后会创建 presenter；重套模板也会重复执行 part lookup 和事件订阅路径。

修复后两个 presenter 都在 static constructor 注册 `Button.ClickEvent` class handler，`OnApplyTemplate()` 只保留 part lookup。原有语义保持：reset 仍清空叶子选中态；ok 仍设置 `IsActiveShutdown` 并关闭 flyout；状态验证覆盖 menu/tree presenter 的本地 handler 清理和 ok 行为。当前 `DataGrid.GalleryShape` smoke 是关闭态 / 初始化场景，不覆盖 filter popup 打开态，因此不声明页面级 timing 收益。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Menu filter presenter local button handlers / materialized presenter | 2 Click handlers | 0 local handlers | `(2 - 0) / 2` | 100.00% | 有效；reset / ok 统一 presenter class handler |
| Tree filter presenter local button handlers / materialized presenter | 2 Click handlers | 0 local handlers | `(2 - 0) / 2` | 100.00% | 有效；reset / ok 统一 presenter class handler |
| Filter presenter template reapply button subscriptions | 2 subscriptions / apply | 0 subscriptions / apply | `(2 - 0) / 2` | 100.00% | 生命周期收益；重套模板不再叠加 part handlers |
| Owner class handler registrations / presenter type | 0 | 1 static class handler | structural change | n/a | 每个 presenter 类型只注册一次 |
| `DataGrid.GalleryShape` KB/item smoke | 12417.7 KB | 12417.4 KB | `(12417.7 - 12417.4) / 12417.7` | 0.00% | smoke-only；关闭态场景不覆盖 popup presenter |
| `DataGrid.GalleryShape` ms/item smoke | 45.405 ms | 44.502 ms | `(45.405 - 44.502) / 45.405` | 1.99% | smoke-only；不作为本轮速度收益证明 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变关闭态 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.740 | 2950.8 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.765 | 2588.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.989 | 2585.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.627 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.066 | 3026.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.319 | 2576.0 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.289 | 2940.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.502 | 12417.4 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.38 Selection column header checkbox class handler cleanup

本轮优化点：`DataGridSelectionColumn.CreateHeader()` 在 Extended selection mode 下为表头全选 `CheckBox` 注册一个本地 `Click` handler。选择列一般每个表格最多 1 个表头实例，但该 handler 仍会写入控件实例的 routed event handler 表；重建 header 时也会重复走订阅路径。

Avalonia 12 依据：`Button.ClickEvent` 是 Bubble routed event（`.referenceprojects/Avalonia/src/Avalonia.Controls/Button.cs:82-83`）；`ToggleButton.OnClick()` 先执行 `Toggle()` 再调用 `base.OnClick()` 抛出 click（`.referenceprojects/Avalonia/src/Avalonia.Controls/Primitives/ToggleButton.cs:73-81`）。因此将表头 checkbox 改为专用 `SelectionHeaderCheckBox` 并注册一次 class handler，可以保持原先“点击后读取新 `IsChecked` 值”的语义。

修复后 `DataGridSelectionColumn` 不再向 header checkbox 注册本地 `Click +=`。`SelectionHeaderCheckBox` 通过 static class handler 分发到所属 column：`IsChecked == true` 仍执行 `SelectAll()`，`IsChecked == false` 仍执行 `ClearRowSelection(true)`。状态验证覆盖本地 handler 清理、全选和清空选择。当前标准 `DataGrid.GalleryShape` smoke 不包含 selection column，因此本轮不声明页面级 timing 收益。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Selection header checkbox local routed handlers / realized selection column header | 1 Click handler | 0 local handlers | `(1 - 0) / 1` | 100.00% | 有效；header checkbox click 迁到 class handler |
| Selection header creation subscriptions | 1 subscription / header creation | 0 subscriptions / header creation | `(1 - 0) / 1` | 100.00% | 生命周期收益；重建 header 不再写入本地 handler 表 |
| Owner class handler registrations / checkbox type | 0 | 1 static class handler | structural change | n/a | 每个 `SelectionHeaderCheckBox` 类型只注册一次 |
| Select all / clear selection behavior | local handler | class handler | behavior preserved | n/a | 正确性验证通过；4 行全选后 selected count = 4，清空后 = 0 |
| `DataGrid.GalleryShape` KB/item smoke | 12417.4 KB | 12417.8 KB | `(12417.4 - 12417.8) / 12417.4` | -0.00% | smoke-only；该 smoke 不覆盖 selection column，不作为本轮收益 |
| `DataGrid.GalleryShape` ms/item smoke | 44.502 ms | 46.659 ms | `(44.502 - 46.659) / 44.502` | -4.85% | smoke-only；不作为速度收益或回退依据 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变标准 GalleryShape 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 16.675 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.803 | 2588.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.899 | 2585.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.453 | 3109.1 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.863 | 3026.2 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.557 | 2576.8 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.672 | 2940.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 46.659 | 12417.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

复测期间 `DataGrid.GalleryShape` ms/item 出现 45.000 / 53.496 / 46.659 的波动，本轮不使用该 timing 计算收益。

### 2.39 Filter request value copy cleanup

本轮优化点：`DataGridColumnHeader.HandleFilterRequest()` 处理 filter popup 选中值时，通过 `args.FilterValues.Cast<object>().ToList()` 将 `List<string>` 转为 `List<object>`。这个路径发生在用户确认 filter request 时；当前标准关闭态 smoke 不触发该交互，但 filter popup 打开并确认时会走该转换。

修复后改为 `CopyFilterValues()` 手写拷贝，保留 `List<object>` 快照语义，同时去掉 LINQ `Cast<object>()` iterator。状态验证覆盖 deferred filter processing：触发 request 后立即修改原始 `List<string>`，最终 `FilterConditions` 仍保持请求时的 `"Joe"`，证明没有把原集合引用传入延迟处理。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Filter request LINQ cast iterators / request | 1 `Cast<object>()` iterator | 0 LINQ iterators | `(1 - 0) / 1` | 100.00% | 有效；手写拷贝避免 LINQ iterator |
| Filter request snapshot lists / request | 1 `List<object>` | 1 `List<object>` | unchanged | 0.00% | 语义保持；仍给 deferred processing 独立快照 |
| Filter request late source mutation | copied by `ToList()` | copied by `CopyFilterValues()` | behavior preserved | n/a | 正确性验证通过；late mutation 不影响 `FilterConditions` |
| `DataGrid.GalleryShape` KB/item smoke | 12417.8 KB | 12417.6 KB | `(12417.8 - 12417.6) / 12417.8` | 0.00% | smoke-only；关闭态场景不触发 filter request |
| `DataGrid.GalleryShape` ms/item smoke | 46.659 ms | 44.823 ms | `(46.659 - 44.823) / 46.659` | 3.93% | smoke-only；不作为本轮速度收益证明 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 closed-state visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 16.594 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.785 | 2588.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.293 | 2585.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.648 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.771 | 3026.2 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.436 | 2576.6 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.428 | 2940.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.823 | 12417.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.40 Filter condition set-compare cleanup

本轮优化点：已有 filter 时，`DataGridColumnHeader.ProcessFilter()` 通过 `filter.FilterConditions.ToHashSet()` 和 `filterValues.ToHashSet()` 比较新旧 filter values，再用 `filterValues.ToList()` 创建 replacement filter 的条件快照。这个路径只在已有 filter 后再次提交 filter request 时触发。

修复后用 `FilterConditionsSetEquals()` 做无分配的集合等价检查，保持旧行为：重复值按 set 语义忽略，空 filter values 仍移除 filter；replacement filter 仍创建独立 `List<object>` 快照。状态验证覆盖：重复 `"Joe"` 与已有 `"Joe"` set-equal 时不替换 filter description；replacement request 后修改原始列表不影响最终 `FilterConditions`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Existing filter request set-compare hash sets / request | 2 `HashSet<object>` | 0 hash sets | `(2 - 0) / 2` | 100.00% | 有效；手写 set-equivalence 循环 |
| Replacement filter LINQ list copy callsites / request | 1 `ToList()` | 0 LINQ `ToList()` | `(1 - 0) / 1` | 100.00% | 有效；改为手写 `CopyFilterValues(List<object>)` |
| Replacement filter snapshot lists / request | 1 `List<object>` | 1 `List<object>` | unchanged | 0.00% | 语义保持；仍给 filter description 独立快照 |
| Duplicate filter values set semantics | `HashSet.SetEquals` | `FilterConditionsSetEquals` | behavior preserved | n/a | 正确性验证通过；重复 `"Joe"` 不触发替换 |
| `DataGrid.GalleryShape` KB/item smoke | 12417.6 KB | 12417.6 KB | `(12417.6 - 12417.6) / 12417.6` | 0.00% | smoke-only；关闭态场景不触发 replacement filter request |
| `DataGrid.GalleryShape` ms/item smoke | 44.823 ms | 40.957 ms | `(44.823 - 40.957) / 44.823` | 8.63% | smoke-only；不作为本轮速度收益证明 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 closed-state visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.260 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.793 | 2589.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.531 | 2585.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.961 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.244 | 3026.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.065 | 2576.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.723 | 2940.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 40.957 | 12417.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.41 MergedComparer comparer array cleanup

本轮优化点：`DataGridCollectionView.MergedComparer` 每次根据 `SortDescriptions` 构造比较器数组时使用 `coll.Select(c => c.Comparer).ToArray()`，会为排序比较器构造路径额外创建 LINQ iterator。该路径在已排序数据插入 / 分组等需要 `MergedComparer` 的场景触发。

修复后改为按 `SortDescriptions.Count` 创建数组并用索引填充，保留唯一必要的 comparer array 存储。状态验证覆盖：两个 sort descriptions 下 `_comparers` 长度保持为 2，比较顺序仍先看第一 comparer，第一 comparer 相等时再落到第二 comparer，全部相等时返回 0。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| MergedComparer LINQ iterators / construction | 1 `Select` iterator | 0 LINQ iterators | `(1 - 0) / 1` | 100.00% | 有效；手写数组填充去掉 iterator |
| MergedComparer comparer arrays / construction | 1 array | 1 array | unchanged | 0.00% | 必要存储保持，不改变比较器生命周期 |
| Sort comparer ordering behavior | LINQ-built array | manual-built array | behavior preserved | n/a | 正确性验证通过；first comparer / fallback comparer 顺序保持 |
| `DataGrid.GalleryShape` KB/item smoke | 12417.6 KB | 12417.6 KB | `(12417.6 - 12417.6) / 12417.6` | 0.00% | smoke-only；主 GalleryShape 不触发排序构造热点 |
| `DataGrid.GalleryShape` ms/item smoke | 40.957 ms | 43.691 ms | `(40.957 - 43.691) / 40.957` | -6.68% | smoke-only；单次 timing 变差不作为本轮收益或回归结论 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 closed-state visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.525 | 2950.8 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.420 | 2588.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.559 | 2585.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.316 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.259 | 3026.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.456 | 2576.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.705 | 2940.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.691 | 12417.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.42 CollectionView source list preallocation

本轮优化点：`DataGridCollectionView.CopySourceToInternalList()` 和无 filter 的 `PrepareLocalArray()` 对 `ICollection` 源集合也从空 `List<object>` 开始逐项 `Add`，构造 / refresh 时会产生 List 扩容。该路径覆盖 DataGrid 初次绑定 ItemsSource、无过滤刷新、取消排序/过滤后回到源集合复制；真实 Gallery 场景的 DataGrid 数据源是 `ObservableCollection<PerfDataGridRow>`，属于可直接读取 Count 的集合。

修复后新增 `CreateListForSource()`，对 `ICollection` 源集合按 `Count` 预分配；有 filter 时仍使用空 List，避免过滤后只保留少量数据却预分配全量源容量。状态验证覆盖：8 项源集合构造后 `Count/Capacity = 8/8`，刷新到 9 项后 `Count/Capacity = 9/9`，filter 后只保留 `[0, 1]` 且 capacity 小于源 Count。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 8-item unfiltered source copy List growth reallocations | 2 growths | 0 growths | `(2 - 0) / 2` | 100.00% | 有效；构造时按源 Count 预分配 |
| 9-item unfiltered refresh final List capacity | 16 slots | 9 slots | `(16 - 9) / 16` | 43.75% | 有效；状态验证覆盖 Count/Capacity |
| Filtered refresh source-count preallocation | possible full source capacity | avoided | behavior guarded | n/a | 正确性验证通过；filter 后顺序和值保持 |
| `DataGrid.GalleryShape` KB/item smoke | 12417.6 KB | 12416.8 KB | `(12417.6 - 12416.8) / 12417.6` | 0.01% | smoke-only；内存轻微下降但不作为单独速度证明 |
| `DataGrid.GalleryShape` ms/item smoke | 43.691 ms | 44.868 ms | `(43.691 - 44.868) / 43.691` | -2.69% | smoke-only；单次 timing 变差不作为本轮回归结论 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.658 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.503 | 2589.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.183 | 2585.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.384 | 3108.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.429 | 3026.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.663 | 2576.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.190 | 2940.2 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.868 | 12416.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.43 Validation exception filtering cleanup

本轮优化点：`ValidationUtils.UnpackException()` 对任意非空异常都会构造 `Where(...)` iterator 并 `ToList()`；单个普通异常还会先创建 singleton array，再复制到 list。`AddExceptionIfNew()` 通过 `Any(...)` predicate 判断重复 message。该路径不是 Gallery closed-state 主路径，但属于 DataGrid validation / data error 工具路径，异常聚合和去重时会被放大。

修复后 `UnpackException()` 改为手写分支：`null` / `BindingChainException` 直接返回 `Array.Empty<Exception>()`，单个普通异常直接返回 singleton array，`AggregateException` 手写过滤并仅在存在非 binding 异常时创建 list；`AddExceptionIfNew()` 改为手写循环，移除 LINQ predicate。状态验证覆盖 null、单个普通异常、单个 `BindingChainException`、aggregate 混合过滤顺序、aggregate 全 binding 为空、相同 message 不重复添加、不同 message 正常追加。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `UnpackException(single non-binding)` LINQ iterators / call | 1 `Where` iterator | 0 LINQ iterators | `(1 - 0) / 1` | 100.00% | 有效；直接返回 singleton array |
| `UnpackException(single non-binding)` list copies / call | 1 `List<Exception>` | 0 lists | `(1 - 0) / 1` | 100.00% | 有效；不再复制单异常 |
| `UnpackException(all binding aggregate)` lists / call | 1 empty list | 0 lists | `(1 - 0) / 1` | 100.00% | 有效；全过滤时返回 shared empty |
| `AddExceptionIfNew` LINQ predicate delegates / call | 1 predicate | 0 predicates | `(1 - 0) / 1` | 100.00% | 有效；手写 message 比较循环 |
| Validation filtering behavior | LINQ filtered list | manual filtered sequence | behavior preserved | n/a | 正确性验证通过；过滤和顺序保持 |
| `DataGrid.GalleryShape` KB/item smoke | 12416.8 KB | 12417.0 KB | `(12416.8 - 12417.0) / 12416.8` | -0.00% | smoke-only；主 GalleryShape 不触发 validation 异常路径 |
| `DataGrid.GalleryShape` ms/item smoke | 44.868 ms | 43.082 ms | `(44.868 - 43.082) / 44.868` | 3.98% | smoke-only；不作为本轮速度收益证明 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.445 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.756 | 2588.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.310 | 2585.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.744 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.838 | 3026.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.926 | 2576.6 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.531 | 2940.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.082 | 12417.0 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.44 CheckBox column pointer edit bounds wait cleanup

本轮优化点：`DataGridCheckBoxColumn.PrepareCellForEdit()` 在 pointer-triggered edit 时，如果新 editing checkbox 还没有非零 `Bounds`，原逻辑通过 `LayoutUpdated +=` 等待下一次布局完成后再用原始 pointer 位置判断是否需要 toggle。该路径只在用户点击 checkbox cell 进入编辑态且 editing element 首帧 bounds 仍为 0 时触发；标准 closed-state smoke 不覆盖该交互。

修复后改为一次性观察 `Visual.BoundsProperty`，首次拿到非零 bounds 后释放订阅，并把 hit-test / toggle 投递到 dispatcher 下一轮执行。这样避免挂到 root `LayoutManager.LayoutUpdated` 的全局 layout-pass 广播，同时保留“布局稳定后再用 pointer 坐标判断”的语义。状态验证覆盖：初始 zero bounds 不立即 toggle、不留下 `LayoutUpdated` 订阅；加入 visual tree 并完成 layout 后 bounds 非零、pointer 落点仍在 checkbox 内、`IsChecked` 从 `false` 切换到 `true`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Pointer edit `LayoutUpdated` subscriptions / zero-bounds checkbox edit | 1 handler | 0 handlers | `(1 - 0) / 1` | 100.00% | 有效；不再挂 root layout updated 广播 |
| Layout-pass callbacks while waiting for first non-zero bounds | 1 callback per layout pass | 0 `LayoutUpdated` callbacks | `(1 - 0) / 1` | 100.00% | 结构收益；改为 Bounds 变化一次性触发 |
| Bounds observers / zero-bounds checkbox edit | 0 | 1 one-shot observer | structural replacement | n/a | 只等目标 editing checkbox 的 Bounds 变化，触发后释放 |
| Pointer-triggered checkbox toggle behavior | LayoutUpdated 后 toggle | Bounds 非零后 dispatcher toggle | behavior preserved | n/a | 正确性验证通过；zero bounds 延迟，layout 后切换为 checked |
| `DataGrid.GalleryShape` KB/item smoke | 12417.0 KB | 12417.2 KB | `(12417.0 - 12417.2) / 12417.0` | -0.00% | smoke-only；标准 GalleryShape 不触发 checkbox edit |
| `DataGrid.GalleryShape` ms/item smoke | 43.082 ms | 41.435 ms | `(43.082 - 41.435) / 43.082` | 3.82% | smoke-only；不作为本轮速度收益证明 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 closed-state visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.315 | 2950.6 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.549 | 2589.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.155 | 2585.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.427 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.639 | 3026.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.688 | 2576.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.470 | 2940.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 41.435 | 12417.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.45 CollectionView sorted list materialization preallocation

本轮优化点：`DataGridCollectionView.SortList()` 的排序链路仍保留 `OrderBy/ThenBy`，但最后通过 `seq.ToList()` materialize sorted result。该路径在 `SortDescriptions.Count > 0` 且数据需要本地排序时触发，覆盖用户点击列排序、程序设置 `SortDescriptions`、排序后 refresh / filter refresh 的 sorted local array 重建。

修复后新增 `MaterializeSortedList(seq, list.Count)`，按输入 list count 预分配 sorted result，再 foreach 同一个 `IOrderedEnumerable<object>` 填充结果。排序 comparer 链、`OrderBy/ThenBy` 稳定排序语义和多 key 优先级不变。状态验证覆盖 9 项 source：sorted internal list 的 `Count/Capacity = 9/9`，并验证按 `Group` 排序后同 key 内 `Order` 仍保持原输入顺序 `0,3,6 / 1,4,7 / 2,5,8`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Sorted result LINQ `ToList()` callsites / sort refresh | 1 callsite | 0 callsites | `(1 - 0) / 1` | 100.00% | 有效；排序结果 materialization 改为手写填充 |
| 9-item sorted result final List capacity | >= 9, implementation-dependent | 9 slots | verified exact capacity | n/a | 有效；状态验证覆盖 `Count/Capacity = 9/9` |
| Stable ordering within equal sort keys | LINQ stable sort + `ToList()` | same sorted sequence + manual materialize | behavior preserved | n/a | 正确性验证通过；同 key 内顺序保持 |
| `DataGrid.GalleryShape` KB/item smoke | 12417.2 KB | 12416.8 KB | `(12417.2 - 12416.8) / 12417.2` | 0.00% | smoke-only；标准 GalleryShape 不触发 sort refresh 热点 |
| `DataGrid.GalleryShape` ms/item smoke | 41.435 ms | 43.026 ms | `(41.435 - 43.026) / 41.435` | -3.84% | smoke-only；不作为本轮收益或回归结论 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 closed-state visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.701 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.907 | 2589.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.500 | 2585.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.799 | 3108.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.221 | 3026.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.719 | 2575.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.809 | 2940.2 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.026 | 12416.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.46 Path sort property comparer cache

本轮优化点：`DataGridSortDescription.FromPath()` 的 `Compare()` 在 `_propertyType` 已知后仍会每次调用 `GetComparerForType(_propertyType)`。非字符串属性会通过反射读取 `Comparer<T>.Default`，该路径会被 `MergedComparer.Compare()`、排序后增量插入和分组比较放大。原逻辑还会在 `Compare()` 路径里把用户传入的 custom path value comparer 覆盖成属性类型默认 comparer，属于正确性风险。

修复后新增 `_internalComparerType` 缓存：没有 custom comparer 时，属性 comparer 只在属性类型首次确定或类型变化时解析；有 custom comparer 时始终保留 custom comparer，并清空 typed wrapper 缓存避免旧 wrapper 复用。`OrderBy/ThenBy` stable sort chain 不改。状态验证覆盖：`Initialize(typeof(SortProbe))` 后 path sort comparer 已准备且后续比较复用同一 comparer；`MergedComparer` 使用 custom descending value comparer 时，`Group=2` 正确排在 `Group=1` 前。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Path sort property comparer resolution / steady-state `Compare()` | 1 `GetComparerForType(...)` call | 0 calls after type cache hit | `(1 - 0) / 1` | 100.00% | 有效；比较热点不再重复解析属性 comparer |
| Path sort comparer cache per property type | not retained for newly initialized path type | retained in `_internalComparer` + `_internalComparerType` | one cached comparer per property type | n/a | 有效；状态验证确认初始化后已缓存且比较后引用不变 |
| Custom path value comparer in `MergedComparer` | can be overwritten by default property comparer | preserved | correctness restored | n/a | 正确性修复；custom descending comparer 行为已验证 |
| `DataGrid.GalleryShape` KB/item smoke | 12416.8 KB | 12414.5 KB | `(12416.8 - 12414.5) / 12416.8` | 0.02% | smoke-only；标准 GalleryShape 不稳定触发 path sort compare 热点 |
| `DataGrid.GalleryShape` ms/item smoke | 43.026 ms | 42.244 ms | `(43.026 - 42.244) / 43.026` | 1.82% | smoke-only；不作为本轮速度收益证明 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 closed-state visual/logical 结构 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.445 | 2950.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.532 | 2588.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.240 | 2584.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.628 | 3108.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.176 | 3025.1 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.703 | 2576.5 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.734 | 2939.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.244 | 12414.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.47 FilterDescription property type cache

本轮优化点：`DataGridFilterDescription.FilterBy()` 每次过滤 item 时会通过 `GetValue(record)` 解析 `PropertyPath` 的属性类型；在 `DataGridDefaultFilter` 中，这条路径会按 `items × filter descriptions × filter conditions` 放大。`PropertyPath` 是过滤描述自身的状态，对同一 item runtime type 可以缓存属性类型；当 `PropertyPath` 变更时需要清空缓存，避免 stale metadata。

修复后 `PropertyPath` 改为带 backing field 的属性，变更时清空 `_propertyOwnerType` / `_propertyType`；`GetPropertyType()` 只在 item runtime type 变化时重新调用 `GetNestedPropertyType(PropertyPath)`。过滤逻辑、`FilterConditions` 遍历顺序、自定义 `Filter` 委托和默认 `ToString().Contains(...)` 语义不变。状态验证覆盖：首次过滤后缓存 owner type 和 property type；同 owner type 再次过滤复用缓存；`PropertyPath` 从 `Name` 切到 `Score` 后重新缓存为 `int`，非字符串值仍按原逻辑通过 `ToString()` 过滤。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `GetNestedPropertyType(PropertyPath)` calls / steady-state filter value lookup for same item type | 1 call | 0 calls after type cache hit | `(1 - 0) / 1` | 100.00% | 有效；过滤热点不再重复解析属性类型 |
| PropertyPath change cache invalidation | implicit no cache | explicit owner/property type cache reset | correctness guarded | n/a | 正确性验证通过；切换 path 后重新缓存为新属性类型 |
| Default filter string semantics | `ToString().Contains(...)` | unchanged | behavior preserved | n/a | 正确性验证通过；string 与 non-string path 都覆盖 |
| `DataGrid.GalleryShape` KB/item smoke | 12414.5 KB | 12414.4 KB | `(12414.5 - 12414.4) / 12414.5` | 0.00% | smoke-only；closed-state Gallery 不稳定触发过滤热点 |
| `DataGrid.GalleryShape` ms/item smoke | 42.244 ms | 42.518 ms | `(42.244 - 42.518) / 42.244` | -0.65% | smoke-only；复测存在 outlier，不作为本轮收益或回归结论 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 closed-state visual/logical 结构 |

当前工作区 smoke（第二次复测；`DataGrid.Basic` 本轮出现 outlier，不用于收益判断）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 21.320 | 2950.4 | 305.0 | 1.0 | smoke 通过；timing outlier |
| DataGrid.Filter.Menu.Closed | 9.819 | 2588.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.659 | 2585.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.560 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.976 | 3025.1 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.782 | 2576.4 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.116 | 2939.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.518 | 12414.4 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.48 FilterDescription per-record value reuse

本轮优化点：上一轮缓存了 `DataGridFilterDescription` 的 property type，但默认过滤在多 `FilterConditions` 下仍会每个 condition 调一次 `GetValue(record)`，并重复执行 `value.ToString()`。在 `DataGridDefaultFilter` 中，这条路径会按 `items × filter descriptions × filter conditions` 放大；filter popup 选择多个值后最容易触发。

修复后 `FilterBy(record)` 先处理空 conditions，再对当前 record 只调用一次 `GetValue(record)`；默认字符串过滤只执行一次 `ToString()`，然后遍历所有 conditions 做 `Contains(...)`。custom `Filter` 保留原来的 first-condition return 语义：有自定义过滤器时仍只用第一个 condition 返回结果，不改成 any/all 语义。状态验证覆盖：两个默认 conditions 下后一个命中仍返回 true，属性 getter `2 -> 1`，`ToString()` `2 -> 1`；custom filter 仍只看第一个 condition，且不触发 `ToString()`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 2-condition default filter property getter calls / record | 2 calls | 1 call | `(2 - 1) / 2` | 50.00% | 有效；状态验证覆盖后一个 condition 命中 |
| 2-condition default filter `ToString()` calls / record | 2 calls | 1 call | `(2 - 1) / 2` | 50.00% | 有效；同一 record 的 string value 复用 |
| Custom filter `ToString()` calls / record | 0 calls | 0 calls | unchanged | 0.00% | 正确；custom filter 不走默认字符串路径 |
| Custom filter condition semantics | first condition returns | first condition returns | behavior preserved | n/a | 正确性验证通过；未改成 any/all |
| `DataGrid.GalleryShape` KB/item smoke | 12414.4 KB | 12417.0 KB | `(12414.4 - 12417.0) / 12414.4` | -0.02% | smoke-only；closed-state Gallery 不稳定触发过滤热点 |
| `DataGrid.GalleryShape` ms/item smoke | 42.518 ms | 44.228 ms | `(42.518 - 44.228) / 42.518` | -4.02% | smoke-only；本轮不声明 timing 收益，复测仍有波动 |
| Visual/logical tree | 1260 / 5 | 1260 / 5 | unchanged | 0.00% | 本轮不改变 closed-state visual/logical 结构 |

当前工作区 smoke（第二次复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.023 | 2950.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.258 | 2589.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.797 | 2585.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.798 | 3109.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.284 | 3026.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.829 | 2576.0 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.465 | 2940.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.228 | 12417.0 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.49 Column sort/filter description lookup LINQ cleanup

本轮优化点：`DataGridColumn.GetSortDescription()` / `GetFilterDescription()` 是列头 sort/filter 状态刷新、filter indicator 状态同步和用户交互路径会反复调用的 lookup。原实现通过 `OfType<DataGridComparerSortDescription>().FirstOrDefault(predicate)` 和 `FirstOrDefault(predicate)` 查找描述；这会在热路径构造 LINQ iterator / predicate delegate。改为直接遍历 `AvaloniaList` indexer，保持原来的 first-match 语义：

- `CustomSortComparer != null` 时仍只匹配 `DataGridComparerSortDescription.SourceComparer`，并忽略 path sort。
- path sort / filter 仍匹配第一个 `HasPropertyPath && PropertyPath == column path` 的描述。
- 未命中仍返回 `null`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Custom comparer sort lookup LINQ callsites | 2 (`OfType` + `FirstOrDefault(predicate)`) | 0 | `(2 - 0) / 2` | 100.00% | 有效；first matching comparer sort 已验证 |
| Path sort lookup LINQ callsites | 1 (`FirstOrDefault(predicate)`) | 0 | `(1 - 0) / 1` | 100.00% | 有效；first matching path sort 已验证 |
| Filter lookup LINQ callsites | 1 (`FirstOrDefault(predicate)`) | 0 | `(1 - 0) / 1` | 100.00% | 有效；first matching filter / no-match null 已验证 |
| Lookup behavior | first-match / no-match null | first-match / no-match null | behavior preserved | n/a | 正确性验证通过 |

Smoke-only 对比上一轮文档中的同参数复测；本轮不声明 timing 收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.023 | 13.608 | `(14.023 - 13.608) / 14.023` | 2.96% | smoke-only；无明显异常 |
| DataGrid.Filter.Menu.Closed | 10.258 | 9.424 | `(10.258 - 9.424) / 10.258` | 8.13% | smoke-only；无明显异常 |
| DataGrid.Filter.Tree.Closed | 9.797 | 8.489 | `(9.797 - 8.489) / 9.797` | 13.35% | smoke-only；无明显异常 |
| DataGrid.RowHeaders | 9.798 | 9.718 | `(9.798 - 9.718) / 9.798` | 0.82% | smoke-only；无明显异常 |
| DataGrid.RowDetails.Collapsed | 10.284 | 10.108 | `(10.284 - 10.108) / 10.284` | 1.71% | smoke-only；首次样本异常偏高后已复测 |
| DataGrid.GroupHeaders | 9.829 | 9.064 | `(9.829 - 9.064) / 9.829` | 7.78% | smoke-only；无明显异常 |
| DataGrid.RowGroups | 10.465 | 10.036 | `(10.465 - 10.036) / 10.465` | 4.10% | smoke-only；无明显异常 |
| DataGrid.GalleryShape | 44.228 | 43.188 | `(44.228 - 43.188) / 44.228` | 2.35% | smoke-only；本轮结构优化不以该 timing 定性 |

当前工作区 smoke（第二次复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.608 | 2949.5 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.424 | 2588.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.489 | 2585.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.718 | 3108.4 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.108 | 3025.8 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.064 | 2575.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.036 | 2939.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.188 | 12414.4 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.50 RowReorderColumn duplicate check LINQ cleanup

本轮优化点：`DataGridRowReorderColumn.EnsureOnlyOneReorderColumn()` 原先用 `Columns.Count(column => column is DataGridRowReorderColumn)` 检查重复 row reorder column。这个检查发生在 reorder column attached / columns collection changed 路径；真实页面一般只允许一个 reorder column，但添加列或 collection reset 时仍会触发。改为按 index 遍历 `Columns`，并在发现第二个 `DataGridRowReorderColumn` 后立即抛出原异常。

状态验证覆盖：单个 reorder column 可添加；添加第二个 reorder column 仍抛出 `Only one DataGridRowReorderColumn is allowed.`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Duplicate check LINQ predicate callsites | 1 `Count(predicate)` | 0 | `(1 - 0) / 1` | 100.00% | 有效；predicate delegate / LINQ count path 移除 |
| Duplicate check scan behavior on invalid duplicate | scans all columns | exits when second reorder column is found | early-exit | n/a | 有效；重复列异常语义已验证 |
| Production DataGrid predicate LINQ callsites outside sort chain | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；`rg` 已确认无剩余 predicate LINQ callsite |
| Duplicate row reorder column behavior | throws | throws | behavior preserved | n/a | 正确性验证通过 |

Smoke-only 对比上一轮文档中的同参数复测；本轮路径不在标准 closed-state Gallery 热路径上，不声明 timing 收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.608 | 14.239 | `(13.608 - 14.239) / 13.608` | -4.64% | smoke-only；不触发 row reorder duplicate check，视为噪声不定性 |
| DataGrid.Filter.Menu.Closed | 9.424 | 8.819 | `(9.424 - 8.819) / 9.424` | 6.42% | smoke-only；无明显异常 |
| DataGrid.Filter.Tree.Closed | 8.489 | 7.740 | `(8.489 - 7.740) / 8.489` | 8.82% | smoke-only；无明显异常 |
| DataGrid.RowHeaders | 9.718 | 9.277 | `(9.718 - 9.277) / 9.718` | 4.54% | smoke-only；无明显异常 |
| DataGrid.RowDetails.Collapsed | 10.108 | 9.890 | `(10.108 - 9.890) / 10.108` | 2.16% | smoke-only；无明显异常 |
| DataGrid.GroupHeaders | 9.064 | 8.591 | `(9.064 - 8.591) / 9.064` | 5.22% | smoke-only；无明显异常 |
| DataGrid.RowGroups | 10.036 | 9.544 | `(10.036 - 9.544) / 10.036` | 4.90% | smoke-only；无明显异常 |
| DataGrid.GalleryShape | 43.188 | 41.620 | `(43.188 - 41.620) / 43.188` | 3.63% | smoke-only；本轮结构优化不以该 timing 定性 |

当前工作区 smoke（第二次复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.239 | 2949.5 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.819 | 2587.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.740 | 2585.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.277 | 3108.4 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.890 | 3025.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.591 | 2575.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.544 | 2939.8 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 41.620 | 12414.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.51 Column reorder null-target drag-over notification dedup

本轮优化点：`DataGridColumnHeader.HandleMouseMoveReorder()` 原先的条件表达式等价于“只要 `targetColumn == null` 就通知”，因此列拖拽移出可重排目标区域后，连续 pointer move 会持续创建 `DataGridColumnDraggingOverEventArgs` 并触发 `DataGrid.ColumnDraggingOver`。修复后只在 drag-over target 发生变化时通知；从某列变为 `null` 会通知一次，后续仍为 `null` 的 pointer move 不再重复通知。

状态验证覆盖：构造列重排中的 header 状态，把当前 target 从列切到 `null`，连续调用两次 reorder move，只允许一次 `DraggingOverColumn == null` 通知。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Repeated null-target drag-over notifications / 2 consecutive pointer moves | 2 | 1 | `(2 - 1) / 2` | 50.00% | 有效；target 从非 null 变为 null 时仍通知一次，后续相同 null target 不再重复通知 |
| Repeated null-target `DataGridColumnDraggingOverEventArgs` allocations / 2 consecutive pointer moves | 2 | 1 | `(2 - 1) / 2` | 50.00% | 有效；事件参数创建跟随通知次数下降 |
| Same target notification behavior | notifies every null-target move | notifies only on target change | target-change gate | n/a | 正确性更一致；non-null frozen target 过滤语义不变 |
| Visual/logical tree | unchanged | unchanged | no tree change | n/a | 本轮不影响页面初始结构 |

Smoke-only 对比上一轮文档中的同参数复测；本轮路径只在列拖拽重排交互中触发，不声明页面加载 timing 收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.239 | 14.573 | `(14.239 - 14.573) / 14.239` | -2.35% | smoke-only；不触发列拖拽路径，不作为回归结论 |
| DataGrid.Filter.Menu.Closed | 8.819 | 9.264 | `(8.819 - 9.264) / 8.819` | -5.05% | smoke-only；不触发列拖拽路径，不作为回归结论 |
| DataGrid.Filter.Tree.Closed | 7.740 | 8.699 | `(7.740 - 8.699) / 7.740` | -12.39% | smoke-only；不触发列拖拽路径，不作为回归结论 |
| DataGrid.RowHeaders | 9.277 | 10.899 | `(9.277 - 10.899) / 9.277` | -17.48% | smoke-only；本轮复测 outlier，不作为回归结论 |
| DataGrid.RowDetails.Collapsed | 9.890 | 9.745 | `(9.890 - 9.745) / 9.890` | 1.47% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GroupHeaders | 8.591 | 8.638 | `(8.591 - 8.638) / 8.591` | -0.55% | smoke-only；不触发列拖拽路径，不作为回归结论 |
| DataGrid.RowGroups | 9.544 | 9.402 | `(9.544 - 9.402) / 9.544` | 1.49% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GalleryShape | 41.620 | 42.685 | `(41.620 - 42.685) / 41.620` | -2.56% | smoke-only；标准 GalleryShape 不触发列拖拽路径 |

当前工作区 smoke（第三次复测；第二次 `GalleryShape=49.667` 为明显 outlier，未用于收益判断）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.573 | 2949.6 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.264 | 2587.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.699 | 2585.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.899 | 3108.4 | 336.0 | 1.0 | smoke 通过；timing outlier，不定性 |
| DataGrid.RowDetails.Collapsed | 9.745 | 3025.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.638 | 2575.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.402 | 2939.8 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.685 | 12414.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.52 Row reorder handle click no-op drop cleanup

本轮优化点：`DataGridRowReorderHandle.OnPointerReleased()` 原先没有判断是否已经进入拖拽。用户只是点击 row reorder handle、没有移动到拖拽状态时，也会执行 `NotifyRowReordered()`、`RowsPresenter.NotifyDropped()`、禁用/恢复 indicator transition 的部分路径，并最终 `InvalidateArrange()`。修复后 `_dragRowIndex == null` 时只清理静态 pointer 状态并直接返回；只有 `HandleMouseMoveBeginReorder()` 已经建立拖拽状态后，release 才执行 drop / reorder / arrange invalidation。

状态验证覆盖：realize 一个带 `DataGridRowReorderColumn` 的 grid，对 row reorder handle 发送 pressed + released 但不发送 move，断言 `RowReordered` 不触发，`DraggedRowIndex` 仍为空，`DragRowOffset` 保持 0。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Row reorder handle click `RowReordered` events / no-drag release | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；未进入拖拽不再误发 reorder event |
| Row reorder handle click drop cleanup calls / no-drag release | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；没有 ghost row 时不再走 `NotifyDropped()` |
| Row reorder handle click rows presenter arrange invalidations / no-drag release | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；未改变 drag offset / indicator 时不再强制 arrange |
| Actual drag release path | drop + reorder + arrange invalidation | unchanged | behavior preserved | n/a | 正确拖拽路径仍保留原清理和重排流程 |

Smoke-only 对比上一轮文档中的同参数复测；本轮路径只在 row reorder handle click/release 交互中触发，不声明页面加载 timing 收益。第一轮复测 `DataGrid.GalleryShape=100.637 ms/item` 是明显机器抖动，未用于收益判断；下表使用第二轮复测：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.573 | 14.659 | `(14.573 - 14.659) / 14.573` | -0.59% | smoke-only；不触发 row reorder click 路径，不作为回归结论 |
| DataGrid.Filter.Menu.Closed | 9.264 | 9.896 | `(9.264 - 9.896) / 9.264` | -6.82% | smoke-only；不触发 row reorder click 路径，不作为回归结论 |
| DataGrid.Filter.Tree.Closed | 8.699 | 8.917 | `(8.699 - 8.917) / 8.699` | -2.51% | smoke-only；不触发 row reorder click 路径，不作为回归结论 |
| DataGrid.RowHeaders | 10.899 | 9.722 | `(10.899 - 9.722) / 10.899` | 10.80% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowDetails.Collapsed | 9.745 | 9.957 | `(9.745 - 9.957) / 9.745` | -2.18% | smoke-only；不触发 row reorder click 路径，不作为回归结论 |
| DataGrid.GroupHeaders | 8.638 | 8.765 | `(8.638 - 8.765) / 8.638` | -1.47% | smoke-only；不触发 row reorder click 路径，不作为回归结论 |
| DataGrid.RowGroups | 9.402 | 9.660 | `(9.402 - 9.660) / 9.402` | -2.74% | smoke-only；不触发 row reorder click 路径，不作为回归结论 |
| DataGrid.GalleryShape | 42.685 | 44.088 | `(42.685 - 44.088) / 42.685` | -3.29% | smoke-only；标准 GalleryShape 不触发 row reorder click 路径 |

当前工作区 smoke（第二轮复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.659 | 2949.5 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.896 | 2588.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.917 | 2584.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.722 | 3108.4 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.957 | 3025.8 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.765 | 2576.0 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.660 | 2939.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.088 | 12414.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.53 DataConnection collection-view Any IsEmpty fast path

本轮优化点：`DataGridDataConnection.Any()` 原先统一调用 `TryGetCount(false, true, out count)`。当 `DataSource` 是 `IDataGridCollectionView` 但不是 `ICollection` 时，这条 get-any fallback 会走 `IEnumerable.GetEnumerator().MoveNext()`。`IDataGridCollectionView` 本身已经暴露 `IsEmpty`，语义上就是 empty check；现在 `CollectionView != null` 时直接返回 `!CollectionView.IsEmpty`，不再枚举 collection view。

状态验证覆盖：通过反射给 `DataGridDataConnection` 注入一个 instrumented `IDataGridCollectionView`，分别验证非空 / 空 collection view 的 `Any()` 返回值正确，且每次调用只读 1 次 `IsEmpty`、`GetEnumerator()` 调用次数为 0。普通非集合 `IEnumerable` fallback 保持上一轮 raw enumerator 行为。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `IDataGridCollectionView.Any()` enumerators / call | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；collection view empty check 不再创建 enumerator |
| `IDataGridCollectionView.Any()` `MoveNext()` calls / call | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；避免触碰 view enumeration path |
| `IDataGridCollectionView.IsEmpty` reads / call | 0 | 1 | intentional direct property | n/a | 有效；改走 collection-view 明确语义 |
| Non-collection `IEnumerable` get-any fallback | raw enumerator + dispose | unchanged | behavior preserved | n/a | 正确性保持；上一轮 fallback 仍覆盖普通 enumerable |

Smoke-only 对比上一轮文档中的同参数复测；本轮收益由 targeted state verification 证明，单次 Gallery timing 只作异常检查，不声明稳定耗时收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.659 | 13.943 | `(14.659 - 13.943) / 14.659` | 4.88% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Menu.Closed | 9.896 | 9.385 | `(9.896 - 9.385) / 9.896` | 5.16% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Tree.Closed | 8.917 | 8.244 | `(8.917 - 8.244) / 8.917` | 7.55% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowHeaders | 9.722 | 9.417 | `(9.722 - 9.417) / 9.722` | 3.14% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowDetails.Collapsed | 9.957 | 9.792 | `(9.957 - 9.792) / 9.957` | 1.66% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GroupHeaders | 8.765 | 8.647 | `(8.765 - 8.647) / 8.765` | 1.35% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowGroups | 9.660 | 9.445 | `(9.660 - 9.445) / 9.660` | 2.23% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GalleryShape | 44.088 | 41.680 | `(44.088 - 41.680) / 44.088` | 5.46% | smoke-only；不作为本轮速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.943 | 2949.5 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.385 | 2587.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.244 | 2585.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.417 | 3108.4 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.792 | 3025.8 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.647 | 2575.4 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.445 | 2940.0 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 41.680 | 12414.4 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.54 DataGridColumn clipboard dead field cleanup

本轮优化点：`DataGridColumn.Privates` 里还保留了一个未使用的 `_clipboardContentBinding` 私有字段，但公开 `ClipboardContentBinding` 已经是 auto property，`DataGridBoundColumn` 也通过 base property 实现 Binding fallback / explicit override。该字段没有读写 callsite，会让每个 `DataGridColumn` 实例多保留一个无效引用槽，并导致 Release build 出现 unused-field warning。现在移除该字段。

状态验证覆盖：新增 `VerifyDataGridColumnClipboardContentBindingBehavior`，验证 `DataGridTextColumn` 未显式设置时返回 `Binding`，显式设置后返回 `ClipboardContentBinding`，清空后恢复 `Binding` fallback。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Unused `_clipboardContentBinding` fields / `DataGridColumn` instance | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；每个 column 实例少一个无效引用字段 |
| `_clipboardContentBinding` read/write callsites | 0 | 0 | unchanged | n/a | 正确性保持；字段本来没有参与运行时语义 |
| DataGrid bound column clipboard fallback behavior | Binding fallback + explicit override | unchanged | behavior preserved | n/a | 已验证；set / clear `ClipboardContentBinding` 语义不变 |
| Performance project build warnings | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；unused-field warning 已消除 |

Smoke-only 对比上一轮文档中的同参数复测；本轮是字段级结构优化，单次 timing 只作异常检查，不声明稳定耗时收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.943 | 12.423 | `(13.943 - 12.423) / 13.943` | 10.90% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Menu.Closed | 9.385 | 8.612 | `(9.385 - 8.612) / 9.385` | 8.24% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Tree.Closed | 8.244 | 7.506 | `(8.244 - 7.506) / 8.244` | 8.95% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowHeaders | 9.417 | 8.375 | `(9.417 - 8.375) / 9.417` | 11.07% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowDetails.Collapsed | 9.792 | 8.800 | `(9.792 - 8.800) / 9.792` | 10.13% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GroupHeaders | 8.647 | 7.801 | `(8.647 - 7.801) / 8.647` | 9.78% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowGroups | 9.445 | 8.429 | `(9.445 - 8.429) / 9.445` | 10.76% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GalleryShape | 41.680 | 37.371 | `(41.680 - 37.371) / 41.680` | 10.34% | smoke-only；不作为本轮速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 12.423 | 2949.5 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.612 | 2588.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.506 | 2585.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 8.375 | 3108.4 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 8.800 | 3025.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 7.801 | 2575.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 8.429 | 2939.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 37.371 | 12414.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.55 Column header no-drag drag-over cleanup guard

本轮优化点：`DataGridColumnHeader.HandleMouseLeftButtonUp()` 在普通列头点击 / 排序路径结束时也会调用 `HandleLostMouseCapture()`；原实现无条件创建 `DataGridColumnDraggingOverEventArgs(null, null)` 并触发 `ColumnDraggingOver`。这个 cleanup 只对列重排有意义，普通 click 没有 drag indicator / drag-over target，发送事件属于无效交互通知。修复后只有 `HeaderDragMode == Reorder` 或已经存在 drag-over target 时才发送 null cleanup；resize / click / sort 释放只做本地状态清理。

状态验证覆盖：对 realized column header 发送 pressed + released 但不发送 move，断言 `ColumnDraggingOver` 不触发；再通过反射模拟已进入 reorder 状态的 lost-capture cleanup，断言仍发送 1 次 `DraggedColumn=null && DraggingOverColumn=null` cleanup。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Column header click `ColumnDraggingOver(null, null)` events / no-drag release | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；普通点击 / 排序不再误发 drag-over cleanup |
| Column header click `DataGridColumnDraggingOverEventArgs` allocations / no-drag release | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；事件参数跟随无效通知一起消除 |
| Reorder lost-capture null cleanup events | 1 | 1 | behavior preserved | 0.00% | 正确性保持；真实列重排结束仍通知一次 cleanup |
| Standard page-load visual/logical tree | unchanged | unchanged | behavior preserved | n/a | 本轮不改变加载结构 |

Smoke-only 对比上一轮文档中的同参数复测；本轮路径只在列头点击 / 释放交互中触发，标准 DataGrid 页面加载不触发该路径。下表使用负向复测样本如实记录，只作异常检查，不作为本轮速度回归结论：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 12.423 | 13.920 | `(12.423 - 13.920) / 12.423` | -12.05% | smoke-only；不触发 column header click cleanup，不作为回归结论 |
| DataGrid.Filter.Menu.Closed | 8.612 | 9.017 | `(8.612 - 9.017) / 8.612` | -4.70% | smoke-only；不触发 column header click cleanup，不作为回归结论 |
| DataGrid.Filter.Tree.Closed | 7.506 | 7.769 | `(7.506 - 7.769) / 7.506` | -3.50% | smoke-only；不触发 column header click cleanup，不作为回归结论 |
| DataGrid.RowHeaders | 8.375 | 9.398 | `(8.375 - 9.398) / 8.375` | -12.21% | smoke-only；不触发 column header click cleanup，不作为回归结论 |
| DataGrid.RowDetails.Collapsed | 8.800 | 9.269 | `(8.800 - 9.269) / 8.800` | -5.33% | smoke-only；不触发 column header click cleanup，不作为回归结论 |
| DataGrid.GroupHeaders | 7.801 | 8.719 | `(7.801 - 8.719) / 7.801` | -11.77% | smoke-only；不触发 column header click cleanup，不作为回归结论 |
| DataGrid.RowGroups | 8.429 | 9.332 | `(8.429 - 9.332) / 8.429` | -10.71% | smoke-only；不触发 column header click cleanup，不作为回归结论 |
| DataGrid.GalleryShape | 37.371 | 41.256 | `(37.371 - 41.256) / 37.371` | -10.40% | smoke-only；不触发 column header click cleanup，不作为回归结论 |

当前工作区 smoke（第二轮复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.920 | 2949.5 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.017 | 2587.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.769 | 2585.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.398 | 3108.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.269 | 3025.8 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.719 | 2575.8 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.332 | 2939.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 41.256 | 12414.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.56 Filter flyout materialization allocation cleanup

本轮优化点：`DataGridFilterIndicator` 原先在构造时无条件生成 `_treeRadioCheckGroupName`，即使该 indicator 没有 filter、是 menu filter、或者 tree filter 从未打开，也会分配一个字符串并推进全局 seed。修复后 group name 延迟到 `PopulateTreeItems()` 首次创建 tree filter item 时才生成，关闭态列头不再支付 tree-only 成本。

同时，`BuildMenuItems()` / `BuildTreeItems()` 原先每层递归都创建一个临时 `List`，再由调用者二次遍历添加到 `Items`。修复后改为 `PopulateMenuItems()` / `PopulateTreeItems()` 直接填充目标 `ItemCollection`，去掉递归 materialization 的中间列表；menu/tree item 顺序、层级和 tree radio group 语义保持。

状态验证覆盖：menu filter indicators 关闭态不创建 tree group name；tree filter indicators 关闭态也不创建 group name；materialize 第一个 tree filter 后，只该 indicator 生成 group name，未打开 sibling 仍为空；menu flyout exact item count 保持 5，tree flyout exact item count 保持 select-all root + 5 configured items，5 个实际 tree filter items 共用同一个 lazy group name。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Tree radio group name strings / closed `DataGridFilterIndicator` | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；关闭态和 menu filter 不再分配 tree-only 字符串 |
| `DataGrid.Basic` closed tree group name strings | 5 | 0 | `(5 - 0) / 5` | 100.00% | 有效；4 columns + top-left header 均延迟 |
| `DataGrid.Filter.Menu.Closed` closed tree group name strings | 4 | 0 | `(4 - 0) / 4` | 100.00% | 有效；menu filter 不再生成 tree group name |
| `DataGrid.Filter.Tree.Closed` closed tree group name strings | 4 | 0 | `(4 - 0) / 4` | 100.00% | 有效；tree filter 首次打开前不生成 group name |
| `DataGrid.GalleryShape` closed tree group name strings | 22 | 0 | `(22 - 0) / 22` | 100.00% | 有效；按当前 perf GalleryShape 的 realized header indicator 数估算 |
| Nested 5-item menu filter temp item lists / first open | 2 | 0 | `(2 - 0) / 2` | 100.00% | 有效；root + nested child list 均移除 |
| Nested 5-item tree filter temp item lists / first open | 2 | 0 | `(2 - 0) / 2` | 100.00% | 有效；root + nested child list 均移除 |
| Menu/tree filter item hierarchy | menu 5 / tree 6 items | menu 5 / tree 6 items | behavior preserved | 0.00% | 正确性保持；状态验证覆盖 exact count |

Smoke-only 对比上一轮文档中的同参数复测；本轮结构收益集中在小对象分配，`datagrid --count 100` timing 只作异常检查，不作为本轮速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.920 | 14.617 | `(13.920 - 14.617) / 13.920` | -5.01% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Menu.Closed | 9.017 | 9.688 | `(9.017 - 9.688) / 9.017` | -7.44% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Tree.Closed | 7.769 | 8.440 | `(7.769 - 8.440) / 7.769` | -8.64% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowHeaders | 9.398 | 10.107 | `(9.398 - 10.107) / 9.398` | -7.54% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowDetails.Collapsed | 9.269 | 10.067 | `(9.269 - 10.067) / 9.269` | -8.61% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GroupHeaders | 8.719 | 9.068 | `(8.719 - 9.068) / 8.719` | -4.00% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowGroups | 9.332 | 10.080 | `(9.332 - 10.080) / 9.332` | -8.02% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GalleryShape | 41.256 | 42.675 | `(41.256 - 42.675) / 41.256` | -3.44% | smoke-only；不作为本轮速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.617 | 2948.5 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.688 | 2587.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.440 | 2583.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.107 | 3107.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.067 | 3024.1 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.068 | 2575.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.080 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.675 | 12410.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.57 DataGrid pagination re-template subscription cleanup

本轮优化点：`DataGrid.OnApplyTemplate()` 原先会先覆盖 `_topPagination` / `_bottomPagination` 字段，再给新 template part 订阅 `CurrentPageChanged`。如果同一个 DataGrid 发生重套模板，旧 top/bottom `Pagination` 上的 DataGrid page-change handler 没有在替换前释放。现在在查找新 template part 前先从旧 part 退订，再订阅新 part。

这是生命周期/正确性修复，不是标准页面加载路径优化。收益集中在重套模板或 template replacement 场景：旧 pagination part 不再保留 DataGrid handler；新 pagination part 仍保持 exactly one handler；最终 detach 后当前 pagination handler 释放。

状态验证覆盖：初始 top/bottom pagination 各 1 个 handler；`Template = null` 后恢复模板并 `ApplyTemplate()`，旧 top/bottom pagination handler count 均为 0；新 top/bottom pagination 与旧实例不同且各 1 个 handler；realized scenario dispose 后新 top/bottom pagination handler count 均为 0。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Old top pagination `CurrentPageChanged` handlers after re-template | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；旧 top pagination 不再保留 DataGrid handler |
| Old bottom pagination `CurrentPageChanged` handlers after re-template | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；旧 bottom pagination 不再保留 DataGrid handler |
| Total stale pagination handlers after re-template | 2 | 0 | `(2 - 0) / 2` | 100.00% | 有效；重套模板 stale handler 全部释放 |
| New top pagination handlers after re-template | 1 | 1 | `(1 - 1) / 1` | 0.00% | 正确性保持；新 part exactly one handler |
| New bottom pagination handlers after re-template | 1 | 1 | `(1 - 1) / 1` | 0.00% | 正确性保持；新 part exactly one handler |
| Current pagination handlers after detach | 2 | 0 | `(2 - 0) / 2` | 100.00% | 生命周期验证；dispose/detach 后当前 top/bottom handler 释放 |

Smoke-only 对比上一轮文档中的同参数复测；本轮路径只在重套模板时触发，标准 DataGrid 页面加载不触发该路径，所以下表只作异常检查，不作为本轮速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.617 | 13.692 | `(14.617 - 13.692) / 14.617` | 6.33% | smoke-only；不触发 re-template cleanup，不作为速度收益证明 |
| DataGrid.Filter.Menu.Closed | 9.688 | 9.040 | `(9.688 - 9.040) / 9.688` | 6.69% | smoke-only；不触发 re-template cleanup，不作为速度收益证明 |
| DataGrid.Filter.Tree.Closed | 8.440 | 8.181 | `(8.440 - 8.181) / 8.440` | 3.07% | smoke-only；不触发 re-template cleanup，不作为速度收益证明 |
| DataGrid.RowHeaders | 10.107 | 9.575 | `(10.107 - 9.575) / 10.107` | 5.26% | smoke-only；不触发 re-template cleanup，不作为速度收益证明 |
| DataGrid.RowDetails.Collapsed | 10.067 | 10.013 | `(10.067 - 10.013) / 10.067` | 0.54% | smoke-only；不触发 re-template cleanup，不作为速度收益证明 |
| DataGrid.GroupHeaders | 9.068 | 8.834 | `(9.068 - 8.834) / 9.068` | 2.58% | smoke-only；不触发 re-template cleanup，不作为速度收益证明 |
| DataGrid.RowGroups | 10.080 | 9.809 | `(10.080 - 9.809) / 10.080` | 2.69% | smoke-only；不触发 re-template cleanup，不作为速度收益证明 |
| DataGrid.GalleryShape | 42.675 | 42.265 | `(42.675 - 42.265) / 42.675` | 0.96% | smoke-only；不触发 re-template cleanup，不作为速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.692 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.040 | 2585.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.181 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.575 | 3108.1 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.013 | 3025.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.834 | 2574.8 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.809 | 2939.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.265 | 12412.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.58 DataGrid row reorder drag state release cleanup

本轮优化点：`DataGridRowReorderHandle` 的拖拽状态由 static 字段保存。原实现真实拖拽 release 后会移除 ghost row，但没有显式清空 `DataGridRowsPresenter.DraggedRowIndex`；如果拖拽中途 row unload / visual detach，也缺少 owner-paired cleanup，可能留下 stale drag state。现在引入 `_dragOwner`，只有当前 owner handle 能释放 static drag 字段；真实 drop 和 row unload 都清空 `DraggedRowIndex` / `DragRowOffset`，row unload 同时移除 ghost row。TopLevel detach 路径只清 static/presenter 标记，不在 Avalonia detach 遍历中改动 rows presenter 的 `VisualChildren`。

状态验证覆盖：直接进入 row reorder drag 后，确认 `DraggedRowIndex` 和 ghost row 已创建；真实 left-button release 仍触发 1 次 `RowReordered`，随后 `DraggedRowIndex=null`、`DragRowOffset=0`、ghost row 移除；active drag 的 row unload 同样清空 state 并移除 ghost row；active drag 的 TopLevel detach 不抛异常，并清空 static/presenter drag state。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Drag release stale `RowsPresenter.DraggedRowIndex` | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；drop 后 presenter 不再保留 stale row index |
| Drag release ghost row indicator | 1 before release / 0 after release | unchanged | behavior preserved | 0.00% | 正确性保持；drop 仍移除 ghost row |
| Drag release `RowReordered` notifications | 1 | 1 | `(1 - 1) / 1` | 0.00% | 正确性保持；真实 drop 事件未丢失 |
| Active drag row unload stale `RowsPresenter.DraggedRowIndex` | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；row unload 释放 presenter drag state |
| Active drag row unload ghost row indicator | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；owner handle unload 移除 ghost row |
| Active drag detach static drag owner/state | 1 active owner/state set | 0 active owner/state set | `(1 - 0) / 1` | 100.00% | 有效；TopLevel detach 不再保留 static drag state |

Smoke-only 对比上一轮文档中的同参数复测；本轮路径只在 row reorder drag/drop/unload/detach 交互中触发，标准 DataGrid 页面加载不触发该路径，所以下表只作异常检查，不作为本轮速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.692 | 14.108 | `(13.692 - 14.108) / 13.692` | -3.04% | smoke-only；不触发 row reorder drag cleanup，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 9.040 | 9.196 | `(9.040 - 9.196) / 9.040` | -1.73% | smoke-only；不触发 row reorder drag cleanup，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.181 | 8.330 | `(8.181 - 8.330) / 8.181` | -1.82% | smoke-only；不触发 row reorder drag cleanup，不作为速度回归结论 |
| DataGrid.RowHeaders | 9.575 | 9.631 | `(9.575 - 9.631) / 9.575` | -0.58% | smoke-only；不触发 row reorder drag cleanup，不作为速度回归结论 |
| DataGrid.RowDetails.Collapsed | 10.013 | 10.308 | `(10.013 - 10.308) / 10.013` | -2.95% | smoke-only；不触发 row reorder drag cleanup，不作为速度回归结论 |
| DataGrid.GroupHeaders | 8.834 | 8.825 | `(8.834 - 8.825) / 8.834` | 0.10% | smoke-only；不触发 row reorder drag cleanup，不作为速度收益证明 |
| DataGrid.RowGroups | 9.809 | 9.709 | `(9.809 - 9.709) / 9.809` | 1.02% | smoke-only；不触发 row reorder drag cleanup，不作为速度收益证明 |
| DataGrid.GalleryShape | 42.265 | 42.696 | `(42.265 - 42.696) / 42.265` | -1.02% | smoke-only；不触发 row reorder drag cleanup，不作为速度回归结论 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.108 | 2949.1 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.196 | 2586.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.330 | 2584.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.631 | 3108.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.308 | 3025.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.825 | 2574.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.709 | 2939.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.696 | 12412.7 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.59 DataGrid column header drag state release cleanup

本轮优化点：`DataGridColumnHeader` 的 resize/reorder 交互使用 static 字段保存 drag state。正常 pointer release / lost capture 会清理这些字段，但如果 active drag 过程中 owner header 被移除或整棵 TopLevel detach，原路径不会经过完整 release，可能留下 stale `_dragColumn` / `_dragStart` / `_currentDraggingOverColumn` 和 headers presenter `DragColumn` / `DragIndicator`。现在引入 `_dragOwner`，只有发起本轮 drag 的 header 能释放 static state；单 header 从 presenter 移除时同步移除 presenter drag indicator，TopLevel detach 只清 state，不改动其他 parent 的 `VisualChildren`。

状态验证覆盖：单 header child detach 会发出一次 null drag-over cleanup、清空 presenter `DragColumn` / `DragIndicator` 和 static drag fields；TopLevel detach 不抛异常，清空 static drag fields 和 presenter `DragColumn`，但不在 Avalonia detach 遍历中移除 drag indicator visual child。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Column header child detach stale `_dragColumn` / `_dragStart` / owner | 1 active state set | 0 active state set | `(1 - 0) / 1` | 100.00% | 有效；owner header detach 释放 static drag state |
| Column header child detach presenter `DragColumn` | 1 retained drag column | 0 retained drag columns | `(1 - 0) / 1` | 100.00% | 有效；presenter 不再保留 stale drag column |
| Column header child detach presenter `DragIndicator` | 1 retained drag indicator | 0 retained drag indicators | `(1 - 0) / 1` | 100.00% | 有效；单 header 移除时同步移除 drag visual |
| Column header child detach null drag-over cleanup | 1 cleanup event | 1 cleanup event | `(1 - 1) / 1` | 0.00% | 正确性保持；仍只发送一次 cleanup |
| Column header TopLevel detach stale static drag state | 1 active state set | 0 active state set | `(1 - 0) / 1` | 100.00% | 有效；整棵树 detach 后不保留 static state |
| TopLevel detach child mutation crash risk | possible mutation during cached child traversal | 0 verified crash | behavior verified | n/a | 正确性修复；不在 TopLevel detach 中移除别的 parent 的 visual child |

Smoke-only 对比上一轮文档中的同参数复测；本轮路径只在 column header resize/reorder drag 的 detach cleanup 中触发，标准 DataGrid 页面加载不触发该路径，所以下表只作异常检查，不作为本轮速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.108 | 15.006 | `(14.108 - 15.006) / 14.108` | -6.37% | smoke-only；不触发 column header detach cleanup，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 9.196 | 9.375 | `(9.196 - 9.375) / 9.196` | -1.95% | smoke-only；不触发 column header detach cleanup，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.330 | 8.612 | `(8.330 - 8.612) / 8.330` | -3.39% | smoke-only；不触发 column header detach cleanup，不作为速度回归结论 |
| DataGrid.RowHeaders | 9.631 | 9.888 | `(9.631 - 9.888) / 9.631` | -2.67% | smoke-only；不触发 column header detach cleanup，不作为速度回归结论 |
| DataGrid.RowDetails.Collapsed | 10.308 | 10.269 | `(10.308 - 10.269) / 10.308` | 0.38% | smoke-only；不触发 column header detach cleanup，不作为速度收益证明 |
| DataGrid.GroupHeaders | 8.825 | 9.145 | `(8.825 - 9.145) / 8.825` | -3.63% | smoke-only；不触发 column header detach cleanup，不作为速度回归结论 |
| DataGrid.RowGroups | 9.709 | 10.156 | `(9.709 - 10.156) / 9.709` | -4.60% | smoke-only；不触发 column header detach cleanup，不作为速度回归结论 |
| DataGrid.GalleryShape | 42.696 | 43.965 | `(42.696 - 43.965) / 42.696` | -2.97% | smoke-only；不触发 column header detach cleanup，不作为速度回归结论 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.006 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.375 | 2586.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.612 | 2584.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.888 | 3108.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.269 | 3025.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.145 | 2575.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.156 | 2939.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.965 | 12412.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.60 DataGrid collection view group key matching

本轮修复点：`CollectionViewGroupRoot.AddToSubgroup()` 在复用已有 subgroup 时没有调用 `GroupBy.KeysMatch(subgroup.Key, key)`，导致分组数据一旦已有 subgroup，后续 item 会直接落入第一个 subgroup。共享 `ListCollectionViewGroupRoot` 的同名路径已有 key matching，DataGrid 这里是遗漏。修复后，非连续重复 key 会回到匹配 subgroup，新 key 会创建新 subgroup。

状态验证覆盖：构造 key 序列 `[1, 2, 1]`，添加 `DataGridPathGroupDescription(nameof(Group))` 后，验证分组数、group key/item count、grouped leaf order 和 `IndexOf()` 都按 key 匹配。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Distinct subgroups for key sequence `[1, 2, 1]` | 1 subgroup | 2 subgroups | `(2 - 1) / 2` | 50.00% | 正确性修复；缺失的 key=2 subgroup 被创建 |
| Missing subgroup count for `[1, 2, 1]` | 1 missing subgroup | 0 missing subgroups | `(1 - 0) / 1` | 100.00% | 有效；每个 distinct key 有对应 subgroup |
| Wrong group assignment for key=2 item | 1 wrong assignment | 0 wrong assignments | `(1 - 0) / 1` | 100.00% | 有效；不再落入第一个 key=1 subgroup |
| Grouped `IndexOf()` for repeated key item | 2 | 1 | expected index match | n/a | 正确性修复；leaf order 按 matching subgroup 计算 |

Smoke-only 对比上一轮文档中的同参数复测；本轮修的是 grouped collection view key matching，标准页面加载 timing 只作异常检查，不作为本轮速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.006 | 13.797 | `(15.006 - 13.797) / 15.006` | 8.06% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Menu.Closed | 9.375 | 9.371 | `(9.375 - 9.371) / 9.375` | 0.04% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Tree.Closed | 8.612 | 8.249 | `(8.612 - 8.249) / 8.612` | 4.22% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowHeaders | 9.888 | 9.715 | `(9.888 - 9.715) / 9.888` | 1.75% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowDetails.Collapsed | 10.269 | 10.044 | `(10.269 - 10.044) / 10.269` | 2.19% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GroupHeaders | 9.145 | 8.960 | `(9.145 - 8.960) / 9.145` | 2.02% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowGroups | 10.156 | 9.800 | `(10.156 - 9.800) / 10.156` | 3.51% | smoke-only；RowGroups 通过但单轮 timing 不作收益证明 |
| DataGrid.GalleryShape | 43.965 | 43.248 | `(43.965 - 43.248) / 43.965` | 1.63% | smoke-only；不作为本轮速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.797 | 2949.1 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.371 | 2586.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.249 | 2584.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.715 | 3108.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.044 | 3025.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.960 | 2574.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.800 | 2939.9 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.248 | 12412.4 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.61 DataGrid path group description owner-type cache

本轮修复点：`DataGridPathGroupDescription` 原先只缓存 `_propertyType`，没有记录该 property type 属于哪个 owner type。对于同一个 group description 处理混合 item type 的情况，如果两个 item 都有同名属性但属性类型不同，第二个 item 会复用第一个 item 的 property type，`TypeHelper.GetNestedPropertyValue()` 会因属性类型不一致返回 null，最终 fallback 到 item 本身作为 group key。现在和 `DataGridFilterDescription` 保持同一口径：缓存 `_propertyOwnerType + _propertyType`，owner type 变化时重新解析 property type。

状态验证覆盖：按顺序调用 `GroupKeyFromItem()`：`SortProbe.Group:int -> StringGroupProbe.Group:string -> SortProbe.Group:int`，验证三次都返回真实属性值，并且最后缓存的 owner type / property type 回到 `SortProbe/int`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Correct group keys for mixed owner sequence `int -> string -> int` | 2 / 3 | 3 / 3 | `(3 - 2) / 3` | 33.33% | 正确性修复；string owner 不再 fallback 到 item 本身 |
| Stale property type after owner type switch | 1 stale type | 0 stale types | `(1 - 0) / 1` | 100.00% | 有效；owner type 变化时刷新 cached property type |
| Cached latest owner type | not tracked | tracked | behavior added | n/a | 结构修复；cache key 从 property type 扩展到 owner type + property type |
| Repeated owner switch back to first type | stale-dependent | correct key | behavior verified | n/a | 正确性保持；切回 int owner 后仍返回 int key |

Smoke-only 对比上一轮文档中的同参数复测。两轮 smoke 都显示 Basic、Filter、GalleryShape 这类不触发本轮 mixed-owner group description 路径的场景同步变慢，判断为机器/热状态噪声；本轮不声明 timing 收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.797 | 17.213 | `(13.797 - 17.213) / 13.797` | -24.76% | smoke-only；不触发本轮路径，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 9.371 | 12.479 | `(9.371 - 12.479) / 9.371` | -33.17% | smoke-only；不触发本轮路径，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.249 | 9.453 | `(8.249 - 9.453) / 8.249` | -14.60% | smoke-only；不触发本轮路径，不作为速度回归结论 |
| DataGrid.RowHeaders | 9.715 | 10.316 | `(9.715 - 10.316) / 9.715` | -6.19% | smoke-only；不触发本轮路径，不作为速度回归结论 |
| DataGrid.RowDetails.Collapsed | 10.044 | 10.919 | `(10.044 - 10.919) / 10.044` | -8.71% | smoke-only；不触发本轮路径，不作为速度回归结论 |
| DataGrid.GroupHeaders | 8.960 | 9.599 | `(8.960 - 9.599) / 8.960` | -7.13% | smoke-only；不触发本轮路径，不作为速度回归结论 |
| DataGrid.RowGroups | 9.800 | 11.332 | `(9.800 - 11.332) / 9.800` | -15.63% | smoke-only；standard row group smoke 不覆盖 mixed owner type，不作为速度回归结论 |
| DataGrid.GalleryShape | 43.248 | 46.611 | `(43.248 - 46.611) / 43.248` | -7.78% | smoke-only；不触发本轮路径，不作为速度回归结论 |

当前工作区 smoke（第二轮复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 17.213 | 2949.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 12.479 | 2587.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.453 | 2584.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.316 | 3108.1 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.919 | 3025.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.599 | 2574.8 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 11.332 | 2939.9 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 46.611 | 12412.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.62 DataGrid collection view property changed args cache

本轮优化点：`DataGridCollectionView.NotifyPropertyChanged(string)` 原先每次通知都会创建新的 `PropertyChangedEventArgs`。这些通知集中在固定属性名集合上，包括 `Count` / `ItemCount` / `CurrentItem` / `CurrentPosition` / `IsEmpty`、分页属性以及 sort/filter descriptions。现在为 19 个已知属性名缓存静态 `PropertyChangedEventArgs`，热路径通知直接复用；未知属性名仍保持原来的每次创建行为，避免引入无界缓存或改变扩展路径语义。

状态验证覆盖：通过反射连续调用 `NotifyPropertyChanged(string)`，验证 19 个已知属性名两次通知复用同一个 event args 引用且 `PropertyName` 保持正确；未知属性名两次通知仍产生不同 event args，且属性名保持原值。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Known `PropertyChangedEventArgs` allocations per notification | 1 allocation | 0 allocations | `(1 - 0) / 1` | 100.00% | 有效；19 个已知属性通知复用静态 event args |
| Cached known property names | 0 / 19 | 19 / 19 | `(19 - 0) / 19` | 100.00% | 有效；覆盖当前 `NotifyPropertyChanged(...)` 调用集合 |
| Unknown property-name cache entries | 0 | 0 | unchanged | 0.00% | 正确性/资源边界保持；未知属性不进入缓存 |
| PropertyChanged notification behavior | property name delivered | property name delivered | behavior preserved | n/a | 正确性保持；已知/未知属性名均已验证 |

Smoke-only 对比上一轮文档中的同参数复测。单次 smoke 本轮只作异常检查，不作为速度收益证明；标准页面加载 timing 的改善不能单独归因到 event args 缓存：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 17.213 | 15.233 | `(17.213 - 15.233) / 17.213` | 11.50% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Menu.Closed | 12.479 | 9.181 | `(12.479 - 9.181) / 12.479` | 26.43% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Tree.Closed | 9.453 | 8.402 | `(9.453 - 8.402) / 9.453` | 11.12% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowHeaders | 10.316 | 9.674 | `(10.316 - 9.674) / 10.316` | 6.22% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowDetails.Collapsed | 10.919 | 10.281 | `(10.919 - 10.281) / 10.919` | 5.84% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GroupHeaders | 9.599 | 8.785 | `(9.599 - 8.785) / 9.599` | 8.48% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.RowGroups | 11.332 | 9.740 | `(11.332 - 9.740) / 11.332` | 14.05% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.GalleryShape | 46.611 | 42.851 | `(46.611 - 42.851) / 46.611` | 8.07% | smoke-only；不作为本轮速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.233 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.181 | 2586.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.402 | 2584.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.674 | 3108.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.281 | 3025.4 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.785 | 2575.4 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.740 | 2939.9 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.851 | 12412.4 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.63 DataGrid collection view reset collection changed args cache

本轮优化点：`DataGridCollectionView` 在刷新、分页完成和非通知源 `IList.Clear()` 路径会发出无 payload 的 `NotifyCollectionChangedAction.Reset`。这些 Reset event args 不携带 item/index，原实现每次都创建新的 `NotifyCollectionChangedEventArgs`。现在把无 payload Reset event args 缓存为静态实例，保留带 `value` 的 Reset 构造路径不变。

状态验证覆盖：连续调用 `Refresh()` 两次，验证发出的 Reset collection changed 通知复用同一个 event args 引用，并且 `NewItems` / `OldItems` 仍为空。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Payload-free Reset `NotifyCollectionChangedEventArgs` allocations per reset notification | 1 allocation | 0 allocations | `(1 - 0) / 1` | 100.00% | 有效；刷新 / 分页 / clear 的纯 Reset 通知复用静态 event args |
| Cached payload-free Reset paths in `DataGridCollectionView` | 0 / 4 | 4 / 4 | `(4 - 0) / 4` | 100.00% | 有效；4 个无 payload Reset 构造点已收敛 |
| Payload-carrying Reset paths | unchanged | unchanged | behavior preserved | 0.00% | 正确性保持；带 `value` 的兼容路径未改 |
| Reset notification payload | empty | empty | behavior preserved | n/a | 正确性保持；`NewItems` / `OldItems` 仍为空 |

Smoke-only 对比上一轮文档中的同参数复测。复测两轮后 timing 仍有混合波动，且本轮结构收益只针对刷新/分页 Reset event args 分配；不声明页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.233 | 14.563 | `(15.233 - 14.563) / 15.233` | 4.40% | smoke-only；不作为本轮速度收益证明 |
| DataGrid.Filter.Menu.Closed | 9.181 | 9.477 | `(9.181 - 9.477) / 9.181` | -3.22% | smoke-only；不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.402 | 8.442 | `(8.402 - 8.442) / 8.402` | -0.48% | smoke-only；不作为速度回归结论 |
| DataGrid.RowHeaders | 9.674 | 10.785 | `(9.674 - 10.785) / 9.674` | -11.48% | smoke-only；不作为速度回归结论 |
| DataGrid.RowDetails.Collapsed | 10.281 | 10.858 | `(10.281 - 10.858) / 10.281` | -5.61% | smoke-only；不作为速度回归结论 |
| DataGrid.GroupHeaders | 8.785 | 9.525 | `(8.785 - 9.525) / 8.785` | -8.42% | smoke-only；不作为速度回归结论 |
| DataGrid.RowGroups | 9.740 | 10.114 | `(9.740 - 10.114) / 9.740` | -3.84% | smoke-only；不作为速度回归结论 |
| DataGrid.GalleryShape | 42.851 | 43.125 | `(42.851 - 43.125) / 42.851` | -0.64% | smoke-only；不作为速度回归结论 |

当前工作区 smoke（第二轮复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.563 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.477 | 2586.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.442 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.785 | 3108.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.858 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.525 | 2574.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.114 | 2939.8 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.125 | 12412.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.64 DataGrid filter flyout close selected-values allocation cleanup

本轮优化点：`DataGridMenuFilterFlyout.OnClosed()` / `DataGridTreeFilterFlyout.OnClosed()` 原先先创建一个空 `List<string>`，随后在 `Popup.Child` 是对应 presenter 时立即用 `presenter.GetFilterValues()` 的返回值覆盖它。也就是说，正常已 materialized presenter 的关闭路径每次都会多一次无用空列表分配。现在改为条件表达式：有 presenter 时直接取 selected values；没有 presenter 时才创建空列表。

状态验证覆盖：直接触发 menu/tree filter flyout 的 close 通知路径，验证需要处理关闭的路径仍直接使用 presenter 返回的 selected values，空选择列表保持为空。2.98 之后默认 `FilterOnClose=false` 的被动关闭已进一步优化为不通知；`FilterOnClose=true` 被动关闭和 OK 确认关闭继续覆盖这条 selected-values 分配优化。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Redundant empty selected-values `List<string>` allocations per presenter-backed close | 1 allocation | 0 allocations | `(1 - 0) / 1` | 100.00% | 有效；menu/tree close 路径不再先创建被覆盖的空列表 |
| Affected filter flyout close implementations | 0 / 2 optimized | 2 / 2 optimized | `(2 - 0) / 2` | 100.00% | 有效；Menu / Tree 两条 close 路径一致收敛 |
| No-presenter close fallback | empty list | empty list | behavior preserved | n/a | 正确性保持；无 presenter 时仍发送空列表 |
| Close confirmation state when notification is needed | passive `false` / OK `true` | passive `false` / OK `true` | behavior preserved | n/a | 正确性保持；状态验证覆盖 |

Smoke-only 对比上一轮文档中的同参数复测。第一轮 smoke 明显偏慢后已复测；本轮路径只在 filter flyout close 时触发，closed-state 页面加载 timing 只作异常检查，不声明页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.563 | 14.696 | `(14.563 - 14.696) / 14.563` | -0.91% | smoke-only；不触发 filter close 路径，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 9.477 | 9.241 | `(9.477 - 9.241) / 9.477` | 2.49% | smoke-only；closed-state 不触发 close 路径，不作为速度收益证明 |
| DataGrid.Filter.Tree.Closed | 8.442 | 8.168 | `(8.442 - 8.168) / 8.442` | 3.25% | smoke-only；closed-state 不触发 close 路径，不作为速度收益证明 |
| DataGrid.RowHeaders | 10.785 | 9.903 | `(10.785 - 9.903) / 10.785` | 8.18% | smoke-only；不触发 filter close 路径，不作为速度收益证明 |
| DataGrid.RowDetails.Collapsed | 10.858 | 10.127 | `(10.858 - 10.127) / 10.858` | 6.73% | smoke-only；不触发 filter close 路径，不作为速度收益证明 |
| DataGrid.GroupHeaders | 9.525 | 8.935 | `(9.525 - 8.935) / 9.525` | 6.19% | smoke-only；不触发 filter close 路径，不作为速度收益证明 |
| DataGrid.RowGroups | 10.114 | 10.092 | `(10.114 - 10.092) / 10.114` | 0.22% | smoke-only；不触发 filter close 路径，不作为速度收益证明 |
| DataGrid.GalleryShape | 43.125 | 44.493 | `(43.125 - 44.493) / 43.125` | -3.17% | smoke-only；本轮 close-path 优化不以该 timing 定性 |

当前工作区 smoke（第二轮复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.696 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.241 | 2588.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.168 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.903 | 3108.0 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.127 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.935 | 2574.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.092 | 2939.8 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.493 | 12412.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.65 DataGrid group data property changed args cache

本轮优化点：DataGrid 分组数据层有 3 类固定 `PropertyChanged` 通知：分组 item 数量变化时的 `ItemCount`、分组是否到底层变化时的 `IsBottomLevel`、`GroupKeys` 集合变化时的 `GroupKeys`。这些属性名固定，原实现每次通知都创建新的 `PropertyChangedEventArgs`。现在改为每类通知复用一个静态 event args。

状态验证覆盖：分别触发 `ItemCount`、`IsBottomLevel`、`GroupKeys` 两次通知，验证两次拿到的是同一个 event args 实例，并且 `PropertyName` 保持正确。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 每次分组 `ItemCount` 通知创建的 event args | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 有效；分组增删 item / count 更新时，每次少创建 1 个事件参数对象 |
| 每次分组 `IsBottomLevel` 通知创建的 event args | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 有效；分组层级切换时，每次少创建 1 个事件参数对象 |
| 每次 `GroupKeys` 变更通知创建的 event args | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 有效；分组 key 集合变化时，每次少创建 1 个事件参数对象 |
| 已缓存的固定分组通知类型 | 0 / 3 cached | 3 / 3 cached | `(3 - 0) / 3` | 100.00% | 有效；3 条固定分组通知路径都已收敛 |
| 通知语义 | property name correct | property name correct | behavior preserved | n/a | 正确性保持；状态验证覆盖 property name 与事件触发次数 |

Smoke-only 对比上一轮文档中的同参数复测。本轮路径只在分组数据通知时触发，页面构造 smoke 只作异常检查，不声明页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.696 | 15.876 | `(14.696 - 15.876) / 14.696` | -8.03% | smoke-only；不触发分组通知路径，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 9.241 | 9.004 | `(9.241 - 9.004) / 9.241` | 2.56% | smoke-only；不触发分组通知路径，不作为速度收益证明 |
| DataGrid.Filter.Tree.Closed | 8.168 | 8.217 | `(8.168 - 8.217) / 8.168` | -0.60% | smoke-only；不触发分组通知路径，不作为速度回归结论 |
| DataGrid.RowHeaders | 9.903 | 9.673 | `(9.903 - 9.673) / 9.903` | 2.32% | smoke-only；不触发分组通知路径，不作为速度收益证明 |
| DataGrid.RowDetails.Collapsed | 10.127 | 10.095 | `(10.127 - 10.095) / 10.127` | 0.32% | smoke-only；不触发分组通知路径，不作为速度收益证明 |
| DataGrid.GroupHeaders | 8.935 | 8.919 | `(8.935 - 8.919) / 8.935` | 0.18% | smoke-only；本场景构造分组头，但不覆盖本轮 repeated notification 热路径 |
| DataGrid.RowGroups | 10.092 | 9.830 | `(10.092 - 9.830) / 10.092` | 2.60% | smoke-only；不作为本轮确定速度收益 |
| DataGrid.GalleryShape | 44.493 | 42.702 | `(44.493 - 42.702) / 44.493` | 4.03% | smoke-only；不作为本轮确定速度收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.876 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.004 | 2586.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.217 | 2584.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.673 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.095 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.919 | 2574.6 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.830 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.702 | 12412.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.66 DataGrid paged enumerator list preallocation

本轮优化点：`DataGridCollectionView.GetEnumerator()` 在 `PageSize > 0` 的分页模式下，会把当前页 item 复制到一个临时 `List<object?>` 再返回枚举器。原实现每次都从空列表开始追加，当前页有多个 item 时会触发列表扩容。现在先计算当前页实际 item 数，并用这个数量作为 list capacity。

状态验证覆盖：构造 7 条数据、`PageSize=3`，分别验证第 2 页 `[3,4,5]` 和末页 `[6]` 的内部分页列表 `Count == Capacity`，并验证 public enumerator 枚举顺序保持一致。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 满页 3 条数据追加前已预留容量 | 0 / 3 items | 3 / 3 items | `(3 - 0) / 3` | 100.00% | 有效；分页枚举当前页时，临时列表一次分配到位 |
| 末页 1 条数据追加前已预留容量 | 0 / 1 item | 1 / 1 item | `(1 - 0) / 1` | 100.00% | 有效；末页只给实际 1 条 item 预留空间 |
| 分页列表容量是否等于当前页 item 数 | no | yes | behavior verified | n/a | 有效；状态验证覆盖第 2 页和末页 |
| public enumerator 顺序 | page order preserved | page order preserved | behavior preserved | n/a | 正确性保持；`[3,4,5]` 和 `[6]` 枚举结果不变 |

Smoke-only 对比上一轮文档中的同参数复测。本轮路径只在分页 collection view 被枚举时触发，Gallery 默认 smoke 不覆盖该分页枚举热路径，不声明页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.876 | 14.517 | `(15.876 - 14.517) / 15.876` | 8.56% | smoke-only；默认页面不覆盖分页枚举路径，不作为本轮确定速度收益 |
| DataGrid.Filter.Menu.Closed | 9.004 | 9.052 | `(9.004 - 9.052) / 9.004` | -0.53% | smoke-only；不触发分页枚举路径，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.217 | 8.035 | `(8.217 - 8.035) / 8.217` | 2.21% | smoke-only；不触发分页枚举路径，不作为速度收益证明 |
| DataGrid.RowHeaders | 9.673 | 9.567 | `(9.673 - 9.567) / 9.673` | 1.10% | smoke-only；不触发分页枚举路径，不作为速度收益证明 |
| DataGrid.RowDetails.Collapsed | 10.095 | 9.774 | `(10.095 - 9.774) / 10.095` | 3.18% | smoke-only；不触发分页枚举路径，不作为速度收益证明 |
| DataGrid.GroupHeaders | 8.919 | 8.698 | `(8.919 - 8.698) / 8.919` | 2.48% | smoke-only；不触发分页枚举路径，不作为速度收益证明 |
| DataGrid.RowGroups | 9.830 | 9.657 | `(9.830 - 9.657) / 9.830` | 1.76% | smoke-only；不触发分页枚举路径，不作为速度收益证明 |
| DataGrid.GalleryShape | 42.702 | 42.047 | `(42.702 - 42.047) / 42.702` | 1.53% | smoke-only；不作为本轮确定速度收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.517 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.052 | 2586.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.035 | 2584.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.567 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.774 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.698 | 2574.6 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.657 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.047 | 12412.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.67 DataGrid column group tree collection accumulator cleanup

本轮优化点：`DataGrid.HandleGroupColumnsInternalCollectionChanged()` 在 `ColumnGroups` add/remove 时需要递归收集列分组树里的 leaf `DataGridColumn`。原实现每进入一个 group/column 节点都会创建一个新的 `List<DataGridColumn>`，再通过 `AddRange()` 合并到父节点。现在改为整次 add/remove 操作共享一个 accumulator list，递归过程直接写入目标列表。

状态验证覆盖：构造 `Profile -> Name + Details(Address, Score) + Age` 的 6 节点嵌套列分组树，验证 add 后 `grid.Columns` 按深度优先顺序得到 4 个 leaf columns，remove 后这 4 个 leaf columns 全部从 `grid.Columns` 移除。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 每次 add/remove 6 节点列分组树的递归临时 `List<DataGridColumn>` | 6 lists | 0 lists | `(6 - 0) / 6` | 100.00% | 有效；动态列分组变化时少创建短命列表 |
| 每次 add/remove 6 节点列分组树的 child-list merge | 5 `AddRange()` merges | 0 merges | `(5 - 0) / 5` | 100.00% | 有效；递归不再生成子列表再合并 |
| 顶层操作级收集列表 | 1 list | 1 list | behavior preserved | n/a | 保持；仍保留一次操作级列表用于统一 add/remove `ColumnsInternal` |
| leaf column 顺序 | depth-first | depth-first | behavior verified | n/a | 正确性保持；验证覆盖 `Name, Address, Score, Age` |
| remove 后残留 leaf columns | 0 | 0 | behavior verified | n/a | 正确性保持；验证覆盖嵌套分组 remove |

Smoke-only 对比上一轮文档中的同参数复测。本轮路径只在动态 add/remove `ColumnGroups` 时触发，默认 DataGrid smoke 不覆盖这个动态列分组变更热路径，不声明页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.517 | 14.631 | `(14.517 - 14.631) / 14.517` | -0.79% | smoke-only；不触发 column group tree add/remove，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 9.052 | 9.612 | `(9.052 - 9.612) / 9.052` | -6.19% | smoke-only；不触发 column group tree add/remove，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.035 | 8.533 | `(8.035 - 8.533) / 8.035` | -6.20% | smoke-only；不触发 column group tree add/remove，不作为速度回归结论 |
| DataGrid.RowHeaders | 9.567 | 10.044 | `(9.567 - 10.044) / 9.567` | -4.99% | smoke-only；不触发 column group tree add/remove，不作为速度回归结论 |
| DataGrid.RowDetails.Collapsed | 9.774 | 10.702 | `(9.774 - 10.702) / 9.774` | -9.49% | smoke-only；不触发 column group tree add/remove，不作为速度回归结论 |
| DataGrid.GroupHeaders | 8.698 | 9.219 | `(8.698 - 9.219) / 8.698` | -5.99% | smoke-only；不触发 column group tree add/remove，不作为速度回归结论 |
| DataGrid.RowGroups | 9.657 | 10.517 | `(9.657 - 10.517) / 9.657` | -8.91% | smoke-only；不触发 column group tree add/remove，不作为速度回归结论 |
| DataGrid.GalleryShape | 42.047 | 44.728 | `(42.047 - 44.728) / 42.047` | -6.38% | smoke-only；不作为本轮确定速度收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.631 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.612 | 2586.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.533 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.044 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.702 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.219 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.517 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.728 | 12412.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.68 DataGrid selection changed delta-list preallocation

本轮优化点：`DataGridSelectedItemsCollection.GetSelectionChangedEventArgs()` 原先每次都用 0 容量创建 added / removed item list。SelectAll、ClearSelection、shift-range selection 这类批量选择会把多个 item 加到同一个事件列表里，列表会从 0 容量开始扩容。现在按当前选择数和旧选择数的差值预分配：批量新增时 added list 一次到位，批量清空时 removed list 一次到位；单项选择变化只预分配 1，不会因为已有大量选中项而过度分配。

状态验证覆盖：Extended selection + selection column header checkbox，验证 select-all 仍发出 1 次 `SelectionChanged`、`AddedItems=4 / RemovedItems=0`，且 added list `Capacity=4`；clear selection 仍发出 1 次 `SelectionChanged`、`AddedItems=0 / RemovedItems=4`，且 removed list `Capacity=4`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| SelectAll 4 行时 added list 追加前容量 | 0 / 4 items | 4 / 4 items | `(4 - 0) / 4` | 100.00% | 有效；批量选择事件列表一次分配到位 |
| ClearSelection 4 行时 removed list 追加前容量 | 0 / 4 items | 4 / 4 items | `(4 - 0) / 4` | 100.00% | 有效；批量清空事件列表一次分配到位 |
| 单项 add 的 added list 预留 | 0 / 1 item | 1 / 1 item | `(1 - 0) / 1` | 100.00% | 有效；只按 delta 预分配，不按总选中数过度分配 |
| SelectAll / ClearSelection 事件语义 | 1 event, exact added/removed counts | 1 event, exact added/removed counts | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在用户选择行、全选、清空选择或范围选择时触发，默认 DataGrid 页面加载 smoke 不覆盖 selection changed hot path，不声明页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.631 | 15.747 | `(14.631 - 15.747) / 14.631` | -7.63% | smoke-only；不触发 selection changed，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 9.612 | 9.642 | `(9.612 - 9.642) / 9.612` | -0.31% | smoke-only；不触发 selection changed，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.533 | 8.723 | `(8.533 - 8.723) / 8.533` | -2.23% | smoke-only；不触发 selection changed，不作为速度回归结论 |
| DataGrid.RowHeaders | 10.044 | 10.693 | `(10.044 - 10.693) / 10.044` | -6.46% | smoke-only；不触发 selection changed，不作为速度回归结论 |
| DataGrid.RowDetails.Collapsed | 10.702 | 11.415 | `(10.702 - 11.415) / 10.702` | -6.66% | smoke-only；不触发 selection changed，不作为速度回归结论 |
| DataGrid.GroupHeaders | 9.219 | 9.089 | `(9.219 - 9.089) / 9.219` | 1.41% | smoke-only；不触发 selection changed，不作为速度收益证明 |
| DataGrid.RowGroups | 10.517 | 10.124 | `(10.517 - 10.124) / 10.517` | 3.74% | smoke-only；不触发 selection changed，不作为速度收益证明 |
| DataGrid.GalleryShape | 44.728 | 44.421 | `(44.728 - 44.421) / 44.728` | 0.69% | smoke-only；不作为本轮确定速度收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.747 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.642 | 2586.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.723 | 2584.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.693 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 11.415 | 3025.0 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.089 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.124 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.421 | 12412.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.69 DataGrid selected-items index rebuild cache preallocation

本轮优化点：`DataGridSelectedItemsCollection.UpdateIndexes()` 在 ItemsSource reset / data refresh 后，会根据仍存在的数据项重建 selected-items cache。原实现用 0 容量临时 `List<object>`，如果 5 个选中项都保留下来，`List` 会按默认增长策略扩到 8。现在按旧 selected-items cache 的 count 预分配，最多保留这么多 item，因此 surviving cache 一次分配到位。

状态验证覆盖：5 行 DataGrid 全选后直接触发 `UpdateIndexes()`，验证 `SelectedItems.Count` 仍为 5，内部 selected-items cache `Count=5 / Capacity=5`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 5 个选中项重建 cache 后容量 | 8 capacity | 5 capacity | `(8 - 5) / 8` | 37.50% | 有效；避免 0→4→8 的过量扩容 |
| 5 个选中项重建 cache 的额外空槽 | 3 empty slots | 0 empty slots | `(3 - 0) / 3` | 100.00% | 有效；只为可能存活的 selected items 预留 |
| 选中项数量 | 5 selected | 5 selected | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在数据源 reset / refresh 后重建 selection indexes 时触发，默认 DataGrid 页面加载 smoke 不覆盖该路径，不声明页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.747 | 14.991 | `(15.747 - 14.991) / 15.747` | 4.80% | smoke-only；不触发 selected-items index rebuild，不作为速度收益证明 |
| DataGrid.Filter.Menu.Closed | 9.642 | 9.146 | `(9.642 - 9.146) / 9.642` | 5.14% | smoke-only；不触发 selected-items index rebuild，不作为速度收益证明 |
| DataGrid.Filter.Tree.Closed | 8.723 | 8.338 | `(8.723 - 8.338) / 8.723` | 4.41% | smoke-only；不触发 selected-items index rebuild，不作为速度收益证明 |
| DataGrid.RowHeaders | 10.693 | 10.168 | `(10.693 - 10.168) / 10.693` | 4.91% | smoke-only；不触发 selected-items index rebuild，不作为速度收益证明 |
| DataGrid.RowDetails.Collapsed | 11.415 | 10.145 | `(11.415 - 10.145) / 11.415` | 11.13% | smoke-only；不触发 selected-items index rebuild，不作为速度收益证明 |
| DataGrid.GroupHeaders | 9.089 | 9.105 | `(9.089 - 9.105) / 9.089` | -0.18% | smoke-only；不触发 selected-items index rebuild，不作为速度回归结论 |
| DataGrid.RowGroups | 10.124 | 9.857 | `(10.124 - 9.857) / 10.124` | 2.64% | smoke-only；不触发 selected-items index rebuild，不作为速度收益证明 |
| DataGrid.GalleryShape | 44.421 | 43.297 | `(44.421 - 43.297) / 44.421` | 2.53% | smoke-only；不作为本轮确定速度收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.991 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.146 | 2585.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.338 | 2584.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.168 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.145 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.105 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.857 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.297 | 12412.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.70 DataGrid single-selection first-slot direct lookup

本轮优化点：`DataGrid.ClearRowSelection(slotException, ...)` 和 `DataGrid.SetRowSelection(...)` 在单选场景下只需要读取“唯一已选 slot”。原实现通过 `_selectedItems.GetIndexes().First()` 取值，会为 `IndexToValueTable.GetIndexes()` 创建 yield iterator，并进入 LINQ `First()`。现在 `IndexToValueTable` 提供 `GetFirstIndex()`，`DataGridSelectedItemsCollection.GetFirstSlot()` 直接返回第一个 range 的 lower bound，单选替换路径不再创建 iterator。

状态验证覆盖：Single selection DataGrid 先选中第 1 行，再切到第 3 行，验证 `SelectedItems.Count=1`、`SelectedIndex=2`、`SelectedItem` 和 `SelectedItems[0]` 都指向新行。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 单选替换时 first selected slot 查询的 iterator 分配 | 1 yield iterator | 0 iterators | `(1 - 0) / 1` | 100.00% | 有效；直接读取第一个 range |
| 单选替换时 LINQ `First()` 调用 | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；热路径不再走 LINQ extension |
| 单选替换后的选中项数量 | 1 selected | 1 selected | behavior verified | n/a | 正确性保持 |
| 单选替换后的 selected item | new row | new row | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在单选模式切换唯一选中行时触发，默认 DataGrid 页面加载 smoke 不覆盖该交互路径，不声明页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.991 | 15.076 | `(14.991 - 15.076) / 14.991` | -0.57% | smoke-only；不触发单选替换，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 9.146 | 9.645 | `(9.146 - 9.645) / 9.146` | -5.46% | smoke-only；不触发单选替换，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.338 | 8.515 | `(8.338 - 8.515) / 8.338` | -2.12% | smoke-only；不触发单选替换，不作为速度回归结论 |
| DataGrid.RowHeaders | 10.168 | 9.689 | `(10.168 - 9.689) / 10.168` | 4.71% | smoke-only；不触发单选替换，不作为速度收益证明 |
| DataGrid.RowDetails.Collapsed | 10.145 | 10.524 | `(10.145 - 10.524) / 10.145` | -3.74% | smoke-only；不触发单选替换，不作为速度回归结论 |
| DataGrid.GroupHeaders | 9.105 | 8.851 | `(9.105 - 8.851) / 9.105` | 2.79% | smoke-only；不触发单选替换，不作为速度收益证明 |
| DataGrid.RowGroups | 9.857 | 9.887 | `(9.857 - 9.887) / 9.857` | -0.30% | smoke-only；不触发单选替换，不作为速度回归结论 |
| DataGrid.GalleryShape | 43.297 | 45.366 | `(43.297 - 45.366) / 43.297` | -4.78% | smoke-only；不作为本轮确定速度收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.076 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.645 | 2586.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.515 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.689 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.524 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.851 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.887 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 45.366 | 12412.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.71 DataGrid selected-items index enumeration cleanup

本轮优化点：`DataGridSelectedItemsCollection.GetSelectionChangedEventArgs()` 和 `SelectedItems.GetEnumerator()` 都需要遍历内部 selected slots。原路径通过 `IndexToValueTable.GetIndexes()` 的 yield iterator 暴露 slot 序列；`SelectionChanged` diff 本身不是 iterator 方法，却仍会为内部 selected slots 创建一次短命 yield iterator。现在 `IndexToValueTable` 增加 struct index enumerator，selected-items 内部热路径直接 `foreach` 这个 struct enumerable，不再额外创建 selected-slot yield iterator。之前尝试把 row-group 页面加载路径也切到新枚举器，但 smoke 连续出现 row-group timing 下滑，因此已收窄并撤回 row-group 路径改动。

状态验证覆盖：Extended selection + selection column header checkbox，验证 select-all 后 `SelectedItems.Count=4`，`SelectedItems` 枚举得到 4 项，`SelectionChanged AddedItems=4 / RemovedItems=0`；clear selection 后 `SelectedItems.Count=0`，`SelectedItems` 枚举为空，`SelectionChanged AddedItems=0 / RemovedItems=4`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 每次 `SelectionChanged` diff 的 selected-slot yield iterator | 1 iterator | 0 iterators | `(1 - 0) / 1` | 100.00% | 有效；diff 内部直接走 struct enumerator |
| SelectAll + ClearSelection 两次 diff 的 selected-slot yield iterator | 2 iterators | 0 iterators | `(2 - 0) / 2` | 100.00% | 有效；批量选择和清空各少一次 iterator |
| 每次 `SelectedItems` public enumeration 的嵌套 selected-slot iterator | 1 nested iterator | 0 nested iterators | `(1 - 0) / 1` | 100.00% | 有效；外层 public enumerator 保持，内部 slot iterator 去掉 |
| SelectAll 后 `SelectedItems` 枚举数量 | 4 items | 4 items | behavior verified | n/a | 正确性保持 |
| ClearSelection 后 `SelectedItems` 枚举数量 | 0 items | 0 items | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮最终代码不再改 row-group 页面加载路径，且默认 DataGrid 页面加载 smoke 不触发 `SelectionChanged` diff；本次机器负载偏高，`DataGrid.Basic` 这类未命中本轮路径的场景也明显变慢，因此不声明页面加载速度收益或回归：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.076 | 19.337 | `(15.076 - 19.337) / 15.076` | -28.26% | smoke-only；不触发 selected-items diff，本次负载偏高，不作为本轮回归结论 |
| DataGrid.Filter.Menu.Closed | 9.645 | 9.946 | `(9.645 - 9.946) / 9.645` | -3.12% | smoke-only；不触发 selected-items diff，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.515 | 9.105 | `(8.515 - 9.105) / 8.515` | -6.93% | smoke-only；不触发 selected-items diff，不作为速度回归结论 |
| DataGrid.RowHeaders | 9.689 | 11.769 | `(9.689 - 11.769) / 9.689` | -21.47% | smoke-only；不触发 selected-items diff，本次负载偏高，不作为本轮回归结论 |
| DataGrid.RowDetails.Collapsed | 10.524 | 11.281 | `(10.524 - 11.281) / 10.524` | -7.19% | smoke-only；不触发 selected-items diff，不作为速度回归结论 |
| DataGrid.GroupHeaders | 8.851 | 10.554 | `(8.851 - 10.554) / 8.851` | -19.24% | smoke-only；不触发 selected-items diff，本次负载偏高，不作为本轮回归结论 |
| DataGrid.RowGroups | 9.887 | 11.786 | `(9.887 - 11.786) / 9.887` | -19.21% | smoke-only；row-group 路径改动已撤回，本次不作为本轮回归结论 |
| DataGrid.GalleryShape | 45.366 | 47.779 | `(45.366 - 47.779) / 45.366` | -5.32% | smoke-only；不作为本轮确定速度收益 |

当前工作区 smoke（本次机器负载偏高，timing 只作跑通记录）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 19.337 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.946 | 2584.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.105 | 2583.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 11.769 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 11.281 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 10.554 | 2574.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 11.786 | 2938.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 47.779 | 12411.7 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.72 DataGrid selected-slots table copy preallocation

本轮优化点：`DataGridSelectedItemsCollection.GetSelectionChangedEventArgs()` 结束时会把当前 `_selectedSlotsTable` 复制到 `_oldSelectedSlotsTable`，作为下一次 selection diff 的旧快照。原实现复制时内部 range list 从 0 容量开始，复制一个连续选区 range 时会按 `List<T>` 默认增长到 4 个槽。现在 `IndexToValueTable.Copy()` 按源表 `_list.Count` 预分配，连续选中 4 行这种 1 range 场景下只分配 1 个槽；清空选择后的空表仍保持 0 容量。

状态验证覆盖：Extended selection + selection column header checkbox，验证 select-all 后 `SelectedItems.Count=4`、旧 selected-slots 快照 range list `Count=1 / Capacity=1`；clear selection 后 `SelectedItems.Count=0`、旧 selected-slots 快照 range list `Count=0 / Capacity=0`。`SelectionChanged` added / removed item 数量和 `SelectedItems` public enumeration 语义保持。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 复制 1 个连续 selected-slot range 后的 range-list 容量 | 4 capacity | 1 capacity | `(4 - 1) / 4` | 75.00% | 有效；避免复制快照时 0→4 的默认扩容 |
| 复制 1 个连续 selected-slot range 后的额外空槽 | 3 empty slots | 0 empty slots | `(3 - 0) / 3` | 100.00% | 有效；只保留实际 range 数量 |
| 清空选择后的旧 selected-slots 快照容量 | 0 capacity | 0 capacity | behavior verified | n/a | 正确性保持；空快照不额外分配 |
| SelectAll / ClearSelection 后 `SelectedItems` 枚举数量 | 4 items / 0 items | 4 items / 0 items | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 `SelectionChanged` diff 完成后复制 selected slots 快照时触发，默认 DataGrid 页面加载 smoke 不覆盖该路径；上一轮 smoke 处于高负载区间，本轮页面加载 timing 回落只作为跑通记录，不作为本轮速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 19.337 | 14.628 | `(19.337 - 14.628) / 19.337` | 24.35% | smoke-only；不触发 selected-slots table copy，不作为速度收益证明 |
| DataGrid.Filter.Menu.Closed | 9.946 | 9.555 | `(9.946 - 9.555) / 9.946` | 3.93% | smoke-only；不触发 selected-slots table copy，不作为速度收益证明 |
| DataGrid.Filter.Tree.Closed | 9.105 | 8.733 | `(9.105 - 8.733) / 9.105` | 4.09% | smoke-only；不触发 selected-slots table copy，不作为速度收益证明 |
| DataGrid.RowHeaders | 11.769 | 9.875 | `(11.769 - 9.875) / 11.769` | 16.09% | smoke-only；不触发 selected-slots table copy，不作为速度收益证明 |
| DataGrid.RowDetails.Collapsed | 11.281 | 10.275 | `(11.281 - 10.275) / 11.281` | 8.92% | smoke-only；不触发 selected-slots table copy，不作为速度收益证明 |
| DataGrid.GroupHeaders | 10.554 | 9.522 | `(10.554 - 9.522) / 10.554` | 9.78% | smoke-only；不触发 selected-slots table copy，不作为速度收益证明 |
| DataGrid.RowGroups | 11.786 | 10.979 | `(11.786 - 10.979) / 11.786` | 6.85% | smoke-only；不触发 selected-slots table copy，不作为速度收益证明 |
| DataGrid.GalleryShape | 47.779 | 44.862 | `(47.779 - 44.862) / 47.779` | 6.10% | smoke-only；不作为本轮确定速度收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.628 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.555 | 2587.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.733 | 2584.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.875 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.275 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.522 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.979 | 2938.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.862 | 12411.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.73 DataGrid CollectionView Reset empty-check fast path

本轮优化点：`DataGridCollectionView.ProcessCollectionChanged(Reset)` 原先为了判断 source reset 后是否为空，会直接 `SourceCollection.GetEnumerator()` 并调用一次 `MoveNext()`。多数数据源同时实现 `ICollection`，例如 `ArrayList`、`List<T>`、`ObservableCollection<T>`；这种情况下空集合判断可以直接读取 `Count`。现在 `ICollection` source 的 Reset 空检查走 `Count == 0`，普通 `IEnumerable` 仍保留原来的枚举 fallback。

状态验证覆盖：使用一个同时实现 `ICollection` 和 `INotifyCollectionChanged` 的计数型 source，触发 clear + Reset 后验证 `DataGridCollectionView.Count=0`，并且 source 在 Reset 后只发生刷新所需的 1 次枚举；空检查本身不再额外创建枚举器。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `ICollection` source 每次 Reset 空检查枚举器 | 1 extra enumerator | 0 extra enumerators | `(1 - 0) / 1` | 100.00% | 有效；空检查直接读 `Count` |
| `ICollection` source 每次 Reset 空检查 `MoveNext()` | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；不再为判断是否为空启动枚举 |
| Reset-to-empty 路径总枚举次数 | 2 enumerations | 1 enumeration | `(2 - 1) / 2` | 50.00% | 有效；剩余 1 次是刷新 view 必需枚举 |
| Reset 后 view item count | 0 items | 0 items | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 source collection 触发 `NotifyCollectionChangedAction.Reset` 时命中，默认 DataGrid 页面加载 smoke 不覆盖该动态 Reset 热路径；本轮 timing 只作异常检查，不声明页面加载速度收益或回归：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.628 | 15.518 | `(14.628 - 15.518) / 14.628` | -6.08% | smoke-only；不触发 collection reset，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 9.555 | 10.022 | `(9.555 - 10.022) / 9.555` | -4.89% | smoke-only；不触发 collection reset，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 8.733 | 9.026 | `(8.733 - 9.026) / 8.733` | -3.36% | smoke-only；不触发 collection reset，不作为速度回归结论 |
| DataGrid.RowHeaders | 9.875 | 10.494 | `(9.875 - 10.494) / 9.875` | -6.27% | smoke-only；不触发 collection reset，不作为速度回归结论 |
| DataGrid.RowDetails.Collapsed | 10.275 | 10.910 | `(10.275 - 10.910) / 10.275` | -6.18% | smoke-only；不触发 collection reset，不作为速度回归结论 |
| DataGrid.GroupHeaders | 9.522 | 9.286 | `(9.522 - 9.286) / 9.522` | 2.48% | smoke-only；不触发 collection reset，不作为速度收益证明 |
| DataGrid.RowGroups | 10.979 | 10.410 | `(10.979 - 10.410) / 10.979` | 5.18% | smoke-only；不触发 collection reset，不作为速度收益证明 |
| DataGrid.GalleryShape | 44.862 | 46.064 | `(44.862 - 46.064) / 44.862` | -2.68% | smoke-only；不作为本轮确定速度回归 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.518 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.022 | 2586.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.026 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.494 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.910 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.286 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.410 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 46.064 | 12412.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.74 DataGrid sort description key selector delegate cache

本轮优化点：`DataGridSortDescription` 的 base comparer sort 路径原来在每次 `OrderBy()` / `ThenBy()` 调用时创建 `o => o` key selector；path sort 路径原来在每次 `OrderBy()` / `ThenBy()` 调用时用 `GetValue` method group 创建 key selector delegate。现在 comparer sort 复用 static identity selector，path sort 在 sort description 构造时缓存 `_getValue` delegate，排序链路重复调用不再分配短命 selector 委托。

状态验证覆盖：comparer sort 使用 cached identity selector 后仍按 comparer 顺序排序；path sort 先走真实 `DataGridCollectionView.SortList()` 调用链中的 `Initialize(itemType)`，再验证升序 `[1,2,3]`、切换降序 `[3,2,1]`，并确认 `_getValue` delegate 在重复 `OrderBy()` / `ThenBy()` 调用后保持同一引用。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Comparer sort `OrderBy()` / `ThenBy()` key selector delegate | 1 delegate / call | 0 delegates / call | `(1 - 0) / 1` | 100.00% | 有效；static identity selector 复用 |
| Path sort `OrderBy()` / `ThenBy()` key selector delegate | 1 method-group delegate / call | 0 delegates / call after construction | `(1 - 0) / 1` | 100.00% | 有效；`_getValue` cached delegate 复用 |
| Repeated path sort `OrderBy()` + `ThenBy()` selector delegates | 2 delegates | 0 delegates | `(2 - 0) / 2` | 100.00% | 有效；排序链重复调用不再生成短命 selector |
| Sort ordering semantics | ascending / descending expected order | same expected order | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid 排序链 `SortList()` 调用 `OrderBy()` / `ThenBy()` 时命中，默认 DataGrid 页面加载 smoke 不主动触发列排序；本轮多项页面 timing 偏慢只记录为 smoke-only，不作为排序路径速度回归结论：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.518 | 15.574 | `(15.518 - 15.574) / 15.518` | -0.36% | smoke-only；不触发排序链，不作为速度回归结论 |
| DataGrid.Filter.Menu.Closed | 10.022 | 11.498 | `(10.022 - 11.498) / 10.022` | -14.73% | smoke-only；不触发排序链，不作为速度回归结论 |
| DataGrid.Filter.Tree.Closed | 9.026 | 10.908 | `(9.026 - 10.908) / 9.026` | -20.85% | smoke-only；不触发排序链，不作为速度回归结论 |
| DataGrid.RowHeaders | 10.494 | 10.694 | `(10.494 - 10.694) / 10.494` | -1.91% | smoke-only；不触发排序链，不作为速度回归结论 |
| DataGrid.RowDetails.Collapsed | 10.910 | 12.372 | `(10.910 - 12.372) / 10.910` | -13.40% | smoke-only；不触发排序链，不作为速度回归结论 |
| DataGrid.GroupHeaders | 9.286 | 10.458 | `(9.286 - 10.458) / 9.286` | -12.62% | smoke-only；不触发排序链，不作为速度回归结论 |
| DataGrid.RowGroups | 10.410 | 11.950 | `(10.410 - 11.950) / 10.410` | -14.79% | smoke-only；不触发排序链，不作为速度回归结论 |
| DataGrid.GalleryShape | 46.064 | 53.578 | `(46.064 - 53.578) / 46.064` | -16.31% | smoke-only；不作为本轮确定速度回归 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.574 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 11.498 | 2586.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 10.908 | 2583.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.694 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 12.372 | 3025.2 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 10.458 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 11.950 | 2938.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 53.578 | 12411.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.75 DataGrid PrepareLocalArray no-filter copy fast path

本轮优化点：`DataGridCollectionView.PrepareLocalArray()` 会在排序、分页、分组或过滤 refresh 时重建本地数组。原实现即使 `_filter == null`，也会对每个 item 执行一次 `Filter == null || PassesFilter(item)` 分支判断。现在无 Filter 路径直接复制 source item；只有设置 Filter 时才进入 `PassesFilter()` 分支。

状态验证覆盖：已有 `VerifyDataGridCollectionViewPreallocatesUnfilteredSourceLists` 覆盖无过滤 source copy 的 item 数量、顺序和 `ICollection.Count` 预分配容量；同一验证也覆盖设置 Filter 后 filtered refresh 仍只保留匹配项，且不过度按全量 source count 预分配。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| No-filter `PrepareLocalArray()` per-item filter branch | 1 branch / item | 0 branches / item | `(1 - 0) / 1` | 100.00% | 有效；无过滤 refresh 直接复制 |
| No-filter `PrepareLocalArray()` `PassesFilter()` call opportunity | 1 guarded callsite / item | 0 callsites / item | `(1 - 0) / 1` | 100.00% | 有效；Filter 为空时不进入过滤逻辑 |
| Filtered `PrepareLocalArray()` behavior | matching items only | matching items only | behavior verified | n/a | 正确性保持 |
| Unfiltered source copy count/capacity | source count / source count | source count / source count | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮改动命中需要 `PrepareLocalArray()` 的刷新路径；默认页面加载 smoke 会覆盖部分无过滤 sort/page/group 重建路径，但单次 timing 仍只作异常检查。`DataGrid.Basic` 本轮明显偏慢，其他场景多为回落，不把单次 smoke 当成确定速度结论：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.574 | 19.394 | `(15.574 - 19.394) / 15.574` | -24.53% | smoke-only；本轮 Basic 波动偏慢，不作为确定回归结论 |
| DataGrid.Filter.Menu.Closed | 11.498 | 10.430 | `(11.498 - 10.430) / 11.498` | 9.29% | smoke-only；只作趋势记录 |
| DataGrid.Filter.Tree.Closed | 10.908 | 10.767 | `(10.908 - 10.767) / 10.908` | 1.29% | smoke-only；只作趋势记录 |
| DataGrid.RowHeaders | 10.694 | 10.638 | `(10.694 - 10.638) / 10.694` | 0.52% | smoke-only；只作趋势记录 |
| DataGrid.RowDetails.Collapsed | 12.372 | 11.363 | `(12.372 - 11.363) / 12.372` | 8.16% | smoke-only；只作趋势记录 |
| DataGrid.GroupHeaders | 10.458 | 10.226 | `(10.458 - 10.226) / 10.458` | 2.22% | smoke-only；只作趋势记录 |
| DataGrid.RowGroups | 11.950 | 11.919 | `(11.950 - 11.919) / 11.950` | 0.26% | smoke-only；只作趋势记录 |
| DataGrid.GalleryShape | 53.578 | 48.588 | `(53.578 - 48.588) / 53.578` | 9.31% | smoke-only；只作趋势记录 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 19.394 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.430 | 2586.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 10.767 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.638 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 11.363 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 10.226 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 11.919 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 48.588 | 12412.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.76 DataGrid empty paged enumerator shared array

本轮优化点：`DataGridCollectionView.GetEnumerator()` 在分页模式且 `PageIndex < 0` 时表示异步分页加载中的空结果。原实现会创建一个空 `List<object?>` 只为了返回空枚举器；现在改为复用静态空数组 `EmptyPagedItems` 的枚举器。这个路径不改变枚举结果，只移除空分页状态下每次枚举的临时 `List<object?>` 分配。

状态验证覆盖：新增 `VerifyDataGridCollectionViewEmptyPagedEnumeratorUsesSharedEmptyArray`，构造 `PageSize=2` 的 collection view，通过反射把 `_pageIndex` 置为 `-1`，验证共享空数组存在，且 public `GetEnumerator()` 在空分页状态下不产生任何 item。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Empty paged `GetEnumerator()` list allocation | 1 empty `List<object?>` / call | 0 lists / call | `(1 - 0) / 1` | 100.00% | 有效；空分页状态不再分配临时 list |
| Empty paged enumerated item count | 0 items | 0 items | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 `PageSize > 0 && PageIndex < 0` 的空分页枚举状态命中；默认 DataGrid 页面加载 smoke 不覆盖该异步分页空状态。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 19.394 | 16.877 | `(19.394 - 16.877) / 19.394` | 12.98% | smoke-only；不覆盖空分页状态，不作为确定速度收益 |
| DataGrid.Filter.Menu.Closed | 10.430 | 10.097 | `(10.430 - 10.097) / 10.430` | 3.19% | smoke-only；不覆盖空分页状态 |
| DataGrid.Filter.Tree.Closed | 10.767 | 10.871 | `(10.767 - 10.871) / 10.767` | -0.97% | smoke-only；不作为速度回归结论 |
| DataGrid.RowHeaders | 10.638 | 24.611 | `(10.638 - 24.611) / 10.638` | -131.35% | smoke-only；本轮明显环境/单次 outlier，不覆盖优化路径 |
| DataGrid.RowDetails.Collapsed | 11.363 | 11.571 | `(11.363 - 11.571) / 11.363` | -1.83% | smoke-only；不作为速度回归结论 |
| DataGrid.GroupHeaders | 10.226 | 9.613 | `(10.226 - 9.613) / 10.226` | 6.00% | smoke-only；不覆盖空分页状态 |
| DataGrid.RowGroups | 11.919 | 10.651 | `(11.919 - 10.651) / 11.919` | 10.64% | smoke-only；不覆盖空分页状态 |
| DataGrid.GalleryShape | 48.588 | 46.451 | `(48.588 - 46.451) / 48.588` | 4.40% | smoke-only；不作为本轮确定页面加载收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 16.877 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.097 | 2586.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 10.871 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 24.611 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing outlier，只作记录 |
| DataGrid.RowDetails.Collapsed | 11.571 | 3024.0 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.613 | 2574.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.651 | 2939.9 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 46.451 | 12412.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.77 DataGrid selection-inclusive start-slot struct enumerator

本轮优化点：`DataGrid.GetSelectionInclusive(startRowIndex, endRowIndex)` 用于按范围返回已选中 item。原路径通过 `_selectedItems.GetSlots(startSlot)` 调到 `IndexToValueTable.GetIndexes(startIndex)`，会为 selected-slot 起始枚举创建一个 yield iterator。现在给已有的 `IndexToValueTable.IndexEnumerable` / `IndexEnumerator` 增加 `startIndex` 能力，`DataGridSelectedItemsCollection.GetSlots(startSlot)` 直接返回 struct enumerable，range selection 内部 slot 遍历不再分配该 yield iterator。

状态验证覆盖：新增 `VerifyDataGridSelectionInclusiveUsesStructSlotEnumerator`，验证 `GetSlots(startSlot)` 返回 value-type `IndexEnumerable`；构造 5 行 Extended selection、SelectAll 后调用 `GetSelectionInclusive(1, 3)`，验证返回第 2 到第 4 行且顺序保持。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Range selection selected-slot iterator allocation | 1 `GetIndexes(start)` yield iterator / enumeration | 0 selected-slot yield iterators / enumeration | `(1 - 0) / 1` | 100.00% | 有效；起始 slot 枚举走 struct enumerator |
| `GetSelectionInclusive(1,3)` item order | rows 1,2,3 | rows 1,2,3 | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 range selection / shift selection 这类选择范围枚举时触发；默认 DataGrid 页面加载 smoke 不覆盖该路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 16.877 | 14.692 | `(16.877 - 14.692) / 16.877` | 12.95% | smoke-only；不覆盖 range selection，不作为确定速度收益 |
| DataGrid.Filter.Menu.Closed | 10.097 | 9.287 | `(10.097 - 9.287) / 10.097` | 8.02% | smoke-only；不覆盖 range selection |
| DataGrid.Filter.Tree.Closed | 10.871 | 8.227 | `(10.871 - 8.227) / 10.871` | 24.32% | smoke-only；不覆盖 range selection |
| DataGrid.RowHeaders | 24.611 | 9.683 | `(24.611 - 9.683) / 24.611` | 60.66% | smoke-only；上一轮 RowHeaders 是 outlier，不作为本轮收益证明 |
| DataGrid.RowDetails.Collapsed | 11.571 | 9.973 | `(11.571 - 9.973) / 11.571` | 13.81% | smoke-only；不覆盖 range selection |
| DataGrid.GroupHeaders | 9.613 | 8.628 | `(9.613 - 8.628) / 9.613` | 10.25% | smoke-only；不覆盖 range selection |
| DataGrid.RowGroups | 10.651 | 9.786 | `(10.651 - 9.786) / 10.651` | 8.12% | smoke-only；不覆盖 range selection |
| DataGrid.GalleryShape | 46.451 | 43.199 | `(46.451 - 43.199) / 46.451` | 7.00% | smoke-only；不作为本轮确定页面加载收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.692 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.287 | 2588.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.227 | 2584.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.683 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.973 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.628 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.786 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.199 | 12412.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.78 DataGrid filter selected-values list capacity preallocation

本轮优化点：menu/tree filter presenter 在确认关闭时会收集 checked leaf 的 `FilterValue`。原路径从空 `List<string>` 开始追加 selected values，单选会触发默认容量 `0 -> 4`，6 个 checked values 会触发 `0 -> 4 -> 8`。现在 `GetFilterValues()` 先统计已选中的 leaf 数量，再用该数量构造 `List<string>`；空选返回共享 0 容量空列表，少选不会按候选总数过度预分配。

正确性补强：递归读取 filter item 时仍优先用 `ContainerFromIndex()`，但对嵌套 menu/tree 子项增加 `Items` fallback。因为嵌套 filter 子项可能还没有实际 container，状态验证改用两组嵌套 filter，覆盖 menu/tree 空选、单选、6 个全选和嵌套 leaf 收集。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Filter selected-values list capacity for 1 checked nested value / close | 4 capacity after first add | 1 capacity preallocated | `(4 - 1) / 4` | 75.00% | 有效；单选不再持有默认 4 容量 |
| Filter selected-values list capacity for 6 checked nested values / close | 8 capacity after growth | 6 capacity preallocated | `(8 - 6) / 8` | 25.00% | 有效；全选容量等于 selected value count |
| Filter selected-values list Add-time backing-array growth for 6 checked nested values / close | 2 growths (`0 -> 4 -> 8`) | 0 Add-time growths | `(2 - 0) / 2` | 100.00% | 有效；收集 selected values 时不再边追加边扩容 |
| Empty filter selected-values list capacity / close | 0 capacity | 0 capacity | behavior verified | n/a | 正确性保持；空选不额外分配 backing array |
| Empty filter selected-values list object allocations / empty close | 1 `List<string>` | 0 new lists | `(1 - 0) / 1` | 100.00% | 有效；menu/tree 空选、reset 空选和未 materialized fallback 复用共享空列表 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 filter flyout 已 materialize 且关闭时收集 selected values 触发；默认 DataGrid 页面加载 smoke 不覆盖该确认关闭路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.692 | 14.865 | `(14.692 - 14.865) / 14.692` | -1.18% | smoke-only；不覆盖 filter selected-values close 路径 |
| DataGrid.Filter.Menu.Closed | 9.287 | 9.521 | `(9.287 - 9.521) / 9.287` | -2.52% | smoke-only；closed page load 不确认 filter values |
| DataGrid.Filter.Tree.Closed | 8.227 | 8.492 | `(8.227 - 8.492) / 8.227` | -3.22% | smoke-only；closed page load 不确认 filter values |
| DataGrid.RowHeaders | 9.683 | 9.920 | `(9.683 - 9.920) / 9.683` | -2.45% | smoke-only；不覆盖本轮路径 |
| DataGrid.RowDetails.Collapsed | 9.973 | 10.362 | `(9.973 - 10.362) / 9.973` | -3.90% | smoke-only；不覆盖本轮路径 |
| DataGrid.GroupHeaders | 8.628 | 9.144 | `(8.628 - 9.144) / 8.628` | -5.98% | smoke-only；不覆盖本轮路径 |
| DataGrid.RowGroups | 9.786 | 10.060 | `(9.786 - 10.060) / 9.786` | -2.80% | smoke-only；不覆盖本轮路径 |
| DataGrid.GalleryShape | 43.199 | 44.357 | `(43.199 - 44.357) / 43.199` | -2.68% | smoke-only；不作为本轮确定页面加载收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.865 | 2949.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.521 | 2587.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.492 | 2584.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.920 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.362 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.144 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.060 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.357 | 12412.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.79 DataGrid filter item children lazy allocation

本轮优化点：`DataGridFilterItem` 原来构造时就会为 `Children` 创建空 `List<DataGridFilterItem>`。这对 leaf filter item 是纯成本；标准 performance filter grid 有 7 个 filter items，其中 6 个是 leaf。现在 `Children` 改为首次读取时懒创建，内部 menu/tree materialize 用 `HasChildren` 判断，避免为了 `Children.Count` 检查而把 leaf 的空列表创建出来。

正确性补强：状态验证覆盖 public `Children` getter 仍返回可变空列表、collection initializer 仍能向 `Children` 添加子项、`HasChildren` 不会 materialize leaf children，以及 menu/tree flyout content materialize 后 root/nested leaf filter item 的 children backing field 仍为 null。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| DataGrid.Filter.*.Closed filter item child lists / grid | 7 lists | 1 list | `(7 - 1) / 7` | 85.71% | 有效；只有 parent filter item 创建 Children list |
| DataGrid.Filter.*.Closed leaf filter empty child lists / grid | 6 empty lists | 0 empty lists | `(6 - 0) / 6` | 100.00% | 有效；leaf filters 不再默认分配空 list |
| DataGrid.GalleryShape leaf filter empty child lists / page | 12 empty lists | 0 empty lists | `(12 - 0) / 12` | 100.00% | 有效；menu/tree 两个 filter grids 均受益 |
| Public `Children` getter mutability | mutable empty list | mutable empty list | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮构造路径被 `DataGrid.Filter.Menu.Closed` / `Tree.Closed` / `GalleryShape` 覆盖；optimized 复测两轮后 KB 方向稳定，timing 仍混合波动，所以 KB 仅作辅助分配信号，timing 只作异常检查：

| Scenario | baseline KB/item | optimized avg KB/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Filter.Menu.Closed | 2587.1 | 2586.55 | `(2587.1 - 2586.55) / 2587.1` | 0.02% | 正向；约 0.55 KB/item |
| DataGrid.Filter.Tree.Closed | 2584.7 | 2584.05 | `(2584.7 - 2584.05) / 2584.7` | 0.03% | 正向；约 0.65 KB/item |
| DataGrid.GalleryShape | 12412.1 | 12411.5 | `(12412.1 - 12411.5) / 12412.1` | 0.005% | 小幅正向；约 0.6 KB/page |

Smoke-only timing（optimized 两轮均值）：

| Scenario | baseline ms/item | optimized avg ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.865 | 14.591 | `(14.865 - 14.591) / 14.865` | 1.84% | smoke-only；不覆盖 filter item 构造 |
| DataGrid.Filter.Menu.Closed | 9.521 | 9.757 | `(9.521 - 9.757) / 9.521` | -2.48% | smoke-only；KB 正向，timing 不作本轮回归结论 |
| DataGrid.Filter.Tree.Closed | 8.492 | 8.754 | `(8.492 - 8.754) / 8.492` | -3.08% | smoke-only；KB 正向，timing 不作本轮回归结论 |
| DataGrid.RowHeaders | 9.920 | 10.029 | `(9.920 - 10.029) / 9.920` | -1.09% | smoke-only；不覆盖本轮路径 |
| DataGrid.RowDetails.Collapsed | 10.362 | 10.475 | `(10.362 - 10.475) / 10.362` | -1.09% | smoke-only；不覆盖本轮路径 |
| DataGrid.GroupHeaders | 9.144 | 8.948 | `(9.144 - 8.948) / 9.144` | 2.14% | smoke-only；不覆盖本轮路径 |
| DataGrid.RowGroups | 10.060 | 9.983 | `(10.060 - 9.983) / 10.060` | 0.77% | smoke-only；不覆盖本轮路径 |
| DataGrid.GalleryShape | 44.357 | 43.324 | `(44.357 - 43.324) / 44.357` | 2.33% | smoke-only；只作趋势记录 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.612 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.309 | 2586.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.230 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.478 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.206 | 3025.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.041 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.055 | 2938.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.881 | 12411.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.80 DataGrid column group tree direct add/remove

本轮优化点：`DataGrid.ColumnGroups` add/remove 处理列分组树时，上一轮已经去掉了递归每层的临时列表，但每个 add/remove 操作仍会创建一个 operation-level `List<DataGridColumn>` 先收集 leaf columns，再二次遍历同步 `ColumnsInternal`。现在递归时直接 add/remove leaf columns，去掉操作级临时列表和第二次遍历。

正确性补强：旧 remove 路径先把 top-level group 的 `OwningGrid` 设为 null，但随后递归收集又把 group owner 设回 grid。新路径 add 时给 top-level/nested group 设置 owner，remove 时 top-level/nested group 都释放 owner。状态验证覆盖 add 后 depth-first leaf columns 顺序、remove 后 leaf columns 不残留、top-level/nested group owner add/remove 语义。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Column group tree operation leaf-column lists / add | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 有效；递归直接 add leaf columns |
| Column group tree operation leaf-column lists / remove | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 有效；递归直接 remove leaf columns |
| Column group tree second leaf-column traversal / add-remove pair | 2 traversals | 0 traversals | `(2 - 0) / 2` | 100.00% | 有效；不再收集后再次遍历临时列表 |
| Removed group item retained owning-grid references / verified tree | 2 retained refs | 0 retained refs | `(2 - 0) / 2` | 100.00% | 正确性/生命周期修复 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 column group add/remove 时触发；`DataGrid.GroupHeaders` / `GalleryShape` 覆盖初始 add，但不覆盖 remove。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.612 | 15.285 | `(14.612 - 15.285) / 14.612` | -4.61% | smoke-only；不覆盖 column group add/remove |
| DataGrid.Filter.Menu.Closed | 9.309 | 9.344 | `(9.309 - 9.344) / 9.309` | -0.38% | smoke-only；不覆盖本轮路径 |
| DataGrid.Filter.Tree.Closed | 9.230 | 8.689 | `(9.230 - 8.689) / 9.230` | 5.86% | smoke-only；不覆盖本轮路径 |
| DataGrid.RowHeaders | 10.478 | 9.626 | `(10.478 - 9.626) / 10.478` | 8.13% | smoke-only；不覆盖本轮路径 |
| DataGrid.RowDetails.Collapsed | 10.206 | 10.121 | `(10.206 - 10.121) / 10.206` | 0.83% | smoke-only；不覆盖本轮路径 |
| DataGrid.GroupHeaders | 9.041 | 9.060 | `(9.041 - 9.060) / 9.041` | -0.21% | smoke-only；覆盖 add，但波动内，不作速度回归结论 |
| DataGrid.RowGroups | 10.055 | 9.941 | `(10.055 - 9.941) / 10.055` | 1.13% | smoke-only；不覆盖本轮路径 |
| DataGrid.GalleryShape | 43.881 | 43.832 | `(43.881 - 43.832) / 43.881` | 0.11% | smoke-only；覆盖 add，只作异常检查 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.285 | 2948.9 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.344 | 2586.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.689 | 2584.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.626 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.121 | 3025.0 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.060 | 2574.0 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.941 | 2938.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.832 | 12411.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.81 DataGrid plain column header template apply direct display-index iteration

本轮优化点：普通 DataGrid 在 `OnApplyTemplate()` 创建 column headers 时，旧路径会复制一份 `ColumnsItemsInternal` 到 `List<DataGridColumn>`，再用 `DisplayIndexComparer` 排序，最后按排序结果插入 header。`ColumnsInternal.DisplayIndexMap` 已维护同一套显示顺序，所以现在直接按 display index 读取列并插入 header，去掉临时 list、comparer 和排序调用。

正确性补强：新增状态验证覆盖 4 列乱序 `DisplayIndex`，确认 template apply 后 presenter 内 public column headers 仍按显示顺序排列；filler/header 槽位不计入 public columns。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Plain DataGrid template-apply column header list / grid template apply | 1 `List<DataGridColumn>` copy | 0 lists | `(1 - 0) / 1` | 100.00% | 有效；直接按 `DisplayIndexMap` 遍历 |
| Plain DataGrid template-apply display-index comparer / grid template apply | 1 comparer | 0 comparers | `(1 - 0) / 1` | 100.00% | 有效；不再排序临时列列表 |
| Plain DataGrid template-apply column sort / grid template apply | 1 `List.Sort()` over columns | 0 sort calls | `(1 - 0) / 1` | 100.00% | 有效；display order 已由 `DisplayIndexMap` 维护 |
| Template apply DisplayIndex order verification / reordered 4-column grid | not covered | covered | n/a | correctness coverage added | 正确性防护；乱序列头顺序保持 |

Smoke-only 对比上一轮同参数复测。本轮路径覆盖普通 DataGrid 套模板；`DataGrid.GroupHeaders` 自身走 column group view，不把它的 timing 当成本轮直接收益证明。下表只作异常检查，不把单次 timing 当成稳定页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.285 | 13.538 | `(15.285 - 13.538) / 15.285` | 11.43% | smoke-only；覆盖普通列头套模板路径 |
| DataGrid.Filter.Menu.Closed | 9.344 | 9.023 | `(9.344 - 9.023) / 9.344` | 3.44% | smoke-only；覆盖普通列头套模板路径 |
| DataGrid.Filter.Tree.Closed | 8.689 | 8.079 | `(8.689 - 8.079) / 8.689` | 7.02% | smoke-only；覆盖普通列头套模板路径 |
| DataGrid.RowHeaders | 9.626 | 9.553 | `(9.626 - 9.553) / 9.626` | 0.76% | smoke-only；覆盖普通列头套模板路径 |
| DataGrid.RowDetails.Collapsed | 10.121 | 9.848 | `(10.121 - 9.848) / 10.121` | 2.70% | smoke-only；覆盖普通列头套模板路径 |
| DataGrid.GroupHeaders | 9.060 | 8.771 | `(9.060 - 8.771) / 9.060` | 3.19% | smoke-only；不覆盖普通列头套模板主路径，按异常检查记录 |
| DataGrid.RowGroups | 9.941 | 9.768 | `(9.941 - 9.768) / 9.941` | 1.74% | smoke-only；覆盖普通列头套模板路径 |
| DataGrid.GalleryShape | 43.832 | 42.297 | `(43.832 - 42.297) / 43.832` | 3.50% | smoke-only；部分覆盖普通列头套模板路径，只作趋势记录 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.538 | 2948.8 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.023 | 2586.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.079 | 2583.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.553 | 3107.8 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.848 | 3025.1 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.771 | 2574.0 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.768 | 2938.4 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.297 | 12411.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.82 DataGrid data connection editable attribute no-attribute lookup cleanup

本轮优化点：`DataGridDataConnection.GetPropertyIsReadOnly()` 判断绑定属性是否只读时，旧路径对每个 property path segment 都直接调用 `GetCustomAttributes(typeof(EditableAttribute), true)`。绝大多数数据模型属性没有 `EditableAttribute`，这会在常见路径上创建空 attribute array。现在先用 `IsDefined(typeof(EditableAttribute), true)` 判断，只有确实存在 attribute 时才沿用原来的 `GetCustomAttributes()` 读取逻辑。

正确性补强：新增状态验证覆盖普通可写属性、`Editable(false)`、`Editable(true)`、getter-only 属性、nested writable path 和 nested `Editable(false)` path，确保只跳过无 attribute 的空数组路径，不改变编辑语义。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| DataConnection editable check no-attribute `GetCustomAttributes` calls / property segment | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；常见无 `EditableAttribute` 属性跳过空数组创建 |
| DataConnection editable check no-attribute `GetCustomAttributes` calls / nested 2-segment path | 2 calls | 0 calls | `(2 - 0) / 2` | 100.00% | 有效；nested path 每段都先走 `IsDefined` |
| Editable(false/true) read-only semantics / verified cases | covered by old behavior only | preserved + verified | n/a | correctness coverage added | 正确性防护；false/true/getter-only/nested 都覆盖 |

Smoke-only 对比上一轮同参数复测。本轮路径属于编辑/只读判断，不是默认 closed Gallery 页面加载主路径；下表只作异常检查，不把单次 timing 当成本轮确定页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.538 | 14.028 | `(13.538 - 14.028) / 13.538` | -3.62% | smoke-only；不覆盖 editable check 热路径 |
| DataGrid.Filter.Menu.Closed | 9.023 | 7.717 | `(9.023 - 7.717) / 9.023` | 14.47% | smoke-only；不覆盖 editable check 热路径 |
| DataGrid.Filter.Tree.Closed | 8.079 | 7.018 | `(8.079 - 7.018) / 8.079` | 13.13% | smoke-only；不覆盖 editable check 热路径 |
| DataGrid.RowHeaders | 9.553 | 8.052 | `(9.553 - 8.052) / 9.553` | 15.71% | smoke-only；不覆盖 editable check 热路径 |
| DataGrid.RowDetails.Collapsed | 9.848 | 8.561 | `(9.848 - 8.561) / 9.848` | 13.07% | smoke-only；不覆盖 editable check 热路径 |
| DataGrid.GroupHeaders | 8.771 | 7.708 | `(8.771 - 7.708) / 8.771` | 12.12% | smoke-only；不覆盖 editable check 热路径 |
| DataGrid.RowGroups | 9.768 | 8.165 | `(9.768 - 8.165) / 9.768` | 16.41% | smoke-only；不覆盖 editable check 热路径 |
| DataGrid.GalleryShape | 42.297 | 36.933 | `(42.297 - 36.933) / 42.297` | 12.68% | smoke-only；不覆盖 editable check 热路径，只作趋势记录 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.028 | 2948.8 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 7.717 | 2584.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.018 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 8.052 | 3107.7 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 8.561 | 3025.2 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 7.708 | 2574.7 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 8.165 | 2938.4 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 36.933 | 12410.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.83 DataGrid TypeHelper read-only attribute no-attribute lookup cleanup

本轮优化点：`TypeHelper.GetIsReadOnly()` 判断 `Type` / `PropertyInfo` 是否带 `ReadOnlyAttribute` 时，旧路径直接调用 `GetCustomAttributes(typeof(ReadOnlyAttribute), true)`。DataGrid 编辑只读判断会对每个 property path segment 检查 `propertyType.GetIsReadOnly()` 和 `propertyInfo.GetIsReadOnly()`；绝大多数业务模型没有 `ReadOnlyAttribute`，旧路径会反复创建空 attribute array。现在先用 `IsDefined(typeof(ReadOnlyAttribute), true)` 判断，只有确实存在 attribute 时才读取 attribute 实例。

正确性补强：状态验证在原有 `Editable(false/true)`、getter-only、nested path 基础上，新增覆盖 property-level `ReadOnly(true)`、property-level `ReadOnly(false)` 和 type-level `ReadOnly(true)`，确保优化只跳过无 attribute 的空数组路径，不改变只读语义。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| TypeHelper read-only check no-attribute `GetCustomAttributes` calls / member | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；常见无 `ReadOnlyAttribute` member 跳过空数组创建 |
| DataConnection editable check no-attribute read-only lookups / writable property segment | 2 calls | 0 calls | `(2 - 0) / 2` | 100.00% | 有效；property type + property info 两次只读检查都受益 |
| `ReadOnly(true/false)` semantics / verified cases | old behavior only | preserved + verified | n/a | correctness coverage added | 正确性防护；property-level true/false 和 type-level true 都覆盖 |

Smoke-only 对比上一轮同参数复测。本轮路径属于编辑/只读判断，不是默认 closed Gallery 页面加载主路径；下表只作异常检查，不把单次 timing 当成本轮确定页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.028 | 14.566 | `(14.028 - 14.566) / 14.028` | -3.84% | smoke-only；不覆盖 read-only attribute check 热路径 |
| DataGrid.Filter.Menu.Closed | 7.717 | 9.037 | `(7.717 - 9.037) / 7.717` | -17.11% | smoke-only；不覆盖 read-only attribute check 热路径 |
| DataGrid.Filter.Tree.Closed | 7.018 | 8.125 | `(7.018 - 8.125) / 7.018` | -15.77% | smoke-only；不覆盖 read-only attribute check 热路径 |
| DataGrid.RowHeaders | 8.052 | 9.546 | `(8.052 - 9.546) / 8.052` | -18.55% | smoke-only；不覆盖 read-only attribute check 热路径 |
| DataGrid.RowDetails.Collapsed | 8.561 | 9.892 | `(8.561 - 9.892) / 8.561` | -15.55% | smoke-only；不覆盖 read-only attribute check 热路径 |
| DataGrid.GroupHeaders | 7.708 | 8.775 | `(7.708 - 8.775) / 7.708` | -13.84% | smoke-only；不覆盖 read-only attribute check 热路径 |
| DataGrid.RowGroups | 8.165 | 9.635 | `(8.165 - 9.635) / 8.165` | -18.00% | smoke-only；不覆盖 read-only attribute check 热路径 |
| DataGrid.GalleryShape | 36.933 | 42.050 | `(36.933 - 42.050) / 36.933` | -13.85% | smoke-only；不覆盖 read-only attribute check 热路径，只作异常检查 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.566 | 2948.8 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.037 | 2586.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.125 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.546 | 3107.7 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.892 | 3025.0 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.775 | 2573.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.635 | 2938.4 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.050 | 12410.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.84 DataGrid display attribute no-attribute lookup cleanup

本轮优化点：DataGrid 自动生成列和分组显示名路径会读取 `DisplayAttribute`。旧路径在每个属性上直接调用 `GetCustomAttributes(typeof(DisplayAttribute), true)`，绝大多数业务模型属性没有 `DisplayAttribute` 时会创建空 attribute array。现在 `TypeHelper.GetDisplayName()` 和 `GenerateColumnsFromProperties()` 都先用 `IsDefined(typeof(DisplayAttribute), true)` 判断，只有确实存在 attribute 时才读取 attribute 实例。

正确性补强：新增状态验证覆盖无 `DisplayAttribute` 返回 null / 使用属性名、`DisplayAttribute.ShortName`、`DisplayAttribute.Name` fallback、`DisplayAttribute.Order` 自动列顺序、`AutoGenerateField=false` 隐藏列，以及生成列仍标记为 `IsAutoGenerated`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| TypeHelper display-name no-attribute `GetCustomAttributes` calls / property | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；无 `DisplayAttribute` 显示名解析跳过空数组创建 |
| Auto-generated column display attribute lookup / plain data property | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；自动生成列的普通属性跳过空数组创建 |
| `DisplayAttribute` auto-generate semantics / verified cases | old behavior only | preserved + verified | n/a | correctness coverage added | 正确性防护；header/order/hidden-column 都覆盖 |

Smoke-only 对比上一轮同参数复测。本轮路径属于自动生成列和显示名解析；当前默认 DataGrid smoke 使用手写列，不覆盖 auto-generate 主路径。下表只作异常检查，不把单次 timing 当成本轮确定页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.566 | 15.086 | `(14.566 - 15.086) / 14.566` | -3.57% | smoke-only；不覆盖 auto-generate display lookup |
| DataGrid.Filter.Menu.Closed | 9.037 | 9.087 | `(9.037 - 9.087) / 9.037` | -0.55% | smoke-only；不覆盖 auto-generate display lookup |
| DataGrid.Filter.Tree.Closed | 8.125 | 8.047 | `(8.125 - 8.047) / 8.125` | 0.96% | smoke-only；不覆盖 auto-generate display lookup |
| DataGrid.RowHeaders | 9.546 | 9.611 | `(9.546 - 9.611) / 9.546` | -0.68% | smoke-only；不覆盖 auto-generate display lookup |
| DataGrid.RowDetails.Collapsed | 9.892 | 9.986 | `(9.892 - 9.986) / 9.892` | -0.95% | smoke-only；不覆盖 auto-generate display lookup |
| DataGrid.GroupHeaders | 8.775 | 8.888 | `(8.775 - 8.888) / 8.775` | -1.29% | smoke-only；分组显示名路径依赖 group property，单轮波动不作回归结论 |
| DataGrid.RowGroups | 9.635 | 9.786 | `(9.635 - 9.786) / 9.635` | -1.57% | smoke-only；不覆盖 auto-generate display lookup |
| DataGrid.GalleryShape | 42.050 | 42.848 | `(42.050 - 42.848) / 42.050` | -1.90% | smoke-only；不作为本轮确定页面加载收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.086 | 2948.8 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.087 | 2586.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.047 | 2584.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.611 | 3107.9 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.986 | 3024.9 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.888 | 2574.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.786 | 2938.4 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.848 | 12411.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.85 DataGrid GetAllRows struct enumerable cleanup

本轮优化点：`DataGrid.GetAllRows()` 旧实现使用 `yield return`，每次遍历已实现行都会创建一个 compiler-generated iterator 对象。该 helper 被列状态刷新、selection column 状态刷新、row data-context 通知、行状态刷新等路径反复调用。现在返回 value-type enumerable/enumerator，遍历 `_rowsPresenter.Children` 时直接跳过非 `DataGridRow` child，不再为每次 traversal 分配 iterator。

正确性补强：新增状态验证通过 reflection 调用 internal `GetAllRows()`，确认返回值是 value type；basic grid 和 grouped grid 都和视觉树中已实现 `DataGridRow` 的数量与顺序一致，确保仍跳过 row group header 等非 row child。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| GetAllRows iterator allocation / realized-row traversal | 1 yield iterator object | 0 iterator objects | `(1 - 0) / 1` | 100.00% | 有效；每次 realized-row 遍历少一个短命对象 |
| GetAllRows non-row child filtering / grouped grid | old yield path | struct enumerator path | n/a | correctness preserved | 正确性防护；grouped grid 仍只返回 `DataGridRow` |
| GetAllRows basic/grouped row sequence verification | not covered | covered | n/a | correctness coverage added | 状态验证覆盖 realized row 数量与顺序 |

Smoke-only 对比上一轮同参数复测。本轮路径是已实现行遍历分配优化；默认 Gallery page load 只间接覆盖部分调用，单次 timing 只作异常检查，不把它当成确定页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.086 | 15.108 | `(15.086 - 15.108) / 15.086` | -0.15% | smoke-only；波动内，只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.087 | 9.439 | `(9.087 - 9.439) / 9.087` | -3.87% | smoke-only；不作为本轮回归结论 |
| DataGrid.Filter.Tree.Closed | 8.047 | 8.241 | `(8.047 - 8.241) / 8.047` | -2.41% | smoke-only；不作为本轮回归结论 |
| DataGrid.RowHeaders | 9.611 | 9.482 | `(9.611 - 9.482) / 9.611` | 1.34% | smoke-only；只作趋势记录 |
| DataGrid.RowDetails.Collapsed | 9.986 | 9.831 | `(9.986 - 9.831) / 9.986` | 1.55% | smoke-only；只作趋势记录 |
| DataGrid.GroupHeaders | 8.888 | 9.075 | `(8.888 - 9.075) / 8.888` | -2.10% | smoke-only；不作为本轮回归结论 |
| DataGrid.RowGroups | 9.786 | 10.013 | `(9.786 - 10.013) / 9.786` | -2.32% | smoke-only；不作为本轮回归结论 |
| DataGrid.GalleryShape | 42.848 | 42.290 | `(42.848 - 42.290) / 42.848` | 1.30% | smoke-only；只作趋势记录 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.108 | 2948.0 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.439 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.241 | 2583.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.482 | 3106.4 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.831 | 3023.2 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.075 | 2573.8 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.013 | 2937.2 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.290 | 12407.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.86 DataGrid row group header slot struct enumeration cleanup

本轮优化点：RowGroupHeadersTable 的分组 header slot 遍历旧路径使用 `GetIndexes()` / `GetIndexes(start)`，每次遍历都会创建一个 yield iterator。`IndexToValueTable` 已经提供 `EnumerateIndexes()` / `EnumerateIndexes(start)` value-type enumerable；现在分组行初始化、slot 插入/删除修正、清理分组订阅、计算分组 header 数量、展开/折叠分组、parent group 查找和 collection-view group 反查都改走 struct enumerator。

正确性补强：新增状态验证在 grouped grid 中通过 reflection 比较 `EnumerateIndexes()` 与旧 `GetIndexes()` 的 full slot 序列，以及 `EnumerateIndexes(start)` 与旧 `GetIndexes(start)` 的 start-index 过滤序列，并断言新路径返回 value-type enumerable。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Row group header slot iterator allocation / traversal | 1 yield iterator object | 0 iterator objects | `(1 - 0) / 1` | 100.00% | 有效；每次分组 slot 遍历少一个短命对象 |
| Row group header slot iterator release callsites | 9 `GetIndexes()` callsites | 0 `GetIndexes()` callsites | `(9 - 0) / 9` | 100.00% | 有效；release 路径全部改为 struct enumerable |
| Row group header full/start slot sequence verification | old yield path only | struct path compared with old path | n/a | correctness coverage added | 正确性防护；full/start 两种序列保持一致 |

Smoke-only 对比上一轮同参数复测。本轮路径直接覆盖 grouped row slot 遍历，`DataGrid.RowGroups` 更接近本轮目标；其它场景只作异常检查。第一次复测 `RowDetails.Collapsed` 出现非目标路径 outlier，已同参数复测，下表记录复测后的当前工作区结果：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.108 | 15.563 | `(15.108 - 15.563) / 15.108` | -3.01% | smoke-only；不覆盖本轮分组 slot 主路径 |
| DataGrid.Filter.Menu.Closed | 9.439 | 8.905 | `(9.439 - 8.905) / 9.439` | 5.66% | smoke-only；不覆盖本轮分组 slot 主路径 |
| DataGrid.Filter.Tree.Closed | 8.241 | 8.124 | `(8.241 - 8.124) / 8.241` | 1.42% | smoke-only；不覆盖本轮分组 slot 主路径 |
| DataGrid.RowHeaders | 9.482 | 9.829 | `(9.482 - 9.829) / 9.482` | -3.66% | smoke-only；不覆盖本轮分组 slot 主路径 |
| DataGrid.RowDetails.Collapsed | 9.831 | 10.069 | `(9.831 - 10.069) / 9.831` | -2.42% | smoke-only；不覆盖本轮分组 slot 主路径，异常后复测已回落 |
| DataGrid.GroupHeaders | 9.075 | 9.329 | `(9.075 - 9.329) / 9.075` | -2.80% | smoke-only；column group 场景，不作为本轮回归结论 |
| DataGrid.RowGroups | 10.013 | 9.613 | `(10.013 - 9.613) / 10.013` | 3.99% | smoke-only；覆盖 grouped row slot 路径，作为趋势记录 |
| DataGrid.GalleryShape | 42.290 | 43.224 | `(42.290 - 43.224) / 42.290` | -2.21% | smoke-only；综合场景波动，不作为本轮页面回归结论 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.563 | 2947.8 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.905 | 2585.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.124 | 2583.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.829 | 3106.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.069 | 3023.0 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.329 | 2573.8 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.613 | 2936.9 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.224 | 12406.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.87 DataGrid selected-items empty reset cache cleanup

本轮优化点：`DataGrid.InitializeElements()` 为了 reset 后恢复 selection，会先复制 `_selectedItems.SelectedItemsCache`。大多数 DataGrid 初始化 / reset 时没有选中项，旧路径仍创建一个空 `List<object>` 再写回 selected-items cache。现在只有 cache 非空才复制；空 selection 路径调用 `UpdateIndexes()` 刷新 selected slot table 和旧 selection 状态，但保留原默认空 cache 引用。

同时 `DataGridSelectedItemsCollection.UpdateIndexes()` 在当前 selected cache 为空且没有 backing capacity 时不再创建并替换新的空 `List<object>`；如果 cache 曾经有选中项、Clear 后仍持有 backing array，则保留旧 shrink 行为，用新空 list 释放容量。有选中项时仍按已有 selected count 预分配 surviving cache。非空 selection reset 继续保留旧选中项。

正确性补强：新增状态验证覆盖空 selection 的 `InitializeElements(false)` 不替换 selected-items cache、`SelectedItems.Count == 0` / `SelectedIndex == -1` / `SelectedItem == null` 保持；SelectAll 后清空 selection 的 cache 会在 `UpdateIndexes()` 后释放 backing capacity；非空 Extended selection 下 SelectAll 后 reset，5 个 selected items 顺序保持。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `InitializeElements()` selected-items cache copy / default-empty selection reset | 1 empty `List<object>` copy | 0 list copies | `(1 - 0) / 1` | 100.00% | 有效；默认无 selection reset 不再复制空 cache |
| `UpdateIndexes()` temp selected-items cache / zero-capacity empty current selection | 1 empty `List<object>` replacement | 0 replacements | `(1 - 0) / 1` | 100.00% | 有效；默认空 cache 引用保持稳定 |
| `UpdateIndexes()` cleared selected cache backing capacity | capacity retained until replacement | capacity released to 0 | behavior verified | n/a | 内存防护；清空 selection 后不滞留 backing array |
| non-empty selection reset preservation | old behavior only | 5 selected items preserved + verified | n/a | correctness coverage added | 正确性防护；非空 selection reset 语义保持 |

Smoke-only 对比上一轮同参数复测。本轮路径主要覆盖无 selection 的 reset / InitializeElements；默认 DataGrid smoke 会间接触发部分初始化路径，但单次 timing 仍只作为异常检查，不把它单独当成页面级速度证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.563 | 13.315 | `(15.563 - 13.315) / 15.563` | 14.44% | smoke-only；初始化路径间接受益，只作趋势记录 |
| DataGrid.Filter.Menu.Closed | 8.905 | 8.525 | `(8.905 - 8.525) / 8.905` | 4.27% | smoke-only；只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.124 | 7.620 | `(8.124 - 7.620) / 8.124` | 6.20% | smoke-only；只作异常检查 |
| DataGrid.RowHeaders | 9.829 | 9.178 | `(9.829 - 9.178) / 9.829` | 6.62% | smoke-only；只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.069 | 9.053 | `(10.069 - 9.053) / 10.069` | 10.09% | smoke-only；只作异常检查 |
| DataGrid.GroupHeaders | 9.329 | 8.354 | `(9.329 - 8.354) / 9.329` | 10.45% | smoke-only；只作异常检查 |
| DataGrid.RowGroups | 9.613 | 9.322 | `(9.613 - 9.322) / 9.613` | 3.03% | smoke-only；只作异常检查 |
| DataGrid.GalleryShape | 43.224 | 40.621 | `(43.224 - 40.621) / 43.224` | 6.02% | smoke-only；综合场景趋势记录，不作为单轮确定页面收益 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.315 | 2948.4 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.525 | 2586.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.620 | 2584.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.178 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.053 | 3024.7 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.354 | 2573.4 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.322 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 40.621 | 12409.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.88 DataGrid empty filter request object copy cleanup

本轮优化点：列头收到 `DataGridColumnFilterEventArgs` 后，会把 `List<string>` 的 filter values 复制成 `List<object>`，再投递到延迟过滤处理。清空过滤时 values 为空，旧路径仍创建一个新的空 `List<object>`；现在 string/object 两个 copy helper 在空输入时复用私有零容量空列表。非空请求仍按原逻辑创建独立快照，避免请求后原列表 mutation 影响延迟处理。

正确性补强：状态验证覆盖空 `List<string>` 和空 `List<object>` copy 均返回同一个 `Count=0 / Capacity=0` 的 shared list；非空 string filter values 仍创建独立 copy，原始列表后续修改不会影响过滤条件。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Empty string filter request object-list copy / clear-filter request | 1 empty `List<object>` | 0 new lists, shared zero-capacity list | `(1 - 0) / 1` | 100.00% | 有效；清空过滤请求不再二次复制空列表 |
| Empty object filter request copy / internal clear-filter path | 1 empty `List<object>` | 0 new lists, shared zero-capacity list | `(1 - 0) / 1` | 100.00% | 有效；两个 copy overload 复用同一空列表 |
| Non-empty filter request snapshot | independent copy | independent copy preserved | behavior verified | n/a | 正确性保持；延迟过滤仍不受原列表 mutation 影响 |

Smoke-only 对比上一轮同参数复测。本轮路径只在用户清空过滤请求时触发，默认 closed DataGrid page-load smoke 不覆盖该交互路径；下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.315 | 14.210 | `(13.315 - 14.210) / 13.315` | -6.72% | smoke-only；不覆盖清空过滤请求路径 |
| DataGrid.Filter.Menu.Closed | 8.525 | 8.741 | `(8.525 - 8.741) / 8.525` | -2.53% | smoke-only；closed page 不触发 filter request |
| DataGrid.Filter.Tree.Closed | 7.620 | 7.575 | `(7.620 - 7.575) / 7.620` | 0.59% | smoke-only；closed page 不触发 filter request |
| DataGrid.RowHeaders | 9.178 | 9.118 | `(9.178 - 9.118) / 9.178` | 0.65% | smoke-only；不覆盖清空过滤请求路径 |
| DataGrid.RowDetails.Collapsed | 9.053 | 9.065 | `(9.053 - 9.065) / 9.053` | -0.13% | smoke-only；不覆盖清空过滤请求路径 |
| DataGrid.GroupHeaders | 8.354 | 8.358 | `(8.354 - 8.358) / 8.354` | -0.05% | smoke-only；不覆盖清空过滤请求路径 |
| DataGrid.RowGroups | 9.322 | 9.405 | `(9.322 - 9.405) / 9.322` | -0.89% | smoke-only；不覆盖清空过滤请求路径 |
| DataGrid.GalleryShape | 40.621 | 40.623 | `(40.621 - 40.623) / 40.621` | 0.00% | smoke-only；综合页面基本持平 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.210 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.741 | 2586.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.575 | 2584.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.118 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.065 | 3024.7 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.358 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.405 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 40.623 | 12409.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.89 DataGrid paged enumerator direct range cleanup

本轮优化点：`DataGridCollectionView.GetEnumerator()` 在 `PageSize > 0 && PageIndex >= 0` 的分页模式下，旧路径即使已经按当前页 item 数预分配，仍会为每次枚举创建一个 `List<object?>`，把当前页 item 逐个复制进去，再把这个临时 list 的 enumerator 交给 `NewItemAwareEnumerator`。现在新增直接页范围枚举器，按 `[pageStartIndex, pageEndIndex)` 读取 `InternalList`；`CurrentAddItem` 仍由 `NewItemAwareEnumerator` 包装处理，`PageIndex < 0` 的空分页状态继续复用 shared empty array。

正确性补强：状态验证覆盖 private `CreatePagedEnumerator()` 返回直接 `PageEnumerator`，第 2 页 / 末页顺序保持，`Reset()` 后重新枚举顺序保持，public `GetEnumerator()` 与 direct page enumerator 的结果一致；空分页 shared empty array 语义仍保持。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Non-empty paged `GetEnumerator()` page-list allocation / call | 1 `List<object?>` + backing array | 0 page lists / backing arrays | `(1 - 0) / 1` | 100.00% | 有效；分页枚举不再为当前页复制临时 list |
| Non-empty paged `GetEnumerator()` page item copy / call | `N` `List.Add` copies for current page | 0 copies | `(N - 0) / N` | 100.00% | 有效；直接按页范围读取 internal list |
| Paged enumerator order / reset semantics | existing list enumerator behavior | direct page enumerator verified | n/a | correctness coverage added | 正确性防护；第 2 页、末页、Reset 后顺序均覆盖 |

Smoke-only 对比上一轮同参数复测。本轮路径只在外部枚举分页 `DataGridCollectionView` 时触发，默认 DataGrid page-load smoke 不直接覆盖分页枚举；下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.210 | 13.633 | `(14.210 - 13.633) / 14.210` | 4.06% | smoke-only；不覆盖分页枚举主路径 |
| DataGrid.Filter.Menu.Closed | 8.741 | 8.600 | `(8.741 - 8.600) / 8.741` | 1.61% | smoke-only；不覆盖分页枚举主路径 |
| DataGrid.Filter.Tree.Closed | 7.575 | 7.637 | `(7.575 - 7.637) / 7.575` | -0.82% | smoke-only；不覆盖分页枚举主路径 |
| DataGrid.RowHeaders | 9.118 | 9.008 | `(9.118 - 9.008) / 9.118` | 1.21% | smoke-only；不覆盖分页枚举主路径 |
| DataGrid.RowDetails.Collapsed | 9.065 | 9.448 | `(9.065 - 9.448) / 9.065` | -4.23% | smoke-only；不覆盖分页枚举主路径 |
| DataGrid.GroupHeaders | 8.358 | 8.526 | `(8.358 - 8.526) / 8.358` | -2.01% | smoke-only；不覆盖分页枚举主路径 |
| DataGrid.RowGroups | 9.405 | 9.349 | `(9.405 - 9.349) / 9.405` | 0.60% | smoke-only；不覆盖分页枚举主路径 |
| DataGrid.GalleryShape | 40.623 | 41.579 | `(40.623 - 41.579) / 40.623` | -2.35% | smoke-only；综合页面波动，不作为本轮回归结论 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.633 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.600 | 2586.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.637 | 2583.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.008 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.448 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.526 | 2573.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.349 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 41.579 | 12408.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.90 DataGrid clipboard row content visible-column preallocation

本轮优化点：`ProcessCopyKey()` 在复制 header row 和每个 selected row 时都会创建 `DataGridRowClipboardEventArgs`，随后按 visible columns 逐个向 `ClipboardRowContent` 添加 `DataGridClipboardCellContent`。旧路径第一次访问 `ClipboardRowContent` 时创建零容量 `List<DataGridClipboardCellContent>`，8 个 visible cells 会触发 `0 -> 4 -> 8` 的 Add-time 扩容；现在 event args 记录当前 `ColumnsInternal.VisibleColumnCount`，首次访问 list 时直接按 visible column count 预分配。默认零容量路径仍保留，避免 public mutable list 在未指定容量时产生额外 backing array。

正确性补强：状态验证覆盖 internal ctor 的默认容量、header metadata、row item metadata、8 visible columns 容量预分配，以及连续添加 8 个 clipboard cell content 后 capacity 仍保持 8。`ClipboardRowContent` 仍是每个 event args 自己的 mutable list，没有改成共享空列表。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Clipboard row content list capacity / copied row with 8 visible cells | 0 capacity, then grows to 8 while adding cells | 8 capacity up front | `(2 - 0) / 2` Add-time growths | 100.00% | 有效；Ctrl+C 复制 header / selected row 时不再边 add 边扩容 |
| Clipboard row content Add-time backing-array growths / 8 visible cells | 2 growths (`0 -> 4 -> 8`) | 0 growths | `(2 - 0) / 2` | 100.00% | 有效；状态验证覆盖 8 次 add 后 capacity 仍为 8 |
| Default empty clipboard args capacity | 0 capacity | 0 capacity | behavior preserved | n/a | 正确性保持；未指定容量时仍不预分配 backing array |

Smoke-only 对比上一轮同参数复测。本轮路径只在用户执行 DataGrid clipboard copy 时触发，默认 DataGrid page-load smoke 不覆盖该交互路径；下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.633 | 15.402 | `(13.633 - 15.402) / 13.633` | -12.98% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.Filter.Menu.Closed | 8.600 | 9.105 | `(8.600 - 9.105) / 8.600` | -5.87% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.Filter.Tree.Closed | 7.637 | 8.024 | `(7.637 - 8.024) / 7.637` | -5.07% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.RowHeaders | 9.008 | 9.721 | `(9.008 - 9.721) / 9.008` | -7.92% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.RowDetails.Collapsed | 9.448 | 9.961 | `(9.448 - 9.961) / 9.448` | -5.43% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.GroupHeaders | 8.526 | 8.679 | `(8.526 - 8.679) / 8.526` | -1.79% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.RowGroups | 9.349 | 9.554 | `(9.349 - 9.554) / 9.349` | -2.19% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.GalleryShape | 41.579 | 43.461 | `(41.579 - 43.461) / 41.579` | -4.53% | smoke-only；综合页面波动，不作为本轮回归结论 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.402 | 2948.4 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.105 | 2586.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.024 | 2583.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.721 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.961 | 3024.3 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.679 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.554 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.461 | 12408.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.91 DataGrid clipboard content direct append formatting cleanup

本轮优化点：`ProcessCopyKey()` 原先每处理 header row 或 selected row，都会先调用 `FormatClipboardContent()` 创建一个临时 row `StringBuilder`，再 `ToString()` 生成整行字符串，最后追加到总 `textBuilder`。同时每个 cell 通过 `$"\"{cellContent}\""` 创建插值字符串，quote escaping 也固定走一次 `string.Replace()`。现在改为 `AppendClipboardContent(textBuilder, args)` 直接向总 builder 追加行内容，并用 `AppendEscapedClipboardCellContent()` 手写转义引号，去掉 per-row 中间 builder / row string、per-cell 插值字符串和 per-cell `Replace()` call。

正确性补强：状态验证覆盖 `"plain"`、包含引号的 `"A\"B"`、`null` content、tab 分隔、CRLF 行尾、以及空 row content 不追加行尾；输出保持为 `"plain"\t"A""B"\t""\r\n`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Clipboard row formatting temporaries / copied row | 1 row `StringBuilder` + 1 row string | 0 row temporaries | `(2 - 0) / 2` | 100.00% | 有效；header row 和每个 selected row 直接写入总 builder |
| Clipboard cell interpolated strings / copied cell | 1 interpolated string | 0 interpolated strings | `(1 - 0) / 1` | 100.00% | 有效；cell 引号和内容直接 append |
| Clipboard quote escaping replacement calls / copied cell | 1 `string.Replace()` call | 0 `Replace()` calls | `(1 - 0) / 1` | 100.00% | 有效；引号转义语义由状态验证覆盖 |
| Clipboard formatting output | quoted cells + tabs + CRLF | unchanged | behavior preserved | n/a | 正确性保持；null、quote、empty row 均覆盖 |

Smoke-only 对比上一轮同参数复测。本轮路径只在用户执行 DataGrid clipboard copy 时触发，默认 DataGrid page-load smoke 不覆盖该交互路径；下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.402 | 14.717 | `(15.402 - 14.717) / 15.402` | 4.45% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.Filter.Menu.Closed | 9.105 | 9.238 | `(9.105 - 9.238) / 9.105` | -1.46% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.Filter.Tree.Closed | 8.024 | 8.445 | `(8.024 - 8.445) / 8.024` | -5.25% | smoke-only；不覆盖 clipboard copy 路径；首轮 12.051 为离群值，已复测 |
| DataGrid.RowHeaders | 9.721 | 9.576 | `(9.721 - 9.576) / 9.721` | 1.49% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.RowDetails.Collapsed | 9.961 | 10.103 | `(9.961 - 10.103) / 9.961` | -1.43% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.GroupHeaders | 8.679 | 8.987 | `(8.679 - 8.987) / 8.679` | -3.55% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.RowGroups | 9.554 | 9.637 | `(9.554 - 9.637) / 9.554` | -0.87% | smoke-only；不覆盖 clipboard copy 路径 |
| DataGrid.GalleryShape | 43.461 | 42.515 | `(43.461 - 42.515) / 43.461` | 2.18% | smoke-only；综合页面波动，不作为本轮复制路径收益证明 |

当前工作区 smoke（第二次复测，首轮 `DataGrid.Filter.Tree.Closed=12.051` 为离群值，未用于收益判断）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.717 | 2948.4 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.238 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.445 | 2583.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.576 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.103 | 3024.8 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.987 | 2573.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.637 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.515 | 12409.2 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.92 DataGrid auto-generated column order-list preallocation

本轮优化点：`GenerateColumnsFromProperties()` 已经知道 `DataConnection.DataProperties.Length`，旧路径仍用零容量 `List<KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>>` 收集待生成列的排序对，首次 `Insert()` 会触发 `0 -> 4` 的 backing array 扩容。现在先缓存 `DataProperties` 数组，并用属性数量预分配 `columnOrderPairs`，自动生成列时不再边插入边扩容。

正确性边界：`DataProperties` 快照只覆盖本次生成列内部 pass，`OnAutoGeneratingColumn(e)` 仍在 `AddGeneratedColumn()` 阶段按原顺序触发；已有状态验证覆盖 `DisplayAttribute.ShortName`、`DisplayAttribute.Name` fallback、`Order` 排序、`AutoGenerateField=false` 隐藏列，以及生成列 `IsAutoGenerated` 标记。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Auto-generated column order-pair list Add-time backing-array growths / 4 data properties | 1 growth (`0 -> 4`) | 0 growths | `(1 - 0) / 1` | 100.00% | 有效；自动生成列排序临时列表按属性数量一次到位 |
| Auto-generated column order-pair list starting capacity / 4 data properties | 0 capacity | 4 capacity | `(4 - 0) / 4` | 100.00% preallocated | 有效；4 个属性场景首个 insert 不再触发扩容 |
| DataProperties property array reads / auto-generate pass | 2 reads (`Length` + `foreach`) | 1 read | `(2 - 1) / 2` | 50.00% | 有效；同一 pass 复用 property array |
| Auto-generated column order/header/hide semantics | `ShortName` / `Name` / `Order` / hidden field | unchanged | behavior preserved | n/a | 正确性保持；状态验证覆盖 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid 自动生成列初始化时触发，默认 DataGrid page-load smoke 不能单独隔离该路径；下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.717 | 15.046 | `(14.717 - 15.046) / 14.717` | -2.24% | smoke-only；不能隔离 auto-generate path |
| DataGrid.Filter.Menu.Closed | 9.238 | 9.886 | `(9.238 - 9.886) / 9.238` | -7.01% | smoke-only；不能隔离 auto-generate path |
| DataGrid.Filter.Tree.Closed | 8.445 | 7.888 | `(8.445 - 7.888) / 8.445` | 6.60% | smoke-only；不能隔离 auto-generate path |
| DataGrid.RowHeaders | 9.576 | 9.966 | `(9.576 - 9.966) / 9.576` | -4.07% | smoke-only；不能隔离 auto-generate path |
| DataGrid.RowDetails.Collapsed | 10.103 | 9.821 | `(10.103 - 9.821) / 10.103` | 2.79% | smoke-only；不能隔离 auto-generate path |
| DataGrid.GroupHeaders | 8.987 | 8.488 | `(8.987 - 8.488) / 8.987` | 5.55% | smoke-only；不能隔离 auto-generate path |
| DataGrid.RowGroups | 9.637 | 9.245 | `(9.637 - 9.245) / 9.637` | 4.07% | smoke-only；不能隔离 auto-generate path |
| DataGrid.GalleryShape | 42.515 | 42.122 | `(42.515 - 42.122) / 42.515` | 0.92% | smoke-only；综合页面波动，不作为本轮收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.046 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.886 | 2586.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.888 | 2583.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.966 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.821 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.488 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.245 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.122 | 12408.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.93 DataGrid star column width adjustment list lazy/preallocation

本轮优化点：`AdjustStarColumnWidths()` 在列 resize / star width 调整时，即使没有任何可调 star 列，旧路径也会创建 outer `starColumns` 空列表，并继续进入 desired / max-min 两次 inner pass，各创建一个空 `starColumnPairs` 列表。现在没有可调 star 列时直接返回，不创建 3 个空列表；有可调 star 列时，outer star list 和 inner pair list 都按剩余 displayed column 数预分配，避免从 0 容量开始插入时扩容。

正确性边界：没有改列宽分配公式、排序因子或 min/max 限制，只改变临时列表创建时机和容量。状态验证覆盖 4 个 star 列从 displayIndex 0 增宽后全部 `100 -> 120`，以及从 displayIndex 2 增宽后前两列保持 120、后两列 `120 -> 140`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Star width adjustment empty temp lists / no eligible star columns | 3 empty lists | 0 lists | `(3 - 0) / 3` | 100.00% | 有效；没有可调 star 列时直接返回 |
| Star width adjustment backing-array growths / 4 eligible star columns | 2 growths | 0 growths | `(2 - 0) / 2` | 100.00% | 有效；outer star list 和 desired pair list 按剩余 displayed columns 预分配 |
| Star width adjustment starting capacity / 4 eligible star columns | 0 capacity | 4 capacity | `(4 - 0) / 4` | 100.00% preallocated | 有效；首个 add/insert 不再触发扩容 |
| Star width distribution behavior | all-star + partial star increase | unchanged | behavior preserved | n/a | 正确性保持；状态验证覆盖 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid 列 resize / star width 调整时触发，默认 DataGrid page-load smoke 不覆盖该交互路径；下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.046 | 12.540 | `(15.046 - 12.540) / 15.046` | 16.66% | smoke-only；不覆盖 resize/star width path |
| DataGrid.Filter.Menu.Closed | 9.886 | 8.655 | `(9.886 - 8.655) / 9.886` | 12.45% | smoke-only；不覆盖 resize/star width path |
| DataGrid.Filter.Tree.Closed | 7.888 | 7.821 | `(7.888 - 7.821) / 7.888` | 0.85% | smoke-only；不覆盖 resize/star width path |
| DataGrid.RowHeaders | 9.966 | 9.500 | `(9.966 - 9.500) / 9.966` | 4.68% | smoke-only；不覆盖 resize/star width path |
| DataGrid.RowDetails.Collapsed | 9.821 | 9.677 | `(9.821 - 9.677) / 9.821` | 1.47% | smoke-only；不覆盖 resize/star width path |
| DataGrid.GroupHeaders | 8.488 | 8.683 | `(8.488 - 8.683) / 8.488` | -2.30% | smoke-only；不覆盖 resize/star width path |
| DataGrid.RowGroups | 9.245 | 9.386 | `(9.245 - 9.386) / 9.245` | -1.53% | smoke-only；不覆盖 resize/star width path |
| DataGrid.GalleryShape | 42.122 | 40.702 | `(42.122 - 40.702) / 42.122` | 3.37% | smoke-only；综合页面波动，不作为本轮收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 12.540 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.655 | 2587.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.821 | 2583.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.500 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.677 | 3024.7 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.683 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.386 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 40.702 | 12409.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.94 DataGrid filter presenter item fallback indexed lookup

本轮优化点：`DataGridMenuFilterFlyoutPresenter` / `DataGridTreeFilterFlyoutPresenter` 在 selected values 收集时会递归按 index 读取 child item。旧路径在 `ContainerFromIndex(index)` 尚未生成容器时，fallback 走 `foreach (Items)` 从头扫描到目标 index；`GetFilterValues()` 又有 count + collect 两次递归 pass，因此未 realized / partially realized presenter 会重复创建枚举器并重复扫描 sibling items。现在 fallback 改为 `ItemsView[index]` 直接读取，Avalonia `ItemsControl.ItemsView` 是只读 items view，`ItemsSourceView` 提供 indexer（`.referenceprojects/Avalonia/src/Avalonia.Controls/ItemsControl.cs:201`，`.referenceprojects/Avalonia/src/Avalonia.Controls/ItemsSourceView.cs:20,67`）。

正确性边界：没有改变 filter item 递归规则、leaf 判定、checked 判定或返回 list 容量；状态验证新增未 realized presenter 路径，menu/tree 都在容器未生成时解析 6 个 nested leaf，并确认单选值 `Count=1 / Capacity=1`，随后复测已 realized presenter 的空选、单选和全选容量语义。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Missing-container filter item lookup enumerator allocations / lookup | 1 `Items` enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；未生成容器时不再为 fallback 取 item 创建枚举器 |
| Missing-container sibling scan steps / lookup at 3-leaf group tail | 3 item steps | 1 bounds check + indexed read | `(3 - 1) / 3` | 66.67% | 有效；按下标直接读取目标 item |
| Selected-values fallback lookup passes / `GetFilterValues()` | 2 passes (count + collect) using scan fallback | 2 passes using indexed fallback | per-lookup cost reduced | n/a | 递归 pass 数不变，但每次 fallback 取 item 更便宜 |
| Unrealized filter presenter selected-values behavior | menu/tree 6 leaves, single select `Count=1 / Capacity=1` | unchanged | behavior preserved | n/a | 正确性保持；状态验证覆盖未 realized fallback |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid filter flyout presenter 收集 selected values，且容器尚未生成或部分生成时触发；默认 closed page-load smoke 不覆盖该交互路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 12.540 | 13.577 | `(12.540 - 13.577) / 12.540` | -8.27% | smoke-only；复测仍不覆盖 filter selected-values fallback path |
| DataGrid.Filter.Menu.Closed | 8.655 | 8.558 | `(8.655 - 8.558) / 8.655` | 1.12% | smoke-only；closed page 不收集 selected values |
| DataGrid.Filter.Tree.Closed | 7.821 | 7.708 | `(7.821 - 7.708) / 7.821` | 1.44% | smoke-only；closed page 不收集 selected values |
| DataGrid.RowHeaders | 9.500 | 9.050 | `(9.500 - 9.050) / 9.500` | 4.74% | smoke-only；不覆盖 filter selected-values fallback path |
| DataGrid.RowDetails.Collapsed | 9.677 | 9.542 | `(9.677 - 9.542) / 9.677` | 1.40% | smoke-only；不覆盖 filter selected-values fallback path |
| DataGrid.GroupHeaders | 8.683 | 8.611 | `(8.683 - 8.611) / 8.683` | 0.83% | smoke-only；不覆盖 filter selected-values fallback path |
| DataGrid.RowGroups | 9.386 | 9.262 | `(9.386 - 9.262) / 9.386` | 1.32% | smoke-only；不覆盖 filter selected-values fallback path |
| DataGrid.GalleryShape | 40.702 | 40.658 | `(40.702 - 40.658) / 40.702` | 0.11% | smoke-only；综合页面波动，不作为本轮收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.577 | 2948.4 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.558 | 2586.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.708 | 2584.1 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.050 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.542 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.611 | 2573.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.262 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 40.658 | 12408.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.95 DataGrid filter presenter reset indexed fallback

本轮优化点：2.94 已把 selected-values 收集路径的 missing-container fallback 从 `foreach (Items)` 线性扫描改为 `ItemsView[index]`。本轮继续把同类 reset / radio clear traversal 收敛到同一个 indexed fallback：`DataGridMenuFilterFlyoutPresenter.ResetFilter()`、`DataGridTreeFilterFlyoutPresenter.ResetFilter()` 和 menu radio 互斥清理在子容器尚未生成时，也能直接读取 `ItemsView[index]`，不再创建 `Items` 枚举器或跳过未 realized 的 nested filter item。Avalonia source 依据同 2.94：`ItemsControl.ItemsView` 暴露只读 items view，`ItemsSourceView` 提供 indexer（`.referenceprojects/Avalonia/src/Avalonia.Controls/ItemsControl.cs:201`，`.referenceprojects/Avalonia/src/Avalonia.Controls/ItemsSourceView.cs:20,67`）。

正确性边界：没有改变 reset 按钮、OK 按钮、leaf 判定、checked 判定或 selected values 返回规则；只是让 reset / radio clear 和 selected-values 收集使用同一套 item lookup。状态验证新增未 realized presenter reset：先选中 nested leaf，调用私有 `ResetFilter()` 后 menu/tree 的 selected values 都恢复为空。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Reset missing-container item lookup enumerator allocations / lookup | 1 `Items` enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；reset traversal 不再为 fallback 取 item 创建枚举器 |
| Reset missing-container sibling scan steps / lookup at 3-leaf group tail | 3 item steps | 1 bounds check + indexed read | `(3 - 1) / 3` | 66.67% | 有效；按下标直接读取目标 item |
| Unrealized nested checked values left after ResetFilter | 1 stale selected value | 0 stale selected values | `(1 - 0) / 1` | 100.00% | 正确性加固；未生成容器的 nested checked leaf 也会被 reset 清空 |
| Menu radio clear traversal fallback | `ContainerFromIndex()` only | shared `ItemsView[index]` fallback | lookup path unified | n/a | 正确性加固；互斥清理不再只依赖已生成容器 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 filter presenter reset / menu radio clear 时触发，默认 closed page-load smoke 不覆盖该交互路径；下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明。本轮复测期间 page-load smoke 整体偏慢，但 Visual/root 和 Logical/root 结构稳定：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.577 | 15.281 | `(13.577 - 15.281) / 13.577` | -12.55% | smoke-only；不覆盖 reset/radio clear path |
| DataGrid.Filter.Menu.Closed | 8.558 | 9.082 | `(8.558 - 9.082) / 8.558` | -6.12% | smoke-only；closed page 不触发 reset/radio clear |
| DataGrid.Filter.Tree.Closed | 7.708 | 9.120 | `(7.708 - 9.120) / 7.708` | -18.32% | smoke-only；closed page 不触发 reset path |
| DataGrid.RowHeaders | 9.050 | 9.554 | `(9.050 - 9.554) / 9.050` | -5.57% | smoke-only；不覆盖 reset/radio clear path |
| DataGrid.RowDetails.Collapsed | 9.542 | 9.917 | `(9.542 - 9.917) / 9.542` | -3.93% | smoke-only；不覆盖 reset/radio clear path |
| DataGrid.GroupHeaders | 8.611 | 8.813 | `(8.611 - 8.813) / 8.611` | -2.35% | smoke-only；不覆盖 reset/radio clear path |
| DataGrid.RowGroups | 9.262 | 9.786 | `(9.262 - 9.786) / 9.262` | -5.66% | smoke-only；不覆盖 reset/radio clear path |
| DataGrid.GalleryShape | 40.658 | 42.746 | `(40.658 - 42.746) / 40.658` | -5.13% | smoke-only；综合页面波动，不作为本轮回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.281 | 2948.4 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.082 | 2586.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.120 | 2583.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.554 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.917 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.813 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.786 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.746 | 12408.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.96 DataGrid SelectedItems public enumerator yield cleanup

本轮优化点：2.71 已把 `SelectedItems` 公共枚举内部的 selected-slot 遍历改为 `IndexToValueTable` struct enumerator，但 `DataGridSelectedItemsCollection.GetEnumerator()` 本身仍是 `yield return` 方法，每次 public enumeration 都会生成一个 compiler-generated iterator state machine。现在改为显式 `SelectedItemsEnumerator`，直接持有 selected-slot struct enumerator 并按 slot 解析 item。

正确性边界：`IList.GetEnumerator()` 仍然必须返回 1 个 `IEnumerator` 对象，所以本轮不宣称“枚举器对象数量减少”；只移除 compiler-generated yield state machine。状态验证覆盖 SelectAll 后枚举仍返回 4 项、ClearSelection 后枚举仍为空，并新增结构断言：public enumerator 类型不再是 `<GetEnumerator>` yield 状态机，且 `Reset()` 继续抛 `NotSupportedException`，保持旧 yield iterator 行为。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `SelectedItems` public enumeration compiler-generated yield state machines / enumeration | 1 state machine | 0 state machines | `(1 - 0) / 1` | 100.00% | 有效但轻量；外层 yield 状态机已移除 |
| `SelectedItems` public enumeration required `IEnumerator` objects / enumeration | 1 object | 1 object | `(1 - 1) / 1` | 0.00% | 边界明确；`IList.GetEnumerator()` 仍需要一个显式 enumerator 对象 |
| SelectAll 后 `SelectedItems` 枚举数量 | 4 items | 4 items | behavior verified | n/a | 正确性保持 |
| ClearSelection 后 `SelectedItems` 枚举数量 | 0 items | 0 items | behavior verified | n/a | 正确性保持 |
| Public enumerator `Reset()` 行为 | throws `NotSupportedException` | throws `NotSupportedException` | behavior preserved | n/a | 兼容性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在外部代码枚举 `DataGrid.SelectedItems` 时触发，标准 DataGrid page-load smoke 基本不覆盖这个交互路径；下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.281 | 15.152 | `(15.281 - 15.152) / 15.281` | 0.84% | smoke-only；不覆盖 public selected-items enumeration |
| DataGrid.Filter.Menu.Closed | 9.082 | 9.129 | `(9.082 - 9.129) / 9.082` | -0.52% | smoke-only；不覆盖 public selected-items enumeration |
| DataGrid.Filter.Tree.Closed | 9.120 | 7.763 | `(9.120 - 7.763) / 9.120` | 14.88% | smoke-only；不覆盖 public selected-items enumeration，不作为本轮收益证明 |
| DataGrid.RowHeaders | 9.554 | 9.293 | `(9.554 - 9.293) / 9.554` | 2.73% | smoke-only；不覆盖 public selected-items enumeration |
| DataGrid.RowDetails.Collapsed | 9.917 | 9.798 | `(9.917 - 9.798) / 9.917` | 1.20% | smoke-only；不覆盖 public selected-items enumeration |
| DataGrid.GroupHeaders | 8.813 | 8.652 | `(8.813 - 8.652) / 8.813` | 1.83% | smoke-only；不覆盖 public selected-items enumeration |
| DataGrid.RowGroups | 9.786 | 9.646 | `(9.786 - 9.646) / 9.786` | 1.43% | smoke-only；不覆盖 public selected-items enumeration |
| DataGrid.GalleryShape | 42.746 | 43.795 | `(42.746 - 43.795) / 42.746` | -2.45% | smoke-only；综合页面波动，不作为本轮回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.152 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.129 | 2586.0 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.763 | 2583.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.293 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.798 | 3024.7 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.652 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.646 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.795 | 12409.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.97 DataGrid filter description default conditions lazy allocation

本轮优化点：`DataGridFilterDescription` 原先在构造时立即创建 `FilterConditions = new List<object>()`。实际筛选请求创建 `DataGridFilterDescription` 时通常马上通过 object initializer 赋值 `FilterConditions = filterValues`，原默认空 list 会被覆盖并丢弃；无条件 filter 调用 `FilterBy()` 时也只需要返回 false，不需要物化空 list。现在 `FilterConditions` 改为懒创建，`FilterBy()` 直接读取 backing field，只有公开 getter 被访问或外部确实要添加条件时才创建零容量 list。

正确性边界：公开 `FilterConditions` getter 仍返回可变 `List<object>`，`new DataGridFilterDescription().FilterConditions.Add(...)` 语义保持；object initializer 赋值的条件列表仍被直接使用；空条件 `FilterBy()` 仍返回 false。状态验证覆盖默认构造不分配 backing list、空条件 `FilterBy()` 不物化 list、公开 getter 返回 `Count=0 / Capacity=0`、object initializer 直接保存传入 conditions 且过滤语义保持。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Default `FilterConditions` list allocations / constructed filter description | 1 empty `List<object>` | 0 lists until needed | `(1 - 0) / 1` | 100.00% | 有效；构造 filter description 不再默认分配空条件列表 |
| Empty-condition `FilterBy()` list materialization | 1 default list already allocated | 0 lists | `(1 - 0) / 1` | 100.00% | 有效；无条件过滤直接返回 false |
| Object initializer overwrite wasted empty list | 1 overwritten empty list | 0 overwritten lists | `(1 - 0) / 1` | 100.00% | 有效；筛选请求创建 description 时不再先创建被覆盖的空 list |
| Public `FilterConditions` getter behavior | mutable zero-capacity list | mutable zero-capacity list | behavior preserved | n/a | 正确性保持 |
| Object-initialized filtering behavior | assigned conditions used | assigned conditions used | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在创建 filter description、空条件 filter 或用户确认筛选时触发；标准 closed page-load smoke 不能隔离该路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.152 | 12.506 | `(15.152 - 12.506) / 15.152` | 17.46% | smoke-only；不覆盖 filter description creation path |
| DataGrid.Filter.Menu.Closed | 9.129 | 9.957 | `(9.129 - 9.957) / 9.129` | -9.07% | smoke-only；closed page 不创建筛选 description |
| DataGrid.Filter.Tree.Closed | 7.763 | 8.088 | `(7.763 - 8.088) / 7.763` | -4.19% | smoke-only；closed page 不创建筛选 description |
| DataGrid.RowHeaders | 9.293 | 9.590 | `(9.293 - 9.590) / 9.293` | -3.20% | smoke-only；不覆盖 filter description creation path |
| DataGrid.RowDetails.Collapsed | 9.798 | 10.262 | `(9.798 - 10.262) / 9.798` | -4.74% | smoke-only；不覆盖 filter description creation path |
| DataGrid.GroupHeaders | 8.652 | 8.887 | `(8.652 - 8.887) / 8.652` | -2.72% | smoke-only；不覆盖 filter description creation path |
| DataGrid.RowGroups | 9.646 | 9.549 | `(9.646 - 9.549) / 9.646` | 1.01% | smoke-only；不覆盖 filter description creation path |
| DataGrid.GalleryShape | 43.795 | 41.932 | `(43.795 - 41.932) / 43.795` | 4.25% | smoke-only；综合页面波动，不作为本轮收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 12.506 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.957 | 2587.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.088 | 2583.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.590 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.262 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.887 | 2573.9 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.549 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 41.932 | 12408.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.98 DataGrid filter flyout passive close skips unused selected-values collection

本轮优化点：`DataGridMenuFilterFlyout` / `DataGridTreeFilterFlyout` 在默认 `FilterOnClose=false` 且不是 OK 确认关闭时，原先仍会从 presenter 递归收集 selected values、创建空 `List<string>` 和内部 `DataGridFilterValuesSelectedEventArgs`，随后 `DataGridFilterIndicator` 因为不是确认关闭而直接丢弃。现在 flyout 关闭时先通过 indicator 提供的实时判断确认是否需要处理被动关闭；默认被动关闭直接返回，不再进入 selected-values 收集路径。

正确性边界：确认关闭仍由 presenter 设置 `IsActiveShutdown=true` 并收集 selected values；`FilterOnClose=true` 的被动关闭仍会收集 selected values 并发起过滤；默认 `FilterOnClose=false` 的被动关闭不再发内部 selected-values 事件，因为现有唯一监听方原本也是 no-op。状态验证覆盖 menu/tree 两种 flyout 的默认被动关闭不通知、`FilterOnClose=true` 被动关闭仍通知空选择、确认关闭仍通知且 `IsConfirmed=true`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Default passive close selected-values traversal / close | 1 recursive presenter traversal | 0 traversals | `(1 - 0) / 1` | 100.00% | 有效；默认点外部关闭 filter flyout 不再扫描 menu/tree items |
| Default passive close empty `List<string>` allocations / close | 1 selected-values list | 0 lists | `(1 - 0) / 1` | 100.00% | 有效；无效关闭路径不再为随后被丢弃的 selected values 建 list |
| Default passive close internal event args allocations / close | 1 `DataGridFilterValuesSelectedEventArgs` | 0 event args | `(1 - 0) / 1` | 100.00% | 有效；默认被动关闭不再创建内部通知对象 |
| `FilterOnClose=true` passive close behavior | selected values reported | selected values reported | behavior verified | n/a | 正确性保持 |
| OK confirmed close behavior | selected values reported with `IsConfirmed=true` | selected values reported with `IsConfirmed=true` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在用户打开过滤弹层后被动关闭时触发；标准 closed page-load smoke 不覆盖该交互路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 12.506 | 14.126 | `(12.506 - 14.126) / 12.506` | -12.95% | smoke-only；不覆盖 passive filter close path |
| DataGrid.Filter.Menu.Closed | 9.957 | 9.065 | `(9.957 - 9.065) / 9.957` | 8.96% | smoke-only；closed page 不打开/关闭 filter flyout |
| DataGrid.Filter.Tree.Closed | 8.088 | 8.048 | `(8.088 - 8.048) / 8.088` | 0.49% | smoke-only；closed page 不打开/关闭 filter flyout |
| DataGrid.RowHeaders | 9.590 | 9.655 | `(9.590 - 9.655) / 9.590` | -0.68% | smoke-only；不覆盖 passive filter close path |
| DataGrid.RowDetails.Collapsed | 10.262 | 10.081 | `(10.262 - 10.081) / 10.262` | 1.76% | smoke-only；不覆盖 passive filter close path |
| DataGrid.GroupHeaders | 8.887 | 8.802 | `(8.887 - 8.802) / 8.887` | 0.96% | smoke-only；不覆盖 passive filter close path |
| DataGrid.RowGroups | 9.549 | 9.734 | `(9.549 - 9.734) / 9.549` | -1.94% | smoke-only；不覆盖 passive filter close path |
| DataGrid.GalleryShape | 41.932 | 42.716 | `(41.932 - 42.716) / 41.932` | -1.87% | smoke-only；综合页面波动，不作为本轮回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.126 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.065 | 2586.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.048 | 2584.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.655 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.081 | 3024.7 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.802 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.734 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.716 | 12409.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.99 DataGrid data-properties reflection cache

本轮优化点：`DataGridDataConnection.DataProperties` 每次读取都会执行 `dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance)`，返回新的 `PropertyInfo[]`。自动生成列、重置数据源和内部状态刷新会重复读取 `DataProperties`；当前属性集合只依赖当前 data type，因此改为按 data type 缓存 `PropertyInfo[]`。`DataSource` setter 和 `ClearDataProperties()` 会清空缓存，下一次读取重新反射，避免 stale properties。

正确性边界：同一个 DataSource / DataType 下重复读取返回同一组属性；`ClearDataProperties()` 后会生成新的属性数组；DataSource 替换后也会刷新缓存。状态验证覆盖重复读取复用、清理后刷新、替换同类型 DataSource 后刷新，且 `PlainName` 等自动生成列属性仍保留。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `DataProperties` `GetProperties(...)` calls / warm repeated read | 1 reflection call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；同 DataType 重复读取不再反射 |
| `PropertyInfo[]` array instances / 2 reads on unchanged source | 2 arrays | 1 cached array | `(2 - 1) / 2` | 50.00% | 有效；第二次读取复用首次数组 |
| `ClearDataProperties()` refresh behavior | fresh properties after clear | fresh properties after clear | behavior verified | n/a | 正确性保持 |
| DataSource replacement refresh behavior | fresh properties after replacement | fresh properties after replacement | behavior verified | n/a | 正确性保持 |
| Auto-generate property names | properties preserved | properties preserved | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只覆盖自动生成列 / DataProperties 重复读取，标准 DataGrid page-load smoke 不隔离该反射路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.126 | 15.519 | `(14.126 - 15.519) / 14.126` | -9.86% | smoke-only；不隔离 DataProperties repeated read path |
| DataGrid.Filter.Menu.Closed | 9.065 | 9.068 | `(9.065 - 9.068) / 9.065` | -0.03% | smoke-only；不覆盖自动生成列反射路径 |
| DataGrid.Filter.Tree.Closed | 8.048 | 8.229 | `(8.048 - 8.229) / 8.048` | -2.25% | smoke-only；不覆盖自动生成列反射路径 |
| DataGrid.RowHeaders | 9.655 | 9.388 | `(9.655 - 9.388) / 9.655` | 2.77% | smoke-only；不隔离 DataProperties repeated read path |
| DataGrid.RowDetails.Collapsed | 10.081 | 9.717 | `(10.081 - 9.717) / 10.081` | 3.61% | smoke-only；不隔离 DataProperties repeated read path |
| DataGrid.GroupHeaders | 8.802 | 8.777 | `(8.802 - 8.777) / 8.802` | 0.28% | smoke-only；不隔离 DataProperties repeated read path |
| DataGrid.RowGroups | 9.734 | 9.540 | `(9.734 - 9.540) / 9.734` | 1.99% | smoke-only；不隔离 DataProperties repeated read path |
| DataGrid.GalleryShape | 42.716 | 42.751 | `(42.716 - 42.751) / 42.716` | -0.08% | smoke-only；综合页面波动，不作为本轮回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.519 | 2948.4 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.068 | 2586.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.229 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.388 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.717 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.777 | 2574.0 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.540 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.751 | 12409.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.100 DataConnection bare IEnumerable item-type probe disposal

本轮优化点：`DataGridDataConnection.DataType` 会通过 `ItemsSource.GetItemType()` 推断行数据类型。对裸 `IEnumerable`（没有泛型 item type、也不是 `ICollection<T>` 这类可直接解析类型）时，`TypeHelper.GetItemType()` 需要创建 enumerator 并读取第一个代表项。旧路径读取后没有释放 enumerator；现在用 `try/finally` 在探测结束后释放实现了 `IDisposable` 的枚举器。

正确性边界：代表项类型推断仍只读取第一个 item；空集合 / null item 仍 fallback 到原 item type；优化只补齐资源释放，不改变枚举顺序和 DataGrid 自动列类型推断语义。状态验证覆盖裸 `IEnumerable` 的 `DataType` 读取仍推断出 `int`、`MoveNext()` 仍为 1 次、并且探测枚举器会被 dispose 1 次。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Open enumerators after bare `IEnumerable` item-type probe / `DataType` read | 1 undisposed enumerator | 0 undisposed enumerators | `(1 - 0) / 1` | 100.00% | 有效；裸 enumerable 类型探测后不再留下未释放枚举器 |
| Enumerator `Dispose()` calls / probe | 0 calls | 1 call | lifecycle fixed | n/a | 正确性/生命周期修复；收益不是减少创建，而是释放资源 |
| Representative item `MoveNext()` calls / probe | 1 call | 1 call | `(1 - 1) / 1` | 0.00% | 行为保持；仍只读取第一个代表项 |
| Representative type inference | `int` | `int` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid 使用裸非泛型 `IEnumerable` 且读取 `DataType` 时触发；标准 DataGrid page-load smoke 不隔离该路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.281 | 14.532 | `(15.281 - 14.532) / 15.281` | 4.90% | smoke-only；不隔离 bare IEnumerable item-type probe |
| DataGrid.Filter.Menu.Closed | 9.082 | 9.364 | `(9.082 - 9.364) / 9.082` | -3.10% | smoke-only；不隔离 bare IEnumerable item-type probe |
| DataGrid.Filter.Tree.Closed | 9.120 | 8.468 | `(9.120 - 8.468) / 9.120` | 7.15% | smoke-only；不隔离 bare IEnumerable item-type probe |
| DataGrid.RowHeaders | 9.554 | 10.074 | `(9.554 - 10.074) / 9.554` | -5.44% | smoke-only；不隔离 bare IEnumerable item-type probe |
| DataGrid.RowDetails.Collapsed | 9.917 | 10.358 | `(9.917 - 10.358) / 9.917` | -4.45% | smoke-only；不隔离 bare IEnumerable item-type probe |
| DataGrid.GroupHeaders | 8.813 | 9.218 | `(8.813 - 9.218) / 8.813` | -4.60% | smoke-only；不隔离 bare IEnumerable item-type probe |
| DataGrid.RowGroups | 9.786 | 9.903 | `(9.786 - 9.903) / 9.786` | -1.20% | smoke-only；不隔离 bare IEnumerable item-type probe |
| DataGrid.GalleryShape | 42.746 | 44.233 | `(42.746 - 44.233) / 42.746` | -3.48% | smoke-only；综合页面波动，不作为本轮回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.532 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.364 | 2586.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.468 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.074 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.358 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.218 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.903 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.233 | 12409.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.101 DataConnection IReadOnlyCollection count fast path

本轮优化点：`DataGridDataConnection.TryGetCount()` 原先只识别非泛型 `ICollection`。当 `ItemsSource` / `DataSource` 只实现 `IReadOnlyCollection<object>` 而不是 `ICollection` 时，Count 路径会退回 raw enumerable：完整 count 要创建 enumerator 并 `MoveNext()` 到末尾，getAny 也会创建 enumerator 并探测第一项。现在 `ICollection` 快路径之后增加 `IReadOnlyCollection<object>` 快路径，直接读取 `Count`，不再枚举。

正确性边界：`ICollection` 原快路径保持优先；`IReadOnlyCollection<object>` 的 Count/Any 都返回 collection 的真实 `Count`，与 `ICollection` 分支一致；普通 bare `IEnumerable` 仍走上一轮已验证的 raw enumerator 路径并释放枚举器。状态验证覆盖 4 项 read-only collection 的 count / getAny 都只读一次 `Count`，且 `GetEnumerator()` 次数为 0。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `IReadOnlyCollection<object>` count enumerators / `TryGetCount(allowSlow: true)` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；只读 Count，不再完整枚举 |
| `IReadOnlyCollection<object>` count `MoveNext()` calls / 4-item source | 5 calls | 0 calls | `(5 - 0) / 5` | 100.00% | 有效；避免 count + 1 次探测 |
| `IReadOnlyCollection<object>` getAny enumerators / `TryGetCount(getAny: true)` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；Any 也走 Count |
| `IReadOnlyCollection<object>` getAny `MoveNext()` calls / non-empty source | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；不触碰 enumeration path |
| Returned count for 4-item read-only collection | 4 | 4 | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataSource 实现 `IReadOnlyCollection<object>` 但不实现 `ICollection` 时触发；标准 DataGrid page-load smoke 不隔离该路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.532 | 14.252 | `(14.532 - 14.252) / 14.532` | 1.93% | smoke-only；不隔离 read-only collection count path |
| DataGrid.Filter.Menu.Closed | 9.364 | 9.210 | `(9.364 - 9.210) / 9.364` | 1.64% | smoke-only；不隔离 read-only collection count path |
| DataGrid.Filter.Tree.Closed | 8.468 | 8.312 | `(8.468 - 8.312) / 8.468` | 1.84% | smoke-only；不隔离 read-only collection count path |
| DataGrid.RowHeaders | 10.074 | 10.028 | `(10.074 - 10.028) / 10.074` | 0.46% | smoke-only；不隔离 read-only collection count path |
| DataGrid.RowDetails.Collapsed | 10.358 | 10.368 | `(10.358 - 10.368) / 10.358` | -0.10% | smoke-only；不隔离 read-only collection count path |
| DataGrid.GroupHeaders | 9.218 | 9.003 | `(9.218 - 9.003) / 9.218` | 2.33% | smoke-only；不隔离 read-only collection count path |
| DataGrid.RowGroups | 9.903 | 9.602 | `(9.903 - 9.602) / 9.903` | 3.04% | smoke-only；不隔离 read-only collection count path |
| DataGrid.GalleryShape | 44.233 | 43.582 | `(44.233 - 43.582) / 44.233` | 1.47% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.252 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.210 | 2586.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.312 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.028 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.368 | 3024.7 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.003 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.602 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.582 | 12409.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.102 DataConnection IReadOnlyList indexed item lookup

本轮优化点：`DataGridDataConnection.GetDataItem(index)` 是行生成、选择、剪贴板和当前行访问会反复调用的取项路径。原实现只对 `DataGridCollectionView` 和非泛型 `IList` 做 indexer 快路径；当数据源只实现 `IReadOnlyList<object>` 而不是 `IList` 时，会退回 `IEnumerable.GetEnumerator()`，从第 0 项一路 `MoveNext()` 到目标 index。现在新增 `IReadOnlyList<object>` 快路径，直接读取 `Count` 并用 indexer 取项，越界仍返回 null。

正确性边界：`DataGridCollectionView` / `IList` 原快路径保持优先；`IReadOnlyList<object>` 命中时 index 范围内返回同一项，越界返回 null；普通 bare `IEnumerable` 仍走上一轮已验证的 raw enumerator 路径并释放枚举器。状态验证覆盖 4 项 read-only list 的 `GetDataItem(2)` 返回 2、只读 1 次 `Count` 和 1 次 indexer、`GetEnumerator()` / `MoveNext()` / `Dispose()` 都为 0；`GetDataItem(4)` 越界返回 null 且不调用 indexer。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `IReadOnlyList<object>` item lookup enumerators / `GetDataItem(2)` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；取项不再创建枚举器 |
| `IReadOnlyList<object>` item lookup `MoveNext()` calls / `GetDataItem(2)` | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | 有效；避免从头扫描到目标行 |
| `IReadOnlyList<object>` item lookup indexer calls / `GetDataItem(2)` | 0 calls | 1 call | direct indexed access | n/a | 行为更直接；状态验证覆盖返回值 |
| Out-of-range `GetDataItem(4)` indexer calls / 4-item source | 0 calls | 0 calls | behavior verified | n/a | 正确性保持；越界仍返回 null |
| Returned item for `GetDataItem(2)` | `2` | `2` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataSource 实现 `IReadOnlyList<object>` 但不实现 `IList` 时触发；标准 DataGrid page-load smoke 不隔离该路径。复测两次，timing 波动明显，Visual / Logical / KB 基本稳定。下表使用第二次复测结果，只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.252 | 15.072 | `(14.252 - 15.072) / 14.252` | -5.75% | smoke-only；不隔离 read-only list item lookup path |
| DataGrid.Filter.Menu.Closed | 9.210 | 9.929 | `(9.210 - 9.929) / 9.210` | -7.81% | smoke-only；不隔离 read-only list item lookup path |
| DataGrid.Filter.Tree.Closed | 8.312 | 9.032 | `(8.312 - 9.032) / 8.312` | -8.66% | smoke-only；不隔离 read-only list item lookup path |
| DataGrid.RowHeaders | 10.028 | 9.893 | `(10.028 - 9.893) / 10.028` | 1.35% | smoke-only；不隔离 read-only list item lookup path |
| DataGrid.RowDetails.Collapsed | 10.368 | 10.177 | `(10.368 - 10.177) / 10.368` | 1.84% | smoke-only；不隔离 read-only list item lookup path |
| DataGrid.GroupHeaders | 9.003 | 10.073 | `(9.003 - 10.073) / 9.003` | -11.89% | smoke-only；不隔离 read-only list item lookup path |
| DataGrid.RowGroups | 9.602 | 10.537 | `(9.602 - 10.537) / 9.602` | -9.74% | smoke-only；不隔离 read-only list item lookup path |
| DataGrid.GalleryShape | 43.582 | 44.482 | `(43.582 - 44.482) / 43.582` | -2.07% | smoke-only；综合页面波动，不作为本轮回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.072 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.929 | 2586.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.032 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.893 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.177 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 10.073 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.537 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.482 | 12409.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.103 DataConnection IReadOnlyList index lookup

本轮优化点：`DataGridDataConnection.IndexOf(item)` 是 current item、selection 和滚动定位等路径会使用的 data-layer 查找。原实现只对 `DataGridCollectionView` 和非泛型 `IList` 走专用查找；当数据源只实现 `IReadOnlyList<object>` 而不是 `IList` 时，会退回 `foreach`，为每次查找创建 enumerator，并对命中项前的每一项调用 `MoveNext()`。现在新增 `IReadOnlyList<object>` 快路径，先读取一次 `Count`，再通过 indexer 顺序扫描，避免 enumerable enumerator。

正确性边界：`DataGridCollectionView` / `IList` 原快路径保持优先；`IReadOnlyList<object>` 命中时返回同一 index，未命中仍返回 -1；`IndexOf(null)` 保持原 enumerable fallback 语义，仍返回 -1 且不触碰 read-only list。状态验证覆盖 4 项 read-only list 的 `IndexOf(2)` 返回 2、只读 1 次 `Count`、读 3 次 indexer、`GetEnumerator()` / `MoveNext()` / `Dispose()` 都为 0；`IndexOf(9)` 未命中返回 -1，读 4 次 indexer 且不枚举；`IndexOf(null)` 返回 -1 且不读取 Count/indexer。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `IReadOnlyList<object>` index lookup enumerators / `IndexOf(2)` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；查找不再创建枚举器 |
| `IReadOnlyList<object>` index lookup `MoveNext()` calls / `IndexOf(2)` | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | 有效；命中项前扫描不再走 enumerable |
| `IReadOnlyList<object>` index lookup enumerator disposals / `IndexOf(2)` | 1 dispose | 0 disposes | `(1 - 0) / 1` | 100.00% | 有效；没有枚举器需要释放 |
| Missing `IReadOnlyList<object>` index lookup `MoveNext()` calls / `IndexOf(9)` on 4-item source | 5 calls | 0 calls | `(5 - 0) / 5` | 100.00% | 有效；未命中也不走 enumerable |
| Missing `IReadOnlyList<object>` indexer calls / `IndexOf(9)` on 4-item source | 0 calls | 4 calls | direct indexed scan | n/a | 行为更直接；状态验证覆盖返回 -1 |
| Null item lookup result / `IndexOf(null)` | `-1` | `-1` | behavior verified | n/a | 正确性保持；不触碰 read-only list |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataSource 实现 `IReadOnlyList<object>` 但不实现 `IList` 且调用 `IndexOf(item)` 时触发；标准 DataGrid page-load smoke 不隔离该路径。Visual / Logical / KB 基本稳定，timing 有本机波动，只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.072 | 15.863 | `(15.072 - 15.863) / 15.072` | -5.25% | smoke-only；不隔离 read-only list index lookup path |
| DataGrid.Filter.Menu.Closed | 9.929 | 10.014 | `(9.929 - 10.014) / 9.929` | -0.86% | smoke-only；不隔离 read-only list index lookup path |
| DataGrid.Filter.Tree.Closed | 9.032 | 10.758 | `(9.032 - 10.758) / 9.032` | -19.11% | smoke-only；不隔离 read-only list index lookup path |
| DataGrid.RowHeaders | 9.893 | 10.019 | `(9.893 - 10.019) / 9.893` | -1.27% | smoke-only；不隔离 read-only list index lookup path |
| DataGrid.RowDetails.Collapsed | 10.177 | 11.141 | `(10.177 - 11.141) / 10.177` | -9.47% | smoke-only；不隔离 read-only list index lookup path |
| DataGrid.GroupHeaders | 10.073 | 10.629 | `(10.073 - 10.629) / 10.073` | -5.52% | smoke-only；不隔离 read-only list index lookup path |
| DataGrid.RowGroups | 10.537 | 10.294 | `(10.537 - 10.294) / 10.537` | 2.31% | smoke-only；不隔离 read-only list index lookup path |
| DataGrid.GalleryShape | 44.482 | 45.106 | `(44.482 - 45.106) / 44.482` | -1.40% | smoke-only；综合页面波动，不作为本轮回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.863 | 2948.4 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.014 | 2586.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 10.758 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.019 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 11.141 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 10.629 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.294 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 45.106 | 12409.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.104 CollectionView IReadOnlyCollection source count fast path

本轮优化点：`DataGridCollectionView.CopySourceToInternalList()` / `CreateListForSource()` 之前只对非泛型 `ICollection` 按 `Count` 预分配。数据源如果是只读集合接口 `IReadOnlyCollection<object>`，会从空 `List<object>` 逐项 `Add`，9 项数据会扩容到 16 slots。`ProcessCollectionChanged(Reset)` 的空集合检查同样只识别 `ICollection`，read-only source 会先创建一个 empty-check enumerator，再执行 refresh 枚举。现在这两处都复用 `IReadOnlyCollection<object>.Count`。

正确性边界：`ICollection` 原快路径保持优先；read-only source 仍逐项复制源数据，顺序不变；Reset 到空集合后 view 仍清空；refresh 仍保留必要的一次 source 枚举来重建内部 list。状态验证覆盖 9 项 read-only source 的 internal list `Count/Capacity = 9/9`、只读 1 次 `Count`；read-only reset source 清空后 view.Count 为 0，Reset 空检查不再创建额外 enumerator，只留下 refresh 的一次枚举。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| 9-item `IReadOnlyCollection<object>` source copy List growth reallocations | 3 growths | 0 growths | `(3 - 0) / 3` | 100.00% | 有效；构造时按 read-only Count 预分配 |
| 9-item `IReadOnlyCollection<object>` source copy final List capacity | 16 slots | 9 slots | `(16 - 9) / 16` | 43.75% | 有效；状态验证覆盖 `Count/Capacity = 9/9` |
| Reset empty-check enumerators / read-only source reset-to-empty | 2 enumerators | 1 enumerator | `(2 - 1) / 2` | 50.00% | 有效；移除 empty-check enumerator，保留 refresh 枚举 |
| Reset empty-check `MoveNext()` calls / read-only source reset-to-empty | 2 calls | 1 call | `(2 - 1) / 2` | 50.00% | 有效；空检查直接读 Count |
| Reset result / read-only source reset-to-empty | view cleared | view cleared | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGridCollectionView 包装 `IReadOnlyCollection<object>` source 或 read-only source Reset 时触发；标准 DataGrid page-load smoke 不隔离该路径。Visual / Logical / KB 基本稳定，timing 只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.863 | 14.722 | `(15.863 - 14.722) / 15.863` | 7.19% | smoke-only；不隔离 read-only collection source path |
| DataGrid.Filter.Menu.Closed | 10.014 | 10.255 | `(10.014 - 10.255) / 10.014` | -2.41% | smoke-only；不隔离 read-only collection source path |
| DataGrid.Filter.Tree.Closed | 10.758 | 8.517 | `(10.758 - 8.517) / 10.758` | 20.83% | smoke-only；不隔离 read-only collection source path |
| DataGrid.RowHeaders | 10.019 | 11.239 | `(10.019 - 11.239) / 10.019` | -12.18% | smoke-only；不隔离 read-only collection source path |
| DataGrid.RowDetails.Collapsed | 11.141 | 10.603 | `(11.141 - 10.603) / 11.141` | 4.83% | smoke-only；不隔离 read-only collection source path |
| DataGrid.GroupHeaders | 10.629 | 9.267 | `(10.629 - 9.267) / 10.629` | 12.81% | smoke-only；不隔离 read-only collection source path |
| DataGrid.RowGroups | 10.294 | 10.309 | `(10.294 - 10.309) / 10.294` | -0.15% | smoke-only；不隔离 read-only collection source path |
| DataGrid.GalleryShape | 45.106 | 44.760 | `(45.106 - 44.760) / 45.106` | 0.77% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.722 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.255 | 2587.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.517 | 2583.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 11.239 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.603 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.267 | 2573.8 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.309 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.760 | 12409.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.105 CollectionView edit currency duplicate index lookup cleanup

本轮优化点：`DataGridCollectionView.AdjustCurrencyForEdit(newCurrentItem, index)` 在 edited item 仍位于 view 内时，先调用 `IndexOf(newCurrentItem)` 判断是否存在，随后又对同一个 item 再调用一次 `IndexOf(newCurrentItem)` 传给 `SetCurrent()`。这个路径在编辑提交、replace/edit collection change 后修正 current item 时触发。现在缓存第一次查找的 index，直接复用它设置 current position。

正确性边界：`newCurrentItem == null` 和 item 不在 view 内的 fallback 路径不变；item 在 view 内时，`CurrentItem` 和 `CurrentPosition` 与原行为一致。状态验证通过反射调用私有 `AdjustCurrencyForEdit()`，用 3 项 source 的计数 item 验证 edited item at index 2 时只扫描一次，`CurrentItem` 仍是目标 item，`CurrentPosition` 仍为 2。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `AdjustCurrencyForEdit` `IndexOf(newCurrentItem)` scans / edited item in view | 2 scans | 1 scan | `(2 - 1) / 2` | 50.00% | 有效；同一个 item 不再重复查找 |
| 3-item edit currency equality checks / edited item at index 2 | 6 calls | 3 calls | `(6 - 3) / 6` | 50.00% | 有效；状态验证覆盖一次扫描 |
| Current item after edit currency adjustment | target item | target item | behavior verified | n/a | 正确性保持 |
| Current position after edit currency adjustment | `2` | `2` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 collection edit/replace 后修正 current item 时触发；标准 DataGrid page-load smoke 不隔离该路径。复测两轮后 Visual / Logical / KB 基本稳定，timing 仍明显波动，只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.722 | 16.170 | `(14.722 - 16.170) / 14.722` | -9.84% | smoke-only；不隔离 edit currency path |
| DataGrid.Filter.Menu.Closed | 10.255 | 10.277 | `(10.255 - 10.277) / 10.255` | -0.21% | smoke-only；不隔离 edit currency path |
| DataGrid.Filter.Tree.Closed | 8.517 | 8.763 | `(8.517 - 8.763) / 8.517` | -2.89% | smoke-only；不隔离 edit currency path |
| DataGrid.RowHeaders | 11.239 | 10.815 | `(11.239 - 10.815) / 11.239` | 3.77% | smoke-only；不隔离 edit currency path |
| DataGrid.RowDetails.Collapsed | 10.603 | 11.074 | `(10.603 - 11.074) / 10.603` | -4.44% | smoke-only；不隔离 edit currency path |
| DataGrid.GroupHeaders | 9.267 | 9.475 | `(9.267 - 9.475) / 9.267` | -2.24% | smoke-only；不隔离 edit currency path |
| DataGrid.RowGroups | 10.309 | 10.179 | `(10.309 - 10.179) / 10.309` | 1.26% | smoke-only；不隔离 edit currency path |
| DataGrid.GalleryShape | 44.760 | 47.540 | `(44.760 - 47.540) / 44.760` | -6.21% | smoke-only；综合页面波动，不作为本轮回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 16.170 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.277 | 2586.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.763 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.815 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 11.074 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.475 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.179 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 47.540 | 12409.9 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.106 CollectionView tracking enumerator lifecycle cleanup

本轮优化点：`DataGridCollectionView` 构造时原本总会在完成 source copy 后再创建一个 `_trackingEnumerator`。但 `INotifyCollectionChanged` source 已通过 `WeakCollectionChangedForwarder` 接收增删改通知，不走 `_pollForChanges`，这个 tracking enumerator 在通知型源上不会被使用。现在只有非通知源才创建 tracking enumerator；非通知源的轮询语义保持，并在重置 tracking enumerator / `Dispose()` 时释放旧 enumerator。

正确性边界：通知型 source 仍完整复制初始数据，并继续通过 `CollectionChanged` forwarder 更新；非通知 source 仍保留一个 tracking enumerator 用于修改检测。状态验证覆盖通知型 3-item source 构造时只创建 1 个 source-copy enumerator，非通知型 source 构造时仍创建 2 个 enumerator，且 `Dispose()` 会释放保留的 tracking enumerator。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `INotifyCollectionChanged` source constructor enumerators / 3-item source | 2 enumerators | 1 enumerator | `(2 - 1) / 2` | 50.00% | 有效；移除未使用的 polling enumerator |
| Unused polling enumerators retained by notifying source | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；通知源不再持有无用 enumerator |
| Notifying source copy `MoveNext()` calls / 3-item source | 4 calls | 4 calls | behavior verified | n/a | 正确性保持；初始 copy 仍完整 |
| Non-notifying source constructor enumerators / 3-item source | 2 enumerators | 2 enumerators | behavior verified | n/a | 轮询语义保持 |
| Undisposed tracking enumerators after non-notifying view `Dispose()` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 正确性修复；释放保留的 tracking enumerator |

Smoke-only 对比上一轮同参数复测。本轮路径主要影响 `DataGridCollectionView` 包装通知型 source 的构造期和非通知型 source 的 Dispose 生命周期；标准 DataGrid page-load smoke 不隔离该路径。Visual / Logical / KB 基本稳定，timing 只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 16.170 | 17.254 | `(16.170 - 17.254) / 16.170` | -6.70% | smoke-only；不隔离 tracking enumerator path |
| DataGrid.Filter.Menu.Closed | 10.277 | 10.395 | `(10.277 - 10.395) / 10.277` | -1.15% | smoke-only；不隔离 tracking enumerator path |
| DataGrid.Filter.Tree.Closed | 8.763 | 9.405 | `(8.763 - 9.405) / 8.763` | -7.33% | smoke-only；不隔离 tracking enumerator path |
| DataGrid.RowHeaders | 10.815 | 10.546 | `(10.815 - 10.546) / 10.815` | 2.49% | smoke-only；不隔离 tracking enumerator path |
| DataGrid.RowDetails.Collapsed | 11.074 | 10.673 | `(11.074 - 10.673) / 11.074` | 3.62% | smoke-only；不隔离 tracking enumerator path |
| DataGrid.GroupHeaders | 9.475 | 9.431 | `(9.475 - 9.431) / 9.475` | 0.46% | smoke-only；不隔离 tracking enumerator path |
| DataGrid.RowGroups | 10.179 | 10.871 | `(10.179 - 10.871) / 10.179` | -6.80% | smoke-only；不隔离 tracking enumerator path |
| DataGrid.GalleryShape | 47.540 | 43.795 | `(47.540 - 43.795) / 47.540` | 7.88% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 17.254 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.395 | 2584.6 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.405 | 2583.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.546 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.673 | 3024.7 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.431 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.871 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.795 | 12409.6 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.107 CollectionView IReadOnlyList source copy fast path

本轮优化点：`DataGridCollectionView.CopySourceToInternalList()` 和无过滤 `PrepareLocalArray()` 之前即使已经通过 `IReadOnlyCollection<object>.Count` 预分配容量，真正复制 `IReadOnlyList<object>` source 时仍会创建 source enumerator 并 `MoveNext()` 到末尾。现在抽出 `CopySourceToList()`，对 `IReadOnlyList<object>` 直接用 `Count + indexer` 复制；普通 `IEnumerable` 仍保留原 raw enumerator + dispose 路径。

正确性边界：`IReadOnlyList<object>` 的复制顺序按 index 从 0 到 Count-1，内部 list 的 Count / Capacity 保持精确；普通 enumerable 仍走原枚举路径；filtered read-only list refresh 在 2.108 继续收敛。状态验证覆盖 9 项 read-only list 的构造、`Refresh()` 和添加 SortDescriptions 触发的无过滤 local-array refresh，均不调用 `GetEnumerator()` / `MoveNext()` / `Dispose()`，并验证 internal list 顺序仍为 0..8。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Constructor source-copy enumerators / 9-item `IReadOnlyList<object>` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；构造复制走 indexer |
| Constructor source-copy `MoveNext()` calls / 9-item `IReadOnlyList<object>` | 10 calls | 0 calls | `(10 - 0) / 10` | 100.00% | 有效；避免完整枚举 |
| `Refresh()` source-copy enumerators / 9-item `IReadOnlyList<object>` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；无过滤刷新走 indexer |
| Unfiltered local-array refresh enumerators / 9-item `IReadOnlyList<object>` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；SortDescriptions 触发刷新也不枚举 source |
| Internal list capacity / 9-item `IReadOnlyList<object>` | 9 slots | 9 slots | behavior verified | n/a | 正确性保持；容量仍精确 |
| Internal list order / 9-item `IReadOnlyList<object>` | `0..8` | `0..8` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGridCollectionView 包装 `IReadOnlyList<object>` source 或无过滤 local-array refresh 时触发；标准 DataGrid page-load smoke 不隔离该路径。Visual / Logical / KB 基本稳定，timing 只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 17.254 | 15.066 | `(17.254 - 15.066) / 17.254` | 12.68% | smoke-only；不隔离 read-only list source copy path |
| DataGrid.Filter.Menu.Closed | 10.395 | 9.268 | `(10.395 - 9.268) / 10.395` | 10.84% | smoke-only；不隔离 read-only list source copy path |
| DataGrid.Filter.Tree.Closed | 9.405 | 8.316 | `(9.405 - 8.316) / 9.405` | 11.58% | smoke-only；不隔离 read-only list source copy path |
| DataGrid.RowHeaders | 10.546 | 9.726 | `(10.546 - 9.726) / 10.546` | 7.78% | smoke-only；不隔离 read-only list source copy path |
| DataGrid.RowDetails.Collapsed | 10.673 | 9.973 | `(10.673 - 9.973) / 10.673` | 6.56% | smoke-only；不隔离 read-only list source copy path |
| DataGrid.GroupHeaders | 9.431 | 9.108 | `(9.431 - 9.108) / 9.431` | 3.42% | smoke-only；不隔离 read-only list source copy path |
| DataGrid.RowGroups | 10.871 | 9.941 | `(10.871 - 9.941) / 10.871` | 8.55% | smoke-only；不隔离 read-only list source copy path |
| DataGrid.GalleryShape | 43.795 | 43.288 | `(43.795 - 43.288) / 43.795` | 1.16% | smoke-only；不隔离 read-only list source copy path |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.066 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.268 | 2587.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.316 | 2583.8 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.726 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.973 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.108 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.941 | 2937.5 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.288 | 12409.5 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.108 CollectionView filtered IReadOnlyList refresh fast path

本轮优化点：`DataGridCollectionView.PrepareLocalArray()` 在有 `Filter` 时原先对所有 source 都走 `foreach`。如果 source 只实现 `IReadOnlyList<object>`，这个 filtered refresh 仍会创建 source enumerator 并完整 `MoveNext()` 扫描。现在 filtered path 对 `IReadOnlyList<object>` 改为读取一次 `Count`，再按 indexer 扫描并只把通过 `PassesFilter()` 的 item 加入结果；普通 enumerable 保留原 `foreach` 语义。

正确性边界：过滤判断仍通过原 `PassesFilter(item)`；结果顺序仍按 source index 从小到大；filtered result 不使用 source Count 预分配，避免少量命中时保留全量容量。状态验证覆盖 9 项 read-only list 设置 `Filter = item < 2` 后，internal list 只包含 `0, 1`，容量小于 9，且 `GetEnumerator()` / `MoveNext()` / `Dispose()` 都为 0。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Filtered refresh source enumerators / 9-item `IReadOnlyList<object>` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；过滤刷新不再创建 source enumerator |
| Filtered refresh `MoveNext()` calls / 9-item `IReadOnlyList<object>` | 10 calls | 0 calls | `(10 - 0) / 10` | 100.00% | 有效；完整扫描改为 indexer loop |
| Filtered refresh source enumerator disposals / 9-item `IReadOnlyList<object>` | 1 dispose | 0 disposes | `(1 - 0) / 1` | 100.00% | 有效；没有枚举器需要释放 |
| Filtered refresh indexer accesses / 9-item `IReadOnlyList<object>` | 0 calls | 9 calls | direct indexed scan | n/a | 行为更直接；每项仍执行一次过滤判断 |
| Filtered result capacity / 2 matched items from 9-item source | `< 9` slots | `< 9` slots | behavior verified | n/a | 正确性保持；没有按全量 source Count 预分配 |
| Filtered result order / `item < 2` | `0, 1` | `0, 1` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 `DataGridCollectionView` 包装 `IReadOnlyList<object>` 且设置 Filter 后刷新时触发；标准 DataGrid page-load smoke 不隔离该 data-layer 路径。连续复测 timing 波动较大，Visual / Logical / KB 基本稳定。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.066 | 17.167 | `(15.066 - 17.167) / 15.066` | -13.95% | smoke-only；不隔离 filtered read-only list refresh path |
| DataGrid.Filter.Menu.Closed | 9.268 | 8.937 | `(9.268 - 8.937) / 9.268` | 3.57% | smoke-only；不隔离 filtered read-only list refresh path |
| DataGrid.Filter.Tree.Closed | 8.316 | 8.181 | `(8.316 - 8.181) / 8.316` | 1.62% | smoke-only；不隔离 filtered read-only list refresh path |
| DataGrid.RowHeaders | 9.726 | 10.811 | `(9.726 - 10.811) / 9.726` | -11.16% | smoke-only；不隔离 filtered read-only list refresh path |
| DataGrid.RowDetails.Collapsed | 9.973 | 10.267 | `(9.973 - 10.267) / 9.973` | -2.95% | smoke-only；不隔离 filtered read-only list refresh path |
| DataGrid.GroupHeaders | 9.108 | 8.985 | `(9.108 - 8.985) / 9.108` | 1.35% | smoke-only；不隔离 filtered read-only list refresh path |
| DataGrid.RowGroups | 9.941 | 9.857 | `(9.941 - 9.857) / 9.941` | 0.84% | smoke-only；不隔离 filtered read-only list refresh path |
| DataGrid.GalleryShape | 43.288 | 45.287 | `(43.288 - 45.287) / 43.288` | -4.62% | smoke-only；综合页面波动，不作为本轮回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 17.167 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.937 | 2586.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.181 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.811 | 3107.3 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.267 | 3024.7 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.985 | 2573.3 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.857 | 2937.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 45.287 | 12409.8 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.109 ValidationUtils member-name comparison cleanup

本轮优化点：`ValidationUtils.FindEqualValidationResult()` 在 error message 相同后，会逐项比较两个 `ValidationResult.MemberNames` 序列。旧实现总是创建两个 enumerator；即使两边 member-name 数量不同，也要开始 `MoveNext()` 才能发现长度不一致，而且 fallback enumerator 没有显式释放。现在抽出 `MemberNamesMatch()`：两边都是 `IReadOnlyList<string>` 时走 Count + indexer；可读 Count 不一致时直接失败；普通 enumerable fallback 用 `using` 释放两个 enumerator。

正确性边界：error message 比较条件不变；member-name 顺序仍必须完全一致才认为相等；不同 member-name 序列仍返回 null；普通 enumerable 仍按原顺序逐项比较。状态验证覆盖不同长度 read-only list 不枚举、相同 read-only list 用 indexer 比较、普通 enumerable fallback 会 dispose，以及 `FindEqualValidationResult()` 的匹配 / 不匹配行为保持。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Different-length member-name comparison enumerators / pair | 2 enumerators | 0 enumerators | `(2 - 0) / 2` | 100.00% | 有效；Count 不一致直接失败 |
| Different-length member-name comparison `MoveNext()` calls / `1 vs 2` names | 4 calls | 0 calls | `(4 - 0) / 4` | 100.00% | 有效；不再进入逐项枚举 |
| Equal read-only list member-name comparison enumerators / 2 names | 2 enumerators | 0 enumerators | `(2 - 0) / 2` | 100.00% | 有效；改用 indexer 比较 |
| Equal read-only list member-name comparison `MoveNext()` calls / 2 names | 6 calls | 0 calls | `(6 - 0) / 6` | 100.00% | 有效；避免 `true,true,false` 枚举探测 |
| Fallback enumerable disposals / equal 2-name sequence | 0 disposes | 2 disposes | resource cleanup | n/a | 正确性修复；fallback enumerator 不再泄漏 |
| `FindEqualValidationResult()` matching behavior | matched result | matched result | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid validation 去重时触发；标准 DataGrid page-load smoke 不隔离该 validation helper。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 17.167 | 14.519 | `(17.167 - 14.519) / 17.167` | 15.42% | smoke-only；不隔离 validation member-name path |
| DataGrid.Filter.Menu.Closed | 8.937 | 9.129 | `(8.937 - 9.129) / 8.937` | -2.15% | smoke-only；不隔离 validation member-name path |
| DataGrid.Filter.Tree.Closed | 8.181 | 8.117 | `(8.181 - 8.117) / 8.181` | 0.78% | smoke-only；不隔离 validation member-name path |
| DataGrid.RowHeaders | 10.811 | 9.763 | `(10.811 - 9.763) / 10.811` | 9.69% | smoke-only；不隔离 validation member-name path |
| DataGrid.RowDetails.Collapsed | 10.267 | 9.853 | `(10.267 - 9.853) / 10.267` | 4.03% | smoke-only；不隔离 validation member-name path |
| DataGrid.GroupHeaders | 8.985 | 9.003 | `(8.985 - 9.003) / 8.985` | -0.20% | smoke-only；不隔离 validation member-name path |
| DataGrid.RowGroups | 9.857 | 10.114 | `(9.857 - 10.114) / 9.857` | -2.61% | smoke-only；不隔离 validation member-name path |
| DataGrid.GalleryShape | 45.287 | 43.292 | `(45.287 - 43.292) / 45.287` | 4.41% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.519 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.129 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.117 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.763 | 3107.1 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.853 | 3024.2 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.003 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.114 | 2937.4 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.292 | 12409.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.110 TypeHelper IReadOnlyList item-type probe fast path

本轮优化点：`DataGridDataConnection.DataType` 会通过 `ItemsSource.GetItemType()` 推断行数据类型。当 `ItemsSource` 是 `IReadOnlyList<object>` 这类只能从泛型接口推到 `object` 的数据源时，旧路径会创建 enumerator 并 `MoveNext()` 一次来取代表项类型。现在 `TypeHelper.GetItemType()` 在 fallback 代表项探测前先识别 `IReadOnlyList<object?>`，用 `Count + indexer` 找第一个非空 item；普通 bare `IEnumerable` 仍走上一轮已验证的 enumerator + dispose fallback。

正确性边界：能从泛型类型直接推断出具体 item type 的集合不受影响；`IReadOnlyList<object>` 仍返回第一个非空代表项的真实运行时类型；空列表或全 null 列表仍回到原来的 `itemType` fallback；普通 enumerable 的 disposal 语义保持。状态验证覆盖 4 项 read-only list 的 `DataType == typeof(int)`，只读 1 次 Count、1 次 indexer，且不调用 `GetEnumerator()` / `MoveNext()` / `Dispose()`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `IReadOnlyList<object>` item-type probe enumerators / DataType read | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；代表项探测不再创建 enumerator |
| `IReadOnlyList<object>` item-type probe `MoveNext()` calls / first item non-null | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；第一项通过 indexer 读取 |
| `IReadOnlyList<object>` item-type probe indexer calls / first item non-null | 0 calls | 1 call | direct indexed access | n/a | 行为更直接；状态验证覆盖返回类型 |
| `IReadOnlyList<object>` item-type probe Count reads / DataType read | 0 reads | 1 read | direct count guard | n/a | 用 Count 控制 indexed scan 边界 |
| Returned `DataType` / 4-item read-only list | `typeof(int)` | `typeof(int)` | behavior verified | n/a | 正确性保持 |
| Bare `IEnumerable` item-type probe disposal | 1 dispose | 1 dispose | behavior verified | n/a | 上一轮 fallback 语义保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid 对 `IReadOnlyList<object>` / object-typed ItemsSource 推断 DataType 时触发；标准 DataGrid page-load smoke 不隔离该 helper。复测两次 timing 都偏慢，但 Visual / Logical / KB 稳定，说明没有额外控件树或内存形态变化；下表使用第二次复测，只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.519 | 15.795 | `(14.519 - 15.795) / 14.519` | -8.79% | smoke-only；不隔离 read-only list item-type path |
| DataGrid.Filter.Menu.Closed | 9.129 | 10.073 | `(9.129 - 10.073) / 9.129` | -10.34% | smoke-only；不隔离 read-only list item-type path |
| DataGrid.Filter.Tree.Closed | 8.117 | 9.016 | `(8.117 - 9.016) / 8.117` | -11.08% | smoke-only；不隔离 read-only list item-type path |
| DataGrid.RowHeaders | 9.763 | 10.868 | `(9.763 - 10.868) / 9.763` | -11.32% | smoke-only；不隔离 read-only list item-type path |
| DataGrid.RowDetails.Collapsed | 9.853 | 10.957 | `(9.853 - 10.957) / 9.853` | -11.20% | smoke-only；不隔离 read-only list item-type path |
| DataGrid.GroupHeaders | 9.003 | 10.320 | `(9.003 - 10.320) / 9.003` | -14.63% | smoke-only；不隔离 read-only list item-type path |
| DataGrid.RowGroups | 10.114 | 10.564 | `(10.114 - 10.564) / 10.114` | -4.45% | smoke-only；不隔离 read-only list item-type path |
| DataGrid.GalleryShape | 43.292 | 47.289 | `(43.292 - 47.289) / 43.292` | -9.23% | smoke-only；两轮 timing 偏慢，不作为本轮回归证明 |

当前工作区 smoke（第二轮复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.795 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.Filter.Menu.Closed | 10.073 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.Filter.Tree.Closed | 9.016 | 2583.7 | 267.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.RowHeaders | 10.868 | 3107.1 | 336.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.957 | 3024.5 | 320.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.GroupHeaders | 10.320 | 2573.1 | 266.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.RowGroups | 10.564 | 2937.4 | 315.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.GalleryShape | 47.289 | 12409.2 | 1260.0 | 5.0 | smoke 通过；timing 偏慢，只作异常检查 |

### 2.111 TypeHelper IList item-type probe fast path

本轮优化点：`DataGridDataConnection.DataType` 通过 `ItemsSource.GetItemType()` 推断行数据类型时，非泛型 `IList` / `ArrayList` 这类数据源也经常只能先得到 `object`。旧路径会创建 enumerator 并 `MoveNext()` 一次来取代表项类型；现在 `TypeHelper.GetItemType()` 在 fallback 枚举前先识别 `IList`，用 `Count + indexer` 找第一个非空 item。`IReadOnlyList<object>` 和普通 bare `IEnumerable` 的路径保持上一轮语义。

正确性边界：能从泛型类型直接推断出具体 item type 的集合不受影响；非泛型 `IList` 仍返回第一个非空代表项的真实运行时类型；空列表或全 null 列表仍回到原来的 `itemType` fallback；普通 enumerable 的 disposal 语义保持。状态验证覆盖 3 项 `IList` 的 `DataType == typeof(int)`，只读 1 次 Count、1 次 indexer，且不调用 `GetEnumerator()` / `MoveNext()` / `Dispose()`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| `IList` item-type probe enumerators / DataType read | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；代表项探测不再创建 enumerator |
| `IList` item-type probe `MoveNext()` calls / first item non-null | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 有效；第一项通过 indexer 读取 |
| `IList` item-type probe indexer calls / first item non-null | 0 calls | 1 call | direct indexed access | n/a | 行为更直接；状态验证覆盖返回类型 |
| `IList` item-type probe Count reads / DataType read | 0 reads | 1 read | direct count guard | n/a | 用 Count 控制 indexed scan 边界 |
| Returned `DataType` / 3-item `IList` | `typeof(int)` | `typeof(int)` | behavior verified | n/a | 正确性保持 |
| Bare `IEnumerable` and `IReadOnlyList<object>` item-type paths | verified | verified | behavior preserved | n/a | 上一轮 fallback / read-only list 语义保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid 对非泛型 `IList` / object-typed ItemsSource 推断 DataType 时触发；标准 DataGrid page-load smoke 不隔离该 helper。复测两次 timing 都明显偏慢，但 Visual / Logical / KB 稳定，说明没有额外控件树或内存形态变化；下表使用第二次复测，只作异常检查，不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.795 | 20.661 | `(15.795 - 20.661) / 15.795` | -30.81% | smoke-only；不隔离 IList item-type path |
| DataGrid.Filter.Menu.Closed | 10.073 | 11.249 | `(10.073 - 11.249) / 10.073` | -11.67% | smoke-only；不隔离 IList item-type path |
| DataGrid.Filter.Tree.Closed | 9.016 | 10.656 | `(9.016 - 10.656) / 9.016` | -18.19% | smoke-only；不隔离 IList item-type path |
| DataGrid.RowHeaders | 10.868 | 13.117 | `(10.868 - 13.117) / 10.868` | -20.69% | smoke-only；不隔离 IList item-type path |
| DataGrid.RowDetails.Collapsed | 10.957 | 13.319 | `(10.957 - 13.319) / 10.957` | -21.56% | smoke-only；不隔离 IList item-type path |
| DataGrid.GroupHeaders | 10.320 | 11.044 | `(10.320 - 11.044) / 10.320` | -7.02% | smoke-only；不隔离 IList item-type path |
| DataGrid.RowGroups | 10.564 | 12.302 | `(10.564 - 12.302) / 10.564` | -16.45% | smoke-only；不隔离 IList item-type path |
| DataGrid.GalleryShape | 47.289 | 53.381 | `(47.289 - 53.381) / 47.289` | -12.88% | smoke-only；两轮 timing 偏慢，不作为本轮回归证明 |

当前工作区 smoke（第二轮复测）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 20.661 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.Filter.Menu.Closed | 11.249 | 2583.9 | 267.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.Filter.Tree.Closed | 10.656 | 2583.1 | 267.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.RowHeaders | 13.117 | 3107.0 | 336.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.RowDetails.Collapsed | 13.319 | 3024.4 | 320.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.GroupHeaders | 11.044 | 2573.1 | 266.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.RowGroups | 12.302 | 2937.1 | 315.0 | 1.0 | smoke 通过；timing 偏慢，只作异常检查 |
| DataGrid.GalleryShape | 53.381 | 12408.4 | 1260.0 | 5.0 | smoke 通过；timing 偏慢，只作异常检查 |

### 2.112 CollectionView IList source copy/filter fast path

本轮优化点：`DataGridCollectionView` 复制 source 到内部列表时，上一轮已经覆盖 `IReadOnlyList<object>`，但非泛型 `IList` / `ArrayList` 仍会在 source copy、Refresh、无过滤 local-array refresh 和 filtered refresh 中创建 source enumerator。现在 `CopySourceToList()` 和 `CopyFilteredSourceToList()` 增加 `IList` 快路径，用 `Count + indexer` 复制或扫描源项；已有的 `IReadOnlyList<object>` 和普通 enumerable fallback 路径保持。

正确性边界：`IReadOnlyList<object>` 原快路径保持优先；`IList` 源集合复制顺序仍按 index 从 0 到 Count-1；无过滤 refresh 仍保持精确 Count/Capacity；带 Filter 的 refresh 仍只保留通过 `PassesFilter()` 的 item，且不按全量源 Count 预分配 filtered result。状态验证覆盖 9 项 `IList` 的构造、Refresh、无过滤 local-array refresh 和 filtered refresh，均只读 1 次 Count、按项读取 indexer，且不调用 `GetEnumerator()` / `MoveNext()` / `Dispose()`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Constructor source-copy enumerators / 9-item `IList` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；构造复制不再创建 source enumerator |
| Constructor source-copy `MoveNext()` calls / 9-item `IList` | 10 calls | 0 calls | `(10 - 0) / 10` | 100.00% | 有效；完整枚举改为 indexer loop |
| `Refresh()` source-copy enumerators / 9-item `IList` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；刷新复制走 indexer |
| Unfiltered local-array refresh enumerators / 9-item `IList` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；SortDescriptions 触发刷新也不枚举 source |
| Filtered refresh source enumerators / 9-item `IList` | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；过滤扫描不再创建 source enumerator |
| Filtered refresh `MoveNext()` calls / 9-item `IList` | 10 calls | 0 calls | `(10 - 0) / 10` | 100.00% | 有效；完整扫描改为 indexer loop |
| Internal list order / 9-item `IList` | `0..8` | `0..8` | behavior verified | n/a | 正确性保持 |
| Filtered result / `item < 2` | `0, 1` | `0, 1` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 `DataGridCollectionView` 包装非泛型 `IList` source 或对该 source 做 filtered refresh 时触发；标准 DataGrid page-load smoke 不隔离该 data-layer 路径。上一轮 timing 明显偏慢，本轮 timing 回落且 Visual / Logical / KB 稳定；下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 20.661 | 14.124 | `(20.661 - 14.124) / 20.661` | 31.64% | smoke-only；不隔离 IList source copy/filter path |
| DataGrid.Filter.Menu.Closed | 11.249 | 9.364 | `(11.249 - 9.364) / 11.249` | 16.76% | smoke-only；不隔离 IList source copy/filter path |
| DataGrid.Filter.Tree.Closed | 10.656 | 8.277 | `(10.656 - 8.277) / 10.656` | 22.33% | smoke-only；不隔离 IList source copy/filter path |
| DataGrid.RowHeaders | 13.117 | 9.560 | `(13.117 - 9.560) / 13.117` | 27.12% | smoke-only；不隔离 IList source copy/filter path |
| DataGrid.RowDetails.Collapsed | 13.319 | 10.187 | `(13.319 - 10.187) / 13.319` | 23.52% | smoke-only；不隔离 IList source copy/filter path |
| DataGrid.GroupHeaders | 11.044 | 8.645 | `(11.044 - 8.645) / 11.044` | 21.72% | smoke-only；不隔离 IList source copy/filter path |
| DataGrid.RowGroups | 12.302 | 9.469 | `(12.302 - 9.469) / 12.302` | 23.03% | smoke-only；不隔离 IList source copy/filter path |
| DataGrid.GalleryShape | 53.381 | 42.410 | `(53.381 - 42.410) / 53.381` | 20.55% | smoke-only；上一轮 timing 偏慢，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.124 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.364 | 2586.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.277 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.560 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.187 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.645 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.469 | 2937.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 42.410 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.113 CollectionView representative item indexed lookup

本轮优化点：`DataGridCollectionView.GetRepresentativeItem()` 原来即使在非分组、非分页的普通 view 上，也会通过 `GetEnumerator()` 创建 `NewItemAwareEnumerator` 和内部列表枚举器来找第一个非空代表项。现在普通路径改为 `Count + GetItemAt(index)` 扫描，避免为 item type fallback 创建枚举器；分组、分页和 `AddNew` 事务仍保留旧枚举器路径，避免改变 `NewItemAwareEnumerator` 对新增项位置的语义。

正确性边界：只优化非分组、非分页、非 `AddNew` 的代表项查找；全空列表仍返回 null；第一个非空 item 的返回值不变；分组 / 分页 / 新增事务继续走原枚举器语义。状态验证覆盖内部列表 `[null, 4, 5]`，返回 `4`，且不调用内部列表 `GetEnumerator()` / `MoveNext()` / `Dispose()`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Representative item lookup internal-list enumerators / non-grouped non-paged view | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；普通代表项查找不再创建内部列表枚举器 |
| Representative item lookup `MoveNext()` calls / first non-null at index 1 | 2 calls | 0 calls | `(2 - 0) / 2` | 100.00% | 有效；null 后首个非空项改为 indexer 扫描 |
| Representative item lookup enumerator dispose path | 1 possible dispose check/path | 0 enumerator dispose path | `(1 - 0) / 1` | 100.00% | 有效；没有枚举器就没有释放路径成本 |
| Representative item lookup indexer calls / first non-null at index 1 | 0 calls | 2 calls | direct indexed scan | n/a | 行为更直接；状态验证覆盖 null + first non-null |
| Representative item result / `[null, 4, 5]` | `4` | `4` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 `DataGridCollectionView` 需要用代表项推断 item type，且 view 处于非分组、非分页、非新增事务时触发；标准 DataGrid page-load smoke 不隔离该 data-layer 路径。本轮 timing 比上一轮略慢，但 Visual / Logical / KB 形态稳定，因此只作异常检查，不作为页面加载速度回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.124 | 15.162 | `(14.124 - 15.162) / 14.124` | -7.35% | smoke-only；不隔离 representative item lookup |
| DataGrid.Filter.Menu.Closed | 9.364 | 8.765 | `(9.364 - 8.765) / 9.364` | 6.40% | smoke-only；不隔离 representative item lookup |
| DataGrid.Filter.Tree.Closed | 8.277 | 8.403 | `(8.277 - 8.403) / 8.277` | -1.52% | smoke-only；不隔离 representative item lookup |
| DataGrid.RowHeaders | 9.560 | 9.817 | `(9.560 - 9.817) / 9.560` | -2.69% | smoke-only；不隔离 representative item lookup |
| DataGrid.RowDetails.Collapsed | 10.187 | 10.244 | `(10.187 - 10.244) / 10.187` | -0.56% | smoke-only；不隔离 representative item lookup |
| DataGrid.GroupHeaders | 8.645 | 8.684 | `(8.645 - 8.684) / 8.645` | -0.45% | smoke-only；不隔离 representative item lookup |
| DataGrid.RowGroups | 9.469 | 10.031 | `(9.469 - 10.031) / 9.469` | -5.94% | smoke-only；不隔离 representative item lookup |
| DataGrid.GalleryShape | 42.410 | 44.192 | `(42.410 - 44.192) / 42.410` | -4.20% | smoke-only；不隔离 representative item lookup，不作为速度收益或回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.162 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.765 | 2586.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.403 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.817 | 3107.1 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.244 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.684 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.031 | 2937.2 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.192 | 12409.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.114 CollectionView trivial sorted refresh sort-chain skip

本轮优化点：`DataGridCollectionView.SortList()` 在 0/1 条数据时，旧实现仍会构造 `OrderBy` / `ThenBy` 链并 materialize 一个新的 sorted list。现在仍然按原顺序初始化每个 sort description，但当 `list.Count <= 1` 时直接返回当前 list，不再创建 LINQ 排序链，也不再做第二次列表 materialization；2 条及以上数据仍走原排序链。

正确性边界：sort description 的 `Initialize(itemType)` 仍会执行，避免隐藏属性路径初始化问题；只跳过 0/1 项无需排序的链路；多项数据排序仍调用 `OrderBy` 并保持排序结果。状态验证覆盖单项 + 两个 sort descriptions 时 `OrderBy/ThenBy == 0`，以及两项数据仍调用一次 `OrderBy` 并排序为 `1, 2`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Sort-chain calls / singleton refresh with 2 sort descriptions | 2 calls | 0 calls | `(2 - 0) / 2` | 100.00% | 有效；单项刷新不再构造 `OrderBy` / `ThenBy` 链 |
| Extra sorted-list materialization / singleton refresh | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 有效；直接复用当前 local list |
| Singleton sorted item result | `42` | `42` | behavior verified | n/a | 正确性保持 |
| Multi-item sorted refresh `OrderBy` calls / 2 items | 1 call | 1 call | behavior preserved | n/a | 正确性保持；多项仍排序 |
| Multi-item sorted order / input `2, 1` | `1, 2` | `1, 2` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在排序刷新后的结果为 0/1 项时触发；标准 DataGrid page-load smoke 不隔离该 data-layer 路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.162 | 14.546 | `(15.162 - 14.546) / 15.162` | 4.06% | smoke-only；不隔离 singleton sorted refresh |
| DataGrid.Filter.Menu.Closed | 8.765 | 8.861 | `(8.765 - 8.861) / 8.765` | -1.10% | smoke-only；不隔离 singleton sorted refresh |
| DataGrid.Filter.Tree.Closed | 8.403 | 7.932 | `(8.403 - 7.932) / 8.403` | 5.61% | smoke-only；不隔离 singleton sorted refresh |
| DataGrid.RowHeaders | 9.817 | 9.198 | `(9.817 - 9.198) / 9.817` | 6.31% | smoke-only；不隔离 singleton sorted refresh |
| DataGrid.RowDetails.Collapsed | 10.244 | 9.761 | `(10.244 - 9.761) / 10.244` | 4.71% | smoke-only；不隔离 singleton sorted refresh |
| DataGrid.GroupHeaders | 8.684 | 9.031 | `(8.684 - 9.031) / 8.684` | -4.00% | smoke-only；不隔离 singleton sorted refresh |
| DataGrid.RowGroups | 10.031 | 9.503 | `(10.031 - 9.503) / 10.031` | 5.26% | smoke-only；不隔离 singleton sorted refresh |
| DataGrid.GalleryShape | 44.192 | 41.923 | `(44.192 - 41.923) / 44.192` | 5.13% | smoke-only；不隔离 singleton sorted refresh，不作为稳定速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.546 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.861 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.932 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.198 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.761 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.031 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.503 | 2937.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 41.923 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.115 CollectionView empty sorted insert comparer skip

本轮优化点：`DataGridCollectionView.ProcessInsertToCollection()` 在 sorted view 空列表首次插入时，旧实现仍会构造 `MergedComparer` 并读取每个 sort description 的 comparer。现在仍保留 sort description 初始化，但当 `InternalList.Count == 0` 时跳过 `MergedComparer` 构造和比较流程，随后统一由原有边界修正把插入位置落到 `0`；非空列表插入仍走原排序比较和二分插入。

正确性边界：过滤判断仍先执行；sort description 初始化仍执行；只跳过空列表无可比较对象时的 comparer array；已有数据时仍构造 comparer 并保持排序插入结果。状态验证覆盖空 sorted insert 后 item 正确插入且 comparer access 为 0，非空 sorted insert 仍访问 comparer 1 次并把 `2, 1` 排为 `1, 2`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Comparer accesses / empty sorted insert with 1 sort description | 1 access | 0 accesses | `(1 - 0) / 1` | 100.00% | 有效；空列表首次插入不再构造 `MergedComparer` |
| Merged comparer arrays / empty sorted insert | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 有效；无可比较对象时跳过 comparer array |
| Empty sorted insert result | inserted at `0` | inserted at `0` | behavior verified | n/a | 正确性保持 |
| Non-empty sorted insert comparer accesses / 1 existing item | 1 access | 1 access | behavior preserved | n/a | 正确性保持；非空仍比较 |
| Non-empty sorted insert order / input `2, 1` | `1, 2` | `1, 2` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 sorted view 从空列表插入第一项时触发；标准 DataGrid page-load smoke 不隔离该 data-layer 路径。下表只作异常检查，不把单次 timing 当成本轮页面加载速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.546 | 12.649 | `(14.546 - 12.649) / 14.546` | 13.04% | smoke-only；不隔离 empty sorted insert |
| DataGrid.Filter.Menu.Closed | 8.861 | 8.827 | `(8.861 - 8.827) / 8.861` | 0.38% | smoke-only；不隔离 empty sorted insert |
| DataGrid.Filter.Tree.Closed | 7.932 | 7.841 | `(7.932 - 7.841) / 7.932` | 1.15% | smoke-only；不隔离 empty sorted insert |
| DataGrid.RowHeaders | 9.198 | 9.070 | `(9.198 - 9.070) / 9.198` | 1.39% | smoke-only；不隔离 empty sorted insert |
| DataGrid.RowDetails.Collapsed | 9.761 | 8.953 | `(9.761 - 8.953) / 9.761` | 8.28% | smoke-only；不隔离 empty sorted insert |
| DataGrid.GroupHeaders | 9.031 | 8.414 | `(9.031 - 8.414) / 9.031` | 6.83% | smoke-only；不隔离 empty sorted insert |
| DataGrid.RowGroups | 9.503 | 9.127 | `(9.503 - 9.127) / 9.503` | 3.96% | smoke-only；不隔离 empty sorted insert |
| DataGrid.GalleryShape | 41.923 | 40.826 | `(41.923 - 40.826) / 41.923` | 2.62% | smoke-only；不隔离 empty sorted insert，不作为稳定速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 12.649 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.827 | 2587.7 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.841 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.070 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 8.953 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.414 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.127 | 2937.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 40.826 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.116 PathSortDescription default comparer cache

本轮优化点：`DataGridPathSortDescription` 解析非 string 属性类型的默认 comparer 时，旧路径每个 sort description 都会通过反射取 `Comparer<T>.Default`。现在非 string 类型 comparer 按 `Type` 静态缓存；string comparer 仍保留每个 sort description 的 culture-sensitive comparer，不共享 culture 状态。

正确性边界：custom comparer 仍优先；string/culture 路径未改；非 string 默认 comparer 只缓存按类型解析出的 comparer；排序方向、path key selector 和 `MergedComparer` 语义保持。状态验证覆盖重复 `int` comparer lookup 只保留 1 个缓存项、第二次返回同一 comparer，并确认 cached comparer 下 path sort 仍把 group `2, 1` 排为 `1, 2`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Reflection default-comparer lookup / repeated same non-string type | 2 lookups | 1 lookup | `(2 - 1) / 2` | 50.00% | 有效；第二次同类型直接走 cache |
| Default-comparer cache entries / repeated `int` lookup | n/a | 1 entry | cache verified | n/a | 有效；按 Type 只缓存一个条目 |
| Repeated `int` comparer object | may resolve through reflection each time | same cached comparer | behavior verified | n/a | 正确性保持 |
| Path sort order / group `2, 1` | `1, 2` | `1, 2` | behavior verified | n/a | 正确性保持 |
| String culture comparer sharing | per sort description | per sort description | unchanged | n/a | 正确性保持；未共享 culture state |

Smoke-only 对比上一轮同参数复测。本轮路径只在创建 path sort description 并解析非 string 默认 comparer 时触发；标准 DataGrid page-load smoke 不隔离该 data-layer 路径。本轮 timing 偏慢但 Visual / Logical / KB 形态稳定，下表只作异常检查，不把单次 timing 当页面速度回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 12.649 | 14.726 | `(12.649 - 14.726) / 12.649` | -16.42% | smoke-only；不隔离 path sort comparer cache |
| DataGrid.Filter.Menu.Closed | 8.827 | 9.683 | `(8.827 - 9.683) / 8.827` | -9.70% | smoke-only；不隔离 path sort comparer cache |
| DataGrid.Filter.Tree.Closed | 7.841 | 8.580 | `(7.841 - 8.580) / 7.841` | -9.42% | smoke-only；不隔离 path sort comparer cache |
| DataGrid.RowHeaders | 9.070 | 10.215 | `(9.070 - 10.215) / 9.070` | -12.62% | smoke-only；不隔离 path sort comparer cache |
| DataGrid.RowDetails.Collapsed | 8.953 | 10.346 | `(8.953 - 10.346) / 8.953` | -15.56% | smoke-only；不隔离 path sort comparer cache |
| DataGrid.GroupHeaders | 8.414 | 9.077 | `(8.414 - 9.077) / 8.414` | -7.88% | smoke-only；不隔离 path sort comparer cache |
| DataGrid.RowGroups | 9.127 | 9.405 | `(9.127 - 9.405) / 9.127` | -3.05% | smoke-only；不隔离 path sort comparer cache |
| DataGrid.GalleryShape | 40.826 | 41.208 | `(40.826 - 41.208) / 40.826` | -0.94% | smoke-only；不隔离 path sort comparer cache，不作为速度收益或回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.726 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.683 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.580 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.215 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.346 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.077 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.405 | 2937.3 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 41.208 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.117 TypeHelper simple property path split fast path

本轮优化点：`TypeHelper.SplitPropertyPath()` 是 DataGrid 自动生成列、只读判断、sort/filter path 解析都会走到的基础路径。对于 `"Name"` 这类没有 nested separator、indexer 或 parenthesis 的简单属性名，旧实现仍会进入通用 splitter，首次 `Add()` 把 `List<string>` 从 0 容量扩到默认 4 个槽。现在简单属性名直接返回容量为 1 的 segment list；复杂路径继续走原 parser，保持 nested / indexer / parenthesis 语义。

正确性边界：空路径仍返回空 list；复杂路径没有改写解析逻辑；简单路径返回的 segment 内容与旧行为一致。状态验证覆盖简单属性路径 `PlainName` 的结果和容量、复杂路径 `Customer.Address[0].Name` 的拆分顺序，以及 empty path 的 `Count=0 / Capacity=0`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Simple property path segment list capacity / `PlainName` | 4 slots | 1 slot | `(4 - 1) / 4` | 75.00% | 有效；单段路径不再保留 4 个槽 |
| Simple property path List growths during split / `PlainName` | 1 growth | 0 growths | `(1 - 0) / 1` | 100.00% | 有效；直接按 1 个 segment 初始化 |
| Simple property path segments / `PlainName` | `PlainName` | `PlainName` | behavior verified | n/a | 正确性保持 |
| Nested/indexer path segments / `Customer.Address[0].Name` | `Customer`, `Address`, `[0]`, `Name` | same | behavior verified | n/a | 正确性保持；复杂 parser 未改 |
| Empty property path list | `Count=0 / Capacity=0` | `Count=0 / Capacity=0` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮收益只在简单 property path 解析时体现；标准 DataGrid page-load smoke 混合了模板、layout 和运行时抖动，不能隔离该路径。Visual / Logical / KB 形态稳定，下表只作异常检查，不把单次 timing 当页面速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.726 | 15.165 | `(14.726 - 15.165) / 14.726` | -2.98% | smoke-only；不隔离 simple path split |
| DataGrid.Filter.Menu.Closed | 9.683 | 8.793 | `(9.683 - 8.793) / 9.683` | 9.19% | smoke-only；不隔离 simple path split |
| DataGrid.Filter.Tree.Closed | 8.580 | 7.729 | `(8.580 - 7.729) / 8.580` | 9.92% | smoke-only；不隔离 simple path split |
| DataGrid.RowHeaders | 10.215 | 9.260 | `(10.215 - 9.260) / 10.215` | 9.35% | smoke-only；不隔离 simple path split |
| DataGrid.RowDetails.Collapsed | 10.346 | 9.590 | `(10.346 - 9.590) / 10.346` | 7.31% | smoke-only；不隔离 simple path split |
| DataGrid.GroupHeaders | 9.077 | 8.552 | `(9.077 - 8.552) / 9.077` | 5.78% | smoke-only；不隔离 simple path split |
| DataGrid.RowGroups | 9.405 | 9.895 | `(9.405 - 9.895) / 9.405` | -5.21% | smoke-only；不隔离 simple path split |
| DataGrid.GalleryShape | 41.208 | 40.952 | `(41.208 - 40.952) / 41.208` | 0.62% | smoke-only；不隔离 simple path split，不作为稳定速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.165 | 2947.7 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.793 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.729 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.260 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.590 | 3024.7 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.552 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.895 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 40.952 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.118 ValidationUtils read-only member-name contains fast path

本轮优化点：`ValidationUtils.ContainsMemberName()` 用于 DataGrid validation result 的成员名匹配。旧路径对 `ValidationResult.MemberNames` 一律 `foreach`，即使来源是数组 / read-only list，也会创建枚举器并通过 `MoveNext()` 扫描。现在 read-only list 走 `Count + indexer`；空 member-name list 只读 `Count` 即可保持空 target 语义；非 indexable enumerable 仍保留原枚举路径并释放枚举器。

正确性边界：成员名命中、未命中、empty target + empty member names 的语义保持；fallback enumerable 语义不变。状态验证覆盖 read-only list 命中 / 未命中都不创建 enumerator，empty list 只读 Count，fallback enumerable 仍枚举并 dispose。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Read-only member-name contains enumerators / match at index 1 | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；read-only list 直接按 index 扫描 |
| Read-only member-name contains `MoveNext()` calls / match at index 1 | 2 calls | 0 calls | `(2 - 0) / 2` | 100.00% | 有效；不走 enumerable path |
| Read-only member-name missing lookup enumerators / 2 names | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；未命中也不创建枚举器 |
| Empty member-name list lookup work / empty target | enumerator + `MoveNext()` | 1 Count read | structural fast path | n/a | 有效；空列表只读 Count |
| Fallback enumerable semantics / match at index 1 | match + dispose | match + dispose | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid validation result member-name contains 检查时触发；标准 page-load smoke 不隔离该 validation 路径。Visual / Logical / KB 形态稳定，timing 波动只作异常检查，不作为本轮速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.165 | 14.141 | `(15.165 - 14.141) / 15.165` | 6.75% | smoke-only；不隔离 validation member-name contains |
| DataGrid.Filter.Menu.Closed | 8.793 | 9.303 | `(8.793 - 9.303) / 8.793` | -5.80% | smoke-only；不隔离 validation member-name contains |
| DataGrid.Filter.Tree.Closed | 7.729 | 8.812 | `(7.729 - 8.812) / 7.729` | -14.01% | smoke-only；不隔离 validation member-name contains |
| DataGrid.RowHeaders | 9.260 | 10.243 | `(9.260 - 10.243) / 9.260` | -10.62% | smoke-only；不隔离 validation member-name contains |
| DataGrid.RowDetails.Collapsed | 9.590 | 10.145 | `(9.590 - 10.145) / 9.590` | -5.79% | smoke-only；不隔离 validation member-name contains |
| DataGrid.GroupHeaders | 8.552 | 9.071 | `(8.552 - 9.071) / 8.552` | -6.07% | smoke-only；不隔离 validation member-name contains |
| DataGrid.RowGroups | 9.895 | 9.938 | `(9.895 - 9.938) / 9.895` | -0.43% | smoke-only；不隔离 validation member-name contains |
| DataGrid.GalleryShape | 40.952 | 44.014 | `(40.952 - 44.014) / 40.952` | -7.48% | smoke-only；不隔离 validation member-name contains，不作为稳定速度回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.141 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.303 | 2586.5 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.812 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.243 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 10.145 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.071 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.938 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 44.014 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.119 CollectionView collection-changed old-items indexed loop

本轮优化点：`DataGridCollectionView.ProcessCollectionChanged()` 在处理 source collection 的 Remove / Replace 通知时，`NotifyCollectionChangedEventArgs.OldItems` 已经是 `IList`，旧路径仍用 `foreach` 扫描 removed items，会为批量 old-items 创建枚举器。现在改为一次读取 `Count`，再按 index 读取 old item 并调用原来的 remove 流程；Add/NewItems 路径原本已按 index 处理，本轮不动。

正确性边界：Remove / Replace 的每个 old item 仍按原顺序进入 `ProcessRemoveEvent()`；分页、分组和 currency 修正逻辑不变；old item 仍沿用原来“非 null item”的假设。状态验证通过反射调用 private `ProcessCollectionChanged()`，用 4 项 view 批量 remove old items `[1, 2]`，确认 old-items list 只读 1 次 Count、2 次 indexer、不创建 enumerator，最终 view 剩余 `[0, 3]`。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| OldItems enumerators / remove 2 items | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；remove old-items 不再走 enumerable |
| OldItems `MoveNext()` calls / remove 2 items | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | 有效；避免遍历器扫描和结束探测 |
| OldItems indexer calls / remove 2 items | 0 calls | 2 calls | direct indexed access | n/a | 行为更直接；状态验证覆盖顺序 |
| Removed result / source `[0,1,2,3]`, old `[1,2]` | `[0,3]` | `[0,3]` | behavior verified | n/a | 正确性保持 |
| Replace/Add new-items path | unchanged | unchanged | unchanged | n/a | 正确性保持；本轮只改 old-items |

Smoke-only 对比上一轮同参数复测。本轮路径只在 source collection 发出 Remove / Replace old-items 通知时触发；标准 page-load smoke 不隔离该 collection-changed 路径。Visual / Logical / KB 形态稳定，timing 只作异常检查，不作为本轮速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.141 | 14.937 | `(14.141 - 14.937) / 14.141` | -5.63% | smoke-only；不隔离 collection-changed old-items |
| DataGrid.Filter.Menu.Closed | 9.303 | 9.507 | `(9.303 - 9.507) / 9.303` | -2.19% | smoke-only；不隔离 collection-changed old-items |
| DataGrid.Filter.Tree.Closed | 8.812 | 8.689 | `(8.812 - 8.689) / 8.812` | 1.40% | smoke-only；不隔离 collection-changed old-items |
| DataGrid.RowHeaders | 10.243 | 10.079 | `(10.243 - 10.079) / 10.243` | 1.60% | smoke-only；不隔离 collection-changed old-items |
| DataGrid.RowDetails.Collapsed | 10.145 | 9.859 | `(10.145 - 9.859) / 10.145` | 2.82% | smoke-only；不隔离 collection-changed old-items |
| DataGrid.GroupHeaders | 9.071 | 9.192 | `(9.071 - 9.192) / 9.071` | -1.33% | smoke-only；不隔离 collection-changed old-items |
| DataGrid.RowGroups | 9.938 | 10.162 | `(9.938 - 10.162) / 9.938` | -2.25% | smoke-only；不隔离 collection-changed old-items |
| DataGrid.GalleryShape | 44.014 | 43.886 | `(44.014 - 43.886) / 44.014` | 0.29% | smoke-only；不隔离 collection-changed old-items，不作为稳定速度收益证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.937 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 9.507 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 8.689 | 2583.9 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 10.079 | 3107.1 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.859 | 3024.5 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 9.192 | 2573.1 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 10.162 | 2936.6 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 43.886 | 12409.1 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.120 DataConnection remove old-items indexed loop

本轮优化点：`DataGridDataConnection.HandleDataSourceCollectionChanged()` 在处理 DataGrid 数据源 Remove 通知时，`NotifyCollectionChangedEventArgs.OldItems` 已经是 `IList`。旧路径仍用 `foreach` 扫描 removed items，会为批量 old-items 创建枚举器。现在复用前置校验后的 `removedItems`，一次读取 `Count`，再按 index 读取 old item 并调用原来的 `_owner.RemoveRowAt(e.OldStartingIndex, item)`；分组场景仍保留原先由 group notification 处理的分支，不改变行为。

正确性边界：Remove 分支仍拒绝 null `OldItems` 和负 `OldStartingIndex`；非 grouping 路径每个 old item 仍以同一个 `OldStartingIndex` 删除，保持批量 remove 时“删除当前位置后下一项顶上来”的旧语义；Add / Replace / Reset 路径不变。状态验证使用真实 realized DataGrid 调用 private handler，批量 old-items `[row1, row2]` 后 SlotCount 从 4 变为 2，并确认 old-items list 只读 1 次 Count、2 次 indexer、不创建 enumerator。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| DataConnection Remove old-items enumerators / remove 2 rows | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；DataConnection remove handler 不再走 enumerable |
| DataConnection Remove old-items `MoveNext()` calls / remove 2 rows | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | 有效；避免遍历器扫描和结束探测 |
| DataConnection Remove old-items indexer calls / remove 2 rows | 0 calls | 2 calls | direct indexed access | n/a | 行为更直接；状态验证覆盖批量顺序 |
| DataGrid row slot count / remove 2 rows from 4-row grid | 4 -> 2 | 4 -> 2 | behavior verified | n/a | 正确性保持 |
| Grouping / Add / Reset paths | unchanged | unchanged | unchanged | n/a | 正确性保持；本轮只改非 grouping Remove old-items |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid 数据源发出 Remove old-items 通知时触发；标准 page-load smoke 不隔离该 collection-changed 路径。本轮 smoke 全部为正向，但仍只作异常检查，不作为页面加载速度收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.937 | 14.686 | `(14.937 - 14.686) / 14.937` | 1.68% | smoke-only；不隔离 DataConnection Remove old-items |
| DataGrid.Filter.Menu.Closed | 9.507 | 8.694 | `(9.507 - 8.694) / 9.507` | 8.55% | smoke-only；不隔离 DataConnection Remove old-items |
| DataGrid.Filter.Tree.Closed | 8.689 | 7.797 | `(8.689 - 7.797) / 8.689` | 10.27% | smoke-only；不隔离 DataConnection Remove old-items |
| DataGrid.RowHeaders | 10.079 | 9.170 | `(10.079 - 9.170) / 10.079` | 9.02% | smoke-only；不隔离 DataConnection Remove old-items |
| DataGrid.RowDetails.Collapsed | 9.859 | 9.493 | `(9.859 - 9.493) / 9.859` | 3.71% | smoke-only；不隔离 DataConnection Remove old-items |
| DataGrid.GroupHeaders | 9.192 | 8.398 | `(9.192 - 8.398) / 9.192` | 8.64% | smoke-only；不隔离 DataConnection Remove old-items |
| DataGrid.RowGroups | 10.162 | 9.244 | `(10.162 - 9.244) / 10.162` | 9.03% | smoke-only；不隔离 DataConnection Remove old-items |
| DataGrid.GalleryShape | 43.886 | 40.705 | `(43.886 - 40.705) / 43.886` | 7.25% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.686 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.694 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.797 | 2584.3 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.170 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.493 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.398 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.244 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 40.705 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.121 CollectionView multi-key group IList indexed loop

本轮优化点：`CollectionViewGroupRoot` 支持一个 item 同时归入多个 subgroup：当 `DataGridGroupDescription.GroupKeyFromItem()` 返回 `ICollection` 时，旧路径用 `foreach` 遍历 key list。实际 key list 常见来源是数组 / `ArrayList` / `List<object>` 这类 `IList`，可以直接用 `Count + indexer`。现在在保持原 `ICollection` fallback 的前提下，为 `IList` 增加新增 subgroup 和 remove subgroup 的索引循环；非 `IList` 的 `ICollection` 仍走原枚举路径，不扩大行为面。

正确性边界：只改变 `key is IList` 时的遍历方式，multi-key 的 key 顺序、subgroup 创建顺序、每个 subgroup 的 item count、删除后 empty view 语义都保持。状态验证用自定义 `DataGridGroupDescription` 返回仪表化 `IList` keys `["alpha", "beta"]`，确认 add grouping 和 remove grouping 都只读 1 次 Count、2 次 indexer，不创建 enumerator；分组结果仍生成 `alpha` / `beta` 两个 subgroup，删除唯一 item 后 view 为空。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Multi-key group add enumerators / 2 keys | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；add subgroup 不再枚举 `IList` keys |
| Multi-key group add `MoveNext()` calls / 2 keys | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | 有效；避免遍历器扫描和结束探测 |
| Multi-key group remove enumerators / 2 keys | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；remove subgroup 不再枚举 `IList` keys |
| Multi-key group remove `MoveNext()` calls / 2 keys | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | 有效；删除路径同样收敛 |
| Subgroup result / keys `alpha,beta` | 2 groups, 1 item each | 2 groups, 1 item each | behavior verified | n/a | 正确性保持 |
| Removal result / one multi-key item | view empty | view empty | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 GroupDescription 返回 `IList` multi-key collection 时触发；标准 page-load smoke 不隔离该 grouping path。Visual / Logical / KB 形态稳定，timing 只作异常检查，不作为本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.686 | 13.128 | `(14.686 - 13.128) / 14.686` | 10.61% | smoke-only；不隔离 multi-key grouping |
| DataGrid.Filter.Menu.Closed | 8.694 | 8.762 | `(8.694 - 8.762) / 8.694` | -0.78% | smoke-only；不隔离 multi-key grouping |
| DataGrid.Filter.Tree.Closed | 7.797 | 7.870 | `(7.797 - 7.870) / 7.797` | -0.94% | smoke-only；不隔离 multi-key grouping |
| DataGrid.RowHeaders | 9.170 | 9.327 | `(9.170 - 9.327) / 9.170` | -1.71% | smoke-only；不隔离 multi-key grouping |
| DataGrid.RowDetails.Collapsed | 9.493 | 9.361 | `(9.493 - 9.361) / 9.493` | 1.39% | smoke-only；不隔离 multi-key grouping |
| DataGrid.GroupHeaders | 8.398 | 8.463 | `(8.398 - 8.463) / 8.398` | -0.77% | smoke-only；不隔离 multi-key grouping |
| DataGrid.RowGroups | 9.244 | 9.197 | `(9.244 - 9.197) / 9.244` | 0.51% | smoke-only；不隔离 multi-key grouping |
| DataGrid.GalleryShape | 40.705 | 41.120 | `(40.705 - 41.120) / 40.705` | -1.02% | smoke-only；综合页面波动，不作为本轮速度回归证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.128 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Menu.Closed | 8.762 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.Filter.Tree.Closed | 7.870 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowHeaders | 9.327 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowDetails.Collapsed | 9.361 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GroupHeaders | 8.463 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.RowGroups | 9.197 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 只作异常检查 |
| DataGrid.GalleryShape | 41.120 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 只作异常检查 |

### 2.122 ValidationResult collection indexed lookup

本轮优化点：`ValidationUtils.FindEqualValidationResult()` 用于 validation result 去重。旧路径对 `ICollection<ValidationResult>` 一律 `foreach`，即使集合本身是 `List<ValidationResult>` / read-only list，也会创建 validation-result list enumerator。现在当 collection 同时是 `IReadOnlyList<ValidationResult>` 时，先用 `Count + indexer` 扫描；非 indexable collection 保留原 enumerable fallback。

正确性边界：ErrorMessage 和 MemberNames 的等价判断仍复用 `MemberNamesMatch()`；命中返回原集合里的旧 `ValidationResult` 实例，未命中仍返回 null；fallback enumerable 语义不变。状态验证覆盖 match at index 1 和 missing 两个路径，均只读 1 次 Count、2 次 indexer，不创建 collection enumerator。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Validation-result list enumerators / match at index 1 | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；read-only list 直接按 index 查重 |
| Validation-result list `MoveNext()` calls / match at index 1 | 2 calls | 0 calls | `(2 - 0) / 2` | 100.00% | 有效；命中路径不再走 enumerable |
| Validation-result list enumerators / missing in 2 results | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；未命中也不创建枚举器 |
| Validation-result list `MoveNext()` calls / missing in 2 results | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | 有效；避免结束探测 |
| Matching result identity | old instance | old instance | behavior verified | n/a | 正确性保持 |
| Missing result | null | null | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid validation result 去重时触发；标准 page-load smoke 不隔离 validation 查重路径。复测两轮 timing 都慢于上一轮，但 Visual / Logical / KB 形态稳定，且该路径不在标准 page-load 场景上触发；因此这里不主张页面加载速度收益，也不把这两轮 timing 当作本轮路径的性能结论：

| Scenario | baseline ms/item | optimized run 1 ms/item | optimized run 2 ms/item | run 1 improvement | run 2 improvement | conclusion |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.128 | 14.626 | 15.971 | -11.41% | -21.66% | smoke-only；不隔离 validation 查重 |
| DataGrid.Filter.Menu.Closed | 8.762 | 9.393 | 9.298 | -7.20% | -6.12% | smoke-only；不隔离 validation 查重 |
| DataGrid.Filter.Tree.Closed | 7.870 | 8.221 | 8.275 | -4.46% | -5.15% | smoke-only；不隔离 validation 查重 |
| DataGrid.RowHeaders | 9.327 | 9.635 | 13.130 | -3.30% | -40.77% | smoke-only；第二轮明显受环境噪声影响 |
| DataGrid.RowDetails.Collapsed | 9.361 | 10.047 | 10.289 | -7.33% | -9.91% | smoke-only；不隔离 validation 查重 |
| DataGrid.GroupHeaders | 8.463 | 8.867 | 8.974 | -4.77% | -6.04% | smoke-only；不隔离 validation 查重 |
| DataGrid.RowGroups | 9.197 | 9.684 | 9.673 | -5.30% | -5.18% | smoke-only；不隔离 validation 查重 |
| DataGrid.GalleryShape | 41.120 | 42.849 | 62.889 | -4.20% | -52.94% | smoke-only；第二轮明显受环境噪声影响 |

当前工作区 smoke（采用第一轮复测，第二轮仅用于判定 timing 不稳定）：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.626 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | 9.393 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | 8.221 | 2584.2 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowHeaders | 9.635 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowDetails.Collapsed | 10.047 | 3024.3 | 320.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GroupHeaders | 8.867 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowGroups | 9.684 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GalleryShape | 42.849 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 不作为本轮收益 |

### 2.123 Exception collection indexed duplicate check

本轮优化点：`ValidationUtils.AddExceptionIfNew()` 用于 DataGrid validation exception 去重。旧路径对 `ICollection<Exception>` 一律 `foreach`，即使调用方传入 `List<Exception>` / read-only list 这类可按 index 访问的集合，也会创建 exception list enumerator。现在 collection 同时是 `IReadOnlyList<Exception>` 时，使用 `Count + indexer` 扫描；非 indexable collection 保留原 enumerable fallback。

正确性边界：重复判断仍只比较 `Exception.Message`；命中相同 message 时不添加，新 message 仍追加到原 collection；fallback enumerable 语义不变。状态验证覆盖 duplicate at index 1 和 missing append 两个路径，均只读 1 次 Count、2 次 indexer，不创建 collection enumerator。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Exception list enumerators / duplicate at index 1 | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；indexable exception collection 直接按 index 去重 |
| Exception list `MoveNext()` calls / duplicate at index 1 | 2 calls | 0 calls | `(2 - 0) / 2` | 100.00% | 有效；命中路径不再走 enumerable |
| Exception list enumerators / missing in 2 exceptions | 1 enumerator | 0 enumerators | `(1 - 0) / 1` | 100.00% | 有效；未命中追加前也不创建枚举器 |
| Exception list `MoveNext()` calls / missing in 2 exceptions | 3 calls | 0 calls | `(3 - 0) / 3` | 100.00% | 有效；避免结束探测 |
| Duplicate message add count | 0 adds | 0 adds | behavior verified | n/a | 正确性保持 |
| New message append | appended new exception | appended new exception | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid validation exception 去重时触发；标准 page-load smoke 不隔离 exception 去重路径。因此这里不主张页面加载速度收益，timing 只用于确认视觉树 / 逻辑树 / KB 形态没有异常漂移：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.626 | 15.707 | `(14.626 - 15.707) / 14.626` | -7.39% | smoke-only；不隔离 validation exception 去重 |
| DataGrid.Filter.Menu.Closed | 9.393 | 9.745 | `(9.393 - 9.745) / 9.393` | -3.75% | smoke-only；不隔离 validation exception 去重 |
| DataGrid.Filter.Tree.Closed | 8.221 | 8.845 | `(8.221 - 8.845) / 8.221` | -7.59% | smoke-only；不隔离 validation exception 去重 |
| DataGrid.RowHeaders | 9.635 | 10.086 | `(9.635 - 10.086) / 9.635` | -4.68% | smoke-only；不隔离 validation exception 去重 |
| DataGrid.RowDetails.Collapsed | 10.047 | 11.120 | `(10.047 - 11.120) / 10.047` | -10.68% | smoke-only；不隔离 validation exception 去重 |
| DataGrid.GroupHeaders | 8.867 | 9.276 | `(8.867 - 9.276) / 8.867` | -4.61% | smoke-only；不隔离 validation exception 去重 |
| DataGrid.RowGroups | 9.684 | 10.253 | `(9.684 - 10.253) / 9.684` | -5.88% | smoke-only；不隔离 validation exception 去重 |
| DataGrid.GalleryShape | 42.849 | 45.644 | `(42.849 - 45.644) / 42.849` | -6.52% | smoke-only；综合页面波动，不作为本轮路径性能结论 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.707 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | 9.745 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | 8.845 | 2584.1 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowHeaders | 10.086 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowDetails.Collapsed | 11.120 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GroupHeaders | 9.276 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowGroups | 10.253 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GalleryShape | 45.644 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 不作为本轮收益 |

### 2.124 Inherited interface property lookup fix

本轮优化点：`TypeHelper.GetPropertyOrIndexer()` 的 interface fallback 原本会枚举 inherited interfaces，但循环内仍然调用 `type.GetProperty(propertyPath)`，等于反复查询原 interface；当 DataGrid 的 item type 是继承接口，属性定义在父接口上时，属性查找会失败。现在循环内改为查询当前 `typeInterface.GetProperty(propertyPath)`，避免无效重复反射并恢复 inherited interface property 查找。

正确性边界：普通 type / 直接 interface property / indexer 路径不变；只有 `type.IsInterface` 且直接 `GetProperty()` 找不到属性时进入 fallback。状态验证覆盖 `IDerivedInterfaceEditableProbe : IBaseInterfaceEditableProbe`，确认 `InterfaceName` 能返回父接口上的 `PropertyInfo`，并确认 DataGridDataConnection 对 `List<IDerivedInterfaceEditableProbe>` 的 writable interface property 不再误判为 read-only。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Inherited interface property lookup result / per property path | null | `IBaseInterfaceEditableProbe.InterfaceName` | behavior fixed | n/a | 正确性修复；父接口属性可被找到 |
| DataGrid interface item writable property state / per property path | read-only | editable | behavior fixed | n/a | 正确性修复；接口 ItemsSource 不再误禁用编辑 |
| Wrong fallback reflection target lookup / per inherited interface | 1 repeated original-interface lookup | 0 repeated original-interface lookups | `(1 - 0) / 1` | 100.00% | 有效；fallback 查当前 inherited interface |
| Useful inherited-interface lookup / per inherited interface | 0 lookups | 1 lookup | `(1 - 0) / 1` | 100.00% | 有效；interface fallback 从无效变为有效 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid item type 是继承接口且属性定义在父接口上时触发；标准 page-load smoke 不隔离该路径。因此这里不主张页面加载速度收益，timing 只用于确认视觉树 / 逻辑树 / KB 形态没有异常漂移：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 15.707 | 13.930 | `(15.707 - 13.930) / 15.707` | 11.31% | smoke-only；不隔离 inherited interface lookup |
| DataGrid.Filter.Menu.Closed | 9.745 | 9.682 | `(9.745 - 9.682) / 9.745` | 0.65% | smoke-only；不隔离 inherited interface lookup |
| DataGrid.Filter.Tree.Closed | 8.845 | 8.870 | `(8.845 - 8.870) / 8.845` | -0.28% | smoke-only；不隔离 inherited interface lookup |
| DataGrid.RowHeaders | 10.086 | 10.714 | `(10.086 - 10.714) / 10.086` | -6.23% | smoke-only；不隔离 inherited interface lookup |
| DataGrid.RowDetails.Collapsed | 11.120 | 10.739 | `(11.120 - 10.739) / 11.120` | 3.43% | smoke-only；不隔离 inherited interface lookup |
| DataGrid.GroupHeaders | 9.276 | 9.423 | `(9.276 - 9.423) / 9.276` | -1.58% | smoke-only；不隔离 inherited interface lookup |
| DataGrid.RowGroups | 10.253 | 10.091 | `(10.253 - 10.091) / 10.253` | 1.58% | smoke-only；不隔离 inherited interface lookup |
| DataGrid.GalleryShape | 45.644 | 43.683 | `(45.644 - 43.683) / 45.644` | 4.30% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.930 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | 9.682 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | 8.870 | 2583.7 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowHeaders | 10.714 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowDetails.Collapsed | 10.739 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GroupHeaders | 9.423 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowGroups | 10.091 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GalleryShape | 43.683 | 12409.4 | 1260.0 | 5.0 | smoke 通过；timing 不作为本轮收益 |

### 2.125 No-property filter skips type lookup

本轮优化点：`DataGridFilterDescription.GetValue()` 在 `PropertyPath` 为空时表示直接过滤 record 本身。旧路径仍会调用 `GetPropertyType()`，再进入 `TypeHelper.GetNestedPropertyType(null)` 并写入 `_propertyOwnerType/_propertyType` cache；这些工作对 no-property filter 没有意义。现在 `PropertyPath` 为空时直接返回 record，避免 property type 解析和 cache 写入；有 `PropertyPath` 的列过滤路径不变。

正确性边界：无 `PropertyPath` 的默认字符串过滤仍对 record 本身做 `ToString()`；custom filter 仍收到 record 本身和第一个 filter condition；有 `PropertyPath` 的属性过滤、property type cache、path 变更 invalidation 保持原验证覆盖。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Property type lookup / no-property `FilterBy()` | 1 lookup | 0 lookups | `(1 - 0) / 1` | 100.00% | 有效；直接过滤 record 本身 |
| Type cache writes / no-property `FilterBy()` | 2 fields | 0 fields | `(2 - 0) / 2` | 100.00% | 有效；`_propertyOwnerType/_propertyType` 保持 null |
| Default no-property filter result | match record string | match record string | behavior verified | n/a | 正确性保持 |
| Custom no-property filter input | record + first condition | record + first condition | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 `DataGridFilterDescription.PropertyPath` 为空的过滤描述上触发；标准 page-load smoke 不隔离该路径。Tree / RowHeaders timing 本轮波动较大，因此这里不主张页面加载速度收益：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.930 | 14.343 | `(13.930 - 14.343) / 13.930` | -2.96% | smoke-only；不隔离 no-property filter |
| DataGrid.Filter.Menu.Closed | 9.682 | 8.836 | `(9.682 - 8.836) / 9.682` | 8.74% | smoke-only；不隔离 no-property filter |
| DataGrid.Filter.Tree.Closed | 8.870 | 14.332 | `(8.870 - 14.332) / 8.870` | -61.58% | smoke-only；本轮明显环境波动 |
| DataGrid.RowHeaders | 10.714 | 13.525 | `(10.714 - 13.525) / 10.714` | -26.24% | smoke-only；本轮明显环境波动 |
| DataGrid.RowDetails.Collapsed | 10.739 | 9.360 | `(10.739 - 9.360) / 10.739` | 12.84% | smoke-only；不隔离 no-property filter |
| DataGrid.GroupHeaders | 9.423 | 8.449 | `(9.423 - 8.449) / 9.423` | 10.34% | smoke-only；不隔离 no-property filter |
| DataGrid.RowGroups | 10.091 | 9.156 | `(10.091 - 9.156) / 10.091` | 9.27% | smoke-only；不隔离 no-property filter |
| DataGrid.GalleryShape | 43.683 | 40.603 | `(43.683 - 40.603) / 43.683` | 7.05% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.343 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | 8.836 | 2586.5 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | 14.332 | 2584.5 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowHeaders | 13.525 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowDetails.Collapsed | 9.360 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GroupHeaders | 8.449 | 2573.9 | 266.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowGroups | 9.156 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GalleryShape | 40.603 | 12409.5 | 1260.0 | 5.0 | smoke 通过；timing 不作为本轮收益 |

### 2.126 Read-only multi-key grouping indexed keys

本轮优化点：`CollectionViewGroupRoot` 的 multi-key grouping 原先只对非泛型 `IList` key 集合走 Count/indexer；如果 `GroupDescription.GroupKeyFromItem()` 返回 `IReadOnlyList<object>`，旧逻辑不会把它展开成多个 subgroup。现在新增 `IReadOnlyList<object>` 分支，新增和删除 subgroup 都通过 Count/indexer 读取 key，避免把 read-only key list 当成单个 key 对象。

正确性边界：既有 `IList` 和 `ICollection` 语义不变；`IReadOnlyList<object>` key 集合现在会按每个 key 创建 subgroup，删除同一 item 时也会逐 key 删除，删除唯一 item 后 view 为空。状态验证覆盖 2 个 read-only key 的新增 / 删除，确认 `Count=1`、`Indexer=2`、`GetEnumerator()` / `MoveNext()` / `Dispose()` 均为 0。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Read-only key list subgroup expansion / add | 1 list object key | 2 key subgroups | behavior fixed | n/a | 正确性修复；read-only multi-key 现在真正展开 |
| Read-only key list subgroup expansion / remove | not expanded | 2 key removals | behavior fixed | n/a | 正确性修复；删除路径与新增路径对称 |
| Read-only key list indexed reads / add 2 keys | 0 useful key reads | 1 Count + 2 indexer reads | behavior fixed | n/a | 行为更直接；按 key 项读取 |
| Read-only key list enumerators / add 2 keys | n/a (not expanded) | 0 enumerators | verified no enumeration | n/a | 展开时不创建 key-list enumerator |
| Read-only key list enumerators / remove 2 keys | n/a (not expanded) | 0 enumerators | verified no enumeration | n/a | 删除时不创建 key-list enumerator |

Smoke-only 对比上一轮同参数复测。本轮路径只在 grouping key 返回 `IReadOnlyList<object>` 时触发；标准 page-load smoke 不隔离该路径，因此不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.343 | 13.886 | `(14.343 - 13.886) / 14.343` | 3.19% | smoke-only；不隔离 read-only multi-key grouping |
| DataGrid.Filter.Menu.Closed | 8.836 | 8.998 | `(8.836 - 8.998) / 8.836` | -1.83% | smoke-only；不隔离 read-only multi-key grouping |
| DataGrid.Filter.Tree.Closed | 14.332 | 10.217 | `(14.332 - 10.217) / 14.332` | 28.71% | smoke-only；上一轮 Tree timing 偏慢，不能作为本轮证明 |
| DataGrid.RowHeaders | 13.525 | 9.374 | `(13.525 - 9.374) / 13.525` | 30.69% | smoke-only；上一轮 RowHeaders timing 偏慢，不能作为本轮证明 |
| DataGrid.RowDetails.Collapsed | 9.360 | 9.856 | `(9.360 - 9.856) / 9.360` | -5.30% | smoke-only；不隔离 read-only multi-key grouping |
| DataGrid.GroupHeaders | 8.449 | 8.591 | `(8.449 - 8.591) / 8.449` | -1.68% | smoke-only；不隔离 read-only multi-key grouping |
| DataGrid.RowGroups | 9.156 | 10.083 | `(9.156 - 10.083) / 9.156` | -10.12% | smoke-only；不隔离 read-only multi-key grouping |
| DataGrid.GalleryShape | 40.603 | 41.792 | `(40.603 - 41.792) / 40.603` | -2.93% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.886 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | 8.998 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | 10.217 | 2584.1 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowHeaders | 9.374 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowDetails.Collapsed | 9.856 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GroupHeaders | 8.591 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowGroups | 10.083 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GalleryShape | 41.792 | 12409.4 | 1260.0 | 5.0 | smoke 通过；timing 不作为本轮收益 |

### 2.127 Indexer parsing avoids trimmed string allocation

本轮优化点：`TypeHelper.GetPropertyOrIndexer()` 在解析 int indexer path 时，旧实现会对 `"[ 1 ]"` 这类带空白的 indexer 片段调用 `stringIndex.Trim()` 后再 `Int32.TryParse()`；当原字符串确实需要裁剪空白时会产生一个临时 trimmed string。现在改为 `stringIndex.AsSpan().Trim()`，直接用 span 解析，避免临时字符串。该 helper 被 DataGrid property path / nested indexer 路径复用。

正确性边界：int indexer 的紧凑形式 `"[2]"` 和带空白形式 `"[ 1 ]"` 都仍解析为整数 index；string indexer 不参与 int trim，仍保留原始 key 文本，例如 `"[ key ]"` 仍传入 `" key "`；默认成员不是 indexer 的类型会被跳过，不再访问空 `GetIndexParameters()` 结果。状态验证覆盖 `List<string>` int indexer、`Dictionary<string, int>` string indexer，以及 `[DefaultMember]` 指向普通属性的非 indexer 类型。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Temporary trimmed strings / padded int indexer parse | 1 string | 0 strings | `(1 - 0) / 1` | 100.00% | 有效；带空白 int indexer 解析不再分配 trimmed string |
| Padded int indexer result / `"[ 1 ]"` | index `1` | index `1` | behavior verified | n/a | 正确性保持 |
| Compact int indexer result / `"[2]"` | index `2` | index `2` | behavior verified | n/a | 正确性保持 |
| String indexer key text / `"[ key ]"` | `" key "` | `" key "` | behavior verified | n/a | 正确性保持 |
| Non-indexer default member / `"[0]"` | potential `IndexOutOfRangeException` | `null` property | behavior fixed | n/a | 正确性修复；非 indexer 默认成员被跳过 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 property path 包含 indexer、且 int indexer 文本带空白时触发；标准 page-load smoke 不隔离该 helper，因此不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.886 | 14.403 | `(13.886 - 14.403) / 13.886` | -3.72% | smoke-only；不隔离 indexer parser |
| DataGrid.Filter.Menu.Closed | 8.998 | 9.824 | `(8.998 - 9.824) / 8.998` | -9.18% | smoke-only；不隔离 indexer parser |
| DataGrid.Filter.Tree.Closed | 10.217 | 7.920 | `(10.217 - 7.920) / 10.217` | 22.48% | smoke-only；本机波动，不作为本轮证明 |
| DataGrid.RowHeaders | 9.374 | 9.337 | `(9.374 - 9.337) / 9.374` | 0.39% | smoke-only；不隔离 indexer parser |
| DataGrid.RowDetails.Collapsed | 9.856 | 9.987 | `(9.856 - 9.987) / 9.856` | -1.33% | smoke-only；不隔离 indexer parser |
| DataGrid.GroupHeaders | 8.591 | 8.783 | `(8.591 - 8.783) / 8.591` | -2.23% | smoke-only；不隔离 indexer parser |
| DataGrid.RowGroups | 10.083 | 9.985 | `(10.083 - 9.985) / 10.083` | 0.97% | smoke-only；不隔离 indexer parser |
| DataGrid.GalleryShape | 41.792 | 42.771 | `(41.792 - 42.771) / 41.792` | -2.34% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.403 | 2948.3 | 305.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | 9.824 | 2583.5 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | 7.920 | 2582.8 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowHeaders | 9.337 | 3106.7 | 336.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowDetails.Collapsed | 9.987 | 3024.0 | 320.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GroupHeaders | 8.783 | 2572.9 | 266.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowGroups | 9.985 | 2936.1 | 315.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GalleryShape | 42.771 | 12407.1 | 1260.0 | 5.0 | smoke 通过；timing 不作为本轮收益 |

### 2.128 Default member lookup skips no-attribute array

本轮优化点：`TypeHelper.PrependDefaultMemberName()` 只在 property path 以 indexer 开头时读取 item type 的 `DefaultMemberAttribute`。旧实现每次都会调用 `GetCustomAttributes(typeof(DefaultMemberAttribute), true)`；普通数据类型没有该 attribute 时也会创建一个空 attribute array。现在 `GetDefaultMemberName()` 先用 `IsDefined()` 判断，无 attribute 的常见路径直接返回 null，避免空数组创建；带默认成员的路径仍保留原 `GetCustomAttributes()` 读取语义。

正确性边界：无默认成员的 item 仍不改写 `"[0]"`；带 `[DefaultMember(nameof(Name))]` 的 item 仍把 `"[0]"` 改写为 `"Name[0]"`；非 indexer property path 和 null item 都不改写。状态验证覆盖这四种路径。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Empty attribute arrays / no-default-member indexer path | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 有效；普通类型不再创建空 `DefaultMemberAttribute` array |
| Default member attribute reads / type with `[DefaultMember]` | 1 read | 1 read | behavior preserved | n/a | 正确性保持；有 attribute 时仍读取 |
| No-default-member result / `"[0]"` | `"[0]"` | `"[0]"` | behavior verified | n/a | 正确性保持 |
| Default-member result / `"[0]"` | `"Name[0]"` | `"Name[0]"` | behavior verified | n/a | 正确性保持 |
| Null item result / `"[0]"` | `"[0]"` | `"[0]"` | behavior verified | n/a | 正确性保持 |

Smoke-only 对比上一轮同参数复测。本轮路径只在 DataGrid property path 以 indexer 开头、且 item type 没有 `DefaultMemberAttribute` 时触发；标准 page-load smoke 不隔离该 helper，因此不把单次 timing 当成本轮页面加载速度收益或回归证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.403 | 13.783 | `(14.403 - 13.783) / 14.403` | 4.30% | smoke-only；不隔离 default-member lookup |
| DataGrid.Filter.Menu.Closed | 9.824 | 8.828 | `(9.824 - 8.828) / 9.824` | 10.14% | smoke-only；不隔离 default-member lookup |
| DataGrid.Filter.Tree.Closed | 7.920 | 7.812 | `(7.920 - 7.812) / 7.920` | 1.36% | smoke-only；不隔离 default-member lookup |
| DataGrid.RowHeaders | 9.337 | 9.492 | `(9.337 - 9.492) / 9.337` | -1.66% | smoke-only；不隔离 default-member lookup |
| DataGrid.RowDetails.Collapsed | 9.987 | 10.397 | `(9.987 - 10.397) / 9.987` | -4.11% | smoke-only；不隔离 default-member lookup |
| DataGrid.GroupHeaders | 8.783 | 9.200 | `(8.783 - 9.200) / 8.783` | -4.75% | smoke-only；不隔离 default-member lookup |
| DataGrid.RowGroups | 9.985 | 9.508 | `(9.985 - 9.508) / 9.985` | 4.78% | smoke-only；不隔离 default-member lookup |
| DataGrid.GalleryShape | 42.771 | 41.560 | `(42.771 - 41.560) / 42.771` | 2.83% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 13.783 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | 8.828 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | 7.812 | 2584.4 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowHeaders | 9.492 | 3107.2 | 336.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowDetails.Collapsed | 10.397 | 3024.6 | 320.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GroupHeaders | 9.200 | 2573.2 | 266.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowGroups | 9.508 | 2936.7 | 315.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GalleryShape | 41.560 | 12409.3 | 1260.0 | 5.0 | smoke 通过；timing 不作为本轮收益 |

### 2.129 Column group traversal uses indexed access

本轮优化点：列分组路径里剩余的 `foreach` 主要集中在 `ColumnGroupsInternal` / `GroupChildren` / `NotifyCollectionChangedEventArgs.OldItems/NewItems`。这些集合在列组增删、组头 view 构建、组头 measure / arrange 时会被反复遍历。现在这批路径统一改为 Count/indexer：列组 collection changed 的 Remove / Add 各自只扫一遍 snapshot；`BuildColumnGroupView()`、`BuildGroupViewItemRecursive()`、`AddColumnsFromGroupTree()`、`RemoveColumnsFromGroupTree()`、`DataGridGroupColumnHeadersPresenter` 的 top-level / recursive measure / arrange 都不再创建集合枚举器。

正确性边界：列组 leaf column 的深度优先顺序保持；top-level / nested group 的 `OwningGrid` 释放保持；`GroupParent` 链保持；真实 realized nested group header 的 top-level、nested、leaf header view item 都能创建、测量并按 leaf 顺序排列。另修复 `DataGridDataConnection.IndexOf()` 的 null 语义：旧 read-only list / bare enumerable fallback 用 `dataItemTmp == null || dataItem.Equals(dataItemTmp)`，会把列表里的 null 错认为任意非 null 目标；现在统一用 `Equals(dataItemTmp, dataItem)`，并且 null 查询也能返回真实 null item index。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Column group collection changed passes / Remove | 2 passes | 1 pass | `(2 - 1) / 2` | 50.00% | 有效；移除列和解除 GroupChanged 订阅合并到同一次 old-items indexed scan |
| Column group collection changed passes / Add | 2 passes | 1 pass | `(2 - 1) / 2` | 50.00% | 有效；添加列和注册 GroupChanged 合并到同一次 new-items indexed scan |
| CollectionChanged item enumerators / top-level group add/remove | 4 enumerators | 0 enumerators | `(4 - 0) / 4` | 100.00% | 有效；OldItems/NewItems snapshot 改 Count/indexer |
| GroupChildren traversal enumerators / build + add/remove recursion | 4 callsites | 0 callsites | `(4 - 0) / 4` | 100.00% | 有效；构建 header view item 和收集/移除 leaf columns 不再 foreach |
| Group header layout traversal enumerators / measure + arrange | 4 callsites | 0 callsites | `(4 - 0) / 4` | 100.00% | 有效；top-level 和 nested group layout 改 Count/indexer |
| IndexOf non-null search through null item | returns null index | returns target index | behavior fixed | n/a | 正确性修复；null entry 不再匹配任意目标 |
| IndexOf null search / read-only list | `-1` | null item index | behavior fixed | n/a | 正确性修复；null 数据项可被定位 |

Smoke-only 对比上一轮同参数复测。本轮主要命中列组增删 / group header layout 和 `IndexOf()` null 边界；标准 page-load smoke 不隔离这些路径，因此不把单次 timing 当成本轮速度证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 13.783 | 14.423 | `(13.783 - 14.423) / 13.783` | -4.64% | smoke-only；不隔离 column group traversal |
| DataGrid.Filter.Menu.Closed | 8.828 | 8.734 | `(8.828 - 8.734) / 8.828` | 1.06% | smoke-only；不隔离 column group traversal |
| DataGrid.Filter.Tree.Closed | 7.812 | 8.558 | `(7.812 - 8.558) / 7.812` | -9.55% | smoke-only；不隔离 column group traversal |
| DataGrid.RowHeaders | 9.492 | 9.685 | `(9.492 - 9.685) / 9.492` | -2.03% | smoke-only；不隔离 column group traversal |
| DataGrid.RowDetails.Collapsed | 10.397 | 9.875 | `(10.397 - 9.875) / 10.397` | 5.02% | smoke-only；不隔离 column group traversal |
| DataGrid.GroupHeaders | 9.200 | 8.532 | `(9.200 - 8.532) / 9.200` | 7.26% | smoke-only；最接近本轮 group header layout 路径，但仍是整页烟测 |
| DataGrid.RowGroups | 9.508 | 10.046 | `(9.508 - 10.046) / 9.508` | -5.66% | smoke-only；不隔离 column group traversal |
| DataGrid.GalleryShape | 41.560 | 40.925 | `(41.560 - 40.925) / 41.560` | 1.53% | smoke-only；综合页面波动，不作为本轮速度证明 |

当前工作区 smoke：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 14.423 | 2948.2 | 305.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Menu.Closed | 8.734 | 2586.2 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.Filter.Tree.Closed | 8.558 | 2584.3 | 267.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowHeaders | 9.685 | 3107.1 | 336.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowDetails.Collapsed | 9.875 | 3024.5 | 320.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GroupHeaders | 8.532 | 2571.9 | 266.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.RowGroups | 10.046 | 2936.6 | 315.0 | 1.0 | smoke 通过；timing 不作为本轮收益 |
| DataGrid.GalleryShape | 40.925 | 12409.1 | 1260.0 | 5.0 | smoke 通过；timing 不作为本轮收益 |

### 2.130 T4.1-T4.5 remaining DataGrid items closed

本轮收尾点：T4.1-T4.5 最后一批候选项经源码核对后不保留无收益改动。`DataGrid` 里剩余 `_rowsPresenter.Children` / `DataGridDetailsPresenter.Children` 遍历基于 `Avalonia.Controls.Controls : AvaloniaList<Control>`；Avalonia 12 `AvaloniaList<T>.GetEnumerator()` 返回 struct enumerator，因此 `foreach` 本身不是 heap iterator 热点。把这些路径改成 Count/indexer 没有分配收益，且会让 page-load smoke 解释变得不清晰，所以已撤回。

审计关闭项：`DataGridColumnCollection.GetDisplayedColumns()` / `GetVisibleColumns()` 系列当前没有生产调用点；`DataGridDisplayData.GetScrollingElements()` / `GetScrollingRows()` 仅剩 DEBUG 打印路径；`GetSelectionInclusive()` 的外层 yield 当前也没有生产调用点，内部 selected-slot traversal 已经是 struct enumerable。因此不继续为这些 helper 引入新抽象，T4.1-T4.5 按“无当前生产热点”关闭。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | --- | --- | --- | ---: | --- |
| Rows/details `Children` heap iterator allocations / traversal | 0 known allocations | 0 allocations | source verified | 0.00% | 审计关闭；AvaloniaList concrete foreach 使用 struct enumerator，不是分配热点 |
| Remaining `DataGridColumnCollection` displayed/visible helper production callsites | 0 callsites | 0 callsites | callsite audit | n/a | 审计关闭；helper 保留兼容路径，不做无调用点重构 |
| Remaining `DataGridDisplayData` scrolling helper production callsites | 0 callsites | 0 callsites | callsite audit | n/a | 审计关闭；仅 DEBUG 打印路径保留 |
| `GetSelectionInclusive()` production callsites | 0 callsites | 0 callsites | callsite audit | n/a | 审计关闭；内部 slot traversal 已是 struct enumerable，不引入更复杂外层 enumerable |
| T4.1-T4.5 active remaining roadmap buckets | 5 partial buckets | 0 partial buckets | `(5 - 0) / 5` | 100.00% | 规划关闭；未发现新的生产热点，非生产 yield helpers 保留兼容路径 |

Smoke-only 对比上一轮同参数复测。本轮没有保留会改变 `DataGrid.Basic` 首屏结构的代码改动；下表只记录发现异常的烟测结果，不作为收益证明：

| Scenario | baseline ms/item | optimized ms/item | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid.Basic | 14.423 | 16.776 | `(14.423 - 16.776) / 14.423` | -16.31% | smoke-only outlier；复测为 15.176ms，且同轮 GalleryShape 出现更大环境抖动 |
| DataGrid.Filter.Menu.Closed | 8.734 | 9.409 | `(8.734 - 9.409) / 8.734` | -7.73% | smoke-only；不隔离本轮 traversal 路径 |
| DataGrid.Filter.Tree.Closed | 8.558 | 8.051 | `(8.558 - 8.051) / 8.558` | 5.92% | smoke-only；不隔离本轮 traversal 路径 |
| DataGrid.RowHeaders | 9.685 | 9.330 | `(9.685 - 9.330) / 9.685` | 3.67% | smoke-only；整页烟测 |
| DataGrid.RowDetails.Collapsed | 9.875 | 9.926 | `(9.875 - 9.926) / 9.875` | -0.52% | smoke-only；整页烟测 |
| DataGrid.GroupHeaders | 8.532 | 9.509 | `(8.532 - 9.509) / 8.532` | -11.45% | smoke-only；不隔离本轮 traversal 路径 |
| DataGrid.RowGroups | 10.046 | 9.364 | `(10.046 - 9.364) / 10.046` | 6.79% | smoke-only；整页烟测 |
| DataGrid.GalleryShape | 40.925 | 42.700 | `(40.925 - 42.700) / 40.925` | -4.34% | smoke-only；综合页面波动，不作为本轮速度证明 |

异常后同参数复测：

| Scenario | ms/item | KB/item | Visual/root | Logical/root | 结论 |
| --- | ---: | ---: | ---: | ---: | --- |
| DataGrid.Basic | 15.176 | 2948.2 | 305.0 | 1.0 | smoke 仍偏慢但低于 outlier；timing 不作为本轮收益 |
| DataGrid.GalleryShape | 82.413 | 12409.4 | 1260.0 | 5.0 | 同轮异常翻倍，说明机器/进程环境抖动明显 |

### 2.131 Special column remove detection indexed loop

本轮优化点：`DataGridSelectionColumn`、`DataGridRowReorderColumn`、`DataGridCheckBoxColumn`、`DataGridOperationColumn`、`DataGridDetailExpanderColumn` 在监听 `Columns.CollectionChanged` 时，旧路径通过 `e.OldItems.Contains(this)` 判断自身是否被移除。本轮抽到 `DataGridColumn.RemovedItemsContain()`，只在 Remove action 下读取 `OldItems.Count` 并按 index 扫描，5 个 special column 复用同一实现。

正确性边界：仍只处理 `NotifyCollectionChangedAction.Remove`；匹配语义保持 `Equals(oldItems[i], item)`；命中后仍走原来的 `ReleaseOwningGrid()`，未命中仍保持订阅。`--verify-datagrid-states` 覆盖 special columns 在 `Columns.Clear()` 后释放 cached owning grid，以及 reorder duplicate check 等相关行为。

| metric | baseline | optimized | formula | improvement | conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Special-column removed-self detection `IList.Contains` callsites | 5 callsites | 0 callsites | `(5 - 0) / 5` | 100.00% | 有效；5 个 special column remove handler 改走共享 indexed scan |
| OldItems action/null checks per special column handler | 2 nested checks | 1 shared helper check | structural | 更集中 | 结构收益；Remove/null/empty 判断统一在 helper 内 |
| Special-column release behavior | existing release path | same release path | behavior preserved | n/a | 正确性保持；状态验证覆盖 `Columns.Clear()` 释放 |

说明：这是动态列移除路径的结构性收益；不声明页面导航 timing 提升。

---

### 2.132 RowGroupHeader frozen-state dictionaries capacity

`DataGridRowGroupHeader` 在 frozen row group header 布局中会为 child `RenderTransform` 和 clip geometry 建字典。可缓存的 child 数量不超过模板 root children 数，本轮在首次创建字典时用 `_rootElement.Children.Count` 预分配；transform / clip 复用、清理和 frozen 语义不变。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| RowGroupHeader frozen transform dictionary growth / first frozen arrange | dynamic growth | root child count capacity | structural | 结构收益 | transform cache 按模板子节点数预分配 |
| RowGroupHeader frozen clip dictionary growth / first frozen arrange | dynamic growth | root child count capacity | structural | 结构收益 | clip geometry cache 按模板子节点数预分配 |
| Frozen row group visual semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |

说明：这是 DataGrid row group frozen layout 路径 structural-only 收益；没有新增页面 timing 对比，不声明页面加载速度提升。

---

### 2.133 Pagination visibility flag check

`DataGridPaginationVisibilityConvertor` 在 top/bottom pagination 可见性转换时只需要判断 flag 是否包含 `Top` 或 `Bottom`。本轮把 enum `HasFlag()` 改为直接 bitwise check，返回语义不变，避免 converter 运行时重复走 enum helper。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid pagination visibility enum `HasFlag` calls / convert | 1 | 0 | `(1 - 0) / 1` | 100.00% | structural-only；top/bottom 判断改为 bitwise check |
| Pagination visibility semantics | unchanged | unchanged | n/a | 0.00% | `None` / `Top` / `Bottom` 语义保持 |

说明：这是 DataGrid pagination converter 路径 structural-only 收益；没有新增页面 timing 对比，不声明页面加载速度提升。

---

### 2.134 ListCollectionView materialization cleanup

`ListCollectionView` 是 DataGrid / ListView 等数据视图共享层。本轮只收敛集合 materialization：源集合复制和无过滤 local-array refresh 按已知 `Count` 预分配；分页枚举按当前页数量预分配，`PageIndex < 0` 的空页直接复用共享空数组枚举器；排序结果 materialization 按输入数量预分配；`MergedComparer` 的 comparer array 改为 Count/indexer 填充，不再创建 `Select().ToArray()` LINQ 链。排序、过滤、分组、分页语义不变。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| Source copy list capacity / unfiltered collection view refresh | dynamic growth | source Count capacity | structural | 分配更紧 | 复制源集合时按已知 Count 预分配 |
| Local-array list capacity / unfiltered sort/group refresh | dynamic growth | source Count capacity | structural | 分配更紧 | 无 Filter 时按源 Count 预分配 |
| Paged enumerator list capacity / page enumeration | dynamic growth | page item count capacity | structural | 分配更紧 | 当前页 list 按实际页项数预分配 |
| Empty async page temporary list / `PageIndex < 0` enumeration | 1 list | 0 lists | `(1 - 0) / 1` | 100.00% | 空页复用 `Array.Empty<object?>()` 枚举器 |
| Sorted result list capacity / sorted refresh | dynamic growth | input Count capacity | structural | 分配更紧 | 排序结果按输入 list Count 预分配 |
| MergedComparer LINQ operators / sorted insert comparer build | 2 operators | 0 operators | `(2 - 0) / 2` | 100.00% | `Select().ToArray()` 改为 Count/indexer array fill |

说明：这是 CollectionView data-layer structural-only 收益；没有新增页面 timing 对比，不声明页面加载速度提升。

---

## 3. 验证

包级收口：路线图 T4.1-T4.5 已全部完成，`DataGridRow` / `DataGridCell` / `DataGridColumnHeader` 等内部子控件不再作为独立 Pending 项排队；它们分别由 core、columns、details、reorder-selection、collection-view-filter 五个 T4 子模块覆盖。

```bash
dotnet build -c Release -f net10.0 tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
```

结果：构建通过，0 warning。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-datagrid-states
```

本轮新增状态验证：DataConnection `IReadOnlyList<object>` `GetDataItem(index)` 直接读取 Count/indexer 且不调用 `GetEnumerator()`，范围内取项和越界 null 语义保持；DataConnection `IReadOnlyList<object>` `IndexOf(item)` 直接读取 Count/indexer 且不调用 `GetEnumerator()`，命中、未命中和 null 查询语义保持；DataConnection `IReadOnlyCollection<object>` Count/Any 直接读取 Count 且不调用 `GetEnumerator()`；DataConnection 裸 `IEnumerable` item-type probe 仍只 `MoveNext()` 一次、仍推断出代表项类型，并会 dispose 探测枚举器；DataConnection `IList` item-type probe 直接读取 Count/indexer 且不调用 `GetEnumerator()`，返回代表项真实类型；DataConnection `IReadOnlyList<object>` item-type probe 直接读取 Count/indexer 且不调用 `GetEnumerator()`，返回代表项真实类型；DataConnection `DataProperties` 同 DataSource/DataType 重复读取复用同一个 `PropertyInfo[]`，`ClearDataProperties()` 后刷新出新数组，DataSource 替换后刷新且属性数量保持；filter flyout 默认 `FilterOnClose=false` 被动关闭不再通知/收集 selected values，`FilterOnClose=true` 被动关闭仍通知空选择，OK 确认关闭仍通知且 `IsConfirmed=true`；`DataGridFilterDescription` 默认构造不分配 conditions backing list、空条件 `FilterBy()` 不物化 list、公开 getter 和 object initializer 语义保持；clipboard row content args 默认路径保持 `Count=0 / Capacity=0`，header args 保持 `IsColumnHeadersRow=true` / `Item=null`，row args 保持 item metadata；指定 8 visible columns 时 `ClipboardRowContent` 首次创建即 `Capacity=8`，连续 add 8 个 cell content 后 capacity 仍为 8；clipboard text formatting 覆盖 quoted cell、embedded quote escaping、tab 分隔、null content、CRLF row terminator 和 empty row no-op；分页 `CreatePagedEnumerator()` 返回直接 `PageEnumerator`，第 2 页 / 末页顺序保持，`Reset()` 后重枚举顺序保持，public `GetEnumerator()` 与 direct page enumerator 结果一致；空 `List<string>` / 空 `List<object>` filter values copy 复用同一个 `Count=0 / Capacity=0` 的 shared object list，非空 filter values 仍创建独立快照且不受原列表后续 mutation 影响；`InitializeElements(false)` 空 selection reset 不替换 selected-items cache，且 `SelectedItems.Count == 0` / `SelectedIndex == -1` / `SelectedItem == null` 保持；SelectAll 后清空 selection 的 cache 会在 `UpdateIndexes()` 后释放 backing capacity；非空 Extended selection 下 SelectAll 后 reset，5 个 selected items 顺序保持；RowGroupHeadersTable 的 `EnumerateIndexes()` / `EnumerateIndexes(start)` 返回 value-type enumerable，并与旧 `GetIndexes()` / `GetIndexes(start)` 的 full/start slot 序列一致；`GetAllRows()` 返回 value-type enumerable，basic/grid grouped grid 都和视觉树已实现 `DataGridRow` 数量与顺序一致，并继续跳过 row group header 等非 row child；DataConnection editable/read-only check 覆盖普通可写属性、`Editable(false)`、`Editable(true)`、property-level `ReadOnly(true)`、property-level `ReadOnly(false)`、type-level `ReadOnly(true)`、getter-only 属性、nested writable path 和 nested `Editable(false)` path；DisplayAttribute lookup 覆盖无 attribute 显示名解析、`ShortName`、`Name` fallback、自动生成列 `Order`、`AutoGenerateField=false` 隐藏列和 generated column 标记，本轮复测确认 order-list 预分配后这些语义仍保持；star column width adjustment 覆盖 4 个 star 列全量增宽后全部 `100 -> 120`，以及从 displayIndex 2 增宽后前两列保持 120、后两列 `120 -> 140`；plain column headers template apply 在 4 列乱序 `DisplayIndex` 下仍按显示顺序插入 public column headers；column group tree add 按深度优先顺序收集 leaf columns，top-level/nested group item 都持有当前 grid；remove 后不残留 leaf columns，top-level/nested group item 都释放 `OwningGrid`；`DataGridFilterItem.Children` public getter 仍返回可变空列表，collection initializer 仍能添加子项，leaf item 构造 / `HasChildren` / menu-tree materialize 都不会创建空 children backing list；menu/tree filter presenter selected-values list 空选保持 `Count=0 / Capacity=0` 且重复空选 / reset 空选复用共享空列表，单个嵌套 leaf checked 时 `Count=1 / Capacity=1`，未 realized `ResetFilter()` 会把 nested checked leaf 清空，6 个嵌套 leaf checked 时 `Count=6 / Capacity=6`；SelectAll / ClearSelection 的 `SelectionChanged` added / removed item list 按 delta count 预分配，事件 added/removed 计数保持；SelectAll / ClearSelection 后 `SelectedItems` public enumeration 数量保持，旧 selected-slots 快照 range list 按 range count 预分配；selected-items index rebuild 后 5 个选中项仍保持，内部 selected-items cache `Count=5 / Capacity=5`；Single selection 从第 1 行切到第 3 行后仍只选中新行；`GetSelectionInclusive(1,3)` 返回第 2 到第 4 行且 `GetSlots(start)` 返回 struct index enumerable；CollectionView 无过滤 source copy 的 item 数量 / 容量保持，`IReadOnlyCollection<object>` source copy 按 Count 预分配且 Reset 空检查不创建额外 enumerator，`IReadOnlyList<object>` source copy / Refresh / unfiltered local-array refresh 通过 Count + indexer 复制且保持 0..8 顺序，filtered read-only list refresh 通过 Count + indexer 扫描且只保留 0、1，`IList` source copy / Refresh / unfiltered local-array refresh 通过 Count + indexer 复制且保持 0..8 顺序，filtered `IList` refresh 通过 Count + indexer 扫描且只保留 0、1，filtered result 容量不按 9 项全量源预分配，edit currency adjustment 只查找一次 edited current item，通知型 source 构造期不创建 polling tracking enumerator，非通知型 source Dispose 会释放 tracking enumerator，设置 Filter 后 filtered refresh 仍只保留匹配项且不过度预分配；CollectionView 处理 `ICollection` source Reset-to-empty 后 view 仍清空，且 Reset 空检查不额外枚举；SortDescription comparer/path key selector delegate 复用，且 comparer sort、path sort 升序和 switched path sort 降序语义保持；CollectionView 空分页状态复用共享空数组，且 `PageIndex < 0` 时 public enumerator 不产生 item。

本轮补充状态验证：CollectionView representative item lookup 在普通非分组 / 非分页 view 上返回首个非空 item，且不创建内部列表枚举器；分组、分页和 `AddNew` 事务保留原枚举器路径。

本轮补充状态验证：CollectionView singleton sorted refresh 保留 sort description 初始化但跳过 `OrderBy` / `ThenBy` 链和额外 sorted list materialization；2 项数据仍调用 `OrderBy` 并保持排序结果。

本轮补充状态验证：CollectionView empty sorted insert 保留 sort description 初始化但跳过 `MergedComparer` 构造；non-empty sorted insert 仍构造 comparer 并保持排序插入结果。

本轮补充状态验证：PathSortDescription 非 string 默认 comparer 按 Type 缓存，重复 `int` lookup 只保留 1 个 cache entry 并返回同一 comparer，path sort 语义保持。

本轮补充状态验证：TypeHelper simple property path split fast path 返回容量为 1 的单 segment list，复杂 nested/indexer path 和 empty path 语义保持。

本轮补充状态验证：TypeHelper GetPropertyOrIndexer 能解析 inherited interface property；DataGridDataConnection 对接口 ItemsSource 的父接口 writable property 不再误判为 read-only。

本轮补充状态验证：TypeHelper GetPropertyOrIndexer 的 int indexer 解析保持 `"[ 1 ]"` / `"[2]"` 语义；string indexer 仍保留原始 key 文本；`[DefaultMember]` 指向普通属性时会跳过非 indexer member。

本轮补充状态验证：TypeHelper PrependDefaultMemberName 的无默认成员 item、带默认成员 item、非 indexer path 和 null item 语义保持。

本轮补充状态验证：FilterDescription 无 `PropertyPath` 时直接过滤 record 本身，不再解析 / 缓存 property type；默认字符串过滤和 custom filter 输入语义保持。

本轮补充状态验证：ValidationUtils ContainsMemberName 对 read-only member names 走 Count/indexer，不再创建 enumerator；empty read-only member names 只读 Count，fallback enumerable 仍枚举并 dispose。

本轮补充状态验证：CollectionView Remove/Replace old-items 处理走 Count/indexer，不再创建 old-items enumerator；批量 remove `[1, 2]` 后 view 剩余 `[0, 3]`。

本轮补充状态验证：DataConnection Remove old-items 处理走 Count/indexer，不再创建 old-items enumerator；真实 realized 4 行 DataGrid 批量 remove 2 行后 SlotCount 变为 2。

本轮补充状态验证：CollectionView multi-key grouping 对 `IList` key 集合新增 / 删除 subgroup 都走 Count/indexer，不再创建 key-list enumerator；`alpha` / `beta` 两个 subgroup 和删除后 empty view 语义保持。

本轮补充状态验证：CollectionView multi-key grouping 对 `IReadOnlyList<object>` key 集合新增 / 删除 subgroup 走 Count/indexer，不创建 key-list enumerator；两个 read-only key subgroup 和删除后 empty view 语义保持。

本轮补充状态验证：ValidationUtils FindEqualValidationResult 对 read-only validation result list 走 Count/indexer，不再创建 collection enumerator；命中返回旧 result 实例、未命中返回 null 的语义保持。

本轮补充状态验证：ValidationUtils AddExceptionIfNew 对 read-only exception list 走 Count/indexer，不再创建 collection enumerator；重复 message 不添加，新 message 仍追加到原 collection。

本轮补充状态验证：DataGrid column group collection changed / tree build / group header measure / arrange 的 indexed traversal 保持多 top-level group、nested group、leaf column 深度优先顺序、`GroupParent` 链、header view item 创建、非空 group header width 和 leaf header arrange 顺序。

本轮补充状态验证：DataGridDataConnection `IndexOf()` 对包含 null 的 `IReadOnlyList<object>` 继续走 Count/indexer 且不枚举；非 null search 会跳过 null entry，null search 会返回 null item index；裸 `IEnumerable` fallback 同样保持 null 语义并释放 enumerator。

结果：`DataGrid state verification passed.` 覆盖：无 filter items 不创建 shell、关闭态 filter content lazy、filter tree group name 延迟到 tree items materialize 且 menu/tree item 层级计数保持、filter flyout presenter reset / ok buttons 不再注册本地 Click handlers 且 ok 仍设置 active shutdown、filter flyout 默认被动关闭不通知，`FilterOnClose=true` 被动关闭和 OK 确认关闭仍通知 selected values、运行时 add/clear filter items 后 indicator 可见性和 flyout shell 同步、filter request 手写 value copy 保留 deferred 快照语义、FilterDescription 属性类型缓存与 path 变更重置、FilterDescription 多条件过滤复用 record value / `ToString()` 且 custom filter 保持 first-condition 语义、DataGridColumn sort/filter description lookup 保持 first-match / no-match null 语义、DataGrid bound column clipboard binding fallback / explicit override / clear fallback 语义保持、replacement filter set-compare 不再分配 hash sets 且重复值 set 语义保持、realized column header 不再注册本地 hover / press / release / move routed handlers、`HeaderPointerPressed` class handler 转发 exactly once 且 sender 保持为 `DataGridColumn`、column header 普通点击不再发送 drag-over cleanup 且 reorder lost-capture 仍发送一次 null cleanup、column header child detach 清空 resize/reorder static drag state 和 presenter drag indicator、column header TopLevel detach 清空 static drag state 且不改动 visual children、realized column group header 不再注册本地 press / release forwarding handlers、group `HeaderPointerPressed` class handler 转发 exactly once 且 sender 保持为 `DataGridColumnGroupItem`、realized row header 不再注册本地 press handler 且左键点击仍选中行并更新 `CurrentSlot`、realized row group header 不再注册本地 press handler 且左键点击仍更新 `CurrentSlot`、realized DataGrid core 不再注册本地 `KeyDown` / `KeyUp` / `GotFocus` / `LostFocus` handlers，且 `GotFocus` 更新 `ContainsFocus`、`Down` key 仍推动 current slot、DataGrid pagination 重套模板后旧 top/bottom part 释放 page-changed handler、新 top/bottom part exactly one handler 且 detach 后释放、realized rows presenter 不再注册本地 `ScrollGesture` handler，且滚动手势仍 handled 并更新 `VerticalOffset`、rows presenter clip `RectangleGeometry` 跨重复 arrange 和尺寸变化复用且 `Rect` 会更新、row bottom grid-line clip `RectangleGeometry` 跨重复 arrange 和水平偏移变化复用且 `Rect` 会更新、row hidden clip `RectangleGeometry` 跨 hidden apply / clear / reapply 复用、row group header 内置模板 part lookup 恢复且 child clip `RectangleGeometry` 跨 repeated arrange 和水平偏移变化复用、frozen 模式清空 child `Clip`、row group header frozen child `TranslateTransform` 跨 repeated arrange 和水平偏移变化复用、frozen 模式清空 child `RenderTransform`、cell clip `RectangleGeometry` 跨 repeated cells presenter arrange 和水平偏移变化复用且 clipping 取消后清空 `Clip`、column header clip `RectangleGeometry` 跨 repeated header presenter arrange 和水平偏移变化复用且 clipping 取消后清空 `Clip`、group column header view item clip `RectangleGeometry` 跨 repeated group header presenter arrange 和水平偏移变化复用且 clipping 取消后清空 `Clip`、column reordering indicator clip `RectangleGeometry` 跨 repeated clip update 和边界变化复用且 clipping 取消 / indicator replacement 后清空旧 `Clip`、details presenter clip `RectangleGeometry` 跨重复 arrange 和水平偏移变化复用且 frozen 模式清空 `Clip`、分组行头内 row header 的 owner/template 顺序不再空引用、DataGridCell 跟随 header sort / reorder state 且 detach 后释放订阅、special columns 在 `Columns.Clear()` 后释放 cached owning grid、DataGridRowReorderColumn duplicate check 仍拒绝第二个 reorder column、row reorder handle click 不再触发 no-op drop / `RowReordered` / rows presenter arrange invalidation、row reorder drag release/row unload/detach 清空 dragged row state 且 unload 移除 ghost row、column reorder drag indicator 复用 cached render pen 且 foreground 变化后重建、column reorder repeated null-target drag-over 只通知一次、DetailsPresenter `ContentHeight` 改变后触发 measure invalidation、RowExpander checked/details visibility 双向同步和 detach 释放、operation buttons 不再注册本地 part routed handlers 且 edit / save / cancel 行为保持、selection column header checkbox 不再注册本地 Click handler 且全选 / 清空选择行为保持、`SelectedItems` public enumeration 在全选 / 清空后数量保持、empty selection `InitializeElements(false)` reset 不替换 selected-items cache 且 selection 保持空、cleared selection cache 在 `UpdateIndexes()` 后释放 backing capacity、non-empty selection reset 后 5 个 selected items 顺序保持、single selection 替换唯一选中行仍只保留新行、DataGridCheckBoxColumn pointer edit 在 zero bounds 时不再订阅 `LayoutUpdated` 且布局后仍正确切换 checked state、DataConnection 非集合 enumerable count/getAny 直接枚举且 dispose raw enumerator、DataConnection `IList` item-type probe 直接读取 Count/indexer 且不枚举、DataConnection collection-view `Any()` 直接读取 `IsEmpty` 且不枚举 view、DataConnection DataProperties 重复读取复用缓存且 clear / DataSource replacement 后刷新、未 realized menu/tree filter presenter 通过 `ItemsView` fallback 解析 6 个 nested leaf 且单选 selected values `Count=1 / Capacity=1`，未 realized `ResetFilter()` 会清空 nested checked leaf，已 realized presenter 空选 / 单选 / 全选容量语义保持、ValidationUtils exception filtering 过滤 BindingChainException / 保持顺序 / message 去重语义保持、ValidationUtils member-name comparison 的 Count / indexer 快路径和 fallback enumerator dispose 语义保持、CollectionView unfiltered source list capacity 预分配且 filtered refresh 不预分配全量源 Count、CollectionView filtered read-only list refresh 不枚举 source 且保留过滤结果顺序、CollectionView IList source copy / Refresh / unfiltered local-array refresh / filtered refresh 不枚举 source 且保留顺序、CollectionView sorted result 按输入 count 预分配且同 key stable order 保持、CollectionView paged enumerator 直接按页范围读取 internal list，第 2 页 / 末页 / Reset 后枚举顺序保持、CollectionView empty paged enumerator 复用共享空数组且 `PageIndex < 0` 枚举 0 项、CollectionView grouped key matching 会为 `[1,2,1]` 创建两个 subgroup 且 repeated key leaf order / `IndexOf()` 正确、PathGroupDescription owner type 切换会刷新 property type 并返回 mixed owner type 的真实 key、CollectionView 已知 `PropertyChanged` 属性名复用 cached event args 且未知属性名仍不缓存、CollectionView 无 payload Reset collection changed 复用 cached event args 且 payload 为空、DataGrid group data 的 `ItemCount` / `IsBottomLevel` / `GroupKeys` 固定 `PropertyChanged` 通知复用 cached event args 且 property name 保持正确、MergedComparer comparer array 长度和 first / fallback comparer 排序顺序保持、PathSortDescription 初始化后缓存 property comparer 且 `MergedComparer` 保留 custom path value comparer、SortDescription comparer/path key selector delegate 复用且排序语义保持、PrepareLocalArray 无过滤 copy 和 filtered refresh 语义保持。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 100
```

关键结果：

| Scenario | ms/item | KB/item | Visual/root | Logical/root |
| --- | ---: | ---: | ---: | ---: |
| DataGrid.Basic | 14.423 | 2948.2 | 305.0 | 1.0 |
| DataGrid.Filter.Menu.Closed | 8.734 | 2586.2 | 267.0 | 1.0 |
| DataGrid.Filter.Tree.Closed | 8.558 | 2584.3 | 267.0 | 1.0 |
| DataGrid.RowHeaders | 9.685 | 3107.1 | 336.0 | 1.0 |
| DataGrid.RowDetails.Collapsed | 9.875 | 3024.5 | 320.0 | 1.0 |
| DataGrid.GroupHeaders | 8.532 | 2571.9 | 266.0 | 1.0 |
| DataGrid.RowGroups | 10.046 | 2936.6 | 315.0 | 1.0 |
| DataGrid.GalleryShape | 40.925 | 12409.1 | 1260.0 | 5.0 |

## 追加结构优化：keyboard modifier flag check

`KeyboardHelper.GetMetaKeyState()` 是 DataGrid 键盘选择、范围选择和快捷键处理的共享入口。旧实现用 `KeyModifiers.HasFlag()` 判断 Ctrl/Cmd、Shift、Alt；本轮改为直接 bitwise check，平台 Ctrl/Cmd 映射仍由 `GetPlatformCtrlOrCmdKeyModifier()` 决定，返回语义不变。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| DataGrid 2-state keyboard modifier `HasFlag` calls / meta-key query | 2 | 0 | `(2 - 0) / 2` | 100.00% | structural-only；Ctrl/Cmd + Shift 判断不再走 enum helper |
| DataGrid 3-state keyboard modifier `HasFlag` calls / meta-key query | 3 | 0 | `(3 - 0) / 3` | 100.00% | structural-only；Ctrl/Cmd + Shift + Alt 判断不再走 enum helper |
| Platform Ctrl/Cmd mapping | unchanged | unchanged | n/a | 0.00% | 行为保持；仍使用 Avalonia hotkey configuration |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |

## 追加结构优化：DataGridLength string parse span fast path

`DataGridLengthConverter.ConvertFrom()` 解析字符串宽度时，旧实现先 `Trim()` 得到新字符串；star 宽度还会再 `Substring()` 去掉 `*`。本轮改为 `ReadOnlySpan<char>.Trim()`、span suffix 判断和 span parse，`Auto` / `SizeToCells` / `SizeToHeader` 匹配也直接比较 span。数值宽度 fallback 仍保留原 `Convert.ToDouble(value, culture)` 路径。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| DataGridLength string trim temp strings / string parse | 1 | 0 | `(1 - 0) / 1` | 100.00% | structural-only；整体 trim 走 span |
| DataGridLength star suffix temp substrings / star parse | 1 | 0 | `(1 - 0) / 1` | 100.00% | structural-only；star 权重切片走 span |
| Unit keyword comparison temp strings / string parse | 0 extra after trim | 0 | n/a | 结构保持 | `Auto` / `SizeToCells` / `SizeToHeader` 直接和 trimmed span 比较 |
| Numeric fallback behavior | `Convert.ToDouble(value, culture)` | same fallback | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |
