# Select 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Select` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Select/Themes/SelectTheme.axaml`

```xml
<Panel>
    <SelectAddOnDecoratedBox Name="{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}">
        <Panel>
            <TextBlock Name="PlaceholderText" />
            <SelectFilterTextBox Name="PART_SingleFilterInput" />
            <SelectResultOptionsBox Name="SelectedOptionsBox" />
        </Panel>
    </SelectAddOnDecoratedBox>
    <Popup Name="PART_Popup" />
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Select
  -> SelectAddOnDecoratedBox (control theme, SelectAddOnDecoratedBoxTheme.axaml)
  -> SelectCandidateListItem (item container control theme, SelectCandidateListItemTheme.axaml)
  -> SelectCandidateList (control theme, SelectCandidateListTheme.axaml)
  -> SelectFilterTextBox (control theme, SelectFilterTextBoxTheme.axaml)
  -> SelectHandle (control theme, SelectHandleTheme.axaml)
     -> Panel (template-stable)
        -> StackPanel#IconLayout (template-stable)
           -> Panel (template-stable)
              -> IconPresenter#OpenIndicator (internal-observable)
              -> InputClearIconButton#PART_ClearButton (template-stable)
           -> ContentPresenter#FormFeedBack (internal-observable)
  -> SelectMaxCountIndicator (control theme, SelectMaxCountIndicatorTheme.axaml)
     -> TextBlock (template-stable)
  -> SelectResultOptionsBox (control theme, SelectResultOptionsBoxTheme.axaml)
     -> Panel#RootLayout (template-stable)
        -> SelectWrapPanel#PART_DefaultPanel (template-stable)
        -> SelectMaxTagAwarePanel#PART_MaxCountAwarePanel (template-stable)
  -> SelectTagAwareTextBox (control theme, SelectTagAwareTextBoxTheme.axaml)
     -> Panel#RootLayout (template-stable)
        -> SelectWrapPanel#PART_DefaultPanel (template-stable)
        -> SelectMaxTagAwarePanel#PART_MaxCountAwarePanel (template-stable)
  -> SelectTag (control theme, SelectTagTheme.axaml)
  -> Select (control theme, SelectTheme.axaml)
     -> Panel (template-stable)
        -> SelectAddOnDecoratedBox#{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart} (internal-observable)
           -> Panel (template-stable)
              -> TextBlock#PlaceholderText (template-stable)
              -> SelectFilterTextBox#PART_SingleFilterInput (template-stable)
              -> SelectResultOptionsBox#SelectedOptionsBox (internal-observable)
        -> Popup#PART_Popup (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Select` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `SelectAddOnDecoratedBox` | control theme | `SelectAddOnDecoratedBoxTheme.axaml` | Select | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SelectCandidateListItem` | item container control theme | `SelectCandidateListItemTheme.axaml` | Select | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SelectCandidateList` | control theme | `SelectCandidateListTheme.axaml` | Select | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SelectFilterTextBox` | control theme | `SelectFilterTextBoxTheme.axaml` | Select | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SelectHandle` | control theme | `SelectHandleTheme.axaml` | Select | `CurrentIndicatorIcon`, `FormFeedback`, `IsCurrentIndicatorVisible`, `IsFormFeedbackVisible`, `IsMotionEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `SelectHandleTheme.axaml` | SelectHandle | `CurrentIndicatorIcon`, `FormFeedback`, `IsCurrentIndicatorVisible`, `IsFormFeedbackVisible`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IconLayout` | template node (StackPanel) | `SelectHandleTheme.axaml` | SelectHandle | `CurrentIndicatorIcon`, `FormFeedback`, `IsCurrentIndicatorVisible`, `IsFormFeedbackVisible`, `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `OpenIndicator` | template node (IconPresenter) | `SelectHandleTheme.axaml` | SelectHandle | `CurrentIndicatorIcon`, `IsCurrentIndicatorVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ClearButton` | template node (InputClearIconButton) | `SelectHandleTheme.axaml` | SelectHandle | `IsMotionEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FormFeedBack` | template node (ContentPresenter) | `SelectHandleTheme.axaml` | SelectHandle | `FormFeedback`, `IsFormFeedbackVisible` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SelectMaxCountIndicator` | control theme | `SelectMaxCountIndicatorTheme.axaml` | Select | `Text` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `SelectResultOptionsBox` | control theme | `SelectResultOptionsBoxTheme.axaml` | Select | `IsResponsiveTagMode`, `MaxTagCount` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (Panel) | `SelectResultOptionsBoxTheme.axaml` | SelectResultOptionsBox | `IsResponsiveTagMode`, `MaxTagCount` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DefaultPanel` | template node (SelectWrapPanel) | `SelectResultOptionsBoxTheme.axaml` | SelectResultOptionsBox | `IsResponsiveTagMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MaxCountAwarePanel` | template node (SelectMaxTagAwarePanel) | `SelectResultOptionsBoxTheme.axaml` | SelectResultOptionsBox | `IsResponsiveTagMode`, `MaxTagCount` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectTagAwareTextBox` | control theme | `SelectTagAwareTextBoxTheme.axaml` | Select | `IsResponsiveTagMode`, `MaxTagCount` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (Panel) | `SelectTagAwareTextBoxTheme.axaml` | SelectTagAwareTextBox | `IsResponsiveTagMode`, `MaxTagCount` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DefaultPanel` | template node (SelectWrapPanel) | `SelectTagAwareTextBoxTheme.axaml` | SelectTagAwareTextBox | `IsResponsiveTagMode` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MaxCountAwarePanel` | template node (SelectMaxTagAwarePanel) | `SelectTagAwareTextBoxTheme.axaml` | SelectTagAwareTextBox | `IsResponsiveTagMode`, `MaxTagCount` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectTag` | control theme | `SelectTagTheme.axaml` | Select | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Select` | control theme | `SelectTheme.axaml` | 用户代码 / 控件宿主 | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `DataValidationErrors`, `FontFamily` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `SelectTheme.axaml` | Select | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `DataValidationErrors`, `FontFamily` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:AddOnDecoratedBox.AddOnDecoratedBoxPart}` | template node (SelectAddOnDecoratedBox) | `SelectTheme.axaml` | Select | `CompactSpaceItemPosition`, `CompactSpaceOrientation`, `ContentLeftAddOn`, `ContentLeftAddOnTemplate`, `DataValidationErrors`, `FontFamily` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PlaceholderText` | template node (TextBlock) | `SelectTheme.axaml` | Select | `IsPlaceholderTextVisible`, `PlaceholderForeground`, `PlaceholderText` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SingleFilterInput` | template node (SelectFilterTextBox) | `SelectTheme.axaml` | Select | `FontFamily`, `FontSize`, `FontStyle`, `FontWeight`, `IsShowOverflowTip`, `OverflowTipDelay` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SelectedOptionsBox` | template node (SelectResultOptionsBox) | `SelectTheme.axaml` | Select | `Height`, `IsDropDownOpen`, `IsEffectiveFilterEnabled`, `IsResponsiveTagMode`, `IsShowOverflowTip`, `MaxTagCount` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_Popup` | template node (Popup) | `SelectTheme.axaml` | Select | `IsDropDownOpen`, `ShouldUseOverlayPopup` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_AddOnDecoratedBox` | `SelectAddOnDecoratedBox` | 输入壳体、Addon、variant、status、CompactSpace 和 hover/pressed 状态承载。 |
