# 自定义控件接入 StyleVariant 与 Status 开发指南

## 1. 概述

本文档面向控件开发者，详细说明如何让自定义控件支持 AtomUI 的样式变体（`InputControlStyleVariant`）和验证状态（`InputControlStatus`）系统，使其与 AtomUI 内置输入控件表现一致，并能在 Form 中自动级联。

> 前置阅读：[输入控件样式变体与验证状态设计原理](./InputControlStyleAndStatusDesign.md)、[Form 集成指南](./FormIntegrationGuide.md)

---

## 2. 快速判断：你的控件需要实现哪些接口？

```
你的控件是输入类控件吗？（用户通过它输入/选择/切换值）
│
├── 是 → 建议同时实现 IInputControlStyleVariantAware 和 IInputControlStatusAware
│        │
│        ├── 控件需要 Form 验证？ → 必须同时实现 IFormItemAware
│        ├── 控件需要跟随 Form 尺寸？ → 实现 ISizeTypeAware
│        └── 控件需要跟随 Form 动画开关？ → 实现 IMotionAwareControl
│
└── 否 → 不需要实现这两个接口
```

---

## 3. 基础实现模板

### 3.1 最小实现

以下展示了一个自定义输入控件同时支持 StyleVariant 和 Status 的最小实现：

```csharp
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace MyApp.Controls;

public class MyInput : TemplatedControl,
                       IInputControlStyleVariantAware,
                       IInputControlStatusAware
{
    // ===== 1. 使用 AddOwner 注册共享属性 =====
    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<MyInput>();

    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<MyInput>();

    // ===== 2. 属性包装器 =====
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
}
```

### 3.2 完整实现（含 Form 集成）

以下是一个支持 Form 完整集成的自定义输入控件模板：

```csharp
using AtomUI;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace MyApp.Controls;

public class MyInput : TemplatedControl,
                       IInputControlStyleVariantAware,
                       IInputControlStatusAware,
                       ISizeTypeAware,
                       IMotionAwareControl,
                       IFormItemAware
{
    #region 公共属性定义

    // ===== 共享属性：必须使用 AddOwner =====
    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<MyInput>();

    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<MyInput>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<MyInput>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MyInput>();

    // ===== 控件自有属性 =====
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<MyInput, string?>(nameof(Value));

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

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    #endregion

    static MyInput()
    {
        ValueProperty.Changed.AddClassHandler<MyInput>(
            (control, _) => control.HandleValueChanged());
    }

    #region IFormItemAware 实现

    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value?.ToString());
    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);

    private void HandleValueChanged()
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(string? value)
    {
        SetCurrentValue(ValueProperty, value);
    }

    protected virtual string? NotifyGetFormValue() => Value;

    protected virtual void NotifyClearFormValue()
    {
        SetCurrentValue(ValueProperty, null);
    }

    // ===== 关键：将 FormValidateStatus 映射为 InputControlStatus =====
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

---

## 4. 控件模板中传递 StyleVariant 和 Status

当自定义控件的模板中包含 AtomUI 内置输入控件时，**必须** 将 StyleVariant 和 Status 通过 `{TemplateBinding}` 向下传递。

### 4.1 AXAML 模板示例

```xml
<ControlTheme x:Key="{x:Type local:MyInput}" TargetType="local:MyInput">
    <Setter Property="Template">
        <ControlTemplate>
            <StackPanel Orientation="Horizontal" Spacing="10">
                <!-- 将 StyleVariant、Status、SizeType 全部传递给内部 LineEdit -->
                <atom:LineEdit SizeType="{TemplateBinding SizeType}"
                               IsMotionEnabled="{TemplateBinding IsMotionEnabled}"
                               Status="{TemplateBinding Status}"
                               StyleVariant="{TemplateBinding StyleVariant}"
                               Width="200"
                               Text="{TemplateBinding Value, Mode=TwoWay}" />
                <!-- 非输入控件不需要传递 StyleVariant 和 Status -->
                <atom:Button SizeType="{TemplateBinding SizeType}"
                             IsMotionEnabled="{TemplateBinding IsMotionEnabled}">
                    Submit
                </atom:Button>
            </StackPanel>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

### 4.2 实际示例

**验证码控件**（`Captcha`）— 组合 LineEdit + Button：

```xml
<ControlTheme x:Key="{x:Type gallery:Captcha}" TargetType="gallery:Captcha">
    <Setter Property="Template">
        <ControlTemplate>
            <StackPanel Orientation="Horizontal" Spacing="10">
                <atom:LineEdit SizeType="{TemplateBinding SizeType}"
                               IsMotionEnabled="{TemplateBinding IsMotionEnabled}"
                               Status="{TemplateBinding Status}"
                               StyleVariant="{TemplateBinding StyleVariant}"
                               Width="200"
                               Text="{TemplateBinding Value, Mode=TwoWay}"/>
                <atom:Button SizeType="{TemplateBinding SizeType}"
                             IsMotionEnabled="{TemplateBinding IsMotionEnabled}">
                    Get captcha
                </atom:Button>
            </StackPanel>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

**价格输入控件**（`PriceInput`）— 组合 NumericUpDown + Select：

```xml
<ControlTheme x:Key="{x:Type gallery:PriceInput}" TargetType="gallery:PriceInput">
    <Setter Property="Template">
        <ControlTemplate>
            <StackPanel Orientation="Horizontal" Spacing="10">
                <atom:NumericUpDown SizeType="{TemplateBinding SizeType}"
                                    IsMotionEnabled="{TemplateBinding IsMotionEnabled}"
                                    Status="{TemplateBinding Status}"
                                    StyleVariant="{TemplateBinding StyleVariant}"
                                    Width="120"
                                    Value="0"
                                    Name="NumberInput"
                                    Minimum="0"/>
                <atom:Select SizeType="{TemplateBinding SizeType}"
                             IsMotionEnabled="{TemplateBinding IsMotionEnabled}"
                             Status="{TemplateBinding Status}"
                             StyleVariant="{TemplateBinding StyleVariant}"
                             DefaultValues="RMB"
                             Name="UnitInput">
                    <atom:SelectOption Header="RMB" Content="RMB"/>
                    <atom:SelectOption Header="Dollar" Content="Dollar"/>
                </atom:Select>
            </StackPanel>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> **要点**：每个内部的 AtomUI 输入控件都绑定了 `SizeType`、`IsMotionEnabled`、`Status`、`StyleVariant` 四个属性。这样当 Form 级联传播这些属性到外层自定义控件时，内部子控件会自动跟随变化。

