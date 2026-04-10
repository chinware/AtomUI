# 核心类型与工具（Core Types & Utilities）

## 概述

核心类型与工具分布在 `src/AtomUI.Core/` 的根目录及 `Utils/`、`Exceptions/`、`Reflection/` 子目录中，构成了整个 AtomUI 框架的**基础类型系统和通用工具集**。这些组件被所有其他子系统广泛使用。

### 在整体架构中的位置

```
所有 AtomUI 子系统（Theme, Motion, Controls 等）
    ↓ 依赖
Core Types & Utilities（本文档 — 基础类型、数学、反射、异常）
    ↓ 基于
.NET Runtime + Avalonia Framework
```

## 目录结构

```
AtomUI.Core/
├── ApplicationExtensions.cs           # 应用程序入口扩展
├── Common.cs                          # 通用枚举与值类型
├── Dimension.cs                       # 维度值类型（绝对/百分比）
├── StdPseudoClass.cs                  # 标准伪类常量集
├── Utils/
│   ├── MathUtils.cs                   # 浮点数学工具
│   ├── MatrixUtils.cs                 # 矩阵变换工具
│   ├── EnumExtensions.cs             # 高性能枚举标志操作
│   ├── SpanStringTokenizer.cs         # 零拷贝字符串分词器
│   ├── StringBuilderCache.cs          # StringBuilder 线程缓存
│   ├── TypeHelper.cs                  # 类型与属性路径解析
│   └── BorderUtils.cs                 # 边框渲染工具
├── Exceptions/
│   ├── BootstrapException.cs          # 框架初始化异常
│   └── InvalidPropertyValueException.cs # 属性值验证异常
└── Reflection/
    ├── ObjectExtension.cs             # 对象反射扩展
    ├── TypeMemberExtension.cs         # 类型成员反射
    └── StyledElementReflectionExtensions.cs # StyledElement 反射
```

## 应用程序入口

### ApplicationExtensions — UseAtomUI()

框架初始化的**入口点**，通过扩展方法集成到 Avalonia 应用启动流程：

```csharp
public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        this.UseAtomUI();  // ← 初始化 AtomUI 框架
    }
}
```

`UseAtomUI()` 内部完成：
1. 注册主题管理器（ThemeManager）
2. 初始化 Token 资源系统
3. 配置默认动效参数
4. 设置语言资源提供器

## 核心值类型

### Common.cs — 通用枚举与类型

#### 枚举定义

| 枚举 | 说明 | 值 |
|------|------|-----|
| `TextDecorationLine` | 文本装饰线类型 | None, Underline, Overline, LineThrough |
| `LineStyle` | 线条样式 | Solid, Dashed, Dotted |
| `Direction` | 方向 | LTR, RTL |
| `SizeType` | 尺寸类型 | Small, Middle, Large |
| `Corner` | 角位置（Flags） | None, TopLeft, TopRight, BottomLeft, BottomRight, All |

#### TextDecorationInfo

```csharp
public record struct TextDecorationInfo(
    TextDecorationLine Line,
    LineStyle Style,
    Color? Color
);
```

紧凑的值类型，描述完整的文本装饰信息。

### Dimension — 带单位的维度值

`Dimension` 是一个值类型，支持**绝对值**和**百分比值**两种模式：

```csharp
// 绝对值
var d1 = new Dimension(100);           // 100px
var d2 = Dimension.Parse("100");       // 100px

// 百分比值
var d3 = new Dimension(50, true);      // 50%
var d4 = Dimension.Parse("50%");       // 50%

// 解析到实际值
double resolved = d3.Resolve(parentSize: 400); // → 200
```

#### 运算符支持

```csharp
Dimension a = 100;    // 隐式转换
Dimension b = 50;
var c = a + b;        // 150
var d = a * 2;        // 200
bool eq = a == b;     // false
```

### StdPseudoClass — 标准伪类常量

定义了 **38 个**标准 CSS-like 伪类常量，避免硬编码魔术字符串：

```csharp
public static class StdPseudoClass
{
    public const string Hover = ":hover";
    public const string Pressed = ":pressed";
    public const string Disabled = ":disabled";
    public const string Focus = ":focus";
    public const string FocusVisible = ":focus-visible";
    public const string Checked = ":checked";
    public const string Selected = ":selected";
    public const string Empty = ":empty";
    // ... 共 38 个
}
```

## 工具类

### MathUtils — 浮点数学工具

针对 UI 渲染中常见的浮点精度问题，提供 epsilon 容差比较：

```csharp
// 浮点容差比较（默认 epsilon = 1e-6）
bool close = MathUtils.AreClose(1.0000001, 1.0);        // true
bool less = MathUtils.LessThan(0.999999, 1.0);           // false（视为相等）
bool greater = MathUtils.GreaterThan(1.000001, 1.0);     // false

// 角度转换
double rad = MathUtils.DegreesToRadians(180);  // → π
double deg = MathUtils.RadiansToDegrees(Math.PI); // → 180

// 值约束
double clamped = MathUtils.Clamp(value, min, max);
```

在动画系统中被广泛使用（如过渡完成检测 `progress ≈ 1.0`）。

### MatrixUtils — 矩阵变换工具

提供便利的 2D 变换矩阵操作：

```csharp
// 围绕指定点旋转
Matrix rotated = MatrixUtils.RotateAt(angle, centerX, centerY);

// 围绕指定点缩放
Matrix scaled = MatrixUtils.ScaleAt(scaleX, scaleY, centerX, centerY);

// 平移
Matrix translated = MatrixUtils.Translate(offsetX, offsetY);
```

### EnumExtensions — 高性能枚举标志

使用 `Unsafe.As` 实现**零装箱**的枚举 Flags 操作：

