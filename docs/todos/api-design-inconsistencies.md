# API 设计一致性分析报告

> 扫描时间：2026-05-10
> 扫描范围：AtomUI.slnx 中所有项目
> 分析维度：属性命名、方法/重写模式、枚举/类型、AXAML 模板

---

## P0 — 拼写错误 / 命名错误

### 1. `ChoosingStatueChanged` → `ChoosingStatusChanged`

单词拼写错误：`Statue`（雕像）应为 `Status`（状态）。

**涉及文件（事件定义）：**

| 文件 | 行号 | 当前命名 |
|---|---|---|
| `src/AtomUI.Desktop.Controls/TimePicker/TimePickerPresenter.cs` | 129 | `ChoosingStatueChanged` |
| `src/AtomUI.Desktop.Controls/DatePicker/DatePickerPresenter.cs` | 119 | `ChoosingStatueChanged` |

**涉及文件（事件订阅/处理方法）：**

| 文件 | 行号 | 当前命名 |
|---|---|---|
| `src/AtomUI.Desktop.Controls/TimePicker/TimePicker.cs` | 159, 170 | `HandleChoosingStatueChanged` |
| `src/AtomUI.Desktop.Controls/TimePicker/RangeTimePicker.cs` | 209, 220 | `HandleChoosingStatueChanged` |
| `src/AtomUI.Desktop.Controls/DatePicker/DatePicker.cs` | 183, 194 | `HandleChoosingStatueChanged` |
| `src/AtomUI.Desktop.Controls/DatePicker/RangeDatePicker.cs` | 178, 201 | `HandleChoosingStatueChanged` |

**修复方案：** 全局重命名为 `ChoosingStatusChanged` / `HandleChoosingStatusChanged`。属于公共 API 破坏性变更，需发 breaking change 说明。

---

### 2. `TimeLine` vs `Timeline` 大小写不一致

枚举用 `TimeLine`（大写 L），但控件类、Token 类、目录名都用 `Timeline`（小写 l）。

| 位置 | 当前命名 |
|---|---|
| `src/AtomUI.Controls/Timeline/TimeLineEnums.cs` | `TimeLineMode`（枚举） |
| `src/AtomUI.Controls/Timeline/Timeline.cs` | `Timeline`（控件类） |
| `src/AtomUI.Controls/Timeline/TimelineToken.cs` | `TimelineToken`（Token 类） |
| 目录名 | `Timeline/` |

**修复方案：** 将枚举重命名为 `TimelineMode`。属于公共 API 破坏性变更。

---

### 3. 重复的 ToolTip 目录

存在两个目录，包含同名文件：

| 目录 | 大小写 |
|---|---|
| `src/AtomUI.Desktop.Controls/ToolTip/` | PascalCase "Tip" |
| `src/AtomUI.Desktop.Controls/Tooltip/` | lowercase "tip" |

两个目录都包含 `ToolTip.cs`、`ToolTipService.cs`、`ToolTipToken.cs` 和 `Themes/` 子目录。

**修复方案：** 合并为一个目录，保留 `Tooltip/`（与 Avalonia 一致）。

---

## P1 — 布尔属性缺少 `Is` 前缀（14 处）

Avalonia/WPF 规范要求布尔属性使用 `Is`、`Has`、`Can`、`Should` 前缀。以下属性不符合规范：

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

**额外说明：** `IsShow*` 模式（如 `IsShowArrow`、`IsShowDescription`）虽然能工作，但在 Avalonia 惯例中更自然的写法是 `IsArrowVisible`、`IsDescriptionVisible`。这属于风格偏好，影响范围更广，可单独评估。

---

## P2 — 方法/重写模式不一致

### 1. `Notify*` vs `On*` vs `Handle*` 混用

主流模式是 `Notify*`（60+ 处），但存在例外：

| 文件 | 行号 | 当前命名 | 建议 |
|---|---|---|---|
| `src/AtomUI.Controls/Primitives/Thumb.cs` | 59-67 | `OnDragStarted` / `OnDragDelta` / `OnDragCompleted` | `NotifyDragStarted` 等 |
| `src/AtomUI.Controls/Badge/AbstractCountBadge.cs` | 197 | `HandleDecoratedTargetChanged` | `NotifyDecoratedTargetChanged` |
| `src/AtomUI.Controls/Badge/AbstractDotBadge.cs` | 178 | `HandleDecoratedTargetChanged` | `NotifyDecoratedTargetChanged` |

> 注：`Thumb` 的 `On*` 命名是 WPF/Avalonia 标准模式，可能有意保留。需确认。

### 2. `CreatePickerPresenter` vs `CreatePresenter`

同一概念（创建弹出层 Presenter）有两种命名：

