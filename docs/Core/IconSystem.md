# AtomUI Icon 系统

> 本文档描述 `AtomUI.Core` 中 Icon 子系统的架构设计，包括矢量图标抽象基类、泛型 XAML 标记扩展、Expression Tree 缓存优化、绘制指令体系及旋转动画支持。

---

## 1. 概述

AtomUI Icon 系统提供了一套可扩展的矢量图标框架，支持多主题类型、自定义笔触/填充、旋转动画（Spin/Pulse），并通过 Expression Tree 编译缓存优化图标实例创建性能。该系统主要服务于 `AtomUI.Icons.AntDesign`（自动生成的 Ant Design 图标集），但架构上允许任何图标集接入。

### 源文件清单

| 文件 | 职责 |
|------|------|
| `Controls/Icon/Icon.cs` | 图标抽象基类，继承 `PathIcon`，定义所有公共属性与自定义渲染 |
| `Controls/Icon/IconProvider.cs` | 泛型 XAML 标记扩展 `IconProvider<TIconKind>`，在 XAML 中创建图标实例 |
| `Controls/Icon/IconProviderCache.cs` | 线程安全的 Expression Tree 编译缓存 |
| `Controls/Icon/DrawingInstruction.cs` | 绘制指令抽象基类及 7 种具体指令 |
| `Controls/Icon/Common.cs` | 枚举定义：`IconAnimation`、`IconThemeType`、`IconBrushType` |
| `Controls/Icon/PulseEasing.cs` | 步进式脉冲缓动曲线 |

---

## 2. 架构总览

```
┌──────────────────────────────────────────────────────────────┐
│  XAML 使用层                                                  │
│  <antdicons:LoadingOutlined Animation="Spin" />              │
│  <antdicons:CheckCircleFilled FillBrush="{...}" />           │
│                              │                                │
│                     MarkupExtension.ProvideValue()            │
│                              │                                │
│  ┌───────────────────────────▼──────────────────────────┐    │
│  │  IconProvider<TIconKind>  (MarkupExtension)           │    │
│  │  - Kind → 枚举值（如 AntDesignIconKind.Loading）       │    │
│  │  - StrokeBrush / FillBrush / Width / Height           │    │
│  │  - GetTypeForKind(kind) → 具体 Icon 类型               │    │
│  │  - CreateFactory(type)  → Expression.Lambda 编译工厂   │    │
│  └───────────────────────────┬──────────────────────────┘    │
│                              │                                │
│  ┌───────────────────────────▼──────────────────────────┐    │
│  │  IconProviderCache  (static, ConcurrentDictionary)    │    │
│  │  - TypeCache:    Enum → EnumValue → Type              │    │
│  │  - CreatorCache:  Enum → EnumValue → Func<Icon>       │    │
│  │  - TypeToCreator: Type → Func<Icon>                   │    │
│  │  → Expression Tree 编译后的无参构造函数委托             │    │
│  └───────────────────────────┬──────────────────────────┘    │
│                              │ creator()                      │
│  ┌───────────────────────────▼──────────────────────────┐    │
│  │  Icon  (abstract, extends PathIcon)                    │    │
│  │  - DrawingInstructions → IList<DrawingInstruction>     │    │
│  │  - DrawBrushes[5] / DrawPens[5]  （笔触/画笔数组）     │    │
│  │  - Render() → 自定义矢量渲染                           │    │
│  │  - SetupRotateAnimation() → Spin/Pulse 旋转动画       │    │
│  │  - ConfigureTransitions() → 颜色过渡动画               │    │
│  │  implements ICustomHitTest, IMotionAwareControl         │    │
│  └──────────────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────────────┘
```

---

## 3. Icon 抽象基类

### 3.1 继承关系

```
Avalonia.Controls.PathIcon
    └── AtomUI.Controls.Icon (abstract)
            ├── implements ICustomHitTest
            └── implements IMotionAwareControl
```

`Icon` 继承自 Avalonia 的 `PathIcon`，但完全重写了渲染逻辑 — 不使用 `PathIcon` 的 `Data` 属性，而是通过 `DrawingInstructions` 绘制指令体系进行自定义渲染。

### 3.2 公共属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|-------|------|
| `StrokeBrush` | `IBrush?` | null | 主笔触颜色 |
| `FillBrush` | `IBrush?` | null | 主填充颜色 |
| `SecondaryStrokeBrush` | `IBrush?` | null | 副笔触颜色（双色图标） |
| `SecondaryFillBrush` | `IBrush?` | null | 副填充颜色（双色图标） |
| `FallbackBrush` | `IBrush?` | White | 备选颜色 |
| `IconTheme` | `IconThemeType` | Filled | 图标主题类型 |
| `StrokeWidth` | `double` | 4 | 笔触宽度 |
| `StrokeLineCap` | `PenLineCap` | Round | 线端样式 |
| `StrokeLineJoin` | `PenLineJoin` | Round | 连接样式 |
| `LoadingAnimation` | `IconAnimation` | None | 旋转动画类型 |
| `LoadingAnimationDuration` | `TimeSpan` | 1s | 旋转动画周期 |
| `FillAnimationDuration` | `TimeSpan` | 200ms | 颜色过渡时长 |
| `IsMotionEnabled` | `bool` | — | 动画开关（绑定全局 Token） |

