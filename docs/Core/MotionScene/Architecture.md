# AtomUI 动效系统核心架构

> 本文档深入解析 AtomUI 动效系统的核心架构，包括 Actor-Motion 分离模型、双驱动执行引擎、布局感知机制、浮动渲染层以及可通知 Transition 体系。
>
> AtomUI 动效系统是 Ant Design 5.0 CSS 动画体系（`components/style/motion/`）在 .NET / Avalonia 平台上的 C# 原生实现。Ant Design 使用 CSS `@keyframes` + `transition` 驱动动画；AtomUI 将其转化为 Avalonia 的 `Animation` + `Transition` 双驱动模型，并通过 Actor-Motion 分离实现了比 CSS 更灵活的动画编排能力。

---

## 1. Actor-Motion 分离模型

AtomUI 动效系统的核心设计思想是 **Actor-Motion 分离**：

- **Motion**（动效定义）— 描述"做什么动画"：时长、缓动、起止值、变换原点。对应 Ant Design 中 `fade.ts`、`zoom.ts`、`slide.ts`、`move.ts`、`collapse.ts` 各文件定义的 CSS `@keyframes`。
- **Actor**（动画载体）— 描述"在哪里播放"：一个 `ContentControl`，包裹被动画的内容。对应 CSS 中应用了动画类名（如 `.ant-zoom-enter-active`）的 DOM 元素。

这种分离带来了三个关键优势（这是 AtomUI 相对于 Ant Design CSS 方案的架构改进）：

1. **可复用** — 同一个 `ZoomBigInMotion` 实例可以作用于 Popup、Tooltip、Select 等不同控件的 Actor。在 Ant Design 中需要为每个组件重复配置 CSS motion 类名。
2. **可组合** — 一个 Actor 可以依次播放多个不同的 Motion（如先 FadeIn 再 ZoomIn）。CSS 动画难以实现这种链式编排。
3. **可替换** — 控件通过 `StyledProperty<AbstractMotion?>` 暴露动效属性，允许使用者替换默认动效。比 Ant Design 的 `motionName` 字符串配置更类型安全。

### 1.1 类关系图

```
IMotion (接口)
  │
  └── AbstractMotion (抽象基类)
        ├── FadeInMotion / FadeOutMotion
        ├── ZoomInMotion / ZoomOutMotion / ZoomBigInMotion / ...
        ├── SlideUpInMotion / SlideDownInMotion / ...
        ├── MoveUpInMotion / MoveDownInMotion / ...
        ├── CollapseMotion / ExpandMotion
        └── [自定义 Motion] — 开发者继承 AbstractMotion

IMotionActor (接口)
  │
  └── BaseMotionActor (ContentControl, 基础载体)
        ├── BaseLayoutAwareMotionActor (布局感知载体)
        └── MotionActor (AtomUI.Controls 中的具体实现, 用于 XAML)
```

### 1.2 工作流程

```
1. 创建 Motion:    var motion = new ZoomBigInMotion(duration, easing);
2. 获取 Actor:     actor = templateRoot.Find<MotionActor>("PART_MotionActor");
3. 执行动画:       motion.Run(actor, aboutToStart, completedAction);
                    └── motion 内部:
                        ├─ [Transition模式] 设置起始值 → 添加 Transitions → 设置结束值
                        └─ [Animation模式] 配置关键帧 → RunAsync(actor)
4. 动画完成:       completedAction 回调被触发
```

---

## 2. IMotion 接口与 AbstractMotion 基类

### 2.1 IMotion 接口

```csharp
public interface IMotion
{
    RelativePoint RenderTransformOrigin { get; }  // 变换原点
    IList<Animation> Animations { get; }          // Animation 模式的动画列表
    IList<INotifyTransitionCompleted> Transitions { get; }  // Transition 模式的过渡列表
    TimeSpan Duration { get; }                    // 动画时长
    Easing Easing { get; }                        // 缓动曲线
    FillMode PropertyValueFillMode { get; }       // 填充模式
    MotionSpiritType SpiritType { get; set; }     // 驱动模式

    void Run(BaseMotionActor actor,
             Action? aboutToStart = null,
             Action? completedAction = null);
}
```

### 2.2 AbstractMotion 虚方法体系

`AbstractMotion` 通过 **Template Method 模式** 组织动画配置流程。子类只需重写特定虚方法即可定义完整动效：

