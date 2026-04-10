# 动画与过渡系统（Animation &amp; Transition System）

## 概述

动画与过渡系统是 AtomUI 动效体系的**底层基础设施**，位于 `src/AtomUI.Core/Animations/` 目录。它为上层的 [MotionScene 动效场景系统](./MotionScene/Overview.md) 提供了类型安全的属性插值、可通知的过渡动画以及动画生命周期管理能力。

与 MotionScene 关注"何时播放什么动效"不同，本系统关注的是**"如何在两个值之间平滑过渡"**这一核心问题。

### 在整体架构中的位置

```
MotionScene（动效场景 — 高层编排）
    ↓ 使用
Animation & Transition System（本文档 — 底层插值与过渡）
    ↓ 基于
Avalonia Animation Framework（框架原生动画能力）
```

## 目录结构

```
Animations/
├── AnimationExtensions.cs            # 动画循环播放扩展
├── AnimatableReflectionExtensions.cs  # Animatable 私有 API 反射访问
├── BaseTransitionUtils.cs             # 过渡动画工厂
├── InterpolateUtils.cs                # 全类型插值引擎（核心）
└── Transitions/
    ├── INotifyTransitionCompleted.cs          # 完成通知接口
    ├── AbstractNotifiableTransition.cs        # 可通知过渡基类
    ├── BoxShadowsTransition.cs               # 阴影过渡
    ├── SolidColorBrushTransition.cs           # 画刷过渡
    ├── NotifiableBoolTransition.cs            # Bool 过渡
    ├── NotifiableDoubleTransition.cs          # Double 过渡
    ├── NotifiableFloatTransition.cs           # Float 过渡
    ├── NotifiableIntegerTransition.cs         # Int 过渡
    ├── NotifiablePointTransition.cs           # Point 过渡
    ├── NotifiableRelativePointTransition.cs   # RelativePoint 过渡
    ├── NotifiableSizeTransition.cs            # Size 过渡
    ├── NotifiableThicknessTransition.cs       # Thickness 过渡
    ├── NotifiableVectorTransition.cs          # Vector 过渡
    ├── NotifiableCornerRadiusTransition.cs    # CornerRadius 过渡
    ├── NotifiableBoxShadowsTransition.cs      # BoxShadows 可通知过渡
    └── NotifiableTransformOperationsTransition.cs  # Transform 过渡
```

## 核心组件

### 1. InterpolateUtils — 全类型插值引擎

`InterpolateUtils` 是整个动画系统的计算核心，提供了覆盖所有 Avalonia 常用类型的静态插值方法。所有过渡类都将实际的数值计算委托给此工具类。

#### 支持的插值类型

| 类型 | 方法 | 说明 |
|------|------|------|
| `double` | `DoubleInterpolate` | 线性插值 |
| `float` | `FloatInterpolate` | 线性插值 |
| `int` | `IntegerInterpolate` | 归一化整数插值 |
| `bool` | `BoolInterpolate` | progress >= 1.0 时切换 |
| `Point` | `PointInterpolate` | 二维点向量插值 |
| `RelativePoint` | `PointInterpolate`（重载） | 感知单位（像素/百分比）的插值 |
| `Size` | `SizeInterpolate` | 宽高插值 |
| `Thickness` | `ThicknessInterpolate` | 四边 Margin/Padding 插值 |
| `Vector` | `VectorInterpolate` | 二维向量插值 |
| `CornerRadius` | `CornerRadiusInterpolate` | 四角圆角独立插值 |
| `Color` | `ColorInterpolate` | 预乘 Alpha 颜色混合 |
| `BoxShadow` / `BoxShadows` | `BoxShadow(s)Interpolate` | 多阴影的颜色、模糊、扩散、偏移插值 |
| `ITransform` | `TransformOperationsInterpolate` | 变换矩阵插值 |
| `ISolidColorBrush` | `SolidColorBrushInterpolate` | 画刷颜色与不透明度插值 |
| `IGradientBrush` | `GradientBrushInterpolate` | 线性/径向/锥形渐变插值 |
| `IBrush` | `BrushInterpolate` | 自动分发到对应画刷插值器 |

#### 颜色插值的特殊处理

颜色插值使用**预乘 Alpha（Premultiplied Alpha）**算法，确保半透明颜色的混合在视觉上正确：

```
R_result = R_from * (1 - progress) + R_to * progress
```

对于接近透明的颜色（Alpha < 1/256），直接采用另一端颜色的 RGB 分量，避免除以零或颜色偏移。

#### 跨类型画刷插值

系统支持 `SolidColorBrush` 与 `GradientBrush` 之间的平滑过渡：将纯色画刷自动转换为与目标渐变同结构的渐变画刷（所有 Stop 颜色相同），然后逐 Stop 进行颜色插值。

### 2. INotifyTransitionCompleted — 完成通知接口

