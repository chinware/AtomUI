# ListView 使用文档

本文档介绍 AtomUI ListView 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 ListView，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;    // ListView, ListViewItem, Pagination
using AtomUI.Controls;             // SizeType 等共享类型
using AtomUI.Controls.Data;        // IListItemData, ListItemData, ListSortDescription
```

---

## 1. 基础用法

ListView 通过 `ItemsSource` 绑定数据源，数据项需要实现 `IListItemData` 接口：

```xml
<atom:ListView ItemsSource="{Binding ListItems}" />
```

```csharp
// ViewModel 或 Code-behind 中准备数据
var listItems = new List<IListItemData>
{
    new ListItemData { Content = "Blue" },
    new ListItemData { Content = "Green" },
    new ListItemData { Content = "Red" },
    new ListItemData { Content = "Yellow" }
};
```

默认情况下，ListView 使用内置的 `ItemTemplate` 渲染 `IListItemData.Content` 属性为文本：

```xml
<!-- 默认 ItemTemplate（内置，通常无需手动指定） -->
<DataTemplate x:DataType="atom:IListItemData">
    <atom:TextBlock Text="{Binding Content}" />
</DataTemplate>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Basic usage" 示例。

---

## 2. 选择模式

通过 `SelectionMode` 属性设置选择模式，结合 `IsShowSelectedIndicator` 展示选中状态：

```xml
<atom:ListView SelectionMode="Single"
               ItemsSource="{Binding SelectionListItems}"
               IsShowSelectedIndicator="True" />
```

### 单选模式（默认）

```xml
<atom:ListView SelectionMode="Single"
               ItemsSource="{Binding Items}"
               IsShowSelectedIndicator="True" />
```

### 多选模式

```xml
<atom:ListView SelectionMode="Multiple"
               ItemsSource="{Binding Items}"
               IsShowSelectedIndicator="True" />
```

### Toggle 模式

```xml
<atom:ListView SelectionMode="Toggle"
               ItemsSource="{Binding Items}"
               IsShowSelectedIndicator="True" />
```

### 动态切换选择模式

```xml
<StackPanel Spacing="20">
    <StackPanel Orientation="Horizontal" Spacing="5">
        <atom:TextBlock VerticalAlignment="Center">Selection Mode:</atom:TextBlock>
        <atom:OptionButtonGroup ButtonStyle="Solid" Name="SelectionModeOptionGroup">
            <atom:OptionButton IsChecked="True"
                               Tag="{x:Static SelectionMode.Single}">Single</atom:OptionButton>
            <atom:OptionButton Tag="{x:Static SelectionMode.Multiple}">Multiple</atom:OptionButton>
            <atom:OptionButton Tag="{x:Static SelectionMode.Toggle}">Toggle</atom:OptionButton>
        </atom:OptionButtonGroup>
    </StackPanel>
    <atom:ListView SelectionMode="{Binding SelectionMode}"
                   ItemsSource="{Binding SelectionListItems}"
                   IsShowSelectedIndicator="True" />
</StackPanel>
```

```csharp
// Code-behind 处理选择模式切换
SelectionModeOptionGroup.OptionCheckedChanged += (sender, e) =>
{
    if (DataContext is ListViewModel viewModel &&
        e.CheckedOption.IsChecked == true &&
        e.CheckedOption.Tag is SelectionMode selectionMode)
    {
        viewModel.SelectionMode = selectionMode;
    }
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Selection" 示例。

---

## 3. 分组

通过 `IsGroupEnabled="True"` 开启分组功能。数据项需要实现 `IGroupHeader` 接口或通过 `Group` 属性指定分组：

```xml
<atom:ListView ItemsSource="{Binding GroupListItems}"
               IsGroupEnabled="True"
               Height="300" />
