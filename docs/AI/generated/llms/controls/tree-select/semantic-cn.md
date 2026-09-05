# TreeSelect 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

TreeSelect 是唯一 Semantic owner，公开 13 个 Semantic Part（语义对齐：`item` / `itemContent` /
`itemRemove` 对应上游多选标签的 `item` 分组；`popup.*` 采用 AtomUI Select 家族的
`root` / `list` / `listItem` 命名，对应上游 TreeSelect `popup` 分组的 `root` / `item` /
`itemTitle` / `itemSwitcher`——弹层树节点的标题、切换器等内部槽位由 `TreeViewItem` 自身已声明的
`itemTitle` / `itemSwitcher` / `itemIcon` / `itemIndicator` Semantic Part 承载，不经 TreeSelect 重复
发布）。声明位于 `TreeSelect.SemanticParts.cs` partial 文件。

触发区部件的 marker 位于 `TreeSelectTheme.axaml` 宿主模板内：`prefix` / `suffix` 借用共享
`AddOnDecoratedBoxTheme` 的 `.semantic-scope-prefix` / `.semantic-scope-suffix` scope 锚点路由到宿主模板
投影给 decorated box 的内容节点（与 Select / Cascader 同构）；`content` / `placeholder` / `input`
直接标注宿主模板节点。`input` 额外在多选模式的运行时搜索框上注入同一 marker（共享
`SelectTagAwareTextBox` 创建搜索框时注入，Cascader 复用同一 marker），使过滤输入在非单选态同样可被
语义高亮与样式命中。`clear` / `item` / `itemContent` / `itemRemove` 声明 `CrossNestedOwners=true`：
`clear` 的物理节点在共享 `SelectHandle` 自有模板内；多选标签的物理节点在共享标签机制内——`item`
的标记由 `SelectTagAwareTextBox` 在标签容器创建时注入，`itemContent` / `itemRemove` 的标记位于共享
`TagTheme` 模板（`SelectTag : Tag` 复用其模板），二者经 `item` 部件（RuntimeCreated，
ContractType=`Tag`）承转主题链完成校验。弹层三部件位于 owner 自有的 Popup 模板内：`popup.root`
标注在宿主模板的 `PopupFrame` 静态节点上，`popup.list`（候选树）与 `popup.listItem`（树节点容器）
的 marker 在运行时容器创建路径注入。

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `root` |
| Selector | TreeSelect 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `TreeSelect` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | TreeSelect owner |
| 职责 | TreeSelect root 是树数据源、选择/勾选、过滤、弹层与状态的组织边界。 |
| 相关 API | 全部 TreeSelect public API |
| 相关 Token | TreeSelectToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `prefix`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `prefix` |
| Selector | `.semantic-prefix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-prefix > .semantic-prefix` |
| Style Type | `TreeSelectPrefixStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TreeSelectTheme.axaml` 中投影给 `TreeSelectAddOnDecoratedBox.ContentLeftAddOn` 的 `AddOnContentPresenter`（经 `$parent[atom:TreeSelect]` 编译绑定呈现公共 API 值） |
| 职责 | 选择框内容前缀区域，承载 `ContentLeftAddOn` 用户内容，在内容框内联展示。 |
| 相关 API | `ContentLeftAddOn`、`ContentLeftAddOnTemplate` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `content`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `content` |
| Selector | `.semantic-content` |
| SelectorRoute | `/template/ .semantic-content` |
| Style Type | `TreeSelectContentStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TreeSelectTheme.axaml` 中承载占位符、单选过滤框与多选标签容器的 Panel |
| 职责 | 选择内容面板，组织占位符、过滤输入、选中结果与多选标签的布局。 |
| 相关 API | `PlaceholderText`、`IsFilterEnabled`、`IsMultiple` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `placeholder`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `placeholder` |
| Selector | `.semantic-placeholder` |
| SelectorRoute | `/template/ .semantic-content > .semantic-placeholder` |
| Style Type | `TreeSelectPlaceholderStyle` |
| ContractType | `TextBlock` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TreeSelectTheme.axaml` 的 `PlaceholderText` TextBlock |
| 职责 | 未选择任何项时显示的占位符文本。 |
| 相关 API | `PlaceholderText`、`PlaceholderForeground` |
| 相关 Token | SharedToken（ColorTextPlaceholder） |
| 稳定性 | stable since 6.0 |

#### `input`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `input` |
| Selector | `.semantic-input` |
| SelectorRoute | `/template/ .semantic-content > .semantic-input` |
| Style Type | `TreeSelectInputStyle` |
| ContractType | `TextBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 单选过滤输入 `PART_SingleFilterInput`；多选模式下同一 marker 由共享 `SelectTagAwareTextBox` 注入到运行时创建的行内搜索框 |
| 职责 | 过滤模式的搜索输入框（单选态为模板节点，多选态为标签区运行时搜索框）。 |
| 相关 API | `IsFilterEnabled`、`FilterValue` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `suffix`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `suffix` |
| Selector | `.semantic-suffix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-suffix > .semantic-suffix` |
| Style Type | `TreeSelectSuffixStyle` |
| ContractType | `StackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TreeSelectTheme.axaml` 中投影给 `ContentRightAddOn` 的水平 StackPanel（含最大数量指示、自定义后缀与 SelectHandle） |
| 职责 | 选择框后缀区域，承载最大数量指示、用户后缀内容与选择 handle。 |
| 相关 API | `ContentRightAddOn`、`ContentRightAddOnTemplate`、`SuffixIcon`、`MaxCount` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `clear`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `clear` |
| Selector | `.semantic-clear` |
| SelectorRoute | `>> .semantic-scope-handle /template/ .semantic-clear` |
| Style Type | `TreeSelectClearStyle` |
| ContractType | `Button` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossNestedOwners | `true` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 共享 `SelectHandle` 模板内的清除按钮（`SelectHandleTheme.axaml`） |
| 职责 | 后缀 handle 内的清除按钮，启用 `IsAllowClear` 时渲染。 |
| 相关 API | `IsAllowClear` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `item`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `>> .semantic-scope-tags >> .semantic-item` |
| Style Type | `TreeSelectItemStyle` |
| ContractType | `Tag` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossNestedOwners | `true` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 多选标签容器（`SelectTagAwareTextBox` 创建 `SelectTag` 时注入 marker） |
| 职责 | 多选模式下选择器中的选中标签。 |
| 相关 API | `SelectedItems`、`IsMultiple`、`MaxTagCount` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `itemContent`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `itemContent` |
| Selector | `.semantic-item-content` |
| SelectorRoute | `>> .semantic-scope-tags >> .semantic-item /template/ .semantic-item-content` |
| Style Type | `TreeSelectItemContentStyle` |
| ContractType | `TextBlock` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossNestedOwners | `true` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 共享 `TagTheme.axaml` 模板内的内容节点 |
| 职责 | 选中标签内的文本内容。 |
| 相关 API | 无（随 `item` 呈现） |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `itemRemove`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `itemRemove` |
| Selector | `.semantic-item-remove` |
| SelectorRoute | `>> .semantic-scope-tags >> .semantic-item /template/ .semantic-item-remove` |
| Style Type | `TreeSelectItemRemoveStyle` |
| ContractType | `IconButton` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossNestedOwners | `true` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 共享 `TagTheme.axaml` 模板内的移除按钮 |
| 职责 | 选中标签内的移除按钮。 |
| 相关 API | 无（随 `item` 呈现） |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `popup.root`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `popup.root` |
| Selector | `.semantic-popup-root` |
| SelectorRoute | `/template/ .semantic-popup-root` |
| Style Type | `TreeSelectPopupRootStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TreeSelectTheme.axaml` 中 `PART_Popup` 的静态子节点 `PopupFrame` |
| 职责 | 候选弹层的根边框节点，承载弹层内容根视觉。 |
| 相关 API | `PopupContentPadding`、`MaxPopupHeight`、`EffectivePopupWidth` |
| 相关 Token | PopupToken、TreeSelectToken（MinPopupWidth） |
| 稳定性 | stable since 6.0 |

