# ProgressBar 使用文档

本文档介绍 AtomUI ProgressBar 家族四种控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ProgressBarShowCase.axaml`

---

## 前置准备

在 AXAML 中使用进度条，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // ProgressBar, CircleProgress, DashboardProgress, StepsProgressBar
using AtomUI.Controls;            // SizeType, ProgressStatus, PercentPosition 等共享类型
```

---

## 1. 线形进度条（ProgressBar）

最基础的进度条类型，展示水平或垂直方向的线形进度。

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ProgressBar Value="30" Minimum="0" Maximum="100" />
    <atom:ProgressBar Value="50" Minimum="0" Maximum="100" />
    <atom:ProgressBar Value="70" Minimum="0" Maximum="100" Status="Exception" />
    <atom:ProgressBar Value="100" Minimum="0" Maximum="100" />
    <atom:ProgressBar Value="50" Minimum="0" Maximum="100" IsShowProgressInfo="False" />
</StackPanel>
```

**行为说明**：
- `Value=100` 时自动切换为成功色并显示 ✓ 图标
- `Status="Exception"` 时使用红色并显示 ✗ 图标
- `IsShowProgressInfo="False"` 隐藏百分比文字

---

## 2. 圆形进度条（CircleProgress）

在紧凑空间中展示进度。

```xml
<WrapPanel Orientation="Horizontal">
    <atom:CircleProgress Value="75" Minimum="0" Maximum="100" />
    <atom:CircleProgress Value="70" Minimum="0" Maximum="100" Status="Exception" />
    <atom:CircleProgress Value="100" Minimum="0" Maximum="100" />
</WrapPanel>
```

---

## 3. 尺寸控制

所有进度条支持三种尺寸，通过 `SizeType` 属性控制：

### 线形进度条

```xml
<StackPanel Orientation="Vertical" HorizontalAlignment="Left" Width="300">
    <atom:ProgressBar Value="50" SizeType="Large" />
    <atom:ProgressBar Value="50" SizeType="Middle" />
    <atom:ProgressBar Value="50" SizeType="Small" />
    <atom:ProgressBar Value="50" IndicatorThickness="20" />  <!-- 自定义线宽 -->
</StackPanel>
```

### 圆形进度条

```xml
<WrapPanel Orientation="Horizontal">
    <atom:CircleProgress Value="50" SizeType="Large" />    <!-- 120px -->
    <atom:CircleProgress Value="50" SizeType="Middle" />   <!-- 90px -->
    <atom:CircleProgress Value="50" SizeType="Small" />    <!-- 60px -->
    <atom:CircleProgress Value="50" Width="20" Height="20" />  <!-- 精确尺寸 -->
</WrapPanel>
```

### 仪表盘进度条

```xml
<WrapPanel Orientation="Horizontal">
    <atom:DashboardProgress Value="50" SizeType="Large" />
    <atom:DashboardProgress Value="50" SizeType="Middle" />
    <atom:DashboardProgress Value="50" SizeType="Small" />
    <atom:DashboardProgress Value="50" Width="20" Height="20" />
</WrapPanel>
```

### 步骤进度条

```xml
<WrapPanel Orientation="Horizontal">
    <atom:StepsProgressBar Value="50" Steps="3" SizeType="Large" />
    <atom:StepsProgressBar Value="50" Steps="3" SizeType="Middle" />
    <atom:StepsProgressBar Value="50" Steps="3" SizeType="Small" />
    <atom:StepsProgressBar Value="50" Steps="3" ChunkHeight="20" ChunkWidth="20" />
    <atom:StepsProgressBar Value="50" Steps="3" ChunkHeight="30" ChunkWidth="20" />
</WrapPanel>
```

---

## 4. 仪表盘进度条（DashboardProgress）

仪表盘形态支持设置缺口位置和缺口角度：

```xml
<WrapPanel Orientation="Horizontal">
    <atom:DashboardProgress Value="75" DashboardGapPosition="Left" />
    <atom:DashboardProgress Value="60" DashboardGapPosition="Top" />
    <atom:DashboardProgress Value="75" DashboardGapPosition="Right" GapDegree="40" />
    <atom:DashboardProgress Value="100" DashboardGapPosition="Bottom" GapDegree="40" />
</WrapPanel>
```

---

## 5. 步骤进度条（StepsProgressBar）

将进度分为离散的块状步骤：

```xml
<StackPanel Orientation="Vertical" Spacing="5">
    <atom:StepsProgressBar Value="50" Steps="3" />
    <atom:StepsProgressBar Value="30" Steps="5" />
    <atom:StepsProgressBar Value="100" Steps="5" SizeType="Middle" />
    <atom:StepsProgressBar Value="80" Steps="8" SizeType="Small" />
    <atom:StepsProgressBar Value="60" Steps="5"
                           StepsStrokeBrush="{Binding StepsChunkBrushes}" />
