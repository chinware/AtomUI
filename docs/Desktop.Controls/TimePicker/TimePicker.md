# TimePicker 时间选择器

## 概述

时间选择器（TimePicker）用于在弹出面板中选择或输入时间（时、分、秒）。它支持 12/24 小时制、分钟/秒钟步进、确认模式、"此刻"快捷按钮等能力，同时提供 `RangeTimePicker` 用于选择时间范围。

AtomUI 的 `TimePicker` 控件对齐了 [Ant Design 5.0 TimePicker](https://ant.design/components/time-picker-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的时间选择器设计哲学

Ant Design 的 TimePicker 定位是：**「当用户需要输入一个时间，可以点击标准输入框，弹出时间面板进行选择」**。它提供了以下核心能力：

- **12/24 小时制**：根据业务场景切换时钟格式。
- **步进选择**：通过 `minuteStep`（分钟步进）和 `secondStep`（秒钟步进）控制候选项粒度。
- **时间范围选择**：通过 `RangePicker` 模式支持选择起止时间。
- **确认模式**：可选"确认"按钮，避免误操作。
- **"此刻"快捷按钮**：一键填充当前时间。

### 与 Avalonia TimePicker 的关系

AtomUI 的 TimePicker **没有**继承 Avalonia 自带的 `Avalonia.Controls.TimePicker`。Avalonia 原生 TimePicker 使用固定的三列滚轮交互模式，功能较为基础。AtomUI 基于自有的 `InfoPickerInput` 基础设施从头构建，提供更丰富的功能和更贴近 Ant Design 的交互体验。

### AtomUI 的扩展设计

AtomUI TimePicker 基于 `InfoPickerInput`（信息选择器输入框）基类构建，该基类封装了弹出面板（Flyout）管理、输入框交互、清除按钮、装饰框（AddOnDecoratedBox）等通用逻辑。在此基础上 TimePicker 扩展了以下能力：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **12/24 小时制** | `ClockIdentifierType` 枚举 | 对齐 Ant Design 的 `use12Hours` 属性 |
| **分钟/秒钟步进** | `MinuteIncrement` / `SecondIncrement` 属性 | 对齐 Ant Design 的 `minuteStep` / `secondStep` |
| **确认模式** | `IsNeedConfirm` 属性 | 对齐 Ant Design 的 `needConfirm`，需要点击确认按钮才应用选择 |
| **"此刻"按钮** | `IsShowNow` 属性 | 对齐 Ant Design 的 `showNow`，一键设置当前时间 |
| **默认值** | `DefaultTime` 属性 | 控件加载时自动填充默认时间 |
| **时间范围选择** | `RangeTimePicker` 独立控件 | 对齐 Ant Design 的 `TimePicker.RangePicker` |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **三种样式变体** | `IInputControlStyleVariantAware` + `StyleVariant` | Outline / Filled / Borderless 三种输入框风格 |
| **验证状态** | `IInputControlStatusAware` + `Status` | Default / Warning / Error 三种状态反馈 |
| **弹出面板** | `TimePickerFlyout` + `TimePickerPresenter` + `TimeView` | 内部组件协作管理弹出面板的渲染和交互 |
| **国际化** | `LanguageProvider` 系统 | AM/PM 文本和"此刻"按钮文案可本地化 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **紧凑空间** | `ICompactSpaceAware` 接口 | 在 `Space.Compact` 容器中自动调整圆角和边框 |

---

## 功能详解

### 时钟格式（ClockIdentifier）

通过 `ClockIdentifier` 属性设置时钟格式：

| 格式 | 枚举值 | 显示格式 | 面板列数 |
|---|---|---|---|
| 24 小时制 | `HourClock24`（默认） | `HH:mm:ss`（如 `14:30:00`） | 3 列（时/分/秒） |
| 12 小时制 | `HourClock12` | `hh:mm:ss AM/PM`（如 `02:30:00 PM`） | 4 列（时/分/秒/上下午） |

12 小时制下，面板会额外显示 AM/PM（上午/下午）选择列。AM/PM 的显示文本通过国际化系统提供（中文为「上午」「下午」，英文为「AM」「PM」）。

### 步进选择（MinuteIncrement / SecondIncrement）

- `MinuteIncrement`：分钟列的步进值，有效范围 1–59，默认 1。
- `SecondIncrement`：秒钟列的步进值，有效范围 1–59，默认 1。

例如设置 `MinuteIncrement="15"` 后，分钟列仅显示 `00`、`15`、`30`、`45` 四个选项。

### 确认模式（IsNeedConfirm）

当 `IsNeedConfirm = true` 时：
1. 面板底部显示「确定」按钮
2. 用户滚动选择时间后，需要点击确认才会将值应用到输入框
3. 在确认前，面板顶部会实时预览 hover 中的时间值

当 `IsNeedConfirm = false`（默认）时：
1. 用户双击某个选项或使用"此刻"按钮时直接关闭面板并应用值
2. 关闭面板时自动应用当前选中的时间

### "此刻"按钮（IsShowNow）

当 `IsShowNow = true`（默认）时，面板底部显示"此刻"/"Now" 快捷按钮。点击后：
- 立即将 `SelectedTime` 设为当前时间（`DateTime.Now.TimeOfDay`）
- 如果不在确认模式下，同时关闭面板

### 默认值（DefaultTime）

控件首次加载（`OnAttachedToLogicalTree`）时，如果 `SelectedTime` 为 `null` 且 `DefaultTime` 不为 `null`，则自动将 `SelectedTime` 设为 `DefaultTime`。

调用 `Reset()` 方法可将时间重置为 `DefaultTime`。调用 `Clear()` 方法则将时间设为 `null`，忽略默认值。

### 时间范围选择（RangeTimePicker）

`RangeTimePicker` 继承自 `RangeInfoPickerInput`，提供两个输入框（起始时间 / 结束时间），共享同一个弹出面板。

**交互流程**：
1. 点击起始输入框，面板打开，选择起始时间
2. 如果结束时间为空，自动切换到结束时间输入框
3. 两个时间都选定后关闭面板
4. 支持通过 `RangeStartDefaultTime` / `RangeEndDefaultTime` 设置默认值

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基本选择 | ✅ 点击弹出面板 | ✅ `TimePickerFlyout` | ✅ 完全对齐 |
| 12/24 小时制 | ✅ `use12Hours` | ✅ `ClockIdentifier` 枚举 | ✅ 完全对齐 |
| 分钟步进 | ✅ `minuteStep` | ✅ `MinuteIncrement` | ✅ 完全对齐 |
| 秒钟步进 | ✅ `secondStep` | ✅ `SecondIncrement` | ✅ 完全对齐 |
| 确认模式 | ✅ `needConfirm` | ✅ `IsNeedConfirm` | ✅ 完全对齐 |
| "此刻"按钮 | ✅ `showNow` | ✅ `IsShowNow` | ✅ 完全对齐 |
| 三种尺寸 | ✅ `size` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 样式变体 | ✅ `variant` | ✅ `StyleVariant`（Outline/Filled/Borderless） | ✅ 完全对齐 |
| 状态反馈 | ✅ `status` | ✅ `Status`（Default/Warning/Error） | ✅ 完全对齐 |
| 范围选择 | ✅ `RangePicker` | ✅ `RangeTimePicker` | ✅ 完全对齐 |
| 禁用 | ✅ `disabled` | ✅ `IsEnabled=False` | ✅ 完全对齐 |
| 占位文本 | ✅ `placeholder` | ✅ `PlaceholderText` | ✅ 完全对齐 |
| 清除按钮 | ✅ `allowClear` | ✅ 内置 `PickerClearUpButton` | ✅ 完全对齐 |
| 自定义图标 | ✅ `suffixIcon` | ✅ `InfoIcon`（默认 ClockCircleOutlined） | ✅ 完全对齐 |
| 禁用指定时间 | ✅ `disabledTime` | ❌ 暂未支持 | ⚠️ 待实现 |
| 自定义格式 | ✅ `format` | ❌ 暂未支持自定义格式 | ⚠️ 内部使用固定格式 |
| 仅时/仅时分 | ✅ 通过 `format` 控制 | ❌ 始终显示时/分/秒 | ⚠️ 待支持 |
| 附加内容 `renderExtraFooter` | ✅ | ❌ 暂未支持 | ⚠️ 待实现 |
| `open` 受控显示 | ✅ | ❌ 暂未支持 | ⚠️ 待实现 |

---

## 继承关系

### TimePicker

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Primitives.InfoPickerInput
        ├── implements IMotionAwareControl
        ├── implements ICompactSpaceAware
        ├── implements IFormItemAware
        ├── implements IInputControlStatusAware
        ├── implements IInputControlStyleVariantAware
        ├── implements ISizeTypeAware
        └── AtomUI.Desktop.Controls.TimePicker
```

### RangeTimePicker

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Primitives.InfoPickerInput
        └── AtomUI.Desktop.Controls.Primitives.RangeInfoPickerInput
              └── AtomUI.Desktop.Controls.RangeTimePicker
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施 |
| `InfoPickerInput` | 弹出面板（Flyout）管理、输入框交互、清除按钮、装饰框（AddOnDecoratedBox）、尺寸/样式变体/状态、表单集成、紧凑空间、Motion 控制 |
| `RangeInfoPickerInput` | 双输入框（起始/结束）、范围激活部分追踪、选中指示器动画 |
| `TimePicker` | 时间特有属性（ClockIdentifier、MinuteIncrement、SelectedTime 等）、TimePickerFlyout 创建、时间格式化显示 |
| `RangeTimePicker` | 范围时间特有属性（RangeStartSelectedTime / RangeEndSelectedTime 等）、范围交互流程 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持 `StyleVariant`（Outline / Filled / Borderless）三种样式变体 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持 `Status`（Default / Warning / Error）三种验证状态 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持 `IsMotionEnabled` 控制过渡动画开关 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整圆角和边框 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

---

## 内部组件结构

TimePicker 的弹出面板由多个内部组件协作完成：

```
TimePicker / RangeTimePicker
  └── TimePickerFlyout (内部, Flyout 子类)
        └── FlyoutPresenter
              └── TimePickerPresenter (内部, PickerPresenterBase 子类)
                    ├── TimeView (内部, TemplatedControl)
                    │   ├── DateTimePickerPanel (小时列)
                    │   ├── DateTimePickerPanel (分钟列)
                    │   ├── DateTimePickerPanel (秒钟列)
                    │   └── DateTimePickerPanel (AM/PM 列, 12小时制时显示)
                    ├── Button ("此刻"按钮)
                    └── Button ("确定"按钮, 确认模式时显示)
```

| 组件 | 可见性 | 职责 |
|---|---|---|
| `TimePickerFlyout` | `internal` | 管理 Flyout 生命周期，创建 `TimePickerPresenter`，透传属性绑定 |
| `TimePickerPresenter` | `internal` | 组装 `TimeView` 与操作按钮，处理键盘事件，管理确认/取消逻辑 |
| `TimeView` | `internal` | 管理时/分/秒/AM-PM 四个 `DateTimePickerPanel` 的数据和交互 |
| `DateTimePickerPanel` | `internal` | 单列滚轮选择器，支持循环滚动、选中高亮、hover 事件 |
| `TimeViewCell` | `internal` | 滚轮中每个时间单元格的视觉表示 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 基类 | `src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/InfoPickerInput.cs` | 信息选择器输入框基类 |
| 基类 | `src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/RangeInfoPickerInput.cs` | 范围信息选择器输入框基类 |
| 控件类 | `src/AtomUI.Desktop.Controls/TimePicker/TimePicker.cs` | TimePicker 实现 |
| 控件类 | `src/AtomUI.Desktop.Controls/TimePicker/RangeTimePicker.cs` | RangeTimePicker 实现 |
| 弹出面板 | `src/AtomUI.Desktop.Controls/TimePicker/TimePickerFlyout.cs` | Flyout 适配 |
| 面板呈现 | `src/AtomUI.Desktop.Controls/TimePicker/TimePickerPresenter.cs` | 面板呈现器（按钮 + TimeView） |
| 时间视图 | `src/AtomUI.Desktop.Controls/TimePicker/TimeView/TimeView.cs` | 时/分/秒滚轮面板 |
| 时间单元格 | `src/AtomUI.Desktop.Controls/TimePicker/TimeView/TimeViewCell.cs` | 时间单元格渲染 |
| 滚轮面板 | `src/AtomUI.Desktop.Controls/TimePicker/TimeView/DateTimePickerPanel.cs` | 单列滚轮选择器 |
| Token 定义 | `src/AtomUI.Desktop.Controls/TimePicker/TimePickerToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/TimePicker/Themes/TimePickerTheme.axaml` | TimePicker ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/TimePicker/Themes/RangeTimePickerTheme.axaml` | RangeTimePicker ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/TimePicker/Themes/TimePickerPresenterTheme.axaml` | 面板呈现器 ControlTheme |
| 主题模板 | `src/AtomUI.Desktop.Controls/TimePicker/Themes/TimeViewTheme.axaml` | TimeView ControlTheme |
| 主题常量 | `src/AtomUI.Desktop.Controls/TimePicker/Themes/TimePickerThemeConstants.cs` | 模板部件名称常量 |
| 国际化 | `src/AtomUI.Desktop.Controls/TimePicker/Localization/en_US.cs` | 英文语言资源 |
| 国际化 | `src/AtomUI.Desktop.Controls/TimePicker/Localization/zh_CN.cs` | 中文语言资源 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TimePickerShowCase.axaml` | 使用范例 |

---

## 模板结构

### TimePicker 模板

TimePicker 的 ControlTheme 基于 `InfoPickerInputTheme`，整体模板结构继承自 InfoPickerInput：

```
AddOnDecoratedBox (PART_AddOnDecoratedBox)
├── [可选] LeftAddOn                           ← 左侧附加内容
├── Border#ContentFrame (PART_PickerInnerBox)   ← 主输入区域
│   ├── TextBox (PART_InfoInputBox)             ← 时间文本显示/输入
│   └── PickerClearUpButton                     ← 清除按钮 + 图标按钮区域
│       ├── ClockCircleOutlined                 ← 默认图标（时钟图标）
│       └── InputClearIconButton                ← 清除按钮（hover 时显示）
└── [可选] RightAddOn                           ← 右侧附加内容
```

### RangeTimePicker 模板

RangeTimePicker 基于 `RangeInfoPickerInputTheme`，增加了双输入框和范围指示器：

```
AddOnDecoratedBox (PART_AddOnDecoratedBox)
├── Border#ContentFrame
│   ├── TextBox (PART_InfoInputBox)              ← 起始时间输入
│   ├── PathIcon (PART_RangePickerArrow)         ← 范围箭头 (→)
│   ├── TextBox (PART_SecondaryInfoInputBox)     ← 结束时间输入
│   ├── PickerClearUpButton                      ← 清除/图标按钮区域
│   └── Rectangle (PART_RangePickerIndicator)    ← 激活部分下划线指示器
```

### TimePickerPresenter 模板

弹出面板的内部结构：

```
DockPanel (PART_MainLayout)
├── Border#PART_ButtonsFrame (DockPanel.Dock="Bottom")  ← 按钮区域框架
│   └── Panel (PART_ButtonsLayout)
│       ├── Button (PART_NowButton)              ← "此刻"快捷按钮 (Link 类型)
│       └── Button (PART_ConfirmButton)          ← "确定"按钮 (Primary 类型)
└── TimeView (PART_TimeView)                     ← 时间滚轮视图（填充剩余空间）
    ├── DateTimePickerPanel (PART_HourSelector)   ← 小时列
    ├── Rectangle (PART_FirstSpacer)              ← 分隔线
    ├── DateTimePickerPanel (PART_MinuteSelector) ← 分钟列
    ├── Rectangle (PART_SecondSpacer)             ← 分隔线
    ├── DateTimePickerPanel (PART_SecondSelector) ← 秒钟列
    ├── Rectangle (PART_ThirdSpacer)              ← 分隔线（12小时制时显示）
    └── Panel (PART_PeriodHost)                   ← AM/PM 选择列（12小时制时显示）
        └── DateTimePickerPanel (PART_PeriodSelector)
```

### 模板部件常量

#### TimePickerPresenterThemeConstants

| 常量名 | 值 | 说明 |
|---|---|---|
| `MainLayoutPart` | `"PART_MainLayout"` | 主布局面板 |
| `NowButtonPart` | `"PART_NowButton"` | "此刻"快捷按钮 |
| `ConfirmButtonPart` | `"PART_ConfirmButton"` | "确定"按钮 |
| `ButtonsLayoutPart` | `"PART_ButtonsLayout"` | 按钮布局面板 |
| `ButtonsFramePart` | `"PART_ButtonsFrame"` | 按钮区域框架 |
| `TimeViewPart` | `"PART_TimeView"` | 时间滚轮视图 |

#### TimeViewThemeConstants

| 常量名 | 值 | 说明 |
|---|---|---|
| `RootLayoutPart` | `"PART_RootLayout"` | 根布局 |
| `MainFramePart` | `"PART_MainFrame"` | 主框架 |
| `PickerSelectorContainerPart` | `"PART_PickerContainer"` | 选择器容器 |
| `HeaderTextPart` | `"PART_HeaderText"` | 头部文本 |
| `HourSelectorPart` | `"PART_HourSelector"` | 小时选择器 |
| `MinuteSelectorPart` | `"PART_MinuteSelector"` | 分钟选择器 |
| `SecondSelectorPart` | `"PART_SecondSelector"` | 秒钟选择器 |
| `PeriodSelectorPart` | `"PART_PeriodSelector"` | AM/PM 选择器 |
| `PeriodHostPart` | `"PART_PeriodHost"` | AM/PM 容器 |
| `FirstSpacerPart` | `"PART_FirstSpacer"` | 第一条分隔线 |
| `SecondSpacerPart` | `"PART_SecondSpacer"` | 第二条分隔线 |
| `ThirdSpacerPart` | `"PART_ThirdSpacer"` | 第三条分隔线 |
