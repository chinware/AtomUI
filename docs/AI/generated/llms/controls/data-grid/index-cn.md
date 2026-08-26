# DataGrid

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

DataGrid 是 AtomUI 桌面控件体系中的数据表格控件，用于列模型、行选择、排序、过滤、编辑、冻结列和分页展示。

DataGrid 不负责简单列表、树视图或业务数据访问层。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls.DataGrid`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls.DataGrid` |
| .NET 命名空间 | `AtomUI.Desktop.Controls.DataGrid` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid` |
| 状态 | Stable |

## 何时使用

DataGrid 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | DataGrid 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | DataGrid 是 AtomUI 桌面控件体系中的数据表格控件，用于列模型、行选择、排序、过滤、编辑、冻结列和分页展示。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `AutoGenerateColumns`、`CanUserFilterColumns`、`CanUserReorderColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`CellEditingTemplate`、`CellTemplate` 等 32 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、collection/filter、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、Control Own Token 和模板绑定如何表达视觉。 | DataGrid Token + ControlTheme。 |

## 公共 API

DataGrid 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AutoGenerateColumns`、`CanUserFilterColumns`、`CanUserReorderColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`CellEditingTemplate`、`CellTemplate`、`ColumnHeaderHeight`、`ContentHeight` 等 32 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `ClipboardCopyMode`、`CurrentSortDirection`、`Filters`、`SelectedFilterValues`、`FilterPresenterMode`、`FilterSelectionMode`、`FilterApplyMode`、`Index`、`IsFilterActivated`、`IsHideOnSinglePage`、`IsHoverMode`、`IsSelected`、`IsSorterTooltipVisible` 等 | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `AscendingIndicatorVisible`、`DescendingIndicatorVisible`、`IsDeleteEnabled`、`IsDetailsVisible`、`IsEditEnabled`、`IsFrameBorderVisible`、`IsFrozen`、`IsLeaf`、`IsMotionEnabled`、`IsOperating` 等 19 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `BottomPaginationAlign`、`ColumnWidth`、`HorizontalAlignment`、`HorizontalScrollBarVisibility`、`MaxColumnWidth`、`MinColumnWidth`、`RowHeight`、`SeparatorBrush`、`SizeType`、`SublevelIndent` 等 14 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `CellTheme`、`CollectionView`、`CustomOperatingIndicator`、`EmptyIndicator`、`Footer`、`FormatString`、`GridLinesVisibility`、`Level`、`Maximum`、`Minimum` 等 15 项 | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `SelectionChanged`、`RowReordering` 和 `RowReordered`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

行拖动重排由 `CanUserReorderRows`、`DataGridRowReorderColumn`、`RowReordering`、`RowReordered` 和可选的
`IDataGridCollectionViewMoveSupport` 共同表达。`DataGrid` 不直接把 View 索引解释为源集合索引，也不直接对
`IList` 执行 `RemoveAt` / `Insert`。CollectionView 只有在实现移动能力接口并声明 `CanMove=true` 时才参与提交：

```csharp
public interface IDataGridCollectionViewMoveSupport
{
    bool CanMove { get; }

    bool TryMove(int sourceIndex, int targetIndex);
}
```

`sourceIndex` 和 `targetIndex` 都使用当前 CollectionView 的零基索引。`TryMove` 只有在项目顺序发生实际变化并且
提交成功时返回 `true`；同位置释放、能力缺失、索引失效或会话在提交前失效时返回 `false`，且不触发
`RowReordered`。内置 `DataGridCollectionView` 只在源集合可写、非只读、非固定长度、未处于新增或编辑状态，
并且没有排序、过滤、分组、分页或延迟刷新时提供移动能力。自定义 CollectionView 可以通过实现该可选接口
定义自己的索引映射和提交语义，而不需要改变已有 `IDataGridCollectionView` 实现。

分页公共契约由 `PageSize`、`PaginationVisibility`、`TopPaginationAlign`、`BottomPaginationAlign` 和
`IsHideOnSinglePage` 共同表达。`PageSize` 默认为 `0`，表示不启用内建分页；非零值配置当前
`DataGridCollectionView` 的每页条数。`PaginationVisibility` 默认为 `Bottom`，只决定分页视觉投影出现的位置；
`TopPaginationAlign` 与 `BottomPaginationAlign` 默认均为 `End`。`IsHideOnSinglePage` 默认为 `false`，只在已经
启用分页后控制单页场景的有效可见性。实际总数、页大小和当前页由 `DataGridCollectionView` 持有，顶部和底部
`Pagination` 只投影该状态并把用户翻页请求交回 CollectionView。`ItemsSource`、分页属性与模板应用的先后顺序
不得改变分页结果；模板重新套用时必须从当前 CollectionView 回放状态，而不能把模板部件的默认值当成真源。

