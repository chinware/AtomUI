# 让控件支持 AtomUI Form 的开发指南

## 1. 概述

本文档详细说明如何让一个自定义控件（或第三方控件）能够在 AtomUI 的 `Form` 中使用，参与数据收集、验证和提交流程。

---

## 2. 快速判断：你的控件需要实现什么？

```
你的控件是"输入控件"吗？（用户通过它输入/选择值）
│
├── 是 → 必须实现 IFormItemAware
│         │
│         ├── 控件需要显示验证状态图标吗？ → 实现 IFormItemFeedbackAware
│         ├── 控件需要响应 Form 统一尺寸？ → 实现 ISizeTypeAware
│         ├── 控件需要响应 Form 统一样式变体？ → 实现 IInputControlStyleVariantAware
│         ├── 控件需要显示验证状态边框？ → 实现 IInputControlStatusAware
│         └── 控件需要响应 Form 动画开关？ → 实现 IMotionAwareControl
│
└── 否（如按钮、文本标签、纯展示控件）
          │
          └── 放在 FormActionsItem 中（无需实现任何接口）
              或 FormItem + IsValidateContentType="False"
```

---

## 3. 接口实现详解

### 3.1 IFormItemAware — 核心接口（必须）

这是控件接入 Form 的**唯一必要接口**。它定义了 Form 与控件之间的数据交互协议。

#### 接口定义

```csharp
public interface IFormItemAware
{
    /// <summary>
    /// 当控件值发生变化时触发。Form 据此判断是否需要验证。
    /// </summary>
    event EventHandler ValueChanged;

    /// <summary>
    /// Form 调用此方法设置控件的值（如初始值、外部赋值）。
    /// </summary>
    void SetFormValue(object? value);

    /// <summary>
    /// Form 调用此方法获取控件的当前值（用于验证和提交）。
    /// </summary>
    object? GetFormValue();

    /// <summary>
    /// Form 重置时调用此方法清除控件的值。
    /// </summary>
    void ClearFormValue();

    /// <summary>
    /// Form 验证完成后调用此方法通知控件当前验证状态。
    /// 控件可据此更新视觉样式（如红色边框）。
    /// </summary>
    void NotifyValidateStatus(FormValidateStatus status);
}
```

#### 实现模板

```csharp
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace YourNamespace;

public class MyInputControl : TemplatedControl, IFormItemAware
{
    // 1. 定义控件的值属性
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<MyInputControl, string?>(nameof(Value));

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    // 2. 监听值变化
    static MyInputControl()
    {
        ValueProperty.Changed.AddClassHandler<MyInputControl>(
            (control, _) => control.HandleValueChanged());
    }

    // ===== IFormItemAware 实现 =====

    // 3. 声明值变更事件（显式接口实现，不暴露为公共 API）
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    // 4. 在值变化时触发事件
    private void HandleValueChanged()
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    // 5. 实现值的设置（注意类型转换）
    void IFormItemAware.SetFormValue(object? value)
        => NotifySetFormValue(value?.ToString());

    // 6. 实现值的获取
    object? IFormItemAware.GetFormValue()
        => NotifyGetFormValue();

    // 7. 实现值的清除
    void IFormItemAware.ClearFormValue()
        => NotifyClearFormValue();

    // 8. 实现验证状态通知
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status)
        => NotifyValidateStatus(status);

    // ===== 可覆写的保护方法（便于子类扩展） =====

    protected virtual void NotifySetFormValue(string? value)
    {
        SetCurrentValue(ValueProperty, value);
    }

    protected virtual string? NotifyGetFormValue()
    {
        return Value;
    }

    protected virtual void NotifyClearFormValue()
    {
        SetCurrentValue(ValueProperty, null);
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
        // 基本实现：如果控件实现了 IInputControlStatusAware，可以在此设置状态
    }
}
```

#### 关键要点

1. **使用显式接口实现**：将 `IFormItemAware` 的成员作为显式接口实现（`void IFormItemAware.SetFormValue(...)`），避免污染控件的公共 API。
2. **使用 `SetCurrentValue` 而非 `SetValue`**：`SetCurrentValue` 设置值但不覆盖外部绑定，是 Form 场景下的最佳实践。
3. **提供 `protected virtual` 方法**：使用委托到 `protected virtual` 方法的模式，使子类可以覆写行为。
4. **值变化必须触发事件**：确保控件值每次变化时都调用 `_formValueChanged?.Invoke(this, EventArgs.Empty)`，否则 Form 无法感知值变更。
5. **注意 `SetFormValue` 的类型转换**：Form 通过 `object?` 传递值，控件需要做适当的类型转换。

