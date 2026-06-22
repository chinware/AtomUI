# ProgressBar 桌面版实现原理

本文档描述 ProgressBar 家族桌面版的内部基类分层、进度归一化、线形和圆形渲染、百分比文本布局、状态图标、Token 边界和维护规则。公共设计与 API 契约见 [ProgressBar 桌面版架构设计](overview.md)，Token 语义见 [ProgressBar Token 设计](token.md)，变化记录见 [ProgressBar Changelog](changelog.md)。

## 1. 实现定位

ProgressBar 的实现基于 Avalonia `RangeBase`。AtomUI 负责把 `Minimum`、`Maximum`、`Value` 转换为百分比、线形尺寸、圆形角度、步骤数量、状态图标和直接绘制结果。

本文档覆盖 `AtomUI.Controls` 中的抽象基类和 `AtomUI.Desktop.Controls` 中的四个具体控件。具体绘制语句、token 默认值和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构、状态流和不变量。

## 2. 源码文件结构

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

## 3. 核心类职责

`AbstractProgressBar` 是状态 owner。它持有共享 public API、`Percentage` direct property、`EffectiveSizeType`、`StrokeThickness`、状态图标 template part 和 `Render` 调度。所有值到比例的计算都应经过 `CalculateProgressRatio`。

`AbstractLineProgress` 负责线形进度的方向和文本测量基础。它同步 `:horizontal` / `:vertical`，并根据宽高变化重新计算 effective size type。

`AbstractGeneralProgressBar` 负责普通线形进度条。它根据 orientation 和 `PercentPosition` 计算进度条 rect、额外信息 rect、成功段 rect，并在内嵌文本模式下选择可读文本颜色。

`AbstractGeneralStepsProgressBar` 负责步骤线形进度。它以 `Steps`、`ChunkWidth`、`ChunkHeight` 和固定间距计算总尺寸，并按已完成步骤数绘制 chunk。

`AbstractCircleProgress` 负责圆形尺寸与中心信息布局。它根据 `SizeType`、`Width`、`Height` 和 `IndicatorThickness` 计算圆形实际尺寸和 stroke thickness，并设置圆形内部文本 / 图标大小。

`AbstractGeneralCircleProgress` 和 `AbstractGeneralDashboardProgress` 负责圆弧绘制。它们把进度比例转换为角度，并根据 `StepCount` / `StepGap` 决定连续绘制或分段绘制。

四个桌面 public 控件只注册 token scope，不持有额外状态。

## 4. 状态与数据流

共享数据流：

```text
RangeBase.Minimum / Maximum / Value
      ↓
AbstractProgressBar.CalculateProgressRatio(value)
      ↓
Percentage
      ↓
NotifyUpdateProgress()
      ↓
text / angle / step count / render invalidation
```

`CalculateProgressRatio` 是进度归一化的唯一共享入口。维护线形长度、成功阈值、圆形角度、仪表盘角度和步骤数量时，必须使用该比例或已由该比例计算出的 `Percentage`。

状态流：

```text
Value change
  ↓
UpdateProgress()
  ↓
Percentage + NotifyUpdateProgress()
  ↓
IsCompleted + pseudo classes
```

`IsCompleted` 是 internal visual state，按 `Value == Maximum` 更新。它驱动 completed 主题 selector、状态图标显示和测量刷新。

`TrailColor` 是 public 覆盖入口。设置后写入 internal `GrooveBrush`；清空后释放该 local value，让主题资源重新生效。

`SizeType` 写入 `EffectiveSizeType`。线形控件还会根据显式宽高在模板应用时重新计算 effective size type。圆形控件则用 `SizeType` 推导默认圆形尺寸。

## 5. 生命周期与模板接入

`AbstractProgressBar.OnApplyTemplate` 获取共享 template part：

- `PART_LayoutTransformControl`
- `PART_PercentageLabel`
- `PART_ExceptionCompletedIconPresenter`
- `PART_SuccessCompletedIconPresenter`

获取 part 后立即调用 `NotifyEffectSizeTypeChanged()` 和 `UpdateProgress()`，保证模板创建后尺寸、文本和状态同步。

`AbstractLineProgress.OnApplyTemplate` 在共享模板接入后：

- 根据显式 `Height` 或 `Width` 计算线形 effective size type。
- 计算 `_extraInfoSize`。
- 同步方向伪类。
- 为线形状态图标设置默认 `CloseCircleFilled` 和 `CheckCircleFilled`。

`AbstractCircleProgress.OnApplyTemplate` 为圆形状态图标设置默认 `CloseOutlined` 和 `CheckOutlined`。

`OnInitialized` 会先禁用 transition，`OnLoaded` 再通过 dispatcher 启用 transition，避免初次加载时出现不必要的进度动画。

ProgressBar 家族当前没有事件订阅、C# relay binding、popup、async 或外部资源 host 生命周期。模板 part 重新获取后不需要显式解绑旧事件，但新增订阅或 binding 时必须成对释放。

## 6. 交互与事件处理

ProgressBar 是展示型控件，没有 pointer、keyboard、focus 编辑、选择、拖拽、popup 或集合事件模型。

可观察行为来自属性变化：

- `Value`、`Minimum`、`Maximum`、`ProgressTextFormat` 改变时刷新 `Percentage` 和文本。
- `Value` 改变时同步 completed 伪类和 internal completed 状态。
- `IsIndeterminate` 改变时同步 `:indeterminate`。
- `Width` / `Height` 改变时触发额外信息可见性和圆形尺寸刷新。
- `PercentPosition` 改变时同步内嵌文本伪类。
- `Orientation` 改变时同步方向伪类并重新计算最小厚度。

