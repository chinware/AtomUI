# Form 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

Form 家族的 Semantic Part 契约由 `FormItem` 声明：`root`、`label`、`content`、`extra`、`help`、`helpItem` 六个职责区域
（§1.1–1.6）。除 `helpItem` 外均为 `Single`，且来自 `FormItemTheme.axaml` 的唯一内置模板——
`Layout`、`FormLayout`、`RequiredMark`、验证状态和 `IsHideItemLabel` 只改变布局排列、可见性或有效视觉值，
不增删模板节点。`helpItem` 是运行时逐条创建的消息节点，数量随验证状态和 `Help` 变化（§1.6）。

`Form` 容器自身不注册 descriptor。上游稳定发布基线（6.6.0）的 Form semantic API（`root`、`label`、`content`、`help`、
`helpItem`、`extra`）中，form 级仅 `root` 落在表单根元素上；在 AtomUI 中对 Form owner 的整体定制通过
Avalonia 原生 owner 样式（`atom|Form` 类型 selector、ControlTheme、实例 Styles）直接完成，不需要
`.semantic-*` 契约。字段级语义（label、content、help、extra）全部由 `FormItem` 的模板节点承载。

### 1.1 `root`

| 字段 | 值 |
| --- | --- |
| Owner | `FormItem` |
| Part | `root` |
| Selector | FormItem 本身 |
| SelectorRoute | 不适用 |
| ContractType | `FormItem` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | FormItem owner |
| 职责 | 承载字段布局、验证状态、内容接入和 owner-scoped Semantic Style 入口。 |
| 相关 API | `Layout`、`LabelAlign`、`ValidateStatus`、`ValidateResult`、`IsRequired`、`Content` |
| 相关 Token | `FormToken`、SharedToken |
| 稳定性 | stable since 6.0 |

`root` 是控件自身，不声明 `.semantic-root` marker。它适合定制 FormItem 整体 `Margin`、`Opacity`、对齐和
尺寸约束；标签列与内容列的 Grid 几何由 `PART_BodyLayout` 布局算法拥有（见实现原理 §7.1），不通过 Semantic
Style 改写。`FormActionsItem` 复用 FormItem 模板，`atom|FormItem` 类型 selector 同样命中它，不注册独立
descriptor。

### 1.2 `label`

| 字段 | 值 |
| --- | --- |
| Owner | `FormItem` |
| Part | `label` |
| Selector | `.semantic-label` |
| SelectorRoute | `/template/ .semantic-label` |
| Style Type | `FormItemLabelStyle` |
| ContractType | `TextBlock` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `TextBlock#PART_Label`（`PART_LabelContentLayout` 内的标签文本） |
| 职责 | 承载 `LabelText` 的文本呈现，包含颜色、字号、对齐和换行。 |
| 相关 API | `LabelText`、`LabelAlign`、`LabelWrapping`、`IsHideItemLabel` |
| 相关 Token | `LabelColor`、`LabelFontSize` |
| 稳定性 | stable since 6.0 |

`label` 对齐上游 `classNames.label` 的文本语义：标记（冒号、必填星号、可选文案、tooltip 图标和自定义
mark）拥有各自的 token 驱动样式，不属于 `label` Part。它适合定制 `Foreground`、`FontSize`、`TextAlignment`、
`Opacity` 和 `Margin`；默认主题的 `LabelColor`、`LabelFontSize` 和 `LabelAlign` selector 可以被同优先级的用户
Semantic Style 覆盖。

布局边界：水平布局下 `PART_Label` 的 `MaxWidth` 通过 `TemplateBinding` 投影自 `LabelMaxWidth`（由
`LayoutUpdated` 测量标签列宽后写入）。用户 Setter 覆盖 `Width` / `MaxWidth` 会中断该测量闭环，导致换行与
裁剪行为退化；这两个属性槽不推荐定制，标签列宽度应通过 `LabelColInfo` API 控制。

### 1.3 `content`

