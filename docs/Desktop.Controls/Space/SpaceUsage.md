# Space 使用文档

本文档介绍 AtomUI Space 控件的各种使用方式和常见场景。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Layout/SpaceShowCase.axaml`

---

## 前置准备

### AXAML 命名空间

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

### C# 命名空间

```csharp
using AtomUI.Desktop.Controls;  // Space, CompactSpace 等
using AtomUI.Controls;           // SizeType, CustomizableSizeType 等共享类型
```

---

## 1. 基本用法

水平排列子元素，默认间距为 Small（8px）：

```xml
<atom:Space>
    <atom:Button ButtonType="Primary">Button</atom:Button>
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}">
        Click to Upload
    </atom:Button>
    <atom:PopupConfirm Title="Delete the task"
                       ConfirmContent="Are you sure to delete this task?"
                       OkText="Ok" CancelText="Cancel">
        <atom:Button ButtonType="Default" IsDanger="True">Confirm</atom:Button>
    </atom:PopupConfirm>
</atom:Space>
```

**使用场景**：当一组按钮或控件需要统一间距时，用 Space 包裹即可，无需为每个子元素设置 `Margin`。

---

## 2. 垂直排列

通过 `Orientation="Vertical"` 切换为垂直方向排列。垂直模式常用于卡片列表、表单字段等场景：

```xml
<atom:Space Orientation="Vertical" HorizontalAlignment="Stretch" Height="300">
    <atom:Card Header="Card" SizeType="Middle" HorizontalAlignment="Stretch">
        <StackPanel Orientation="Vertical" Spacing="10">
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
        </StackPanel>
    </atom:Card>

    <atom:Card Header="Card" SizeType="Middle" HorizontalAlignment="Stretch">
        <StackPanel Orientation="Vertical" Spacing="10">
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
        </StackPanel>
    </atom:Card>

    <atom:Card Header="Card" SizeType="Middle" HorizontalAlignment="Stretch">
        <StackPanel Orientation="Vertical" Spacing="10">
            <atom:TextBlock>Card content</atom:TextBlock>
            <atom:TextBlock>Card content</atom:TextBlock>
        </StackPanel>
    </atom:Card>
</atom:Space>
```

---

## 3. 三种预设尺寸

通过 `SizeType` 属性设置间距大小，支持 `Small`（默认）、`Middle`、`Large` 三种预设，以及 `Custom` 自定义：

```xml
<atom:Space SizeType="Small">
    <atom:Button>Small</atom:Button>
    <atom:Button>Small</atom:Button>
</atom:Space>

<atom:Space SizeType="Middle">
    <atom:Button>Middle</atom:Button>
    <atom:Button>Middle</atom:Button>
</atom:Space>

<atom:Space SizeType="Large">
    <atom:Button>Large</atom:Button>
    <atom:Button>Large</atom:Button>
</atom:Space>
```

### 动态切换尺寸

配合 RadioButton 和数据绑定实现运行时动态切换（参考 Gallery "Space Size" 示例）：

```xml
<StackPanel Orientation="Vertical" Spacing="15">
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:RadioButton Tag="{x:Static atom:CustomizableSizeType.Small}"
                          IsCheckedChanged="HandleSizeTypeChanged"
                          IsChecked="True">Small</atom:RadioButton>
        <atom:RadioButton Tag="{x:Static atom:CustomizableSizeType.Middle}"
                          IsCheckedChanged="HandleSizeTypeChanged">Middle</atom:RadioButton>
        <atom:RadioButton Tag="{x:Static atom:CustomizableSizeType.Large}"
                          IsCheckedChanged="HandleSizeTypeChanged">Large</atom:RadioButton>
        <atom:RadioButton Tag="{x:Static atom:CustomizableSizeType.Custom}"
                          IsCheckedChanged="HandleSizeTypeChanged">Custom</atom:RadioButton>
    </StackPanel>
    <atom:Slider Name="CustomSizeSlider"
                 Minimum="0" Maximum="100"
                 Value="{Binding CustomSpacingValue, Mode=TwoWay}"
                 IsVisible="False" />
    <atom:Space Orientation="Horizontal"
                SizeType="{Binding SizeType}"
                LineSpacing="{Binding #CustomSizeSlider.Value, Priority=Template}"
                ItemSpacing="{Binding #CustomSizeSlider.Value, Priority=Template}">
        <atom:Button ButtonType="Primary">Primary</atom:Button>
        <atom:Button>Default</atom:Button>
        <atom:Button ButtonType="Dashed">Dashed</atom:Button>
        <atom:Button ButtonType="Link">Link</atom:Button>
    </atom:Space>
