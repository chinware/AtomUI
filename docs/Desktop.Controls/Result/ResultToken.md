# Result Design Token

Result 使用 `ResultToken`（Token ID: `"Result"`）作为组件级 Design Token。所有视觉属性均从全局 `SharedToken` 派生，遵循 Ant Design 5.0 的 Design Token 三层派生体系。

---

## Token 概述

| 项目 | 值 |
|---|---|
| **Token 类** | `ResultToken` (`internal`) |
| **Token ID** | `"Result"` |
| **ScopeProvider** | `ResultToken.ScopeProvider` |
| **源码位置** | `src/AtomUI.Desktop.Controls/Result/ResultToken.cs` |

---

## Token 属性列表

### 排版相关

| Token | 类型 | 派生逻辑 | 说明 |
|---|---|---|---|
| `HeaderFontSize` | `double` | `SharedToken.FontSizeHeading3` | 标题字体大小 |
| `SubHeaderFontSize` | `double` | `SharedToken.FontSize` | 副标题字体大小 |

### 图标 & 插画相关

| Token | 类型 | 派生逻辑 | 说明 |
|---|---|---|---|
| `IconSize` | `double` | `SharedToken.FontSizeHeading3 × 3` | 状态图标尺寸（宽高） |
| `ImageWidth` | `double` | `250`（固定值） | HTTP 错误码 SVG 插画宽度 |
| `ImageHeight` | `double` | `295`（固定值） | HTTP 错误码 SVG 插画高度 |

### 状态颜色

| Token | 类型 | 派生逻辑 | 说明 |
|---|---|---|---|
| `ResultInfoIconColor` | `Color` | `SharedToken.ColorInfo` | Info 状态图标颜色（蓝色） |
| `ResultSuccessIconColor` | `Color` | `SharedToken.ColorSuccess` | Success 状态图标颜色（绿色） |
| `ResultWarningIconColor` | `Color` | `SharedToken.ColorWarning` | Warning 状态图标颜色（黄色） |
| `ResultErrorIconColor` | `Color` | `SharedToken.ColorError` | Error 状态图标颜色（红色） |

### 间距 & 内边距

| Token | 类型 | 派生逻辑 | 说明 |
|---|---|---|---|
| `FramePadding` | `Thickness` | `(UniformlyPaddingLG × 2, UniformlyMarginXL)` | 根框架内边距 |
| `ExtraMargin` | `Thickness` | `(0, UniformlyMargin, 0, 0)` | 额外操作区上外边距 |
| `ContentPadding` | `Thickness` | `(UniformlyPadding × 2.5, UniformlyPaddingLG)` | 子内容区域内边距 |
| `ContentMargin` | `Thickness` | `(0, UniformlyPaddingLG, 0, 0)` | 子内容区域上外边距 |
| `HeaderMargin` | `Thickness` | `(0, UniformlyMarginXS)` | 标题上下外边距 |
| `StatusImageMargin` | `Thickness` | `(0, 0, 0, UniformlyMargin)` | 状态图标/插画下外边距 |

---

## Token 资源访问方式

### 在 AXAML 中使用

```xml
<!-- 组件级 Token：使用 ResultTokenResource 标记扩展 -->
<Setter Property="Padding" Value="{atom:ResultTokenResource FramePadding}" />
<Setter Property="FontSize" Value="{atom:ResultTokenResource HeaderFontSize}" />
<Setter Property="Width" Value="{atom:ResultTokenResource IconSize}" />

<!-- 全局共享 Token -->
<Setter Property="Foreground" Value="{atom:SharedTokenResource ColorTextHeading}" />
<Setter Property="Background" Value="{atom:SharedTokenResource ColorFillAlter}" />
```

### 在 C# 实例样式中使用

```csharp
// 使用生成的 ResultTokenKind 枚举
var iconStyle = new Style(x => x.Nesting().Descendant().Name("PART_StatusIconPresenter").Child());
iconStyle.Add(WidthProperty, ResultTokenKind.IconSize);
iconStyle.Add(HeightProperty, ResultTokenKind.IconSize);
iconStyle.Add(ForegroundProperty, ResultTokenKind.ResultInfoIconColor);
```

---

## 消费的全局 SharedToken

以下全局 SharedToken 被 ResultToken 的 `CalculateTokenValues()` 或 ControlTheme AXAML 直接引用：

| SharedToken | 用途 |
|---|---|
| `FontSizeHeading3` | 计算 `HeaderFontSize`、`IconSize` |
| `FontSize` | 计算 `SubHeaderFontSize` |
| `ColorInfo` | Info 状态图标颜色 |
| `ColorSuccess` | Success 状态图标颜色 |
| `ColorWarning` | Warning 状态图标颜色 |
| `ColorError` | Error 状态图标颜色 |
| `ColorTextHeading` | 标题文本颜色（在 ControlTheme 中直接引用） |
| `ColorTextDescription` | 副标题文本颜色（在 ControlTheme 中直接引用） |
| `ColorFillAlter` | 子内容区域背景色（在 ControlTheme 中直接引用） |
| `RelativeLineHeightHeading3` | 标题行高系数（在 ControlTheme 中绑定到 `RelativeHeaderLineHeight`） |
| `RelativeLineHeight` | 副标题行高系数（在 ControlTheme 中绑定到 `RelativeSubHeaderLineHeight`） |
| `UniformlyMargin` | 计算 `ExtraMargin`、`StatusImageMargin` |
| `UniformlyMarginXS` | 计算 `HeaderMargin` |
| `UniformlyMarginXL` | 计算 `FramePadding` |
| `UniformlyPadding` | 计算 `ContentPadding` |
| `UniformlyPaddingLG` | 计算 `FramePadding`、`ContentPadding`、`ContentMargin` |

---

## Token 派生链示意

```
SharedToken (全局 DesignToken)
  ├── FontSizeHeading3 ──────► HeaderFontSize
  │                       └──► IconSize (× 3)
  ├── FontSize ──────────────► SubHeaderFontSize
  ├── ColorInfo ─────────────► ResultInfoIconColor
  ├── ColorSuccess ──────────► ResultSuccessIconColor
  ├── ColorWarning ──────────► ResultWarningIconColor
  ├── ColorError ────────────► ResultErrorIconColor
  ├── UniformlyPaddingLG ────► FramePadding, ContentPadding, ContentMargin
  ├── UniformlyMarginXL ─────► FramePadding
  ├── UniformlyPadding ──────► ContentPadding
  ├── UniformlyMargin ───────► ExtraMargin, StatusImageMargin
  └── UniformlyMarginXS ────► HeaderMargin
```

---

## 深色模式

`CalculateTokenValues(bool isDarkMode)` 方法接收 `isDarkMode` 参数。当前 ResultToken 的所有颜色值均引用 SharedToken 的语义色（如 `ColorInfo`、`ColorSuccess` 等），这些语义色在深色模式下会自动调整，因此 ResultToken 本身无需针对深色模式做额外处理。
