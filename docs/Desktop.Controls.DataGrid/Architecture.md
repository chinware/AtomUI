# DataGrid 架构深度分析

## 1. 整体架构概览

DataGrid 是 AtomUI 中最复杂的控件之一，其源码分布在 `src/AtomUI.Desktop.Controls.DataGrid/` 目录下，包含超过 30 个核心类型和约 15000+ 行代码。控件采用**分部类（partial class）**组织方式，将不同职责的逻辑拆分到独立文件中。

### 1.1 源码文件组织

| 文件 | 职责 | 行数级别 |
|------|------|----------|
| `DataGrid.cs` | 主控件定义、公共属性/事件、模板化控件逻辑 | ~1200 |
| `DataGrid.Privates.cs` | 内部属性、滚动计算、选择逻辑、剪贴板、分页配置 | ~4770 |
| `DataGrid.Rows.cs` | 行管理、行虚拟化、行详情、行分组可见性 | ~3437 |
| `DataGrid.Columns.cs` | 列管理、列自动生成、列分组构建、固定列处理 | ~2379 |
| `DataGridDisplayData.cs` | 显示数据管理、行回收池、循环列表 | ~326 |
| `Data/DataGridDataConnection.cs` | 数据源连接、数据属性反射、CollectionView 交互 | ~708 |
| `Data/DataGridCollectionView.cs` | 自定义 CollectionView（排序/过滤/分页/分组） | ~2500+ |
| `Row/DataGridRow.cs` | 数据行控件 | ~1500+ |
| `Column/DataGridColumn.cs` | 列基类（抽象） | ~1200+ |
| `Themes/DataGrid.axaml` | 默认主题样式 | ~800+ |

### 1.2 架构分层

```
┌─────────────────────────────────────────────────────┐
│                   用户交互层                          │
│  (Pointer/Keyboard 事件 → 排序/过滤/选择/编辑/拖拽)    │
├─────────────────────────────────────────────────────┤
│                   控件逻辑层                          │
│  DataGrid 主控件 + 分部类                             │
│  ├── 属性管理 (DataGrid.cs)                          │
│  ├── 行管理 (DataGrid.Rows.cs)                       │
│  ├── 列管理 (DataGrid.Columns.cs)                    │
│  └── 内部逻辑 (DataGrid.Privates.cs)                 │
├─────────────────────────────────────────────────────┤
│                   数据管理层                          │
│  DataGridDataConnection ←→ DataGridCollectionView    │
│  (排序/过滤/分页/分组)                                │
├─────────────────────────────────────────────────────┤
│                   虚拟化层                           │
│  DataGridDisplayData (行回收池 + 循环列表)            │
│  + 列虚拟化 (ShouldDisplayCell)                       │
├─────────────────────────────────────────────────────┤
│                   视觉元素层                          │
│  DataGridRow / DataGridCell / DataGridColumnHeader   │
│  DataGridRowGroupHeader / DataGridColumnGroupHeader  │
├─────────────────────────────────────────────────────┤
│                   主题 Token 层                       │
│  DataGridToken + ControlTheme                        │
└─────────────────────────────────────────────────────┘
```

---

## 2. 数据管理层

### 2.1 DataGridDataConnection

`DataGridDataConnection` 是 DataGrid 与数据源之间的桥梁，负责：

- **数据源绑定**：管理 `ItemsSource` → `CollectionView` 的转换
- **数据属性反射**：通过 `DataProperties` 缓存数据类型的 `PropertyInfo[]`，用于自动生成列
- **数据类型推断**：通过 `GetItemType()` 推断 `IEnumerable<T>` 的泛型参数 `T`
- **CollectionView 交互**：监听 CollectionView 的 `CollectionChanged`、`CurrentChanged` 事件

```
ItemsSource (IEnumerable)
    │
    ▼
DataGridDataConnection
    │
    ├── DataSource (IEnumerable) ──→ 包装为 DataGridCollectionView
    ├── DataType (Type) ──→ 反射推断
    ├── DataProperties (PropertyInfo[]) ──→ 缓存反射结果
    └── CollectionView (IDataGridCollectionView) ──→ 排序/过滤/分页/分组
```

