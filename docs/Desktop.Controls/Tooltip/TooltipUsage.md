# Tooltip 使用文档

本文档介绍 AtomUI ToolTip 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TooltipShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 ToolTip，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // ToolTip 控件
```

---

## 1. 基本用法

鼠标悬浮时显示简单文字提示。ToolTip 通过附加属性附加到任意控件上，无需嵌套或修改控件模板：

```xml
<atom:TextBlock
    HorizontalAlignment="Left"
    atom:ToolTip.Tip="prompt text">
    Tooltip will show on mouse enter.
</atom:TextBlock>
```

也可以附加到按钮等其他控件上：

```xml
<atom:Button atom:ToolTip.Tip="This is a helpful tooltip">
    Hover me
</atom:Button>
```

> **提示**：当 `Tip` 为字符串时，ToolTip 内部自动将其转换为带自动换行的 `TextBlock`（`TextWrapping="Wrap"`），超过 `ToolTipMaxWidth`（默认 250px）时自动换行。

---

## 2. 弹出方向

通过 `Placement` 附加属性设置 12 种弹出方向。不设置时默认为 `Top`。

```xml
<!-- 上方（默认） -->
<atom:Button atom:ToolTip.Tip="Top" atom:ToolTip.Placement="Top">Top</atom:Button>

<!-- 下方 -->
<atom:Button atom:ToolTip.Tip="Bottom" atom:ToolTip.Placement="Bottom">Bottom</atom:Button>

<!-- 左侧 -->
<atom:Button atom:ToolTip.Tip="Left" atom:ToolTip.Placement="Left">Left</atom:Button>

<!-- 右侧 -->
<atom:Button atom:ToolTip.Tip="Right" atom:ToolTip.Placement="Right">Right</atom:Button>

<!-- 上方左对齐 -->
<atom:Button atom:ToolTip.Tip="TopLeft"
             atom:ToolTip.Placement="TopEdgeAlignedLeft">
    TopLeft
</atom:Button>

<!-- 上方右对齐 -->
<atom:Button atom:ToolTip.Tip="TopRight"
             atom:ToolTip.Placement="TopEdgeAlignedRight">
    TopRight
</atom:Button>
```

### 完整的 12 方向布局

Gallery 中展示了 12 种方向的完整布局，使用 Grid 排列：

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
    </Grid.ColumnDefinitions>

    <!-- 左侧列 -->
    <atom:Button Grid.Row="1" Grid.Column="0" Content="LT"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="LeftEdgeAlignedTop" />
    <atom:Button Grid.Row="2" Grid.Column="0" Content="Left"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="Left" />
    <atom:Button Grid.Row="3" Grid.Column="0" Content="LB"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="LeftEdgeAlignedBottom" />

    <!-- 顶部行 -->
    <atom:Button Grid.Row="0" Grid.Column="1" Content="TL"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="TopEdgeAlignedLeft" />
    <atom:Button Grid.Row="0" Grid.Column="2" Content="Top"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="Top" />
    <atom:Button Grid.Row="0" Grid.Column="3" Content="TR"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="TopEdgeAlignedRight" />

    <!-- 右侧列 -->
    <atom:Button Grid.Row="1" Grid.Column="4" Content="RT"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="RightEdgeAlignedTop" />
    <atom:Button Grid.Row="2" Grid.Column="4" Content="Right"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="Right" />
    <atom:Button Grid.Row="3" Grid.Column="4" Content="RB"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="RightEdgeAlignedBottom" />

    <!-- 底部行 -->
    <atom:Button Grid.Row="4" Grid.Column="1" Content="BL"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="BottomEdgeAlignedLeft" />
    <atom:Button Grid.Row="4" Grid.Column="2" Content="Bottom"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="Bottom" />
    <atom:Button Grid.Row="4" Grid.Column="3" Content="BR"
                 atom:ToolTip.Tip="prompt text"
                 atom:ToolTip.Placement="BottomEdgeAlignedRight" />
</Grid>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TooltipShowCase.axaml` 中 "Placement" 示例。

---

## 3. 箭头控制

ToolTip 支持三种箭头模式：显示、隐藏和居中指向。

### 显示箭头（默认）

```xml
<atom:Button atom:ToolTip.Tip="With arrow"
             atom:ToolTip.IsShowArrow="True">
    Arrow
</atom:Button>
```

### 隐藏箭头

```xml
<atom:Button atom:ToolTip.Tip="No arrow"
             atom:ToolTip.IsShowArrow="False">
    No Arrow
</atom:Button>
```

### 箭头指向目标中心

