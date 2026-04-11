# AtomUI Form 系统设计原理

## 1. 概述

AtomUI 的 Form 系统是对 [Ant Design 5.0 Form 组件](https://ant.design/components/form) 的忠实 C# / Avalonia 实现。它提供了一套完整的表单数据收集、验证与提交机制，并通过接口契约系统使任意控件（包括自定义控件和第三方控件）都能无缝接入 Form 工作流。

### 核心设计理念

1. **接口驱动的松耦合**：Form 不直接依赖任何具体输入控件，而是通过 `IFormItemAware` 接口与内容控件交互。任何实现了该接口的控件都可以放入 Form 中参与数据收集和验证。
2. **属性级联传播**：Form 自动将 `SizeType`、`StyleVariant`、`IsMotionEnabled` 等配置通过 FormItem 中继传播到内容控件，实现统一的外观和行为控制。
3. **异步验证管线**：内置异步验证管线，支持多种验证触发时机、防抖、短路策略和 Warning-only 非阻塞验证。
4. **事件冒泡架构**：`SubmitButton` 和 `ResetButton` 通过 Avalonia 路由事件冒泡到 Form，实现声明式的提交/重置，无需手动编码事件绑定。

---

## 2. 架构总览

```
┌─────────────────────────────────────────────────────────────┐
│                        Form (IForm)                         │
│  ┌───────────────────────────────────────────────────────┐  │
│  │   ISizeTypeAware, IMotionAwareControl,                │  │
│  │   IInputControlStyleVariantAware                      │  │
│  │                                                       │  │
│  │   职责:                                               │  │
│  │   - 管理 FormItem 集合                                │  │
│  │   - 级联配置（SizeType, StyleVariant, Layout 等）     │  │
│  │   - 编排验证、提交、重置流程                          │  │
│  │   - 收集/分发表单值（FormValues）                     │  │
│  └───────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌───────────────────────────────────────────────────────┐  │
│  │              FormItem (IFormItem)                      │  │
│  │                                                       │  │
│  │   职责:                                               │  │
│  │   - 包裹一个内容控件                                  │  │
│  │   - 渲染 Label、必填标记、冒号、Tooltip               │  │
│  │   - 级联配置到内容控件                                │  │
│  │   - 编排单项验证逻辑                                  │  │
│  │   - 显示验证错误/警告消息                             │  │
│  │                                                       │  │
│  │   ┌───────────────────────────────────────────────┐   │  │
│  │   │     Content (IFormItemAware)                   │   │  │
│  │   │                                               │   │  │
│  │   │   - 提供/接收表单值                           │   │  │
│  │   │   - 通知值变更                                │   │  │
│  │   │   - 响应验证状态                              │   │  │
│  │   └───────────────────────────────────────────────┘   │  │
│  └───────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌───────────────────────────────────────────────────────┐  │
│  │          FormActionsItem                              │  │
│  │   SubmitButton / ResetButton / 自定义按钮             │  │
│  │   (通过路由事件冒泡到 Form)                           │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### 代码分层

| 层级 | 项目 | 内容 |
|---|---|---|
| 接口层 | `AtomUI.Controls` | `IForm`, `IFormItem`, `IFormItemAware`, `IFormItemFeedbackAware`, `IFormValidator`, `IFormValues`, `IFormValidateFeedback`, 枚举, 验证器基类, 事件参数 |
| 实现层 | `AtomUI.Desktop.Controls` | `Form`, `FormItem`, `FormActionsItem`, `FormItemDecorator`, `SubmitButton`, `ResetButton`, `FormToken`, 主题 |

---

## 3. 核心接口

### 3.1 IFormItemAware — 表单值感知（必须实现）

```csharp
public interface IFormItemAware
{
    event EventHandler ValueChanged;       // 值变更事件
    void SetFormValue(object? value);      // 外部设值（如 InitialValues、SetFormValues）
    object? GetFormValue();                // 获取当前值
    void ClearFormValue();                 // 重置为默认值
    void NotifyValidateStatus(FormValidateStatus status);  // 接收验证状态
}
```

**这是控件接入 Form 系统的唯一必要接口。** FormItem 在设置 `Content` 时会检查内容控件是否实现了此接口：

```csharp
// FormItem.NotifyContentAdded()
if (change.NewValue is IFormItemAware newFormItemAware)
{
    newFormItemAware.ValueChanged += HandleContentValueChanged;
}
else if (IsValidateContentType)
{
    throw new Exception($"Form item content: {change.NewValue?.GetType().FullName} not implement IFormItemAware interface");
}
```

如果 `IsValidateContentType` 为 `true`（默认值），不实现 `IFormItemAware` 会抛出异常。

### 3.2 IFormItemFeedbackAware — 验证反馈感知（可选）

```csharp
public interface IFormItemFeedbackAware
{
    void SetFeedbackControl(FormValidateFeedback? value);
}
```

当 `FormItem.IsValidateFeedbackEnabled = true` 时，FormItem 会构建 `FormValidateFeedback` 控件并通过此接口传递给内容控件。内容控件可以在自己的 UI 中显示验证状态图标（✓、✗、⚠、⏳）。

### 3.3 ISizeTypeAware — 尺寸感知（推荐实现）

```csharp
public interface ISizeTypeAware
{
    SizeType SizeType { get; set; }
}
```

FormItem 检测到内容控件实现此接口后，会自动将 Form 的 `SizeType` 级联绑定下去：

```csharp
if (Content is ISizeTypeAware)
{
    _disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, Content, SizeTypeProperty));
}
```

### 3.4 IInputControlStyleVariantAware — 样式变体感知（推荐实现）

```csharp
public interface IInputControlStyleVariantAware
{
    InputControlStyleVariant StyleVariant { get; set; }
}
```

Form 的 `StyleVariant`（Outlined / Filled / Borderless / Underlined）会自动级联到实现此接口的内容控件。

### 3.5 IMotionAwareControl — 动画感知（推荐实现）

```csharp
public interface IMotionAwareControl
{
    bool IsMotionEnabled { get; }
}
```

Form 的 `IsMotionEnabled` 会自动级联到实现此接口的内容控件。

### 3.6 IInputControlStatusAware — 输入状态感知（推荐实现）

```csharp
public interface IInputControlStatusAware
{
    InputControlStatus Status { get; set; }
}
```

虽然 FormItem 不会自动级联此属性，但它在 `NotifyValidateStatus` 中非常有用——控件可以根据验证状态设置自身的视觉状态（Error 红框、Warning 橙框等）。

---

## 4. 接口关系矩阵

| 接口 | 是否必须 | 作用 | 级联方式 |
|---|---|---|---|
| `IFormItemAware` | **必须** | 值的读写、变更通知、验证状态回调 | FormItem 直接调用接口方法 |
| `IFormItemFeedbackAware` | 可选 | 接收验证反馈 UI 控件 | FormItem 调用 `SetFeedbackControl()` |
| `ISizeTypeAware` | 推荐 | 尺寸级联 | `BindUtils.RelayBind` 自动绑定 |
| `IInputControlStyleVariantAware` | 推荐 | 样式变体级联 | `BindUtils.RelayBind` 自动绑定 |
| `IMotionAwareControl` | 推荐 | 动画开关级联 | `BindUtils.RelayBind` 自动绑定 |
| `IInputControlStatusAware` | 推荐 | 验证状态视觉反馈 | 控件在 `NotifyValidateStatus` 中自行设置 |

---

## 5. 数据流

### 5.1 值变更流

```
用户输入
  │
  ▼