### 2.2 DataGridCollectionView

`DataGridCollectionView` 是 AtomUI 自定义的 CollectionView 实现，**不依赖** Avalonia 内置的 `ICollectionView`，而是完全独立实现 `IDataGridCollectionView` 接口。

#### 核心能力

| 能力 | 实现方式 | 关键类型 |
|------|----------|----------|
| **排序** | `DataGridSortDescriptionCollection` + `IComparer` | `DataGridSortDescription` |
| **过滤** | `DataGridFilterDescriptionCollection` + `Func<object, bool>` | `DataGridFilterDescription` |
| **分页** | `PageSize` / `PageIndex` / `MoveToPage()` | 内部分页索引计算 |
| **分组** | `DataGridCollectionViewGroup` 树形结构 | `DataGridGroupDescription` |
| **编辑** | `AddNew()` / `CommitNew()` / `CancelNew()` | `IDataGridEditableCollectionView` |

#### 分页实现原理

分页是 `DataGridCollectionView` 的核心特性之一：

```
InternalList (全部数据)
    │
    ├── PageSize = 0 → 不分页，显示全部数据
    │
    └── PageSize > 0 → 分页模式
         ├── PageIndex → 当前页索引（从 0 开始）
         ├── PageCount → 总页数 = ⌈ItemCount / PageSize⌉
         ├── Count → 当前页数据量 = min(PageSize, ItemCount - PageIndex * PageSize)
         └── MoveToPage(index) → 切换页面，触发 CollectionChanged
```

分页切换时，`MoveToPage()` 会：
1. 检查 `CurrentChanging` 事件是否允许切换
2. 更新 `_pageIndex`
3. 重建当前页的数据视图
4. 触发 `CollectionChanged(Reset)` 通知 DataGrid 刷新
5. 触发 `PageChanged` 事件通知 Pagination 控件

#### 分组实现原理

分组使用树形 `DataGridCollectionViewGroup` 结构：

```
DataGridCollectionViewGroupInternal (根组)
├── Group "A" (DataGridCollectionViewGroup)
│   ├── Item 1
│   └── Item 2
├── Group "B" (DataGridCollectionViewGroup)
│   ├── Item 3
│   └── Item 4
└── Group "C" (DataGridCollectionViewGroup)
    └── Item 5
```

支持多级分组（`GroupingDepth`），每级分组由 `DataGridGroupDescription` 描述。

#### DeferRefresh 机制

`DataGridCollectionView` 实现了 `DeferRefresh()` 模式，允许批量修改后一次性刷新：

```csharp
using (collectionView.DeferRefresh())
{
    // 批量添加排序、过滤描述
    collectionView.SortDescriptions.Add(...);
    collectionView.FilterDescriptions.Add(...);
    // 退出 using 块时自动 Refresh()
}
```

内部通过 `CollectionViewFlags` 位标志跟踪延迟状态：
- `IsRefreshDeferred` — 是否在延迟刷新周期内
- `IsMoveToPageDeferred` — 是否需要延迟页面切换
- `IsUpdatePageSizeDeferred` — 是否需要延迟 PageSize 更新

---

## 3. 虚拟化层

### 3.1 行虚拟化

DataGrid 实现了**基于 Slot 的行虚拟化**，只创建和显示视口内可见的行元素。

#### Slot 概念

Slot 是 DataGrid 中行的逻辑位置索引，包含数据行和分组行头：

```
Slot 0: DataGridRowGroupHeader (Level 0, Group "A")
Slot 1: DataGridRow (数据行)
Slot 2: DataGridRow (数据行)
Slot 3: DataGridRowGroupHeader (Level 0, Group "B")
Slot 4: DataGridRow (数据行)
Slot 5: DataGridRow (数据行)
...
```

- `SlotFromRowIndex(rowIndex)` — 将数据行索引转换为 Slot
- `RowIndexFromSlot(slot)` — 将 Slot 转换为数据行索引
- `SlotCount` — 总 Slot 数（包含分组行头）

#### DataGridDisplayData — 显示数据管理器

`DataGridDisplayData` 管理当前可见的滚动元素，核心数据结构：