列宽公共契约由 `DataGrid.ColumnWidth`、`DataGridColumn.Width`、控件级与列级最小/最大宽度，以及
`DataGridLengthUnitType` 共同表达。`ColumnWidth` 默认为 `Auto`；单列可以使用 `Pixel`、`Auto`、
`SizeToHeader`、`SizeToCells` 或 `Star`。内容驱动模式先形成期望宽度，star 模式再按权重分配有限列视口中的
剩余空间。列宽状态与统一分配算法由 `DataGrid` 持有，普通表头、分组表头和 rows/cells presenter 只提供当前
布局可证明的有限宽度或内容测量结果。空数据时列宽求解不能依赖已隐藏的 rows presenter，完整契约见
[DataGrid 列宽分配设计](column-sizing-design.md)。

列过滤契约采用数据源与选中值分离的模型。`Filters` 是列过滤项数据源入口，应作为可绑定 Avalonia 属性维护，允许直接绑定 ViewModel 或数据库查询结果。`SelectedFilterValues` 是当前列过滤选中值的唯一 public 状态 owner，默认按双向绑定语义工作。`DataGridColumn` 实现 `IDataContextProvider`，列加入或离开 `DataGrid` 时由 `DataGrid` 同步/释放列级 `DataContext`，保证 `Filters="{Binding ...}"` 和 `SelectedFilterValues="{Binding ...}"` 能绑定到 Gallery 或业务 ViewModel。若同一个 `DataGrid` 通过 `x:DataType` 声明行模型类型，列级 ViewModel 绑定必须避免被行模型上下文捕获：可在 XAML 绑定上显式指定 VM 类型；若具体工具链无法稳定解析这种嵌套上下文，可在页面加载或 View 初始化时直接把 VM 集合赋给列属性，但仍必须复用 `SelectedFilterValues` 作为唯一状态 owner，不能另建并行选中状态。`DataGrid` 内部的 `FilterDescriptions` 只承载 collection view 过滤投影，不应成为列过滤菜单、VM 状态或 checked state 的并行 owner。

列过滤项不应强制用户构造 UI 专属对象。`Filters` 中的元素可以是 `DataGridFilterItem`，也可以是业务 DTO；当使用业务 DTO 时，通过 `FilterTextMemberPath`、`FilterValueMemberPath` 和 `FilterChildrenMemberPath` 声明展示文本、过滤值和树形子项路径。DTO 成员路径只走生成的 data member accessor，DTO 类型需要使用 `[GenerateDataMemberAccessors]` 或等价生成描述；内置过滤项解析不做运行时反射兜底。过滤值以 `object?` 作为语义类型，字符串只是默认文本匹配路径的一种输入，不应成为过滤值契约的硬限制。

过滤交互模式使用显式枚举表达：`FilterPresenterMode` 表达菜单或树形弹层，`FilterSelectionMode` 表达单选或多选，`FilterApplyMode` 表达确认、关闭或选择变化时应用过滤。枚举状态比布尔开关更适合作为长期 public API，因为它能把展示方式、选择方式和提交时机拆成三个正交状态，避免布尔组合在状态流中产生歧义。

主要公开类型与枚举：

- 类型：`DataGrid`、`DataGridAbstractTextColumn`、`DataGridAutoGeneratingColumnEventArgs`、`DataGridBeginningEditEventArgs`、`DataGridBoundColumn`、`DataGridCell`、`DataGridCellCollection`、`DataGridCellCoordinates`、`DataGridCellEditEndedEventArgs`、`DataGridCellEditEndingEventArgs` 等 public DataGrid 类型。
- 枚举：`DataGridClipboardCopyMode`、`DataGridEditAction`、`DataGridEditingUnit`、`DataGridFilterPresenterMode`、`DataGridFilterSelectionMode`、`DataGridFilterApplyMode`、`DataGridGridLinesVisibility`、`DataGridHeadersVisibility`、`DataGridLangResourceKind`、`DataGridLengthUnitType`、`DataGridPaginationVisibility`、`DataGridRowDetailsVisibilityMode` 等。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Ascending` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_BottomGridLine` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_BottomPagination` | `Pagination` | 投影 CollectionView 分页状态并转发底部翻页请求。 |
| `PART_ColumnHeadersPresenter` | `DataGridColumnHeadersPresenter` | 测量普通列头，并在空数据布局中提供有限列视口宽度。 |
| `PART_ContentFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_ContentPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_Descending` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_FocusVisual` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_Frame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_GroupColumnHeadersPresenter` | `DataGridGroupColumnHeadersPresenter` | 测量分组列头，并与普通列头共享列宽输入契约。 |
| `PART_HeaderPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_HorizontalIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_IndicatorIconButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ItemsPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_RightGridLine` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_RootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_RowPresenter` | `DataGridRowsPresenter` | 承载已物化行；空数据时保持隐藏，不作为 star 求解的必要前置。 |
| `PART_SortIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_TopPagination` | `Pagination` | 投影 CollectionView 分页状态并转发顶部翻页请求。 |
| `PART_VerticalIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_VerticalSeparator` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

