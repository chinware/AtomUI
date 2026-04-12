# Button Design Token

Button 控件使用 `ButtonToken`（Token ID: `"Button"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ButtonTokenResource PrimaryColor}
{atom:ButtonTokenResource DefaultBg}
{atom:ButtonTokenResource Padding}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource BorderThickness}
```

---

## 组件级 Token 一览

以下是 `ButtonToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PrimaryColor` | `Color` | `SharedToken.ColorTextLightSolid` | 主按钮文本颜色（通常为白色） |
| `DefaultColor` | `Color` | `SharedToken.ColorText` | 默认按钮文本颜色 |
| `DefaultBg` | `Color` | `SharedToken.ColorBgContainer` | 默认按钮背景色 |
| `DefaultBorderColor` | `Color` | `SharedToken.ColorBorder` | 默认按钮边框颜色 |
| `DefaultBorderColorDisabled` | `Color` | `SharedToken.ColorBorder` | 禁用态默认边框颜色 |
| `DangerColor` | `Color` | `SharedToken.ColorTextLightSolid` | 危险按钮文本颜色 |
| `BorderColorDisabled` | `Color` | `SharedToken.ColorBorder` | 禁用态边框颜色 |
| `DefaultGhostColor` | `Color` | `SharedToken.ColorBgContainer` | 默认幽灵按钮文本颜色 |
| `GhostBg` | `Color` | `Colors.Transparent` | 幽灵按钮背景色（透明） |
| `DefaultGhostBorderColor` | `Color` | `SharedToken.ColorBgContainer` | 默认幽灵按钮边框颜色 |
| `GroupBorderColor` | `Color` | `SharedToken.ColorPrimaryHover` | 按钮组边框颜色 |
| `SolidTextColor` | `Color` | 基于 `SharedToken.ColorBgSolid` 亮度计算 | 实心按钮文本色（自动适配深浅背景） |
| `TextTextColor` | `Color` | `SharedToken.ColorText` | 文本按钮文本颜色 |
| `TextTextHoverColor` | `Color` | `SharedToken.ColorText` | 文本按钮悬浮文本颜色 |
| `TextTextActiveColor` | `Color` | `SharedToken.ColorText` | 文本按钮激活文本颜色 |

### 悬浮/激活态颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DefaultHoverBg` | `Color` | `SharedToken.ColorBgContainer` | 默认按钮悬浮背景色 |
| `DefaultHoverColor` | `Color` | `SharedToken.ColorPrimaryHover` | 默认按钮悬浮文本颜色 |
| `DefaultHoverBorderColor` | `Color` | `SharedToken.ColorPrimaryHover` | 默认按钮悬浮边框颜色 |
| `DefaultActiveBg` | `Color` | `SharedToken.ColorBgContainer` | 默认按钮激活背景色 |
| `DefaultActiveColor` | `Color` | `SharedToken.ColorPrimaryActive` | 默认按钮激活文本颜色 |
| `DefaultActiveBorderColor` | `Color` | `SharedToken.ColorPrimaryActive` | 默认按钮激活边框颜色 |
| `LinkHoverBg` | `Color` | `Colors.Transparent` | 链接按钮悬浮背景色 |
| `TextHoverBg` | `Color` | `SharedToken.ColorBgTextHover` | 文本按钮悬浮背景色 |

### 阴影 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DefaultShadow` | `BoxShadows` | 基于 `SharedToken.ColorControlOutline` | 默认按钮阴影 |
| `PrimaryShadow` | `BoxShadows` | 基于 `SharedToken.ColorControlOutline` | 主按钮阴影 |
| `DangerShadow` | `BoxShadows` | 基于 `SharedToken.ColorErrorOutline` | 危险按钮阴影 |

### 内间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `Padding` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontal`、`SharedToken.ControlHeight` | 中号按钮内间距 |
| `PaddingLG` | `Thickness` | 基于 `SharedToken.PaddingContentHorizontal`、`SharedToken.ControlHeightLG` | 大号按钮内间距 |
| `PaddingSM` | `Thickness` | 基于 `SharedToken.ControlHeightSM` | 小号按钮内间距 |
| `CirclePadding` | `Thickness` | `PaddingSM.Left / 2` | 圆形按钮内间距 |
| `IconOnyPadding` | `Thickness` | 基于 `SharedToken.ControlHeight` | 仅图标按钮中号内间距 |
| `IconOnyPaddingLG` | `Thickness` | 基于 `SharedToken.ControlHeightLG` | 仅图标按钮大号内间距 |
| `IconOnyPaddingSM` | `Thickness` | 基于 `SharedToken.ControlHeightSM` | 仅图标按钮小号内间距 |

### 图标 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IconSize` | `double` | `SharedToken.IconSize` | 中号按钮图标尺寸 |
| `IconSizeLG` | `double` | `SharedToken.IconSize` | 大号按钮图标尺寸 |
| `IconSizeSM` | `double` | `SharedToken.IconSizeSM` | 小号按钮图标尺寸 |
| `OnlyIconSize` | `double` | `SharedToken.IconSizeLG` | 仅图标中号按钮图标尺寸 |
| `OnlyIconSizeLG` | `double` | `SharedToken.IconSizeLG` | 仅图标大号按钮图标尺寸 |
| `OnlyIconSizeSM` | `double` | `SharedToken.IconSize` | 仅图标小号按钮图标尺寸 |
| `IconMargin` | `Thickness` | `SharedToken.UniformlyPaddingXXS` | 图标与文本之间的间距 |

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `FontWeight` | `double` | 固定 `400` | 按钮文字字重 |
| `ContentFontSize` | `double` | `SharedToken.FontSize` | 中号按钮字体大小 |
| `ContentFontSizeLG` | `double` | `SharedToken.FontSizeLG` | 大号按钮字体大小 |
| `ContentFontSizeSM` | `double` | `SharedToken.FontSize` | 小号按钮字体大小 |
| `ContentLineHeight` | `double` | 基于 `ContentFontSize` 计算 | 中号按钮行高 |
| `ContentLineHeightLG` | `double` | 基于 `ContentFontSizeLG` 计算 | 大号按钮行高 |
| `ContentLineHeightSM` | `double` | 基于 `ContentFontSizeSM` 计算 | 小号按钮行高 |