---

## 5. 在 AXAML 中独立使用

### 5.1 设置样式变体

```xml
<!-- 四种变体 -->
<atom:LineEdit PlaceholderText="Outlined" StyleVariant="Outline" />
<atom:LineEdit PlaceholderText="Filled"   StyleVariant="Filled" />
<atom:LineEdit PlaceholderText="Borderless" StyleVariant="Borderless" />
<atom:LineEdit PlaceholderText="Underlined" StyleVariant="Underlined" />
```

### 5.2 设置验证状态

```xml
<!-- 手动设置 Status（不在 Form 中时） -->
<atom:LineEdit PlaceholderText="Error"   Status="Error" />
<atom:LineEdit PlaceholderText="Warning" Status="Warning" />
```

### 5.3 组合使用

```xml
<!-- StyleVariant + Status 自由组合 -->
<atom:LineEdit PlaceholderText="Filled + Error"
               StyleVariant="Filled"
               Status="Error" />
<atom:LineEdit PlaceholderText="Underlined + Warning"
               StyleVariant="Underlined"
               Status="Warning" />
```

### 5.4 在 Form 中自动级联

```xml
<!-- Form 级别设置 StyleVariant，所有子控件自动跟随 -->
<atom:Form StyleVariant="Filled" SizeType="Large">
    <atom:FormItem LabelText="Username" FieldName="username">
        <!-- 此 LineEdit 自动采用 Filled 变体 + Large 尺寸 -->
        <atom:LineEdit PlaceholderText="Enter username" />
    </atom:FormItem>
    <atom:FormItem LabelText="Email" FieldName="email">
        <!-- 此 LineEdit 也自动采用 Filled 变体 + Large 尺寸 -->
        <atom:LineEdit PlaceholderText="Enter email" />
    </atom:FormItem>
</atom:Form>
```

---

## 6. NotifyValidateStatus 映射规则

当控件接入 Form 时，`NotifyValidateStatus` 是将 Form 验证结果映射到控件视觉反馈的关键方法。标准映射规则如下：

```csharp
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
```

| FormValidateStatus | InputControlStatus | 说明 |
|---|---|---|
| `Error` | `Error` | 验证失败，显示红色反馈 |
| `Warning` | `Warning` | 验证警告，显示黄色反馈 |
| `Success` | `Default` | 验证通过，恢复正常样式 |
| `Validating` | `Default` | 验证中，保持正常样式 |
| `Default` | `Default` | 初始状态 |

> **注意**：使用 `SetCurrentValue` 而非 `SetValue`，以避免覆盖用户的外部绑定。

---

## 7. 实现检查清单

### StyleVariant 接入

- [ ] 实现 `IInputControlStyleVariantAware` 接口
- [ ] 使用 `InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<T>()` 注册属性（**不要** 自行 `Register`）
- [ ] 模板中的内部 AtomUI 输入控件通过 `{TemplateBinding StyleVariant}` 绑定
- [ ] 默认值为 `Outline`（由共享属性自动提供）

### Status 接入

- [ ] 实现 `IInputControlStatusAware` 接口
- [ ] 使用 `InputControlStatusProperty.StatusProperty.AddOwner<T>()` 注册属性（**不要** 自行 `Register`）
- [ ] 模板中的内部 AtomUI 输入控件通过 `{TemplateBinding Status}` 绑定
- [ ] 如果实现了 `IFormItemAware`，在 `NotifyValidateStatus` 中映射 `FormValidateStatus` → `InputControlStatus`
- [ ] 默认值为 `Default`（由共享属性自动提供）

### Form 集成

- [ ] 同时实现 `IFormItemAware`（参见 [Form 集成指南](./FormIntegrationGuide.md)）
- [ ] 同时实现 `ISizeTypeAware`（使 Form 的 SizeType 能级联）
- [ ] 同时实现 `IMotionAwareControl`（使 Form 的动画开关能级联）
- [ ] 所有 `IFormItemAware` 成员使用显式接口实现

### 常见错误

| 错误 | 后果 | 解决方案 |
|---|---|---|
| 使用 `Register` 而非 `AddOwner` | Form 级联绑定静默失败 | 使用 `AddOwner<T>()` |
| 模板中遗漏 `{TemplateBinding StyleVariant}` | 内部控件不跟随变体变化 | 为所有内部输入控件添加绑定 |
| 模板中遗漏 `{TemplateBinding Status}` | 内部控件不显示验证状态 | 为所有内部输入控件添加绑定 |
| `NotifyValidateStatus` 中使用 `SetValue` | 可能覆盖外部绑定 | 使用 `SetCurrentValue` |
| 未实现 `IInputControlStyleVariantAware` 但 Form 中使用 | Form 不会级联 StyleVariant 到该控件 | 实现接口 |

