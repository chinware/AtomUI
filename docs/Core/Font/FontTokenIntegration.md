# AtomUI 字体系统 — 字体与 Design Token 系统的集成

> 本文档详细描述字体相关属性如何融入 AtomUI 的 Design Token 四层架构（Seed → Map → Alias → Component），包括字号/行高的自动派生算法、字体相关 Token 的完整列表，以及控件主题中的字体绑定机制。

---

## 1. 字体在 Token 推导链中的位置

AtomUI Design Token 系统的四层推导链中，字体相关 Token 分布在前三层：

```
┌─────────────────────────────────────────────────────────────────────────┐
│  Seed Token（种子变量）                                                   │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │  FontFamily = "fonts:AlibabaSans#Alibaba Sans, ..."             │    │
│  │  FontSize   = 14                                                │    │
│  └──────────────────────────┬──────────────────────────────────────┘    │
│                              │                                          │
│              IThemeVariantCalculator.Calculate()                        │
│              → CalculatorUtils.CalculateFontMapTokenValues()           │
│                              │                                          │
│  ┌──────────────────────────▼──────────────────────────────────────┐    │
│  │  Map Token（梯度变量）                                            │    │
│  │  FontSizeSM, FontSizeLG, FontSizeXL                             │    │
│  │  FontSizeHeading1 ~ FontSizeHeading5                            │    │
│  │  RelativeLineHeight, RelativeLineHeightSM, RelativeLineHeightLG │    │
│  │  RelativeLineHeightHeading1 ~ RelativeLineHeightHeading5        │    │
│  │  FontHeight, FontHeightSM, FontHeightLG                         │    │
│  └──────────────────────────┬──────────────────────────────────────┘    │
│                              │                                          │
│              DesignToken.CalculateAliasTokenValues()                    │
│                              │                                          │
│  ┌──────────────────────────▼──────────────────────────────────────┐    │
│  │  Alias Token（别名变量）                                          │    │
│  │  FontSizeIcon = FontSizeSM                                      │    │
│  │  FontWeightStrong = FontWeight.SemiBold                         │    │
│  └─────────────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Seed Token：字体种子

### 2.1 FontFamily（字体族）

**源文件：** `DesignToken.Seed.cs`

```csharp
/// <summary>
/// 字体
/// Ant Design 的字体家族中优先使用系统默认的界面字体，同时提供了一套利于屏显的备用字体库，
/// 来维护在不同平台以及浏览器的显示下，字体始终保持良好的易读性和可读性，体现了友好、稳定和专业的特性。
/// </summary>
[DesignTokenKind(DesignTokenKind.Seed)]
public FontFamily? FontFamily { get; set; }
```

**默认值初始化：** `DesignToken.cs` → `InitSeedTokenValues()`

```csharp
FontFamily = FontFamily.Parse(
    "fonts:AlibabaSans#Alibaba Sans, Segoe UI, Segoe UI Symbol, " +
    "Helvetica Neue, Noto Sans, Noto Sans CJK SC, 文泉驿正黑, " +
    "Microsoft YaHei, PingFang SC, $Default"
);
```

`FontFamily` 是一个特殊的 Seed Token — 它不参与主题算法的自动推导（不像 `ColorPrimary` 会被色板算法展开为 10 级梯度），而是直接作为资源值发布到 `ResourceDictionary` 中，供控件主题通过 `{atom:SharedTokenResource FontFamily}` 消费。

### 2.2 FontSize（基准字号）

```csharp
/// <summary>
/// 默认字号
/// 设计系统中使用最广泛的字体大小，文本梯度也将基于该字号进行派生。
/// </summary>
[DesignTokenKind(DesignTokenKind.Seed)]
public double FontSize { get; set; } = 14;
```

`FontSize = 14` 是 Ant Design 5.0 的标准基准字号。所有字号梯度（FontSizeSM、FontSizeLG、FontSizeHeading1~5 等）都以此为基础，通过自然对数函数自动派生。

---

## 3. Map Token：字号梯度派生

### 3.1 字号梯度派生算法

**源文件：** `CalculatorUtils.cs` → `CalculateFontMapTokenValues()` + `CalculateFontSize()`

字号梯度的核心算法基于自然对数（e ≈ 2.71828）的指数增长曲线：

```csharp
public static IList<FontSizeInfo> CalculateFontSize(double baseValue)
{
    var fontSizes = new List<double>(10);
    for (var index = 0; index < 10; ++index)
    {
        var i        = index - 1;
        var baseSize = baseValue * Math.Pow(2.71828, i / 5.0);
        var intSize  = index > 1 ? Math.Floor(baseSize) : Math.Ceiling(baseSize);
        // 转换为偶数
        fontSizes.Add((int)(Math.Floor(intSize / 2.0d) * 2));
    }

    fontSizes[1] = baseValue;  // 确保 index=1 为精确的基准字号
    
    // 计算每个字号对应的行高
    var results = new List<FontSizeInfo>();
    foreach (var size in fontSizes)
    {
        results.Add(new FontSizeInfo
        {
            Size       = size,
            LineHeight = CalculateLineHeight(size)
        });
    }
    return results;
}
```

**算法特点：**

1. 以基准字号（14px）为中心，使用 `e^(i/5)` 生成对数增长序列。
2. 将结果取整为偶数（`Floor(x/2)*2`），确保像素对齐。
3. 保证 `index=1` 位置严格等于基准字号。
4. 生成 10 个字号值（index 0~9），取前 7 个分配给 Map Token。

### 3.2 行高计算公式

```csharp
public static double CalculateLineHeight(double fontSize)
{
    return (fontSize + 8) / fontSize;
}
```

这是 Ant Design 5.0 的标准行高公式。例如：
- `fontSize = 14` → `lineHeight = (14+8)/14 ≈ 1.571`
- `fontSize = 12` → `lineHeight = (12+8)/12 ≈ 1.667`
- `fontSize = 16` → `lineHeight = (16+8)/16 = 1.500`

字号越小，行高比例越大，确保小字文本也有足够的行间距。

### 3.3 Map Token 分配

```csharp
public static void CalculateFontMapTokenValues(DesignToken designToken)
{
    var fontSizePairs = CalculateFontSize(designToken.FontSize);
    IList<double> fontSizes   = fontSizePairs.Select(item => item.Size).ToList();
    IList<double> lineHeights = fontSizePairs.Select(item => item.LineHeight).ToList();

    // 字号分配
    designToken.FontSizeSM = fontSizes[0];        // 小号字号
    designToken.FontSize   = fontSizes[1];         // 基准字号（保持不变）
    designToken.FontSizeLG = fontSizes[2];         // 大号字号
    designToken.FontSizeXL = fontSizes[3];         // 超大号字号

    designToken.FontSizeHeading5 = fontSizes[2];   // h5 标题字号
    designToken.FontSizeHeading4 = fontSizes[3];   // h4 标题字号
    designToken.FontSizeHeading3 = fontSizes[4];   // h3 标题字号
    designToken.FontSizeHeading2 = fontSizes[5];   // h2 标题字号
    designToken.FontSizeHeading1 = fontSizes[6];   // h1 标题字号

    // 行高分配
    designToken.RelativeLineHeight   = lineHeights[1];  // 基准行高
    designToken.RelativeLineHeightLG = lineHeights[2];  // 大号行高
    designToken.RelativeLineHeightSM = lineHeights[0];  // 小号行高

    designToken.RelativeLineHeightHeading5 = lineHeights[2]; // h5 行高
    designToken.RelativeLineHeightHeading4 = lineHeights[3]; // h4 行高
    designToken.RelativeLineHeightHeading3 = lineHeights[4]; // h3 行高
    designToken.RelativeLineHeightHeading2 = lineHeights[5]; // h2 行高
    designToken.RelativeLineHeightHeading1 = lineHeights[6]; // h1 行高

    // 文字高度（FontHeight = Round(fontSize * lineHeight)）
    designToken.FontHeight   = Math.Round(lineHeights[1] * fontSizes[1]);
    designToken.FontHeightLG = Math.Round(lineHeights[2] * fontSizes[2]);
    designToken.FontHeightSM = Math.Round(lineHeights[0] * fontSizes[0]);
}
```

### 3.4 默认字号梯度值（FontSize = 14 时）

| Index | FontSize (px) | LineHeight (ratio) | 分配给 Token |
|-------|-------------|-------------------|-------------|
| 0 | 12 | 1.667 | `FontSizeSM` |
| 1 | 14 | 1.571 | `FontSize`（基准） |
| 2 | 16 | 1.500 | `FontSizeLG`, `FontSizeHeading5` |
| 3 | 20 | 1.400 | `FontSizeXL`, `FontSizeHeading4` |
| 4 | 24 | 1.333 | `FontSizeHeading3` |
| 5 | 30 | 1.267 | `FontSizeHeading2` |
| 6 | 38 | 1.211 | `FontSizeHeading1` |

| Token | 值 | 说明 |
|-------|---|------|
| `FontHeight` | 22 | Round(14 × 1.571) |
| `FontHeightSM` | 20 | Round(12 × 1.667) |
| `FontHeightLG` | 24 | Round(16 × 1.500) |

---

## 4. Map Token 完整列表

**源文件：** `DesignToken.FontMap.cs`

### 4.1 字号 Token

| Token | 类型 | 说明 | 默认值 (px) |
|-------|------|------|------------|
| `FontSizeSM` | `double` | 小号字体大小 | 12 |
| `FontSizeLG` | `double` | 大号字体大小 | 16 |
| `FontSizeXL` | `double` | 超大号字体大小 | 20 |
| `FontSizeHeading1` | `double` | 一级标题字号 (H1) | 38 |
| `FontSizeHeading2` | `double` | 二级标题字号 (H2) | 30 |
| `FontSizeHeading3` | `double` | 三级标题字号 (H3) | 24 |
| `FontSizeHeading4` | `double` | 四级标题字号 (H4) | 20 |
| `FontSizeHeading5` | `double` | 五级标题字号 (H5) | 16 |

### 4.2 行高 Token

| Token | 类型 | 说明 | 默认值 |
|-------|------|------|--------|
| `RelativeLineHeight` | `double` | 基准文本行高 | ≈1.571 |
| `RelativeLineHeightSM` | `double` | 小型文本行高 | ≈1.667 |
| `RelativeLineHeightLG` | `double` | 大型文本行高 | 1.500 |
| `RelativeLineHeightHeading1` | `double` | H1 行高 | ≈1.211 |
| `RelativeLineHeightHeading2` | `double` | H2 行高 | ≈1.267 |
| `RelativeLineHeightHeading3` | `double` | H3 行高 | ≈1.333 |
| `RelativeLineHeightHeading4` | `double` | H4 行高 | 1.400 |
| `RelativeLineHeightHeading5` | `double` | H5 行高 | 1.500 |

### 4.3 文字高度 Token

| Token | 类型 | 说明 | 默认值 (px) |
|-------|------|------|------------|
| `FontHeight` | `double` | Round(FontSize × RelativeLineHeight) | 22 |
| `FontHeightSM` | `double` | Round(FontSizeSM × RelativeLineHeightSM) | 20 |
| `FontHeightLG` | `double` | Round(FontSizeLG × RelativeLineHeightLG) | 24 |

---

## 5. Alias Token：语义化字体 Token

**源文件：** `DesignToken.Alias.cs` + `DesignToken.cs` → `CalculateAliasTokenValues()`

### 5.1 Alias Token 列表

| Token | 类型 | 计算逻辑 | 说明 |
|-------|------|---------|------|
| `FontSizeIcon` | `double` | `= FontSizeSM` | 操作图标字体大小（如选择器中的箭头图标） |
| `FontWeightStrong` | `FontWeight` | `= FontWeight.SemiBold` | 标题类组件或选中项的字体粗细 |

### 5.2 Alias 计算代码

```csharp
internal void CalculateAliasTokenValues()
{
    // ...
    // Font
    FontSizeIcon = FontSizeSM;
    // ...
    FontWeightStrong = FontWeight.SemiBold;
    // ...
}
```

---

## 6. Token 算法调用链

字体 Token 的计算发生在主题算法的 `Calculate()` 方法中：

```
DefaultThemeVariantCalculator.Calculate(designToken)
    │
    ├── CalculatorUtils.CalculateFontMapTokenValues(designToken)
    │     ├── CalculateFontSize(designToken.FontSize)      // 从基准字号派生 10 级梯度
    │     ├── 分配 FontSizeSM/LG/XL/Heading1~5             // Map Token 赋值
    │     ├── 分配 RelativeLineHeight/SM/LG/Heading1~5     // 行高 Map Token 赋值
    │     └── 计算 FontHeight/SM/LG                         // 文字高度 Map Token 赋值
    │
    └── ... (其他 Map Token 计算)

