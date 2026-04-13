# Drawer 使用文档

本文档介绍 AtomUI Drawer 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/DrawerShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Drawer，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Drawer, DrawerPlacement
using AtomUI.Controls;            // CustomizableSizeType
using AtomUI;                     // Dimension, DimensionUnitType
```

---

## 1. 基础抽屉

通过 `IsOpen` 属性控制抽屉的打开和关闭，通常使用数据绑定或控件属性绑定：

```xml
<Panel>
    <atom:ToggleSwitch />
    <atom:Drawer IsOpen="{Binding $parent[Panel].((atom:ToggleSwitch)Children[0]).IsChecked}"
                 Title="Basic Drawer"
                 DialogSize="50%">
        <StackPanel Orientation="Vertical" Spacing="5">
            <atom:TextBlock Text="Some contents..." />
            <atom:TextBlock Text="Some contents..." />
            <atom:TextBlock Text="Some contents..." />
        </StackPanel>
    </atom:Drawer>
</Panel>
```

---

## 2. 自定义方向

通过 `Placement` 属性设置抽屉从哪个边缘滑入：

```xml
<atom:Drawer Placement="Left" Title="Left Drawer" />
<atom:Drawer Placement="Top" Title="Top Drawer" />
<atom:Drawer Placement="Right" Title="Right Drawer" />
<atom:Drawer Placement="Bottom" Title="Bottom Drawer" />
```

使用 OptionButtonGroup 动态切换方向：

```xml
<StackPanel Orientation="Horizontal" Spacing="5">
    <atom:TextBlock VerticalAlignment="Center">Placement:</atom:TextBlock>
    <atom:OptionButtonGroup ButtonStyle="Outline"
                             OptionCheckedChanged="HandlePlacementChanged">
        <atom:OptionButton Tag="{x:Static atom:DrawerPlacement.Left}">Left</atom:OptionButton>
        <atom:OptionButton Tag="{x:Static atom:DrawerPlacement.Top}">Top</atom:OptionButton>
        <atom:OptionButton IsChecked="True" Tag="{x:Static atom:DrawerPlacement.Right}">Right</atom:OptionButton>
        <atom:OptionButton Tag="{x:Static atom:DrawerPlacement.Bottom}">Bottom</atom:OptionButton>
    </atom:OptionButtonGroup>
</StackPanel>
```

```csharp
private void HandlePlacementChanged(object? sender, OptionCheckedChangedEventArgs args)
{
    var option = args.CheckedOption;
    if (option.IsChecked == true && option.Tag is DrawerPlacement placement)
    {
        if (DataContext is MyViewModel vm)
        {
            vm.DrawerPlacement = placement;
        }
    }
}
```

---

## 3. 多级抽屉

在一个 Drawer 的内容中声明另一个 Drawer，打开子抽屉时父抽屉会自动推开：

```xml
<atom:Drawer Title="First-level Drawer"
             IsOpen="{Binding IsFirstLevelOpen}"
             Placement="Right">
    <StackPanel Orientation="Vertical" Spacing="5">
        <atom:TextBlock Text="Some contents..." />
        <atom:Button ButtonType="Primary"
                     Click="HandleOpenSecondLevel">
            Two-level drawer
        </atom:Button>
        <atom:Drawer Title="Two-level Drawer"
                     Name="SecondLevelDrawer"
                     Placement="Right">
            <StackPanel Orientation="Vertical" Spacing="5">
                <atom:TextBlock Text="Some contents..." />
            </StackPanel>
        </atom:Drawer>
    </StackPanel>
</atom:Drawer>
```

```csharp
private void HandleOpenSecondLevel(object? sender, RoutedEventArgs e)
{
    SecondLevelDrawer.IsOpen = true;
}
```

> **说明**：子 Drawer 会自动继承父 Drawer 的 `OpenOn` 和 `IsMotionEnabled` 属性。推移量由 `PushOffsetPercent`（默认 40%）控制。

---

## 4. 标题栏额外操作和底部区域

通过 `Extra` 和 `Footer` 属性设置额外内容：

```xml
<atom:Drawer Title="Basic Drawer"
             Placement="{Binding DrawerPlacement}">
    <atom:Drawer.Extra>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:Button>Cancel</atom:Button>
            <atom:Button ButtonType="Primary">Ok</atom:Button>
        </StackPanel>
    </atom:Drawer.Extra>
    <atom:Drawer.Footer>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:Button>Edit</atom:Button>
            <atom:Button ButtonType="Primary">Upload</atom:Button>
            <atom:Button ButtonType="Primary" IsDanger="True">Delete</atom:Button>
        </StackPanel>
    </atom:Drawer.Footer>
    <StackPanel Orientation="Vertical" Spacing="5">
        <atom:TextBlock Text="Some contents..." />
    </StackPanel>