| 字段 | 值 |
| --- | --- |
| Owner | `FormItem` |
| Part | `content` |
| Selector | `.semantic-content` |
| SelectorRoute | `/template/ .semantic-content` |
| Style Type | `FormItemContentStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `ContentPresenter#ContentPresenter`（`ContentFrame` 内主内容 presenter） |
| 职责 | 承载 `Content` 输入控件的最终呈现位置。 |
| 相关 API | `Content`、`IsValidateContentType`、`ChildrenSpacing` |
| 相关 Token | `FormItemSpacing`、SharedToken |
| 稳定性 | stable since 6.0 |

`content` 对齐上游 `classNames.content` 语义，是内容呈现区域，
不等于用户 `Content` 子控件本身。适合定制 `Margin`、`Opacity`、`VerticalAlignment` 和 presenter 级排版属性；
`Content` 创建的输入控件子树（LineEdit、Select 等自身的模板与 Semantic Part）不属于本 Part，继续由各自
owner 契约拥有。

布局边界：presenter 的 `MaxWidth` 投影自 `ContentPresenterMaxWidth`（`LayoutUpdated` 测量内容列宽后写入，
`FormLayout=Inline` 时为正无穷）。用户 Setter 覆盖 `Width` / `MaxWidth` 会中断测量闭环；内容列宽度应通过
`WrapperColInfo` API 控制。`ContentFrame` 的最小高度与标签列对齐，属于布局算法内部节点。

### 1.4 `extra`

| 字段 | 值 |
| --- | --- |
| Owner | `FormItem` |
| Part | `extra` |
| Selector | `.semantic-extra` |
| SelectorRoute | `/template/ .semantic-extra` |
| Style Type | `FormItemExtraStyle` |
| ContractType | `ContentPresenter` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `ContentPresenter#ExtraPresenter`（`PART_ContentLayout` 内 help 区域之后的 Extra presenter） |
| 职责 | 承载 `Extra` 与 `ExtraTemplate` 的最终呈现。 |
| 相关 API | `Extra`、`ExtraTemplate` |
| 相关 Token | SharedToken `ColorTextDescription`、`ControlHeightSM` |
| 稳定性 | stable since 6.0 |

`extra` 对齐上游 `classNames.extra` 语义，是 `Extra` API 的呈现区域。呈现位置与上游一致：位于输入控件
与 help 区域（`additional` 区）下方、与内容列对齐，不占用控件水平空间；默认主题使用说明文字色
（`ColorTextDescription`）与 `ControlHeightSM` 最小高度。`ExtraTemplate` 创建的用户子树不属于本 Part。
`Extra=null` 时 presenter 隐藏（`IsVisible` 绑定 `Extra` 非空），仍属于静态模板结构，marker 与 `Single`
数量不变。

### 1.5 `help`

| 字段 | 值 |
| --- | --- |
| Owner | `FormItem` |
| Part | `help` |
| Selector | `.semantic-help` |
| SelectorRoute | `/template/ .semantic-help` |
| Style Type | `FormItemHelpStyle` |
| ContractType | `StackPanel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | `StackPanel#ExtraInfoLayout`（`PART_ContentLayout` 内消息区域） |
| 职责 | 承载验证消息与 `Help` 文案的聚合展示区域。 |
| 相关 API | `Help`、`ValidateStatus`、`ErrorMessageForeground`、`WarningMessageForeground` |
| 相关 Token | `FormItemSpacing`、`ColorErrorText`、`ColorWarningText`、`ColorTextDescription` |
| 稳定性 | stable since 6.0 |

`help` 对齐上游 `classNames.help`（ErrorList 根节点）语义，覆盖同一段视觉职责：验证错误消息、警告
消息和 `Help` 帮助文案共同居住在该区域，逐条内容以 `helpItem` 节点呈现（§1.6）。`HasErrorOrWarningMsg=False`
时默认主题把该区域折叠为 `MaxHeight=0` 并由 `PART_ContentLayout` 的 `FormItemSpacing` 收紧间距；容器节点与
marker 始终存在，数量语义为 `Single`。

