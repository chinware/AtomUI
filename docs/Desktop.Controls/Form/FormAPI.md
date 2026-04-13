# Form API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;  // Form, FormItem, FormActionsItem, SubmitButton, ResetButton, FormItemDecorator
namespace AtomUI.Controls;           // FormLayout, FormLabelAlign, FormRequiredMark 等枚举, IFormValidator 等接口
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### FormLayout

表单布局枚举。

| 值 | 说明 |
|---|---|
| `Horizontal` | 水平布局（默认）——标签在左、内容在右，一行一个字段 |
| `Vertical` | 垂直布局——标签在上、内容在下 |
| `Inline` | 内联布局——所有字段水平排列在同一行 |

### FormLabelAlign

标签对齐方式枚举。

| 值 | 说明 |
|---|---|
| `Left` | 标签左对齐 |
| `Right` | 标签右对齐（默认） |

### FormRequiredMark

必填标记模式枚举。

| 值 | 说明 |
|---|---|
| `Default` | 显示红色星号（*）标记必填字段（默认） |
| `Hidden` | 隐藏所有必填标记 |
| `Optional` | 在非必填字段旁显示"（optional）"文字 |
| `Customize` | 使用自定义模板（通过 `CustomRequireMark` / `CustomOptionalMark` 设置） |

### FormValidateTrigger

验证触发时机枚举。

| 值 | 说明 |
|---|---|
| `OnChanged` | 值变化时触发验证（默认） |
| `OnBlur` | 控件失焦时触发验证 |
| `OnSubmit` | 仅在提交时触发验证 |

### FormValidateStrategy

验证执行策略枚举。

| 值 | 说明 |
|---|---|
| `StopWhenFirstFailed` | 遇到第一个错误即停止（默认），但会收集已执行的警告 |
| `Sequential` | 顺序执行所有验证器，收集所有错误和警告 |
| `Parallel` | 并行执行所有验证器，收集所有错误和警告 |

### FormValidateStatus

字段验证状态枚举。

| 值 | 说明 |
|---|---|
| `Default` | 初始状态（未验证） |
| `Success` | 验证通过 |
| `Warning` | 验证警告（不阻止提交） |
| `Error` | 验证错误（阻止提交） |
| `Validating` | 验证进行中 |

### FormValidateResult

验证结果枚举（验证器返回值）。

| 值 | 说明 |
|---|---|
| `Success` | 验证通过 |
| `Error` | 验证错误 |
| `Warning` | 验证警告 |

### FormItemLayout

FormItem 内部布局枚举。

| 值 | 说明 |
|---|---|
| `Horizontal` | 标签在左、内容在右 |
| `Vertical` | 标签在上、内容在下 |

### InputControlStyleVariant（来自 `AtomUI.Controls`）

输入控件样式变体枚举。

| 值 | 说明 |
|---|---|
| `Outline` | 有边框（默认） |
| `Filled` | 填充背景 |
| `Borderless` | 无边框 |
| `Underlined` | 下划线 |

---

## Form 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `FormLayout` | `FormLayout` | `Horizontal` | 表单布局模式 |
| `LabelAlign` | `FormLabelAlign` | `Right` | 标签对齐方式 |
| `LabelWrapping` | `TextWrapping` | `NoWrap` | 标签文本换行方式 |
| `IsShowColon` | `bool` | `true` | 是否在标签后显示冒号 |
| `RequiredMark` | `FormRequiredMark` | `Default` | 必填标记模式 |
| `ValidateTrigger` | `FormValidateTrigger` | `OnChanged` | 全局验证触发时机 |
| `SizeType` | `SizeType` | `Middle` | 表单控件尺寸（共享属性） |
| `StyleVariant` | `InputControlStyleVariant` | `Outline` | 表单控件样式变体（共享属性） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用动画（共享属性） |
| `IsValidateFeedbackEnabled` | `bool` | `false` | 是否在控件内显示验证状态图标 |
| `LabelColInfo` | `MediaBreakGridLength?` | `null` | 标签列宽度（支持响应式） |
| `WrapperColInfo` | `MediaBreakGridLength?` | `null` | 内容列宽度（支持响应式） |
| `ScrollToFirstError` | `bool` | `false` | 验证失败时是否滚动到第一个错误字段 |
| `CustomRequireMark` | `object?` | `null` | 自定义必填标记内容 |
| `CustomRequireMarkTemplate` | `IDataTemplate?` | `null` | 自定义必填标记模板 |
| `CustomOptionalMark` | `object?` | `null` | 自定义可选标记内容 |
| `CustomOptionalMarkTemplate` | `IDataTemplate?` | `null` | 自定义可选标记模板 |
| `FeedbackTemplate` | `IDataTemplate?` | `null` | 验证反馈图标模板 |
| `ErrorMessageForeground` | `IBrush?` | `null`（由 Token 控制） | 错误消息前景色 |
| `WarningMessageForeground` | `IBrush?` | `null`（由 Token 控制） | 警告消息前景色 |
| `MinItemCount` | `int` | `1` | 最少数据项数量（控制删除按钮可见性） |
| `ItemDeleteButtonIcon` | `IIconTemplate?` | `null` | 删除按钮自定义图标 |
| `IsShowItemDeleteButton` | `bool` | `false` | 是否显示删除按钮 |
| `IsHideItemLabel` | `bool` | `false` | 是否隐藏 FormItem 的标签区域 |
| `SuccessFeedback` | `object?` | `null` | 成功状态反馈内容 |
| `SuccessFeedbackTemplate` | `IDataTemplate?` | `null` | 成功状态反馈模板 |
| `WarningFeedback` | `object?` | `null` | 警告状态反馈内容 |
| `WarningFeedbackTemplate` | `IDataTemplate?` | `null` | 警告状态反馈模板 |
| `ErrorFeedback` | `object?` | `null` | 错误状态反馈内容 |
| `ErrorFeedbackTemplate` | `IDataTemplate?` | `null` | 错误状态反馈模板 |
| `ValidatingFeedback` | `object?` | `null` | 验证中状态反馈内容 |
| `ValidatingFeedbackTemplate` | `IDataTemplate?` | `null` | 验证中状态反馈模板 |

