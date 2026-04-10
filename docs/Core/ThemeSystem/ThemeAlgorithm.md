# AtomUI 主题系统 — 主题算法与调色板生成

> 本文档详细描述 AtomUI 主题算法（Default/Dark/Compact）的实现以及调色板生成算法，对应 Ant Design 5.0 中 `theme.algorithm` 的完整 C# 实现。

---

## 1. 主题算法概述

### 1.1 Ant Design 中的主题算法

Ant Design 5.0 提供三种内置算法：

| 算法 | Ant Design API | 功能 |
|------|---------------|------|
| 默认算法 | `theme.defaultAlgorithm` | 亮色主题（默认） |
| 暗色算法 | `theme.darkAlgorithm` | 暗色主题 |
| 紧凑算法 | `theme.compactAlgorithm` | 紧凑布局（缩小尺寸和间距） |

算法可以组合使用，例如 `algorithm: [theme.darkAlgorithm, theme.compactAlgorithm]` 产出暗色紧凑主题。

### 1.2 AtomUI 的实现

AtomUI 使用 **装饰器（Decorator）/ 链式（Chain）模式** 实现算法组合：

```csharp
public enum ThemeAlgorithm
{
    Default,    // 默认（亮色）
    Dark,       // 暗色
    Compact     // 紧凑
}
```

```
算法链构建示例（Dark + Compact）：

DefaultThemeVariantCalculator
        │
        │ 作为 baseCalculator 传入
        ▼
DarkThemeVariantCalculator(baseCalculator)
        │
        │ 作为 baseCalculator 传入
        ▼
CompactThemeVariantCalculator(baseCalculator)
        │
        │ Calculate(designToken)
        │   ├── _compositeGenerator.Calculate()  ← 先执行 Dark
        │   │       └── _compositeGenerator.Calculate()  ← Dark 先执行 Default
        │   └── 覆盖 Compact 特定的 Token
        ▼
最终 DesignToken
```

---

## 2. 算法接口与基类

### 2.1 IThemeVariantCalculator

```csharp
public interface IThemeVariantCalculator
{
    void Calculate(DesignToken designToken);
    Color ColorBgBase { get; }      // 计算后的背景基色
    Color ColorTextBase { get; }    // 计算后的文本基色
}
```

### 2.2 AbstractThemeVariantCalculator

所有算法的抽象基类，提供通用的颜色梯度计算方法：

```csharp
public abstract class AbstractThemeVariantCalculator : IThemeVariantCalculator
{
    protected IThemeVariantCalculator? _compositeGenerator;  // 装饰器链
    protected Color _colorBgBase;
    protected Color _colorTextBase;

    // 子类需实现：生成色板映射
    protected virtual ColorMap GenerateColorPalettes(Color baseColor) { ... }
    
    // 子类需实现：计算中性色（文本/填充/背景/边框）
    protected virtual void CalculateNeutralColorPalettes(...) { ... }
    
    // 核心方法：将 ColorMap 赋值到 DesignToken 的各色系
    protected void CalculateColorMapTokenValues(DesignToken designToken)
    {
        // 1. 为 Primary/Success/Warning/Error/Info/Link 生成色板
        var primaryColors = GenerateColorPalettes(colorPrimaryBase);
        var successColors = GenerateColorPalettes(colorSuccessBase);
        // ...
        
        // 2. 中性色计算
        CalculateNeutralColorPalettes(colorBgBase, colorTextBase, designToken);
        
        // 3. 将色板映射到 DesignToken 属性
        designToken.ColorPrimaryBg          = primaryColors.Color1;   // 1号色
        designToken.ColorPrimaryBgHover     = primaryColors.Color2;   // 2号色
        designToken.ColorPrimaryBorder      = primaryColors.Color3;   // 3号色
        designToken.ColorPrimaryBorderHover = primaryColors.Color4;   // 4号色
        designToken.ColorPrimaryHover       = primaryColors.Color5;   // 5号色
        designToken.ColorPrimary            = primaryColors.Color6;   // 6号色 (主色)
        designToken.ColorPrimaryActive      = primaryColors.Color7;   // 7号色
        // ... 同理 Success/Warning/Error/Info/Link
    }
}
```

### 2.3 ColorMap

10 级色阶的数据容器：