#### `popup.list`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `popup.list` |
| Selector | `.semantic-popup-list` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-list` |
| Style Type | `TreeSelectPopupListStyle` |
| ContractType | `Control` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `EnsurePopupContent` 运行时创建的 `TreeSelectTreeView`（创建时注入 marker） |
| 职责 | 弹层内的候选树容器，承载树形候选数据、展开与勾选状态。 |
| 相关 API | `ItemsSource`、`TreeViewToggleType`、`IsDefaultExpandAll` |
| 相关 Token | TreeSelectToken |
| 稳定性 | stable since 6.0 |

#### `popup.listItem`

| 字段 | 值 |
| --- | --- |
| Owner | `TreeSelect` |
| Part | `popup.listItem` |
| Selector | `.semantic-popup-list-item` |
| SelectorRoute | `/template/ .semantic-popup-root >> .semantic-popup-list-item` |
| Style Type | `TreeSelectPopupListItemStyle` |
| ContractType | `TemplatedControl` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `TreeSelectTreeView` 容器创建路径生成的 `TreeViewSelectTreeViewItem`（构造时注入 marker，覆盖任意嵌套层级与回收） |
| 职责 | 候选树中的单个树节点条目，运行时创建。 |
| 相关 API | 无（随 `popup.list` 呈现） |
| 相关 Token | TreeSelectToken、TreeViewToken |
| 稳定性 | stable since 6.0 |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectTheme.axaml`

