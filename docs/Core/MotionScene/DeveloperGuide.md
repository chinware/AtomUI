# AtomUI 动效系统开发者指南

> 本文档面向 AtomUI 框架开发者和控件库使用者，详细说明如何使用预置动效、创建自定义动效、在控件中集成动效系统，以及最佳实践。所有动效设计均遵循 [Ant Design 动效三原则](https://ant.design/docs/spec/motion)：**自然**（Natural）、**高效**（Performant）、**克制**（Concise）。

---

## 0. 动效选择指南

在为控件选择动效之前，请先回答以下问题（基于 Ant Design 动效设计原则）：

### 0.1 是否需要动效？

遵循 **克制** 原则 — 只在有明确目的时添加动效：

| 场景 | 是否需要动效 | 理由 |
|------|------------|------|
| 弹层出现/消失 | ✅ 需要 | 描述层级关系，建立空间关联 |
| 列表项增删 | ✅ 需要 | 解释发生了什么，保持上下文 |
| 按钮点击反馈 | ✅ 需要 | 提供反馈，确认用户操作 |
| 普通文本更新 | ❌ 不需要 | 没有空间变化，动效无意义 |
| 高频数据刷新 | ❌ 不需要 | 影响性能，干扰用户 |

### 0.2 选择哪种动效？

根据 Ant Design [过渡设计模式](https://ant.design/docs/spec/transition) 选择：

| 交互场景 | 推荐动效 | Ant Design 对应 | 设计意图 |
|---------|---------|-----------------|---------|
| 浮层从触发点弹出 | `ZoomBigIn/Out` | `zoom-big` | 描述层级关系（从触发点缩放） |
| 面板从边缘展开 | `ZoomUp/Down/Left/RightIn/Out` | `zoom-up/down/left/right` | 保持上下文（方向性缩放） |
| 内容区域展开/折叠 | `SlideUpIn/Out` 或 `Expand/Collapse` | `slide-up` / `ant-motion-collapse` | 保持上下文（空间连续性） |
| 元素从屏幕外滑入 | `MoveUp/Down/Left/RightIn/Out` | `move-*` | 描述空间位置关系 |
| 简单的显示/隐藏 | `FadeIn/Out` | `fade` | 最小化干扰 |

### 0.3 选择什么缓动曲线？

遵循 Ant Design 的 **高效** 原则和 **不对称进出场** 规律：

```
进入 (In)  → 使用 Ease-Out 曲线（快速出现 → 缓慢到位）— 物体"跃入"
退出 (Out) → 使用 Ease-In 曲线（缓慢启动 → 快速消失）— 物体"收回"
```

| 动效类别 | 进入缓动 | 退出缓动 | 对应 Ant Design Token |
|---------|---------|---------|---------------------|
| Zoom | `CircularEaseOut` | `CircularEaseInOut` | `motionEaseOutCirc` / `motionEaseInOutCirc` |
| Slide | `QuinticEaseOut` | `QuinticEaseIn` | `motionEaseOutQuint` / `motionEaseInQuint` |
| Move | `CircularEaseOut` | `CircularEaseInOut` | `motionEaseOutCirc` / `motionEaseInOutCirc` |
| Collapse | `CubicEaseInOut` | `CubicEaseInOut` | `motionEaseInOut` |
| Fade | `LinearEasing` | `LinearEasing` | `linear` |

---

## 1. 使用预置动效

### 1.1 基本用法：在控件中播放动效

最简单的场景 — 在一个已有的 `MotionActor` 上播放预置动效：

```csharp
// 1. 获取 MotionActor（通常来自 ControlTheme 模板）
var actor = this.FindDescendantOfType<MotionActor>();

// 2. 创建 Motion
var motion = new ZoomBigInMotion(
    duration: TimeSpan.FromMilliseconds(200),
    easing: new CircularEaseOut()
);

// 3. 播放
motion.Run(actor,
    aboutToStart: () => { /* 动画即将开始 */ },
    completedAction: () => { /* 动画已完成 */ }
);
```

### 1.2 异步用法

```csharp
var motion = new FadeInMotion(TimeSpan.FromMilliseconds(300));
await motion.RunAsync(actor, aboutToStart: () => actor.IsVisible = true);
// 到达这里时动画已完成
```

### 1.3 在 ControlTheme 中放置 MotionActor

在 AXAML ControlTheme 模板中，将需要动画的内容包裹在 `MotionActor` 中：

```xml
<ControlTheme x:Class="AtomUI.Desktop.Controls.Themes.MyControlTheme"
              TargetType="atom:MyControl">
    <Setter Property="Template">
        <ControlTemplate>
            <atomp:MotionActor x:Name="PART_MotionActor">
                <!-- 被动画的内容 -->
                <Border Background="{TemplateBinding Background}">
                    <ContentPresenter Content="{TemplateBinding Content}" />
                </Border>
            </atomp:MotionActor>
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

> **命名空间**: `atomp:` 对应 `AtomUI.Controls.Primitives`（`MotionActor` 所在命名空间）。

在代码中获取 Actor：

```csharp
protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    _motionActor = e.NameScope.Find<MotionActor>("PART_MotionActor");
}
```

---

## 2. 为控件添加动效支持

### 2.1 实现 IMotionAwareControl

任何需要响应全局动画开关的控件都应实现 `IMotionAwareControl`：

```csharp
using AtomUI.Controls;
using AtomUI.MotionScene;

public class MyControl : TemplatedControl, IMotionAwareControl
{
    // 1. 注册 IsMotionEnabled 属性
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MyControl>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public MyControl()
    {
        // 2. 自动绑定全局 EnableMotion Token
        this.ConfigureMotionBindingStyle();
    }

    private void PlayOpenAnimation()
    {
        // 3. 在播放动画前检查开关
        if (!IsMotionEnabled || _motionActor == null)
        {
            // 跳过动画，直接设置最终状态
            SetFinalState();
            return;
        }

        var motion = new SlideUpInMotion(/* duration */);
        motion.Run(_motionActor, completedAction: () => { /* ... */ });
    }
}
```

### 2.2 使用 MotionDuration 属性

如果控件需要自定义动画时长（从 Token 派生或允许使用者覆盖）：

```csharp
public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
    MotionAwareControlProperty.MotionDurationProperty.AddOwner<MyControl>();

public TimeSpan MotionDuration
{
    get => GetValue(MotionDurationProperty);
    set => SetValue(MotionDurationProperty, value);
}
```

并在 Theme 或 Instance Style 中绑定到 Token：

```csharp
// 在构造函数中
var style = new Style();
style.Add(MotionDurationProperty, SharedTokenKind.MotionDurationMid);
this.Styles.Add(style);
```

### 2.3 暴露可替换的 Motion 属性

允许控件使用者替换默认动效：

```csharp
public static readonly StyledProperty<AbstractMotion?> OpenMotionProperty =
    AvaloniaProperty.Register<MyControl, AbstractMotion?>(nameof(OpenMotion));

public static readonly StyledProperty<AbstractMotion?> CloseMotionProperty =
    AvaloniaProperty.Register<MyControl, AbstractMotion?>(nameof(CloseMotion));

// 使用时
var motion = OpenMotion ?? new ZoomBigInMotion();
motion.Duration = MotionDuration;
motion.Run(_motionActor, ...);
```

使用者可以在 XAML 中替换：

```xml
<atom:MyControl>
    <atom:MyControl.OpenMotion>
        <motionScene:FadeInMotion Duration="0:0:0.5" />
    </atom:MyControl.OpenMotion>
</atom:MyControl>
```

---

## 3. 创建自定义动效

### 3.1 Transition 模式（简单两点过渡）

适用于只有起始和结束两个状态的简单动效：

```csharp
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

public class MyCustomFadeZoomInMotion : AbstractMotion
{
    public MyCustomFadeZoomInMotion(TimeSpan? duration = null, Easing? easing = null)
        : base(duration ?? TimeSpan.FromMilliseconds(250),
               easing ?? new CubicEaseOut(),
               FillMode.Forward)
    {
        // SpiritType 默认为 Transition，无需设置
    }

    protected override void ConfigureTransitions()
    {
        // 调用基类创建 Opacity + MotionTransform 的默认 Transition
        base.ConfigureTransitions();
        // 设置变换原点
        RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.5);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}
```

> **要点**:
> - `base.ConfigureTransitions()` 已经创建了 `NotifiableDoubleTransition`（Opacity）和 `NotifiableTransformOperationsTransition`（MotionTransform），通常无需额外添加。
> - 使用 `BuildScaleTransform()`、`BuildTranslateTransform()` 等辅助方法构造变换。

### 3.2 Animation 模式（多关键帧动画）

适用于需要中间状态的复杂动效：

```csharp
public class MyBounceInMotion : AbstractMotion
{
    public MyBounceInMotion(TimeSpan? duration = null)
        : base(duration ?? TimeSpan.FromMilliseconds(400),
               new CubicEaseOut(),
               FillMode.Forward)
    {
        // 关键：切换到 Animation 驱动模式
        SpiritType = MotionSpiritType.Animation;
    }

    protected override void ConfigureAnimation()
    {
        var animation = CreateAnimation();  // 使用基类辅助方法

        // 关键帧 1: 0% — 初始状态
        var startFrame = new KeyFrame { Cue = new Cue(0.0) };
        startFrame.Setters.Add(new Setter
        {
            Property = Visual.OpacityProperty,
            Value = 0.0
        });
        startFrame.Setters.Add(new Setter
        {
            Property = BaseMotionActor.MotionTransformOperationsProperty,
            Value = BuildScaleTransform(0.3)
        });
        animation.Children.Add(startFrame);

        // 关键帧 2: 60% — 弹性过冲
        var overshootFrame = new KeyFrame { Cue = new Cue(0.6) };
        overshootFrame.Setters.Add(new Setter
        {
            Property = Visual.OpacityProperty,
            Value = 0.8
        });
        overshootFrame.Setters.Add(new Setter
        {
            Property = BaseMotionActor.MotionTransformOperationsProperty,
            Value = BuildScaleTransform(1.1)  // 过冲到 110%
        });
        animation.Children.Add(overshootFrame);

        // 关键帧 3: 100% — 最终状态
        var endFrame = new KeyFrame { Cue = new Cue(1.0) };
        endFrame.Setters.Add(new Setter
        {
            Property = Visual.OpacityProperty,
            Value = 1.0
        });
        endFrame.Setters.Add(new Setter
        {
            Property = BaseMotionActor.MotionTransformOperationsProperty,
            Value = BuildScaleTransform(1.0)
        });
        animation.Children.Add(endFrame);

        // 设置变换原点
        RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

        // 添加到 Animations 列表
        Animations.Add(animation);
    }

    protected override void NotifyPreStart(BaseMotionActor actor)
    {
        base.NotifyPreStart(actor);
        // 在动画开始前设置初始状态
        actor.Opacity = 0.0;
    }
}
```

> **要点**:
> - Animation 模式必须设置 `SpiritType = MotionSpiritType.Animation`。
> - 使用 `CreateAnimation()` 辅助方法创建预配置了 Duration/Easing/FillMode 的 Animation。
> - 关键帧使用 `MotionTransformOperationsProperty`（而不是 `MotionTransformProperty`），因为 Animation 需要 TransformOperations 类型。
> - `NotifyPreStart` 可用于在动画开始前设置初始可见状态。

### 3.3 使用辅助方法构建 Transform

`AbstractMotion` 提供了以下静态辅助方法：

```csharp
// 等比缩放
protected static ITransform BuildScaleTransform(double scale)
protected static ITransform BuildScaleTransform(double scaleX, double scaleY)

// 仅 X 轴缩放
protected static ITransform BuildScaleXTransform(double scale)

// 仅 Y 轴缩放
protected static ITransform BuildScaleYTransform(double scale)

// 位移
protected static ITransform BuildTranslateTransform(double offsetX, double offsetY)

// 缩放 + 位移组合
protected static ITransform BuildTranslateScaleAndTransform(
    double scaleX, double scaleY, double offsetX, double offsetY)
```

### 3.4 为 SceneLayer 计算尺寸

如果自定义动效涉及位移（Translate），需要重写 SceneLayer 尺寸计算方法，确保浮动渲染层有足够空间：

```csharp
internal override Size CalculateSceneSize(Size motionTargetSize)
{
    // 位移动画需要扩大渲染区域
    return motionTargetSize.WithHeight(motionTargetSize.Height * 2);
}

internal override Point CalculateScenePosition(Size motionTargetSize, Point motionTargetPosition)
{
    // 调整渲染层的起始位置
    return motionTargetPosition.WithY(motionTargetPosition.Y - motionTargetSize.Height);
}
```

---

## 4. 使用 Transition 系统（属性过渡）

除了 MotionScene 中的 Motion 动效，AtomUI 还提供了丰富的属性 Transition 工具，用于控件状态变化时的平滑过渡（如 hover 时颜色变化）。

### 4.1 TransitionUtils — Token 感知的 Transition 工厂

```csharp
using AtomUI.Controls;

// 创建一个与 Token 时长绑定的过渡
var transition = TransitionUtils.CreateTransition<DoubleTransition>(
    targetProperty: OpacityProperty,
    durationResourceKey: SharedTokenKind.MotionDurationFast,  // 从 Token 获取时长
    easing: new CubicEaseOut()
);

// 在控件中使用
Transitions = new Transitions
{
    TransitionUtils.CreateTransition<DoubleTransition>(
        OpacityProperty, SharedTokenKind.MotionDurationFast),
    TransitionUtils.CreateTransition<SolidColorBrushTransition>(
        ForegroundProperty, SharedTokenKind.MotionDurationMid),
    TransitionUtils.CreateTransition<BoxShadowsTransition>(
        BoxShadowProperty, SharedTokenKind.MotionDurationFast)
};
```

### 4.2 可用的 Transition 类型

| 类型 | 属性类型 | 来源 |
|------|---------|------|
| `DoubleTransition` | `double` | Avalonia 内置 |
| `ThicknessTransition` | `Thickness` | Avalonia 内置 |
| `CornerRadiusTransition` | `CornerRadius` | Avalonia 内置 |
| `TransformOperationsTransition` | `TransformOperations` | Avalonia 内置 |
| `SolidColorBrushTransition` | `IBrush` | **AtomUI 自定义** — 支持渐变笔刷 |
| `BoxShadowsTransition` | `BoxShadows` | **AtomUI 自定义** — 多阴影插值 |

### 4.3 时长 Token 参考

| Token | 默认值 | 适用场景 |
|-------|-------|---------|
| `SharedTokenKind.MotionDurationFast` | 100ms | 小型元素状态变化（按钮 hover、图标旋转） |
| `SharedTokenKind.MotionDurationMid` | 200ms | 中型元素变化（面板切换、内容过渡） |
| `SharedTokenKind.MotionDurationSlow` | 300ms | 大型面板动画（Drawer、Modal） |
| `SharedTokenKind.MotionDurationVerySlow` | 800ms | 特殊长动画 |

---

## 5. WaveSpirit 点击涟漪效果

> Ant Design 标志性的点击涟漪源自"树叶飘浮在水面"的设计隐喻 — 按下时叶子下沉，释放时反弹，产生涟漪扩散。这是 Ant Design **自然** 原则的标杆实现。原版使用 `box-shadow: 0 0 0 6px currentcolor` + opacity 0.2→0 的 CSS 过渡实现（见 `components/_util/wave/style.ts`，使用 `motionEaseOutCirc` 缓动），AtomUI 使用独立的尺寸 + 不透明度双动画管线忠实复刻。

### 5.1 在 ControlTheme 中添加涟漪

在 AXAML 模板中添加 `WaveSpiritDecorator`：

```xml
<Grid>
    <!-- 主要内容 -->
    <Border x:Name="PART_RootBorder" ...>
        <ContentPresenter ... />
    </Border>
    
    <!-- 涟漪层（覆盖在内容之上） -->
    <atomp:WaveSpiritDecorator x:Name="PART_WaveSpirit" />
</Grid>
```

### 5.2 在代码中触发涟漪

```csharp
private WaveSpiritDecorator? _waveSpiritDecorator;

protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    _waveSpiritDecorator = e.NameScope.Find<WaveSpiritDecorator>("PART_WaveSpirit");
}