```csharp
public class AbstractMotion : IMotion
{
    // ── Transition 模式虚方法 ──
    protected virtual void ConfigureTransitions()       // 配置 Transition 列表
    protected virtual void ConfigureMotionStartValue()  // 设置动画起始属性值
    protected virtual void ConfigureMotionEndValue()    // 设置动画结束属性值

    // ── Animation 模式虚方法 ──
    protected virtual void ConfigureAnimation()         // 配置 KeyFrame 动画

    // ── 生命周期通知 ──
    protected virtual void NotifyPreStart(actor)        // 动画即将开始
    protected virtual void NotifyCompleted(actor)       // 动画已完成

    // ── SceneLayer 支持 ──
    internal virtual Size CalculateSceneSize(motionTargetSize)       // 计算浮动层尺寸
    internal virtual Point CalculateScenePosition(size, position)    // 计算浮动层位置
}
```

### 2.3 双驱动执行引擎详解

`AbstractMotion.Run()` 是动画执行的入口，根据 `SpiritType` 分发到两条执行路径。这对应 Ant Design 中两种不同的 CSS 动画实现方式：

- **Transition 路径** ← Ant Design 的 Collapse 动效使用 CSS `transition`（`components/style/motion/collapse.ts`）
- **Animation 路径** ← Ant Design 的 Fade/Zoom/Slide/Move 动效使用 CSS `@keyframes` + `animation`（`components/style/motion/motion.ts` 中的 `initMotion()` 函数）

#### Transition 驱动路径

**适用场景**: 简单的两点插值动画（Fade、Zoom、Slide、Collapse/Expand）。

**执行时序**:

```
Run()
  ├── ConfigureTransitions()           // 1. 子类创建 Transition 列表
  │     └── 默认实现创建:
  │         ├── NotifiableDoubleTransition (Opacity)
  │         └── NotifiableTransformOperationsTransition (MotionTransform)
  ├── actor.RenderTransformOrigin = ...// 2. 设置变换原点
  ├── actor.NotifyMotionPreStart()     // 3. 通知 Actor 动画开始
  ├── NotifyPreStart(actor)            // 4. 子类预处理钩子
  ├── aboutToStart?.Invoke()           // 5. 外部回调
  ├── actor.Transitions = null         // 6. 清空原有 Transitions
  ├── ConfigureMotionStartValue(actor) // 7. 设置起始值（无过渡，立即生效）
  │
  └── [UI Thread Post]
      ├── actor.Transitions = transitions  // 8. 安装 Transition 列表
      ├── ConfigureMotionEndValue(actor)   // 9. 设置结束值（触发 Transition 插值）
      ├── await Task.WhenAny(tasks)        // 10. 等待所有 Transition 完成
      ├── actor.NotifyMotionCompleted()    // 11. 通知 Actor 动画完成
      ├── NotifyCompleted(actor)           // 12. 子类后处理钩子
      └── completedAction?.Invoke()        // 13. 外部回调
```

**关键设计**: 步骤 6-7 和 8-9 的顺序至关重要。先清空 Transitions 再设置起始值，确保起始值 **立即生效** 不产生过渡。然后安装 Transitions 后再设置结束值，框架检测到属性变化后自动启动插值动画。

**完成检测**: 使用自定义的 `INotifyTransitionCompleted` 接口（基于 `Subject<bool>`），当 `Interpolate` 方法中 `progress >= 1.0` 时发出完成信号。为防止极端情况下信号丢失，额外设置了 `maxDuration * 1.1` 的超时兜底。

#### Animation 驱动路径

**适用场景**: 复杂的多关键帧动画（Move 系列等需要中间状态的动效）。

**执行时序**:

```
Run()
  ├── ConfigureAnimation()                // 1. 子类构建 KeyFrame 动画
  ├── actor.RenderTransformOrigin = ...   // 2. 设置变换原点
  ├── actor.NotifyMotionPreStart()        // 3. 通知开始
  ├── NotifyPreStart(actor)               // 4. 子类预处理
  ├── aboutToStart?.Invoke()              // 5. 外部回调
  │
  └── [UI Thread Post → InvokeAsync]
      ├── foreach (animation in Animations)
      │     └── await animation.RunAsync(actor)  // 6. 依次执行 Avalonia Animation
      ├── actor.NotifyMotionCompleted()          // 7. 通知完成
      ├── NotifyCompleted(actor)                 // 8. 子类后处理
      └── completedAction?.Invoke()              // 9. 外部回调
```

**Animation 模式** 使用 Avalonia 内置的 `Animation` 类（支持 `KeyFrame` + `Cue` 定义时间线），适合需要中间关键帧（如 Move 系列在 0% → 80% → 100% 有不同的插值曲线）的场景。

---

## 3. BaseMotionActor 详解

### 3.1 核心属性

