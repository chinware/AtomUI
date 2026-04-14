# DataGrid 拖拽功能

## 概述

DataGrid 支持列拖拽排序和行拖拽排序两种拖拽交互，均通过原生实现，无需第三方库。

---

## 列拖拽排序

### 启用列拖拽

通过 `CanUserReorderColumns` 全局启用，列级 `CanUserReorder` 控制单列：

```xml
<atom:DataGrid CanUserReorderColumns="True">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn CanUserReorder="True" Header="Name" Binding="{Binding Name}" />
        <atom:DataGridTextColumn CanUserReorder="True" Header="Age" Binding="{Binding Age}" />
        <atom:DataGridTextColumn Header="Action" CanUserReorder="False" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 列拖拽事件

| 事件 | 参数类型 | 说明 |
|------|---------|------|
| `ColumnReordering` | `DataGridColumnReorderingEventArgs` | 列拖拽开始 |
| `ColumnDraggingOver` | `DataGridColumnDraggingOverEventArgs` | 列拖拽经过目标 |
| `ColumnReordered` | `DataGridColumnEventArgs` | 列拖拽完成 |
| `ColumnDisplayIndexChanged` | `DataGridColumnEventArgs` | 列显示索引变化 |

### 列拖拽与固定列配合

固定列区域内的列只能在固定区域内重排，不会跨区域拖拽：

```xml
<atom:DataGrid CanUserReorderColumns="True"
               LeftFrozenColumnCount="2" RightFrozenColumnCount="2">
    <!-- ... -->
</atom:DataGrid>
```

### 列拖拽视觉

拖拽时当前列背景色由 `ColumnReorderActiveBg` Token 控制。

---

## 行拖拽排序

### 启用行拖拽

通过 `CanUserReorderRows` 启用，配合 `DataGridRowReorderColumn` 显示拖拽手柄：

```xml
<atom:DataGrid CanUserReorderRows="True">
    <atom:DataGrid.Columns>
        <atom:DataGridRowReorderColumn />
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 行拖拽事件

| 事件 | 参数类型 | 说明 |
|------|---------|------|
| `RowReordering` | `DataGridRowReorderingEventArgs` | 行拖拽开始 |
| `RowReordered` | `DataGridRowEventArgs` | 行拖拽完成 |

### 行拖拽指示器

拖拽手柄图标尺寸由 `RowReorderIndicatorSize` Token 控制，默认为 `SharedToken.SizeMD`。

---

## 注意事项

- 列拖拽和行拖拽可同时启用
- 行拖拽需要数据源支持排序变更（如 `ObservableCollection`）
- 行拖拽与分页功能配合时，仅能在当前页内拖拽

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Drag Column sorting / Drag sorting with handler