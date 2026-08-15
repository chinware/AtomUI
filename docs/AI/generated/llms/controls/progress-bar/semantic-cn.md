# ProgressBar 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

四个 public 控件各自拥有独立 descriptor。Part 名称与上游 Progress Semantic DOM 保持一致：
`root`、`body`、`rail`、`track`、`indicator`。官方 `steps` 模式明确不存在 rail，因此 `StepsProgressBar` 只声明
`root`、`body`、`track`、`indicator`；不得为统一表面虚构一个始终不存在的 rail。其余同名 Part 表达一致的产品职责，
不表示四个 owner 共享运行时节点、几何或状态。

### 1.1 `ProgressBar`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `ProgressBar` |
| Part | `root` |
| Selector | ProgressBar 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `ProgressBar` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 普通线形进度 owner |
| 职责 | 承载进度范围、状态、尺寸、方向、文本位置和 Semantic Style 作用域。 |
| 相关 API | 全部 `ProgressBar` public API |
| 相关 Token | ProgressBarToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `body`

| 字段 | 值 |
| --- | --- |
| Owner | `ProgressBar` |
| Part | `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `/template/ .semantic-body` |
| Style Type | `ProgressBarBodyStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 线形进度主体布局 |
| 职责 | 统一承载 rail、track、成功段和 indicator 的布局边界。 |
| 相关 API | `Orientation`、`PercentPosition`、`IsProgressInfoVisible` |
| 相关 Token | `LineExtraInfoMargin`、`LineProgressPadding` |
| 稳定性 | stable since 6.0 |

#### `rail`

| 字段 | 值 |
| --- | --- |
| Owner | `ProgressBar` |
| Part | `rail` |
| Selector | `.semantic-rail` |
| SelectorRoute | `/template/ .semantic-body > .semantic-rail` |
| Style Type | `ProgressBarRailStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 未完成轨道盒视觉 |
| 职责 | 表达线形进度的完整剩余轨道。 |
| 相关 API | `TrailColor`、`StrokeLineCap`、`IndicatorThickness` |
| 相关 Token | `RemainingColor` |
| 稳定性 | stable since 6.0 |

#### `track`

| 字段 | 值 |
| --- | --- |
| Owner | `ProgressBar` |
| Part | `track` |
| Selector | `.semantic-track` |
| SelectorRoute | `/template/ .semantic-body > .semantic-track` |
| Style Type | `ProgressBarTrackStyle` |
| ContractType | `Border` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 当前进度盒视觉 |
| 职责 | 表达由 `Value` 计算出的线形已完成区域。 |
| 相关 API | `Value`、`StrokeBrush`、`StrokeLineCap`、`IndicatorThickness` |
| 相关 Token | `DefaultColor`、SharedToken 状态色 |
| 稳定性 | stable since 6.0 |

#### `indicator`

| 字段 | 值 |
| --- | --- |
| Owner | `ProgressBar` |
| Part | `indicator` |
| Selector | `.semantic-indicator` |
| SelectorRoute | `/template/ .semantic-body > .semantic-indicator` |
| Style Type | `ProgressBarIndicatorStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 百分比或状态信息区域 |
| 职责 | 统一承载格式化百分比、成功图标和异常图标的替代呈现。 |
| 相关 API | `IsProgressInfoVisible`、`ProgressTextFormat`、`PercentPosition`、`Status`、完成图标 API |
| 相关 Token | 文本、图标尺寸和状态色 Token |
| 稳定性 | stable since 6.0 |

### 1.2 `StepsProgressBar`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `StepsProgressBar` |
| Part | `root` |
| Selector | StepsProgressBar 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `StepsProgressBar` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 步骤线形进度 owner |
| 职责 | 承载进度范围、步骤数量、逐步画刷、尺寸、方向和 Semantic Style 作用域。 |
| 相关 API | 全部 `StepsProgressBar` public API |
| 相关 Token | ProgressBarToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `body`

