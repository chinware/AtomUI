# Cascader 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Cascader 是唯一 Semantic owner，公开 13 个 Semantic Part（与上游 Cascader 的 Semantic Part
语义对齐：`item` / `itemContent` / `itemRemove` 对应上游多选标签的 `item` 分组，`popup.*` 对应上游
`popup` 分组的 `root` / `list` / `listItem`）。声明位于 `Cascader.SemanticParts.cs` partial 文件。

触发区部件的 marker 位于 `CascaderTheme.axaml` 宿主模板内：`prefix` / `suffix` 借用共享
`AddOnDecoratedBoxTheme` 的 `.semantic-scope-prefix` / `.semantic-scope-suffix` scope 锚点路由到宿主模板
投影给 decorated box 的内容节点（与 LineEdit 同构）；`content` / `placeholder` / `input` 直接标注宿主模板
节点。`clear` / `item` / `itemContent` / `itemRemove` 声明 `CrossNestedOwners=true`：`clear` 的物理节点在
共享 `SelectHandle` 自有模板内；多选标签的物理节点在共享标签机制内——`item` 的标记由
`SelectTagAwareTextBox` 在标签容器创建时注入，`itemContent` / `itemRemove` 的标记位于共享 `TagTheme`
模板（`SelectTag : Tag` 复用其模板），二者经 `item` 部件（RuntimeCreated，ContractType=`Tag`）承转主题链
完成校验。弹层三部件位于 owner 自有的
Popup 模板内：`popup.root` 标注在宿主模板节点上，`popup.list` 的根列与过滤列表 marker 位于
`CascaderViewTheme.axaml`，`popup.list` 子级列与 `popup.listItem` 的 marker 在运行时容器创建路径注入。

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `root` |
| Selector | Cascader 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `Cascader` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | Cascader owner |
| 职责 | Cascader root 是数据源、选择、过滤、弹层与状态的组织边界。 |
| 相关 API | 全部 Cascader public API |
| 相关 Token | CascaderToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `prefix`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `prefix` |
| Selector | `.semantic-prefix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-prefix > .semantic-prefix` |
| Style Type | `CascaderPrefixStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `CascaderTheme.axaml` 中投影给 `CascaderAddOnDecoratedBox.ContentLeftAddOn` 的 `AddOnContentPresenter`（经 `$parent[atom:Cascader]` 编译绑定呈现公共 API 值） |
| 职责 | 选择框内容前缀区域，承载 `ContentLeftAddOn` 用户内容，在内容框内联展示。 |
| 相关 API | `ContentLeftAddOn`、`ContentLeftAddOnTemplate` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `content`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `content` |
| Selector | `.semantic-content` |
| SelectorRoute | `/template/ .semantic-content` |
| Style Type | `CascaderContentStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `CascaderTheme.axaml` 中 decorated box 内容面板 |
| 职责 | 选择内容面板，承载占位符、单选结果文本、搜索输入与多选标签盒。 |
| 相关 API | `PlaceholderText`、`SelectedOptionPath`、`EffectiveSelectedOptions` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `placeholder`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `placeholder` |
| Selector | `.semantic-placeholder` |
| SelectorRoute | `/template/ .semantic-content > .semantic-placeholder` |
| Style Type | `CascaderPlaceholderStyle` |
| ContractType | `TextBlock` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `CascaderTheme.axaml` 中 `PlaceholderText` |
| 职责 | 未选择任何项时显示的占位符文本。 |
| 相关 API | `PlaceholderText`、`PlaceholderForeground` |
| 相关 Token | SharedToken（ColorTextPlaceholder） |
| 稳定性 | stable since 6.0 |

