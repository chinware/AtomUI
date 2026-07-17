# Transfer 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Transfer` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Transfer
  -> TransferItemDecorator (control theme, TransferItemDecoratorTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> DockPanel#RootLayout (template-stable)
           -> PixelAlignedBorder#HeaderFrame (template-stable)
              -> DockPanel#HeaderLayout (template-stable)
                 -> CheckBox#SelectAllCheckBox (template-stable)
                 -> TransferSelectDropdown#MenuIndicator (internal-observable)
                 -> ContentPresenter#SelectedInfo (internal-observable)
                 -> ContentPresenter#TitleContentPresenter (internal-observable)
           -> LineEdit#FilterInput (template-stable)
           -> PixelAlignedBorder#FooterFrame (template-stable)
              -> ContentPresenter#FooterPresenter (internal-observable)
           -> ContentPresenter#ContentPresenter (internal-observable)
  -> TransferListItem (item container control theme, TransferListItemTheme.axaml)
     -> Border#Frame (template-stable)
        -> DockPanel#ContentLayout (template-stable)
           -> CheckBox#SelectedIndicator (template-stable)
           -> TransferRemoveItemButton#RemoveButton (public)
           -> ContentPresenter#ContentPresenter (internal-observable)
  -> TransferListView (control theme, TransferListViewTheme.axaml)
  -> TransferSelectDropdown (control theme, TransferSelectDropdownTheme.axaml)
  -> TransferTreeViewItemHeader (control theme, TransferTreeViewItemHeaderTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> Grid#ItemsLayout (template-stable)
           -> NodeSwitcherButton#{x:Static atom:TreeViewItemHeaderThemeConstants.NodeSwitcherButtonPart} (template-stable)
           -> Decorator (template-stable)
              -> CheckBox#ToggleCheckbox (template-stable)
           -> Decorator (template-stable)
              -> RadioButton#ToggleRadio (template-stable)
           -> Decorator (template-stable)
              -> IconPresenter#{x:Static atom:TreeViewItemHeaderThemeConstants.IconPresenterPart} (internal-observable)
           -> Decorator (template-stable)
              -> Border#{x:Static atom:TreeViewItemHeaderThemeConstants.HeaderContentFramePart} (template-stable)
                 -> Panel (template-stable)
                    -> ContentPresenter#HeaderPresenter (internal-observable)
                    -> TextBlock#FilterHighlighter (template-stable)
  -> TransferTreeViewItem (item container control theme, TransferTreeViewItemTheme.axaml)
     -> StackPanel (template-stable)
        -> TransferTreeViewItemHeader#Header (internal-observable)
        -> LayoutAwareMotionActor#{x:Static atom:TreeViewItemThemeConstants.ItemsPresenterMotionActorPart} (internal-observable)
           -> ItemsPresenter#ItemsPresenter (internal-observable)
  -> TransferTreeView (control theme, TransferTreeViewTheme.axaml)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Transfer` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `TransferItemDecorator` | control theme | `TransferItemDecoratorTheme.axaml` | Transfer | `BodyCornerRadius`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (PixelAlignedBorder) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BodyCornerRadius`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RootLayout` | template node (DockPanel) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BodyCornerRadius`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius`, `FilterPlaceholderText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderFrame` | template node (PixelAlignedBorder) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BorderThickness`, `CornerRadius`, `IsAllSelected`, `IsItemsSourceEmpty`, `IsMotionEnabled`, `IsOneWay` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderLayout` | template node (DockPanel) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `IsAllSelected`, `IsItemsSourceEmpty`, `IsMotionEnabled`, `IsOneWay`, `IsPaginationEnabled`, `IsShowSelectDropdownMenu` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectAllCheckBox` | template node (CheckBox) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `IsAllSelected`, `IsItemsSourceEmpty` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `MenuIndicator` | template node (TransferSelectDropdown) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `IsAllSelected`, `IsItemsSourceEmpty`, `IsMotionEnabled`, `IsOneWay`, `IsPaginationEnabled`, `IsShowSelectDropdownMenu` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SelectedInfo` | template node (ContentPresenter) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `SelectedMessage` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TitleContentPresenter` | template node (ContentPresenter) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `Title`, `TitleTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `FilterInput` | template node (LineEdit) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `FilterPlaceholderText`, `IsFilterEnabled`, `ViewType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FooterFrame` | template node (PixelAlignedBorder) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BorderThickness`, `CornerRadius`, `Footer`, `FooterTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FooterPresenter` | template node (ContentPresenter) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `Footer`, `FooterTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `TransferItemDecoratorTheme.axaml` | TransferItemDecorator | `BodyCornerRadius`, `Content`, `ContentTemplate`, `ListHeight` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TransferListItem` | item container control theme | `TransferListItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `HorizontalContentAlignment`, `IsCheckable` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `TransferListItemTheme.axaml` | TransferListItem | `Background`, `Content`, `ContentTemplate`, `CornerRadius`, `HorizontalContentAlignment`, `IsCheckable` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentLayout` | template node (DockPanel) | `TransferListItemTheme.axaml` | TransferListItem | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `IsCheckable`, `IsSelected`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectedIndicator` | template node (CheckBox) | `TransferListItemTheme.axaml` | TransferListItem | `IsCheckable`, `IsSelected` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `RemoveButton` | template node (TransferRemoveItemButton) | `TransferListItemTheme.axaml` | TransferListItem | `IsCheckable` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ContentPresenter` | template node (ContentPresenter) | `TransferListItemTheme.axaml` | TransferListItem | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TransferListView` | control theme | `TransferListViewTheme.axaml` | 用户代码 / 控件宿主 | 主题状态 / visual state | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `TransferSelectDropdown` | control theme | `TransferSelectDropdownTheme.axaml` | Transfer | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TransferTreeViewItemHeader` | control theme | `TransferTreeViewItemHeaderTheme.axaml` | Transfer | `Background`, `BorderThickness`, `Content`, `ContentFrameBackground`, `ContentTemplate`, `CornerRadius` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (PixelAlignedBorder) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Background`, `BorderThickness`, `Content`, `ContentFrameBackground`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsLayout` | template node (Grid) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Content`, `ContentFrameBackground`, `ContentTemplate`, `FilterHighlightRuns`, `GroupName`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:TreeViewItemHeaderThemeConstants.NodeSwitcherButtonPart}` | template node (NodeSwitcherButton) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `IsExpanded`, `IsLoading`, `IsMotionEnabled`, `SwitcherCollapseIcon`, `SwitcherExpandIcon`, `SwitcherLeafIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ToggleCheckbox` | template node (CheckBox) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `IsChecked` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ToggleRadio` | template node (RadioButton) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `GroupName`, `IsChecked` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:TreeViewItemHeaderThemeConstants.IconPresenterPart}` | template node (IconPresenter) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Icon`, `IconEffectiveVisible`, `IsEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `{x:Static atom:TreeViewItemHeaderThemeConstants.HeaderContentFramePart}` | template node (Border) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Content`, `ContentFrameBackground`, `ContentTemplate`, `FilterHighlightRuns`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Content`, `ContentTemplate`, `FilterHighlightRuns` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderPresenter` | template node (ContentPresenter) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `FilterHighlighter` | template node (TextBlock) | `TransferTreeViewItemHeaderTheme.axaml` | TransferTreeViewItemHeader | `FilterHighlightRuns` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TransferTreeViewItem` | item container control theme | `TransferTreeViewItemTheme.axaml` | 用户代码 / 控件宿主 | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StackPanel` | template node (StackPanel) | `TransferTreeViewItemTheme.axaml` | TransferTreeViewItem | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Header` | template node (TransferTreeViewItemHeader) | `TransferTreeViewItemTheme.axaml` | TransferTreeViewItem | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `Content`、`ContentTemplate`、`FilterPlaceholderText`、`FilterValueSelector`、`FooterTemplate`、`ItemTemplate`、`SelectionsIcon`、`SelectionsIconTemplate`、`SourceTitle`、`SourceTitleTemplate` 等 22 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `TargetKeys`、`SelectedKeys`、`Filter`、`IsAllSelected`、`IsFilterEnabled`、`PageSize` | 维护目标集合、当前面板选择、过滤、分页和集合状态。 |
| 交互与状态 | `IsMasked`、`IsMotionEnabled`、`IsOneWay`、`IsPaginationEnabled`、`IsShowSearch`、`IsShowSelectAll`、`IsShowSelectAllCheckbox`、`IsShowSelectDropdownMenu`、`IsStretchView`、`Status` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `ListHeight`、`ListWidth`、`SizeType` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `Footer`、`TargetView`、`TargetViewFooter`、`ViewType` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、collection/filter、input/value、motion、visual option。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Transfer Token + ControlTheme。 |

## State Flow

Transfer 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、collection/filter、input/value、motion、visual option 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `TargetKeys` 是目标集合的 public owner，源/目标面板数据由 `ItemsSource` 与 `TargetKeys` 推导；`SelectedKeys` 是当前选择的 public owner，内部源面板选择和目标面板选择按 key 是否存在于 `TargetKeys` 自动拆分。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

Transfer 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `AbstractTransferTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ListTransferTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TransferItemDecoratorTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferListItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferListViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TransferSelectDropdownTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TransferThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `TransferTreeViewItemHeaderTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferTreeViewItemTheme.axaml` | 定义集合项、容器项或局部单元的状态视觉。 |
| `TransferTreeViewTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `TreeTransferTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

Transfer 使用 `TransferToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、collection/filter、input/value、motion、visual option 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

Transfer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `TransferToken`，scope id 为 `Transfer`，源码位于 `src/AtomUI.Desktop.Controls/Transfer/TransferToken.cs`。

## Customization Boundaries

维护 Transfer 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Transfer 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- `TargetKeys` / `SelectedKeys` 不能与 `TransferListView.SelectedItems`、`TransferTreeView.CheckedItems` 或容器状态形成多个业务 owner。
- 对绑定集合的移动、移除和清空不能无条件替换集合实例；可写集合必须原地更新。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
