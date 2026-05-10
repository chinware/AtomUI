# API 设计一致性分析报告

> 扫描时间：2026-05-10（修订）
> 扫描范围：AtomUI.slnx 中所有项目
> 分析维度：属性命名、方法/重写模式、枚举/类型、AXAML 模板、伪类与 Part 常量

---

## P0 — 拼写错误 / 命名错误

### 1. `ChoosingStatue` → `ChoosingStatus`（21+ 处，覆盖 7 个文件）

单词拼写错误：`Statue`（雕像）应为 `Status`（状态）。

**事件定义：**

| 文件 | 当前命名 |
|---|---|
| `src/AtomUI.Desktop.Controls/TimePicker/TimePickerPresenter.cs` | `ChoosingStatueChanged`（事件，多处） |
| `src/AtomUI.Desktop.Controls/DatePicker/DatePickerPresenter.cs` | `ChoosingStatueChanged`（事件）+ `EmitChoosingStatueChanged`（方法） |

**事件订阅 / 处理方法：**

| 文件 | 当前命名 |
|---|---|
| `src/AtomUI.Desktop.Controls/TimePicker/TimePicker.cs` | `HandleChoosingStatueChanged` |
| `src/AtomUI.Desktop.Controls/TimePicker/RangeTimePicker.cs` | `HandleChoosingStatueChanged` |
| `src/AtomUI.Desktop.Controls/DatePicker/DatePicker.cs` | `HandleChoosingStatueChanged` |
| `src/AtomUI.Desktop.Controls/DatePicker/RangeDatePicker.cs` | `HandleChoosingStatueChanged` |
| `src/AtomUI.Desktop.Controls/DatePicker/RangeDatePickerPresenter.cs` | `EmitChoosingStatueChanged` 调用 |

**修复方案：** 全局重命名为 `ChoosingStatusChanged` / `HandleChoosingStatusChanged` / `EmitChoosingStatusChanged`。属于公共 API 破坏性变更，需发 breaking change 说明。

---

### 2. `TimeLine` vs `Timeline` 大小写不一致

枚举用 `TimeLine`（大写 L），但控件类、Token 类、目录名都用 `Timeline`（小写 l）。

| 位置 | 当前命名 |
|---|---|
| `src/AtomUI.Controls/Timeline/TimeLineEnums.cs` | `TimeLineMode`（枚举） |
| `src/AtomUI.Controls/Timeline/Timeline.cs` | `Timeline`（控件类） |
| `src/AtomUI.Controls/Timeline/TimelineToken.cs` | `TimelineToken`（Token 类） |
| 目录名 | `Timeline/` |

**修复方案：** 将枚举重命名为 `TimelineMode`，文件重命名为 `TimelineEnums.cs`。属于公共 API 破坏性变更。

---

> **说明：** 原报告中"重复 ToolTip 目录"条目已失效 —— 当前仅存 `src/AtomUI.Desktop.Controls/Tooltip/`，无重复。

---

## P1 — 布尔属性缺少 `Is` 前缀（共 22 处）

Avalonia/WPF 规范要求布尔属性使用 `Is`、`Has`、`Can`、`Should` 前缀。

### 1.1 裸动词 / 裸名词布尔属性（15 处）

