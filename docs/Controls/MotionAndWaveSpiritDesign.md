# AtomUI 动画与涟漪效果控制系统设计原理

## 1. 概述

AtomUI 提供两个层级的动画控制系统：**通用动画控制**（`IMotionAwareControl`）管理所有过渡动画和关键帧动画的全局开关，**涟漪效果控制**（`IWaveSpiritAwareControl`）管理 Ant Design 标志性的点击涟漪扩散效果。两者通过 Design Token 系统的 Seed Token 实现全局统一控制，并支持 Form 级联传播。

这一设计对应 Ant Design 5.0 的 [`token.motion`](https://ant.design/docs/react/customize-theme) 和 [`token.wave`](https://ant.design/components/config-provider#api) 配置，AtomUI 将其抽象为两个接口 + 共享属性 + Token 绑定体系。

### 核心设计理念

1. **分层控制**：`IMotionAwareControl` 控制所有过渡动画（颜色渐变、尺寸变化），`IWaveSpiritAwareControl` 在此基础上额外控制点击涟漪效果。涟漪接口继承自动画接口，形成自然的层级关系。
2. **全局开关**：通过 `EnableMotion` 和 `EnableWaveSpirit` 两个 Seed Token，可在 `ThemeManager` 级别一键开启/关闭所有动画或涟漪效果。
3. **自动绑定**：控件通过扩展方法自动将 `IsMotionEnabled` / `IsWaveSpiritEnabled` 属性绑定到全局 Token 资源，无需手动编码。
4. **Form 级联**：Form → FormItem → 内容控件的 `IsMotionEnabled` 自动向下传播。
5. **无障碍友好**：关闭全局动画后，所有控件立即切换为无动画模式，满足 `prefers-reduced-motion` 等无障碍需求。

---

## 2. 接口继承关系

```
IMotionAwareControl           ← 通用动画控制（所有动画类控件）
    │
    └── IWaveSpiritAwareControl   ← 涟漪效果控制（可点击的交互控件）
```

`IWaveSpiritAwareControl` **继承自** `IMotionAwareControl`，意味着：
- 实现涟漪效果的控件自动也支持通用动画控制
- 关闭 `IsMotionEnabled` 时，过渡动画和涟漪效果同时停止
- 可以单独关闭涟漪效果（`IsWaveSpiritEnabled = false`）而保留过渡动画

---

## 3. 类型定义

### 3.1 IMotionAwareControl

定义在 `AtomUI.Core/Controls/IMotionAwareControl.cs`：

```csharp
public interface IMotionAwareControl
{
    bool IsMotionEnabled { get; }
}
```

标记控件支持**动画开关控制**。当 `IsMotionEnabled` 为 `false` 时，控件应禁用所有过渡动画（Transition）和关键帧动画（Animation）。

### 3.2 MotionAwareControlProperty（共享属性持有者）

```csharp
public abstract class MotionAwareControlProperty
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        AvaloniaProperty.Register<StyledElement, bool>("IsMotionEnabled");

    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        AvaloniaProperty.Register<StyledElement, TimeSpan>("MotionDuration", TimeSpan.FromMilliseconds(200));
}
```

### 3.3 IWaveSpiritAwareControl

定义在 `AtomUI.Controls.Shared/WaveSpirit/IWaveSpiritAware.cs`：

```csharp
public interface IWaveSpiritAwareControl : IMotionAwareControl
{
    bool IsWaveSpiritEnabled { get; }
}
```

标记控件支持**点击涟漪效果**。继承自 `IMotionAwareControl`，额外增加涟漪专属开关。

### 3.4 WaveSpiritAwareControlProperty（共享属性持有者）

```csharp
public abstract class WaveSpiritAwareControlProperty : MotionAwareControlProperty
{
    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        AvaloniaProperty.Register<StyledElement, bool>("IsWaveSpiritEnabled");

    public static readonly StyledProperty<IBrush?> WaveSpiritBrushProperty =
        AvaloniaProperty.Register<StyledElement, IBrush?>("WaveSpiritBrush");

    public static readonly StyledProperty<WaveSpiritType> WaveSpiritTypeProperty =
        AvaloniaProperty.Register<StyledElement, WaveSpiritType>("WaveSpiritType");
}
```

注意 `WaveSpiritAwareControlProperty` **继承自** `MotionAwareControlProperty`，因此通过 `WaveSpiritAwareControlProperty` 可以同时访问 `IsMotionEnabledProperty` 和 `IsWaveSpiritEnabledProperty`。

### 3.5 WaveSpiritType 枚举

```csharp
public enum WaveSpiritType
{
    RoundRectWave,   // 圆角矩形涟漪（按钮、复选框等）
    CircleWave,      // 圆形涟漪（圆形按钮）
    PillWave         // 药丸形涟漪（Round 形状按钮）
}
```

---

## 4. 全局 Token 控制

### 4.1 Seed Token

| Seed Token | 默认值 | 说明 |
|---|---|---|
| `EnableMotion` | `true` | 全局动画开关，控制所有过渡动画和关键帧动画 |
| `EnableWaveSpirit` | `true` | 全局涟漪开关，控制所有点击涟漪效果 |

### 4.2 ThemeManager 运行时控制

`ThemeManager` 提供运行时切换能力。推荐通过 `Application` 扩展方法调用：

```csharp
var application = Application.Current!;

// 全局关闭所有动画
application.SetMotionEnabled(false);

// 仅关闭涟漪效果，保留过渡动画
application.SetWaveSpiritEnabled(false);
```

修改后，所有实现了相应接口的控件会通过 Token 资源绑定自动响应。

> 详细的全局开关使用方法、传播机制和完整示例请参阅 [全局动画与涟漪效果开关使用指南](./GlobalMotionControlGuide.md)。

### 4.3 自动绑定机制

控件通过扩展方法将属性绑定到全局 Token 资源：

**仅动画控制**（`IMotionAwareControl`）：

```csharp
public static void ConfigureMotionBindingStyle(this IMotionAwareControl motionAwareControl)
{
    if (motionAwareControl is StyledElement styledElement)
    {
        var bindingStyle = new Style();
        bindingStyle.Add(MotionAwareControlProperty.IsMotionEnabledProperty, SharedTokenKind.EnableMotion);
        styledElement.Styles.Add(bindingStyle);
    }
}
```

**动画 + 涟漪控制**（`IWaveSpiritAwareControl`）：

```csharp
public static void ConfigureWaveSpiritBindingStyle(this IWaveSpiritAwareControl waveSpiritAwareControl)
{
    if (waveSpiritAwareControl is StyledElement styledElement)
    {
        var bindingStyle = new Style();
        bindingStyle.Add(MotionAwareControlProperty.IsMotionEnabledProperty, SharedTokenKind.EnableMotion);
        bindingStyle.Add(WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty, SharedTokenKind.EnableWaveSpirit);
        styledElement.Styles.Add(bindingStyle);
    }
}
```

`ConfigureWaveSpiritBindingStyle` 同时绑定了 `EnableMotion` 和 `EnableWaveSpirit`，因此实现 `IWaveSpiritAwareControl` 的控件只需调用这一个方法。

---

## 5. WaveSpiritDecorator 组件

### 5.1 概述

`WaveSpiritDecorator` 是实现涟漪动画的核心渲染组件，定义在 `AtomUI.Controls/Primitives/WaveSpiritDecorator.cs`（`internal`）。它是一个无模板控件，通过 `Render` 方法直接绘制涟漪波纹。

### 5.2 核心属性

| 属性 | 类型 | 说明 |
|---|---|---|
| `WaveType` | `WaveSpiritType` | 涟漪形状（圆角矩形/圆形/药丸形） |
| `WaveBrush` | `IBrush?` | 涟漪颜色（默认绑定到 `ColorPrimary`） |
| `CornerRadius` | `CornerRadius` | 圆角半径（圆角矩形涟漪使用） |
| `WaveRange` | `double` | 涟漪扩散范围（像素） |
| `OriginOpacity` | `double` | 涟漪起始不透明度 |
| `SizeMotionDuration` | `TimeSpan` | 尺寸动画时长 |
| `OpacityMotionDuration` | `TimeSpan` | 透明度动画时长 |

### 5.3 默认 Token 绑定

`WaveSpiritDecorator` 的默认主题从全局 SharedToken 读取所有参数：

```xml
<ControlTheme TargetType="atom:WaveSpiritDecorator">
    <Setter Property="WaveBrush" Value="{atom:SharedTokenResource ColorPrimary}"/>
    <Setter Property="OriginOpacity" Value="{atom:SharedTokenResource WaveStartOpacity}"/>
    <Setter Property="SizeMotionDuration" Value="{atom:SharedTokenResource MotionDurationSlow}"/>
    <Setter Property="OpacityMotionDuration" Value="{atom:SharedTokenResource MotionDurationSlow}"/>
    <Setter Property="WaveRange" Value="{atom:SharedTokenResource WaveAnimationRange}"/>
</ControlTheme>
```

### 5.4 双动画管线

涟漪效果由两个并行动画组成：

1. **尺寸动画**：涟漪从控件原始尺寸向外扩展 `WaveRange` 像素
2. **透明度动画**：涟漪从 `OriginOpacity` 淡出到 0

两个动画同时播放，使用 `CubicEaseOut` 缓动曲线，完成后涟漪消失。

### 5.5 三种涟漪 Painter

| Painter | WaveSpiritType | 适用场景 |
|---|---|---|
| `RoundRectWavePainter` | `RoundRectWave` | 矩形/圆角矩形控件（按钮、复选框） |
| `PillWavePainter` | `PillWave` | 药丸形控件（Round 按钮） |
| `CircleWavePainter` | `CircleWave` | 圆形控件（Circle 按钮） |

---

## 6. 动画控制的工作方式

### 6.1 过渡动画（Transition）

实现 `IMotionAwareControl` 的控件在 `IsMotionEnabled` 变化时动态配置 `Transitions`：

```csharp
// 来自 Button.cs 的典型模式
private void ConfigureTransitions(bool force)
{
    if (IsMotionEnabled)
    {
        if (force || Transitions == null)
        {
            var transitions = new Transitions();
            transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
            transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
            Transitions = transitions;
        }
    }
    else
    {
        Transitions = null;  // ← 关闭动画时移除所有过渡
    }
}
```

### 6.2 涟漪效果触发

实现 `IWaveSpiritAwareControl` 的控件在用户交互时触发涟漪：

```csharp
// Button —— 在按钮释放时触发涟漪
if (change.Property == IsPressedProperty)
{
    if (!IsLoading && IsWaveSpiritEnabled &&
        (change.OldValue as bool? == true))  // ← pressed → released
    {
        Dispatcher.UIThread.Post(() =>
        {
            _waveSpiritDecorator?.Play();
        });
    }
}

// ToggleSwitch —— 在选中状态切换时触发涟漪
if (change.Property == IsCheckedProperty && IsMotionEnabled)
{
    _waveSpiritDecorator?.Play();
}
```

---

## 7. Form 级联传播

### 7.1 IsMotionEnabled 级联链

```
ThemeManager ─── EnableMotion (Seed Token)
    ↓ 自动绑定
Form ─── IsMotionEnabled ──→ FormItem ─── IsMotionEnabled ──→ Content (子控件)
```

FormItem 中的级联代码：

```csharp
if (Content is IMotionAwareControl)
{
    _disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, Content, IsMotionEnabledProperty));
}
```

### 7.2 IsWaveSpiritEnabled 不通过 Form 级联

`IsWaveSpiritEnabled` 没有通过 FormItem 级联，因为：

- Form 内的输入控件（LineEdit、Select 等）通常不需要涟漪效果
- 涟漪效果主要用于按钮、复选框、开关等可点击控件
- 这些控件通过全局 `EnableWaveSpirit` Token 自动绑定，无需 Form 传播

---

## 8. 已实现控件清单

### 8.1 实现 IWaveSpiritAwareControl 的控件（动画 + 涟漪）

这些控件同时支持过渡动画控制和点击涟漪效果：

| 控件 | 项目 | 涟漪形状 | 触发时机 |
|---|---|---|---|
| `Button` | Desktop.Controls | 根据 Shape 自动选择 | 按钮释放时 |
| `SplitButton` | Desktop.Controls | RoundRectWave | 按钮释放时 |
| `AbstractCheckBox` | Controls | RoundRectWave | 选中状态切换时 |
| `AbstractRadioButton` | Controls | CircleWave | 选中时 |
| `AbstractToggleSwitch` | Controls | RoundRectWave | 切换时 |
| `AbstractOptionButtonGroup` | Controls | RoundRectWave | 选项切换时 |

### 8.2 仅实现 IMotionAwareControl 的控件（仅动画）

这些控件仅支持过渡动画控制，不包含涟漪效果：

| 控件 | 项目 |
|---|---|
| `TextBox`、`TextArea` | Desktop.Controls |
| `ComboBox`、`AbstractSelect` | Desktop.Controls |
| `Mentions` | Desktop.Controls |
| `ListBox` | Desktop.Controls |
| `NavMenu` | Desktop.Controls |
| `Dialog`、`DialogHost` | Desktop.Controls |
| `Upload` | Desktop.Controls |
| `AbstractTransfer` | Desktop.Controls |
| `WindowNotificationManager` | Desktop.Controls |
| `WindowMessageManager` | Desktop.Controls |
| `AddOnDecoratedBox` | Desktop.Controls |
| `InfoPickerInput` | Desktop.Controls |
| `AbstractColorPicker` | Desktop.Controls.ColorPicker |

---

## 9. 设计优势分析

### 9.1 接口继承的自然层级

`IWaveSpiritAwareControl : IMotionAwareControl` 的继承关系意味着：

- 类型检查 `is IMotionAwareControl` 可以匹配两类控件
- Form 的 `RelayBind` 只需检查 `IMotionAwareControl` 即可覆盖所有需要动画控制的控件
- 涟漪控件自动获得动画开关能力，无需重复实现

### 9.2 全局一键控制

通过修改两个 Seed Token 即可影响整个应用：

- 无障碍模式：`EnableMotion = false` → 所有动画立即停止
- 性能优化：低端设备上关闭动画以提升性能
- 用户偏好：允许用户在设置中自行选择

### 9.3 Token 自动绑定

扩展方法 `ConfigureMotionBindingStyle` / `ConfigureWaveSpiritBindingStyle` 将属性绑定到全局 Token 资源。当 `ThemeManager.IsMotionEnabled` 变化时，所有控件自动响应，无需逐个通知。

### 9.4 涟漪参数完全由 Token 驱动

涟漪的颜色、不透明度、扩散范围、动画时长全部从 SharedToken 读取，确保：

- 切换主题时涟漪表现自动适配
- 所有控件的涟漪参数全局一致
- 可通过自定义 Token 调整涟漪表现

---

## 10. 涟漪形状与控件形态的对应关系

Button 控件根据 `Shape` 属性自动选择合适的涟漪形状：

| ButtonShape | WaveSpiritType | 视觉效果 |
|---|---|---|
| `Default` | `RoundRectWave` | 圆角矩形波纹，匹配按钮的圆角 |
| `Round` | `PillWave` | 药丸形波纹，匹配胶囊形按钮 |
| `Circle` | `CircleWave` | 圆形波纹，匹配圆形按钮 |

```csharp
private void ConfigureWaveSpiritType()
{
    WaveSpiritType waveType = Shape switch
    {
        ButtonShape.Default => WaveSpiritType.RoundRectWave,
        ButtonShape.Round   => WaveSpiritType.PillWave,
        ButtonShape.Circle  => WaveSpiritType.CircleWave,
        _                   => WaveSpiritType.RoundRectWave
    };
    WaveSpiritType = waveType;
}
```

涟漪形状在 AXAML 模板中通过 `{TemplateBinding WaveSpiritType}` 传递给 `WaveSpiritDecorator`：

```xml
<atom:WaveSpiritDecorator Name="PART_WaveSpirit"
                           CornerRadius="{TemplateBinding EffectiveCornerRadius}"
                           WaveType="{TemplateBinding WaveSpiritType}" />
```