```csharp
public interface INotifyTransitionCompleted : ITransition
{
    IObservable<bool> CompletedObservable { get; }
    event EventHandler<TransitionCompletedEventArgs>? TransitionCompleted;
    TimeSpan Duration { get; }
}
```

该接口扩展了 Avalonia 原生的 `ITransition`，增加了两种完成通知机制：
- **事件（Event）**：传统的 `TransitionCompleted` 事件
- **响应式流（Observable）**：`CompletedObservable` 提供 Rx 风格的订阅

### 3. AbstractNotifiableTransition&lt;T&gt; — 可通知过渡基类

```
Avalonia.TransitionBase
  └── Avalonia.InterpolatingTransitionBase<T>
        └── AbstractNotifiableTransition<T>  ← 实现 INotifyTransitionCompleted
              └── 12 个具体过渡类型
```

所有可通知过渡都继承此抽象基类，它封装了：
- 基于 `Subject<bool>` 的响应式完成信号发射
- 使用 `MathUtils.AreClose()` 的浮点容差完成检测（而非精确的 `progress == 1.0`）
- 统一的 `CheckCompletedStatus` + `NotifyTransitionCompleted` 模板

#### 统一的插值模式

每个具体过渡类遵循相同的实现模式：

```csharp
protected override T Interpolate(double progress, T from, T to)
{
    var result = InterpolateUtils.XxxInterpolate(progress, from, to);
    if (CheckCompletedStatus(progress))
    {
        NotifyTransitionCompleted(true);
    }
    return result;
}
```

### 4. BaseTransitionUtils — 过渡工厂

提供泛型工厂方法，简化过渡实例的创建：

```csharp
public static ITransition CreateTransition<T>(
    AvaloniaProperty targetProperty,
    TimeSpan? duration = null,    // 默认 300ms
    Easing? easing = null         // 默认 LinearEasing
) where T : TransitionBase, new()
```

### 5. AnimationExtensions — 无限循环动画

```csharp
// 在取消前无限循环播放动画
await animation.RunInfiniteAsync(control, cancellationToken);
```

内部将 `IterationCount` 设为 1，在循环中反复调用 `RunAsync`，直到 `CancellationToken` 被取消。用于图标旋转（Spin）等需要持续播放的场景。

### 6. AnimatableReflectionExtensions — Animatable 状态控制

通过反射访问 Avalonia `Animatable` 类的私有方法，实现过渡的启用/禁用：

```csharp
animatable.EnableTransitions();   // 启用过渡
animatable.DisableTransitions();  // 禁用过渡
```

使用 `[DynamicDependency]` 特性确保 AOT/Trimming 安全。

## 类型继承体系

```
Avalonia.Animation.TransitionBase
  └── Avalonia.Animation.InterpolatingTransitionBase<T>
        ├── BoxShadowsTransition           （非通知型）
        ├── SolidColorBrushTransition      （非通知型，支持跨类型画刷）
        └── AbstractNotifiableTransition<T> （可通知基类）
              ├── NotifiableBoolTransition
              ├── NotifiableDoubleTransition
              ├── NotifiableFloatTransition
              ├── NotifiableIntegerTransition
              ├── NotifiablePointTransition
              ├── NotifiableRelativePointTransition
              ├── NotifiableSizeTransition
              ├── NotifiableThicknessTransition
              ├── NotifiableVectorTransition
              ├── NotifiableCornerRadiusTransition
              ├── NotifiableBoxShadowsTransition
              └── NotifiableTransformOperationsTransition
```

## 设计模式

| 模式 | 应用 |
|------|------|
| **工厂模式** | `BaseTransitionUtils.CreateTransition<T>()` 创建类型化过渡实例 |
| **委托模式** | 所有过渡将插值计算委托给 `InterpolateUtils` |
| **模板方法** | `AbstractNotifiableTransition<T>` 定义骨架，子类覆写 `Interpolate` |
| **观察者模式** | `INotifyTransitionCompleted` 同时支持事件和响应式流 |
| **策略模式** | 不同类型使用不同的插值策略（颜色、向量、画刷、变换等） |

## 与 MotionScene 的关系

MotionScene 系统在创建 Motion 时，使用本系统提供的过渡类型来驱动属性动画：

1. `AbstractMotion.ConfigureTransitions()` 调用 `BaseTransitionUtils.CreateTransition<T>()` 创建过渡
2. 过渡被添加到 `IMotion.Transitions` 列表
3. 当 MotionActor 播放时，Avalonia 调用各过渡的 `Interpolate()` 方法
4. `InterpolateUtils` 执行实际的数值计算
5. 动画完成时通过 `INotifyTransitionCompleted` 通知 MotionScene

## 相关文档

- [MotionScene 概览](./MotionScene/Overview.md) — 上层动效编排系统
- [MotionScene 架构](./MotionScene/Architecture.md) — Actor-Motion 架构设计
- [内置动效参考](./MotionScene/BuiltinMotions.md) — 预置动效类型
- [主题系统概览](./ThemeSystem/Overview.md) — 动画时长 Token 来源
