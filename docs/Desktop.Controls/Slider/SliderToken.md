# Slider Design Token

Slider 控件使用 `SliderToken`（Token ID: `"Slider"`）作为组件级 Design Token。所有 Token 值均从全局 `SharedToken` 派生，遵循 Ant Design 5.0 的三层 Token 派生体系。

> 📖 Token 源码位置：`src/AtomUI.Desktop.Controls/Slider/SliderToken.cs`

---

## Token 基础信息

| 项目 | 值 |
|---|---|
| Token 类 | `SliderToken`（`internal`，标记 `[ControlDesignToken]`） |
| Token ID | `"Slider"` |
| ScopeProvider | `SliderToken.ScopeProvider` |
| 基类 | `AbstractControlDesignToken` |
| TokenKind 类型 | `SliderTokenKind`（源码生成） |

---

## Token 资源访问方式

在 AXAML 中通过 `atom:SliderTokenResource` 或 `atom:TokenResource` 访问：

```xml
<!-- 组件级 Token -->
{atom:SliderTokenResource SliderTrackSize}
{atom:SliderTokenResource ThumbCircleSize}
{atom:SliderTokenResource RailBg}

<!-- 全局 Token（SharedToken） -->
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBgContainer}
```

---

## 组件级 Token 一览

### 尺寸 Token

| Token 名 | 类型 | 默认值 / 来源 | 说明 |
|---|---|---|---|
| `SliderTrackSize` | `double` | `SharedToken.ControlHeightSM / 2` | 滑动条控件的高度（包含滑块活动区域） |
| `RailSize` | `double` | `4` | 轨道条高度（细长条） |
| `MarkSize` | `double` | `8` | 刻度标记圆点大小 |
| `ThumbSize` | `double` | 计算值（`ThumbCircleSizeHover + 边框 + outline`） | Thumb 推荐的最大尺寸，用于布局 |
| `ThumbCircleSize` | `double` | `SharedToken.ControlHeightLG / 4` | 滑块圆形尺寸（默认态） |
| `ThumbCircleSizeHover` | `double` | `SharedToken.ControlHeightSM / 2` | 滑块圆形尺寸（悬浮态），比默认态略大 |
| `ThumbCircleBorderThickness` | `Thickness` | `SharedToken.LineWidth + 1` | 滑块边框宽度（默认态） |
| `ThumbCircleBorderThicknessHover` | `Thickness` | `SharedToken.LineWidth + 1.5` | 滑块边框宽度（悬浮态），比默认态略粗 |

### 轨道颜色 Token

| Token 名 | 类型 | 默认值 / 来源 | 说明 | 影响范围 |
|---|---|---|---|---|
| `RailBg` | `Color` | `SharedToken.ColorFillTertiary` | 轨道底层背景色（灰色） | 轨道未覆盖部分的颜色 |
| `RailHoverBg` | `Color` | `SharedToken.ColorFillSecondary` | 轨道底层悬浮态背景色 | 鼠标悬浮时轨道底层颜色加深 |
| `TrackBg` | `Color` | `SharedToken.ColorPrimaryBorder` | 轨道已覆盖部分背景色（主色） | 从 Minimum 到 Value 的高亮部分 |
| `TrackHoverBg` | `Color` | `SharedToken.ColorPrimaryBorderHover` | 轨道已覆盖部分悬浮态背景色 | 鼠标悬浮时高亮部分颜色变化 |
| `TrackBgDisabled` | `Color` | `SharedToken.ColorBgContainerDisabled` | 轨道禁用态背景色 | 禁用时高亮部分变灰 |

### 刻度标记颜色 Token

| Token 名 | 类型 | 默认值 / 来源 | 说明 | 影响范围 |
|---|---|---|---|---|
| `MarkBorderColor` | `Color` | `SharedToken.ColorBorderSecondary` | 刻度标记圆点边框颜色（默认态） | 未被覆盖范围内的刻度标记 |
| `MarkBorderColorHover` | `Color` | `SharedToken.ColorFillContentHover` | 刻度标记圆点悬浮态颜色 | 鼠标悬浮在轨道时刻度标记变化 |
| `MarkBorderColorActive` | `Color` | `SharedToken.ColorPrimaryBorder` | 刻度标记激活态颜色（主色） | 被覆盖范围内的刻度标记 |

### 滑块颜色 Token

