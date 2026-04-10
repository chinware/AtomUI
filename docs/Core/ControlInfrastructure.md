# 控件基础设施（Control Infrastructure）

## 概述

控件基础设施位于 `src/AtomUI.Core/` 的多个目录中（`Controls/`、`Input/`、`Reactive/`），为所有 AtomUI 控件提供了动效感知、坐标追踪、焦点导航、响应式编程等基础能力。这些不是具体的 UI 控件，而是控件开发的**底层支撑框架**。

### 在整体架构中的位置

```
具体 UI 控件（Button, Input, Select 等）
    ↓ 继承/实现
Control Infrastructure（本文档 — 接口、工具类、反射扩展）
    ↓ 基于
Avalonia Controls Framework（框架控件基类）
```

## 目录结构

```
Controls/
├── IMotionAwareControl.cs             # 动效感知接口
├── AnimationUtils.cs                  # 过渡动画工厂（主题感知）
├── TemplatedControlUtils.cs           # 模板控件工具
├── TransformTrackingHelper.cs         # 坐标变换追踪
├── IClickableControl.cs               # 可点击控件接口
├── VisualReflectionExtensions.cs      # Visual 反射扩展
├── ItemCollectionReflectionExtensions.cs  # ItemCollection 反射
├── ILayoutRootReflectionExtensions.cs # LayoutRoot 反射
├── StyledElementReflectionExtensions.cs   # StyledElement 反射
├── RawPointerEventArgsReflectionExtensions.cs # 指针事件反射

Input/
├── XYFocusHelpers.cs                  # XY 方向焦点导航

Reactive/
├── IAtomUISubject.cs                  # 双向响应式接口
├── LightweightObservableBase.cs       # 轻量级 Observable 基类
└── LightweightSubject.cs             # 轻量级 Subject 实现
```

## 核心组件

### 1. IMotionAwareControl — 动效感知接口

所有需要参与动效系统的控件都应实现此接口：

```csharp
public interface IMotionAwareControl
{
    // 是否启用动效（StyledProperty，可在 XAML 中设置）
    bool IsMotionEnabled { get; set; }
    
    // 动效持续时长（StyledProperty）
    TimeSpan MotionDuration { get; set; }
}
```

#### MotionAwareControlProperty

提供静态工具方法注册 `IsMotionEnabled` 和 `MotionDuration` 的 `StyledProperty`，确保所有控件使用一致的属性定义。

```csharp
// 在控件中注册动效属性
public static readonly StyledProperty<bool> IsMotionEnabledProperty =
    MotionAwareControlProperty.Register<MyControl>(nameof(IsMotionEnabled));
```

### 2. AnimationUtils（TransitionUtils）— 主题感知过渡工厂

与 `Animations/BaseTransitionUtils` 不同，`AnimationUtils` 创建的过渡动画会自动从**主题 Token** 获取动画时长：

```csharp
// 创建过渡（时长自动从主题 Token 中获取）
var transition = AnimationUtils.CreateTransition<DoubleTransition>(
    targetProperty: OpacityProperty,
    tokenDuration: GlobalTokenResourceKey.MotionDurationMid
);
```

这实现了动画时长与主题系统的联动——当用户切换主题或禁用动效时，过渡时长自动调整。

### 3. TransformTrackingHelper — 坐标变换追踪

追踪控件在整个视觉树中的**累积坐标变换**，实时计算控件到根元素的变换矩阵。

#### 核心功能

```csharp
// 创建追踪器
var tracker = new TransformTrackingHelper(control);

// 获取控件相对于根的变换矩阵
Matrix transform = tracker.ComputedTransform;

// 监听变换变化
tracker.TransformChanged += OnTransformChanged;
```

#### 实现机制

1. 沿视觉树向上遍历到根元素
2. 订阅每一级父元素的 `TransformChanged` 事件
3. 使用 `Dispatcher.InvokeAsync` 延迟更新，合并同一帧内的多次变化
4. 当视觉树结构变化时（Attached/Detached），自动重建追踪链

用于弹出层定位（Popup positioning）等需要精确坐标的场景。

### 4. TemplatedControlUtils — 模板控件工具

解决 Avalonia 模板控件中模板父级（TemplatedParent）传播的问题：

```csharp
// 递归应用模板父级到子控件树
TemplatedControlUtils.ApplyTemplatedParentRecursively(
    rootControl, 
    templatedParent
);
```

在复杂的控件组合场景中，确保嵌套控件正确继承模板上下文。