```

```csharp
// 数据准备 — 使用 Group 属性指定分组
var groupListItems = new List<IListItemData>
{
    new ListItemData { Content = "Red",    Group = "Basic Colors" },
    new ListItemData { Content = "Orange", Group = "Basic Colors" },
    new ListItemData { Content = "Green",  Group = "Basic Colors" },
    new ListItemData { Content = "Blue",   Group = "Basic Colors" },
    new ListItemData { Content = "Brown",  Group = "Neutral Colors" },
    new ListItemData { Content = "White",  Group = "Neutral Colors" },
    new ListItemData { Content = "Black",  Group = "Neutral Colors" },
    new ListItemData { Content = "Turquoise", Group = "Specific Shades" },
    new ListItemData { Content = "Violet",    Group = "Specific Shades" },
    new ListItemData { Content = "Magenta",   Group = "Specific Shades" },
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Group" 示例。

---

## 4. 禁用项

通过设置 `IListItemData.IsEnabled = false` 禁用特定列表项：

```xml
<atom:ListView ItemsSource="{Binding ListItemsWidthDisabled}" />
```

```csharp
var listItems = new List<IListItemData>
{
    new ListItemData { Content = "Blue" },
    new ListItemData { Content = "Green" },
    new ListItemData { Content = "Red" },
    new ListItemData { Content = "Yellow", IsEnabled = false }  // 此项被禁用
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Item disabled" 示例。

---

## 5. 空状态

当数据源为空时，ListView 自动显示空状态指示器。通过按钮动态增删项目来演示空/非空切换：

```xml
<StackPanel Spacing="12">
    <StackPanel Orientation="Horizontal" Spacing="8">
        <atom:Button Content="Add Item" Click="HandleAddEmptyItemClicked" />
        <atom:Button Content="Remove Item" Click="HandleRemoveEmptyItemClicked" />
    </StackPanel>
    <atom:ListView ItemsSource="{Binding EmptyDemoItems}"
                   IsShowEmptyIndicator="True" />
</StackPanel>
```

```csharp
private void HandleAddEmptyItemClicked(object? sender, RoutedEventArgs e)
{
    if (DataContext is not ListViewModel viewModel) return;

    var items = viewModel.EmptyDemoItems != null
        ? new List<IListItemData>(viewModel.EmptyDemoItems)
        : new List<IListItemData>();

    items.Add(new ListItemData { Content = "Dynamic item" });
    viewModel.EmptyDemoItems = items;
}

private void HandleRemoveEmptyItemClicked(object? sender, RoutedEventArgs e)
{
    if (DataContext is not ListViewModel viewModel) return;

    if (viewModel.EmptyDemoItems is null || viewModel.EmptyDemoItems.Count <= 1)
    {
        viewModel.EmptyDemoItems = [];
        return;
    }

    var items = new List<IListItemData>(viewModel.EmptyDemoItems);
    items.RemoveAt(items.Count - 1);
    viewModel.EmptyDemoItems = items;
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Empty" 示例。

---

## 6. 过滤

通过 `Filter` 和 `FilterValue` 属性对数据进行过滤。可使用内置的 `ValueFilterProvider` 标记扩展：

```xml
<atom:ListView ItemsSource="{Binding FilteredGroupListItems}"
               Filter="{atom:ValueFilterProvider Contains}"
               FilterValue="a" />
```

上述配置会过滤出内容包含字母 "a" 的所有项目。

### 自定义过滤值提取

如果需要按数据项的特定属性过滤，可设置 `FilterValueSelector`：

```csharp
// 按自定义属性过滤
listView.FilterValueSelector = record =>
{
    if (record is MyDataItem item)
        return item.Category;
    return null;
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Filter" 示例。

---

## 7. 排序

通过 `SortDescriptions` 属性对数据进行排序：

```xml
<atom:ListView Name="OrderedList"
               ItemsSource="{Binding OrderedGroupListItems}"
               IsGroupEnabled="True" />
```

```csharp
// Code-behind 设置排序
OrderedList.SortDescriptions = [ListSortDescription.FromPath("Content")];
```

`ListSortDescription.FromPath` 根据属性路径创建排序描述，默认为升序。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Ordered" 示例。

---

## 8. 分页

ListView 支持内置分页功能，通过 `BottomPagination`（或 `TopPagination`）放置分页器，配合 `PageSize` 设置每页数量：

```xml
<atom:ListView ItemsSource="{Binding PaginationListItems}"
               PaginationVisibility="Bottom"
               SelectionMode="Multiple"
               Height="400"
               PageSize="100">
    <atom:ListView.BottomPagination>
        <atom:Pagination />
    </atom:ListView.BottomPagination>
</atom:ListView>
```

```csharp
// 准备大量数据用于分页
var list = new List<IListItemData>();
for (var i = 0; i < 2000; i++)
{
    list.Add(new ListItemData
    {
        ItemKey = $"{i}",
        Content = $"Content {i}"
    });
}
viewModel.PaginationListItems = list;
```

### 分页器位置控制

```xml
<!-- 仅顶部分页 -->
<atom:ListView PaginationVisibility="Top" PageSize="50">
    <atom:ListView.TopPagination>
        <atom:Pagination />
    </atom:ListView.TopPagination>
</atom:ListView>

<!-- 顶部 + 底部同时显示 -->
<atom:ListView PaginationVisibility="Both" PageSize="50">
    <atom:ListView.TopPagination>
        <atom:Pagination />
    </atom:ListView.TopPagination>
    <atom:ListView.BottomPagination>
        <atom:Pagination />
    </atom:ListView.BottomPagination>
</atom:ListView>

<!-- 隐藏分页（仍使用 PageSize 分页数据） -->
<atom:ListView PaginationVisibility="None" PageSize="50" />
```

### 分页器对齐

```xml
<atom:ListView TopPaginationAlign="Center"
               BottomPaginationAlign="End"
               PageSize="50">
    <!-- ... -->
</atom:ListView>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` 中 "Pagination list" 示例。

---

## 9. 选择绑定

### 绑定 SelectedItem

```xml
<atom:ListView ItemsSource="{Binding Items}"
               SelectedItem="{Binding CurrentItem}" />
```

### 绑定 SelectedIndex

```xml
<atom:ListView ItemsSource="{Binding Items}"
               SelectedIndex="{Binding CurrentIndex}" />
```

### 多选绑定 SelectedItems

```xml
<atom:ListView ItemsSource="{Binding Items}"
               SelectionMode="Multiple"
               SelectedItems="{Binding SelectedItemsList}" />
```

### 使用 SelectedValue + SelectedValueBinding

```xml
<atom:ListView ItemsSource="{Binding Items}"
               SelectedValue="{Binding SelectedId}"
               SelectedValueBinding="{Binding ItemKey}" />
```

### 监听选择变化事件

```xml
<atom:ListView ItemsSource="{Binding Items}"
               SelectionChanged="HandleSelectionChanged" />
```

```csharp
private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    foreach (var added in e.AddedItems)
    {
        // 处理新选中的项
    }
    foreach (var removed in e.RemovedItems)
    {
        // 处理取消选中的项
    }
}
```

---

## 10. 加载状态

通过 `IsOperating` 属性展示加载状态覆盖层：

```xml
<atom:ListView ItemsSource="{Binding Items}"
               IsOperating="{Binding IsLoading}"
               OperatingMsg="Loading data..." />
```

```csharp
// 模拟异步加载
public async Task LoadDataAsync()
{
    IsLoading = true;
    try
    {
        var data = await _service.GetDataAsync();
        Items = data;
    }
    finally
    {
        IsLoading = false;
    }
}
```

---

## 11. 监听项目点击

```xml
<atom:ListView ItemsSource="{Binding Items}"
               ItemClicked="HandleItemClicked" />
```

```csharp
private void HandleItemClicked(object? sender, ListViewItemClickedEventArgs e)
{
    var clickedItem = e.Item;
    // 处理点击逻辑
}
```

---

## 常见组合模式

### 基础数据列表

```xml
<atom:ListView ItemsSource="{Binding Items}"
               IsShowSelectedIndicator="True"
               SelectionMode="Single" />
```

### 可搜索的过滤列表

```xml
<StackPanel Spacing="10">
    <atom:SearchEdit PlaceholderText="Search..."
                     TextChanged="HandleSearchTextChanged" />
    <atom:ListView Name="FilterableList"
                   ItemsSource="{Binding Items}"
                   Filter="{atom:ValueFilterProvider Contains}" />
</StackPanel>
```

```csharp
private void HandleSearchTextChanged(object? sender, TextChangedEventArgs e)
{
    if (sender is SearchEdit searchEdit)
    {
        FilterableList.FilterValue = searchEdit.Text?.Trim();
    }
}
```

### 分组 + 排序列表

```xml
<atom:ListView ItemsSource="{Binding GroupItems}"
               IsGroupEnabled="True"
               Height="400" />
```

```csharp
// 同时设置排序
listView.SortDescriptions = [ListSortDescription.FromPath("Content")];
```

### 大数据分页列表

```xml
<atom:ListView ItemsSource="{Binding LargeDataSet}"
               PageSize="50"
               SelectionMode="Multiple"
               PaginationVisibility="Bottom"
               BottomPaginationAlign="End"
               Height="500">
    <atom:ListView.BottomPagination>
        <atom:Pagination />
    </atom:ListView.BottomPagination>
</atom:ListView>
```
