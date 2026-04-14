# TimePicker 使用文档

本文档介绍 AtomUI TimePicker 和 RangeTimePicker 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TimePickerShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 TimePicker，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // TimePicker, RangeTimePicker 控件
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最简单的 TimePicker 用法，点击输入框弹出时间选择面板：

```xml
<atom:TimePicker PlaceholderText="Select time"
                 IsNeedConfirm="False"
                 IsShowNow="True" />
```

默认配置下：
- 使用 24 小时制
- 显示"此刻"快捷按钮
- 不需要点击确认（双击选项或点击"此刻"直接应用）

---

## 2. 确认模式

启用确认模式后，用户需要点击"确定"按钮才会应用选择的时间：

```xml
<atom:TimePicker PlaceholderText="Select time"
                 IsNeedConfirm="True"
                 IsShowNow="True"
                 ClockIdentifier="HourClock24" />
```

---

## 3. 时钟格式（12/24 小时制）

通过 `ClockIdentifier` 属性切换时钟格式：

```xml
<!-- 12 小时制 -->
<atom:TimePicker PlaceholderText="Select time"
                 DefaultTime="12:08:23"
                 ClockIdentifier="HourClock12" />

<!-- 24 小时制（默认） -->
<atom:TimePicker PlaceholderText="Select time"
                 DefaultTime="12:08:23"
                 ClockIdentifier="HourClock24" />
```

12 小时制下，弹出面板会增加 AM/PM（上午/下午）选择列。

---

## 4. 三种尺寸

通过 `SizeType` 属性设置尺寸，支持 `Large`、`Middle`（默认）、`Small` 三种：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:TimePicker PlaceholderText="Select time"
                     SizeType="Large"
                     DefaultTime="12:08:23" />
    <atom:TimePicker PlaceholderText="Select time"
                     SizeType="Middle"
                     DefaultTime="12:08:23" />
    <atom:TimePicker PlaceholderText="Select time"
                     SizeType="Small"
                     DefaultTime="12:08:23" />
</StackPanel>
```

---

## 5. 禁用状态

通过 `IsEnabled="False"` 禁用时间选择器：

```xml
<atom:TimePicker PlaceholderText="Select time"
                 IsEnabled="False"
                 DefaultTime="12:08:23" />
```

---

## 6. 步进选择

通过 `MinuteIncrement` 和 `SecondIncrement` 控制分钟和秒钟列的候选项粒度：

```xml
<atom:TimePicker PlaceholderText="Select time"
                 DefaultTime="12:08:23"
                 MinuteIncrement="15"
                 SecondIncrement="10" />
```

设置 `MinuteIncrement="15"` 后，分钟列仅显示 `00`、`15`、`30`、`45` 四个选项。

---

## 7. 样式变体（Variants）

TimePicker 支持三种样式变体：Outline（默认）、Filled、Borderless。

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- Outline 样式 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TimePicker PlaceholderText="Outline" StyleVariant="Outline" />
        <atom:RangeTimePicker StyleVariant="Outline" PlaceholderText="Outline" />
    </StackPanel>

    <!-- Filled 样式 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TimePicker PlaceholderText="Filled" StyleVariant="Filled" />
        <atom:RangeTimePicker StyleVariant="Filled" PlaceholderText="Filled" />
    </StackPanel>

    <!-- Borderless 样式 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TimePicker PlaceholderText="Borderless" StyleVariant="Borderless" />
        <atom:RangeTimePicker StyleVariant="Borderless" PlaceholderText="Borderless" />
    </StackPanel>
</StackPanel>
```

---

## 8. 验证状态（Status）

通过 `Status` 属性设置验证状态反馈：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 默认状态 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TimePicker Status="Default" PlaceholderText="Select time" />
        <atom:RangeTimePicker Status="Default"
                              PlaceholderText="Start time"
                              SecondaryPlaceholderText="End time" />
    </StackPanel>

    <!-- 警告状态 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TimePicker Status="Warning" PlaceholderText="Select time" />
        <atom:RangeTimePicker Status="Warning"
                              PlaceholderText="Start time"
                              SecondaryPlaceholderText="End time" />
    </StackPanel>

    <!-- 错误状态 -->
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TimePicker Status="Error" PlaceholderText="Select time" />
        <atom:RangeTimePicker Status="Error"
                              PlaceholderText="Start time"
                              SecondaryPlaceholderText="End time" />
    </StackPanel>