</atom:Drawer>
```

---

## 5. 无遮罩模式

通过 `IsShowMask="False"` 隐藏遮罩层，用户可以同时操作抽屉和主内容区域：

```xml
<atom:Drawer IsOpen="{Binding IsDrawerOpen}"
             Title="Basic Drawer"
             IsShowMask="False">
    <StackPanel Orientation="Vertical" Spacing="5">
        <atom:TextBlock Text="Some contents..." />
    </StackPanel>
</atom:Drawer>
```

---

## 6. 预设尺寸和自定义尺寸

### 预设尺寸

通过 `SizeType` 属性选择预设尺寸：

```csharp
// 小号（默认 378px）
drawer.SizeType = CustomizableSizeType.Small;

// 大号（736px）
drawer.SizeType = CustomizableSizeType.Large;
```

### 自定义像素尺寸

```csharp
drawer.SizeType   = CustomizableSizeType.Custom;
drawer.DialogSize = new Dimension(400); // 400px
drawer.IsOpen     = true;
```

### 自定义百分比尺寸

```csharp
drawer.SizeType   = CustomizableSizeType.Custom;
drawer.DialogSize = new Dimension(50, DimensionUnitType.Percentage); // 50%
drawer.IsOpen     = true;
```

也可以在 AXAML 中使用百分比：

```xml
<atom:Drawer DialogSize="50%" Title="Half Width Drawer" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/DrawerShowCase.axaml` 中 "Preset size" 示例。

---

## 7. 作用域渲染

默认情况下抽屉在整个窗口区域渲染。通过 `OpenOn` 属性可以限制渲染范围：

```xml
<Panel Height="300">
    <StackPanel Orientation="Vertical" Spacing="10">
        <atom:TextBlock>Render in this</atom:TextBlock>
        <atom:ToggleSwitch />
    </StackPanel>
    <atom:Drawer
        IsOpen="{Binding ...}"
        Title="Basic Drawer"
        OpenOn="{Binding $parent[gallery:ShowCaseItem]}">
        <atom:TextBlock Text="Some contents..." />
    </atom:Drawer>
</Panel>
```

> **说明**：`OpenOn` 可以绑定到任何 `Control`，Drawer 将仅在该控件区域内渲染。

---

## 常见组合模式

### 列表详情模式

```xml
<Panel>
    <atom:ListBox SelectionChanged="HandleSelectionChanged" />
    <atom:Drawer Title="Details"
                 IsOpen="{Binding IsDetailOpen}"
                 Placement="Right"
                 SizeType="Middle">
        <StackPanel Spacing="10">
            <atom:TextBlock Text="{Binding SelectedItem.Name}" />
            <atom:TextBlock Text="{Binding SelectedItem.Description}" />
        </StackPanel>
    </atom:Drawer>
</Panel>
```

### 设置面板模式

```xml
<atom:Drawer Title="Settings"
             Placement="Right"
             SizeType="Small"
             IsShowMask="False">
    <atom:Drawer.Extra>
        <atom:Button ButtonType="Primary" SizeType="Small">Save</atom:Button>
    </atom:Drawer.Extra>
    <StackPanel Spacing="15">
        <StackPanel Spacing="5">
            <atom:TextBlock>Theme</atom:TextBlock>
            <atom:OptionButtonGroup ButtonStyle="Outline">
                <atom:OptionButton IsChecked="True">Light</atom:OptionButton>
                <atom:OptionButton>Dark</atom:OptionButton>
            </atom:OptionButtonGroup>
        </StackPanel>
    </StackPanel>
</atom:Drawer>
```

### 表单编辑模式

```xml
<atom:Drawer Title="Edit User"
             Placement="Right"
             SizeType="Middle">
    <atom:Drawer.Footer>
        <StackPanel Orientation="Horizontal" Spacing="10"
                    HorizontalAlignment="Right">
            <atom:Button Click="HandleCancel">Cancel</atom:Button>
            <atom:Button ButtonType="Primary" Click="HandleSave">Save</atom:Button>
        </StackPanel>
    </atom:Drawer.Footer>
    <StackPanel Spacing="10">
        <atom:TextBox Watermark="Name" />
        <atom:TextBox Watermark="Email" />
        <atom:TextBox Watermark="Phone" />
    </StackPanel>
</atom:Drawer>
```
