# DataGrid 桌面版实现原理

本文档描述 DataGrid 桌面版的内部实现范围、源码职责、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [DataGrid 桌面版架构设计](overview.md)，列宽算法和 presenter 协作见 [DataGrid 列宽分配设计](column-sizing-design.md)，变化记录见 [DataGrid Changelog](changelog.md)。涉及 Control Own Token 的实现应同时阅读 [DataGrid Token 设计](token.md)。

Popup 接入边界：`DataGrid` 负责 column-filter 业务状态和内容准备，filter Flyout 仅作为 relay 适配层，filter Popup 负责实际显示。模板重建或宿主切换时必须先释放旧 relay，再绑定新的 Popup；普通外点、Escape、失焦和业务关闭在 pinned 状态下被拦截，detach、窗口销毁、跨 TopLevel 和无效锚点必须走生命周期关闭并释放 Popup host。完整状态机见 [Popup 钉住打开设计](../../other/popup/popup-pinned-open-design.md)。

## 1. 实现定位

本文档覆盖 DataGrid 的控件实现、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls.DataGrid`：代表文件包括 `AtomUIDataGridThemesProvider.cs`、`ThemeManagerBuilderExtensions.cs`、`DataGrid.Cells.cs`、`DataGrid.Columns.cs`、`DataGrid.Privates.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Cell`：4 个文件，代表文件 `DataGridCell.cs`、`DataGridCellCollection.cs`、`DataGridCellCoordinates.cs`、`DataGridCellsPresenter.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/Column`：31 个文件，代表文件 `DataGridAbstractTextColumn.cs`、`DataGridBoundColumn.cs`、`DataGridCheckBoxColumn.cs`、`DataGridColumn.Privates.cs`、`DataGridColumn.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Column/Filters`：7 个文件，代表文件 `DataGridFilterIndicator.cs`、`DataGridFilterItem.cs`、`DataGridFilterValuesSelectedEventArgs.cs`、`DataGridMenuFilterFlyout.cs`、`DataGridMenuFilterFlyoutPresenter.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Data`：15 个文件，代表文件 `CollectionViewGroupRoot.cs`、`DataGridCollectionView.cs`、`DataGridCollectionViewGroup.cs`、`DataGridCollectionViewGroupInternal.cs`、`DataGridCurrentChangingEventArgs.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/EventArgs`：18 个文件，代表文件 `DataGridAutoGeneratingColumnEventArgs.cs`、`DataGridBeginningEditEventArgs.cs`、`DataGridCellEditEndedEventArgs.cs`、`DataGridCellEditEndingEventArgs.cs`、`DataGridCellEventArgs.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.Localization`：生成 Catalog descriptor、语言模块注册入口和 `DataGridLangResource` 扩展。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.ResourceHost.ScopedResourceHostGenerator`：1 个文件，代表文件 `GenerateScopedResourceHostAttribute.g.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator`：生成 `GeneratedControlPackageRegistration.g.cs`、`GeneratedThemeSchema.g.cs` 和 `TokenResourceConst.g.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.ThemeAssetManifestGenerator`：生成独立主题叶子的 `GeneratedControlThemeAssetManifest.g.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/Localization`：`DataGridLangResourceKind.cs` 定义稳定 Catalog，`en-US.xlf`、`zh-CN.xlf`、`zh-TW.xlf` 提供内置翻译。
- `src/AtomUI.Desktop.Controls.DataGrid/Properties`：1 个文件，代表文件 `AssemblyInfo.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/Row`：7 个文件，代表文件 `DataGridDetailsPresenter.cs`、`DataGridRow.Privates.cs`、`DataGridRow.cs`、`DataGridRowGroupHeader.cs`、`DataGridRowGroupInfo.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Themes`：21 个文件，代表文件 `DataGridCellTheme.axaml`、`DataGridColumnGroupHeaderTheme.axaml`、`DataGridColumnHeaderTheme.axaml`、`DataGridColumnHeaderTheme.cs`、`DataGridHeaderViewItemTheme.axaml` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Utils`：8 个文件，代表文件 `DataGridFrozenGrid.cs`、`DataGridHelper.cs`、`DataGridValueConverter.cs`、`KeyboardHelper.cs`、`Range.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Utils/Converters`：2 个文件，代表文件 `DataGridPaginationVisibilityConvertor.cs`、`DataGridUniformBorderThicknessToScalarConverter.cs`。

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 3. 核心类职责

- `AtomUIDataGridThemesProvider`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `CollectionViewGroupComparer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `CollectionViewGroupRoot`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGrid`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridAbstractTextColumn`：集合项、节点或容器类型，承载单项状态和模板协作。
- `DataGridBoundColumn`：集合项、节点或容器类型，承载单项状态和模板协作。
- `DataGridCell`：集合项、节点或容器类型，承载单项状态和模板协作。
- `DataGridCellCollection`：数据、状态或行为协作类型，维护集合同步和事件路径。
- `DataGridCellCoordinates`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridCheckBoxColumn`：集合项、节点或容器类型，承载单项状态和模板协作。
- `DataGridCollectionViewGroup`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridCollectionViewGroupInternal`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridColumnCollection`：数据、状态或行为协作类型，维护集合同步和事件路径。
- `DataGridColumnDraggingOverIndicator`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridColumnGroupChangedArgs`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridColumnGroupHeader`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridColumnGroupItem`：集合项、节点或容器类型，承载单项状态和模板协作。
- `DataGridColumnHeader`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridColumnHeaderTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `DataGridComparerSortDescription`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridDataConnection`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridDefaultFilter`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `DataGridDisplayData`：数据、状态或行为协作类型，维护集合同步和事件路径。
- 其他 54 个内部类型按源码目录分层维护，修改前应先确认所有引用路径。