### 3.2 IFormItemFeedbackAware — 验证反馈（可选）

当控件需要在输入框内显示验证状态图标（✓、✗、⚠、⏳）时，实现此接口。

```csharp
public interface IFormItemFeedbackAware
{
    void SetFeedbackControl(FormValidateFeedback? value);
}
```

#### 实现方式

```csharp
public class MyInputControl : TemplatedControl,
                               IFormItemAware,
                               IFormItemFeedbackAware
{
    // 定义内部属性存储反馈控件
    internal static readonly StyledProperty<FormValidateFeedback?> FormFeedbackProperty =
        AvaloniaProperty.Register<MyInputControl, FormValidateFeedback?>(nameof(FormFeedback));

    internal FormValidateFeedback? FormFeedback
    {
        get => GetValue(FormFeedbackProperty);
        set => SetValue(FormFeedbackProperty, value);
    }

    // 实现接口
    void IFormItemFeedbackAware.SetFeedbackControl(FormValidateFeedback? value)
        => NotifySetFeedBackControl(value);

    protected virtual void NotifySetFeedBackControl(FormValidateFeedback? value)
    {
        FormFeedback = value;
    }

    // ... 在模板中使用 {TemplateBinding FormFeedback} 展示反馈控件
}
```

### 3.3 ISizeTypeAware — 尺寸级联（推荐）

```csharp
public class MyInputControl : TemplatedControl, ISizeTypeAware
{
    // 必须使用 AddOwner，不能自行 Register
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<MyInputControl>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
}
```

> ⚠️ **必须使用 `AddOwner`**，确保属性名 `"SizeType"` 与系统一致，级联绑定才能正确工作。

### 3.4 IInputControlStyleVariantAware — 样式变体级联（推荐）

```csharp
public class MyInputControl : TemplatedControl, IInputControlStyleVariantAware
{
    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<MyInputControl>();

    public InputControlStyleVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }
}
```

### 3.5 IInputControlStatusAware — 验证状态视觉反馈（推荐）

当控件需要根据验证状态显示不同的边框颜色时：

```csharp
public class MyInputControl : TemplatedControl,
                               IFormItemAware,
                               IInputControlStatusAware
{
    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<MyInputControl>();

    public InputControlStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    // 在 NotifyValidateStatus 中映射 FormValidateStatus → InputControlStatus
    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
        var inputStatus = status switch
        {
            FormValidateStatus.Error   => InputControlStatus.Error,
            FormValidateStatus.Warning => InputControlStatus.Warning,
            _                          => InputControlStatus.Default
        };
        SetCurrentValue(StatusProperty, inputStatus);
    }
}
```

然后在 AXAML 主题中使用伪类或属性选择器实现视觉反馈：

```xml
<Style Selector="^[Status=Error]">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorError}" />
</Style>
<Style Selector="^[Status=Warning]">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorWarning}" />
</Style>
```

### 3.6 IMotionAwareControl — 动画开关级联（可选）

```csharp
public class MyInputControl : TemplatedControl, IMotionAwareControl
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MyInputControl>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
}
```

---

## 4. 完整实现示例

### 4.1 简单文本输入控件

一个只需要基本 Form 集成的控件：

```csharp
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace MyApp.Controls;

public class SimpleInput : TemplatedControl, IFormItemAware
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<SimpleInput, string?>(nameof(Text));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    static SimpleInput()
    {
        TextProperty.Changed.AddClassHandler<SimpleInput>(
            (control, _) => control.HandleValueChanged());
    }

    #region IFormItemAware 实现
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value)
        => SetCurrentValue(TextProperty, value?.ToString());

    object? IFormItemAware.GetFormValue() => Text;

    void IFormItemAware.ClearFormValue()
        => SetCurrentValue(TextProperty, null);

    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) { }

    private void HandleValueChanged()
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
```

在 AXAML 中使用：

```xml
<atom:FormItem LabelText="Simple Input" FieldName="simpleInput">
    <atom:FormItem.Validators>
        <atom:FormValidatorProvider>
            <atom:FormStringNotEmptyValidator Message="Please input!" />
        </atom:FormValidatorProvider>
    </atom:FormItem.Validators>
    <local:SimpleInput />
</atom:FormItem>
```

