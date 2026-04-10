# 属性定义规范

> 本文档规定 AtomUI 控件中所有 Avalonia 属性的定义方式、选型规则、命名约定与 CLR 包装器规范。

---

## 1. Avalonia 属性类型选型

Avalonia 提供三种属性类型，在 AtomUI 中各有明确的使用场景：

| 属性类型 | 类 | 使用场景 | 参与样式 | 参与动画 | 支持继承 | 可只读 |
|---|---|---|---|---|---|---|
| **StyledProperty** | `StyledProperty<T>` | 需要被样式、动画、值优先级系统消费的属性 | ✅ | ✅ | ✅ | ❌ |
| **DirectProperty** | `DirectProperty<TOwner, TValue>` | 只读属性、性能敏感属性、不需要参与样式的内部计算属性 | ❌ | ❌ | ❌ | ✅ |
| **AttachedProperty** | `AttachedProperty<T>` | 需要附加到其他控件上的属性（如布局面板设置子项属性） | ✅ | ✅ | ✅ | ❌ |

### 1.1 选型决策树

```
需要定义一个属性？
├─ 属性是否需要被样式/动画消费？
│  ├─ 是 → 该属性是否已在其他控件上定义？
│  │  ├─ 是 → 使用 AddOwner<T>() 复用
│  │  └─ 否 → 注册新的 StyledProperty
│  └─ 否 → 属性是否需要只读？
│     ├─ 是 → 使用 DirectProperty（省略 setter）
│     └─ 否 → 属性是否是性能敏感的高频读写？
│        ├─ 是 → 使用 DirectProperty
│        └─ 否 → 使用 StyledProperty
├─ 属性是否需要附加到其他控件？
│  └─ 是 → 使用 AttachedProperty
```

### 1.2 AtomUI 中各类型的典型用法

**StyledProperty —— 绝大多数控件属性**

```csharp
// ✅ 正确：公共属性，用户可在 AXAML 中设置、样式中覆盖
public static readonly StyledProperty<ButtonType> ButtonTypeProperty =
    AvaloniaProperty.Register<Button, ButtonType>(nameof(ButtonType));

// ✅ 正确：内部属性，仅在 Theme 中由样式系统消费
internal static readonly StyledProperty<Thickness> EffectiveBorderThicknessProperty =
    AvaloniaProperty.Register<Button, Thickness>(nameof(EffectiveBorderThickness));
```

**DirectProperty —— 只读或计算属性**

```csharp
// ✅ 正确：内部计算属性（只读语义）
internal static readonly DirectProperty<Button, CornerRadius> EffectiveCornerRadiusProperty =
    AvaloniaProperty.RegisterDirect<Button, CornerRadius>(
        nameof(EffectiveCornerRadius),
        o => o.EffectiveCornerRadius,
        (o, v) => o.EffectiveCornerRadius = v);

// ✅ 正确：Avalonia 内置的只读 DirectProperty
public static readonly DirectProperty<Button, bool> IsPressedProperty =
    AvaloniaProperty.RegisterDirect<Button, bool>(nameof(IsPressed), b => b.IsPressed);
```

**AttachedProperty —— 面板对子项的附加属性**

```csharp
// ✅ 正确：布局面板为子控件定义附加属性
public static readonly AttachedProperty<int> ColumnProperty =
    AvaloniaProperty.RegisterAttached<MyPanel, Control, int>("Column", defaultValue: 0);

public static int GetColumn(Control element) => element.GetValue(ColumnProperty);
public static void SetColumn(Control element, int value) => element.SetValue(ColumnProperty, value);
```

---

## 2. StyledProperty 详细规范

### 2.1 注册新属性

```csharp
public static readonly StyledProperty<T> XxxProperty =
    AvaloniaProperty.Register<OwnerType, T>(nameof(Xxx), defaultValue);
```

**强制规则：**