protected override void OnClick()
{
    base.OnClick();
    _waveSpiritDecorator?.Play();
}
```

### 5.3 WaveSpirit 类型

| 类型 | 效果 | 适用控件 |
|------|------|---------|
| `RoundRectWave` | 圆角矩形扩散波纹 | Button、Tag 等矩形控件 |
| `PillWave` | 药丸形扩散波纹 | 全圆角按钮 |
| `CircleWave` | 圆形扩散波纹 | CircleButton、Radio |

---

## 6. 常见模式与最佳实践

### 6.1 Popup 开关动画模式

AtomUI 的 `Popup` 类提供了完整的动效感知开关方法：

```csharp
// 使用 MotionAwareOpen/Close 代替原生 Open/Close
popup.MotionAwareOpen(opened: () => { /* Popup 已完全打开 */ });
popup.MotionAwareClose(closed: () => { /* Popup 已完全关闭 */ });

// 异步版本
await popup.MotionAwareOpenAsync();
await popup.MotionAwareCloseAsync();

// 通过属性绑定控制（MVVM 友好）
popup.IsMotionAwareOpen = true;   // 触发 MotionAwareOpen
popup.IsMotionAwareOpen = false;  // 触发 MotionAwareClose
```

### 6.2 折叠/展开动画模式

以 `CollapseItem` 为例：

```csharp
private void ShowExpandContent()
{
    if (!IsMotionEnabled || _contentMotionActor == null) return;
    
    var motion = new SlideUpInMotion(MotionDuration, new CubicEaseOut());
    motion.Run(_contentMotionActor, completedAction: () =>
    {
        // 动画完成后的状态设置
    });
}

