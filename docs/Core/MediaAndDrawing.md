# AtomUI 媒体与绘图系统（Media & Drawing System）

## 概述

媒体与绘图系统位于 `src/AtomUI.Core/Media/` 目录，为 AtomUI 提供了丰富的图形处理基础设施。该系统覆盖了从颜色科学计算、几何图形构建到 CSS 变换解析等多个领域，是 UI 渲染层的核心支撑。

### 在整体架构中的位置

```
UI 控件渲染层
    ↓ 使用
Media & Drawing System（本文档 — 颜色/几何/变换/排版）
    ↓ 基于
Avalonia.Media（底层渲染引擎）
```

## 目录结构

```
Media/
├── ColorUtils.cs                      # 颜色科学计算（WCAG 无障碍）
├── ColorExtensions.cs                 # 颜色操作流式 API
├── CommonShapeBuilder.cs              # 通用图形构建器
├── DrawingContextExtensions.cs        # 绘图上下文扩展
├── TransformParser.cs                 # CSS transform 解析器
├── PenUtils.cs                        # 画笔工具
├── GeometryUtils.cs                   # 几何计算工具
├── BoxShadowExtensions.cs             # 阴影扩展
├── TextUtils.cs                       # 文本测量工具
├── FontUtils.cs                       # 字体工具
├── Transitions/
│   ├── ColorTransition.cs             # 颜色过渡
│   ├── RectTransition.cs              # 矩形过渡
│   └── PixelPointTransition.cs        # 像素点过渡
└── TextFormatting/
    ├── FormattedTextSource.cs                          # 格式化文本源
    └── TextParagraphPropertiesReflectionExtensions.cs  # 段落属性反射
```

## 核心组件

### 1. ColorUtils — 颜色科学与 WCAG 无障碍

`ColorUtils` 提供了基于色彩科学标准的颜色处理能力，特别关注 **WCAG 2.0 无障碍标准**。

#### WCAG 可读性计算

```csharp
// 计算两个颜色之间的对比度（WCAG 2.0 标准）
double ratio = ColorUtils.Readability(foreground, background);

// 检查是否满足可读性要求
bool ok = ColorUtils.IsReadable(fg, bg, ReadabilitySize.AA_Large);

// 从候选颜色中选择最佳可读色
Color best = ColorUtils.MostReadable(background, candidates, 
    includeFallbackColors: true);
```

#### 可读性等级

| 等级 | 最小对比度 | 场景 |
|------|-----------|------|
| AA_Small | 4.5:1 | 正常文字 |
| AA_Large | 3.0:1 | 大号文字（18px+ 或 14px+ 粗体） |
| AAA_Small | 7.0:1 | 增强正常文字 |
| AAA_Large | 4.5:1 | 增强大号文字 |

#### HSL/HSV 颜色空间操作

```csharp
// RGB ↔ HSL 互转
HslColor hsl = ColorUtils.RgbToHsl(r, g, b);
Color rgb = ColorUtils.HslToRgb(h, s, l);

// RGB ↔ HSV 互转
HsvColor hsv = ColorUtils.RgbToHsv(r, g, b);
Color rgb = ColorUtils.HsvToRgb(h, s, v);
```

#### CSS 颜色解析

```csharp
// 支持多种 CSS 颜色格式
Color c1 = ColorUtils.FromCssString("#ff6600");    // Hex
Color c2 = ColorUtils.FromCssString("rgb(255,0,0)"); // RGB
Color c3 = ColorUtils.FromCssString("hsl(120,50%,50%)"); // HSL
```

### 2. ColorExtensions — 流式颜色操作 API

为 `Color` 类型提供丰富的扩展方法链式调用：

```csharp
Color result = baseColor
    .Lighten(20)       // 增加亮度 20%
    .Desaturate(10)    // 降低饱和度 10%
    .Spin(30)          // 色相旋转 30°
    .SetAlpha(0.8);    // 设置透明度
```

#### 可用操作

| 方法 | 说明 |
|------|------|
| `Desaturate(amount)` | 降低饱和度 |
| `Saturate(amount)` | 增加饱和度 |
| `Lighten(amount)` | 增加亮度 |
| `Darken(amount)` | 降低亮度 |
| `BrightenColor(amount)` | 增加明度 |
| `Spin(degrees)` | 色相旋转 |
| `GetBrightness()` | 获取感知明度 |
| `GetLuminance()` | 获取相对亮度（WCAG） |
| `Mix(other, amount)` | 两色混合 |
| `TintColor(amount)` | 添加白色调 |
| `ShadeColor(amount)` | 添加黑色调 |

### 3. CommonShapeBuilder — 通用图形构建器

使用 `StreamGeometry` 高效构建常用的 UI 图形：

```csharp
// 构建对勾图形（用于 Checkbox 等）
Geometry checkmark = CommonShapeBuilder.BuildCheckMark(width, height);

// 构建圆弧
Geometry arc = CommonShapeBuilder.BuildArc(center, radius, startAngle, endAngle);

// 构建箭头
Geometry arrow = CommonShapeBuilder.BuildArrow(direction, size);
```

