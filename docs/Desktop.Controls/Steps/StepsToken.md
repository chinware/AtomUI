# Steps Design Token

Steps 控件使用 `StepsToken`（Token ID: `"Steps"`）作为组件级 Design Token。Steps 拥有约 50+ 个 Token，覆盖四种步骤状态（Wait/Process/Finish/Error）× 多种展示风格（Default/Navigation/Inline/Dot）的完整颜色和尺寸体系。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:StepsTokenResource IconSize}
{atom:StepsTokenResource ProcessIconColor}
{atom:StepsTokenResource WaitTitleColor}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorError}
{atom:SharedTokenResource Spacing}
```

---

## 组件级 Token 一览

### 尺寸 / 布局 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IconSize` | `double` | `SharedToken.ControlHeight` | 标准指示器容器尺寸 |
| `IconFontSize` | `double` | `SharedToken.FontSize` | 标准指示器内数字字号 |
| `IconSizeSM` | `double` | `SharedToken.FontSizeHeading3` | 小尺寸指示器容器尺寸 |
| `CustomIconSize` | `double` | `SharedToken.ControlHeight` | 自定义图标指示器容器尺寸 |
| `CustomIconFontSize` | `double` | `SharedToken.ControlHeightSM` | 自定义图标字号 |
| `DotSize` | `double` | `ControlHeight / 4` | 点状指示器尺寸 |
| `DotCurrentSize` | `double` | `ControlHeightLG / 4` | 当前步骤点状指示器尺寸 |
| `DescriptionMaxWidth` | `double` | `140` | 描述区域最大宽度 |
| `StepsProgressSize` | `double` | `SharedToken.ControlHeightLG` | 进度环尺寸 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `HorizontalHeaderMargin` | `Thickness` | `(0, 0, UniformlyMargin, 0)` | 水平标签排列的外间距 |
| `HorizontalDotMargin` | `Thickness` | `(UniformlyMarginXS, 0)` | 水平点状指示器外间距 |
| `VerticalDotMargin` | `Thickness` | `(0, UniformlyMarginXS)` | 垂直点状指示器外间距 |
| `VerticalItemSpacing` | `double` | `SpacingXXS * 1.5` | 垂直排列时步骤项间距 |
| `VerticalDescriptionPadding` | `Thickness` | `(0, 0, 0, UniformlyPaddingXS)` | 垂直描述内间距 |
| `VerticalLabelContentMargin` | `Thickness` | `(0, UniformlyMarginSM, 0, 0)` | 垂直标签与图标间距 |
| `VerticalNavArrowMargin` | `Thickness` | `(0, UniformlyMarginSM)` | 导航箭头外间距 |
| `VerticalNavArrowMarginSM` | `Thickness` | `(0, UniformlyMarginXS)` | 导航箭头外间距（小尺寸） |
| `NavItemGutter` | `double` | `SharedToken.Spacing` | 导航类型内容与指示线间距 |
| `NavItemGutterSM` | `double` | `SharedToken.SpacingXS` | 导航类型间距（小尺寸） |

### 进度环 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ProgressFramePadding` | `Thickness` | `LineWidthBold * 2` | 进度环与指示器的间距 |
| `ProgressFramePaddingSM` | `Thickness` | `LineWidthBold` | 进度环间距（小尺寸） |
| `ProgressGrooveColor` | `Color` | `SharedToken.ColorSplit` | 进度环底色（未完成部分） |
| `ProgressColor` | `Color` | `SharedToken.ColorPrimary` | 进度环前景色（已完成部分） |
| `DotLineThickness` | `double` | `LineWidth * 3` | 点状连接线粗细 |

### 导航箭头 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `NavArrowColor` | `Color` | `SharedToken.ColorTextDisabled` | 导航箭头颜色 |
| `StepsNavActiveColor` | `Color` | `SharedToken.ColorPrimary` | 导航风格激活态颜色 |

---

## 四种状态的颜色 Token

Steps 为每种步骤状态定义了完整的颜色 Token 集合：

### Wait（等待）状态

| Token 名 | 来源 | 用途 |
|---|---|---|
| `WaitIconColor` | `ColorTextLabel` | 指示器内数字/图标颜色 |
| `WaitIconBgColor` | `ColorFillContent` | 指示器背景色 |
| `WaitIconBorderColor` | `Transparent` | 指示器边框色 |
| `WaitTitleColor` | `ColorTextDescription` | 步骤标题颜色 |
| `WaitDescriptionColor` | `ColorTextDescription` | 步骤描述颜色 |
| `WaitTailColor` | `ColorSplit` | 连接线颜色 |
| `WaitDotColor` | `ColorTextDisabled` | 点状指示器颜色 |

