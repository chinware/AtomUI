# DatePicker 使用文档

本文档介绍 AtomUI DatePicker 和 RangeDatePicker 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/DatePickerShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 DatePicker，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // DatePicker, RangeDatePicker 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最简单的日期选择器，点击弹出日历面板，选择日期后自动回填。

```xml
<atom:DatePicker PlaceholderText="Select date" />
```

**默认行为**：
- 显示格式：`yyyy-MM-dd`
- 选择后立即确认（无需点击确认按钮）
- 左侧显示日历图标

---

## 2. 范围日期选择

使用 `RangeDatePicker` 选择一个日期范围，提供双输入框 + 箭头分隔符。

```xml
<atom:RangeDatePicker PlaceholderText="Select date"
                      SecondaryPlaceholderText="End date" />
```

**默认行为**：
- 点击左侧输入框选择起始日期
- 选择后自动切换到右侧输入框选择结束日期
- 不显示时间时使用双月日历面板

---

## 3. 确认模式

通过 `IsNeedConfirm` 属性启用手动确认，选择日期后需点击「确定」按钮才会提交。

```xml
<!-- 单日期选择 + 确认按钮 -->
<atom:DatePicker IsNeedConfirm="True" PlaceholderText="Select date" />

<!-- 范围选择 + 确认按钮 -->
<atom:RangeDatePicker IsNeedConfirm="True"
                      IsShowTime="False"
                      PlaceholderText="Select date"
                      SecondaryPlaceholderText="End date" />
```

---

## 4. 日期+时间选择

通过 `IsShowTime="True"` 开启时间选择，日历面板右侧将附加时间选择器。

```xml
<!-- 单日期+时间选择 -->
<atom:DatePicker IsNeedConfirm="True"
                 IsShowTime="True"
                 PlaceholderText="Select date" />

<!-- 范围+时间选择 -->
<atom:RangeDatePicker IsNeedConfirm="True"
                      IsShowTime="True"
                      PlaceholderText="Select date"
                      SecondaryPlaceholderText="End date" />
```

**注意**：开启 `IsShowTime` 后：
- 显示格式自动扩展为 `yyyy-MM-dd HH:mm:ss`
- 确认模式自动开启（`IsNeedConfirm` 被强制设为 `true`）
- 底部快捷按钮从「今天」变为「现在」

---

## 5. 12 小时制

通过 `ClockIdentifier` 属性切换 12/24 小时制：

```xml
<atom:RangeDatePicker StyleVariant="Outline"
                      Status="Default"
                      PlaceholderText="Start date"
                      SecondaryPlaceholderText="End date"
                      IsNeedConfirm="True"
                      ClockIdentifier="HourClock24" />
```

---

## 6. 禁用状态

通过 `IsEnabled="False"` 禁用日期选择器：

```xml
<!-- 禁用的单日期选择器（带预选值） -->
<atom:DatePicker IsNeedConfirm="True"
                 PlaceholderText="Select date"
                 SelectedDateTime="2024-01-20"
                 IsEnabled="False" />

<!-- 禁用的时间日期选择器 -->
<atom:DatePicker IsNeedConfirm="True"
                 PlaceholderText="Select date"
                 IsShowTime="True"
                 SelectedDateTime="2024-01-20 12:22:23 AM"
                 IsEnabled="False" />

<!-- 禁用的范围选择器 -->
<atom:RangeDatePicker IsNeedConfirm="True"
                      PlaceholderText="Select date"
                      SecondaryPlaceholderText="End date"
                      RangeStartDefaultDate="2024-01-20"
                      RangeEndDefaultDate="2024-03-20"
                      IsEnabled="False" />

<!-- 禁用的范围+时间选择器 -->
<atom:RangeDatePicker IsNeedConfirm="True"
                      PlaceholderText="Select date"
                      SecondaryPlaceholderText="End date"
                      IsShowTime="True"
                      RangeStartDefaultDate="2024-01-20 12:22:23 AM"
                      RangeEndDefaultDate="2024-02-20 07:22:23 AM"
                      IsEnabled="False" />
```

---

## 7. 三种尺寸

通过 `SizeType` 属性设置尺寸，支持 `Large`、`Middle`（默认）、`Small` 三种。

