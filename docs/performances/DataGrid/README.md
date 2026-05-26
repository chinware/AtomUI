# DataGrid 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Phase F / Tier 4
> 状态：T4.1 cell header-state binding + row/row group header pointer handler + core input handler + pagination re-template subscription cleanup + rows presenter scroll gesture / clip geometry + row gridline / row hidden clip / row group header child clip / row group header transform cleanup partial done；T4.2 column filter flyout lifecycle + filter indicator binding + filter materialization allocation cleanup + filter presenter button class handler + column/group header pointer handler + column header click drag-over cleanup guard + column header / group header view item clip cleanup、column/header/group header presenter visible-column iterator cleanup、column lifecycle / edit / copy / resize-hit-test iterator cleanup、column sort/filter description lookup cleanup、column clipboard dead field cleanup partial done；T4.3 special column detach lifecycle + row expander details binding、operation buttons class handler cleanup partial done；T4.4 column reorder drag indicator render + reordering clip cleanup + column drag-over null-target notification dedup + row reorder duplicate check + row reorder click no-op drop cleanup + row reorder drag state release + checkbox edit pointer bounds wait cleanup partial done；T4.5 row details presenter measure registration、data connection enumerable count cleanup / collection-view `Any()` IsEmpty fast path、MergedComparer comparer array cleanup、unfiltered source list preallocation、sorted list materialization preallocation、path sort comparer cache、filter description property type cache / record value reuse、validation exception filtering cleanup partial done；DataGrid core / row / virtualization 仍待后续专项。

---

## 0. 结论

本轮优化有效，但不是大幅减少 DataGrid 主视觉树的优化。收益集中在关闭态过滤列头、列/行生命周期、pagination 重套模板生命周期、row details binding 和 data-layer hot paths：