Content 控件值变化
  │
  ├─► Content 触发 IFormItemAware.ValueChanged 事件
  │     │
  │     ▼
  │   FormItem.HandleContentValueChanged()
  │     │
  │     ├─► 根据 ValidateTrigger 决定是否触发验证
  │     │     OnChanged → 立即验证（可配 Debounce）
  │     │     OnBlur    → 失焦时验证
  │     │     OnSubmit  → 仅提交时验证
  │     │
  │     └─► 冒泡 ValueChangedEvent → Form.HandleFormItemValueChanged()
  │           │
  │           ├─► 触发 Form.ItemValueChanged 事件
  │           └─► 通知其他 FormItem（NotifyFormItemChanged）
  │
  ▼
其他 FormItem 收到通知（可实现联动逻辑）
```

### 5.2 验证流

```
触发验证（OnChanged / OnBlur / Submit / 手动调用 Validate）
  │
  ▼
FormItem.ValidateValueAsync()
  │
  ├─► 从 Content 获取值: IFormItemAware.GetFormValue()
  │
  ├─► 遍历 Validators 执行验证（支持三种策略）
  │     StopWhenFirstFailed → 遇到第一个失败即停止
  │     Sequential          → 逐个执行，收集全部结果
  │     Parallel            → 并行执行，收集全部结果
  │
  ├─► 设置 ValidateStatus (Default / Success / Warning / Error / Validating)
  │
  ├─► 回调 Content: IFormItemAware.NotifyValidateStatus(status)
  │
  ├─► 构建错误/警告消息 Inlines
  │
  └─► 冒泡 ValidateChangedEvent → Form.HandleFormItemValidateChanged()
        │
        └─► 更新 Form.IsFormValid