### 4.2 完整功能的复合输入控件

一个包含完整 Form 集成的复合输入控件：

```csharp
using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace MyApp.Controls;

// 值类型
public record PriceInfo(decimal Value, string Unit);

public class PriceInput : TemplatedControl,
                          IFormItemAware,
                          ISizeTypeAware,
                          IMotionAwareControl,
                          IInputControlStatusAware,
                          IInputControlStyleVariantAware
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<PriceInput>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<PriceInput>();

    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<PriceInput>();

    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<PriceInput>();

    public static readonly StyledProperty<PriceInfo?> ValueProperty =
        AvaloniaProperty.Register<PriceInput, PriceInfo?>(nameof(Value));

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public InputControlStyleVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public InputControlStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public PriceInfo? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    #endregion

    // 内部子控件
    private NumericUpDown? _numberInput;
    private Select? _unitSelect;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        // 查找模板中的子控件并订阅变更事件
        _numberInput = e.NameScope.Find<NumericUpDown>("NumberInput");
        _unitSelect  = e.NameScope.Find<Select>("UnitSelect");

        if (_numberInput != null)
            _numberInput.ValueChanged += (_, _) => UpdateValue();
        if (_unitSelect != null)
            _unitSelect.SelectionChanged += (_, _) => UpdateValue();
    }

    private void UpdateValue()
    {
        var amount = _numberInput?.Value ?? 0;
        var unit   = _unitSelect?.SelectedOption?.Content?.ToString() ?? "RMB";
        Value = new PriceInfo(amount, unit);
        _formValueChanged?.Invoke(this, EventArgs.Empty); // 通知 Form 值已变更
    }

    #region IFormItemAware 实现

    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value)
        => NotifySetFormValue(value as PriceInfo);

    object? IFormItemAware.GetFormValue()
        => NotifyGetFormValue();

    void IFormItemAware.ClearFormValue()
        => NotifyClearFormValue();

    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status)
        => NotifyValidateStatus(status);

    protected virtual void NotifySetFormValue(PriceInfo? value)
    {
        SetCurrentValue(ValueProperty, value);
    }

    protected virtual PriceInfo? NotifyGetFormValue() => Value;

    protected virtual void NotifyClearFormValue()
    {
        SetCurrentValue(ValueProperty, null);
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
        var inputStatus = status switch
        {
            FormValidateStatus.Error   => InputControlStatus.Error,
            FormValidateStatus.Warning => InputControlStatus.Warning,
            _                          => InputControlStatus.Default
        };
        SetCurrentValue(StatusProperty, inputStatus);
    }

    #endregion
}
```

### 4.3 自定义验证器

```csharp
using AtomUI.Controls;

namespace MyApp.Validators;

public class PriceValidator : AbstractFormValidator
{
    protected override async Task<bool> NotifyValidateAsync(
        string fieldName,
        object? value,
        CancellationToken cancellationToken)
    {
        var price = value as PriceInfo;
        return await Task.FromResult(price != null && price.Value > 0);
    }
}
```

在 AXAML 中使用：

```xml
<atom:FormItem LabelText="Price" FieldName="price">
    <atom:FormItem.Validators>
        <atom:FormValidatorProvider>
            <local:PriceValidator Message="Price must be greater than zero!" />
        </atom:FormValidatorProvider>
    </atom:FormItem.Validators>
    <local:PriceInput />
</atom:FormItem>
```

---

## 5. 使用 FormItemDecorator 包裹控件

当你需要在 FormItem 中为控件添加额外 UI（如后缀说明文字），但不想修改原控件时，可以用 `FormItemDecorator`：

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

`FormItemDecorator` 自身实现了 `IFormItemAware` 和 `IFormItemFeedbackAware`，它会将所有 Form 交互透传给内部子控件。

> **注意**：子控件本身必须已经实现了 `IFormItemAware`。`FormItemDecorator` 不会凭空为非 Form-aware 的控件添加 Form 能力。

---

## 6. 自定义 FormItem 子类

当需要实现 FormItem 之间的联动（如"Gender"选择变化后自动填充"Note"），可以继承 `FormItem` 并覆写 `NotifyFormItemChanged`：