核心协作规则：

- 控件实例是 public API 和运行时状态 owner。
- Template part 是视觉协作对象，生命周期必须受 `OnApplyTemplate` 或模板加载流程管理。
- 数据对象、选项对象、任务对象或节点对象只保存业务数据，不应反向持有不可释放的视觉对象。
- 弹层、窗口、计时器、异步 loader 和全局管理器必须有明确关闭、解绑或释放路径。
- `DataGrid` 是行拖动会话 owner；`DataGridRowReorderHandle` 只转发 Pointer 输入，`DataGridRowsPresenter` 只维护
  ghost row，二者都不保存跨控件共享的拖拽状态。
- `IDataGridCollectionViewMoveSupport` 是 CollectionView 的可选移动能力边界；`DataGridCollectionView` 负责
  内置源集合的能力判断、索引解释、提交、通知和失败回滚。
- `DataGrid` 是列宽状态和统一 star solver 的 owner；普通/分组列头、rows 和 cells presenter 只报告有限视口
  或内容测量结果，不能各自维护列显示宽度。
- `DataGridFillerColumn` 只投影统一调整后仍无法由真实列吸收的正剩余空间，不参与决定 star 分配是否执行。

## 4. 状态与数据流

DataGrid 的状态流遵循下面路径：

```text
Public API / ItemsSource / Command / Event
  -> 控件实例状态
  -> internal state / effective state / pseudo-class
  -> template part property / AXAML selector
  -> renderer / popup / adorner / Gallery observable behavior
```

源码中的状态入口按以下语义维护：

- 内容与数据：`AutoGenerateColumns`、`CanUserFilterColumns`、`CanUserReorderColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`CellEditingTemplate`、`CellTemplate`、`ColumnHeaderHeight`、`ContentHeight` 等 32 项。
- 选择与集合：`ClipboardCopyMode`、`CurrentSortDirection`、`Filters`、`SelectedFilterValues`、`FilterPresenterMode`、`FilterSelectionMode`、`FilterApplyMode`、`Index`、`IsFilterActivated`、`IsHideOnSinglePage`、`IsHoverMode`、`IsSelected`、`IsSorterTooltipVisible` 等。
- 交互与状态：`AscendingIndicatorVisible`、`DescendingIndicatorVisible`、`IsDeleteEnabled`、`IsDetailsVisible`、`IsEditEnabled`、`IsFrameBorderVisible`、`IsFrozen`、`IsLeaf`、`IsMotionEnabled`、`IsOperating` 等 19 项。
- 视觉与布局：`BottomPaginationAlign`、`ColumnWidth`、`HorizontalAlignment`、`HorizontalScrollBarVisibility`、`MaxColumnWidth`、`MinColumnWidth`、`RowHeight`、`SeparatorBrush`、`SizeType`、`SublevelIndent` 等 14 项。
- 其他稳定入口：`CellTheme`、`CollectionView`、`CustomOperatingIndicator`、`EmptyIndicator`、`Footer`、`FormatString`、`GridLinesVisibility`、`Level`、`Maximum`、`Minimum` 等 15 项。

维护要求：

- 外部设置的 Avalonia 属性必须在模板应用前后保持一致。
- 集合、选择、展开、过滤、分页、上传任务或异步 loader 必须能处理 reset、replace 和 clear。
- 伪类和 internal state 必须从单一 owner 推导，避免双向同步导致循环更新。
- overview.md 的 API 契约说明应与源码实际状态流一致。

分页状态流以 `DataGridCollectionView` 为 owner：

```text
DataGrid.ItemsSource / DataGrid.PageSize
  -> DataGridCollectionView.ItemCount / PageSize / PageIndex
  -> DataGrid pagination state projection
  -> PART_TopPagination / PART_BottomPagination

