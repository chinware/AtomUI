# 事件与生命周期规范

> 本文档规定 AtomUI 控件中路由事件的定义方式、控件生命周期管理、OnPropertyChanged 响应模式、模板部件获取与 Transition 配置。

---

## 1. 路由事件（RoutedEvent）

### 1.1 定义方式

```csharp
#region 公共事件定义

/// <summary>
/// Defines the <see cref="Closed"/> event.
/// </summary>
public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent =
    RoutedEvent.Register<AbstractTag, RoutedEventArgs>(
        nameof(Closed), 
        RoutingStrategies.Bubble);

/// <summary>
/// 当标签关闭时触发。
/// </summary>
public event EventHandler<RoutedEventArgs>? Closed
{
    add => AddHandler(ClosedEvent, value);
    remove => RemoveHandler(ClosedEvent, value);
}

#endregion
```

### 1.2 强制规则

| 规则 | 说明 |
|---|---|
| 静态字段名：`{EventName}Event` | 如 `ClosedEvent`、`ClickEvent` |
| `name` 参数使用 `nameof()` | `nameof(Closed)` |
| CLR 事件使用 `AddHandler` / `RemoveHandler` | 不直接使用 `event` 委托字段 |
| 路由策略选择 | 大多数事件使用 `RoutingStrategies.Bubble` |
| 放在 `#region 公共事件定义` 中 | 在公共属性之后，内部属性之前 |
| 事件参数继承 `RoutedEventArgs` | 如需自定义数据，创建 `XxxEventArgs : RoutedEventArgs` |

### 1.3 触发事件

```csharp
private void HandleCloseRequest(object? sender, EventArgs e)
{
    RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
}
```

### 1.4 路由策略选择

| 策略 | 使用场景 |
|---|---|
| `RoutingStrategies.Bubble` | 默认选择。事件从源控件向上冒泡到父级。适用于大多数用户交互事件。 |
| `RoutingStrategies.Tunnel` | 事件从根向下隧道到源控件。适用于需要预处理的事件。 |
| `RoutingStrategies.Direct` | 不冒泡不隧道，仅在源控件上触发。适用于内部通知事件。 |

---

## 2. OnPropertyChanged 响应

### 2.1 基本模式

```csharp
protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);  // ← 必须：始终先调用 base

    if (change.Property == ButtonTypeProperty)
    {
        ConfigureWaveSpiritType();
    }

    if (change.Property == ContentProperty ||
        change.Property == IsLoadingProperty)
    {
        UpdatePseudoClasses();
    }
    else if (change.Property == BorderBrushProperty ||
             change.Property == ButtonTypeProperty)
    {
        ConfigureEffectiveBorderThickness();
    }
}
```

### 2.2 强制规则

| 规则 | 说明 |
|---|---|
| **始终先调用 `base.OnPropertyChanged(change)`** | 确保基类和属性系统正确处理变更 |
| 使用 `change.Property == XxxProperty` 匹配 | 使用引用比较，不使用字符串 |
| 使用 `change.GetOldAndNewValue<T>()` 获取值 | 类型安全的值获取方式 |
| 复杂逻辑抽取为独立方法 | 如 `ConfigureWaveSpiritType()`、`UpdatePseudoClasses()` |

### 2.3 获取旧值和新值

```csharp
// 同时获取旧值和新值
if (change.Property == FlyoutProperty)
{
    var (oldFlyout, newFlyout) = change.GetOldAndNewValue<FlyoutBase?>();
    if (oldFlyout != null) { /* ... */ }
    if (newFlyout != null) { /* ... */ }
}

// 仅获取新值
if (change.Property == IsDefaultProperty)
{
    var isDefault = change.GetNewValue<bool>();
}

// 检查旧值（用于检测变更方向）
if (change.Property == IsPressedProperty)
{
    if (change.OldValue as bool? == true)
    {
        // 从 pressed 变为 unpressed
    }
}
```

### 2.4 条件处理：仅在已附加到可视树时

某些逻辑仅在控件已加载时才需要执行：

```csharp
if (this.IsAttachedToVisualTree())
{
    if (change.Property == TagColorProperty)
    {
        SetupTagColorInfo(TagColor);
    }
}
```

```csharp
if (IsLoaded)
{
    if (change.Property == IsMotionEnabledProperty)
    {
        ConfigureTransitions(true);
    }
}
```

---

## 3. OnApplyTemplate — 模板部件获取

### 3.1 基本模式