消息文本颜色由 `ValidateStatus` selector（`ColorErrorText` / `ColorWarningText` / `ColorTextDescription`）和
`ErrorMessageForeground` / `WarningMessageForeground` API 拥有，`help` 容器 Setter 不改变消息着色。适合定制
`Margin`、`Spacing`、`Opacity` 和对齐。

### 1.6 `helpItem`

| 字段 | 值 |
| --- | --- |
| Owner | `FormItem` |
| Part | `helpItem` |
| Selector | `.semantic-help-item` |
| SelectorRoute | `/template/ .semantic-help > .semantic-help-item` |
| Style Type | `FormItemHelpItemStyle` |
| ContractType | `TextBlock` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | `ExtraInfoLayout` 内逐条消息 `TextBlock`（运行时创建）与静态 `TextBlock#HelpText` |
| 职责 | 承载单条验证错误、警告消息或 `Help` 帮助文案的文本呈现。 |
| 相关 API | `Help`、`ValidateStatus`、`ErrorMessageForeground`、`WarningMessageForeground` |
| 相关 Token | `ColorErrorText`、`ColorWarningText`、`ColorTextDescription` |
| 稳定性 | stable since 6.0 |

`helpItem` 对齐上游 `classNames.helpItem`（ErrorList 逐条消息项）语义。FormItem 在验证结果变化时以代码逐条
创建消息 `TextBlock`，使用生成的 semantic class 常量添加 marker，并按"错误与警告消息在前、`Help` 文案在后"
的顺序排列；静态 `HelpText` 节点同样携带 marker，是 `Help` 文案的固定实例。消息节点带显式
`Foreground`（来自 `ErrorMessageForeground` / `WarningMessageForeground`），`HelpText` 着色由 `ValidateStatus`
selector 拥有；Semantic Style 服从 Avalonia 原生优先级，可以按需覆盖。

