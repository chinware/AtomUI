# DataGrid 控件设计文档

## 1. 概述

`DataGrid` 是 AtomUI 中最复杂的数据展示控件之一，用于以表格形式显示和编辑结构化数据集合。该控件对齐 Ant Design 的 [Table](https://ant.design/components/table/) 组件规范，在 Avalonia 原生 `DataGrid` 的基础上进行了深度定制和功能扩展。

### 设计意图

- **Ant Design 规范对齐**：视觉风格、交互模式与 Ant Design Table 保持一致，包括表头排序、行悬浮/选中态、固定列阴影、可展开行等。
- **企业级数据展示**：支持排序、过滤、分页、行选择、可展开行、分组表头、列拖拽排序、行拖拽排序等企业级功能。
- **主题系统集成**：通过 Design Token 体系实现完整的主题定制能力，支持亮色/暗色模式自动切换。
- **尺寸自适应**：支持 `Large`、`Middle`、`Small` 三种尺寸，影响单元格内边距和字体大小。

### 功能范围

| 功能 | 说明 | Ant Design 对齐程度 |
|------|------|---------------------|
| 基础表格 | 数据绑定、列定义、表头/表尾 | ✅ 完全对齐 |
| 排序 | 单列排序、多列排序、自定义排序 | ✅ 完全对齐 |
| 过滤 | 列头过滤、自定义过滤 UI | ✅ 完全对齐 |
| 行选择 | 单选/多选、Checkbox 列 | ✅ 完全对齐 |
| 可展开行 | 行详情展开/折叠 | ✅ 完全对齐 |
| 固定列 | 左/右固定列、固定表头 | ✅ 完全对齐 |
| 列拖拽排序 | 拖拽列头重排列顺序 | ✅ 完全对齐 |
| 行拖拽排序 | 拖拽行重排数据顺序 | ✅ 完全对齐 |
| 分页 | 内置分页器、上下分页 | ✅ 完全对齐 |
| 分组表头 | 多级表头嵌套 | ✅ 完全对齐 |
| 单元格编辑 | 文本/数值/模板列编辑 | ✅ 完全对齐 |
| 自定义空状态 | 空数据和加载状态 | ✅ 完全对齐 |
| 列隐藏 | 动态显示/隐藏列 | ✅ 完全对齐 |
| 行头 | 可选行头显示 | ⚠️ 部分对齐（Avalonia 扩展） |
| 列宽调整 | 拖拽调整列宽 | ✅ 完全对齐 |

## 2. 架构设计

### 继承关系

```
AvaloniaObject
└── Avalonia.Controls.Control
    └── AtomUI.Desktop.Controls.DataGrid
        (继承自 Avalonia Control，组合使用内部子元素)
```

`DataGrid` 继承自 Avalonia 的 `Control` 基类，而非 Avalonia 原生的 `DataGrid`。AtomUI 对 DataGrid 进行了完全重写，以实现 Ant Design 风格的视觉效果和交互体验。

### 核心组合关系

DataGrid 内部由以下关键子组件组成：

| 子组件 | 类型 | 说明 |
|--------|------|------|
| `DataGridRow` | 行控件 | 表示数据行，包含单元格集合 |
| `DataGridCell` | 单元格控件 | 表示单个数据单元格 |
| `DataGridColumnHeader` | 列头控件 | 表示列标题头 |
| `DataGridRowHeader` | 行头控件 | 表示行标题头 |
| `DataGridRowGroupHeader` | 分组行头 | 用于分组数据的行头 |
| `DataGridDetailsPanel` | 行详情面板 | 展开行时显示的详情内容 |
| `DataGridColumnGroupItem` | 列分组项 | 用于分组表头的嵌套结构 |

### 列类型体系

```
DataGridColumn (抽象基类)
├── DataGridBoundColumn (抽象，支持数据绑定的列)
│   ├── DataGridTextColumn     - 文本列
│   ├── DataGridNumericColumn  - 数值列
│   └── DataGridCheckBoxColumn - 复选框列
├── DataGridTemplateColumn     - 模板列（自定义 CellTemplate/CellEditingTemplate）
├── DataGridSelectionColumn    - 行选择列（Checkbox）
├── DataGridDetailExpanderColumn - 行展开/折叠控制列
├── DataGridRowReorderColumn   - 行拖拽排序手柄列
└── DataGridColumnGroupItem    - 列分组项（用于分组表头）
```

### 设备无关层

DataGrid 控件完全位于设备相关层（`AtomUI.Desktop.Controls`），没有设备无关的基类定义。所有代码位于 `src/AtomUI.Desktop.Controls.DataGrid` 项目中。

## 3. 源码组织

DataGrid 源码位于 `src/AtomUI.Desktop.Controls.DataGrid` 目录，结构如下：

```
src/AtomUI.Desktop.Controls.DataGrid/
├── DataGrid.cs                    # 主控件实现
├── DataGridToken.cs               # Design Token 定义
├── Column/                        # 列相关类型
│   ├── DataGridColumn.cs          # 列抽象基类
│   ├── DataGridBoundColumn.cs     # 绑定列基类
│   ├── DataGridTextColumn.cs      # 文本列
│   ├── DataGridNumericColumn.cs   # 数值列
│   ├── DataGridCheckBoxColumn.cs  # 复选框列
│   ├── DataGridTemplateColumn.cs  # 模板列
│   ├── DataGridSelectionColumn.cs # 选择列
│   ├── DataGridDetailExpanderColumn.cs # 展开列
│   ├── DataGridRowReorderColumn.cs     # 行拖拽列
│   └── DataGridColumnGroupItem.cs      # 列分组项
├── Row/                           # 行相关类型
│   ├── DataGridRow.cs             # 行控件
│   ├── DataGridRowHeader.cs       # 行头控件
│   ├── DataGridRowGroupHeader.cs  # 分组行头
│   └── DataGridCell.cs            # 单元格控件
├── Header/                        # 表头相关类型
│   ├── DataGridColumnHeader.cs    # 列头控件
│   └── DataGridColumnGroupHeader.cs # 分组列头
├── Events/                        # 事件参数类型
├── Utils/                         # 工具类
├── Themes/                        # 主题/样式定义
│   └── DataGrid.axaml             # 控件默认样式
└── GeneratedFiles/                # 代码生成文件
```

## 4. 与 Avalonia 原生 DataGrid 的差异

AtomUI 的 DataGrid 相对于 Avalonia 原生 DataGrid 有以下主要差异：

| 特性 | Avalonia 原生 | AtomUI |
|------|--------------|--------|
| 视觉风格 | 默认 Fluent 风格 | Ant Design 风格 |
| 主题系统 | ControlTheme | Design Token 体系 |
| 排序指示器 | 简单箭头 | Ant Design 风格排序图标 + Tooltip |
| 过滤功能 | 无内置 | 内置列头过滤下拉 |
| 行选择列 | 无内置 | `DataGridSelectionColumn` |
| 展开行控制列 | 无内置 | `DataGridDetailExpanderColumn` |
| 行拖拽排序 | 无内置 | `DataGridRowReorderColumn` + `CanUserReorderRows` |
| 分组表头 | 不支持 | `DataGridColumnGroupItem` 支持 |
| 内置分页 | 不支持 | `PaginationVisibility` + `PageSize` |
| 固定列阴影 | 无 | `LeftFrozenShadows` / `RightFrozenShadows` Token |
| 空状态/加载 | 无内置 | `IsOperating` + `EmptyContentTemplate` |
| 尺寸变体 | 无 | `SizeType` (Large/Middle/Small) |

## 5. Gallery 示例参考

DataGrid 的完整示例位于：

- **AXAML**: `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml`
- **Code-behind**: `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml.cs`
- **ViewModel**: `controlgallery/AtomUIGallery/ShowCases/ViewModels/DataDisplay/DataGridViewModel.cs`

Gallery 中展示了以下场景：
1. 基础用法
2. 行选择（单选/多选）
3. 排序与过滤
4. 多列排序
5. 可展开行
6. 展开列/选择列顺序控制
7. 行头显示
8. 分组表头
9. 列隐藏
10. 固定表头
11. 固定列
12. 固定列+固定表头
13. 列拖拽排序
14. 行拖拽排序
15. 自定义空状态/加载
16. 可编辑单元格
17. 分页
18. 尺寸变体（大/中/小）
19. 自定义表头/表尾