```xml
<Panel>
    <TreeSelectAddOnDecoratedBox Name="{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}">
        <Panel>
            <TextBlock Name="PlaceholderText" />
            <SelectFilterTextBox Name="PART_SingleFilterInput" />
            <SelectTagAwareTextBox Name="SelectedItemsBox" />
        </Panel>
    </TreeSelectAddOnDecoratedBox>
    <Popup Name="PART_Popup">
        <Border Name="PopupFrame" />
    </Popup>
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
TreeSelect
  -> TreeSelectAddOnDecoratedBox (control theme, TreeSelectAddOnDecoratedBoxTheme.axaml)
  -> TreeSelect (control theme, TreeSelectTheme.axaml)
     -> Panel (template-stable)
        -> TreeSelectAddOnDecoratedBox#{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart} (internal-observable)
           -> Panel (template-stable)
              -> TextBlock#PlaceholderText (template-stable)
              -> SelectFilterTextBox#PART_SingleFilterInput (template-stable)
              -> SelectTagAwareTextBox#SelectedItemsBox (template-stable)
        -> Popup#PART_Popup (template-stable)
           -> Border#PopupFrame (template-stable)
  -> TreeViewSelectTreeViewItem (item container control theme, TreeSelectTreeViewItemTheme.axaml)
     -> StackPanel (template-stable)
        -> TreeViewItemHeader#Header (template-stable)
        -> LayoutAwareMotionActor#PART_ItemsPresenterMotionActor (template-stable)
           -> ItemsPresenter#ItemsPresenter (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `TreeSelect` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `TreeSelectAddOnDecoratedBox` | control theme | `TreeSelectAddOnDecoratedBoxTheme.axaml` | TreeSelect | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `TreeSelect` | control theme | `TreeSelectTheme.axaml` | 用户代码 / 控件宿主 | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `DataValidationErrors`, `EffectivePopupWidth`, `EffectiveSelectedItems`, `FontFamily` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `TreeSelectTheme.axaml` | TreeSelect | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `DataValidationErrors`, `EffectivePopupWidth`, `EffectiveSelectedItems`, `FontFamily` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}` | template node (TreeSelectAddOnDecoratedBox) | `TreeSelectTheme.axaml` | TreeSelect | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `DataValidationErrors`, `EffectiveSelectedItems`, `FontFamily`, `FontSize` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PlaceholderText` | template node (TextBlock) | `TreeSelectTheme.axaml` | TreeSelect | `IsPlaceholderTextVisible`, `PlaceholderForeground`, `PlaceholderText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SingleFilterInput` | template node (SelectFilterTextBox) | `TreeSelectTheme.axaml` | TreeSelect | `FontFamily`, `FontSize`, `FontStyle`, `FontWeight`, `IsShowOverflowTip`, `OverflowTipDelay` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectedItemsBox` | template node (SelectTagAwareTextBox) | `TreeSelectTheme.axaml` | TreeSelect | `EffectiveSelectedItems`, `Height`, `IsDropDownOpen`, `IsFilterEnabled`, `IsResponsiveTagMode`, `IsShowOverflowTip` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Popup` | template node (Popup) | `TreeSelectTheme.axaml` | TreeSelect | `EffectivePopupWidth`, `IsDropDownOpen`, `MaxPopupHeight`, `PopupContentPadding`, `PopupPlacement`, `ShouldUseOverlayPopup` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PopupFrame` | template node (Border) | `TreeSelectTheme.axaml` | TreeSelect | `EffectivePopupWidth`, `MaxPopupHeight`, `PopupContentPadding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TreeViewSelectTreeViewItem` | item container control theme | `TreeSelectTreeViewItemTheme.axaml` | TreeSelect | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `StackPanel` | template node (StackPanel) | `TreeSelectTreeViewItemTheme.axaml` | TreeViewSelectTreeViewItem | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Header` | template node (TreeViewItemHeader) | `TreeSelectTreeViewItemTheme.axaml` | TreeViewSelectTreeViewItem | `BorderThickness`, `FilterHighlightForeground`, `FilterHighlightWords`, `FilterStrategy`, `Focusable`, `GroupName` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenterMotionActor` | template node (LayoutAwareMotionActor) | `TreeSelectTreeViewItemTheme.axaml` | TreeViewSelectTreeViewItem | `ItemsPanel` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `TreeSelectTreeViewItemTheme.axaml` | TreeViewSelectTreeViewItem | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_InputControlFrame` | `TreeSelectAddOnDecoratedBox` | `InputControlFrame` / AddOnDecoratedBox 组合、Addon、EffectiveStatus、CompactSpace 和多选布局承载。 |
| `PART_SingleFilterInput` | `SelectFilterTextBox` | 单选模式结果显示和搜索输入。 |
| `SelectedItemsBox` | `SelectTagAwareTextBox` | 多选和勾选模式已选 tag 展示。 |
| `PART_SelectMaxCountIndicator` | `SelectMaxCountIndicator` | 最大选择数量提示。 |
| `PART_ContentRightAddOnPresenter` | `ContentPresenter` | 用户内部右侧内容承载。 |
| `PART_SelectHandle` | `SelectHandle` | 展开、loading、清除和 Form feedback 图标入口。 |
| `PART_Popup` | `Popup` | 候选弹层宿主。 |
| `PopupFrame` | `Border` | 懒创建的候选弹层外壳。 |
| `PART_TreeView` | `TreeSelectTreeView` | 懒创建的树候选控件。 |

## Pseudo Classes

- Template part、Token 名称、伪类和主题 selector 不得在未授权情况下重命名或删除。

## State Flow

TreeSelect 的核心状态流：

```text
ItemsSource / Items
      ↓
