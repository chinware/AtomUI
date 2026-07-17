# ProgressBar

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## 概述

ProgressBar 是 AtomUI 桌面反馈体系中的进度展示控件，用于表达任务完成比例、状态结果、成功阈值、分段进度和环形进度。它继承 Avalonia `RangeBase` 的 `Minimum`、`Maximum` 和 `Value` 语义，并扩展 `Status`、`SizeType`、成功段、文字格式、线帽、方向、分段、圆形和仪表盘模型。

ProgressBar 的职责是展示已经由外部业务计算好的进度值。它不负责启动任务、取消任务、调度异步流程、估算剩余时间、处理下载队列或持有业务任务状态。需要任务生命周期、错误重试或进度聚合时，应由业务 ViewModel 或服务层维护，并把当前值写入 ProgressBar。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar` |
| 状态 | Stable |

## 何时使用

ProgressBar 的设计语言来自轨道、已完成段、状态色、状态图标和百分比文本的组合。

| 维度 | 含义 | 典型表达 |
| --- | --- | --- |
| 进度范围 | 在有限区间中展示完成比例。 | `Minimum`、`Maximum`、`Value`、`Percentage`。 |
| 视觉形态 | 用不同几何表达进度。 | `ProgressBar`、`StepsProgressBar`、`CircleProgress`、`DashboardProgress`。 |
| 进度方向 | 线形和步骤进度支持水平或垂直方向。 | `Orientation`、`:horizontal`、`:vertical`。 |
| 状态结果 | 表达普通、活动、成功、异常状态。 | `Status`、`:completed`、状态图标。 |
| 成功阈值 | 在整体进度中标记成功段。 | `SuccessThreshold`、`SuccessStrokeBrush`。 |
| 信息展示 | 显示百分比或自定义格式文本。 | `IsProgressInfoVisible`、`ProgressTextFormat`、`PercentPosition`。 |
| 尺寸语义 | 根据大中小规格或显式厚度计算视觉尺寸。 | `SizeType`、`IndicatorThickness`、`ChunkWidth`、`ChunkHeight`。 |
| 动效 | 进度值和颜色变化可以带 transition。 | `IsMotionEnabled`。 |

ProgressBar 的主视觉必须是进度本身。百分比文本、状态图标和成功阈值是辅助反馈，不应改变 `Value` 的业务含义。

## 公共 API

ProgressBar 家族由一个共享基类模型和四个具体控件组成。

| 控件 | 基类 | 语义 |
| --- | --- | --- |
| `ProgressBar` | `AbstractGeneralProgressBar` | 普通线形进度条，支持水平、垂直、内外百分比文本和成功段。 |
| `StepsProgressBar` | `AbstractGeneralStepsProgressBar` | 线形分段进度条，以固定 chunk 表达离散进度。 |
| `CircleProgress` | `AbstractGeneralCircleProgress` | 圆形进度条，支持普通圆弧和圆形分段。 |
| `DashboardProgress` | `AbstractGeneralDashboardProgress` | 仪表盘进度条，支持缺口方向、缺口角度和分段。 |

共享 RangeBase API：

| API | 类型 | 语义 |
| --- | --- | --- |
| `Minimum` | `double` | 进度范围最小值。 |
| `Maximum` | `double` | 进度范围最大值。 |
| `Value` | `double` | 当前进度值。 |
| `Percentage` | `double` | 只读百分比，按 `(Value - Minimum) / (Maximum - Minimum) * 100` 计算。 |

共享 Progress API：

| API | 类型 | 语义 |
| --- | --- | --- |
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
| `ExceptionCompletedIcon` | `PathIcon?` | 异常状态图标。 |
| `SuccessCompletedIcon` | `PathIcon?` | 成功状态图标。 |

线形扩展 API：

| API | 控件 | 类型 | 语义 |
| --- | --- | --- | --- |
| `Orientation` | `ProgressBar`、`StepsProgressBar` | `Orientation` | 水平或垂直布局。 |
| `PercentPosition` | `ProgressBar` | `PercentPosition` | 百分比文本在进度条内外及起点、中间、终点的定位。 |
| `PercentPosition` | `StepsProgressBar` | `LinePercentAlignment` | 步骤进度百分比文本的起点、中间、终点定位。 |
| `Steps` | `StepsProgressBar` | `int` | 步骤数量，最小为 1。 |
| `StepsStrokeBrush` | `StepsProgressBar` | `List<IBrush>?` | 每个已完成步骤的自定义画刷。 |
| `ChunkWidth` | `StepsProgressBar` | `double` | 步骤块宽度，`NaN` 时由 `SizeType` 计算。 |
| `ChunkHeight` | `StepsProgressBar` | `double` | 步骤块高度，`NaN` 时由 `IndicatorThickness` / `StrokeThickness` 计算。 |

圆形和仪表盘扩展 API：

| API | 控件 | 类型 | 语义 |
| --- | --- | --- | --- |
| `StepCount` | `CircleProgress`、`DashboardProgress` | `int` | 圆弧分段数量，0 表示普通连续圆弧。 |
| `StepGap` | `CircleProgress`、`DashboardProgress` | `double` | 分段间隔角度，默认 2。 |
| `DashboardGapPosition` | `DashboardProgress` | `DashboardGapPosition` | 仪表盘缺口方向，默认 `Bottom`。 |
| `GapDegree` | `DashboardProgress` | `double` | 仪表盘缺口角度，范围 `[0, 295]`，默认 75。 |

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_LayoutTransformControl` | `LayoutTransformControl` | 承载百分比文本并在垂直内嵌模式中旋转。 |
| `PART_PercentageLabel` | `Label` | 显示格式化后的进度文本。 |
| `PART_ExceptionCompletedIconPresenter` | `IconPresenter` | 显示异常状态图标。 |
| `PART_SuccessCompletedIconPresenter` | `IconPresenter` | 显示成功状态图标。 |