```xml
<atom:Button atom:ToolTip.Tip="Point at center"
             atom:ToolTip.IsShowArrow="True"
             atom:ToolTip.IsPointAtCenter="True"
             atom:ToolTip.Placement="TopEdgeAlignedLeft">
    Point at Center
</atom:Button>
```

### 通过 Segmented 切换箭头模式（Gallery 示例）

Gallery 中提供了通过 `Segmented` 控件动态切换箭头模式的示例：

```xml
<atom:Segmented x:Name="ArrowSegmented">
    <atom:SegmentedItem>Show</atom:SegmentedItem>
    <atom:SegmentedItem>Hide</atom:SegmentedItem>
    <atom:SegmentedItem>Center</atom:SegmentedItem>
</atom:Segmented>

<atom:Button atom:ToolTip.Tip="prompt text"
             atom:ToolTip.Placement="Top"
             atom:ToolTip.IsShowArrow="{Binding ShowArrow}"
             atom:ToolTip.IsPointAtCenter="{Binding IsPointAtCenter}">
    Top
</atom:Button>
```

```csharp
// ViewModel 处理 Segmented 选中变更
public void HandleSelectionChanged(object? sender, SelectionChangedEventArgs args)
{
    if (sender is Segmented segmented)
    {
        if (segmented.SelectedIndex == 0)
        {
            ShowArrow       = true;
            IsPointAtCenter = false;
        }
        else if (segmented.SelectedIndex == 1)
        {
            ShowArrow       = false;
            IsPointAtCenter = false;
        }
        else if (segmented.SelectedIndex == 2)
        {
            IsPointAtCenter = true;
            ShowArrow       = true;
        }
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TooltipShowCase.axaml` 中 "Arrow" 示例。

---

## 4. 预设颜色

使用 Ant Design 调色板提供的 14 种标准颜色：

```xml
<WrapPanel HorizontalAlignment="Left">
    <atom:Button Content="Blue" atom:ToolTip.PresetColor="Blue">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Red" atom:ToolTip.PresetColor="Red">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Volcano" atom:ToolTip.PresetColor="Volcano">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Orange" atom:ToolTip.PresetColor="Orange">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Gold" atom:ToolTip.PresetColor="Gold">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Yellow" atom:ToolTip.PresetColor="Yellow">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Lime" atom:ToolTip.PresetColor="Lime">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Green" atom:ToolTip.PresetColor="Green">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Cyan" atom:ToolTip.PresetColor="Cyan">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="GeekBlue" atom:ToolTip.PresetColor="GeekBlue">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Purple" atom:ToolTip.PresetColor="Purple">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Pink" atom:ToolTip.PresetColor="Pink">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Magenta" atom:ToolTip.PresetColor="Magenta">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="Grey" atom:ToolTip.PresetColor="Grey">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
</WrapPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TooltipShowCase.axaml` 中 "Colorful Tooltip" → "Presets" 示例。

---

## 5. 自定义颜色

使用任意颜色值作为 ToolTip 背景色：

```xml
<WrapPanel HorizontalAlignment="Left">
    <atom:Button Content="#f50" atom:ToolTip.Color="#f50">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="#2db7f5" atom:ToolTip.Color="#2db7f5">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="#87d068" atom:ToolTip.Color="#87d068">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
    <atom:Button Content="#108ee9" atom:ToolTip.Color="#108ee9">
        <atom:ToolTip.Tip>prompt text</atom:ToolTip.Tip>
        <atom:ToolTip.Placement>Top</atom:ToolTip.Placement>
    </atom:Button>
</WrapPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TooltipShowCase.axaml` 中 "Colorful Tooltip" → "Custom" 示例。

---

## 6. 延迟显示

通过 `ShowDelay` 控制鼠标悬浮后多久显示 ToolTip：

```xml
<!-- 立即显示（无延迟） -->
<atom:Button atom:ToolTip.Tip="Instant"
             atom:ToolTip.ShowDelay="0">
    Instant
</atom:Button>

<!-- 默认延迟（400ms） -->
<atom:Button atom:ToolTip.Tip="Default delay">
    Default (400ms)
</atom:Button>

<!-- 延迟 1 秒 -->
<atom:Button atom:ToolTip.Tip="Delayed 1s"
             atom:ToolTip.ShowDelay="1000">
    Delayed
</atom:Button>
```

### 快速切换（BetweenShowDelay）

当用户在多个带 ToolTip 的控件之间快速移动鼠标时，`BetweenShowDelay` 控制是否跳过 `ShowDelay` 直接显示：

