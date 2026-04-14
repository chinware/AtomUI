# DataGrid 基础用法

## 最简表格

```xml
<atom:DataGrid ItemsSource="{Binding Users}" AutoGenerateColumns="True" />
```

`AutoGenerateColumns="True"` 时，DataGrid 根据数据源属性自动生成 `DataGridTextColumn`。

---

## 手动定义列

```xml
<atom:DataGrid ItemsSource="{Binding Users}" AutoGenerateColumns="False">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" FormatString="0" />
        <atom:DataGridTextColumn Header="地址" Binding="{Binding Address}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 不同尺寸

通过 `SizeType` 切换表格尺寸：

```xml
<!-- 大尺寸（默认） -->
<atom:DataGrid SizeType="Large" />

<!-- 中等尺寸 -->
<atom:DataGrid SizeType="Middle" />

<!-- 小尺寸 -->
<atom:DataGrid SizeType="Small" />
```

---

## 带边框

```xml
<atom:DataGrid GridLinesVisibility="All" />
```

---

## 空数据

```xml
<atom:DataGrid ItemsSource="{Binding EmptyList}">
    <atom:DataGrid.EmptyContentTemplate>
        <DataTemplate>
            <atom:Empty Description="暂无数据" />
        </DataTemplate>
    </atom:DataGrid.EmptyContentTemplate>
</atom:DataGrid>
```

---

## 加载状态

```xml
<atom:DataGrid IsOperating="True" />
```

---

## 表头和表尾

```xml
<atom:DataGrid>
    <atom:DataGrid.HeaderTemplate>
        <DataTemplate>
            <TextBlock Text="用户列表" FontWeight="Bold" />
        </DataTemplate>
    </atom:DataGrid.HeaderTemplate>
    <atom:DataGrid.FooterTemplate>
        <DataTemplate>
            <TextBlock Text="共 100 条数据" />
        </DataTemplate>
    </atom:DataGrid.FooterTemplate>
</atom:DataGrid>
```

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml`