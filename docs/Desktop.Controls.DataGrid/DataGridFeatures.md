# DataGrid 功能详解

## 1. 基础用法

DataGrid 最基本的用法是通过 `ItemsSource` 绑定数据集合，并通过 `Columns` 定义列。

```xml
<atom:DataGrid ItemsSource="{Binding DataSource}">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
        <atom:DataGridTextColumn Header="Address" Binding="{Binding Address}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

当 `AutoGenerateColumns="True"`（默认值）时，DataGrid 会根据数据源类型自动生成列。设置 `AutoGenerateColumns="False"` 可禁用此行为。

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Basic Usage

---

## 2. 行选择

### 选择模式

通过 `SelectionMode` 属性控制行选择行为：

- **`Single`**：单行选择，点击即选中，不可多选。
- **`Extended`**：扩展选择，支持 Ctrl+点击多选、Shift+点击范围选。

```xml
<!-- 单选 -->
<atom:DataGrid SelectionMode="Single" />

<!-- 多选 -->
<atom:DataGrid SelectionMode="Extended" />
```

### 选择列（Checkbox）

使用 `DataGridSelectionColumn` 在列头和每行添加 Checkbox：

```xml
<atom:DataGrid SelectionMode="Extended">
    <atom:DataGrid.Columns>
        <atom:DataGridSelectionColumn />
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 编程式选择

```csharp
// 选中指定项
dataGrid.SelectedItem = dataItem;

// 全选/取消全选
dataGrid.SelectAll();
dataGrid.UnselectAll();

// 监听选择变化
dataGrid.SelectionChanged += (s, e) => {
    var selected = dataGrid.SelectedItems;
};
```

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Selection

---

## 3. 排序

### 基础排序

点击列头即可触发排序。通过 `CanUserSortColumns` 控制全局排序能力，通过列级 `CanUserSort` 控制单列排序能力。

```xml
<atom:DataGrid CanUserSortColumns="True">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" CanUserSort="True" />
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" CanUserSort="True" />
        <!-- 禁止排序的列 -->
        <atom:DataGridTextColumn Header="Address" Binding="{Binding Address}" CanUserSort="False" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 排序方向控制

通过 `SupportedSortDirections` 限制列支持的排序方向：

```xml
<atom:DataGridTextColumn Header="Name" Binding="{Binding Name}"
    SupportedSortDirections="Ascending" />
```

### 自定义排序

监听 `Sorting` 事件实现自定义排序逻辑：

```csharp
dataGrid.Sorting += (s, e) => {
    // 自定义排序逻辑
    e.Handled = true; // 标记已处理
};
```

### 排序提示

列级 `ShowSorterTooltip` 属性控制是否在悬浮时显示排序方向提示。

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Filter And Sorter / Multi Sorter

---

## 4. 过滤

### 内置过滤

DataGrid 内置列头过滤功能。通过 `CanUserFilter` 控制列是否可过滤：

```xml
<atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" CanUserFilter="True" />
```

### 过滤模式

- `FilterMode`：设置过滤模式
- `FilterMultiple`：是否支持多选过滤（默认 `true`）
- `FilterOnClose`：是否在关闭过滤面板时触发过滤

### 自定义过滤

监听 `Filtering` 事件实现自定义过滤逻辑：

```csharp
dataGrid.Filtering += (s, e) => {
    // 自定义过滤逻辑
};
```

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Filter And Sorter / Filter In Tree

---

## 5. 可展开行

使用 `DataGridDetailExpanderColumn` 和 `RowDetailsTemplate` 实现行展开/折叠功能：

```xml
<atom:DataGrid RowDetailsVisibilityMode="Collapsed">
    <atom:DataGrid.Columns>
        <atom:DataGridDetailExpanderColumn />
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
    </atom:DataGrid.Columns>
    <atom:DataGrid.RowDetailsTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
        </DataTemplate>
    </atom:DataGrid.RowDetailsTemplate>
</atom:DataGrid>
```

### 展开列/选择列顺序控制

`DataGridDetailExpanderColumn` 和 `DataGridSelectionColumn` 可以放在 `Columns` 集合的任意位置，控制其在表格中的显示顺序：

```xml
<atom:DataGrid.Columns>
    <atom:DataGridTemplateColumn Header="Name">
        <!-- ... -->
    </atom:DataGridTemplateColumn>
    <atom:DataGridDetailExpanderColumn />  <!-- 展开列放在第二列 -->
    <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
    <atom:DataGridSelectionColumn />       <!-- 选择列放在第四列 -->
