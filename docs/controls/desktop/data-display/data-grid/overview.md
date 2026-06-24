# DataGrid 桌面版架构设计

本文档定义 `DataGrid` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [DataGrid 桌面版实现原理](implementation.md)，DataGrid Token 的专项设计见 [DataGrid Token 设计](token.md)，设计和契约变化记录见 [DataGrid Changelog](changelog.md)。

## 1. 控件定位

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
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | DataGrid Token + ControlTheme。 |

## 3. API 与契约模型

DataGrid 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `AutoGenerateColumns`、`CanUserFilterColumns`、`CanUserReorderColumns`、`CanUserReorderRows`、`CanUserResizeColumns`、`CanUserSortColumns`、`CellEditingTemplate`、`CellTemplate`、`ColumnHeaderHeight`、`ContentHeight` 等 32 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `ClipboardCopyMode`、`CurrentSortDirection`、`FilterMode`、`Index`、`IsFilterActivated`、`IsHideOnSinglePage`、`IsHoverMode`、`IsMultipleFilterEnabled`、`IsSelected`、`IsSorterTooltipVisible` 等 20 项 | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `AscendingIndicatorVisible`、`DescendingIndicatorVisible`、`IsDeleteEnabled`、`IsDetailsVisible`、`IsEditEnabled`、`IsFrameBorderVisible`、`IsFrozen`、`IsLeaf`、`IsMotionEnabled`、`IsOperating` 等 19 项 | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `BottomPaginationAlign`、`ColumnWidth`、`HorizontalAlignment`、`HorizontalScrollBarVisibility`、`MaxColumnWidth`、`MinColumnWidth`、`RowHeight`、`SeparatorBrush`、`SizeType`、`SublevelIndent` 等 14 项 | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `CellTheme`、`CollectionView`、`CustomOperatingIndicator`、`EmptyIndicator`、`Footer`、`FormatString`、`GridLinesVisibility`、`Level`、`Maximum`、`Minimum` 等 15 项 | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

稳定事件包括 `SelectionChanged`。事件触发顺序属于兼容契约，不能因内部状态重排而改变。

主要公开类型与枚举：

- 类型：`AtomUIDataGridThemesProvider`、`CollectionViewGroupComparer`、`CollectionViewGroupRoot`、`ControlTokenTypePool`、`DataGrid`、`DataGridAbstractTextColumn`、`DataGridAutoGeneratingColumnEventArgs`、`DataGridBeginningEditEventArgs`、`DataGridBoundColumn`、`DataGridCell`、`DataGridCellCollection`、`DataGridCellCoordinates`、`DataGridCellEditEndedEventArgs`、`DataGridCellEditEndingEventArgs` 等 95 项。
- 枚举：`DataGridClipboardCopyMode`、`DataGridEditAction`、`DataGridEditingUnit`、`DataGridFilterMode`、`DataGridGridLinesVisibility`、`DataGridHeadersVisibility`、`DataGridLangResourceKind`、`DataGridLengthUnitType`、`DataGridPaginationVisibility`、`DataGridRowDetailsVisibilityMode` 等 15 项。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_Ascending` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_BottomGridLine` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_ContentFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_ContentPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_Descending` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_FocusVisual` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_Frame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_HeaderPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_HorizontalIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
| `PART_IndicatorIconButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ItemsPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_RightGridLine` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_RootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_SortIndicator` | `?` | 展示指示器、进度、分页或状态反馈。 |
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
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## 5. 视觉与主题模型

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

## 6. 控件家族或集成关系

DataGrid 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

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

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme 和 Gallery ShowCase 的示例/API/Token 表保持一致。
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
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 选择与当前项模型

DataGrid 的当前项状态必须由单一 owner 推导。public 选择属性、集合项容器和伪类之间只能做单向同步，集合替换、清空和模板重套用时必须回放当前状态。

### 8.2 集合与数据同步模型

DataGrid 的集合状态必须能处理 source replace、reset、clear 和 container recycle。业务数据对象不应反向持有视觉对象，虚拟化或懒创建路径必须在容器回收时清理旧状态。

### 8.3 动效模型

DataGrid 的动效只表达状态变化反馈，不应改变 public API 语义。初始加载、禁用态和卸载路径应能抑制或取消动效，避免保留旧控件实例。

### 8.4 视觉选项模型

DataGrid 的视觉选项通过 public API 归一为 theme variables、伪类或模板绑定。Token 保存组件语义值，不能保存实例运行时状态或业务色值。

## 9. 文档导航与验证策略

关联文档：

- [DataGrid 桌面版实现原理](implementation.md)
- [DataGrid Token 设计](token.md)
- [DataGrid Changelog](changelog.md)

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 selection/checked/active、collection/filter、motion、visual option、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| Token | 检查 TokenKind、AXAML token resource、Gallery Token 表和文档同步。 |
| Gallery | 走查对应 ShowCase 示例、API 表和 Token 表入口。 |