### Form 只读属性（DirectProperty）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Values` | `IFormValues?` | 当前表单值（只读，提交时收集） |
| `InitialValues` | `IFormValues?` | 表单初始值（加载时自动填充） |
| `IsFormValid` | `bool?` | 表单整体验证状态（只读） |

---

## FormItem 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `LabelText` | `string?` | `null` | 标签文本 |
| `FieldName` | `string?` | `null` | 字段名称（表单值收集的 key） |
| `Content` | `Control?` | `null` | 表单项内容控件（标记为 `[Content]`） |
| `IsRequired` | `bool` | `false` | 是否必填（显示必填标记） |
| `Validators` | `IList<IFormValidator>?` | `null` | 验证器列表 |
| `ValidateTrigger` | `FormValidateTrigger` | 继承自 Form | 验证触发时机（可覆盖 Form 级设置） |
| `ValidateDebounce` | `TimeSpan` | `TimeSpan.Zero` | 验证防抖延迟 |
| `ValidateStrategy` | `FormValidateStrategy` | `StopWhenFirstFailed` | 验证执行策略 |
| `Layout` | `FormItemLayout` | `Horizontal` | FormItem 内部布局（可覆盖 Form 级设置） |
| `LabelAlign` | `FormLabelAlign` | 继承自 Form | 标签对齐方式 |
| `LabelWrapping` | `TextWrapping` | 继承自 Form | 标签换行方式 |
| `LabelColInfo` | `MediaBreakGridLength?` | 继承自 Form | 标签列宽度 |
| `WrapperColInfo` | `MediaBreakGridLength?` | 继承自 Form | 内容列宽度 |
| `Tooltip` | `string?` | `null` | 标签旁的提示文字 |
| `TooltipIcon` | `PathIcon?` | `QuestionCircleOutlined` | 提示图标 |
| `Help` | `string?` | `null` | 固定帮助文本（始终显示在控件下方） |
| `Extra` | `object?` | `null` | 额外内容（显示在控件右侧） |
| `ExtraTemplate` | `IDataTemplate?` | `null` | 额外内容模板 |
| `InitialValue` | `object?` | `null` | 字段初始值 |
| `IsValidateFeedbackEnabled` | `bool` | 继承自 Form | 是否显示验证反馈图标 |
| `IsValidateContentType` | `bool` | `true` | 是否验证内容控件类型（IFormItemAware） |
| `ChildrenSpacing` | `double` | `0` | 子控件间距 |

### FormItem 只读属性（DirectProperty）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `ValidateStatus` | `FormValidateStatus` | 当前验证状态 |
| `ValidateResult` | `FormValidateResult` | 当前验证结果 |

---

## SubmitButton 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsWatchValidateResult` | `bool` | `false` | 是否监听表单验证结果自动启用/禁用按钮 |

> `SubmitButton` 继承自 `Button`，拥有 `Button` 的所有属性（`ButtonType`、`Content`、`SizeType` 等）。

---

## ResetButton

> `ResetButton` 继承自 `Button`，无额外属性。点击时自动触发 Form 重置。

---

