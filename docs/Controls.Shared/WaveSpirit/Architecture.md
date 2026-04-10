# AtomUI WaveSpirit 系统 — 架构设计

> 本文档描述 AtomUI 波浪动画系统的内部架构，包括分层职责、核心类型定义、Painter 绘制机制、动画管线及 Token 集成方式。

---

## 1. 架构总览

WaveSpirit 系统由三层组件协作实现，严格遵循 AtomUI 的分层架构：

```
┌──────────────────────────────────────────────────────────────────────────────┐
│  平台控件层 (AtomUI.Desktop.Controls)                                        │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  WaveSpiritDecoratorTheme.axaml                                        │  │
│  │  → 将 Token 绑定到 WaveSpiritDecorator 的属性                           │  │
│  │  → WaveBrush={SharedTokenResource ColorPrimary}                        │  │
│  │  → OriginOpacity={SharedTokenResource WaveStartOpacity}                │  │
│  │  → WaveRange={SharedTokenResource WaveAnimationRange}                  │  │
│  │  → SizeMotionDuration/OpacityMotionDuration={SharedTokenResource ...}  │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  各控件主题 (ButtonTheme.axaml, CheckBoxIndicatorTheme.axaml, ...)     │  │
│  │  → 在控件模板 <Panel> 中放置 <atom:WaveSpiritDecorator>                 │  │
│  │  → 通过 Style Selector 设置 WaveType                                   │  │
│  │  → Setter: IsWaveSpiritEnabled={SharedTokenResource EnableWaveSpirit}  │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  各控件实现 (Button.cs 等)                                              │  │
│  │  → 实现 IWaveSpiritAwareControl                                        │  │
│  │  → 在交互事件中调用 _waveSpiritDecorator.Play()                         │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
├──────────────────────────────────────────────────────────────────────────────┤
│  基础控件层 (AtomUI.Controls)                                                │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  WaveSpiritDecorator (internal)                                        │  │
│  │  → 管理 Painter 生命周期                                                │  │
│  │  → 构建和运行 Size/Opacity 两条动画管线                                  │  │
│  │  → 在 Render 中调用 Painter.Paint() 绘制波纹                            │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
├──────────────────────────────────────────────────────────────────────────────┤
│  基础共享层 (AtomUI.Controls.Shared)                                         │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  IWaveSpiritAwareControl 接口                                          │  │
│  │  WaveSpiritAwareControlProperty 共享属性                                │  │
│  │  WaveSpiritType 枚举                                                   │  │
│  │  IWaveSpiritPainter 接口                                               │  │
│  │  AbstractWavePainter 基类                                               │  │
│  │  ├── RoundRectWavePainter                                              │  │
│  │  ├── CircleWavePainter                                                 │  │
│  │  └── PillWavePainter                                                   │  │
│  │  WaveSpiritAwareControlExtensions 扩展方法                              │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
├─────────────��────────────────────────────────────────────────────────────────┤
│  核心层 (AtomUI.Core)                                                        │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │  DesignToken: EnableWaveSpirit (Seed), WaveAnimationRange,             │  │
│  │              WaveStartOpacity (Alias)                                  │  │
│  │  ThemeManager: IsWaveSpiritEnabled 属性 + ConfigureEnableWaveSpirit()  │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. 核心类型定义

### 2.1 接口与枚举（`AtomUI.Controls.Shared/WaveSpirit/`）

#### `WaveSpiritType` 枚举

```csharp
public enum WaveSpiritType
{
    RoundRectWave,   // 圆角矩形波纹
    CircleWave,      // 圆形波纹
    PillWave         // 药丸形波纹
}
```

决定波纹的几何形状，由控件根据自身外观选择合适的类型。

#### `IWaveSpiritAwareControl` 接口

```csharp
public interface IWaveSpiritAwareControl : IMotionAwareControl
{
    bool IsWaveSpiritEnabled { get; }
}
```

实现此接口的控件声明自己支持波浪动画。接口继承 `IMotionAwareControl`，表明波浪动画是动画系统的子集——只有在动画总开关打开时才有意义。

#### `WaveSpiritAwareControlProperty` 共享属性

```csharp
public abstract class WaveSpiritAwareControlProperty : MotionAwareControlProperty
{
    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty = ...;
    public static readonly StyledProperty<IBrush?> WaveSpiritBrushProperty = ...;
    public static readonly StyledProperty<WaveSpiritType> WaveSpiritTypeProperty = ...;
}
```

遵循 AtomUI 的 `AddOwner` 共享属性模式，供所有支持波浪动画的控件复用同一 `StyledProperty` 定义。继承自 `MotionAwareControlProperty`，同时携带 `IsMotionEnabled` 属性。

三个共享属性：

| 属性 | 类型 | 说明 |
|---|---|---|
| `IsWaveSpiritEnabledProperty` | `bool` | 是否启用波浪动画 |
| `WaveSpiritBrushProperty` | `IBrush?` | 波纹颜色画刷 |
| `WaveSpiritTypeProperty` | `WaveSpiritType` | 波纹几何形状类型 |

#### `WaveSpiritAwareControlExtensions` 扩展方法

```csharp
public static class WaveSpiritAwareControlExtensions
{
    public static void ConfigureWaveSpiritBindingStyle(
        this IWaveSpiritAwareControl waveSpiritAwareControl)
    {
        if (waveSpiritAwareControl is StyledElement styledElement)
        {
            var bindingStyle = new Style();
            bindingStyle.Add(MotionAwareControlProperty.IsMotionEnabledProperty,
                             SharedTokenKind.EnableMotion);
            bindingStyle.Add(WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty,
                             SharedTokenKind.EnableWaveSpirit);
            styledElement.Styles.Add(bindingStyle);
        }
    }
}
```

便捷扩展方法，一次性将 `EnableMotion` 和 `EnableWaveSpirit` 两个全局 Token 绑定到控件的对应属性。适用于不通过 AXAML 主题设置这些属性的场景。

### 2.2 Painter 绘制器（`AtomUI.Controls.Shared/WaveSpirit/`）

#### `IWaveSpiritPainter` 接口（internal）

```csharp
internal interface IWaveSpiritPainter
{
    WaveSpiritType WaveType { get; }
    void Paint(DrawingContext context, object size, double opacity);
    AbstractWavePainter Clone();
    void NotifyBuildSizeAnimation(Animation animation, AvaloniaProperty targetProperty);
    void NotifyBuildOpacityAnimation(Animation animation, AvaloniaProperty targetProperty);
}
```

定义了 Painter 的核心契约：

- `Paint()`：在给定的 `DrawingContext` 上绘制当前帧的波纹图形。
- `NotifyBuildSizeAnimation()`：为尺寸变化构建关键帧动画。
- `NotifyBuildOpacityAnimation()`：为透明度变化构建关键帧动画。

#### `AbstractWavePainter` 抽象基类（internal）

```csharp
internal abstract class AbstractWavePainter : IWaveSpiritPainter
{
    public Point OriginPoint { get; set; }         // 波纹原点
    public IBrush? WaveBrush { get; set; }         // 波纹画刷
    public WaveSpiritType WaveType { get; }        // 波纹类型
    public double WaveRange { get; set; }          // 波纹扩散范围
    public TimeSpan SizeMotionDuration { get; set; }    // 尺寸动画时长
    public TimeSpan OpacityMotionDuration { get; set; }  // 透明度动画时长
    public Easing? SizeEasingCurve { get; set; }        // 尺寸缓动曲线
    public Easing? OpacityEasingCurve { get; set; }     // 透明度缓动曲线
    public double OriginOpacity { get; set; }            // 初始透明度
}
```

提供了所有 Painter 的公共配置参数，并实现了 `NotifyBuildOpacityAnimation()` 的默认逻辑（从 `OriginOpacity` 到 `0` 的淡出动画）。

#### 三种 Painter 实现

| Painter | 波纹类型 | 尺寸参数 | 绘制原理 |
|---|---|---|---|
| `RoundRectWavePainter` | `RoundRectWave` | `OriginSize` (Size), `CornerRadius` | 用两个圆角矩形做差集（`CombinedGeometry.Exclude`），外矩形随动画扩大，内矩形固定，形成环状波纹 |
| `CircleWavePainter` | `CircleWave` | `OriginRadius` (double) | 用两个椭圆做差集，外圆随动画扩大，内圆固定 |
| `PillWavePainter` | `PillWave` | `OriginSize` (Size) | 类似 RoundRectWave，但圆角半径固定为高度的一半（药丸形） |

**绘制原理图**：

```
RoundRectWave                  CircleWave              PillWave
┌───────────────────┐         ╭─────────╮          ╭───────────────╮
│ ┌───────────────┐ │         │ ╭─────╮ │          │ ╭───────────╮ │
│ │               │ │         │ │     │ │          │ │           │ │
│ │   原始控件     │ │         │ │     │ │          │ │  原始控件  │ │
│ │   区域        │ │         │ │     │ │          │ │  区域      │ │
│ │               │ │         │ ╰─────╯ │          │ ╰───────────╯ │
│ └───────────────┘ │         ╰─────────╯          ╰───────────────╯
└───────────────────┘         ↑ 波纹区域             ↑ 波纹区域
↑ 波纹区域 = 外矩形 - 内矩形     = 外圆 - 内圆         = 外药丸 - 内药丸
```

#### RoundRectWavePainter 的圆角缩放

`RoundRectWavePainter` 在波纹扩散时会按比例缩放圆角半径，确保圆角不会在扩大过程中失真：

```csharp
var salt = deltaSize.Width / WaveRange / 2 + 1;
var currentCornerRadius = new CornerRadius(
    CornerRadius.TopLeft * salt,
    CornerRadius.TopRight * salt,
    CornerRadius.BottomRight * salt,
    CornerRadius.BottomLeft * salt);
