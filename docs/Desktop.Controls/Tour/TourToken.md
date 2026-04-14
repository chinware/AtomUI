# Tour Design Token

Tour 控件使用 `TourToken`（Token ID: `"Tour"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:TourTokenResource CloseBtnSize}
{atom:TourTokenResource PrimaryPrevBtnBg}
{atom:TourTokenResource TourBorderRadius}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBgElevated}
{atom:SharedTokenResource ColorBgMask}
```

---

## 组件级 Token 一览

以下是 `TourToken` 定义的全部组件级 Token，按功能分组说明。

### 外观 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CloseBtnSize` | `double` | `SharedToken.FontSize * SharedToken.RelativeLineHeight` | 关闭按钮尺寸（宽高） |
| `HeaderColor` | `Color` | `SharedToken.ColorTextHeading` | 标题文字颜色 |
| `TourBorderRadius` | `CornerRadius` | `SharedToken.BorderRadiusLG` | Tour 气泡卡片的圆角半径 |

### 尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TourViewMinWidth` | `double` | 固定 `200` | Tour 内容视图（TourStepsView）最小宽度 |
| `TourViewMinHeight` | `double` | 固定 `120` | Tour 内容视图（TourStepsView）最小高度 |
| `IndicatorSize` | `double` | 固定 `6` | 默认圆点指示器尺寸 |
| `PopupMarginToAnchor` | `double` | `SharedToken.SpacingXXS` | Popup 与目标控件的默认间距 |

### Primary 风格按钮 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `PrimaryPrevBtnBg` | `Color` | `SharedToken.ColorTextLightSolid` 透明度 15% | Primary 模式下"上一步"按钮背景色 |
| `PrimaryNextBtnHoverBg` | `Color` | `ColorUtils.OnBackground(SharedToken.ColorBgTextHover, SharedToken.ColorWhite)` | Primary 模式下"下一步"按钮悬浮背景色 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Tour 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### 颜色相关

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | Primary 风格下 ArrowDecoratedBox 背景色 |
| `ColorBgElevated` | Default 风格下 ArrowDecoratedBox 背景色 |
| `ColorBgMask` | Tour 遮罩层默认颜色 |
| `ColorTextLightSolid` | Primary 风格下文字颜色、关闭按钮图标颜色 |
| `ColorTextHeading` | Default 风格下标题颜色（通过 `HeaderColor` Token） |
| `ColorFill` | 默认圆点指示器非活跃颜色 |
| `ColorWhite` | Primary 风格下导航按钮前景色 |

### 间距相关

| Token 资源键 | 使用场景 |
|---|---|
| `SpacingXS` | TourStep 内部垂直间距、TourStepsView 操作按钮间距、导航栏水平间距 |
| `SpacingXXS` | TourStepsView 内容与导航栏间距、圆点指示器项间距 |
| `PaddingXXS` | TourStepsView 整体内间距 |

### 字体相关

| Token 资源键 | 使用场景 |
|---|---|
| `FontWeightStrong` | 步骤标题字体粗细 |
| `IconSize` | 关闭按钮图标尺寸 |

### 阴影相关

| Token 资源键 | 使用场景 |
|---|---|
| `BoxShadowsTertiary` | Tour Popup 的投影阴影 |

### 动画相关

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | Tour 默认动画开关 |

---

## Token 对外观的具体影响

### 风格类型与 Token 映射

| 风格 | ArrowDecoratedBox 背景 | 文字颜色 | 标题颜色 | 上一步按钮背景 | 下一步/完成按钮背景 |
|---|---|---|---|---|---|
| **Default** | `ColorBgElevated` | 默认前景色 | `HeaderColor`（`ColorTextHeading`） | 标准 Button | 标准 Primary Button |
| **Primary** | `ColorPrimary` | `ColorTextLightSolid` | `ColorTextLightSolid` | `PrimaryPrevBtnBg` | `ColorWhite` + `ColorPrimary` 前景 |

### 指示器与 Token 映射

| 风格 | 非活跃圆点 | 活跃圆点 |
|---|---|---|
| **Default** | `ColorFill` | `ColorPrimary` |
| **Primary** | `PrimaryPrevBtnBg` | `ColorTextLightSolid` |

### 导航按钮悬浮态

| 风格 | 上一步悬浮 | 下一步/完成悬浮 |
|---|---|---|
| **Default** | 标准 Button hover | 标准 Primary Button hover |
| **Primary** | `PrimaryPrevBtnBg` + 透明边框 | `PrimaryNextBtnHoverBg` |