| 规则 | 说明 |
|---|---|
| 字段名 = `{PropertyName}Property` | 必须以 `Property` 后缀结尾 |
| `name` 参数 = `nameof(Xxx)` | **必须** 使用 `nameof()` 确保与 CLR 属性名一致，禁止硬编码字符串 |
| 类型参数 `TOwner` = 当前类 | 注册时 owner 类型必须是当前定义属性的类 |
| 字段修饰符 = `public static readonly` | 公共属性用 `public`，内部属性用 `internal static readonly` |
| 提供合理默认值 | 值类型使用语义化默认值；引用类型如果默认 `null` 可省略 |

**Register 方法的可选参数用法：**

```csharp
public static readonly StyledProperty<bool> BorderedProperty =
    AvaloniaProperty.Register<AbstractTag, bool>(
        nameof(Bordered), 
        defaultValue: true);                    // ← 默认值

public static readonly StyledProperty<SizeType> SizeTypeProperty =
    AvaloniaProperty.Register<StyledElement, SizeType>(
        nameof(SizeType), 
        SizeType.Middle);                       // ← 枚举默认值

public static readonly StyledProperty<ICommand?> CommandProperty =
    AvaloniaProperty.Register<Button, ICommand?>(
        nameof(Command), 
        enableDataValidation: true);            // ← 启用数据验证
```

### 2.2 使用 AddOwner 复用已有属性

当共享接口或其他控件已定义了相同语义的属性时，**必须** 使用 `AddOwner<T>()` 而非重新注册：

```csharp
// ✅ 正确：复用共享属性持有器中的定义
public static readonly StyledProperty<SizeType> SizeTypeProperty =
    SizeTypeControlProperty.SizeTypeProperty.AddOwner<Button>();

public static readonly StyledProperty<bool> IsMotionEnabledProperty =
    MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Button>();

public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
    WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Button>();

// ✅ 正确：复用 Avalonia 内置控件的属性
public static readonly StyledProperty<KeyGesture?> HotKeyProperty =
    HotKeyManager.HotKeyProperty.AddOwner<Button>();
```

```csharp
// ❌ 错误：重复注册已存在的属性
public static readonly StyledProperty<SizeType> SizeTypeProperty =
    AvaloniaProperty.Register<Button, SizeType>(nameof(SizeType), SizeType.Middle);
//  ↑ SizeTypeControlProperty 中已定义，应使用 AddOwner
```

**AddOwner 的适用场景：**

| 来源 | 示例 |
|---|---|
| `AtomUI.Controls.Shared` 中的共享属性持有器 | `SizeTypeControlProperty.SizeTypeProperty.AddOwner<T>()` |
| `AtomUI.Core` 中的属性持有器 | `MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<T>()` |
| `AtomUI.Desktop.Controls` 中的内部属性持有器 | `CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<T>()` |
| Avalonia 内置控件的属性 | `Border.BackgroundProperty.AddOwner<T>()` |

### 2.3 OverrideDefaultValue

当需要在子类中修改已有属性的默认值时，在 **静态构造函数** 中调用 `OverrideDefaultValue`：

```csharp
static Button()
{
    FocusableProperty.OverrideDefaultValue(typeof(Button), true);
}
```

---

## 3. DirectProperty 详细规范

### 3.1 注册方式

```csharp
internal static readonly DirectProperty<OwnerType, T> XxxProperty =
    AvaloniaProperty.RegisterDirect<OwnerType, T>(
        nameof(Xxx),
        o => o.Xxx,
        (o, v) => o.Xxx = v);      // 如果只读则省略 setter

private T _xxx;

internal T Xxx
{
    get => _xxx;
    set => SetAndRaise(XxxProperty, ref _xxx, value);  // ← 必须用 SetAndRaise
}
```

**强制规则：**

