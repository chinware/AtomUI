# Slider 桌面版实现原理

本文档描述 Slider 桌面版的内部轨道渲染、thumb 布局、指针/键盘交互、mark 处理、tooltip 同步、Form 集成和 Token 边界。公共设计与 API 契约见 [Slider 桌面版架构设计](overview.md)，Token 语义见 [Slider Token 设计](token.md)，变化记录见 [Slider Changelog](changelog.md)。

## 1. 实现定位

Slider 的实现基于 Avalonia `RangeBase`。AtomUI 负责轨道、thumb、mark、范围模式、tooltip、主题 token、动效和 Form 集成。实现文档聚焦 `Slider`、`SliderTrack` 和 `SliderThumb` 的协作关系，不重新说明 `RangeBase` 的基础数值和自动化行为。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Slider/Slider.cs`：public API、pointer / keyboard 交互、tooltip 同步、tick 吸附、Form 接口和自动化 peer。
- `src/AtomUI.Desktop.Controls/Slider/SliderTrack.cs`：track / mark 渲染、thumb 布局、范围值裁剪、drag delta 转值、全局 pointer 订阅和内部 render context。
- `src/AtomUI.Desktop.Controls/Slider/SliderThumb.cs`：thumb pointer capture、drag routed events、focus / pressed 状态和直接绘制。
- `src/AtomUI.Desktop.Controls/Slider/SliderToken.cs`：Slider Token scope、尺寸、颜色、padding、outline 默认值计算。
- `src/AtomUI.Desktop.Controls/Slider/SliderAutomationPeer.cs`：Slider 自动化 peer。
- `src/AtomUI.Desktop.Controls/Slider/SliderThumbAutomationPeer.cs`：SliderThumb 自动化 peer。
- `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTheme.axaml`：Slider 根模板、track / thumb 创建和状态样式。
- `src/AtomUI.Desktop.Controls/Slider/Themes/SliderTrackTheme.axaml`：track token、mark token 和 transition。
- `src/AtomUI.Desktop.Controls/Slider/Themes/SliderThumbTheme.axaml`：thumb 尺寸、边框、outline、focus / hover 和 transition。

## 3. 核心类职责

`Slider` 是 public 控件入口。它保存 public state，处理键盘和 pointer 隧道路由，选择当前有效 thumb，并把值写入 `Value` 或 `RangeValue`。

`SliderTrack` 是布局和渲染核心。它根据 `Minimum`、`Maximum`、`Value`、`RangeValue`、`Orientation`、`IsDirectionReversed` 和 `Marks` 计算 thumb 中心点、rail rect、active track rect、mark rect 和 mark 文本 rect。

`SliderThumb` 是可拖动视觉元素。它不直接知道 `Minimum`、`Maximum` 或业务值，只把 pointer 位移转换为 `DragDelta` 事件交给 `SliderTrack` 或 `Slider` 解释。

`SliderToken` 提供组件级视觉语义。实例值、当前拖动状态、mark 集合和 tooltip 文本不是 token。

## 4. 状态与数据流

单值模式数据流：

```text
Value
  ↓
SliderTrack.Value
  ↓
CalculateThumbValuePivotOffset()
  ↓
StartThumb arrange
  ↓
Render rail / active track / marks
```

范围模式数据流：

```text
RangeValue.StartValue / EndValue
  ↓
SliderTrack.RangeValue
  ↓
CalculateThumbValuePivotOffset()
  ↓
StartThumb + EndThumb arrange
  ↓
Render selected range track + active marks
```

Pointer 写值流：

```text
PointerPressed on Slider
  ↓
GetEffectiveMoveThumb()
  ↓
mark hit ? write mark.Value : MoveToPoint()
  ↓
Value or RangeValue
  ↓
tooltip and render refresh
```

Drag 写值流：

```text
SliderThumb pointer move
  ↓
DragDelta(Vector)
  ↓
SliderTrack.ValueFromDistance()
  ↓