</StackPanel>
```

---

## 9. 时间范围选择器（RangeTimePicker）

使用 `RangeTimePicker` 选择时间范围，提供起始和结束两个输入框：

```xml
<atom:RangeTimePicker PlaceholderText="Start time"
                      SecondaryPlaceholderText="End time"
                      RangeStartDefaultTime="10:09:20"
                      RangeEndDefaultTime="12:12:20" />
```

**交互流程**：
1. 点击起始输入框 → 弹出面板 → 选择起始时间
2. 如果结束时间为空 → 自动切换到结束输入框
3. 两个时间都选定后 → 自动关闭面板

RangeTimePicker 也支持确认模式：

```xml
<atom:RangeTimePicker PlaceholderText="Start time"
                      SecondaryPlaceholderText="End time"
                      IsNeedConfirm="True"
                      ClockIdentifier="HourClock24" />
```

---

## 10. 默认值

通过 `DefaultTime` 设置控件加载时的默认时间：

```xml
<!-- 单选：设置默认时间 -->
<atom:TimePicker PlaceholderText="Select time"
                 DefaultTime="14:30:00" />

<!-- 范围选择：分别设置起止默认时间 -->
<atom:RangeTimePicker PlaceholderText="Start time"
                      SecondaryPlaceholderText="End time"
                      RangeStartDefaultTime="09:00:00"
                      RangeEndDefaultTime="18:00:00" />
```

---

## 11. MVVM 数据绑定

TimePicker 的 `SelectedTime` 属性支持双向绑定，可在 MVVM 模式下使用：

```xml
<atom:TimePicker PlaceholderText="Select time"
                 SelectedTime="{Binding SelectedTime}" />
```

```csharp
// ViewModel（使用 ReactiveUI）
public class MyViewModel : ReactiveObject
{
    [Reactive]
    public TimeSpan? SelectedTime { get; set; }
}
```

RangeTimePicker 的 `RangeStartSelectedTime` 和 `RangeEndSelectedTime` 同样支持双向绑定：

```xml
<atom:RangeTimePicker PlaceholderText="Start time"
                      SecondaryPlaceholderText="End time"
                      RangeStartSelectedTime="{Binding StartTime}"
                      RangeEndSelectedTime="{Binding EndTime}" />
```

---

## 12. Code-Behind 操作

在代码中操作 TimePicker：

```csharp
// 获取选中时间
TimeSpan? selectedTime = myTimePicker.SelectedTime;

// 设置时间
myTimePicker.SelectedTime = new TimeSpan(14, 30, 0);

// 清除值（不考虑默认值）
myTimePicker.Clear();

// 重置为默认值
myTimePicker.Reset();

// 关闭弹出面板
myTimePicker.ClosePickerFlyout();
```

---

## 常见组合模式

### 表单中的时间选择

```xml
<atom:FormItem Label="上班时间">
    <atom:TimePicker PlaceholderText="请选择时间"
                     SizeType="Middle"
                     StyleVariant="Outline" />
</atom:FormItem>
```

### 时间范围筛选

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <TextBlock VerticalAlignment="Center">工作时段：</TextBlock>
    <atom:RangeTimePicker PlaceholderText="开始"
                          SecondaryPlaceholderText="结束"
                          RangeStartDefaultTime="09:00:00"
                          RangeEndDefaultTime="18:00:00" />
</StackPanel>
```

### 12 小时制 + 确认模式

```xml
<atom:TimePicker PlaceholderText="Select time"
                 ClockIdentifier="HourClock12"
                 IsNeedConfirm="True"
                 IsShowNow="True" />
```

### 步进选择 + 紧凑尺寸

```xml
<atom:TimePicker PlaceholderText="Select time"
                 SizeType="Small"
                 MinuteIncrement="15"
                 SecondIncrement="30" />
```