| Token 名 | 类型 | 默认值 / 来源 | 说明 | 影响范围 |
|---|---|---|---|---|
| `ThumbCircleBorderColor` | `Color` | `SharedToken.ColorPrimaryBorder` | 滑块圆形边框颜色（默认态） | 滑块正常显示时的边框 |
| `ThumbCircleBorderHoverColor` | `Color` | `SharedToken.ColorPrimaryBorderHover` | 滑块边框悬浮态颜色 | 鼠标悬浮在 Slider 区域但未悬浮在滑块上时 |
| `ThumbCircleBorderActiveColor` | `Color` | `SharedToken.ColorPrimary` | 滑块边框激活态颜色 | `:pointerover` 和 `:focus` 状态 |
| `ThumbCircleBorderColorDisabled` | `Color` | 混合计算（`ColorTextDisabled` over `ColorBgContainer`） | 滑块禁用颜色 | 禁用态的滑块边框 |
| `ThumbOutlineColor` | `Color` | 基于 `ColorPrimary` 的 20% 透明度 | 滑块 outline 环颜色 | 聚焦/悬浮时的外圈光晕效果 |
| `ThumbOutlineThickness` | `Thickness` | `SharedToken.WaveAnimationRange` | outline 环厚度 | 聚焦/悬浮时外圈光晕的粗细 |

### 内边距 Token

| Token 名 | 类型 | 默认值 / 来源 | 说明 | 影响范围 |
|---|---|---|---|---|
| `SliderPaddingHorizontal` | `Thickness` | `(SliderTrackSize/2, (ControlHeight-SliderTrackSize)/2)` | 水平方向内边距 | 水平 Slider 的上下和左右内间距 |
| `SliderPaddingVertical` | `Thickness` | `((ControlHeight-SliderTrackSize)/2, SliderTrackSize/2)` | 垂直方向内边距 | 垂直 Slider 的上下和左右内间距 |
| `MarginPartWithMark` | `Thickness` | `(0, 0, 0, ControlHeightLG-SliderTrackSize)` | 有刻度标记时的额外边距 | 当存在 Marks 时为标签文字留出空间 |

---

## Token 派生关系图

```
SharedToken (全局 DesignToken)
├── ControlHeightLG ──────► ThumbCircleSize = ControlHeightLG / 4
├── ControlHeightSM ──────► ThumbCircleSizeHover = ControlHeightSM / 2
│                          └► SliderTrackSize = ControlHeightSM / 2
├── LineWidth ────────────► ThumbCircleBorderThickness = LineWidth + 1
│                          └► ThumbCircleBorderThicknessHover = LineWidth + 1.5
├── ColorFillTertiary ────► RailBg
├── ColorFillSecondary ───► RailHoverBg
├── ColorPrimaryBorder ───► TrackBg, MarkBorderColorActive, ThumbCircleBorderColor
├── ColorPrimaryBorderHover ► TrackHoverBg, ThumbCircleBorderHoverColor
├── ColorPrimary ─────────► ThumbCircleBorderActiveColor
│                          └► ThumbOutlineColor (20% opacity)
├── ColorBorderSecondary ─► MarkBorderColor
├── ColorFillContentHover ► MarkBorderColorHover
├── ColorBgContainerDisabled ► TrackBgDisabled
├── ColorTextDisabled + ColorBgContainer ► ThumbCircleBorderColorDisabled
├── WaveAnimationRange ───► ThumbOutlineThickness
└── ControlHeight ────────► SliderPaddingHorizontal, SliderPaddingVertical
```

---

## 引用的全局 Token（通过 SharedTokenResource 直接使用）

除组件级 Token 外，Slider 主题中还直接引用了以下全局 Token：

| SharedToken | 使用位置 | 说明 |
|---|---|---|
| `EnableMotion` | SliderTheme, SliderTrackTheme | 控制是否启用过渡动画 |
| `ColorBgContainer` | SliderThumbTheme | 滑块的白色背景填充 |
| `ColorBgElevated` | SliderTrackTheme | 刻度标记圆点的背景色 |
| `ColorText` | SliderTrackTheme | 刻度标签文字颜色（正常态） |
| `ColorTextDisabled` | SliderTrackTheme | 刻度标签文字颜色（禁用态） |

---

## 暗黑模式

`SliderToken.CalculateTokenValues(bool isDarkMode)` 接收 `isDarkMode` 参数，所有颜色值从 `SharedToken` 派生，而 `SharedToken` 在暗黑模式下会自动使用 `DarkThemeVariantCalculator` 计算的暗色系色彩。因此 Slider 在暗黑模式下无需额外处理，颜色会自动适配。