运行时边界：消息节点由验证结果应用的统一写入点创建、重建和清空，reset、detach 与新一轮验证不会遗留旧节点或
旧 marker；节点是 `ExtraInfoLayout` 的直接子节点，不进入 logical tree，不持有验证状态。适合定制 `FontSize`、
`Margin`、`Opacity` 和文本排版属性。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Form/Themes/FormTheme.axaml`

```xml
<Border Name="Frame">
    <ItemsPresenter Name="PART_ItemsPresenter" />
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Form
  -> FormItemDecorator (control theme, FormItemDecoratorTheme.axaml)
     -> PixelAlignedBorder (template-stable)
        -> StackPanel (template-stable)
           -> ContentPresenter#Content (internal-observable)
           -> ContentPresenter#Extra (internal-observable)
  -> FormItem (item container control theme, FormItemTheme.axaml)
     -> DockPanel#PART_RootLayout (template-stable)
        -> Panel#ItemDeleteButtonLayout (template-stable)
           -> ItemDeleteButton#ItemDeleteButton (internal-observable)
        -> Grid#PART_BodyLayout (template-stable)
           -> Panel#PART_LabelLayout (template-stable)
              -> DockPanel#PART_LabelContentLayout (template-stable)
                 -> TextBlock#PART_Colon (template-stable)
                 -> TextBlock#OptionalMark (template-stable)
                 -> IconPresenter#TooltipIconPresenter (internal-observable)
                 -> ContentPresenter#CustomRequiredMarkPresenter (internal-observable)
                 -> ContentPresenter#CustomOptionalMarkPresenter (internal-observable)
                 -> TextBlock#PART_DefaultRequireMark (template-stable)
                 -> TextBlock#PART_Label (template-stable)
           -> StackPanel#PART_ContentLayout (template-stable)
              -> Border#ContentFrame (template-stable)
                 -> ContentPresenter#ContentPresenter (internal-observable)
              -> StackPanel#ExtraInfoLayout (template-stable)
                 -> TextBlock#HelpText (template-stable)
              -> ContentPresenter#ExtraPresenter (internal-observable)
  -> Form (control theme, FormTheme.axaml)
     -> Border#Frame (template-stable)
        -> ItemsPresenter#PART_ItemsPresenter (template-stable)
  -> FormValidateFeedback (control theme, FormValidateFeedbackTheme.axaml)
     -> ContentPresenter#Content (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Form` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `FormItemDecorator` | control theme | `FormItemDecoratorTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `Child`, `CornerRadius`, `Extra` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `StackPanel` | template node (StackPanel) | `FormItemDecoratorTheme.axaml` | FormItemDecorator | `Child`, `Extra`, `ExtraTemplate`, `ItemSpacing` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Content` | template node (ContentPresenter) | `FormItemDecoratorTheme.axaml` | FormItemDecorator | `Child` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Extra` | template node (ContentPresenter) | `FormItemDecoratorTheme.axaml` | FormItemDecorator | `Extra`, `ExtraTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `FormItem` | item container control theme | `FormItemTheme.axaml` | 用户代码 / 控件宿主 | `Content`, `ContentPresenterMaxWidth`, `CustomOptionalMark`, `CustomOptionalMarkTemplate`, `CustomRequireMark`, `CustomRequireMarkTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_RootLayout` | template node (DockPanel) | `FormItemTheme.axaml` | FormItem | `Content`, `ContentPresenterMaxWidth`, `CustomOptionalMark`, `CustomOptionalMarkTemplate`, `CustomRequireMark`, `CustomRequireMarkTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemDeleteButtonLayout` | template node (Panel) | `FormItemTheme.axaml` | FormItem | `ItemDeleteButtonIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ItemDeleteButton` | template node (ItemDeleteButton) | `FormItemTheme.axaml` | FormItem | `ItemDeleteButtonIcon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_BodyLayout` | template node (Grid) | `FormItemTheme.axaml` | FormItem | `Content`, `ContentPresenterMaxWidth`, `CustomOptionalMark`, `CustomOptionalMarkTemplate`, `CustomRequireMark`, `CustomRequireMarkTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LabelLayout` | template node (Panel) | `FormItemTheme.axaml` | FormItem | `CustomOptionalMark`, `CustomOptionalMarkTemplate`, `CustomRequireMark`, `CustomRequireMarkTemplate`, `IsColonVisible`, `LabelMaxWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LabelContentLayout` | template node (DockPanel) | `FormItemTheme.axaml` | FormItem | `CustomOptionalMark`, `CustomOptionalMarkTemplate`, `CustomRequireMark`, `CustomRequireMarkTemplate`, `IsColonVisible`, `LabelMaxWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Colon` | template node (TextBlock) | `FormItemTheme.axaml` | FormItem | `IsColonVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `OptionalMark` | template node (TextBlock) | `FormItemTheme.axaml` | FormItem | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TooltipIconPresenter` | template node (IconPresenter) | `FormItemTheme.axaml` | FormItem | `Tooltip`, `TooltipIcon` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CustomRequiredMarkPresenter` | template node (ContentPresenter) | `FormItemTheme.axaml` | FormItem | `CustomRequireMark`, `CustomRequireMarkTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `CustomOptionalMarkPresenter` | template node (ContentPresenter) | `FormItemTheme.axaml` | FormItem | `CustomOptionalMark`, `CustomOptionalMarkTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_DefaultRequireMark` | template node (TextBlock) | `FormItemTheme.axaml` | FormItem | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Label` | template node (TextBlock) | `FormItemTheme.axaml` | FormItem | `LabelMaxWidth`, `LabelText`, `LabelWrapping` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentLayout` | template node (StackPanel) | `FormItemTheme.axaml` | FormItem | `Content`, `ContentPresenterMaxWidth`, `Extra`, `ExtraTemplate`, `Help` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentFrame` | template node (Border) | `FormItemTheme.axaml` | FormItem | `Content`, `ContentPresenterMaxWidth` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `FormItemTheme.axaml` | FormItem | `Content`, `ContentPresenterMaxWidth` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ExtraInfoLayout` | template node (StackPanel) | `FormItemTheme.axaml` | FormItem | `Help` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HelpText` | template node (TextBlock) | `FormItemTheme.axaml` | FormItem | `Help` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ExtraPresenter` | template node (ContentPresenter) | `FormItemTheme.axaml` | FormItem | `Extra`, `ExtraTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Form` | control theme | `FormTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `BoxShadow`, `CornerRadius`, `Padding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `FormTheme.axaml` | Form | `Background`, `BorderBrush`, `BorderThickness`, `BoxShadow`, `CornerRadius`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `FormTheme.axaml` | Form | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FormValidateFeedback` | control theme | `FormValidateFeedbackTheme.axaml` | Form | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Content` | template node (ContentPresenter) | `FormValidateFeedbackTheme.axaml` | FormValidateFeedback | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_BodyLayout` | `Grid` | FormItem 标签列和内容列布局。 |