稳定伪类：

| 伪类 | 语义 |
| --- | --- |
| `:indeterminate` | `IsIndeterminate=true`。 |
| `:completed` | `Value` 等于 `Maximum`。 |
| `:horizontal` | 线形进度为水平布局。 |
| `:vertical` | 线形进度为垂直布局。 |
| `:labelinner` | `ProgressBar.PercentPosition.IsInner=true`。 |
| `:labelinner-start` | 内嵌百分比位于起点。 |
| `:labelinner-center` | 内嵌百分比位于中间。 |
| `:labelinner-end` | 内嵌百分比位于终点。 |

ProgressBar 当前不实现 `ICustomizeSizeTypeAware`。自定义尺寸通过 `IndicatorThickness`、`Width`、`Height`、`ChunkWidth` 和 `ChunkHeight` 等现有属性表达。

## 事件与命令

ProgressBar 的事件与命令以公共 API、Avalonia 基类契约和 Gallery API 表为准；生成器不从源码发明额外事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 进度条

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarShowCase.axaml:46`

Gallery key：`ExamplesContent` / item `0`

```axaml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ProgressBar Value="30" Minimum="0" Maximum="100" />
    <atom:ProgressBar Value="50" Minimum="0" Maximum="100" />
    <atom:ProgressBar Value="70" Minimum="0" Maximum="100" Status="Exception" />
    <atom:ProgressBar Value="100" Minimum="0" Maximum="100" />
    <atom:ProgressBar Value="50" Minimum="0" Maximum="100" IsProgressInfoVisible="False" />
</StackPanel>
```

### 环形进度条

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarShowCase.axaml:64`

Gallery key：`ExamplesContent` / item `1`

```axaml
<WrapPanel Orientation="Horizontal">
    <atom:CircleProgress Value="75" Minimum="0" Maximum="100" />
    <atom:CircleProgress Value="70" Minimum="0" Maximum="100" Status="Exception" />
    <atom:CircleProgress Value="100" Minimum="0" Maximum="100" />
</WrapPanel>
```

### 迷你尺寸进度条

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarShowCase.axaml:80`

Gallery key：`ExamplesContent` / item `2`

```axaml
<WrapPanel Orientation="Horizontal" Width="180" HorizontalAlignment="Left">
    <atom:ProgressBar Value="30" Minimum="0" Maximum="100" SizeType="Middle" />
    <atom:ProgressBar Value="50" Minimum="0" Maximum="100" SizeType="Middle" />
    <atom:ProgressBar Value="70" Minimum="0" Maximum="100" Status="Exception" SizeType="Middle" />
    <atom:ProgressBar Value="100" Minimum="0" Maximum="100" SizeType="Middle" />
    <atom:ProgressBar Value="50" Minimum="0" Maximum="100" IsProgressInfoVisible="False" SizeType="Middle" />
</WrapPanel>
```

### 更小的环形进度条

来源：`controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarShowCase.axaml:98`

Gallery key：`ExamplesContent` / item `3`

```axaml
<WrapPanel Orientation="Horizontal">
    <atom:CircleProgress Value="75" Minimum="0" Maximum="100" SizeType="Middle" />
    <atom:CircleProgress Value="70" Minimum="0" Maximum="100" Status="Exception" SizeType="Middle" />
    <atom:CircleProgress Value="100" Minimum="0" Maximum="100" SizeType="Middle" />