### Process（进行中）状态

| Token 名 | 来源 | 用途 |
|---|---|---|
| `ProcessIconColor` | `ColorTextLightSolid` | 指示器内数字颜色（白色） |
| `ProcessIconBgColor` | `ColorPrimary` | 指示器背景色（主色调） |
| `ProcessIconBorderColor` | `ColorPrimary` | 指示器边框色 |
| `ProcessTitleColor` | `ColorText` | 步骤标题颜色（深色） |
| `ProcessDescriptionColor` | `ColorText` | 步骤描述颜色 |
| `ProcessTailColor` | `ColorSplit` | 连接线颜色 |
| `ProcessDotColor` | `ColorPrimary` | 点状指示器颜色（主色调） |

### Finish（完成）状态

| Token 名 | 来源 | 用途 |
|---|---|---|
| `FinishIconColor` | `ColorPrimary` | 指示器内图标颜色（主色调） |
| `FinishIconBgColor` | `ControlItemBgActive` | 指示器背景色 |
| `FinishIconBorderColor` | `ControlItemBgActive` | 指示器边框色 |
| `FinishTitleColor` | `ColorText` | 步骤标题颜色 |
| `FinishDescriptionColor` | `ColorTextDescription` | 步骤描述颜色 |
| `FinishTailColor` | `ColorPrimary` | 连接线颜色（主色调） |
| `FinishDotColor` | `ColorPrimary` | 点状指示器颜色 |

### Error（错误）状态

| Token 名 | 来源 | 用途 |
|---|---|---|
| `ErrorIconColor` | `ColorTextLightSolid` | 指示器内图标颜色（白色） |
| `ErrorIconBgColor` | `ColorError` | 指示器背景色（错误色） |
| `ErrorIconBorderColor` | `ColorError` | 指示器边框色 |
| `ErrorTitleColor` | `ColorError` | 步骤标题颜色（错误色） |
| `ErrorDescriptionColor` | `ColorError` | 步骤描述颜色 |
| `ErrorTailColor` | `ColorSplit` | 连接线颜色 |
| `ErrorDotColor` | `ColorError` | 点状指示器颜色 |

---

## Inline 风格专用 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `InlineDotSize` | `double` | `6` | 内联指示器点的尺寸 |
| `InlineHeaderMargin` | `Thickness` | `(0, MarginXS - LineWidth, 0, 0)` | 内联标题外间距 |
| `InlineHeaderPadding` | `Thickness` | `(MarginXS, 0)` | 内联标题内间距 |
| `InlineItemPadding` | `Thickness` | `(0, MarginXS)` | 内联步骤项内间距 |
| `InlineTitleColor` | `Color` | `ColorTextQuaternary` | 内联标题颜色 |
| `InlineTailColor` | `Color` | `ColorBorderSecondary` | 内联连接线颜色 |

---

## Token 对外观的影响总结

### 状态颜色映射

```
Wait    →  灰色调（ColorFillContent / ColorTextDescription / ColorTextDisabled）
Process →  主色调（ColorPrimary / ColorTextLightSolid / ColorText）
Finish  →  完成态（ColorPrimary 图标 + ControlItemBgActive 背景）
Error   →  错误色（ColorError 全系列）
```

### 尺寸变体影响

| Token | 标准尺寸 | 小尺寸（SizeType=Small） |
|---|---|---|
| 指示器尺寸 | `IconSize`（ControlHeight） | `IconSizeSM`（FontSizeHeading3） |
| 进度环间距 | `ProgressFramePadding` | `ProgressFramePaddingSM` |
| 导航箭头间距 | `VerticalNavArrowMargin` | `VerticalNavArrowMarginSM` |
| 导航间距 | `NavItemGutter` | `NavItemGutterSM` |

### 过渡动画

StepsItem 和 StepsItemIndicator 均支持以下过渡动画：

- **StepsItem**：`Background`、`Foreground`、`SubTitleForeground`、`DescriptionForeground` 的 `SolidColorBrushTransition`，以及导航指示线的 `TransformOperationsTransition`
- **StepsItemIndicator**：`Background`、`BorderBrush`、`Foreground` 的 `SolidColorBrushTransition`，以及 `Width`/`Height` 的 `DoubleTransition`