```

同时，对于非均匀圆角（四个角半径不同），使用 `RoundRectGeometryBuilder` 生成精确的 `StreamGeometry`，确保非对称圆角的正确渲染。

---

## 3. WaveSpiritDecorator（动画协调器）

`WaveSpiritDecorator` 是 WaveSpirit 系统的核心协调组件，定义在 `AtomUI.Controls/Primitives/` 中，是一个 `internal` 的 Avalonia `Control`。

### 3.1 职责

1. **Painter 生命周期管理**：根据 `WaveType` 属性自动创建对应的 `AbstractWavePainter` 实例。
2. **动画构建与播放**：在 `Play()` 调用时，构建尺寸和透明度两条并行动画，驱动 `LastWaveSize` / `LastWaveRadius` 和 `LastWaveOpacity` 属性变化。
3. **渲染管线**：在 `Render()` 中，将当前动画帧的尺寸和透明度传递给 Painter 进行绘制。
4. **属性同步**：监听外部属性变化（`WaveBrush`、`CornerRadius`、`WaveType` 等），及时更新 Painter 配置。

### 3.2 StyledProperty 定义

```csharp
internal class WaveSpiritDecorator : Control
{
    public const string WaveSpiritPart = "PART_WaveSpirit";  // 统一模板部件名称

