# Spin Design Token

Spin 控件使用 `SpinToken`（Token ID: `"Spin"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:SpinTokenResource DotSize}
{atom:SpinTokenResource DotSizeSM}
{atom:SpinTokenResource DotSizeLG}
{atom:SpinTokenResource IndicatorSize}
{atom:SpinTokenResource IndicatorSizeSM}
{atom:SpinTokenResource IndicatorSizeLG}
{atom:SpinTokenResource IndicatorDuration}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBgMask}
{atom:SharedTokenResource EnableMotion}
{atom:SharedTokenResource SpacingXXS}
```

---

## 组件级 Token 一览

以下是 `SpinToken` 定义的全部组件级 Token，按功能分组说明。

### 圆点尺寸 Token

| Token 名 | 类型 | 计算公式 | 说明 |
|---|---|---|---|
| `DotSize` | `double` | `((ControlHeightLG / 2 - MarginXXS / 2) / 2) × 0.75` | 中号旋转圆点直径 |
| `DotSizeSM` | `double` | `((ControlHeightLG × 0.35 - MarginXXS / 2) / 2) × 0.75` | 小号旋转圆点直径 |
| `DotSizeLG` | `double` | `((ControlHeight - MarginXXS) / 2) × 0.75` | 大号旋转圆点直径 |

**计算逻辑说明：**
- 基准值分别来自 `ControlHeightLG`（中/小号）和 `ControlHeight`（大号），经过缩放和边距扣减后乘以 `0.75` 系数，确保圆点不会占满整个指示器区域，留出适当的旋转空间。
- `MarginXXS`（`UniformlyMarginXXS`）作为圆点之间的最小间距扣减。

### 指示器尺寸 Token

| Token 名 | 类型 | 计算公式 | 说明 |
|---|---|---|---|
| `IndicatorSize` | `double` | `ControlHeightLG / 2 + 2` | 中号指示器整体尺寸（宽/高） |
| `IndicatorSizeSM` | `double` | `ControlHeightLG × 0.35 + 1` | 小号指示器整体尺寸（宽/高） |
| `IndicatorSizeLG` | `double` | `ControlHeight + 4` | 大号指示器整体尺寸（宽/高） |

**指示器尺寸包含圆点加额外间距**（+2 / +1 / +4），确保旋转时圆点不会溢出指示器边界。该尺寸同时用于自定义指示器图标（`PathIcon` / `Icon`）的 `Width` / `Height`。

### 动画 Token

| Token 名 | 类型 | 计算公式 | 说明 |
|---|---|---|---|
| `IndicatorDuration` | `TimeSpan` | `SharedToken.MotionDurationSlow × 4` | 旋转动画一个完整周期（360°）的时长 |

**动画时长说明：**
- `MotionDurationSlow` 是全局慢速动画基准值（默认约 300ms），乘以 4 得到约 1.2 秒的旋转周期。
- 使用 `LinearEasing`（匀速旋转），使四圆点的追光效果均匀连贯。

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Spin 和 SpinIndicator 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### Spin 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | 提示文字（`#Tip`）的前景色 |
| `ColorBgMask` | 遮罩背景色（`IsMaskBackgroundEnabled = true` 时应用） |
| `EnableMotion` | 全局动画开关，控制 `IsMotionEnabled` 默认值 |
| `SpacingXXS` | 指示器与提示文字之间的间距（`StackPanel#IndicatorLayout` 的 `Spacing`） |

### SpinIndicator 使用的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `ColorPrimary` | 圆点填充颜色（`DotBgBrush`），是旋转圆点的主色调 |

### Token 计算依赖的 SharedToken

| Token 资源键 | 使用场景 |
|---|---|
| `ControlHeightLG` | 中号 / 小号指示器和圆点尺寸的计算基准 |
| `ControlHeight` | 大号指示器和圆点尺寸的计算基准 |
| `UniformlyMarginXXS` | 圆点尺寸计算中的间距扣减 |
| `MotionDurationSlow` | 旋转周期时长的计算基准 |

---

## Token 对外观的具体影响

### 尺寸与 Token 映射

| SizeType | 指示器大小 Token | 圆点大小 Token | 自定义图标尺寸 |
|---|---|---|---|
| `Small` | `IndicatorSizeSM` | `DotSizeSM` | `IndicatorSizeSM` × `IndicatorSizeSM` |
| `Middle`（默认） | `IndicatorSize` | `DotSize` | `IndicatorSize` × `IndicatorSize` |
| `Large` | `IndicatorSizeLG` | `DotSizeLG` | `IndicatorSizeLG` × `IndicatorSizeLG` |

### 遮罩状态与 Token 映射

| 状态 | 遮罩方式 | 涉及 Token |
|---|---|---|
| `IsSpinning=False` | 无遮罩，`MaskOpacity = 1.0` | — |
| `IsSpinning=True`, `IsMaskBlurEnabled=False`（默认） | 内容透明度降至 `0.5` | `EnableMotion`（控制过渡动画） |
| `IsSpinning=True`, `IsMaskBlurEnabled=True` | 内容启用高斯模糊（`BlurEffect Radius=5`） | — |
| `IsMaskBackgroundEnabled=True` | 遮罩层显示半透明背景 | `ColorBgMask` |

### 颜色与 Token 映射

| 视觉元素 | Token | 说明 |
|---|---|---|
| 旋转圆点填充色 | `ColorPrimary` | 四个圆点的基础颜色，透明度由正弦波动态调节 |
| 提示文字颜色 | `ColorPrimary` | `Tip` 文本的前景色，与圆点颜色保持一致 |
| 遮罩背景色 | `ColorBgMask` | 半透明遮罩层的背景（仅 `IsMaskBackgroundEnabled = true` 时） |

### 动画与 Token 映射

| 动画行为 | Token | 说明 |
|---|---|---|
| 旋转周期 | `IndicatorDuration` | 指示器完成一次 360° 旋转的时间 |
| 缓动曲线 | `LinearEasing`（硬编码默认） | 匀速旋转，可通过 `MotionEasingCurve` 属性覆盖 |
| 透明度过渡 | `EnableMotion` | 控制 `MaskOpacity` 变化是否使用 `DoubleTransition` 平滑过渡 |