```xml
<atom:DatePicker IsNeedConfirm="True"
                 PlaceholderText="Select date"
                 SelectedDateTime="2024-01-20"
                 SizeType="{Binding PickerSizeType}" />

<atom:RangeDatePicker IsNeedConfirm="True"
                      PlaceholderText="Select date"
                      SecondaryPlaceholderText="End date"
                      IsShowTime="True"
                      RangeStartDefaultDate="2024-01-20 12:22:23 AM"
                      RangeEndDefaultDate="2024-02-20 07:22:23 AM"
                      SizeType="{Binding PickerSizeType}" />
```

尺寸可通过数据绑定动态切换（参考 Gallery 中的 `OptionButtonGroup` 示例）。

---

## 8. 验证状态

通过 `Status` 属性设置验证状态，支持 `Default`、`Warning`、`Error` 三种：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 默认状态 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:DatePicker Status="Default" PlaceholderText="Select time" />
        <atom:RangeDatePicker StyleVariant="Outline"
                              Status="Default"
                              PlaceholderText="Start date"
                              SecondaryPlaceholderText="End date"
                              IsNeedConfirm="True"
                              ClockIdentifier="HourClock24" />
    </StackPanel>
    
    <!-- 警告状态 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:DatePicker Status="Warning" PlaceholderText="Select time" />
        <atom:RangeDatePicker StyleVariant="Outline"
                              Status="Warning"
                              PlaceholderText="Start date"
                              SecondaryPlaceholderText="End date"
                              IsNeedConfirm="True"
                              ClockIdentifier="HourClock24" />
    </StackPanel>
    
    <!-- 错误状态 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:DatePicker Status="Error" PlaceholderText="Select time" />
        <atom:RangeDatePicker StyleVariant="Outline"
                              Status="Error"
                              PlaceholderText="Start date"
                              SecondaryPlaceholderText="End date"
                              IsNeedConfirm="True"
                              ClockIdentifier="HourClock24" />
    </StackPanel>
</StackPanel>
```

---

## 9. 样式变体

通过 `StyleVariant` 属性设置外观变体：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- Outline 轮廓样式（默认） -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:DatePicker StyleVariant="Outline" PlaceholderText="Outline" />
        <atom:RangeDatePicker StyleVariant="Outline"
                              SecondaryPlaceholderText="End date"
                              IsNeedConfirm="True"
                              PlaceholderText="Outline" />
    </StackPanel>
    
    <!-- Filled 填充样式 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:DatePicker StyleVariant="Filled" PlaceholderText="Filled" />
        <atom:RangeDatePicker StyleVariant="Filled"
                              SecondaryPlaceholderText="End date"
                              IsNeedConfirm="True"
                              PlaceholderText="Filled" />
    </StackPanel>
    
    <!-- Filled + Error 状态组合 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:DatePicker StyleVariant="Filled" Status="Error"
                         PlaceholderText="Filled" />
        <atom:RangeDatePicker StyleVariant="Filled" Status="Error"
                              SecondaryPlaceholderText="End date"
                              IsNeedConfirm="True"
                              PlaceholderText="Filled" />
    </StackPanel>
    
    <!-- Borderless 无边框样式 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:DatePicker StyleVariant="Borderless" PlaceholderText="Borderless" />
        <atom:RangeDatePicker StyleVariant="Borderless"
                              SecondaryPlaceholderText="End date"
                              IsNeedConfirm="True"
                              PlaceholderText="Borderless" />
    </StackPanel>
</StackPanel>
```

---

## 10. 弹出方向

通过 `PickerPlacement` 属性控制日历面板弹出方向：

```xml
<atom:DatePicker IsNeedConfirm="True"
                 PlaceholderText="Select date"
                 SelectedDateTime="2024-01-20"
                 PickerPlacement="{Binding PickerPlacement}" />

<atom:RangeDatePicker IsNeedConfirm="True"
                      PlaceholderText="Select date"
                      IsShowTime="True"
                      RangeStartDefaultDate="2024-01-20 12:22:23 AM"
                      RangeEndDefaultDate="2024-02-20 07:22:23 AM"
                      PickerPlacement="{Binding PickerPlacement}" />
```

