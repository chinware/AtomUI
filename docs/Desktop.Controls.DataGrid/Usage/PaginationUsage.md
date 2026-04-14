# DataGrid 分页用法

## 基础分页

```xml
<atom:DataGrid ItemsSource="{Binding Users}" PageSize="10" PaginationVisibility="Bottom" Height="400">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 顶部+底部分页

```xml
<atom:DataGrid PageSize="10" PaginationVisibility="Top, Bottom" />
```

---

## 监听分页事件

```csharp
dataGrid.PageChanging += (s, e) => {
    // 可取消翻页
    e.Cancel = false;
};

dataGrid.PageChanged += (s, e) => {
    Console.WriteLine($"当前页: {e.NewPageIndex}");
};
```

---

## 编程式控制

```csharp
// 设置每页条数
dataGrid.PageSize = 20;

// 切换分页器位置
dataGrid.PaginationVisibility = DataGridPaginationVisibility.Bottom;
```

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Basic Paging