| 文件 | 行号 | 当前命名 | 建议命名 |
|---|---|---|---|
| `src/AtomUI.Controls/ProgressBar/AbstractProgressBar.cs` | 197 | `PercentLabelVisibleProperty` | `IsPercentLabelVisibleProperty` |
| `src/AtomUI.Controls/ProgressBar/AbstractProgressBar.cs` | 200 | `StatusIconVisibleProperty` | `IsStatusIconVisibleProperty` |
| `src/AtomUI.Controls/Watermark/Glyphs/WatermarkGlyph.cs` | 68 | `UseMirrorProperty` | `IsMirrorUsedProperty` |
| `src/AtomUI.Controls/Watermark/Glyphs/WatermarkGlyph.cs` | 77 | `UseCrossProperty` | `IsCrossUsedProperty` |
| `src/AtomUI.Controls/Grid/Row.cs` | 20 | `WrapProperty` | `IsWrappedProperty` |
| `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinner.cs` | 31 | `AllowSpinProperty` | `IsSpinEnabledProperty` |
| `src/AtomUI.Desktop.Controls/ButtonSpinner/ButtonSpinner.cs` | 34 | `ShowButtonSpinnerProperty` | `IsButtonSpinnerVisibleProperty` |
| `src/AtomUI.Desktop.Controls/Drawer/Drawer.cs` | 45 | `CloseWhenClickOnMaskProperty` | `IsCloseOnMaskClickProperty` |
| `src/AtomUI.Desktop.Controls/TimePicker/TimePickerPresenter.cs` | 73 | `ButtonsPanelVisibleProperty` | `IsButtonsPanelVisibleProperty` |
| `src/AtomUI.Desktop.Controls/Form/Form.cs` | 94 | `ScrollToFirstErrorProperty` | `IsScrollToFirstErrorEnabledProperty` |
| `src/AtomUI.Desktop.Controls/Card/Card.cs` | 157 | `ActionsPanelVisibleProperty` | `IsActionsPanelVisibleProperty` |
| `src/AtomUI.Desktop.Controls/Dialog/Dialog.cs` | 75 | `TopmostProperty` | `IsTopmostProperty` |
| `src/AtomUI.Desktop.Controls/Slider/SliderTrack.cs` | 59 | `IncludedProperty` | `IsIncludedProperty` |
| `src/AtomUI.Desktop.Controls/Transfer/AbstractTransfer.cs` | 312 | `ToTargetButtonEnabledProperty` | `IsToTargetButtonEnabledProperty` |
| `src/AtomUI.Desktop.Controls/Transfer/AbstractTransfer.cs` | 317 | `ToSourceButtonEnabledProperty` | `IsToSourceButtonEnabledProperty` |

### 1.2 `IsShow*` 反模式 → `Is*Visible`（10 处）

Avalonia 惯例：可见性用 `Is{Subject}Visible` 而非 `IsShow{Subject}`。

| 文件 | 当前命名 | 建议命名 |
|---|---|---|
| `src/AtomUI.Controls/Empty/AbstractEmpty.cs` | `IsShowDescriptionProperty` | `IsDescriptionVisibleProperty` |
| `src/AtomUI.Controls/ProgressBar/AbstractProgressBar.cs` | `IsShowProgressInfoProperty` | `IsProgressInfoVisibleProperty` |
| `src/AtomUI.Controls/Spin/AbstractSpin.cs` | `IsShowTipProperty` | `IsTipVisibleProperty` |
| `src/AtomUI.Controls/Primitives/ArrowDecoratedBox/AbstractArrowDecoratedBox.cs` | `IsShowArrowProperty` | `IsArrowVisibleProperty` |
| `src/AtomUI.Controls/Badge/AbstractCountBadge.cs` | `IsShowZeroProperty` | `IsZeroVisibleProperty` |
| `src/AtomUI.Desktop.Controls.ColorPicker/AbstractColorPicker.cs` | `IsShowArrowProperty` | `IsArrowVisibleProperty` |
| `src/AtomUI.Desktop.Controls.ColorPicker/AbstractColorPicker.cs` | `IsShowTextProperty` | `IsTextVisibleProperty` |
| `src/AtomUI.Desktop.Controls.DataGrid/DataGrid.cs` | `IsShowFrameBorderProperty` | `IsFrameBorderVisibleProperty` |
| `src/AtomUI.Desktop.Controls.DataGrid/DataGrid.cs` | `ShowSorterTooltipProperty` | `IsSorterTooltipVisibleProperty` |
| `src/AtomUI.Desktop.Controls/Input/SearchEditDecoratedBox.cs:18` | `SearchButtonLoadingProperty` | `IsSearchButtonLoadingProperty` |

### 1.3 动词前缀缺 `Is`（7 处）

| 文件 | 行号 | 当前命名 | 建议命名 |
|---|---|---|---|
| `src/AtomUI.Desktop.Controls.ColorPicker/ColorSlider/AbstractColorPickerSliderTrack.cs` | 45 | `IgnoreThumbDragProperty` | `IsThumbDragIgnoredProperty` |
| `src/AtomUI.Desktop.Controls.ColorPicker/ColorSlider/AbstractColorPickerSliderTrack.cs` | 48 | `DeferThumbDragProperty` | `IsThumbDragDeferredProperty` |
| `src/AtomUI.Desktop.Controls.DataGrid/Column/Filters/DataGridFilterIndicator.cs` | 18 | `FilterMultipleProperty` | `IsMultipleFilterEnabledProperty` |

**总体修复策略：** 所有都是公共 API 破坏性变更，建议随下一个大版本批量处理，提供 `[Obsolete]` 过渡别名 1~2 个版本。

---

## P2 — 方法/重写模式不一致

### 2.1 `Notify*` vs `On*` vs `Handle*` 混用

