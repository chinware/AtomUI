# Form Design Token

Form 控件使用 `FormToken`（Token ID: `"Form"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:FormTokenResource LabelRequiredMarkColor}
{atom:FormTokenResource LabelColor}
{atom:FormTokenResource FormItemSpacing}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorError}
{atom:SharedTokenResource ColorTextHeading}
{atom:SharedTokenResource FontSize}
```

---

## 组件级 Token 一览

以下是 `FormToken` 定义的全部组件级 Token。

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `LabelRequiredMarkColor` | `Color` | `SharedToken.ColorError` | 必填项标记（*）的颜色，默认为错误色（红色） |
| `LabelColor` | `Color` | `SharedToken.ColorTextHeading` | 标签文本颜色 |
| `LabelFontSize` | `double` | `SharedToken.FontSize` | 标签字体大小 |
| `LabelColonMargin` | `Thickness` | `SharedToken.UniformlyMarginXXS / 2, 0, SharedToken.UniformlyMarginXS, 0` | 标签冒号的内联间距（左右空白） |
| `FormItemSpacing` | `double` | `SharedToken.SpacingLG` | 水平/垂直布局下 FormItem 之间的垂直间距 |
| `InlineItemSpacing` | `double` | `SharedToken.Spacing` | 内联布局下 FormItem 之间的水平间距 |
| `VerticalLabelPadding` | `Thickness` | `0, 0, 0, SharedToken.UniformlyPaddingXS` | 垂直布局时标签的内边距（底部留白） |
| `VerticalLabelMargin` | `Thickness` | `default` (0,0,0,0) | 垂直布局时标签的外边距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Form 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorError` | 必填标记颜色、错误态边框色 |
| `ColorWarning` | 警告态边框色 |
| `ColorSuccess` | 成功态颜色（验证反馈图标） |
| `ColorTextHeading` | 标签文本颜色 |
| `ColorTextSecondary` | 帮助文本颜色 |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `FontSize` | 标签字体大小 |
| `FontSizeSM` | 错误/警告消息字体大小 |
| `ControlHeight` / `ControlHeightSM` / `ControlHeightLG` | 控件高度（受 SizeType 影响） |
| `SpacingLG` | FormItem 间距 |
| `Spacing` | Inline 布局间距 |
| `UniformlyMarginXS` / `UniformlyMarginXXS` | 冒号间距、标签间距 |
| `UniformlyPaddingXS` | 垂直布局标签内边距 |

---

## Token 对外观的具体影响

### 布局与间距

| 布局模式 | 间距 Token | 说明 |
|---|---|---|
| Horizontal / Vertical | `FormItemSpacing` | FormItem 之间的垂直间距 |
| Inline | `InlineItemSpacing` | FormItem 之间的水平间距 |
| Vertical 标签 | `VerticalLabelPadding` | 标签下方的间距（标签与内容之间） |

### 标签样式

| 视觉元素 | Token | 说明 |
|---|---|---|
| 标签文字 | `LabelColor` + `LabelFontSize` | 标签的颜色和字号 |
| 必填星号（*） | `LabelRequiredMarkColor` | 红色星号颜色 |
| 冒号间距 | `LabelColonMargin` | 冒号左右的空白 |

### 验证状态颜色映射

| 验证状态 | 对应 SharedToken | 视觉效果 |
|---|---|---|
| `Success` | `ColorSuccess` | 绿色边框/图标 |
| `Warning` | `ColorWarning` | 黄色边框/图标，警告消息文本 |
| `Error` | `ColorError` | 红色边框/图标，错误消息文本 |
| `Validating` | `ColorPrimary` | 主色调旋转图标 |
| `Default` | 默认边框色 | 无特殊状态 |