```csharp
public abstract class BaseMotionActor : ContentControl, IMotionActor
{
    // ── 驱动属性 ──
    StyledProperty<ITransform?> MotionTransformProperty       // 变换矩阵（Transition 模式）
    StyledProperty<TransformOperations?> MotionTransformOperationsProperty  // 变换操作（Animation 模式）

    // ── 生命周期事件 ──
    RoutedEvent<RoutedEventArgs> PreStartEvent   // 动画开始前事件
    RoutedEvent<RoutedEventArgs> CompletedEvent  // 动画完成后事件
}
```

### 3.2 属性变化 → RenderTransform 管线

当 `MotionTransform` 或 `MotionTransformOperations` 属性变化时，`BaseMotionActor` 自动执行以下管线：

```
属性变化
  → HandleLayoutTransformChanged() / HandleLayoutTransformOperationsChanged()
    → 订阅 Transform.Changed 事件
    → ApplyMotionTransform()
      → 从 MotionTransform / MotionTransformOperations 提取 Matrix
      → RoundMatrix() 四舍五入（避免浮点精度问题）
      → 设置 MatrixTransform.Matrix
      → 设置 RenderTransform = MatrixTransform
      → InvalidateVisual()
```

### 3.3 Follow 模式

`BaseMotionActor` 支持 **Follow 模式**，允许一个 Actor 绑定到另一个 Actor 的属性，实现"影子跟随"效果：

```csharp
// buddyActor 跟随 mainActor 的所有变换
buddyActor.Follow(mainActor);

// 绑定的属性:
// MotionTransform, MotionTransformOperations, Opacity, RenderTransformOrigin
```

典型应用：Popup 的 `PopupBuddyLayer` 使用 Follow 模式让阴影层跟随内容层的动画。

> **注意**: 处于 Follow 模式的 Actor 不能直接调用 `Motion.Run()`，否则会抛出 `InvalidOperationException`。

---

## 4. BaseLayoutAwareMotionActor

`BaseLayoutAwareMotionActor` 继承自 `BaseMotionActor`，增加了 **布局感知** 能力。

### 4.1 问题背景

普通 `RenderTransform` 只影响渲染，不影响布局。例如将一个元素 Scale 到 0.5，它在父容器中仍然占据原始尺寸的空间。这在折叠/展开动画中会导致内容突然跳动。

### 4.2 解决方案

`BaseLayoutAwareMotionActor` 重写了 `MeasureOverride` 和 `ArrangeOverride`，在计算可用空间和排列位置时将变换矩阵考虑在内：

```csharp
protected override Size MeasureOverride(Size availableSize)
{
    // 1. 测量子元素的 DesiredSize
    MotionTransformRoot.Measure(availableSize);
    var desiredSize = MotionTransformRoot.DesiredSize;

    // 2. 将 DesiredSize 通过变换矩阵映射到 AABB（轴对齐包围盒）
    Rect transformedRect = new Rect(0, 0, desiredSize.Width, desiredSize.Height)
                           .TransformToAABB(Transformation);

    // 3. 返回变换后的尺寸 → 父容器会按此尺寸分配空间
    return new Size(transformedRect.Width, transformedRect.Height);
}
```

### 4.3 UseRenderTransform 属性

当 `UseRenderTransform = true` 时，回退到普通 RenderTransform 行为（不影响布局）。这允许在同一个 Actor 上按需切换两种模式。

### 4.4 ApplyMotionTransform 覆写

布局感知版本的 `ApplyMotionTransform` 会 **过滤掉 Scale 分量**（仅保留 Translate/Rotate），因为 Scale 已经通过 Measure/Arrange 影响了布局尺寸：

```csharp
// 仅将 Scale 设为 1.0，保留其他矩阵分量
private static Matrix FilterScaleTransform(Matrix matrix)
{
    return new Matrix(1.0, matrix.M12, matrix.M21, 1.0, matrix.M31, matrix.M32);
}
```

这样避免了 Scale 被 "双重应用"（一次通过布局，一次通过渲染）。

---

## 5. 可通知 Transition 体系（Notifiable Transitions）

### 5.1 问题背景

Avalonia 原生的 `TransitionBase<T>` 没有提供"过渡完成"的通知机制。AtomUI 的 Motion 系统需要精确知道过渡何时结束，以便触发 `CompletedEvent` 和清理状态。

> **Ant Design 中的类似机制**: Ant Design 在 `initCollapseMotion`（`_util/motion.ts`）中使用 `motionDeadline: 500`（500ms 超时兜底）来确保动画在极端情况下也能结束。AtomUI 的可通知 Transition 体系是一种更精确的解决方案 — 通过在插值过程中检测 `progress >= 1.0` 来准确触发完成通知。

