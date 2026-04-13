# RadioButton 使用文档

本文档介绍 AtomUI RadioButton 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/RadioButtonShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 RadioButton，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // RadioButton, RadioButtonGroup
using AtomUI.Controls;            // RadioButtonOption, IRadioButtonOption
```

---

## 1. 基本用法

最简单的单选框使用方式：

```xml
<atom:RadioButton>Radio</atom:RadioButton>
```

两个单选框互斥（默认选中一个）：

```xml
<StackPanel Orientation="Horizontal">
    <atom:RadioButton>Option A</atom:RadioButton>
    <atom:RadioButton IsChecked="True">Option B</atom:RadioButton>
</StackPanel>
```

---

## 2. 禁用状态

通过 `IsEnabled="False"` 禁用单选框：

```xml
<StackPanel Orientation="Vertical">
    <StackPanel Orientation="Horizontal">
        <atom:RadioButton x:Name="ToggleDisabledRadioUnChecked">Radio1</atom:RadioButton>
        <atom:RadioButton x:Name="ToggleDisabledRadioChecked" IsChecked="True">Radio2</atom:RadioButton>
    </StackPanel>
    <atom:Button ButtonType="Primary"
                 Margin="0, 20, 0, 0"
                 Click="ToggleDisabledStatus">
        toggle disabled
    </atom:Button>
</StackPanel>
```

```csharp
public void ToggleDisabledStatus(object? sender, RoutedEventArgs args)
{
    ToggleDisabledRadioUnChecked.IsEnabled = !ToggleDisabledRadioUnChecked.IsEnabled;
    ToggleDisabledRadioChecked.IsEnabled   = !ToggleDisabledRadioChecked.IsEnabled;
}
```

禁用时的视觉变化：
- 文本变为 `ColorTextDisabled`（灰色）
- 指示器背景变为 `ColorBgContainerDisabled`（灰色背景）
- 如果选中，圆点变为 `DotColorDisabled`（灰色圆点）
- 不可点击，鼠标样式不会变为手型

> 📖 参考：Gallery 中 "Disabled" 示例。

---

## 3. 单选框组（RadioButtonGroup）

### 声明式用法

直接在 AXAML 中声明 RadioButton 子元素：

```xml
<!-- 水平排列（默认） -->
<atom:RadioButtonGroup>
    <atom:RadioButton>Option A</atom:RadioButton>
    <atom:RadioButton>Option B</atom:RadioButton>
    <atom:RadioButton>Option C</atom:RadioButton>
</atom:RadioButtonGroup>
```

### 垂直排列

```xml
<atom:RadioButtonGroup Orientation="Vertical" HorizontalAlignment="Left">
    <atom:RadioButton>Option A</atom:RadioButton>
    <atom:RadioButton>Option B</atom:RadioButton>
    <atom:RadioButton>Option C</atom:RadioButton>
    <atom:RadioButton>Option D</atom:RadioButton>
</atom:RadioButtonGroup>
```

### 自定义内容（图标 + 文字）

RadioButton 的 `Content` 属性接受任意控件，可以用来创建带图标的选项：

```xml
<atom:RadioButtonGroup>
    <atom:RadioButton>
        <StackPanel Spacing="5" Orientation="Vertical">
            <antdicons:LineChartOutlined Width="18" Height="18" />
            <TextBlock>LineChart</TextBlock>
        </StackPanel>
    </atom:RadioButton>
    <atom:RadioButton>
        <StackPanel Spacing="5" Orientation="Vertical">
            <antdicons:DotChartOutlined Width="18" Height="18" />
            <TextBlock>DotChart</TextBlock>
        </StackPanel>
    </atom:RadioButton>
    <atom:RadioButton>
        <StackPanel Spacing="5" Orientation="Vertical">
            <antdicons:BarChartOutlined Width="18" Height="18" />
            <TextBlock>BarChart</TextBlock>
        </StackPanel>
    </atom:RadioButton>
    <atom:RadioButton>
        <StackPanel Spacing="5" Orientation="Vertical">
            <antdicons:PieChartOutlined Width="18" Height="18" />
            <TextBlock>PieChart</TextBlock>
        </StackPanel>
    </atom:RadioButton>
</atom:RadioButtonGroup>
```

> 📖 参考：Gallery 中 "Radio Group" 示例。

### 数据驱动用法

通过 `ItemsSource` 绑定 `IRadioButtonOption` 集合，动态生成子项：

```xml
<atom:RadioButtonGroup Name="RadioButtonGroup" HorizontalAlignment="Left" />
```

```csharp
// Code-behind 或 ViewModel
var options = new List<RadioButtonOption>
{
    new() { Content = "Option A" },
    new() { Content = "Option B", IsChecked = true },
    new() { Content = "Option C" },
    new() { Content = "Option D", IsEnabled = false },
};
RadioButtonGroup.ItemsSource = options;
```

使用 ReactiveUI 数据绑定模式：

```csharp
this.OneWayBind(ViewModel, vm => vm.RadioOptions, v => v.RadioButtonGroup.ItemsSource)
    .DisposeWith(disposables);
```

> 📖 参考：Gallery 中 "Radio Group by Items source" 示例 及 `RadioButtonShowCase.axaml.cs`。

### 监听选中变化

```csharp
// 声明式用法 — CheckedItem 返回 RadioButton 实例
RadioButtonGroup.CheckedChanged += (sender, args) =>
{
    if (args.NewCheckedItem is RadioButton radioButton)
    {
        Console.WriteLine($"Selected: {radioButton.Content}");
    }
};

