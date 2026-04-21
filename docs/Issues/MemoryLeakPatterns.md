# AtomUI 常见内存泄漏模式与规避指南

**创建日期**: 2026-04-21
**适用范围**: AtomUI 项目中基于 Avalonia 的控件开发

---

## 概述

本文档总结了 AtomUI 项目中已确认的内存泄漏模式，帮助团队在开发中规避这些问题。每种模式都包含泄漏原理、识别方法和修复方案。

---

## 模式一：全局 Property.Changed.Subscribe 泄漏

### 泄漏原理

Avalonia 中每个 `AvaloniaProperty` 都有一个静态的 `Changed` 字段（类型为 `LightweightSubject`）。在实例方法中调用 `Property.Changed.Subscribe()` 会向这个**全局**观察者列表添加 observer，lambda 捕获的控件实例将永远无法被 GC。

### 典型错误代码

```csharp
// ❌ 错误：在实例方法中订阅全局 Changed
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    _subscription = SomeProperty.Changed.Subscribe(args =>
    {
        if (args.Sender == _target)
        {
            // 处理逻辑
        }
    });
}
```

### 保留链

```
SomeProperty._changed (LightweightSubject, 全局静态)
  → observer (lambda)
    → this (控件实例)
      → 整个控件子树
```

### 正确做法

```csharp
// ✅ 正确：使用实例级 GetObservable
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    _subscription?.Dispose();
    _subscription = _target.GetObservable(SomeProperty)
        .Subscribe(value =>
        {
            // 处理逻辑
        });
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _subscription?.Dispose();
    _subscription = null;
}
```

### 关键区别

| 方式 | 作用域 | GC 行为 |
|------|--------|---------|
| `Property.Changed.Subscribe()` | 全局静态，所有实例的变化都触发 | observer 永驻全局列表，阻止 GC |
| `instance.GetObservable(Property)` | 单个实例 | 实例被 GC 时订阅自动失效 |
| `Property.Changed.AddClassHandler<T>()` | 全局静态，但仅在静态构造函数中调用一次 | 不增长，安全 |

### 安全使用场景

- `static` 构造函数中调用 `AddClassHandler` — 由属性系统管理，不创建 LightweightSubject 订阅
- ~~`static` 构造函数中调用 `Changed.Subscribe` 且回调为静态方法~~ — 见模式四，应改用 `AddClassHandler`

### 已修复案例

- `FlyoutStateHelper.cs` — `IsPointerOverProperty.Changed.Subscribe` / `IsFocusedProperty.Changed.Subscribe`
- `DatePickerPresenter.cs` — `IsPointerInMonthViewProperty.Changed.Subscribe`
- `TimePickerPresenter.cs` — `IsPointerInSelectorProperty.Changed.Subscribe`
- `GradientColorSlider.cs` — `ActivatedThumbProperty.Changed.Subscribe`

---

## 模式二：AXAML 属性选择器导致 PropertyEqualsActivator 泄漏

### 泄漏原理

AXAML 主题中使用属性选择器（如 `^[StyleVariant=Outlined]`）时，Avalonia 的样式系统会为每个控件实例创建 `PropertyEqualsActivator`。该 activator 通过 `AvaloniaPropertyObservable` 订阅属性变化。当控件从 visual tree 移除时，如果框架未正确 dispose 这些订阅，就会形成泄漏。

### 典型保留链

```
AvaloniaPropertyObservable<Object, Object>._observers
  → List<IObserver<Object>>._items
    → PropertyEqualsActivator._property
      → StyledProperty<T>
```

### 典型错误代码

```xml
<!-- ❌ 属性选择器：每个实例创建 PropertyEqualsActivator -->
<Style Selector="^[StyleVariant=Outlined]">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />
</Style>
<Style Selector="^[StyleVariant=Filled]">
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorFillTertiary}" />
</Style>
```

### 正确做法

使用伪类选择器替代属性选择器。伪类通过控件自身的 `Classes` 集合驱动样式激活，不创建外部订阅，生命周期与控件绑定。

**Step 1 — 定义伪类常量：**

```csharp
public class MyControlPseudoClass
{
    public const string Outline = ":outline";
    public const string Filled = ":filled";
}
```

**Step 2 — 同步伪类状态：**

```csharp
protected virtual void UpdatePseudoClasses()
{
    PseudoClasses.Set(MyControlPseudoClass.Outline, StyleVariant == InputControlStyleVariant.Outlined);
    PseudoClasses.Set(MyControlPseudoClass.Filled, StyleVariant == InputControlStyleVariant.Filled);
}

protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);
    if (change.Property == StyleVariantProperty)
    {
        UpdatePseudoClasses(); // 属性变化时立即同步，不受 IsAttachedToVisualTree 限制
    }
}

protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    UpdatePseudoClasses(); // 确保初始状态正确
}
```

**Step 3 — AXAML 使用伪类选择器：**

```xml
<!-- ✅ 伪类选择器：不创建 PropertyEqualsActivator -->
<Style Selector="^:outline">
    <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />
</Style>
<Style Selector="^:filled">
    <Setter Property="Background" Value="{atom:SharedTokenResource ColorFillTertiary}" />
</Style>
```

### 为什么伪类不泄漏

