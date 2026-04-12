# DatePicker 日期选择器

## 概述

日期选择器（DatePicker）用于选择日期或日期范围。用户通过点击输入框弹出日历面板，在面板中选择日期。它是表单场景中最常用的日期输入控件，广泛应用于预约、筛选、时间范围设定等业务场景。

AtomUI 的 `DatePicker` 和 `RangeDatePicker` 控件完整复刻了 [Ant Design 5.0 DatePicker](https://ant.design/components/date-picker-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的日期选择器设计哲学

Ant Design 对日期选择器的定位是：**「当用户需要输入一个日期，可以点击标准输入框，弹出日期面板进行选择」**。为了覆盖不同的日期选择场景，Ant Design 建立了一套完整的日期选择器体系：

**两种选择器模式**：

| 模式 | 设计意图 | 典型用途 |
|---|---|---|
| 📅 **DatePicker（单日期选择）** | 选择单个日期或日期时间，是最基础的日期输入方式 | 「出生日期」「发布时间」「截止日期」 |
| 📅↔📅 **RangeDatePicker（范围日期选择）** | 同时选择起止日期，通过双日历面板直观展示日期范围 | 「入住/退房日期」「统计周期」「请假起止时间」 |

**关键交互特性**：

| 特性 | 设计意图 |
|---|---|
| 📆 **日历面板弹出** | 点击输入框弹出浮层日历面板，选择后自动回填 |
| ⏰ **日期+时间组合** | 通过 `IsShowTime` 开启时间选择，在日历面板旁附加时间选择器 |
| ✅ **确认按钮** | 通过 `IsNeedConfirm` 控制是否需要手动确认，防止误选 |
| 📍 **今天/现在快捷按钮** | 快速定位到当前日期或当前时刻 |
| 🚫 **禁用状态** | 灰色调 + 不可交互，适用于权限不足或条件不满足的场景 |
| 🧹 **一键清除** | 悬浮时显示清除按钮，快速清空已选值 |

### Avalonia 基础能力

AtomUI 的 `DatePicker` 并不继承自 Avalonia 的内置 `Avalonia.Controls.DatePicker`，而是基于 AtomUI 自研的 `InfoPickerInput` 基础架构构建。`InfoPickerInput` 是一个通用的"信息选择器输入框"抽象基类，提供了输入框 + Flyout 弹出面板的标准交互模式。

**InfoPickerInput 提供的核心能力：**

| 能力 | 说明 |
|---|---|
| 输入框外观 | 标准化的输入框容器，支持前后缀（AddOn）、图标、占位文本 |
| Flyout 弹出 | 点击输入框自动弹出选择面板，支持多种弹出方向 |
| 清除按钮 | 悬浮时显示清除图标，一键清空 |
| 尺寸系统 | 支持 `SizeType`（Large / Middle / Small）三种尺寸 |
| 样式变体 | 支持 `StyleVariant`（Outline / Filled / Borderless）三种外观 |
| 状态指示 | 支持 `Status`（Default / Warning / Error）验证状态 |
| 表单集成 | 实现 `IFormItemAware`，可参与表单验证 |
| 紧凑空间 | 实现 `ICompactSpaceAware`，在 `Space.Compact` 中自动调整 |

### AtomUI 的扩展设计

AtomUI DatePicker 在 `InfoPickerInput` 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **单日期选择** | `DatePicker` 控件 + `DatePickerPresenter` | 最基础的日期选择场景 |
| **范围日期选择** | `RangeDatePicker` 控件 + 双日历面板 | 对齐 Ant Design 的 `RangePicker` |
| **日期+时间选择** | `IsShowTime` 属性 + `TimeView` 面板 | 选择精确到时分秒的时间点 |
| **确认模式** | `IsNeedConfirm` 属性 + 确认按钮 | 防止误选，需手动确认 |
| **今天/现在快捷** | `IsShowNow` 属性 + 快捷按钮 | 快速定位到当前时间 |
| **12/24小时制** | `ClockIdentifier` 属性 | 支持 12 小时制（AM/PM）和 24 小时制 |
| **自定义格式** | `Format` 属性 | 自定义日期显示格式 |
| **默认值支持** | `DefaultDateTime` 属性 | 初始化时自动设置默认日期 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` | 全局统一的 Large / Middle / Small 尺寸系统 |
| **三种样式变体** | `IInputControlStyleVariantAware` 接口 | Outline / Filled / Borderless 外观样式 |
| **验证状态** | `IInputControlStatusAware` 接口 | Default / Warning / Error 状态反馈 |
| **Design Token** | `DatePickerToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |
| **国际化** | `LanguageProvider` 系统 | 「今天」「现在」按钮文本支持 i18n |

---

## 功能详解

### 单日期选择（DatePicker）

DatePicker 是最基础的日期选择控件。点击输入框弹出日历面板，用户在面板中选择日期后自动回填到输入框中。

默认行为：
- 选择日期后**立即确认**（无需点击确认按钮），Flyout 自动关闭
- 显示格式为 `yyyy-MM-dd`
- 左侧显示日历图标（`CalendarOutlined`）

### 范围日期选择（RangeDatePicker）

RangeDatePicker 用于选择一个日期范围（起始日期 ~ 结束日期）。它继承自 `RangeInfoPickerInput`，提供双输入框 + 箭头分隔符的标准范围选择器外观。

默认行为：
- 点击左侧输入框选择起始日期，选择后自动切换到右侧输入框选择结束日期
- 不显示时间时，使用**双月日历面板**（`DualMonthRangeCalendar`），一次展示两个月的日期
- 显示时间时，使用**单日历面板 + 时间选择器**（`TimedRangeDatePickerPresenter`）
- 清除时，起始和结束日期同时清空

### 日期+时间选择（IsShowTime）

当 `IsShowTime = true` 时：
1. 日历面板右侧附加 `TimeView` 时间选择器
2. 显示格式自动扩展为 `yyyy-MM-dd HH:mm:ss`（24小时制）或 `yyyy-MM-dd hh:mm:ss tt`（12小时制）
3. 自动启用确认模式（`IsNeedConfirm` 被强制设为 `true`）
4. 底部显示「现在」快捷按钮（替代「今天」按钮）

### 确认模式（IsNeedConfirm）

当 `IsNeedConfirm = true` 时：
- 底部操作栏显示「确定」按钮
- 选择日期后不会立即关闭 Flyout，需点击确认按钮才会提交
- 当尚未选择日期时，确认按钮为禁用状态

### 今天/现在快捷按钮（IsShowNow）

当 `IsShowNow = true`（默认值）时：
- **非时间模式**：显示「今天」链接按钮，点击后选择当天日期
- **时间模式**：显示「现在」链接按钮，点击后选择当前日期和时间

### 自定义格式（Format）

通过 `Format` 属性可以自定义日期显示格式，支持标准的 .NET DateTime 格式字符串。如果未设置，将根据 `IsShowTime` 和 `ClockIdentifier` 自动确定默认格式。

### 弹出方向（PickerPlacement）

通过 `PickerPlacement` 属性控制日历面板的弹出方向，支持 `TopLeft`、`TopRight`、`BottomLeft`（默认）、`BottomRight` 四个方向。

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 日期选择 `DatePicker` | ✅ | ✅ `DatePicker` 控件 | ✅ 完全对齐 |
| 范围选择 `RangePicker` | ✅ | ✅ `RangeDatePicker` 控件 | ✅ 完全对齐 |
| 日期+时间 `showTime` | ✅ 布尔/对象 | ✅ `IsShowTime` 布尔 | ⚠️ 不支持 TimePicker 配置对象 |
| 确认按钮 `needConfirm` | ✅ | ✅ `IsNeedConfirm` | ✅ 完全对齐 |
| 今天按钮 `showNow` | ✅ | ✅ `IsShowNow` | ✅ 完全对齐 |
| 自定义格式 `format` | ✅ | ✅ `Format` 属性 | ✅ 完全对齐 |
| 禁用 `disabled` | ✅ | ✅ `IsEnabled` | ✅ 完全对齐 |
| 清除 `allowClear` | ✅ | ✅ 内置清除按钮 | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ `large / middle / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 样式变体 `variant` | ✅ `outlined / filled / borderless` | ✅ `StyleVariant` 属性 | ✅ 完全对齐 |
| 状态 `status` | ✅ `warning / error` | ✅ `Status` 属性 | ✅ 完全对齐 |
| 弹出方向 `placement` | ✅ | ✅ `PickerPlacement` 属性 | ✅ 完全对齐 |
| 12/24小时制 | ✅ `use12Hours` | ✅ `ClockIdentifier` 属性 | ✅ 完全对齐 |
| 国际化 | ✅ `locale` | ✅ `LanguageProvider` 系统 | ✅ 完全对齐 |
| 默认值 `defaultValue` | ✅ | ✅ `DefaultDateTime` / `RangeStartDefaultDate` | ✅ 完全对齐 |
| 占位文本 `placeholder` | ✅ | ✅ `PlaceholderText` | ✅ 完全对齐 |
| 日期面板 `picker` | ✅ `date / week / month / quarter / year` | ⚠️ 仅支持 `date` 模式 | ⚠️ 待扩展 |
| 预设范围 `presets` | ✅ | ❌ 暂未支持 | ⚠️ 待支持 |
| 禁用日期 `disabledDate` | ✅ 回调函数 | ❌ 暂未支持 | ⚠️ 待支持 |
| 额外页脚 `renderExtraFooter` | ✅ | ❌ 暂未支持 | ⚠️ 待支持 |

---

## 继承关系

### DatePicker

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Primitives.InfoPickerInput
        ├── implements IMotionAwareControl
        ├── implements ICompactSpaceAware
        ├── implements IFormItemAware
        ├── implements IInputControlStatusAware
        ├── implements IInputControlStyleVariantAware
        ├── implements ISizeTypeAware
        └── implements IFormItemFeedbackAware
              └── AtomUI.Desktop.Controls.DatePicker
```

### RangeDatePicker

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Primitives.InfoPickerInput
        └── AtomUI.Desktop.Controls.Primitives.RangeInfoPickerInput
              └── AtomUI.Desktop.Controls.RangeDatePicker
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化控件基础设施、样式系统 |
| `InfoPickerInput` | 输入框 + Flyout 弹出面板标准交互模式、清除按钮、尺寸系统、样式变体、验证状态、AddOn 前后缀、表单集成、紧凑空间适配 |
| `RangeInfoPickerInput` | 双输入框布局、范围激活部分管理、选择指示器动画、起止输入框切换逻辑 |
| `DatePicker` | 单日期选择逻辑、日历面板创建与绑定、日期格式化、默认值管理 |
| `RangeDatePicker` | 范围日期选择逻辑、双月日历/单月+时间面板切换、起止日期管理与自动修正 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持过渡动画开关 |
| `ICompactSpaceAware` | `AtomUI.Controls.Shared` | 在 `Space.Compact` 中使用时自动调整 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可参与 `FormItem` 表单验证 |
| `IInputControlStatusAware` | `AtomUI.Controls.Shared` | 支持验证状态（Warning / Error） |
| `IInputControlStyleVariantAware` | `AtomUI.Controls.Shared` | 支持样式变体（Outline / Filled / Borderless） |
| `IFormItemFeedbackAware` | `AtomUI.Controls.Shared` | 支持表单验证反馈展示 |

---

## 内部组件体系

DatePicker 的实现涉及多个内部组件协同工作：

| 组件 | 可见性 | 职责 |
|---|---|---|
| `DatePicker` | `public` | 单日期选择器主控件 |
| `RangeDatePicker` | `public` | 范围日期选择器主控件 |
| `DatePickerFlyout` | `internal` | 单日期选择弹出层，创建并管理 `DatePickerPresenter` |
| `RangeDatePickerFlyout` | `internal` | 范围日期选择弹出层，根据 `IsShowTime` 动态切换 Presenter 类型 |
| `DatePickerPresenter` | `internal` | 单日期选择面板，包含日历视图 + 操作按钮 |
| `RangeDatePickerPresenter` | `internal` | 范围日期选择面板基类，支持分步选择起止日期 |
| `DualMonthRangeDatePickerPresenter` | `internal` | 双月日历范围选择面板（无时间选择时使用） |
| `TimedRangeDatePickerPresenter` | `internal` | 含时间选择的范围选择面板（有时间选择时使用） |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/DatePicker/DatePicker.cs` | 单日期选择器 |
| 控件类 | `src/AtomUI.Desktop.Controls/DatePicker/RangeDatePicker.cs` | 范围日期选择器 |
| Flyout | `src/AtomUI.Desktop.Controls/DatePicker/DatePickerFlyout.cs` | 单日期弹出层 |
| Flyout | `src/AtomUI.Desktop.Controls/DatePicker/RangeDatePickerFlyout.cs` | 范围日期弹出层 |
| Presenter | `src/AtomUI.Desktop.Controls/DatePicker/DatePickerPresenter.cs` | 单日期选择面板 |
| Presenter | `src/AtomUI.Desktop.Controls/DatePicker/RangeDatePickerPresenter.cs` | 范围日期选择面板基类 |
| Presenter | `src/AtomUI.Desktop.Controls/DatePicker/DualMonthRangeDatePickerPresenter.cs` | 双月日历范围选择面板 |
| Presenter | `src/AtomUI.Desktop.Controls/DatePicker/TimedRangeDatePickerPresenter.cs` | 含时间的范围选择面板 |
| Token 定义 | `src/AtomUI.Desktop.Controls/DatePicker/DatePickerToken.cs` | 组件级 Design Token |
| 国际化 | `src/AtomUI.Desktop.Controls/DatePicker/Localization/en_US.cs` | 英文语言资源 |
| 国际化 | `src/AtomUI.Desktop.Controls/DatePicker/Localization/zh_CN.cs` | 中文语言资源 |
| 主题模板 | `src/AtomUI.Desktop.Controls/DatePicker/Themes/DatePickerTheme.axaml` | DatePicker 主题（基于 InfoPickerInputTheme） |
| 主题模板 | `src/AtomUI.Desktop.Controls/DatePicker/Themes/RangeDatePickerTheme.axaml` | RangeDatePicker 主题（基于 RangeInfoPickerInputTheme） |
| 主题模板 | `src/AtomUI.Desktop.Controls/DatePicker/Themes/DatePickerPresenterTheme.axaml` | Presenter 主题模板 |
| 主题模板 | `src/AtomUI.Desktop.Controls/DatePicker/Themes/DualMonthRangeDatePickerPresenterTheme.axaml` | 双月范围 Presenter 主题 |
| 主题模板 | `src/AtomUI.Desktop.Controls/DatePicker/Themes/TimedRangeDatePickerPresenterTheme.axaml` | 含时间范围 Presenter 主题 |
| 主题常量 | `src/AtomUI.Desktop.Controls/DatePicker/Themes/DatePickerThemeConstants.cs` | 模板部件名称常量 |
| 主题注册 | `src/AtomUI.Desktop.Controls/DatePicker/Themes/DatePickerThemes.axaml` | 主题资源注册 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/DatePickerShowCase.axaml` | 使用范例 |

---

## 模板结构

### DatePicker 模板

DatePicker 的 ControlTheme 基于 `InfoPickerInputTheme`，即标准的信息选择器输入框模板。其核心结构为：

```
InfoPickerInput 标准布局
├── AddOnDecoratedBox (外框 + 前后缀)
│   ├── ContentLeftAddOn (左侧内容附加区)
│   ├── PickerInnerBox (输入框区域)
│   │   ├── InfoIcon (CalendarOutlined 日历图标)
│   │   ├── TextBox (日期文本输入框)
│   │   └── ClearButton (清除按钮，悬浮时显示)
│   └── ContentRightAddOn (右侧内容附加区)
└── Flyout → DatePickerPresenter
    ├── Calendar (日历视图)
    ├── TimeView (时间选择器，IsShowTime=true 时显示)
    └── ButtonsPanel (操作按钮栏)
        ├── TodayButton / NowButton (今天/现在快捷按钮)
        └── ConfirmButton (确认按钮)
```

### RangeDatePicker 模板

RangeDatePicker 基于 `RangeInfoPickerInputTheme`，提供双输入框布局：

```
RangeInfoPickerInput 标准布局
├── AddOnDecoratedBox (外框)
│   ├── InfoInputBox (起始日期输入框)
│   ├── RangePickerArrow (↔ 箭头分隔符)
│   ├── SecondaryInfoInputBox (结束日期输入框)
│   ├── InfoIcon (CalendarOutlined)
│   ├── ClearButton (清除按钮)
│   └── RangePickerIndicator (选择指示条)
└── Flyout → DualMonthRangeDatePickerPresenter 或 TimedRangeDatePickerPresenter
    ├── DualMonthRangeCalendar / RangeCalendar (双月/单月日历)
    ├── TimeView (时间选择器，IsShowTime=true 时显示)
    └── ButtonsPanel (操作按钮栏)
```

### Presenter 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `DatePickerPresenterThemeConstants.NowButtonPart` | `"PART_NowButton"` | 「现在」快捷按钮 |
| `DatePickerPresenterThemeConstants.TodayButtonPart` | `"PART_TodayButton"` | 「今天」快捷按钮 |
| `DatePickerPresenterThemeConstants.ConfirmButtonPart` | `"PART_ConfirmButton"` | 确认按钮 |
| `DatePickerPresenterThemeConstants.CalendarViewPart` | `"PART_CalendarView"` | 日历视图 |
| `DatePickerPresenterThemeConstants.TimeViewPart` | `"PART_TimeView"` | 时间选择器视图 |