// 数据驱动用法 — CheckedItem 返回数据对象
RadioButtonGroup.CheckedChanged += (sender, args) =>
{
    if (args.NewCheckedItem is RadioButtonOption option)
    {
        Console.WriteLine($"Selected: {option.Content}");
    }
};
```

---

## 4. 按钮样式单选框（OptionButtonGroup）

Ant Design 的 `Radio.Button` 在 AtomUI 中通过 `OptionButtonGroup` + `OptionButton` 实现：

### Outline 样式（默认）

```xml
<atom:OptionButtonGroup ButtonStyle="Outline">
    <atom:OptionButton>Apple</atom:OptionButton>
    <atom:OptionButton IsChecked="True">Pear</atom:OptionButton>
    <atom:OptionButton>Orange</atom:OptionButton>
</atom:OptionButtonGroup>
```

### Solid 填充样式

```xml
<atom:OptionButtonGroup ButtonStyle="Solid">
    <atom:OptionButton IsChecked="True">Apple</atom:OptionButton>
    <atom:OptionButton>Pear</atom:OptionButton>
    <atom:OptionButton>Orange</atom:OptionButton>
</atom:OptionButtonGroup>
```

### 带图标的按钮样式

```xml
<atom:OptionButtonGroup ButtonStyle="Solid">
    <atom:OptionButton IsChecked="True"
                       Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">
        macOS
    </atom:OptionButton>
    <atom:OptionButton Icon="{antdicons:AntDesignIconProvider Kind=LinuxOutlined}">
        Linux
    </atom:OptionButton>
    <atom:OptionButton Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}">
        Windows
    </atom:OptionButton>
</atom:OptionButtonGroup>
```

### 三种尺寸

```xml
<atom:OptionButtonGroup SizeType="Large">
    <atom:OptionButton IsChecked="True">Hangzhou</atom:OptionButton>
    <atom:OptionButton>Shanghai</atom:OptionButton>
    <atom:OptionButton>Beijing</atom:OptionButton>
    <atom:OptionButton>Chengdu</atom:OptionButton>
</atom:OptionButtonGroup>

<atom:OptionButtonGroup SizeType="Middle">
    <atom:OptionButton IsChecked="True">Hangzhou</atom:OptionButton>
    <atom:OptionButton>Shanghai</atom:OptionButton>
    <atom:OptionButton>Beijing</atom:OptionButton>
    <atom:OptionButton>Chengdu</atom:OptionButton>
</atom:OptionButtonGroup>

<atom:OptionButtonGroup SizeType="Small">
    <atom:OptionButton IsChecked="True">Hangzhou</atom:OptionButton>
    <atom:OptionButton>Shanghai</atom:OptionButton>
    <atom:OptionButton>Beijing</atom:OptionButton>
    <atom:OptionButton>Chengdu</atom:OptionButton>
</atom:OptionButtonGroup>
```

### 禁用状态

```xml
<!-- 部分禁用 -->
<atom:OptionButtonGroup>
    <atom:OptionButton IsChecked="True">Hangzhou</atom:OptionButton>
    <atom:OptionButton IsEnabled="False">Shanghai</atom:OptionButton>
    <atom:OptionButton>Beijing</atom:OptionButton>
</atom:OptionButtonGroup>

<!-- 整组禁用 -->
<atom:OptionButtonGroup ButtonStyle="Solid" IsEnabled="False">
    <atom:OptionButton IsChecked="True">macOS</atom:OptionButton>
    <atom:OptionButton>Linux</atom:OptionButton>
    <atom:OptionButton>Windows</atom:OptionButton>
</atom:OptionButtonGroup>
```

> 📖 参考：Gallery 中 "Option Button"、"Option Button with icon"、"option style"、"Solid option button"、"Size type" 示例。

---

## 5. 控制动画行为

```xml
<!-- 禁用选中波纹效果 -->
<atom:RadioButton IsWaveSpiritEnabled="False">No Wave</atom:RadioButton>

<!-- 禁用过渡动画 -->
<atom:RadioButton IsMotionEnabled="False">No Animation</atom:RadioButton>

<!-- 通过组统一禁用动画（传递给所有子项） -->
<atom:RadioButtonGroup IsMotionEnabled="False">
    <atom:RadioButton>A</atom:RadioButton>
    <atom:RadioButton>B</atom:RadioButton>
</atom:RadioButtonGroup>
```

---

## 常见组合模式

### 表单中使用

```xml
<atom:FormItem Label="性别">
    <atom:RadioButtonGroup>
        <atom:RadioButton>男</atom:RadioButton>
        <atom:RadioButton>女</atom:RadioButton>
        <atom:RadioButton>其他</atom:RadioButton>
    </atom:RadioButtonGroup>
</atom:FormItem>
```

### 切换视图模式

使用按钮样式单选框在几种模式间切换：

```xml
<atom:OptionButtonGroup ButtonStyle="Solid">
    <atom:OptionButton IsChecked="True"
                       Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}" />
    <atom:OptionButton
                       Icon="{antdicons:AntDesignIconProvider Kind=UnorderedListOutlined}" />
</atom:OptionButtonGroup>
```

### MVVM 数据绑定

```xml
<atom:RadioButtonGroup CheckedItem="{Binding SelectedOption, Mode=TwoWay}">
    <atom:RadioButton>Option A</atom:RadioButton>
    <atom:RadioButton>Option B</atom:RadioButton>
    <atom:RadioButton>Option C</atom:RadioButton>
</atom:RadioButtonGroup>
```

### 混合禁用项

```xml
<StackPanel Orientation="Horizontal" HorizontalAlignment="Left">
    <atom:RadioButton IsChecked="True">Apple</atom:RadioButton>
    <atom:RadioButton>Pear</atom:RadioButton>
    <atom:RadioButton IsEnabled="False">Orange</atom:RadioButton>
</StackPanel>
```

> 📖 参考：Gallery 中 "Option Button" 示例中的第二个 StackPanel。