DesignToken.CalculateAliasTokenValues()
    │
    ├── FontSizeIcon = FontSizeSM                           // Alias Token 赋值
    └── FontWeightStrong = FontWeight.SemiBold               // Alias Token 赋值
```

在 **紧凑模式（Compact）** 下，`CompactThemeVariantCalculator` 同样调用 `CalculatorUtils.CalculateFontMapTokenValues()`，但紧凑模式可能修改了 `FontSize` Seed Token 的值（使用更小的基准字号），从而自动级联影响所有派生 Token。

---

## 7. 控件主题中的字体绑定

### 7.1 根级字体设置

字体族和基准字号在 **根级控件主题**（Window、PopupRoot、OverlayPopupHost）中设置，通过 CSS 继承机制向下级控件传播：

**WindowTheme.axaml：**
```xml
<Setter Property="FontFamily" Value="{atom:SharedTokenResource FontFamily}" />
<Setter Property="FontSize" Value="{atom:SharedTokenResource FontSize}" />
```

**PopupRootTheme.axaml：**
```xml
<Setter Property="FontFamily" Value="{atom:SharedTokenResource FontFamily}" />
```

**OverlayPopupHostTheme.axaml：**
```xml
<Setter Property="FontFamily" Value="{atom:SharedTokenResource FontFamily}" />
```

### 7.2 Token 资源键

字体相关 Token 通过 Source Generator 自动生成资源键枚举：

```csharp
// 自动生成文件：TokenResourceConst.g.cs
public enum SharedTokenKind
{
    // ...
    FontFamily,       // Seed Token
    FontSize,         // Seed Token (会被 Map 计算覆盖)
    FontSizeSM,       // Map Token
    FontSizeLG,       // Map Token
    FontSizeXL,       // Map Token
    FontSizeHeading1, // Map Token
    // ... 更多 Map Token ...
    FontSizeIcon,     // Alias Token
    FontWeightStrong, // Alias Token
    // ...
}
```

### 7.3 在 AXAML 中消费字体 Token

```xml
<!-- 全局 Token：使用 SharedTokenResource -->
<Setter Property="FontFamily" Value="{atom:SharedTokenResource FontFamily}" />
<Setter Property="FontSize" Value="{atom:SharedTokenResource FontSize}" />
<Setter Property="FontSize" Value="{atom:SharedTokenResource FontSizeSM}" />
<Setter Property="FontSize" Value="{atom:SharedTokenResource FontSizeLG}" />
```

---

## 8. 与 Ant Design 5.0 TypeScript 版本的对应关系

| Ant Design (TypeScript) | AtomUI (C#) |
|------------------------|-------------|
| `theme.token.fontFamily` | `DesignToken.FontFamily` (Seed) |
| `theme.token.fontSize` | `DesignToken.FontSize` (Seed) |
| `theme.token.fontSizeSM` | `DesignToken.FontSizeSM` (Map) |
| `theme.token.fontSizeLG` | `DesignToken.FontSizeLG` (Map) |
| `theme.token.fontSizeHeading1` | `DesignToken.FontSizeHeading1` (Map) |
| `theme.token.lineHeight` | `DesignToken.RelativeLineHeight` (Map) |
| `theme.token.fontSizeIcon` | `DesignToken.FontSizeIcon` (Alias) |
| `theme.token.fontWeightStrong` | `DesignToken.FontWeightStrong` (Alias) |
| `genFontSizes()` (TypeScript) | `CalculatorUtils.CalculateFontSize()` (C#) |
| `getLineHeight()` (TypeScript) | `CalculatorUtils.CalculateLineHeight()` (C#) |

---

## 9. 字体工具类

### 9.1 FontUtils — em 到像素转换

**源文件：** `AtomUI.Core/Media/FontUtils.cs`

```csharp
public static class FontUtils
{
    public static double ConvertEmToPixel(double value, double fontSize, double renderScaling = 1.0)
    {
        var fontSizePx = fontSize * value * renderScaling;
        return fontSizePx * value;
    }
}
```

### 9.2 TextUtils — 文本尺寸计算

**源文件：** `AtomUI.Core/Media/TextUtils.cs`

```csharp
public static class TextUtils
{
    public static Size CalculateTextSize(string text,
                                         double fontSize,
                                         FontFamily fontFamily,
                                         FontStyle fontStyle = FontStyle.Normal,
                                         FontWeight fontWeight = FontWeight.Normal)
    {
        var typeface   = new Typeface(fontFamily, fontStyle, fontWeight);
        using var textLayout = new TextLayout(text, typeface, fontSize, null);
        return new Size(textLayout.Width, textLayout.Height);
    }
}
```

这些工具类在控件实现中用于测量文本尺寸、计算布局空间等场景。

