# 自定义控件接入动画与涟漪效果开发指南

## 1. 概述

本文档面向控件开发者，详细说明如何让自定义控件支持 AtomUI 的动画控制系统（`IMotionAwareControl`）和点击涟漪效果系统（`IWaveSpiritAwareControl`），使其与 AtomUI 内置控件表现一致。

> 前置阅读：[动画与涟漪效果控制系统设计原理](./MotionAndWaveSpiritDesign.md)
>
> 相关文档：[全局动画与涟漪效果开关使用指南](./GlobalMotionControlGuide.md)

---

## 2. 快速判断：你的控件需要实现哪个接口？

```
你的控件有动画效果吗？（过渡动画、展开/折叠、渐变等）
│
├── 是 → 实现 IMotionAwareControl
│        │
│        └── 控件可点击且需要涟漪反馈？（按钮、复选框、开关等）
│            │
│            ├── 是 → 改为实现 IWaveSpiritAwareControl（已包含 IMotionAwareControl）
│            └── 否 → 保持 IMotionAwareControl 即可
│
└── 否 → 不需要实现任何动画接口
```

---

## 3. 实现 IMotionAwareControl（仅动画控制）

### 3.1 属性注册

```csharp
using AtomUI.Controls;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;

namespace MyApp.Controls;

public class MyAnimatedControl : TemplatedControl, IMotionAwareControl
{
    // ===== 使用 AddOwner 注册共享属性 =====
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MyAnimatedControl>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
}
```

### 3.2 绑定全局 Token

在控件构造函数中调用扩展方法，将 `IsMotionEnabled` 绑定到全局 `EnableMotion` Token：

```csharp
public MyAnimatedControl()
{
    // 自动绑定 IsMotionEnabled ← SharedToken.EnableMotion
    this.ConfigureMotionBindingStyle();
}
```

### 3.3 根据 IsMotionEnabled 管理过渡动画

在控件中根据 `IsMotionEnabled` 动态启用/禁用过渡动画：

```csharp
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

protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);

    if (IsLoaded && change.Property == IsMotionEnabledProperty)
    {
        ConfigureTransitions(true);
    }
}

private void ConfigureTransitions(bool force)
{
    if (IsMotionEnabled)
    {
        if (force || Transitions == null)
        {
            Transitions = [
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty)
            ];
        }
    }
    else
    {
        Transitions = null;  // ← 关闭动画时移除所有过渡
    }
}
```

**关键模式**：

1. `OnLoaded` 中首次配置过渡（`force = false`，仅在 Transitions 为 null 时设置）
2. `OnUnloaded` 中清除过渡（避免脱离视觉树后仍有动画资源）
3. `IsMotionEnabledProperty` 变化时强制重新配置（`force = true`）
4. `IsMotionEnabled == false` 时，设置 `Transitions = null` 完全禁用过渡

---

## 4. 实现 IWaveSpiritAwareControl（动画 + 涟漪）

### 4.1 属性注册

```csharp
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace MyApp.Controls;

public class MyClickableControl : TemplatedControl, IWaveSpiritAwareControl
{
    // ===== IMotionAwareControl 属性（继承自接口） =====
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MyClickableControl>();

    // ===== IWaveSpiritAwareControl 属性 =====
    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<MyClickableControl>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }
}
```

### 4.2 绑定全局 Token

使用 `ConfigureWaveSpiritBindingStyle`（同时绑定 `EnableMotion` 和 `EnableWaveSpirit`）：

```csharp
public MyClickableControl()
{
    // 同时绑定 IsMotionEnabled ← EnableMotion 和 IsWaveSpiritEnabled ← EnableWaveSpirit
    this.ConfigureWaveSpiritBindingStyle();
}
```

> ⚠️ **只需调用 `ConfigureWaveSpiritBindingStyle`**，无需额外调用 `ConfigureMotionBindingStyle`，因为前者已包含后者的绑定。

