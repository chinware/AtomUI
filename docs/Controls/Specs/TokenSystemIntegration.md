# Design Token 集成规范

> 本文档规定 AtomUI 控件的 Design Token 定义方式、值派生规则、资源引用方式和完整的 Token 生命周期。

---

## 1. Token 系统概述

AtomUI 的 Design Token 系统严格实现了 [Ant Design 5.0 Token 架构](https://ant.design/docs/react/customize-theme)。Token 分为三个全局层级和一个组件层级：

```
Seed Token ──(算法)──► Map Token ──(别名计算)──► Alias Token
  (种子)               (梯度)                    (语义)
                                                    ↓
                                          Component Token (组件 Token)
                                          从 SharedToken 派生
```

| 层级 | 定义位置 | 说明 |
|---|---|---|
| **Seed Token** | `DesignToken.Seed.cs` | 设计意图的起点（如 `ColorPrimary`、`FontSize`） |
| **Map Token** | `DesignToken.ColorPrimaryMap.cs` 等 | 由算法从 Seed Token 派生的梯度变量（如色板 1-10 号色） |
| **Alias Token** | `DesignToken.Alias.cs` | 语义化别名（如 `ColorTextDisabled`、`PaddingContentHorizontal`） |
| **Component Token** | `AtomUI.Desktop.Controls/ControlName/ControlNameToken.cs` | 组件专属 Token，从 `SharedToken`（即全局 `DesignToken`）派生 |

---

## 2. 组件 Token 类定义

### 2.1 完整结构

```csharp
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]                                         // ← 必须：标记为组件 Token
internal class ButtonToken : AbstractControlDesignToken      // ← 必须：internal + 继承
{
    public const string ID = "Button";                       // ← 必须：组件标识符
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);  // ← 必须

    /// <summary>
    /// 主要按钮文本颜色
    /// </summary>
    public Color PrimaryColor { get; set; }                  // ← Token 属性

    /// <summary>
    /// 按钮内间距
    /// </summary>
    public Thickness Padding { get; set; }

    public ButtonToken()
        : base(ID)                                           // ← 必须：传入 ID
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);               // ← 必须：调用 base
        PrimaryColor = SharedToken.ColorTextLightSolid;      // ← 从 SharedToken 派生
        Padding = new Thickness(
            SharedToken.PaddingContentHorizontal - SharedToken.LineWidth,
            /* ... */);
    }

    protected override Type GetTokenKindType() => typeof(ButtonTokenKind);  // ← 必须：返回生成的枚举类型
}
```

### 2.2 强制规则

| 规则 | 说明 |
|---|---|
| 类修饰符为 `internal` | Token 是实现细节，不暴露给外部 |
| 标记 `[ControlDesignToken]` | Source Generator 需要此标记生成 TokenKind 枚举 |
| 继承 `AbstractControlDesignToken` | 提供 SharedToken 访问和资源字典构建基础设施 |
| 定义 `const string ID` | 唯一标识组件，用于 Token 作用域隔离 |
| 定义 `static readonly ControlTokenResourceScopeProvider ScopeProvider` | 控件构造函数中注册 Token 作用域时使用 |
| 构造函数调用 `base(ID)` | 注册组件标识 |
| 重写 `CalculateTokenValues(bool isDarkMode)` | 计算所有 Token 值 |
| 重写 `GetTokenKindType()` | 返回 Source Generator 生成的枚举类型 |

---

## 3. Token 值派生规则

### 3.1 核心原则：所有值从 SharedToken 派生

Token 类中的所有颜色、尺寸、间距、字号值 **必须** 从 `SharedToken`（全局 `DesignToken` 实例）的属性派生。**禁止** 硬编码魔法数字。

```csharp
public override void CalculateTokenValues(bool isDarkMode)
{
    base.CalculateTokenValues(isDarkMode);

    // ✅ 正确：从 SharedToken 派生
    PrimaryColor         = SharedToken.ColorTextLightSolid;
    DefaultColor         = SharedToken.ColorText;
    DefaultBg            = SharedToken.ColorBgContainer;
    DefaultBorderColor   = SharedToken.ColorBorder;
    GroupBorderColor     = SharedToken.ColorPrimaryHover;
    TextHoverBg          = SharedToken.ColorBgTextHover;
    IconMargin           = new Thickness(0, 0, SharedToken.UniformlyPaddingXXS, 0);

    // ✅ 正确：基于 SharedToken 的计算
    var lineWidth       = SharedToken.LineWidth;
    var controlHeight   = SharedToken.ControlHeight;
    Padding = new Thickness(
        SharedToken.PaddingContentHorizontal - lineWidth,
        Math.Max((controlHeight - ContentLineHeight) / 2 - lineWidth, 0));

    // ✅ 正确：使用工具函数和 SharedToken
    DefaultBg = ColorUtils.OnBackground(SharedToken.ColorFillQuaternary, SharedToken.ColorBgContainer);
}
```

```csharp
// ❌ 错误：硬编码颜色值
PrimaryColor = Color.FromRgb(255, 255, 255);

// ❌ 错误：硬编码尺寸
Padding = new Thickness(16, 8);

// ❌ 错误：硬编码字号
ContentFontSize = 14;
```

### 3.2 可接受的常量使用

仅以下情况允许使用字面量：

| 场景 | 示例 | 原因 |
|---|---|---|
| `Colors.Transparent` | `GhostBg = Colors.Transparent` | 语义明确的特殊颜色 |
| `Colors.Black` / `Colors.White` | 条件判断后赋值 | 语义明确 |
| 字重值 | `FontWeight = 400` | CSS 标准字重值 |
| BoxShadow 参数 | `OffsetX = 0, Blur = 3` | 视觉设计的固定参数 |
| 数学计算中的因子 | `PaddingSM.Left / 2` | 基于 Token 的比例计算 |

### 3.3 暗色模式处理

`CalculateTokenValues` 的 `isDarkMode` 参数指示当前主题是否为暗色模式。大多数情况下不需要显式分支处理，因为 `SharedToken` 已经被主题算法切换为暗色版本。仅在需要 **额外** 暗色适配时使用：

```csharp
public override void CalculateTokenValues(bool isDarkMode)
{
    base.CalculateTokenValues(isDarkMode);
    
    // 大多数属性直接从 SharedToken 读取即可
    // SharedToken 已经由 ThemeAlgorithm 切换为暗色版本
    DefaultBg = SharedToken.ColorBgContainer;  // 暗色模式下自动是暗色

    // 仅需要额外处理时使用 isDarkMode
    var isBright = ColorUtils.IsBright(SharedToken.ColorBgSolid, Colors.White);
    SolidTextColor = isBright ? Colors.Black : Colors.White;
}
```

### 3.4 支持自定义覆盖的 Token

当 Token 属性允许被用户自定义覆盖时，使用"NaN 哨兵"模式：

```csharp
/// <summary>
/// 按钮内容字体大小
/// </summary>
public double ContentFontSize { get; set; } = double.NaN;  // ← NaN 表示未自定义

public override void CalculateTokenValues(bool isDarkMode)
{
    base.CalculateTokenValues(isDarkMode);
    
    // 如果未被自定义，从 SharedToken 派生
    ContentFontSize = !double.IsNaN(ContentFontSize)
        ? ContentFontSize
        : SharedToken.FontSize;
}
```

---

## 4. Token 属性类型

### 4.1 支持的类型

| 类型 | 用途 | 示例 |
|---|---|---|
| `Color` | 颜色值（自动转换为 `ImmutableSolidColorBrush`） | `PrimaryColor`、`DefaultBg` |
| `double` | 数值（字号、尺寸、间距） | `FontWeight`、`IconSize`、`ContentFontSize` |
| `Thickness` | 内外边距 | `Padding`、`IconMargin` |
| `CornerRadius` | 圆角 | `BorderRadius` |
| `BoxShadows` | 阴影 | `DefaultShadow`、`PrimaryShadow` |

> ⚠️ **Color 类型的特殊处理**：`AbstractControlDesignToken.BuildResourceDictionary` 会自动将 `Color` 类型的 Token 包装为 `ImmutableSolidColorBrush`，因此在 AXAML 中可以直接绑定到 `Brush` 类型的属性。

### 4.2 Token 属性命名约定

| 模式 | 含义 | 示例 |
|---|---|---|
| `{State}{Aspect}` | 某状态下的某个视觉属性 | `DefaultHoverBg`、`PrimaryColor` |
| `{Aspect}{Size}` | 某尺寸变体的属性 | `PaddingSM`、`PaddingLG`、`IconSizeSM` |
| `{Component}{Aspect}` | 子组件的属性 | `TagFontSize`、`TagIconSize` |

---

## 5. Token 作用域注册

控件在构造函数中 **必须** 注册 Token 作用域：

```csharp
public class Tag : AbstractTag
{
    public Tag()
    {
        this.RegisterTokenResourceScope(TagToken.ScopeProvider);
    }
}
```

**注册位置**：只在 **Platform 层**（如 `AtomUI.Desktop.Controls`）的具体控件中注册。**Base 层** 的抽象控件 **不注册** Token 作用域。

---

## 6. Token 资源在 AXAML 中的引用

### 6.1 组件 Token 引用

使用 `{atom:TokenResource}` 或 `{atom:XxxTokenResource}` 标记扩展：

```xml
<!-- 通用 TokenResource 标记 -->
<Setter Property="Padding" Value="{atom:TokenResource ButtonPadding}" />

<!-- 组件特定的 TokenResource 标记（Source Generator 生成） -->
<Setter Property="Background" Value="{atom:ButtonTokenResource DefaultBg}" />
<Setter Property="Foreground" Value="{atom:ButtonTokenResource DefaultColor}" />
<Setter Property="FontSize" Value="{atom:ButtonTokenResource ContentFontSize}" />

<!-- Tag 组件 Token -->
<Setter Property="Background" Value="{atom:TagTokenResource DefaultBg}" />
<Setter Property="Foreground" Value="{atom:TagTokenResource DefaultColor}" />
```

### 6.2 全局共享 Token 引用

使用 `{atom:SharedTokenResource}` 标记扩展：

```xml
<Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorBorder}" />
<Setter Property="CornerRadius" Value="{atom:SharedTokenResource BorderRadiusSM}" />
<Setter Property="BorderThickness" Value="{atom:SharedTokenResource BorderThickness}" />
```

### 6.3 选择规则

| 场景 | 使用 |
|---|---|
| 属性值是组件专属的（如按钮背景、标签字号） | `{atom:XxxTokenResource PropertyName}` |
| 属性值是全局共享的（如通用边框颜色、圆角） | `{atom:SharedTokenResource PropertyName}` |
| **禁止** 在 AXAML 中硬编码颜色/尺寸 | 必须使用 Token 资源引用 |

---

## 7. 完整 Token 示例

以下是 `TagToken` 的完整示例，展示一个简单组件的 Token 定义：

```csharp
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class TagToken : AbstractControlDesignToken
{
    public const string ID = "Tag";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
    /// <summary>
    /// 默认背景色
    /// </summary>
    public Color DefaultBg { get; set; }

    /// <summary>
    /// 默认文字颜色
    /// </summary>
    public Color DefaultColor { get; set; }

    public double TagFontSize { get; set; }
    public double TagLineHeight { get; set; }
    public double TagIconSize { get; set; }
    public double TagCloseIconSize { get; set; }
    public Thickness TagPadding { get; set; }
    public Thickness TagTextPaddingInline { get; set; }
    public Color TagBorderlessBg { get; set; }

    public TagToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        
        // 全部从 SharedToken 派生，无硬编码
        TagFontSize      = SharedToken.FontSizeSM;
        TagLineHeight    = SharedToken.FontHeightSM;
        TagCloseIconSize = SharedToken.IconSizeXS;
        TagIconSize      = SharedToken.FontSizeIcon;
        TagPadding       = new Thickness(SharedToken.SizeXS - 1, 0);
        DefaultBg        = ColorUtils.OnBackground(
            SharedToken.ColorFillQuaternary, SharedToken.ColorBgContainer);
        TagBorderlessBg      = DefaultBg;
        DefaultColor         = SharedToken.ColorText;
        TagTextPaddingInline = new Thickness(SharedToken.UniformlyPaddingXXS, 0);
    }

    protected override Type GetTokenKindType() => typeof(TagTokenKind);
}
```

---

## 8. Source Generator 生成的代码

`[ControlDesignToken]` 标记会触发 `AtomUI.Generator` 中的 `TokenResourceKeyGenerator`，自动生成：

1. **`XxxTokenKind` 枚举**：枚举所有 Token 属性名，用于资源字典键
2. **`XxxTokenResource` 标记扩展**：AXAML 中引用组件 Token 的快捷方式

这些文件生成在 `GeneratedFiles/` 目录中，**禁止** 手动编辑。

---

## 9. 禁止事项

| 禁止事项 | 原因 |
|---|---|
| ❌ Token 类标记为 `public` | Token 是内部实现细节 |
| ❌ Token 属性硬编码颜色/尺寸 | 必须从 SharedToken 派生 |
| ❌ 遗漏 `[ControlDesignToken]` 特性 | Source Generator 无法生成代码 |
| ❌ 遗漏 `const string ID` | Token 作用域隔离依赖此标识 |
| ❌ 在 AXAML 中使用字面量颜色/尺寸 | 必须使用 `{atom:XxxTokenResource}` 或 `{atom:SharedTokenResource}` |
| ❌ 在 Base 层控件中注册 Token 作用域 | Token 是平台特定的，仅在 Platform 层注册 |
| ❌ 手动编辑 `GeneratedFiles/` 中的文件 | 由 Source Generator 自动生成 |

