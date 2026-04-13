# ListView 列表视图

## 概述

列表视图（ListView）是一种用于展示和操作结构化数据集合的高级控件。相比基础的 `ListBox`，ListView 提供了更加丰富的企业级功能——分组、排序、过滤、分页、虚拟化、加载状态覆盖等，适用于需要处理大量数据并提供良好交互体验的场景。

AtomUI 的 `ListView` 控件对标 [Ant Design 5.0 List](https://ant.design/components/list-cn) 组件的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和数据操作能力。

---

## 设计原理

### Ant Design 的列表设计哲学

Ant Design 对列表的定位是：**「最基础的列表展示，可承载文字、列表、图片、段落，常用于后台数据展示页面」**。其核心设计理念包括：

- **数据驱动**：列表内容完全由数据源驱动，而非手动添加 UI 元素
- **可选的选择模式**：支持单选、多选、Toggle 选择等多种模式
- **内置分组**：支持按属性对数据进行分组展示，带有分组标题
- **过滤与排序**：内置数据视图（CollectionView）支持，无需手动操作数据源
- **分页支持**：内置分页器集成，支持大数据量的分页展示
- **空状态**：当无数据时自动展示空状态指示器

### Avalonia ItemsControl 基础能力

AtomUI 的 `ListView` 直接继承自 Avalonia 框架的 `Avalonia.Controls.ItemsControl`（而非 `Avalonia.Controls.ListBox`）。ListView 自行实现了完整的选择逻辑，以支持更灵活的分页选择和数据视图集成。

**Avalonia ItemsControl 的核心职责：**

Avalonia 的 `ItemsControl` 是所有集合控件的基类，它负责管理数据项集合、创建容器控件、以及通过 `ItemsPresenter` + `Panel` 进行布局。其继承链为：

```
Control → TemplatedControl → ItemsControl
```

**ItemsControl 提供的基础能力：**

| 能力 | 说明 |
|---|---|
| `Items` / `ItemsSource` | 项目集合管理，支持直接添加和数据源绑定 |
| `ItemTemplate` | 通过 `IDataTemplate` 定义项目的数据展示方式 |
| `ItemsPanel` | 自定义项目面板布局（默认为 `VirtualizingStackPanel`） |
| 容器化 | 自动为每个数据项创建容器控件（`ListViewItem`），支持容器回收 |
| 虚拟化 | 通过 `VirtualizingStackPanel` 实现 UI 虚拟化，仅渲染可见项 |

### AtomUI ListView 的扩展设计

AtomUI `ListView` 在 Avalonia ItemsControl 的基础上，做了大量扩展以对齐 Ant Design 规范并满足企业级需求：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **完整选择系统** | 自实现 `ISelectionModel` + `SelectionMode` | 独立于 Avalonia ListBox 的选择逻辑，支持分页全局索引选择 |
| **三种尺寸** | `ISizeTypeAware` 接口 + `SizeType` 共享属性 | 全局统一的 Large / Middle / Small 尺寸系统 |
| **数据视图集成** | `IListCollectionView` + `ListCollectionView` | 自动包装数据源为可排序、可过滤、可分组、可分页的视图 |
| **分组展示** | `IsGroupEnabled` + `GroupPropertySelector` | 按指定属性对数据进行分组，带分组标题行 |
| **排序** | `SortDescriptions` 属性 | 支持多字段排序，通过 `ListSortDescription` 配置 |
| **过滤** | `Filter` + `FilterValue` + `FilterValueSelector` | 内置过滤器框架，支持自定义过滤逻辑 |
| **分页** | `TopPagination` / `BottomPagination` + `PageSize` | 集成 `AbstractPagination` 控件实现客户端分页 |
| **空状态指示** | `EmptyIndicator` + `EmptyIndicatorTemplate` | 数据为空时自动展示 Empty 控件 |
| **加载状态覆盖** | `IsOperating` + `OperatingMsg` + `Spin` | 异步操作时展示 Spin 加载遮罩 |
| **选中指示器** | `IsShowSelectedIndicator` + `SelectedIndicator` | 可选的勾选图标，增强选中态的视觉辨识度 |
| **无边框模式** | `IsBorderless` 属性 | 嵌入其他容器控件时隐藏边框 |
| **过渡动画** | `IsMotionEnabled` + ListViewItem 的 Transitions | 列表项背景色、前景色平滑过渡 |
| **虚拟化上下文** | `IListVirtualizingContextAware` 接口 | 虚拟化滚动时保存和恢复容器状态 |
| **Design Token** | `ListViewToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生，支持主题切换 |

---

## 功能详解

### 选择系统

ListView 自行实现了完整的选择系统（未使用 Avalonia 的 `SelectingItemsControl`），通过 `ListView.Selecting.cs` 分部类管理：

- **选择模式**：通过 `SelectionMode` 属性支持 `Single`（单选）、`Multiple`（多选）、`Toggle`（切换选择）、`AlwaysSelected`（始终有选中项）
- **选择模型**：使用 `ListViewSelectionModel`（内部继承自 `SelectionModel<object?>`），支持批量选择操作
- **全局索引**：在分页模式下，选择基于全局索引（`GlobalIndex`）而非页面内本地索引
- **键盘导航**：支持方向键导航选择、Space/Enter 切换选择、Ctrl+A 全选（多选模式）
- **自动滚动**：`AutoScrollToSelectedItem` 控制选中项变化时是否自动滚动到可视区域
- **文本搜索**：`IsTextSearchEnabled` 启用后，键入文字可快速定位匹配项

### 数据视图（CollectionView）

当设置 `ItemsSource` 时，ListView 自动将数据源包装为 `ListCollectionView`（实现 `IListCollectionView`），提供：

- **透明包装**：如果传入的已是 `IListCollectionView`，直接使用；否则自动创建 `ListCollectionView` 包装
- **过滤描述**：通过 `FilterDescriptions` 管理过滤条件
- **排序描述**：通过 `SortDescriptions` 管理排序规则
- **分组描述**：通过 `GroupDescriptions` 管理分组逻辑
- **分页**：通过 `PageSize` 和 `MoveToPage` 管理客户端分页

### 分组

通过 `IsGroupEnabled = true` 开启分组功能。分组依赖 `GroupPropertySelector` 委托，从每个数据项中提取分组键值。默认情况下，如果数据项实现了 `IGroupHeader` 接口，会使用其 `Group` 属性作为分组键。

分组项在列表中显示为不可选择的标题行，使用 `GroupItemTemplate` 渲染，样式使用较小的字号和描述性颜色（`GroupHeaderColor` Token）。

### 过滤

ListView 内置过滤器框架：

- `Filter`：`IValueFilter` 类型，定义过滤逻辑（如包含匹配、精确匹配等）
- `FilterValue`：过滤关键字，设置后触发过滤
- `FilterValueSelector`：委托，从数据项中提取要过滤的属性值（默认提取 `IListItemData.Content`）

AXAML 中可使用 `{atom:ValueFilterProvider Contains}` 标记扩展快速绑定内置过滤器。

### 分页

ListView 支持顶部/底部分页器集成：

- `TopPagination` / `BottomPagination`：放置 `Pagination` 控件实例
- `PageSize`：每页数据量，设置后自动启用分页
- `PaginationVisibility`：控制分页器显示位置（`None` / `Top` / `Bottom` / `Both`）
- `TopPaginationAlign` / `BottomPaginationAlign`：分页器对齐方式（`Start` / `Center` / `End`）

分页器与 `IListCollectionView` 双向联动：翻页时自动通知数据视图切换页面。

### 加载状态（Operating）

当执行异步操作（如加载数据、保存等）时，可通过 `IsOperating = true` 展示加载状态覆盖层：

- 模板内置 `Spin` 控件作为加载指示器
- `OperatingMsg` 显示加载提示文本
- `CustomOperatingIndicator` / `CustomOperatingIndicatorTemplate` 支持自定义加载指示器

### 空状态

当列表数据为空（`TotalItemCount == 0`）且 `IsShowEmptyIndicator == true` 时，自动显示空状态指示器：

- 默认使用 `Empty` 控件的 Simple 预设图
- 可通过 `EmptyIndicator` / `EmptyIndicatorTemplate` 自定义空状态展示
- `EmptyIndicatorPadding` 控制空状态内边距

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 基础列表 | ✅ `List` + `List.Item` | ✅ `ListView` + `ListViewItem` | ✅ 完全对齐 |
| 三种尺寸 `size` | ✅ `large / default / small` | ✅ `SizeType` 属性 | ✅ 完全对齐 |
| 边框控制 `bordered` | ✅ 布尔属性 | ✅ `IsBorderless` 属性（语义相反） | ✅ 对齐 |
| 数据源 `dataSource` | ✅ 数组 | ✅ `ItemsSource` + `IListCollectionView` | ✅ 对齐（增强） |
| 渲染项 `renderItem` | ✅ 函数 | ✅ `ItemTemplate` 数据模板 | ✅ 对齐（方式不同，语义一致） |
| 分页 `pagination` | ✅ 对象配置 | ✅ `TopPagination` / `BottomPagination` | ✅ 对齐 |
| 加载状态 `loading` | ✅ 布尔/对象 | ✅ `IsOperating` + `Spin` | ✅ 对齐 |
| 空状态 `locale.emptyText` | ✅ ReactNode | ✅ `EmptyIndicator` + `EmptyIndicatorTemplate` | ✅ 对齐 |
| 分组 | ⚠️ 无内置（需手动实现） | ✅ `IsGroupEnabled` + `GroupPropertySelector` | ✅ 增强 |
| 排序 | ⚠️ 无内置 | ✅ `SortDescriptions` | ✅ 增强 |
| 过滤 | ⚠️ 无内置 | ✅ `Filter` + `FilterValue` | ✅ 增强 |
| Grid 栅格布局 | ✅ `grid` 属性 | ❌ 暂未支持 | ⚠️ 待支持 |
| 操作列 `actions` / `extra` | ✅ List.Item 属性 | ❌ 通过 ItemTemplate 自定义实现 | ⚠️ 通过模板覆盖 |
| 头部/脚部 `header` / `footer` | ✅ ReactNode | ❌ 暂未支持 | ⚠️ 待支持 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── AtomUI.Desktop.Controls.ListView
              ├── implements ISizeTypeAware
              ├── implements IMotionAwareControl
              └── implements IListVirtualizingContextAware
```

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ItemsControl` | 项目集合管理、容器创建/回收、`ItemTemplate` 模板化呈现、`ItemsPanel` 面板布局、虚拟化基础设施 |
| `AtomUI.Desktop.Controls.ListView` | 完整选择系统、Ant Design 视觉体系（三种尺寸）、数据视图（排序/过滤/分组/分页）、Design Token 集成、空状态指示、加载状态覆盖、选中指示器、虚拟化上下文管理 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `ISizeTypeAware` | `AtomUI.Controls.Shared` | 支持 `SizeType`（Small / Middle / Large）三种尺寸切换 |
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持动画开关 |
| `IListVirtualizingContextAware` | `AtomUI.Controls` | 虚拟化滚动时保存和恢复容器状态 |

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类（主文件） | `src/AtomUI.Desktop.Controls/ListView/ListView.cs` | 属性定义、容器管理、核心逻辑 |
| 控件类（选择） | `src/AtomUI.Desktop.Controls/ListView/ListView.Selecting.cs` | 选择系统实现 |
| 控件类（分页） | `src/AtomUI.Desktop.Controls/ListView/ListView.Pagination.cs` | 分页逻辑 |
| 控件类（虚拟化） | `src/AtomUI.Desktop.Controls/ListView/ListView.Virtualizing.cs` | 虚拟化上下文管理 |
| 选择模型 | `src/AtomUI.Desktop.Controls/ListView/ListViewSelectionModel.cs` | 内部选择模型实现 |
| 列表项 | `src/AtomUI.Desktop.Controls/ListView/ListViewItem.cs` | 列表项容器控件 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/ListView/ListPseudoClass.cs` | 伪类定义 |
| 枚举 | `src/AtomUI.Desktop.Controls/ListView/ListPaginationVisibility.cs` | 分页可见性枚举 |
| Token 定义 | `src/AtomUI.Desktop.Controls/ListView/ListViewToken.cs` | 组件级 Design Token |
| 主题模板 | `src/AtomUI.Desktop.Controls/ListView/Themes/ListViewTheme.axaml` | ListView ControlTheme AXAML |
| 列表项主题 | `src/AtomUI.Desktop.Controls/ListView/Themes/ListViewItemTheme.axaml` | ListViewItem ControlTheme AXAML |
| 模板常量 | `src/AtomUI.Desktop.Controls/ListView/Themes/ListViewThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ListShowCase.axaml` | 使用范例 |

---

## 模板结构

### ListView 模板

```
Border#Frame                                     ← 主框架（背景、边框、圆角、内边距）
└── DockPanel
    ├── ContentPresenter#TopPaginationPresenter   ← 顶部分页器（DockPanel.Dock="Top"）
    ├── ContentPresenter#BottomPaginationPresenter← 底部分页器（DockPanel.Dock="Bottom"）
    └── Spin                                     ← 加载状态覆盖层
        └── Panel
            ├── ScrollViewer (PART_ScrollViewer)  ← 滚动容器（数据非空时可见）
            │   └── ItemsPresenter               ← 项目呈现器（使用 VirtualizingStackPanel）
            └── ContentPresenter#EmptyIndicator   ← 空状态指示器（数据为空时可见）
```

### ListViewItem 模板

```
Border#Frame                                ← 项目框架（背景、圆角、内边距）
└── DockPanel
    ├── IconTemplatePresenter#SelectedIndicator  ← 选中指示器（DockPanel.Dock="Right"，选中时可见）
    └── ContentPresenter#ContentPresenter        ← 内容展示器
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `ListViewThemeConstants.ScrollViewerPart` | `"PART_ScrollViewer"` | 滚动容器 |
| `ListViewThemeConstants.TopPaginationPart` | `"PART_TopPagination"` | 顶部分页器 |
| `ListViewThemeConstants.BottomPaginationPart` | `"PART_BottomPagination"` | 底部分页器 |
| `ListViewThemeConstants.ListViewPart` | `"PART_ListView"` | 列表视图 |
