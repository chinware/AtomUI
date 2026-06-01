# Primitives Performance Notes

## Scope

本轮处理并完成审计以下 Primitives 共享路径：

- `InfoPickerInput` 体系里的 `PickerClearUpButton`。它被 `DatePicker`、`RangeDatePicker`、`TimePicker`、`RangeTimePicker` 等 picker 输入控件复用。
- `AddOnDecoratedBox` 的 `PART_ContentFrame` pointer 状态路径。它被 LineEdit / Select / TreeSelect / Cascader / DatePicker / TimePicker / ButtonSpinner 等输入类控件复用。
- `CandidateList` 的键盘候选高亮路径。它被 AutoComplete / Mentions 等 popup 候选列表复用。
- `RangeInfoPickerInput` 的 range indicator 布局写入路径。它被 RangeDatePicker / RangeTimePicker 复用。
- `InfoPickerInput` 的通用 open / close 写入路径。它被 DatePicker / TimePicker 及 range picker 共享。
- `TreeNodePath` 的路径派生操作。它被 Cascader / NavMenu / TreeView 等树形路径控件复用。
- `MotionGhostControl` 的 shadow mask 几何更新路径。它被带 motion ghost 的浮层 / motion 控件复用。
- `AbstractArrowDecoratedBox` 的 arrow shell 布局状态路径。它被 Popup / Flyout / DatePicker / TimePicker 等带箭头浮层复用。
- `WaveSpiritDecorator` 的 wave painter 配置路径。它被 Button / Switch / Radio / CheckBox 等点击波纹控件复用。
- `CompactSpaceItem` / `CompactSpaceAddOn` 的 compact-space 状态通知路径，以及 `TextBox` / `TextArea` / `ButtonSpinner` / `NumericUpDown` / `AbstractSelect` 的 compact-space / form feedback / validation 状态同步路径。
- `PenUtils` / `BorderRenderHelper` 的 render-loop pen 复用路径。它被 DashedBorder / OptionButton / TreeView / ButtonSpinner / RadioIndicator 等自绘边框控件复用。

本轮收口 audit 结论：Primitives 目录剩余扫描项为 false positive、已验证结构路径或此前已回滚候选；没有继续排队的开放优化项。

## InfoPickerInput Root Cause

- `PickerClearUpButton.OnApplyTemplate()` 原先给模板内 `PART_ClearButton` 注册 lambda 形式的本地 `Click` handler。
- 该 lambda 没有 re-template 释放路径，每个实例都有 1 条本地 routed-event handler。
- `PART_ClearButton` 是静态模板节点，非 clear mode 下通过 `IsVisible=false` 隐藏；旧逻辑如果被测试或内部代码直接 raise `ClickEvent`，仍会触发 `ClearRequest`。
- `DatePicker` 状态验证仍按旧的动态 slot 模型断言 popup/accessory 子树必须不存在，和当前静态模板策略不一致。

## AddOnDecoratedBox Root Cause

- `AddOnDecoratedBox.OnApplyTemplate()` 原先给模板内 `PART_ContentFrame` 注册 4 条本地 pointer routed-event handler：`PointerEntered` / `PointerExited` / `PointerPressed` / `PointerReleased`。
- `PART_ContentFrame` 是每个 AddOnDecoratedBox 实例都存在的静态模板节点；LineEdit / Select / TreeSelect / Cascader / DatePicker / TimePicker 这类页面会按实例数线性放大这 4 条 handler。
- Avalonia 12 里 `PointerEntered` / `PointerExited` 是 Direct routed event，`PointerPressed` / `PointerReleased` 是 Tunnel + Bubble routed event；事件 CLR add/remove 会落入本地 `_eventHandlers`。参考 `.referenceprojects/Avalonia/src/Avalonia.Base/Input/InputElement.cs:144-177`、`:355-392`。
- Select / TreeSelect / Cascader 的 `PART_ContentRightAddOnPresenter.ContentTemplate` 还错误绑定到了 `ContentLeftAddOnTemplate`，导致 right addon template 不生效。
- `AddOnDecoratedBox` 的 addon status foreground / icon brush、inner corner radius、inner border thickness 和 content-frame pointer state 路径原先会在结果未变化时仍调用写入 / clear。

## CandidateList Root Cause

- `CandidateList.TrySetCandidateItemSelected(index)` 原先每次候选高亮移动都会从 `0..ItemCount-1` 扫描全部容器，并对每个已实现 `CandidateListItem` 重写 `IsCandidateSelected`。
- 键盘上下导航只需要清理旧候选项并设置新候选项；全量扫描在 AutoComplete / Mentions popup 打开后会按按键次数线性放大。
- `CandidateVirtualizingStackPanel.ScrollCandidateItemIntoView(index)` 已经能把目标项滚入视图，本轮只复用目标容器，不改变虚拟化、过滤、提交 / 取消语义。
- `CandidateList` 的 max-count enable 状态、candidate selected 状态、empty indicator 和 commit/reset 路径原先会在结果未变化时重复写入。
- 空 `CandidateList` 的上下键导航原先会尝试写入无效 candidate index；commit/cancel 后的 `ClearState()` 和虚拟化清理也可能重复写 null / -1 / ClearValue。

## TreeNodePath Root Cause

