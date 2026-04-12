# Calendar API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### CalendarMode

日历显示模式枚举，控制日历面板的视图层级。

| 值 | 说明 |
|---|---|
| `Month` | 月视图，显示一个月的日期网格（默认） |
| `Year` | 年视图，显示一年中的12个月份 |
| `Decade` | 十年视图，显示一个十年中的年份 |

### CalendarSelectionMode

日历选择模式枚举，控制日期选择行为。

| 值 | 说明 |
|---|---|
| `SingleDate` | 仅可选择单个日期 |
| `SingleRange` | 可选择一个连续日期范围（默认） |
| `MultipleRange` | 可选择多个不连续的日期范围 |
| `None` | 不允许选择任何日期 |

---

## 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `FirstDayOfWeek` | `DayOfWeek` | 跟随系统区域设置 | 一周的起始日（如 `Sunday` 或 `Monday`） |
| `IsTodayHighlighted` | `bool` | `true` | 是否高亮显示今天的日期 |
| `HeaderBackground` | `IBrush?` | `null` | 头部导航区域的自定义背景色 |
| `DisplayMode` | `CalendarMode` | `CalendarMode.Month` | 当前显示模式（Month / Year / Decade） |
| `SelectionMode` | `CalendarSelectionMode` | `CalendarSelectionMode.SingleRange` | 选择模式 |
| `SelectedDate` | `DateTime?` | `null` | 当前选中的日期（双向绑定，`TwoWay`） |
| `DisplayDate` | `DateTime` | `DateTime.Today` | 当前显示的日期（双向绑定，`TwoWay`），决定显示哪个月/年 |
| `DisplayDateStart` | `DateTime?` | `null` | 可显示的最早日期（双向绑定，`TwoWay`），`null` 表示无限制 |
| `DisplayDateEnd` | `DateTime?` | `null` | 可显示的最晚日期（双向绑定，`TwoWay`），`null` 表示无限制 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用过渡动画（共享属性，通过 `AddOwner` 注册） |

### 公共集合属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `SelectedDates` | `SelectedDatesCollection` | 所有已选日期的集合（只读属性，内容可修改）。支持 `Add`、`AddRange`、`Clear`、`Remove` 操作 |
| `BlackoutDates` | `CalendarBlackoutDatesCollection` | 被禁用（不可选）的日期集合。支持添加单个日期或日期范围，提供 `AddDatesInPast()` 便捷方法 |

### 继承自 TemplatedControl 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Background` | `IBrush?` | 由 Token 控制 (`ColorBgContainer`) | 日历面板背景色 |
| `BorderBrush` | `IBrush?` | 由 Token 控制 (`ColorBorder`) | 边框颜色 |
| `BorderThickness` | `Thickness` | 由 Token 控制 (`BorderThickness`) | 边框粗细 |
| `CornerRadius` | `CornerRadius` | 由 Token 控制 (`BorderRadius`) | 圆角半径 |
| `Padding` | `Thickness` | 由 Token 控制 (`PanelContentPadding`) | 面板内间距 |
| `MinWidth` | `double` | 由 Token 控制 (`ItemPanelMinWidth`) | 最小宽度 |
| `MinHeight` | `double` | 由 Token 控制 (`ItemPanelMinHeight`) | 最小高度 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `HorizontalAlignment` | `HorizontalAlignment` | `Left` | 水平对齐方式 |
| `VerticalAlignment` | `VerticalAlignment` | `Top` | 垂直对齐方式 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `SelectedDatesChanged` | `EventHandler<SelectionChangedEventArgs>` | 选中日期集合发生变化时触发。`AddedItems` 包含新增日期，`RemovedItems` 包含移除的日期 |
| `DisplayDateChanged` | `EventHandler<CalendarDateChangedEventArgs>` | `DisplayDate` 属性变化时触发。`RemovedDate` 为旧日期，`AddedDate` 为新日期 |
| `DisplayModeChanged` | `EventHandler<CalendarModeChangedEventArgs>` | `DisplayMode` 属性变化时触发。`OldMode` 为旧模式，`NewMode` 为新模式 |

### 事件参数类型

#### CalendarDateChangedEventArgs

```csharp
public class CalendarDateChangedEventArgs : RoutedEventArgs
{
    public DateTime? RemovedDate { get; }  // 之前显示的日期
    public DateTime? AddedDate { get; }    // 新显示的日期
}
```

#### CalendarModeChangedEventArgs

```csharp
public class CalendarModeChangedEventArgs : RoutedEventArgs
{
    public CalendarMode OldMode { get; }  // 之前的显示模式
    public CalendarMode NewMode { get; }  // 新的显示模式
}
```

---

## 伪类（Pseudo-Classes）

Calendar 本身不定义自定义伪类，但其内部子控件定义了丰富的伪类用于样式驱动。

### BaseCalendarDayButton 伪类（月视图中的日期按钮）

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:today` | `BaseCalendarDayButton.TodayPC` | 该日期是今天，且 `IsTodayHighlighted == true` |
| `:selected` | `StdPseudoClass.Selected` | 该日期被选中 |
| `:inactive` | `StdPseudoClass.InActive` | 该日期不属于当前显示月份（上月或下月的溢出日期） |
| `:blackout` | `BaseCalendarDayButton.BlackoutPC` | 该日期在 `BlackoutDates` 集合中 |
| `:dayfocused` | `BaseCalendarDayButton.DayfocusedPC` | 该日期是当前键盘焦点所在日期 |
| `:pressed` | `StdPseudoClass.Pressed` | 按钮被按下 |
| `:disabled` | `StdPseudoClass.Disabled` | 按钮被禁用 |
| `:pointerover` | — | 鼠标悬浮 |

### BaseCalendarButton 伪类（年/十年视图中的月份/年份按钮）

| 伪类 | 触发条件 |
|---|---|
| `:selected` | 该月份/年份被选中 |
| `:inactive` | 该年份不属于当前显示的十年范围 |
| `:btnfocused` | 该按钮是当前键盘焦点所在按钮 |
| `:pointerover` | 鼠标悬浮 |

### CalendarItem 伪类

| 伪类 | 触发条件 |
|---|---|
| `:calendardisabled` | 父 Calendar 控件 `IsEnabled == false` |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制日历单元格的背景色过渡动画。当 `IsMotionEnabled = false` 时，日期按钮和月/年按钮在悬浮时不再有渐变过渡效果。

---

## 辅助类型

### CalendarBlackoutDatesCollection

不可选日期集合，继承自 `ObservableCollection<CalendarDateRange>`。

| 方法 | 说明 |
|---|---|
| `Add(CalendarDateRange)` | 添加一个日期范围为不可选 |
| `AddDatesInPast()` | 将所有过去的日期标记为不可选 |
| `Contains(DateTime)` | 检查指定日期是否在禁用集合中 |
| `ContainsAny(CalendarDateRange)` | 检查指定范围是否与禁用日期有交集 |

### SelectedDatesCollection

已选日期集合。

| 方法 | 说明 |
|---|---|
| `Add(DateTime)` | 添加一个日期到选中集合 |
| `AddRange(DateTime start, DateTime end)` | 添加一个日期范围到选中集合 |
| `Clear()` | 清空所有选中日期 |
| `Remove(DateTime)` | 从选中集合中移除指定日期 |

### CalendarDateRange

表示一个日期范围。

| 属性 | 类型 | 说明 |
|---|---|---|
| `Start` | `DateTime` | 范围起始日期 |
| `End` | `DateTime` | 范围结束日期 |
