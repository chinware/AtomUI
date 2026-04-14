# DataGrid 选择用法

## 单行选择

```xml
<atom:DataGrid ItemsSource="{Binding Users}" SelectionMode="Single"
               SelectedItem="{Binding SelectedUser}"
               SelectedIndex="{Binding SelectedIndex}">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 多行选择（Checkbox）

```xml
<atom:DataGrid ItemsSource="{Binding Users}" SelectionMode="Extended"
               SelectedItems="{Binding SelectedUsers}">
    <atom:DataGrid.Columns>
        <atom:DataGridSelectionColumn />
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 监听选择变化

```csharp
dataGrid.SelectionChanged += (s, e) => {
    foreach (var item in e.AddedItems)
        Console.WriteLine($"选中: {item}");
    foreach (var item in e.RemovedItems)
        Console.WriteLine($"取消选中: {item}");
};
```

---

## 编程式选择

```csharp
// 选中指定项
dataGrid.SelectedItem = users[0];

// 全选
dataGrid.SelectAll();

// 取消全选
dataGrid.UnselectAll();
```

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Selection