```csharp
// 传统方式（装箱）
bool has = myFlags.HasFlag(MyEnum.FlagA);

// 高性能方式（零装箱）
bool has = myFlags.HasAllFlags(MyEnum.FlagA);
bool any = myFlags.HasAnyFlag(MyEnum.FlagA | MyEnum.FlagB);
```

通过将枚举值直接转换为底层整数类型进行位操作，完全避免了 `HasFlag` 方法的装箱开销。

### SpanStringTokenizer — 零拷贝分词器

`ref struct` 实现的字符串分词器，直接操作 `ReadOnlySpan<char>`：

```csharp
var tokenizer = new SpanStringTokenizer("10px 20px 30px");
while (tokenizer.TryReadNext(out var token))
{
    // token 是 ReadOnlySpan<char>，无堆分配
    ProcessToken(token);
}
```

特点：
- **零堆分配**：作为 `ref struct` 在栈上运行
- **零拷贝**：通过 `Span<char>` 切片原始字符串
- 用于 CSS 值解析等高频场景

### StringBuilderCache — StringBuilder 线程缓存

移植自 `dotnet/runtime` 的高性能工具，使用 `[ThreadStatic]` 缓存 `StringBuilder` 实例：

```csharp
// 从缓存获取 StringBuilder
var sb = StringBuilderCache.Acquire(capacity: 256);
sb.Append("Hello");
sb.Append(" World");

// 获取结果并归还缓存
string result = StringBuilderCache.GetStringAndRelease(sb);
```

避免频繁创建和销毁 `StringBuilder` 对象。

### TypeHelper — 类型与属性路径

解析深层属性路径，支持嵌套属性、可空类型和索引器：

```csharp
// 解析属性路径 "Customer.Address.City"
var segments = TypeHelper.ParsePropertyPath("Customer.Address.City");

// 支持可空类型处理
Type underlying = TypeHelper.GetUnderlyingType(typeof(int?)); // → int

// 支持索引器属性
var value = TypeHelper.GetPropertyValue(obj, "Items[0].Name");
```

### BorderUtils — 边框渲染工具

提供渲染缩放感知的边框厚度调整：

```csharp
// 根据渲染缩放比例调整 Thickness
// 确保在高 DPI 下边框清晰、不模糊
Thickness adjusted = BorderUtils.AdjustForRenderScale(
    thickness, renderScale
);
```

## 异常类型

### BootstrapException

框架初始化阶段的专用异常，当 `UseAtomUI()` 配置失败时抛出：

```csharp
throw new BootstrapException("ThemeManager initialization failed");
```

### InvalidPropertyValueException

属性值验证异常，当设置无效的属性值时抛出：

```csharp
throw new InvalidPropertyValueException(
    propertyName: "FontSize",
    value: -1,
    reason: "Font size must be positive"
);
```

## 反射工具

### ObjectExtension — 通用对象反射

提供类型安全的反射操作，包含 `Try` 和 `OrThrow` 两种风格：

```csharp
// Try 风格（返回 bool）
if (obj.TryGetProperty<string>("Name", out var name))
{
    // 使用 name
}

// OrThrow 风格（失败时抛异常）
string name = obj.GetPropertyOrThrow<string>("Name");

// 支持字段和方法
obj.TryGetField<int>("_count", out var count);
obj.TryInvokeMethod("Reset");
obj.InvokeMethodOrThrow("Initialize", arg1, arg2);
```

### TypeMemberExtension — 类型成员查找

高效的类型成员反射查找，支持搜索继承层次：

```csharp
// 查找属性（包括私有和继承的）
PropertyInfo? prop = type.FindProperty("InternalState", 
    BindingFlags.NonPublic | BindingFlags.Instance);

// 查找字段
FieldInfo? field = type.FindField("_backing");

// 查找方法
MethodInfo? method = type.FindMethod("OnPropertyChanged");

// 查找事件
EventInfo? evt = type.FindEvent("ValueChanged");
```

### StyledElementReflectionExtensions

访问 Avalonia `StyledElement` 的内部成员：

```csharp
// 获取逻辑子元素集合（通常是 internal 的）
var children = element.GetLogicalChildren();

// 设置模板父级
element.SetTemplatedParent(parent);
```

## 设计模式

| 模式 | 应用 |
|------|------|
| **扩展方法模式** | `ApplicationExtensions`、`EnumExtensions`、`ObjectExtension` 等 |
| **值对象模式** | `Dimension`、`TextDecorationInfo` 作为不可变值类型 |
| **常量类模式** | `StdPseudoClass` 集中定义伪类字符串常量 |
| **缓存模式** | `StringBuilderCache` 线程静态实例缓存 |
| **外观模式** | 反射扩展封装复杂的反射调用为简洁的 API |
| **空对象模式** | `Try*` 方法避免异常，返回默认值 |

## 与其他系统的关系

- **全系统依赖**：`MathUtils`、`EnumExtensions` 等被所有子系统使用
- **动画系统**：`MathUtils.AreClose()` 用于过渡完成检测
- **主题系统**：`ApplicationExtensions.UseAtomUI()` 初始化主题管理器
- **媒体系统**：`MatrixUtils` 用于变换计算，`SpanStringTokenizer` 用于 CSS 解析
- **控件系统**：`StdPseudoClass` 定义控件状态伪类

## 相关文档

- [架构概览](./Architecture.md) — AtomUI.Core 整体架构
- [动画系统](./AnimationSystem.md) — 使用 MathUtils 的过渡检测
- [媒体与绘图](./MediaAndDrawing.md) — 使用矩阵工具和分词器
- [主题系统概览](./ThemeSystem/Overview.md) — UseAtomUI 初始化的主题系统
