# ProgressBar API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;        // ProgressBar, CircleProgress, DashboardProgress, StepsProgressBar
namespace AtomUI.Controls.Commons;         // Abstract 基类
namespace AtomUI.Controls;                 // 枚举、伪类常量
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### ProgressStatus（来自 `AtomUI.Controls`）

进度状态枚举。

| 值 | 说明 |
|---|---|
| `Normal` | 正常进行中（默认），使用 `ColorInfo` 主色 |
| `Success` | 成功完成，使用 `ColorSuccess` 成功色 |
| `Exception` | 异常/失败，使用 `ColorError` 错误色，显示异常图标 |
| `Active` | 活跃进行中（线形进度条专属），主色 + 动画效果 |

### DashboardGapPosition（来自 `AtomUI.Controls`）

仪表盘缺口位置枚举。

| 值 | 说明 |
|---|---|
| `Left` | 缺口在左侧 |
| `Top` | 缺口在顶部 |
| `Right` | 缺口在右侧 |
| `Bottom` | 缺口在底部（默认） |

### LinePercentAlignment（来自 `AtomUI.Controls`）

百分比标签对齐方式枚举。

| 值 | 说明 |
|---|---|
| `Start` | 起始位置（水平→左侧，垂直→顶部） |
| `Center` | 居中（水平→下方居中，垂直→右侧居中） |
| `End` | 末尾位置（水平→右侧，垂直→底部）。默认值 |

### PercentPosition（来自 `AtomUI.Controls`，record struct）

百分比标签位置结构体，控制标签是否在进度条内部及其对齐方式。

| 属性 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsInner` | `bool` | `false` | 是否将百分比标签放在进度条内部 |
| `Alignment` | `LinePercentAlignment` | `End` | 标签对齐方式 |

### SizeType（来自 `AtomUI.Controls`）

尺寸类型枚举。

| 值 | 说明 |
|---|---|
| `Small` | 小号（线宽 4px） |
| `Middle` | 中号（线宽 6px） |
| `Large` | 大号（线宽 8px，默认） |

---

## 所有进度条共享属性（AbstractProgressBar）

以下属性由所有四种进度条控件共享，定义在 `AbstractProgressBar` 基类中。

### 公共属性（StyledProperty / DirectProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Value` | `double` | `0` | 当前进度值（继承自 `RangeBase`） |
| `Minimum` | `double` | `0` | 最小值（继承自 `RangeBase`） |
| `Maximum` | `double` | `100` | 最大值（继承自 `RangeBase`） |
| `Percentage` | `double` | — | 只读。自动计算的百分比 = (Value - Minimum) / (Maximum - Minimum) × 100 |
| `IsIndeterminate` | `bool` | `false` | 是否为不定态（显示持续动画而非具体进度） |
| `IsShowProgressInfo` | `bool` | `true` | 是否显示进度信息（百分比文字或状态图标） |
| `ProgressTextFormat` | `string` | `"{0:0}%"` | 进度文字格式化字符串，`{0}` 为百分比值 |
| `StrokeBrush` | `IBrush?` | 由主题 Token 控制 | 进度指示器描边颜色，支持纯色和渐变 |
| `TrailColor` | `Color?` | `null` | 轨道（未完成部分）颜色，设置后覆盖默认 `GrooveBrush` |
| `StrokeLineCap` | `PenLineCap` | `PenLineCap.Round` | 端点样式：`Round`（圆角）、`Flat`（平头）、`Square`（方头） |
| `SizeType` | `SizeType` | `SizeType.Large` | 尺寸类型，影响默认线宽 |
| `Status` | `ProgressStatus` | `ProgressStatus.Normal` | 进度状态，影响颜色和图标 |
| `IndicatorThickness` | `double` | `NaN`（自动） | 自定义进度条粗细，设置后覆盖尺寸自动计算 |
| `SuccessThreshold` | `double` | `NaN`（禁用） | 成功阈值，达到该值的部分使用 `SuccessStrokeBrush` |
| `SuccessStrokeBrush` | `IBrush?` | 由主题控制 | 成功阈值段的描边颜色 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用过渡动画 |
| `ExceptionCompletedIcon` | `PathIcon?` | 自动设置 | 异常状态完成图标（线形：CloseCircleFilled，圆形：CloseOutlined） |
| `SuccessCompletedIcon` | `PathIcon?` | 自动设置 | 成功状态完成图标（线形：CheckCircleFilled，圆形：CheckOutlined） |

