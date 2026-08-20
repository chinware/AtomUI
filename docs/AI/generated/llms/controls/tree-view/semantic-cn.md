# TreeView 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

### 2.1 `TreeView`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeView` |
| Part | `root` |
| Selector | TreeView 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `TreeView` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | TreeView owner |
| 职责 | 树根是 Items、选择、勾选、展开、过滤、拖拽、异步加载、空状态、switcher 图标与动效配置的统一 owner；作为顶层 `item` Part 的 owner-scoped Selector 作用域边界。 |
| 相关 API | `Items`、`ItemsSource`、`SelectionMode`、`SelectedItem`、`SelectedItems`、`ToggleType`、`IsCheckStrictly`、`IsDefaultExpandAll`、`DefaultSelectedPaths`、`DefaultCheckedPaths`、`DefaultExpandedPaths`、`IsDraggable`、`IsShowIcon`、`IsShowLine`、`IsShowLeafIcon`、`NodeHoverMode`、`Switcher*Icon`、`IsSwitcherRotation`、`IsSelectable`、`IsSelectOnRightClick`、`DataLoader`、`Filter`、`FilterStrategy`、`EmptyIndicator`、`IsMotionEnabled`、`OpenMotion`、`CloseMotion` |
| 相关 Token | `TreeViewToken`、SharedToken |
| 稳定性 | stable since 6.0 |

#### `item`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeView` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `> .semantic-item` |
| Style Type | `TreeViewItemStyle` |
| ContractType | `TreeViewItem` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个**顶层** `TreeViewItem` 容器 |
| 职责 | 统一表示树中直接挂在 `TreeView` 根下的节点容器，覆盖 `TreeViewItem` 容器为公开 item 容器的顶层形态。 |
| 相关 API | `Header`、`HeaderTemplate`、`Icon`、`IsChecked`、`IsLeaf`、`IsLoading`、`IsSelected`、`IsExpanded`、`IsEnabled`、`IsDragging`、`IsDragOver`、`NodeHoverMode`、`IsShowLine` |
| 相关 Token | `TreeItemMargin`、`HeaderHeight`、SharedToken（`ColorBorder`） |
| 稳定性 | stable since 6.0 |

### 2.2 `TreeViewItem`

`TreeViewItem` 是递归 owner：其 `item` Part 覆盖下一层子节点容器，`itemSwitcher` / `itemIcon` / `itemTitle` 覆盖每个
节点的内容区域。`TreeViewItem` 的 `root` 是节点容器本身（implicit Part，无 `.semantic-root` marker，不生成 Style）。

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeViewItem` |
| Part | `root` |
| Selector | TreeViewItem 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用 |
| ContractType | `TreeViewItem` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 每个 `TreeViewItem` 容器 |
| 职责 | 单个树节点容器：承载节点级状态（selected / checked / expanded / disabled / loading / drag / filter）与树形连线渲染表面；作为子节点容器与节点内容 Part 的 owner-scoped Selector 作用域边界。 |
| 相关 API | `Header`、`HeaderTemplate`、`Icon`、`IsChecked`、`IsLeaf`、`IsLoading`、`IsSelected`、`IsExpanded`、`IsEnabled`、`IsDragging`、`IsDragOver`、`NodeHoverMode`、`IsShowLine` |
| 相关 Token | `TreeItemMargin`、`HeaderHeight`、SharedToken（`ColorBorder`） |
| 稳定性 | stable since 6.0 |

#### `item`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeViewItem` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `> .semantic-item` |
| Style Type | `TreeViewItemItemStyle` |
| ContractType | `TreeViewItem` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个**子级** `TreeViewItem` 容器 |
| 职责 | 递归表示当前节点容器生成的下一层子节点容器。与 `TreeView.item` 使用同一 `.semantic-item` 身份，共同保证任意深度的节点都可被 `atom|TreeViewItem` / `atom|TreeView` owner scope 命中。 |
| 相关 API | 同 `TreeViewItem` root |
| 相关 Token | `TreeItemMargin`、`HeaderHeight` |
| 稳定性 | stable since 6.0 |