</StackPanel>
```

每个步骤可以设置独立颜色：

```csharp
// ViewModel
public List<IBrush> StepsChunkBrushes { get; } = new()
{
    new SolidColorBrush(Colors.Green),
    new SolidColorBrush(Colors.Green),
    new SolidColorBrush(Colors.Orange),
    new SolidColorBrush(Colors.Red),
    new SolidColorBrush(Colors.Red),
};
```

---

## 6. 成功阈值

展示部分完成的成功段：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ProgressBar Value="60" SuccessThreshold="30" />
    <WrapPanel Orientation="Horizontal">
        <atom:CircleProgress Value="60" SuccessThreshold="30" />
        <atom:DashboardProgress Value="60" SuccessThreshold="30"
                                SuccessStrokeBrush="Chocolate" />
    </WrapPanel>
</StackPanel>
```

---

## 7. 端点样式（StrokeLineCap）

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ProgressBar Value="75" StrokeLineCap="Square" />
    <WrapPanel Orientation="Horizontal">
        <atom:CircleProgress Value="75" StrokeLineCap="Square" />
        <atom:DashboardProgress Value="75" StrokeLineCap="Square" />
    </WrapPanel>
</StackPanel>
```

---

## 8. 自定义渐变颜色

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 线形渐变 -->
    <atom:ProgressBar Value="99" StrokeBrush="{Binding TwoStopsGradientStrokeColor}" />
    <atom:ProgressBar Value="50" StrokeBrush="{Binding TwoStopsGradientStrokeColor}" Status="Active" />

    <!-- 圆形渐变 -->
    <WrapPanel Orientation="Horizontal">
        <atom:CircleProgress Value="90" StrokeBrush="{Binding TwoStopsGradientStrokeColor}" />
        <atom:CircleProgress Value="100" StrokeBrush="{Binding TwoStopsGradientStrokeColor}" />
        <atom:CircleProgress Value="93" StrokeBrush="{Binding ThreeStopsGradientStrokeColor}" />
    </WrapPanel>

    <!-- 仪表盘渐变 -->
    <WrapPanel Orientation="Horizontal">
        <atom:DashboardProgress Value="90" StrokeLineCap="Square"
                                StrokeBrush="{Binding TwoStopsGradientStrokeColor}" />
        <atom:DashboardProgress Value="100" StrokeLineCap="Square"
                                StrokeBrush="{Binding TwoStopsGradientStrokeColor}" />
        <atom:DashboardProgress Value="93" StrokeLineCap="Square"
                                StrokeBrush="{Binding ThreeStopsGradientStrokeColor}" />
    </WrapPanel>
</StackPanel>
```

---

## 9. 圆形/仪表盘步进段

圆形和仪表盘进度条支持分段显示：

```xml
<StackPanel Orientation="Vertical" Spacing="5">
    <WrapPanel Orientation="Horizontal">
        <atom:CircleProgress Value="50" StepCount="4" StepGap="8" IndicatorThickness="20" />
        <atom:CircleProgress Value="100" StepCount="10" StepGap="8" IndicatorThickness="20" />
        <atom:CircleProgress Value="77" StepCount="8" StepGap="10"
                             IndicatorThickness="20" Status="Exception" />
        <atom:CircleProgress Value="77" StepCount="8" StepGap="10"
                             IndicatorThickness="20" SuccessThreshold="30" />
    </WrapPanel>
    <WrapPanel Orientation="Horizontal">
        <atom:DashboardProgress Value="50" StepCount="4" StepGap="8" IndicatorThickness="20" />
        <atom:DashboardProgress Value="70" StepCount="10" StepGap="8" IndicatorThickness="20" />
        <atom:DashboardProgress Value="77" StepCount="8" StepGap="10"
                                IndicatorThickness="20" Status="Exception" />
        <atom:DashboardProgress Value="77" StepCount="8" StepGap="10"
                                IndicatorThickness="20" SuccessThreshold="30" />
    </WrapPanel>
</StackPanel>
```

---

## 10. 自定义文字格式

```xml
<WrapPanel Orientation="Horizontal">
    <atom:CircleProgress Value="75" ProgressTextFormat="\{0\} Days" />
    <atom:CircleProgress Value="100" />
</WrapPanel>
```

`ProgressTextFormat` 使用 C# 标准格式化字符串，`{0}` 为当前百分比值。

---

## 11. 百分比标签位置

### ProgressBar 内外标签