- `TreeNodePath.Append(string)`、`Append(TreeNodePath)`、`GetParent()` 和 `WithSegment()` 原先先创建新 segments 数组，再调用公开构造函数二次校验并复制一份数组。
- 这些派生操作的来源路径已经是已校验的 `TreeNodePath`，新增 segment 也在方法入口单独校验；因此可以使用私有 trusted 构造，保留公开构造函数的外部输入校验语义，同时避免二次数组分配。
- 空路径 append 已有路径、单段路径取 parent、同值 `WithSegment()` 原先仍会创建新数组 / 新 wrapper；这些场景都可以复用已有不可变对象。
- `TreeNodePath.StartsWith(path)` 对同一不可变实例原先仍按 segment 数量逐项比较。
- `TreeNodePath(string)` 原先 `Split('/', RemoveEmptyEntries)` 后再进入公开构造的二次复制；本轮改为先数段、再按段构造结果数组，保留前导、连续、尾随 `/` 的空段跳过语义。

## MotionGhostControl Root Cause

- `MotionGhostControl.HandleMaskCornerRadiusOrSizeChanged()` 原先在 `MaskCornerRadius` 或 `MaskSize` 任一变化时，都会对每个 shadow renderer 重写 `CornerRadius`、`Width`、`Height` 三项。
- content size callback 原先也会直接重写 mask center frame 的 `Width` / `Height`。
- 本轮只跳过同值写入，不改变 motion ghost 的创建、shadow renderer 数量、bitmap capture 或动画时序。

## ArrowDecoratedBox / WaveSpirit Root Cause

- `AbstractArrowDecoratedBox` 在 template apply、`ArrowPosition` 变化和重复 arrange 后，会重复写 `ArrowDirection`、`ArrowIndicatorBounds`、`ArrowIndicatorLayoutBounds`；外部 shadow mask provider 重复设置相同 arrow opacity 时也会重复写 `ArrowOpacity`。
- `WaveSpiritDecorator.ConfigureWavePainter()` 原先每次配置 painter 都创建两个新的 `CubicEaseOut`，但实际曲线固定不变。
- wave painter 的 size / opacity animation 兜底路径原先在没有配置 easing 时会 `new LinearEasing()`；可以复用静态默认实例。
- `CompactSpaceItem.MeasureOverride()` 原先每次 measure 都会为同一 offset 重建 `TranslateTransform`；从 Middle / Last 回到单项位置时还可能残留旧 offset transform。
- `CompactSpaceItem` / `CompactSpaceAddOn` / `TextBox` / `ButtonSpinner` / `NumericUpDown` 原先在 repeated compact-space position / orientation 通知下继续写 `IsUsedInCompactSpace`、`CompactSpaceItemPosition`、`CompactSpaceOrientation`。
- `TextBox` / `TextArea` / `AbstractSelect` 原先在 repeated same form feedback control 通知下继续写 `FormFeedback`，可能触发 feedback subscription refresh。
- `TextArea` / `NumericUpDown` / `AbstractSelect` 原先在 repeated validation status 通知下继续写 `Status`。
- `PenUtils.TryModifyOrCreate()` 原先在 immutable brush 路径下，即使 brush / thickness / dash / cap / join / miter 全部未变化，也会重新创建 `ImmutablePen`；带 dash 时还会重新创建 `ImmutableDashStyle`。
- `BorderRenderHelper` 原先的 geometry cache key 未记录 dashed enabled 状态；同一个 helper 从非 dashed 切到 dashed 或反向切换时，可能沿用旧的复杂 / 简单渲染模式。

## RangeInfoPickerInput Root Cause

- `InfoPickerInput.OnPointerReleased()`、`ClosePickerFlyout()`、window deactivated 和 owned presenter cleanup 都可能重复写 `IsPickerOpen`，即使当前开关状态已经相同。
- `RangeInfoPickerInput.MeasureOverride()` 每次 layout 都会写 `PickerIndicatorOffsetY`，即使 indicator Y offset 没变也会进入 Avalonia styled property 写入路径。
- `RangeInfoPickerInput.ArrangeOverride()` 每次 layout 都会写 `Canvas.Left` / `Canvas.Top` attached properties，即使 range indicator 位置没变也会进入 attached property 写入路径。
- `RangeInfoPickerInput.OnPointerReleased()` 在判断 start / end 输入框命中时最多会调用两次 `args.GetPosition(this)`；同一次 pointer release 的相对坐标可以取一次后复用。
- 带 `ContentLeftAddOn` 的 range 输入点击路径中，start / end hit-test 原先会各自换算一次 left addon 坐标；同一次 pointer release 可共用同一个 left boundary。
- 点击已经打开的同一 range 输入端时，旧路径仍会重复写 `RangeActivatedPart` 和 `IsPickerOpen=true`；关闭已关闭 popup 时也会重复写 `IsPickerOpen=false`。
- `RangeActivatedPart` 切换到 Start 时，`PickerPlacement` 默认已经是 `BottomEdgeAlignedLeft`，旧路径仍会重复写入；重复 setup 同一个 popup target 也会重复写 `PlacementTarget`。
- `InfoPickerInput.ConfigureIsClearButtonVisible()`、`ConfigureShowArrowEffective()`、`ConfigureArrowPosition()` 和 `NotifyValidateStatus()` 原先会在结果未变化时重复写 styled property。
- `InfoPickerInput` 的 compact-space position / orientation 通知和 form feedback 控件同步原先会在同一值重复下发时继续写 styled property，并触发模板绑定 / feedback 订阅链路。
- RangeDatePicker / RangeTimePicker 的 range indicator 是静态模板节点，本轮只跳过 unchanged 写入，不改变模板、动画、popup 或 active range 语义。