```csharp
public record ColorMap
{
    public Color Color1 { get; set; }   // 1号色 — 最浅
    public Color Color2 { get; set; }   // 2号色
    public Color Color3 { get; set; }   // 3号色
    public Color Color4 { get; set; }   // 4号色
    public Color Color5 { get; set; }   // 5号色
    public Color Color6 { get; set; }   // 6号色 — 主色
    public Color Color7 { get; set; }   // 7号色
    public Color Color8 { get; set; }   // 8号色
    public Color Color9 { get; set; }   // 9号色
    public Color Color10 { get; set; }  // 10号色 — 最深
}
```

---

## 3. DefaultThemeVariantCalculator（亮色算法）

### 3.1 职责

- 设置默认的 `ColorBgBase = White(255,255,255)` 和 `ColorTextBase = Black(0,0,0)`
- 生成所有预设颜色的色板
- 计算完整的 Map Token（颜色/字体/尺寸/高度/样式）

### 3.2 Calculate() 流程

```csharp
public override void Calculate(DesignToken designToken)
{
    // 1. 用户可能已自定义 ColorBgBase / ColorTextBase
    if (designToken.ColorBgBase.HasValue)
        _colorBgBase = designToken.ColorBgBase.Value;
    if (designToken.ColorTextBase.HasValue)
        _colorTextBase = designToken.ColorTextBase.Value;

    // 2. 为 14 种预设颜色生成色板
    SetupColorPalettes(designToken);
    
    // 3. 计算所有色系的 Map Token（10 级色阶 → DesignToken 属性）
    CalculateColorMapTokenValues(designToken);
    
    // 4. 计算非颜色类 Map Token
    CalculatorUtils.CalculateFontMapTokenValues(designToken);       // 字体梯度
    CalculatorUtils.CalculateSizeMapTokenValues(designToken);       // 尺寸梯度
    CalculatorUtils.CalculateControlHeightMapTokenValues(designToken); // 高度梯度
    CalculatorUtils.CalculateStyleMapTokenValues(designToken);      // 圆角/动效梯度
}
```

### 3.3 亮色模式色板映射

```csharp
protected override ColorMap GenerateColorPalettes(Color baseColor)
{
    var colors = PaletteGenerator.GeneratePalette(baseColor);
    // 亮色模式：直接使用色板序列
    return new ColorMap
    {
        Color1  = colors[0],   // 最浅
        Color2  = colors[1],
        Color3  = colors[2],
        Color4  = colors[3],
        Color5  = colors[4],
        Color6  = colors[5],   // 主色
        Color7  = colors[6],   // 最深
        Color8  = colors[4],   // 文本悬浮 = 5号色
        Color9  = colors[5],   // 文本色 = 6号色
        Color10 = colors[6],   // 文本激活 = 7号色
    };
}
```

### 3.4 亮色模式中性色计算

```csharp
// 文本色 — 黑色基底加透明度
designToken.ColorText           = AlphaColor(colorTextBase, 0.88);
designToken.ColorTextSecondary  = AlphaColor(colorTextBase, 0.65);
designToken.ColorTextTertiary   = AlphaColor(colorTextBase, 0.45);
designToken.ColorTextQuaternary = AlphaColor(colorTextBase, 0.25);

// 填充色 — 黑色基底加透明度
designToken.ColorFill           = AlphaColor(colorTextBase, 0.15);
designToken.ColorFillSecondary  = AlphaColor(colorTextBase, 0.06);
designToken.ColorFillTertiary   = AlphaColor(colorTextBase, 0.04);
designToken.ColorFillQuaternary = AlphaColor(colorTextBase, 0.02);

// 背景色 — 白色基底降低亮度
designToken.ColorBgLayout    = SolidColor(colorBgBase, 4);    // 微暗
designToken.ColorBgContainer = SolidColor(colorBgBase, 0);    // 白色
designToken.ColorBgElevated  = SolidColor(colorBgBase, 0);    // 白色

// 边框色 — 白色基底降低亮度
designToken.ColorBorder          = SolidColor(colorBgBase, 15);
designToken.ColorBorderSecondary = SolidColor(colorBgBase, 6);
```

---

## 4. DarkThemeVariantCalculator（暗色算法）

### 4.1 职责

- 装饰 Default 算法（先执行 Default）
- 覆盖为暗色模式的 `ColorBgBase = Black(0,0,0)` 和 `ColorTextBase = White(255,255,255)`
- 使用暗色色板算法生成色阶
- 调整选中状态背景色

### 4.2 Calculate() 流程