private void HideExpandContent()
{
    if (!IsMotionEnabled || _contentMotionActor == null) return;
    
    var motion = new SlideUpOutMotion(MotionDuration, new CubicEaseIn());
    motion.Run(_contentMotionActor, completedAction: () =>
    {
        // 动画完成后隐藏内容
    });
}
```

### 6.3 方向感知动画模式

以 `Drawer` 为例，根据放置方向选择不同 Motion：

```csharp
AbstractMotion motion;
switch (Placement)
{
    case Direction.Left:
        motion = isOpen
            ? new MoveLeftInMotion(actor.DesiredSize.Width, duration, new CubicEaseOut())
            : new MoveLeftOutMotion(actor.DesiredSize.Width, duration, new CubicEaseOut());
        break;
    case Direction.Right:
        motion = isOpen
            ? new MoveRightInMotion(actor.DesiredSize.Width, duration, new CubicEaseOut())
            : new MoveRightOutMotion(actor.DesiredSize.Width, duration, new CubicEaseOut());
        break;
    case Direction.Top:
        motion = isOpen
            ? new MoveUpInMotion(actor.DesiredSize.Height, duration, new CubicEaseOut())
            : new MoveUpOutMotion(actor.DesiredSize.Height, duration, new CubicEaseOut());
        break;
    case Direction.Bottom:
        motion = isOpen
            ? new MoveDownInMotion(actor.DesiredSize.Height, duration, new CubicEaseOut())
            : new MoveDownOutMotion(actor.DesiredSize.Height, duration, new CubicEaseOut());
        break;
}
motion.Run(_motionActor, completedAction: () => { /* ... */ });
```

### 6.4 防止动画状态竞争

当用户快速操作可能导致打开/关闭动画重叠时，使用状态标志保护：

```csharp
private bool _openAnimating;
private bool _closeAnimating;

