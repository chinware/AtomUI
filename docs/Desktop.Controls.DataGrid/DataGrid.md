# DataGrid 控件概述与设计原理

## 1. 控件简介

DataGrid 是 AtomUI 中功能最丰富的数据展示控件，用于以表格形式展示和操作结构化数据集合。它对齐 Ant Design 的 [Table](https://ant.design/components/table/) 组件规范，在 Avalonia 原生 DataGrid 的基础上进行了深度重构和功能扩展。

### 核心定位

- **数据展示**：以行列结构展示结构化数据
- **数据操作**：支持排序、过滤、选择、编辑、拖拽排序等交互
- **视觉规范**：严格遵循 Ant Design Table 的视觉规范和交互模式
- **主题适配**：通过 Design Token 体系实现亮色/暗色模式自动切换

---

## 2. 与 Ant Design Table 的对齐程度

| 功能 | Ant Design Table | AtomUI DataGrid | 对齐状态 |
|------|-----------------|-----------------|---------|
| 基础表格 | ✅ | ✅ | 完全对齐 |
| 固定表头 | ✅ | ✅ | 完全对齐 |
| 固定列 | ✅ | ✅ | 完全对齐 |
| 排序 | ✅ | ✅ | 完全对齐 |
| 过滤 | ✅ | ✅ | 完全对齐 |
| 行选择 | ✅ | ✅ | 完全对齐 |
| 可展开行 | ✅ | ✅ | 完全对齐 |
| 分页 | ✅ | ✅ | 完全对齐 |
| 树形数据 | ✅ | ✅ | 完全对齐 |
| 分组表头 | ✅ | ✅ | 完全对齐 |
| 拖拽排序（行） | ✅（社区） | ✅ | 原生支持 |
| 拖拽排序（列） | ✅（社区） | ✅ | 原生支持 |
| 可编辑单元格 | ✅（社区） | ✅ | 原生支持 |
| 尺寸变体 | ✅ | ✅ | 完全对齐 |
| 空状态 | ✅ | ✅ | 完全对齐 |
| 加载状态 | ✅ | ✅ | 完全对齐 |
| 自定义表头/表尾 | ✅ | ✅ | 完全对齐 |
| 圆角边框 | ✅ | ✅ | 完全对齐 |
| 暗色模式 | ✅ | ✅ | 完全对齐 |

---

## 3. 与 Avalonia DataGrid 的关系

### 继承关系

AtomUI DataGrid **不继承**自 Avalonia 的 `DataGrid`，而是完全独立实现，继承自 `Avalonia.Controls.Control`：

```
Avalonia.Controls.Control
└── AtomUI.Desktop.Controls.DataGrid
```

### 重构原因

1. **视觉规范对齐**：Avalonia DataGrid 的视觉风格与 Ant Design 差异较大，无法通过简单样式覆盖实现
2. **功能扩展**：需要原生支持分组表头、行拖拽、列拖拽、内置过滤、内置分页等 Ant Design 特性
3. **Token 体系**：需要深度集成 AtomUI 的 Design Token 体系，实现主题自动适配
4. **圆角边框**：Ant Design Table 使用圆角边框，Avalonia DataGrid 的矩形结构无法直接支持

### 主要差异

| 方面 | Avalonia DataGrid | AtomUI DataGrid |
|------|-------------------|-----------------|
| 基类 | `Control` | `Control`（独立实现） |
| 视觉规范 | 默认 Avalonia 风格 | Ant Design Table 规范 |
| 主题系统 | `ControlTheme` | `ControlTheme` + Design Token |
| 分组表头 | 不支持 | `DataGridColumnGroupItem` |
| 行拖拽 | 不支持 | `CanUserReorderRows` + `DataGridRowReorderColumn` |
| 列拖拽 | 不支持 | `CanUserReorderColumns` |
| 内置过滤 | 不支持 | `CanUserFilter` + `FilterMode` |
| 内置分页 | 不支持 | `PaginationVisibility` + `PageSize` |
| 尺寸变体 | 不支持 | `SizeType`（Large/Middle/Small） |
| 圆角边框 | 不支持 | `CornerRadius` |
| 固定列阴影 | 不支持 | `LeftFrozenShadows` / `RightFrozenShadows` |
| 空状态模板 | 不支持 | `EmptyContentTemplate` |
| 加载状态 | 不支持 | `IsOperating` |
| 自定义表头/表尾 | 不支持 | `HeaderTemplate` / `FooterTemplate` |

---

## 4. 控件架构

### 核心类型

```
AtomUI.Desktop.Controls.DataGrid          — 主控件
├── DataGridColumn (abstract)             — 列基类
│   ├── DataGridBoundColumn (abstract)    — 绑定列基类
│   │   ├── DataGridTextColumn            — 文本列
│   │   ├── DataGridNumericColumn         — 数值列
│   │   └── DataGridCheckBoxColumn        — 复选框列
│   ├── DataGridTemplateColumn            — 模板列
│   ├── DataGridSelectionColumn           — 选择列
│   ├── DataGridDetailExpanderColumn      — 展开列
│   ├── DataGridRowReorderColumn          — 行拖拽列
│   └── DataGridColumnGroupItem           — 列分组项
├── DataGridRow                           — 数据行
├── DataGridCell                          — 数据单元格
├── DataGridColumnHeader                  — 列头
├── DataGridRowHeader                     — 行头
├── DataGridRowGroupHeader                — 分组行头
└── DataGridColumnGroupHeader             — 分组列头
```

### 辅助类型

```
AtomUI.Desktop.Controls.DataGridToken              — Design Token 定义
AtomUI.Desktop.Controls.DataGridLength              — 列宽类型
AtomUI.Desktop.Controls.DataGridSortDescription     — 排序描述
AtomUI.Desktop.Controls.DataGridRowClipboardContent — 剪贴板内容
```

### 事件参数类型

```
DataGridAutoGeneratingColumnEventArgs
DataGridBeginningEditEventArgs
DataGridCellEditEndedEventArgs
DataGridCellEditEndingEventArgs
DataGridCellPointerPressedEventArgs
DataGridColumnEventArgs
DataGridColumnReorderingEventArgs
DataGridColumnDraggingOverEventArgs
DataGridRowEventArgs
DataGridRowReorderingEventArgs
DataGridRowEditEndedEventArgs
DataGridRowEditEndingEventArgs
DataGridPreparingCellForEditEventArgs
DataGridRowDetailsEventArgs
DataGridRowGroupHeaderEventArgs
DataGridRowClipboardEventArgs
PageChangedEventArgs
PageChangingEventArgs
```

---

## 5. 源码位置

| 模块 | 路径 |
|------|------|
| 控件实现 | `src/AtomUI.Desktop.Controls.DataGrid/` |
| Design Token | `src/AtomUI.Desktop.Controls.DataGrid/DataGridToken.cs` |
| 主题样式 | `src/AtomUI.Desktop.Controls.DataGrid/Themes/DataGrid.axaml` |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` |

---

## 6. 文档导航

| 文档 | 路径 | 说明 |
|------|------|------|
| 主控件 API | [API/DataGridAPI.md](API/DataGridAPI.md) | DataGrid 属性、事件、方法 |
| 列类型 API | [API/DataGridColumnAPI.md](API/DataGridColumnAPI.md) | 所有列类型的属性 |
| 枚举类型 | [API/DataGridEnums.md](API/DataGridEnums.md) | 枚举值汇总 |
| 排序功能 | [Features/Sorting.md](Features/Sorting.md) | 排序功能详解 |
| 过滤功能 | [Features/Filtering.md](Features/Filtering.md) | 过滤功能详解 |
| 行选择功能 | [Features/Selection.md](Features/Selection.md) | 选择功能详解 |
| 固定列 | [Features/FixedColumns.md](Features/FixedColumns.md) | 固定列与固定表头 |
| 拖拽功能 | [Features/DragDrop.md](Features/DragDrop.md) | 列拖拽与行拖拽 |
| 编辑功能 | [Features/Editing.md](Features/Editing.md) | 单元格编辑 |
| 可展开行 | [Features/ExpandableRows.md](Features/ExpandableRows.md) | 行展开/折叠 |
| 分页功能 | [Features/Pagination.md](Features/Pagination.md) | 内置分页 |
| 分组表头 | [Features/GroupHeaders.md](Features/GroupHeaders.md) | 多级嵌套表头 |
| 样式指南 | [Styling/DataGridStyling.md](Styling/DataGridStyling.md) | 自定义样式 |
| Token 参考 | [Styling/DataGridToken.md](Styling/DataGridToken.md) | Design Token |
| 基础用法 | [Usage/BasicUsage.md](Usage/BasicUsage.md) | 基础用法与尺寸 |
| 选择示例 | [Usage/SelectionUsage.md](Usage/SelectionUsage.md) | 选择功能示例 |
| 排序过滤示例 | [Usage/SortAndFilterUsage.md](Usage/SortAndFilterUsage.md) | 排序与过滤示例 |
| 固定拖拽示例 | [Usage/FixedAndDragUsage.md](Usage/FixedAndDragUsage.md) | 固定列与拖拽示例 |
| 编辑展开示例 | [Usage/EditAndExpandUsage.md](Usage/EditAndExpandUsage.md) | 编辑与展开行示例 |
| 分页示例 | [Usage/PaginationUsage.md](Usage/PaginationUsage.md) | 分页示例 |
| 高级示例 | [Usage/AdvancedUsage.md](Usage/AdvancedUsage.md) | 分组表头、空状态等 |