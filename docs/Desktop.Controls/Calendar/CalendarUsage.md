# Calendar 使用文档

本文档介绍 AtomUI Calendar 控件的各种使用方式。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/CalendarShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Calendar，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Calendar 控件及相关类型
```

---

## 1. 基本用法

最简单的日历，显示当前月份，默认允许单范围选择：

```xml
<atom:Calendar />
```

---

## 2. 选择模式

通过 `SelectionMode` 属性设置不同的选择行为：

### 单日选择

```xml
<atom:Calendar SelectionMode="SingleDate" />
```

只允许选择一个日期，点击新日期会取消之前的选择。

### 单范围选择（默认）

```xml
<atom:Calendar SelectionMode="SingleRange" />
```

允许选择一个连续日期范围：
- 点击第一个日期，然后 Shift+点击第二个日期
- 或按住鼠标拖动选择范围

### 多范围选择

```xml
<atom:Calendar SelectionMode="MultipleRange" />
```

允许选择多个不连续的日期范围：
- Ctrl+点击追加单个日期
- Ctrl+Shift+点击追加一个范围
- Ctrl+点击已选日期可取消选择

### 禁止选择

```xml
<atom:Calendar SelectionMode="None" />
```

日历仅供浏览，不允许选择任何日期。

---

## 3. 今日高亮

默认情况下，今天的日期会显示主色调边框高亮。可通过 `IsTodayHighlighted` 关闭：

```xml
<!-- 默认：今日高亮 -->
<atom:Calendar IsTodayHighlighted="True" />

<!-- 关闭今日高亮 -->
<atom:Calendar IsTodayHighlighted="False" />
```

---

## 4. 一周起始日

通过 `FirstDayOfWeek` 设置一周的起始日：

```xml
<!-- 以周一为一周起始日 -->
<atom:Calendar FirstDayOfWeek="Monday" />

<!-- 以周日为一周起始日（默认跟随系统区域） -->
<atom:Calendar FirstDayOfWeek="Sunday" />
```

---

## 5. 限制日期范围

通过 `DisplayDateStart` 和 `DisplayDateEnd` 限制可显示和可选择的日期：

```xml
<!-- 只允许选择 2025 年内的日期 -->
<atom:Calendar DisplayDateStart="2025/01/01" DisplayDateEnd="2025/12/31" />

<!-- 只允许选择从今天开始未来 30 天的日期 -->
<atom:Calendar x:Name="RangeCalendar" />
```

```csharp
// Code-behind
RangeCalendar.DisplayDateStart = DateTime.Today;
RangeCalendar.DisplayDateEnd = DateTime.Today.AddDays(30);
```

---

## 6. 禁用日期（BlackoutDates）

通过 `BlackoutDates` 集合标记不可选的日期：

```xml
<atom:Calendar x:Name="BlackoutCalendar" />
```

```csharp
// Code-behind：禁用所有过去的日期
BlackoutCalendar.BlackoutDates.AddDatesInPast();

// 禁用特定日期范围（如节假日）
BlackoutCalendar.BlackoutDates.Add(
    new CalendarDateRange(new DateTime(2025, 10, 1), new DateTime(2025, 10, 7)));

// 禁用特定单日
BlackoutCalendar.BlackoutDates.Add(
    new CalendarDateRange(new DateTime(2025, 12, 25)));
```

---

## 7. 指定初始显示日期

通过 `DisplayDate` 设置日历初始显示哪个月份：

```xml
<!-- 初始显示 2025 年 6 月 -->
<atom:Calendar DisplayDate="2025/06/01" />
```

---

## 8. 自定义头部背景

```xml
<!-- 头部导航区域使用浅蓝色背景 -->
<atom:Calendar HeaderBackground="#F0F5FF" />
```

---

## 9. 禁用状态

```xml
<!-- 日历整体禁用，呈灰色调 -->
<atom:Calendar IsEnabled="False" />
```

---

## 10. 数据绑定

Calendar 的关键属性支持双向绑定，适合 MVVM 架构：

### SelectedDate 绑定

```xml
<atom:Calendar SelectionMode="SingleDate"
               SelectedDate="{Binding SelectedDate}" />