public void AnimatedOpen()
{
    // 防止重入
    if (_openAnimating || _closeAnimating) return;
    
    _openAnimating = true;
    var motion = new ZoomBigInMotion();
    motion.Run(_motionActor, completedAction: () =>
    {
        _openAnimating = false;
    });
}
```

Popup 类中还实现了更复杂的 "关闭请求排队" 模式：

```csharp
// 当打开动画未完成时收到关闭请求
if (_openAnimating)
{
    RequestCloseWhereAnimationCompleted = true;  // 标记
    return;
}

// 在打开动画完成回调中检查
completedAction: () =>
{
    _openAnimating = false;
    if (RequestCloseWhereAnimationCompleted)
    {
        RequestCloseWhereAnimationCompleted = false;
        Dispatcher.UIThread.Post(() => MotionAwareClose());  // 延迟执行关闭
    }
}
```

### 6.5 超时兜底

为防止动画完成回调因极端情况未被触发，使用 `DispatcherTimer` 超时兜底：

```csharp
var completedFuncCalled = false;

// 超时兜底 — 时长 * 1.2
_timeoutDisposable = DispatcherTimer.RunOnce(() =>
{
    if (!completedFuncCalled)
    {
        completedFuncCalled = true;
        // 强制执行完成逻辑
    }
}, motion.Duration * 1.2);

