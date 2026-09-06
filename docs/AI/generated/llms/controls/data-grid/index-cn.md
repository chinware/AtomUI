# DataGrid

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

DataGrid 是 AtomUI 桌面控件体系中的数据表格控件，用于列模型、行选择、排序、过滤、分组、编辑、冻结列、连续虚拟滚动和分页展示。本地集合与远端服务通过同一个范围数据源契约接入。

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
| 产品语义 | 控件在界面中承担的稳定职责。 | DataGrid 以不可变 Query 描述数据语义，以 Range Source 支持本地或远端数据，并只物化当前视口。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `ItemsSource : IDataGridSource?`、`Query`、`GroupExpansion`、列 `FieldId`、`CellTemplate` 和 `CellEditingTemplate`。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | AppliedQuery、LoadState、Selection、CurrentRowKey、sort/filter/group projection、motion 和 visual option。 |
| 主题语义 | ControlTheme、SharedToken、Control Own Token 和模板绑定如何表达视觉。 | DataGrid Token + ControlTheme。 |

## 公共 API

DataGrid 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ItemsSource`、`Query`、`AppliedQuery`、`GroupExpansion`、`TotalItemCount`、`TotalEntryCount`、`AutoGenerateColumns`、`CellTemplate`、`CellEditingTemplate` | 定义数据输入、查询、范围 presentation、模板和业务对象入口；`ItemsSource` 的类型是 `IDataGridSource?`。 |
| 选择与当前项 | `Selection`、`CurrentRowKey`、`SelectionChanged`、`ClipboardCopyMode` | 以稳定 row key、query scope 和 index interval 维护可跨 range/page 的声明式状态。 |
| 交互与加载 | `CanUserFilterColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`QueryChanged`、`LoadState`、`LoadError`、`IsDataStale`、`Reload()` | 表达用户查询意图、异步生命周期、错误与可提交能力。 |
| 视觉与布局 | `BottomPaginationAlign`、`ColumnWidth`、`HorizontalAlignment`、`HorizontalScrollBarVisibility`、`MaxColumnWidth`、`MinColumnWidth`、`RowHeight`、`SeparatorBrush`、`SizeType`、`SublevelIndent` 等 14 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 列查询契约 | `FieldId`、`CanUserSort`、`SupportedSortDirections`、只读 `SortState`、过滤候选展示属性 | 让列声明协议字段与能力覆盖，显示 Binding 不参与查询 identity。 |
| 其他稳定入口 | `CellTheme`、`CustomOperatingIndicator`、`EmptyIndicator`、`Footer`、`FormatString`、`GridLinesVisibility`、`Level`、`Maximum`、`Minimum` 等 | 保留非数据架构 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

`ItemsSource` 只接受 `IDataGridSource?`，不是旧集合型 `IEnumerable` 入口。本地集合使用 `DataGridLocalSource.Create(...)`
建立强类型 range source，远端实现使用同一接口翻译 Query 并按请求范围返回数据；DataGrid 不拥有或 dispose 外部 Source。

稳定事件包括 `QueryChanged`、`SelectionChanged`、`RowReordering` 和 `RowReordered`。`QueryChanged` 是不可取消的观察事件，在 Query 的内存提交与视觉投影之后、对应 Source 请求之前触发；其他事件的参数和触发顺序同样属于契约。

行拖动重排由 `CanUserReorderRows`、`DataGridRowReorderColumn`、`RowReordering`、`RowReordered` 和可选
`IDataGridMovableSource` 共同表达。DataGrid 使用 source row key 与目标 before/after key 提交移动，不把 view、window 或 display
索引解释为 Source 集合索引。Source 不支持移动、当前 Query 语义禁止移动、编辑未能提交或 snapshot 失效时，handle 不进入
可提交状态，也不发布 `RowReordered`。

分页公共契约由 `PageSize`、`PaginationVisibility`、`TopPaginationAlign`、`BottomPaginationAlign` 和
`IsHideOnSinglePage` 共同表达。`PageSize=0` 表示连续模式；非零值形成 `DataGridPageRequest`，其中 data start 使用 long、单页
count 使用 int。分页总数只来自已提交 Source result 的 `TotalDataCount`，顶部和底部 Pagination 只投影同一份 applied page
state。翻页成功后垂直 offset 归零；失败时页码、rows、totals、scrollbar 和 Query 共同回退到最后成功 presentation。

列宽公共契约由 `DataGrid.ColumnWidth`、`DataGridColumn.Width`、控件级与列级最小/最大宽度，以及
`DataGridLengthUnitType` 共同表达。`ColumnWidth` 默认为 `Auto`；单列可以使用 `Pixel`、`Auto`、
`SizeToHeader`、`SizeToCells` 或 `Star`。内容驱动模式先形成期望宽度，star 模式再按权重分配有限列视口中的
剩余空间。列宽状态与统一分配算法由 `DataGrid` 持有，普通表头、分组表头和 rows/cells presenter 只提供当前
布局可证明的有限宽度或内容测量结果。空数据时列宽求解不能依赖已隐藏的 rows presenter，完整契约见
[DataGrid 列宽分配设计](column-sizing-design.md)。