#### `input`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `input` |
| Selector | `.semantic-input` |
| SelectorRoute | `/template/ .semantic-content > .semantic-input` |
| Style Type | `CascaderInputStyle` |
| ContractType | `TextBox` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `CascaderTheme.axaml` 中 `PART_SingleFilterInput`（internal `SelectFilterTextBox`，公共契约承诺 Avalonia `TextBox`） |
| 职责 | 过滤模式（`IsFilterEnabled`）下渲染的搜索输入框；非过滤态隐藏但模板节点存在。 |
| 相关 API | `IsFilterEnabled`、`Filter`、`FilterValue` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `suffix`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `suffix` |
| Selector | `.semantic-suffix` |
| SelectorRoute | `/template/ .semantic-scope-input /template/ .semantic-scope-suffix > .semantic-suffix` |
| Style Type | `CascaderSuffixStyle` |
| ContractType | `StackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `CascaderTheme.axaml` 中 `ContentRightAddOn` StackPanel |
| 职责 | 内容后缀区域，承载最大数量指示、用户 `ContentRightAddOn` 与选择 handle。 |
| 相关 API | `ContentRightAddOn`、`ContentRightAddOnTemplate`、`MaxCount` |
| 相关 Token | SharedToken `ColorTextQuaternary`（默认前景） |
| 稳定性 | stable since 6.0 |

suffix 区默认前景色为 `ColorTextQuaternary`，下拉指示箭头（SelectHandle 内图标）与后缀内容跟随该颜色：
在 `CascaderSuffixStyle` 上设置 `TextElement.Foreground` 即可同时定制箭头与后缀内容颜色（例如
`#1890FF` 蓝色箭头，与上游语义样式示例一致）。清除按钮、表单校验反馈与最大数量指示各自维护颜色，
不随该前景色变化。

#### `clear`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `clear` |
| Selector | `.semantic-clear` |
| SelectorRoute | `>> .semantic-scope-handle /template/ .semantic-clear` |
| Style Type | `CascaderClearStyle` |
| ContractType | `Button` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `SelectHandleTheme.axaml` 中 `PART_ClearButton`（internal `InputClearIconButton`，公共契约承诺 Avalonia `Button`） |
| 职责 | 后缀 handle 内的清除按钮，`IsAllowClear` 启用、非空选择且输入区 hover / pressed 时可见。 |
| 相关 API | `IsAllowClear`、`Clear()` |
| 相关 Token | SharedToken |
| 稳定性 | stable since 6.0 |

#### `item`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `item` |
| Selector | `.semantic-item` |
| SelectorRoute | `>> .semantic-scope-tags >> .semantic-item` |
| Style Type | `CascaderItemStyle` |
| ContractType | `Tag` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | internal `SelectTag` 实例（`SelectTagAwareTextBox` 创建标签容器时注入 marker），公共契约承诺 public `Tag` |
| 职责 | 多选模式下选择器中的选中标签。 |
| 相关 API | `IsMultiple`、`SelectedOptions`、`MaxTagCount`、`IsResponsiveTagMode` |
| 相关 Token | SelectToken（MultipleItemBg、MultipleItemHeight*） |
| 稳定性 | stable since 6.0 |

#### `itemContent`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `itemContent` |
| Selector | `.semantic-item-content` |
| SelectorRoute | `>> .semantic-scope-tags >> .semantic-item /template/ .semantic-item-content` |
| Style Type | `CascaderItemContentStyle` |
| ContractType | `TextBlock` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TagTheme.axaml` 中 `TagTextLabel`（`SelectTag : Tag` 复用其模板） |
| 职责 | 选中标签内的文本内容。 |
| 相关 API | 无（随 tag 展示） |
| 相关 Token | TagToken（TagLineHeight） |
| 稳定性 | stable since 6.0 |

#### `itemRemove`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `itemRemove` |
| Selector | `.semantic-item-remove` |
| SelectorRoute | `>> .semantic-scope-tags >> .semantic-item /template/ .semantic-item-remove` |
| Style Type | `CascaderItemRemoveStyle` |
| ContractType | `IconButton` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TagTheme.axaml` 中 `PART_CloseButton`（`IsClosable=false` 时隐藏但模板节点存在） |
| 职责 | 选中标签内的移除按钮。 |
| 相关 API | `IsClosable`（经标签机制） |
| 相关 Token | SharedToken（IconSizeXS） |
| 稳定性 | stable since 6.0 |