| 规则 | 说明 |
|---|---|
| CLR setter 使用 `SetAndRaise` | 禁止直接赋值字段，`SetAndRaise` 同时更新字段并触发属性变更通知 |
| backing field 以 `_` 前缀 | 例如 `_effectiveCornerRadius` |
| 只读属性省略 setter 委托 | `AvaloniaProperty.RegisterDirect<T, V>(name, getter)` |
| 只读属性 CLR setter 标记 `private` | 外部无法直接设置 |

### 3.2 AtomUI 中的典型用法

```csharp
// 内部计算属性，由控件逻辑写入
internal static readonly DirectProperty<Button, CornerRadius> EffectiveCornerRadiusProperty =
    AvaloniaProperty.RegisterDirect<Button, CornerRadius>(
        nameof(EffectiveCornerRadius),
        o => o.EffectiveCornerRadius,
        (o, v) => o.EffectiveCornerRadius = v);

private CornerRadius _effectiveCornerRadius;

internal CornerRadius EffectiveCornerRadius
{
    get => _effectiveCornerRadius;
    set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
}

// 只读标记属性
internal static readonly DirectProperty<AbstractTag, bool> IsPresetColorTagProperty =
    AvaloniaProperty.RegisterDirect<AbstractTag, bool>(
        nameof(IsPresetColorTag),
        o => o.IsPresetColorTag,
        (o, v) => o.IsPresetColorTag = v);

private bool _isPresetColorTag;

internal bool IsPresetColorTag
{
    get => _isPresetColorTag;
    set => SetAndRaise(IsPresetColorTagProperty, ref _isPresetColorTag, value);
}
```

### 3.3 何时选择 DirectProperty

在 AtomUI 中，以下情况 **应该** 使用 `DirectProperty`：

1. **需要在 Theme AXAML 中绑定但不需要被样式覆盖的计算属性**（如 `EffectiveCornerRadius`）
2. **控件内部状态标记**（如 `IsPresetColorTag`、`IsColorSet`）
3. **只读属性**（如 Avalonia 内置的 `IsPressed`）

> ⚠️ 注意：`DirectProperty` 不参与样式系统。如果属性需要在 `Style Selector` 中被设置值，必须使用 `StyledProperty`。

---

## 4. AttachedProperty 详细规范

### 4.1 注册方式

```csharp
public static readonly AttachedProperty<T> XxxProperty =
    AvaloniaProperty.RegisterAttached<OwnerType, HostType, T>(
        "Xxx", defaultValue);

public static T GetXxx(HostType element) => element.GetValue(XxxProperty);
public static void SetXxx(HostType element, T value) => element.SetValue(XxxProperty, value);
```

**强制规则：**

| 规则 | 说明 |
|---|---|
| 提供静态 `GetXxx` / `SetXxx` 方法 | 这是 XAML 引擎发现附加属性的约定 |
| `name` 参数不带 `Property` 后缀 | 即 `"Column"` 而非 `"ColumnProperty"` |
| `THost` 通常为 `Control` 或 `AvaloniaObject` | 表示可以附加到哪些类型 |

### 4.2 AtomUI 中的典型用法

附加属性在 AtomUI 中主要用于布局面板（如 Grid 的 Row/Col 系统）和控件容器场景。

---

## 5. CLR 属性包装器规范

### 5.1 StyledProperty 的 CLR 包装器

```csharp
/// <summary>
/// 获取或设置按钮类型。
/// </summary>
public ButtonType ButtonType
{
    get => GetValue(ButtonTypeProperty);
    set => SetValue(ButtonTypeProperty, value);
}
```

**强制规则：**

| 规则 | 说明 |
|---|---|
| getter 调用 `GetValue(XxxProperty)` | 禁止从 backing field 读取（StyledProperty 没有 backing field） |
| setter 调用 `SetValue(XxxProperty, value)` | 禁止绕过属性系统 |
| CLR 属性名与静态字段名去掉 `Property` 后缀后必须一致 | `ButtonTypeProperty` → `ButtonType` |
| 访问修饰符与静态字段一致 | `public` 字段 → `public` 属性；`internal` 字段 → `internal` 属性 |