控件专属或内部伪类包括 `DataGridPseudoClass.EmptyColumns`、`DataGridPseudoClass.EmptyRows`、`DataGridPseudoClass.FirstColumnHeader`、`DataGridPseudoClass.GroupItemLeaf`、`DataGridPseudoClass.LastColumnHeader`、`DataGridPseudoClass.MiddleColumnHeader`、`DataGridPseudoClass.RowHover`、`EmptyColumns=:empty-columns`、`EmptyRows=:empty-rows`、`FirstColumnHeader=:first-column-header`、`GroupItemLeaf=:is-leaf-item`、`LastColumnHeader=:last-column-header` 等 14 项。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

DataGrid 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `SelectionChanged`、`RowReordering` 和 `RowReordered`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。
- 类型：`DataGrid`、`DataGridAbstractTextColumn`、`DataGridAutoGeneratingColumnEventArgs`、`DataGridBeginningEditEventArgs`、`DataGridBoundColumn`、`DataGridCell`、`DataGridCellCollection`、`DataGridCellCoordinates`、`DataGridCellEditEndedEventArgs`、`DataGridCellEditEndingEventArgs` 等 public DataGrid 类型。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

- `controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml`

## 状态模型

DataGrid 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、collection/filter、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 列过滤状态以 `SelectedFilterValues` 为 owner：VM 更新它时重建当前列的 collection view 过滤投影并回放到 flyout checked state；用户在 flyout 中选择过滤项时先更新它，再由同一管线投影到 `FilterDescriptions`。
- `Filters` 替换、reset 或 clear 时，Header 和 FilterIndicator 必须重新计算过滤入口可见性并重新物化 flyout 内容；已有 `SelectedFilterValues` 只能保留仍能匹配到有效过滤项的值。
- `ClearFilters()` 和单列清除过滤必须通过清空列级 `SelectedFilterValues` 完成，不能只清空 `FilterDescriptions`，否则 VM 绑定、过滤图标激活态和 flyout 勾选态会分裂。
- 分页状态以当前 `DataGridCollectionView` 为 owner；顶部和底部分页部件必须从同一份 `ItemCount`、`PageSize`
  和 `PageIndex` 投影，不能互相覆盖，也不能在模板重建时反向重置 CollectionView。
- 行拖动状态以当前 `DataGrid` 的单一拖拽会话为 owner；handle 和 RowsPresenter 只投影输入与 ghost row，不能保存
  跨 DataGrid 共享的静态拖拽状态。Pointer capture、源行、CollectionView 和目标索引必须属于同一个会话。
- `RowReordering` 在超过拖动阈值后且创建 ghost row 前触发一次；事件取消或事件回调改变 DataGrid、源行、
  ItemsSource、CollectionView 或移动能力时，本次 Pointer 会话保持取消状态，不得在后续移动帧重复开始。
- `RowReordered` 只在 CollectionView 成功提交顺序变化，并且 ghost、capture、动画与会话状态全部清理后触发。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 主题与 Design Token

DataGrid 的视觉模型由控件模板、ControlTheme、SharedToken 和 DataGrid Own Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `DataGridCellTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridColumnGroupHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridColumnHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridHeaderViewItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridOperationButtons.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `DataGridRowExpanderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridRowGroupHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridRowHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridRowReorderHandle.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridRowTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DataGridTopLeftColumnHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridFilterIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DataGridFilterMenuItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridFilterTreeItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `DataGridMenuFilterFlyoutPresenterTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `DataGridSortIndicatorTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `DataGridTreeFilterFlyoutPresenterTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |

DataGrid 拥有独立 Control identity；`DataGridToken` 只表达 DataGrid Own Token 语义，不承载 selection/checked/active、collection/filter、motion 或 visual option 运行时状态。Control 级 Global Token 覆盖与 Own Token 通过 `DataGridTokenResource` 统一读取。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 来源：

DataGrid Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DataGridToken`，scope id 为 `DataGrid`，源码位于 `src/AtomUI.Desktop.Controls.DataGrid/DataGridToken.cs`。

## AOT 与裁剪注意事项

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

## 源码索引

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

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/data-grid/overview.md`
- 实现文档：`docs/controls/desktop/data-display/data-grid/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/data-grid/token.md`
- 变更记录：`docs/controls/desktop/data-display/data-grid/changelog.md`
- 语义结构：`./semantic-cn.md`
