# TabControl 使用文档

本文档介绍 AtomUI TabControl 控件家族的常见使用方式，包含 TabControl 系列（带内容面板）和 TabStrip 系列（仅标签栏）。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Navigation/TabControlShowCase.axaml`

---

## 前置准备

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

```csharp
using AtomUI.Desktop.Controls;
```

---

## 基本用法

### 线条式标签页

```xml
<atom:TabControl>
    <atom:TabItem Header="Tab 1">Content of Tab Pane 1</atom:TabItem>
    <atom:TabItem Header="Tab 2">Content of Tab Pane 2</atom:TabItem>
    <atom:TabItem Header="Tab 3">Content of Tab Pane 3</atom:TabItem>
</atom:TabControl>
```

### 卡片式标签页

```xml
<atom:CardTabControl>
    <atom:TabItem Header="Tab 1">Content of Tab Pane 1</atom:TabItem>
    <atom:TabItem Header="Tab 2">Content of Tab Pane 2</atom:TabItem>
    <atom:TabItem Header="Tab 3">Content of Tab Pane 3</atom:TabItem>
</atom:CardTabControl>
```

---

## 带图标的标签

```xml
<atom:TabControl>
    <atom:TabItem Header="Tab 1"
                  Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">
        Content of Tab Pane 1
    </atom:TabItem>
    <atom:TabItem Header="Tab 2"
                  Icon="{antdicons:AntDesignIconProvider Kind=AndroidOutlined}">
        Content of Tab Pane 2
    </atom:TabItem>
    <atom:TabItem Header="Tab 3"
                  Icon="{antdicons:AntDesignIconProvider Kind=WechatOutlined}">
        Content of Tab Pane 3
    </atom:TabItem>
</atom:TabControl>
```

---

## 禁用标签

```xml
<atom:TabControl>
    <atom:TabItem Header="Tab 1">Content of Tab Pane 1</atom:TabItem>
    <atom:TabItem Header="Tab 2" IsEnabled="False">Content of Tab Pane 2</atom:TabItem>
    <atom:TabItem Header="Tab 3">Content of Tab Pane 3</atom:TabItem>
</atom:TabControl>
```

---

## 标签居中

```xml
<atom:TabControl TabAlignmentCenter="True">
    <atom:TabItem Header="Tab 1">Content of Tab Pane 1</atom:TabItem>
    <atom:TabItem Header="Tab 2">Content of Tab Pane 2</atom:TabItem>
    <atom:TabItem Header="Tab 3">Content of Tab Pane 3</atom:TabItem>
</atom:TabControl>
```

---

## 四方向布局

```xml
<!-- 标签栏在左侧 -->
<atom:TabControl TabStripPlacement="Left">
    <atom:TabItem Header="Tab 1">Content of Tab Pane 1</atom:TabItem>
    <atom:TabItem Header="Tab 2">Content of Tab Pane 2</atom:TabItem>
    <atom:TabItem Header="Tab 3">Content of Tab Pane 3</atom:TabItem>
</atom:TabControl>

<!-- 卡片式四方向同样支持 -->
<atom:CardTabControl TabStripPlacement="Right" IsShowAddTabButton="True">
    <atom:TabItem Header="Tab 1">Content of Tab Pane 1</atom:TabItem>
    <atom:TabItem Header="Tab 2">Content of Tab Pane 2</atom:TabItem>
</atom:CardTabControl>
```

---

## 三种尺寸

```xml
<!-- 小号 -->
<atom:TabControl SizeType="Small">...</atom:TabControl>

<!-- 中号（默认） -->
<atom:TabControl SizeType="Middle">...</atom:TabControl>

<!-- 大号 -->
<atom:TabControl SizeType="Large">...</atom:TabControl>
```

---

## 可关闭标签

### 全局启用关闭按钮

```xml
<atom:CardTabControl IsTabClosable="True"
                     IsTabAutoHideCloseButton="True">
    <atom:TabItem Header="Tab 1">Content 1</atom:TabItem>
    <atom:TabItem Header="Tab 2" IsClosable="False">Content 2（不可关闭）</atom:TabItem>
    <atom:TabItem Header="Tab 3">Content 3</atom:TabItem>
</atom:CardTabControl>
```

### 处理关闭事件

```csharp
myTabControl.Closing += (sender, args) =>
{
    // 阻止关闭
    if (args.TabItem.Header?.ToString() == "重要标签")
    {
        args.Cancel = true;
    }
};

