# ToggleSwitch 桌面版实现原理

本文档描述 ToggleSwitch 桌面版的内部布局、状态流转、模板接入、加载动画、内容绑定、Form 和 Token 使用。公共设计与 API 契约见 [ToggleSwitch 桌面版架构设计](overview.md)，Token 语义见 [ToggleSwitch Token 设计](token.md)，变化记录见 [ToggleSwitch Changelog](changelog.md)。

## 1. 实现定位

ToggleSwitch 的 public 桌面类型位于 `AtomUI.Desktop.Controls`，实际交互和布局实现位于 `AtomUI.Controls.AbstractToggleSwitch`。桌面层通过 `ToggleSwitchToken` 和主题文件提供 参考设计体系 风格视觉。本文档聚焦 AtomUI 增强层，不重新说明 Avalonia `ToggleButton` 的基础点击和键盘切换算法。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Switch/ToggleSwitch.cs`：public 桌面控件，注册 `ToggleSwitchToken` resource scope。
- `src/AtomUI.Controls/Switch/AbstractToggleSwitch.cs`：公共 API、测量、布局、内容绑定、loading、hit test、Form 和渲染主逻辑。
- `src/AtomUI.Controls/Switch/SwitchKnob.cs`：把手绘制、加载指示绘制、把手宽度动画和加载动画生命周期。
- `src/AtomUI.Desktop.Controls/Switch/ToggleSwitchToken.cs`：ToggleSwitch 组件级 Token。
- `src/AtomUI.Desktop.Controls/Switch/Themes/ToggleSwitchTheme.axaml`：主模板、状态 selector、SizeType 分支和 Token 引用。
- `src/AtomUI.Desktop.Controls/Switch/Themes/SwitchKnobTheme.axaml`：把手主题、加载动画周期和动效。
- `src/AtomUI.Desktop.Controls/Switch/Themes/ToggleSwitchThemes.axaml`：主题聚合入口。

## 3. 核心类职责

`ToggleSwitch` 是桌面包对外入口。它不新增 API，也不重写布局和交互，只把实例接入 `ToggleSwitchToken` scope。

`AbstractToggleSwitch` 是状态和布局核心。它负责读取 `IsChecked`、`IsLoading`、`SizeType`、`OnContent`、`OffContent` 等状态，计算轨道尺寸、把手位置、内容偏移和 Form value。

`SwitchKnob` 是内部把手。它根据 `KnobSize` 和 `KnobRenderWidth` 绘制圆形或胶囊把手，在 loading 状态下绘制旋转 loading arc。

`WaveSpiritDecorator` 是视觉效果边界，只在 checked 状态变化且 `IsWaveSpiritEnabled=true` 时播放，不参与值状态计算。

## 4. 状态与数据流

基础状态流：

```text
IsChecked / IsPressed / SizeType / content / token
      ↓
MeasureOverride
      ↓
Track size + content desired size
      ↓
CalculateElementsOffset
      ↓
KnobRect + KnobMovingRect + OnContentOffset + OffContentOffset
      ↓
ArrangeOverride + Render
```

checked 变化时，如果启用 motion，控件先基于当前 groove size 重新计算元素位置，再由 `RectTransition` 和 `PointTransition` 执行动画。`_isCheckedChanged` 用于让 arrange 阶段在 checked 切换后使用 `KnobMovingRect`，保持过渡位置一致。

loading 状态流：

```text
IsLoading changed
  → HandleLoadingState
  → set cursor local value / dispose cursor local value
  → SwitchKnob.NotifyStartLoading / NotifyStopLoading
```

Form 状态流：

```text
Form.SetValue(bool?) → IsChecked
IsChecked changed    → Form ValueChanged
Form.GetValue()      → IsChecked
Form.ClearValue()    → IsChecked = null
```

## 5. 生命周期与模板接入

`OnApplyTemplate` 获取以下稳定 part：

- `PART_SwitchKnob`
- `PART_OnContentPresenter`
- `PART_OffContentPresenter`
- `PART_WaveSpirit`
- `PART_MainContainer`

模板接入后，控件把当前 `KnobSize` 写入 `SwitchKnob`，再根据当前 `IsLoading` 同步把手 loading 状态。模板 part 没有全局事件订阅，主要生命周期风险来自内容图标 relay binding 和 loading animation。

内容变化时，`SetupContent` 会释放旧 on/off 内容对应的 `CompositeDisposable`，再为新的 `PathIcon` / `Icon` 建立尺寸和前景色绑定。维护时必须保持内容替换和 disposable 释放成对出现。

`SwitchKnob` 在 `OnAttachedToVisualTree` 中根据 `_isLoading` 启动加载动画，在 `OnDetachedFromVisualTree` 中取消并释放 `CancellationTokenSource`。

## 6. 交互与事件处理

Pointer press 流程：

```text
OnPointerPressed
  → if !IsLoading then base.OnPointerPressed
  → if knob not mid-animation
  → _isCheckedChanged=false
  → AdjustOffsetOnPressed