```csharp
internal class DataGridDisplayData
{
    // 循环列表：存储当前显示的行/分组行头元素
    private List<Control> _scrollingElements;
    
    // 循环列表头索引（用于实现循环队列）
    private int _headScrollingElements;
    
    // 可回收行池（未完全回收，避免重新 Measure）
    private Stack<DataGridRow> _recyclableRows;
    
    // 完全回收行池（Visibility = Collapsed）
    private Stack<DataGridRow> _fullyRecycledRows;
    
    // 可回收分组行头池
    private Stack<DataGridRowGroupHeader> _recyclableGroupHeaders;
    
    // 完全回收分组行头池
    private Stack<DataGridRowGroupHeader> _fullyRecycledGroupHeaders;
    
    // 视口范围
    public int FirstScrollingSlot;    // 第一个可见 Slot
    public int LastScrollingSlot;     // 最后一个可见 Slot
}
```

#### 行回收机制

DataGrid 使用**两级回收池**优化行实例化性能：

```
┌──────────────────────────────────────────────────┐
│              行回收流程                            │
│                                                  │
│  滚动出视口的行                                    │
│      │                                           │
│      ▼                                           │
│  _recyclableRows (可回收池)                        │
│  ├── 行 DetachFromDataGrid(true)                  │
│  ├── 行仍保持 Visibility=Visible                  │
│  └── 优势：重新使用时无需重新 Measure              │
│      │                                           │
│      ▼ (FullyRecycleElements)                    │
│  _fullyRecycledRows (完全回收池)                   │
│  ├── 行 Visibility = Collapsed                   │
│  └── 优势：减少布局计算开销                        │
│      │                                           │
│      ▼ (GetUsedRow)                             │
│  重新使用时：                                     │
│  ├── 优先从 _recyclableRows 取出                  │
│  ├── 其次从 _fullyRecycledRows 取出并设 Visible   │
│  └── 都为空则创建新 DataGridRow 实例               │
└──────────────────────────────────────────────────┘
```

#### 循环列表实现

`_scrollingElements` 使用**循环列表（Circular List）**管理显示元素，通过 `_headScrollingElements` 索引标记列表头：

```
滚动向下时：
  [Row3] [Row4] [Row5]   →   回收 Row3，添加 Row6
  head=0                      [Row4] [Row5] [Row6]  head=0

循环插入时（insertIndex > Count）：
  [Row4] [Row5] [Row6]   →   head 向前移动
```

`GetCircularListIndex(slot, wrap)` 方法将 Slot 索引映射到循环列表中的实际位置。

#### UpdateDisplayedRows — 视口更新

`UpdateDisplayedRows(int newFirstDisplayedSlot, double displayHeight)` 是行虚拟化的核心方法：

1. 计算新的视口范围 `[newFirstDisplayedSlot, newLastDisplayedSlot]`
2. 与当前视口 `[FirstScrollingSlot, LastScrollingSlot]` 比较
3. 回收离开视口的行（加入回收池）
4. 从回收池取出或创建新行，填充进入视口的 Slot
5. 更新 `FirstScrollingSlot` / `LastScrollingSlot`

`UpdateDisplayedRowsFromBottom(int newLastDisplayedScrollingRow)` 处理向上滚动的情况，从底部开始计算。

### 3.2 列虚拟化

列虚拟化通过 `ShouldDisplayCell` 方法实现，在单元格 Measure 阶段判断该列是否在水平视口内：

```
水平视口范围：
  HorizontalOffset → HorizontalOffset + CellsWidth

列显示条件：
  column.ActualWidth > 0
  && column.Visibility == Visible
  && column.DisplayIndex >= FirstDisplayedScrollingCol
  && column.DisplayIndex <= LastTotallyDisplayedScrollingCol
```

不在视口内的列，其单元格 `Width` 被设为 0，避免布局计算。

---

## 4. 固定列（Frozen Columns）实现

### 4.1 固定列架构

DataGrid 支持左右两侧固定列，通过 `LeftFrozenColumnCount` 和 `RightFrozenColumnCount` 属性控制。