Pagination.CurrentPageChanged
  -> DataGridCollectionView.MoveToPage(oneBasedPage - 1)
  -> DataGridCollectionView.PageChanging
  -> both Pagination.CurrentPage
```

`PageSize` 配置和分页部件状态投影是两个不同职责。数据或 `PageSize` 变化时，DataGrid 先配置 CollectionView，
再把 CollectionView 最终的 `ItemCount`、`PageSize` 和 `PageIndex` 同步到当前分页部件。模板首次应用或重新套用时，
CollectionView 已经是有效状态真源，只回放分页投影，不应重新配置数据视图。顶部和底部分页部件不持久保存分页状态；
任一部件缺失时只跳过该视觉投影，不改变 CollectionView 或另一个部件。

列宽状态流以 `DataGrid` 和 `DataGridColumn` 为 owner：

```text
DataGridColumnHeadersPresenter / DataGridGroupColumnHeadersPresenter
  -> header desired widths + finite header viewport when rows are absent
DataGridRowsPresenter / DataGridCellsPresenter
  -> CellsWidth + realized cell desired widths when rows are present
DataGrid
  -> complete initial Auto measurement
  -> resolve star widths through AdjustColumnWidths
DataGridColumn display widths
  -> headers / rows / cells / filler / scrollbars
```

普通和分组列头 presenter 共享同一输入契约：完成 header 内容测量，并在 rows presenter 因空数据不参与布局时
把有限 `availableSize.Width` 交给 DataGrid。正常数据路径继续以 `CellsWidth` 表达扣除行头等占用后的列区域。
presenter 不直接修改一组 star 列，也不把 filler 当作宽度分配结果。完整模式矩阵、算法和兼容边界见
[DataGrid 列宽分配设计](column-sizing-design.md)。

列过滤状态流以列对象为状态 owner：

```text
DataGridColumn.Filters
  -> DataGridFilterIndicator materialized items
  -> DataGridColumn.SelectedFilterValues
  -> DataConnection.FilterDescriptions
  -> CollectionView refresh / filter icon active state
```

`Filters` 是过滤候选项数据源，必须允许替换、绑定和集合变更通知。过滤项可以来自 `DataGridFilterItem`，也可以来自业务 DTO；解析文本、值和 children 时优先使用列上的 member path 配置，避免把 Gallery 示例对象变成业务层必须依赖的模型。DTO member path 解析只允许生成 accessor 路径，不在过滤项解析中启用运行时反射。`SelectedFilterValues` 是当前选中值集合，负责连接 VM、filter flyout checked state 和 collection view 过滤描述。`FilterDescriptions` 只由列过滤管线生成和回收，不直接承担 public 选中状态。

Pinned filter 状态由 `DataGrid` 单独拥有，并与过滤选择状态正交：

```text
DataGrid.IsPopupPinnedOpen
  -> first eligible column in DisplayIndex order
  -> DataGridColumnHeader.IsPopupPinnedOpen
  -> DataGridFilterIndicator.IsPopupPinnedOpen
  -> current filter Flyout.IsPopupPinnedOpen
  -> Popup.IsPopupPinnedOpen