```

Pointer release 流程：

```text
OnPointerReleased
  → if !IsLoading then base.OnPointerReleased
  → AdjustOffsetOnReleased
  → InvalidateArrange
```

loading 状态下 pointer press/release 不进入基类逻辑，因此不会切换 `IsChecked`。键盘切换仍来自 `ToggleButton` 基类，维护 loading 行为时必须确认键盘路径也不产生业务不一致。

`HitTest(Point)` 只在控件 enabled 且非 loading 时返回 groove rect 命中结果。该接口用于自定义命中测试，不应把内容 presenter 或 wave 外扩区域作为可点击范围。

## 7. 内部算法与关键流程

### 7.1 测量与宽度计算

测量阶段分别测量 on/off content presenter，取二者最大宽度作为内容宽度，再叠加 `InnerMinMargin` 和 `InnerMaxMargin`。最终宽度取该值与 `TrackMinWidth` 的较大值，高度使用 `TrackHeight`。

### 7.2 把手位置

`HandleRect(bool isChecked, Size controlSize)` 根据 checked 状态计算把手左上角。unchecked 时把手位于左侧 `TrackPadding`，checked 时位于 `controlSize.Width - TrackPadding - handleSize`。按下时把手宽度乘以 `STRETCH_FACTOR`。

### 7.3 内容偏移

`ExtraInfoRect` 根据当前状态和内部边距计算内容区域。checked 时 on content 进入可见区域，off content 移到右侧不可见区域；unchecked 时反向处理。内容偏移使用 `PointTransition` 过渡。

### 7.4 轨道绘制

`AbstractToggleSwitch.Render` 使用 `DrawPilledRect` 绘制轨道，并通过 `SwitchOpacity` 控制 disabled/loading 透明度。轨道背景来自 `GrooveBackground`，通常由主题 selector 设置。

### 7.5 把手与 loading 绘制

`SwitchKnob.Render` 根据 `KnobRenderWidth` 和高度判断绘制圆形或胶囊把手。loading 状态下，按当前 `Rotation` 绘制 90 度 arc。`GetLoadIndicatorPen` 缓存 pen，并在 brush 引用变化时重建。

## 8. 资源、性能与 AOT 边界

ToggleSwitch 不通过反射访问模板结构。模板结构由稳定 part 和 AXAML `TemplateBinding` 表达。

资源与生命周期边界：

- on/off 图标内容的 relay binding 必须在内容替换时释放。
- `IsLoading` 设置的 cursor local value 必须在退出 loading 时 dispose。
- `SwitchKnob` 的 loading `CancellationTokenSource` 必须在停止 loading 和 detach 时释放。
- Token 只表达尺寸、颜色、阴影、字体和加载动画周期，不承载 `IsChecked`、`IsLoading` 或内容实例状态。

当前图标内容绑定使用 C# relay binding，因为目标对象来自用户提供的 runtime content，不是稳定模板 part。模板内部固定关系应继续优先使用 AXAML binding 和 selector。

AOT 边界：

- Token 类型通过 generator 显式注册。
- Gallery API / Token 表使用显式 view model 数据。
- 文档中列出的 part 名称必须与 AXAML 和 C# 查找代码一致。

## 9. 维护不变量

内部重构必须保持以下不变量：

- `ToggleSwitch` public 类型保持轻量桌面入口，不把共享实现复制到 Desktop 包。
- `AbstractToggleSwitch` 继续作为 API 和状态核心。
- loading 不触发新切换，并释放 cursor local value 和 animation token。
- 内容图标 relay binding 在内容替换时释放。
- `MeasureOverride` 必须同时考虑 on/off content 的最大宽度。
- `SwitchKnob` loading animation detach 时必须取消。
- `SizeType=Custom` 默认与 Middle 视觉一致。
- `SwitchOpacity` 只表达 disabled/loading 视觉透明度，不改变 enabled 状态。
- WaveSpirit 只作为视觉反馈，不影响 `IsChecked`。

## 10. 测试与验证

验证范围：

- checked、unchecked、null 绑定和 Form 读写清空。
- loading 状态下 pointer 和命中测试不触发切换。
- loading attach/detach 不泄漏动画 token。
- `OnContent` / `OffContent` 为文本和图标时的测量、偏移、前景色和尺寸同步。
- Large / Middle / Small / Custom 下的高度、最小宽度、内容边距和图标尺寸。
- disabled、pointerover、pressed、checked 和 unchecked 主题 selector。
- WaveSpirit 开关不改变值状态。
- Gallery ToggleSwitch 示例、API 表和 Token 表。
- 文档改动运行 `git diff --check`，并检查相对链接存在。
