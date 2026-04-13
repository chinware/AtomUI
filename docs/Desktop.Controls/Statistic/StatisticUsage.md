# Statistic 使用文档

本文档介绍 AtomUI Statistic 控件家族的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/StatisticShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Statistic，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Statistic, TimerStatistic, StatisticCountUp
```

---

## 1. 基础用法

最简单的 Statistic 用法——展示一个统计数字：

```xml
<atom:Statistic Header="Active Users" Value="112893" />
```

展示带小数精度的数值：

```xml
<atom:Statistic Header="Account Balance (CNY)" Value="112893" Precision="2" />
```

> 📖 参考：Gallery ShowCase 中 "Basic" 示例。

---

## 2. 前缀与后缀

通过 `ValuePrefixAddOn` 和 `ValueSuffixAddOn` 在数值前后添加装饰内容：

### 图标前缀

```xml
<atom:Statistic Header="Feedback" Value="1128"
                ValuePrefixAddOn="{antdicons:AntDesignIconProvider LikeOutlined}" />
```

### 文本后缀

```xml
<atom:Statistic Header="Unmerged" Value="93" ValueSuffixAddOn="/ 100" />
```

### 带图标和百分号的组合

```xml
<atom:Statistic Header="Active" Value="11.28" Precision="2"
                ValuePrefixAddOn="{antdicons:AntDesignIconProvider ArrowUpOutlined}"
                ValueSuffixAddOn="%"
                ContentForeground="#3f8600" />

<atom:Statistic Header="Idle" Value="9.3" Precision="2"
                ValuePrefixAddOn="{antdicons:AntDesignIconProvider ArrowDownOutlined}"
                ValueSuffixAddOn="%"
                ContentForeground="#cf1322" />
```

> 📖 参考：Gallery ShowCase 中 "Unit" 和 "In Card" 示例。

---

## 3. 在卡片中使用

Statistic 常与 Card 组合，在卡片中展示统计数据，这是 Ant Design 的经典用法：

```xml
<Border Background="#F0F2F5" Padding="20, 30">
    <UniformGrid Columns="2" ColumnSpacing="20">
        <atom:Card SizeType="Large">
            <atom:Statistic Header="Active" Value="11.28" Precision="2"
                            ValuePrefixAddOn="{antdicons:AntDesignIconProvider ArrowUpOutlined}"
                            ValueSuffixAddOn="%"
                            ContentForeground="#3f8600" />
        </atom:Card>
        <atom:Card SizeType="Large">
            <atom:Statistic Header="Idle" Value="9.3" Precision="2"
                            ValuePrefixAddOn="{antdicons:AntDesignIconProvider ArrowDownOutlined}"
                            ValueSuffixAddOn="%"
                            ContentForeground="#cf1322" />
        </atom:Card>
    </UniformGrid>
</Border>
```

> 📖 参考：Gallery ShowCase 中 "In Card" 示例。

---

## 4. 加载状态

通过 `IsLoading` 属性控制加载骨架屏，在数据未就绪时显示占位：

```xml
<atom:Statistic Header="Active Users" Value="112893" IsLoading="True" />
```

实际使用中，通常绑定 ViewModel 中的加载状态：

```xml
<atom:Statistic Header="Revenue" Value="{Binding Revenue}"
                IsLoading="{Binding IsDataLoading}" />
```

```csharp
public class DashboardViewModel : ReactiveObject
{
    private bool _isDataLoading = true;
    public bool IsDataLoading
    {
        get => _isDataLoading;
        set => this.RaiseAndSetIfChanged(ref _isDataLoading, value);
    }

    private int _revenue;
    public int Revenue
    {
        get => _revenue;
        set => this.RaiseAndSetIfChanged(ref _revenue, value);
    }
}
```

---

## 5. 数值递增动画（StatisticCountUp）

通过将 `StatisticCountUp` 嵌入 `Statistic.Content` 来实现从 0 到目标值的递增动画：

```xml
<atom:Statistic Header="Active Users">
    <atom:Statistic.Content>
        <atom:StatisticCountUp EndValue="112893" Precision="2" />
    </atom:Statistic.Content>
</atom:Statistic>
```

### 绑定 ViewModel 数据

```xml
<atom:Statistic Header="Account Balance (CNY)">
    <atom:Statistic.Content>
        <atom:StatisticCountUp EndValue="{Binding AccountBalance}"
                               Precision="2" />
    </atom:Statistic.Content>
</atom:Statistic>
```

> 📖 参考：Gallery ShowCase 中 "Animated number" 示例。

**注意事项：**
- `StatisticCountUp` 嵌入 `Statistic.Content` 后，其 `DataContext` 会被自动设为父 `Statistic` 实例
- 动画在控件首次加载时自动触发
- 如果 `EndValue` 在运行时改变，会重新触发动画过渡到新值

---

## 6. 计时器/倒计时（TimerStatistic）

### 基础倒计时

```xml
<atom:TimerStatistic Value="{Binding Deadline}" />
```

### 毫秒精度倒计时

```xml
<atom:TimerStatistic Header="Million Seconds"
                     Value="{Binding Deadline}"
                     Format="hh\:mm\:ss\.fff" />
```

### 短时间倒计时

```xml
<atom:TimerStatistic Header="Countdown"
                     Value="{Binding TenSecondsLater}" />
```

### 正向计时（从过去的时间点开始计时）

```xml
<atom:TimerStatistic Header="Countup"
                     Value="{Binding Before}" />
```

### 天级格式

```xml
<!-- 倒计时天级格式 -->
<atom:TimerStatistic Header="Day Level (Countdown)"
                     Value="{Binding Deadline}"
                     Format="d\ \天\ h\ \时\ m\ \分\ s\ \秒" />