    // 公共属性（由主题 AXAML 设置）
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty = ...;
    public static readonly StyledProperty<TimeSpan> SizeMotionDurationProperty = ...;
    public static readonly StyledProperty<TimeSpan> OpacityMotionDurationProperty = ...;
    public static readonly StyledProperty<Easing> SizeEasingCurveProperty = ...;
    public static readonly StyledProperty<Easing> OpacityEasingCurveProperty = ...;
    public static readonly StyledProperty<double> OriginOpacityProperty = ...;
    public static readonly StyledProperty<double> WaveRangeProperty = ...;
    public static readonly StyledProperty<IBrush?> WaveBrushProperty = ...;
    public static readonly StyledProperty<WaveSpiritType> WaveTypeProperty = ...;

    // 内部动画驱动属性（DirectProperty）
    protected static readonly DirectProperty<..., double> LastWaveOpacityProperty = ...;
    protected static readonly DirectProperty<..., Size> LastWaveSizeProperty = ...;
    protected static readonly DirectProperty<..., double> LastWaveRadiusProperty = ...;
}
```

**设计要点**：

- 公共属性通过主题 AXAML 绑定 Token 值。
- 内部 `DirectProperty` 用于动画系统驱动——Avalonia 的 `Animation` 通过设置这些属性来驱动每帧渲染。
- `AffectsRender` 注册了三个内部属性，确保属性变化时自动触发 `Render()`。

### 3.3 动画管线

`WaveSpiritDecorator` 的 `Play()` 方法实现了双轨并行动画：

```
用户点击控件
    │
    ▼
