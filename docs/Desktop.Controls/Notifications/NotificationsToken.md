# Notification Design Token

Notification 控件使用 `NotificationToken`（Token ID: `"Notification"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:NotificationTokenResource NotificationBg}
{atom:NotificationTokenResource NotificationPadding}
{atom:NotificationTokenResource NotificationWidth}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorSuccess}
{atom:SharedTokenResource BorderRadiusLG}
```

---

## 组件级 Token 一览

以下是 `NotificationToken` 定义的全部组件级 Token，按功能分组说明。

### 背景与颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `NotificationBg` | `Color` | `SharedToken.ColorBgElevated` | 通知卡片背景色 |
| `NotificationProgressBg` | `IImmutableBrush?` | 线性渐变（`ColorPrimaryHover` → `ColorPrimary`） | 进度条渐变背景色 |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `NotificationWidth` | `double` | 固定值 `384` | 通知卡片宽度 |
| `NotificationIconSize` | `double` | `SharedToken.FontSizeLG * SharedToken.RelativeLineHeightLG` | 类型图标尺寸 |
| `NotificationCloseButtonSize` | `double` | `SharedToken.ControlHeightLG * 0.55` | 关闭按钮尺寸 |
| `NotificationProgressHeight` | `double` | 固定值 `2` | 进度条高度 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `NotificationPadding` | `Thickness` | `SharedToken.UniformlyPaddingLG` (左右), `SharedToken.UniformlyPaddingMD` (上), `0` (下) | 通知卡片内间距 |
| `NotificationIconMargin` | `Thickness` | `0, 0, SharedToken.UniformlyMarginSM, 0` | 类型图标右外边距 |
| `NotificationCloseButtonPadding` | `Thickness` | `SharedToken.PaddingXXS` | 关闭按钮内间距 |
| `NotificationMarginBottom` | `Thickness` | `0, 0, 0, SharedToken.UniformlyMargin` | 通知卡片之间的底部间距 |
| `NotificationTopMargin` | `Thickness` | `SharedToken.UniformlyMarginLG` (左、上、右), `0` (下) | 顶部位置时的卡片外边距 |
| `NotificationBottomMargin` | `Thickness` | `SharedToken.UniformlyMarginLG` (左、右、下), `0` (上) | 底部位置时的卡片外边距 |
| `NotificationProgressMargin` | `Thickness` | `0, 0, 0, 1` | 进度条外边距 |
| `NotificationContentMargin` | `Thickness` | `0, 0, 0, SharedToken.UniformlyPaddingMD` | 正文内容区域外边距 |
| `HeaderMargin` | `Thickness` | `0, 0, 0, SharedToken.UniformlyMarginXS` | 标题栏底部外边距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Notification 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制 `IsMotionEnabled` 默认值 |
| `MotionDurationMid` | 出入动画时长 |
| `BoxShadows` | 通知卡片的阴影效果 |
| `BorderRadiusLG` | 通知卡片圆角半径 |
| `BorderRadiusSM` | 关闭按钮圆角半径 |
| `ColorPrimary` | Information 类型图标颜色 |
| `ColorSuccess` | Success 类型图标颜色 |
| `ColorWarning` | Warning 类型图标颜色 |
| `ColorError` | Error 类型图标颜色 |
| `ColorTextHeading` | 标题文本颜色 |
| `ColorText` | 正文文本颜色 |
| `FontSize` | 正文字体大小 |
| `FontSizeLG` | 标题字体大小 |
| `FontHeight` | 正文行高 |
| `FontHeightLG` | 标题行高 |
| `IconSizeSM` | 关闭按钮图标尺寸 |

---

## Token 对外观的具体影响

### 通知卡片布局

通知卡片的整体布局由以下 Token 控制：

| 区域 | Token | 说明 |
|---|---|---|
| 卡片宽度 | `NotificationWidth` | 固定 384px（与 Ant Design 对齐） |
| 卡片背景 | `NotificationBg` | 从 `ColorBgElevated` 派生，支持明暗主题切换 |
| 卡片圆角 | `BorderRadiusLG`（SharedToken） | 大圆角样式 |
| 卡片阴影 | `BoxShadows`（SharedToken） | 浮层阴影效果 |
| 卡片内间距 | `NotificationPadding` | 上：PaddingMD，左右：PaddingLG，下：0 |

### 图标区域

| 区域 | Token | 说明 |
|---|---|---|
| 图标尺寸 | `NotificationIconSize` | 基于 `FontSizeLG * RelativeLineHeightLG` 计算 |
| 图标间距 | `NotificationIconMargin` | 图标右侧留出 `UniformlyMarginSM` 间距 |
| 图标颜色 | `ColorPrimary` / `ColorSuccess` / `ColorWarning` / `ColorError` | 根据 `NotificationType` 选择 |

### 标题区域

| 区域 | Token | 说明 |
|---|---|---|
| 标题字号 | `FontSizeLG`（SharedToken） | 大号字体 |
| 标题行高 | `FontHeightLG`（SharedToken） | 大号行高 |
| 标题颜色 | `ColorTextHeading`（SharedToken） | 标题专用色 |
| 标题间距 | `HeaderMargin` | 标题栏底部留出 `UniformlyMarginXS` 间距 |

### 关闭按钮

| 区域 | Token | 说明 |
|---|---|---|
| 按钮圆角 | `BorderRadiusSM`（SharedToken） | 小圆角 |
| 按钮内间距 | `NotificationCloseButtonPadding` | 基于 `PaddingXXS` |
| 图标尺寸 | `IconSizeSM`（SharedToken） | 小号图标 |

### 正文区域

| 区域 | Token | 说明 |
|---|---|---|
| 正文字号 | `FontSize`（SharedToken） | 标准字体大小 |
| 正文行高 | `FontHeight`（SharedToken） | 标准行高 |
| 正文颜色 | `ColorText`（SharedToken） | 标准文本色 |
| 正文间距 | `NotificationContentMargin` | 正文底部留出 `UniformlyPaddingMD` 间距 |

### 进度条

| 区域 | Token | 说明 |
|---|---|---|
| 进度条高度 | `NotificationProgressHeight` | 固定 2px |
| 进度条颜色 | `NotificationProgressBg` | 线性渐变（`ColorPrimaryHover` → `ColorPrimary`） |
| 进度条间距 | `NotificationProgressMargin` | 底部 1px 间距 |

### 位置相关边距

| 位置 | Token | 说明 |
|---|---|---|
| Top 系列（TopLeft / TopRight / TopCenter） | `NotificationTopMargin` | 左、上、右边距为 `UniformlyMarginLG`，下为 0 |
| Bottom 系列（BottomLeft / BottomRight / BottomCenter） | `NotificationBottomMargin` | 左、右、下边距为 `UniformlyMarginLG`，上为 0 |

---

## 暗黑模式支持

`NotificationToken.CalculateTokenValues(bool isDarkMode)` 会在暗黑模式下根据 `SharedToken` 的变化自动重新计算所有 Token 值。由于所有颜色和尺寸均从 `SharedToken` 派生，切换主题时通知的背景色（`ColorBgElevated`）、文本色（`ColorText` / `ColorTextHeading`）、图标色（`ColorPrimary` 等）会自动适配暗黑模式，无需额外配置。
