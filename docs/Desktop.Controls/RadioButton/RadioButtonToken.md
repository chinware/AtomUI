# RadioButton Design Token

RadioButton 控件使用 `RadioButtonToken`（Token ID: `"RadioButton"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:RadioButtonTokenResource RadioSize}
{atom:RadioButtonTokenResource DotSize}
{atom:RadioButtonTokenResource TextMargin}
{atom:RadioButtonTokenResource DotColorDisabled}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ColorBorder}
{atom:SharedTokenResource ColorBgContainer}
{atom:SharedTokenResource BorderThickness}
```

---

## 组件级 Token 一览

以下是 `RadioButtonToken` 定义的全部组件级 Token，按功能分组说明。

### 指示器尺寸 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `RadioSize` | `double` | `SharedToken.FontSizeLG` | 单选框圆形指示器的外框大小（宽 = 高 = RadioSize） |
| `DotSize` | `double` | `RadioSize - (DotPadding + LineWidth) × 2` | 选中时内部圆点的最终大小（基于 RadioSize 动态计算） |
| `DotPadding` | `double` | 固定 `4` | 圆点与外框之间的内间距 |

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `RadioColor` | `Color` | `SharedToken.ColorWhite` | 选中态圆点内部颜色（通常为白色） |
| `RadioBgColor` | `Color` | `SharedToken.ColorPrimary` | 选中态指示器背景色（主题主色） |
| `DotColorDisabled` | `Color` | `SharedToken.ColorTextDisabled` | 禁用态选中圆点颜色（灰色） |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `TextMargin` | `Thickness` | `SharedToken.UniformlyMarginXS, 0, SharedToken.UniformlyMarginXS, 0` | 文本内容与指示器之间的左右间距 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，RadioButton 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

### 颜色相关

| Token 资源键 | 使用场景 |
|---|---|
| `ColorText` | 单选框文本颜色（正常态） |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `ColorPrimary` | 选中态指示器边框色和背景色 |
| `ColorBorder` | 未选中态指示器边框色；禁用态指示器边框色 |
| `ColorBgContainer` | 未选中态指示器背景色 |
| `ColorBgContainerDisabled` | 禁用态指示器背景色 |

### 动画相关

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关，控制是否启用过渡动画 |
| `EnableWaveSpirit` | 全局波纹开关，控制是否启用选中波纹效果 |
| `BorderThickness` | 指示器默认边框厚度 |

### 间距相关（RadioButtonGroup 使用）

| Token 资源键 | 使用场景 |
|---|---|
| `SpacingXS` | RadioButtonGroup 的默认 `ItemSpacing` 和 `LineSpacing` 值 |

---

## Token 对外观的具体影响

### 选中状态与 Token 映射

| 状态 | 指示器背景 | 指示器边框 | 圆点颜色 | 文本颜色 |
|---|---|---|---|---|
| **未选中** | `ColorBgContainer` | `ColorBorder` | — | `ColorText` |
| **未选中 + 悬浮** | `ColorBgContainer` | `ColorPrimary` | — | `ColorText` |
| **选中** | `ColorPrimary` | `ColorPrimary` | `RadioColor`（白色） | `ColorText` |
| **禁用（未选中）** | `ColorBgContainerDisabled` | `ColorBorder` | — | `ColorTextDisabled` |
| **禁用（选中）** | `ColorBgContainerDisabled` | `ColorBorder` | `DotColorDisabled`（灰色） | `ColorTextDisabled` |

### 指示器尺寸计算逻辑

```
RadioSize = SharedToken.FontSizeLG
DotPadding = 4                          （内间距，固定值）
DotSize = RadioSize - (DotPadding + SharedToken.LineWidth) × 2  （内部圆点直径）
```

指示器在不同状态下的圆点效果大小（`RadioDotEffectSize`）：
- **选中 + 启用**：`DotSize`（完整大小）
- **选中 + 禁用**：`RadioSize - DotPadding × 2`（稍大，填充更多区域）
- **未选中**：`DotSize × 0.6`（缩小，动画起始点）

### 过渡动画与 Token 的关系

RadioIndicator 的过渡动画在 `IsMotionEnabled = true` 时启用，包括：

| 过渡属性 | 过渡类型 | 持续时间 | 说明 |
|---|---|---|---|
| `RadioBorderBrush` | `SolidColorBrushTransition` | 默认 `MotionDurationMid` | 边框颜色在选中/未选中/悬浮之间平滑切换 |
| `RadioDotEffectSize` | `DoubleTransition` | 默认 `MotionDurationMid` | 圆点大小缩放，产生选中/取消选中的弹性效果 |
| `RadioBackground` | `SolidColorBrushTransition` | `MotionDurationFast` | 背景色在透明/主色之间快速切换 |

### 暗色模式适配

Token 值通过 `CalculateTokenValues(bool isDarkMode)` 在暗色模式下自动适配：
- `SharedToken.ColorPrimary` 在暗色模式下为适配暗色背景的主色调
- `SharedToken.ColorBgContainer` 在暗色模式下为深色背景
- `SharedToken.ColorBorder` 在暗色模式下为低对比度边框色
- 无需额外配置，切换主题时自动响应