```

### 5.3 提交流

```
SubmitButton.Click
  │
  ├─► SubmitButton 冒泡 SubmitEvent
  │
  ▼
Form.HandleSubmitButtonClick()
  │
  ├─► Form.Submit()
  │     │
  │     ├─► 验证全部 FormItem
  │     │
  │     ├─► 如果全部通过 → 收集 FormValues → 触发 Submitted 事件
  │     │
  │     └─► 如果有失败 → 触发 Validated 事件（包含错误信息）
```

### 5.4 重置流

```
ResetButton.Click
  │
  ├─► ResetButton 冒泡 ResetEvent
  │
  ▼
Form.HandleResetButtonClick()
  │
  ├─► Form.Reset()
  │     │
  │     ├─► 遍历 FormItem 调用 ResetItemValue()
  │     │     │
  │     │     └─► IFormItemAware.ClearFormValue()
  │     │     └─► IFormItemAware.NotifyValidateStatus(Default)
  │     │
  │     └─► 触发 ResetCompleted 事件
```

---

## 6. 属性级联机制

Form 通过两种方式将配置传播给 FormItem 和内容控件：

### 6.1 Form → FormItem（Avalonia 属性绑定）

在 `PrepareContainerForItemOverride` 中通过 `[!Property]` 语法创建单向绑定：

```csharp
formItem[!FormItem.SizeTypeProperty]        = this[!SizeTypeProperty];
formItem[!FormItem.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
formItem[!FormItem.StyleVariantProperty]    = this[!StyleVariantProperty];
formItem[!FormItem.RequiredMarkProperty]    = this[!RequiredMarkProperty];
formItem[!FormItem.LabelWrappingProperty]   = this[!LabelWrappingProperty];
// ... 以及更多属性
```

### 6.2 FormItem → Content（接口检测 + RelayBind）

在 `NotifyContentAdded` 中通过接口检测决定是否绑定：

```csharp
if (Content is ISizeTypeAware)
{
    _disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, Content, SizeTypeProperty));
}
if (Content is IMotionAwareControl)
{
    _disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, Content, IsMotionEnabledProperty));
}
if (Content is IInputControlStyleVariantAware)
{
    _disposables.Add(BindUtils.RelayBind(this, StyleVariantProperty, Content, StyleVariantProperty));
}
```

### 级联链路图

```
Form.SizeType ──► FormItem.SizeType ──► Content.SizeType (if ISizeTypeAware)
Form.StyleVariant ──► FormItem.StyleVariant ──► Content.StyleVariant (if IInputControlStyleVariantAware)
Form.IsMotionEnabled ──► FormItem.IsMotionEnabled ──► Content.IsMotionEnabled (if IMotionAwareControl)
Form.LabelAlign ──► FormItem.LabelAlign
Form.RequiredMark ──► FormItem.RequiredMark
Form.ValidateTrigger ──► FormItem.ValidateTrigger
Form.FormLayout ──► FormItem.Layout (Horizontal→Horizontal, Vertical→Vertical)
```

---

## 7. 验证系统

### 7.1 验证器接口

```csharp
public interface IFormValidator
{
    string? Message { get; }      // 验证失败时的提示消息
    bool WarningOnly { get; }     // 是否仅警告（不阻塞提交）
    Task<FormValidateResult> ValidateAsync(string fieldName, object? value, CancellationToken cancellationToken);
}
```

### 7.2 内置验证器

| 验证器 | 用途 |
|---|---|
| `FormNotNullValidator` | 非空验证（值不为 null） |
| `FormStringNotEmptyValidator` | 字符串非空验证 |
| `FormStringLengthValidator` | 字符串长度范围验证 |
| `FormUrlValidator` | URL 格式验证 |
| `FormAssertValidator` | 断言验证（用于调试/演示） |

### 7.3 验证配置

```xml
<atom:FormItem LabelText="Username"
               FieldName="username"
               IsRequired="True"
               ValidateTrigger="OnChanged"
               ValidateDebounce="00:00:01"
               ValidateStrategy="StopWhenFirstFailed">
    <atom:FormItem.Validators>
        <atom:FormValidatorProvider>
            <atom:FormStringNotEmptyValidator Message="Please input your username!" />
            <atom:FormStringLengthValidator Message="Username must be 3-20 characters!"
                                            MinimumLength="3" MaximumLength="20" />
        </atom:FormValidatorProvider>
    </atom:FormItem.Validators>
    <atom:LineEdit />
