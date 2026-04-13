# OptionButtonGroup Design Token

OptionButtonGroup 和 OptionButton 控件使用 `OptionButtonToken`（Token ID: `"OptionButton"`）作为组件级 Design Token。所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:OptionButtonTokenResource ButtonBackground}
{atom:OptionButtonTokenResource ButtonSolidCheckedBackground}
{atom:OptionButtonTokenResource ContentFontSize}
{atom:OptionButtonTokenResource Padding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource BorderRadius}
```

---

## 组件级 Token 一览

以下是 `OptionButtonToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ButtonBackground` | `Color` | `SharedToken.ColorBgContainer` | 按钮默认背景色 |
| `ButtonCheckedBackground` | `Color` | `SharedToken.ColorBgContainer` | 选中按钮背景色（Outline 模式） |
| `ButtonColor` | `Color` | `SharedToken.ColorText` | 按钮默认文本颜色 |
| `ButtonCheckedBgDisabled` | `Color` | `SharedToken.ControlItemBgActiveDisabled` | 选中并禁用时的背景色 |
| `ButtonCheckedColorDisabled` | `Color` | `SharedToken.ColorTextDisabled` | 选中并禁用时的文本颜色 |
| `ButtonSolidCheckedColor` | `Color` | `SharedToken.ColorTextLightSolid` | Solid 模式选中时文本颜色（通常为白色） |
| `ButtonSolidCheckedBackground` | `Color` | `SharedToken.ColorPrimary` | Solid 模式选中时背景色 |
| `ButtonSolidCheckedHoverBackground` | `Color` | `SharedToken.ColorPrimaryHover` | Solid 模式选中悬浮时背景色 |
| `ButtonSolidCheckedActiveBackground` | `Color` | `SharedToken.ColorPrimaryActive` | Solid 模式选中按下时背景色 |
| `ButtonPadding` | `Thickness` | `SharedToken.UniformlyPadding` | 按钮内间距（水平方向） |

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ContentFontSize` | `double` | `SharedToken.FontSize` | 中号按钮字体大小 |
| `ContentFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号按钮字体大小 |
| `ContentFontSizeSM` | `double` | `SharedToken.FontSize` | 小号按钮字体大小 |
| `ContentLineHeight` | `double` | 基于 `ContentFontSize` 计算 | 中号按钮行高 |
| `ContentLineHeightLG` | `double` | 基于 `ContentFontSizeLG` 计算 | 大号按钮行高 |
| `ContentLineHeightSM` | `double` | 基于 `ContentFontSizeSM` 计算 | 小号按钮行高 |

### 内间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `Padding` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontal`、`SharedToken.ControlHeight` | 中号按钮内间距 |
| `PaddingLG` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontal`、`SharedToken.ControlHeightSM` | 大号按钮内间距 |
| `PaddingSM` | `Thickness` | 基于固定值 `8`、`SharedToken.ControlHeightLG` | 小号按钮内间距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，OptionButtonGroup 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### OptionButtonGroup 主题引用

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关 |
| `EnableWaveSpirit` | 全局波纹开关 |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 三种尺寸的组高度 |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | 三种尺寸的圆角半径 |
| `ColorBorder` | 组外围边框颜色 |
| `ColorPrimary` | 选中项边框颜色（`SelectedOptionBorderColor`） |
| `BorderThickness` | 组外围边框厚度 |

### OptionButton 主题引用

| Token 资源键 | 使用场景 |
|---|---|
| `SpacingXXS` | 图标与文本之间的间距（`DockPanel.HorizontalSpacing`） |
| `ColorText` | 未选中按钮文本颜色（Solid 模式） |
| `ColorPrimary` | 选中按钮前景色和边框色（Outline 模式） |
| `ColorPrimaryHover` | 悬浮时前景色/边框色 |
| `ColorPrimaryActive` | 按下时前景色/边框色 |
| `ColorBorder` | 禁用态边框颜色 |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `ColorBgContainerDisabled` | 禁用态背景色 |
| `IconSize` / `IconSizeSM` / `IconSizeLG` | 三种尺寸的图标大小 |

---

## Token 对外观的具体影响

### 按钮样式与 Token 映射

#### Solid 模式

| 状态 | 前景色 | 背景色 |
|---|---|---|
| 选中 | `ButtonSolidCheckedColor`（白色） | `ButtonSolidCheckedBackground`（主色） |
| 选中 + 悬浮 | `ButtonSolidCheckedColor` | `ButtonSolidCheckedHoverBackground` |
| 选中 + 按下 | `ButtonSolidCheckedColor` | `ButtonSolidCheckedActiveBackground` |
| 未选中 | `ColorText` | 透明 |
| 未选中 + 悬浮 | `ColorPrimaryHover` | 透明 |
| 未选中 + 按下 | `ColorPrimaryActive` | 透明 |
| 禁用 | `ColorTextDisabled` | `ColorBgContainerDisabled` |
| 禁用 + 选中 | `ButtonCheckedColorDisabled` | `ButtonCheckedBgDisabled` |

#### Outline 模式

| 状态 | 前景色 | 边框色 | 背景色 |
|---|---|---|---|
| 选中 | `ColorPrimary` | `ColorPrimary` | 透明 |
| 未选中 | `ButtonColor` | `ColorBorder` | 透明 |
| 悬浮 | `ColorPrimaryHover` | `ColorPrimaryHover` | 透明 |
| 按下 | `ColorPrimaryActive` | `ColorPrimaryActive` | 透明 |
| 禁用 | `ColorTextDisabled` | `ColorBorder` | `ColorBgContainerDisabled` |
| 禁用 + 选中 | `ButtonCheckedColorDisabled` | `ColorBorder` | `ButtonCheckedBgDisabled` |

### 尺寸与 Token 映射

| 尺寸 | 组高度 | 圆角 | 字号 | 内间距 | 图标大小 |
|---|---|---|---|---|---|
| `Large` | `ControlHeightLG` | `BorderRadiusLG` | `ContentFontSizeLG` | `PaddingLG` | `IconSizeLG` |
| `Middle` | `ControlHeight` | `BorderRadius` | `ContentFontSize` | `Padding` | `IconSizeLG` |
| `Small` | `ControlHeightSM` | `BorderRadiusSM` | `ContentFontSizeSM` | `PaddingSM` | `IconSizeSM` |
