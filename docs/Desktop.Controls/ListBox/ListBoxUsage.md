# ListBox 使用指南

本文档展示 `AtomUI.Desktop.Controls.ListBox` 控件的常见使用方式，所有示例均可在 Gallery 应用中找到对应演示。

> 📁 Gallery 演示位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml`

---

## 1. 基础用法 — 直接添加项目

最简单的方式是直接在 AXAML 中添加 `ListBoxItem` 子元素：

```xml
<atom:ListBox>
    <atom:ListBoxItem>Racing car sprays burning fuel into crowd.</atom:ListBoxItem>
    <atom:ListBoxItem>Japanese princess to wed commoner.</atom:ListBoxItem>
    <atom:ListBoxItem>Australian walks 100km after outback crash.</atom:ListBoxItem>
    <atom:ListBoxItem>Man charged over missing wedding girl.</atom:ListBoxItem>
    <atom:ListBoxItem>Los Angeles battles huge wildfires.</atom:ListBoxItem>
</atom:ListBox>
```

> 参考：`ListShowCase.axaml` 第 89–100 行

---

## 2. 数据绑定 — 使用 ItemsSource

通过 `ItemsSource` 绑定数据集合，配合 `IListItemData` 接口：

```xml
<atom:ListBox ItemsSource="{Binding BasicListBoxItems}"
              IsShowSelectedIndicator="True" />
```

**ViewModel 侧数据准备：**

```csharp
using AtomUI.Controls.Data;

// 使用 ListItemData（实现 IListItemData 接口）
viewModel.BasicListBoxItems = [
    new ListItemData { Content = "Racing car sprays burning fuel into crowd." },
    new ListItemData { Content = "Japanese princess to wed commoner." },
    new ListItemData { Content = "Australian walks 100km after outback crash." },
    new ListItemData { Content = "Man charged over missing wedding girl." },
    new ListItemData { Content = "Los Angeles battles huge wildfires." },
];
```

> 参考：`ListShowCase.axaml` 第 102–108 行，`ListShowCase.axaml.cs` 第 242–266 行

---

## 3. 选中指示器

设置 `IsShowSelectedIndicator="True"` 可在已选中项右侧显示勾选图标：

```xml
<atom:ListBox ItemsSource="{Binding Items}"
              IsShowSelectedIndicator="True" />
```

默认图标为 `CheckOutlined`。

---

## 4. 搜索过滤

通过 `ItemFilterValue` 属性触发过滤，配合 `SearchEdit` 实现搜索功能：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:SearchEdit PlaceholderText="Search"
                     SearchButtonClick="HandleFilterListBoxClicked" />
    <atom:ListBox Name="SearchListBox">
        <atom:ListBoxItem>Racing car sprays burning fuel into crowd.</atom:ListBoxItem>
        <atom:ListBoxItem>Japanese princess to wed commoner.</atom:ListBoxItem>
        <atom:ListBoxItem>Australian walks 100km after outback crash.</atom:ListBoxItem>
        <atom:ListBoxItem>Man charged over missing wedding girl.</atom:ListBoxItem>
        <atom:ListBoxItem>Los Angeles battles huge wildfires.</atom:ListBoxItem>
    </atom:ListBox>
</StackPanel>
```

**Code-behind：**

```csharp
private void HandleFilterListBoxClicked(object? sender, RoutedEventArgs e)
{
    if (sender is SearchEdit searchEdit)
    {
        SearchListBox.ItemFilterValue = searchEdit.Text?.Trim();
    }
}
```

> 参考：`ListShowCase.axaml` 第 110–124 行，`ListShowCase.axaml.cs` 第 305–311 行

### 过滤行为说明

- 当 `ItemFilterValue` 非空时，自动进入过滤模式（`IsFiltering == true`）。
- 默认使用 `DefaultListBoxItemFilter`，基于字符串包含匹配。
- 匹配的文本会根据 `ItemFilterHighlightStrategy` 进行高亮（默认红色）。
- 未匹配的项会被隐藏（当策略包含 `HideUnMatched`）。
- 设置 `ItemFilterValue = null` 可退出过滤模式。

### 自定义过滤器