// 正常完成路径
motion.Run(actor, completedAction: () =>
{
    _timeoutDisposable?.Dispose();
    if (!completedFuncCalled)
    {
        completedFuncCalled = true;
        // 正常完成逻辑
    }
});
```

---

## 7. 注意事项

### 7.1 线程安全

所有动画 API 必须在 **UI 线程** 上调用。`AbstractMotion.Run()` 内部使用 `Dispatcher.UIThread` 确保执行上下文正确。

### 7.2 Follow 模式限制

处于 Follow 模式（`actor.Follow(target)`）的 Actor 不能直接播放 Motion，否则会抛出异常。需要先调用 `actor.UnFollow()` 解除绑定。

### 7.3 Out 动效的状态重置

所有 `*OutMotion`（退出动效）在 `NotifyCompleted` 中会重置 Actor 状态：

```csharp
protected override void NotifyCompleted(BaseMotionActor actor)
{
    actor.Opacity = 1.0;          // 恢复不透明
    actor.MotionTransform = null;  // 清除变换
}
```

这确保 Actor 在下次复用时处于干净状态。如果你的自定义 Out Motion 没有这个重置，可能导致 Actor 在后续使用中仍然是不可见的。

### 7.4 避免硬编码时长

始终使用 Design Token 派生的时长（对应 Ant Design 的 `motionDurationFast` / `motionDurationMid` / `motionDurationSlow`）：

```csharp
// ❌ 不推荐
motion.Duration = TimeSpan.FromMilliseconds(200);