```csharp
public override void Calculate(DesignToken designToken)
{
    // 1. 先执行链中的前一个算法（Default）
    _compositeGenerator!.Calculate(designToken);
    
    // 2. 覆盖基色
    _colorBgBase   = Color.FromRgb(0, 0, 0);     // 黑色
    _colorTextBase = Color.FromRgb(255, 255, 255); // 白色
    
    // 3. 重新生成暗色色板
    SetupColorPalettes(designToken);
    CalculateColorMapTokenValues(designToken);
    
    // 4. 特殊调整：暗色模式下选中背景色使用 Border 色
    designToken.ColorPrimaryBg      = designToken.ColorPrimaryBorder;
    designToken.ColorPrimaryBgHover = designToken.ColorPrimaryBorderHover;
}
```

### 4.3 暗色模式色板映射

暗色模式的 ColorMap 映射与亮色不同（色号顺序有调整）：

```csharp
protected override ColorMap GenerateColorPalettes(Color baseColor)
{
    var colors = PaletteGenerator.GeneratePalette(baseColor, new PaletteGenerateOption
    {
        ThemeVariant = ThemeVariant.Dark    // 暗色模式色板
    });
    return new ColorMap
    {
        Color1  = colors[0],
        Color2  = colors[1],
        Color3  = colors[2],
        Color4  = colors[3],
        Color5  = colors[6],   // ← 注意：暗色反转
        Color6  = colors[5],
        Color7  = colors[4],   // ← 注意：暗色反转
        Color8  = colors[6],
        Color9  = colors[5],
        Color10 = colors[4],
    };
}
```

### 4.4 暗色模式中性色计算

```csharp
// 文本色 — 白色基底加透明度
designToken.ColorText           = AlphaColor(colorTextBase, 0.85);
designToken.ColorTextSecondary  = AlphaColor(colorTextBase, 0.65);
designToken.ColorTextTertiary   = AlphaColor(colorTextBase, 0.45);
designToken.ColorTextQuaternary = AlphaColor(colorTextBase, 0.25);

// 填充色 — 白色基底加透明度（暗色模式比亮色更高）
designToken.ColorFill           = AlphaColor(colorTextBase, 0.18);  // vs 亮色 0.15
designToken.ColorFillSecondary  = AlphaColor(colorTextBase, 0.12);  // vs 亮色 0.06
designToken.ColorFillTertiary   = AlphaColor(colorTextBase, 0.08);  // vs 亮色 0.04
designToken.ColorFillQuaternary = AlphaColor(colorTextBase, 0.04);  // vs 亮色 0.02

// 实心背景色（暗色模式独有）
designToken.ColorBgSolid       = AlphaColor(colorTextBase, 1);
designToken.ColorBgSolidHover  = AlphaColor(colorTextBase, 0.75);
designToken.ColorBgSolidActive = AlphaColor(colorTextBase, 0.95);

// 背景色 — 黑色基底增加亮度
designToken.ColorBgElevated  = SolidColor(colorBgBase, 12);   // 浮层比容器更亮
designToken.ColorBgContainer = SolidColor(colorBgBase, 8);
designToken.ColorBgLayout    = SolidColor(colorBgBase, 0);    // 最暗
designToken.ColorBgSpotlight = SolidColor(colorBgBase, 26);

// 边框色
designToken.ColorBorder          = SolidColor(colorBgBase, 26);
designToken.ColorBorderSecondary = SolidColor(colorBgBase, 19);
```

---

## 5. CompactThemeVariantCalculator（紧凑算法）

### 5.1 职责

- 装饰前一个算法（Default 或 Dark）
- 缩小尺寸梯度和控件高度
- 不影响颜色

### 5.2 Calculate() 流程

```csharp
public override void Calculate(DesignToken designToken)
{
    // 1. 先执行链中的前一个算法
    _compositeGenerator!.Calculate(designToken);
    
    _colorBgBase   = _compositeGenerator.ColorBgBase;
    _colorTextBase = _compositeGenerator.ColorTextBase;
    
    // 2. 缩小控件高度
    var controlHeight = designToken.ControlHeight - 4;  // 32 → 28
    
    // 3. 缩小尺寸梯度
    CalculateCompactSizeMapTokenValues(designToken);
    
    // 4. 重新计算字体梯度（保持字体大小不变，但间距缩小）
    CalculatorUtils.CalculateFontMapTokenValues(designToken);
    
    // 5. 应用缩小的控件高度，重新计算高度梯度
    designToken.ControlHeight = controlHeight;
    CalculatorUtils.CalculateControlHeightMapTokenValues(designToken);
}
```

### 5.3 紧凑尺寸计算