## Changes

- `PickerClearUpButton` 改为 `Button.ClickEvent.AddClassHandler<PickerClearUpButton>()`。
- 移除 `OnApplyTemplate()` 中的 `_clearButton.Click += ...` lambda。
- `IsInClearMode=false` 时直接 `ClickEvent` 不再触发 `ClearRequest`。
- `AddOnDecoratedBox` 模板里的静态 `PART_ContentFrame` 从 `Border` 换成 `AddOnDecoratedBoxContentFrame : Border`，由子类 override pointer 事件回写 `IsInnerBoxHover` / `IsInnerBoxPressed`。
- `AddOnDecoratedBoxContentFrame` 保持 `StyleKeyOverride => typeof(Border)`，确保原有 `Border#PART_ContentFrame` size/padding selector 继续命中。
- 移除 `AddOnDecoratedBox.OnApplyTemplate()` 对 `ContentFrame` 的 4 条本地 pointer handler 注册 / 退订。
- 修复 Select / TreeSelect / Cascader 的 right addon presenter template binding：`ContentLeftAddOnTemplate` -> `ContentRightAddOnTemplate`。
- `CandidateList.TrySetCandidateItemSelected(index)` 改为只清旧 index、设置新 index，不再每次移动扫描全部 `ItemCount`。
- `CandidateList` 的 container `IsEnabled`、`IsCandidateSelected`、`CandidateSelectedItem`、`CandidateSelectedIndex`、`SelectedItem` 和 `IsEffectiveEmptyVisible` 写入增加 unchanged guard。
- `CandidateList` 空列表上下键导航直接返回；`ClearState()`、detach cleanup 和 virtualized container clear 路径增加 unchanged guard。
- `AddOnDecoratedBox` 的 addon foreground / icon brush、inner radius / thickness 和 content-frame hover / pressed 状态写入增加 unchanged guard。
- `TreeNodePath` 新增私有 trusted 构造；`Append` / `GetParent` / `WithSegment` 改为只分配目标 segments 数组一次。
- `TreeNodePath.Empty.Append(path)`、单段 `GetParent()` 和同值 `WithSegment()` 改为直接返回已有不可变对象。
- `TreeNodePath.StartsWith()` 对同一实例和 empty prefix 增加 fast path。
- `MotionGhostControl` 的 shadow renderer radius / width / height 和 mask center frame width / height 写入增加 unchanged guard。
- `AbstractArrowDecoratedBox` 的 arrow direction、indicator bounds、indicator layout bounds 和 arrow opacity 写入增加 unchanged guard。
- `WaveSpiritDecorator` 复用静态 `CubicEaseOut` 作为默认 wave painter easing，避免每次配置创建 2 个 easing 对象。
- `AbstractWavePainter` 复用静态 `LinearEasing` 作为 animation fallback easing，避免兜底路径重复创建 easing 对象。
- `CompactSpaceItem` / `CompactSpaceAddOn` / `TextBox` / `ButtonSpinner` / `NumericUpDown` 的 compact-space position / orientation 写入增加 unchanged guard。
- `CompactSpaceItem` 的 offset transform 改为同 offset 复用，进入单项 / first 状态时清理旧 transform。
- `TextBox` / `TextArea` / `AbstractSelect` 的 form feedback control 写入增加 unchanged guard。
- `TextArea` / `NumericUpDown` / `AbstractSelect` 的 validation status 写入增加 unchanged guard。
- `PenUtils.TryModifyOrCreate()` 在已有 `IPen` 与目标 brush / thickness / dash / cap / join / miter 完全一致时直接返回，不再创建新 pen / dash style。
- `BorderRenderHelper` 将 dashed enabled 状态加入 cache key，dash 状态切换会刷新 geometry rendering mode。
- `--verify-datepicker-states` 改为验证静态 popup shell / accessory slot：模板 shell 保留，但 presenter/content 在关闭态为空；detach 后 presenter/content 释放。
- `--verify-datepicker-states` 新增 clear button lifecycle 覆盖：隐藏 direct click 惰性、本地 Click handler 为 0、clear mode 下仍能清空 `SelectedDateTime`。
- `--verify-addon-states` 新增 ContentFrame pointer lifecycle 覆盖：本地 pointer routed handler 为 0，直接事件仍正确更新 hover / pressed 状态；同时覆盖 Select / TreeSelect / Cascader right-addon template。
- `--verify-addon-states` 覆盖 `PART_ContentFrame` 的 `StyleKey == Border` 和 token padding 非零，避免 DatePicker / TimePicker / LineEdit 输入框内边距回归。
- `--verify-addon-states` 新增 CompactSpaceItem / CompactSpaceAddOn / TextBox / TextArea / ButtonSpinner / NumericUpDown / AbstractSelect repeated state notification 覆盖，以及 CompactSpaceItem offset transform 复用和 stale transform 清理覆盖。
- `--verify-addon-states` 新增 `PenUtils` repeated immutable / dashed pen 复用覆盖。
- `--verify-listbox-states` 新增 CandidateList 候选高亮移动覆盖：index 0 -> 1 -> -1 时只保留当前候选态；同时修正 ListBox 静态 slot 校验口径。
- `RangeInfoPickerInput` 的 range indicator 写入增加 unchanged guard：`PickerIndicatorOffsetY`、`Canvas.Left`、`Canvas.Top`、`RangePickerIndicatorOpacity`、indicator `Width`、`PickerIndicatorOffsetX` 只在值变化时写入。
- `RangeInfoPickerInput.OnPointerReleased()` 将 pointer position 改为单次读取后复用，start / end hit-test 语义不变。
- `RangeInfoPickerInput` 的 start / end hit-test 合并为一个 helper，`ContentLeftAddOn` boundary 单次计算后复用。
- `RangeInfoPickerInput` 的 `IsPickerOpen`、`RangeActivatedPart`、`PickerPlacement`、popup `PlacementTarget` 写入增加 unchanged guard。
- `InfoPickerInput` 新增共享 `SetPickerOpenIfChanged()`，单输入 picker 和 range picker 的 open / close 写入都先判断值是否变化。
- `InfoPickerInput` 的 clear button 可见性、arrow effective、arrow position 和 validation status 写入增加 unchanged guard。
- `InfoPickerInput` 的 compact-space position、compact-space orientation 和 form feedback 控件写入增加 unchanged guard。
- `InfoPickerInput.IsPointerInInfoInputBox()` 直接返回 bounds 命中结果，去掉多余分支。
- `--verify-datepicker-states` 新增 RangeDatePicker indicator 重复 layout 稳定性覆盖，以及 DatePicker repeated compact-space / form-feedback 通知不重写状态覆盖。