```csharp
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);  // ← 必须：始终先调用 base

    // 1. 清理旧模板部件的事件订阅
    if (CloseButton != null)
    {
        CloseButton.Click -= HandleCloseRequest;
    }

    // 2. 获取新模板部件
    CloseButton = e.NameScope.Find<AbstractIconButton>("PART_CloseButton");
    _waveSpiritDecorator = e.NameScope.Find<WaveSpiritDecorator>(
        ButtonThemeConstants.WaveSpiritPart);

    // 3. 为新模板部件订阅事件
    if (CloseButton != null)
    {
        CloseButton.Click += HandleCloseRequest;
    }

    // 4. 初始化状态
    UpdatePseudoClasses();
    ConfigureEffectiveBorderThickness();
}
```

### 3.2 强制规则

| 规则 | 说明 |
|---|---|
| **始终先调用 `base.OnApplyTemplate(e)`** | 确保基类正确处理模板应用 |
| 先清理旧订阅，再获取新部件 | 防止内存泄漏和重复订阅 |
| 使用 `e.NameScope.Find<T>()` | 类型安全的模板部件查找 |
| 部件名使用 `ThemeConstants` 常量 | 如 `ButtonThemeConstants.WaveSpiritPart` |
| 部件引用可为 `null` | 如果模板中未定义该部件 |

### 3.3 模板部件字段定义

```csharp
// 使用 protected 以便子类访问
protected AbstractIconButton? CloseButton;

// 使用 private 如果仅当前类使用
private WaveSpiritDecorator? _waveSpiritDecorator;
```

---

## 4. Transition（动画过渡）配置

### 4.1 配置模式

AtomUI 控件的 Transition 配置遵循 **延迟初始化** 模式：在 `OnLoaded` 中配置，在 `OnUnloaded` 中清理。

```csharp
protected override void OnLoaded(RoutedEventArgs e)
{
    base.OnLoaded(e);
    ConfigureTransitions(false);  // ← 初次配置
}

protected override void OnUnloaded(RoutedEventArgs e)
{
    base.OnUnloaded(e);
    Transitions = null;           // ← 清理
}
```

### 4.2 Transition 配置方法

```csharp
private void ConfigureTransitions(bool force)
{
    if (IsMotionEnabled)
    {
        if (force || Transitions == null)
        {
            var transitions = new Transitions();
            transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(
                BackgroundProperty));
            
            // 根据控件状态添加不同的 transition
            if (ButtonType == ButtonType.Default)
            {
                transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(
                    BorderBrushProperty));
                transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(
                    ForegroundProperty));
            }

            Transitions = transitions;
        }
    }
    else
    {
        Transitions = null;
    }
}
```

### 4.3 强制规则

| 规则 | 说明 |
|---|---|
| 检查 `IsMotionEnabled` | 尊重用户的动画开关设置 |
| `force` 参数 | `true` 表示强制重建，`false` 表示仅在未初始化时创建 |
| 使用 `TransitionUtils.CreateTransition<T>()` | AtomUI 的标准 Transition 创建工具 |
| `OnUnloaded` 中置 `null` | 控件卸载时清理 Transition |
| `IsMotionEnabled` 变更时重新配置 | 在 `OnPropertyChanged` 中监听 |

### 4.4 动态响应动画开关

```csharp
protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);

    if (IsLoaded)
    {
        if (change.Property == IsMotionEnabledProperty ||
            change.Property == IsWaveSpiritEnabledProperty)
        {
            ConfigureTransitions(true);  // ← force = true 重建
        }
    }
}
```

---

## 5. 视觉树附加/分离

### 5.1 OnAttachedToVisualTree / OnDetachedFromVisualTree

用于管理与可视化树生命周期相关的订阅：

```csharp
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    
    // 订阅主题变更
    if (ThemeManager.Current != null)
    {
        ThemeManager.Current.ThemeChanged += HandleActualThemeVariantChanged;
    }
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    
    // 取消订阅
    if (ThemeManager.Current != null)
    {
        ThemeManager.Current.ThemeChanged -= HandleActualThemeVariantChanged;
    }
}
```

### 5.2 规则

| 规则 | 说明 |
|---|---|
| 在 `Attached` 中订阅，在 `Detached` 中取消 | 防止内存泄漏 |
| **始终先调用 `base`** | 确保基类正确处理 |
| 用于管理外部事件订阅 | 如 ThemeChanged、窗口事件等 |

---