实现 `IListBoxItemFilter` 接口可自定义过滤逻辑：

```csharp
public class MyCustomFilter : IListBoxItemFilter
{
    public bool Filter(ListBox listBox, ListBoxItem listBoxItem, object? filterValue)
    {
        // 自定义过滤逻辑
        var text = listBoxItem.Content?.ToString();
        var keyword = filterValue?.ToString();
        if (text == null || keyword == null) return false;
        return text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase);
    }
}
```

```xml
<atom:ListBox ItemFilter="{x:Static local:MyCustomFilter.Instance}"
              ItemFilterValue="{Binding SearchText}" />
```

---

## 5. 选择模式

ListBox 继承 Avalonia 的 `SelectionMode`，支持多种选择模式：

```xml
<!-- 单选（默认） -->
<atom:ListBox SelectionMode="Single" />

<!-- 多选 -->
<atom:ListBox SelectionMode="Multiple" />

<!-- 切换选择 -->
<atom:ListBox SelectionMode="Toggle" />

<!-- 始终保持选中 -->
<atom:ListBox SelectionMode="AlwaysSelected" />
```

### 禁用选择

```xml
<!-- 纯展示模式 -->
<atom:ListBox IsSelectable="False" />
```

当 `IsSelectable="False"` 时，键盘导航和鼠标点击都不会触发选择。

---

## 6. 尺寸切换

```xml
<atom:ListBox SizeType="Large" />
<atom:ListBox SizeType="Middle" />   <!-- 默认 -->
<atom:ListBox SizeType="Small" />
```

尺寸影响列表项的高度、内边距和圆角。

---

## 7. 无边框模式

在嵌入其他控件内部时，可去除边框：

```xml
<atom:ListBox IsBorderless="True" />
```

---

## 8. 空状态指示器

当列表无内容时自动显示空状态（默认开启）：

```xml
<atom:ListBox IsShowEmptyIndicator="True" />
```

自定义空状态内容：

```xml
<atom:ListBox>
    <atom:ListBox.EmptyIndicatorTemplate>
        <DataTemplate>
            <StackPanel HorizontalAlignment="Center" Spacing="8">
                <antdicons:InboxOutlined Width="40" Height="40" Opacity="0.3" />
                <atom:TextBlock Text="暂无数据" HorizontalAlignment="Center" />
            </StackPanel>
        </DataTemplate>
    </atom:ListBox.EmptyIndicatorTemplate>
</atom:ListBox>
```

---

## 9. 自定义 ItemTemplate

```xml
<atom:ListBox ItemsSource="{Binding Items}">
    <atom:ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Spacing="8">
                <antdicons:UserOutlined Width="16" Height="16" />
                <atom:TextBlock Text="{Binding Content}" />
            </StackPanel>
        </DataTemplate>
    </atom:ListBox.ItemTemplate>
</atom:ListBox>
```

---

## 10. 响应项目点击

```csharp
myListBox.ItemClicked += (sender, args) =>
{
    var clickedItem = args.Item;
    // 处理点击逻辑
};
```

---

## 11. 监听选择变化

```csharp
myListBox.SelectionChanged += (sender, args) =>
{
    var selectedItem = myListBox.SelectedItem;
    var selectedIndex = myListBox.SelectedIndex;
    // 处理选择变化
};
```

---

## 12. 自定义悬浮/选中背景色

```xml
<atom:ListBox ItemHoverBg="#E6F4FF"
              ItemSelectedBg="#BAE0FF" />
```

---

## 常见场景

### 作为下拉选项容器

ListBox 可作为 Popup 内的选项列表（多用于自定义下拉组件）：

```xml
<Popup>
    <atom:ListBox IsBorderless="True"
                  ItemsSource="{Binding Options}"
                  IsShowSelectedIndicator="True" />
</Popup>
```

### 带搜索的选项列表

```xml
<StackPanel>
    <atom:SearchEdit PlaceholderText="搜索..."
                     SearchButtonClick="OnSearch" />
    <atom:ListBox Name="MyList"
                  ItemsSource="{Binding AllItems}"
                  IsShowSelectedIndicator="True" />
</StackPanel>
```