### 4.3 在 AXAML 模板中放置 WaveSpiritDecorator

```xml
<ControlTheme x:Key="{x:Type local:MyClickableControl}" TargetType="local:MyClickableControl">
    <Setter Property="Template">
        <ControlTemplate>
            <Panel>
                <!-- 涟漪装饰器必须放在最底层 -->
                <atom:WaveSpiritDecorator Name="PART_WaveSpirit"
                                           CornerRadius="{TemplateBinding CornerRadius}"
                                           WaveType="RoundRectWave" />
                <!-- 控件主体内容在涟漪之上 -->
                <Border Name="Frame"
                        Background="{TemplateBinding Background}"
                        BorderBrush="{TemplateBinding BorderBrush}"
                        BorderThickness="{TemplateBinding BorderThickness}"
                        CornerRadius="{TemplateBinding CornerRadius}"
                        Padding="{TemplateBinding Padding}">
                    <ContentPresenter Content="{TemplateBinding Content}" />
                </Border>
            </Panel>
        </ControlTemplate>
    </Setter>

    <Setter Property="ClipToBounds" Value="False" />
</ControlTheme>
```

**要点**：

- `WaveSpiritDecorator` 必须放在 `Panel` 的**第一个子元素**位置（最底层），涟漪在控件内容之下扩散
- `Name` 必须为 `"PART_WaveSpirit"`（使用 `WaveSpiritDecorator.WaveSpiritPart` 常量）
- `CornerRadius` 绑定到控件的圆角，确保涟漪形状与控件匹配
- `WaveType` 设置涟漪形状（`RoundRectWave` / `CircleWave` / `PillWave`）
- 控件的 `ClipToBounds` 必须为 `False`，否则涟漪向外扩散的部分会被裁剪

### 4.4 在代码中获取并触发涟漪

```csharp
private WaveSpiritDecorator? _waveSpiritDecorator;

protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    _waveSpiritDecorator = e.NameScope.Find<WaveSpiritDecorator>("PART_WaveSpirit");
}

// 在用户交互时触发涟漪
private void OnClicked()
{
    if (IsWaveSpiritEnabled)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _waveSpiritDecorator?.Play();
        });
    }
}
```

**触发时机的最佳实践**：

| 控件类型 | 触发时机 | 示例 |
|---|---|---|
| 按钮 | `IsPressed` 从 `true` 变为 `false`（释放时） | `Button` |
| 复选框/开关 | `IsChecked` 变化时 | `CheckBox`、`ToggleSwitch` |
| 单选按钮 | 选中时 | `RadioButton` |

### 4.5 动态切换涟漪形状

如果控件形状可变（如 Button 支持 Default/Round/Circle），需要在形状变化时更新涟漪类型：

```csharp
// 注册 WaveSpiritType 属性
internal static readonly StyledProperty<WaveSpiritType> WaveSpiritTypeProperty =
    WaveSpiritAwareControlProperty.WaveSpiritTypeProperty.AddOwner<MyClickableControl>();

// 在形状变化时更新
private void ConfigureWaveSpiritType()
{
    WaveSpiritType = Shape switch
    {
        MyShape.Default => WaveSpiritType.RoundRectWave,
        MyShape.Round   => WaveSpiritType.PillWave,
        MyShape.Circle  => WaveSpiritType.CircleWave,
        _               => WaveSpiritType.RoundRectWave
    };
}
```

并在模板中使用 `{TemplateBinding WaveSpiritType}` 动态绑定。

### 4.6 自定义涟漪颜色

默认情况下，涟漪颜色为 `ColorPrimary`（主色）。某些场景需要自定义涟漪颜色（如 Danger 按钮使用红色涟漪）：

```csharp
// 在触发涟漪前设置自定义颜色
if (IsDanger)
{
    _waveSpiritDecorator.WaveBrush = Foreground;  // 或 Background
}
Dispatcher.UIThread.Post(() => _waveSpiritDecorator?.Play());
```