```xml
<!-- 快速切换时 100ms 内直接显示（默认行为） -->
<StackPanel Orientation="Horizontal" Spacing="4">
    <atom:Button atom:ToolTip.Tip="Tooltip A"
                 atom:ToolTip.BetweenShowDelay="100">A</atom:Button>
    <atom:Button atom:ToolTip.Tip="Tooltip B"
                 atom:ToolTip.BetweenShowDelay="100">B</atom:Button>
    <atom:Button atom:ToolTip.Tip="Tooltip C"
                 atom:ToolTip.BetweenShowDelay="100">C</atom:Button>
</StackPanel>
```

---

## 7. 程序化控制

通过 `IsOpen` 附加属性控制 ToolTip 的显示/隐藏，需同时设置 `IsCustomShowAndHide="True"`：

```xml
<atom:Button atom:ToolTip.Tip="Controlled tooltip"
             atom:ToolTip.IsOpen="{Binding IsTooltipOpen}"
             atom:ToolTip.IsCustomShowAndHide="True">
    Controlled
</atom:Button>
```

```csharp
// ViewModel
public class MyViewModel : ReactiveObject
{
    private bool _isTooltipOpen;
    public bool IsTooltipOpen
    {
        get => _isTooltipOpen;
        set => this.RaiseAndSetIfChanged(ref _isTooltipOpen, value);
    }

    // 通过按钮或其他操作控制
    public void ToggleTooltip()
    {
        IsTooltipOpen = !IsTooltipOpen;
    }
}
```

> ⚠️ 必须设置 `IsCustomShowAndHide="True"`，否则 `ToolTipService` 的自动管理会覆盖 `IsOpen` 绑定。

---

## 8. 在禁用控件上显示

默认情况下，禁用的控件不显示 ToolTip。设置 `ShowOnDisabled="True"` 可改变此行为：

```xml
<atom:Button IsEnabled="False"
             atom:ToolTip.Tip="This button is disabled"
             atom:ToolTip.ShowOnDisabled="True">
    Disabled
</atom:Button>
```

---

## 9. 拦截 ToolTip 打开事件

可以在 ToolTip 打开前拦截并取消：

```csharp
// 在 Code-behind 中
ToolTip.AddToolTipOpeningHandler(myButton, (sender, args) =>
{
    // 根据条件决定是否允许打开
    if (ShouldPreventTooltip())
    {
        args.Cancel = true;
    }
});
```

---

## 常见组合模式

### 图标按钮提示

为仅图标的按钮提供功能说明：

```xml
<atom:Button ButtonType="Text" Shape="Circle"
             Icon="{antdicons:AntDesignIconProvider Kind=QuestionCircleOutlined}"
             atom:ToolTip.Tip="Click for more information" />
```

### 工具栏按钮提示

为工具栏中的每个按钮添加功能提示：

```xml
<StackPanel Orientation="Horizontal" Spacing="4">
    <atom:Button ButtonType="Text"
                 Icon="{antdicons:AntDesignIconProvider Kind=BoldOutlined}"
                 atom:ToolTip.Tip="Bold (Ctrl+B)" />
    <atom:Button ButtonType="Text"
                 Icon="{antdicons:AntDesignIconProvider Kind=ItalicOutlined}"
                 atom:ToolTip.Tip="Italic (Ctrl+I)" />
    <atom:Button ButtonType="Text"
                 Icon="{antdicons:AntDesignIconProvider Kind=UnderlineOutlined}"
                 atom:ToolTip.Tip="Underline (Ctrl+U)" />
</StackPanel>
```

### 截断文本提示完整内容

当文本被截断时，使用 ToolTip 显示完整内容：

```xml
<atom:TextBlock Text="{Binding LongText}"
                TextTrimming="CharacterEllipsis"
                MaxWidth="200"
                atom:ToolTip.Tip="{Binding LongText}" />
```

### 表单字段说明

为表单输入框提供辅助说明：

```xml
<StackPanel Orientation="Horizontal" Spacing="4">
    <atom:TextBox Watermark="Enter email" />
    <atom:Button ButtonType="Text" Shape="Circle"
                 Icon="{antdicons:AntDesignIconProvider Kind=InfoCircleOutlined}"
                 atom:ToolTip.Tip="We'll never share your email with anyone else."
                 atom:ToolTip.Placement="Right" />
</StackPanel>
```

### 导航菜单项提示

折叠状态的侧边栏菜单项：

```xml
<atom:Button ButtonType="Text"
             Icon="{antdicons:AntDesignIconProvider Kind=HomeOutlined}"
             atom:ToolTip.Tip="Dashboard"
             atom:ToolTip.Placement="Right" />
<atom:Button ButtonType="Text"
             Icon="{antdicons:AntDesignIconProvider Kind=SettingOutlined}"
             atom:ToolTip.Tip="Settings"
             atom:ToolTip.Placement="Right" />
```