| `PART_LabelLayout` | `Panel` | 标签、必填标记、tooltip、可选标记和冒号承载。 |
| `PART_ContentLayout` | `Panel` | 输入内容、错误消息/帮助文本与 Extra 承载。 |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

Form 的核心状态流：

```text
Form config
  → Prepare / sync FormItem style values
  → FormItem content via IFormItemAware
  → value changed / blur / submit validation trigger
  → FormItem validators
  → DataValidationErrors for error
  → ValidateStatus / ValidateResult / feedback / messages as projection
  → Form IsFormValid aggregation
  → SubmitButton watch state and submit result
```

验证触发模型：

- `OnChanged` 是 Form 的默认触发时机；内容控件触发 `IFormItemAware.ValueChanged` 后按 `ValidateDebounce` 延迟验证，使提交后或编辑中的错误能够随输入及时更新。
- `OnSubmit` 仅在手动验证或提交时验证，适合显式要求只在提交入口展示错误的表单。
- `OnBlur` 在 FormItem 失去焦点时按 `ValidateDebounce` 延迟验证。
- 手动 `Validate()`、`ValidateAsync()` 和 `Submit()` 直接进入验证流程，不依赖输入变化触发。

验证策略模型：

| 策略 | 行为 |
| --- | --- |
| `StopWhenFirstFailed` | 顺序执行验证器，遇到第一个 error 后停止，同时保留 error 前已产生的 warning。 |
| `Sequential` | 顺序执行所有验证器。 |
| `Parallel` | 并行执行所有验证器并聚合结果。 |

验证结果模型：

- `Error` 会使 Form 聚合为无效状态，并阻止 `Submit()` 继续提交。
- error 状态以内容控件的 `DataValidationErrors.HasErrors` 为最高优先级；Form validators 产生的 error 也写入同一 native validation 通道。
- `Warning` 会展示警告状态和警告消息，但 Form 聚合只把 error 作为提交阻断条件。
- `Validating` 和 `Default` 在 `IsFormValid` 聚合中不视为有效完成状态。
- 重置会取消未完成验证、清空 Form-owned 消息并把表单项扩展状态恢复为 `Default`；它只能清理由 Form 写入的 validation error，不能清掉 binding 或 ViewModel 写入的 native error。

提交与重置模型：

```text
Submit()
  → cancel previous form validation run
  → ValidateAsync()
  → if success, collect FormValues by FieldName
  → Validated(success, values)
  → Submitted(values)
```

```text
Reset()
  → cancel pending form validation
  → mark IsResetting
  → FormItem.ResetItemValue()
  → NotifyReset()
  → IsFormValid = false
  → ResetCompleted
  → clear IsResetting on dispatcher
```

## Theme and Token Boundaries

Form 视觉由 Form 根模板、FormItem 模板、FormValidateFeedback 模板和按钮主题协作完成。

| 主题或资源 | 职责 |
| --- | --- |
| `FormTheme.axaml` | Form 根模板、ItemsPresenter、表单项排列方向、反馈模板和默认操作图标。 |
| `FormItemTheme.axaml` | 标签、内容、辅助信息、删除按钮、必填标记、冒号和尺寸 selector。 |
| `FormItemDecoratorTheme.axaml` | 组合表单控件的水平内容和 Extra 布局。 |
| `FormValidateFeedbackTheme.axaml` | 根据 `ValidateStatus` 选择成功、警告、错误或验证中反馈内容。 |
| `SubmitButtonTheme.axaml` | 提交按钮默认 primary 风格和本地化文案。 |
| `ResetButtonTheme.axaml` | 重置按钮默认 default 风格和本地化文案。 |
| `FormToken` | 标签颜色、必填标记颜色、标签字体和表单项间距。 |
| `SharedToken` | 输入高度、图标尺寸、状态色、字体和通用 spacing。 |