## FormItemDecorator 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Child` | `Control?` | `null` | 包装的子控件（标记为 `[Content]`） |
| `Extra` | `object?` | `null` | 额外内容（如单位文本） |
| `ExtraTemplate` | `IDataTemplate?` | `null` | 额外内容模板 |
| `ItemSpacing` | `double` | `0` | 子控件与额外内容的间距 |
| `StyleVariant` | `InputControlStyleVariant` | 继承自 FormItem | 样式变体 |
| `Status` | `InputControlStatus` | `Default` | 输入控件状态 |
| `SizeType` | `SizeType` | 继承自 FormItem | 尺寸 |
| `IsMotionEnabled` | `bool` | 继承自 FormItem | 动画开关 |

---

## 事件

### Form 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `AboutToValidate` | `EventHandler<EventArgs>` | 即将开始验证时触发 |
| `Validated` | `EventHandler<FormValidatedEventArgs>` | 验证完成后触发（包含验证结果和消息列表） |
| `Submitted` | `EventHandler<FormSubmittedEventArgs>` | 表单提交时触发（验证通过后，包含表单值） |
| `ItemValueChanged` | `EventHandler<FormItemValueChangedEventArgs>` | 任意字段值变化时触发 |
| `ResetCompleted` | `EventHandler` | 表单重置完成后触发 |

### FormItem 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `ValidateChanged` | `EventHandler<FormItemValidateChangedEventArgs>` | 字段验证状态变化时触发（路由事件） |

### SubmitButton 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Submit` | `EventHandler<RoutedEventArgs>` | 提交按钮点击时触发（路由事件，冒泡至 Form） |

### ResetButton 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Reset` | `EventHandler<RoutedEventArgs>` | 重置按钮点击时触发（路由事件，冒泡至 Form） |

---

## 公共方法

### Form 方法

| 方法 | 返回值 | 说明 |
|---|---|---|
| `Validate()` | `void` | 触发所有字段验证（异步执行） |
| `ValidateAsync(CancellationToken)` | `Task<FormValidateResult>` | 异步验证所有字段，返回汇总结果 |
| `Submit()` | `void` | 先验证，验证通过后收集值并触发 Submitted 事件 |
| `Reset()` | `void` | 重置所有字段到初始状态，清除验证状态 |
| `SetFormValues(IFormValues)` | `void` | 批量设置表单字段值 |
| `DeleteFormItem(IFormItem)` | `void` | 从表单中删除指定字段项 |

---

## 内置验证器

| 验证器类 | 说明 | 关键属性 |
|---|---|---|
| `FormStringNotEmptyValidator` | 字符串不为空/空白验证 | `Message` |
| `FormStringLengthValidator` | 字符串长度范围验证 | `Message`, `MinimumLength`, `MaximumLength` |
| `FormUrlValidator` | URL 格式验证 | `Message`, `WarningOnly` |
| `FormNotNullValidator` | 非 null 验证 | `Message` |
| `FormAssertValidator` | 断言验证（用于演示/测试） | `Message`, `AssertResult`, `WarningOnly`, `Delay` |

所有验证器通过 `FormValidatorProvider` 容器在 AXAML 中声明：

```xml
<atom:FormItem.Validators>
    <atom:FormValidatorProvider>
        <atom:FormStringNotEmptyValidator Message="必填字段!" />
        <atom:FormStringLengthValidator Message="长度至少6位!" MinimumLength="6" />
    </atom:FormValidatorProvider>
</atom:FormItem.Validators>
```

---

## 实现的接口

### Form

| 接口 | 作用 |
|---|---|
| `ISizeTypeAware` | 支持 `SizeType`（Small / Middle / Large）尺寸切换，自动传播到所有子控件 |
| `IMotionAwareControl` | 支持动画开关 |
| `IForm` | 表单核心接口（Validate / Submit / Reset / SetFormValues / DeleteFormItem） |
| `IInputControlStyleVariantAware` | 支持样式变体（Outlined / Filled / Borderless / Underlined），自动传播到子控件 |

### FormItem

| 接口 | 作用 |
|---|---|
| `IFormItem` | 表单字段项接口（FieldName、Content、GetItemValue / SetItemValue / ResetItemValue） |

### FormItemDecorator

| 接口 | 作用 |
|---|---|
| `IFormItemAware` | 代理子控件的表单值接口 |
| `IFormItemFeedbackAware` | 代理子控件的验证反馈接口 |
| `ISizeTypeAware` | 尺寸传播 |
| `IMotionAwareControl` | 动画传播 |
| `IInputControlStatusAware` | 输入状态传播 |
| `IInputControlStyleVariantAware` | 样式变体传播 |