```
┌──────────────┬─────────────────────────────┬──────────────┐
│  Left Frozen │      Scrollable Area        │ Right Frozen │
│  (2 cols)    │      (N cols)               │  (1 col)     │
│              │◄────── 水平滚动 ──────►│              │
└──────────────┴─────────────────────────────┴──────────────┘
```

### 4.2 ProcessFrozenColumnCount

当固定列数变化时，调用 `ProcessFrozenColumnCount()`：

```csharp
private void ProcessFrozenColumnCount()
{
    CorrectColumnFrozenStates();   // 修正所有列的 IsFrozen 状态
    ComputeScrollBarsLayout();     // 重新计算滚动条布局
    
    InvalidateColumnHeadersArrange();  // 重排列头
    InvalidateCellsArrange();         // 重排单元格
}
```

### 4.3 CorrectColumnFrozenStates

遍历所有可见列，根据其 `DisplayIndex` 设置 `IsFrozen` 状态：

```
DisplayIndex < LeftFrozenColumnCount → IsFrozen = true (左侧固定)
DisplayIndex >= Columns.Count - RightFrozenColumnCount → IsFrozen = true (右侧固定)
其他 → IsFrozen = false (可滚动)
```

### 4.4 CheckFrozenColumnCount

验证固定列数合法性：

```csharp
private void CheckFrozenColumnCount()
{
    var totalFrozenCount = LeftFrozenColumnCount + RightFrozenColumnCount;
    if (totalFrozenCount > Columns.Count)
    {
        throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo(
            "LeftFrozenColumnCount + RightFrozenColumnCount",
            "Columns.Count", Columns.Count);
    }
}
```

### 4.5 固定列阴影效果

固定列与滚动区域的分界处显示阴影，通过 `LeftFrozenShadows` / `RightFrozenShadows` 内部属性控制。阴影由主题样式中的 `BoxShadow` 实现，在 `DataGrid.axaml` 中定义。

### 4.6 分组行头固定

`IsRowGroupHeadersFrozen` 属性控制分组行头（`DataGridRowGroupHeader`）是否跟随水平滚动：

- `true`（默认）：分组行头横跨整个表格宽度，不随水平滚动移动
- `false`：分组行头参与水平滚动

`SetupColumnGroupFrozenState()` 方法在列分组变化时更新分组行头的固定状态。

---

## 5. 树形表头（Column Group Headers）实现

### 5.1 数据结构

树形表头通过 `DataGridColumnGroupItem` 构建层级结构：

```
IDataGridColumnGroupItem (接口)
├── DataGridColumnGroupItem (分组节点)
│   ├── Header              — 分组标题
│   ├── HeaderTemplate      — 标题模板
│   ├── GroupChildren       — 子项集合 (ObservableCollection<IDataGridColumnGroupItem>)
│   ├── GroupParent         — 父节点
│   └── HeaderCell          — 对应的 DataGridColumnGroupHeader 控件
└── DataGridColumn (叶节点)
    └── 通过 GroupParent 关联到父分组
```

### 5.2 构建流程

用户通过 `ColumnGroups` 属性定义分组结构：

```xml
<atom:DataGrid.ColumnGroups>
    <atom:DataGridColumnGroupItem Header="基本信息">
        <atom:DataGridColumnGroupItem Header="个人">
            <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
            <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
        </atom:DataGridColumnGroupItem>
        <atom:DataGridTextColumn Header="地址" Binding="{Binding Address}" />
    </atom:DataGridColumnGroupItem>
</atom:DataGrid.ColumnGroups>
```

`BuildColumnGroupView()` 方法将 `ColumnGroups` 展平为 `ColumnsInternal` 集合：

1. 遍历 `ColumnGroups` 的每个顶层项
2. 递归调用 `CollectColumnsFromGroupTree()` 提取所有 `DataGridColumn` 叶节点
3. 将提取的列添加到 `ColumnsInternal` 集合
4. 设置 `IsGroupHeaderMode = true`

### 5.3 集合变更处理

`HandleGroupColumnsInternalCollectionChanged()` 监听 `ColumnGroups` 的变更：