```

目标筛选使用 `ColumnsInternal.GetDisplayedColumns()`，跳过 filler、不可见、不可过滤、无过滤项或无 Header 的列。同一时间
只有一个 Header 被 pin。列集合、DisplayIndex、可见性、`CanUserFilterColumns`、列级 `CanUserFilter` 和过滤项变化都会重新
计算目标；replacement 先 lifecycle-close 旧 Popup 并 unpin 旧 Header，再 pin 新 Header。Header 到 Indicator、Indicator 到
当前 Menu/Tree Flyout 的两个 `BindUtils.RelayBind` 分别由 `ClearFilterIndicator` 和 `ClearFlyout` 释放。

列绑定通过 `DataGridColumn.DataContext` 完成。`DataGridColumn` 实现 `IDataContextProvider`，列插入 `DataGridColumnCollection` 时复制当前 `DataGrid.DataContext`，`DataGrid.OnDataContextEndUpdate` 时向所有列同步新 `DataContext`，列移除或清空时释放为 `null`。这条 acquire/release 配对是 `Filters="{Binding NameFilters}"` 和 `SelectedFilterValues="{Binding SelectedNames}"` 可用的基础，也避免列持有旧 ViewModel。列订阅外部 `Filters` / `SelectedFilterValues` collection 时必须跟随列 attach/detach 注册和释放；过滤投影写入 `FilterDescriptions` 必须避开列集合插入/删除的中间态，等列集合索引、display index 和 current cell 状态稳定后再刷新 collection view。由用户操作、`Filter(...)` 或清除过滤触发的选中值更新，应优先修改现有可变 `SelectedFilterValues` 列表实例；只有当前没有可变列表时才替换属性值。这样双向绑定、代码侧赋值和 Gallery 示例接线都共享同一个列表 owner。

Gallery 或业务 XAML 常见写法会在 `DataGrid` 上用 `x:DataType` 声明行模型类型，以便 `Binding="{Binding Address}"` 这类单元格绑定被编译。此时列级 ViewModel 绑定不能只写裸 `{Binding NameFilters}`，否则 XAML 编译器可能按行模型解析。优先在列级绑定上显式指定 VM 类型；如果 IDE、XAML 编译器或模板嵌套让上下文仍然歧义，则在页面加载或 View 初始化时直接设置 `Filters` 与 `SelectedFilterValues`。这种代码侧接线只能替代 binding 表达式，不能引入第二套 selected/filter 状态，也不能绕过列过滤管线。

过滤状态同步必须避免循环：

- VM 修改 `SelectedFilterValues` 时，列过滤管线比较归一化后的值集合；值未变时不重建 `DataGridFilterDescription`。
- 用户操作 flyout 时，Presenter 只收集过滤值并提交给列；列先更新 `SelectedFilterValues`，再投影到 `FilterDescriptions`。
- `FilterDescriptions` 因 collection view 或清除 API 变化时，同步回 `SelectedFilterValues` 前必须判断来源，避免 clear / apply 重入。
- `Filters` 重置、替换或集合变更后需要重新物化 flyout，并通过同一管线剔除已不在有效叶子过滤项中的选中值。

行拖动状态流以 `DataGrid` 实例会话和 CollectionView 移动能力为 owner：

```text
DataGridRowReorderHandle.PointerPressed
  -> RowReorderSession(Pressed, pointer, row, item, view, sourceIndex)
  -> pointer distance > Constants.DragThreshold
  -> RowReordering
  -> revalidate owner / row / item / view / CanMove
  -> DataGridRowsPresenter.ShowDragIndicator(item)
  -> RowReorderSession(Dragging, targetIndex)

PointerReleased
  -> IDataGridCollectionViewMoveSupport.TryMove(sourceIndex, targetIndex)
  -> CancelDragSession / cleanup
  -> RowReordered (only when TryMove returned true and the view is still current)