</WrapPanel>
```

## 状态模型

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

## 主题与 Design Token

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

Token 来源：

ProgressBarToken 是 ProgressBar 家族的组件级 Token scope。它定义默认进度色、剩余轨道色、圆形文本和图标最小尺寸、线形状态图标尺寸、线形额外信息间距和线形内部 padding。

ProgressBarToken 不承载以下状态：

- `Value`、`Minimum`、`Maximum`、`Percentage` 等实例进度值。
- `Status`、`IsIndeterminate`、completed、disabled 等运行时状态。
- `PercentPosition`、`Orientation`、`StepCount`、`Steps`、`DashboardGapPosition` 等布局或形态状态。
- `StrokeBrush`、`SuccessStrokeBrush`、`TrailColor` 等实例画刷覆盖。
- `IndicatorThickness`、`ChunkWidth`、`ChunkHeight`、显式 `Width` / `Height` 等实例尺寸覆盖。

这些状态分别由 public API、internal effective state、主题 selector 和直接绘制逻辑处理。

## AOT 与裁剪注意事项

ProgressBar 不依赖运行时反射、字符串 binding、动态类型扫描或 C# relay binding。模板状态主要通过 template part、`TemplateBinding`、style selector 和 direct render 完成。

资源边界：

- 默认颜色来自 `ProgressBarToken` 和 SharedToken。
- `TrailColor` 会写入 `GrooveBrush` local value；清空后恢复主题资源。
- 状态图标由 `IconPresenter` 承载，默认图标在模板应用后以 template priority 设置。

性能边界：

- 线形、步骤、圆形和仪表盘均使用直接绘制，避免创建大量视觉子节点。
- 圆形和仪表盘使用 cached pen helper 更新 pen。
- 文本尺寸通过 `_extraInfoSize` 缓存，尺寸、状态或文本相关属性变化时刷新。
- 动效只绑定 `Value`、`StrokeBrush` 和 `Foreground` transition。

AOT 边界：

- 不新增反射读取 public API、token 或 template part。
- 新增 Token 必须走 `ProgressBarToken` 和 generator 支持的 token kind。
- Gallery API / Token 表应显式维护，不依赖运行时扫描。

## 源码索引

共享实现位于 `src/AtomUI.Controls/ProgressBar/`：

- `ProgressBarEnums.cs`：`ProgressStatus`、`DashboardGapPosition`、`PercentPosition`、`LinePercentAlignment`。
- `ProgressBarPseudoClass.cs`：ProgressBar 家族使用的伪类常量。
- `AbstractProgressBar.cs`：RangeBase 集成、共享 public API、百分比计算、状态伪类、模板 part、动效入口和直接渲染调度。
- `AbstractLineProgress.cs`：线形方向、线形 extra info 测量、方向伪类、线形默认状态图标。
- `AbstractGeneralProgressBar.cs`：普通线形进度的测量、布局、成功段、内外百分比位置和百分比文字颜色。
- `AbstractGeneralStepsProgressBar.cs`：线形步骤进度的 chunk 尺寸、步骤布局、步骤绘制和百分比文本布局。
- `AbstractCircleProgress.cs`：圆形尺寸、圆形文本和状态图标布局、圆形 thickness 计算。
- `AbstractGeneralCircleProgress.cs`：圆形 groove、连续圆弧、分段圆弧和成功弧段绘制。
- `AbstractGeneralDashboardProgress.cs`：仪表盘缺口角度、连续仪表盘弧、分段仪表盘弧和成功弧段绘制。

桌面具体控件位于 `src/AtomUI.Desktop.Controls/ProgressBar/`：

- `ProgressBar.cs`：注册 `ProgressBarToken` scope 的普通线形控件入口。
- `StepsProgressBar.cs`：注册 token scope 的步骤线形控件入口。
- `CircleProgress.cs`：注册 token scope 的圆形控件入口。
- `DashboardProgress.cs`：注册 token scope 的仪表盘控件入口。
- `ProgressBarToken.cs`：组件级 Token 默认值计算。
- `Themes/*.axaml`：共享主题、线形主题、圆形主题和具体控件主题。

Gallery 示例和 API / Token 表位于 `controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/`。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/progress-bar/overview.md`
- 实现文档：`docs/controls/desktop/feedback/progress-bar/implementation.md`
- Token 文档：`docs/controls/desktop/feedback/progress-bar/token.md`
- 变更记录：`docs/controls/desktop/feedback/progress-bar/changelog.md`
- 语义结构：`./semantic-cn.md`