```csharp
private void CalculateCompactSizeMapTokenValues(DesignToken designToken)
{
    var sizeUnit        = designToken.SizeUnit;    // 4
    var sizeStep        = designToken.SizeStep;    // 4
    var compactSizeStep = sizeStep - 2;            // 2（关键：步长减 2）
    
    // 对比默认值：
    //                    Default    Compact
    designToken.SizeXXL = sizeUnit * (compactSizeStep + 10); // 48  →  48
    designToken.SizeXL  = sizeUnit * (compactSizeStep + 6);  // 32  →  32
    designToken.SizeLG  = sizeUnit * (compactSizeStep + 2);  // 24  →  16
    designToken.SizeMD  = sizeUnit * (compactSizeStep + 2);  // 20  →  16
    designToken.SizeMS  = sizeUnit * (compactSizeStep + 1);  // 16  →  12
    designToken.Size    = sizeUnit * compactSizeStep;         // 16  →  8
    designToken.SizeSM  = sizeUnit * compactSizeStep;         // 12  →  8
    designToken.SizeXS  = sizeUnit * (compactSizeStep - 1);  // 8   →  4
    designToken.SizeXXS = sizeUnit * (compactSizeStep - 1);  // 4   →  4
}
```

---

## 6. 算法组合与主题变体

### 6.1 四种算法组合

AtomUI 对每个主题定义文件自动生成 4 种变体：

| 算法组合 | ThemeVariant ID | isDarkMode | 算法链 |
|----------|-----------------|------------|--------|
| Default | `DaybreakBlue` | false | Default |
| Default + Dark | `DaybreakBlue-Dark` | true | Default → Dark |
| Default + Dark + Compact | `DaybreakBlue-Dark-Compact` | true | Default → Dark → Compact |
| Default + Compact | `DaybreakBlue-Compact` | false | Default → Compact |

### 6.2 算法链构建

```csharp
// ThemeManager.CreateThemeVariantCalculator()
IThemeVariantCalculator? baseCalculator = null;
IThemeVariantCalculator? calculator     = null;

foreach (var algorithm in algorithms)  // [Default, Dark, Compact]
{
    calculator     = CreateThemeVariantCalculator(algorithm, baseCalculator);
    baseCalculator = calculator;
}
// 最终 calculator 是链式最后一个，调用 Calculate() 会依次执行整条链
```

### 6.3 自定义算法

通过实现 `IThemeVariantCalculatorFactory` 接口可替换内置算法：

```csharp
public interface IThemeVariantCalculatorFactory
{
    IThemeVariantCalculator Create(ThemeAlgorithm algorithm, IThemeVariantCalculator? baseAlgorithm);
}

// 使用
builder.WithThemeVariantCalculatorFactory(new CustomCalculatorFactory());
```

---

## 7. 调色板生成算法（PaletteGenerator）

### 7.1 概述

`PaletteGenerator` 对标 Ant Design 的 `@ant-design/colors` 包，实现了基于 **HSV 色彩空间** 的 10 级色阶生成算法。

### 7.2 算法参数

```csharp
public const int HUE_STEP = 2;               // 色相阶梯
public const float SATURATION_STEP1 = 0.16f;  // 饱和度阶梯（浅色）
public const float SATURATION_STEP2 = 0.05f;  // 饱和度阶梯（深色）
public const float BRIGHTNESS_STEP1 = 0.05f;  // 亮度阶梯（浅色）
public const float BRIGHTNESS_STEP2 = 0.15f;  // 亮度阶梯（深色）
public const int LIGHT_COLOR_COUNT = 5;        // 主色上方浅色数量
public const int DARK_COLOR_COUNT = 4;         // 主色下方深色数量
```

### 7.3 生成流程

```
输入：基础颜色 (如 #1677ff)
         │
         ├── 转换为 HSV 色彩空间
         │
         ├── 向浅色方向生成 5 级
         │   ├── 色相：H ± HUE_STEP * i（根据色温方向决定 +/-）
         │   ├── 饱和度：S - SATURATION_STEP1 * i（递减）
         │   └── 亮度：V + BRIGHTNESS_STEP1 * i（递增）
         │
         ├── 主色（第 6 级）= 输入颜色
         │
         ├── 向深色方向生成 4 级
         │   ├── 色相：H ± HUE_STEP * i（方向相反）
         │   ├── 饱和度：S + SATURATION_STEP2 * i（递增）
         │   └── 亮度：V - BRIGHTNESS_STEP2 * i（递减）
         │
         └── 输出：10 级色阶 [浅1, 浅2, 浅3, 浅4, 浅5, 主色, 深1, 深2, 深3, 深4]
```

### 7.4 色相方向规则

