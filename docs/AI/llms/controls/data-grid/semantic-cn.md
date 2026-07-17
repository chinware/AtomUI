# DataGrid 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

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
| 内容与数据 | `AutoGenerateColumns`、`CanUserFilterColumns`、`CanUserReorderColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`CellEditingTemplate`、`CellTemplate`、`ColumnHeaderHeight`、`ContentHeight` 等 32 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `ClipboardCopyMode`、`CurrentSortDirection`、`Filters`、`SelectedFilterValues`、`FilterPresenterMode`、`FilterSelectionMode`、`FilterApplyMode`、`Index`、`IsFilterActivated`、`IsHideOnSinglePage`、`IsHoverMode`、`IsSelected`、`IsSorterTooltipVisible` 等 | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `AscendingIndicatorVisible`、`DescendingIndicatorVisible`、`IsDeleteEnabled`、`IsDetailsVisible`、`IsEditEnabled`、`IsFrameBorderVisible`、`IsFrozen`、`IsLeaf`、`IsMotionEnabled`、`IsOperating` 等 19 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `BottomPaginationAlign`、`ColumnWidth`、`HorizontalAlignment`、`HorizontalScrollBarVisibility`、`MaxColumnWidth`、`MinColumnWidth`、`RowHeight`、`SeparatorBrush`、`SizeType`、`SublevelIndent` 等 14 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `CellTheme`、`CollectionView`、`CustomOperatingIndicator`、`EmptyIndicator`、`Footer`、`FormatString`、`GridLinesVisibility`、`Level`、`Maximum`、`Minimum` 等 15 项 | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、collection/filter、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | DataGrid Token + ControlTheme。 |

## State Flow

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
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

DataGrid 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AtomUIDataGridThemesProvider.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
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

DataGrid 使用 `DataGridToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、collection/filter、motion、visual option 运行时状态。

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

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 DataGrid 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 列过滤只能有一个选中状态 owner；`Filters`、flyout checked state、`SelectedFilterValues` 和 `FilterDescriptions` 之间不得形成互相覆盖的并行状态源。
- 过滤项解析必须支持业务 DTO 和 `DataGridFilterItem` 两类输入，不得要求 VM 反向依赖内部 flyout、menu item 或 tree item 类型；业务 DTO 必须有生成的 data member accessor，不在 AOT 敏感路径中使用运行时反射兜底。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