```xml
<!-- Inner 模式 -->
<atom:ProgressBar Value="30" Width="300"
                  PercentPosition="{Binding InnerStartPercentPosition}" />
<atom:ProgressBar Value="60" Width="300"
                  PercentPosition="{Binding InnerCenterPercentPosition}" />
<atom:ProgressBar Value="50" Width="300"
                  PercentPosition="{Binding InnerEndPercentPosition}" />

<!-- Outer 模式 -->
<atom:ProgressBar Value="100"
                  PercentPosition="{Binding OutterStartPercentPosition}" />
<atom:ProgressBar Value="60"
                  PercentPosition="{Binding OutterCenterPercentPosition}" SizeType="Small" />
```

### StepsProgressBar 标签位置

```xml
<atom:StepsProgressBar Value="100" Steps="8" PercentPosition="Start" />
<atom:StepsProgressBar Value="100" Steps="8" PercentPosition="Center" />
<atom:StepsProgressBar Value="100" Steps="8" PercentPosition="End" />
```

---

## 12. 垂直方向

ProgressBar 和 StepsProgressBar 支持垂直方向：

### 线形

```xml
<StackPanel Orientation="Horizontal" Spacing="10" Height="300">
    <atom:ProgressBar Value="100" Orientation="Vertical" />
    <atom:ProgressBar Value="55" Orientation="Vertical" />
    <atom:ProgressBar Value="55" Orientation="Vertical" SizeType="Small" />
    <atom:ProgressBar Value="55" Orientation="Vertical"
                      PercentPosition="{Binding InnerEndPercentPosition}" />
</StackPanel>
```

### 步骤

```xml
<StackPanel Orientation="Horizontal" Spacing="10" Height="300">
    <atom:StepsProgressBar Value="100" Steps="10" Orientation="Vertical"
                           PercentPosition="End" />
    <atom:StepsProgressBar Value="55" Steps="5" Orientation="Vertical" />
    <atom:StepsProgressBar Value="55" Steps="10" Orientation="Vertical"
                           SizeType="Small" />
    <atom:StepsProgressBar Value="55" Steps="6" Orientation="Vertical"
                           PercentPosition="Start" />
</StackPanel>
```

---

## 13. 动态进度

通过数据绑定实现动态进度更新：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ProgressBar Value="{Binding ProgressValue}" IsShowProgressInfo="False" />
    <atom:CircleProgress Value="{Binding ProgressValue}" />
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button SizeType="Small" Command="{Binding SubProgressValue}">Sub</atom:Button>
        <atom:Button SizeType="Small" Command="{Binding AddProgressValue}">Add</atom:Button>
    </StackPanel>
</StackPanel>
```

---

## 14. 禁用状态

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:ProgressBar Value="30" IsEnabled="{Binding ToggleStatus}" />
    <atom:StepsProgressBar Value="30" Steps="10" IsEnabled="{Binding ToggleStatus}" />
    <WrapPanel Orientation="Horizontal">
        <atom:CircleProgress Value="75" SizeType="Middle" IsEnabled="{Binding ToggleStatus}" />
        <atom:DashboardProgress Value="75" SizeType="Middle" IsEnabled="{Binding ToggleStatus}" />
    </WrapPanel>
    <atom:Button Content="{Binding ToggleDisabledText}" Command="{Binding ToggleEnabledStatus}" />
</StackPanel>
```

---

## 常见组合模式

### 文件上传进度

```xml
<StackPanel Orientation="Vertical" Spacing="4">
    <TextBlock Text="Uploading file.zip..." />
    <atom:ProgressBar Value="{Binding UploadProgress}" SizeType="Small"
                      IsShowProgressInfo="True" />
</StackPanel>
```

### 仪表板统计

```xml
<WrapPanel Orientation="Horizontal" Spacing="16">
    <StackPanel Spacing="4" HorizontalAlignment="Center">
        <atom:CircleProgress Value="85" SizeType="Middle" />
        <TextBlock Text="CPU" HorizontalAlignment="Center" />
    </StackPanel>
    <StackPanel Spacing="4" HorizontalAlignment="Center">
        <atom:CircleProgress Value="62" SizeType="Middle" />
        <TextBlock Text="Memory" HorizontalAlignment="Center" />
    </StackPanel>
    <StackPanel Spacing="4" HorizontalAlignment="Center">
        <atom:DashboardProgress Value="45" SizeType="Middle" />
        <TextBlock Text="Disk" HorizontalAlignment="Center" />
    </StackPanel>
</WrapPanel>
```

### 步骤流程

```xml
<StackPanel Orientation="Vertical" Spacing="8">
    <TextBlock Text="Step 3 of 5: Processing data..." />
    <atom:StepsProgressBar Value="60" Steps="5" SizeType="Large" />
</StackPanel>
```