<TextBlock Text="{Binding SelectedDate, StringFormat='选中日期: {0:yyyy-MM-dd}'}" />
```

```csharp
// ViewModel
public class CalendarViewModel : ReactiveObject
{
    [Reactive]
    public DateTime? SelectedDate { get; set; }
}
```

### DisplayDate 绑定

```xml
<atom:Calendar DisplayDate="{Binding CurrentDisplayDate}" />
```

```csharp
// ViewModel
[Reactive]
public DateTime CurrentDisplayDate { get; set; } = DateTime.Today;
```

---

## 11. 事件处理

### 监听选中日期变化

```xml
<atom:Calendar SelectionMode="SingleDate"
               SelectedDatesChanged="OnSelectedDatesChanged" />
```

```csharp
private void OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
{
    foreach (DateTime added in e.AddedItems)
    {
        Console.WriteLine($"新选中: {added:yyyy-MM-dd}");
    }
    foreach (DateTime removed in e.RemovedItems)
    {
        Console.WriteLine($"取消选中: {removed:yyyy-MM-dd}");
    }
}
```

### 监听显示日期变化

```xml
<atom:Calendar DisplayDateChanged="OnDisplayDateChanged" />
```

```csharp
private void OnDisplayDateChanged(object? sender, CalendarDateChangedEventArgs e)
{
    Console.WriteLine($"从 {e.RemovedDate:yyyy-MM} 切换到 {e.AddedDate:yyyy-MM}");
}
```

### 监听显示模式变化

```xml
<atom:Calendar DisplayModeChanged="OnDisplayModeChanged" />
```

```csharp
private void OnDisplayModeChanged(object? sender, CalendarModeChangedEventArgs e)
{
    Console.WriteLine($"从 {e.OldMode} 切换到 {e.NewMode}");
}
```

---

## 12. 控制动画行为

```xml
<!-- 禁用单元格悬浮过渡动画 -->
<atom:Calendar IsMotionEnabled="False" />
```

---

## 键盘导航速查

Calendar 支持完整的键盘操作：

| 按键 | Month 模式 | Year/Decade 模式 |
|---|---|---|
| ←/→ | 前/后一天 | 前/后一个月/年 |
| ↑/↓ | 前/后一周 | 前/后一行 |
| PageUp | 上一年 | 上一年/十年 |
| PageDown | 下一年 | 下一年/十年 |
| Home | 月初 | 年首/十年首 |
| End | 月末 | 年末/十年末 |
| Ctrl+↑ | 切换到 Year 视图 | 切换到上级视图 |
| Ctrl+↓ | — | 切换到下级视图 |
| Enter/Space | — | 进入选中月/年 |
| Shift+方向键 | 范围选择 | — |

---

## 常见组合模式

### 预约日期选择器

```xml
<StackPanel Spacing="8">
    <TextBlock Text="请选择预约日期：" />
    <atom:Calendar x:Name="BookingCalendar"
                   SelectionMode="SingleDate"
                   SelectedDatesChanged="OnBookingDateChanged" />
</StackPanel>
```

```csharp
public BookingView()
{
    InitializeComponent();
    // 禁用过去的日期
    BookingCalendar.BlackoutDates.AddDatesInPast();
}

private void OnBookingDateChanged(object? sender, SelectionChangedEventArgs e)
{
    if (e.AddedItems.Count > 0)
    {
        var date = (DateTime)e.AddedItems[0]!;
        // 处理预约逻辑...
    }
}
```

### 日期范围筛选

```xml
<StackPanel Spacing="8">
    <TextBlock Text="选择日期范围：" />
    <atom:Calendar x:Name="FilterCalendar"
                   SelectionMode="SingleRange"
                   SelectedDatesChanged="OnFilterRangeChanged" />
</StackPanel>
```

```csharp
private void OnFilterRangeChanged(object? sender, SelectionChangedEventArgs e)
{
    if (sender is Calendar calendar && calendar.SelectedDates.Count > 0)
    {
        var startDate = calendar.SelectedDates.Min();
        var endDate = calendar.SelectedDates.Max();
        // 使用 startDate ~ endDate 进行数据筛选...
    }
}
```