</atom:DataGrid.Columns>
```

### 行详情可见性模式

| 模式 | 说明 |
|------|------|
| `Collapsed` | 默认折叠，点击展开按钮展开 |
| `Visible` | 默认展开 |
| `VisibleWhenSelected` | 选中行时自动展开 |

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Expandable Row / Order Specific Column

---

## 6. 固定列与固定表头

### 固定列

通过 `LeftFrozenColumnCount` 和 `RightFrozenColumnCount` 设置左右固定列数：

```xml
<atom:DataGrid LeftFrozenColumnCount="2" RightFrozenColumnCount="1">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />     <!-- 固定列 -->
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />       <!-- 固定列 -->
        <atom:DataGridTextColumn Header="Col1" Binding="{Binding Address}" />
        <!-- ... 更多可滚动列 ... -->
        <atom:DataGridTemplateColumn Header="Action">                          <!-- 右固定列 -->
            <!-- ... -->
        </atom:DataGridTemplateColumn>
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

固定列在水平滚动时保持不动，并显示阴影效果（由 `LeftFrozenShadows` / `RightFrozenShadows` Token 控制）。

### 固定表头

设置 DataGrid 的 `Height` 属性，当数据行超出高度时表头自动固定：

```xml
<atom:DataGrid Height="400">
    <atom:DataGrid.Columns>
        <!-- ... -->
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 固定列+固定表头

可同时使用固定列和固定表头：

```xml
<atom:DataGrid LeftFrozenColumnCount="2" RightFrozenColumnCount="1"
               HeadersVisibility="All" Height="400">
    <!-- ... -->
</atom:DataGrid>
```

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Fixed Header / Fixed Columns / Fixed Columns And Headers

---

## 7. 列拖拽排序

通过 `CanUserReorderColumns` 启用列拖拽排序，列级 `CanUserReorder` 控制单列是否可拖拽：

```xml
<atom:DataGrid CanUserReorderColumns="True">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn CanUserReorder="True" Header="Name" Binding="{Binding Name}" />
        <atom:DataGridTextColumn CanUserReorder="True" Header="Age" Binding="{Binding Age}" />
        <atom:DataGridTextColumn Header="Action" CanUserReorder="False" Binding="{Binding Address}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 相关事件

- `ColumnReordering`：列拖拽开始
- `ColumnDraggingOver`：列拖拽经过目标
- `ColumnReordered`：列拖拽完成

### 与固定列配合

列拖拽排序可与固定列配合使用，固定列区域内的列只能在固定区域内重排：

```xml
<atom:DataGrid CanUserReorderColumns="True"
               LeftFrozenColumnCount="2" RightFrozenColumnCount="2">
    <!-- ... -->
</atom:DataGrid>
```

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Drag Column sorting

---

## 8. 行拖拽排序

通过 `CanUserReorderRows` 启用行拖拽排序，配合 `DataGridRowReorderColumn` 显示拖拽手柄：

```xml
<atom:DataGrid CanUserReorderRows="True">
    <atom:DataGrid.Columns>
        <atom:DataGridRowReorderColumn />
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 相关事件

- `RowReordering`：行拖拽开始
- `RowReordered`：行拖拽完成

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Drag sorting with handler

---

## 9. 分组表头

使用 `ColumnGroups` 属性和 `DataGridColumnGroupItem` 构建多级嵌套表头：

```xml
<atom:DataGrid>
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
</atom:DataGrid>
```

`DataGridColumnGroupItem` 支持任意层级嵌套，每个分组项可以包含叶子列和子分组项。

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Grouping table head

---

## 10. 分页

DataGrid 内置分页功能，通过 `PaginationVisibility` 和 `PageSize` 配置：

```xml
<atom:DataGrid PaginationVisibility="Bottom" PageSize="10" Height="400">
    <atom:DataGrid.Columns>
        <!-- ... -->
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 分页器位置

`PaginationVisibility` 为标志枚举，可组合使用：

```csharp
// 仅底部
dataGrid.PaginationVisibility = DataGridPaginationVisibility.Bottom;

// 顶部和底部
dataGrid.PaginationVisibility = DataGridPaginationVisibility.Top | DataGridPaginationVisibility.Bottom;
```

### 分页事件

- `PageChanging`：分页索引变化前触发，可取消
- `PageChanged`：分页索引变化后触发

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Basic Paging

---

## 11. 单元格编辑

### 编辑模式

设置 `IsReadOnly="False"`（默认）允许编辑。双击单元格进入编辑模式。

### 列类型与编辑器

| 列类型 | 显示控件 | 编辑控件 |
|--------|---------|---------|
| `DataGridTextColumn` | `TextBlock` | `TextBox` |
| `DataGridNumericColumn` | `TextBlock` | `NumericUpDown` |
| `DataGridCheckBoxColumn` | `CheckBox` | `CheckBox` |
| `DataGridTemplateColumn` | `CellTemplate` | `CellEditingTemplate` |

### 模板列编辑

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

### 编辑事件

- `BeginningEdit`：编辑开始前
- `PreparingCellForEdit`：准备编辑控件
- `CellEditEnding`：编辑即将结束
- `CellEditEnded`：编辑结束
- `RowEditEnding`：行编辑即将结束
- `RowEditEnded`：行编辑结束