```

会话状态只允许 `Idle -> Pressed -> Dragging -> Completed` 或
`Idle -> Pressed/Dragging -> Cancelled`。取消状态在对应 Pointer 释放前保持终止，不能回到 Pressed 或重复触发
`RowReordering`。Pointer、handle、row、item 和 CollectionView 必须全部匹配会话快照；任何公开事件或集合通知
返回后都重新验证，不依赖事件调用前的视觉容器或索引继续执行。

## 5. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册必要状态，不依赖 template part。
- 模板应用时获取 part、建立事件订阅和绑定，并先释放旧 part 订阅。
- 分页模板部件取得后，先从当前 `DataGridCollectionView` 回放总数、页大小和当前页，再订阅
  `CurrentPageChanged`。这一顺序防止属性回放产生的分页条件通知被误认为用户翻页请求。
- 控件卸载、弹层关闭、窗口关闭、集合替换或 container recycle 时释放事件订阅和资源宿主。
- DynamicResource、TokenResourceBinder 或 C# binding 必须有明确 owner 和释放点。
- Browser 和 Desktop 宿主下的主题加载顺序不得影响 public API 语义。
- `PART_ColumnHeadersPresenter` 与 `PART_GroupColumnHeadersPresenter` 在模板应用后必须接入同一列宽输入路径；
  `PART_RowPresenter` 在空数据时保持隐藏，不能作为完成 star 求解的必要生命周期节点。
- 空数据与有数据切换、表头模式切换或模板重套用时，新的 presenter 只接管几何输入，现有列宽 state owner
  仍是 DataGrid 和 DataGridColumn。
- 行拖动开始时由 handle 捕获并记录具体 Pointer；正常释放和 `PointerCaptureLost` 都进入同一个会话终止入口。
- `IsEnabled=false`、`CanUserReorderRows=false`、ItemsSource 或 CollectionView 变化、源行回收、重排列移除、
  模板重套用以及 DataGrid detach 必须主动取消当前行拖动，而不是等待 PointerReleased 补偿清理。
- `CancelDragSession` 先把会话标记为终止以阻止事件重入，再释放 capture、移除 ghost row、重置拖动偏移、
  恢复 transition，最后清除会话引用。会话清理必须幂等，允许 release、capture lost 和 detach 连续到达。
- 源行因自动滚动而被虚拟化回收时，会话立即取消；同一 PointerMoved 帧必须在滚动返回后重新检查会话，
  不得继续读取已经清空的坐标、row 或 presenter 状态。
- Pinned filter 的 Header、Indicator 或 Flyout replacement 先失效旧 Loaded-priority callback，再 lifecycle-close 旧 Popup、
  dispose relay 和事件订阅。Indicator callback 校验 generation、pin、attach、effective enabled/visible、OwningGrid、TopLevel
  和 Flyout identity，旧模板或旧列不能在 teardown 后复活。
- DataGrid unpin 只清除当前 Header pin，不关闭已经打开的 filter Flyout；DataGrid detach、template reapply、禁用或本地隐藏
  则清空目标并走 lifecycle close。Header/Indicator 单独 detach 依赖各自 teardown 和 Popup target tracking，重新 attach 时
  由仍然有效的 DataGrid pin 请求重新创建 Flyout shell 并打开。

稳定 template part 接入点：

- `PART_Ascending`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_BottomGridLine`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_BottomPagination`：底部分页状态投影；由 DataGrid 管理状态回放和翻页事件订阅。
- `PART_ColumnHeadersPresenter`：普通表头内容测量与空数据有限列视口输入。
- `PART_ContentFrame`：承载根视觉、边框、背景或尺寸基线。
- `PART_ContentPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_Descending`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_FocusVisual`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_Frame`：承载根视觉、边框、背景或尺寸基线。
- `PART_GroupColumnHeadersPresenter`：分组表头组合测量，与普通表头共享列宽输入契约。
- `PART_HeaderPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_HorizontalIndicator`：展示指示器、进度、分页或状态反馈。
- `PART_IndicatorIconButton`：承载用户触发入口、导航或关闭动作。
- `PART_ItemsPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_RightGridLine`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_RootLayout`：承载根视觉、边框、背景或尺寸基线。
- `PART_RowPresenter`：已物化行布局入口；空数据时隐藏，不作为 star 求解的必要生命周期节点。
- `PART_SortIndicator`：展示指示器、进度、分页或状态反馈。
- `PART_TopPagination`：顶部分页状态投影；由 DataGrid 管理状态回放和翻页事件订阅。
- `PART_VerticalIndicator`：展示指示器、进度、分页或状态反馈。
- `PART_VerticalSeparator`：稳定模板协作入口，重命名前必须同步主题和实现。

## 6. 交互与事件处理

DataGrid 的交互事件应从输入源收敛到控件级语义事件：