### 3.3 笔刷类型体系

`Icon` 内部维护了一个 5 元素的笔刷数组（`DrawBrushes[5]`）和对应的画笔数组（`DrawPens[5]`），索引由 `IconBrushType` 枚举决定：

```csharp
public enum IconBrushType
{
    Stroke,           // [0] 主笔触
    Fill,             // [1] 主填充
    SecondaryStroke,   // [2] 副笔触
    SecondaryFill,     // [3] 副填充
    Fallback,          // [4] 备选
    None               // 无笔刷
}
```

绘制指令通过 `StrokeBrush` 和 `FillBrush` 字段引用枚举值，在渲染时调用 `Icon.FindIconBrush(brushType)` 从数组中查找实际笔刷。这种间接引用设计允许：
- 多个绘制指令共享同一笔刷实例
- 笔刷变化时自动影响所有引用该类型的指令
- 支持 `BindingPriority.Animation` 优先级的颜色过渡

### 3.4 自定义渲染流程

```
Icon.Render(DrawingContext)
│
├── 1. FillRectangle(Background) — 绘制背景
├── 2. 计算 ViewBox → DesiredSize 的缩放比 scale
├── 3. CalculateGlobalGeometryMatrix() — 获取全局变换矩阵
├── 4. PushTransform(scale) — 应用缩放
└── 5. foreach instruction in DrawingInstructions:
        └── instruction.Draw(context, globalMatrix, this)
            ├── BuildGeometry() — 延迟构建几何体（缓存）
            ├── FindIconBrush() — 从 Icon 查找笔刷
            ├── BuildPen() — 构建画笔
            ├── 应用 Transform + globalMatrix
            └── DrawGeometry(fillBrush, pen, geometry)
```

### 3.5 旋转动画

Icon 支持两种旋转动画模式，通过 `LoadingAnimation` 属性控制：

| 模式 | 效果 | 缓动 | 适用场景 |
|------|------|------|---------|
| `Spin` | 匀速连续旋转 0°→360° | 线性（默认） | Loading 图标 |
| `Pulse` | 步进式旋转（8 步） | `PulseEasing`（阶梯函数） | 脉冲加载效果 |

动画通过 `Style.Animations` 机制实现：

```csharp
var animation = new Animation {
    Duration = LoadingAnimationDuration,
    IterationCount = IterationCount.Infinite,
    FillMode = FillMode.Backward,
    Children = {
        new KeyFrame { Cue = new Cue(0d), Setters = { new Setter(AngleAnimationRotateProperty, 0d) } },
        new KeyFrame { Cue = new Cue(1d), Setters = { new Setter(AngleAnimationRotateProperty, 360d) } }
    }
};
if (LoadingAnimation == IconAnimation.Pulse) {
    animation.Easing = new PulseEasing();  // 8 步阶梯函数
}
```

`PulseEasing` 将连续的 0→1 进度映射为 8 个离散步进值（0, 0.125, 0.25, ...），产生"卡顿旋转"的视觉效果。

### 3.6 颜色过渡

当 `IsMotionEnabled = true` 时，Icon 自动为 5 种笔刷属性添加 `SolidColorBrushTransition`：

```csharp
Transitions = [
    BaseTransitionUtils.CreateTransition<SolidColorBrushTransition>(StrokeBrushProperty, FillAnimationDuration),
    BaseTransitionUtils.CreateTransition<SolidColorBrushTransition>(FillBrushProperty, FillAnimationDuration),
    BaseTransitionUtils.CreateTransition<SolidColorBrushTransition>(SecondaryFillBrushProperty, FillAnimationDuration),
    BaseTransitionUtils.CreateTransition<SolidColorBrushTransition>(SecondaryStrokeBrushProperty, FillAnimationDuration),
    BaseTransitionUtils.CreateTransition<SolidColorBrushTransition>(FallbackBrushProperty, FillAnimationDuration),
];
```

这使得图标在颜色变化时（如 hover 状态）有平滑的颜色过渡效果。

---

## 4. IconProvider — 泛型标记扩展

### 4.1 设计

`IconProvider<TIconKind>` 是一个 `MarkupExtension`，允许在 XAML 中通过枚举值创建图标实例：

```xml
<antdicons:LoadingOutlined />                       <!-- Kind 自动匹配 -->
<antdicons:CheckCircleFilled FillBrush="Green" />   <!-- 自定义笔刷 -->
```

