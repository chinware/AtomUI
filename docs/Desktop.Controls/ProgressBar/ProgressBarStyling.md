# ProgressBar 自定义样式指南

ProgressBar 家族（ProgressBar、CircleProgress、DashboardProgress、StepsProgressBar）的视觉表现通过自定义渲染 + Design Token 系统控制。由于进度条采用 `DrawingContext` 直接绘制，样式自定义主要通过属性控制和 Token 覆盖实现。

---

## 1. 使用属性直接控制

### 线形进度条（ProgressBar）

```xml
<!-- 基本用法 -->
<atom:ProgressBar Value="30" Minimum="0" Maximum="100" />

<!-- 不同状态 -->
<atom:ProgressBar Value="70" Status="Exception" />
<atom:ProgressBar Value="100" />   <!-- 自动显示完成图标 -->

<!-- 隐藏进度信息 -->
<atom:ProgressBar Value="50" IsShowProgressInfo="False" />

<!-- 不同尺寸 -->
<atom:ProgressBar Value="50" SizeType="Large" />
<atom:ProgressBar Value="50" SizeType="Middle" />
<atom:ProgressBar Value="50" SizeType="Small" />

<!-- 自定义线宽 -->
<atom:ProgressBar Value="50" IndicatorThickness="20" />

<!-- 方角端点 -->
<atom:ProgressBar Value="75" StrokeLineCap="Square" />

<!-- 垂直方向 -->
<atom:ProgressBar Value="55" Orientation="Vertical" Height="300" />
```

### 圆形进度条（CircleProgress）

```xml
<!-- 基本用法 -->
<atom:CircleProgress Value="75" Minimum="0" Maximum="100" />

<!-- 不同尺寸 -->
<atom:CircleProgress Value="75" SizeType="Large" />
<atom:CircleProgress Value="75" SizeType="Middle" />
<atom:CircleProgress Value="75" SizeType="Small" />

<!-- 精确尺寸控制 -->
<atom:CircleProgress Value="50" Width="20" Height="20" />

<!-- 自定义线宽 -->
<atom:CircleProgress Value="50" IndicatorThickness="20" />

<!-- 步进段 -->
<atom:CircleProgress Value="50" StepCount="4" StepGap="8" IndicatorThickness="20" />

<!-- 自定义文字格式 -->
<atom:CircleProgress Value="75" ProgressTextFormat="\{0\} Days" />
```

### 仪表盘进度条（DashboardProgress）

```xml
<!-- 不同缺口位置 -->
<atom:DashboardProgress Value="75" DashboardGapPosition="Left" />
<atom:DashboardProgress Value="60" DashboardGapPosition="Top" />
<atom:DashboardProgress Value="75" DashboardGapPosition="Right" GapDegree="40" />
<atom:DashboardProgress Value="100" DashboardGapPosition="Bottom" GapDegree="40" />

<!-- 步进段仪表盘 -->
<atom:DashboardProgress Value="50" StepCount="4" StepGap="8" IndicatorThickness="20" />
```

### 步骤进度条（StepsProgressBar）

```xml
<!-- 基本用法 -->
<atom:StepsProgressBar Value="50" Steps="3" />
<atom:StepsProgressBar Value="30" Steps="5" />
<atom:StepsProgressBar Value="100" Steps="5" SizeType="Middle" />
<atom:StepsProgressBar Value="80" Steps="8" SizeType="Small" />

<!-- 自定义块尺寸 -->
<atom:StepsProgressBar Value="50" Steps="3" ChunkHeight="20" ChunkWidth="20" />

<!-- 垂直方向 -->
<atom:StepsProgressBar Value="55" Steps="5" Orientation="Vertical" Height="300" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ProgressBarShowCase.axaml`

---

## 2. 自定义颜色

### 自定义描边颜色

```xml
<!-- 纯色 -->
<atom:ProgressBar Value="70" StrokeBrush="#001342" />

<!-- 线性渐变 -->
<atom:ProgressBar Value="99">
    <atom:ProgressBar.StrokeBrush>
        <LinearGradientBrush StartPoint="0%,0%" EndPoint="100%,0%">
            <GradientStop Color="#108ee9" Offset="0" />
            <GradientStop Color="#87d068" Offset="1" />
        </LinearGradientBrush>
    </atom:ProgressBar.StrokeBrush>
</atom:ProgressBar>

<!-- 圆形渐变 -->
<atom:CircleProgress Value="90">
    <atom:CircleProgress.StrokeBrush>
        <LinearGradientBrush StartPoint="0%,0%" EndPoint="100%,0%">
            <GradientStop Color="#108ee9" Offset="0" />
            <GradientStop Color="#87d068" Offset="1" />
        </LinearGradientBrush>
    </atom:CircleProgress.StrokeBrush>
</atom:CircleProgress>
```

### 自定义轨道颜色

```xml
<atom:ProgressBar Value="50" TrailColor="LightGray" />
```

### 每个步骤独立颜色

```xml
<!-- 通过绑定设置步骤颜色列表 -->
<atom:StepsProgressBar Value="60" Steps="5"
                       StepsStrokeBrush="{Binding StepsChunkBrushes}" />
```

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

## 3. 成功阈值

