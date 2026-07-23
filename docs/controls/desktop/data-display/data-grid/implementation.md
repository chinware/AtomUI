# DataGrid 桌面版实现原理

本文档描述 DataGrid 桌面版的内部实现范围、源码职责、状态流、生命周期、资源边界和维护规则。公共设计与 API 契约见 [DataGrid 桌面版架构设计](overview.md)，变化记录见 [DataGrid Changelog](changelog.md)。涉及组件 Token 的实现应同时阅读 [DataGrid Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 DataGrid 的控件实现、主题接入、状态同步和 Gallery 可见维护边界。具体属性注册、默认值、绘制细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls.DataGrid`：18 个文件，代表文件 `AtomUIDataGridThemesProvider.axaml`、`AtomUIDataGridThemesProvider.cs`、`DataGrid.Cells.cs`、`DataGrid.Columns.cs`、`DataGrid.Privates.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Cell`：4 个文件，代表文件 `DataGridCell.cs`、`DataGridCellCollection.cs`、`DataGridCellCoordinates.cs`、`DataGridCellsPresenter.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/Column`：31 个文件，代表文件 `DataGridAbstractTextColumn.cs`、`DataGridBoundColumn.cs`、`DataGridCheckBoxColumn.cs`、`DataGridColumn.Privates.cs`、`DataGridColumn.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Column/Filters`：7 个文件，代表文件 `DataGridFilterIndicator.cs`、`DataGridFilterItem.cs`、`DataGridFilterValuesSelectedEventArgs.cs`、`DataGridMenuFilterFlyout.cs`、`DataGridMenuFilterFlyoutPresenter.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/Data`：15 个文件，代表文件 `CollectionViewGroupRoot.cs`、`DataGridCollectionView.cs`、`DataGridCollectionViewGroup.cs`、`DataGridCollectionViewGroupInternal.cs`、`DataGridCurrentChangingEventArgs.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/EventArgs`：18 个文件，代表文件 `DataGridAutoGeneratingColumnEventArgs.cs`、`DataGridBeginningEditEventArgs.cs`、`DataGridCellEditEndedEventArgs.cs`、`DataGridCellEditEndingEventArgs.cs`、`DataGridCellEventArgs.cs` 等。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.LanguageGenerator`：2 个文件，代表文件 `LanguageProviderPool.g.cs`、`LanguageResourceConst.g.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.ResourceHost.ScopedResourceHostGenerator`：1 个文件，代表文件 `GenerateScopedResourceHostAttribute.g.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.TokenResourceKeyGenerator`：2 个文件，代表文件 `ControlTokenTypePool.g.cs`、`TokenResourceConst.g.cs`。
- `src/AtomUI.Desktop.Controls.DataGrid/Localization`：3 个文件，代表文件 `en_US.cs`、`zh_CN.cs`、`zh_TW.cs`。
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
- `ControlTokenTypePool`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
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

列过滤状态流以列对象为状态 owner：

```text
DataGridColumn.Filters
  -> DataGridFilterIndicator materialized items
  -> DataGridColumn.SelectedFilterValues
  -> DataConnection.FilterDescriptions
  -> CollectionView refresh / filter icon active state
```

`Filters` 是过滤候选项数据源，必须允许替换、绑定和集合变更通知。过滤项可以来自 `DataGridFilterItem`，也可以来自业务 DTO；解析文本、值和 children 时优先使用列上的 member path 配置，避免把 Gallery 示例对象变成业务层必须依赖的模型。DTO member path 解析只允许生成 accessor 路径，不在过滤项解析中启用运行时反射。`SelectedFilterValues` 是当前选中值集合，负责连接 VM、filter flyout checked state 和 collection view 过滤描述。`FilterDescriptions` 只由列过滤管线生成和回收，不直接承担 public 选中状态。

列绑定通过 `DataGridColumn.DataContext` 完成。`DataGridColumn` 实现 `IDataContextProvider`，列插入 `DataGridColumnCollection` 时复制当前 `DataGrid.DataContext`，`DataGrid.OnDataContextEndUpdate` 时向所有列同步新 `DataContext`，列移除或清空时释放为 `null`。这条 acquire/release 配对是 `Filters="{Binding NameFilters}"` 和 `SelectedFilterValues="{Binding SelectedNames}"` 可用的基础，也避免列持有旧 ViewModel。列订阅外部 `Filters` / `SelectedFilterValues` collection 时必须跟随列 attach/detach 注册和释放；过滤投影写入 `FilterDescriptions` 必须避开列集合插入/删除的中间态，等列集合索引、display index 和 current cell 状态稳定后再刷新 collection view。由用户操作、`Filter(...)` 或清除过滤触发的选中值更新，应优先修改现有可变 `SelectedFilterValues` 列表实例；只有当前没有可变列表时才替换属性值。这样双向绑定、代码侧赋值和 Gallery 示例接线都共享同一个列表 owner。

Gallery 或业务 XAML 常见写法会在 `DataGrid` 上用 `x:DataType` 声明行模型类型，以便 `Binding="{Binding Address}"` 这类单元格绑定被编译。此时列级 ViewModel 绑定不能只写裸 `{Binding NameFilters}`，否则 XAML 编译器可能按行模型解析。优先在列级绑定上显式指定 VM 类型；如果 IDE、XAML 编译器或模板嵌套让上下文仍然歧义，则在页面加载或 View 初始化时直接设置 `Filters` 与 `SelectedFilterValues`。这种代码侧接线只能替代 binding 表达式，不能引入第二套 selected/filter 状态，也不能绕过列过滤管线。

过滤状态同步必须避免循环：

- VM 修改 `SelectedFilterValues` 时，列过滤管线比较归一化后的值集合；值未变时不重建 `DataGridFilterDescription`。
- 用户操作 flyout 时，Presenter 只收集过滤值并提交给列；列先更新 `SelectedFilterValues`，再投影到 `FilterDescriptions`。
- `FilterDescriptions` 因 collection view 或清除 API 变化时，同步回 `SelectedFilterValues` 前必须判断来源，避免 clear / apply 重入。
- `Filters` 重置、替换或集合变更后需要重新物化 flyout，并通过同一管线剔除已不在有效叶子过滤项中的选中值。

## 5. 生命周期与模板接入

生命周期规则：

- 构造阶段只注册必要状态，不依赖 template part。
- 模板应用时获取 part、建立事件订阅和绑定，并先释放旧 part 订阅。
- 分页模板部件取得后，先从当前 `DataGridCollectionView` 回放总数、页大小和当前页，再订阅
  `CurrentPageChanged`。这一顺序防止属性回放产生的分页条件通知被误认为用户翻页请求。
- 控件卸载、弹层关闭、窗口关闭、集合替换或 container recycle 时释放事件订阅和资源宿主。
- DynamicResource、TokenResourceBinder 或 C# binding 必须有明确 owner 和释放点。
- Browser 和 Desktop 宿主下的主题加载顺序不得影响 public API 语义。

稳定 template part 接入点：

- `PART_Ascending`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_BottomGridLine`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_BottomPagination`：底部分页状态投影；由 DataGrid 管理状态回放和翻页事件订阅。
- `PART_ContentFrame`：承载根视觉、边框、背景或尺寸基线。
- `PART_ContentPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_Descending`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_FocusVisual`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_Frame`：承载根视觉、边框、背景或尺寸基线。
- `PART_HeaderPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_HorizontalIndicator`：展示指示器、进度、分页或状态反馈。
- `PART_IndicatorIconButton`：承载用户触发入口、导航或关闭动作。
- `PART_ItemsPresenter`：展示用户内容、文本、图标或模板化数据。
- `PART_RightGridLine`：稳定模板协作入口，重命名前必须同步主题和实现。
- `PART_RootLayout`：承载根视觉、边框、背景或尺寸基线。
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

稳定事件路径包括 `SelectionChanged`。事件参数和触发时机属于兼容边界。

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

列过滤算法不变量：

- `Filters` 替换或集合变更时，Header 过滤入口可见性、FilterIndicator 激活态和 flyout 内容必须来自同一份有效过滤项视图，并同步剪枝 `SelectedFilterValues`。
- Flyout 物化菜单或树节点时，应按 `SelectedFilterValues` 初始化 checked state；不能只依赖当前 presenter 内部状态。
- `SelectedFilterValues` 写入 `FilterDescriptions` 时应保留过滤值原始类型，不能提前转成字符串；默认文本匹配只在默认 evaluator 中发生。
- 单选模式只允许一个有效过滤值进入 `SelectedFilterValues`；多选模式保持集合顺序稳定，但比较时按集合值语义去重。
- 清除过滤通过清空 `SelectedFilterValues` 进入同一状态管线，最终移除对应 `DataGridFilterDescription` 并刷新图标激活态。

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

## 9. 维护不变量

维护 DataGrid 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 列过滤只能有一个选中状态 owner；`Filters`、flyout checked state、`SelectedFilterValues` 和 `FilterDescriptions` 之间不得形成互相覆盖的并行状态源。
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
