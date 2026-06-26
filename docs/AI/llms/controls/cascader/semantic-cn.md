# Cascader 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Cascader` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

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
  -> CascaderViewItem (item container control theme, CascaderViewItemTheme.axaml)
     -> Border#Frame (template-stable)
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
     -> Border#Frame (template-stable)
        -> Panel (template-stable)
           -> Panel (template-stable)
              -> ScrollViewer (template-stable)
                 -> CascaderViewFrame (internal-observable)
                    -> StackPanel#PART_ItemsPanel (template-stable)
                       -> CascaderViewLevelList#PART_RootLevelList (template-stable)
              -> ContentPresenter#EmptyIndicator (internal-observable)
           -> CascaderViewFilterList#PART_FilterList (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Cascader` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `CascaderAddOnDecoratedBox` | control theme | `CascaderAddOnDecoratedBoxTheme.axaml` | Cascader | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Cascader` | control theme | `CascaderTheme.axaml` | 用户代码 / 控件宿主 | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `DataLoader`, `DefaultSelectOptionPath` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `CascaderTheme.axaml` | Cascader | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `DataLoader`, `DefaultSelectOptionPath` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}` | template node (CascaderAddOnDecoratedBox) | `CascaderTheme.axaml` | Cascader | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `EffectiveSelectedOptions`, `Height` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PlaceholderText` | template node (TextBlock) | `CascaderTheme.axaml` | Cascader | `IsPlaceholderTextVisible`, `PlaceholderForeground`, `PlaceholderText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SingleSelectResultPresenter` | template node (TextBlock) | `CascaderTheme.axaml` | Cascader | `SelectedOptionPath` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SingleFilterInput` | template node (SelectFilterTextBox) | `CascaderTheme.axaml` | Cascader | `SelectedOptionPath`, `SizeType` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectedOptionsBox` | template node (SelectTagAwareTextBox) | `CascaderTheme.axaml` | Cascader | `EffectiveSelectedOptions`, `Height`, `IsDropDownOpen`, `IsFilterEnabled`, `IsResponsiveTagMode`, `MaxTagCount` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Popup` | template node (Popup) | `CascaderTheme.axaml` | Cascader | `DataLoader`, `DefaultSelectOptionPath`, `EffectivePopupWidth`, `ExpandIcon`, `ExpandTrigger`, `Filter` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PopupFrame` | template node (Border) | `CascaderTheme.axaml` | Cascader | `DataLoader`, `DefaultSelectOptionPath`, `EffectivePopupWidth`, `ExpandIcon`, `ExpandTrigger`, `Filter` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_CascaderView` | template node (CascaderView) | `CascaderTheme.axaml` | Cascader | `DataLoader`, `DefaultSelectOptionPath`, `ExpandIcon`, `ExpandTrigger`, `Filter`, `FilterHighlightForeground` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `CascaderViewFilterList` | control theme | `CascaderViewFilterListTheme.axaml` | Cascader | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CascaderViewItem` | item container control theme | `CascaderViewItemTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderThickness`, `CornerRadius`, `ExpandIcon`, `Header`, `HeaderTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `CascaderViewItemTheme.axaml` | CascaderViewItem | `Background`, `BorderThickness`, `CornerRadius`, `ExpandIcon`, `Header`, `HeaderTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
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
| `Frame` | template node (Border) | `CascaderViewTheme.axaml` | CascaderView | `Background`, `BorderBrush`, `BorderThickness`, `ClipToBounds`, `CornerRadius`, `EmptyIndicator` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Panel` | template node (Panel) | `CascaderViewTheme.axaml` | CascaderView | `BorderBrush`, `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `ExpandTrigger`, `FilteredPathInfos` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPanel` | template node (StackPanel) | `CascaderViewTheme.axaml` | CascaderView | `ExpandTrigger`, `IsAllowSelectParent`, `OptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RootLevelList` | template node (CascaderViewLevelList) | `CascaderViewTheme.axaml` | CascaderView | `ExpandTrigger`, `IsAllowSelectParent`, `OptionTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `EmptyIndicator` | template node (ContentPresenter) | `CascaderViewTheme.axaml` | CascaderView | `EmptyIndicator`, `EmptyIndicatorPadding`, `EmptyIndicatorTemplate`, `IsEffectiveEmptyVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_FilterList` | template node (CascaderViewFilterList) | `CascaderViewTheme.axaml` | CascaderView | `FilteredPathInfos`, `IsFiltering` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_AddOnDecoratedBox` | `CascaderAddOnDecoratedBox` | 输入壳体、Addon、variant、status、CompactSpace、多选状态和 content padding 承载。 |
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

- `:dropdownopen`、`:error`、`:warning`、`:pressed`。
- AddOnDecoratedBox variant 伪类：`:outlined`、`:filled`、`:borderless`。
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
- 清空过滤值或关闭 popup 后，过滤列表、过滤计数和缓存路径会被清理。

Form：

- 单选 Form value 为 `SelectedOption`。
- 多选 Form value 为 `SelectedOptions`。
- Form clear 会按当前 `IsMultiple` 清空对应选择状态。

## Theme and Token Boundaries

Cascader 的默认视觉由 Cascader 根主题、输入壳体、PopupHost、CascaderView、CascaderViewItem、SelectTagAwareTextBox、SelectHandle 和 CascaderToken 协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `CascaderTheme.axaml` | 根模板、输入壳体、单选结果、多选 tag、handle、popup 和 CascaderView 绑定。 |
| `CascaderViewTheme.axaml` | 弹层内部级联列、空状态、过滤列表和默认 option template。 |
| `CascaderViewItemTheme.axaml` | option 行、checkbox、icon、header、展开 / loading icon、hover / expanded / disabled 状态。 |
| `CascaderViewLevelListTheme.axaml` | 单列列表宽度、高度、padding 和滚动行为。 |
| `CascaderToken` | Cascader 输入宽度、列宽、弹层高度、选项高度、padding、状态色和过滤高亮。 |
| `PopupHostToken` | popup margin、阴影和圆角。 |
| SharedToken | 字体、输入高度、图标尺寸、placeholder、disabled、motion 和全局 spacing。 |

主题不可破坏的视觉边界：

- 输入壳体必须继续由 `CascaderAddOnDecoratedBox` 承载，保持 variant、status、CompactSpace 和 content padding 语义。
- `PART_SelectHandle` 的 hover / pressed / dropdown open / clear / loading 状态必须与输入壳体保持同步。
- `OptionTemplate` 的 DataContext 必须保持为 `ICascaderOption`，不能改为 header 文本。
- Popup 宽度和空状态宽度匹配语义必须保持：普通级联列使用列宽，空状态需要匹配输入宽度。
- 选项行的 checkbox、icon、header、expand / loading icon 间距由 Token 管理，不应在单个模板节点中写死。
- disabled、expanded、pointerover、checked 和 loading 状态 selector 不能被绕过。

Token 边界：

`CascaderToken` 描述 Cascader 输入弹层和级联选项的尺寸、列宽、选项状态色、padding、过滤高亮和内部元素间距。它只表达控件主题常量，不表达实例选择状态、当前展开路径、当前过滤值、当前 loading 状态或最大选择数量状态。

## Customization Boundaries

维护 Cascader 时必须保持以下不变量：

- `CascaderOption` 保持轻量数据模型定位，不直接改造成 `AvaloniaObject`。
- 需要 binding target 或动态资源能力时使用 `BindableCascaderOption`。
- `Header` 的容器内容必须继续是 option 对象本身，避免破坏 `OptionTemplate` 的数据上下文。
- `SelectedOption` 和 `SelectedOptions` 的单选 / 多选边界不能混用。
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