- 无过滤项的列头不再创建空 filter flyout shell。
- 有过滤项的列头只保留轻量 flyout shell，菜单/树过滤项延迟到首次打开前创建。
- filter indicator detach 时释放 flyout shell，并补齐 `CollectionView.FilterDescriptions` 订阅切换/释放。
- filter indicator 可见性不再为每个列头创建 3 路 `MultiBinding` + converter，改为列头 DirectProperty + `{TemplateBinding}`。
- filter popup materialized 后，menu/tree presenter 的 reset / ok 按钮不再注册本地 Click handlers，改为 presenter class handler。
- filter indicator 的 tree radio group name 不再在每个列头构造时创建，改为首次 materialize tree filter items 时按需创建。
- menu/tree filter items materialize 时不再递归返回临时 `List`，改为直接填充 `ItemCollection`，保持 menu 5 项 / tree 6 项层级计数。
- column header hover 不再为每个 realized `DataGridColumnHeader` 注册本地 `PointerEntered` / `PointerExited` handler，改走 Avalonia 的 virtual override 路径。
- column header press/release/move 不再为每个 realized `DataGridColumnHeader` 注册本地 handler；内部 resize/reorder/sort 逻辑和 `DataGridColumn.HeaderPointerPressed/Released` 转发都改为 class handler。
- column header 普通点击 / 排序释放不再发送无效 `ColumnDraggingOver(null, null)` cleanup；真正列重排释放仍保留一次 null cleanup。
- `DataGridColumnHeader` 在 frozen-column 裁剪路径不再每次 header presenter arrange 新建 clip `RectangleGeometry`，改为 header 级复用；不需要裁剪时仍清空 `Clip`。
- column group header press/release 不再为每个 realized `DataGridColumnGroupHeader` 注册本地 forwarding lambda，改为 class handler 转发到 `DataGridColumnGroupItem.HeaderPointerPressed/Released`。
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
- DataGrid column frozen-state 修正和 header pseudo-class 刷新不再通过 displayed / visible column iterator 和 `ToList()` 临时集合，改为 display index 直接遍历。
- DataGrid 水平列坐标和列滚动 offset 计算不再通过 visible/frozen/scrolling column iterator，改为 display index 直接遍历。
- DataGrid 自动列宽完成、ItemsSource header 初始化、列宽属性变更、编辑元素生成、star width coerce、列 resize、剪贴板复制和 column header hit-test 不再通过 visible/displayed column iterator，改为 display index 直接遍历。
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
- `DataGridRowReorderColumn` 的重复列检查不再通过 `Columns.Count(predicate)` 构造 LINQ predicate/enumerator，改为 indexed loop，并在发现第二个 reorder column 时提前退出。
- `DataGridRowReorderHandle` 现在只有真正进入拖拽后才执行 drop / `RowReordered` / rows presenter arrange invalidation；单击 row reorder handle 不再触发无效重排事件。
- `DataGridRowReorderHandle` 在真实 drop 后清空 rows presenter 的 dragged row state；row unload 会按 owner 释放 ghost row，TopLevel detach 只清 static state，避免 detach 遍历期间改动 rows presenter visual children。
- `DataGridDetailsPresenter.ContentHeight` 的 `AffectsMeasure` 注册从实例构造函数移到静态构造函数，避免每个 details presenter 重复添加 class-level property observer。
- `DataGridDataConnection.TryGetCount()` 对非 `ICollection` 的 `IEnumerable` fallback 不再通过 `Cast<object>().Count()/Any()` 构造 LINQ iterator，改为直接使用 raw `IEnumerator`。
- `DataGridDataConnection.Any()` 在 `DataSource` 已是 `IDataGridCollectionView` 时直接读取 `IsEmpty`，不再为非 `ICollection` collection view 构造 enumerator / 调 `MoveNext()`。
- `DataGridFilterDescription` 对同一 item type 的 `PropertyPath` 属性类型解析结果做缓存，`PropertyPath` 变更时清空缓存并重新解析；同一 record 的多条件过滤只取一次属性值并只执行一次 `ToString()`。
- `DataGridColumn.GetSortDescription()` / `GetFilterDescription()` 不再通过 `OfType()` / `FirstOrDefault(predicate)` 构造 LINQ iterator / predicate delegate，改为 indexed loop 并保持 first-match 语义。
- `DataGridColumn` 移除已失效的 `_clipboardContentBinding` 私有字段，`ClipboardContentBinding` 仍保持 auto property；`DataGridBoundColumn` 的 Binding fallback / explicit override / clear fallback 行为已验证。
- `DataGridCollectionView.SortList()` 仍保留原 LINQ stable sort chain，但最终 sorted result 不再用 `seq.ToList()`，改为按输入 count 预分配并手写 materialize。
- `DataGridSortDescription.FromPath()` 的属性 comparer 不再在每次 `Compare()` 时重新解析；已知属性类型后缓存 comparer，并保持自定义 path value comparer 在 `MergedComparer` 路径中不被默认 comparer 覆盖。
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
| Filter indicator tree radio group name strings / closed indicator | 1 string | 0 strings | 100.00% deferred | 结构/分配优化；只有首次 materialize tree filter items 时创建 |
| DataGrid.GalleryShape closed tree group name strings | 22 strings | 0 strings | 100.00% deferred | 结构/分配优化；按当前 performance GalleryShape 的 header indicator 数估算 |
| Nested 5-item filter materialization temp item lists / first open | 2 lists | 0 lists | 100.00% removed | 结构/分配优化；menu/tree 递归直接填充 `ItemCollection` |
| Filter flyout item hierarchy | menu 5 / tree 6 items | menu 5 / tree 6 items | behavior preserved | 正确性保持；状态验证覆盖 exact count 和 tree group name |
| Column header local hover routed handlers | 2 per realized header | 0 per realized header | 100.00% removed | 结构/分配优化；hover 语义保留在 `OnPointerEntered` / `OnPointerExited` override |
| Column header local press/release/move handlers | 5 per standard header | 0 per standard header | 100.00% removed | 结构/分配优化；internal handler + public column event forwarding 均迁到 class handler |
| Column header click drag-over cleanup events | 1 `ColumnDraggingOver(null, null)` per no-drag click release | 0 events | 100.00% removed | 交互路径优化；普通点击 / 排序不再误发 drag-over cleanup |
| Column header clip geometry allocation | 1 new `RectangleGeometry` per clipped header arrange | 0 new `RectangleGeometry` per clipped header arrange after first | 100.00% repeated-arrange allocation removed | 结构/分配优化；frozen-column header clip instance 复用、Rect 更新和 clear 已验证 |
| Column group header forwarding handlers | 2 per group header | 0 per group header | 100.00% removed | 结构/分配优化；Gallery group header 示例有 4 个 group items |
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
| Column frozen-state displayed-column iterator | 1 iterator per correction pass | 0 iterator | 100.00% removed | 结构/分配优化；left/right frozen 状态按 display index 直接计算 |
| Header pseudo-class visible-column list allocation | 1 iterator + 1 `List<DataGridColumn>` copy per refresh | 0 iterator/list allocation | 100.00% removed | 结构/分配优化；first/last/middle header 状态按 display index 刷新 |
| Horizontal column coordinate visible-column iterators | 4 iterators across compute/display/scroll coordinate paths | 0 iterators | 100.00% removed | 结构/分配优化；frozen width、column X、negative offset 和 scroll offset 直接 indexed 遍历 |
| Column lifecycle / edit / copy / resize hit-test iterator callsites | 11 `GetDisplayedColumns()` / `GetVisibleColumns()` callsites | 0 callsites | 100.00% removed | 结构/分配优化；自动列宽、header 初始化、列宽属性、编辑、star coerce、resize、copy、header hit-test 直接 indexed 遍历 |
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
| Details presenter measure registration | 1 `property.Changed` subscription per presenter instance | 1 static registration per control type | duplicate class-level subscriptions removed | 结构/生命周期优化；`ContentHeight` 仍触发 measure invalidation |
| Data connection enumerable count LINQ iterator | 1 `Cast<object>()` iterator per non-collection count/any fallback | 0 LINQ iterators | 100.00% removed | 结构/分配优化；raw `IEnumerator` count/getAny 行为已验证 |
| DataConnection Any on `IDataGridCollectionView` | 1 enumerator + 1 `MoveNext()` for non-`ICollection` view | 0 enumerators + 1 `IsEmpty` read | 100.00% enumeration removed | 结构/分配优化；collection-view empty check 走接口语义，且不枚举 view |
| CollectionView sorted result materialization | 1 LINQ `ToList()` callsite after sort chain | 0 LINQ `ToList()` callsites | 100.00% removed | 结构/分配优化；sorted result 按输入 count 预分配，stable order 已验证 |
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