- **Add**：递归收集新增分组下的所有列，添加到 `ColumnsInternal`
- **Remove**：递归收集移除分组下的所有列，从 `ColumnsInternal` 移除
- **Reset**：清空 `ColumnsInternal`

子项变更通过 `IDataGridColumnGroupChanged` 接口传播：

```csharp
public interface IDataGridColumnGroupChanged
{
    event EventHandler<DataGridColumnGroupChangedArgs>? GroupChanged;
}
```

### 5.4 DataGridColumnGroupHeader 控件

`DataGridColumnGroupHeader` 是分组表头的视觉元素，由 `DataGridColumnGroupItem.CreateHeader()` 创建：

```csharp
internal virtual DataGridColumnGroupHeader CreateHeader()
{
    var result = new DataGridColumnGroupHeader();
    result.OwningGroupItem = this;
    // 绑定属性
    result[!HeaderProperty] = this[!HeaderProperty];
    result[!HeaderTemplateProperty] = this[!HeaderTemplateProperty];
    result[!HorizontalContentAlignmentProperty] = this[!HorizontalAlignmentProperty];
    result[!VerticalContentAlignmentProperty] = this[!VerticalAlignmentProperty];
    result[!SizeTypeProperty] = OwningGrid[!DataGrid.SizeTypeProperty];
    
    result.PointerPressed += (s, e) => { HeaderPointerPressed?.Invoke(this, e); };
    result.PointerReleased += (s, e) => { HeaderPointerReleased?.Invoke(this, e); };
    return result;
}
```

分组表头与普通列头共享 `DataGridColumnHeader` 的部分样式，但具有独立的 `ControlTheme`。

### 5.5 多级嵌套渲染

表头区域使用 `DataGridHeaderViewItem` 作为容器，按层级从上到下排列：

```
┌─────────────────────────────────────────┐
│ Level 0: "基本信息"                       │  ← DataGridColumnGroupHeader
├─────────────────┬───────────────────────┤
│ Level 1: "个人"  │                       │  ← DataGridColumnGroupHeader
├────────┬────────┼───────────────────────┤
│ 姓名   │ 年龄   │ 地址                    │  ← DataGridColumnHeader
└────────┴────────┴───────────────────────┘
```

每层表头的高度由 `ColumnHeaderHeight` 统一控制，分组层级深度决定了表头区域的总高度。

---

## 6. 行分组（Row Grouping）实现

### 6.1 分组数据结构

行分组使用 `DataGridCollectionViewGroup` 树形结构，由 `DataGridCollectionView` 管理：

```
DataGridCollectionView
    │
    ├── GroupDescriptions (分组描述集合)
    │   └── DataGridPathGroupDescription("Category")
    │
    └── Groups (顶级分组)
        ├── DataGridCollectionViewGroup (Key="Electronics")
        │   ├── SubGroups (如果有二级分组)
        │   └── Items (叶级数据项)
        └── DataGridCollectionViewGroup (Key="Books")
            └── Items
```

### 6.2 RowGroupHeadersTable

`RowGroupHeadersTable`（`IndexToValueTable<DataGridRowGroupInfo>`）记录所有分组行头的 Slot 位置：

```csharp
internal class DataGridRowGroupInfo
{
    public CollectionViewGroup? CollectionViewGroup;  // 对应的分组
    int Level;                    // 分组层级
    int Slot;                     // 所在 Slot
    int LastSubItemSlot;          // 最后一个子项的 Slot
    bool IsVisible;               // 是否可见
}
```

### 6.3 分组行展开/折叠

分组行的展开/折叠通过 `_collapsedSlotsTable` 跟踪：

- **展开**：从 `_collapsedSlotsTable` 移除该分组的 Slot，使其子行可见
- **折叠**：将该分组的 Slot 加入 `_collapsedSlotsTable`，隐藏其子行

`EnsureRowGroupVisibility(rowGroupInfo, isVisible, animate)` 方法处理分组可见性变更：

1. 更新 `RowGroupHeadersTable` 中的 `IsVisible` 状态
2. 更新 `_collapsedSlotsTable`
3. 重新计算 `SlotCount` 和行索引映射
4. 刷新显示数据 `UpdateDisplayedRows()`

### 6.4 ExpandRowGroupParentChain

