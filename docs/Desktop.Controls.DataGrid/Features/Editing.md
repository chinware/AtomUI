# DataGrid 单元格编辑功能

## 概述

DataGrid 支持单元格编辑，双击进入编辑模式。不同列类型提供不同的编辑控件。

---

## 编辑控制

| 属性 | 默认值 | 说明 |
|------|--------|------|
| `IsReadOnly` | `false` | 全局只读开关 |
| `CanUserSort` | `true` | 列级排序（影响编辑触发） |

---

## 列类型与编辑控件

| 列类型 | 显示控件 | 编辑控件 | 说明 |
|--------|---------|---------|------|
| `DataGridTextColumn` | `TextBlock` | `TextBox` | 文本编辑 |
| `DataGridNumericColumn` | `TextBlock` | `NumericUpDown` | 数值编辑 |
| `DataGridCheckBoxColumn` | `CheckBox` | `CheckBox` | 布尔切换 |
| `DataGridTemplateColumn` | `CellTemplate` | `CellEditingTemplate` | 自定义编辑 |

---

## 模板列编辑

通过 `CellTemplate` 和 `CellEditingTemplate` 完全自定义：

```xml
<atom:DataGridTemplateColumn Header="Custom">
    <atom:DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Value}" />
        </DataTemplate>
    </atom:DataGridTemplateColumn.CellTemplate>
    <atom:DataGridTemplateColumn.CellEditingTemplate>
        <DataTemplate>
            <TextBox Text="{Binding Value}" />
        </DataTemplate>
    </atom:DataGridTemplateColumn.CellEditingTemplate>
</atom:DataGridTemplateColumn>
```

---

## 编辑事件

| 事件 | 参数类型 | 说明 |
|------|---------|------|
| `BeginningEdit` | `DataGridBeginningEditEventArgs` | 编辑开始前，可取消 |
| `PreparingCellForEdit` | `DataGridPreparingCellForEditEventArgs` | 准备编辑控件 |
| `CellEditEnding` | `DataGridCellEditEndingEventArgs` | 编辑即将结束 |
| `CellEditEnded` | `DataGridCellEditEndedEventArgs` | 编辑结束 |
| `RowEditEnding` | `DataGridRowEditEndingEventArgs` | 行编辑即将结束 |
| `RowEditEnded` | `DataGridRowEditEndedEventArgs` | 行编辑结束 |

---

## 编程式编辑

```csharp
dataGrid.BeginEdit();   // 进入编辑
dataGrid.CommitEdit();  // 提交编辑
dataGrid.CancelEdit();  // 取消编辑
```

---

## 编辑触发方式

- **双击**单元格进入编辑模式
- **Tab/Enter** 键提交编辑并移动到下一单元格
- **Esc** 键取消编辑

---

## 只读模式

设置 `IsReadOnly="True"` 禁止所有编辑：

```xml
<atom:DataGrid IsReadOnly="True">
    <!-- ... -->
</atom:DataGrid>
```

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Editable Cells