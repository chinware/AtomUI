# Badge 使用文档

本文档介绍 AtomUI Badge 系列控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/BadgeShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Badge，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // CountBadge, DotBadge, RibbonBadge
using AtomUI.Controls.Commons;    // CountBadgeSize, DotBadgeStatus, RibbonBadgePlacement
```

---

## 1. CountBadge — 数字徽标

### 基本用法

最简单的用法是用 CountBadge 包裹目标控件，并设置 `Count`：

```xml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="5">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="0" ShowZero="True">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

**要点**：
- `Count > 0` 时自动显示数字气泡
- `Count == 0` 时默认隐藏，设置 `ShowZero="True"` 可强制显示

### 封顶数字

当 `Count > OverflowCount` 时显示 `{OverflowCount}+`：

```xml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="99">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="100">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="99" OverflowCount="10">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="1000" OverflowCount="999">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

### 偏移量

通过 `Offset` 属性精细调整徽标位置：

```xml
<atom:CountBadge Count="5" Offset="10, 10">
    <Border Width="40" Height="40"
            Background="rgb(191,191,191)" CornerRadius="8" />
</atom:CountBadge>
```

### 尺寸

通过 `Size` 属性切换默认尺寸和小号尺寸：

```xml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="5">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
    <atom:CountBadge Count="5" Size="Small">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
</StackPanel>
```

### 独立使用

不设置 `DecoratedTarget`，CountBadge 作为独立元素显示数字气泡：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:ToggleSwitch IsChecked="{Binding StandaloneSwitchChecked}" />
    <atom:CountBadge BadgeColor="#faad14"
                     Count="{Binding StandaloneBadgeCount1}"
                     ShowZero="True" />
    <atom:CountBadge Count="{Binding StandaloneBadgeCount2}" />
    <atom:CountBadge BadgeColor="#52c41a"
                     Count="{Binding StandaloneBadgeCount3}" />
</StackPanel>
```

```csharp
// ViewModel
private void HandleStandaloneSwitchChecked(bool value)
{
    if (value)
    {
        StandaloneBadgeCount1 = 11;
        StandaloneBadgeCount2 = 25;
        StandaloneBadgeCount3 = 109;
    }
    else
    {
        StandaloneBadgeCount1 = 0;
        StandaloneBadgeCount2 = 0;
        StandaloneBadgeCount3 = 0;
    }
}
```

---

## 2. DotBadge — 圆点徽标

### 装饰模式（小红点）

给图标或链接按钮添加红点标记：

```xml
<StackPanel Orientation="Horizontal">
    <atom:DotBadge Offset="-7,8">
        <atom:Button ButtonType="Link"
                     Icon="{antdicons:AntDesignIconProvider Kind=NotificationOutlined}" />
    </atom:DotBadge>
    <atom:DotBadge Offset="-14,12">
        <atom:Button ButtonType="Link" Content="Link something" />
    </atom:DotBadge>
</StackPanel>
```

**提示**：通过 `Offset` 属性可以精细调整红点位置，以适配不同形状的目标控件。

### 五种语义状态

独立模式下使用 `Status` 属性，配合 `Text` 显示状态文字：

```xml
<!-- 仅圆点 -->
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:DotBadge Status="Success" />
    <atom:DotBadge Status="Error" />
    <atom:DotBadge Status="Default" />
    <atom:DotBadge Status="Processing" />
    <atom:DotBadge Status="Warning" />
</StackPanel>

<!-- 圆点 + 状态文字 -->
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:DotBadge Status="Success" Text="Success" />
    <atom:DotBadge Status="Error" Text="Error" />
    <atom:DotBadge Status="Default" Text="Default" />
    <atom:DotBadge Status="Processing" Text="Processing" />
    <atom:DotBadge Status="Warning" Text="Warning" />
</StackPanel>
```

### 预设颜色

通过 `DotColor` 属性使用 13 种预设色名：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:DotBadge DotColor="Pink" Text="Pink" />
    <atom:DotBadge DotColor="Red" Text="Red" />
    <atom:DotBadge DotColor="Yellow" Text="Yellow" />
    <atom:DotBadge DotColor="Orange" Text="Orange" />
    <atom:DotBadge DotColor="Cyan" Text="Cyan" />
    <atom:DotBadge DotColor="Green" Text="Green" />
    <atom:DotBadge DotColor="Blue" Text="Blue" />
    <atom:DotBadge DotColor="Purple" Text="Purple" />
    <atom:DotBadge DotColor="GeekBlue" Text="GeekBlue" />
    <atom:DotBadge DotColor="Magenta" Text="Magenta" />
    <atom:DotBadge DotColor="Volcano" Text="Volcano" />
    <atom:DotBadge DotColor="Gold" Text="Gold" />
    <atom:DotBadge DotColor="Lime" Text="Lime" />
</StackPanel>
```

### 自定义 CSS 颜色

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:DotBadge DotColor="#f50" Text="#f50" />
    <atom:DotBadge DotColor="rgb(45, 183, 245)" Text="rgb(45, 183, 245)" />
    <atom:DotBadge DotColor="hsl(102, 53%, 61%)" Text="hsl(102, 53%, 61%)" />
    <atom:DotBadge DotColor="rgb(15, 141, 230)" Text="rgb(15, 141, 230)" />
</StackPanel>
```

---

## 3. RibbonBadge — 缎带徽标

### 基本用法

用 RibbonBadge 包裹卡片，缎带默认显示在右上角：