</StackPanel>
```

```csharp
// ViewModel
public class SpaceViewModel : ReactiveObject
{
    private CustomizableSizeType _sizeType;
    public CustomizableSizeType SizeType
    {
        get => _sizeType;
        set => this.RaiseAndSetIfChanged(ref _sizeType, value);
    }

    private double _customSpacingValue;
    public double CustomSpacingValue
    {
        get => _customSpacingValue;
        set => this.RaiseAndSetIfChanged(ref _customSpacingValue, value);
    }
}
```

---

## 4. 自定义间距

直接设置 `ItemSpacing`（项目间距）和 `LineSpacing`（行间距）覆盖预设值：

```xml
<atom:Space ItemSpacing="24">
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
</atom:Space>
```

---

## 5. 对齐方式

通过 `ItemsAlignment` 控制每行子项在交叉轴方向上的对齐方式：

```xml
<!-- 顶部对齐（默认） -->
<atom:Space ItemsAlignment="Start">
    <atom:TextBlock>Text</atom:TextBlock>
    <atom:Button ButtonType="Primary">Primary</atom:Button>
    <Border Background="{atom:SharedTokenResource ColorBgLayout}"
            Height="70" Width="50" Padding="4">
        <atom:TextBlock VerticalAlignment="Center">Block</atom:TextBlock>
    </Border>
</atom:Space>

<!-- 居中对齐 -->
<atom:Space ItemsAlignment="Center">
    <atom:TextBlock>Text</atom:TextBlock>
    <atom:Button ButtonType="Primary">Primary</atom:Button>
    <Border Background="{atom:SharedTokenResource ColorBgLayout}"
            Height="70" Width="50" Padding="4">
        <atom:TextBlock VerticalAlignment="Center">Block</atom:TextBlock>
    </Border>
</atom:Space>

<!-- 底部对齐 -->
<atom:Space ItemsAlignment="End">
    <atom:TextBlock>Text</atom:TextBlock>
    <atom:Button ButtonType="Primary">Primary</atom:Button>
    <Border Background="{atom:SharedTokenResource ColorBgLayout}"
            Height="70" Width="50" Padding="4">
        <atom:TextBlock VerticalAlignment="Center">Block</atom:TextBlock>
    </Border>
</atom:Space>
```

---

## 6. 自动换行

Space 在水平方向空间不足时会自动换行，类似 CSS `flex-wrap: wrap`：

```xml
<atom:Space Orientation="Horizontal" ItemSpacing="8" LineSpacing="16">
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
    <atom:Button>Button</atom:Button>
</atom:Space>
```

`LineSpacing` 控制换行后行与行之间的间距（此处为 16px），`ItemSpacing` 控制同一行内子项之间的间距（此处为 8px）。

---

## 7. 带分割线

通过 `SplitTemplate` 在相邻子项之间自动插入分隔控件：

```xml
<atom:Space Orientation="Horizontal" ItemsAlignment="Center">
    <atom:Space.SplitTemplate>
        <Template>
            <atom:Separator Orientation="Vertical" />
        </Template>
    </atom:Space.SplitTemplate>
    <atom:HyperLinkTextBlock>Link</atom:HyperLinkTextBlock>
    <atom:HyperLinkTextBlock>Link</atom:HyperLinkTextBlock>
    <atom:HyperLinkTextBlock>Link</atom:HyperLinkTextBlock>
</atom:Space>
```

分割线模板可以是任意控件，如 `Separator`、`Divider` 或自定义 `Border`。

---

## 8. CompactSpace 紧凑组合

### 表单控件组合

将多个表单控件紧密排列，去除间距并共享边框：

```xml
<!-- 输入框 + 按钮 -->
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:LineEdit Text="https://atomui.net" atom:CompactSpace.ItemSize="4*" />
    <atom:Button ButtonType="Primary">Submit</atom:Button>
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*" />
</atom:CompactSpace>

<!-- 区号 + 电话号码 -->
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:LineEdit Text="0571" atom:CompactSpace.ItemSize="2*" />
    <atom:LineEdit Text="26888888" atom:CompactSpace.ItemSize="3*" />
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*" />
</atom:CompactSpace>

