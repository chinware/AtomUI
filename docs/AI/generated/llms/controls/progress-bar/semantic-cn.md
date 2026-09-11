# ProgressBar 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `ProgressBar` | 反馈控件根语义区域，承载 public API、反馈状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `host` | `宿主或弹层区域` | 承载 overlay、popup、portal、message host、drawer 或 modal 容器。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `surface` | `反馈表面` | 承载背景、边框、阴影、尺寸、placement 和视觉状态。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载标题、正文、图标、进度、结果、操作或关闭入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效区域` | 表达进入退出、loading、progress、skeleton 或水印刷新反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。

## Template Parts

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_LayoutTransformControl` | `LayoutTransformControl` | 承载百分比文本并在垂直内嵌模式中旋转。 |
| `PART_PercentageLabel` | `Label` | 显示格式化后的进度文本。 |
| `PART_ExceptionCompletedIconPresenter` | `IconPresenter` | 显示异常状态图标。 |
| `PART_SuccessCompletedIconPresenter` | `IconPresenter` | 显示成功状态图标。 |

## Pseudo Classes

| `IsIndeterminate` | `bool` | 是否处于不确定进度状态，并同步 `:indeterminate` 伪类。 |
| `IsProgressInfoVisible` | `bool` | 是否显示百分比文本或状态图标区域。 |
| `ProgressTextFormat` | `string` | 百分比文本格式，默认 `{0:0}%`。 |
| `StrokeBrush` | `IBrush?` | 已完成段画刷。 |
| `TrailColor` | `Color?` | 剩余轨道颜色，设置后覆盖默认 `GrooveBrush`。 |
| `StrokeLineCap` | `PenLineCap` | 已完成段端点形态，默认 `Round`。 |
| `SizeType` | `SizeType` | 大中小规格，默认 `Large`。 |
| `Status` | `ProgressStatus` | 普通、成功、异常、活动状态。 |
| `IndicatorThickness` | `double` | 显式指示器厚度；`NaN` 时由尺寸规格计算。 |
| `SuccessThreshold` | `double` | 成功段阈值；`NaN` 时不绘制成功段。 |
| `SuccessStrokeBrush` | `IBrush?` | 成功段画刷。 |
| `IsMotionEnabled` | `bool` | 是否启用进度值和颜色 transition。 |

## State Flow

基础进度状态流：

```text
Minimum / Maximum / Value
      ↓
CalculateProgressRatio(value)
      ↓
Percentage + Indicator geometry
      ↓