```xml
<atom:RibbonBadge Text="精益求精，打造体验优秀的 UISDK">
    <Border Padding="10,0,10,0"
            BorderBrush="#d9d9d9" BorderThickness="1" CornerRadius="6">
        <StackPanel Orientation="Vertical">
            <atom:TextBlock Height="38" FontWeight="Bold" LineHeight="38">
                Pushes open the window
            </atom:TextBlock>
            <atom:Separator LineColor="#d9d9d9" Orientation="Horizontal" />
            <atom:TextBlock Margin="0,10,0,0">
                and raises the spyglass.
            </atom:TextBlock>
        </StackPanel>
    </Border>
</atom:RibbonBadge>
```

### 预设颜色

```xml
<atom:RibbonBadge RibbonColor="Pink" Text="甲辰计划雄起">
    <Border Padding="10,0,10,0" BorderBrush="#d9d9d9"
            BorderThickness="1" CornerRadius="6">
        <!-- 内容 -->
    </Border>
</atom:RibbonBadge>

<atom:RibbonBadge RibbonColor="Cyan" Text="Avalonia 非常优秀">
    <!-- ... -->
</atom:RibbonBadge>

<atom:RibbonBadge RibbonColor="Green" Text="Hippies">
    <!-- ... -->
</atom:RibbonBadge>
```

### 左上角放置

通过 `Placement="Start"` 将缎带放在左上角：

```xml
<atom:RibbonBadge Placement="Start" RibbonColor="purple" Text="Hippies">
    <Border Padding="10,0,10,0" BorderBrush="#d9d9d9"
            BorderThickness="1" CornerRadius="6">
        <!-- 内容 -->
    </Border>
</atom:RibbonBadge>

<atom:RibbonBadge Placement="Start" RibbonColor="volcano" Text="Hippies">
    <!-- ... -->
</atom:RibbonBadge>

<atom:RibbonBadge Placement="Start" RibbonColor="magenta" Text="Hippies">
    <!-- ... -->
</atom:RibbonBadge>
```

---

## 4. 动态交互

### 动态增减数字

通过数据绑定实时更新 CountBadge 的数字：

```xml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge Count="{Binding DynamicBadgeCount}" OverflowCount="99">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
    <StackPanel VerticalAlignment="Center" Orientation="Horizontal" Spacing="10">
        <atom:Button Command="{Binding AddDynamicBadgeCount}" SizeType="Small">Add</atom:Button>
        <atom:Button Command="{Binding SubDynamicBadgeCount}" SizeType="Small">Sub</atom:Button>
        <atom:Button Command="{Binding RandomDynamicBadgeCount}" SizeType="Small">Random</atom:Button>
    </StackPanel>
</StackPanel>
```

```csharp
// ViewModel
public void AddDynamicBadgeCount()
{
    DynamicBadgeCount += 1;
}

public void SubDynamicBadgeCount()
{
    var value = DynamicBadgeCount;
    value -= 1;
    value = Math.Max(value, 0);
    DynamicBadgeCount = value;
}

public void RandomDynamicBadgeCount()
{
    var random = new Random();
    DynamicBadgeCount = random.Next(0, 110);
}
```

### 动态显隐（带动画）

通过绑定 `BadgeIsVisible` 属性控制 Badge 的显示和隐藏，配合缩放动画效果：

```xml
<StackPanel Orientation="Horizontal" Spacing="20">
    <atom:CountBadge BadgeIsVisible="{Binding DynamicDotBadgeVisible}" Count="9">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:CountBadge>
    <atom:DotBadge BadgeIsVisible="{Binding DynamicDotBadgeVisible}">
        <Border Width="40" Height="40"
                Background="rgb(191,191,191)" CornerRadius="8" />
    </atom:DotBadge>
    <atom:ToggleSwitch VerticalAlignment="Center"
                       IsChecked="{Binding DynamicDotBadgeVisible, Mode=TwoWay}" />
</StackPanel>
```

---

## 常见组合模式

### 消息通知图标

```xml
<atom:CountBadge Count="{Binding UnreadCount}">
    <atom:Button ButtonType="Link"
                 Icon="{antdicons:AntDesignIconProvider Kind=NotificationOutlined}" />
</atom:CountBadge>
```

### 在线状态头像

```xml
<atom:DotBadge Status="Success">
    <Border Width="32" Height="32"
            Background="rgb(191,191,191)" CornerRadius="16" />
</atom:DotBadge>
```

### 卡片角标

```xml
<atom:RibbonBadge Text="Hot" RibbonColor="Red">
    <Border Padding="16" BorderBrush="#d9d9d9"
            BorderThickness="1" CornerRadius="8">
        <TextBlock>Card Content</TextBlock>
    </Border>
</atom:RibbonBadge>
```

### 购物车商品数

```xml
<atom:CountBadge Count="{Binding CartItemCount}" OverflowCount="99">
    <atom:Button ButtonType="Link"
                 Icon="{antdicons:AntDesignIconProvider Kind=ShoppingCartOutlined}" />
</atom:CountBadge>
```

### 链接上的红点提醒

```xml
<StackPanel Orientation="Horizontal">
    <atom:DotBadge Offset="-14,12">
        <atom:Button ButtonType="Link" Content="Messages" />
    </atom:DotBadge>
</StackPanel>
```

### 表格行状态指示

```xml
<StackPanel Orientation="Vertical" Spacing="8">
    <atom:DotBadge Status="Success" Text="Completed" />
    <atom:DotBadge Status="Processing" Text="In Progress" />
    <atom:DotBadge Status="Error" Text="Failed" />
    <atom:DotBadge Status="Warning" Text="Pending Review" />
    <atom:DotBadge Status="Default" Text="Draft" />
</StackPanel>
```