<!-- Select + Input -->
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:Select IsAllowClear="True" DefaultValues="Zhejiang">
        <atom:SelectOption Header="Zhejiang" Content="Zhejiang" />
        <atom:SelectOption Header="Jiangsu" Content="Jiangsu" />
    </atom:Select>
    <atom:LineEdit Text="Xihu District, Hangzhou" atom:CompactSpace.ItemSize="4*" />
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="4*" />
</atom:CompactSpace>

<!-- 复杂组合：Select + Input + NumericUpDown -->
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:Select DefaultValues="Option1">
        <atom:SelectOption Header="Option1" Content="Option1" />
        <atom:SelectOption Header="Option2" Content="Option2" />
    </atom:Select>
    <atom:LineEdit Text="input content" atom:CompactSpace.ItemSize="5*" />
    <atom:NumericUpDown Value="12" atom:CompactSpace.ItemSize="80" />
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*" />
</atom:CompactSpace>
```

### 带 AddOn 的组合

使用 `CompactSpaceAddOn` 添加装饰性前缀/后缀：

```xml
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:LineEdit PlaceholderText="input here" />
    <atom:CompactSpaceAddOn Content="$" />
    <atom:NumericUpDown PlaceholderText="another input" atom:CompactSpace.ItemSize="1*" />
    <atom:NumericUpDown PlaceholderText="another input" atom:CompactSpace.ItemSize="1*" />
    <atom:CompactSpaceAddOn Content="$" />
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="*" />
</atom:CompactSpace>

<!-- Button + Input + AddOn -->
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:Button ButtonType="Primary">Button</atom:Button>
    <atom:LineEdit PlaceholderText="input here" />
    <atom:CompactSpaceAddOn Content="$" />
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="*" />
</atom:CompactSpace>
```

### 范围输入

```xml
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:Select DefaultValues="1">
        <atom:SelectOption Header="Between" Content="1" />
        <atom:SelectOption Header="Except" Content="2" />
    </atom:Select>
    <atom:LineEdit atom:CompactSpace.ItemSize="100" PlaceholderText="Minimum" />
    <atom:LineEdit PlaceholderText="~" IsEnabled="False" />
    <atom:LineEdit atom:CompactSpace.ItemSize="100" PlaceholderText="Maximum" />
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*" />
</atom:CompactSpace>
```

---

## 9. CompactSpace 按钮组

### 工具栏图标按钮组

```xml
<atom:CompactSpace Orientation="Horizontal">
    <atom:Button Icon="{antdicons:AntDesignIconProvider LikeOutlined}" atom:ToolTip.Tip="Like" />
    <atom:Button Icon="{antdicons:AntDesignIconProvider CommentOutlined}" atom:ToolTip.Tip="Comment" />
    <atom:Button Icon="{antdicons:AntDesignIconProvider StarOutlined}" atom:ToolTip.Tip="Star" />
    <atom:Button Icon="{antdicons:AntDesignIconProvider HeartOutlined}" atom:ToolTip.Tip="Heart" />
    <atom:Button Icon="{antdicons:AntDesignIconProvider ShareAltOutlined}" atom:ToolTip.Tip="Share" />
    <atom:Button Icon="{antdicons:AntDesignIconProvider DownloadOutlined}" atom:ToolTip.Tip="Download" />
    <atom:DropdownButton TriggerType="Click"
                         Placement="BottomEdgeAlignedRight"
                         IsShowOpenIndicator="False"
                         Icon="{antdicons:AntDesignIconProvider EllipsisOutlined}">
        <atom:DropdownButton.DropdownFlyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="Report" Icon="{antdicons:AntDesignIconProvider Kind=WarningOutlined}" />
                <atom:MenuItem Header="Mail" Icon="{antdicons:AntDesignIconProvider Kind=MailOutlined}" />
                <atom:MenuItem Header="Mobile" Icon="{antdicons:AntDesignIconProvider Kind=MobileOutlined}" />
            </atom:MenuFlyout>
        </atom:DropdownButton.DropdownFlyout>
    </atom:DropdownButton>
</atom:CompactSpace>
```

### Primary 按钮组

```xml
<atom:CompactSpace Orientation="Horizontal">
    <atom:Button ButtonType="Primary">Button 1</atom:Button>
    <atom:Button ButtonType="Primary">Button 2</atom:Button>
    <atom:Button ButtonType="Primary">Button 3</atom:Button>
    <atom:Button ButtonType="Primary">Button 4</atom:Button>