控件调用 _waveSpiritDecorator.Play()
    │
    ▼
BuildWavePainter() — 按 WaveType 创建/复用 Painter
    │
    ▼
Painter.NotifyBuildSizeAnimation()  ──► sizeAnimation (KeyFrame 动画)
Painter.NotifyBuildOpacityAnimation() ─► opacityAnimation (KeyFrame 动画)
    │
    ▼
sizeAnimation.RunAsync()    ←── 并行运行 ──→  opacityAnimation.RunAsync()
    │                                              │
    ▼                                              ▼
设置 LastWaveSize/LastWaveRadius               设置 LastWaveOpacity
    │                                              │
    ├──────────── AffectsRender ───────────────────┤
    │
    ▼
Render() → Painter.Paint(context, currentSize, currentOpacity)
    │
    ▼
绘制当前帧波纹图形（CombinedGeometry 差集）
```

**关键实现细节**：

```csharp
public void Play()
{
    if (_isPlaying || !this.IsAttachedToVisualTree()) return;
    if (SizeMotionDuration <= TimeSpan.Zero || OpacityMotionDuration <= TimeSpan.Zero) return;

    BuildWavePainter(false);

    var sizeAnimation    = new Animation();
    var opacityAnimation = new Animation();

    // 根据波纹类型选择驱动属性
    AvaloniaProperty targetProperty = _wavePainter.WaveType == WaveSpiritType.CircleWave
        ? LastWaveRadiusProperty    // 圆形波纹驱动半径
        : LastWaveSizeProperty;     // 矩形/药丸波纹驱动尺寸

    _wavePainter.NotifyBuildSizeAnimation(sizeAnimation, targetProperty);
    _wavePainter.NotifyBuildOpacityAnimation(opacityAnimation, LastWaveOpacityProperty);

    // 取消前一次动画，并行启动新的双轨动画
    _cancellationTokenSource?.Cancel();
    _cancellationTokenSource = new CancellationTokenSource();

    var sizeTask    = sizeAnimation.RunAsync(this, _cancellationTokenSource.Token);
    var opacityTask = opacityAnimation.RunAsync(this, _cancellationTokenSource.Token);
    _isPlaying = true;

    Dispatcher.UIThread.InvokeAsync(async () =>
    {
        await Task.WhenAll(sizeTask, opacityTask);
        _isPlaying = false;
    });
}
```

### 3.4 Painter 配置同步

当外部属性发生变化时，`WaveSpiritDecorator` 自动同步到 Painter：

```csharp
private void ConfigureWavePainter()
{
    if (_wavePainter != null)
    {
        if (_wavePainter is RoundRectWavePainter roundWavePainter)
        {
            roundWavePainter.CornerRadius = CornerRadius;
        }
        _wavePainter.SizeEasingCurve       = new CubicEaseOut();
        _wavePainter.OpacityEasingCurve    = new CubicEaseOut();
        _wavePainter.OriginOpacity         = Math.Clamp(OriginOpacity, 0.0, 1.0);
        _wavePainter.SizeMotionDuration    = SizeMotionDuration;
        _wavePainter.OpacityMotionDuration = OpacityMotionDuration.Add(TimeSpan.FromMilliseconds(50));
        _wavePainter.WaveRange             = Math.Min(WaveRange, 8);
        _wavePainter.WaveBrush             = WaveBrush;
    }
}
```

**注意**：
- 透明度动画时长额外增加 50ms，使波纹淡出效果稍晚于尺寸扩散完成，视觉上更自然。
- `WaveRange` 上限被限制为 8 像素，防止波纹过大。
- 缓动曲线统一使用 `CubicEaseOut`（三次方减速曲线），与 Ant Design 的缓动效果一致。

---

## 4. Token 集成

### 4.1 全局开关

WaveSpirit 的全局开关通过 Token 三层推导链路传递：

```
DesignToken.EnableWaveSpirit (Seed Token, 默认 true)
    │
    ▼