---

## ProgressBar 线形进度条

**继承**：`RangeBase` → `AbstractProgressBar` → `AbstractLineProgress` → `AbstractGeneralProgressBar` → `ProgressBar`

### 专有属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientation` | `Horizontal` | 方向：`Horizontal`（水平）或 `Vertical`（垂直） |
| `PercentPosition` | `PercentPosition` | `{ IsInner=false, Alignment=End }` | 百分比标签位置。支持 Inner（进度条内部）和 Outer（外部）模式 |

---

## CircleProgress 圆形进度条

**继承**：`RangeBase` → `AbstractProgressBar` → `AbstractCircleProgress` → `AbstractGeneralCircleProgress` → `CircleProgress`

### 专有属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `StepCount` | `int` | `0` | 步进段数量。>0 时将圆环分为等分段 |
| `StepGap` | `double` | `2` | 步进段之间的间隙（像素） |

### 尺寸行为

| SizeType | 默认直径 |
|---|---|
| `Large` | 120px |
| `Middle` | 90px |
| `Small` | 60px |

圆形进度条会根据可用空间自动调整大小。设置 `Width`/`Height` 可精确控制尺寸。

---

## DashboardProgress 仪表盘进度条

**继承**：`RangeBase` → `AbstractProgressBar` → `AbstractCircleProgress` → `AbstractGeneralDashboardProgress` → `DashboardProgress`

### 专有属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `DashboardGapPosition` | `DashboardGapPosition` | `Bottom` | 仪表盘缺口位置 |
| `GapDegree` | `double` | `75` | 仪表盘缺口角度（0–295） |
| `StepCount` | `int` | `0` | 步进段数量（继承自 `AbstractCircleProgress`） |
| `StepGap` | `double` | `2` | 步进段间隙（继承自 `AbstractCircleProgress`） |

---

## StepsProgressBar 步骤进度条

**继承**：`RangeBase` → `AbstractProgressBar` → `AbstractLineProgress` → `AbstractGeneralStepsProgressBar` → `StepsProgressBar`

### 专有属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Orientation` | `Orientation` | `Horizontal` | 方向（继承自 `AbstractLineProgress`） |
| `Steps` | `int` | `1` | 步骤总数（最小 1） |
| `ChunkWidth` | `double` | `NaN`（自动） | 每个块的宽度。自动时根据 SizeType：Large=14, Middle=6, Small=2 |
| `ChunkHeight` | `double` | `NaN`（自动） | 每个块的高度 |
| `StepsStrokeBrush` | `List<IBrush>?` | `null` | 每个步骤的独立颜色列表。`null` 时使用统一 `StrokeBrush` |
| `PercentPosition` | `LinePercentAlignment` | `End` | 百分比标签位置 |

---

## 伪类（Pseudo-Classes）

所有进度条共享以下伪类：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:indeterminate` | `ProgressBarPseudoClass.Indeterminate` | `IsIndeterminate == true` |
| `:completed` | `ProgressBarPseudoClass.Completed` | `Value == Maximum` |

### 线形进度条（ProgressBar）额外伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:horizontal` | `ProgressBarPseudoClass.Horizontal` | `Orientation == Horizontal` |
| `:vertical` | `ProgressBarPseudoClass.Vertical` | `Orientation == Vertical` |
| `:labelinner` | `ProgressBarPseudoClass.PercentLabelInner` | `PercentPosition.IsInner == true` |
| `:labelinner-start` | `ProgressBarPseudoClass.PercentLabelInnerStart` | Inner 模式下 `Alignment == Start` |
| `:labelinner-center` | `ProgressBarPseudoClass.PercentLabelInnerCenter` | Inner 模式下 `Alignment == Center` |
| `:labelinner-end` | `ProgressBarPseudoClass.PercentLabelInnerEnd` | Inner 模式下 `Alignment == End` |

### StepsProgressBar 额外伪类

| 伪类 | 触发条件 |
|---|---|
| `:horizontal` / `:vertical` | 与 ProgressBar 相同 |

---

## 实现的接口

### ISizeTypeAware

```csharp
public SizeType SizeType { get; set; }
```

控制进度条的默认粗细。线形进度条：Large=8px, Middle=6px, Small=4px。圆形进度条根据直径自动按比例计算。

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制 `Value`、`StrokeBrush`、`Foreground` 属性变化时的过渡动画。
