# Form 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Form` | 数据录入控件根语义区域，承载 public API、值状态、验证状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `input` | `输入或编辑区域` | 承载用户输入、当前值、占位、格式化或只读状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `trigger` | `触发区域` | 承载清除、展开、提交、步进、上传或辅助操作。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `popup` | `弹层或候选区域` | 承载下拉、候选项、日历、颜色面板或异步内容。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `validation` | `校验反馈区域` | 承载 Form、status、错误、警告、help 或 loading 状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

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
                 -> DockPanel (template-stable)
                    -> ContentPresenter#ExtraPresenter (internal-observable)
                    -> ContentPresenter#ContentPresenter (internal-observable)
              -> StackPanel#ExtraInfoLayout (template-stable)
                 -> TextBlock#PART_ErrorMsg (template-stable)
                 -> TextBlock#HelpText (template-stable)
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
| `PART_ContentLayout` | template node (StackPanel) | `FormItemTheme.axaml` | FormItem | `Content`, `ContentPresenterMaxWidth`, `ErrorMessageInlines`, `Extra`, `ExtraTemplate`, `Help` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentFrame` | template node (Border) | `FormItemTheme.axaml` | FormItem | `Content`, `ContentPresenterMaxWidth`, `Extra`, `ExtraTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `FormItemTheme.axaml` | FormItem | `Content`, `ContentPresenterMaxWidth`, `Extra`, `ExtraTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ExtraPresenter` | template node (ContentPresenter) | `FormItemTheme.axaml` | FormItem | `Extra`, `ExtraTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `FormItemTheme.axaml` | FormItem | `Content`, `ContentPresenterMaxWidth` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ExtraInfoLayout` | template node (StackPanel) | `FormItemTheme.axaml` | FormItem | `ErrorMessageInlines`, `Help` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ErrorMsg` | template node (TextBlock) | `FormItemTheme.axaml` | FormItem | `ErrorMessageInlines` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `HelpText` | template node (TextBlock) | `FormItemTheme.axaml` | FormItem | `Help` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Form` | control theme | `FormTheme.axaml` | 用户代码 / 控件宿主 | `CornerRadius`, `Padding` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Frame` | template node (Border) | `FormTheme.axaml` | Form | `CornerRadius`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ItemsPresenter` | template node (ItemsPresenter) | `FormTheme.axaml` | Form | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FormValidateFeedback` | control theme | `FormValidateFeedbackTheme.axaml` | Form | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Content` | template node (ContentPresenter) | `FormValidateFeedbackTheme.axaml` | FormValidateFeedback | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_BodyLayout` | `Grid` | FormItem 标签列和内容列布局。 |
| `PART_LabelLayout` | `Panel` | 标签、必填标记、tooltip、可选标记和冒号承载。 |
| `PART_ContentLayout` | `Panel` | 输入内容、Extra、错误消息和帮助文本承载。 |

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