myTabControl.Closed += (sender, args) =>
{
    // 标签已关闭
    Console.WriteLine($"Closed: {args.TabItem.Header}");
};
```

---

## 添加标签按钮（仅卡片式）

```xml
<atom:CardTabControl IsShowAddTabButton="True"
                     Name="AddTabDemoTabControl">
    <atom:TabItem Header="Tab 1">Content 1</atom:TabItem>
    <atom:TabItem Header="Tab 2">Content 2</atom:TabItem>
</atom:CardTabControl>
```

```csharp
// Code-behind
AddTabDemoTabControl.AddTabRequest += (sender, args) =>
{
    var newTab = new TabItem
    {
        Header = $"New Tab {AddTabDemoTabControl.ItemCount + 1}",
        Content = $"Content {AddTabDemoTabControl.ItemCount + 1}"
    };
    AddTabDemoTabControl.Items.Add(newTab);
};
```

---

## 头部额外内容

```xml
<atom:TabControl>
    <atom:TabControl.HeaderStartExtraContent>
        <atom:Button>Start extra action</atom:Button>
    </atom:TabControl.HeaderStartExtraContent>
    <atom:TabControl.HeaderEndExtraContent>
        <atom:Button>End extra action</atom:Button>
    </atom:TabControl.HeaderEndExtraContent>
    <atom:TabItem Header="Tab 1">Content 1</atom:TabItem>
    <atom:TabItem Header="Tab 2">Content 2</atom:TabItem>
</atom:TabControl>
```

---

## 数据绑定（ItemsSource）

### 使用 ITabItemData 接口

```csharp
public class MyTabItemData : ITabItemData
{
    public object? Header { get; init; }
    public PathIcon? Icon { get; init; }
    public PathIcon? CloseIcon { get; init; }
    public bool IsEnabled { get; init; } = true;
    public bool IsClosable { get; init; }
    public bool IsAutoHideCloseButton { get; init; }
    
    // 额外的内容属性
    public string Content { get; init; } = "";
}
```

```xml
<atom:TabControl ItemsSource="{Binding TabItemDataSource}">
    <atom:TabControl.ItemTemplate>
        <DataTemplate x:DataType="local:MyTabItemData">
            <Border Padding="10">
                <TextBlock Text="{Binding Content}" />
            </Border>
        </DataTemplate>
    </atom:TabControl.ItemTemplate>
</atom:TabControl>
```

### 使用 TabItemData 默认实现

```csharp
var items = new ObservableCollection<TabItemData>
{
    new() { Header = "Tab 1", IsClosable = true },
    new() { Header = "Tab 2", IsEnabled = false },
    new() { Header = "Tab 3", Icon = new AppleOutlined() }
};
```

---

## TabStrip 系列（仅标签栏）

### 基本用法

```xml
<atom:TabStrip>
    <atom:TabStripItem>Tab 1</atom:TabStripItem>
    <atom:TabStripItem>Tab 2</atom:TabStripItem>
    <atom:TabStripItem>Tab 3</atom:TabStripItem>
</atom:TabStrip>
```

### 卡片式标签栏

```xml
<atom:CardTabStrip IsShowAddTabButton="True">
    <atom:TabStripItem Icon="{antdicons:AntDesignIconProvider Kind=AppleOutlined}">Tab 1</atom:TabStripItem>
    <atom:TabStripItem Icon="{antdicons:AntDesignIconProvider Kind=AndroidOutlined}">Tab 2</atom:TabStripItem>
</atom:CardTabStrip>
```

### 配合内容区域使用

TabStrip 不含内容面板，需要自行管理内容：

```xml
<DockPanel Height="300">
    <atom:TabStrip DockPanel.Dock="Top"
                   TabStripPlacement="Top"
                   Name="MyTabStrip">
        <atom:TabStripItem>Tab 1</atom:TabStripItem>
        <atom:TabStripItem>Tab 2</atom:TabStripItem>
    </atom:TabStrip>
    <Border HorizontalAlignment="Stretch"
            VerticalAlignment="Stretch"
            Background="{atom:SharedTokenResource ColorBgElevated}">
        <TextBlock HorizontalAlignment="Center"
                   VerticalAlignment="Center">
            Tab Content
        </TextBlock>
    </Border>
</DockPanel>
```

### TabStrip 数据绑定

```xml
<atom:TabStrip ItemsSource="{Binding TabStripItemDataSource}">
    <atom:TabStrip.ItemTemplate>
        <DataTemplate x:DataType="atom:TabItemData">
            <TextBlock Text="{Binding Header}" />
        </DataTemplate>
    </atom:TabStrip.ItemTemplate>
</atom:TabStrip>
```

---

## 编程方式关闭标签

```csharp
// 获取 TabItem 并关闭
if (myTabControl.ContainerFromIndex(2) is TabItem tabItem)
{
    bool closed = myTabControl.CloseTab(tabItem);
}
```
