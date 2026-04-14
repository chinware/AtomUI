# DataGrid 主控件 API 参考

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `Avalonia.Controls.Control`

---

## 属性

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

---

## 事件

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

---

## 方法

| 方法名 | 返回类型 | 说明 |
|--------|---------|------|
| `BeginEdit()` | `bool` | 进入编辑模式 |
| `CancelEdit()` | `void` | 取消编辑 |
| `CommitEdit()` | `void` | 提交编辑 |
| `ScrollIntoView(object item, DataGridColumn? column)` | `void` | 滚动到指定项和列 |
| `SelectAll()` | `void` | 全选 |
| `UnselectAll()` | `void` | 取消全选 |

---

## DataGridRow

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `Avalonia.Controls.Control`

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

## DataGridCell

**命名空间**: `AtomUI.Desktop.Controls`  
**基类**: `Avalonia.Controls.Control`

表示 DataGrid 中的单个单元格，通过 `DataGridColumn` 的列类型决定其内容和编辑行为。