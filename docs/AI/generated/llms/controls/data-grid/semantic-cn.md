# DataGrid 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `DataGrid` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls.DataGrid/Themes/DataGridTheme.axaml`

```xml
<PixelAlignedBorder Name="Frame">
    <Border Name="FrameContentClip">
        <Spin>
            <DockPanel>
                <Pagination Name="{x:Static atom:DataGridThemeConstants.TopPaginationPart}" />
                <PixelAlignedBorder Name="TitleFrame">
                    <ContentPresenter Name="Title" />
                </PixelAlignedBorder>
                <Pagination Name="{x:Static atom:DataGridThemeConstants.BottomPaginationPart}" />
                <ContentPresenter Name="Footer" />
                <Grid>
                    <DataGridTopLeftColumnHeader Name="{x:Static atom:DataGridThemeConstants.TopLeftCornerPart}" />
                    <Border Name="ColumnHeadersPresenterFrame">
                        <Panel>
                        </Panel>
                    </Border>
                    <PixelAlignedBorder Name="ColumnHeadersAndRowsSeparator" />
                    <DataGridRowsPresenter Name="{x:Static atom:DataGridThemeConstants.RowsPresenterPart}" />
                    <ContentPresenter Name="EmptyIndicator" />
                    <Rectangle Name="{x:Static atom:DataGridThemeConstants.BottomRightCornerPart}" />
                    <ScrollBar Name="{x:Static atom:DataGridThemeConstants.VerticalScrollbarPart}" />
                    <ScrollBar Name="{x:Static atom:DataGridThemeConstants.HorizontalScrollbarPart}" />
                    <Border Name="DisabledVisualElement" />
                    <DataGridColumnDraggingOverIndicator Name="{x:Static atom:DataGridThemeConstants.DraggingOverIndicatorPart}" />
                </Grid>
            </DockPanel>
        </Spin>
    </Border>
</PixelAlignedBorder>
```

## Composition Model

该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ItemsSource`、`Query`、`AppliedQuery`、`GroupExpansion`、`TotalItemCount`、`TotalEntryCount`、`AutoGenerateColumns`、`CellTemplate`、`CellEditingTemplate` | 定义数据输入、查询、范围 presentation、模板和业务对象入口；`ItemsSource` 的类型是 `IDataGridSource?`。 |
| 选择与当前项 | `Selection`、`CurrentRowKey`、`SelectionChanged`、`ClipboardCopyMode` | 以稳定 row key、query scope 和 index interval 维护可跨 range/page 的声明式状态。 |
| 交互与加载 | `CanUserFilterColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`QueryChanged`、`LoadState`、`LoadError`、`IsDataStale`、`Reload()` | 表达用户查询意图、异步生命周期、错误与可提交能力。 |
| 视觉与布局 | `BottomPaginationAlign`、`ColumnWidth`、`HorizontalAlignment`、`HorizontalScrollBarVisibility`、`MaxColumnWidth`、`MinColumnWidth`、`RowHeight`、`SeparatorBrush`、`SizeType`、`SublevelIndent` 等 14 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 列查询契约 | `FieldId`、`CanUserSort`、`SupportedSortDirections`、只读 `SortState`、过滤候选展示属性 | 让列声明协议字段与能力覆盖，显示 Binding 不参与查询 identity。 |
| 其他稳定入口 | `CellTheme`、`CustomOperatingIndicator`、`EmptyIndicator`、`Footer`、`FormatString`、`GridLinesVisibility`、`Level`、`Maximum`、`Minimum` 等 | 保留非数据架构 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | AppliedQuery、LoadState、Selection、CurrentRowKey、sort/filter/group projection、motion 和 visual option。 |
| 主题语义 | ControlTheme、SharedToken、Control Own Token 和模板绑定如何表达视觉。 | DataGrid Token + ControlTheme。 |

## State Flow

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

## Theme and Token Boundaries

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

Token 边界：

DataGrid Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `DataGridToken`，scope id 为 `DataGrid`，源码位于 `src/AtomUI.Desktop.Controls.DataGrid/DataGridToken.cs`。

## Customization Boundaries

维护 DataGrid 时必须保持以下不变量：

- `ItemsSource`、`Query`、`Selection` 和可选 Source capability 的 identity 与 ownership 不能被模板或 presenter 改写。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- Ready 状态的视觉树、尺寸、列宽、滚动条、冻结列、RowDetails、selected/sorted 优先级和嵌套滚动链保持稳定。
- Template part 重新应用、Source 替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅、请求、cache 和资源宿主。
- 不通过隐藏延迟、强制刷新、ignore flag 或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- Source result 必须满足精确切片、snapshot、totals、key 唯一性和 index mapping 合同；无效结果不得部分显示。
- Source 必须协作处理 FetchAsync 的 CancellationToken 才能获得快速跳转的低延迟保证；无论 Source 是否协作，旧 viewport 结果
  都不能覆盖最新 presentation，DataGrid 也不能突破有界并发掩盖不可抢占 I/O。
- 连续模式超出 int presentation 上限时明确失败；分页通过 long data start 访问更大数据，不截断或饱和索引。
- Measure、Arrange、container prepare/recycle 和 input handler 不执行 Source I/O 或同步等待。
- 未声明 movable capability 时安全拒绝移动，不能错误提交、部分提交或发出成功事件。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 DataGrid 时不得破坏：

- Query、AppliedQuery、Source、Selection、CurrentRowKey 和 PresentationSnapshot 的单一 ownership。
- QueryRevision/DataGeneration 与 Source identity/snapshot 的迟到结果隔离。
- Result 精确切片、key 唯一性、totals 恒定和三索引域映射。
- DesiredViewport 与 CommittedViewport 隔离，layout 热路径零 I/O。
- 每个 generation 只有一个 active viewport scope；新 scope 先转移共享 block lease，再取消 orphaned visible/prefetch work。
- visible queue 优先于 prefetch queue；旧 prefetch 不能占据新 visible target 的排队顺序。
- Cache、pool、height metadata、selection 和 RowDetails 状态的有界或用户显式增长属性。
- Container reset 完整性，以及 realized/edit/drag/applied block 的 pin 生命周期。
- Pinned filter 的唯一目标和 Header -> Indicator -> Flyout relay 对称释放。
- Row reorder 的 DataGrid session、presenter ghost 与 Source mutation 职责分离。
- 列宽 solver 不依赖 RowsPresenter 可见性；filler 不掩盖 star 分配。
- ControlTheme key、Template Part、伪类、Token、Semantic Part 和 Ready 视觉优先级。
- Light/Dark、Browser/Desktop、SizeType、冻结列、RowDetails 和 nested scrolling 的一致语义。
- 文档、源码 public surface、Gallery、tests 与 generated LLMS 的一致性。