支持的弹出方向：`TopLeft`、`TopRight`、`BottomLeft`（默认）、`BottomRight`。

---

## 11. 默认值

通过 `DefaultDateTime`（单日期）或 `RangeStartDefaultDate` / `RangeEndDefaultDate`（范围）设置默认值：

```xml
<!-- 单日期默认值 -->
<atom:DatePicker DefaultDateTime="2024-01-20"
                 PlaceholderText="Select date" />

<!-- 范围默认值 -->
<atom:RangeDatePicker RangeStartDefaultDate="2024-01-20"
                      RangeEndDefaultDate="2024-03-20"
                      PlaceholderText="Start date"
                      SecondaryPlaceholderText="End date" />

<!-- 范围+时间默认值 -->
<atom:RangeDatePicker IsShowTime="True"
                      RangeStartDefaultDate="2024-01-20 12:22:23 AM"
                      RangeEndDefaultDate="2024-02-20 07:22:23 AM"
                      PlaceholderText="Start date"
                      SecondaryPlaceholderText="End date" />
```

---

## 12. MVVM 数据绑定

DatePicker 完整支持双向数据绑定：

```xml
<!-- 单日期绑定 -->
<atom:DatePicker SelectedDateTime="{Binding SelectedDate}"
                 PlaceholderText="Select date" />

<!-- 范围绑定 -->
<atom:RangeDatePicker RangeStartSelectedDate="{Binding StartDate}"
                      RangeEndSelectedDate="{Binding EndDate}"
                      PlaceholderText="Start date"
                      SecondaryPlaceholderText="End date" />
```

```csharp
// ViewModel（使用 ReactiveUI）
public class DatePickerViewModel : ReactiveObject
{
    [Reactive]
    public DateTime? SelectedDate { get; set; }

    [Reactive]
    public DateTime? StartDate { get; set; }

    [Reactive]
    public DateTime? EndDate { get; set; }
}
```

---

## 13. 程序化控制

在 code-behind 中操作 DatePicker：

```csharp
// 清除选中值
datePicker.Clear();

// 重置为默认值
datePicker.Reset();

// 设置选中日期
datePicker.SelectedDateTime = new DateTime(2024, 6, 15);

// 范围选择器
rangeDatePicker.RangeStartSelectedDate = new DateTime(2024, 1, 1);
rangeDatePicker.RangeEndSelectedDate = new DateTime(2024, 12, 31);

// 清除范围
rangeDatePicker.Clear();

// 重置为默认范围
rangeDatePicker.Reset();
```

---

## 14. 紧凑空间中使用

在 `Space.Compact` 容器中，DatePicker 会自动调整边框和圆角：

```xml
<atom:Space Compact="True">
    <atom:DatePicker atom:CompactSpace.ItemSize="5*" />
    <atom:Button ButtonType="Primary">查询</atom:Button>
    <atom:RangeDatePicker atom:CompactSpace.ItemSize="7*" />
</atom:Space>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/SpaceShowCase.axaml`

---

## 常见组合模式

### 日期筛选栏

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:DatePicker PlaceholderText="Start date" SizeType="Small" />
    <atom:TextBlock VerticalAlignment="Center">至</atom:TextBlock>
    <atom:DatePicker PlaceholderText="End date" SizeType="Small" />
    <atom:Button ButtonType="Primary" SizeType="Small">查询</atom:Button>
</StackPanel>
```

### 范围日期+确认

```xml
<atom:RangeDatePicker IsNeedConfirm="True"
                      PlaceholderText="开始日期"
                      SecondaryPlaceholderText="结束日期" />
```

### 日期时间精确选择

```xml
<atom:DatePicker IsShowTime="True"
                 IsNeedConfirm="True"
                 PlaceholderText="选择日期和时间" />
```

### 表单中使用

```xml
<atom:FormItem Label="出生日期" IsRequired="True">
    <atom:DatePicker PlaceholderText="请选择出生日期" />
</atom:FormItem>

<atom:FormItem Label="有效期" IsRequired="True">
    <atom:RangeDatePicker PlaceholderText="开始日期"
                          SecondaryPlaceholderText="结束日期" />
</atom:FormItem>
```
