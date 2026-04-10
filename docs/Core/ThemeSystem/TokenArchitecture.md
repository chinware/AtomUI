# AtomUI 主题系统 — Design Token 四层架构详解

> 本文档详细描述 AtomUI Design Token 体系的四层架构设计，完整对应 Ant Design 5.0 的 [Design Token](https://ant.design/docs/react/customize-theme-cn) 系统。

---

## 1. Token 四层推导链

Ant Design 5.0 将 Design Token 分解为三个全局层级加一个组件层级，AtomUI 完整实现了这一架构：

```
┌─────────────────────────────────────────────────────────────────────┐
│  第 1 层：Seed Token（种子变量）                                      │
│  定义文件：DesignToken.Seed.cs                                       │
│  数量：约 20 个                                                       │
│  特征：设计意图的最小集合，修改一个即可全局级联                          │
│  示例：ColorPrimary, ColorSuccess, FontSize, BorderRadius            │
│  Ant Design 对应：SeedToken interface                                │
└────────────────────────────┬────────────────────────────────────────┘
                             │
                             │  IThemeVariantCalculator.Calculate()
                             │  （主题算法自动推导）
                             ▼
┌─────────────────────────────────────────────────────────────────────┐
│  第 2 层：Map Token（梯度变量）                                       │
│  定义文件：DesignToken.Color*Map.cs, DesignToken.FontMap.cs,          │
│           DesignToken.SizeMap.cs, DesignToken.HeightMap.cs,           │
│           DesignToken.StyleMap.cs, DesignToken.ColorNeutralMap.cs     │
│  数量：约 100+ 个                                                     │
│  特征：由 Seed Token 经算法系统派生的梯度序列                           │
│  示例：ColorPrimaryBg(1号色), ColorPrimaryHover(5号色), FontSizeLG    │
│  Ant Design 对应：MapToken interface                                  │
└────────────────────────────┬────────────────────────────────────────┘
                             │
                             │  DesignToken.CalculateAliasTokenValues()
                             │  （别名计算 — 直接赋值 / 透明度混合）
                             ▼
┌─────────────────────────────────────────────────────────────────────┐
│  第 3 层：Alias Token（别名变量 / 语义变量）                           │
│  定义文件：DesignToken.Alias.cs                                       │
│  数量：约 100+ 个                                                     │
│  特征：Map Token 的语义化封装，面向组件批量消费                         │
│  示例：ColorTextDisabled, ColorBgContainer, PaddingContentHorizontal │
│  Ant Design 对应：AliasToken interface                                │
└────────────────────────────┬────────────────────────────────────────┘
                             │
                             │  AbstractControlDesignToken.CalculateTokenValues()
                             │  （组件 Token 从 SharedToken 派生）
                             ▼
┌─────────────────────────────────────────────────────────────────────┐
│  第 4 层：Component Token（组件变量）                                  │
│  定义文件：各组件的 *Token.cs（如 ButtonToken.cs, TagToken.cs）        │
│  数量：每个组件 5-30 个不等                                            │
│  特征：组件专属 Token，从 SharedToken 派生，可被用户单独覆盖            │
│  示例：ButtonToken.PrimaryColor, TagToken.DefaultBg                  │
│  Ant Design 对应：ComponentToken interface (theme.components.Button)  │
└─────────────────────────────────────────────────────────────────────┘
```

**关键概念：SharedToken**

在 AtomUI 中，`DesignToken` 类（即 Ant Design 中 Seed + Map + Alias 的统一体）被称为 `SharedToken`。它是 `Theme` 级别的全局变量集，所有 Component Token 通过 `SharedToken` 属性访问全局 Token 值。

---

## 2. Seed Token 详解

### 2.1 定义方式

Seed Token 定义在 `DesignToken.Seed.cs` 中，使用 `[DesignTokenKind(DesignTokenKind.Seed)]` 标注：

```csharp
[GlobalDesignToken]
public partial class DesignToken
{
    [DesignTokenKind(DesignTokenKind.Seed)]
    public Color ColorPrimary { get; set; }        // 品牌主色 — #1677ff

    [DesignTokenKind(DesignTokenKind.Seed)]
    public Color ColorSuccess { get; set; }        // 成功色 — #52c41a

    [DesignTokenKind(DesignTokenKind.Seed)]
    public double FontSize { get; set; } = 14;     // 基础字号

    [DesignTokenKind(DesignTokenKind.Seed)]
    public CornerRadius BorderRadius { get; set; }  // 基础圆角

    [DesignTokenKind(DesignTokenKind.Seed)]
    public double ControlHeight { get; set; } = 32; // 基础控件高度

    [DesignTokenKind(DesignTokenKind.Seed)]
    public bool EnableMotion { get; set; } = true;  // 是否开启动画
    
    // ... 更多 Seed Token
}
```

### 2.2 完整 Seed Token 列表

| Token 名称 | 类型 | 默认值 | 说明 |
|------------|------|--------|------|
| `ColorPrimary` | `Color` | `#1677ff` | 品牌主色 |
| `ColorSuccess` | `Color` | `#52c41a` | 成功色 |
| `ColorWarning` | `Color` | `#faad14` | 警戒色 |
| `ColorError` | `Color` | `#ff4d4f` | 错误色 |
| `ColorInfo` | `Color` | `#1677ff` | 信息色 |
| `ColorTextBase` | `Color?` | `null` | 基础文本色（用于派生，不直接使用） |
| `ColorBgBase` | `Color?` | `null` | 基础背景色（用于派生，不直接使用） |
| `ColorLink` | `Color?` | `null` | 超链接颜色 |
| `ColorTransparent` | `Color` | `Transparent` | 透明色 |
| `FontFamily` | `FontFamily?` | Alibaba Sans 族 | 字体家族 |
| `FontSize` | `double` | `14` | 默认字号 |
| `LineWidth` | `double` | `1` | 基础线宽 |
| `LineType` | `LineStyle` | `Solid` | 线条样式 |
| `BorderRadius` | `CornerRadius` | `6` | 基础圆角 |
| `SizeUnit` | `double` | `4` | 尺寸变化单位 |
| `SizeStep` | `double` | `4` | 尺寸步长 |
| `SizePopupArrow` | `double` | `16` | 弹出箭头大小 |
| `ControlHeight` | `double` | `32` | 基础控件高度 |
| `ZIndexBase` | `int` | `0` | 基础 z-index |
| `ZIndexPopupBase` | `int` | `1000` | 浮层 z-index |
| `OpacityImage` | `double` | `1.0` | 图片不透明度 |
| `MotionUnit` | `int` | `100` | 动画时长单位 (ms) |
| `MotionBase` | `int` | `0` | 动画基础时长 (ms) |
| `EnableMotion` | `bool` | `true` | 是否开启动画 |
| `EnableWaveSpirit` | `bool` | `true` | 是否开启波浪动画 |

### 2.3 设计意图

Seed Token 是整个 Token 体系的**源头**。用户只需修改极少数 Seed Token（典型场景只需改 `ColorPrimary`），算法就能自动派生出完整的 Map Token 和 Alias Token，保证设计一致性。

这与 Ant Design 官方文档中的说法一致：

> *"种子 Token 是主题中的一级变量，比如 `colorPrimary`、`borderRadius`。通过修改种子 Token 来自动生成 Map Token 和 Alias Token，进而影响所有使用到这些 Token 的组件。"*

---

## 3. Map Token 详解

### 3.1 概述

Map Token 是由 Seed Token 通过**主题算法**自动推导的梯度变量。在 Ant Design 中，Map Token 形成了完整的色彩体系、尺寸体系、字体体系。

### 3.2 颜色梯度体系（10 号色系统）

每个功能色（Primary / Success / Warning / Error / Info）都从种子颜色自动生成 10 个梯度色，编号 1-10：

```
Seed Color (6号色)
   │
   ├── PaletteGenerator.GeneratePalette()
   │
   ▼
┌───────────────────────────────────────────────────────────┐
│  1号色  浅色背景      ColorPrimaryBg       (#e6f4ff)     │
│  2号色  浅色背景悬浮   ColorPrimaryBgHover  (#bae0ff)     │
│  3号色  描边色        ColorPrimaryBorder    (#91caff)     │
│  4号色  描边色悬浮    ColorPrimaryBorderHover(#69b1ff)     │
│  5号色  悬浮态        ColorPrimaryHover      (#4096ff)     │
│  6号色  主色 ★        ColorPrimary          (#1677ff)     │
│  7号色  激活态        ColorPrimaryActive     (#0958d9)     │
│  8号色  文本悬浮态    ColorPrimaryTextHover  (#4096ff)     │
│  9号色  文本色        ColorPrimaryText       (#1677ff)     │
│ 10号色  文本激活态    ColorPrimaryTextActive  (#0958d9)     │
└───────────────────────────────────────────────────────────┘
```

**语义映射规则：**

| 色号 | 语义 | 典型用途 |
|------|------|---------|
| 1 号色 | `*Bg` | 选中状态的浅背景 |
| 2 号色 | `*BgHover` | 浅背景的悬浮态 |
| 3 号色 | `*Border` | 描边色 |
| 4 号色 | `*BorderHover` | 描边悬浮态 |
| 5 号色 | `*Hover` | 主色悬浮态 |
| 6 号色 | 主色 (= Seed) | 主按钮、主色调 |
| 7 号色 | `*Active` | 主色激活态（按下） |
| 8 号色 | `*TextHover` | 文本色悬浮态 |
| 9 号色 | `*Text` | 文本色 |
| 10 号色 | `*TextActive` | 文本色激活态 |

### 3.3 中性色体系

中性色不是从单一种子色派生，而是由 `ColorBgBase`（背景基色）和 `ColorTextBase`（文本基色）通过透明度/明度运算生成：

**文本色梯度：**

| Token | 亮色模式 | 暗色模式 |
|-------|---------|---------|
| `ColorText` | rgba(0,0,0,0.88) | rgba(255,255,255,0.85) |
| `ColorTextSecondary` | rgba(0,0,0,0.65) | rgba(255,255,255,0.65) |
| `ColorTextTertiary` | rgba(0,0,0,0.45) | rgba(255,255,255,0.45) |
| `ColorTextQuaternary` | rgba(0,0,0,0.25) | rgba(255,255,255,0.25) |

**填充色梯度：**

| Token | 亮色模式 | 暗色模式 |
|-------|---------|---------|
| `ColorFill` | rgba(0,0,0,0.15) | rgba(255,255,255,0.18) |
| `ColorFillSecondary` | rgba(0,0,0,0.06) | rgba(255,255,255,0.12) |
| `ColorFillTertiary` | rgba(0,0,0,0.04) | rgba(255,255,255,0.08) |
| `ColorFillQuaternary` | rgba(0,0,0,0.02) | rgba(255,255,255,0.04) |

**背景色梯度：**

| Token | 亮色模式 | 暗色模式 |
|-------|---------|---------|
| `ColorBgLayout` | 白色微暗(4%) | 纯黑 |
| `ColorBgContainer` | 白色 | 黑色微亮(8%) |
| `ColorBgElevated` | 白色 | 黑色微亮(12%) |
| `ColorBgSpotlight` | rgba(0,0,0,0.85) | 黑色微亮(26%) |

### 3.4 字体梯度

从 `FontSize` (Seed) 通过自然对数曲线函数 `e^(i/5)` 派生字体大小序列：

```csharp
// CalculatorUtils.CalculateFontSize()
var baseSize = baseValue * Math.Pow(2.71828, i / 5.0);
```

生成的字体梯度：

| Token | 值 (FontSize=14 时) |
|-------|---------------------|
| `FontSizeSM` | 12 |
| `FontSize` | 14 (Seed) |
| `FontSizeLG` | 16 |
| `FontSizeXL` | 20 |
| `FontSizeHeading5` | 16 |
| `FontSizeHeading4` | 20 |
| `FontSizeHeading3` | 24 |
| `FontSizeHeading2` | 30 |
| `FontSizeHeading1` | 38 |

行高通过公式 `(fontSize + 8) / fontSize` 自动计算。

### 3.5 尺寸梯度

从 `SizeUnit`(4) 和 `SizeStep`(4) 派生：

```csharp
// CalculatorUtils.CalculateSizeMapTokenValues()
designToken.SizeXXL = sizeUnit * (sizeStep + 8); // 48
designToken.SizeXL  = sizeUnit * (sizeStep + 4); // 32
designToken.SizeLG  = sizeUnit * (sizeStep + 2); // 24
designToken.SizeMD  = sizeUnit * (sizeStep + 1); // 20
designToken.Size    = sizeUnit * sizeStep;        // 16
designToken.SizeSM  = sizeUnit * (sizeStep - 1); // 12
designToken.SizeXS  = sizeUnit * (sizeStep - 2); // 8
designToken.SizeXXS = sizeUnit * (sizeStep - 3); // 4
```

### 3.6 控件高度梯度

从 `ControlHeight`(32) 通过比例派生：

```csharp
designToken.ControlHeightSM = controlHeight * 0.75; // 24
designToken.ControlHeightXS = controlHeight * 0.5;  // 16
designToken.ControlHeightLG = controlHeight * 1.25; // 40
```

### 3.7 样式梯度

包含圆角梯度、线宽梯度、动效时长梯度：

```csharp
// 动效时长
designToken.MotionDurationFast     = motionBase + motionUnit;      // 100ms
designToken.MotionDurationMid      = motionBase + motionUnit * 2;  // 200ms
designToken.MotionDurationSlow     = motionBase + motionUnit * 3;  // 300ms
designToken.MotionDurationVerySlow = motionBase + motionUnit * 8;  // 800ms

// 圆角 (基于 BorderRadius=6 的 LUT)
designToken.BorderRadiusXS    = 2;   // 最小圆角
designToken.BorderRadiusSM    = 4;   // 小圆角
designToken.BorderRadius      = 6;   // 基础圆角 (Seed)
designToken.BorderRadiusLG    = 8;   // 大圆角
designToken.BorderRadiusOuter = 4;   // 外部圆角
```

---

## 4. Alias Token 详解

### 4.1 概述

Alias Token 是 Map Token 的**语义化封装**。它们不引入新的计算逻辑，而是将 Map Token 赋予更具业务含义的名称，便于组件批量消费。

计算逻辑在 `DesignToken.CalculateAliasTokenValues()` 中：

```csharp
internal void CalculateAliasTokenValues()
{
    // 背景相关
    ColorFillContent         = ColorFillSecondary;
    ColorFillContentHover    = ColorFill;
    ColorBgContainerDisabled = ColorFillTertiary;
    
    // 文本相关
    ColorTextPlaceholder = ColorTextQuaternary;
    ColorTextDisabled    = ColorTextQuaternary;
    ColorTextHeading     = ColorText;
    ColorTextLabel       = ColorTextSecondary;
    ColorTextDescription = ColorTextTertiary;
    
    // 控件交互
    ControlItemBgHover       = ColorFillTertiary;
    ControlItemBgActive      = ColorPrimaryBg;
    ControlItemBgActiveHover = ColorPrimaryBgHover;
    
    // 间距 (从 Size 梯度映射到 Padding/Margin/Spacing)
    PaddingContentHorizontal   = SizeMS;
    PaddingContentVertical     = SizeSM;
    ...
}
```

### 4.2 Alias Token 分类

#### 4.2.1 语义颜色

| Token | 来源 | 语义 |
|-------|------|------|
| `ColorTextPlaceholder` | `ColorTextQuaternary` | 占位文本颜色 |
| `ColorTextDisabled` | `ColorTextQuaternary` | 禁用文本颜色 |
| `ColorTextHeading` | `ColorText` | 标题文本颜色 |
| `ColorTextLabel` | `ColorTextSecondary` | 标签文本颜色 |
| `ColorTextDescription` | `ColorTextTertiary` | 描述文本颜色 |
| `ColorTextLightSolid` | `ColorWhite` | 深色背景上的文本 |
| `ColorHighlight` | `ColorError` | 高亮颜色 |
| `ColorIcon` | `ColorTextTertiary` | 弱操作图标颜色 |
| `ColorIconHover` | `ColorText` | 图标悬浮色 |
| `ColorBgContainerDisabled` | `ColorFillTertiary` | 禁用容器背景 |
| `ColorSplit` | 混合计算 | 分割线颜色 |
| `ColorControlOutline` | 混合计算 | 输入组件外轮廓 |
| `ColorErrorOutline` | 混合计算 | 错误状态外轮廓 |
| `ColorWarningOutline` | 混合计算 | 警告状态外轮廓 |

#### 4.2.2 间距与布局

| Token | 来源 | 语义 |
|-------|------|------|
| `PaddingXXS` ~ `PaddingXL` | `SizeXXS` ~ `SizeXL` | 内间距梯度（Thickness） |
| `UniformlyPaddingXXS` ~ `UniformlyPaddingXL` | `SizeXXS` ~ `SizeXL` | 统一内间距（double） |
| `MarginXXS` ~ `MarginXXL` | `SizeXXS` ~ `SizeXXL` | 外边距梯度 |
| `SpacingXXS` ~ `SpacingXXL` | `SizeXXS` ~ `SizeXXL` | 布局间距梯度 |
| `PaddingContentHorizontal*` | `SizeMS` / `SizeSM` / `SizeLG` | 内容区水平内间距 |
| `PaddingContentVertical*` | `SizeSM` / `SizeXS` / `SizeMS` | 内容区垂直内间距 |

#### 4.2.3 控件交互

| Token | 来源 | 语义 |
|-------|------|------|
| `ControlItemBgHover` | `ColorFillTertiary` | 列表项悬浮背景 |
| `ControlItemBgActive` | `ColorPrimaryBg` | 列表项选中背景 |
| `ControlItemBgActiveHover` | `ColorPrimaryBgHover` | 选中项悬浮背景 |
| `ControlItemBgActiveDisabled` | `ColorFill` | 禁用选中项背景 |
| `ControlInteractiveSize` | `ControlHeight / 2` | 控件交互区大小 |
| `ControlOutlineWidth` | `LineWidth * 2` | 外轮廓线宽 |
| `LineWidthFocus` | `LineWidth * 2` | 聚焦态线宽 |

#### 4.2.4 阴影

| Token | 说明 |
|-------|------|
| `BoxShadows` | 一级阴影（3 层复合阴影） |
| `BoxShadowsSecondary` | 二级阴影 |
| `BoxShadowsTertiary` | 三级阴影 |

#### 4.2.5 响应式断点

| Token | 值 |
|-------|-----|
| `ScreenXS` | 480px |
| `ScreenSM` | 576px |
| `ScreenMD` | 768px |
| `ScreenLG` | 992px |
| `ScreenXL` | 1200px |
| `ScreenXXL` | 1600px |

---

## 5. Component Token 详解

### 5.1 定义规范

每个控件有自己的 Token 类，继承 `AbstractControlDesignToken`：

```csharp
[ControlDesignToken]
internal class ButtonToken : AbstractControlDesignToken
{
    // 必须有 const string ID
    public const string ID = "Button";
    
    // 必须有 static readonly ScopeProvider
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    /// <summary>
    /// 主要按钮文本颜色
    /// </summary>
    public Color PrimaryColor { get; set; }

    /// <summary>
    /// 按钮内间距
    /// </summary>
    public Thickness Padding { get; set; }

    // 构造函数必须调用 base(ID)
    public ButtonToken() : base(ID) { }

    // 从 SharedToken 派生所有值 — 禁止硬编码
    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        PrimaryColor = SharedToken.ColorTextLightSolid;
        Padding = new Thickness(
            SharedToken.PaddingContentHorizontal - SharedToken.LineWidth, 0);
    }

    // 返回 Source Generator 生成的 TokenKind 枚举类型
    protected override Type GetTokenKindType() => typeof(ButtonTokenKind);
}
```

### 5.2 Component Token 的 SharedToken 覆盖机制

Ant Design 5.0 允许为某个组件单独修改全局 Token，这在 AtomUI 中通过以下机制实现：

1. **默认情况**：组件的 `SharedToken` 指向全局的 `DesignToken` 实例。
2. **有组件级配置时**：
   - 克隆一份全局 `DesignToken`：`var copiedSharedToken = (DesignToken)_sharedToken.Clone()`
   - 在克隆副本上应用组件级的 Seed/Map/Alias 覆盖
   - 如果 `EnableAlgorithm = true`，重新运行算法推导
   - 将修改后的副本赋给组件 Token：`controlToken.AssignSharedToken(copiedSharedToken)`
3. **差值字典**：`BuildSharedResourceDeltaDictionary()` 计算组件级 SharedToken 与全局 SharedToken 的差异，生成差值 `ResourceDictionary`，在控件级别通过 `ControlTokenResourceScopeHost` 注入。

这完整对应了 Ant Design 中 `theme.components.Button = { colorPrimary: '#xxx' }` 的能力。

### 5.3 Token 值类型转换

从 XML/XAML 配置加载的 Token 值是字符串，需要转换为 C# 类型。`BuiltInTokenValueConverters.cs` 提供了内置转换器：

| 转换器 | 目标类型 |
|--------|---------|
| `StringTokenValueConverter` | `string` |
| `IntegerTokenValueConverter` | `int` |
| `DoubleTokenValueConverter` | `double` |
| `FloatTokenValueConverter` | `float` |
| `BoolTokenValueConverter` | `bool` |
| `ColorTokenValueConverter` | `Color` |
| `BoxShadowTokenValueConverter` | `BoxShadows` |
| `TextDecorationTokenValueConverter` | `TextDecorationInfo` |
| `LineStyleTokenValueConverter` | `LineStyle` |
| `ThicknessTokenValueConverter` | `Thickness` |
| `CornerRadiusTokenValueConverter` | `CornerRadius` |

转换器通过 `[TokenValueConverter]` 标注自动发现和注册。

---

## 6. Token 标注系统

### 6.1 Attribute 定义

| Attribute | 目标 | 作用 |
|-----------|------|------|
| `[GlobalDesignToken]` | `DesignToken` partial class | 标记为全局 Token 定义（Source Generator 识别） |
| `[ControlDesignToken]` | 组件 Token 类 | 标记为组件 Token（Source Generator 识别） |
| `[DesignTokenKind(kind)]` | Token 属性 | 标注 Token 所属层级（Seed/Map/Alias） |
| `[NotTokenDefinition]` | 属性 | 排除不是 Token 的属性 |
| `[TokenValueConverter]` | 转换器类 | 标记 Token 值转换器 |

### 6.2 Source Generator 生成物

`AtomUI.Generator` 中的 `TokenResourceKeyGenerator` 会扫描所有标注了 `[GlobalDesignToken]` 和 `[ControlDesignToken]` 的类，自动生成：

1. **`SharedTokenKind` 枚举** — 全局 Token 的资源键枚举
2. **`{Control}TokenKind` 枚举** — 每个组件 Token 的资源键枚举
3. **`ControlTokenTypePool` 类** — 所有组件 Token 类型的注册池

这些生成的枚举作为 `ResourceDictionary` 的键，确保 Token 访问的编译期类型安全。

---

## 7. Token 推导完整流程

以一个主题加载为例，完整的 Token 推导流程如下：

```
Theme.BuildThemeResource()
│
├── 1. 初始化 DesignToken（Seed Token 默认值）
│       ColorPrimary = #1677ff, FontSize = 14, ...
│
├── 2. 从 XML 加载用户自定义 Seed Token 覆盖
│       _sharedToken.LoadConfig(seedTokenConfig)
│       // 用户可能覆盖 ColorPrimary = #ff0000
│
├── 3. 主题算法计算 Map Token
│       calculator.Calculate(_sharedToken)
│       ├── PaletteGenerator.GeneratePalette(ColorPrimary)
│       │     → 生成主色 10 级色阶
│       ├── 同理生成 Success/Warning/Error/Info 色阶
│       ├── CalculateNeutralColorPalettes() → 中性色体系
│       ├── CalculateFontMapTokenValues() → 字体梯度
│       ├── CalculateSizeMapTokenValues() → 尺寸梯度
│       ├── CalculateControlHeightMapTokenValues() → 高度梯度
│       └── CalculateStyleMapTokenValues() → 圆角/动效梯度
│
├── 4. 从 XML 加载用户自定义 Map Token 覆盖
│       _sharedToken.LoadConfig(mapTokenConfig)
│
├── 5. 计算 Alias Token
│       _sharedToken.CalculateAliasTokenValues()
│       // ColorTextDisabled = ColorTextQuaternary, ...
│
├── 6. 从 XML 加载用户自定义 Alias Token 覆盖
│       _sharedToken.LoadConfig(aliasTokenConfig)
│
├── 7. SharedToken 写入 ResourceDictionary
│       _sharedToken.BuildResourceDictionary(ResourceDictionary)
│
├── 8. 收集所有注册的 ControlToken 类型，实例化
│       CollectControlTokens()
│
├── 9. 为每个 ControlToken 分配 SharedToken
│       ├── 无组件级配置 → 直接使用全局 SharedToken
│       └── 有组件级配置 → 克隆 SharedToken → 应用覆盖 → 可选重新算法推导
│
├── 10. 计算每个 ControlToken 的值
│        controlToken.CalculateTokenValues(isDarkMode)
│
├── 11. 从 XML 加载用户自定义 ControlToken 覆盖
│        controlToken.LoadConfig(componentTokenConfig)
│
├── 12. ControlToken 写入 ResourceDictionary
│        controlToken.BuildResourceDictionary(ResourceDictionary)
│
└── 13. 有组件级配置的 ControlToken，构建差值字典
         controlToken.BuildSharedResourceDeltaDictionary()
```

---

## 8. 与 Ant Design 的 Token 使用方式对比

### Ant Design (React)

```tsx
<ConfigProvider
  theme={{
    token: {
      colorPrimary: '#00b96b',         // 修改 Seed Token
      borderRadius: 2,
    },
    components: {
      Button: {
        colorPrimary: '#ff0000',       // 组件级 Token 覆盖
        algorithm: true,               // 启用算法重新派生
      },
    },
  }}
>
  <App />
</ConfigProvider>
```

### AtomUI (C# / XAML)

**方式一：XML 主题定义文件**
```xml
<Theme Name="Custom" IsDefault="true">
    <SharedTokens>
        <Token Name="ColorPrimary" Value="#00b96b" />
        <Token Name="BorderRadius" Value="2" />
    </SharedTokens>
    <ControlTokens>
        <ControlToken Id="Button" EnableAlgorithm="true">
            <Token Name="ColorPrimary" Value="#ff0000" IsShared="true" />
        </ControlToken>
    </ControlTokens>
</Theme>
```

**方式二：XAML ThemeConfigProvider**
```xml
<atom:ThemeConfigProvider>
    <atom:ThemeConfigProvider.SharedTokenSetters>
        <atom:TokenSetter Key="ColorPrimary" Value="#00b96b" />
    </atom:ThemeConfigProvider.SharedTokenSetters>
    <atom:ThemeConfigProvider.ControlTokenInfoSetters>
        <atom:ControlTokenInfoSetter TokenId="Button" EnableAlgorithm="True">
            <atom:TokenSetter Key="ColorPrimary" Value="#ff0000" />
        </atom:ControlTokenInfoSetter>
    </atom:ThemeConfigProvider.ControlTokenInfoSetters>
    
    <StackPanel><!-- 子控件受此配置影响 --></StackPanel>
</atom:ThemeConfigProvider>
```