| `PART_SingleFilterInput` | `SelectFilterTextBox` | 单选模式结果显示和搜索输入。 |
| `SelectedOptionsBox` | `SelectResultOptionsBox` | 多选和 Tags 模式已选标签与搜索输入。 |
| `PART_SelectMaxCountIndicator` | `SelectMaxCountIndicator` | 最大选择数量提示。 |
| `PART_ContentRightAddOnPresenter` | `ContentPresenter` | 用户内部右侧内容承载。 |
| `PART_SelectHandle` | `SelectHandle` | 展开、loading、清除和 Form feedback 图标入口。 |
| `PART_Popup` | `Popup` | 候选弹层宿主。 |
| `PopupFrame` | `Border` | 懒创建的候选弹层外壳。 |
| `PART_CandidateList` | `SelectCandidateList` | 懒创建的候选项列表、键盘导航和提交/取消。 |

## Pseudo Classes

Select 的稳定伪类包括 `:dropdownopen`，同时通过标准 `:pressed`、`:disabled` 和 AddOnDecoratedBox 相关状态表达输入表面视觉。

## State Flow

Select 的核心状态流：

```text
OptionsSource / Options / OptionsLoader
      ↓
用户选项源
      ↓
Effective options = 用户选项源 + Tags 运行时动态选项
      ↓
SelectCandidateList
      ↓
SelectedOption / SelectedOptions
      ↓
SingleFilterInput / SelectedOptionsBox
      ↓
Form value + SelectionChanged
```