#### `popup.root`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `popup.root` |
| Selector | `.semantic-popup-root` |
| SelectorRoute | `/template/ .semantic-popup-root` |
| Style Type | `CascaderPopupRootStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `CascaderTheme.axaml` 中 `PopupFrame` |
| 职责 | 级联菜单弹层根 `Border`，可定制弹层边框、背景与宽度。 |
| 相关 API | `MaxPopupHeight`、`EffectivePopupWidth`、`PopupContentPadding` |
| 相关 Token | PopupTokenResource（PopupCornerRadius）、SharedToken（ColorBgElevated） |
| 稳定性 | stable since 6.0 |

#### `popup.list`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `popup.list` |
| Selector | `.semantic-popup-list` |
| SelectorRoute | `/template/ .semantic-popup-root > .semantic-scope-view /template/ .semantic-scope-frame >> .semantic-popup-list` |
| Style Type | `CascaderPopupListStyle` |
| ContractType | `Control` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | internal `CascaderViewLevelList`（根列为 `CascaderViewTheme.axaml` 静态节点，子级列运行时创建）与 `CascaderViewFilterList`（过滤替代实现），公共契约承诺 `Control` |
| 职责 | 弹层内的一条级联菜单列；过滤模式下由过滤结果列表替代，同一时刻至多一类可见。 |
| 相关 API | `ExpandTrigger`、`Filter` |
| 相关 Token | CascaderToken（ControlItemWidth、MenuPadding） |
| 稳定性 | stable since 6.0 |

#### `popup.listItem`

| 字段 | 值 |
| --- | --- |
| Owner | `Cascader` |
| Part | `popup.listItem` |
| Selector | `.semantic-popup-list-item` |
| SelectorRoute | `/template/ .semantic-popup-root > .semantic-scope-view /template/ .semantic-scope-frame >> .semantic-popup-list-item` |
| Style Type | `CascaderPopupListItemStyle` |
| ContractType | `TemplatedControl` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `true` |
| RuntimeCreated | `true` |
| AtomUI 节点 | public `CascaderViewItem`（层级列条目）与 internal `CascaderViewFilterListItem`（过滤结果条目），公共祖先承诺 `TemplatedControl` |
| 职责 | 菜单列中的单个选项条目，均为运行时容器创建，虚拟化回收复用时 marker 保持。 |
| 相关 API | `OptionTemplate`、`IsAllowSelectParent` |
| 相关 Token | CascaderToken（OptionPadding、HeaderHeight、OptionSelectedBg） |
| 稳定性 | stable since 6.0 |

`ContractType` 不参与 selector 匹配，只约束 `x:SetterTargetType` 与兼容性下界；实现节点为 internal 类型时，
公共契约承诺到最低 public 基类（`input`→`TextBox`、`item`→`Tag`、`popup.list`→`Control`、
`popup.listItem`→`TemplatedControl`）。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Cascader/Themes/CascaderTheme.axaml`