### 5.2 解决方案

AtomUI 自定义了一套 **可通知 Transition** 体系：

```
INotifyTransitionCompleted (接口)
  │  CompletedObservable: IObservable<bool>     // Rx 完成信号
  │  TransitionCompleted: EventHandler           // 事件通知
  │  Duration: TimeSpan                          // 时长（用于超时计算）
  │
  └── AbstractNotifiableTransition<T> (抽象基类)
        │  继承 InterpolatingTransitionBase<T>
        │  在 Interpolate() 中检测 progress >= 1.0 时通知完成
        │
        ├── NotifiableDoubleTransition       (double)
        ├── NotifiableTransformOperationsTransition  (TransformOperations)
        ├── NotifiableFloatTransition        (float)
        ├── NotifiableCornerRadiusTransition (CornerRadius)
        ├── NotifiableThicknessTransition    (Thickness)
        ├── NotifiablePointTransition        (Point)
        ├── NotifiableSizeTransition         (Size)
        ├── NotifiableBoolTransition         (bool)
        └── NotifiableVectorTransition       (Vector)
```

### 5.3 工作原理

```csharp
internal class NotifiableDoubleTransition : AbstractNotifiableTransition<double>
{
    protected override double Interpolate(double progress, double from, double to)
    {
        var result = InterpolateUtils.DoubleInterpolate(progress, from, to);
        // 当 progress 达到 1.0 时发出完成通知
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}
```

`AbstractMotion` 的 Transition 执行路径使用 `Subject<bool>` 的 `ToTask()` 将 Rx Observable 转为 `Task`，再用 `Task.WhenAny` 等待完成。

---

## 6. SceneLayer（浮动渲染层）

### 6.1 设计目的

某些动画需要在控件的正常视觉树之外渲染。例如：

- Popup 的缩放动画如果在 PopupRoot 内部播放，会被 ClipToBounds 裁剪。
- 带阴影的控件在缩放时，阴影需要独立于内容进行渲染。

`SceneLayer` 提供了一个 **独立的顶层透明窗口**（基于 `IPopupImpl`），用于承载这些特殊动画。

### 6.2 核心特性

```csharp
internal class SceneLayer : WindowBase, IHostedVisualTreeRoot, IDisposable
{
    // ── 透明背景，不接收鼠标事件 ──
    Background        = Brushes.Transparent
    IsHitTestVisible  = false
    WindowTransparencyLevel = Transparent  // 无边框透明窗口
    WindowManagerAddShadowHint = false      // 不添加系统阴影
    IgnoreMouseEvents = true                // 穿透鼠标事件

    // ── 定位 ──
    void MoveAndResize(Point, Size)         // 精确定位到目标位置
}
```

### 6.3 PopupBuddyLayer

`PopupBuddyLayer` 是 `SceneLayer` 的典型子类，用于 Popup 控件的开关动画：

```
PopupBuddyLayer : SceneLayer, IShadowAwareLayer
  ├── 包含自己的 MotionActor
  ├── 克隆 Popup 的 Child 作为 Ghost 显示在浮动层
  ├── 使用 Follow 模式让阴影 Actor 跟随内容 Actor
  ├── RunOpenMotion() → 播放 OpenMotion（默认 ZoomBigInMotion）
  └── RunCloseMotion() → 播放 CloseMotion（默认 ZoomBigOutMotion）
```

---

## 7. InterpolateUtils 插值算法库

`InterpolateUtils` 提供了所有类型的插值算法，既被 Notifiable Transition 使用，也可独立使用：

| 方法 | 类型 | 说明 |
|------|------|------|
| `DoubleInterpolate` | `double` | 线性插值 `(to - from) * progress + from` |
| `FloatInterpolate` | `float` | 同上，float 版本 |
| `BoolInterpolate` | `bool` | progress ≥ 1.0 返回 newValue |
| `ColorInterpolate` | `Color` | 预乘 Alpha 空间线性插值 |
| `BoxShadowsInterpolate` | `BoxShadows` | 逐分量插值（Offset、Blur、Spread、Color） |
| `CornerRadiusInterpolate` | `CornerRadius` | 四角独立插值 |
| `PointInterpolate` | `Point` / `RelativePoint` | 坐标插值 |
| `SizeInterpolate` | `Size` | 宽高插值 |
| `ThicknessInterpolate` | `Thickness` | 四边插值 |
| `TransformOperationsInterpolate` | `TransformOperations` | 委托 Avalonia 内置插值 |
| `VectorInterpolate` | `Vector` | 向量插值 |
| `SolidColorBrushInterpolate` | `ISolidColorBrush` | Color + Opacity 插值 |
| `GradientBrushInterpolate` | `IGradientBrush` | 渐变停靠点逐个插值 |
| `BrushInterpolate` | `IBrush` | 自动判断类型分发 |

