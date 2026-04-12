# CheckBox 使用文档

本文档介绍 AtomUI CheckBox 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/CheckBoxShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 CheckBox，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // CheckBox, CheckBoxGroup
using AtomUI.Controls;            // CheckBoxOption, ICheckBoxOption
```

---

## 1. 基本用法

最简单的复选框，通过 `Content` 设置标签文本：

```xml
<atom:CheckBox>Checkbox</atom:CheckBox>
```

---

## 2. 禁用状态

通过 `IsEnabled="False"` 禁用复选框，三种状态均支持禁用：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:CheckBox IsChecked="False" IsEnabled="False">UnChecked</atom:CheckBox>
    <atom:CheckBox IsChecked="{x:Null}" IsEnabled="False">Indeterminate</atom:CheckBox>
    <atom:CheckBox IsChecked="True" IsEnabled="False">Checked</atom:CheckBox>
</StackPanel>
```

---

## 3. 受控复选框

通过数据绑定控制复选框状态：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:CheckBox IsChecked="{Binding ControlledCheckBoxCheckedStatus}"
                   IsEnabled="{Binding ControlledCheckBoxEnabledStatus}"
                   Content="{Binding ControlledCheckBoxText}" />
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button SizeType="Small" ButtonType="Primary"
                     Content="{Binding CheckStatusBtnText}" />
        <atom:Button SizeType="Small" ButtonType="Primary"
                     Content="{Binding EnableStatusBtnText}" />
    </StackPanel>
</StackPanel>
```

---

## 4. 复选框组

### 声明式用法

直接在 `CheckBoxGroup` 中添加 `CheckBox` 子项：

```xml
<!-- 默认全选 -->
<atom:CheckBoxGroup>
    <atom:CheckBox IsChecked="True">Apple</atom:CheckBox>
    <atom:CheckBox IsChecked="True">Pear</atom:CheckBox>
    <atom:CheckBox IsChecked="True">Orange</atom:CheckBox>
</atom:CheckBoxGroup>

<!-- 部分选中 -->
<atom:CheckBoxGroup>
    <atom:CheckBox>Apple</atom:CheckBox>
    <atom:CheckBox IsChecked="True">Pear</atom:CheckBox>
    <atom:CheckBox>Orange</atom:CheckBox>
</atom:CheckBoxGroup>

<!-- 部分禁用 -->
<atom:CheckBoxGroup>
    <atom:CheckBox IsChecked="True" IsEnabled="False">Apple</atom:CheckBox>
    <atom:CheckBox IsEnabled="False">Pear</atom:CheckBox>
    <atom:CheckBox IsEnabled="False">Orange</atom:CheckBox>
</atom:CheckBoxGroup>
```

### 数据驱动用法

通过 `ItemsSource` 绑定 `ICheckBoxOption` 集合：

```csharp
// ViewModel 或 Code-behind
var options = new List<CheckBoxOption>
{
    new() { Content = "Apple", IsChecked = true },
    new() { Content = "Pear" },
    new() { Content = "Orange" }
};
BasicCheckBoxGroup.ItemsSource = options;
```

---

## 5. 全选/部分选联动

通过 Indeterminate 不确定态实现全选联动效果：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:CheckBox Name="CheckAllCheckbox"
                   IsChecked="{Binding CheckedAllStatus}">
        Check all
    </atom:CheckBox>
    <WrapPanel Margin="0,20,0,0">
        <atom:CheckBox IsChecked="{Binding AppleCheckedStatus}">Apple</atom:CheckBox>
        <atom:CheckBox IsChecked="{Binding PearCheckedStatus}">Pear</atom:CheckBox>
        <atom:CheckBox IsChecked="{Binding OrangeCheckedStatus}">Orange</atom:CheckBox>
    </WrapPanel>
</StackPanel>
```

**实现逻辑**：
- 全选框的 `IsChecked` 绑定到 ViewModel 的计算属性
- 当所有子项选中时，全选框为 `true`
- 当部分子项选中时，全选框为 `null`（不确定态，显示横杠）
- 当没有子项选中时，全选框为 `false`
- 点击全选框时，全部子项同步切换

---

## 6. Grid 布局中使用

利用 Grid 实现复杂的复选框布局：

```xml
<Grid ColumnDefinitions="*,*,*" RowDefinitions="Auto,Auto" Margin="10">
    <atom:CheckBox Grid.Row="0" Grid.Column="0">A</atom:CheckBox>
    <atom:CheckBox Grid.Row="0" Grid.Column="1">B</atom:CheckBox>
    <atom:CheckBox Grid.Row="0" Grid.Column="2">C</atom:CheckBox>
    <atom:CheckBox Grid.Row="1" Grid.Column="0">D</atom:CheckBox>
    <atom:CheckBox Grid.Row="1" Grid.Column="1">E</atom:CheckBox>
</Grid>
```

---

## 7. MVVM 命令绑定

CheckBox 继承自 Button，完整支持 `Command` 绑定：

```xml
<atom:CheckBox Command="{Binding ToggleOptionCommand}"
               CommandParameter="newsletter">
    Subscribe to newsletter
</atom:CheckBox>
```

---

## 8. 控制动画行为

```xml
<!-- 禁用点击波纹效果 -->
<atom:CheckBox IsWaveSpiritEnabled="False">No Wave</atom:CheckBox>
```

---

## 常见组合模式

### 设置选项列表

```xml
<StackPanel Spacing="8">
    <atom:TextBlock Text="Notification Preferences" FontWeight="Bold" />
    <atom:CheckBox IsChecked="True">Email notifications</atom:CheckBox>
    <atom:CheckBox IsChecked="True">Push notifications</atom:CheckBox>
    <atom:CheckBox>SMS notifications</atom:CheckBox>
</StackPanel>
```

### 表单中使用

```xml
<atom:FormItem Label="Interests">
    <atom:CheckBoxGroup>
        <atom:CheckBox>Music</atom:CheckBox>
        <atom:CheckBox>Sports</atom:CheckBox>
        <atom:CheckBox>Reading</atom:CheckBox>
        <atom:CheckBox>Travel</atom:CheckBox>
    </atom:CheckBoxGroup>
</atom:FormItem>
```

### 协议确认

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <atom:CheckBox Name="AgreeCheckBox">
        I have read and agree to the
    </atom:CheckBox>
    <atom:Button ButtonType="Link" Padding="0">Terms of Service</atom:Button>
</StackPanel>
<atom:Button ButtonType="Primary"
             IsEnabled="{Binding #AgreeCheckBox.IsChecked}">
    Submit
</atom:Button>
```