主流模式是 `Notify*`（60+ 处），但存在例外：

| 文件 | 行号 | 当前命名 | 建议 |
|---|---|---|---|
| `src/AtomUI.Controls/Primitives/Thumb.cs` | 59-67 | `OnDragStarted` / `OnDragDelta` / `OnDragCompleted` | 保留（WPF/Avalonia 标准） |
| `src/AtomUI.Controls/Badge/AbstractCountBadge.cs` | 197 | `HandleDecoratedTargetChanged` | `NotifyDecoratedTargetChanged` |
| `src/AtomUI.Controls/Badge/AbstractDotBadge.cs` | 178 | `HandleDecoratedTargetChanged` | `NotifyDecoratedTargetChanged` |

### 2.2 事件触发方法不一致：`Emit*` vs 直接 `Invoke`

少数 Presenter 通过 `EmitXxxChanged()` 方法触发事件（`DatePickerPresenter` 中的 `EmitChoosingStatueChanged`），其他 Presenter 直接内联 `XxxChanged?.Invoke(...)`。两种风格并存且没有统一规则。

**建议：** 选一种风格统一（推荐内部触发封装为 `RaiseXxxChanged` 或 `OnXxxChanged` 虚方法，便于子类钩入）。

### 2.3 `CreatePickerPresenter` vs `CreatePresenter`

同一概念（创建弹出层 Presenter）有两种命名：

| 文件 | 方法名 |
|---|---|
| `src/AtomUI.Desktop.Controls.ColorPicker/AbstractColorPicker.cs:981` | `CreatePickerPresenter()` |
| `src/AtomUI.Desktop.Controls.DataGrid/Column/Filters/DataGridTreeFilterFlyout.cs:30` | `CreatePresenter()` |
| `src/AtomUI.Desktop.Controls.DataGrid/Column/Filters/DataGridMenuFilterFlyout.cs:20` | `CreatePresenter()` |

**建议：** 统一为 `CreatePresenter()`。

### 2.4 `abstract` 与 `Notify*` 命名语义冲突

`Notify*` 前缀暗示可选重写的钩子（virtual），但以下方法是 `abstract`（强制重写）：

| 文件 | 行号 | 方法 |
|---|---|---|
| `src/AtomUI.Controls/FloatButton/AbstractFloatButtonHost.cs` | 216 | `protected abstract NotifyCreateFloatButton()` |
| `src/AtomUI.Controls/Form/Validators/AbstractFormValidator.cs` | 23 | `protected abstract NotifyValidateAsync()` |

**建议：** `Create*` 用于强制重写（abstract），`Notify*` 用于可选重写（virtual）。

### 2.5 Dispose/Cleanup 模式不统一

清理逻辑散布在不同位置：
- 部分控件在 `OnApplyTemplate` 中手工解/绑
- 部分控件在 `DetachedFromVisualTree` 事件中清理
- 部分控件使用 `CompositeDisposable`（如 `AbstractFloatButtonHost`、`IconPresenter`）
- 没有控件在自身实现 `IDisposable`
- `CancellationTokenSource` 内联释放，无统一模式

**建议：** 在共享基类中引入 `protected virtual void DisposeManagedResources()` 或统一文档约定。

---

## P3 — 枚举/类型碎片化

### 3.1 Status 枚举碎片化（7 个独立枚举）

| 枚举 | 文件 | 成员 |
|---|---|---|
| `InputControlStatus` | `AtomUI.Controls.Shared/InputControlStatus.cs` | Default, Warning, Error |
| `TagStatus` | `AtomUI.Controls/Tag/TagEnums.cs` | Success, Info, Error, Warning |
| `DotBadgeStatus` | `AtomUI.Controls/Badge/AbstractDotBadge.cs` | Default, Success, Processing, Error, Warning |
| `ProgressStatus` | `AtomUI.Controls/ProgressBar/ProgressBarEnums.cs` | Normal, Success, Exception, Active |
| `ResultStatus` | `AtomUI.Controls/Result/ResultStatus.cs` | Info, Success, Error, Warning, ErrorCode404, ErrorCode403, ErrorCode500 |
| `QRCodeStatus` | `AtomUI.Controls/QRCode/QRCodeEnums.cs` | Active, Expired, Loading, Scanned |
| `StepsItemStatus` | `AtomUI.Desktop.Controls/Steps/Steps.cs` | Wait, Process, Finish, Error |

**语义碎片：**