### 5.2 DirectProperty 的 CLR 包装器

```csharp
private CornerRadius _effectiveCornerRadius;

internal CornerRadius EffectiveCornerRadius
{
    get => _effectiveCornerRadius;
    set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
}
```

**强制规则：**

| 规则 | 说明 |
|---|---|
| getter 直接读取 backing field | `get => _xxx;` |
| setter 使用 `SetAndRaise` | `set => SetAndRaise(XxxProperty, ref _xxx, value);` |
| **禁止** 对 DirectProperty 调用 `SetValue` | 会抛出异常 |

### 5.3 内容属性（Content Property）

当属性是控件的主要内容时，使用 `[Content]` 特性标记：

```csharp
[Content]
public string? TagText
{
    get => GetValue(TagTextProperty);
    set => SetValue(TagTextProperty, value);
}
```

这允许在 AXAML 中直接编写内容而无需显式指定属性名：

```xml
<atom:Tag>Hello World</atom:Tag>
<!-- 等价于 -->
<atom:Tag TagText="Hello World" />
```

---

## 6. 属性定义的代码区域组织

AtomUI 控件中的属性 **必须** 按以下 `#region` 组织：

```csharp
public class Button : AvaloniaButton, ISizeTypeAware, IWaveSpiritAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<ButtonType> ButtonTypeProperty = ...;
    public static readonly StyledProperty<SizeType> SizeTypeProperty = ...;
    // ... 所有 public 属性的静态字段

    public ButtonType ButtonType { get => ...; set => ...; }
    public SizeType SizeType { get => ...; set => ...; }
    // ... 所有 public 属性的 CLR 包装器

    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent = ...;

    public event EventHandler<RoutedEventArgs>? Closed { ... }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsIconVisibleProperty = ...;
    internal static readonly DirectProperty<Button, CornerRadius> EffectiveCornerRadiusProperty = ...;
    // ... 所有 internal 属性的静态字段

    // DirectProperty 的 backing fields
    private CornerRadius _effectiveCornerRadius;

    internal bool IsIconVisible { get => ...; set => ...; }
    internal CornerRadius EffectiveCornerRadius { get => ...; set => ...; }
    // ... 所有 internal 属性的 CLR 包装器

    #endregion

    // 其他私有字段
    private WaveSpiritDecorator? _waveSpiritDecorator;

    // 静态构造函数
    static Button() { ... }

    // 实例构造函数
    public Button() { ... }

    // 方法（OnPropertyChanged、OnApplyTemplate 等）
}
```

**关键规则：**

1. **先静态字段，后 CLR 包装器**：每个 `#region` 中先列出所有静态字段定义，再列出 CLR 属性包装器。
2. **公共属性在前，内部属性在后**：`#region 公共属性定义` → `#region 公共事件定义` → `#region 内部属性定义`。
3. **DirectProperty 的 backing field** 放在内部属性的 CLR 包装器之前。
4. **静态构造函数** 放在所有属性定义之后。

---

## 7. 共享属性持有器模式

AtomUI 使用"抽象属性持有器类"模式来定义跨控件共享的属性，避免重复注册：

### 7.1 定义共享属性

```csharp
// 在 AtomUI.Controls.Shared 或 AtomUI.Core 中定义
public abstract class SizeTypeControlProperty
{
    public const string SizeTypePropertyName = "SizeType";
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<StyledElement, SizeType>(SizeTypePropertyName, SizeType.Middle);
}
```

**规则：**

| 规则 | 说明 |
|---|---|
| 持有器类为 `abstract` | 防止被实例化 |
| 属性名字符串使用 `const string` | 如 `SizeTypePropertyName = "SizeType"` |
| 注册类型为 `StyledElement` | 使属性可被任意 Avalonia 控件使用 |

### 7.2 消费端使用

```csharp
// 在具体控件中使用 AddOwner
public static readonly StyledProperty<SizeType> SizeTypeProperty =
    SizeTypeControlProperty.SizeTypeProperty.AddOwner<Button>();
```