ThemeManager.IsWaveSpiritEnabled (运行时属性)
    │
    ▼
SharedTokenKind.EnableWaveSpirit (ResourceDictionary 资源)
    │
    ▼
控件 IsWaveSpiritEnabled 属性 (通过 {atom:SharedTokenResource EnableWaveSpirit} 绑定)
```

`ThemeManager` 管理运行时切换：

```csharp
// ThemeManager.cs
public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty = ...;

public bool IsWaveSpiritEnabled
{
    get => GetValue(IsWaveSpiritEnabledProperty);
    set => SetValue(IsWaveSpiritEnabledProperty, value);
}

private void ConfigureEnableWaveSpirit()
{
    var themeResource = Resources.ThemeDictionaries[ThemeVariant];
    if (themeResource is ResourceDictionary globalResourceDictionary)
    {
        globalResourceDictionary[SharedTokenKind.EnableWaveSpirit] = IsWaveSpiritEnabled;
    }
}
```

### 4.2 WaveSpiritDecorator 默认主题

平台层定义了 `WaveSpiritDecorator` 的 ControlTheme，将 Token 绑定到装饰器属性：

```xml
<!-- AtomUI.Desktop.Controls/Primitives/Themes/WaveSpiritDecoratorTheme.axaml -->
<ControlTheme x:Key="{x:Type atom:WaveSpiritDecorator}"
              TargetType="atom:WaveSpiritDecorator">
    <Setter Property="WaveBrush"
            Value="{atom:SharedTokenResource ColorPrimary}"/>
    <Setter Property="OriginOpacity"
            Value="{atom:SharedTokenResource WaveStartOpacity}"/>
    <Setter Property="SizeMotionDuration"
            Value="{atom:SharedTokenResource MotionDurationSlow}"/>
    <Setter Property="OpacityMotionDuration"
            Value="{atom:SharedTokenResource MotionDurationSlow}"/>
    <Setter Property="WaveRange"
            Value="{atom:SharedTokenResource WaveAnimationRange}"/>
</ControlTheme>
```

这意味着放置在模板中的 `WaveSpiritDecorator` 会自动获得全局主题色、动画时长和波纹范围，无需手动配置。

### 4.3 Alias Token 计算

`WaveAnimationRange` 和 `WaveStartOpacity` 是 Alias Token，由 `DesignToken.CalculateAliasTokenValues()` 计算：

```csharp
// DesignToken.cs
WaveAnimationRange = LineWidth * 6;   // 默认 LineWidth=1，范围=6px
WaveStartOpacity   = 0.4;             // 初始透明度 40%
```

---

## 5. 数据流总图

```
用户点击 Button                   ThemeManager
    │                                │
    ▼                                ▼
Button.OnPropertyChanged         SharedTokenKind.EnableWaveSpirit
(IsPressedProperty: true→false)      │
    │                                ▼
    │ 检查:                    控件 IsWaveSpiritEnabled
    │  !IsLoading                    │
    │  IsWaveSpiritEnabled ──────────┘
    │  ButtonType ∈ {Primary,Default,Dashed}
    │
    ▼
[可选] 覆盖 WaveBrush（IsDanger 时使用 Foreground/Background）
    │
    ▼
_waveSpiritDecorator.Play()
    │
    ▼