#### `itemSwitcher`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeViewItem` |
| Part | `itemSwitcher` |
| Selector | `.semantic-item-switcher` |
| SelectorRoute | `/template/ .semantic-scope-header /template/ .semantic-item-switcher` |
| Style Type | `TreeViewItemItemSwitcherStyle` |
| ContractType | `ToggleButton` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 header 模板中的 `NodeSwitcherButton#PART_NodeSwitcherButton` |
| 职责 | 统一表示节点展开/收起 switcher 区域：展开、收起、叶子与加载图标入口；对应上游 `.ant-tree-switcher` 节点。 |
| 相关 API | `SwitcherExpandIcon`、`SwitcherCollapseIcon`、`SwitcherRotationIcon`、`SwitcherLoadingIcon`、`SwitcherLeafIcon`、`IsSwitcherRotation`、`IsLeaf`、`IsLoading`、`IsExpanded` |
| 相关 Token | `HeaderHeight`、`NodeHoverBg`、`TreeNodeSwitcherMargin`、SharedToken（`IconSize`、`IconSizeXS`、`ColorTextSecondary`） |
| 稳定性 | stable since 6.0 |

#### `itemIcon`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeViewItem` |
| Part | `itemIcon` |
| Selector | `.semantic-item-icon` |
| SelectorRoute | `/template/ .semantic-scope-header /template/ .semantic-item-icon` |
| Style Type | `TreeViewItemItemIconStyle` |
| ContractType | `IconPresenter` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 header 模板中的 `IconPresenter#PART_IconPresenter` |
| 职责 | 统一表示节点图标区域：`Icon` 内容的呈现、尺寸与边距；对应上游 `.ant-tree-iconEle` 节点。 |
| 相关 API | `Icon`、`IsShowIcon`、`IsShowLeafIcon` |
| 相关 Token | `TreeNodeIconMargin`、SharedToken（`IconSize`） |
| 稳定性 | stable since 6.0 |

#### `itemTitle`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeViewItem` |
| Part | `itemTitle` |
| Selector | `.semantic-item-title` |
| SelectorRoute | `/template/ .semantic-scope-header /template/ .semantic-item-title` |
| Style Type | `TreeViewItemItemTitleStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 每个 header 模板中的 `ContentPresenter#HeaderPresenter` |
| 职责 | 统一表示节点标题文字区域：`Header` / `HeaderTemplate` 内容的呈现、颜色、字体与对齐；对应上游 `.ant-tree-title` 节点。 |
| 相关 API | `Header`、`HeaderTemplate`、`Content`、`ContentTemplate` |
| 相关 Token | SharedToken（`ColorText`、`ColorTextDisabled`） |
| 稳定性 | stable since 6.0 |

### 2.3 marker 放置与路由

`root` 是隐式 Part，不声明 `.semantic-root` marker。非 root Part 的 marker 放置：

- `TreeView.item` 与 `TreeViewItem.item` 的 marker `.semantic-item` 在 `TreeView` 与 `TreeViewItem` 的容器创建与
  prepare 路径幂等添加，覆盖用户显式 `TreeViewItem`、`ItemsSource` 数据驱动容器与递归子节点容器三条来源。
- `itemSwitcher`、`itemIcon`、`itemTitle` 的 marker 静态声明在 `TreeViewItemHeaderTheme.axaml`（header 模板）。
- `.semantic-scope-header` 静态声明在 `TreeViewItemTheme.axaml` 的 `TreeViewItemHeader#Header` 上，是 `TreeViewItem`
  模板到 `TreeViewItemHeader` 模板之间的路由跳点，不发布为 Part。

`TreeViewItem` owner 的四个运行时 Part（`item`、`itemSwitcher`、`itemIcon`、`itemTitle`）都位于 `TreeViewItem` 容器
及其模板内部，descriptor 统一声明 `RuntimeCreated=true`，生成器不按 owner 主题资产做静态校验。

- `item` 的 route `> .semantic-item` 经一步 `>` 直达子容器：子 `TreeViewItem` 容器的逻辑父级是当前 `TreeViewItem`
  owner 本身。