```xml
<!-- 线形 -->
<atom:ProgressBar Value="60" SuccessThreshold="30" />

<!-- 圆形 -->
<atom:CircleProgress Value="60" SuccessThreshold="30" />

<!-- 仪表盘 + 自定义成功色 -->
<atom:DashboardProgress Value="60" SuccessThreshold="30" SuccessStrokeBrush="Chocolate" />

<!-- 步进段 + 成功阈值 -->
<atom:CircleProgress Value="77" StepCount="8" StepGap="10"
                     IndicatorThickness="20" SuccessThreshold="30" />
```

---

## 4. 百分比标签位置（ProgressBar）

ProgressBar 支持通过 `PercentPosition` 精确控制百分比标签位置：

```csharp
// ViewModel 中定义不同位置
public PercentPosition InnerStartPercentPosition { get; } = new() { IsInner = true, Alignment = LinePercentAlignment.Start };
public PercentPosition InnerCenterPercentPosition { get; } = new() { IsInner = true, Alignment = LinePercentAlignment.Center };
public PercentPosition InnerEndPercentPosition { get; } = new() { IsInner = true, Alignment = LinePercentAlignment.End };
public PercentPosition OuterStartPercentPosition { get; } = new() { IsInner = false, Alignment = LinePercentAlignment.Start };
public PercentPosition OuterCenterPercentPosition { get; } = new() { IsInner = false, Alignment = LinePercentAlignment.Center };
```

```xml
<!-- Inner 模式：标签在进度条内部 -->
<atom:ProgressBar Value="30" Width="300"
                  PercentPosition="{Binding InnerStartPercentPosition}" />
<atom:ProgressBar Value="60" Width="300"
                  PercentPosition="{Binding InnerCenterPercentPosition}" />
<atom:ProgressBar Value="50" Width="300"
                  PercentPosition="{Binding InnerEndPercentPosition}" />

<!-- Outer 模式：标签在进度条外部 -->
<atom:ProgressBar Value="100"
                  PercentPosition="{Binding OuterStartPercentPosition}" />
<atom:ProgressBar Value="60"
                  PercentPosition="{Binding OuterCenterPercentPosition}" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ProgressBarShowCase.axaml` 中 "Change progress value position" 示例。

---

## 5. 通过 Style 覆盖样式

```xml
<Window.Styles>
    <!-- 统一圆形进度条间距 -->
    <Style Selector="atom|CircleProgress">
        <Setter Property="Margin" Value="5" />
    </Style>
    
    <!-- 统一仪表盘间距 -->
    <Style Selector="atom|DashboardProgress">
        <Setter Property="Margin" Value="5" />
    </Style>
    
    <!-- 所有线形进度条使用方角 -->
    <Style Selector="atom|ProgressBar">
        <Setter Property="StrokeLineCap" Value="Square" />
    </Style>
    
    <!-- 异常状态使用自定义前景色 -->
    <Style Selector="atom|ProgressBar[Status=Exception]">
        <Setter Property="Foreground" Value="DarkRed" />
    </Style>
</Window.Styles>
```

---

## 6. 动态交互示例

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

```csharp
public class ProgressBarViewModel : ReactiveObject
{
    [Reactive] public double ProgressValue { get; set; } = 50;
    
    public ReactiveCommand<Unit, Unit> AddProgressValue { get; }
    public ReactiveCommand<Unit, Unit> SubProgressValue { get; }
    
    public ProgressBarViewModel()
    {
        AddProgressValue = ReactiveCommand.Create(() => 
        {
            ProgressValue = Math.Min(100, ProgressValue + 10);
        });
        SubProgressValue = ReactiveCommand.Create(() => 
        {
            ProgressValue = Math.Max(0, ProgressValue - 10);
        });
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ProgressBarShowCase.axaml` 中 "Dynamic" 示例。

---

## 样式选择器速查

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|ProgressBar` | 匹配所有线形进度条 |
| `atom\|CircleProgress` | 匹配所有圆形进度条 |
| `atom\|DashboardProgress` | 匹配所有仪表盘进度条 |
| `atom\|StepsProgressBar` | 匹配所有步骤进度条 |

### 按状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|ProgressBar[Status=Normal]` | Normal 状态的线形进度条 |
| `atom\|ProgressBar[Status=Exception]` | 异常状态的线形进度条 |
| `atom\|ProgressBar[Status=Active]` | 活跃状态的线形进度条 |
| `atom\|CircleProgress[Status=Exception]` | 异常状态的圆形进度条 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|ProgressBar[SizeType=Large]` | 大号线形进度条 |
| `atom\|ProgressBar[SizeType=Small]` | 小号线形进度条 |
| `atom\|CircleProgress[SizeType=Middle]` | 中号圆形进度条 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|ProgressBar:completed` | 已完成的进度条（Value == Maximum） |
| `atom\|ProgressBar:indeterminate` | 不定态进度条 |
| `atom\|ProgressBar:horizontal` | 水平方向进度条 |
| `atom\|ProgressBar:vertical` | 垂直方向进度条 |
| `atom\|ProgressBar:labelinner` | 百分比标签在内部的进度条 |
| `atom\|ProgressBar:disabled` | 禁用状态 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|ProgressBar[Status=Exception]:completed` | 异常但已完成的进度条 |
| `atom\|ProgressBar:vertical[SizeType=Small]` | 垂直方向小号进度条 |
| `atom\|StepsProgressBar:horizontal[SizeType=Large]` | 水平方向大号步骤进度条 |
