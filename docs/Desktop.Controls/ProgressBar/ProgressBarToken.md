# ProgressBar Design Token

所有进度条变体（ProgressBar、CircleProgress、DashboardProgress、StepsProgressBar）共享同一个 `ProgressBarToken`（Token ID: `"ProgressBar"`），所有视觉属性均从全局 `SharedToken` 派生。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:ProgressBarTokenResource DefaultColor}
{atom:ProgressBarTokenResource RemainingColor}
{atom:ProgressBarTokenResource CircleTextColor}
{atom:ProgressBarTokenResource LineBorderRadius}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorInfo}
{atom:SharedTokenResource ColorSuccess}
{atom:SharedTokenResource ColorError}
```

---

## 组件级 Token 一览

以下是 `ProgressBarToken` 定义的全部组件级 Token，按功能分组说明。

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `DefaultColor` | `Color` | `SharedToken.ColorInfo` | 进度指示器默认颜色（Normal 状态下使用） |
| `RemainingColor` | `Color` | `SharedToken.ColorFillSecondary` | 轨道（未完成部分）颜色 |
| `CircleTextColor` | `Color` | `SharedToken.ColorText` | 圆形/仪表盘进度条中心百分比文字颜色 |

### 圆角 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `LineBorderRadius` | `CornerRadius` | 固定 `100`（胶囊形） | 线形进度条圆角半径（非常大的值实现胶囊效果） |

### 圆形进度条 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `CircleMinimumTextFontSize` | `double` | `SharedToken.FontSizeSM - 2` | 圆形进度条百分比文字最小字号 |
| `CircleMinimumIconSize` | `double` | `SharedToken.SizeXS` | 圆形进度条状态图标最小尺寸 |

### 线形进度条 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `LineInfoIconSize` | `double` | `SharedToken.IconSize` | 线形进度条信息图标尺寸（正常大小） |
| `LineInfoIconSizeSM` | `double` | `SharedToken.IconSizeSM` | 线形进度条信息图标尺寸（小号） |
| `LineExtraInfoMargin` | `double` | `SharedToken.ControlPaddingHorizontalSM` | 百分比标签与进度条之间的间距 |
| `LineProgressPadding` | `double` | `SharedToken.UniformlyPaddingXXS / 2` | 线形进度条内间距（Inner 模式下标签与边缘的间距） |

### 步骤进度条内部 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ProgressStepMinWidth` | `double` | — | 步骤进度条最小宽度 |
| `ProgressStepMarginInlineEnd` | `Thickness` | — | 步骤之间的间距 |
| `ProgressActiveMotionDuration` | `double` | — | Active 状态动画持续时间 |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，进度条的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `ColorInfo` | Normal 状态的指示器颜色（蓝色） |
| `ColorSuccess` | Success 状态/完成态的指示器颜色（绿色） |
| `ColorError` | Exception 状态的指示器和图标颜色（红色） |
| `ColorFillSecondary` | 轨道（未完成部分）默认颜色 |
| `ColorText` | 线形进度条百分比文字颜色 |
| `ColorTextLightSolid` | Inner 模式下浅色背景上的文字颜色 |
| `FontSize` / `FontSizeSM` | 百分比文字字号（正常/小号） |
| `FontFamily` | 百分比文字字体 |
| `IconSize` / `IconSizeSM` | 状态图标尺寸 |
| `EnableMotion` | 全局动画开关 |
| `MotionDurationVerySlow` | Value 值过渡动画持续时间 |
| `MotionDurationFast` | 颜色过渡动画持续时间 |

---

## Token 对外观的具体影响

### 状态与颜色映射

| ProgressStatus | 指示器颜色 | 文字/图标颜色 | 说明 |
|---|---|---|---|
| `Normal` | `ColorInfo` | `ColorText` | 默认蓝色进度 |
| `Success` | `ColorSuccess` | `ColorSuccess` | 绿色完成态 |
| `Exception` | `ColorError` | `ColorError` | 红色异常态 |
| `Active` | `ColorInfo` + 动画 | `ColorText` | 蓝色 + 脉冲动画 |
| 完成（`Value == Maximum`） | `ColorSuccess` | 显示 ✓ 图标 | 自动切换到成功色 |

### 尺寸与线宽映射

#### 线形进度条（ProgressBar / StepsProgressBar）

| SizeType | 线宽 | 字号 | 图标大小 |
|---|---|---|---|
| `Large` | 8px | `FontSize` | `LineInfoIconSize` |
| `Middle` | 6px | `FontSize` | `LineInfoIconSize` |
| `Small` | 4px | `FontSizeSM` | `LineInfoIconSizeSM` |

#### 圆形/仪表盘进度条（CircleProgress / DashboardProgress）

| SizeType | 默认直径 | 线宽（自动计算） | 字号（自动计算） |
|---|---|---|---|
| `Large` | 120px | ≈ 8px | 直径 × 0.15 + 6 |
| `Middle` | 90px | ≈ 6px | 直径 × 0.15 + 6 |
| `Small` | 60px | ≈ 4px | 直径 × 0.15 + 6（≥ CircleMinimumTextFontSize） |

#### 步骤进度条默认块宽度

| SizeType | ChunkWidth |
|---|---|
| `Large` | 14px |
| `Middle` | 6px |
| `Small` | 2px |

块间距固定为 2px（`DEFAULT_CHUNK_SPACE`）。

### 过渡动画

当 `IsMotionEnabled == true` 时，以下属性变化会有平滑过渡：

| 属性 | 过渡类型 | 持续时间 | 缓动函数 |
|---|---|---|---|
| `Value` | `DoubleTransition` | `MotionDurationVerySlow` | `ExponentialEaseOut` |
| `StrokeBrush` | `SolidColorBrushTransition` | `MotionDurationFast` | 线性 |
| `Foreground` | `SolidColorBrushTransition` | `MotionDurationFast` | 线性 |