</atom:FormItem>
```

### 7.4 验证触发时机

| `FormValidateTrigger` | 时机 |
|---|---|
| `OnChanged` | 内容控件值变更时立即触发（默认） |
| `OnBlur` | 内容控件失焦时触发 |
| `OnSubmit` | 仅在 Form.Submit() 时触发 |

### 7.5 验证策略

| `FormValidateStrategy` | 行为 |
|---|---|
| `StopWhenFirstFailed` | 遇到第一个失败的验证器即停止（默认） |
| `Sequential` | 逐个执行所有验证器，收集全部结果 |
| `Parallel` | 并行执行所有验证器，收集全部结果 |

### 7.6 验证状态

| `FormValidateStatus` | 含义 | UI 表现 |
|---|---|---|
| `Default` | 初始状态 | 无特殊样式 |
| `Success` | 验证通过 | 绿色（配合 Feedback 显示 ✓） |
| `Warning` | 警告（不阻塞提交） | 橙色 |
| `Error` | 错误（阻塞提交） | 红色 |
| `Validating` | 验证进行中 | 加载动画（配合 Feedback 显示 ⏳） |

### 7.7 WarningOnly 非阻塞验证

验证器设置 `WarningOnly="True"` 时，验证失败只会产生 Warning 状态，不会阻塞表单提交：

```xml
<atom:FormUrlValidator Message="URL is not a valid url!" WarningOnly="True" />
```

---

## 8. 特殊控件

### 8.1 FormActionsItem

`FormActionsItem` 是 `FormItem` 的特殊子类，用于放置表单操作按钮：

```csharp
public class FormActionsItem : FormItem
{
    static FormActionsItem()
    {
        IsValidateContentTypeProperty.OverrideDefaultValue<FormActionsItem>(false);
        IsValueItemProperty.OverrideDefaultValue<FormActionsItem>(false);
    }
}
```

- `IsValidateContentType = false`：不要求内容实现 `IFormItemAware`
- `IsValueItem = false`：不参与值收集和验证

### 8.2 SubmitButton

继承自 `Button`，点击时冒泡 `SubmitEvent` 到 Form：

```csharp
protected override void OnClick()
{
    base.OnClick();
    RaiseEvent(new RoutedEventArgs(SubmitEvent));
}
```

支持 `IsWatchValidateResult` 属性，自动根据表单验证状态启用/禁用按钮。

### 8.3 ResetButton

继承自 `Button`，点击时冒泡 `ResetEvent` 到 Form：

```csharp
protected override void OnClick()
{
    base.OnClick();
    RaiseEvent(new RoutedEventArgs(ResetEvent));
}
```

### 8.4 FormItemDecorator

一个中间层控件，实现了 `IFormItemAware` 和 `IFormItemFeedbackAware`，用于包裹已实现 `IFormItemAware` 的子控件，并添加额外的 UI 元素（如后缀文字）：

```xml
<atom:FormItem LabelText="InputNumber" FieldName="inputNumber">
    <atom:FormItemDecorator>
        <atom:NumericUpDown Minimum="1" Maximum="10" Width="120"/>
        <atom:FormItemDecorator.Extra>
            machines
        </atom:FormItemDecorator.Extra>
    </atom:FormItemDecorator>