### 7.3 已有的共享属性持有器

在创建新属性前，**必须** 检查以下持有器中是否已有相同语义的属性：

| 持有器类 | 项目 | 包含的属性 |
|---|---|---|
| `SizeTypeControlProperty` | `AtomUI.Controls.Shared` | `SizeTypeProperty` |
| `CustomizableSizeTypeControlProperty` | `AtomUI.Controls.Shared` | `SizeTypeProperty`（使用 `CustomizableSizeType`） |
| `InputControlStatusProperty` | `AtomUI.Controls.Shared` | `StatusProperty` |
| `InputControlStyleVariantProperty` | `AtomUI.Controls.Shared` | `StyleVariantProperty` |
| `MotionAwareControlProperty` | `AtomUI.Core` | `IsMotionEnabledProperty`、`MotionDurationProperty` |
| `WaveSpiritAwareControlProperty` | `AtomUI.Controls.Shared` | `IsWaveSpiritEnabledProperty`、`WaveSpiritBrushProperty`、`WaveSpiritTypeProperty` |
| `CompactSpaceAwareControlProperty` | `AtomUI.Desktop.Controls` | `CompactSpaceItemPositionProperty`、`CompactSpaceOrientationProperty`、`IsUsedInCompactSpaceProperty` |

---

## 8. AffectsMeasure 与 AffectsRender 声明

在静态构造函数中，**必须** 声明属性变更对布局和渲染的影响：

```csharp
static Button()
{
    // 影响测量（布局）的属性
    AffectsMeasure<Button>(
        SizeTypeProperty,
        ShapeProperty,
        IconProperty,
        CompactSpaceItemPositionProperty,
        CompactSpaceOrientationProperty);

    // 影响渲染（重绘）的属性
    AffectsRender<Button>(
        ButtonTypeProperty,
        IsDangerProperty,
        IsGhostProperty);
}
```

**规则：**

| 规则 | 说明 |
|---|---|
| 改变控件尺寸的属性 → `AffectsMeasure` | 如 `SizeType`、`Shape`、`Icon`、`Padding` |
| 改变视觉外观但不改变尺寸的属性 → `AffectsRender` | 如 `ButtonType`、`IsDanger`、`Color` |
| 两者都不影响的属性不需要声明 | 如纯逻辑属性 |

---

## 9. 属性的 XML 文档注释

所有 **公共** 属性的静态字段和 CLR 包装器 **必须** 添加 XML 文档注释：

```csharp
/// <summary>
/// Defines the <see cref="ButtonType"/> property.
/// </summary>
public static readonly StyledProperty<ButtonType> ButtonTypeProperty = ...;

/// <summary>
/// 获取或设置按钮类型。
/// </summary>
public ButtonType ButtonType { ... }
```

**Token 属性** 也必须添加文档注释，说明其用途：

```csharp
/// <summary>
/// 主要按钮文本颜色
/// </summary>
public Color PrimaryColor { get; set; }
```

---

## 10. 禁止事项总结

| 禁止事项 | 原因 |
|---|---|
| ❌ 对 StyledProperty 使用 backing field | 值存储在属性系统中，backing field 会导致数据不一致 |
| ❌ 对 DirectProperty 调用 `SetValue` | 会抛出异常，必须用 `SetAndRaise` |
| ❌ `name` 参数硬编码字符串 | 必须使用 `nameof()` 确保一致性 |
| ❌ 重新注册已有的共享属性 | 必须使用 `AddOwner<T>()` 复用 |
| ❌ 在属性名中使用句号（`.`） | Avalonia 属性系统不允许 |
| ❌ 引用类型的默认值使用可变对象 | 会在所有实例间共享，导致状态污染 |
| ❌ 省略 `nameof()` 对齐检查 | CLR 属性名与注册名不一致会导致 XAML 绑定和样式失效 |