列过滤候选项与过滤条件分离。列 `Filters` 只负责菜单或树形弹层中的候选内容，可以绑定 ViewModel 集合或数据库返回的
DTO；`DataGrid.Query.Filters` 是已应用条件的唯一 owner。Flyout checked state、FilterIndicator 激活态与 Source request 都从
Query 投影，用户确认后一次性生成新的不可变 Query。过滤 DTO 的显示成员可以使用 generated data member accessor；Query 中的
值则转换为确定性的 `DataGridScalar`，不保留可变 object、predicate 或反射 path。

过滤交互模式使用显式枚举表达：`FilterPresenterMode` 表达菜单或树形弹层，`FilterSelectionMode` 表达单选或多选，`FilterApplyMode` 表达确认、关闭或选择变化时应用过滤。枚举状态比布尔开关更适合作为长期 public API，因为它能把展示方式、选择方式和提交时机拆成三个正交状态，避免布尔组合在状态流中产生歧义。

主要公开类型与枚举：

- 类型：`DataGrid`、`DataGridQuery`、`IDataGridSource`、`DataGridSourceSchema`、`DataGridFieldSchema`、`DataGridFieldDisplayAccessor`、`DataGridRange`、`DataGridPageRequest`、`DataGridRangeResult`、`DataGridSelectionState`、`DataGridColumn`、`DataGridRow` 和 `DataGridCell` 等。
- 枚举：`DataGridSortDirection`、`DataGridSortUpdateMode`、`DataGridLoadState`、`DataGridSourceEntryKind`、`DataGridClipboardCopyMode`、`DataGridEditingUnit`、`DataGridLengthUnitType`、`DataGridPaginationVisibility` 和 `DataGridRowDetailsVisibilityMode` 等。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Ascending` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_BottomGridLine` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_BottomPagination` | `Pagination` | 投影 applied page state 并转发底部翻页意图。 |
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
| `PART_TopPagination` | `Pagination` | 投影 applied page state 并转发顶部翻页意图。 |
| `PART_VerticalIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_VerticalSeparator` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

控件专属或内部伪类包括 `DataGridPseudoClass.EmptyColumns`、`DataGridPseudoClass.EmptyRows`、`DataGridPseudoClass.FirstColumnHeader`、`DataGridPseudoClass.GroupItemLeaf`、`DataGridPseudoClass.LastColumnHeader`、`DataGridPseudoClass.MiddleColumnHeader`、`DataGridPseudoClass.RowHover`、`EmptyColumns=:empty-columns`、`EmptyRows=:empty-rows`、`FirstColumnHeader=:first-column-header`、`GroupItemLeaf=:is-leaf-item`、`LastColumnHeader=:last-column-header` 等 14 项。这些伪类属于主题 selector 可观察契约，不能在未同步主题和 Gallery 的情况下重命名或删除。

## 事件与命令

DataGrid 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
稳定事件包括 `QueryChanged`、`SelectionChanged`、`RowReordering` 和 `RowReordered`。`QueryChanged` 是不可取消的观察事件，在 Query 的内存提交与视觉投影之后、对应 Source 请求之前触发；其他事件的参数和触发顺序同样属于契约。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

- `controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml`

## 状态模型

DataGrid 的状态流按以下路径收敛：

```text
Public API / inherited command / Source invalidation / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- Query、Selection、current、loading、motion 和 visual option 状态由 DataGrid 或明确 Source capability 单向推导，不能在 template part 之间双向竞争。
- 排序、过滤和分组只由 `Query` 拥有；列、Header、Cell 和 Flyout 只投影相应字段状态。
- `Filters` 候选项替换、reset 或 clear 时可以重新物化 Flyout 内容，但不能直接改变已应用 Query；用户确认或显式 API 才提交新的 Query。
- 分页状态以 applied PageRequest 和 `TotalItemCount` 为 owner；上下 Pagination 不能互相覆盖，也不能在模板重建时反向重置 Query 或 Source。
- Loading 没有可展示的已提交 presentation，实际挂起时驱动 Spin；Refreshing 保持旧 presentation 的几何与完整不透明度，
  不自动启动 Spin。两种状态都禁止 edit/delete/move，成功时原子交换，失败时完整回退。
- 连续滚轮、惯性或 scrollbar thumb 输入只保留最新有效 `DesiredViewport`。新目标先接管仍需要的 block，再使旧视口 scope
  失效；不再被任何有效 scope 使用的排队或执行中请求立即收到取消，旧 prefetch 不能排在新 visible range 之前。
- 同步 Source 与 cache hit 在当前调用中直接进入 Ready，不发布瞬时 Loading/Refreshing；调用方显式设置 `IsOperating=true`
  时仍可在任意 LoadState 显示 Spin。
- 行拖动状态以当前 `DataGrid` 的单一拖拽会话为 owner；handle 和 RowsPresenter 只投影输入与 ghost row，不能保存
  跨 DataGrid 共享的静态拖拽状态。Pointer capture、源 row key、Source snapshot 和目标邻接 key 必须属于同一个会话。