---

## 5. 在 Form 中使用

### 5.1 IsMotionEnabled 自动级联

当控件放入 Form 中时，如果实现了 `IMotionAwareControl`，FormItem 会自动将 `IsMotionEnabled` 从 Form 级联到控件：

```xml
<atom:Form IsMotionEnabled="False">
    <!-- 内部所有实现 IMotionAwareControl 的控件自动禁用动画 -->
    <atom:FormItem LabelText="Name" FieldName="name">
        <atom:LineEdit />  <!-- IsMotionEnabled 自动为 False -->
    </atom:FormItem>
</atom:Form>
```

### 5.2 IsWaveSpiritEnabled 不通过 Form 级联

涟漪效果由全局 Token 直接控制，不参与 Form 级联。在 Form 中放置按钮时，按钮的涟漪效果始终由全局 `EnableWaveSpirit` Token 决定。

---

## 6. 实现检查清单

### IMotionAwareControl 接入

- [ ] 实现 `IMotionAwareControl` 接口
- [ ] 使用 `MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<T>()` 注册属性
- [ ] 在构造函数中调用 `this.ConfigureMotionBindingStyle()`
- [ ] 在 `OnLoaded` 中初始化 Transitions
- [ ] 在 `OnUnloaded` 中清除 Transitions
- [ ] 在 `IsMotionEnabledProperty` 变化时重新配置 Transitions
- [ ] `IsMotionEnabled == false` 时设置 `Transitions = null`

### IWaveSpiritAwareControl 接入

- [ ] 实现 `IWaveSpiritAwareControl` 接口（自动包含 `IMotionAwareControl`）
- [ ] 使用 `AddOwner<T>()` 注册 `IsMotionEnabledProperty` 和 `IsWaveSpiritEnabledProperty`
- [ ] 在构造函数中调用 `this.ConfigureWaveSpiritBindingStyle()`（**不要**额外调用 `ConfigureMotionBindingStyle`）
- [ ] 在 AXAML 模板中放置 `<atom:WaveSpiritDecorator Name="PART_WaveSpirit" />`
- [ ] `WaveSpiritDecorator` 放在 Panel 的最底层
- [ ] 控件 `ClipToBounds` 设为 `False`
- [ ] 在 `OnApplyTemplate` 中获取 `_waveSpiritDecorator` 引用
- [ ] 在用户交互时判断 `IsWaveSpiritEnabled` 后调用 `_waveSpiritDecorator?.Play()`
- [ ] 使用 `Dispatcher.UIThread.Post` 包裹 `Play()` 调用

### 常见错误

| 错误 | 后果 | 解决方案 |
|---|---|---|
| 使用 `Register` 而非 `AddOwner` | Form 级联绑定失败 | 使用 `AddOwner<T>()` |
| 未调用 `ConfigureMotionBindingStyle` / `ConfigureWaveSpiritBindingStyle` | 控件不响应全局动画开关 | 在构造函数中调用 |
| `IWaveSpiritAwareControl` 控件同时调用两个扩展方法 | `IsMotionEnabled` 被绑定两次 | 只调用 `ConfigureWaveSpiritBindingStyle` |
| `WaveSpiritDecorator` 不在 Panel 最底层 | 涟漪遮挡控件内容 | 放在 Panel 的第一个子元素位置 |
| `ClipToBounds` 为 `True` | 涟漪扩散被裁剪 | 设为 `False` |
| `OnUnloaded` 中未清除 Transitions | 脱离视觉树后仍持有动画资源 | 设置 `Transitions = null` |
| 未判断 `IsWaveSpiritEnabled` 就调用 `Play()` | 全局关闭涟漪后仍触发 | 先判断 `IsWaveSpiritEnabled` |
| 未使用 `Dispatcher.UIThread.Post` | 可能在属性变更回调中触发动画导致异常 | 使用 `Post` 延迟到下一帧 |

