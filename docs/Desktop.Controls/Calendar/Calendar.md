# Calendar 日历

## 概述

日历（Calendar）是一个可视化日期选择控件，以月历面板形式展示日期，用户可以浏览月份/年份/十年，并从中选择一个或多个日期。Calendar 是日期类交互的基础组件，通常独立使用或作为 `DatePicker` 的下拉面板嵌入。

AtomUI 的 `Calendar` 控件基于 Microsoft 的日历控件实现（最初来自 Silverlight / WPF），在此基础上融入了 [Ant Design 5.0 Calendar](https://ant.design/components/calendar-cn) 的视觉风格，并通过 AtomUI 的 Design Token 系统实现主题化和样式统一。

---

## 设计原理

### 核心功能

Calendar 提供以下核心能力：

| 能力 | 说明 |
|---|---|
| **三种显示模式** | Month（月视图，显示天）、Year（年视图，显示月）、Decade（十年视图，显示年） |
| **四种选择模式** | SingleDate（单日选择）、SingleRange（单范围选择）、MultipleRange（多范围选择）、None（禁止选择） |
| **禁用日期** | 通过 `BlackoutDates` 集合标记不可选日期，显示为禁用样式 |
| **今日高亮** | 默认高亮显示今天的日期，可通过 `IsTodayHighlighted` 关闭 |
| **日期范围约束** | 通过 `DisplayDateStart` / `DisplayDateEnd` 限制可显示和可选择的日期范围 |
| **键盘导航** | 完整的键盘操作支持（方向键、PageUp/Down、Home/End、Enter/Space） |
| **滚轮导航** | 鼠标滚轮翻页，Ctrl+滚轮切换显示模式 |
| **动画过渡** | 单元格悬浮背景色渐变过渡动画 |

### Avalonia 基础能力

AtomUI 的 `Calendar` 直接继承自 Avalonia 框架的 `TemplatedControl`（模板化控件）。与 Button 不同，Calendar 不是 `ContentControl`，它的视觉结构完全由 `ControlTemplate` 定义。继承链为：

```
Control → TemplatedControl → Calendar
```

**TemplatedControl 提供的基础设施：**

| 能力 | 说明 |
|---|---|
| `Template` | ControlTemplate 模板化，所有视觉结构在 AXAML 中定义 |
| `Background` / `BorderBrush` / `BorderThickness` / `CornerRadius` | 外观基础属性 |
| `FontSize` / `Foreground` | 文本基础属性 |
| `Padding` | 内间距 |
| `IsEnabled` | 启用/禁用状态 |

### AtomUI 的扩展设计

AtomUI `Calendar` 在 TemplatedControl 基础上做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **三种显示模式** | `CalendarMode` 枚举 + MonthView/YearView 切换 | 支持按月浏览、按年浏览、按十年浏览 |
| **四种选择模式** | `CalendarSelectionMode` 枚举 + `SelectedDates` 集合 | 满足单选、范围选择、多范围选择等业务需求 |
| **禁用日期** | `CalendarBlackoutDatesCollection` 集合 | 标记不可选日期（节假日、已预约日期等） |
| **日期范围约束** | `DisplayDateStart` / `DisplayDateEnd` 属性 | 限制可浏览和可选择的日期边界 |
| **今日高亮** | `IsTodayHighlighted` 属性 + `:today` 伪类 | 视觉锚点，帮助用户快速定位当前日期 |
| **头部背景自定义** | `HeaderBackground` 属性 | 支持自定义头部导航区域的背景色 |
| **动画过渡** | `IMotionAwareControl` + 单元格 `Transitions` | 背景色平滑过渡，提升交互体验 |
| **Design Token** | `CalendarToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 显示模式（CalendarMode）

Calendar 支持三种显示模式，通过 `DisplayMode` 属性设置，也可通过点击头部标题按钮切换：

| 模式 | 视图内容 | 导航操作 |
|---|---|---|
| `Month` | 显示一个月的日期网格（7列×6行），顶行为星期标题 | 点击左右箭头切换月/年，点击头部标题进入 Year 模式 |
| `Year` | 显示一年的12个月份按钮（4列×3行） | 点击月份进入 Month 模式，点击头部标题进入 Decade 模式 |
| `Decade` | 显示一个十年的年份按钮（4列×3行） | 点击年份进入 Year 模式 |

**头部导航按钮布局：**

```
[<<] [<] ──── 2025年4月 ──── [>] [>>]
 │    │         │              │    │
 │    │     HeadTextButton     │    │
 │    │   (年月标题/年/十年)   │    │
 │    └─ 上月 (仅Month模式)    │    │
 │                              └─ 下月 (仅Month模式)
 └─ 上年/上一个十年             └─ 下年/下一个十年
```

- Month 模式：显示四个导航按钮（上年 `<<`、上月 `<`、下月 `>`、下年 `>>`）
- Year/Decade 模式：仅显示两个导航按钮（`<<` 和 `>>`），月级按钮自动隐藏

### 选择模式（CalendarSelectionMode）

| 模式 | 行为 |
|---|---|
| `SingleDate` | 只能选择一个日期。使用 `SelectedDate` 获取/设置选中值 |
| `SingleRange` | 可选择一个连续日期范围。Shift+点击或 Shift+方向键选择范围 |
| `MultipleRange` | 可选择多个不连续的日期范围。Ctrl+点击追加，Shift+点击范围选择 |
| `None` | 禁止选择，日历仅供浏览 |

### 禁用日期（BlackoutDates）

`BlackoutDates` 是一个 `CalendarBlackoutDatesCollection`，可添加单个日期或日期范围。被禁用的日期呈灰色调且不可点击。集合提供便捷方法 `AddDatesInPast()` 可一次性禁用所有过去的日期。

### 键盘导航

| 按键组合 | Month 模式 | Year 模式 | Decade 模式 |
|---|---|---|---|
| ↑/↓/←/→ | 移动焦点日期 | 移动焦点月份 | 移动焦点年份 |
| Shift+方向键 | 范围选择（非 SingleDate/None） | — | — |
| Ctrl+↑ | 切换到 Year 模式 | 切换到 Decade 模式 | — |
| Ctrl+↓ | — | 切换到 Month 模式 | 切换到 Year 模式 |
| PageUp/PageDown | 上/下一年 | 上/下一年 | 上/下一个十年 |
| Shift+PageUp/Down | 上/下一月并范围选择 | — | — |
| Home/End | 跳到月初/月末 | 跳到1月/12月 | 跳到十年首/末年 |
| Enter/Space | — | 进入对应月份的 Month 视图 | 进入对应年份的 Year 视图 |

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 月视图 | ✅ | ✅ Month 模式 | ✅ 完全对齐 |
| 年视图 | ✅ 通过 mode 切换 | ✅ Year / Decade 模式 | ✅ 完全对齐 |
| 日期选择 | ✅ | ✅ 四种选择模式 | ✅ 完全对齐 |
| 禁用日期 | ✅ disabledDate | ✅ BlackoutDates 集合 | ⚠️ 语法不同但语义一致 |
| 今日高亮 | ✅ | ✅ IsTodayHighlighted | ✅ 完全对齐 |
| 自定义单元格渲染 | ✅ cellRender | ❌ 暂不支持 | ⚠️ 待支持 |
| 全屏日历模式 | ✅ fullscreen | ❌ 暂不支持 | ⚠️ 待支持 |
| 国际化/locale | ✅ locale 属性 | ⚠️ 跟随系统区域设置 | ⚠️ 通过 CultureInfo 实现 |
| 头部自定义 | ✅ headerRender | ⚠️ HeaderBackground 属性 | ⚠️ 部分支持 |
| Design Token 主题 | ✅ | ✅ CalendarToken | ✅ 完全对齐 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── AtomUI.Desktop.Controls.Calendar
        └── implements IMotionAwareControl
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `TemplatedControl` | 模板化视觉结构、外观基础属性（Background、BorderBrush、CornerRadius）、字体属性 |
| `Calendar` | 日期选择逻辑（显示模式、选择模式、日期范围）、键盘/鼠标导航、BlackoutDates、CalendarItem/CalendarDayButton 子控件管理、Design Token 集成 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls` | 支持 `IsMotionEnabled` 属性，控制单元格悬浮动画开关 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类 | `src/AtomUI.Desktop.Controls/Calendar/Calendar.cs` | Calendar 主控件 |
| 日历项 | `src/AtomUI.Desktop.Controls/Calendar/CalendarItem.cs` | 日历面板（MonthView + YearView + 头部导航） |
| 日按钮基类 | `src/AtomUI.Desktop.Controls/Calendar/BaseCalendarDayButton.cs` | 月视图中的日期按钮基类 |
| 日按钮 | `src/AtomUI.Desktop.Controls/Calendar/CalendarDayButton.cs` | 日期按钮具体实现 |
| 月/年按钮基类 | `src/AtomUI.Desktop.Controls/Calendar/BaseCalendarButton.cs` | 年/十年视图中的按钮基类 |
| 月/年按钮 | `src/AtomUI.Desktop.Controls/Calendar/CalendarButton.cs` | 月/年按钮具体实现 |
| 头部标题按钮 | `src/AtomUI.Desktop.Controls/Calendar/HeadTextButton.cs` | 头部年月标题按钮 |
| 禁用日期集合 | `src/AtomUI.Desktop.Controls/Calendar/CalendarBlackoutDatesCollection.cs` | 不可选日期集合 |
| 选中日期集合 | `src/AtomUI.Desktop.Controls/Calendar/SelectedDatesCollection.cs` | 已选日期集合 |
| 日期范围 | `src/AtomUI.Desktop.Controls/Calendar/CalendarDateRange.cs` | 日期范围辅助类 |
| 日期工具 | `src/AtomUI.Desktop.Controls/Calendar/DateTimeHelper.cs` | 日期计算辅助工具 |
| 扩展工具 | `src/AtomUI.Desktop.Controls/Calendar/CalendarExtensions.cs` | 日历扩展方法 |
| Token 定义 | `src/AtomUI.Desktop.Controls/Calendar/CalendarToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/Calendar/Themes/CalendarTheme.axaml` | Calendar ControlTheme |
| 日历项主题 | `src/AtomUI.Desktop.Controls/Calendar/Themes/CalendarItemTheme.axaml` | CalendarItem ControlTheme |
| 日按钮主题 | `src/AtomUI.Desktop.Controls/Calendar/Themes/BaseCalendarDayButtonTheme.axaml` | 日期按钮 ControlTheme |
| 月/年按钮主题 | `src/AtomUI.Desktop.Controls/Calendar/Themes/BaseCalendarButtonTheme.axaml` | 月/年按钮 ControlTheme |
| 标题按钮主题 | `src/AtomUI.Desktop.Controls/Calendar/Themes/HeadTextButtonTheme.axaml` | 头部标题按钮 ControlTheme |
| 主题注册 | `src/AtomUI.Desktop.Controls/Calendar/Themes/CalendarThemes.axaml` | ResourceDictionary 注册 |
| 模板常量 | `src/AtomUI.Desktop.Controls/Calendar/Themes/CalendarThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CalendarShowCase.axaml` | 使用范例 |

---

## 模板结构

Calendar 的整体模板由 Calendar → CalendarItem → MonthView/YearView 三层嵌套组成：

```
Border#PART_Frame (边框、背景、圆角、内间距)
└── Panel#PART_Root (裁剪容器)
    └── CalendarItem#PART_CalendarItem (日历核心面板)
        └── DockPanel#PART_ItemRootLayout
            ├── Grid#PART_HeaderLayout (头部导航，DockPanel.Dock="Top")
            │   ├── IconButton#PART_PreviousButton   ← 上年 (<<)
            │   ├── IconButton#PART_PreviousMonthButton ← 上月 (<，仅 Month 模式可见)
            │   ├── HeadTextButton#PART_HeaderButton  ← 年月/年/十年 标题
            │   ├── IconButton#PART_NextMonthButton   ← 下月 (>，仅 Month 模式可见)
            │   └── IconButton#PART_NextButton        ← 下年 (>>)
            ├── Grid#PART_MonthView (月视图，7列×7行网格)
            │   ├── [Row 0] 星期标题 (DayTitleTemplate × 7)
            │   └── [Row 1-6] CalendarDayButton × 42 (6行 × 7列日期按钮)
            └── Grid#PART_YearView (年/十年视图，4列×3行网格)
                └── CalendarButton × 12 (月份或年份按钮)
```

**分层设计理由：**
- **Border#PART_Frame**：提供外边框、背景色和圆角，所有视觉属性由 Token 驱动。
- **Panel#PART_Root**：`ClipToBounds="True"`，确保内容不溢出边框。
- **CalendarItem**：核心面板，管理头部导航和两个视图（MonthView / YearView），通过 `IsVisible` 切换。
- **MonthView / YearView**：互斥显示，Month 模式显示 MonthView，Year/Decade 模式显示 YearView。

### 模板部件常量

#### CalendarThemeConstants

| 常量名 | 值 | 说明 |
|---|---|---|
| `RootPart` | `"PART_Root"` | 根面板（Panel） |
| `CalendarItemPart` | `"PART_CalendarItem"` | 日历核心面板（CalendarItem） |
| `FramePart` | `"PART_Frame"` | 外边框（Border） |

#### CalendarItemThemeConstants

| 常量名 | 值 | 说明 |
|---|---|---|
| `ItemFramePart` | `"PART_ItemFrame"` | 日历项外框 |
| `ItemRootLayoutPart` | `"PART_ItemRootLayout"` | 日历项根布局 |
| `MonthViewLayoutPart` | `"PART_MonthViewLayout"` | 月视图布局 |
| `MonthViewPart` | `"PART_MonthView"` | 月视图网格 |
| `YearViewPart` | `"PART_YearView"` | 年/十年视图网格 |
| `HeaderLayoutPart` | `"PART_HeaderLayout"` | 头部导航布局 |
| `HeaderFramePart` | `"PART_HeaderFrame"` | 头部外框 |
| `PreviousButtonPart` | `"PART_PreviousButton"` | 上年按钮 |
| `PreviousMonthButtonPart` | `"PART_PreviousMonthButton"` | 上月按钮 |
| `HeaderButtonPart` | `"PART_HeaderButton"` | 头部标题按钮 |
| `NextMonthButtonPart` | `"PART_NextMonthButton"` | 下月按钮 |
| `NextButtonPart` | `"PART_NextButton"` | 下年按钮 |

#### BaseCalendarDayButtonThemeConstants / BaseCalendarButtonThemeConstants / HeadTextButtonThemeConstants

| 常量名 | 值 | 说明 |
|---|---|---|
| `ContentPart` | `"PART_Content"` | 内容展示器 |