```xml
<Panel>
    <CascaderAddOnDecoratedBox Name="{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}">
        <Panel>
            <TextBlock Name="PlaceholderText" />
            <TextBlock Name="SingleSelectResultPresenter" />
            <SelectFilterTextBox Name="PART_SingleFilterInput" />
            <SelectTagAwareTextBox Name="SelectedOptionsBox" />
        </Panel>
    </CascaderAddOnDecoratedBox>
    <Popup Name="PART_Popup">
        <Border Name="PopupFrame">
            <CascaderView Name="PART_CascaderView" />
        </Border>
    </Popup>
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Cascader
  -> CascaderAddOnDecoratedBox (control theme, CascaderAddOnDecoratedBoxTheme.axaml)
  -> Cascader (control theme, CascaderTheme.axaml)
     -> Panel (template-stable)
        -> CascaderAddOnDecoratedBox#{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart} (internal-observable)
           -> Panel (template-stable)
              -> TextBlock#PlaceholderText (template-stable)
              -> TextBlock#SingleSelectResultPresenter (template-stable)
              -> SelectFilterTextBox#PART_SingleFilterInput (template-stable)
              -> SelectTagAwareTextBox#SelectedOptionsBox (template-stable)
        -> Popup#PART_Popup (template-stable)
           -> Border#PopupFrame (template-stable)
              -> CascaderView#PART_CascaderView (template-stable)
  -> CascaderViewFilterList (control theme, CascaderViewFilterListTheme.axaml)
  -> CascaderViewFilterListItem (item container control theme, CascaderViewFilterListTheme.axaml)
  -> CascaderViewItem (item container control theme, CascaderViewItemTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> Grid#ItemsLayout (template-stable)
           -> Panel#Indicator (template-stable)
              -> CheckBox#ToggleCheckbox (template-stable)
           -> IconPresenter#IconPresenter (internal-observable)
           -> ContentPresenter#HeaderPresenter (internal-observable)
           -> IconTemplatePresenter#ExpandIconPresenter (internal-observable)
           -> IconTemplatePresenter#LoadingIconPresenter (internal-observable)
  -> CascaderViewLevelList (control theme, CascaderViewLevelListTheme.axaml)
     -> ScrollViewer (template-stable)
        -> ItemsPresenter (internal-observable)
  -> CascaderView (control theme, CascaderViewTheme.axaml)
     -> PixelAlignedBorder#Frame (template-stable)
        -> Panel (template-stable)
           -> Panel (template-stable)
              -> ScrollViewer (template-stable)
                 -> CascaderViewFrame (internal-observable)
                    -> StackPanel#PART_ItemsPanel (template-stable)
                       -> CascaderViewLevelList#PART_RootLevelList (template-stable)
              -> ContentPresenter#EmptyIndicator (internal-observable)
              -> Empty#DefaultEmptyIndicator (template-stable)
           -> CascaderViewFilterList#PART_FilterList (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Cascader` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CascaderAddOnDecoratedBox` | control theme | `CascaderAddOnDecoratedBoxTheme.axaml` | Cascader | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Cascader` | control theme | `CascaderTheme.axaml` | 用户代码 / 控件宿主 | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `DataLoader`, `DataValidationErrors`, `DefaultSelectOptionPath`, `EffectivePopupWidth` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `CascaderTheme.axaml` | Cascader | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `DataLoader`, `DataValidationErrors`, `DefaultSelectOptionPath`, `EffectivePopupWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}` | template node (CascaderAddOnDecoratedBox) | `CascaderTheme.axaml` | Cascader | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `DataValidationErrors`, `EffectiveSelectedOptions`, `FontFamily`, `FontSize` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PlaceholderText` | template node (TextBlock) | `CascaderTheme.axaml` | Cascader | `IsPlaceholderTextVisible`, `PlaceholderForeground`, `PlaceholderText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SingleSelectResultPresenter` | template node (TextBlock) | `CascaderTheme.axaml` | Cascader | `IsShowOverflowTip`, `OverflowTipDelay`, `OverflowTipPlacement`, `SelectedOptionPath` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SingleFilterInput` | template node (SelectFilterTextBox) | `CascaderTheme.axaml` | Cascader | `FontFamily`, `FontSize`, `FontStyle`, `FontWeight`, `IsShowOverflowTip`, `OverflowTipDelay` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectedOptionsBox` | template node (SelectTagAwareTextBox) | `CascaderTheme.axaml` | Cascader | `EffectiveSelectedOptions`, `Height`, `IsDropDownOpen`, `IsFilterEnabled`, `IsResponsiveTagMode`, `IsShowOverflowTip` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Popup` | template node (Popup) | `CascaderTheme.axaml` | Cascader | `DataLoader`, `DefaultSelectOptionPath`, `EffectivePopupWidth`, `ExpandIcon`, `ExpandTrigger`, `Filter` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PopupFrame` | template node (Border) | `CascaderTheme.axaml` | Cascader | `DataLoader`, `DefaultSelectOptionPath`, `EffectivePopupWidth`, `ExpandIcon`, `ExpandTrigger`, `Filter` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CascaderView` | template node (CascaderView) | `CascaderTheme.axaml` | Cascader | `DataLoader`, `DefaultSelectOptionPath`, `ExpandIcon`, `ExpandTrigger`, `Filter`, `FilterHighlightForeground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CascaderViewFilterList` | control theme | `CascaderViewFilterListTheme.axaml` | Cascader | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CascaderViewFilterListItem` | item container control theme | `CascaderViewFilterListTheme.axaml` | Cascader | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CascaderViewItem` | item container control theme | `CascaderViewItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderThickness`, `CornerRadius`, `ExpandIcon`, `Header`, `HeaderTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `CascaderViewItemTheme.axaml` | CascaderViewItem | `Background`, `BorderThickness`, `CornerRadius`, `ExpandIcon`, `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemsLayout` | template node (Grid) | `CascaderViewItemTheme.axaml` | CascaderViewItem | `ExpandIcon`, `Header`, `HeaderTemplate`, `Icon`, `IsChecked`, `IsEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Indicator` | template node (Panel) | `CascaderViewItemTheme.axaml` | CascaderViewItem | `IsChecked`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ToggleCheckbox` | template node (CheckBox) | `CascaderViewItemTheme.axaml` | CascaderViewItem | `IsChecked`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IconPresenter` | template node (IconPresenter) | `CascaderViewItemTheme.axaml` | CascaderViewItem | `Icon`, `IsEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `HeaderPresenter` | template node (ContentPresenter) | `CascaderViewItemTheme.axaml` | CascaderViewItem | `Header`, `HeaderTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ExpandIconPresenter` | template node (IconTemplatePresenter) | `CascaderViewItemTheme.axaml` | CascaderViewItem | `ExpandIcon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `LoadingIconPresenter` | template node (IconTemplatePresenter) | `CascaderViewItemTheme.axaml` | CascaderViewItem | `IsLoading`, `LoadingIcon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CascaderViewLevelList` | control theme | `CascaderViewLevelListTheme.axaml` | Cascader | `ItemsPanel`, `ScrollViewer`, `atom` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ItemsPresenter` | template node (ItemsPresenter) | `CascaderViewLevelListTheme.axaml` | CascaderViewLevelList | `ItemsPanel` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CascaderView` | control theme | `CascaderViewTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `ClipToBounds`, `CornerRadius`, `EmptyIndicator` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (PixelAlignedBorder) | `CascaderViewTheme.axaml` | CascaderView | `Background`, `BorderBrush`, `BorderThickness`, `ClipToBounds`, `CornerRadius`, `EmptyIndicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `CascaderViewTheme.axaml` | CascaderView | `BorderBrush`, `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `ExpandTrigger`, `FilteredPathInfos` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPanel` | template node (StackPanel) | `CascaderViewTheme.axaml` | CascaderView | `ExpandTrigger`, `IsAllowSelectParent`, `OptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLevelList` | template node (CascaderViewLevelList) | `CascaderViewTheme.axaml` | CascaderView | `ExpandTrigger`, `IsAllowSelectParent`, `OptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `EmptyIndicator` | template node (ContentPresenter) | `CascaderViewTheme.axaml` | CascaderView | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsEffectiveEmptyVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `DefaultEmptyIndicator` | template node (Empty) | `CascaderViewTheme.axaml` | CascaderView | `EmptyIndicatorPadding`, `IsDefaultEmptyIndicatorVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FilterList` | template node (CascaderViewFilterList) | `CascaderViewTheme.axaml` | CascaderView | `FilteredPathInfos`, `IsFiltering` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_InputControlFrame` | `CascaderAddOnDecoratedBox` | `InputControlFrame` / AddOnDecoratedBox 组合、Addon、EffectiveStatus、CompactSpace、多选状态和 content padding 承载。 |
| `PART_SingleFilterInput` | `SelectFilterTextBox` | 单选过滤输入，过滤启用时显示。 |
| `SelectedOptionsBox` | `SelectTagAwareTextBox` | 多选结果 tag 展示和多选过滤输入承载。 |
| `PART_SelectMaxCountIndicator` | `SelectMaxCountIndicator` | 最大选择数量提示。 |
| `PART_ContentRightAddOnPresenter` | `ContentPresenter` | 用户内部右侧内容承载。 |
| `PART_SelectHandle` | `SelectHandle` | 展开、loading、清除和 Form feedback 图标入口。 |
| `PART_Popup` | `Popup` | 级联弹层宿主。 |
| `PopupFrame` | `Border` | 弹层外壳、最大高度、最小宽度和 popup padding。 |
| `PART_CascaderView` | `CascaderView` | 级联弹层内容、列、过滤和选项交互。 |
| `PART_ItemsPanel` | `StackPanel` | CascaderView 内部横向列容器。 |
| `PART_RootLevelList` | `CascaderViewLevelList` | CascaderView root 列。 |
| `PART_FilterList` | `CascaderViewFilterList` | CascaderView 过滤结果列表。 |

## Pseudo Classes

稳定伪类来自 `AbstractSelect` 和 `CascaderViewItem`：

- `:dropdownopen`、native validation `:error`、AtomUI warning `:warning`、`:pressed`。
- `InputControlFrame` variant 伪类：`:outlined`、`:filled`、`:borderless`、`:underlined`；Cascader 专用伪类只表达 dropdown、候选和结果状态。
- `CascaderViewItem` 使用 `:expanded`、`:checked`、`:selected` 和 checkbox toggle type 伪类。

## State Flow

核心状态流：

```text
OptionsSource / Options
      ↓
