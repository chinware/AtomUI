# AtomUI 输入控件样式变体与验证状态设计原理

## 1. 概述

AtomUI 的输入控件提供两个正交的外观维度：**样式变体**（StyleVariant）控制控件的视觉风格，**验证状态**（Status）反馈数据校验结果。两者可自由组合，通过 Design Token 系统驱动全部视觉表现，并能在 Form 中自动级联传播。

这一设计对应 Ant Design 5.0 中 Input 组件的 [`variant`](https://ant.design/components/input#api) 和 [`status`](https://ant.design/components/input#api) 属性，AtomUI 将其抽象为两组独立的接口 + 共享属性 + Token 映射体系，确保所有输入类控件（包括自定义控件）能够表现一致。

### 核心设计理念

1. **正交性**：StyleVariant 和 Status 是两个独立维度，4 种变体 × 3 种状态 = 12 种组合，每种组合在主题中有完整的视觉定义。
2. **接口驱动**：通过 `IInputControlStyleVariantAware` 和 `IInputControlStatusAware` 接口标记控件支持相应能力，Form 据此实现自动级联。
3. **共享属性模式**：所有控件使用 `AddOwner<T>()` 注册同名 `StyledProperty`，确保属性名一致，级联绑定正确生效。
4. **Token 驱动视觉**：每种 Variant × Status × State（hover / focus / pressed / disabled）组合的颜色均由 Design Token 定义，无硬编码值。
5. **Form 自动级联**：Form → FormItem → 内容控件的 StyleVariant 自动向下传播；验证管线自动将 `FormValidateStatus` 映射为 `InputControlStatus`。

---

## 2. 类型定义

### 2.1 InputControlStyleVariant 枚举

定义在 `AtomUI.Controls.Shared/InputControlStyleVariant.cs`：

```csharp
public enum InputControlStyleVariant
{
    Outline,      // 带边框（默认）
    Filled,       // 填充背景
    Borderless,   // 无边框
    Underlined    // 仅下划线
}
```

| 值 | 对应 Ant Design | 视觉特征 |
|---|---|---|
| `Outline` | `outlined`（默认） | 完整边框 + 透明背景，hover 时边框变主色，focus 时边框高亮 + 阴影 |
| `Filled` | `filled` | 灰色填充背景 + 同色边框，hover 时背景加深，focus 时切换为透明背景 + 主色边框 |
| `Borderless` | `borderless` | 完全无边框无背景，最小化视觉干扰 |
| `Underlined` | — | 仅底部下划线（AtomUI 扩展），适用于极简风格表单 |

### 2.2 InputControlStatus 枚举

定义在 `AtomUI.Controls.Shared/InputControlStatus.cs`：

```csharp
public enum InputControlStatus
{
    Default,    // 正常状态
    Warning,    // 警告（黄色系）
    Error       // 错误（红色系）
}
```

| 值 | 对应 Ant Design | 视觉特征 |
|---|---|---|
| `Default` | 无 status 属性 | 正常边框/背景颜色 |
| `Warning` | `status="warning"` | 边框/背景切换为 Warning 色系（`ColorWarning` 系列 Token） |
| `Error` | `status="error"` | 边框/背景切换为 Error 色系（`ColorError` 系列 Token） |

### 2.3 两者的正交组合

StyleVariant × Status 构成一个 4×3 的视觉矩阵，每种组合都有完整的交互状态定义：

```
                    Default         Warning          Error
              ┌────────────────┬────────────────┬────────────────┐
  Outline     │ 灰边框         │ 黄边框         │ 红边框         │
              │ hover→主色     │ hover→黄hover  │ hover→红hover  │
              │ focus→主色+影  │ focus→黄+黄影  │ focus→红+红影  │
              ├────────────────┼────────────────┼────────────────┤
  Filled      │ 灰底           │ 黄底           │ 红底           │
              │ hover→深灰     │ hover→深黄     │ hover→深红     │
              │ focus→白底+主框│ focus→白底+黄框│ focus→白底+红框│
              ├────────────────┼────────────────┼────────────────┤
  Borderless  │ 透明           │ 透明           │ 透明           │
              │ （无边框变化） │ （无边框变化） │ （无边框变化） │
              ├────────────────┼────────────────┼────────────────┤
  Underlined  │ 灰下划线       │ 黄下划线       │ 红下划线       │
              │ hover→主色     │ hover→黄hover  │ hover→红hover  │
              │ focus→主色     │ focus→黄色     │ focus→红色     │
              └────────────────┴────────────────┴────────────────┘
```

---

## 3. 接口与共享属性

### 3.1 IInputControlStyleVariantAware

```csharp
public interface IInputControlStyleVariantAware
{
    InputControlStyleVariant StyleVariant { get; set; }
}
```

标记控件支持**样式变体切换**。Form 系统通过 `is IInputControlStyleVariantAware` 类型检查来决定是否向子控件级联 `StyleVariant`。

### 3.2 InputControlStyleVariantProperty（共享属性持有者）

```csharp
public abstract class InputControlStyleVariantProperty
{
    public const string StyleVariantPropertyName = "StyleVariant";
    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        AvaloniaProperty.Register<StyledElement, InputControlStyleVariant>(
            StyleVariantPropertyName, InputControlStyleVariant.Outline);
}
```

所有控件 **必须** 使用 `AddOwner<T>()` 注册此属性，**禁止** 自行 `Register` 同名属性：

```csharp
// ✅ 正确
public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
    InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<MyControl>();

// ❌ 错误：自行 Register 会导致属性名相同但实例不同，级联绑定失败
public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
    AvaloniaProperty.Register<MyControl, InputControlStyleVariant>("StyleVariant");
```

### 3.3 IInputControlStatusAware

```csharp
public interface IInputControlStatusAware
{
    InputControlStatus Status { get; set; }
}
```

标记控件支持**验证状态反馈**。当 Form 验证完成后，通过 `IFormItemAware.NotifyValidateStatus()` 通知控件更新状态。

### 3.4 InputControlStatusProperty（共享属性持有者）

```csharp
public abstract class InputControlStatusProperty
{
    public const string StatusPropertyName = "Status";
    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        AvaloniaProperty.Register<StyledElement, InputControlStatus>(
            StatusPropertyName, InputControlStatus.Default);
}
```

同理，**必须** 使用 `AddOwner<T>()`：

```csharp
public static readonly StyledProperty<InputControlStatus> StatusProperty =
    InputControlStatusProperty.StatusProperty.AddOwner<MyControl>();
```

### 3.5 为什么必须使用 AddOwner？

Avalonia 的属性级联绑定（`BindUtils.RelayBind`）要求父控件和子控件的 `StyledProperty` 实例来自同一个 `Register` 调用链。如果子控件自行 `Register` 了同名属性，虽然属性名相同，但 Avalonia 内部的属性标识符不同，导致：

- Form → FormItem → 子控件 的 `RelayBind` 绑定静默失败
- 在 CompactSpace 中属性传播失效
- 在 AXAML 中使用 `{TemplateBinding StyleVariant}` 时无法正确匹配

---

## 4. Token 集成

样式变体和验证状态的视觉表现完全由 Design Token 驱动。以 `AddOnDecoratedBox`（所有输入控件的通用装饰框）为例：

### 4.1 组件 Token 定义

```csharp
[ControlDesignToken]
internal class AddOnDecoratedBoxToken : AbstractControlDesignToken
{
    /// <summary>
    /// 悬浮态边框色
    /// </summary>
    public Color HoverBorderColor { get; set; }

    /// <summary>
    /// 激活态边框色
    /// </summary>
    public Color ActiveBorderColor { get; set; }

    /// <summary>
    /// 激活态阴影
    /// </summary>
    public BoxShadow ActiveShadow { get; set; }

    /// <summary>
    /// 错误状态时激活态阴影
    /// </summary>
    public BoxShadow ErrorActiveShadow { get; set; }

    /// <summary>
    /// 警告状态时激活态阴影
    /// </summary>
    public BoxShadow WarningActiveShadow { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        ActiveBorderColor = SharedToken.ColorPrimary;
        HoverBorderColor  = SharedToken.ColorPrimaryHover;
        ActiveShadow = new BoxShadow
        {
            Spread = SharedToken.ControlOutlineWidth,
            Color  = SharedToken.ColorControlOutline
        };
        ErrorActiveShadow = new BoxShadow
        {
            Spread = SharedToken.ControlOutlineWidth,
            Color  = SharedToken.ColorErrorOutline       // ← Error 色系
        };
        WarningActiveShadow = new BoxShadow
        {
            Spread = SharedToken.ControlOutlineWidth,
            Color  = SharedToken.ColorWarningOutline     // ← Warning 色系
        };
    }
}
```

### 4.2 全局共享 Token 映射表

各种 Status × State 组合使用的 SharedToken：

| 场景 | Token | 说明 |
|---|---|---|
| Outline 默认边框 | `ColorBorder` | 灰色边框 |
| Outline hover | `HoverBorderColor`（组件 Token，= `ColorPrimaryHover`） | 主色悬浮 |
| Outline focus/pressed | `ActiveBorderColor`（组件 Token，= `ColorPrimary`） | 主色激活 |
| Error 边框 | `ColorError` | 红色 |
| Error hover | `ColorErrorBorderHover` | 红色悬浮 |
| Error focus 阴影 | `ErrorActiveShadow`（= `ColorErrorOutline`） | 红色光晕 |
| Warning 边框 | `ColorWarning` | 黄色 |
| Warning hover | `ColorWarningBorderHover` | 黄色悬浮 |
| Warning focus 阴影 | `WarningActiveShadow`（= `ColorWarningOutline`） | 黄色光晕 |
| Filled 默认背景 | `ColorFillTertiary` | 浅灰填充 |
| Filled hover 背景 | `ColorFillSecondary` | 中灰填充 |
| Filled + Error 背景 | `ColorErrorBg` | 浅红填充 |
| Filled + Warning 背景 | `ColorWarningBg` | 浅黄填充 |
| Disabled 背景 | `ColorBgContainerDisabled` | 禁用灰色 |

---

## 5. 主题选择器模式

### 5.1 AXAML 中 StyleVariant 选择器

AtomUI 同时使用两种选择器策略：

**属性选择器**（用于主题 AXAML）：

```xml
<!-- Outline 变体 -->
<Style Selector="^[StyleVariant=Outline]">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />
</Style>

<!-- Filled 变体 -->
<Style Selector="^[StyleVariant=Filled]">
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorFillTertiary}" />
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorFillTertiary}" />
</Style>

<!-- Borderless 变体 -->
<Style Selector="^[StyleVariant=Borderless]">
    <!-- 无边框、无背景 -->
</Style>

<!-- Underlined 变体 -->
<Style Selector="^[StyleVariant=Underlined]">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />
</Style>
```

### 5.2 Status 选择器（嵌套在 StyleVariant 内）

Status 选择器 **嵌套** 在 StyleVariant 选择器内部，因为不同变体下 Status 的视觉表达方式不同：

```xml
<Style Selector="^[StyleVariant=Outline]">
    <!-- Outline 默认样式 -->
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />

    <!-- Outline + Error -->
    <Style Selector="^[Status=Error]">
        <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorError}" />
        <Style Selector="^[IsInnerBoxHover=True]">
            <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorErrorBorderHover}" />
        </Style>
        <Style Selector="^:focus-within">
            <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorError}" />
        </Style>
    </Style>

    <!-- Outline + Warning -->
    <Style Selector="^[Status=Warning]">
        <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorWarning}" />
        <Style Selector="^[IsInnerBoxHover=True]">
            <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorWarningBorderHover}" />
        </Style>
        <Style Selector="^:focus-within">
            <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorWarning}" />
        </Style>
    </Style>
</Style>
```

### 5.3 伪类策略

部分控件同时使用属性选择器和伪类来处理 StyleVariant/Status 变化：

```csharp
// AddOnDecoratedBox 中的伪类更新
protected virtual void UpdatePseudoClasses()
{
    PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline,    StyleVariant == InputControlStyleVariant.Outline);
    PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled,     StyleVariant == InputControlStyleVariant.Filled);
    PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == InputControlStyleVariant.Borderless);
}

// LineEdit 中的 Status 伪类更新
PseudoClasses.Set(StdPseudoClass.Error,   Status == InputControlStatus.Error);
PseudoClasses.Set(StdPseudoClass.Warning, Status == InputControlStatus.Warning);
```

> **设计选择**：主题 AXAML 中优先使用 `[StyleVariant=Outline]`、`[Status=Error]` 等属性选择器（语义更清晰）；伪类（`:outline`、`:error`）主要用于控件内部需要向下游子控件传递状态的场景。

---

## 6. Form 级联传播

### 6.1 StyleVariant 级联链

Form 中的 StyleVariant 通过 `BindUtils.RelayBind` 自动向下传播：

```
Form ─── StyleVariant ──→ FormItem ─── StyleVariant ──→ Content (子控件)
                                    └── StyleVariant ──→ FormItemDecorator ──→ Child
```

**FormItem 中的级联代码**：

```csharp
// FormItem.cs — 当 Content 变化时设置级联绑定
if (Content is IInputControlStyleVariantAware)
{
    _disposables.Add(BindUtils.RelayBind(this, StyleVariantProperty, Content, StyleVariantProperty));
}
```

**FormItemDecorator 中的级联代码**：

```csharp
// FormItemDecorator.cs — 当 Child 变化时设置级联绑定
if (newChild is IInputControlStyleVariantAware)
{
    _disposables.Add(BindUtils.RelayBind(this, StyleVariantProperty, newChild, StyleVariantProperty));
}
```

这意味着开发者只需在 `<atom:Form>` 上设置一次 `StyleVariant`，所有子输入控件会自动采用相同的样式变体。

### 6.2 Status 从验证管线自动设置

Status 不通过级联绑定传播，而是由 Form 验证管线通过 `IFormItemAware.NotifyValidateStatus()` 设置：

```
Form 验证管线
    ↓ ValidateValueAsync()
FormItem
    ↓ NotifyValidateStatus(FormValidateStatus.Error)
IFormItemAware.NotifyValidateStatus()
    ↓ 控件内部映射
SetCurrentValue(StatusProperty, InputControlStatus.Error)
```

**控件中的映射代码**（以 LineEdit 为例）：

```csharp
protected override void NotifyValidateStatus(FormValidateStatus status)
{
    if (status == FormValidateStatus.Error)
    {
        SetCurrentValue(StatusProperty, InputControlStatus.Error);
    }
    else if (status == FormValidateStatus.Warning)
    {
        SetCurrentValue(StatusProperty, InputControlStatus.Warning);
    }
    else
    {
        SetCurrentValue(StatusProperty, InputControlStatus.Default);
    }
}
```

### 6.3 级联传播的设计优势

| 特性 | 说明 |
|---|---|
| **单点配置** | 在 Form 级别设置 `StyleVariant="Filled"`，所有子控件统一切换 |
| **逐级覆盖** | 单个控件可覆盖 Form 级别的 StyleVariant |
| **类型安全** | 只有实现了 `IInputControlStyleVariantAware` 的控件才会参与级联 |
| **自动清理** | 当控件从 Form 移除时，通过 `CompositeDisposable` 自动解除绑定 |
| **验证自动化** | Status 由验证管线自动设置，开发者无需手动管理 |

---

## 7. 已实现控件清单

### 7.1 同时实现 StyleVariant + Status 的控件

这些控件同时支持样式变体切换和验证状态反馈，是完整的输入控件：

| 控件 | 项目 | 说明 |
|---|---|---|
| `LineEdit` | Desktop.Controls | 单行文本输入 |
| `TextArea` | Desktop.Controls | 多行文本输入 |
| `SearchButton` | Desktop.Controls | 搜索按钮（SearchEdit 内部） |
| `ButtonSpinner` | Desktop.Controls | 带按钮的微调器 |
| `NumericUpDown` | Desktop.Controls | 数值输入 |
| `ComboBox` | Desktop.Controls | 下拉选择 |
| `AbstractSelect` | Desktop.Controls | Select 基类 |
| `Mentions` | Desktop.Controls | @提及输入 |
| `AbstractAutoComplete` | Desktop.Controls | AutoComplete 基类 |
| `InfoPickerInput` | Desktop.Controls | 日期/时间选择器输入框 |
| `AddOnDecoratedBox` | Desktop.Controls | 输入装饰框（内部组件） |
| `CompactSpaceAddOn` | Desktop.Controls | 紧凑空间装饰（内部组件） |
| `FormItemDecorator` | Desktop.Controls | 表单项装饰器 |
| `AbstractColorPicker` | Desktop.Controls.ColorPicker | 颜色选择器 |

### 7.2 仅实现 Status 的控件

| 控件 | 项目 | 说明 |
|---|---|---|
| `AbstractTransfer` | Desktop.Controls | 穿梭框基类 |
| `TransferItemDecorator` | Desktop.Controls | 穿梭框项装饰器 |

### 7.3 仅实现 StyleVariant 的控件

| 控件 | 项目 | 说明 |
|---|---|---|
| `Form` | Desktop.Controls | 表单容器（仅负责向下级联） |

---

## 8. 设计优势分析

### 8.1 与 Ant Design 的对齐

Ant Design 5.0 引入了 `variant` 属性（替代旧版的 `bordered` 布尔值），AtomUI 将其映射为类型安全的枚举，并增加了 `Underlined` 变体以适应更多场景。`status` 属性与 Ant Design 完全一致。

### 8.2 与尺寸系统的解耦

StyleVariant 和 SizeType 是完全独立的维度：

- `SizeType` 控制高度、内边距、字号
- `StyleVariant` 控制边框、背景、视觉风格
- `Status` 控制色彩反馈

三者可自由组合，例如 `SizeType="Small" StyleVariant="Filled" Status="Error"` 产生一个小号填充式红色错误输入框。

### 8.3 统一的视觉语义

所有输入控件共享同一套颜色语义：

- Error 永远使用 `ColorError` 系列 Token
- Warning 永远使用 `ColorWarning` 系列 Token
- 默认状态永远使用 `ColorPrimary` 系列 Token 作为交互色

这保证了不同控件在相同 Status 下视觉表现一致。

### 8.4 Icon 自动染色

`AddOnDecoratedBox` 在 Status 变化时自动将内部 Icon 的颜色切换为对应的状态色：

```csharp
// Warning 状态下自动将前后缀图标染为黄色
var warningStyle = new Style(x =>
    x.PropertyEquals(StatusProperty, InputControlStatus.Warning));
iconStyle.Add(Icon.FillBrushProperty, SharedTokenKind.ColorWarning);
iconStyle.Add(Icon.ForegroundProperty, SharedTokenKind.ColorWarning);
```

开发者可通过 `skip-status` CSS 类排除特定图标不参与自动染色。

---

## 9. 设计限制与注意事项

### 9.1 Borderless 变体下 Status 视觉有限

Borderless 变体没有边框和背景，因此 Error/Warning 状态的视觉反馈仅体现在：
- 内部 Icon 的颜色变化
- 如果结合 Form 使用，由 FormItem 显示验证消息

### 9.2 Status 不影响 Disabled 样式

当控件处于 `:disabled` 状态时，所有 Status 的视觉反馈被覆盖为统一的禁用样式（灰色背景、灰色边框、灰色文字），Status 设置仍保留但不可见。

### 9.3 Status 与 Form 的关系

- **独立使用**：可以在 Form 外部直接设置 `Status="Error"` 手动显示错误状态。
- **Form 内使用**：验证管线自动管理 Status 值，开发者通常不需要手动设置。
- **两者冲突**：如果在 Form 内手动设置了 Status，下次验证触发时会被覆盖。