当需要显示某个深层分组的行时，`ExpandRowGroupParentChain(level, slot)` 递归展开其所有父分组：

```csharp
private void ExpandRowGroupParentChain(int level, int slot)
{
    // 从当前层级向上查找父分组
    // 如果父分组已折叠，先递归展开更上层
    // 然后确保当前层级的分组可见
}
```

---

## 7. 排序与过滤实现

### 7.1 排序流程

```
用户点击列头
    │
    ▼
DataGridColumnHeader.OnPointerPressed/OnPointerReleased
    │
    ▼
DataGrid.ProcessSortClick(column)
    │
    ├── 检查 CanUserSortColumns
    ├── 检查 column.CanUserSort
    ├── 触发 Sorting 事件
    │
    ▼
DataGridCollectionView.SortDescriptions
    │
    ├── 清除或添加 DataGridSortDescription
    ├── Refresh() 重建排序视图
    │
    ▼
DataGrid 刷新显示
```

排序方向由 `DataGridSortDescription.Direction` 控制（Ascending/Descending），支持三态排序（升序→降序→无排序）。

### 7.2 过滤流程

```
用户点击列头过滤图标
    │
    ▼
DataGridColumnHeader 打开过滤弹窗
    │
    ▼
DataGridDefaultFilter (默认过滤控件)
    │
    ├── 文本列：搜索框 + 列表选择
    ├── 数值列：范围选择
    └── 布尔列：勾选列表
    │
    ▼
用户确认过滤条件
    │
    ▼
DataGridCollectionView.FilterDescriptions
    │
    ├── 添加 DataGridFilterDescription
    ├── Refresh() 重建过滤视图
    │
    ▼
DataGrid 刷新显示
```

`DataGridFilterDescription` 支持多种过滤模式（`DataGridFilterMode`）：
- `None` — 不支持过滤
- `Default` — 使用内置过滤控件
- `Custom` — 自定义过滤控件

---

## 8. 编辑实现

### 8.1 编辑状态机

```
┌──────────┐  BeginEdit   ┌──────────┐  CommitEdit   ┌──────────┐
│  Normal  │─────────────►│ Editing  │──────────────►│  Normal  │
│  (只读)  │              │ (编辑中)  │               │ (已提交)  │
└──────────┘              └──────────┘               └──────────┘
                               │
                               │ CancelEdit
                               ▼
                          ┌──────────┐
                          │  Normal  │
                          │ (已取消)  │
                          └──────────┘
```

### 8.2 编辑流程

1. **进入编辑**：`BeginCellEdit()` → 触发 `BeginningEdit` 事件 → 创建编辑元素
2. **编辑中**：编辑控件（TextBox/CheckBox/NumericUpDown 等）接收用户输入
3. **提交编辑**：`CommitEdit()` → 触发 `CellEditEnding` → 写回数据 → 触发 `CellEditEnded`
4. **取消编辑**：`CancelEdit()` → 触发 `CellEditEnding(Cancel=true)` → 恢复原值

### 8.3 列类型与编辑控件

| 列类型 | 显示元素 | 编辑元素 |
|--------|----------|----------|
| `DataGridTextColumn` | `TextBlock` | `TextBox` |
| `DataGridNumericColumn` | `TextBlock` | `NumericUpDown` |
| `DataGridCheckBoxColumn` | `CheckBox`（只读） | `CheckBox`（可交互） |
| `DataGridTemplateColumn` | `CellTemplate` | `CellEditingTemplate` |

---

## 9. 拖拽排序实现

### 9.1 列拖拽

列拖拽通过 `DataGridColumnHeader` 的 Pointer 事件实现：

```
PointerPressed → 记录起始位置
    │
    ▼ (拖拽距离超过阈值)
ColumnReordering 事件 → 创建拖拽视觉反馈
    │
    ▼ (拖拽中)
ColumnDraggingOver 事件 → 更新插入位置指示器
    │
    ▼ (释放)
ColumnReordered 事件 → 更新列 DisplayIndex
```

### 9.2 行拖拽

行拖拽通过 `DataGridRowReorderColumn` + `DataGridRow` 的 Pointer 事件实现：

