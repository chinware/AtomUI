# Segmented 使用文档

本文档介绍 AtomUI Segmented 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/SegmentedShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Segmented，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Segmented, SegmentedItem
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最基本的 Segmented 用法——在几个文本选项之间切换：

```xml
<atom:Segmented>
    <atom:SegmentedItem>Daily</atom:SegmentedItem>
    <atom:SegmentedItem>Weekly</atom:SegmentedItem>
    <atom:SegmentedItem>Monthly</atom:SegmentedItem>
    <atom:SegmentedItem>Quarterly</atom:SegmentedItem>
    <atom:SegmentedItem>Yearly</atom:SegmentedItem>
</atom:Segmented>
```

**默认行为**：首次渲染时自动选中第一个选项（`SelectedIndex = 0`），点击其他选项时色块平滑滑动到新位置。

---

## 2. Block 模式

通过 `IsExpanding="True"` 让 Segmented 撑满父容器，所有选项均分宽度：

```xml
<atom:Segmented IsExpanding="True">
    <atom:SegmentedItem>123</atom:SegmentedItem>
    <atom:SegmentedItem>456</atom:SegmentedItem>
    <atom:SegmentedItem>longtext-longtext-longtext-longtext</atom:SegmentedItem>
</atom:Segmented>
```

**使用场景**：当 Segmented 作为内容区域的主切换器，需要与容器等宽排列时使用。

---

## 3. 三种尺寸

通过 `SizeType` 属性设置尺寸，支持 `Large`、`Middle`（默认）、`Small` 三种。

```xml
<!-- 大号 -->
<atom:Segmented SizeType="Large">
    <atom:SegmentedItem>Daily</atom:SegmentedItem>
    <atom:SegmentedItem>Weekly</atom:SegmentedItem>
    <atom:SegmentedItem>Monthly</atom:SegmentedItem>
    <atom:SegmentedItem>Quarterly</atom:SegmentedItem>
    <atom:SegmentedItem>Yearly</atom:SegmentedItem>
</atom:Segmented>

<!-- 中号（默认） -->
<atom:Segmented>
    <atom:SegmentedItem>Daily</atom:SegmentedItem>
    <atom:SegmentedItem>Weekly</atom:SegmentedItem>
    <atom:SegmentedItem>Monthly</atom:SegmentedItem>
    <atom:SegmentedItem>Quarterly</atom:SegmentedItem>
    <atom:SegmentedItem>Yearly</atom:SegmentedItem>
</atom:Segmented>

<!-- 小号 -->
<atom:Segmented SizeType="Small">
    <atom:SegmentedItem>Daily</atom:SegmentedItem>
    <atom:SegmentedItem>Weekly</atom:SegmentedItem>
    <atom:SegmentedItem>Monthly</atom:SegmentedItem>
    <atom:SegmentedItem>Quarterly</atom:SegmentedItem>
    <atom:SegmentedItem>Yearly</atom:SegmentedItem>
</atom:Segmented>
```

---

## 4. 纯图标选项

每个选项仅显示图标（不设 Content）：

```xml
<atom:Segmented>
    <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=BarsOutlined}" />
    <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}" />
</atom:Segmented>
```

**使用场景**：视图模式切换（列表视图/网格视图）等图标本身语义明确的场景。

---

## 5. 图标 + 文字选项

同时设置 `Icon` 和 `Content`，图标显示在文字左侧：

```xml
<atom:Segmented>
    <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=BarsOutlined}">
        List
    </atom:SegmentedItem>
    <atom:SegmentedItem Content="Kanban"
                        Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}" />
</atom:Segmented>
```

图标 + 文字可与尺寸组合使用：

```xml
<atom:Segmented SizeType="Large">
    <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=BarsOutlined}">
        Ava 牛逼
    </atom:SegmentedItem>
    <atom:SegmentedItem Content="群主牛逼"
                        Icon="{antdicons:AntDesignIconProvider Kind=WechatOutlined}" />
    <atom:SegmentedItem Content="微软牛逼"
                        Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}" />
</atom:Segmented>
```

---

## 6. 禁用选项

### 全部禁用

```xml
<atom:Segmented>
    <atom:SegmentedItem IsEnabled="False">Map</atom:SegmentedItem>
    <atom:SegmentedItem IsEnabled="False">Transit</atom:SegmentedItem>
    <atom:SegmentedItem IsEnabled="False">Satellite</atom:SegmentedItem>
</atom:Segmented>
```

### 部分禁用

```xml
<atom:Segmented>
    <atom:SegmentedItem>Daily</atom:SegmentedItem>
    <atom:SegmentedItem IsEnabled="False">Weekly</atom:SegmentedItem>
    <atom:SegmentedItem>Monthly</atom:SegmentedItem>
    <atom:SegmentedItem IsEnabled="False">Quarterly</atom:SegmentedItem>
    <atom:SegmentedItem>Yearly</atom:SegmentedItem>
</atom:Segmented>
```

