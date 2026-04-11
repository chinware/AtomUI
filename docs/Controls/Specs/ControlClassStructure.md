# 控件类结构规范

> 本文档规定 AtomUI 控件的类继承模型、接口实现、构造函数、命名空间与代码组织约定。

---

## 1. 两层继承模型

AtomUI 采用 **Base 层 + Platform 层** 的两层控件继承模型，最大化跨平台代码复用：

```
AtomUI.Controls (Base 层, 设备无关)            AtomUI.Desktop.Controls (Platform 层)
──────────────────────────────────────         ──────────────────────────────────────
AbstractTag      (行为、属性、逻辑)    ─────►  Tag : AbstractTag      (+ Token 注册)
AbstractAvatar   (行为、属性、逻辑)    ─────►  Avatar : AbstractAvatar (+ Token 注册)
AbstractIconButton (行为、属性)        ─────►  IconButton : AbstractIconButton (+ Token 注册)
```

### 1.1 Base 层控件（`AtomUI.Controls`）

**职责**：定义设备无关的共享行为、属性、伪类和逻辑。

**规则：**

| 规则 | 说明 |
|---|---|
| 类名前缀 `Abstract` | 如 `AbstractTag`、`AbstractAvatar` |
| 通常为 `public abstract class` | 不可直接实例化 |
| 继承 Avalonia 基类 | 通常为 `TemplatedControl`、`ContentControl` 等 |
| **不定义 Token** | Token 是平台特定的，属于 Platform 层 |
| **不定义 Theme** | Theme 是平台特定的，属于 Platform 层 |
| **不注册 Token 作用域** | 由 Platform 层的具体控件注册 |
| 可定义模板部件获取 | 在 `OnApplyTemplate` 中 |
| 可定义事件 | `RoutedEvent` 定义在需要的层级 |

**示例：**

```csharp
// AtomUI.Controls/Tag/AbstractTag.cs
namespace AtomUI.Controls.Commons;

[TemplatePart("PART_CloseButton", typeof(AbstractIconButton))]
public abstract class AbstractTag : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> TagColorProperty =
        AvaloniaProperty.Register<AbstractTag, string?>(nameof(Color));

    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<AbstractTag, bool>(nameof(IsClosable));

    public string? TagColor
    {
        get => GetValue(TagColorProperty);
        set => SetValue(TagColorProperty, value);
    }

    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent =
        RoutedEvent.Register<AbstractTag, RoutedEventArgs>(nameof(Closed), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }

    #endregion

    // ...内部属性、逻辑
}
```

### 1.2 Platform 层控件（`AtomUI.Desktop.Controls`）

**职责**：继承 Base 层的抽象基类，注册 Token 作用域，提供平台特定的行为。

**规则：**

| 规则 | 说明 |
|---|---|
| 必须继承对应的抽象基类 | `Tag : AbstractTag` |
| 构造函数注册 Token 作用域 | `this.RegisterTokenResourceScope(XxxToken.ScopeProvider)` |
| 可添加平台特定属性 | 桌面特有的交互属性 |
| 命名不带 `Abstract` 前缀 | `Tag`、`Avatar`、`Button` |

**示例（最简形式）：**

```csharp
// AtomUI.Desktop.Controls/Tag/Tag.cs
namespace AtomUI.Desktop.Controls;

public class Tag : AbstractTag
{
    public Tag()
    {
        this.RegisterTokenResourceScope(TagToken.ScopeProvider);
    }
}
```

**示例（带平台特定逻辑）：**

```csharp
// AtomUI.Desktop.Controls/Buttons/Button.cs
namespace AtomUI.Desktop.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

[PseudoClasses(ButtonPseudoClass.IconOnly,
    ButtonPseudoClass.Loading,
    ButtonPseudoClass.IsDanger,
    ButtonPseudoClass.DefaultType,
    /* ... */)]
public class Button : AvaloniaButton,
                      ISizeTypeAware,
                      IWaveSpiritAwareControl,
                      ICompactSpaceAware,
                      IFormItemAware
{
    // ...属性、逻辑
    
    public Button()
    {
        this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
    }
}
```

### 1.3 不需要 Base 层的控件

如果控件没有跨平台复用的需求（纯桌面控件），可以直接在 Platform 层定义，**不需要** 创建抽象基类：

```csharp
// 直接在 AtomUI.Desktop.Controls 中定义，没有对应的 Abstract 基类
public class SomeDesktopOnlyControl : TemplatedControl
{
    public SomeDesktopOnlyControl()
    {
        this.RegisterTokenResourceScope(SomeDesktopOnlyControlToken.ScopeProvider);
    }
}
```

---

## 2. 控件别名（Aliasing）

当 AtomUI 控件与 Avalonia 内置控件同名时，**必须** 使用 `using` 别名避免命名冲突：

```csharp
using AvaloniaButton = Avalonia.Controls.Button;

public class Button : AvaloniaButton, ISizeTypeAware { ... }
```