```
PointerPressed (在拖拽手柄列上) → 记录起始行
    │
    ▼ (拖拽距离超过阈值)
RowReordering 事件 → 创建行拖拽视觉反馈
    │
    ▼ (拖拽中)
更新插入位置指示器
    │
    ▼ (释放)
RowReordered 事件 → 移动数据项位置
```

---

## 10. 滚动与布局

### 10.1 滚动计算

`ComputeScrollBarsLayout()` 计算滚动条的状态：

```
垂直滚动条：
  Maximum = 总内容高度 - 视口高度
  ViewportSize = 视口高度
  
水平滚动条：
  Maximum = 总列宽 - 视口宽度
  ViewportSize = 视口宽度
```

### 10.2 ScrollSlotsByHeight

`ScrollSlotsByHeight(double height)` 处理像素级垂直滚动：

1. 计算新的 `FirstScrollingSlot`（基于行高估算）
2. 调用 `UpdateDisplayedRows()` 更新视口
3. 更新 `_negVerticalOffset`（行对齐的余量偏移）

### 10.3 列宽计算

列宽支持三种模式（`DataGridLength`）：

| 模式 | 说明 | 计算方式 |
|------|------|----------|
| `Auto` | 自动宽度 | 根据内容计算最大宽度 |
| `Pixel` | 固定像素 | 直接使用指定值 |
| `Star` | 比例宽度 | 按比例分配剩余空间 |

`UsesStarSizing` 标志判断是否使用 Star 模式。当 DataGrid 具有无限可用宽度时，Star 列会被限制为 10000 像素以避免无限扩展。

`AutoSizingColumns` 标志跟踪是否有 Auto 列正在等待所有行测量完成以确定最终宽度。

---

## 11. 选择系统

### 11.1 选择模式

| 模式 | 说明 |
|------|------|
| `Single` | 单选，只能选中一行 |
| `Extended` | 扩展多选，支持 Ctrl/Shift 多选 |

### 11.2 选择操作

`UpdateSelectionAndCurrency(columnIndex, slot, action, scrollIntoView)` 是选择系统的核心方法，支持以下操作：

| 操作 | 说明 |
|------|------|
| `SelectCurrent` | 选中当前行/单元格 |
| `SelectFromAnchor` | 从锚点行到当前行范围选择 |
| `Toggle` | 切换当前行选中状态 |
| `AddToSelection` | 将当前行添加到选中集合 |
| `None` | 不改变选择 |

### 11.3 选择批量操作

`NoSelectionChangeCount` 用于批量选择操作，避免多次触发 `SelectionChanged` 事件：

```csharp
NoSelectionChangeCount++;
try
{
    // 执行多个选择操作
    UpdateSelectionAndCurrency(...);
    UpdateSelectionAndCurrency(...);
}
finally
{
    NoSelectionChangeCount--;  // 归零时触发 FlushSelectionChanged()
}
```

---

## 12. 圆角边框与视觉规范

### 12.1 圆角边框

DataGrid 支持 Ant Design 风格的圆角边框，通过以下内部属性控制：

- `FrameBorderThickness` — 外框边框厚度
- `HeaderCornerRadius` — 表头区域圆角

`ConfigureFrameBorderThickness()` 根据是否有表尾和网格线动态调整边框：

```csharp
if (Footer == null && AreHorizontalGridLinesVisible)
{
    // 无表尾且有水平网格线时，底部无边框
    FrameBorderThickness = new Thickness(Left, Top, Right, 0);
}
else
{
    FrameBorderThickness = BorderThickness;
}
```

`ConfigureHeaderCornerRadius()` 根据是否有 Title 和行头动态调整表头圆角：

```csharp
if (Title == null)
{
    if (!IsRowHeadersVisible)
        HeaderCornerRadius = new CornerRadius(TopLeft, TopRight, 0, 0);
    else
        HeaderCornerRadius = new CornerRadius(0, TopRight, 0, 0);
}
else
{
    HeaderCornerRadius = new CornerRadius(0);  // 有 Title 时表头无圆角
}
```

### 12.2 尺寸变体

`SizeType` 属性控制表格的整体尺寸，影响行高、字体大小、内边距等：