**说明**：禁用的选项文字变为灰色（`ColorTextDisabled`），不响应点击操作，也不会显示悬浮态背景色。

---

## 7. 选中项监听

### 通过事件

```xml
<atom:Segmented SelectionChanged="OnSegmentedSelectionChanged">
    <atom:SegmentedItem>Option A</atom:SegmentedItem>
    <atom:SegmentedItem>Option B</atom:SegmentedItem>
    <atom:SegmentedItem>Option C</atom:SegmentedItem>
</atom:Segmented>
```

```csharp
private void OnSegmentedSelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    if (sender is Segmented segmented)
    {
        var selectedIndex = segmented.SelectedIndex;
        var selectedItem = segmented.SelectedItem;
        // 处理选中变化...
    }
}
```

### 通过数据绑定

```xml
<atom:Segmented SelectedIndex="{Binding SelectedSegmentIndex}">
    <atom:SegmentedItem>Daily</atom:SegmentedItem>
    <atom:SegmentedItem>Weekly</atom:SegmentedItem>
    <atom:SegmentedItem>Monthly</atom:SegmentedItem>
</atom:Segmented>
```

```csharp
public class MyViewModel : ReactiveObject
{
    private int _selectedSegmentIndex;
    public int SelectedSegmentIndex
    {
        get => _selectedSegmentIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedSegmentIndex, value);
    }
}
```

---

## 8. 数据绑定（ItemsSource）

可以使用 `ItemsSource` 绑定数据集合，配合 `ItemTemplate` 定义选项外观：

```xml
<atom:Segmented ItemsSource="{Binding TimeRanges}"
                SelectedItem="{Binding SelectedTimeRange}">
    <atom:Segmented.ItemTemplate>
        <DataTemplate>
            <atom:TextBlock Text="{Binding}" />
        </DataTemplate>
    </atom:Segmented.ItemTemplate>
</atom:Segmented>
```

```csharp
public class MyViewModel : ReactiveObject
{
    public ObservableCollection<string> TimeRanges { get; } = new()
    {
        "Daily", "Weekly", "Monthly", "Quarterly", "Yearly"
    };
    
    private string? _selectedTimeRange;
    public string? SelectedTimeRange
    {
        get => _selectedTimeRange;
        set => this.RaiseAndSetIfChanged(ref _selectedTimeRange, value);
    }
}
```

---

## 9. 控制动画行为

```xml
<!-- 禁用色块滑动动画和选项背景色过渡 -->
<atom:Segmented IsMotionEnabled="False">
    <atom:SegmentedItem>A</atom:SegmentedItem>
    <atom:SegmentedItem>B</atom:SegmentedItem>
    <atom:SegmentedItem>C</atom:SegmentedItem>
</atom:Segmented>
```

---

## 常见组合模式

### 视图模式切换

```xml
<StackPanel Orientation="Vertical" Spacing="16">
    <atom:Segmented SelectionChanged="OnViewModeChanged">
        <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=BarsOutlined}">
            List
        </atom:SegmentedItem>
        <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}">
            Grid
        </atom:SegmentedItem>
    </atom:Segmented>
    
    <!-- 根据选中项切换内容视图 -->
</StackPanel>
```

### 时间范围选择器

```xml
<atom:Segmented SizeType="Small">
    <atom:SegmentedItem>Day</atom:SegmentedItem>
    <atom:SegmentedItem>Week</atom:SegmentedItem>
    <atom:SegmentedItem>Month</atom:SegmentedItem>
    <atom:SegmentedItem>Year</atom:SegmentedItem>
</atom:Segmented>
```

### 撑满宽度的导航

```xml
<Border Width="500">
    <atom:Segmented IsExpanding="True">
        <atom:SegmentedItem>Overview</atom:SegmentedItem>
        <atom:SegmentedItem>Details</atom:SegmentedItem>
        <atom:SegmentedItem>Settings</atom:SegmentedItem>
    </atom:Segmented>
</Border>
```

### 带禁用和图标的混合使用

```xml
<atom:Segmented>
    <atom:SegmentedItem Icon="{antdicons:AntDesignIconProvider Kind=BarsOutlined}">
        Ava 牛逼
    </atom:SegmentedItem>
    <atom:SegmentedItem Content="群主牛逼"
                        Icon="{antdicons:AntDesignIconProvider Kind=WechatOutlined}"
                        IsEnabled="False" />
    <atom:SegmentedItem Content="微软牛逼"
                        Icon="{antdicons:AntDesignIconProvider Kind=WindowsOutlined}" />
</atom:Segmented>
```