```csharp
using AvaloniaToggleSwitch = Avalonia.Controls.ToggleSwitch;

public class ToggleSwitch : AvaloniaToggleSwitch, ISizeTypeAware { ... }
```

**命名规则**：别名格式为 `Avalonia{ClassName}`。

---

## 3. 接口实现

### 3.1 必须实现的接口

根据控件特性，**必须** 实现对应的共享接口：

| 控件特性 | 必须实现的接口 | 所在项目 |
|---|---|---|
| 支持 Small/Middle/Large 尺寸 | `ISizeTypeAware` | `AtomUI.Controls.Shared` |
| 输入控件带验证状态 | `IInputControlStatusAware` | `AtomUI.Controls.Shared` |
| 支持 Outlined/Filled/Borderless 变体 | `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` |
| 支持点击涟漪动画 | `IWaveSpiritAwareControl` | `AtomUI.Controls.Shared` |
| 支持动画开关 | `IMotionAwareControl` | `AtomUI.Core` |
| 参与表单验证 | `IFormItemAware` | `AtomUI.Controls` |
| 支持紧凑间距模式 | `ICompactSpaceAware` | `AtomUI.Desktop.Controls` |
| 响应式断点感知 | `IMediaBreakAware` | `AtomUI.Controls.Shared` |

### 3.2 接口与属性关系

实现接口时，对应的属性 **必须** 使用 `AddOwner<T>()` 从共享属性持有器复用：

```csharp
// ISizeTypeAware 要求：
public class Button : ..., ISizeTypeAware
{
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Button>();  // ← AddOwner

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
}
```

```csharp
// IWaveSpiritAwareControl 要求（继承自 IMotionAwareControl）：
public class Button : ..., IWaveSpiritAwareControl
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Button>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Button>();

    public bool IsMotionEnabled { get => ...; set => ...; }
    public bool IsWaveSpiritEnabled { get => ...; set => ...; }
}
```

### 3.3 IFormItemAware 的实现模式

`IFormItemAware` 接口的实现遵循固定模式，使用显式接口实现 + 虚方法：

```csharp
#region 实现 FormItem 接口

private EventHandler? _formValueChanged;
event EventHandler? IFormItemAware.ValueChanged
{
    add => _formValueChanged += value;
    remove => _formValueChanged -= value;
}

void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value);
object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);

protected virtual void NotifySetFormValue(object? value) { }
protected virtual object? NotifyGetFormValue() => null;
protected virtual void NotifyClearFormValue() { }
protected virtual void NotifyValidateStatus(FormValidateStatus status) { }

#endregion
```

### 3.4 ICompactSpaceAware 的实现模式

```csharp
void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
{
    IsUsedInCompactSpace     = position != null;
    CompactSpaceItemPosition = position;
}

void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
{
    CompactSpaceOrientation = orientation;
}

bool ICompactSpaceAware.IsAlwaysActiveZIndex()
{
    return ButtonType == ButtonType.Primary;
}

double ICompactSpaceAware.GetBorderThickness() => GetBorderThicknessForCompactSpace();
```

---

## 4. 命名空间约定

| 项目 | 命名空间 | 用途 |
|---|---|---|
| `AtomUI.Controls` | `AtomUI.Controls` | 基类控件的主命名空间 |
| `AtomUI.Controls` | `AtomUI.Controls.Commons` | 通用抽象基类（如 `AbstractTag`） |
| `AtomUI.Desktop.Controls` | `AtomUI.Desktop.Controls` | 桌面具体控件 |
| `AtomUI.Desktop.Controls` | `AtomUI.Desktop.Controls.Themes` | Theme 代码后台 |
| `AtomUI.Controls.Shared` | `AtomUI.Controls` | 共享接口、枚举、属性持有器 |
| `AtomUI.Core` | `AtomUI` 及子命名空间 | 核心基础设施 |

**文件作用域命名空间**：所有 C# 文件 **必须** 使用文件作用域命名空间：

```csharp
// ✅ 正确
namespace AtomUI.Desktop.Controls;

public class Button : AvaloniaButton { ... }

// ❌ 错误：不使用块作用域命名空间
namespace AtomUI.Desktop.Controls
{
    public class Button : AvaloniaButton { ... }
}
```

---

## 5. 构造函数

### 5.1 静态构造函数

用于以下目的：

1. **声明 `AffectsMeasure` / `AffectsRender`**
2. **覆盖默认值** `OverrideDefaultValue`
3. **注册类级别事件处理器** `AddClassHandler`

```csharp
static Button()
{
    AffectsMeasure<Button>(SizeTypeProperty, ShapeProperty, IconProperty);
    AffectsRender<Button>(ButtonTypeProperty, IsDangerProperty, IsGhostProperty);
    FocusableProperty.OverrideDefaultValue(typeof(Button), true);
}
```

### 5.2 实例构造函数

