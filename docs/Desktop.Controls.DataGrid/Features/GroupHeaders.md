# DataGrid 分组表头

## 概述

DataGrid 支持多级嵌套分组表头，通过 `ColumnGroups` 属性和 `DataGridColumnGroupItem` 构建。支持任意层级嵌套。

---

## 基础分组表头

```xml
<atom:DataGrid>
    <atom:DataGrid.ColumnGroups>
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
        <atom:DataGridColumnGroupItem Header="Other">
            <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
            <atom:DataGridTextColumn Header="Gender" Binding="{Binding Gender}" />
        </atom:DataGridColumnGroupItem>
    </atom:DataGrid.ColumnGroups>
</atom:DataGrid>
```

---

## 多级嵌套

`DataGridColumnGroupItem` 支持任意层级嵌套：

```xml
<atom:DataGrid.ColumnGroups>
    <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
    <atom:DataGridColumnGroupItem Header="Other">
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
        <atom:DataGridColumnGroupItem Header="Address">
            <atom:DataGridTextColumn Header="Street" Binding="{Binding Street}" />
            <atom:DataGridTextColumn Header="Building" Binding="{Binding Building}" />
        </atom:DataGridColumnGroupItem>
    </atom:DataGridColumnGroupItem>
    <atom:DataGridTextColumn Header="Gender" Binding="{Binding Gender}" />
</atom:DataGrid.ColumnGroups>
```

---

## 分组表头与固定列

分组表头可与固定列配合使用，固定列的分组表头也会被固定：

```xml
<atom:DataGrid LeftFrozenColumnCount="2">
    <atom:DataGrid.ColumnGroups>
        <atom:DataGridColumnGroupItem Header="Basic Info">
            <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
            <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
        </atom:DataGridColumnGroupItem>
        <atom:DataGridTextColumn Header="Address" Binding="{Binding Address}" />
    </atom:DataGrid.ColumnGroups>
</atom:DataGrid>
```

---

## 分组表头控件

分组表头由 `DataGridColumnGroupHeader` 控件渲染，其样式通过 `DataGridColumnGroupHeader` ControlTheme 控制。

---

## 注意事项

- 使用 `ColumnGroups` 时，不要同时使用 `Columns` 属性
- `DataGridColumnGroupItem` 中的叶子列必须是 `DataGridTextColumn`、`DataGridNumericColumn` 等数据列类型
- 分组表头不支持排序和过滤功能

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Grouping table head