- `itemSwitcher` / `itemIcon` / `itemTitle` 需要两次 `/template/` 跳点：先进入 `TreeViewItem` 模板命中
  `TreeViewItemHeader#Header` 上的 `.semantic-scope-header` 跳点，再进入 `TreeViewItemHeader` 模板命中对应 marker。

`ContractType` 只定义 Setter 可以稳定依赖的最低 public 类型，并通过 `x:SetterTargetType` 提供 AXAML 编译期类型
上下文；它不参与 `.semantic-*` 的身份匹配。`itemSwitcher` 的真实节点 `NodeSwitcherButton` 是 internal 类型，因此
ContractType 使用其公开基类 `ToggleButton`（与 Timeline 对 internal `TimelineIndicator` 节点使用公开 `Border`、
Calendar 对 internal cell 使用 `TemplatedControl` 同一决策）。`itemIcon` 的节点 `IconPresenter` 与 `itemTitle` 的
节点 `ContentPresenter` 均为公开类型，直接取节点真实 public 类型作为最低依赖类型。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/TreeView/Themes/TreeViewTheme.axaml`

```xml
<Border Name="Frame">
    <Panel>
        <ScrollViewer>
            <ItemsPresenter Name="ItemsPresenter" />
        </ScrollViewer>
        <ContentPresenter Name="EmptyIndicator" />
        <Empty Name="DefaultEmptyIndicator" />
    </Panel>
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
TreeView
  -> NodeSwitcherButton (control theme, NodeSwitcherButtonTheme.axaml)
     -> Panel#RootLayout (template-stable)
        -> IconPresenter#CurrentIconPresenter (internal-observable)
  -> TreeViewItemHeader (control theme, TreeViewItemHeaderTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> Grid#ItemsLayout (template-stable)
           -> NodeSwitcherButton#PART_NodeSwitcherButton (template-stable)
           -> CheckBox#ToggleCheckbox (template-stable)
           -> RadioButton#ToggleRadio (template-stable)
           -> IconPresenter#PART_IconPresenter (template-stable)
           -> Border#PART_HeaderContentFrame (template-stable)
              -> Panel (template-stable)
                 -> ContentPresenter#HeaderPresenter (internal-observable)
                 -> TextBlock#FilterHighlighter (template-stable)
  -> TreeViewItem (item container control theme, TreeViewItemTheme.axaml)
     -> StackPanel (template-stable)
        -> TreeViewItemHeader#Header (internal-observable)
        -> LayoutAwareMotionActor#PART_ItemsPresenterMotionActor (template-stable)
           -> ItemsPresenter#ItemsPresenter (internal-observable)
  -> TreeView (control theme, TreeViewTheme.axaml)
     -> Border#Frame (template-stable)
        -> Panel (template-stable)
           -> ScrollViewer (template-stable)
              -> ItemsPresenter#ItemsPresenter (internal-observable)
           -> ContentPresenter#EmptyIndicator (internal-observable)
           -> Empty#DefaultEmptyIndicator (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `TreeView` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `NodeSwitcherButton` | control theme | `NodeSwitcherButtonTheme.axaml` | TreeView | `CurrentIcon`, `IsCurrentIconVisible`, `RotationIconRenderTransform` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (Panel) | `NodeSwitcherButtonTheme.axaml` | NodeSwitcherButton | `CurrentIcon`, `IsCurrentIconVisible`, `RotationIconRenderTransform` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CurrentIconPresenter` | template node (IconPresenter) | `NodeSwitcherButtonTheme.axaml` | NodeSwitcherButton | `CurrentIcon`, `IsCurrentIconVisible`, `RotationIconRenderTransform` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TreeViewItemHeader` | control theme | `TreeViewItemHeaderTheme.axaml` | TreeView | `Background`, `BorderThickness`, `Content`, `ContentFrameBackground`, `ContentTemplate`, `CornerRadius` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Frame` | template node (PixelAlignedBorder) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `Background`, `BorderThickness`, `Content`, `ContentFrameBackground`, `ContentTemplate`, `CornerRadius` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsLayout` | template node (Grid) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `Content`, `ContentFrameBackground`, `ContentTemplate`, `FilterHighlightRuns`, `GroupName`, `Icon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_NodeSwitcherButton` | template node (NodeSwitcherButton) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `IsExpanded`, `IsLoading`, `IsMotionEnabled`, `SwitcherCollapseIcon`, `SwitcherExpandIcon`, `SwitcherLeafIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ToggleCheckbox` | template node (CheckBox) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `IsChecked` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ToggleRadio` | template node (RadioButton) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `GroupName`, `IsChecked` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_IconPresenter` | template node (IconPresenter) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `Icon`, `IconEffectiveVisible`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HeaderContentFrame` | template node (Border) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `Content`, `ContentFrameBackground`, `ContentTemplate`, `FilterHighlightRuns`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `Content`, `ContentTemplate`, `FilterHighlightRuns` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HeaderPresenter` | template node (ContentPresenter) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `FilterHighlighter` | template node (TextBlock) | `TreeViewItemHeaderTheme.axaml` | TreeViewItemHeader | `FilterHighlightRuns` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TreeViewItem` | item container control theme | `TreeViewItemTheme.axaml` | 用户代码 / 控件宿主 | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StackPanel` | template node (StackPanel) | `TreeViewItemTheme.axaml` | TreeViewItem | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Header` | template node (TreeViewItemHeader) | `TreeViewItemTheme.axaml` | TreeViewItem | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ItemsPresenterMotionActor` | template node (LayoutAwareMotionActor) | `TreeViewItemTheme.axaml` | TreeViewItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `TreeViewItemTheme.axaml` | TreeViewItem | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TreeView` | control theme | `TreeViewTheme.axaml` | 用户代码 / 控件宿主 | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsDefaultEmptyIndicatorVisible`, `IsEffectiveEmptyVisible`, `ItemsPanel` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `TreeViewTheme.axaml` | TreeView | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsDefaultEmptyIndicatorVisible`, `IsEffectiveEmptyVisible`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `TreeViewTheme.axaml` | TreeView | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsDefaultEmptyIndicatorVisible`, `IsEffectiveEmptyVisible`, `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `TreeViewTheme.axaml` | TreeView | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `EmptyIndicator` | template node (ContentPresenter) | `TreeViewTheme.axaml` | TreeView | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsEffectiveEmptyVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DefaultEmptyIndicator` | template node (Empty) | `TreeViewTheme.axaml` | TreeView | `EmptyIndicatorPadding`, `IsDefaultEmptyIndicatorVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| API | 语义 |
| --- | --- |
| `IsAutoExpandParent` | 子节点展开时是否自动展开父节点。 |
| `IsDraggable` | 是否启用节点拖拽重排。 |
| `IsShowIcon` | 是否显示节点图标。 |
| `IsShowLine` | 是否显示树形连线。 |
| `IsDefaultExpandAll` | 加载后是否默认展开全部节点。 |
| `NodeHoverMode` | 节点 hover 背景范围，支持 `Default`、`Block`、`WholeLine`。 |
| `SwitcherExpandIcon` / `SwitcherCollapseIcon` / `SwitcherRotationIcon` / `SwitcherLoadingIcon` / `SwitcherLeafIcon` | switcher 图标入口。 |
| `IsShowLeafIcon` | 是否显示叶子节点图标。 |
| `IsSwitcherRotation` | 是否使用旋转图标表达展开收起。 |
| `IsSelectable` | 是否允许节点选择。 |
| `IsSelectOnRightClick` | 右键节点时是否更新选择。 |
| `ToggleType` | 节点勾选模式，支持 none、checkbox、radio。 |
| `IsCheckStrictly` | checkbox 模式下是否关闭父子级级联。 |
| `DefaultSelectedPaths` / `DefaultCheckedPaths` / `DefaultExpandedPaths` | 初始选择、勾选和展开路径入口。 |
| `DataLoader` / `AsyncLoadTimeout` | 异步加载子节点入口和超时时间。 |
| `Filter` / `FilterValue` / `FilterValueSelector` / `FilterStrategy` | 过滤、高亮、展开路径和隐藏未命中节点入口。 |
| `FilterResultCount` | 当前过滤命中数量。 |
| `EmptyIndicator` / `EmptyIndicatorTemplate` / `IsShowEmptyIndicator` / `EmptyIndicatorPadding` | 空状态展示入口。 |
| `IsMotionEnabled` / `OpenMotion` / `CloseMotion` | 展开收起动效入口。 |

## Pseudo Classes

TreeView 的公共契约由 TreeView API、TreeViewItem API、节点数据 API、事件 API、template part 和伪类组成。

TreeView 核心 API：

| API | 语义 |
| --- | --- |
| `IsAutoExpandParent` | 子节点展开时是否自动展开父节点。 |
| `IsDraggable` | 是否启用节点拖拽重排。 |
| `IsShowIcon` | 是否显示节点图标。 |
| `IsShowLine` | 是否显示树形连线。 |
| `IsDefaultExpandAll` | 加载后是否默认展开全部节点。 |
| `NodeHoverMode` | 节点 hover 背景范围，支持 `Default`、`Block`、`WholeLine`。 |

## State Flow

TreeView 的状态模型由节点状态、选择状态、勾选状态、展开状态、过滤状态、异步加载状态、拖拽状态和空状态组成。

选择行为：

- `IsSelectable=false` 时不允许节点被选中，并清空 TreeView 当前选择。
- `IsSelectOnRightClick=false` 时，右键不更新选择。
- `SelectionMode` 继承 Avalonia `TreeView` 语义，单选使用 `SelectedItem`，多选使用 `SelectedItems`。
- Form 集成以单选 / 多选模式分别读取和写入 `SelectedItem` 或 `SelectedItems`，并保持原始节点对象 / 列表实例，不把节点值转换为字符串。

勾选行为：

- `ToggleType=None` 时不显示勾选入口。
- `ToggleType=CheckBox` 时显示 checkbox。
- `ToggleType=Radio` 时只在叶子节点显示 radio。
- `IsCheckStrictly=false` 时 checkbox 勾选会级联子树，并根据子级状态更新父级 true / false / null。
- `IsCheckStrictly=true` 时 checkbox 只同步当前节点，不级联父子级。
- `CheckedItems` 是当前勾选数据集合，变化会同步已实现容器状态并触发 `CheckedItemsChanged`。

展开行为：

- switcher 触发展开收起。
- `IsDefaultExpandAll=true` 时加载后展开全部节点，并优先于 `DefaultExpandedPaths`。
- 默认路径通过 `TreeNodePath` 和 `ItemKey` / `Value` 匹配。
- 展开收起动效由 `IsMotionEnabled`、`OpenMotion`、`CloseMotion` 和 `MotionDuration` 控制。

过滤行为：

- `Filter`、`FilterValue` 和 `FilterValueSelector` 共同决定节点是否命中。
- `FilterStrategy` 控制高亮 match、整行高亮、加粗、展开命中路径和隐藏未命中节点。
- `FilterResultCount` 表示命中节点数量。
- 过滤模式下空状态依据 `FilterResultCount` 判断。

拖拽行为：

- `IsDraggable=true` 时，左键按下并超过拖拽阈值后进入拖拽。
- TreeView 创建拖拽预览和 drop indicator。
- drop 目标支持插入到根、插入到兄弟前后、插入到目标节点内部。
- 不允许把节点 drop 到自身或自身后代内。
- 拖拽命中以已实现的 `TreeViewItem` 容器计算，结构修改以数据源为权威。
- drop 操作通过 TreeView 内部数据控制器移动 root 集合或节点 `Children`，不直接修改生成容器的 `Items`。
- 节点移动是结构重排，不是业务删除；选中、勾选和展开状态按节点身份保留。

异步加载行为：

- `DataLoader` 只在 `ItemsSource` 数据驱动场景下使用。
- 未加载节点点击 switcher 时触发加载。
- 加载中节点显示 loading switcher icon。
- 加载成功后把返回子节点写入目标节点 `Children`，并展开目标节点。

绑定型节点行为：

- `TreeItemNode` 的定位是轻量数据源节点，不承载 `DynamicResource`、Avalonia styled binding target 或资源宿主职责。
- 需要在 XAML 中直接绑定节点属性，或把节点 `Header`、`Icon`、状态属性设置为 `DynamicResource` 时，使用独立的 `BindableTreeItemNode`。
- `BindableTreeItemNode` 进入 TreeView 容器生命周期时，由 owner TreeView / TreeViewItem attach scoped resource host；离开容器、detach、re-template 或 container recycle 时释放 attach token。
- 绑定型节点属性变化应同步当前生成的 `TreeViewItem` 容器；容器交互导致的 checked、selected、expanded 等状态变化也应按契约回写节点状态。
- 自定义 `ITreeItemNode` 仍按普通数据模型处理；TreeView 不要求用户模型继承 `BindableTreeItemNode`。

## Theme and Token Boundaries

TreeView 主题按 root、item、header、switcher 四层组织。

```text
TreeViewTheme
  root scroll viewer
  items presenter
  empty indicator

TreeViewItemTheme
  header
  child items motion actor

TreeViewItemHeaderTheme
  switcher
  checkbox / radio
  icon
  header content frame
  filter highlighter

NodeSwitcherButtonTheme
  current icon presenter
  hover background
  rotation / loading transition
```

视觉规则：

- `NodeHoverMode=Default` 时 header 内容背景按内容宽度绘制。
- `NodeHoverMode=Block` 时 header 内容背景横向拉伸到剩余区域。
- `NodeHoverMode=WholeLine` 时背景由 TreeViewItem 行级绘制，覆盖整行宽度。
- disabled 节点应使用 disabled 文本色和弱化图标，不应保留可交互 hover 视觉。
- filter match 时显示 `FilterHighlighter`，未命中时显示普通 `HeaderPresenter`。
- `IsShowLine=true` 时，TreeViewItem 自绘树形连线。
- drag indicator 由 TreeView 自绘，不进入节点模板内部。

TreeViewToken 提供节点高度、hover / selected 背景、目录树选中颜色、节点间距、header padding、switcher / icon 间距、拖拽指示线宽和过滤高亮色。Token 详情见 [TreeView Token 设计](token.md)。

Token 边界：

TreeViewToken 是 TreeView 的组件级设计变量层。它把全局颜色、尺寸、间距、线宽和状态色转换为 TreeView 节点 header、switcher、icon、拖拽指示器和过滤高亮可消费的语义值。

TreeViewToken 服务以下主题和控件：

- `TreeViewTheme.axaml`
- `TreeViewItemTheme.axaml`
- `TreeViewItemHeaderTheme.axaml`
- `NodeSwitcherButtonTheme.axaml`
- `TreeView` drag indicator render state
- `TreeViewItem` line render state
- `TreeViewItemHeader` hover / selected / filter state

TreeViewToken 不承载 `SelectedItem`、`SelectedItems`、`CheckedItems`、`IsExpanded`、`IsChecked`、`IsFilterMode`、`FilterResultCount`、`IsDragging`、`DragIndicatorRenderInfo` 等实例状态。这些状态由 C# 状态模型、容器属性和主题 selector 处理。

## Customization Boundaries

维护 TreeView 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 TreeView / TreeViewItem / ITreeItemNode public API。
- `TreeItemNode` 保持轻量 POCO / record 数据节点定位，不直接改造成 `AvaloniaObject`。
- 绑定型节点能力通过独立 `BindableTreeItemNode` 承载，不能通过破坏 `TreeItemNode` record 语义、init 属性或相等性来实现。
- `DefaultSelectedPaths`、`DefaultCheckedPaths`、`DefaultExpandedPaths` 是默认状态入口，不是持续受控状态。
- `IsDefaultExpandAll=true` 优先于 `DefaultExpandedPaths`。
- `SelectedItem` / `SelectedItems` 的 Avalonia 选择语义不变。
- `CheckedItems` 与已实现容器的 `IsChecked` 必须双向同步，内部同步不得递归触发重复事件。
- `IsCheckStrictly=false` 时 checkbox 保持父子级级联和半选语义；`true` 时只同步当前节点。
- `ToggleType=Radio` 只在叶子节点显示 radio，并遵守 `GroupName` 分组。
- `TreeNodePath` 匹配优先使用 `ItemKey`，没有 `ItemKey` 时才使用 `Value` 字符串。
- `ItemsSource` 变化后应尽量按节点身份路径恢复运行期选择和勾选状态，再回放默认状态。
- 多选模式下 `SelectedItems` 是运行期选择恢复的权威来源，`ItemsSource` 变化或容器首次回放不能因 `SelectedItem` 非空而把多选折叠成单选。
- filter 清除后必须恢复过滤前节点可见性、展开状态和高亮状态。
- 异步加载只在数据节点模型下写入 `ITreeItemNode.Children`，不修改普通手写 `TreeViewItem` 子树。
- 非 Visual `AvaloniaObject` 节点只要承载 `DynamicResource` 或 token-resource binding，就必须使用 scoped resource host，并有明确 attach/release 路径。
- 拖拽不得允许节点 drop 到自身或自身后代。
- 拖拽结构修改必须通过内部数据控制器执行，不能在 `ItemsSource` 场景直接写 `TreeView.Items` 或 `TreeViewItem.Items`。
- TreeView 维护节点到父级、兄弟集合和索引的内部索引；拖拽过程中不能为每次 drop 全树扫描定位节点。
- 跨父级移动后，节点 `ParentNode`、root / child 集合、选中集合、勾选集合和展开状态必须保持一致。
- Template part 名称和职责不擅自修改。
- Token 名称和语义不擅自重命名或迁移为实例状态。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- public API、事件、Avalonia 属性字段和 CLR wrapper 不擅自变更。
- `TreeView.cs` 保留公共属性、事件、公共方法、生命周期和接口入口；复杂内部逻辑可继续按功能拆分 partial。
- `TraverseTreeViewPath` 是路径回放和路径操作的统一入口。
- 默认状态回放顺序保持 selected、checked、filter、expanded。
- ItemsSource 变化先尝试恢复运行期状态，再回退默认状态。
- 多选模式的 ItemsSource 变化恢复必须以 `SelectedItems` 为权威；不能因为 `SelectedItem` 非空而丢弃其它已选节点。
- `CheckedItemsSyncScope` 必须包裹内部批量 checked 集合更新。
- filter 进入时备份上下文，退出时恢复。
- `TreeViewItemHeader` 替换 `PART_HeaderContentFrame` 时必须解除旧 pointer 事件。
- `DefaultTreeViewInteractionHandler.Detach` 必须释放 pointer、input manager、root handler 和 radio group 关系。
- `NodeSwitcherButton.Toggle` 在节点加载中不重复触发展开。
- drag preview、drag-over、drop target 和 indicator 状态必须在拖拽完成或取消时清理。
- 拖拽结构修改只能通过 `TreeDataController` 执行，不能直接写生成容器 `Items`。
- `TreeNodeIndex` 是 drop 定位的权威索引；drag pointer move 不得触发全树数据遍历。
- 节点 move 不能被当成 remove 清理选中、勾选或展开状态。
- root 数据源、节点 `Children`、parent node 和索引必须在移动后保持一致。
- `TreeItemNode` 保持轻量数据节点定位，不承载 Avalonia 属性系统。
- `BindableTreeItemNode` 的 resource host attach、属性订阅和容器同步必须与容器生命周期成对释放。
- 绑定型节点不能永久保存当前 `TreeViewItem`、header、template part 或 visual container。
- Semantic Part 的 marker 放置（`TreeViewItemTheme.axaml` 的 `semantic-scope-header` 静态跳点、
  `TreeViewItemHeaderTheme.axaml` 的三个静态 Part marker、容器创建/prepare 路径的 `.semantic-item`）属于维护不变量：
  状态切换、容器复用/回收、items 集合变化与模板重应用不得增删 marker，默认主题不得消费 `.semantic-*` selector。
- `TreeViewItemHeader` / `NodeSwitcherButton` 不得改为 public owner 或承载独立 Semantic descriptor；`itemSwitcher` 的
  `ContractType` 保持公开基类 `ToggleButton`，不把 internal `NodeSwitcherButton` 泄漏进公共契约。