Platform 层控件的构造函数 **必须** 注册 Token 作用域：

```csharp
public Button()
{
    this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
}
```

> Base 层的抽象控件 **不注册** Token 作用域。

---

## 6. 访问修饰符约定

| 成员类型 | 修饰符 | 说明 |
|---|---|---|
| 控件类 | `public` | 用户直接使用 |
| 公共 StyledProperty 字段 | `public static readonly` | 可被外部代码和 AXAML 引用 |
| 内部 StyledProperty 字段 | `internal static readonly` | 仅 Theme 和内部逻辑使用 |
| 公共 CLR 属性 | `public` | 用户 API |
| 内部 CLR 属性 | `internal` | Theme 绑定和内部逻辑 |
| Token 类 | `internal` | Token 是实现细节 |
| Theme 代码后台 | 视需求 | `ButtonTheme` 为 `public`（因 AXAML 引用），`TagTheme` 为 `internal` |
| PseudoClass 常量类 | `public static` | 可被外部用于样式选择器 |

---

## 7. 枚举定义

### 7.1 控件专属枚举

控件专属的枚举 **可以** 定义在以下位置：

- **Base 层**（`AtomUI.Controls/ControlName/ControlNameEnums.cs`）：当枚举是跨平台共享的。
- **Platform 层**（`AtomUI.Desktop.Controls/ControlName/ControlName.cs` 文件顶部）：当枚举仅用于桌面控件。

```csharp
// 可以定义在控件文件中（Platform 层特有枚举）
namespace AtomUI.Desktop.Controls;

public enum ButtonType
{
    Default,
    Dashed,
    Primary,
    Link,
    Text
}

public enum ButtonShape
{
    Default,
    Circle,
    Round
}

public class Button : AvaloniaButton { ... }
```

```csharp
// 也可以独立文件（Base 层共享枚举）
// AtomUI.Controls/Tag/TagEnums.cs
namespace AtomUI.Controls;

public enum TagStatus
{
    Success,
    Info,
    Warning,
    Error
}
```

### 7.2 共享枚举

在 `AtomUI.Controls.Shared` 中定义的枚举：

| 枚举 | 用途 |
|---|---|
| `SizeType` | Small / Middle / Large |
| `CustomizableSizeType` | 可自定义的尺寸类型 |
| `InputControlStatus` | Default / Success / Warning / Error |
| `InputControlStyleVariant` | Outline / Filled / Borderless |
| `FormValidateStatus` | 表单验证状态 |
| `WaveSpiritType` | RoundRectWave / CircleWave / PillWave |

使用共享枚举时 **禁止** 重新定义同义枚举。

---

## 8. `[TemplatePart]` 特性

当控件在 `OnApplyTemplate` 中获取模板部件时，**应该** 在类上声明 `[TemplatePart]` 特性：

```csharp
[TemplatePart("PART_CloseButton", typeof(AbstractIconButton))]
public abstract class AbstractTag : TemplatedControl
{
    protected AbstractIconButton? CloseButton;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        CloseButton = e.NameScope.Find<AbstractIconButton>("PART_CloseButton");
    }
}
```

模板部件名称遵循 `PART_` 前缀约定（参见 [ThemeFileStructure](./ThemeFileStructure.md)）。

---

## 9. 完整控件类模板

以下是一个符合规范的控件类完整模板：

```csharp
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

using AvaloniaBaseType = Avalonia.Controls.SomeBaseType;  // 仅在同名时需要别名

/// <summary>
/// AtomUI 的 Xxx 控件，遵循 Ant Design 5.0 规范。
/// </summary>
[PseudoClasses(XxxPseudoClass.SomeState, XxxPseudoClass.AnotherState)]
public class Xxx : AbstractXxx,  // 或直接继承 TemplatedControl / AvaloniaBaseType
                   ISizeTypeAware,
                   IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Xxx>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Xxx>();

    public static readonly StyledProperty<XxxVariant> VariantProperty =
        AvaloniaProperty.Register<Xxx, XxxVariant>(nameof(Variant));

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

    /// <summary>
    /// 获取或设置控件变体。
    /// </summary>
    public XxxVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<Xxx, bool> IsComputedStateProperty =
        AvaloniaProperty.RegisterDirect<Xxx, bool>(
            nameof(IsComputedState),
            o => o.IsComputedState,
            (o, v) => o.IsComputedState = v);

    private bool _isComputedState;

    internal bool IsComputedState
    {
        get => _isComputedState;
        set => SetAndRaise(IsComputedStateProperty, ref _isComputedState, value);
    }

    #endregion

    static Xxx()
    {
        AffectsMeasure<Xxx>(SizeTypeProperty, VariantProperty);
        AffectsRender<Xxx>(IsComputedStateProperty);
    }

    public Xxx()
    {
        this.RegisterTokenResourceScope(XxxToken.ScopeProvider);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        // ...
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(XxxPseudoClass.SomeState, /* condition */);
    }
}
```