| 字段 | 值 |
| --- | --- |
| Owner | `StepsProgressBar` |
| Part | `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `/template/ .semantic-body` |
| Style Type | `StepsProgressBarBodyStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 步骤进度主体布局 |
| 职责 | 统一承载步骤 track 和 indicator；steps 不创建 rail。 |
| 相关 API | `Orientation`、`Steps`、`PercentPosition`、`IsProgressInfoVisible` |
| 相关 Token | `LineExtraInfoMargin` |
| 稳定性 | stable since 6.0 |

#### `track`

| 字段 | 值 |
| --- | --- |
| Owner | `StepsProgressBar` |
| Part | `track` |
| Selector | `.semantic-track` |
| SelectorRoute | `/template/ .semantic-body > .semantic-track` |
| Style Type | `StepsProgressBarTrackStyle` |
| ContractType | `Rectangle` |
| Cardinality | `Multiple` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `true` |
| AtomUI 节点 | 全部步骤块 |
| 职责 | 表达全部步骤单元；完成状态只决定每个 target 使用进度色还是 rail 色。 |
| 相关 API | `Value`、`Steps`、`StepsStrokeBrush`、`StrokeBrush`、`TrailColor` |
| 相关 Token | `DefaultColor`、`RemainingColor`、SharedToken 状态色 |
| 稳定性 | stable since 6.0 |

#### `indicator`

| 字段 | 值 |
| --- | --- |
| Owner | `StepsProgressBar` |
| Part | `indicator` |
| Selector | `.semantic-indicator` |
| SelectorRoute | `/template/ .semantic-body > .semantic-indicator` |
| Style Type | `StepsProgressBarIndicatorStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 百分比或状态信息区域 |
| 职责 | 统一承载步骤进度的百分比、成功图标和异常图标。 |
| 相关 API | `IsProgressInfoVisible`、`ProgressTextFormat`、`PercentPosition`、`Status` |
| 相关 Token | 文本、图标尺寸和状态色 Token |
| 稳定性 | stable since 6.0 |

### 1.3 `CircleProgress`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `CircleProgress` |
| Part | `root` |
| Selector | CircleProgress 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `CircleProgress` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 圆形进度 owner |
| 职责 | 承载进度范围、圆形尺寸、分段状态和 Semantic Style 作用域。 |
| 相关 API | 全部 `CircleProgress` public API |
| 相关 Token | ProgressBarToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `body`

| 字段 | 值 |
| --- | --- |
| Owner | `CircleProgress` |
| Part | `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `/template/ .semantic-body` |
| Style Type | `CircleProgressBodyStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 圆形进度主体布局 |
| 职责 | 统一承载圆形 rail、track、成功弧段和居中 indicator。 |
| 相关 API | `SizeType`、`Width`、`Height`、`StepCount`、`StepGap` |
| 相关 Token | 圆形尺寸与信息 Token |
| 稳定性 | stable since 6.0 |

#### `rail`

| 字段 | 值 |
| --- | --- |
| Owner | `CircleProgress` |
| Part | `rail` |
| Selector | `.semantic-rail` |
| SelectorRoute | `/template/ .semantic-body > .semantic-rail` |
| Style Type | `CircleProgressRailStyle` |
| ContractType | `Shape` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 圆形未完成轨道几何 |
| 职责 | 表达连续圆或分段圆的完整剩余轨道。 |
| 相关 API | `TrailColor`、`IndicatorThickness`、`StepCount`、`StepGap` |
| 相关 Token | `RemainingColor` |
| 稳定性 | stable since 6.0 |

#### `track`

| 字段 | 值 |
| --- | --- |
| Owner | `CircleProgress` |
| Part | `track` |
| Selector | `.semantic-track` |
| SelectorRoute | `/template/ .semantic-body > .semantic-track` |
| Style Type | `CircleProgressTrackStyle` |
| ContractType | `Shape` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 圆形当前进度几何 |
| 职责 | 表达由 `Value` 计算出的连续或分段圆弧。 |
| 相关 API | `Value`、`StrokeBrush`、`StrokeLineCap`、`IndicatorThickness`、`StepCount`、`StepGap` |
| 相关 Token | `DefaultColor`、SharedToken 状态色 |
| 稳定性 | stable since 6.0 |

#### `indicator`

| 字段 | 值 |
| --- | --- |
| Owner | `CircleProgress` |
| Part | `indicator` |
| Selector | `.semantic-indicator` |
| SelectorRoute | `/template/ .semantic-body > .semantic-indicator` |
| Style Type | `CircleProgressIndicatorStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 圆心信息区域 |
| 职责 | 统一承载圆心百分比、成功图标和异常图标。 |
| 相关 API | `IsProgressInfoVisible`、`ProgressTextFormat`、`Status`、完成图标 API |
| 相关 Token | `CircleMinimumTextFontSize`、`CircleMinimumIconSize` |
| 稳定性 | stable since 6.0 |

