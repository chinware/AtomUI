# DataGrid 排序与过滤用法

## 单列排序

```xml
<atom:DataGrid ItemsSource="{Binding Users}" CanUserSortColumns="True">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" CanUserSort="True" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" CanUserSort="True" />
        <atom:DataGridTextColumn Header="地址" Binding="{Binding Address}" CanUserSort="False" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

点击列头循环切换：无排序 → 升序 → 降序 → 无排序。

---

## 多列排序

按住 Shift 点击列头添加排序列：

```xml
<atom:DataGrid ItemsSource="{Binding Users}" CanUserSortColumns="True">
    <!-- Shift+点击第二列头实现多列排序 -->
</atom:DataGrid>
```

---

## 限制排序方向

```xml
<atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}"
    SupportedSortDirections="Ascending" />
```

---

## 列过滤

```xml
<atom:DataGrid ItemsSource="{Binding Users}">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}"
            CanUserFilter="True" FilterMultiple="True" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}"
            CanUserFilter="True" FilterMode="GreaterThan" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 排序+过滤组合

```xml
<atom:DataGrid ItemsSource="{Binding Users}" CanUserSortColumns="True">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}"
            CanUserSort="True" CanUserFilter="True" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}"
            CanUserSort="True" CanUserFilter="True" FilterMode="GreaterThan" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 自定义排序逻辑

```csharp
dataGrid.Sorting += (s, e) => {
    // 自定义排序
    e.Handled = true;
};
```

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Filter And Sorter / Multi Sorter