</atom:CompactSpace>
```

### 混合按钮组（Default + Primary + Dropdown）

```xml
<atom:CompactSpace Orientation="Horizontal">
    <atom:Button ButtonType="Default">Button 1</atom:Button>
    <atom:Button ButtonType="Default">Button 2</atom:Button>
    <atom:Button ButtonType="Default">Button 3</atom:Button>
    <atom:Button ButtonType="Primary">Button 4</atom:Button>
    <atom:DropdownButton TriggerType="Click"
                         Placement="BottomEdgeAlignedRight"
                         ButtonType="Primary"
                         IsShowOpenIndicator="False"
                         Icon="{antdicons:AntDesignIconProvider EllipsisOutlined}">
        <atom:DropdownButton.DropdownFlyout>
            <atom:MenuFlyout>
                <atom:MenuItem Header="1st item" />
                <atom:MenuItem Header="2nd item" />
                <atom:MenuItem Header="3rd item" />
            </atom:MenuFlyout>
        </atom:DropdownButton.DropdownFlyout>
    </atom:DropdownButton>
</atom:CompactSpace>
```

---

## 10. 垂直紧凑模式

```xml
<StackPanel Spacing="10" Orientation="Horizontal">
    <atom:CompactSpace Orientation="Vertical">
        <atom:Button ButtonType="Default">Button 1</atom:Button>
        <atom:Button ButtonType="Default">Button 2</atom:Button>
        <atom:Button ButtonType="Default">Button 3</atom:Button>
    </atom:CompactSpace>

    <atom:CompactSpace Orientation="Vertical">
        <atom:Button ButtonType="Dashed">Button 1</atom:Button>
        <atom:Button ButtonType="Dashed">Button 2</atom:Button>
        <atom:Button ButtonType="Dashed">Button 3</atom:Button>
    </atom:CompactSpace>

    <atom:CompactSpace Orientation="Vertical">
        <atom:Button ButtonType="Primary">Button 1</atom:Button>
        <atom:Button ButtonType="Primary">Button 2</atom:Button>
        <atom:Button ButtonType="Primary">Button 3</atom:Button>
    </atom:CompactSpace>
</StackPanel>
```

---

## 11. CompactSpace 小尺寸

通过 `SizeType="Small"` 切换紧凑空间的子控件尺寸：

```xml
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch" SizeType="Small">
    <atom:LineEdit Text="https://atomui.net" atom:CompactSpace.ItemSize="4*" />
    <atom:Button ButtonType="Primary">Submit</atom:Button>
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="5*" />
</atom:CompactSpace>
```

---

## 常见组合模式

### 操作栏

```xml
<atom:Space>
    <atom:Button ButtonType="Primary">Submit</atom:Button>
    <atom:Button>Cancel</atom:Button>
    <atom:Button ButtonType="Dashed">Draft</atom:Button>
</atom:Space>
```

### 链接分割

```xml
<atom:Space ItemsAlignment="Center">
    <atom:Space.SplitTemplate>
        <Template>
            <atom:Separator Orientation="Vertical" />
        </Template>
    </atom:Space.SplitTemplate>
    <atom:HyperLinkTextBlock>Home</atom:HyperLinkTextBlock>
    <atom:HyperLinkTextBlock>About</atom:HyperLinkTextBlock>
    <atom:HyperLinkTextBlock>Contact</atom:HyperLinkTextBlock>
</atom:Space>
```

### 搜索栏

```xml
<atom:CompactSpace Orientation="Horizontal" HorizontalAlignment="Stretch">
    <atom:Select DefaultValues="All">
        <atom:SelectOption Header="All" Content="All" />
        <atom:SelectOption Header="Title" Content="Title" />
        <atom:SelectOption Header="Author" Content="Author" />
    </atom:Select>
    <atom:LineEdit PlaceholderText="Search..." atom:CompactSpace.ItemSize="4*" />
    <atom:Button ButtonType="Primary"
                 Icon="{antdicons:AntDesignIconProvider SearchOutlined}">
        Search
    </atom:Button>
    <atom:CompactSpaceFiller atom:CompactSpace.ItemSize="4*" />
</atom:CompactSpace>
```

### 数据绑定

```xml
<atom:Space SizeType="{Binding SpaceSizeType}" Orientation="{Binding SpaceOrientation}">
    <atom:Button ButtonType="Primary">Submit</atom:Button>
    <atom:Button>Cancel</atom:Button>
</atom:Space>
```