---

## 3. 验证

```bash
dotnet build -c Release -f net10.0 tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
```

结果：构建通过，0 warning。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-datagrid-states
```

结果：`DataGrid state verification passed.` 覆盖：无 filter items 不创建 shell、关闭态 filter content lazy、filter tree group name 延迟到 tree items materialize 且 menu/tree item 层级计数保持、filter flyout presenter reset / ok buttons 不再注册本地 Click handlers 且 ok 仍设置 active shutdown、运行时 add/clear filter items 后 indicator 可见性和 flyout shell 同步、filter request 手写 value copy 保留 deferred 快照语义、FilterDescription 属性类型缓存与 path 变更重置、FilterDescription 多条件过滤复用 record value / `ToString()` 且 custom filter 保持 first-condition 语义、DataGridColumn sort/filter description lookup 保持 first-match / no-match null 语义、DataGrid bound column clipboard binding fallback / explicit override / clear fallback 语义保持、replacement filter set-compare 不再分配 hash sets 且重复值 set 语义保持、realized column header 不再注册本地 hover / press / release / move routed handlers、`HeaderPointerPressed` class handler 转发 exactly once 且 sender 保持为 `DataGridColumn`、column header 普通点击不再发送 drag-over cleanup 且 reorder lost-capture 仍发送一次 null cleanup、realized column group header 不再注册本地 press / release forwarding handlers、group `HeaderPointerPressed` class handler 转发 exactly once 且 sender 保持为 `DataGridColumnGroupItem`、realized row header 不再注册本地 press handler 且左键点击仍选中行并更新 `CurrentSlot`、realized row group header 不再注册本地 press handler 且左键点击仍更新 `CurrentSlot`、realized DataGrid core 不再注册本地 `KeyDown` / `KeyUp` / `GotFocus` / `LostFocus` handlers，且 `GotFocus` 更新 `ContainsFocus`、`Down` key 仍推动 current slot、DataGrid pagination 重套模板后旧 top/bottom part 释放 page-changed handler、新 top/bottom part exactly one handler 且 detach 后释放、realized rows presenter 不再注册本地 `ScrollGesture` handler，且滚动手势仍 handled 并更新 `VerticalOffset`、rows presenter clip `RectangleGeometry` 跨重复 arrange 和尺寸变化复用且 `Rect` 会更新、row bottom grid-line clip `RectangleGeometry` 跨重复 arrange 和水平偏移变化复用且 `Rect` 会更新、row hidden clip `RectangleGeometry` 跨 hidden apply / clear / reapply 复用、row group header 内置模板 part lookup 恢复且 child clip `RectangleGeometry` 跨 repeated arrange 和水平偏移变化复用、frozen 模式清空 child `Clip`、row group header frozen child `TranslateTransform` 跨 repeated arrange 和水平偏移变化复用、frozen 模式清空 child `RenderTransform`、cell clip `RectangleGeometry` 跨 repeated cells presenter arrange 和水平偏移变化复用且 clipping 取消后清空 `Clip`、column header clip `RectangleGeometry` 跨 repeated header presenter arrange 和水平偏移变化复用且 clipping 取消后清空 `Clip`、group column header view item clip `RectangleGeometry` 跨 repeated group header presenter arrange 和水平偏移变化复用且 clipping 取消后清空 `Clip`、column reordering indicator clip `RectangleGeometry` 跨 repeated clip update 和边界变化复用且 clipping 取消 / indicator replacement 后清空旧 `Clip`、details presenter clip `RectangleGeometry` 跨重复 arrange 和水平偏移变化复用且 frozen 模式清空 `Clip`、分组行头内 row header 的 owner/template 顺序不再空引用、DataGridCell 跟随 header sort / reorder state 且 detach 后释放订阅、special columns 在 `Columns.Clear()` 后释放 cached owning grid、DataGridRowReorderColumn duplicate check 仍拒绝第二个 reorder column、row reorder handle click 不再触发 no-op drop / `RowReordered` / rows presenter arrange invalidation、row reorder drag release/row unload/detach 清空 dragged row state 且 unload 移除 ghost row、column reorder drag indicator 复用 cached render pen 且 foreground 变化后重建、column reorder repeated null-target drag-over 只通知一次、DetailsPresenter `ContentHeight` 改变后触发 measure invalidation、RowExpander checked/details visibility 双向同步和 detach 释放、operation buttons 不再注册本地 part routed handlers 且 edit / save / cancel 行为保持、selection column header checkbox 不再注册本地 Click handler 且全选 / 清空选择行为保持、DataGridCheckBoxColumn pointer edit 在 zero bounds 时不再订阅 `LayoutUpdated` 且布局后仍正确切换 checked state、DataConnection 非集合 enumerable count/getAny 直接枚举且 dispose raw enumerator、DataConnection collection-view `Any()` 直接读取 `IsEmpty` 且不枚举 view、ValidationUtils exception filtering 过滤 BindingChainException / 保持顺序 / message 去重语义保持、CollectionView unfiltered source list capacity 预分配且 filtered refresh 不预分配全量源 Count、CollectionView sorted result 按输入 count 预分配且同 key stable order 保持、MergedComparer comparer array 长度和 first / fallback comparer 排序顺序保持、PathSortDescription 初始化后缓存 property comparer 且 `MergedComparer` 保留 custom path value comparer。

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite datagrid --count 100
```

关键结果：

| Scenario | ms/item | KB/item | Visual/root | Logical/root |
| --- | ---: | ---: | ---: | ---: |
| DataGrid.Basic | 14.108 | 2949.1 | 305.0 | 1.0 |
| DataGrid.Filter.Menu.Closed | 9.196 | 2586.8 | 267.0 | 1.0 |
| DataGrid.Filter.Tree.Closed | 8.330 | 2584.5 | 267.0 | 1.0 |
| DataGrid.RowHeaders | 9.631 | 3108.0 | 336.0 | 1.0 |
| DataGrid.RowDetails.Collapsed | 10.308 | 3025.4 | 320.0 | 1.0 |
| DataGrid.GroupHeaders | 8.825 | 2574.7 | 266.0 | 1.0 |
| DataGrid.RowGroups | 9.709 | 2939.3 | 315.0 | 1.0 |
| DataGrid.GalleryShape | 42.696 | 12412.7 | 1260.0 | 5.0 |