TreeSelectTreeView
      ↓
SelectedItem / SelectedItems
      ↓
SingleFilterInput / SelectedItemsBox
      ↓
Form value + MaxCount state
```

选择模式语义：

- 单选模式使用 `SelectedItem` 作为表单值，候选节点点击后关闭弹层。
- 多选模式使用 `SelectedItems` 作为表单值，候选树使用多选 selection，并通过 tag 展示结果。
- `IsTreeCheckable=true` 使用 checkbox 作为节点切换入口，TreeView selection 不再作为主要选择入口。
- `ShowCheckedStrategy` 只影响多选 tag 展示集合，不改变 `SelectedItems` 的真实值。
- `SelectedItems` 是用户拥有的受控集合。集合引用替换和 `ObservableCollection` 等 `INotifyCollectionChanged` 原地 `Add`、`Remove`、`Reset` 都必须刷新 tag、`SelectedCount`、Form value changed、候选树 selection / checked items 和最大选择数状态。

过滤行为：

- `FilterValue` 来自单选搜索输入。
- `FilterStrategy` 控制命中高亮、加粗、展开路径和隐藏不匹配节点。
- 弹层关闭后，单选模式清空过滤值并恢复结果显示。

清除行为通过 `SelectHandle.ClearRequestedEvent` 冒泡到 TreeSelect，并调用 `Clear()` 清空当前选择。

## Theme and Token Boundaries

TreeSelect 的默认视觉由 TreeSelect 专属主题、`InputControlFrame` / Select 家族 AddOnDecoratedBox、TreeView 和 PopupHost 协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `TreeSelectTheme.axaml` | 根模板、输入壳体、单选输入、多选结果区域、handle、popup 和基础 selector。 |
| `SelectTagAwareTextBox` / `SelectTag` 主题 | 多选 tag 布局、高度、关闭按钮和禁用态。 |
| `TreeView` / `TreeViewItem` 主题 | 树节点缩进、展开、勾选、图标、连线和过滤视觉。 |
| `InputControlFrameTheme` / `SharedToken` | 输入壳体边框、圆角、effective status、focus 和 CompactSpace 视觉。 |
| `PopupHostToken` | popup 阴影、圆角和 anchor margin。 |
| `TreeSelectToken` | TreeSelect 候选弹层最小宽度。 |
| `SelectToken` | TreeSelect 复用的 popup padding、多选 tag 和输入内容 padding。 |

单选结果文本和多选 tag 的完整内容提示复用共享 `OverflowTip` attached behavior。主题通过 `IsShowOverflowTip`、`OverflowTipDelay` 和 `OverflowTipPlacement` 控制提示开关、延迟和位置，实际 tooltip 仅在文本视觉溢出时托管到 `ToolTip`。

右侧 count、content add-on 和 handle 的稳定 template part 状态由 AXAML compiled ancestor binding 表达。C# 中只保留 frame layout part 到 SelectHandle 的 hover / pressed sibling 状态转发，因为该关系不是 templated parent 绑定，不能用 `TemplateBinding` 表达；该转发不改变 `InputControlFrame` 对输入表面状态的唯一 ownership。

Token 边界：

TreeSelectToken 是 TreeSelect 的控件级 Token scope，目前只承载 TreeSelect 候选弹层的最小宽度下限。输入表面的通用边框、圆角、effective status、focus ring、disabled 背景和 AddOn 结构来自 `InputControlFrameTheme`、SharedToken 和 PopupHostToken；多选 tag、popup padding 和输入内容 padding 复用 SelectToken。

TreeSelectToken 不承载以下状态：

- `ItemsSource`、`Items`、`SelectedItem`、`SelectedItems` 等数据状态。
- `FilterValue`、`IsDropDownOpen`、`IsTreeCheckable`、`IsMultiple`、`Status` 等运行状态。
- 树节点 hover、pressed、selected、checked、expanded、disabled 的当前实例状态。
- Popup 当前宽度、高度、offset、placement 或异步加载结果。

## Customization Boundaries

维护 TreeSelect 时必须保持以下不变量：

- 单选模式使用 `SelectedItem`，多选和勾选模式使用 `SelectedItems`。
- `SelectedItem` 和 `SelectedItems` 必须保持默认双向绑定，并通过 Avalonia `DataValidationErrors` 承接 binding / Form error。
- `SelectedItems` 原地变更必须与集合替换走同一套展示、Form 和候选树同步路径，不能依赖用户重新赋值。
- `IsTreeCheckable=true` 必须继续把 TreeSelect 归入多选结果模型。
- `ShowCheckedStrategy` 只能影响 `EffectiveSelectedItems`，不能改写真实 `SelectedItems`。
- `ItemsSource` 变化必须尽量按节点路径 identity 保留已有选择。
- `Clear()` 必须同时清空单选和多选状态。
- Popup 打开、关闭和 light-dismiss 语义必须继续由 `AbstractSelect` 管理。
- 重新套用模板或 detach 时必须释放旧 popup 内容、TreeView 事件订阅和模板 part relay binding。
- 右侧稳定 template part 优先使用 AXAML binding，不能把可静态表达的绑定重新搬回 C#。
- `SizeType=Custom` 必须继续以 `Middle` 作为未显式覆盖时的默认视觉基线。
- Template part、Token 名称、伪类和主题 selector 不得在未授权情况下重命名或删除。

维护不变量：

内部重构必须保持以下不变量：

- public/protected API、Avalonia property、template part、Token 和 selector 契约不变。
- 构造函数之后必须先放 TreeSelect 自身 public/protected API，再放 Form 接口区，private 实现方法必须位于接口区之后。
- 可用 AXAML 表达的模板绑定不能回退为 `BindUtils.RelayBind`。
- C# relay binding 必须有与获取路径匹配的释放路径。
- `SelectedItem` / `SelectedItems` 与 TreeView selection / checked items 的同步不能形成递归事件。
- `SelectedItems` 集合引用替换和原地变更必须刷新同一组 value-state，避免 tag、`SelectedCount`、Form 值和 popup TreeView 状态不同步。
- `ShowCheckedStrategy` 只能派生展示集合，不能改写真实选择集合。
- `ItemsSource` 替换时的选择保留必须继续使用节点路径 identity。
- popup 内容清理必须断开事件、ItemsSource、TemplatedParent 和 popup child 引用。
- Semantic Part marker 的维护边界：`TreeSelectTheme.axaml` 承载触发区静态 marker（`semantic-scope-input`、`semantic-prefix`、`semantic-suffix`、`semantic-scope-handle`、`semantic-content`、`semantic-placeholder`、`semantic-input`、`semantic-scope-tags`、`semantic-popup-root`）；共享 `SelectHandleTheme.axaml` 承载清除按钮的 `semantic-clear` marker（`clear` Part 声明 `CrossNestedOwners=true`，生成器沿 SelectHandle 主题链校验）；共享 `TagTheme.axaml` 承载 `itemContent` / `itemRemove` marker。运行时注入点：共享 `SelectTagAwareTextBox` 创建标签时追加 `TagItemClass`、创建搜索框时追加 `TagSearchInputClass`；`TreeSelect.EnsurePopupContent` 创建候选树时追加 `TreeSelectSemanticParts.PopupListClass` 并显式 `SetTemplatedParent(this)`；`TreeViewSelectTreeViewItem` 构造函数追加 `TreeSelectSemanticParts.PopupListItemClass`。marker 随容器实例创建一次，prepare/clear/recycle 路径不得增删；`PopupFrame` 以 `Popup.Child` 取回，不在控件中缓存字段。
