# Tag Design Token

Tag 控件使用 `TagToken`（Token ID: `"Tag"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:TagTokenResource DefaultBg}
{atom:TagTokenResource DefaultColor}
{atom:TagTokenResource TagFontSize}
{atom:TagTokenResource TagPadding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource BorderThickness}
{atom:SharedTokenResource BorderRadiusSM}
```

---

## 组件级 Token 一览

以下是 `TagToken` 定义的全部组件级 Token。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DefaultBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorFillQuaternary, SharedToken.ColorBgContainer)` | 默认标签背景色（浅灰色） |
| `DefaultColor` | `Color` | `SharedToken.ColorText` | 默认标签文字颜色 |
| `TagBorderlessBg` | `Color` | 同 `DefaultBg` | 无边框标签背景色 |

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TagFontSize` | `double` | `SharedToken.FontSizeSM` | 标签字号（小号字体） |
| `TagLineHeight` | `double` | `SharedToken.FontHeightSM` | 标签行高 |

### 图标 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TagIconSize` | `double` | `SharedToken.FontSizeIcon` | 标签图标尺寸 |
| `TagCloseIconSize` | `double` | `SharedToken.IconSizeXS` | 关闭按钮图标尺寸 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TagPadding` | `Thickness` | `Thickness(SharedToken.SizeXS - 1, 0)` | 标签外层内间距（水平方向） |
| `TagTextPaddingInline` | `Thickness` | `Thickness(SharedToken.UniformlyPaddingXXS, 0)` | 文字区域内间距（图标与文字间距） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Tag 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorBorder` | 默认边框颜色 |
| `BorderThickness` | 边框粗细 |
| `BorderRadiusSM` | 圆角半径 |
| `ColorTextLightSolid` | 自定义颜色模式下的文字色（白色） |
| `IconSizeXS` | 关闭按钮图标尺寸 |

---

## Token 对外观的具体影响

### 颜色模式与 Token 映射

| 颜色模式 | 背景色 | 文字色 | 边框色 |
|---|---|---|---|
| **默认（无 TagColor）** | `DefaultBg` | `DefaultColor` | `ColorBorder` |
| **预设颜色** | 调色板 1 号色 | 调色板 7 号色 | 调色板 3 号色 |
| **状态颜色** | 状态背景色（如 `ColorSuccessBg`） | 状态主色（如 `ColorSuccess`） | 状态边框色（如 `ColorSuccessBorder`） |
| **自定义颜色** | 用户指定色 | `ColorTextLightSolid`（白色） | 无边框 |

### 状态颜色与 SharedToken 映射

| 状态 | 文字色 Token | 背景色 Token | 边框色 Token |
|---|---|---|---|
| `Success` | `ColorSuccess` | `ColorSuccessBg` | `ColorSuccessBorder` |
| `Info` | `ColorInfo` | `ColorInfoBg` | `ColorInfoBorder` |
| `Warning` | `ColorWarning` | `ColorWarningBg` | `ColorWarningBorder` |
| `Error` | `ColorError` | `ColorErrorBg` | `ColorErrorBorder` |