- Pointer、keyboard、focus 和 command 事件不应绕过 Avalonia 基础控件语义。
- 没有弹层职责的路径不应引入额外 popup 或全局输入捕获。
- 集合类路径必须稳定处理 container prepare、clear、过滤、分组和虚拟化回收。
- 值提交或命令触发必须保持继承控件的事件顺序。

稳定事件路径包括 `SelectionChanged`、`RowReordering` 和 `RowReordered`。事件参数和触发时机属于兼容边界。

行重排事件路径遵循以下顺序：

- `RowReordering` 仅在 Pointer 移动超过 `Constants.DragThreshold` 后触发一次，并且发生在 ghost row 创建和数据提交前。
- `RowReordering` 返回后重新确认 DataGrid、row、item、CollectionView 和 `CanMove`；回调导致任一 owner 变化时取消会话。
- `RowReordered` 只表示 CollectionView 已经产生实际顺序变化。目标为空、同位置释放、取消、能力不足、
  会话失效或移动失败都不触发该事件。
- `RowReordered` 在 capture、ghost、offset、transition 和会话引用清理后触发，事件处理器可以安全替换 ItemsSource、
  刷新 View 或移除重排列。

## 7. 内部算法与关键流程

维护者需要重点关注以下流程：

- API 默认值到 effective state 的归一。
- Template part 重新应用时的状态回放。
- 主题资源、Token 和 SharedToken 计算后的视觉更新。
- ItemsSource、selection、checked、expanded、filter、paging 或 upload task 的集合同步。
- 动效启停、初始加载阶段 transition 抑制和卸载取消。

实现文档不逐行解释私有方法。若某个私有算法成为稳定维护入口，应在本节补充算法不变量，而不是把代码复述为说明书。

分页同步不变量：

- 分页部件的 `Total` 来自 `DataGridCollectionView.ItemCount`，`PageSize` 来自 CollectionView 接受后的
  `PageSize`，不能分别从不同状态源读取。
- 分页部件使用一基页码；CollectionView 使用零基 `PageIndex`。当 `PageIndex < 0` 时投影为
  `Pagination.DefaultCurrentPage`，否则投影为 `PageIndex + 1`。
- 模板状态回放必须发生在新分页部件订阅 `CurrentPageChanged` 之前；旧部件必须先解绑，避免重新套模板后
  旧视觉对象继续发出翻页请求。
- 运行期用户翻页只通过 `MoveToPage` 修改 CollectionView；CollectionView 的 `PageChanging` 再把同一目标页
  投影到上下两个分页部件，确保双分页显示一致。
- `PageSize = 0` 时由 `EffectivePaginationVisibility` 隐藏分页区域；位置可见性不能代替分页状态初始化。

Frame 与 Header 圆角不变量：

- `FrameCornerRadius` 是根外框 `Frame` 的派生圆角，只表达整张表外壳裁剪。`IsFrameBorderVisible=false` 时只保留顶部圆角，底部圆角必须为 `0`，避免最后一根横向分割线被根裁剪成短线；`IsFrameBorderVisible=true` 时使用完整 `CornerRadius`，由外边框承担整表圆角视觉。
- `FrameBorderThickness` 是根外框 `Frame` 的派生边框厚度。`IsFrameBorderVisible=false` 时为 `0`；`IsFrameBorderVisible=true` 时必须使用完整 `BorderThickness`，不能因为存在横向行分割线而去掉底边框，否则底部圆角边框会缺失。
- `FrameContentClip` 是 `Frame` 内部的内容裁剪层，必须和 `FrameCornerRadius` 保持一致，用于阻止行背景、分页、Footer 或加载态内容进入外框圆角区域并遮挡边框。不要把这层裁剪合并到 rows presenter 或单个 row 上，否则空数据、Footer、滚动条和加载态会出现不同的裁剪规则。
- `PART_BottomGridLine` 和行头横向分割线只表达行间分隔，不表达整表外轮廓。`IsFrameBorderVisible=true` 且 rows 区域直接贴住 Frame 底边时，最后一个 displayed row 必须隐藏底部分割线，由 Frame 底边承担唯一底线；存在 `Footer`、底部分页或水平滚动条时，rows 区域下方还有内容，最后一行分割线必须恢复显示。
- `HeaderCornerRadius` 只表达表头容器圆角。它根据 `Title`、`HeadersVisibility` 和 `CornerRadius` 派生，不应被根外框复用。

