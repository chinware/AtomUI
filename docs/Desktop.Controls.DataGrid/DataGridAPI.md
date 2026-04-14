# DataGrid API 参考

## 1. DataGrid 主控件

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `Avalonia.Controls.Control`

### 属性

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `ItemsSource` | `IEnumerable?` | `null` | 数据源集合 |
| `Columns` | `ObservableCollection<DataGridColumn>` | 空集合 | 列集合 |
| `ColumnGroups` | `ObservableCollection<object>` | 空集合 | 分组表头列集合（支持嵌套 `DataGridColumnGroupItem`） |
| `AutoGenerateColumns` | `bool` | `true` | 是否根据数据源自动生成列 |
| `IsReadOnly` | `bool` | `false` | 是否只读（禁止编辑） |
| `SelectionMode` | `DataGridSelectionMode` | `Extended` | 行选择模式 |
| `SelectedIndex` | `int` | `-1` | 选中行索引 |
| `SelectedItem` | `object?` | `null` | 选中数据项 |
| `SelectedItems` | `IList?` | `null` | 多选时的选中项集合 |
| `ColumnWidth` | `DataGridLength` | `Auto` | 默认列宽 |
| `MinColumnWidth` | `double` | `0` | 最小列宽 |
| `MaxColumnWidth` | `double` | `PositiveInfinity` | 最大列宽 |
| `ColumnHeaderHeight` | `double` | `NaN` | 列头高度 |
| `RowHeight` | `double` | `NaN` | 行高（NaN 表示自动） |
| `MinRowHeight` | `double` | `0` | 最小行高 |
| `MaxRowHeight` | `double` | `PositiveInfinity` | 最大行高 |
| `RowHeaderWidth` | `double` | `NaN` | 行头宽度 |
| `RowDetailsTemplate` | `IDataTemplate?` | `null` | 行详情模板 |
| `RowDetailsVisibilityMode` | `DataGridRowDetailsVisibilityMode` | `Collapsed` | 行详情可见性模式 |
| `RowHeaderContentTemplate` | `IDataTemplate?` | `null` | 行头内容模板 |
| `HeadersVisibility` | `DataGridHeadersVisibility` | `Column` | 表头可见性 |
| `GridLinesVisibility` | `DataGridGridLinesVisibility` | `None` | 网格线可见性 |
| `HorizontalScrollBarVisibility` | `ScrollBarVisibility` | `Auto` | 水平滚动条可见性 |
| `VerticalScrollBarVisibility` | `ScrollBarVisibility` | `Auto` | 垂直滚动条可见性 |
| `CanUserReorderColumns` | `bool` | `false` | 是否允许用户拖拽重排列顺序 |
| `CanUserResizeColumns` | `bool` | `true` | 是否允许用户调整列宽 |
| `CanUserSortColumns` | `bool` | `true` | 是否允许用户点击列头排序 |
| `CanUserReorderRows` | `bool` | `false` | 是否允许用户拖拽行排序 |
| `LeftFrozenColumnCount` | `int` | `0` | 左侧固定列数 |
| `RightFrozenColumnCount` | `int` | `0` | 右侧固定列数 |
| `IsRowGroupHeadersFrozen` | `bool` | `true` | 分组行头是否固定 |
| `SizeType` | `SizeType` | `Large` | 表格尺寸（Large/Middle/Small） |
| `PaginationVisibility` | `DataGridPaginationVisibility` | `None` | 分页器显示位置 |
| `PageSize` | `int` | `10` | 每页数据条数 |
| `IsOperating` | `bool` | `false` | 是否显示加载状态 |
| `EmptyContentTemplate` | `IDataTemplate?` | `null` | 空数据时显示的内容模板 |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 表头区域模板 |
| `FooterTemplate` | `IDataTemplate?` | `null` | 表尾区域模板 |

### 事件