```csharp
public class NoteFormItem : FormItem
{
    protected override void NotifyFormItemChanged(IFormItem formItem)
    {
        if (formItem.FieldName == "gender")
        {
            if (formItem.Content is Select select && select.Mode == SelectMode.Single)
            {
                if (formItem.GetItemValue() is ISelectOption selectOption)
                {
                    SetItemValue($"Hi, {selectOption.Content}!");
                }
            }
        }
    }
}
```

当 Form 中任何 FormItem 的值发生变化时，Form 会遍历其他 FormItem 调用 `NotifyFormItemChanged`，传入变化了值的 FormItem 引用。

---

## 7. 实现检查清单

为确保控件的 Form 集成完整且正确，请逐项核对：

### 必须项

- [ ] 实现 `IFormItemAware` 接口（如果控件是输入控件）
- [ ] `ValueChanged` 事件在每次值变化时触发
- [ ] `SetFormValue` 使用 `SetCurrentValue` 设置值属性
- [ ] `GetFormValue` 返回正确的值（注意类型）
- [ ] `ClearFormValue` 将值重置为默认状态
- [ ] `NotifyValidateStatus` 正确处理验证状态

### 推荐项

- [ ] 实现 `ISizeTypeAware`（使用 `AddOwner`，不要自行 `Register`）
- [ ] 实现 `IInputControlStyleVariantAware`（使用 `AddOwner`）
- [ ] 实现 `IInputControlStatusAware`（使用 `AddOwner`），并在 `NotifyValidateStatus` 中映射状态
- [ ] 实现 `IMotionAwareControl`（使用 `AddOwner`）

### 可选项

- [ ] 实现 `IFormItemFeedbackAware`（如需显示验证状态图标）
- [ ] 提供自定义 `IFormValidator`（如控件的值类型需要特殊验证逻辑）

### 常见错误

| 错误 | 后果 | 解决方案 |
|---|---|---|
| 未实现 `IFormItemAware` | 运行时抛出异常 | 实现接口或设置 `IsValidateContentType="False"` |
| 值变化时未触发 `ValueChanged` | Form 无法感知值变更，验证不会触发 | 确保在值变化的回调中调用 `_formValueChanged?.Invoke(...)` |
| `SizeType` 使用 `Register` 而非 `AddOwner` | 级联绑定不生效，尺寸不会跟随 Form 变化 | 使用 `SizeTypeControlProperty.SizeTypeProperty.AddOwner<T>()` |
| `SetFormValue` 中使用 `SetValue` 而非 `SetCurrentValue` | 可能覆盖用户的外部绑定 | 使用 `SetCurrentValue` |
| `GetFormValue` 返回类型不匹配验证器预期 | 验证器无法正确处理值 | 确保返回值类型与验证器的类型检查匹配 |

---

## 8. AXAML 中 Form 使用速查

### 基本表单

```xml
<atom:Form LabelColInfo="8*" WrapperColInfo="16*">
    <atom:FormItem LabelText="Username" FieldName="username" IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>
    <atom:FormActionsItem>
        <atom:SubmitButton />
    </atom:FormActionsItem>
</atom:Form>
```

### 带初始值的表单

```csharp
var values = new FormValues();
values.Add("username", "admin");
values.Add("remember", true);
myForm.InitialValues = values;
```

### 外部设值

```csharp
var values = new FormValues();
values.Add("url", "https://example.com/");
myForm.SetFormValues(values);
```

### 监听表单事件

```csharp
myForm.Submitted += (sender, args) =>
{
    // args.Values 包含所有表单值
    var username = args.Values["username"];
};

myForm.Validated += (sender, args) =>
{
    if (args.Result == FormValidateResult.Error)
    {
        // args.ErrorMessages 包含验证失败的信息
    }
};

myForm.ItemValueChanged += (sender, args) =>
{
    // args.Item 是变化了值的 FormItem
    // args.Value 是新值
};
```

### 动态添加 FormItem

```csharp
var formItem = new FormItem
{
    FieldName  = "newField",
    LabelText  = "New Field",
    Content    = new LineEdit(),
    Validators = new List<IFormValidator>
    {
        new FormStringNotEmptyValidator { Message = "Required!" }
    }
};
myForm.Items.Insert(index, formItem);
```

### SubmitButton 自动启用/禁用

```xml
<!-- IsWatchValidateResult="True" 时，按钮根据表单验证结果自动启用/禁用 -->
<atom:SubmitButton IsWatchValidateResult="True" />
```