### 编程式编辑

```csharp
dataGrid.BeginEdit();   // 进入编辑
dataGrid.CommitEdit();  // 提交编辑
dataGrid.CancelEdit();  // 取消编辑
```

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Editable Cells

---

## 12. 列隐藏

通过列的 `IsVisible` 属性动态控制列的显示/隐藏：

```xml
<atom:DataGrid x:Name="grid">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn x:Name="col1" Header="Column 1" Binding="{Binding Name}" />
        <atom:DataGridTextColumn x:Name="col2" Header="Column 2" Binding="{Binding Address}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

```csharp
col1.IsVisible = checkBox1.IsChecked ?? true;
col2.IsVisible = checkBox2.IsChecked ?? true;
```

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Hidden Columns

---

## 13. 自定义空状态与加载

### 空数据状态

通过 `EmptyContentTemplate` 自定义无数据时的显示内容：

```xml
<atom:DataGrid x:Name="grid">
    <atom:DataGrid.EmptyContentTemplate>
        <DataTemplate>
            <StackPanel Orientation="Vertical" Spacing="10" HorizontalAlignment="Center">
                <atom:Icon Icon="{x:Static atom:AntDesignIconPackage.InboxOutlined}" />
                <TextBlock Text="No Data" />
            </StackPanel>
        </DataTemplate>
    </atom:DataGrid.EmptyContentTemplate>
</atom:DataGrid>
```

### 加载状态

通过 `IsOperating` 属性切换加载状态：

```csharp
grid.IsOperating = true;  // 显示加载动画
grid.IsOperating = false; // 隐藏加载动画
```

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Custom empty and loading

---

## 14. 尺寸变体

通过 `SizeType` 属性切换表格尺寸，影响单元格内边距和字体大小：

```xml
<!-- 大尺寸（默认） -->
<atom:DataGrid SizeType="Large" />

<!-- 中等尺寸 -->
<atom:DataGrid SizeType="Middle" />

<!-- 小尺寸 -->
<atom:DataGrid SizeType="Small" />
```

| 尺寸 | CellPadding | CellFontSize |
|------|-------------|-------------|
| Large | `SharedToken.Padding` | `SharedToken.FontSize` |
| Middle | `SharedToken.PaddingSM` | `SharedToken.FontSize` |
| Small | `SharedToken.PaddingXS` | `SharedToken.FontSize` |

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Size / Middle Size / Small Size

---

## 15. 自定义表头与表尾

通过 `HeaderTemplate` 和 `FooterTemplate` 自定义表格的表头和表尾区域：

```xml
<atom:DataGrid>
    <atom:DataGrid.HeaderTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Spacing="10">
                <TextBlock Text="Custom Header" FontWeight="Bold" />
            </StackPanel>
        </DataTemplate>
    </atom:DataGrid.HeaderTemplate>
    <atom:DataGrid.FooterTemplate>
        <DataTemplate>
            <TextBlock Text="Custom Footer" />
        </DataTemplate>
    </atom:DataGrid.FooterTemplate>
</atom:DataGrid>
```

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Custom Header And Footer

---

## 16. 行头

通过 `HeadersVisibility` 控制行头显示，配合 `RowHeaderContentTemplate` 自定义行头内容：

```xml
<atom:DataGrid HeadersVisibility="All">
    <atom:DataGrid.RowHeaderContentTemplate>
        <DataTemplate>
            <atom:TextBlock Text="{Binding $parent[atom:DataGridRow].LogicIndex}" />
        </DataTemplate>
    </atom:DataGrid.RowHeaderContentTemplate>
</atom:DataGrid>
```

> 📖 Gallery 参考：`DataGridShowCase.axaml` - Row Header

---

## 17. 列宽调整

通过 `CanUserResizeColumns` 全局控制列宽调整，列级 `CanUserResize` 控制单列：

```xml
<atom:DataGrid CanUserResizeColumns="True">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" CanUserResize="True" />
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" CanUserResize="False" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 列宽类型

```xml
<!-- 自动宽度 -->
<atom:DataGridTextColumn Width="Auto" />

<!-- 固定像素宽度 -->
<atom:DataGridTextColumn Width="100" />

<!-- 按比例分配 -->
<atom:DataGridTextColumn Width="2*" />
```

---

## 18. 自动生成列

当 `AutoGenerateColumns="True"` 时，DataGrid 会根据 `ItemsSource` 中数据项的公共属性自动生成 `DataGridTextColumn`。

可通过 `AutoGeneratingColumn` 事件自定义或取消自动生成的列：

```csharp
dataGrid.AutoGeneratingColumn += (s, e) => {
    // 修改列标题
    e.Column.Header = e.PropertyName switch {
        "Name" => "姓名",
        "Age" => "年龄",
        _ => e.PropertyName
    };
    
    // 取消某列的自动生成
    if (e.PropertyName == "Id")
        e.Cancel = true;
};