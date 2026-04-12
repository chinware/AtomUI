# Badge Design Token

Badge 系列控件（CountBadge、DotBadge、RibbonBadge）共享 `BadgeToken`（Token ID: `"Badge"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:BadgeTokenResource IndicatorHeight}
{atom:BadgeTokenResource BadgeColor}
{atom:BadgeTokenResource DotSize}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorError}
{atom:SharedTokenResource ColorSuccess}
{atom:SharedTokenResource ColorPrimary}
```

---

## 组件级 Token 一览

以下是 `BadgeToken` 定义的全部组件级 Token，按功能分组说明。

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `IndicatorHeight` | `double` | `SharedToken.FontSize × SharedToken.RelativeLineHeight - 2 × SharedToken.LineWidth` | 默认数字徽标高度 |
| `IndicatorHeightSM` | `double` | `SharedToken.FontSize` | 小号数字徽标高度 |
| `DotSize` | `double` | `SharedToken.FontSizeSM / 2` | 圆点徽标尺寸 |
| `StatusSize` | `double` | `SharedToken.FontSizeSM / 2` | 状态圆点尺寸 |

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TextFontSize` | `double` | `SharedToken.FontSizeSM` | 数字徽标文字字号（默认尺寸） |
| `TextFontSizeSM` | `double` | `SharedToken.FontSizeSM - 2` | 数字徽标文字字号（小号尺寸） |
| `TextFontWeight` | `FontWeight` | `FontWeight.Normal` | 数字徽标文字粗细 |
| `BadgeFontHeight` | `double` | `SharedToken.FontHeight` | 字体行高（缎带徽标使用） |

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `BadgeTextColor` | `Color` | `SharedToken.ColorTextLightSolid` | 数字徽标文字颜色（白色） |
| `BadgeColor` | `Color` | `SharedToken.ColorError` | 默认徽标背景色（红色） |
| `BadgeColorHover` | `Color` | `SharedToken.ColorErrorHover` | 悬浮态徽标背景色 |

### 阴影 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `BadgeShadowSize` | `double` | `SharedToken.LineWidth` | 徽标外围阴影尺寸 |
| `BadgeShadowColor` | `Color` | `SharedToken.ColorBorderBg` | 徽标外围阴影颜色 |

### 动画 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `BadgeProcessingDuration` | `TimeSpan` | 固定 `1200ms` | Processing 状态圆点的动画周期 |

### 间距与圆角 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CountBadgeTextPadding` | `Thickness` | `(SharedToken.UniformlyPaddingXXS, 0)` | 数字徽标文字内间距 |
| `CountBadgeCornerRadius` | `CornerRadius` | `IndicatorHeight`（全圆角） | 默认数字徽标圆角 |
| `CountBadgeCornerRadiusSM` | `CornerRadius` | `IndicatorHeightSM`（全圆角） | 小号数字徽标圆角 |
| `DotBadgeLabelMargin` | `Thickness` | `(SharedToken.UniformlyMarginXS, 0, 0, 0)` | 圆点徽标文字与圆点之间的间距 |

### 缎带 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `BadgeRibbonOffset` | `Point` | `(SharedToken.UniformlyMarginXS, SharedToken.UniformlyMarginXS)` | 缎带与容器角落的偏移量 |
| `BadgeRibbonCornerTransform` | `ImmutableTransform?` | `ScaleTransform(1, 0.75)` | 缎带折角的缩放变换 |
| `BadgeRibbonCornerDarkenAmount` | `int` | 固定 `15` | 折角颜色相对缎带色的加深程度 |
| `BadgeRibbonTextPadding` | `Thickness` | `(SharedToken.UniformlyPaddingXS, 0)` | 缎带文字内间距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Badge 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### CountBadge 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `MotionDurationMid` | 显隐动画持续时间 |

### DotBadge 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `ColorError` | Error 状态圆点颜色 |
| `ColorSuccess` | Success 状态圆点颜色 |
| `ColorWarning` | Warning 状态圆点颜色 |
| `ColorInfo` | Processing 状态圆点颜色 |
| `ColorTextPlaceholder` | Default 状态圆点颜色 |
| `MotionDurationMid` | 显隐动画持续时间 |

### RibbonBadge 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | 缎带默认颜色（主题主色） |
| `BorderRadiusSM` | 缎带圆角半径 |
| `ColorTextLightSolid` | 缎带文字颜色（白色） |

---

## Token 对外观的具体影响

### 数字徽标（CountBadge）

| 视觉元素 | 正常态 Token | 说明 |
|---|---|---|
| 背景色 | `BadgeColor`（默认红色）或自定义 `BadgeColor` | `BadgeColor` 属性优先于 Token 默认值 |
| 文字色 | `BadgeTextColor`（白色） | 数字文本始终为白色 |
| 外围阴影 | `BadgeShadowSize` + `BadgeShadowColor` | 形成轮廓描边效果，增强边缘清晰度 |
| 默认尺寸高度 | `IndicatorHeight` | 数字气泡高度 |
| 默认尺寸最小宽度 | `IndicatorHeight` | 保证至少为正圆 |
| 默认尺寸圆角 | `CountBadgeCornerRadius` | 全圆角 |
| 默认尺寸字号 | `TextFontSize` | 数字文字大小 |
| 小号尺寸高度 | `IndicatorHeightSM` | 小号气泡高度 |
| 小号尺寸最小宽度 | `IndicatorHeightSM` | 小号最小宽度 |
| 小号尺寸圆角 | `CountBadgeCornerRadiusSM` | 小号全圆角 |
| 小号尺寸字号 | `TextFontSizeSM` | 小号文字大小 |
| 文字内间距 | `CountBadgeTextPadding` | 数字两侧留白 |

### 圆点徽标（DotBadge）

| 视觉元素 | Token | 说明 |
|---|---|---|
| 圆点尺寸 | `DotSize` | 圆点的宽度和高度 |
| 默认圆点颜色 | `BadgeColor`（红色） | 未设 `Status` 和 `DotColor` 时的默认色 |
| Error 状态色 | `SharedToken.ColorError` | 红色 |
| Success 状态色 | `SharedToken.ColorSuccess` | 绿色 |
| Warning 状态色 | `SharedToken.ColorWarning` | 黄色 |
| Processing 状态色 | `SharedToken.ColorInfo` | 蓝色 |
| Default 状态色 | `SharedToken.ColorTextPlaceholder` | 灰色 |
| 外围阴影 | `BadgeShadowSize` + `BadgeShadowColor` | 圆点轮廓描边 |
| 文字间距 | `DotBadgeLabelMargin` | 圆点与文字之间的距离 |

### 缎带徽标（RibbonBadge）

| 视觉元素 | Token | 说明 |
|---|---|---|
| 缎带背景色 | `SharedToken.ColorPrimary`（默认）或自定义 `RibbonColor` | 缎带矩形的填充色 |
| 缎带文字色 | `SharedToken.ColorTextLightSolid` | 缎带上的文字颜色（白色） |
| 缎带圆角 | `SharedToken.BorderRadiusSM` | 缎带矩形的圆角半径 |
| 缎带偏移量 | `BadgeRibbonOffset` | 缎带相对容器角落的位置偏移 |
| 缎带文字间距 | `BadgeRibbonTextPadding` | 缎带文字两侧留白 |
| 折角缩放 | `BadgeRibbonCornerTransform` | 折角三角形的变换（Y 方向缩放 0.75） |
| 折角加深量 | `BadgeRibbonCornerDarkenAmount` | 折角颜色比缎带色加深 15 |
| 文字行高 | `BadgeFontHeight` | 缎带文字的行高 |