列宽分配不变量：

- 有限列视口宽度必须通过 DataGrid 的共享入口参与求解；普通列头、分组列头和 rows/cells 路径不得复制
  `adjustment = availableCellsWidth - VisibleEdgedColumnsWidth` 之后的调整逻辑。
- `AutoSizingColumns` 只覆盖初始内容测量期。完成初始测量时先固定当前已知的 desired widths，再使用同一次布局
  提供的有限宽度执行 star 分配，最后重新 measure header/cell。
- 空数据时，当前可见列头 presenter 是有限列视口的权威输入；`SizeToCells` 保持约束基线，`Auto` 使用 header
  结果，star 列仍必须吸收可分配的剩余空间。
- 有已物化行时继续使用 `CellsWidth`，保证行头、滚动条和横向滚动语义与现有布局一致。
- `AdjustColumnWidths` 继续统一处理增长、收缩、star 权重、min/max 和用户调整约束。所有 star 列达到
  `MaxWidth` 后仍存在的正剩余空间才允许进入 filler。
- 无限宽度不执行有限剩余空间分配，保持既有 star 退化规则。

列过滤算法不变量：

- `Filters` 替换或集合变更时，Header 过滤入口可见性、FilterIndicator 激活态和 flyout 内容必须来自同一份有效过滤项视图，并同步剪枝 `SelectedFilterValues`。
- Flyout 物化菜单或树节点时，应按 `SelectedFilterValues` 初始化 checked state；不能只依赖当前 presenter 内部状态。
- `SelectedFilterValues` 写入 `FilterDescriptions` 时应保留过滤值原始类型，不能提前转成字符串；默认文本匹配只在默认 evaluator 中发生。
- 单选模式只允许一个有效过滤值进入 `SelectedFilterValues`；多选模式保持集合顺序稳定，但比较时按集合值语义去重。
- 清除过滤通过清空 `SelectedFilterValues` 进入同一状态管线，最终移除对应 `DataGridFilterDescription` 并刷新图标激活态。

行拖动算法不变量：

- PointerPressed 只创建 Pressed 会话；Pointer 移动距离超过 `Constants.DragThreshold` 后才允许进入 Dragging。
- 拖动目标索引来自当前显示 `DataGridRow.Index`，但只作为 CollectionView 的 View 索引传递，不能直接作为源
  `IList` 索引使用。
- 顶部自动滚动量限制在 `[-VerticalScrollBar.Value, 0]`，底部自动滚动量限制在
  `[0, VerticalScrollBar.Maximum - VerticalScrollBar.Value]`。ghost offset 只能使用实际采用的滚动量。
- `IDataGridCollectionViewMoveSupport.CanMove` 是开始拖动和提交前的双重能力门。内置 `DataGridCollectionView`
  仅在源集合可写、非只读、非固定长度、没有新增或编辑事务、没有排序/过滤/分组/分页且未延迟刷新时返回 true。
- `TryMove` 的两个索引都相对当前 View，范围为 `[0, Count)`；索引相同返回 false。内置平面 View 的源索引与
  View 索引相同，移动时按源索引执行 `RemoveAt`，不能通过 `Remove(item)` 的值相等语义定位源对象。
- 内置移动事务抑制自身对中间 Remove/Add 通知的普通处理，并验证预期项目身份、源集合数量和通知序列。
  检测到额外集合重入时中止提交并执行可行回滚；提交成功后统一重建 View 并发出稳定的集合重置信号。
- Insert 或通知处理失败时，移动事务尝试把原项目恢复到原索引并恢复 CollectionView 的处理标志；原始异常在
  拖拽会话清理后继续向上传递。回滚本身失败时必须保留原始异常上下文，不能把数据异常转换成成功返回。
- CollectionView 在 `TryMove` 期间被替换时，旧 View 可以完成已经开始的数据事务，但 DataGrid 不再向新 View
  投射 ghost、目标索引或 `RowReordered`。

## 8. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。
- 行拖动 PointerMoved 热路径只更新会话坐标、目标索引、ghost offset 和必要的自动滚动请求；不在移动帧修改
  集合、刷新 View、重建模板或分配新的 ghost row。
- 每次有效 PointerPressed 最多创建一个轻量行拖动会话，每次进入 Dragging 最多创建一个 ghost row；两者在
  完成或取消时释放。移动能力通过直接接口能力判断，不使用反射、动态调用或运行时类型扫描。