### 4. TransformParser — CSS Transform 解析器

完整实现 CSS `transform` 属性字符串的解析，支持所有标准变换函数：

#### 支持的变换函数

| 函数 | 示例 |
|------|------|
| `translate` | `translate(10px, 20px)` |
| `translateX/Y` | `translateX(50%)` |
| `scale` | `scale(1.5, 2.0)` |
| `scaleX/Y` | `scaleY(0.5)` |
| `rotate` | `rotate(45deg)` |
| `skew` | `skew(10deg, 20deg)` |
| `skewX/Y` | `skewX(15deg)` |
| `matrix` | `matrix(1,0,0,1,0,0)` |

#### 角度单位支持

```csharp
// 支持多种角度单位
TransformOperations ops = TransformParser.Parse("rotate(0.5turn)");
// deg（度）、rad（弧度）、grad（百分度）、turn（圈）
```

### 5. DrawingContextExtensions — 绘图上下文扩展

为 Avalonia 的 `DrawingContext` 提供自定义绘制方法：

```csharp
// 绘制药丸形矩形（两端完全圆角）
context.DrawPilledRect(brush, pen, rect, orientation);

// 绘制圆弧
context.DrawArc(pen, center, radius, startAngle, sweepAngle);
```

### 6. PenUtils — 画笔智能管理

根据画笔的可变性状态智能选择复用或创建策略：

```csharp
// 对于不可变画笔 → 直接复用
// 对于可变画笔 → 创建新实例
IPen pen = PenUtils.CreateOrReusePen(brush, thickness);
```

### 7. GeometryUtils — 几何计算

```csharp
// 计算缩放后的矩形（保持渲染精度）
Rect scaled = GeometryUtils.CalculateScaledRect(original, renderScale);

// 获取 CornerRadius 的标量值
double radius = GeometryUtils.CornerRadiusScalarValue(cornerRadius);
```

### 8. BoxShadowExtensions — 阴影处理

```csharp
// 将 BoxShadows 转换为等效的 Thickness（用于布局计算）
Thickness padding = boxShadows.ToThickness();
```

### 9. 文本与字体工具

#### TextUtils
```csharp
// 测量文本渲染尺寸
Size size = TextUtils.MeasureText(text, fontSize, typeface);
```

#### FontUtils
```csharp
// EM 单位到像素的转换
double px = FontUtils.ConvertEmToPixel(emValue, fontSize);
```

## 文本格式化（TextFormatting）

### FormattedTextSource

实现 Avalonia 的 `ITextSource` 接口，提供**字形感知**（Grapheme-aware）的文本分段：

- 正确处理 Unicode 组合字符和 emoji
- 为每个文本段（TextRun）分配正确的排版属性
- 支持自定义的文本格式化流程

### TextParagraphPropertiesReflectionExtensions

通过反射访问 Avalonia `TextParagraphProperties` 的 `LineSpacing` 属性（该属性在某些版本中未公开）。使用 `[DynamicDependency]` 确保 AOT 安全。

## Media Transitions（媒体过渡）

系统还提供了三个专用的过渡动画类型，补充 `Animations/Transitions/` 中的通用过渡：

| 过渡类型 | 说明 |
|----------|------|
| `ColorTransition` | `Color` 类型属性的平滑过渡 |
| `RectTransition` | `Rect` 类型属性的平滑过渡（位置+尺寸） |
| `PixelPointTransition` | `PixelPoint` 类型属性的平滑过渡 |

## 设计模式

| 模式 | 应用 |
|------|------|
| **建造者模式** | `CommonShapeBuilder` 使用 StreamGeometry 逐步构建复杂图形 |
| **解析器模式** | `TransformParser` 实现完整的 CSS transform 字符串解析 |
| **策略模式** | `PenUtils` 根据画笔状态选择复用或创建策略 |
| **扩展方法模式** | `ColorExtensions`、`DrawingContextExtensions` 等提供流式 API |
| **适配器模式** | `FormattedTextSource` 将文本数据适配为 Avalonia 文本排版接口 |

## 与其他系统的关系

- **主题系统**：`ColorUtils` 的颜色算法被主题算法（`ThemeAlgorithm`）大量使用来生成色板
- **图标系统**：`CommonShapeBuilder` 构建的图形用于内置图标
- **动画系统**：`Media/Transitions/` 中的过渡类型补充了核心动画系统
- **控件渲染**：`DrawingContextExtensions` 在控件的 `Render` 方法中使用

## 相关文档

- [架构概览](./Architecture.md) — AtomUI.Core 整体架构
- [图标系统](./IconSystem.md) — 矢量图标系统
- [主题算法](./ThemeSystem/ThemeAlgorithm.md) — 使用颜色算法的主题生成
- [动画系统](./AnimationSystem.md) — 核心动画与过渡
