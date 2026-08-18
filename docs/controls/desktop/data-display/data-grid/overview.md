# DataGrid 桌面版架构设计

本文档定义 `DataGrid` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/development/control-development-guidelines.md)，内部实现原理见 [DataGrid 桌面版实现原理](implementation.md)，列宽测量与分配见 [DataGrid 列宽分配设计](column-sizing-design.md)，DataGrid Token 的专项设计见 [DataGrid Token 设计](token.md)，设计和契约变化记录见 [DataGrid Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls.DataGrid` |
| .NET 命名空间 | `AtomUI.Desktop.Controls.DataGrid` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid` |
| 控件状态 | Stable |

DataGrid 是 AtomUI 桌面控件体系中的数据表格控件，用于列模型、行选择、排序、过滤、编辑、冻结列和分页展示。

DataGrid 不负责简单列表、树视图或业务数据访问层。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls.DataGrid`

## 2. 设计语言

DataGrid 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | DataGrid 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | DataGrid 是 AtomUI 桌面控件体系中的数据表格控件，用于列模型、行选择、排序、过滤、编辑、冻结列和分页展示。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `AutoGenerateColumns`、`CanUserFilterColumns`、`CanUserReorderColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`CellEditingTemplate`、`CellTemplate` 等 32 项。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、collection/filter、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、Control Own Token 和模板绑定如何表达视觉。 | DataGrid Token + ControlTheme。 |

## 3. API 与契约模型

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

DataGrid 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

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

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme、控件文档和 Gallery ShowCase 示例保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 DataGrid 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- `IDataGridCollectionViewMoveSupport` 是独立的 opt-in 能力接口；不得把成员直接追加到
  `IDataGridCollectionView`，避免破坏已有自定义 View 的二进制和源码兼容性。
- 普通可变平面列表的拖动结果保持现有顺序语义；排序、过滤、分组、分页、编辑、只读或固定长度数据源在没有
  专用移动能力时必须安全拒绝，不能退化为错误移动、部分提交或完成事件假成功。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 选择与当前项模型

DataGrid 的当前项状态必须由单一 owner 推导。public 选择属性、集合项容器和伪类之间只能做单向同步，集合替换、清空和模板重套用时必须回放当前状态。

### 8.2 集合与数据同步模型

DataGrid 的集合状态必须能处理 source replace、reset、clear 和 container recycle。业务数据对象不应反向持有视觉对象，虚拟化或懒创建路径必须在容器回收时清理旧状态。

### 8.3 行拖动重排模型

行拖动重排采用“控件会话负责交互、CollectionView 负责数据提交”的单向模型：

```text
PointerPressed
  -> DataGrid row reorder session (Pressed)
  -> DragThreshold
  -> RowReordering
  -> session validation
  -> RowsPresenter ghost row (Dragging)
  -> IDataGridCollectionViewMoveSupport.TryMove
  -> session cleanup
  -> RowReordered
```

每个 `DataGrid` 同时最多拥有一个行拖动会话。会话跟踪 Pointer、源 handle、源 row、源项目、开始时的
CollectionView、源 View 索引、目标 View 索引和拖动几何。Pointer 不匹配、capture 丢失、控件禁用、
`CanUserReorderRows=false`、ItemsSource 或 CollectionView 替换、行回收、重排列移除、模板重建或 DataGrid detach
都会取消会话。取消和完成共用同一个清理边界，必须移除 ghost row、恢复动效、释放 capture 并清除会话引用。

移动提交由 CollectionView 维护自己的数据和通知。DataGrid 不推断自定义 View 的源集合结构；CollectionView
不支持移动时，handle 不进入可提交的 Dragging 状态。源集合在移动过程中抛出异常时，View 先执行可行的回滚并
恢复自己的通知状态，DataGrid 再无条件清理视觉会话；异常不允许留下 ghost row 或可复用的旧索引。

### 8.4 列过滤模型

列过滤模型由四层组成：

```text
Column.Filters / Filter*MemberPath
  -> 过滤项视图模型
  -> Column.SelectedFilterValues
  -> DataConnection.FilterDescriptions
```

`Filters` 只表达候选过滤项来源，`SelectedFilterValues` 只表达当前选择，二者不能互相替代。`DataGridFilterIndicator`、菜单 flyout 和树形 flyout 是过滤状态的视觉投影，不拥有业务状态。它们在打开或重新物化时从 `SelectedFilterValues` 初始化 checked state，在确认、关闭或选择变化时把结果写回 `SelectedFilterValues`。

`DataConnection.FilterDescriptions` 是 collection view 执行过滤所需的内部描述集合。列过滤管线负责根据 `SelectedFilterValues` 创建、替换或移除当前列对应的 `DataGridFilterDescription`。外部 collection view 状态变化需要同步回列级选中值时，必须通过同一归一化逻辑处理，避免过滤图标激活态、菜单 checked state 和 VM 绑定值不一致。

`FilterEvaluator` 用于表达列过滤谓词。默认谓词应按选中值集合判断当前单元格值是否匹配；复杂场景通过显式 evaluator 扩展，不应把字符串 `Contains` 作为所有过滤值类型的唯一默认语义。

### 8.5 列宽分配模型

列宽采用“presenter 提供几何输入，DataGrid 统一求解，所有视觉消费同一结果”的单向模型：

```text
header / rows / cells available width and content measure
  -> DataGrid column sizing state
  -> constrained Auto completion and star distribution
  -> DataGridColumn display widths
  -> header / row / cell / filler / scrollbar projection
```

有已物化行时，rows/cells 路径提供包含行头和滚动布局修正后的 `CellsWidth`；空数据时，当前可见的普通或分组
列头 presenter 使用自己的有限测量宽度驱动同一个求解器。`AutoSizingColumns` 只表示初始内容测量阶段，完成
Auto 测量和分配 star 剩余空间是两个独立职责。filler 只承载所有可调整列达到 min/max 约束后仍无法分配的正
剩余空间，不能替代 star 列分配。

### 8.6 动效模型

DataGrid 的动效只表达状态变化反馈，不应改变 public API 语义。初始加载、禁用态和卸载路径应能抑制或取消动效，避免保留旧控件实例。

### 8.7 视觉选项模型

DataGrid 的视觉选项通过 public API 归一为 theme variables、伪类或模板绑定。Token 保存组件语义值，不能保存实例运行时状态或业务色值。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [DataGrid 桌面版实现原理](implementation.md)
- [DataGrid 列宽分配设计](column-sizing-design.md)
- [DataGrid Token 设计](token.md)
- [DataGrid Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `DataGrid` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + `implementation.md` + `column-sizing-design.md` + `token.md` + Gallery ShowCase | 生成 `controls/data-grid/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + `column-sizing-design.md` + theme/template 信息 | 生成 `controls/data-grid/semantic-cn.md` |
| API 表 | overview.md 语义摘要 + 源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | token.md、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 selection/checked/active、collection/filter、motion、visual option、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Token 类型、生成数据和 token.md和文档同步。 |
| Gallery | 走查对应 ShowCase 示例和源码片段入口。 |