选择模式语义：

- `Single` 使用 `SelectedOption` 作为唯一表单值，内部单行过滤输入负责展示当前 `Header`。
- `Multiple` 使用 `SelectedOptions` 作为表单值，已选项以 `SelectTag` 展示。
- `Tags` 以 `Multiple` 为基础，始终启用有效过滤，并在过滤结果为空且输入非空时创建 `IsDynamicAdded=true` 的运行时动态选项。

弹层交互优先级：

```text
Disabled / invisible / window deactivated
> DropDownClosing cancellation
> DropDownOpening cancellation
> IsDropDownOpen
> pointer / keyboard open request
```

键盘行为：

- 弹层打开时按键优先交给 `SelectCandidateList.HandleKeyDown()`。
- `Enter` 在单选模式提交候选项，在多选模式切换候选项选中状态。
- `Escape` 取消候选并关闭弹层。
- `F4` 或 `Alt+Up/Down` 切换弹层。
- 弹层关闭时，`Up/Down`、`Enter`、`Space` 可打开弹层。
- 多选和 Tags 模式下，过滤输入为空时 `Backspace/Delete` 删除最后一个已选项。

候选交互：

- `SelectCandidateList` 维护唯一 active candidate，鼠标移动和键盘导航必须更新同一个候选状态。
- 鼠标在可用候选项上实际移动时，该项成为 active candidate；鼠标路径不滚动候选列表，也不提交公共选择。
- `Up/Down` 在当前有效候选视图中移动 active candidate，并可以把键盘候选滚动到可见区域。
- `Enter` 只提交或切换当前 active candidate，候选视觉目标和提交目标必须一致。
- active candidate 与已确认选择相互独立；已选项继续保持 selected 视觉和公共选择语义。
- `:pointerover` 只表达指针命中事实，不能与 `IsCandidateSelected` 分别绘制两个候选高亮。

完整状态、Theme、虚拟化、性能和验证边界见 [Select 候选交互设计](candidate-interaction-design.md)。

清除行为通过 `SelectHandle.ClearRequestedEvent` 冒泡到 Select，并调用 `ClearValue()` 清空 `SelectedOption` 和 `SelectedOptions`。

## Theme and Token Boundaries

Select 的默认视觉由 Select 专属主题、AddOnDecoratedBox、ListView 和 PopupHost 协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `SelectTheme.axaml` | 根模板、输入壳体、单选输入、多选结果区域、handle、popup 和基础 selector。 |
| `SelectAddOnDecoratedBoxTheme.axaml` | 输入框 variant、status、dropdown open、hover、pressed 和多选 padding。 |
| `SelectResultOptionsBoxTheme.axaml` | 多选标签布局、响应式标签布局和搜索输入承载。 |
| `SelectCandidateListTheme.axaml` | 候选列表基础 ListView 主题和默认候选模板。 |
| `SelectCandidateListItemTheme.axaml` | 候选项 active、selected、disabled 和隐藏已选项视觉；active 只由统一候选状态驱动。 |
| `SelectTagTheme.axaml` | 多选标签高度、背景、关闭按钮和禁用态。 |
| `SelectHandleTheme.axaml` | 展开、loading、清除、过滤指示和 Form feedback 图标。 |
| `PopupHostToken` | popup 阴影、圆角和 anchor margin。 |
| `SelectToken` | 多选标签、候选项、popup padding 和输入 padding。 |

选中结果的完整内容提示复用 `OverflowTip` attached behavior。模板只在单选文本和多选 tag 上声明 `IsShowOverflowTip` / `OverflowTipDelay` / `OverflowTipPlacement`，实际 tooltip 只在文本视觉宽度不足时写入 `ToolTip.Tip`，且不会覆盖用户手动设置的 tooltip。

候选弹层内容采用懒创建模型。`PART_Popup` 属于模板稳定 part；`PopupFrame` 和 `PART_CandidateList` 在打开前由 C# 创建并设置 `TemplatedParent`，关闭或重新套用模板时释放引用和事件订阅。

Token 边界：

SelectToken 是 Select 的控件级 Token scope，描述多选标签、候选项、候选弹层 padding 和 Select 输入内容 padding。输入壳体的通用边框、圆角、状态色、focus ring、disabled 背景和 AddOn 结构来自 SharedToken、AddOnDecoratedBoxToken 和 PopupHostToken。

SelectToken 不承载以下状态：