## Benefit

这轮收益是结构和正确性收益：picker 输入控件每个 `PickerClearUpButton` 少 1 条本地 Click 订阅；每个 `AddOnDecoratedBox` 少 4 条本地 pointer 订阅；隐藏 clear button 不再能被直接事件误触发；Select / TreeSelect / Cascader 的 right-addon template 绑定恢复正确。页面构建 timing 未作为本轮收益证明。

| Metric | Baseline | Optimized | Formula | Improvement | Conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Local Click handlers / `PickerClearUpButton` | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；改由 class handler 统一处理 |
| Hidden clear direct-click side effects / click | 1 clear request | 0 clear requests | `(1 - 0) / 1` | 100.00% | 正确性修复；非 clear mode 不再清值 |
| Local pointer handlers / `AddOnDecoratedBox` content frame | 4 / instance | 0 / instance | `(4 - 0) / 4` | 100.00% | 有效；静态 template part 保留，pointer 状态改由 `Border` 子类 override |
| Local pointer handlers / `CompactSpace.LineEdit` root | 12 / root | 0 / root | `(12 - 0) / 12` | 100.00% | 有效；3 个 AddOnDecoratedBox × 4 条 handler |
| AddOnDecoratedBox addon foreground writes / repeated same status brush | 4 writes | 0 writes | `(4 - 0) / 4` | 100.00% | 结构收益；4 个 addon presenter foreground 已是同一 brush 时不写 |
| AddOnDecoratedBox icon brush writes / repeated same status brush per icon | 3 writes | 0 writes | `(3 - 0) / 3` | 100.00% | 结构收益；`FillBrush` / `StrokeBrush` / `Foreground` 已是同一 brush 时不写 |
| AddOnDecoratedBox inner geometry writes / repeated layout update | 2 writes | 0 writes | `(2 - 0) / 2` | 100.00% | 结构收益；inner radius / thickness 计算结果未变时不写 |
| AddOnDecoratedBox pointer state writes / repeated content-frame event | 2 writes | 0 writes | `(2 - 0) / 2` | 100.00% | 结构收益；hover / pressed 状态未变时不写 |
| Wrong right-addon template bindings / Select family | 3 controls | 0 controls | `(3 - 0) / 3` | 100.00% | 正确性修复；Select / TreeSelect / Cascader 均已验证 |
| Lost input content padding after ContentFrame subclassing | 1 shared path | 0 shared paths | `(1 - 0) / 1` | 100.00% | 正确性修复；`PART_ContentFrame` 继续命中 `Border#PART_ContentFrame` padding selector |
| CandidateList candidate container checks / keyboard move (`Items20`) | 20 checks | <= 2 checks | `(20 - 2) / 20` | 90.00% | 结构收益；AutoComplete / Mentions 候选 popup 键盘上下移动少扫容器，不声明页面加载 speedup |
| CandidateList max-count enable writes / repeated same enabled state (`Items20`) | 20 writes | 0 writes | `(20 - 0) / 20` | 100.00% | 结构收益；容器 enabled 状态未变时不写 |
| CandidateList candidate selected writes / repeated same candidate | 2 writes | 0 writes | `(2 - 0) / 2` | 100.00% | 结构收益；当前 item 和 index 未变时不写 |
| CandidateList empty indicator writes / repeated same filter-empty state | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；empty indicator 结果未变时不写 |
| CandidateList invalid candidate writes / Down key on empty list | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益 + 正确性收敛；空列表不再尝试选择 index 0 |
| CandidateList clear-state writes / already cleared commit-cancel cleanup | 3 writes | 0 writes | `(3 - 0) / 3` | 100.00% | 结构收益；`SelectedItems` / `SelectedItem` / `SelectedIndex` 未变时不写 |
| CandidateList virtualized selected clear / already default container | 1 clear | 0 clears | `(1 - 0) / 1` | 100.00% | 结构收益；未设置 candidate selected 时不 `ClearValue` |
| TreeNodePath allocations / `Append(string)` | 2 arrays | 1 array | `(2 - 1) / 2` | 50.00% | 结构收益；新增 segment 已单独校验，避免公开构造二次复制 |
| TreeNodePath allocations / `Append(TreeNodePath)` | 2 arrays | 1 array | `(2 - 1) / 2` | 50.00% | 结构收益；两个来源路径均已校验，避免二次复制 |
| TreeNodePath allocations / `GetParent()` non-empty path | 2 arrays | 1 array | `(2 - 1) / 2` | 50.00% | 结构收益；去掉 range slice + 构造函数复制的双数组 |
| TreeNodePath allocations / `WithSegment(index, value)` | 2 arrays | 1 array | `(2 - 1) / 2` | 50.00% | 结构收益；替换值入口校验后复用 trusted 构造 |
| TreeNodePath `WithSegment(index, value)` LINQ copy callsites | 1 `ToArray()` | 0 `ToArray()` | `(1 - 0) / 1` | 100.00% | 结构收益；仍保留 1 个目标数组，复制改为直接 `Array.Copy` |
| TreeNodePath append copy helper dispatches | 2 `Array.CopyTo()` | 0 `Array.CopyTo()` | `(2 - 0) / 2` | 100.00% | 结构收益；append 路径统一直接 `Array.Copy` |
| TreeNodePath allocations / `TreeNodePath.Empty.Append(path)` | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；空路径 append 直接复用已有不可变 path |
| TreeNodePath allocations / single-segment `GetParent()` | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；直接复用 `TreeNodePath.Empty` |
| TreeNodePath allocations / same-value `WithSegment(index, value)` | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；segment 未变时返回当前不可变 path |
| TreeNodePath segment comparisons / `StartsWith(sameInstance)` length N | N comparisons | 0 comparisons | `(N - 0) / N` | 100.00% | 结构收益；同一不可变 path 直接返回 true |
| TreeNodePath path-string split arrays / `new TreeNodePath(\"a/b\")` | 1 split array | 0 split arrays | `(1 - 0) / 1` | 100.00% | 结构收益；手写扫描保留 `RemoveEmptyEntries` 语义 |
| TreeNodePath path-string segment array copies / `new TreeNodePath(\"a/b\")` | 1 copy array | 0 copy arrays | `(1 - 0) / 1` | 100.00% | 结构收益；直接填充最终 segments 数组 |
| TreeNodePath empty array allocations / empty public segments | 1 zero-length array risk | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；空输入复用 `[]` |
| MotionGhostControl shadow renderer writes / `MaskCornerRadius` only changes, per shadow | 3 writes | 1 write | `(3 - 1) / 3` | 66.67% | 结构收益；width / height 未变时不写 |
| MotionGhostControl shadow renderer writes / `MaskSize` only changes, per shadow | 3 writes | 2 writes | `(3 - 2) / 3` | 33.33% | 结构收益；corner radius 未变时不写 |
| MotionGhostControl mask center size writes / repeated same size callback | 2 writes | 0 writes | `(2 - 0) / 2` | 100.00% | 结构收益；center frame width / height 未变时不写 |
| ArrowDecoratedBox arrow direction writes / same placement direction | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；`ArrowPosition` 映射出的方向未变时不写 |
| ArrowDecoratedBox indicator bounds writes / repeated arrange | 2 writes | 0 writes | `(2 - 0) / 2` | 100.00% | 结构收益；indicator bounds / layout bounds 未变时不写 |
| ArrowDecoratedBox arrow opacity writes / repeated same opacity | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；shadow mask provider 重复设置同值 opacity 时不写 |
| WaveSpiritDecorator default easing allocations / painter configure | 2 objects | 0 objects | `(2 - 0) / 2` | 100.00% | 结构收益；复用静态 `CubicEaseOut`，曲线不变 |
| WavePainter fallback easing allocations / animation build without configured easing | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 结构收益；复用静态 `LinearEasing`，兜底曲线不变 |
| CompactSpaceItem compact-space usage writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；wrapper compact 状态未变时不写 |
| CompactSpaceItem compact-space position writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；wrapper position 未变时不写 |
| CompactSpaceItem compact-space orientation writes / repeated same orientation notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；wrapper orientation 未变时不写 |
| CompactSpaceItem offset transform allocations / repeated same measure after first | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 结构收益；同 offset 复用现有 `TranslateTransform` |
| CompactSpaceItem stale offset transforms / become single item after middle/last | 1 stale transform | 0 stale transforms | `(1 - 0) / 1` | 100.00% | 正确性修复；单项 / first 状态不保留旧位移 |
| CompactSpaceAddOn compact-space usage writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；addon compact 状态未变时不写 |
| CompactSpaceAddOn compact-space position writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；addon position 未变时不写 |
| CompactSpaceAddOn compact-space orientation writes / repeated same orientation notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；addon orientation 未变时不写 |
| TextBox compact-space usage writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；TextBox compact 状态未变时不写 |
| TextBox compact-space position writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；TextBox position 未变时不写 |
| TextBox compact-space orientation writes / repeated same orientation notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；TextBox orientation 未变时不写 |
| TextBox form feedback writes / repeated same feedback control | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；同一个 feedback control 不重复写 |
| TextArea validation status writes / repeated same form status | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；重复 Error / Warning / Default 不写 `Status` |
| TextArea form feedback writes / repeated same feedback control | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；同一个 feedback control 不重复写 |
| ButtonSpinner compact-space usage writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；ButtonSpinner compact 状态未变时不写 |
| ButtonSpinner compact-space position writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；ButtonSpinner position 未变时不写 |
| ButtonSpinner compact-space orientation writes / repeated same orientation notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；ButtonSpinner orientation 未变时不写 |
| NumericUpDown compact-space usage writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；NumericUpDown compact 状态未变时不写 |
| NumericUpDown compact-space position writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；NumericUpDown position 未变时不写 |
| NumericUpDown compact-space orientation writes / repeated same orientation notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；NumericUpDown orientation 未变时不写 |
| NumericUpDown validation status writes / repeated same form status | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；重复 Error / Warning / Default 不写 `Status` |
| PenUtils allocations / repeated immutable brush pen request | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 结构收益；同参数重复 render 复用已有 `ImmutablePen` |
| PenUtils allocations / repeated immutable dashed pen request | 2 objects | 0 objects | `(2 - 0) / 2` | 100.00% | 结构收益；同参数重复 render 不再创建 `ImmutableDashStyle` + `ImmutablePen` |
| OptionButtonGroup separator render pen allocations / repeated frame after first | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 结构收益；选项分隔线复用缓存 `IPen`，不声明页面加载 speedup |
| FloatButtonSeparatorLayer separator render pen allocations / repeated frame after first | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 结构收益；悬浮按钮分隔线复用缓存 `IPen`，不声明页面加载 speedup |
| ButtonSpinnerHandle divider render pen allocations / repeated frame after first | 2 calls creating pens | 0 new objects | `(2 - 0) / 2` | 100.00% | 结构收益；竖线/横线共用同参数缓存 `IPen`，不声明页面加载 speedup |
| CardActionPanel action separator render pen allocations / 3 actions repeated frame after first | 3 objects | 0 objects | `(3 - 0) / 3` | 100.00% | 结构收益；顶部线和 action 分隔线复用缓存 `IPen`，不声明页面加载 speedup |
| TextAreaDecoratedBox resize indicator render pen allocations / repeated frame after first | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 结构收益；resize 指示线复用缓存 `IPen`，不声明页面加载 speedup |
| CascaderViewFrame column separator render pen allocations / repeated frame after first | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 结构收益；列分隔线复用缓存 `IPen`，不声明页面加载 speedup |
| SliderThumb circle/outline render pen allocations / repeated frame after first | 2 objects | 0 objects | `(2 - 0) / 2` | 100.00% | 结构收益；thumb 圆和 outline 复用缓存 `IPen`，不声明页面加载 speedup |
| SliderTrack mark border render pen allocations / active + inactive marks repeated frame after first | 2 objects | 0 objects | `(2 - 0) / 2` | 100.00% | 结构收益；active / inactive mark border 各复用一个缓存 `IPen`，不声明页面加载 speedup |
| MenuSeparator line render pen allocations / repeated frame after first | 1 object | 0 objects | `(1 - 0) / 1` | 100.00% | 结构收益；按 render scaling 复用缓存 `IPen`，不声明页面加载 speedup |
| CircleProgress render pen allocations / normal repeated frame after first | 3 objects | 0 objects | `(3 - 0) / 3` | 100.00% | 结构收益；groove / indicator / success 三类 pen 复用，不声明页面加载 speedup |
| CircleProgress render pen allocations / step repeated frame after first | 3 objects | 0 objects | `(3 - 0) / 3` | 100.00% | 结构收益；step groove / indicator / success 三类 pen 复用，不声明页面加载 speedup |
| DashboardProgress render pen allocations / normal repeated frame after first | 3 objects | 0 objects | `(3 - 0) / 3` | 100.00% | 结构收益；dashboard groove / indicator / success 三类 pen 复用，不声明页面加载 speedup |
| DashboardProgress render pen allocations / step repeated frame after first | 3 objects | 0 objects | `(3 - 0) / 3` | 100.00% | 结构收益；dashboard step groove / indicator / success 三类 pen 复用，不声明页面加载 speedup |
| BorderRenderHelper dashed mode cache correctness / dash enabled toggle | stale mode risk | refreshed mode | `1 -> 0 stale cache risk` | 100.00% | 正确性收敛；dash 状态切换纳入 cache key |
| AddOnDecoratedBox icon brush descendant filter LINQ operators / container brush sync | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 结构收益；addon icon 刷色扫描改为显式 `Icon` first-class 分支，不声明页面加载 speedup |
| AbstractSelect compact-space usage writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；Select compact 状态未变时不写 |
| AbstractSelect compact-space position writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；Select position 未变时不写 |
| AbstractSelect compact-space orientation writes / repeated same orientation notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；Select orientation 未变时不写 |
| AbstractSelect validation status writes / repeated same form status | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；重复 Error / Warning / Default 不写 `Status` |
| AbstractSelect form feedback writes / repeated same feedback control | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；同一个 feedback control 不重复写 |
| RangeInfoPickerInput unchanged Y offset styled-property writes / repeated measure | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；RangeDatePicker / RangeTimePicker 重复 layout 少进一次 styled property 写入路径 |
| RangeInfoPickerInput unchanged indicator attached-property writes / repeated arrange | 2 writes | 0 writes | `(2 - 0) / 2` | 100.00% | 结构收益；`Canvas.Left` / `Canvas.Top` 位置没变时不再重复写入 |
| RangeInfoPickerInput unchanged active indicator setup writes / repeated setup | 3 writes | 0 writes | `(3 - 0) / 3` | 100.00% | 结构收益；active range 端重复 setup 时不再写 opacity、width、X offset |
| RangeInfoPickerInput unchanged indicator writes / repeated layout + setup | 6 writes | 0 writes | `(6 - 0) / 6` | 100.00% | 结构收益；不声明页面加载 speedup |
| RangeInfoPickerInput pointer position reads / secondary-box click | 2 reads | 1 read | `(2 - 1) / 2` | 50.00% | 结构收益；同一次 pointer release 复用坐标 |
| RangeInfoPickerInput left-addon coordinate translations / secondary-box click with left addon | 2 translations | 1 translation | `(2 - 1) / 2` | 50.00% | 结构收益；start / end hit-test 共用 left boundary |
| RangeInfoPickerInput repeated same-open writes / click already-open same side | 2 writes | 0 writes | `(2 - 0) / 2` | 100.00% | 结构收益；`RangeActivatedPart` 和 `IsPickerOpen` 未变化时不写 |
| RangeInfoPickerInput repeated close writes / click close while already closed | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；`IsPickerOpen=false` 未变化时不写 |
| RangeInfoPickerInput default start placement writes / first start activation | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；默认 `BottomEdgeAlignedLeft` 不重复写 |
| RangeInfoPickerInput repeated popup target writes / same target setup | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；同一 `PlacementTarget` 不重复写 |
| InfoPickerInput repeated open writes / click already-open input | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；DatePicker / TimePicker `IsPickerOpen=true` 未变化时不写 |
| InfoPickerInput repeated close writes / close already-closed picker | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；public close、window deactivated、cleanup 复用同一 guard |
| InfoPickerInput clear-button visibility writes / repeated same hover-text state | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；hover / readonly / text 长度结果未变时不写 |
| InfoPickerInput clear-button visibility writes / clear when already hidden | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；清空后 already hidden 不重复写 |
| InfoPickerInput arrow effective writes / unchanged placement or arrow visibility | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；arrow effective 结果未变时不写 |
| InfoPickerInput arrow position writes / unchanged popup flip or placement | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；arrow position 未变时不写 |
| InfoPickerInput validation status writes / repeated same form status | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；重复 Error / Warning / Default 不写 `Status` |
| InfoPickerInput compact-space usage writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；`IsUsedInCompactSpace` 结果未变时不写 |
| InfoPickerInput compact-space position writes / repeated same position notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；`CompactSpaceItemPosition` 未变时不写 |
| InfoPickerInput compact-space orientation writes / repeated same orientation notification | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；`CompactSpaceOrientation` 未变时不写 |
| InfoPickerInput form feedback writes / repeated same feedback control | 1 write | 0 writes | `(1 - 0) / 1` | 100.00% | 结构收益；同一个 feedback control 不重复写，避免下游重复订阅刷新 |
| InfoPickerInput hit-test branch count / pointer release | 1 extra branch | 0 extra branches | `(1 - 0) / 1` | 100.00% | 结构收益；bounds 命中结果直接返回 |
| DateTimeUtils month sweep enumerable allocations / widest datetime width calculation | 1 enumerable | 0 enumerables | `(1 - 0) / 1` | 100.00% | 结构收益；按需月份扫描改为 for loop |
| DateTimeUtils weekday sweep enumerable allocations / widest datetime width calculation | 1 enumerable | 0 enumerables | `(1 - 0) / 1` | 100.00% | 结构收益；按需星期扫描改为 for loop |
| DateTimeUtils hour sample array allocations / widest datetime width calculation | 1 array | 0 arrays | `(1 - 0) / 1` | 100.00% | 结构收益；AM/PM 样本小时改为索引分支 |
| MotionGhostControl shadow renderer list growth / build shadow renderers | dynamic capacity | exact capacity | structural | 分配更紧 | 阴影 renderer 数量已知时按 `BoxShadows.Count` 预分配 |
| CompactSpace add wrapper list growth / children add | dynamic capacity | exact capacity | structural | 分配更紧 | 按 `NewItems.Count` 预分配 wrapper 列表 |
| CompactSpace remove lookup set growth / children remove | dynamic capacity | exact capacity | structural | 分配更紧 | 按 `OldItems.Count` 预分配 lookup set |
| CompactSpace add item filter LINQ operators / children add | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 结构收益；wrapper 构造直接按 `NewItems.Count/indexer` 读取 |
| CompactSpace remove item filter LINQ operators / children remove | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 结构收益；lookup set 构造直接按 `OldItems.Count/indexer` 读取 |
| CompactSpace template wrapper list growth / apply template | dynamic capacity | exact capacity | structural | 分配更紧 | 按 `Children.Count` 预分配初始 wrapper 列表 |
| SliderTrack mark rect list growth / render with marks | dynamic capacity | exact capacity | structural | 分配更紧 | horizontal / vertical marks 列表按 `Marks.Count` 预分配 |
| SliderTrack mark text rect list growth / render with marks | dynamic capacity | exact capacity | structural | 分配更紧 | mark 文本命中区域列表按 `Marks.Count` 预分配 |
| ListBox virtualizing context dictionary growth / recycled item | dynamic capacity | exact capacity 1 | structural | 分配更紧 | 保存 1 个虚拟化状态键时避免 dictionary 增长 |
| ListView virtualizing context dictionary growth / recycled item | dynamic capacity | exact capacity 2 | structural | 分配更紧 | 保存 2 个虚拟化状态键时避免 dictionary 增长 |
| Cascader level virtualizing context dictionary growth / recycled item | dynamic capacity | exact capacity 5 | structural | 分配更紧 | 保存 5 个虚拟化状态键时避免 dictionary 增长 |
| ProgressBar size threshold dictionary growth / progress instance | dynamic capacity | exact capacity 3 | structural | 分配更紧 | line / circle progress 尺寸阈值表按 `SizeType` 三档预分配 |
| Tag preset color map growth / theme color map build | dynamic capacity | exact capacity 14 | structural | 分配更紧 | preset 颜色表按 `PresetColorType` 已知数量预分配 |
| Tag status color map growth / theme color map build | dynamic capacity | exact capacity 4 | structural | 分配更紧 | status 颜色表按 `TagStatus` 已知数量预分配 |
| Popup overlay layer manager fallback LINQ operators / lookup | 2 operators | 0 operators | `(2 - 0) / 2` | 100.00% | 结构收益；`GetPopupOverlayLayer()` TopLevel fallback 改为显式 first-match，不声明页面加载 speedup |
| ScopeAwareOverlayLayer direct child lookup LINQ operators / lookup | 1 operator | 0 operators | `(1 - 0) / 1` | 100.00% | 结构收益；已有 overlay layer first-match 查找改为显式遍历 |
| `DatePicker.Selected.Closed` KB/item | 627.9 KB | 627.5 KB | `(627.9 - 627.5) / 627.9` | 0.06% | smoke-only；分配小幅下降，不作为页面收益证明 |
| `DatePicker.Default.PresenterOnly` ms/item | 17.582 ms | 16.919 ms | `(17.582 - 16.919) / 17.582` | 3.77% | smoke-only；单次 timing 只作异常检查 |
| `DatePicker.Default.Closed` ms/item | 3.841 ms | 4.299 ms | `(3.841 - 4.299) / 3.841` | -11.92% | smoke-only；单次 timing 回退，不作为本轮收益证明 |
| `LineEdit.Default` KB/item | 406.2 KB | 402.0 KB | `(406.2 - 402.0) / 406.2` | 1.03% | smoke-only；单次 addon suite 分配小幅下降 |
| `CompactSpace.LineEdit.Horizontal` KB/item | 1334.7 KB | 1321.8 KB | `(1334.7 - 1321.8) / 1334.7` | 0.97% | smoke-only；单次 addon suite 分配小幅下降 |
| `LineEdit.AllowClear` repeated ms/item | 3.738 ms | 3.717 ms median / 3.685 ms mean | `(3.738 - 3.717) / 3.738` | 0.56% | smoke-only 10-run retest；未复现 -23.73%，波动范围 2.947-4.573 ms，不作为本轮收益证明 |
| `LineEdit.AllowClear` KB/item | 490.6 KB | 486.3 KB | `(490.6 - 486.3) / 490.6` | 0.88% | smoke-only；10 次复测稳定为 486.3 KB/item |