| 事件名 | 事件参数类型 | 说明 |
|--------|-------------|------|
| `AutoGeneratingColumn` | `DataGridAutoGeneratingColumnEventArgs` | 自动生成列时触发，可取消或修改列 |
| `BeginningEdit` | `DataGridBeginningEditEventArgs` | 单元格进入编辑模式前触发 |
| `CellEditEnded` | `DataGridCellEditEndedEventArgs` | 单元格编辑结束后触发 |
| `CellEditEnding` | `DataGridCellEditEndingEventArgs` | 单元格编辑即将结束时触发 |
| `CellPointerPressed` | `DataGridCellPointerPressedEventArgs` | 单元格被鼠标按下时触发 |
| `ColumnDisplayIndexChanged` | `DataGridColumnEventArgs` | 列显示索引变化时触发 |
| `ColumnReordered` | `DataGridColumnEventArgs` | 列拖拽排序完成时触发 |
| `ColumnReordering` | `DataGridColumnReorderingEventArgs` | 列拖拽排序开始时触发 |
| `ColumnDraggingOver` | `DataGridColumnDraggingOverEventArgs` | 列拖拽经过目标列时触发 |
| `RowReordered` | `DataGridRowEventArgs` | 行拖拽排序完成时触发 |
| `RowReordering` | `DataGridRowReorderingEventArgs` | 行拖拽排序开始时触发 |
| `CurrentCellChanged` | `EventArgs` | 当前单元格变化时触发 |
| `LoadingRow` | `DataGridRowEventArgs` | 行实例化后触发，可自定义行 |
| `UnloadingRow` | `DataGridRowEventArgs` | 行可被回收时触发 |
| `PreparingCellForEdit` | `DataGridPreparingCellForEditEventArgs` | 模板列进入编辑模式时触发 |
| `RowEditEnded` | `DataGridRowEditEndedEventArgs` | 行编辑结束后触发 |
| `RowEditEnding` | `DataGridRowEditEndingEventArgs` | 行编辑即将结束时触发 |
| `SelectionChanged` | `SelectionChangedEventArgs` | 选中项变化时触发 |
| `Sorting` | `DataGridColumnEventArgs` | 列排序请求时触发 |
| `Filtering` | `DataGridColumnEventArgs` | 列过滤请求时触发 |
| `LoadingRowDetails` | `DataGridRowDetailsEventArgs` | 行详情模板应用时触发 |
| `RowDetailsVisibilityChanged` | `DataGridRowDetailsEventArgs` | 行详情可见性变化时触发 |
| `UnloadingRowDetails` | `DataGridRowDetailsEventArgs` | 行详情可被回收时触发 |
| `LoadingRowGroup` | `DataGridRowGroupHeaderEventArgs` | 分组行头加载时触发 |
| `UnloadingRowGroup` | `DataGridRowGroupHeaderEventArgs` | 分组行头卸载时触发 |
| `CopyingRowClipboardContent` | `DataGridRowClipboardEventArgs` | 复制行内容到剪贴板时触发 |
| `PageChanged` | `PageChangedEventArgs` | 分页索引变化完成时触发 |
| `PageChanging` | `PageChangingEventArgs` | 分页索引变化请求时触发 |
| `HorizontalScroll` | `ScrollEventArgs` | 水平滚动时触发 |
| `VerticalScroll` | `ScrollEventArgs` | 垂直滚动时触发 |

### 方法

| 方法名 | 返回类型 | 说明 |
|--------|---------|------|
| `BeginEdit()` | `bool` | 进入编辑模式 |
| `CancelEdit()` | `void` | 取消编辑 |
| `CommitEdit()` | `void` | 提交编辑 |
| `ScrollIntoView(object item, DataGridColumn? column)` | `void` | 滚动到指定项和列 |
| `SelectAll()` | `void` | 全选 |
| `UnselectAll()` | `void` | 取消全选 |

---

## 2. DataGridColumn（抽象基类）

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `AvaloniaObject`

所有列类型的抽象基类，定义列的通用属性。