- `OptionsSource`、`Options`、`SelectedOption`、`SelectedOptions`、`DefaultValues` 等数据状态。
- `FilterValue`、`IsDropDownOpen`、`IsLoading`、`Status`、`Mode` 等运行状态。
- 候选项 hover、pressed、selected、disabled 的当前实例状态。
- Popup 当前宽度、高度、offset、placement 或异步加载结果。

## Customization Boundaries

维护 Select 时必须保持以下不变量：

- `Mode=Single` 使用 `SelectedOption`，`Mode=Multiple/Tags` 使用 `SelectedOptions`。
- `SelectedOption` 与 `SelectedOptions` 必须保持默认 `TwoWay` 绑定；`SelectedOptions` 绑定到 `INotifyCollectionChanged` 集合时，集合原地变化也必须刷新内部选择投影。
- `SelectionChanged` 必须在选择属性变化时继续触发，并包含模式、旧值和新值。
- `OptionsSource`、`Options` 和异步加载结果表达用户选项源；Tags 模式运行时动态选项不得写入这些用户选项源。
- 候选列表必须使用用户选项源和 Tags 运行时动态选项合成后的有效选项源。
- `OptionsSource` 变化必须按 `ItemKey` 优先、`Content` 兜底映射已有选择；已选 Tags 动态选项在没有正式选项可映射时必须保留。
- `DefaultValues` 只在当前选择为空时应用。
- `Tags` 模式必须保持有效过滤能力，并只在该模式下创建动态选项。
- `MaxCount` 达到上限时，未选候选项不可继续选择，已选候选项仍可取消。
- `IsHideSelectedOptions` 不能隐藏分组标题导致空状态判断错误。
- 鼠标和键盘必须共享唯一 active candidate，任意时刻最多显示一个未确认候选高亮。
- `:pointerover` 不能独立成为 Select 候选视觉或提交状态 owner。
- active candidate 变化不能提前修改 `SelectedOption` 或 `SelectedOptions`，`Enter` 必须提交当前视觉候选。
- 弹层打开、关闭事件的取消语义不能被绕过。
- 窗口失活、控件不可见或祖先不可见时必须关闭弹层。
- 重新套用模板或 detach 时必须释放旧 popup 内容、候选列表事件订阅、opened 期间订阅和 TopLevel deactivation 订阅。
- `SizeType=Custom` 必须继续以 `Middle` 作为未显式覆盖时的默认视觉基线。
- Template part、Token 名称、伪类和主题 selector 不得在未授权情况下重命名或删除。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- `AbstractSelect` 继续持有输入壳体、弹层、Form、CompactSpace 和 Motion 的基础契约。
- `Select` 继续持有选择、过滤、用户选项源、Tags 运行时动态选项、有效候选选项和异步加载状态。
- `OptionsSource` 写入不能破坏 `Options` 的内容集合语义。
- Tags 运行时动态选项不能写入用户 `OptionsSource`，也不能写入 XAML 内容子项 `Options`。
- 候选列表必须绑定到有效候选选项源，不能直接绑定到只读用户选项源。
- `SelectCandidateList` 必须是 active candidate 的唯一 owner；鼠标和键盘不能分别维护候选状态。
- `CandidateSelectedIndex`、`CandidateSelectedItem` 和已准备容器的 `IsCandidateSelected` 必须指向同一候选。
- `:pointerover` 只能产生鼠标候选迁移请求，不能独立决定 Select 候选背景或 `Enter` 提交目标。
- active candidate 迁移不能提前修改 public selection，selected 视觉必须继续覆盖候选 active 视觉。
- 选择同步中的 `_ignoreSyncSelection` 只用于防止候选列表和 public selection 相互递归，必须通过成对 helper 恢复，不能吞掉外部选择变化。
- `IgnorePropertyChange` 只用于内部恢复下拉开关状态，必须通过成对 helper 恢复，不能影响下一次外部 `IsDropDownOpen` 变化。
- `Tags` 动态选项只在 `Tags` 模式创建和清理，生命周期由 Select 内部运行时动态选项集合拥有。
- 单选过滤输入在弹层关闭时显示已选项文本，弹层打开且可过滤时清空为搜索输入。
- 多选搜索输入关闭弹层时只读并清空。
- 弹层取消事件必须能阻止打开或关闭。
- 重新套用模板和 detach 不能保留旧候选列表、旧 popup child、旧 template part 绑定或旧 TopLevel 订阅。
- `SelectToken` 不承载选项数据、过滤值、loading、选择集合、popup 打开状态或 Form 状态。