// ✅ 推荐
motion.Duration = MotionDuration;  // 来自 Token 绑定的属性
```

或在创建 Transition 时使用 `SharedTokenKind`：

```csharp
TransitionUtils.CreateTransition<DoubleTransition>(
    OpacityProperty,
    SharedTokenKind.MotionDurationFast  // 从全局 Token 获取时长
);
```

### 7.5 EnableMotion 全局开关

当 `EnableMotion = false` 时，控件应跳过所有动画直接到达最终状态。这对应 Ant Design 的 `motion: false` Seed Token 和 `@media (prefers-reduced-motion: reduce)` 无障碍降级：

```csharp
if (!IsMotionEnabled)
{
    // 直接设置最终状态，不播放动画
    _contentPanel.IsVisible = true;
    _contentPanel.Opacity = 1.0;
    return;
}
// 正常播放动画...
```

### 7.6 遵循 Ant Design 动效三原则

在为控件添加动效时，请始终检查是否符合以下原则：

| 原则 | 检查点 |
|------|-------|
| **自然** | 缓动曲线是否模拟了真实物理运动？进入用 Ease-Out、退出用 Ease-In？ |
| **高效** | 动画时长是否足够短？是否使用了正确的 `MotionDuration*` Token？退出是否比进入更快？ |
| **克制** | 动效是否有明确目的？是否存在不必要的装饰性动画？是否避免了同时播放过多动画？ |

### 7.7 退出动画的 HitTest 处理

Ant Design 在退出动画激活时设置 `pointerEvents: 'none'`，防止正在消失的元素响应鼠标事件。在 AtomUI 中，退出动效应在动画开始时设置 `IsHitTestVisible = false`：

```csharp
// 在退出动画开始前
motion.Run(_motionActor,
    aboutToStart: () =>
    {
        _motionActor.IsHitTestVisible = false;  // 禁用鼠标事件
    },
    completedAction: () =>
    {
        _motionActor.IsVisible = false;
        _motionActor.IsHitTestVisible = true;   // 恢复
    });