### 5. IClickableControl — 可点击控件接口

```csharp
public interface IClickableControl
{
    event EventHandler<RoutedEventArgs>? Click;
}
```

为所有可点击控件提供统一的点击事件契约。

### 6. XYFocusHelpers — XY 方向焦点导航

处理基于方向的焦点导航逻辑，根据输入设备类型验证导航模式：

```csharp
// 检查控件是否启用了 XY 焦点导航
bool enabled = XYFocusHelpers.IsXYNavigationEnabled(control, deviceType);
```

支持的设备类型包括键盘、游戏手柄等，不同设备可能有不同的导航行为。

## 响应式编程基础设施（Reactive）

### 7. IAtomUISubject&lt;T&gt; — 双向响应式接口

```csharp
public interface IAtomUISubject<T> : IObserver<T>, IObservable<T>
{
    // 同时是观察者和可观察对象
}
```

统一了 Observer 和 Observable 的角色，允许一个对象既能接收值又能发射值。

### 8. LightweightObservableBase&lt;T&gt; — 轻量级 Observable

高性能的 `IObservable<T>` 基类，相比 Rx 的标准实现做了多项优化：

#### 优化策略

| 优化 | 说明 |
|------|------|
| **延迟初始化** | 订阅者列表在首次订阅时才分配 |
| **单订阅者优化** | 只有一个订阅者时不使用列表 |
| **线程安全** | 使用 `lock` 保护订阅者管理 |
| **结构化取消** | `Unsubscribed()` / `Subscribed()` 回调支持子类响应 |

```csharp
public abstract class LightweightObservableBase<T> : IObservable<T>
{
    protected abstract void Subscribed(IObserver<T> observer, bool first);
    protected abstract void Unsubscribed(IObserver<T> observer, bool last);
    protected void PublishNext(T value);    // 向所有订阅者发射值
    protected void PublishCompleted();       // 完成信号
    protected void PublishError(Exception e); // 错误信号
}
```

### 9. LightweightSubject&lt;T&gt; — 轻量级 Subject

继承 `LightweightObservableBase<T>`，实现 `IAtomUISubject<T>`：

```csharp
var subject = new LightweightSubject<int>();

// 作为 Observable 被订阅
subject.Subscribe(observer);

// 作为 Observer 接收值（并转发给订阅者）
subject.OnNext(42);
```

## 反射扩展集

控件基础设施包含大量反射扩展，用于访问 Avalonia 内部 API：

| 扩展类 | 目标类型 | 访问内容 |
|--------|----------|----------|
| `VisualReflectionExtensions` | `Visual` | 视觉树内部状态 |
| `ItemCollectionReflectionExtensions` | `ItemCollection` | 内部集合状态 |
| `ILayoutRootReflectionExtensions` | `ILayoutRoot` | 布局根信息 |
| `StyledElementReflectionExtensions` | `StyledElement` | LogicalChildren、TemplatedParent |
| `RawPointerEventArgsReflectionExtensions` | `RawPointerEventArgs` | 原始指针事件数据 |

所有反射扩展均使用 `[DynamicDependency]` 特性确保 AOT/Trimming 安全。

## 设计模式

| 模式 | 应用 |
|------|------|
| **接口隔离** | `IMotionAwareControl`、`IClickableControl` 各自定义最小契约 |
| **观察者模式** | `LightweightObservableBase<T>` 实现高性能的发布-订阅 |
| **中介者模式** | `TransformTrackingHelper` 中介控件与视觉树变换 |
| **工厂模式** | `AnimationUtils` 创建主题感知的过渡实例 |
| **访问者模式** | `TemplatedControlUtils` 递归遍历控件树应用模板 |
| **外观模式** | 反射扩展类封装了复杂的反射调用为简单的方法 |

## 与其他系统的关系

- **动画系统**：`IMotionAwareControl` 控制控件是否参与动效，`AnimationUtils` 使用动画系统创建过渡
- **主题系统**：`AnimationUtils` 从主题 Token 获取动画时长
- **MotionScene**：MotionActor 检查控件的 `IMotionAwareControl` 接口决定是否播放动效
- **集合系统**：`ItemCollectionReflectionExtensions` 访问集合内部状态

## 相关文档

- [架构概览](./Architecture.md) — AtomUI.Core 整体架构
- [动画系统](./AnimationSystem.md) — 底层过渡与插值
- [MotionScene 概览](./MotionScene/Overview.md) — 动效场景编排
- [主题系统概览](./ThemeSystem/Overview.md) — Token 与主题管理