| 机制 | 属性选择器 | 伪类选择器 |
|------|-----------|-----------|
| 激活方式 | 创建 `PropertyEqualsActivator`，通过 `GetObservable` 订阅属性 | 控件自身调用 `PseudoClasses.Set()`，通过 `Classes` 集合通知样式系统 |
| 引用方向 | 外部 observable 持有对 activator 的强引用 | 整个引用链在控件自己的对象图内 |
| GC 行为 | 需要框架显式 dispose 订阅 | 控件不可达时整条链自动回收 |
| 本质区别 | "拉"模式 — 创建 observable 去监听 | "推"模式 — 控件主动通知样式系统 |

### 已修复案例

- `AddOnDecoratedBoxTheme.axaml` — `^[StyleVariant=Outlined/Filled/Borderless/Underlined]` → `^:outline/:filled/:borderless/:underlined`

---

## 模式三：覆盖旧订阅未 Dispose

### 泄漏原理

在生命周期方法中创建新订阅时，直接覆盖旧的 `IDisposable` 字段而未先 Dispose，导致旧订阅永远留在观察者列表中。

### 典型错误代码

```csharp
// ❌ 错误：直接覆盖，旧订阅泄漏
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    _disposable = new CompositeDisposable(2);  // 旧的 _disposable 未 Dispose
    _disposable.Add(someObservable.Subscribe(...));
}
```

### 正确做法

```csharp
// ✅ 正确：先 Dispose 旧的再创建新的
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    _disposable?.Dispose();  // 先清理
    _disposable = new CompositeDisposable(2);
    _disposable.Add(someObservable.Subscribe(...));
}

protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _disposable?.Dispose();
    _disposable = null;  // 设置为 null 防止重复 Dispose
}
```

### 已修复案例

- `WindowTitleBar.cs` — `OnAttachedToVisualTree` 中覆盖 `_disposables` 未先 Dispose

---

## 模式四：静态构造函数中使用 Subscribe 代替 AddClassHandler

### 泄漏原理

在静态构造函数中通过 `Property.Changed.Subscribe()` 订阅属性变化，会在 `LightweightSubject` 上创建一个永久存在的 reactive 订阅。虽然不会捕获实例引用，但这个订阅绕过了 Avalonia 属性系统的 class handler 机制，无法被框架管理生命周期。对于附加属性（Attached Property），所有设置了该属性的控件实例的变化都会经过这个订阅，增加不必要的开销。

### 典型错误代码

```csharp
// ❌ 错误：在静态构造函数中使用 Subscribe
static MyControl()
{
    MyAttachedProperty.Changed.Subscribe(HandlePropertyChanged);
}

private static void HandlePropertyChanged(AvaloniaPropertyChangedEventArgs<Control?> e)
{
    var sender = (Visual)e.Sender;
    var value = e.NewValue.GetValueOrDefault();
    // ...
}
```

### 正确做法

```csharp
// ✅ 正确：使用 AddClassHandler，由属性系统管理
static MyControl()
{
    MyAttachedProperty.Changed.AddClassHandler<Visual>(HandlePropertyChanged);
}

private static void HandlePropertyChanged(Visual sender, AvaloniaPropertyChangedEventArgs e)
{
    var value = e.NewValue as Control;
    // ...
}
```

### 关键区别

| 方式 | 机制 | 生命周期管理 |
|------|------|-------------|
| `Property.Changed.Subscribe()` | 在 `LightweightSubject` 上创建 reactive 订阅 | 需要手动 Dispose（静态构造函数中无法做到） |
| `Property.Changed.AddClassHandler<T>()` | 注册到属性系统的 class handler 列表 | 由 Avalonia 属性系统管理，无需 Dispose |

### 注意事项

- 使用 `AddClassHandler<T>` 后，handler 签名从 `(AvaloniaPropertyChangedEventArgs<TValue> e)` 变为 `(T sender, AvaloniaPropertyChangedEventArgs e)`
- 取值方式从 `e.NewValue.GetValueOrDefault()` 变为 `e.NewValue as TValue`（非泛型 args）
- `sender` 参数直接提供，无需从 `e.Sender` 强转

### 已修复案例

- `ScopeAwareAdornerLayer.cs` — `AdornedElementProperty.Changed.Subscribe` / `AdornerProperty.Changed.Subscribe`

---

## 快速检查清单

开发新控件或 review 代码时，检查以下要点：

- [ ] 是否在实例方法中使用了 `Property.Changed.Subscribe()`？→ 改用 `instance.GetObservable(Property)`
- [ ] 是否在静态构造函数中使用了 `Property.Changed.Subscribe()`？→ 改用 `Property.Changed.AddClassHandler<T>()`
- [ ] AXAML 主题中是否使用了 `^[Property=Value]` 属性选择器？→ 考虑改用伪类选择器
- [ ] 创建新订阅前是否先 Dispose 了旧的？
- [ ] `OnDetachedFromVisualTree` 中是否清理了所有订阅？
- [ ] Dispose 后是否将字段设置为 null？
- [ ] 伪类是否在 `OnApplyTemplate` 和 `OnPropertyChanged` 中都正确同步？

---

## 诊断工具

使用内存分析工具（如 dotMemory）时，关注以下 retention path 特征：

1. **模式一特征**: `LightweightSubject` → observer 链无限增长
2. **模式二特征**: `AvaloniaPropertyObservable._observers` → `PropertyEqualsActivator._property` → `StyledProperty<T>`
3. **模式三特征**: 同一属性的多个 observer 实例（应该只有一个）
4. **模式四特征**: `LightweightSubject` 中存在永久的静态方法委托订阅，对应属性为 AttachedProperty
