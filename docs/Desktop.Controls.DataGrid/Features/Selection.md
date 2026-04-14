# DataGrid 行选择功能

## 概述

DataGrid 支持单行选择和扩展多行选择，可通过 `DataGridSelectionColumn` 添加 Checkbox 列实现可视化多选。

---

## 选择模式

### Single — 单行选择

点击即选中，不可多选：

```xml
<atom:DataGrid SelectionMode="Single" />
```

### Extended — 扩展选择

支持 Ctrl+点击多选、Shift+点击范围选：

```xml
<atom:DataGrid SelectionMode="Extended" />
```

---

## 选择列（Checkbox）

使用 `DataGridSelectionColumn` 在列头和每行添加 Checkbox：

```xml
<atom:DataGrid SelectionMode="Extended">
    <atom:DataGrid.Columns>
        <atom:DataGridSelectionColumn />
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

选择列宽度由 `SelectionColumnWidth` Token 控制，默认为 `SharedToken.ControlHeight`。

---

## 编程式选择

```csharp
// 选中指定项
dataGrid.SelectedItem = dataItem;

// 获取选中项
var selected = dataGrid.SelectedItem;
var selectedItems = dataGrid.SelectedItems;

// 全选/取消全选
dataGrid.SelectAll();
dataGrid.UnselectAll();
```

---

## 选择事件

| 事件 | 说明 |
|------|------|
| `SelectionChanged` | 选中项变化时触发 |

```csharp
dataGrid.SelectionChanged += (s, e) => {
    var added = e.AddedItems;
    var removed = e.RemovedItems;
};
```

---

## 选择列位置控制

`DataGridSelectionColumn` 可放在 `Columns` 集合的任意位置：

```xml
<atom:DataGrid.Columns>
    <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
    <atom:DataGridSelectionColumn />  <!-- 放在第二列 -->
    <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
</atom:DataGrid.Columns>
```

---

## 选中行视觉

| 状态 | 背景Token | 说明 |
|------|----------|------|
| 选中 | `RowSelectedBg` | 选中行背景色 |
| 选中+悬浮 | `RowSelectedHoverBg` | 选中行悬浮背景色 |

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Selection