- `RowReordering` 在超过拖动阈值后且创建 ghost row 前触发一次；事件取消或事件回调改变 DataGrid、源行、
  Source、Query、snapshot 或移动能力时，本次 Pointer 会话保持取消状态，不得在后续移动帧重复开始。
- `RowReordered` 只在 movable Source 成功提交顺序变化，并且 ghost、capture、动画与会话状态全部清理后触发。
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

### 9.1 热路径

- Scroll input 只更新/合并 DesiredViewport，handler 返回后由 coordinator 调度 request。
- 每次不同的 DesiredViewport 只替换一个 active scope；旧排队项不会作为无界 task 留在 semaphore 前等待。
- PointerMoved 只更新 drag session、ghost offset、target key 和必要自动滚动 intent。
- Measure/Arrange/prepare/recycle 只做 committed index lookup 与 container state 投影。
- Query no-op 不请求、不刷新 rows；sort state 没有 per-cell 长期订阅。
- Ready 模板不增加 visual node；prefetch 不触发布局。
- 同步 Source/cache hit 不发布瞬时 Loading/Refreshing；异步首次加载驱动 Spin，已有 snapshot 的异步刷新不启动自动遮罩。

### 9.2 复杂度与资源上限

| 路径 | 上限 |
| --- | --- |
| fixed-height offset lookup | O(1) |
| variable-height offset lookup | O(log M)，M 由 cache/pin/显式状态约束 |
| realized row/group controls | visible + editing row + drag row |
| request concurrency | 默认最多 2 |
| active viewport scope | 每个 generation 1 个 |
| queued obsolete viewport work | 0；scope supersede 时从可发现队列移除并取消 |
| prefetch priority | 低于所有 visible 缺口，并绑定产生它的 committed scope |
| range size | Schema.MaximumRangeSize，最大 4096 |
| cache | 有限 block LRU，与 total 无关 |
| snapshot-expiry auto retry | 每个用户操作最多 1 次 |

所有 request、cache、pool、subscription 和 sparse state 必须在长时间往返滚动后达到稳态，不能随滚动次数或曾访问 row 数持续
增长。

### 9.3 AOT

- Query/Source public 类型只位于 DataGrid package，不引入 Core/Shared 反向依赖。
- LocalSource 使用静态泛型 descriptor，并把 typed getter 显式投影为 schema `DisplayAccessor`；generated
  `IDataMemberAccessor` 可通过 `FromDataMember<TItem>` 复用。自动列只消费该 accessor 并生成 compiled binding，绝不把
  FieldId 当作 CLR path。
- 没有 `DisplayAccessor` 的远端 schema 字段只支持显式 Column/Binding；`PropertyChangedName` 与 FieldId 相互独立，binding
  dispose/recycle 时解除 `INotifyPropertyChanged` 订阅。
- 不使用 PropertyInfo.GetValue、Expression.Compile、assembly scan、runtime registration、动态泛型构造或字符串 Binding。
- 不新增 linker root、trimming suppression 或反射 fallback。
- Source generator 输出只通过输入源和 generator 更新。

## 源码索引

稳定 ownership 按职责组织：

```text
src/AtomUI.Desktop.Controls.DataGrid/
├── DataGrid.cs                         public contract + lifecycle
├── DataGrid.Query.cs                   Query commit and visual projection
├── DataGrid.RangeLoading.cs            viewport-to-coordinator integration
├── DataGrid.Virtualization.cs          desired/committed viewport + container bridge
├── Data/
│   ├── Query/                          immutable values, scalar and validation
│   ├── Source/                         schema, request, result and capability contracts
│   │   └── Local/                      typed local source and projection
│   └── Virtualization/                 viewport scope, request scheduler, snapshot, cache, index and heights
├── Column/                             column contract, header, sorting/filtering interaction
├── Column/Filters/                     filter indicator, flyout and candidate presentation
├── Row/                                row, group header, details and row presenters
├── Cell/                               cell state and horizontal virtualization
├── EventArgs/                          public event payloads
├── Themes/                             static templates, selectors and resource binding
├── Localization/                       generated-catalog-backed localized strings
└── Utils/                              narrow shared algorithms and converters
```

维护规则：

- 主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- partial 与 helper 按稳定 owner 拆分，不按 public/private 或单个触发点拆分。
- Theme 文件负责静态视觉结构、Template Part、selector 和资源绑定；不在 C# 中动态复制同一结构。
- Token 只保存组件视觉变量，不保存 Query、loading、selection、expanded 或 popup runtime state。
- Gallery 只展示用法和验证行为，不成为运行时状态 owner。
- GeneratedFiles 由对应 generator 维护，不手工编辑。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/data-grid/overview.md`
- 实现文档：`docs/controls/desktop/data-display/data-grid/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/data-grid/token.md`
- 变更记录：`docs/controls/desktop/data-display/data-grid/changelog.md`
- 语义结构：`./semantic-cn.md`