```csharp
// 60°-240° 范围内（暖色调）：浅色向左旋转，深色向右旋转
// 其他范围（冷色调）：浅色向右旋转，深色向左旋转
if (Math.Round(hsvColor.H) >= 60d && Math.Round(hsvColor.H) <= 240d)
{
    hue = isLight ? H - HUE_STEP * i : H + HUE_STEP * i;
}
else
{
    hue = isLight ? H + HUE_STEP * i : H - HUE_STEP * i;
}
```

### 7.5 暗色模式色板

暗色模式不是简单反转，而是使用**颜色混合**算法：

```csharp
// 暗色色板映射表（索引 + 混合透明度）
sm_darkColorMap = [
    { Index = 7, Opacity = 0.15f },  // 1号色：深色 + 15% 色板7号
    { Index = 6, Opacity = 0.25f },  // 2号色：深色 + 25% 色板6号
    { Index = 5, Opacity = 0.3f },   // 3号色
    { Index = 5, Opacity = 0.45f },  // 4号色
    { Index = 5, Opacity = 0.65f },  // 5号色
    { Index = 5, Opacity = 0.85f },  // 6号色（主色）
    { Index = 4, Opacity = 0.9f },   // 7号色
    { Index = 3, Opacity = 0.95f },  // 8号色
    { Index = 2, Opacity = 0.97f },  // 9号色
    { Index = 1, Opacity = 0.98f },  // 10号色
];

// 混合公式：RGB 线性插值
// result = bg_color + (palette_color - bg_color) * opacity
// 默认背景色 = #141414
```

### 7.6 预设颜色

AtomUI 预定义了 14 种预设颜色（与 Ant Design 一致）：

| 枚举值 | 颜色 | HEX |
|--------|------|-----|
| `Red` | 红色 | `#F5222D` |
| `Volcano` | 火山色 | `#FA541C` |
| `Orange` | 橙色 | `#FA8C16` |
| `Gold` | 金色 | `#FAAD14` |
| `Yellow` | 黄色 | `#FADB14` |
| `Lime` | 青柠 | `#A0D911` |
| `Green` | 绿色 | `#52C41A` |
| `Cyan` | 青色 | `#13C2C2` |
| `Blue` | 蓝色 | `#1677FF` |
| `GeekBlue` | 极客蓝 | `#2F54EB` |
| `Purple` | 紫色 | `#722ED1` |
| `Pink` | 粉色 | `#EB2F96` |
| `Magenta` | 洋红 | `#EB2F96` |
| `Grey` | 灰色 | `#666666` |

每种预设颜色在主题加载时会自动生成亮色和暗色两套色板（通过 `SetupColorPalettes()`），存储在 `DesignToken.ColorPalettes` 字典中。

---

## 8. CalculatorUtils 工具函数

`CalculatorUtils` 是独立于具体算法的静态工具类，提供非颜色类 Map Token 的计算：

### 8.1 字体梯度计算

```csharp
// 基于自然对数曲线的字体大小序列
public static IList<FontSizeInfo> CalculateFontSize(double baseValue)
{
    for (var index = 0; index < 10; ++index)
    {
        var i        = index - 1;
        var baseSize = baseValue * Math.Pow(2.71828, i / 5.0);
        // 偶数对齐
        fontSizes.Add((int)(Math.Floor(intSize / 2.0) * 2));
    }
    fontSizes[1] = baseValue;  // 确保 index=1 是基础字号
}

// 行高公式
public static double CalculateLineHeight(double fontSize)
{
    return (fontSize + 8) / fontSize;
}
```

### 8.2 圆角梯度计算

基于 `BorderRadius` (Seed) 通过查表（LUT）规则派生：

```
BorderRadius (Seed)    XS    SM    LG    Outer
────────────────────   ──    ──    ──    ─────
< 2                    base  base  base  base
2 ≤ r < 5             1     base  base  base
5 ≤ r < 6             1     4     r+1   4
6 ≤ r < 7             2     4     r+2   4
7 ≤ r < 8             2     5     r+2   4
8 ≤ r < 14            2     6     r+2   6
14 ≤ r < 16           2     7     r+2   6
≥ 16                  2     8     16    6
```

### 8.3 动效时长梯度

```csharp
MotionDurationFast     = MotionBase + MotionUnit * 1;  // 100ms
MotionDurationMid      = MotionBase + MotionUnit * 2;  // 200ms
MotionDurationSlow     = MotionBase + MotionUnit * 3;  // 300ms
MotionDurationVerySlow = MotionBase + MotionUnit * 8;  // 800ms
```