| SizeType | 行高 | 字体 | 内边距 |
|----------|------|------|--------|
| `Large` | 54px | 14px | 16px |
| `Middle` | 46px | 14px | 12px |
| `Small` | 36px | 12px | 8px |

尺寸值通过 `DataGridToken` 中的 Design Token 定义，支持主题自动适配。

---

## 13. 关键内部类型索引

| 类型 | 文件 | 职责 |
|------|------|------|
| `DataGridDisplayData` | `DataGridDisplayData.cs` | 显示数据管理、行回收池 |
| `DataGridDataConnection` | `Data/DataGridDataConnection.cs` | 数据源连接 |
| `DataGridCollectionView` | `Data/DataGridCollectionView.cs` | 排序/过滤/分页/分组 |
| `DataGridCollectionViewGroup` | `Data/DataGridCollectionViewGroup.cs` | 分组数据节点 |
| `DataGridCollectionViewGroupInternal` | `Data/DataGridCollectionViewGroupInternal.cs` | 分组内部实现 |
| `CollectionViewGroupRoot` | `Data/CollectionViewGroupRoot.cs` | 分组根节点 |
| `DataGridSortDescription` | `Data/DataGridSortDescription.cs` | 排序描述 |
| `DataGridFilterDescription` | `Data/DataGridFilterDescription.cs` | 过滤描述 |
| `DataGridGroupDescription` | `Data/DataGridGroupDescription.cs` | 分组描述 |
| `DataGridPathGroupDescription` | `Data/DataGridPathGroupDescription.cs` | 基于路径的分组描述 |
| `DataGridDefaultFilter` | `Data/DataGridDefaultFilter.cs` | 默认过滤控件 |
| `DataGridRowGroupInfo` | `Row/DataGridRowGroupInfo.cs` | 行分组信息 |
| `DataGridCellCoordinates` | `DataGridCellCoordinates.cs` | 单元格坐标 |
| `IndexToValueTable<T>` | `Utils/` | 索引→值映射表 |
| `DataGridError` | `Utils/` | 错误信息辅助 |

---

## 14. 性能优化要点

### 14.1 行虚拟化

- 只实例化视口内可见的行，避免创建大量 DOM 元素
- 两级回收池减少行实例化和 Measure 开销
- 循环列表避免频繁的 List 插入/删除操作

### 14.2 列虚拟化

- 不在视口内的列，其单元格宽度设为 0
- `ShouldDisplayCell` 在 Measure 阶段快速判断

### 14.3 数据操作优化

- `DeferRefresh()` 批量操作避免多次刷新
- `DataProperties` 缓存反射结果
- `StringBuilderCache` 优化剪贴板字符串构建

### 14.4 布局优化

- `AutoSizingColumns` 延迟 Auto 列的最终宽度计算
- `RowHeightEstimate` / `RowDetailsHeightEstimate` 用于快速估算滚动位置
- `HorizontalAdjustment` 处理视口宽度变化时的偏移修正

---

## 15. 与 Avalonia DataGrid 的关键差异

| 方面 | Avalonia DataGrid | AtomUI DataGrid |
|------|-------------------|-----------------|
| 数据视图 | 依赖 `ICollectionView` | 自定义 `DataGridCollectionView` |
| 分页 | 不支持 | 内置 `PageSize` / `MoveToPage()` |
| 过滤 | 不支持 | 内置 `FilterDescriptions` + 默认过滤控件 |
| 列分组 | 不支持 | `DataGridColumnGroupItem` 树形结构 |
| 行拖拽 | 不支持 | `DataGridRowReorderColumn` |
| 固定列阴影 | 不支持 | `LeftFrozenShadows` / `RightFrozenShadows` |
| 圆角边框 | 不支持 | `FrameBorderThickness` + `HeaderCornerRadius` |
| 尺寸变体 | 不支持 | `SizeType` (Large/Middle/Small) |
| 空状态 | 不支持 | `EmptyContentTemplate` |
| 加载状态 | 不支持 | `IsOperating` |
| 表头/表尾 | 不支持 | `HeaderTemplate` / `FooterTemplate` |