### 1.4 `DashboardProgress`

#### `root`

| 字段 | 值 |
| --- | --- |
| Owner | `DashboardProgress` |
| Part | `root` |
| Selector | DashboardProgress 本身 |
| SelectorRoute | 不适用 |
| Style Type | 不适用（root 不生成 Style） |
| ContractType | `DashboardProgress` |
| Cardinality | `Single` |
| Customization | `Root` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 仪表盘进度 owner |
| 职责 | 承载进度范围、缺口、尺寸、分段状态和 Semantic Style 作用域。 |
| 相关 API | 全部 `DashboardProgress` public API |
| 相关 Token | ProgressBarToken、SharedToken |
| 稳定性 | stable since 6.0 |

#### `body`

| 字段 | 值 |
| --- | --- |
| Owner | `DashboardProgress` |
| Part | `body` |
| Selector | `.semantic-body` |
| SelectorRoute | `/template/ .semantic-body` |
| Style Type | `DashboardProgressBodyStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 仪表盘进度主体布局 |
| 职责 | 统一承载仪表盘 rail、track、成功弧段和居中 indicator。 |
| 相关 API | `SizeType`、`Width`、`Height`、`DashboardGapPosition`、`GapDegree` |
| 相关 Token | 圆形尺寸与信息 Token |
| 稳定性 | stable since 6.0 |

#### `rail`

| 字段 | 值 |
| --- | --- |
| Owner | `DashboardProgress` |
| Part | `rail` |
| Selector | `.semantic-rail` |
| SelectorRoute | `/template/ .semantic-body > .semantic-rail` |
| Style Type | `DashboardProgressRailStyle` |
| ContractType | `Shape` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 仪表盘未完成轨道几何 |
| 职责 | 表达带指定缺口的连续或分段剩余轨道。 |
| 相关 API | `TrailColor`、`IndicatorThickness`、`DashboardGapPosition`、`GapDegree`、`StepCount`、`StepGap` |
| 相关 Token | `RemainingColor` |
| 稳定性 | stable since 6.0 |

#### `track`

| 字段 | 值 |
| --- | --- |
| Owner | `DashboardProgress` |
| Part | `track` |
| Selector | `.semantic-track` |
| SelectorRoute | `/template/ .semantic-body > .semantic-track` |
| Style Type | `DashboardProgressTrackStyle` |
| ContractType | `Shape` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 仪表盘当前进度几何 |
| 职责 | 表达由 `Value` 计算出的带缺口连续或分段圆弧。 |
| 相关 API | `Value`、`StrokeBrush`、`StrokeLineCap`、`DashboardGapPosition`、`GapDegree` |
| 相关 Token | `DefaultColor`、SharedToken 状态色 |
| 稳定性 | stable since 6.0 |

#### `indicator`

| 字段 | 值 |
| --- | --- |
| Owner | `DashboardProgress` |
| Part | `indicator` |
| Selector | `.semantic-indicator` |
| SelectorRoute | `/template/ .semantic-body > .semantic-indicator` |
| Style Type | `DashboardProgressIndicatorStyle` |
| ContractType | `Panel` |
| Cardinality | `Single` |
| Customization | `Selector` |
| CrossVisualRoot | `false` |
| RuntimeCreated | `false` |
| AtomUI 节点 | 仪表盘中心信息区域 |
| 职责 | 统一承载仪表盘百分比、成功图标和异常图标。 |
| 相关 API | `IsProgressInfoVisible`、`ProgressTextFormat`、`Status`、完成图标 API |
| 相关 Token | `CircleMinimumTextFontSize`、`CircleMinimumIconSize` |
| 稳定性 | stable since 6.0 |

所有 root 都是隐式 Part，不添加 `.semantic-root`。四个 owner 均不跨 VisualRoot，也不提供 Semantic Part Theme。
`ProgressBar`、`CircleProgress` 和 `DashboardProgress` 的非 root Part 是静态模板 target；`StepsProgressBar` 只有 track
按 `Steps` 创建、重用或清理 runtime Rectangle。

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/ProgressBar/Themes/ProgressBarTheme.axaml`