CascaderView.Options
      ↓
CascaderViewLevelList
      ↓
CascaderViewItem
      ↓
SelectedOption / SelectedOptions
      ↓
SelectedOptionPath / EffectiveSelectedOptions
      ↓
input display + Form value
```

单选模式：

- `IsMultiple=false` 时使用 `SelectedOption` 作为真实值。
- 叶子节点点击后提交选择并关闭 popup。
- `IsAllowSelectParent=true` 时，非叶子节点也可提交为选择结果。
- `SelectedOptionPath` 根据当前选项的 parent 链生成 header 路径，用于输入框展示。
- `DefaultSelectOptionPath` 只在当前选择为空或强制刷新路径时应用。

多选模式：

- `IsMultiple=true` 时使用 `SelectedOptions` 作为真实值，并让内部 `CascaderView` 进入 checkable 模式。
- `SelectedOptions` 保留真实勾选集合，`ShowCheckedStrategy` 只计算 `EffectiveSelectedOptions`，用于 tag 展示。
- `SelectedOptions` 支持外部集合替换，也支持 `INotifyCollectionChanged` 集合的原地 `Add`、`Remove`、`Replace`、`Move` 和 `Reset`；这些变化会同步刷新 tag、计数、空状态、Form value 和内部 `CascaderView` 勾选状态。
- `MaxCount` 达到上限时，未选项通过 `IsMaxSelectReached` 进入受限状态；已选项仍可取消。
- 关闭单个 tag 时，目标节点及其子孙会从 `SelectedOptions` 中移除。

展开和异步加载：

- 展开按节点 parent 链从 root 到目标逐级执行。
- 同一级只保持一个已展开分支，展开新分支会折叠同级旧分支。
- 未加载且非 leaf 的节点在有 `DataLoader` 时进入 loading，加载完成后把返回子项加入目标 `Children`。
- 控件 detached 时取消待处理异步加载，避免离开视觉树后继续处理结果。

过滤：

- `FilterValue` 非空且控件 loaded 时，CascaderView 收集所有叶子路径并按 `Filter` 过滤。
- 过滤结果显示完整路径文本，选中过滤结果后回写目标 option。
- 过滤模式下，`Up` / `Down` 在可用结果间循环移动内部候选高亮，不修改 `SelectedOption`；`Enter` 提交当前候选，尚无候选时提交第一个可用结果，没有可用结果时保持选择和 popup 状态不变。路径中任一祖先 disabled 时，该过滤结果也不可作为候选或提交。
- 过滤列表拥有独立于树列的 active candidate owner；过滤结果重建、过滤清空、popup 关闭和容器回收时清除旧候选。树列与过滤列不会同时保留两个候选视觉。
- 清空过滤值或关闭 popup 后，过滤列表、过滤计数和缓存路径会被清理。

树形键盘导航：

- popup 打开且未过滤时，`Up` / `Down` 在当前已展开列的可见 enabled item 间循环移动内部候选；候选高亮与真实选择相互独立。
- 普通树列的 active candidate 由 `CascaderView` 单一持有；鼠标移动到 enabled item 时迁移该候选并继续执行 `ExpandTrigger=Hover` 的展开逻辑，但不提前提交选择、不滚动列表。`Enter` 使用同一 active candidate 作为选择或展开目标。
- `Right` 从当前候选或第一个可见 enabled item 开始，展开可展开节点并把候选移到下一列的第一个 enabled child。
- `Left` 优先把子级候选移回父级；候选已位于展开的根级非叶节点时折叠该节点。
- `Enter` 提交 enabled、非 loading 的叶子候选；`IsAllowSelectParent=true` 时也可提交父节点，否则沿用 `Right` 的展开并进入子级行为。

Form：

- 单选 Form value 为 `SelectedOption`。
- 多选 Form value 为 `SelectedOptions`。
- Form 校验错误写入同一份 Avalonia `DataValidationErrors`；`SelectedOption` 和 `SelectedOptions` 不维护独立错误状态。
- Form clear 会按当前 `IsMultiple` 清空对应选择状态。

## Theme and Token Boundaries

Cascader 的默认视觉由 Cascader 根主题、`InputControlFrame` / CascaderAddOnDecoratedBox、PopupHost、CascaderView、CascaderViewItem、SelectTagAwareTextBox、SelectHandle 和 CascaderToken 协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `CascaderTheme.axaml` | 根模板、输入壳体、单选结果、多选 tag、handle、popup 和 CascaderView 绑定。 |
| `CascaderViewTheme.axaml` | 弹层内部级联列、空状态、过滤列表和默认 option template。 |
| `CascaderViewItemTheme.axaml` | option 行、checkbox、icon、header、展开 / loading icon、hover / expanded / disabled 状态。 |
| `CascaderViewLevelListTheme.axaml` | 单列列表宽度、高度、padding 和滚动行为。 |
| `CascaderToken` | Cascader 输入宽度、列宽、弹层高度、选项高度、padding、状态色和过滤高亮。 |
| `PopupHostToken` | popup margin、阴影和圆角。 |
| SharedToken | 字体、输入高度、图标尺寸、placeholder、disabled、motion 和全局 spacing。 |

单选路径和多选 tag 的完整内容提示复用共享 `OverflowTip` attached behavior。主题只把 `IsShowOverflowTip`、`OverflowTipDelay`、`OverflowTipPlacement` 和当前展示文本传给显示节点；tooltip 仅在视觉溢出时写入，且不覆盖用户手动声明的 `ToolTip.Tip`。

主题不可破坏的视觉边界：

- 输入表面必须继续由 `InputControlFrame` 承载；`CascaderAddOnDecoratedBox` 只扩展多选、dropdown 和 content padding 布局。
- `PART_SelectHandle` 的 hover / pressed / dropdown open / clear / loading 状态必须与输入壳体保持同步。
- `OptionTemplate` 的 DataContext 必须保持为 `ICascaderOption`，不能改为 header 文本。
- Popup 宽度和空状态宽度匹配语义必须保持：普通级联列使用列宽，空状态需要匹配输入宽度。
- 选项行的 checkbox、icon、header、expand / loading icon 间距由 Token 管理，不应在单个模板节点中写死。
- disabled、expanded、pointerover、checked、loading 和 active candidate 状态 selector 不能被绕过；`pointerover` 只触发 active candidate 迁移，不独立绘制第二个候选背景。

Token 边界：

`CascaderToken` 描述 Cascader 输入弹层和级联选项的尺寸、列宽、选项状态色、padding、过滤高亮和内部元素间距。它只表达控件主题常量，不表达实例选择状态、当前展开路径、当前过滤值、当前 loading 状态或最大选择数量状态。

## Customization Boundaries

维护 Cascader 时必须保持以下不变量：

- `CascaderOption` 保持轻量数据模型定位，不直接改造成 `AvaloniaObject`。
- 需要 binding target 或动态资源能力时使用 `BindableCascaderOption`。
- `Header` 的容器内容必须继续是 option 对象本身，避免破坏 `OptionTemplate` 的数据上下文。
- `SelectedOption` 和 `SelectedOptions` 的单选 / 多选边界不能混用。
- 键盘候选只能表达当前导航位置，不能通过 `SelectedIndex` 或 `SelectedOption` 提前提交真实选择；disabled 或 loading item 不得成为可提交候选。
- `ShowCheckedStrategy` 只能影响 `EffectiveSelectedOptions`，不能改写真实 `SelectedOptions`。
- `IsAllowSelectParent=false` 时，非 leaf 节点不能作为普通单选结果提交。
- `DefaultSelectOptionPath` 的路径段必须继续按 `ItemKey` 优先、`Value` 兜底匹配。
- `OptionsSource` 变化后必须尽量按路径 identity 保留当前选择。
- 异步加载时 detached 必须取消待处理加载。
- 容器回收、ItemsSource 变化和 detach 时必须释放绑定型选项的 resource host attach、children 集合订阅和属性订阅。
- 右侧稳定 template part 绑定优先使用 AXAML compiled binding，不把可静态表达的绑定搬回 C#。
- Template part、Token 名称、伪类和主题 selector 不得在未授权情况下重命名或删除。

维护不变量：

- 不把 `CascaderOption` 改造成 `AvaloniaObject`。
- 新增 binding target 需求时使用独立 bindable 类型。
- 容器清理时先释放订阅，再清空容器属性。
- `Header` 容器值保持 option 对象，不改成 header 文本。
- checked / expanded 的反写只针对 `BindableCascaderOption`，不改变普通 `CascaderOption` 的 CLR 数据模型语义。
- `ShowCheckedStrategy` 不能改写真实 `SelectedOptions`。
- `DefaultSelectOptionPath` 和 `OptionsSource` 重映射必须继续使用 `ItemKey` 优先、`Value` 兜底的路径 identity。
- 展开、过滤、异步加载和勾选不能通过延迟刷新或抑制标记掩盖状态所有权问题。
- 重新套用模板、container recycle、ItemsSource replacement 和 detach 都必须有对应 release 路径。