### 额外区域 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ExtraContentItemSpacing` | `double` | `SharedToken.UniformlyMarginXXS / 2` | 额外内容区域子项间距 |
| `ExtraContentMargin` | `Thickness` | `Padding.Left / 2` | 中号额外内容区域外边距 |
| `ExtraContentMarginLG` | `Thickness` | `PaddingLG.Left / 2` | 大号额外内容区域外边距 |
| `ExtraContentMarginSM` | `Thickness` | `PaddingSM.Left / 2` | 小号额外内容区域外边距 |

### 其他 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `GutterToFlyout` | `double` | `SharedToken.UniformlyMarginXXS` | 下拉菜单与按钮之间的间距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Button 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `BorderThickness` | 按钮默认边框厚度 |
| `EnableMotion` | 全局动画开关 |
| `EnableWaveSpirit` | 全局波纹开关 |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 按钮高度（中/小/大） |
| `BorderRadius` / `BorderRadiusSM` / `BorderRadiusLG` | 圆角半径（中/小/大） |
| `ColorPrimary` / `ColorPrimaryHover` / `ColorPrimaryActive` | 主色调及其交互态 |
| `ColorError` / `ColorErrorHover` / `ColorErrorActive` / `ColorErrorBorderHover` | 错误/危险色系 |
| `ColorErrorBgHover` / `ColorErrorBgActive` | 错误背景色（用于 Text 类型危险按钮） |
| `ColorErrorOutline` | 危险阴影颜色 |
| `ColorLink` / `ColorLinkHover` / `ColorLinkActive` | 链接色系 |
| `ColorText` / `ColorTextSecondary` / `ColorTextDisabled` / `ColorTextLightSolid` | 文本色系 |
| `ColorBgContainer` / `ColorBgContainerDisabled` | 容器背景色 |
| `ColorBgTextHover` / `ColorBgTextActive` | 文本背景交互色 |
| `ColorBorder` | 边框颜色 |
| `ColorControlOutline` | 控件外轮廓颜色 |
| `ColorFocusBorder` | 焦点边框颜色 |
| `FocusVisualBorderThickness` | 焦点指示器边框厚度 |
| `OpacityLoading` | 加载态不透明度 |
| `IconSize` / `IconSizeSM` / `IconSizeLG` | 全局图标尺寸 |

---

## Token 对外观的具体影响

### 按钮类型与 Token 映射

| 按钮类型 | 正常态 | 悬浮态 | 按下态 | 禁用态 |
|---|---|---|---|---|
| **Default** | `DefaultColor` / `DefaultBg` / `DefaultBorderColor` | `DefaultHoverColor` / `DefaultHoverBorderColor` | `DefaultActiveColor` / `DefaultActiveBorderColor` | `ColorTextDisabled` / `ColorBgContainerDisabled` |
| **Primary** | `PrimaryColor` / `ColorPrimary` | - / `ColorPrimaryHover` | - / `ColorPrimaryActive` | `ColorTextDisabled` / `ColorBgContainerDisabled` |
| **Dashed** | `DefaultColor` / `DefaultBg` / `DefaultBorderColor` | `DefaultHoverColor` / `DefaultHoverBorderColor` | `DefaultActiveColor` / `DefaultActiveBorderColor` | `ColorTextDisabled` / `ColorBgContainerDisabled` |
| **Link** | `ColorLink` / 透明 | `ColorLinkHover` | `ColorLinkActive` | `ColorTextDisabled` / 透明 |
| **Text** | `DefaultColor` / 透明 | - / `TextHoverBg` | - / `ColorBgTextActive` | `ColorTextDisabled` / 透明 |

### 危险态下的 Token 覆盖

当 `IsDanger=True` 时，上述色系会被替换为 Error 色系（`ColorError` / `ColorErrorHover` / `ColorErrorActive`），阴影替换为 `DangerShadow`。

### 幽灵态下的 Token 覆盖

当 `IsGhost=True` 时，背景变为透明，文字/边框使用对应类型的主色调（如 Primary 幽灵使用 `ColorPrimary`，Default 幽灵使用 `ColorTextLightSolid`）。

### 尺寸与 Token 映射

| 尺寸 | 高度 | 内间距 | 字号 | 圆角 | 图标大小 |
|---|---|---|---|---|---|
| `Large` | `ControlHeightLG` | `PaddingLG` | `ContentFontSizeLG` | `BorderRadiusLG` | `IconSizeLG` |
| `Middle` | `ControlHeight` | `Padding` | `ContentFontSize` | `BorderRadius` | `IconSize` |
| `Small` | `ControlHeightSM` | `PaddingSM` | `ContentFontSizeSM` | `BorderRadiusSM` | `IconSizeSM` |

