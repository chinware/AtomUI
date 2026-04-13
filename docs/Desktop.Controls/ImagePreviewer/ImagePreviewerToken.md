# ImagePreviewer Design Token

ImagePreviewer 使用 `ImagePreviewerToken`（Token ID: `"ImagePreviewer"`）作为组件级 Design Token。所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ImagePreviewerTokenResource PreviewOperationSize}
{atom:ImagePreviewerTokenResource PreviewOperationColor}
{atom:ImagePreviewerTokenResource MaskBgColor}
{atom:ImagePreviewerTokenResource NavButtonBgColor}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource EnableMotion}
{atom:SharedTokenResource ColorBgLayout}
{atom:SharedTokenResource IconSizeLG}
```

---

## 组件级 Token 一览

以下是 `ImagePreviewerToken` 定义的全部组件级 Token，按功能分组说明。

### 操作图标 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PreviewOperationSize` | `double` | `SharedToken.FontSizeIcon * 1.5` | 预览操作按钮的图标大小 |
| `PreviewOperationColor` | `Color` | `SharedToken.ColorTextLightSolid` (alpha 0.85) | 预览操作按钮正常态图标颜色（半透明白色） |
| `PreviewOperationHoverColor` | `Color` | `SharedToken.ColorTextLightSolid` | 预览操作按钮悬浮态图标颜色（不透明白色） |
| `PreviewOperationColorDisabled` | `Color` | `SharedToken.ColorTextDisabled` | 预览操作按钮禁用态图标颜色 |

### 导航按钮 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ImagePreviewSwitchSize` | `double` | `SharedToken.ControlHeightLG` | 左/右图片切换按钮尺寸 |
| `NavButtonBgColor` | `Color` | `SharedToken.ColorBgMask` (alpha 0.1) | 导航按钮正常态背景颜色（低透明度） |
| `NavButtonBgHoverColor` | `Color` | `SharedToken.ColorBgMask` (alpha 0.2) | 导航按钮悬浮态背景颜色（稍高透明度） |

### 封面 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `MaskBgColor` | `Color` | `rgba(0, 0, 0, 0.3)` | 封面遮罩半透明背景色 |
| `CoverImageWidth` | `double` | 固定 `200` | `ImageGroupPreviewer` 封面缩略图默认宽度 |

### 对话框 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DialogMinWidth` | `double` | 固定 `710` | 预览窗口最小宽度 |
| `DialogMinHeight` | `double` | 固定 `240` | 预览窗口最小高度 |
| `TitleBarBackgroundColor` | `Color` | 亮色模式：`SharedToken.ColorBorderSecondary.Darken(1)`；暗色模式：`SharedToken.ColorBorderSecondary` | 预览窗口标题栏背景色（暗色主题自动适配） |

### 浮动工具栏 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `FloatToolbarPadding` | `Thickness` | `SharedToken.UniformlyPaddingLG / 2, 0` | 浮动工具栏水平内间距 |
| `FloatToolbarIndicatorPadding` | `Thickness` | `SharedToken.UniformlyPaddingXS, 0` | 浮动工具栏索引指示器内间距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，ImagePreviewer 的 ControlTheme 还直接引用了以下全局 SharedToken：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关 |
| `SpacingXXS` | 封面指示器内图标与文字间距 |
| `SpacingXS` | 浮动工具栏指示器与操作区域间距 |
| `PaddingXS` | 导航按钮内间距 |
| `PaddingSM` | 浮动工具栏操作按钮内间距 |
| `Margin` | ImageViewer 内导航按钮外边距 |
| `MarginXS` | 对话框内导航按钮外边距 |
| `MarginLG` | 浮动工具栏底部外边距 |
| `IconSizeLG` | 标题栏工具栏图标大小 |
| `ColorTextLightSolid` | 封面遮罩指示器文字/图标颜色 |
| `ColorBgLayout` | 预览对话框背景色 |
| `ColorBgTextHover` | 标题栏工具栏按钮悬浮背景色 |
| `ColorBgTextActive` | 标题栏工具栏按钮按下背景色 |

---

## Token 对外观的影响

### 操作按钮状态映射

| 状态 | 图标颜色 | 背景色 |
|---|---|---|
| 正常 | `PreviewOperationColor`（85% 白色） | 透明 |
| 悬浮 | `PreviewOperationHoverColor`（100% 白色） | — |
| 禁用 | `PreviewOperationColorDisabled` | — |

### 导航按钮状态映射

| 状态 | 图标颜色 | 背景色 |
|---|---|---|
| 正常 | `PreviewOperationColor` | `NavButtonBgColor`（10% 遮罩色） |
| 悬浮 | `PreviewOperationHoverColor` | `NavButtonBgHoverColor`（20% 遮罩色） |
| 禁用 | `PreviewOperationColorDisabled` | 透明 |

### 暗色主题适配

`TitleBarBackgroundColor` 根据 `isDarkMode` 参数自动调整：
- 亮色模式：`ColorBorderSecondary.Darken(1)`（较深）
- 暗色模式：`ColorBorderSecondary`（原始值）

其余颜色 Token 由 SharedToken 的暗色主题算法自动重新计算。