## Verification

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-addon-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-datepicker-states --verify-timepicker-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-listbox-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-switch-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite datepicker --count 80
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite addon --count 80
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite listbox --count 80
```

Notes:

- `LineEdit.AllowClear` 的 `4.625 ms/item` 是单次 smoke timing 异常点；同一优化版本 `--suite addon --count 80` 重复 10 次结果为 2.947 / 3.084 / 3.477 / 3.546 / 3.604 / 3.829 / 3.864 / 3.952 / 3.971 / 4.573 ms/item，median 3.717 ms/item，mean 3.685 ms/item。该场景 timing 只能用于异常排查，不能作为本轮收益证明。
- `PART_ClearButton` 是静态模板节点，隐藏态通过 `IsVisible=false` 退出布局/渲染；本轮没有把模板元素迁到 C# 动态创建。
- `PART_ContentFrame` 仍是静态模板节点；本轮没有按需创建 / 销毁 ContentFrame，只把 pointer 状态逻辑移到该静态节点自己的 override。
- `PART_ContentFrame` 的行为子类必须继续以 `Border` 作为 StyleKey；普通 `Border#PART_ContentFrame` selector 是精确 StyleKey 匹配，丢失 StyleKey 会导致 size token padding 不生效。
- `CandidateList` 本轮是交互结构优化，不改变 closed popup / page-load visual tree；`CandidateList.Default.Items20` 单次 smoke 为 `2.097 ms/item`、`473.0 KB/item`，只作异常检查，不作为速度收益证明。
- `RangeInfoPickerInput` 本轮是 repeated layout 写入路径优化；未新增 Gallery timing 对比，不声明页面加载速度提升。
- `DatePicker` verifier 的旧动态 slot 断言已修正为当前静态模板口径。
- `Select` / `Cascader` 全量 state verifier 仍含若干旧动态 slot 断言，本轮只以 `--verify-addon-states` 覆盖 right-addon template 正确性。

## CandidateList 单选判断补充

通用 `CandidateList.IsSingleMode()` 原先用 `SelectionMode.HasFlag(Single) && !HasFlag(Multiple)`。由于 Avalonia `SelectionMode.Single = 0`，本轮改为直接判断“不包含 Multiple”，保持原单选语义并去掉 zero-value flag 的 `HasFlag()` 调用。

| 指标 | baseline | optimized | 公式 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| CandidateList single-mode `HasFlag` callsites / selection check | 2 | 0 | `(2 - 0) / 2` | 100.00% | structural-only；单选判断直接检查 Multiple bit |
| Candidate single/multiple semantics | unchanged | unchanged | n/a | 0.00% | 行为保持 |
| Page-load timing claim | none | none | n/a | n/a | 本轮没有有效前后 timing，不声明页面级速度收益 |