| 概念 | 分散为 |
|---|---|
| 成功 | Success / Normal / Finish / Active |
| 错误 | Error / Exception |
| 等待 | Wait / Processing / Loading / Active |

**建议：** 提取公共 `ControlStatus` 基础枚举（Default, Success, Warning, Error, Info, Processing），特殊状态由各控件扩展。

### 3.2 Placement 枚举碎片化（6 个独立枚举）

| 枚举 | 成员 |
|---|---|
| `FloatButtonPlacement` | Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight, Center |
| `DrawerPlacement` | Left, Top, Right, Bottom |
| `AutoCompletePlacementMode` | Top, Bottom |
| `MentionsPlacementMode` | Top, Bottom |
| `TourPlacementMode` | Center, Left, LeftTop, LeftBottom, Right, RightTop, RightBottom, Top, TopLeft, TopRight, Bottom, BottomLeft, BottomRight |
| `SelectPopupPlacement` | TopEdgeAlignedLeft, TopEdgeAlignedRight, BottomEdgeAlignedLeft, BottomEdgeAlignedRight |

**命名不一致：** `Placement` vs `PlacementMode` vs `PopupPlacement`。

**建议：** Top/Bottom 类用 `PopupPlacementMode`；8 方位 + Center 用 `Placement`（可复用 `Avalonia.Controls.PlacementMode`）。

### 3.3 `CountBadgeSize` 应复用共享 `SizeType`

| 枚举 | 成员 |
|---|---|
| `CountBadgeSize`（`AbstractCountBadge.cs`） | Default, Small |
| `SizeType`（`AtomUI.Core/Common.cs`） | Large, Middle, Small |

`CountBadgeSize` 完全可以复用 `SizeType`。

### 3.4 Token 类访问修饰符不一致

| Token 类 | 访问修饰符 |
|---|---|
| `ColorPickerToken`（`src/AtomUI.Desktop.Controls.ColorPicker/ColorPickerToken.cs`） | `public` |
| 其他 80+ Token 类 | `internal` |

**建议：** 若无外部消费需要，统一为 `internal`；若必须公开，将 80+ 其余 Token 也公开以保持一致。需要先确认设计意图。

### 3.5 `DataGridSortDirections` 复数命名

`[Flags]` 枚举使用复数 `DataGridSortDirections`，但其他 `[Flags]` 枚举（`DataGridGridLinesVisibility`、`DataGridHeadersVisibility`、`DataGridPaginationVisibility`）使用单数。

**文件：** `src/AtomUI.Desktop.Controls.DataGrid/DataGridEnumerations.cs:90`

**建议：** 重命名为 `DataGridSortDirection` 保持单数统一。

---

## P4 — AXAML 模板 / 伪类 / Part 常量不一致

### 4.1 PART_ 前缀缺失

主流控件全部使用 `PART_` 前缀（577+ 处），但以下控件使用裸名：

**ColorPicker 主题：**

| 文件 | 当前命名 |
|---|---|
| `ColorPicker/Themes/ColorBlockTheme.axaml` | `ColorPreviewFrame`, `ColorPreview`, `EmptyColorFrame` |
| `ColorPicker/Themes/ColorPickerTheme.axaml` | `Frame`, `ColorText` |
| `ColorPicker/Themes/GradientColorPickerTheme.axaml` | `Frame`, `ColorText` |
| `ColorPicker/Themes/ColorView/ColorPickerInput.axaml` | `RootLayout`, `HsvValuePanel`, `RgbValuePanel` |
| `ColorPicker/Themes/ColorView/ColorSpectrumTheme.axaml` | `Frame` |

**DataGrid 主题：**

| 文件 | 当前命名 |
|---|---|
| `DataGrid/Themes/DataGridColumnHeaderTheme.axaml` | `Frame`, `ContentFrame`, `RootLayout`, `VerticalSeparator`, `FocusVisual` |
| `DataGrid/Themes/DataGridRowTheme.axaml` | `RowBorder`, `FocusVisual` |

**其他：**

| 文件 | 当前命名 |
|---|---|
| `FloatButton/Themes/FloatButtonItemsControlTheme.axaml` | `ItemsLayout` |

**建议：** 全部加上 `PART_` 前缀。

### 4.2 Part 常量覆盖率不足

全解决方案 137 个文件直接出现 `"PART_..."` 字面量，但只有 75 处定义为 `const string * = "PART_*"`。绝大多数控件仍硬编码 part 名（例如 `Input/SearchEditDecoratedBox.cs` 的 `e.NameScope.Find<Button>("PART_RightAddOn")`），没有集中到 `*ThemeConstants` 类。

