# Rate 使用文档

本文档介绍 AtomUI Rate 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RateShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Rate，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Rate
using AtomUI.Controls;            // RateValueChangedEventArgs, SizeType
```

---

## 1. 基本用法

最简单的评分，默认 5 颗星，默认值 0：

```xml
<atom:Rate />
```

设置默认值：

```xml
<atom:Rate DefaultValue="3" />
```

> 📖 参考：Gallery 中 "Basic" 示例。

---

## 2. 半星评分

通过 `IsAllowHalf="True"` 启用半星精度，支持 0.5 步长的评分：

```xml
<atom:Rate DefaultValue="3.5" IsAllowHalf="True" />
```

半星模式下：
- 鼠标位于星星**左半区域**时，评分为 `index + 0.5`
- 鼠标位于星星**右半区域**时，评分为 `index + 1`
- 键盘操作步长变为 0.5

> 📖 参考：Gallery 中 "Half star" 示例。

---

## 3. 提示文案

通过 `ToolTips` 属性为每颗星设置悬浮提示文案，结合 `ValueChanged` 事件显示当前评价描述：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Rate Name="ToolTipRate" ValueChanged="HandleValueChanged" />
    <TextBlock Text="{Binding ActiveTooltip}" />
</StackPanel>
```

```csharp
// Code-behind
public partial class RateShowCase : ReactiveUserControl<RateViewModel>
{
    public RateShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is RateViewModel viewModel)
            {
                viewModel.Tooltips = new List<string>
                {
                    "terrible", "bad", "normal", "good", "wonderful"
                };
                this.OneWayBind(viewModel, vm => vm.Tooltips, v => v.ToolTipRate.ToolTips)
                    .DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }

    private void HandleValueChanged(object? sender, RateValueChangedEventArgs e)
    {
        if (DataContext is RateViewModel viewModel)
        {
            var index = (int)Math.Round(e.NewValue, MidpointRounding.AwayFromZero) - 1;
            if (viewModel.Tooltips?.Count > 0 && index >= 0)
            {
                viewModel.ActiveTooltip = viewModel.Tooltips[index];
            }
            else
            {
                viewModel.ActiveTooltip = null;
            }
        }
    }
}
```

> 📖 参考：Gallery 中 "Show copywriting" 示例 及 `RateShowCase.axmal.cs`。

---

## 4. 只读模式

设置 `IsEnabled="False"` 禁止交互，常用于展示已有评分：

```xml
<atom:Rate DefaultValue="2" IsEnabled="False" />
```

只读模式下：
- 鼠标不会变为手型
- 不响应点击和悬浮
- 不响应键盘操作
- 星星不会显示悬浮缩放效果

> 📖 参考：Gallery 中 "Read only" 示例。

---

## 5. 清除行为控制

`IsAllowClear` 控制是否允许再次点击相同值清除评分：

```xml
<StackPanel Spacing="20">
    <!-- 允许清除（默认） -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Rate DefaultValue="3" IsAllowClear="True" />
        <TextBlock>IsAllowClear: true</TextBlock>
    </StackPanel>
    
    <!-- 禁止清除 -->
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Rate DefaultValue="3" IsAllowClear="False" />
        <TextBlock>IsAllowClear: false</TextBlock>
    </StackPanel>
</StackPanel>
```

清除行为说明：
- `IsAllowClear = true`：再次点击当前评分的星星，评分清零
- `IsAllowClear = false`：再次点击不会改变评分，最低保持当前值

> 📖 参考：Gallery 中 "Clear star" 示例。

---

## 6. 自定义字符

通过 `Character` 属性替换默认星星，支持图标、字母、汉字等三种类型：

### 图标字符

```xml
<atom:Rate IsAllowHalf="True" IsAllowClear="True"
           Character="{antdicons:AntDesignIconProvider HeartOutlined}" />
```

### 字母字符

```xml
<atom:Rate IsAllowHalf="True" IsAllowClear="True" Character="A" />
```

### 汉字字符

```xml
<atom:Rate IsAllowHalf="True" IsAllowClear="True" Character="秦" />
```

> 📖 参考：Gallery 中 "Other Character" 示例。

---

## 7. 尺寸控制

通过 `SizeType` 属性控制星星大小，支持三种尺寸：

```xml
<!-- 大号 -->
<atom:Rate SizeType="Large" DefaultValue="3" />

<!-- 中号（默认） -->
<atom:Rate SizeType="Middle" DefaultValue="3" />

<!-- 小号 -->
<atom:Rate SizeType="Small" DefaultValue="3" />
```

---

## 8. 键盘操作

Rate 默认支持键盘操作（`IsKeyboardEnabled = true`）。先点击一颗星获取焦点，然后使用方向键：

- **← 左方向键**：减少评分
- **→ 右方向键**：增加评分
- 半星模式下步长为 0.5，整星模式下步长为 1

禁用键盘操作：

```xml
<atom:Rate IsKeyboardEnabled="False" />
```

---

## 9. 监听事件

### 监听评分值变化

```csharp
myRate.ValueChanged += (sender, args) =>
{
    Console.WriteLine($"评分从 {args.OldValue} 变为 {args.NewValue}");
};
```

### 监听悬浮值变化

```csharp
myRate.HoverValueChanged += (sender, args) =>
{
    Console.WriteLine($"悬浮预览: {args.NewValue}");
};
```

---

## 10. 控制动画行为

```xml
<!-- 禁用悬浮缩放动画 -->
<atom:Rate IsMotionEnabled="False" DefaultValue="3" />
```

---

## 常见组合模式

### 表单中的评分

```xml
<atom:FormItem Label="服务评分">
    <atom:Rate DefaultValue="3" />
</atom:FormItem>
```

### 带数值展示的评分

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:Rate Name="DisplayRate" DefaultValue="3" />
    <TextBlock Text="{Binding #DisplayRate.Value, StringFormat={}{0:0.0}}"
               VerticalAlignment="Center" />
</StackPanel>
```

### 带文案描述的评分

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:Rate Name="DescRate" DefaultValue="3"
               ValueChanged="HandleDescValueChanged" />
    <TextBlock Name="DescText" VerticalAlignment="Center" />
</StackPanel>
```

```csharp
private readonly string[] _descriptions = { "terrible", "bad", "normal", "good", "wonderful" };

private void HandleDescValueChanged(object? sender, RateValueChangedEventArgs e)
{
    var index = (int)Math.Round(e.NewValue, MidpointRounding.AwayFromZero) - 1;
    DescText.Text = index >= 0 && index < _descriptions.Length ? _descriptions[index] : "";
}
```

### 自定义颜色的爱心评分

```xml
<atom:Rate Character="{antdicons:AntDesignIconProvider HeartOutlined}"
           StarColor="Red"
           IsAllowHalf="True"
           DefaultValue="3.5" />
```

### MVVM 数据绑定

```xml
<atom:Rate Value="{Binding Rating, Mode=TwoWay}"
           IsAllowHalf="{Binding AllowHalf}"
           Count="{Binding MaxStars}" />
```

```csharp
// ViewModel
[Reactive]
public double Rating { get; set; } = 3;

[Reactive]
public bool AllowHalf { get; set; } = true;

[Reactive]
public int MaxStars { get; set; } = 5;
```