- 列宽求解复用列集合可见宽度缓存和 `AdjustColumnWidths`；无 star 列、输入无限、adjustment 为零或初始 Auto
  测量未完成时应直接退出，不在 presenter 中分配辅助集合或建立额外订阅。
- Pinned filter 目标选择直接遍历 displayed columns，不做反射、runtime type discovery 或全视觉树扫描；两级 relay 只在
  当前 Header/Indicator/Flyout 生命周期内存在，Loaded callback 由 generation 合并和失效。

## 9. 维护不变量

维护 DataGrid 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 列过滤只能有一个选中状态 owner；`Filters`、flyout checked state、`SelectedFilterValues` 和 `FilterDescriptions` 之间不得形成互相覆盖的并行状态源。
- Pinned filter 同一时间只能有一个目标，目标必须按 DisplayIndex 选择第一个 eligible column；Header -> Indicator -> Flyout
  relay 必须在 replacement、container clear、detach 和 template reapply 时对称释放。
- DataGrid unpin 不关闭已打开 filter Flyout；目标 replacement 或 lifecycle teardown 必须关闭旧 Popup，且旧 Loaded callback
  不得重新打开已释放的 Flyout。
- 行拖动只能有一个 DataGrid 实例级会话 owner；禁止在 handle 类型上保存 static Pointer、row、index、bounds、
  offset 或 owner 状态。
- Handle、RowsPresenter 和 CollectionView 的职责不能重新混合：handle 不修改数据，presenter 不决定移动语义，
  CollectionView 不持有视觉对象。
- 所有行拖动终止路径都必须移除 ghost、释放 capture 并清空会话；`RowReordered` 不能用于通知未提交的拖动。
- 列宽求解不能依赖 `DataGridRowsPresenter` 可见性；空数据、普通表头和分组表头必须共享 DataGrid-owned solver。
- filler 不能掩盖未执行的 star 分配；star 可吸收剩余空间时 filler 宽度必须为零。
- 过滤项解析必须支持业务 DTO 和 `DataGridFilterItem` 两类输入，不得要求 VM 反向依赖内部 flyout、menu item 或 tree item 类型；业务 DTO 必须有生成的 data member accessor，不在 AOT 敏感路径中使用运行时反射兜底。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。

## 10. 测试与验证

推荐验证：

- 纯文档改动运行 `git diff --check` 并检查相对链接。
- 控件 API 或行为变更运行对应 `tests/AtomUI.Desktop.Controls.Tests` 或专用包测试。
- DataGrid 相关变更运行 `tests/AtomUI.Desktop.Controls.DataGrid.Tests`。
- Gallery 示例或源码片段变更运行 `tests/AtomUIGallery.Tests`。
- AOT、生成器或动态数据路径变更按 Gallery NativeAOT 发布流程验证。
- 行重排状态测试覆盖两个 DataGrid 的会话隔离、PointerCaptureLost、禁用和 detach 清理、拖动阈值、取消去重、
  RowReordering 重入、自动滚动虚拟化回收和顶部/底部滚动边界。
- CollectionView 移动测试覆盖普通可变列表、数组、只读或固定长度集合、普通 IEnumerable、编辑和新增状态、
  排序/过滤/分组/分页拒绝、同位置释放、null 项目、重复 Equals 项目、自定义移动 View、异常回滚和额外集合重入。
- 事件测试确认 `RowReordering` 每个 Pointer 会话最多一次，`RowReordered` 只在成功提交和完整清理之后一次触发。
- 列宽测试覆盖空数据下 `Auto + * + *`、全 star、`Pixel` / `SizeToHeader` / `SizeToCells` 与 star 组合、
  `1* + 2*`、空视口 resize、空数据新增后再次清空，以及普通/分组表头一致性。
- 列宽约束测试覆盖 min/max、冻结列、行头、滚动条和 filler：star 可吸收空间时 filler 为零，只有约束阻止
  继续分配时才允许 filler 为正。
- `DataGridFilterDialogPopupTests` 覆盖按 DisplayIndex 选择唯一 pinned filter、普通 Hide 拦截、detach/reattach、目标列失效
  replacement、Menu/Tree presenter mode replacement 和 unpin 后保持已打开状态。