**建议：** 为每个控件定义 `XxxThemeConstants`（或扩展现有的 `AddOnDecoratedBoxThemeConstants`），所有 Part 名通过常量引用。便于重命名和静态分析。

### 4.3 内联伪类字符串（绕过常量约定）

代码库里大多数伪类已集中在 `XxxPseudoClass` 常量类（如 `AddOnDecoratedBoxPseudoClass.Outline`），但以下两处直接写字面量：

| 文件 | 行号 | 字面量 |
|---|---|---|
| `src/AtomUI.Desktop.Controls.ColorPicker/AbstractColorPicker.cs` | 998 | `PseudoClasses.Set(":flyout-open", ...)` |
| `src/AtomUI.Desktop.Controls/Tooltip/ToolTip.cs` | 466 | `PseudoClasses.Set(":open", ...)` |

**建议：** 提取为 `ColorPickerPseudoClass.FlyoutOpen`、`ToolTipPseudoClass.Open`。

### 4.4 `:active` vs `:pressed` 伪类不一致

| 文件 | 行号 | 伪类 |
|---|---|---|
| `ColorPicker/Themes/ColorSlider/ColorSliderThumbTheme.axaml` | 39 | `^:focus,^:active` |
| 其他所有主题 | — | `^:pressed` |

**建议：** 统一使用 `:pressed`（Avalonia 标准伪类）。

### 4.5 `Name=` vs `x:Name=` 混用

| 用法 | 数量 |
|---|---|
| `Name="PART_*"` | 566 处 |
| `x:Name="PART_*"` | 11 处 |

**建议：** 统一使用 `Name=`（Avalonia 原生属性）。

### 4.6 字符串 Key 的 ControlTheme（5 处）

以下主题使用字符串 Key 而非类型 Key：

| 文件 | Key |
|---|---|
| `Menu/Themes/TopLevelMenuItemTheme.axaml` | `x:Key="TopLevelMenuItemTheme"` |
| `TabControl/Themes/TabItemTheme.axaml` | `x:Key="TabItemTheme"` |
| `TabControl/Themes/CardTabItemTheme.axaml` | `x:Key="CardTabItemTheme"` |
| `TabControl/Themes/TabStrip/TabStripItemTheme.axaml` | `x:Key="TabStripItemTheme"` |
| `TabControl/Themes/TabStrip/CardTabStripItemTheme.axaml` | `x:Key="CardTabStripItemTheme"` |

这些都是同一控件类型的变体主题，使用字符串 Key 可能是有意设计（主题切换）。**需先确认设计意图**再决定是否迁移为类型 Key。

---

## 总结

| 优先级 | 类别 | 数量 | 是否 Breaking |
|---|---|---|---|
| P0 | 拼写错误 | 2 处（21+ 引用 / 4 文件 + 1 枚举） | 是（公共 API） |
| P1 | 布尔属性命名 | 22 处（1.1=15 + 1.2=10 + 1.3=3，去重） | 是（公共 API） |
| P2 | 方法/模式不一致 | 5 类 | 部分 |
| P3 | 枚举/类型碎片化 | 5 类 | 是（架构层面） |
| P4 | AXAML/伪类/Part 常量 | 6 类 | 否（内部实现） |

**建议处理顺序：**

1. **P0 拼写错误** — 影响最小、收益明显，优先修复，为 P1 批量重命名做参考样例
2. **P4.3 内联伪类字符串** 与 **P4.2 Part 常量提取** — 只是内部重构，不破坏 API，顺手能做
3. **P4.1/4.4/4.5** AXAML 统一 — 内部主题，风险可控
4. **P1 布尔属性** — Breaking，需配合版本发布并提供 `[Obsolete]` 别名
5. **P2 方法命名** — 逐步统一，可与 P1 同批发布
6. **P3 枚举碎片化** — 架构重构，影响面最大，需专门设计评审

---

## 变更日志

- **2026-05-10 修订：**
  - 删除"重复 ToolTip 目录"条目（已修复，当前仅存 `Tooltip/`）
  - P0-1 受影响文件从 4 个扩充为 7 个，补充 `EmitChoosingStatueChanged` 方法
  - P1 从 14 处扩充为 22 处，拆分为 1.1/1.2/1.3 三组
  - P2 新增 2.2 "事件触发方法 `Emit*` vs `Invoke`" 条目
  - P4 拆分并新增 4.2 "Part 常量覆盖率不足"、4.3 "内联伪类字符串"两条