### 属性

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `Header` | `object?` | `null` | 列头内容 |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 列头内容模板 |
| `CellTheme` | `ControlTheme?` | `null` | 单元格主题 |
| `Width` | `DataGridLength` | `Auto` | 列宽 |
| `MinWidth` | `double` | `0` | 最小列宽 |
| `MaxWidth` | `double` | `PositiveInfinity` | 最大列宽 |
| `ActualWidth` | `double` | (只读) | 实际列宽 |
| `IsVisible` | `bool` | `true` | 是否可见 |
| `DisplayIndex` | `int` | `-1` | 列显示顺序索引 |
| `CanUserSort` | `bool` | `true` | 是否允许用户排序此列 |
| `CanUserReorder` | `bool` | `true` | 是否允许用户拖拽此列 |
| `CanUserResize` | `bool` | `true` | 是否允许用户调整此列宽 |
| `CanUserFilter` | `bool` | `true` | 是否允许用户过滤此列 |
| `ShowSorterTooltip` | `bool` | `true` | 是否显示排序提示 |
| `SupportedSortDirections` | `DataGridSortDirections` | `All` | 支持的排序方向 |
| `HeaderContentHorizontalAlignment` | `HorizontalAlignment` | `Left` | 列头内容水平对齐 |
| `HeaderContentVerticalAlignment` | `VerticalAlignment` | `Center` | 列头内容垂直对齐 |
| `FilterMode` | `DataGridFilterMode` | - | 过滤模式 |
| `FilterMultiple` | `bool` | `true` | 是否支持多选过滤 |
| `FilterOnClose` | `bool` | `false` | 是否在关闭过滤面板时触发过滤 |
| `IsAutoGenerated` | `bool` | (只读) | 是否为自动生成的列 |
| `IsFrozen` | `bool` | (只读) | 是否为固定列 |
| `IsLeftFrozen` | `bool` | (只读) | 是否为左侧固定列 |
| `IsRightFrozen` | `bool` | (只读) | 是否为右侧固定列 |

---

## 3. DataGridBoundColumn（抽象基类）

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `DataGridColumn`

支持数据绑定的列基类。

### 属性

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `Binding` | `IBinding?` | `null` | 数据绑定表达式 |
| `StringFormat` | `string?` | `null` | 字符串格式化 |

---

## 4. DataGridTextColumn

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `DataGridBoundColumn`

文本列，以 `TextBlock` 显示数据，编辑时使用 `TextBox`。

### 属性

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| 继承自 `DataGridBoundColumn` | | | `Binding`, `StringFormat` |
| `FormatString` | `string?` | `null` | 格式化字符串（如 `"0"` 用于整数） |

### 示例

```xml
<atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
<atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" FormatString="0" />
```

---

## 5. DataGridNumericColumn

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `DataGridBoundColumn`

数值列，以 `TextBlock` 显示数据，编辑时使用 `NumericUpDown`。

### 属性

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| 继承自 `DataGridBoundColumn` | | | `Binding`, `StringFormat` |
| `FormatString` | `string?` | `null` | 数值格式化字符串 |

### 示例

```xml
<atom:DataGridNumericColumn Header="Age" Binding="{Binding Age}" FormatString="0" />
```

---

## 6. DataGridCheckBoxColumn

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `DataGridBoundColumn`

复选框列，以 `CheckBox` 显示和编辑布尔值数据。

### 属性

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| 继承自 `DataGridBoundColumn` | | | `Binding` |

---

## 7. DataGridTemplateColumn

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `DataGridColumn`

模板列，通过 `DataTemplate` 完全自定义单元格的显示和编辑方式。

### 属性

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `CellTemplate` | `IDataTemplate?` | `null` | 单元格显示模板 |
| `CellEditingTemplate` | `IDataTemplate?` | `null` | 单元格编辑模板 |

### 示例

```xml
<atom:DataGridTemplateColumn Header="Action">
    <atom:DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Spacing="15">
                <atom:HyperLinkTextBlock Text="Invite" />
                <atom:HyperLinkTextBlock Text="Delete" />
            </StackPanel>
        </DataTemplate>
    </atom:DataGridTemplateColumn.CellTemplate>
</atom:DataGridTemplateColumn>
```

---

## 8. DataGridSelectionColumn

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `DataGridColumn`

行选择列，在每行显示 `CheckBox` 用于行选择。配合 `SelectionMode="Extended"` 实现多选。

### 示例