┌─────────────────────────────────────────┐
│ WaveSpiritDecorator                     │
│                                         │
│  BuildWavePainter()                     │
│    → RoundRectWavePainter               │
│    / CircleWavePainter                  │
│    / PillWavePainter                    │
│                                         │
│  构建 sizeAnimation + opacityAnimation  │
│    → 关键帧: 原始尺寸 → 原始+WaveRange  │
│    → 关键帧: OriginOpacity → 0          │
│                                         │
│  并行运行两条动画                        │
│    → 驱动 LastWaveSize + LastWaveOpacity │
│                                         │
│  Render() 每帧调用                       │
│    → Painter.Paint(ctx, size, opacity)  │
│    → CombinedGeometry(Exclude) 差集绘制  │
│                                         │
│  动画完成 → _isPlaying = false           │
└─────────────────────────────────────────┘
```

---

## 6. 文件索引

### 基础共享层（`AtomUI.Controls.Shared/WaveSpirit/`）

| 文件 | 类型 | 职责 |
|---|---|---|
| `WaveSpiritType.cs` | `WaveSpiritType` 枚举 | 定义三种波纹几何形状 |
| `IWaveSpiritAware.cs` | `IWaveSpiritAwareControl` 接口 + `WaveSpiritAwareControlProperty` | 波浪感知接口和共享属性定义 |
| `IWaveSpiritPainter.cs` | `IWaveSpiritPainter` 接口 | Painter 契约定义 |
| `AbstractWavePainter.cs` | `AbstractWavePainter` 抽象类 | Painter 基类，公共配置和默认透明度动画 |
| `RoundRectWavePainter.cs` | `RoundRectWavePainter` | 圆角矩形波纹绘制器 |
| `CircleWavePainter.cs` | `CircleWavePainter` | 圆形波纹绘制器 |
| `PillWavePainter.cs` | `PillWavePainter` | 药丸形波纹绘制器 |
| `WaveSpiritAwareControlExtensions.cs` | 扩展方法 | 便捷 Token 绑定 |

### 基础控件层（`AtomUI.Controls/Primitives/`）

| 文件 | 类型 | 职责 |
|---|---|---|
| `WaveSpiritDecorator.cs` | `WaveSpiritDecorator` | 核心协调组件：Painter 管理、动画管线、渲染 |

### 平台层（`AtomUI.Desktop.Controls/`）

| 文件 | 类型 | 职责 |
|---|---|---|
| `Primitives/Themes/WaveSpiritDecoratorTheme.axaml` | ControlTheme | 将全局 Token 绑定到 WaveSpiritDecorator 属性 |
| `Buttons/Themes/ButtonTheme.axaml` | ControlTheme | Button 模板中放置 WaveSpiritDecorator |
| `CheckBox/Themes/CheckBoxIndicatorTheme.axaml` | ControlTheme | CheckBox 指示器模板中放置 WaveSpiritDecorator |
| `RadioButton/Themes/RadioIndicatorTheme.axaml` | ControlTheme | RadioButton 指示器模板中放置 WaveSpiritDecorator |
| `Switch/Themes/ToggleSwitchTheme.axaml` | ControlTheme | ToggleSwitch 模板中放置 WaveSpiritDecorator |
| `OptionButtonGroup/Themes/OptionButtonTheme.axaml` | ControlTheme | OptionButton 模板中放置 WaveSpiritDecorator |

### 核心层（`AtomUI.Core/`）

| 文件 | 类型 | 职责 |
|---|---|---|
| `Theme/TokenSystem/TokenDefinitions/DesignToken.Seed.cs` | `EnableWaveSpirit` Seed Token | 全局波浪动画开关 |
| `Theme/TokenSystem/TokenDefinitions/DesignToken.Alias.cs` | `WaveAnimationRange`, `WaveStartOpacity` | 波纹范围和初始透明度 |
| `Theme/TokenSystem/TokenDefinitions/DesignToken.cs` | Alias 计算 | `WaveAnimationRange = LineWidth * 6` 等 |
| `Theme/ThemeManager.cs` | `IsWaveSpiritEnabled` 属性 | 运行时全局开关管理 |