泛型参数 `TIconKind` 是图标集的枚举类型（如 `AntDesignIconKind`），由 `AtomUI.Icons.AntDesign.Generator` 自动生成。

### 4.2 实例创建流程

```
IconProvider.ProvideValue(serviceProvider)
│
├── 1. GetIcon(Kind)
│      ├── GetTypeForKind(kind) → 具体 Icon 子类的 Type
│      ├── IconProviderCache.GetOrAddCreator(enumType, kind, ...)
│      │      └── CreateFactory(type) → Expression.Lambda<Func<Icon>>.Compile()
│      └── creator() → new XxxIcon()
│
├── 2. 设置可选属性
│      ├── Animation → LoadingAnimationProperty
│      ├── StrokeBrush / FillBrush / SecondaryStroke / SecondaryFill
│      └── Width / Height
│
└── 3. return icon
```

### 4.3 Expression Tree 缓存优化

每个图标类型的无参构造函数被编译为一个 `Func<Icon>` 委托并缓存，避免反复反射调用：

```csharp
protected virtual Func<Icon> CreateFactory(Type type)
{
    var constructor = type.GetConstructor(Type.EmptyTypes);
    var newExpr = Expression.New(constructor);
    var lambda = Expression.Lambda<Func<Icon>>(newExpr);
    return lambda.Compile();  // 编译为原生委托
}
```

`IconProviderCache` 使用三层 `ConcurrentDictionary` 缓存：

```
TypeCache:     Enum类型 → { 枚举值 → Type }           // Kind → 具体图标类型
CreatorCache:  Enum类型 → { 枚举值 → Func<Icon> }      // Kind → 编译后工厂
TypeToCreator: Type → Func<Icon>                        // 类型 → 工厂（跨枚举共享）
```

这确保了每个图标类型的构造函数只被编译一次，后续调用直接使用缓存的委托，性能接近直接 `new`。

---

## 5. DrawingInstruction — 绘制指令体系

### 5.1 指令继承体系

```
DrawingInstruction (abstract)
├── RectDrawingInstruction       — 矩形（支持圆角）
├── CircleDrawingInstruction     — 圆形
├── EllipseDrawingInstruction    — 椭圆
├── LineDrawingInstruction       — 直线
├── PolylineDrawingInstruction   — 折线
├── PolygonDrawingInstruction    — 多边形
└── PathDrawingInstruction       — SVG 路径
```

### 5.2 指令基类属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Transform` | `Matrix?` | 指令级变换矩阵 |
| `Opacity` | `double` | 指令级不透明度 |
| `StrokeBrush` | `IconBrushType?` | 笔触类型引用 |
| `FillBrush` | `IconBrushType?` | 填充类型引用 |
| `IsStrokeEnabled` | `bool` | 是否启用笔触 |
| `IsStrokeWidthCustomizable` | `bool` | 笔触宽度是否跟随 Icon 设置 |
| `IsStrokeLinejoinCustomizable` | `bool` | 连接样式是否跟随 |
| `IsStrokeLinecapCustomizable` | `bool` | 端点样式是否跟随 |

### 5.3 延迟构建与缓存

每个 `DrawingInstruction` 在首次 `Draw()` 调用时通过 `BuildGeometry()` 构建 `Geometry` 对象并缓存。后续渲染复用缓存的 Geometry，仅更新 Transform 和 Brush：

```csharp
public void Draw(DrawingContext drawingContext, in Matrix globalGeometryMatrix, Icon icon)
{
    _geometry ??= BuildGeometry();  // 延迟构建 + 缓存
    // ... 应用变换、获取笔刷、绘制
}
```

---

## 6. 图标主题类型

```csharp
public enum IconThemeType
{
    Filled,      // 实心填充
    Outlined,    // 描边
    Rounded,     // 圆角
    Sharp,       // 尖角
    TwoTone,     // 双色
    MultiColor   // 多色
}
```

当 `IconTheme` 为 `Filled`、`Outlined`、`Rounded`、`Sharp` 时，Icon 标记为 `IsSupportSimpleTransition = true`，允许简化的颜色过渡。`TwoTone` 和 `MultiColor` 类型使用多个笔刷通道，过渡逻辑更复杂。

---

## 7. 与 AtomUI.Icons.AntDesign 的关系

`AtomUI.Icons.AntDesign` 项目由 `AtomUI.Icons.AntDesign.Generator` 源代码生成器自动生成。生成器读取 Ant Design 图标的 SVG 数据，为每个图标生成：

1. 一个继承 `Icon` 的具体类（如 `LoadingOutlinedIcon`）
2. `DrawingInstructions` 列表中的绘制指令
3. `IconProvider<AntDesignIconKind>` 的具体实现

Icon 系统本身不依赖任何具体图标集 — 它只提供框架。具体图标集通过实现 `Icon` 子类和 `IconProvider<T>` 接入。