| 文件 | 方法名 |
|---|---|
| `src/AtomUI.Desktop.Controls.ColorPicker/AbstractColorPicker.cs:981` | `CreatePickerPresenter()` |
| `src/AtomUI.Desktop.Controls.DataGrid/Column/Filters/DataGridTreeFilterFlyout.cs:30` | `CreatePresenter()` |
| `src/AtomUI.Desktop.Controls.DataGrid/Column/Filters/DataGridMenuFilterFlyout.cs:20` | `CreatePresenter()` |

**建议：** 统一为 `CreatePresenter()`（更通用）。

### 3. `abstract` 与 `Notify*` 命名语义冲突

`Notify*` 前缀暗示可选重写的钩子（virtual），但以下方法是 `abstract`（强制重写）：

| 文件 | 行号 | 方法 |
|---|---|---|
| `src/AtomUI.Controls/FloatButton/AbstractFloatButtonHost.cs` | 216 | `protected abstract NotifyCreateFloatButton()` |
| `src/AtomUI.Controls/Form/Validators/AbstractFormValidator.cs` | 23 | `protected abstract NotifyValidateAsync()` |

**建议：** `Create*` 用于强制重写（abstract），`Notify*` 用于可选重写（virtual）。

### 4. Dispose/Cleanup 模式不统一

清理逻辑散布在不同位置：
- 部分控件在 `OnApplyTemplate` 中替换旧订阅
- 部分控件在 `DetachedFromVisualTree` 事件中清理
- 部分控件使用 `CompositeDisposable`（如 `AbstractFloatButtonHost`）
- 没有控件在自身实现 `IDisposable`
- `CancellationTokenSource` 内联释放，无统一模式

**建议：** 在共享基类中引入 `protected virtual void DisposeManagedResources()` 或统一文档约定。

---

## P3 — 枚举/类型碎片化

### 1. Status 枚举碎片化（7 个独立枚举）

| 枚举 | 文件 | 成员 |
|---|---|---|
| `InputControlStatus` | `AtomUI.Controls.Shared/InputControlStatus.cs` | Default, Warning, Error |
| `TagStatus` | `AtomUI.Controls/Tag/TagEnums.cs` | Success, Info, Error, Warning |
| `DotBadgeStatus` | `AtomUI.Controls/Badge/AbstractDotBadge.cs` | Default, Success, Processing, Error, Warning |
| `ProgressStatus` | `AtomUI.Controls/ProgressBar/ProgressBarEnums.cs` | Normal, Success, Exception, Active |
| `ResultStatus` | `AtomUI.Controls/Result/ResultStatus.cs` | Info, Success, Error, Warning, ErrorCode404, ErrorCode403, ErrorCode500 |
| `QRCodeStatus` | `AtomUI.Controls/QRCode/QRCodeEnums.cs` | Active, Expired, Loading, Scanned |
| `StepsItemStatus` | `AtomUI.Desktop.Controls/Steps/Steps.cs` | Wait, Process, Finish, Error |

**语义碎片化：**

| 概念 | 分散为 |
|---|---|
| 成功 | Success / Normal / Finish / Active |
| 错误 | Error / Exception |
| 等待 | Wait / Processing / Loading / Active |

**建议：** 提取公共 `ControlStatus` 基础枚举（Default, Success, Warning, Error, Info, Processing），特殊状态由各控件扩展。

### 2. Placement 枚举碎片化（6 个独立枚举）

| 枚举 | 文件 | 成员 |
|---|---|---|
| `FloatButtonPlacement` | `AtomUI.Controls/FloatButton/...` | Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight, Center |
| `DrawerPlacement` | `AtomUI.Desktop.Controls/Drawer/...` | Left, Top, Right, Bottom |
| `AutoCompletePlacementMode` | `AtomUI.Desktop.Controls/AutoComplete/...` | Top, Bottom |
| `MentionsPlacementMode` | `AtomUI.Desktop.Controls/Mentions/...` | Top, Bottom |
| `TourPlacementMode` | `AtomUI.Desktop.Controls/Tour/...` | Center, Left, LeftTop, LeftBottom, Right, RightTop, RightBottom, Top, TopLeft, TopRight, Bottom, BottomLeft, BottomRight |
| `SelectPopupPlacement` | `AtomUI.Desktop.Controls/Select/...` | TopEdgeAlignedLeft, TopEdgeAlignedRight, BottomEdgeAlignedLeft, BottomEdgeAlignedRight |

**命名不一致：** `Placement` vs `PlacementMode` vs `PopupPlacement`。

**建议：** 对于 Top/Bottom 类弹出位置，统一使用 `PopupPlacementMode`；对于 8 方位 + Center，使用 `Placement`（已有 `Avalonia.Controls.PlacementMode` 可复用）。

### 3. `CountBadgeSize` 应复用共享 `SizeType`