```xml
<LineProgressPanel Name="PART_ProgressBody">
    <Border Name="PART_ProgressRail" />
    <Border Name="PART_ProgressTrack" />
    <Border Name="PART_ProgressSuccess" />
    <Canvas Name="PART_ProgressIndicator">
        <LayoutTransformControl Name="PART_LayoutTransformControl">
            <Label Name="PART_PercentageLabel" />
        </LayoutTransformControl>
        <IconPresenter Name="PART_ExceptionCompletedIconPresenter" />
        <IconPresenter Name="PART_SuccessCompletedIconPresenter" />
    </Canvas>
</LineProgressPanel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
ProgressBar
  -> ProgressBar (control theme, ProgressBarTheme.axaml)
     -> LineProgressPanel#PART_ProgressBody (template-stable)
        -> Border#PART_ProgressRail (template-stable)
        -> Border#PART_ProgressTrack (template-stable)
        -> Border#PART_ProgressSuccess (template-stable)
        -> Canvas#PART_ProgressIndicator (template-stable)
           -> LayoutTransformControl#PART_LayoutTransformControl (template-stable)
              -> Label#PART_PercentageLabel (template-stable)
           -> IconPresenter#PART_ExceptionCompletedIconPresenter (template-stable)
           -> IconPresenter#PART_SuccessCompletedIconPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `ProgressBar` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ProgressBar` | control theme | `ProgressBarTheme.axaml` | 用户代码 / 控件宿主 | `ExceptionCompletedIcon`, `IsEnabled`, `IsProgressInfoVisible`, `PercentageLabelColor`, `SuccessCompletedIcon` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_ProgressBody` | template node (LineProgressPanel) | `ProgressBarTheme.axaml` | ProgressBar | `ExceptionCompletedIcon`, `IsEnabled`, `IsProgressInfoVisible`, `PercentageLabelColor`, `SuccessCompletedIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ProgressRail` | template node (Border) | `ProgressBarTheme.axaml` | ProgressBar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ProgressTrack` | template node (Border) | `ProgressBarTheme.axaml` | ProgressBar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ProgressSuccess` | template node (Border) | `ProgressBarTheme.axaml` | ProgressBar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ProgressIndicator` | template node (Canvas) | `ProgressBarTheme.axaml` | ProgressBar | `ExceptionCompletedIcon`, `IsEnabled`, `IsProgressInfoVisible`, `PercentageLabelColor`, `SuccessCompletedIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LayoutTransformControl` | template node (LayoutTransformControl) | `ProgressBarTheme.axaml` | ProgressBar | `IsEnabled`, `PercentageLabelColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PercentageLabel` | template node (Label) | `ProgressBarTheme.axaml` | ProgressBar | `IsEnabled`, `PercentageLabelColor` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ExceptionCompletedIconPresenter` | template node (IconPresenter) | `ProgressBarTheme.axaml` | ProgressBar | `ExceptionCompletedIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SuccessCompletedIconPresenter` | template node (IconPresenter) | `ProgressBarTheme.axaml` | ProgressBar | `SuccessCompletedIcon` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

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
| `AbstractCircleProgressTheme.axaml` | 圆形家族共享 Token、Shape stroke 和居中状态图标 selector，不拥有 concrete Template。 |
| `CircleProgressTheme.axaml` | 圆形进度 leaf Theme 和 concrete Template，拥有静态 Semantic marker。 |
| `DashboardProgressTheme.axaml` | 仪表盘进度 leaf Theme 和 concrete Template，拥有静态 Semantic marker。 |

进度视觉由模板中的真实 Semantic target 承载，owner 继续负责全部值、尺寸和角度计算：

- 普通线形 rail/track 由静态 `Border` target 表达，可选成功段保持内部 `Border` 附加层；`LineProgressPanel`
  是这些盒视觉和 indicator 的唯一布局所有者。
- 步骤进度按 `Steps` 创建 track `Rectangle` target，并根据 `Percentage` 切换每个 target 的 active/remaining Brush；steps 不公开 rail。
- 圆形进度用静态 Shape 承载完整圆或组合分段圆弧。
- 仪表盘进度用静态 Shape 承载带缺口的连续或组合分段圆弧。
- 状态图标和百分比文本统一位于 indicator Panel 内，不参与进度几何计算。

ProgressBarToken 提供默认进度色、剩余轨道色、圆形文字和图标最小尺寸、线形文本间距、线形内部 padding 和线形图标尺寸。状态色主要来自 SharedToken。

Token 边界：

ProgressBarToken 是 ProgressBar 家族的控件级 Token scope。它定义默认进度色、剩余轨道色、圆形文本和图标最小尺寸、线形状态图标尺寸、线形额外信息间距和线形内部 padding。

ProgressBarToken 不承载以下状态：

- `Value`、`Minimum`、`Maximum`、`Percentage` 等实例进度值。
- `Status`、`IsIndeterminate`、completed、disabled 等运行时状态。
- `PercentPosition`、`Orientation`、`StepCount`、`Steps`、`DashboardGapPosition` 等布局或形态状态。
- `StrokeBrush`、`SuccessStrokeBrush`、`TrailColor` 等实例画刷覆盖。
- `IndicatorThickness`、`ChunkWidth`、`ChunkHeight`、显式 `Width` / `Height` 等实例尺寸覆盖。

这些状态分别由 public API、internal effective state、主题 selector 和模板几何布局逻辑处理。

## Customization Boundaries

维护 ProgressBar 时必须保持以下不变量：

- 不修改 `ProgressBar`、`StepsProgressBar`、`CircleProgress`、`DashboardProgress` 的既有 public API、默认值、Template Part、伪类和 Token 名称。
- 不删除或重命名四个 owner 的 `root`、`body`、`rail`、`track`、`indicator`，也不改变 selector route、ContractType 或 cardinality。
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
- `ChunkWidth=NaN` 必须保持自动状态；运行期 SizeType 变化不能遗留首次 Large 计算值。
- 四个 owner 已声明的 body/rail/track/indicator marker 必须与 descriptor route、ContractType 和 cardinality 一致。
- rail/track marker 必须位于直接绘制最终视觉的可见 target：普通线形使用 `Border`，圆形和仪表盘使用 `Shape`；indicator marker 必须保持非零的实际信息 Bounds。
- 成功阈值附加视觉、label、状态 IconPresenter 和几何实现类型不得提升为额外公开 Part。
- 新增 binding、event handler 或 resource host 时必须定义释放位置。