## 7. 内部算法与关键流程

### 7.1 进度归一化

所有进度几何都从同一公式出发：

```text
ratio = (value - Minimum) / (Maximum - Minimum)
Percentage = ratio * 100
```

当 `Maximum` 和 `Minimum` 几乎相等时，比例退化为 1.0。成功阈值绘制前会裁剪到 `[Minimum, Maximum]`，再进入同一比例计算。

### 7.2 线形进度绘制

`AbstractGeneralProgressBar.RenderGroove` 先计算 `_grooveRect`。`RenderIndicatorBar` 根据 `_grooveRect` 的宽度或高度计算 deflate 值，然后调用 `DrawIndicatorBar` 绘制已完成段。

当 `SuccessThreshold` 有效时，线形控件再次按成功阈值计算 deflate 值并使用 `SuccessStrokeBrush` 覆盖绘制成功段。

`StrokeLineCap=Round` 时使用胶囊形绘制；其他线帽使用矩形填充。绘制前会判断 indicator rect 是否太小，避免绘制不可见的小块。

### 7.3 线形百分比布局

普通线形控件使用 `GetProgressBarRect` 和 `GetExtraInfoRect` 配合计算进度条区域和文本区域。

外部百分比模式会为 label 或状态图标预留起点、中间或终点空间。内嵌模式会先计算当前 indicator rect，再把文本放在已完成进度区域内。垂直内嵌模式会交换文本尺寸，并通过主题旋转文本。

内嵌文本颜色由当前背景色可读性决定。当前实现只对 solid brush 进行可读性计算；渐变画刷不会参与该颜色推导。

### 7.4 步骤线形绘制

`AbstractGeneralStepsProgressBar` 使用固定 `DEFAULT_CHUNK_SPACE` 作为步骤间距。`RenderGroove` 绘制全部 chunk 的 groove，`RenderIndicatorBar` 按 `Math.Round(Steps * Percentage / 100)` 绘制已完成 chunk。

`StepsStrokeBrush` 可以为已完成 chunk 提供逐项画刷。索引超出提供数量时回退到 `StrokeBrush`。

`ChunkWidth` 缺省时由 `EffectiveSizeType` 推导；`ChunkHeight` 缺省时使用 `StrokeThickness`。`ChunkHeight` 和 `IndicatorThickness` 双向同步是当前兼容行为，维护时不得用 suppression flag 打断该状态流。

### 7.5 圆形进度绘制

圆形进度在 `RenderGroove` 中计算当前圆形 rect，并向内 deflate 半个 stroke thickness。普通模式绘制完整椭圆；分段模式按 `(360 - StepGap * StepCount) / StepCount` 计算每段角度。

`NotifyUpdateProgress` 把当前 `Value` 转换成 `IndicatorAngle`。普通 indicator 直接绘制从 -90 度开始的 arc；分段 indicator 按已完成段数量绘制。

成功阈值在普通圆弧中绘制成功弧段，在分段模式中计算成功段数量并切换成功 pen。

### 7.6 仪表盘绘制

仪表盘使用 `DashboardGapPosition` 和 `GapDegree` 计算起始角和总跨度：

- Bottom：从底部缺口两侧开始。
- Left / Top / Right：按对应方向旋转缺口。
- 总跨度为 `360 - GapDegree`。

普通模式绘制一个带缺口的 arc。分段模式按 `(360 - GapDegree - StepGap * StepCount) / StepCount` 计算每段角度。

`GapDegree` 改变和 `DashboardGapPosition` 改变时重新计算角度 pair。`Value` 改变时只更新 `IndicatorAngle`。

## 8. 资源、性能与 AOT 边界

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

## 9. 维护不变量

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

## 10. 测试与验证

现有验证入口：

- `tests/AtomUI.Desktop.Controls.Tests/ProgressBar/ProgressBarBehaviorTests.cs`
- `tests/AtomUIGallery.Tests/ShowCases/ProgressBarShowCasePageTests.cs`
- `tests/AtomUIGallery.Tests/ShowCases/ProgressBarShowCaseExamples.snapshot`

重点验证场景：

- `Minimum != 0` 时 `Percentage`、线形成功段、内嵌百分比位置、圆形角度和仪表盘角度。
- `IsIndeterminate` 运行期切换后的 `:indeterminate` 伪类。
- `Status=Normal/Active/Success/Exception` 与 completed 状态的图标和颜色。
- `IsProgressInfoVisible=false` 下文本和图标隐藏但进度仍绘制。
- `Orientation=Horizontal/Vertical` 下线形和步骤布局。
- `PercentPosition` 内外、起点、中间、终点布局。
- `Steps`、`StepCount`、`StepGap`、`ChunkWidth`、`ChunkHeight`。
- `DashboardGapPosition` 和 `GapDegree`。
- `StrokeLineCap`、渐变 `StrokeBrush`、`TrailColor`、`SuccessStrokeBrush`。
- `SizeType`、显式 `IndicatorThickness`、显式圆形宽高。
- disabled 状态和 `IsMotionEnabled=false`。

文档改动至少运行 `git diff --check`，并检查 `overview.md`、`implementation.md`、`token.md`、`changelog.md` 之间的相对链接存在。