## 6. OnAttachedToLogicalTree / OnDetachedFromLogicalTree

用于管理与逻辑树相关的操作（如 Command 绑定）：

```csharp
protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
{
    base.OnAttachedToLogicalTree(e);
    
    if (Command is not null)
    {
        Command.CanExecuteChanged += CanExecuteChangedHandler;
    }
}

protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
{
    base.OnDetachedFromLogicalTree(e);
    
    if (Command is { } command)
    {
        command.CanExecuteChanged -= CanExecuteChangedHandler;
    }
}
```

---

## 7. 生命周期方法调用顺序

一个典型 AtomUI 控件的生命周期方法调用顺序：

```
构造函数
    ↓
OnAttachedToLogicalTree    ← 逻辑树附加
    ↓
OnAttachedToVisualTree     ← 可视树附加（订阅外部事件）
    ↓
OnApplyTemplate            ← 模板应用（获取部件、初始化状态）
    ↓
OnLoaded                   ← 控件加载完成（配置 Transition）
    ↓
[OnPropertyChanged ×N]     ← 属性变更响应
    ↓
OnUnloaded                 ← 控件卸载（清理 Transition）
    ↓
OnDetachedFromVisualTree   ← 可视树分离（取消订阅）
    ↓
OnDetachedFromLogicalTree  ← 逻辑树分离
```

### 7.1 各阶段职责

| 生命周期方法 | 职责 |
|---|---|
| 构造函数 | 注册 Token 作用域 |
| 静态构造函数 | `AffectsMeasure`、`AffectsRender`、`OverrideDefaultValue` |
| `OnAttachedToLogicalTree` | Command 绑定、HotKey 设置 |
| `OnAttachedToVisualTree` | 订阅 ThemeChanged、窗口事件 |
| `OnApplyTemplate` | 获取模板部件、初始化伪类、初始化状态 |
| `OnLoaded` | 配置 Transition |
| `OnPropertyChanged` | 响应属性变更、更新伪类、重配置 |
| `OnUnloaded` | 清理 Transition（`Transitions = null`） |
| `OnDetachedFromVisualTree` | 取消订阅 ThemeChanged、窗口事件 |
| `OnDetachedFromLogicalTree` | 取消 Command 绑定、清理 HotKey |

---

## 8. MeasureOverride / ArrangeOverride

当控件需要自定义测量或排列逻辑时：

```csharp
protected override Size MeasureOverride(Size availableSize)
{
    var size = base.MeasureOverride(availableSize);
    var targetWidth  = size.Width;
    var targetHeight = size.Height;

    targetWidth = Math.Max(targetWidth, targetHeight);

    if (Shape == ButtonShape.Circle)
    {
        targetWidth  = targetHeight;
        CornerRadius = new CornerRadius(targetHeight);
    }
    else if (Shape == ButtonShape.Round)
    {
        CornerRadius = new CornerRadius(targetHeight);
        targetWidth  = Math.Max(targetWidth, targetHeight + targetHeight / 2);
    }

    return new Size(targetWidth, targetHeight);
}
```

**规则**：通常先调用 `base.MeasureOverride` 获取基础尺寸，再根据控件特性调整。

---

## 9. 主题变更响应

当控件需要响应全局主题变更时：

```csharp
private void HandleActualThemeVariantChanged(object? sender, ThemeChangedEventArgs e)
{
    // 重新计算依赖主题的数据
    SetupStatusColorMap(true);
    SetupPresetColorMap(true);
    
    if (TagColor is not null)
    {
        SetupTagColorInfo(TagColor);
    }
    
    InvalidateVisual();
}
```

---

## 10. 禁止事项

| 禁止事项 | 原因 |
|---|---|
| ❌ 在 `OnPropertyChanged` 中不调用 `base` | 破坏属性系统和基类逻辑 |
| ❌ 在 `OnApplyTemplate` 中不调用 `base` | 破坏模板应用流程 |
| ❌ 在构造函数中获取模板部件 | 模板尚未应用 |
| ❌ 在 `OnAttachedToVisualTree` 中订阅而不在 `OnDetachedFromVisualTree` 中取消 | 导致内存泄漏 |
| ❌ 不检查 `IsMotionEnabled` 就配置 Transition | 不尊重用户动画设置 |
| ❌ 在 `OnLoaded` 之前配置 Transition | 控件可能尚未完全初始化 |
| ❌ 在 `OnPropertyChanged` 中直接执行重量级操作 | 应抽取为方法，保持可读性 |