ValueProperty or deferred drag
```

`SliderTrack.IgnoreThumbDrag=true` 由 `Slider` 在模板接入时设置，避免 `SliderTrack` 自己处理 thumb drag，与 Slider 的轨道拖动模型冲突。维护时不要让 `Slider` 和 `SliderTrack` 同时写同一个拖动值。

## 5. 生命周期与模板接入

`Slider.OnApplyTemplate` 负责：

- 释放旧 pointer handler。
- 获取 `PART_Track`。
- 设置 `SliderTrack.IgnoreThumbDrag=true`。
- 通过 tunneling pointer 事件接入轨道按下、移动和释放。
- 根据当前值设置 thumb tooltip 文本。
- 设置 tooltip placement。
- 同步 `:horizontal` / `:vertical` 伪类。

`SliderTrack.ThumbChanged` 负责把 `StartSliderThumb` 和 `EndSliderThumb` 加入或移出 logical / visual children，并订阅或解绑 `DragDelta`、`DragCompleted`。不要绕过该属性直接操作 `VisualChildren`。

`SliderTrack.OnAttachedToVisualTree` 订阅全局 `IInputManager.Process`，用于点击轨道外部时取消 thumb focus。`OnDetachedFromVisualTree` 必须释放该订阅。

`SliderThumb` 通过 pointer capture 生命周期维护 `_lastPoint`。capture lost 和 pointer released 都会结束 drag，并移除 pressed 伪类。

## 6. 交互与事件处理

### 6.1 指针

轨道 pointer down 时，Slider 选择当前操作 thumb：

- 单值模式固定使用 `StartSliderThumb`。
- 范围模式比较点击点到两个 thumb 中心的距离，选择更近的 thumb。

点击 mark 文本命中区域时，Slider 直接写入 mark 值；未命中 mark 时，根据轨道坐标计算连续值。拖动期间 `MoveToPoint` 会继续根据当前 pointer 位置更新值。

### 6.2 键盘

键盘入口在 `Slider.OnKeyDown`。方向键、Page 键、Home 和 End 进入 `RangeBase.Value` 单值路径。范围模式不把键盘操作映射为起始或结束 thumb 的独立编辑状态，维护时不能把范围键盘行为描述为已完整覆盖。

### 6.3 Thumb Drag

`SliderThumb` 只发出 drag 事件。`SliderTrack` 根据 `Orientation` 和 `IsDirectionReversed` 把位移转换成 value delta。`DeferThumbDrag=true` 时，拖动位移先保存到 `_deferredThumbDrag`，直到关闭 defer 后再应用。

### 6.4 Tooltip

`Slider` 负责设置 tooltip 内容、placement 和 host width。`SliderTrack` 和 `SliderThumb` 不格式化 tooltip 文本。`ValueFormatTemplate` 变更后应重新计算 tooltip 文本和宽度。

## 7. 内部算法与关键流程

### 7.1 值到坐标

`SliderTrack.CalculateThumbValuePivotOffset` 使用以下输入计算 thumb 中心点：

- `Minimum`
- `Maximum`
- `Value` 或 `RangeValue`
- 当前 orientation
- thumb 尺寸
- 是否垂直布局

水平布局中，值越大通常越靠右；垂直布局中，值越大通常越靠上。`IsDirectionReversed` 会在指针转值路径中反转方向。

### 7.2 坐标到值

`Slider.MoveToPoint` 从 pointer 坐标计算逻辑位置，再映射到 `[Minimum, Maximum]`。计算步骤包括：

1. 扣除 thumb 半径，使 pointer 坐标对应 thumb 中心。
2. 根据 orientation 选择 X 或 Y 坐标。
3. 根据 `IsDirectionReversed` 和垂直方向反转关系计算逻辑比例。
4. 映射到数值范围。
5. 根据 `IsSnapToTickEnabled` 决定是否吸附。

### 7.3 Tick 吸附

`SnapToTick` 在 `IsSnapToTickEnabled=true` 时生效。`TickFrequency > 0` 时按 tick 间隔寻找最近 tick；否则只在 `Minimum` 和 `Maximum` 之间选择。键盘移动时，如果吸附结果仍等于当前值，会继续寻找下一个 tick，避免方向键停在原地。

### 7.4 Mark 渲染与命中

`SliderTrack.CalculateMaxMarkSize` 根据 `Marks` 和字体属性测量标签尺寸，并缓存到 `SliderMark` 的 internal 字段。渲染前 `PrepareRenderInfo` 计算 mark 点和文本矩形；`GetMarkForPosition` 使用这些矩形判断 pointer 是否命中 mark 文本。

Mark active 状态由 `IsIncluded` 控制：

- 单值模式：`mark.Value <= Value`。
- 范围模式：`RangeValue.StartValue <= mark.Value <= RangeValue.EndValue`。

### 7.5 渲染

`SliderTrack.Render` 每次绘制前准备 render context，然后按顺序绘制 groove、active track 和 mark。`SliderThumb.Render` 绘制圆形 thumb 和 outline。直接绘制路径必须继续使用 token 传入的 brush、size 和 thickness，不把运行时状态写回 token。

## 8. 资源、性能与 AOT 边界

Slider 不依赖反射扫描或运行时动态发现控件成员。模板协同通过 `TemplateBinding`、稳定 template part 和 Avalonia property 完成。

性能边界：

- `RenderContextData` 是单次渲染准备数据，不应被外部缓存为长期状态。
- Mark 文本测量只在 mark、字体或 enabled 相关变化时刷新，避免每次 render 重新测量文本。
- `SliderThumb` 使用 cached pen helper 更新画笔，避免每帧创建多余对象。
- 全局 input subscription 必须在 detach 时释放。

AOT 边界：

- 不新增反射访问 public API、template part 或 token kind。
- 新增 Token 必须走 source generator 支持的 `SliderToken` 属性。
- Gallery API / Token 表应显式维护，不依赖运行时反射扫描。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `Slider` 是 pointer / keyboard 写值入口；`SliderTrack` 不应在 `IgnoreThumbDrag=true` 路径下同时写值。
- `StartSliderThumb` / `EndSliderThumb` 的 logical / visual children 和 drag 事件由 `ThumbChanged` 成对管理。
- 模板重新应用时旧 pointer handler 必须释放。
- `SliderTrack` detach 时释放全局 input subscription。
- `RangeValue` 必须保持非 NaN、非 Infinity，并裁剪到 `[Minimum, Maximum]`。
- `SliderRangeValue.Parse` 必须拒绝起始值大于结束值的表达式。
- `Marks` 改变后必须重新测量 mark 标签。
- `IsIncluded=false` 只影响 active track / active mark 绘制，不影响值计算和 mark 命中。
- tooltip 格式化只由 `ValueFormatTemplate` 控制。
- Thumb focus / hover outline 由 `SliderThumbTheme` 和 token 控制，不在 `Slider` 中手写视觉状态。

## 10. 测试与验证

验证范围：

- 单值模式下 pointer 点击、拖动、方向键、Page 键、Home / End。
- 范围模式下两个 thumb 可见性、最近 thumb 选择、mark 点击和 `RangeValue` 裁剪。
- `Orientation=Horizontal/Vertical` 下 thumb 位置、tooltip placement 和 padding。
- `IsDirectionReversed=true` 下 pointer 和键盘方向。
- `IsSnapToTickEnabled=true` 与不同 `TickFrequency` 的吸附结果。
- `Marks` 标签测量、active 状态、点击命中和 disabled 文本色。
- `IsIncluded=false` 下 active track 不绘制但交互保持。
- `IsMotionEnabled=false` 下 transition 禁用。
- 模板重建后 pointer handler 和 tooltip 重新接入。
- detach 后全局 input subscription 释放。
- Gallery `SliderShowCasePageTests` 保持 API、Token 和示例结构一致。
- 文档改动运行 `git diff --check`。