```xml
<atom:DataGrid SelectionMode="Extended">
    <atom:DataGrid.Columns>
        <atom:DataGridSelectionColumn />
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 9. DataGridDetailExpanderColumn

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `DataGridColumn`

行展开/折叠控制列，显示展开/折叠图标按钮。配合 `RowDetailsTemplate` 使用。

### 示例

```xml
<atom:DataGrid RowDetailsVisibilityMode="Collapsed">
    <atom:DataGrid.Columns>
        <atom:DataGridDetailExpanderColumn />
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
    </atom:DataGrid.Columns>
    <atom:DataGrid.RowDetailsTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
        </DataTemplate>
    </atom:DataGrid.RowDetailsTemplate>
</atom:DataGrid>
```

---

## 10. DataGridRowReorderColumn

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `DataGridColumn`

行拖拽排序手柄列，显示拖拽图标。配合 `CanUserReorderRows="True"` 使用。

### 示例

```xml
<atom:DataGrid CanUserReorderRows="True">
    <atom:DataGrid.Columns>
        <atom:DataGridRowReorderColumn />
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 11. DataGridColumnGroupItem

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `DataGridColumn`

列分组项，用于构建多级嵌套表头。可包含子列或嵌套的 `DataGridColumnGroupItem`。

### 属性

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| 继承自 `DataGridColumn` | | | `Header` 等 |
| `Columns` | 集合 | 空集合 | 子列集合 |

### 示例

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
</atom:DataGrid.ColumnGroups>
```

---

## 12. DataGridRow

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `Avalonia.Controls.Control`

表示 DataGrid 中的数据行。

### 属性

| 属性名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `Header` | `object?` | `null` | 行头内容 |
| `HeaderContentTemplate` | `IDataTemplate?` | `null` | 行头内容模板 |
| `IsSelected` | `bool` | `false` | 是否选中 |
| `IsValid` | `bool` | `true` | 数据验证是否通过 |
| `DetailsTemplate` | `IDataTemplate?` | `null` | 行详情模板 |
| `IsDetailsVisible` | `bool` | `false` | 行详情是否可见 |
| `Index` | `int` | (只读) | 行索引 |
| `LogicIndex` | `int` | (只读) | 逻辑行索引（排除分组行） |

---

## 13. DataGridCell

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `Avalonia.Controls.Control`

表示 DataGrid 中的单个单元格。

---

## 14. 枚举类型

### DataGridSelectionMode

| 值 | 说明 |
|----|------|
| `Single` | 单行选择 |
| `Extended` | 扩展选择（支持多选，配合 Ctrl/Shift） |

### DataGridHeadersVisibility

| 值 | 说明 |
|----|------|
| `None` | 不显示表头 |
| `Row` | 仅显示行头 |
| `Column` | 仅显示列头 |
| `All` | 显示行头和列头 |

### DataGridGridLinesVisibility

| 值 | 说明 |
|----|------|
| `None` | 不显示网格线 |
| `Horizontal` | 仅显示水平网格线 |
| `Vertical` | 仅显示垂直网格线 |
| `All` | 显示所有网格线 |

### DataGridRowDetailsVisibilityMode

| 值 | 说明 |
|----|------|
| `Collapsed` | 行详情默认折叠 |
| `Visible` | 行详情默认可见 |
| `VisibleWhenSelected` | 选中时显示行详情 |

### DataGridSortDirections

| 值 | 说明 |
|----|------|
| `None` | 不支持排序 |
| `Ascending` | 升序 |
| `Descending` | 降序 |
| `All` | 升序和降序 |

### DataGridFilterMode

| 值 | 说明 |
|----|------|
| (具体值待确认) | 过滤模式枚举 |

### DataGridPaginationVisibility

| 值 | 说明 |
|----|------|
| `None` | 不显示分页器 |
| `Top` | 在表格顶部显示分页器 |
| `Bottom` | 在表格底部显示分页器 |

> 注意：`PaginationVisibility` 为标志枚举（Flags），可组合使用，如 `Top | Bottom`。

### DataGridLength

| 类型 | 说明 |
|------|------|
| `Auto` | 自动宽度 |
| `SizeToCells` | 根据单元格内容调整宽度 |
| `SizeToHeader` | 根据列头内容调整宽度 |
| `Pixel(double)` | 固定像素宽度 |
| `Star(double)` | 按比例分配剩余空间 |

### SizeType

| 值 | 说明 |
|----|------|
| `Large` | 大尺寸（默认） |
| `Middle` | 中等尺寸 |
| `Small` | 小尺寸 |