</atom:FormItem>
```

`FormItemDecorator` 透传所有 `IFormItemAware` 调用到内部子控件，同时级联 `SizeType`、`StyleVariant` 等属性。

---

## 9. 表单布局

### 9.1 三种布局模式

| `FormLayout` | 说明 |
|---|---|
| `Horizontal` | Label 和控件在同一行（默认），Label 在左侧 |
| `Vertical` | Label 在控件上方 |
| `Inline` | 所有 FormItem 水平排列 |

### 9.2 响应式栅格

通过 `LabelColInfo` 和 `WrapperColInfo` 控制 Label 和内容区域的宽度比例：

```xml
<atom:Form LabelColInfo="8*" WrapperColInfo="16*">
    <!-- Label 占 8 份，内容占 16 份 = 1:2 比例 -->
</atom:Form>

<atom:Form LabelColInfo="200" WrapperColInfo="*">
    <!-- Label 固定 200px，内容占剩余空间 -->
</atom:Form>
```

支持 `MediaBreakGridLength` 响应式断点。

---

## 10. 已实现 IFormItemAware 的内置控件

| 控件 | 值类型 | 是否支持 Feedback |
|---|---|---|
| `TextBox` (LineEdit) | `string?` | ✅ `IFormItemFeedbackAware` |
| `TextArea` | `string?` | ✅ `IFormItemFeedbackAware` |
| `NumericUpDown` | `decimal?` | ✅ `IFormItemFeedbackAware` |
| `AbstractSelect` (Select, Cascader, TreeSelect) | `ISelectOption?` / `IList<ISelectOption>?` | ✅ `IFormItemFeedbackAware` |
| `ComboBox` | `string?` | ✅ `IFormItemFeedbackAware` |
| `Mentions` | `string?` | ✅ `IFormItemFeedbackAware` |
| `InfoPickerInput` (DatePicker, TimePicker, RangeDatePicker) | 日期/时间值 | ✅ `IFormItemFeedbackAware` |
| `AbstractAutoComplete` | `string?` | ✅ |
| `AbstractColorPicker` (ColorPicker) | `Color?` | ✅ |
| `FormItemDecorator` | 透传子控件值 | ✅ `IFormItemFeedbackAware` |

### 无需 IFormItemAware 即可放入 Form 的控件

以下控件可以放入 `FormItem`，但因为它们不是典型的"输入控件"，不实现 `IFormItemAware`。需要将 `IsValidateContentType="False"` 或使用 `FormActionsItem`：

| 控件 | 场景 |
|---|---|
| `CheckBox` | 简单勾选项（如"记住我"） |
| `RadioButtonGroup` | 单选组 |
| `OptionButtonGroup` | 选项按钮组 |
| `CheckBoxGroup` | 多选组 |
| `ToggleSwitch` | 开关 |
| `Slider` | 滑块 |
| `Rate` | 评分 |
| `Upload` | 文件上传 |
| `TreeView` | 树视图 |
| `Button` | 普通按钮 |
| `TextBlock` | 纯文本 |

> **注意**：这些控件放入 FormItem 后可以正常显示，但不参与 Form 的验证和值收集。如果需要参与验证，需要实现 `IFormItemAware` 或用 `FormItemDecorator` 包裹。