视觉兼容边界：

- `FormItem` 标签列和内容列必须继续由 `PART_BodyLayout`、`PART_LabelLayout`、`PART_ContentLayout` 协作。
- `FormLayout=Inline` 必须保持横向表单项排列和 `InlineItemSpacing`。
- `FormLayout=Horizontal/Vertical` 必须保持纵向表单项排列。
- `SizeType=Custom` 的 FormItem 标签最小高度默认走 `Middle` 分支，子输入控件的自定义尺寸由子控件自身属性决定。

Token 边界：

FormToken 是 Form 的控件级 Token scope，描述标签视觉、必填标记、冒号间距和表单项布局间距。输入控件自身高度、边框、状态色、图标尺寸、字体基础值和通用 spacing 来自 SharedToken 或对应输入控件 Token。

FormToken 不承载以下状态：

- `FieldName`、`InitialValues`、提交 values 或业务数据。
- `ValidateStatus`、`ValidateResult`、错误消息、警告消息或验证中状态。
- `FormLayout`、`RequiredMark`、`ValidateTrigger`、`ValidateStrategy` 等实例配置。
- 动态表单项数量、删除按钮显示状态或响应式 breakpoint 当前值。

## Customization Boundaries

维护 Form 时必须保持以下不变量：

- 默认 `ValidateTrigger` 必须为 `OnChanged`，保持字段值变化时触发验证的默认语义。
- `FormItem.Content` 默认必须实现 `IFormItemAware`，否则应保持当前异常语义。
- `FormItem` 重新设置 Content 时必须释放旧内容的值变化订阅和 feedback 引用。
- 新验证运行必须取消旧验证和 debounce，旧异步结果不能覆盖新结果或 reset 后状态。
- `Reset()` 必须取消未完成验证，并避免 reset 引起的值变化触发新验证。
- `Reset()`、验证成功和重新验证只能清理 Form-owned `DataValidationErrors`，不得删除外部 native validation error。
- `Submit()` 只有在没有 error 时才收集值并触发提交事件。
- `Warning` 状态不得按 error 处理，除非获得明确行为变更授权。
- `FormItemDecorator` 必须继续向子控件转发 value、validation status、feedback、size、motion 和 style variant，并保持 native validation error 的目标控件稳定。
- `SubmitButton.IsWatchValidateResult=false` 时不得因为未找到 Form 或 Form 无效而强制禁用。
- Template part、token 名称、ControlTheme key 和验证枚举值不得在未授权情况下重命名或删除。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- Form 继续作为 FormItem 的 owner 和配置传播者，FormItem 继续作为字段级值和验证 owner。
- FormItem 的验证逻辑保持集中在验证职责域，不能重新散落到模板、布局或事件 handler 中。
- 新验证、reset、detach 和新 submit 必须取消旧验证运行。
- `IsResetting` 必须阻止 reset 期间的值变化触发验证，并在 dispatcher 队列中恢复。
- `ApplyValidationOutcome()` 必须继续作为 Form-owned `DataValidationErrors`、扩展验证状态、消息、feedback 和事件的统一写入点。
- `Warning` 和 `Error` 的聚合语义不能混淆。
- FormItem 内容替换必须释放旧内容订阅和旧 feedback。
- FormItemDecorator 的 `Child` 必须实现 `IFormItemAware`，并继续转发 feedback 和 validation status。
- 尺寸转发必须继续兼容 `ICustomizableSizeTypeAware` 和旧 `ISizeTypeAware`。
- C# relay binding 只用于运行时内容或 AXAML 无法表达的关系，新增 binding 必须有明确释放路径。
- FormToken 不承载实例数据、验证结果、表单值、loading、提交状态或具体业务字段。