render groove / progress / success segment + extra info
```

状态语义：

- `Percentage` 始终以 `Minimum` 为基准，不直接使用 `Value / Maximum`。
- `Value == Maximum` 时进入 completed 状态，设置 `IsCompleted` 和 `:completed`。
- `Status=Success` 使用成功色和成功图标；`Status=Exception` 使用错误色和异常图标。
- `Status=Normal` 和 `Status=Active` 在完成前使用默认进度色，完成后使用成功色。
- `IsIndeterminate` 只表达不确定进度伪类，不改变 `Value`、`Percentage` 或 RangeBase 值。
- `IsProgressInfoVisible=false` 隐藏文本和状态图标区域，但不影响轨道和进度绘制。
- disabled 状态使用禁用轨道、禁用进度色和禁用文本色。

尺寸状态：

- 线形进度默认根据 `SizeType` 映射到大、中、小三档厚度。
- `IndicatorThickness` 有值时覆盖线形和圆形的计算厚度。
- 圆形进度默认尺寸由 `SizeType` 映射为 `120 / 90 / 60`，显式 `Width` / `Height` 会参与最终尺寸。
- `StepsProgressBar` 使用 `Steps`、chunk 尺寸和固定 chunk 间距计算总尺寸。
- `StepsProgressBar.ChunkHeight` 与 `IndicatorThickness` 保持同步，是现有兼容契约的一部分。

百分比文本：

- `ProgressTextFormat` 格式化 `Percentage`。
- `ProgressBar.PercentPosition.IsInner=true` 时，文本布局跟随已完成进度区域。
- 垂直内嵌文本通过 `LayoutTransformControl` 旋转。
- 非内嵌完成态显示状态图标时，百分比文本隐藏。
- 圆形进度根据可用内圆区域决定百分比文本或状态图标是否可见。

## Theme and Token Boundaries

ProgressBar 的默认视觉由抽象主题和具体控件主题组合：

| 主题 | 职责 |
| --- | --- |
| `AbstractProgressBarTheme.axaml` | 共享模板入口、默认对齐、动效、状态色、completed / disabled 状态。 |
| `AbstractLineProgressTheme.axaml` | 线形模板、百分比 label、状态图标、线形尺寸和图标尺寸。 |
| `ProgressBarTheme.axaml` | 普通线形控件主题、内嵌百分比伪类、垂直文本旋转、内嵌图标隐藏规则。 |
| `StepsProgressBarTheme.axaml` | 步骤线形控件主题、方向对齐和百分比位置对应图标对齐。 |
| `AbstractCircleProgressTheme.axaml` | 圆形模板、居中文本和居中状态图标。 |
| `CircleProgressTheme.axaml` | 圆形进度具体主题入口。 |
| `DashboardProgressTheme.axaml` | 仪表盘进度具体主题入口。 |

视觉绘制由控件直接渲染完成：

- 线形进度绘制 groove、已完成段和可选成功段。
- 步骤进度绘制固定数量 chunk，再按 `Percentage` 绘制已完成 chunk。
- 圆形进度绘制完整圆或分段圆弧。
- 仪表盘进度绘制带缺口的圆弧或分段圆弧。
- 状态图标和百分比文本通过模板 part 布局，不参与进度几何绘制。

ProgressBarToken 提供默认进度色、剩余轨道色、圆形文字和图标最小尺寸、线形文本间距、线形内部 padding 和线形图标尺寸。状态色主要来自 SharedToken。

Token 边界：

ProgressBarToken 是 ProgressBar 家族的控件级 Token scope。它定义默认进度色、剩余轨道色、圆形文本和图标最小尺寸、线形状态图标尺寸、线形额外信息间距和线形内部 padding。

ProgressBarToken 不承载以下状态：

- `Value`、`Minimum`、`Maximum`、`Percentage` 等实例进度值。
- `Status`、`IsIndeterminate`、completed、disabled 等运行时状态。
- `PercentPosition`、`Orientation`、`StepCount`、`Steps`、`DashboardGapPosition` 等布局或形态状态。
- `StrokeBrush`、`SuccessStrokeBrush`、`TrailColor` 等实例画刷覆盖。
- `IndicatorThickness`、`ChunkWidth`、`ChunkHeight`、显式 `Width` / `Height` 等实例尺寸覆盖。

这些状态分别由 public API、internal effective state、主题 selector 和直接绘制逻辑处理。

## Customization Boundaries

维护 ProgressBar 时必须保持以下不变量：

- 不修改 `ProgressBar`、`StepsProgressBar`、`CircleProgress`、`DashboardProgress` 的 public API、默认值、Template Part、伪类和 Token 名称。
- `Value`、`Minimum`、`Maximum` 必须继续遵循 `RangeBase` 语义。
- `Percentage` 和所有绘制位置必须以 `(value - Minimum) / (Maximum - Minimum)` 为基准。
- `IsIndeterminate` 不得写入或重置 `Value`。
- `Status=Exception`、`Status=Success` 和 completed 状态的图标显示规则不擅自改变。
- `SuccessThreshold=NaN` 时不绘制成功段；有值时按 `[Minimum, Maximum]` 裁剪。
- `ProgressTextFormat` 只影响文本展示，不参与真实值计算。
- `PercentPosition` 只影响百分比文本和图标位置，不改变进度值。
- `StepsProgressBar` 的 chunk 绘制数量必须由 `Percentage` 推导，不直接按 `Value` 推导。
- `CircleProgress` 和 `DashboardProgress` 的角度必须继续由共享进度比例推导。
- `IsMotionEnabled=false` 必须禁用默认 transition。
- 重新套用模板后必须重新获取文本和图标 template part，并按当前状态刷新进度。
- Token 名称和语义不擅自重命名、删除或迁移为实例属性。

如果实现某项能力时无法保持这些不变量，应先停止实现，说明原因、影响范围、替代方案和迁移方式，并获得授权。

维护不变量：

内部重构必须保持以下不变量：

- `AbstractProgressBar` 是进度比例、状态伪类、completed 状态和共享 template part 的 owner。
- 所有 value-to-geometry 算法必须使用 `CalculateProgressRatio` 或 `Percentage`。
- 线形和圆形成功阈值必须与普通进度使用同一个 `Minimum` / `Maximum` 坐标系。
- `IsIndeterminate` 只更新伪类，不改变真实进度值。
- `IsProgressInfoVisible=false` 不影响进度绘制。
- `Status=Exception` 非内嵌线形场景显示异常图标，不格式化百分比文本。
- completed 状态的文本隐藏、图标显示和成功色 selector 不擅自改动。
- 线形方向伪类和内嵌 label 伪类必须跟随属性变化同步。
- 圆形控件显式 `Width` / `Height` 和 stretch alignment 仍参与最终圆形尺寸计算。
- `StepCount=0` 的圆形和仪表盘必须保持连续圆弧模式。
- `StepsProgressBar.Steps` 最小值为 1，chunk 尺寸最小值为 1。
- 新增 binding、event handler 或 resource host 时必须定义释放位置。