<!-- 计时天级格式 -->
<atom:TimerStatistic Header="Day Level (Countup)"
                     Value="{Binding Before}"
                     Format="d\ \天\ h\ \时\ m\ \分\ s\ \秒" />
```

> 📖 参考：Gallery ShowCase 中 "Timer" 示例。

### ViewModel 设置

```csharp
public class StatisticViewModel : ReactiveObject
{
    private DateTime _deadline;
    public DateTime Deadline
    {
        get => _deadline;
        set => this.RaiseAndSetIfChanged(ref _deadline, value);
    }

    private DateTime _tenSecondsLater;
    public DateTime TenSecondsLater
    {
        get => _tenSecondsLater;
        set => this.RaiseAndSetIfChanged(ref _tenSecondsLater, value);
    }

    private DateTime _before;
    public DateTime Before
    {
        get => _before;
        set => this.RaiseAndSetIfChanged(ref _before, value);
    }
}

// 初始化
viewModel.Deadline = DateTime.Now.Add(TimeSpan.FromSeconds(60 * 60 * 24 * 2 + 30));
viewModel.Before   = DateTime.Now.Subtract(TimeSpan.FromSeconds(60 * 60 * 24 * 2 + 30));
viewModel.TenSecondsLater = DateTime.Now.AddSeconds(10);
```

### 倒计时完成事件

```xml
<atom:TimerStatistic Header="Limited Offer"
                     Value="{Binding OfferDeadline}"
                     CountdownFinished="OnCountdownFinished" />
```

```csharp
private void OnCountdownFinished(object? sender, EventArgs e)
{
    // 倒计时结束，执行后续逻辑
    if (sender is TimerStatistic timer)
    {
        // 例如：禁用某个按钮、跳转页面等
    }
}
```

---

## 7. 自定义格式化

### Statistic 自定义格式化（C# Code-behind）

```csharp
var statistic = new Statistic
{
    Header = "Custom Format",
    Value = 12345.6789,
    Formatter = (s, val) =>
    {
        if (val is double d)
        {
            return $"${d:N2}"; // 输出: $12,345.68
        }
        return val?.ToString();
    }
};
```

### TimerStatistic 自定义格式化

```csharp
var timer = new TimerStatistic
{
    Header = "Custom Timer",
    Value = DateTime.Now.AddHours(2),
    Formatter = remaining =>
    {
        if (remaining.TotalMinutes < 1)
            return $"{remaining.Seconds} 秒";
        if (remaining.TotalHours < 1)
            return $"{remaining.Minutes} 分 {remaining.Seconds} 秒";
        return $"{(int)remaining.TotalHours} 时 {remaining.Minutes} 分";
    }
};
```

---

## 8. 自定义数值颜色

通过 `ContentForeground` 属性设置数值颜色：

```xml
<!-- 绿色表示增长 -->
<atom:Statistic Header="Growth" Value="20.5" Precision="1"
                ContentForeground="#3f8600"
                ValueSuffixAddOn="%" />

<!-- 红色表示下降 -->
<atom:Statistic Header="Decline" Value="5.2" Precision="1"
                ContentForeground="#cf1322"
                ValueSuffixAddOn="%" />
```

---

## 常见组合模式

### 仪表盘统计面板

```xml
<UniformGrid Columns="4" ColumnSpacing="16">
    <atom:Card SizeType="Large">
        <atom:Statistic Header="Total Users" Value="31280" />
    </atom:Card>
    <atom:Card SizeType="Large">
        <atom:Statistic Header="Active Today" Value="2450" />
    </atom:Card>
    <atom:Card SizeType="Large">
        <atom:Statistic Header="Revenue (¥)" Value="89523.45" Precision="2" />
    </atom:Card>
    <atom:Card SizeType="Large">
        <atom:Statistic Header="Conversion" Value="3.28" Precision="2"
                        ValueSuffixAddOn="%" ContentForeground="#3f8600" />
    </atom:Card>
</UniformGrid>
```

### 增减对比组

```xml
<StackPanel Orientation="Horizontal" Spacing="48">
    <atom:Statistic Header="Daily Active" Value="11.28" Precision="2"
                    ValuePrefixAddOn="{antdicons:AntDesignIconProvider ArrowUpOutlined}"
                    ValueSuffixAddOn="%"
                    ContentForeground="#3f8600" />
    <atom:Statistic Header="Weekly Churn" Value="2.3" Precision="1"
                    ValuePrefixAddOn="{antdicons:AntDesignIconProvider ArrowDownOutlined}"
                    ValueSuffixAddOn="%"
                    ContentForeground="#cf1322" />
</StackPanel>
```

### 倒计时 + 操作按钮

```xml
<StackPanel Spacing="16">
    <atom:TimerStatistic Header="Flash Sale Ends In"
                         Value="{Binding SaleDeadline}"
                         Format="hh\:mm\:ss\.fff" />
    <atom:Button ButtonType="Primary" IsDanger="True">
        Buy Now
    </atom:Button>
</StackPanel>
```

### 带动画的 KPI 展示

```xml
<UniformGrid Columns="3" ColumnSpacing="16">
    <atom:Statistic Header="Customers">
        <atom:Statistic.Content>
            <atom:StatisticCountUp EndValue="5280" />
        </atom:Statistic.Content>
    </atom:Statistic>
    <atom:Statistic Header="Orders">
        <atom:Statistic.Content>
            <atom:StatisticCountUp EndValue="12500" />
        </atom:Statistic.Content>
    </atom:Statistic>
    <atom:Statistic Header="Revenue (¥)">
        <atom:Statistic.Content>
            <atom:StatisticCountUp EndValue="895234.50" Precision="2" />
        </atom:Statistic.Content>
    </atom:Statistic>
</UniformGrid>
```