### 颜色插值的预乘 Alpha

颜色插值在 **预乘 Alpha（Premultiplied Alpha）** 空间进行，避免了在透明度变化时出现"黑边"问题：

```csharp
// 预乘
r1 = fromColor.R * fromColor.A;
g1 = fromColor.G * fromColor.A;
// ... 插值 ...
// 反预乘
r = r_interpolated / a_interpolated;
```

---

## 8. 自定义 Transition 类型

除了 Notifiable 系列（用于 Motion 完成检测），AtomUI 还提供了两个公共 Transition 类型：

### 8.1 SolidColorBrushTransition

支持 `ISolidColorBrush` 和 `IGradientBrush` 之间的平滑过渡。当两端都是 `ISolidColorBrush` 时直接插值颜色；当一端是渐变笔刷时，自动将纯色笔刷转换为相同结构的渐变笔刷后再插值。

### 8.2 BoxShadowsTransition

支持 `BoxShadows`（多阴影）的平滑过渡。逐个阴影插值 OffsetX、OffsetY、Blur、Spread、Color。

---

## 9. 线程模型

所有动画操作必须在 **UI 线程** 上执行。`AbstractMotion` 通过 `Dispatcher.UIThread.Post()` 和 `Dispatcher.UIThread.InvokeAsync()` 确保这一点。

### Transition 模式的时序要求

```
[当前 UI 帧]  清空 Transitions → 设置起始值（立即生效）
[下一 UI 帧]  安装 Transitions → 设置结束值（触发过渡）→ 等待完成
```

这个两帧分离是必要的：Avalonia 的 Transition 系统需要在 "当前值已经是起始值" 的状态下才能正确检测到属性变化并启动插值。

### Animation 模式的异步执行

```
[UI Thread Post]
  └── [InvokeAsync]
        └── foreach animation → await RunAsync(actor)
```

`InvokeAsync` 返回的 Task 在所有关键帧播放完成后 resolve。

---

## 10. 与 Ant Design CSS 动画架构的对照

### 10.1 Ant Design 的 `initMotion()` 模式

Ant Design 在 `components/style/motion/motion.ts` 中定义了一个核心函数 `initMotion()`，它为每种动效生成三组 CSS 规则：

```typescript
// 1. 进入/出现 — 初始暂停状态
`${motionCls}-enter, ${motionCls}-appear`: { animationDuration: duration, animationFillMode: 'both', animationPlayState: 'paused' }
// 2. 退出 — 初始暂停状态
`${motionCls}-leave`: { animationDuration: duration, animationFillMode: 'both', animationPlayState: 'paused' }
// 3. 激活 — 播放动画
`${motionCls}-enter-active, ${motionCls}-appear-active`: { animationName: inKeyframes, animationPlayState: 'running' }
`${motionCls}-leave-active`: { animationName: outKeyframes, animationPlayState: 'running', pointerEvents: 'none' }
```

AtomUI 的 `AbstractMotion.Run()` 方法等效于这个生命周期：
- 创建 Motion 并设置初始值 = 添加 `-enter` 类名（暂停状态）
- 调用 `Run()` 开始动画 = 添加 `-enter-active` 类名（播放状态）
- 动画完成后清理 = 移除所有动画类名

### 10.2 无动画降级（No-Motion）

Ant Design 在 `components/style/motion/util.ts` 中提供了无障碍降级：

```typescript
export const genNoMotionStyle = (): CSSObject => ({
  '@media (prefers-reduced-motion: reduce)': {
    transition: 'none',
    animation: 'none',
  },
});
```

AtomUI 通过以下机制实现等效功能：
- **全局开关**: `EnableMotion` Seed Token（对应 Ant Design 的 `motion: boolean` Seed Token）
- **控件响应**: `IMotionAwareControl` 接口 + `IsMotionEnabled` 属性
- **自动绑定**: `ConfigureMotionBindingStyle()` 将控件的 `IsMotionEnabled` 绑定到全局 Token
- **条件跳过**: 控件在播放动画前检查 `IsMotionEnabled`，为 false 时直接设置最终状态

### 10.3 退出动画的 `pointerEvents: 'none'`

Ant Design 在退出动画激活时设置 `pointerEvents: 'none'`，防止正在消失的元素仍然响应鼠标事件。AtomUI 在退出动效的 `NotifyPreStart` 中通过设置 `IsHitTestVisible = false` 实现相同效果。