| 枚举 | 文件 | 成员 |
|---|---|---|
| `CountBadgeSize` | `AtomUI.Controls/Badge/AbstractCountBadge.cs` | Default, Small |
| `SizeType`（共享） | `AtomUI.Core/Common.cs` | Large, Middle, Small |

`CountBadgeSize` 只有 `Default` 和 `Small`，完全可以复用 `SizeType`。

### 4. Token 类访问修饰符不一致

| Token 类 | 访问修饰符 |
|---|---|
| `ColorPickerToken` | `public` |
| 其他 80+ Token 类 | `internal` |

**文件：** `src/AtomUI.Desktop.Controls.ColorPicker/ColorPickerToken.cs`

**建议：** 统一为 `internal`（Token 类是实现细节，不应暴露给外部）。

### 5. `DataGridSortDirections` 复数命名

`[Flags]` 枚举使用复数 `DataGridSortDirections`，但其他 `[Flags]` 枚举（如 `DataGridGridLinesVisibility`）使用单数。

**文件：** `src/AtomUI.Desktop.Controls.DataGrid/DataGridEnumerations.cs`

**建议：** 统一使用单数。

---

## P4 — AXAML 模板不一致

### 1. PART_ 命名不一致

主流控件全部使用 `PART_` 前缀（577 处），但以下控件使用裸名：

**ColorPicker 主题：**

| 文件 | 当前命名 |
|---|---|
| `ColorPicker/Themes/ColorBlockTheme.axaml` | `ColorPreviewFrame`, `ColorPreview`, `EmptyColorFrame` |
| `ColorPicker/Themes/ColorPickerTheme.axaml` | `Frame`, `ColorText` |
| `ColorPicker/Themes/GradientColorPickerTheme.axaml` | `Frame`, `ColorText` |

**DataGrid 主题：**

| 文件 | 当前命名 |
|---|---|
| `DataGrid/Themes/DataGridColumnHeaderTheme.axaml` | `Frame`, `ContentFrame`, `RootLayout`, `VerticalSeparator`, `FocusVisual` |
| `DataGrid/Themes/DataGridRowTheme.axaml` | `RowBorder`, `FocusVisual` |

**其他：**

| 文件 | 当前命名 |
|---|---|
| `FloatButton/Themes/FloatButtonItemsControlTheme.axaml` | `ItemsLayout` |

**建议：** 全部加上 `PART_` 前缀，与 Avalonia 规范一致。

### 2. `:active` vs `:pressed` 伪类不一致

| 文件 | 伪类 |
|---|---|
| `ColorPicker/Themes/ColorSlider/ColorSliderThumbTheme.axaml:39` | `^:active` |
| 其他所有主题 | `^:pressed` |

**建议：** 统一使用 `:pressed`（Avalonia 标准伪类）。

### 3. `Name=` vs `x:Name=` 混用

| 用法 | 数量 |
|---|---|
| `Name="PART_*"` | 566 处 |
| `x:Name="PART_*"` | 11 处 |

Avalonia 两种写法等效，但混用不规范。

**建议：** 统一使用 `Name=`（Avalonia 原生属性）。

### 4. 5 个 ControlTheme 用字符串 Key

以下主题使用字符串 Key 而非类型 Key：

| 文件 | Key |
|---|---|
| `Menu/Themes/TopLevelMenuItemTheme.axaml` | `x:Key="TopLevelMenuItemTheme"` |
| `TabControl/Themes/TabItemTheme.axaml` | `x:Key="TabItemTheme"` |
| `TabControl/Themes/CardTabItemTheme.axaml` | `x:Key="CardTabItemTheme"` |
| `TabControl/Themes/TabStrip/TabStripItemTheme.axaml` | `x:Key="TabStripItemTheme"` |
| `TabControl/Themes/TabStrip/CardTabStripItemTheme.axaml` | `x:Key="CardTabStripItemTheme"` |

这些都是同一控件类型的变体主题，使用字符串 Key 可能是有意设计（用于主题切换）。需确认是否应该改为类型 Key。

---

## 总结

| 优先级 | 类别 | 数量 | 是否 Breaking |
|---|---|---|---|
| P0 | 拼写错误 | 3 处 | 是（公共 API） |
| P1 | 布尔属性缺 Is 前缀 | 14 处 | 是（公共 API） |
| P2 | 方法命名不一致 | 4 类 | 部分 |
| P3 | 枚举/类型碎片化 | 5 类 | 是（架构层面） |
| P4 | AXAML 模板不一致 | 4 类 | 否（内部实现） |

**建议处理顺序：**
1. P0 拼写错误 — 影响最小，优先修复
2. P4 AXAML 模板 — 不影响公共 API，可安全修复
3. P1 布尔属性 — Breaking change，需配合版本发布
4. P2 方法命名 — 逐步统一
5. P3 枚举碎片化 — 架构重构，影响面最大
