# Primitives Performance Notes

## Scope

本轮处理两个 Primitives 共享路径：

- `InfoPickerInput` 体系里的 `PickerClearUpButton`。它被 `DatePicker`、`RangeDatePicker`、`TimePicker`、`RangeTimePicker` 等 picker 输入控件复用。
- `AddOnDecoratedBox` 的 `PART_ContentFrame` pointer 状态路径。它被 LineEdit / Select / TreeSelect / Cascader / DatePicker / TimePicker / ButtonSpinner 等输入类控件复用。
- `CandidateList` 的键盘候选高亮路径。它被 AutoComplete / Mentions 等 popup 候选列表复用。

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

## CandidateList Root Cause

- `CandidateList.TrySetCandidateItemSelected(index)` 原先每次候选高亮移动都会从 `0..ItemCount-1` 扫描全部容器，并对每个已实现 `CandidateListItem` 重写 `IsCandidateSelected`。
- 键盘上下导航只需要清理旧候选项并设置新候选项；全量扫描在 AutoComplete / Mentions popup 打开后会按按键次数线性放大。
- `CandidateVirtualizingStackPanel.ScrollCandidateItemIntoView(index)` 已经能把目标项滚入视图，本轮只复用目标容器，不改变虚拟化、过滤、提交 / 取消语义。

## Changes

- `PickerClearUpButton` 改为 `Button.ClickEvent.AddClassHandler<PickerClearUpButton>()`。
- 移除 `OnApplyTemplate()` 中的 `_clearButton.Click += ...` lambda。
- `IsInClearMode=false` 时直接 `ClickEvent` 不再触发 `ClearRequest`。
- `AddOnDecoratedBox` 模板里的静态 `PART_ContentFrame` 从 `Border` 换成 `AddOnDecoratedBoxContentFrame : Border`，由子类 override pointer 事件回写 `IsInnerBoxHover` / `IsInnerBoxPressed`。
- `AddOnDecoratedBoxContentFrame` 保持 `StyleKeyOverride => typeof(Border)`，确保原有 `Border#PART_ContentFrame` size/padding selector 继续命中。
- 移除 `AddOnDecoratedBox.OnApplyTemplate()` 对 `ContentFrame` 的 4 条本地 pointer handler 注册 / 退订。
- 修复 Select / TreeSelect / Cascader 的 right addon presenter template binding：`ContentLeftAddOnTemplate` -> `ContentRightAddOnTemplate`。
- `CandidateList.TrySetCandidateItemSelected(index)` 改为只清旧 index、设置新 index，不再每次移动扫描全部 `ItemCount`。
- `--verify-datepicker-states` 改为验证静态 popup shell / accessory slot：模板 shell 保留，但 presenter/content 在关闭态为空；detach 后 presenter/content 释放。
- `--verify-datepicker-states` 新增 clear button lifecycle 覆盖：隐藏 direct click 惰性、本地 Click handler 为 0、clear mode 下仍能清空 `SelectedDateTime`。
- `--verify-addon-states` 新增 ContentFrame pointer lifecycle 覆盖：本地 pointer routed handler 为 0，直接事件仍正确更新 hover / pressed 状态；同时覆盖 Select / TreeSelect / Cascader right-addon template。
- `--verify-addon-states` 覆盖 `PART_ContentFrame` 的 `StyleKey == Border` 和 token padding 非零，避免 DatePicker / TimePicker / LineEdit 输入框内边距回归。
- `--verify-listbox-states` 新增 CandidateList 候选高亮移动覆盖：index 0 -> 1 -> -1 时只保留当前候选态；同时修正 ListBox 静态 slot 校验口径。

## Benefit

这轮收益是结构和正确性收益：picker 输入控件每个 `PickerClearUpButton` 少 1 条本地 Click 订阅；每个 `AddOnDecoratedBox` 少 4 条本地 pointer 订阅；隐藏 clear button 不再能被直接事件误触发；Select / TreeSelect / Cascader 的 right-addon template 绑定恢复正确。页面构建 timing 未作为本轮收益证明。

| Metric | Baseline | Optimized | Formula | Improvement | Conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Local Click handlers / `PickerClearUpButton` | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；改由 class handler 统一处理 |
| Hidden clear direct-click side effects / click | 1 clear request | 0 clear requests | `(1 - 0) / 1` | 100.00% | 正确性修复；非 clear mode 不再清值 |
| Local pointer handlers / `AddOnDecoratedBox` content frame | 4 / instance | 0 / instance | `(4 - 0) / 4` | 100.00% | 有效；静态 template part 保留，pointer 状态改由 `Border` 子类 override |
| Local pointer handlers / `CompactSpace.LineEdit` root | 12 / root | 0 / root | `(12 - 0) / 12` | 100.00% | 有效；3 个 AddOnDecoratedBox × 4 条 handler |
| Wrong right-addon template bindings / Select family | 3 controls | 0 controls | `(3 - 0) / 3` | 100.00% | 正确性修复；Select / TreeSelect / Cascader 均已验证 |
| Lost input content padding after ContentFrame subclassing | 1 shared path | 0 shared paths | `(1 - 0) / 1` | 100.00% | 正确性修复；`PART_ContentFrame` 继续命中 `Border#PART_ContentFrame` padding selector |
| CandidateList candidate container checks / keyboard move (`Items20`) | 20 checks | <= 2 checks | `(20 - 2) / 20` | 90.00% | 结构收益；AutoComplete / Mentions 候选 popup 键盘上下移动少扫容器，不声明页面加载 speedup |
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
- `DatePicker` verifier 的旧动态 slot 断言已修正为当前静态模板口径。
- `Select` / `Cascader` 全量 state verifier 仍含若干旧动态 slot 断言，本轮只以 `--verify-addon-states` 覆盖 right-addon template 正确性。