```

---

## 8. 完整示例：自定义控件的动效集成

以下是一个完整的控件动效集成示例，展示所有最佳实践：

```csharp
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.MotionScene;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls;

public class ExpandablePanel : TemplatedControl, IMotionAwareControl
{
    #region 公共属性

    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<ExpandablePanel, bool>(nameof(IsExpanded));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ExpandablePanel>();

    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<ExpandablePanel>();

    public static readonly StyledProperty<AbstractMotion?> ExpandMotionProperty =
        AvaloniaProperty.Register<ExpandablePanel, AbstractMotion?>(nameof(ExpandMotion));

    public static readonly StyledProperty<AbstractMotion?> CollapseMotionProperty =
        AvaloniaProperty.Register<ExpandablePanel, AbstractMotion?>(nameof(CollapseMotion));

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    public AbstractMotion? ExpandMotion
    {
        get => GetValue(ExpandMotionProperty);
        set => SetValue(ExpandMotionProperty, value);
    }

    public AbstractMotion? CollapseMotion
    {
        get => GetValue(CollapseMotionProperty);
        set => SetValue(CollapseMotionProperty, value);
    }

    #endregion

    private MotionActor? _motionActor;
    private bool _animating;

    public ExpandablePanel()
    {
        // 自动绑定全局 EnableMotion Token
        this.ConfigureMotionBindingStyle();

        // 绑定 MotionDuration 到 Token
        var style = new Style();
        style.Add(MotionDurationProperty, SharedTokenKind.MotionDurationMid);
        Styles.Add(style);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _motionActor = e.NameScope.Find<MotionActor>("PART_MotionActor");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsExpandedProperty)
        {
            if (change.GetNewValue<bool>())
                PlayExpandAnimation();
            else
                PlayCollapseAnimation();
        }
    }

    private void PlayExpandAnimation()
    {
        if (_animating || _motionActor == null) return;

        if (!IsMotionEnabled)
        {
            _motionActor.IsVisible = true;
            return;
        }

        _animating = true;
        _motionActor.IsVisible = true;

        var motion = ExpandMotion ?? new ExpandMotion(Direction.Top, MotionDuration, new CubicEaseOut());
        if (MotionDuration != TimeSpan.Zero)
            motion.Duration = MotionDuration;

        motion.Run(_motionActor, completedAction: () => { _animating = false; });
    }

    private void PlayCollapseAnimation()
    {
        if (_animating || _motionActor == null) return;

        if (!IsMotionEnabled)
        {
            _motionActor.IsVisible = false;
            return;
        }

        _animating = true;

        var motion = CollapseMotion ?? new CollapseMotion(Direction.Top, MotionDuration, new CubicEaseIn());
        if (MotionDuration != TimeSpan.Zero)
            motion.Duration = MotionDuration;

        motion.Run(_motionActor, completedAction: () =>
        {
            _motionActor.IsVisible = false;
            _animating = false;
        });
    }
}
```

