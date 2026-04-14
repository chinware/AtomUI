# TreeView 树形控件

## 概述

树形控件（TreeView）用于展示具有层级关系的数据结构。它支持节点的展开/折叠、复选框勾选、拖拽排序、搜索过滤、异步加载等功能，是文件浏览器、组织架构、分类目录等场景中最常见的数据展示控件之一。

AtomUI 的 `TreeView` 控件对齐了 [Ant Design 5.0 Tree](https://ant.design/components/tree-cn) 的设计规范，在 .NET / Avalonia 平台上提供与 Ant Design React 版一致的视觉体验和交互行为。

---

## 设计原理

### Ant Design 的树形控件设计哲学

Ant Design 对树形控件的定位是：**「展示具有层级关系的信息，可支持展开收起、复选等交互」**。树形控件承载两类核心需求：

1. **数据展示**：将具有父子关系的数据以可视化的层级结构呈现，帮助用户理解数据的组织方式。
2. **交互操作**：支持节点选中（单选/多选）、勾选（Checkbox）、展开/折叠、拖拽重排、搜索过滤等丰富的交互模式。

**核心功能特性**：

| 功能 | 设计意图 | 典型场景 |
|---|---|---|
| 📁 **展开/折叠** | 按需展示层级数据，减少视觉负担 | 文件目录、组织架构 |
| ☑️ **复选框（Checkable）** | 对树节点进行批量勾选，支持父子联动 | 权限配置、分类筛选 |
| 🔘 **单选按钮（Radio）** | 对同一层级节点进行单选 | 分组选择 |
| 🖱️ **拖拽排序（Draggable）** | 通过拖放调整节点位置和层级关系 | 菜单编排、目录管理 |
| 🔍 **搜索过滤** | 快速定位匹配节点并高亮显示 | 大量节点中快速查找 |
| ⏳ **异步加载** | 展开节点时按需加载子节点数据 | 大数据量、远程数据源 |
| ➖ **连接线（ShowLine）** | 用连接线展示节点间的层级关系 | 增强视觉层级感 |
| 🎨 **自定义图标** | 为每个节点设置自定义图标 | 文件类型区分、状态标识 |

**三种悬浮高亮模式**：

| 模式 | 设计意图 |
|---|---|
| `Default` | 仅高亮节点文字区域（默认模式） |
| `Block` | 高亮整个节点行（包含缩进区域），增强视觉反馈 |
| `WholeLine` | 高亮整行（从容器左边缘到右边缘），适合目录风格 |

### Avalonia TreeView 基础能力

AtomUI 的 `TreeView` 继承自 Avalonia 框架的 `Avalonia.Controls.TreeView`。理解 Avalonia TreeView 的基础能力有助于理解 AtomUI 在其之上做了哪些扩展。

**Avalonia TreeView 的核心职责：**

Avalonia 的 `TreeView` 是一个 `ItemsControl` 的子类，用于以树形结构展示层级数据。其继承链为：

```
Control → TemplatedControl → ItemsControl → SelectingItemsControl → TreeView
```

作为 `ItemsControl`，TreeView 可以通过 `Items` 集合直接添加 `TreeViewItem`，也可以通过 `ItemsSource` + `ItemTemplate` / `TreeDataTemplate` 进行数据绑定。Avalonia TreeView 提供了基础的选择模型（`SelectedItem` / `SelectedItems` / `SelectionMode`）以及节点容器化管理。

**Avalonia TreeView 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Items` | 直接添加 `TreeViewItem` 子节点 |
| `ItemsSource` | 数据绑定源 |
| `ItemTemplate` | 项模板（`TreeDataTemplate` 支持层级绑定） |
| `SelectedItem` | 当前选中项 |
| `SelectedItems` | 选中项集合（多选模式） |
| `SelectionMode` | 选择模式（`Single` / `Multiple` / `Toggle`） |

**Avalonia TreeViewItem 提供的基础属性：**

| 属性 | 说明 |
|---|---|
| `Header` | 节点标题内容 |
| `HeaderTemplate` | 标题模板 |
| `IsExpanded` | 节点是否展开 |
| `IsSelected` | 节点是否选中 |
| `Level` | 节点层级（只读，自动计算） |
| `Items` | 子节点集合 |

### AtomUI 的扩展设计

AtomUI `TreeView` 在 Avalonia TreeView 的基础上，遵循 Ant Design 5.0 规范做了以下扩展：

| 扩展能力 | 实现方式 | 设计动机 |
|---|---|---|
| **复选框勾选** | `ToggleType` 枚举 + `IsChecked` 三态属性 | 对齐 Ant Design 的 `checkable`，支持 CheckBox 和 Radio 两种模式 |
| **父子联动** | `IsCheckStrictly` 属性 + 递归勾选算法 | 勾选父节点自动勾选所有子节点，取消时反之 |
| **拖拽排序** | `IsDraggable` 属性 + 拖拽指示线渲染 | 对齐 Ant Design 的 `draggable`，支持拖放重排节点 |
| **搜索过滤** | `Filter` + `FilterValue` + `FilterHighlightStrategy` | 对齐 Ant Design 的搜索功能，支持高亮匹配、隐藏未匹配等策略 |
| **异步加载** | `DataLoader` + `ITreeItemNodeLoader` 接口 | 对齐 Ant Design 的 `loadData`，展开时按需加载子节点 |
| **连接线** | `IsShowLine` 属性 + `OnRender` 自绘 | 对齐 Ant Design 的 `showLine`，绘制节点间连接线 |
| **悬浮模式** | `NodeHoverMode` 三种模式 | 不同场景下的悬浮高亮效果 |
| **节点图标** | `IsShowIcon` / `IsShowLeafIcon` + `PathIcon` | 对齐 Ant Design 的 `showIcon`，支持自定义节点图标 |
| **自定义开关图标** | `SwitcherExpandIcon` / `SwitcherCollapseIcon` 等 | 对齐 Ant Design 的 `switcherIcon`，支持替换展开/折叠图标 |
| **展开/折叠动画** | `IsMotionEnabled` + `ExpandMotion` / `CollapseMotion` | 展开/折叠带缓动动画，提升交互体验 |
| **默认路径** | `DefaultExpandedPaths` / `DefaultSelectedPaths` / `DefaultCheckedPaths` | 对齐 Ant Design 的 `defaultExpandedKeys` 等，通过路径指定初始状态 |
| **空数据提示** | `EmptyIndicator` / `IsShowEmptyIndicator` | 当无数据时展示空状态指示器 |
| **表单集成** | `IFormItemAware` 接口 | 可参与 FormItem 验证流程 |
| **Design Token** | `TreeViewToken` + `RegisterTokenResourceScope` | 所有视觉值从全局/组件级 Token 派生 |

---

## 功能详解

### 节点展开与折叠

TreeView 支持多种方式控制节点展开状态：

- **手动展开**：点击节点前的展开/折叠图标（Switcher）
- **全部展开**：设置 `IsDefaultExpandAll="True"` 或调用 `ExpandAll()` 方法
- **全部折叠**：调用 `CollapseAll()` 方法
- **路径展开**：通过 `DefaultExpandedPaths` 指定初始展开路径
- **自动展开父节点**：设置 `IsAutoExpandParent="True"`（默认值），选中子节点时自动展开其父节点

展开/折叠带有缓动动画（`ExpandMotion` / `CollapseMotion`），可通过 `IsMotionEnabled` 属性控制是否启用。

### 复选框勾选（ToggleType）

通过 `ToggleType` 属性启用节点勾选功能：

| ToggleType | 行为 |
|---|---|
| `None` | 不显示勾选控件（默认） |
| `CheckBox` | 显示 CheckBox，支持三态（选中/半选/未选） |
| `Radio` | 显示 Radio，同组内单选 |

**父子联动机制**（`IsCheckStrictly == false` 时，默认行为）：
- 勾选父节点 → 递归勾选所有可勾选子节点
- 取消父节点 → 递归取消所有子节点
- 子节点全部勾选 → 父节点自动变为「选中」状态
- 子节点部分勾选 → 父节点自动变为「半选」（Indeterminate）状态
- 子节点全部取消 → 父节点自动变为「未选」状态

设置 `IsCheckStrictly="True"` 可关闭父子联动，每个节点独立勾选。

### 拖拽排序（IsDraggable）

设置 `IsDraggable="True"` 启用拖拽功能：
- 拖动时显示拖拽预览浮层（`DragPreviewAdorner`）
- 拖拽过程中渲染指示线，指示插入位置（上方/下方/内部）
- 不允许将父节点拖入自身子节点中（防止循环引用）
- 拖拽完成后自动调整节点位置

拖拽提供完整的事件回调：`ItemDragStarted`、`ItemDragEnter`、`ItemDragLeave`、`ItemDragOver`、`ItemDragCompleted`、`ItemDropped`。

### 搜索过滤（Filter）

TreeView 内置搜索过滤功能，通过设置 `FilterValue` 触发过滤：

1. 设置 `Filter` 属性（默认使用 `DefaultTreeItemFilter`，按 `Header` 文本匹配）
2. 设置 `FilterValue` 为搜索关键词
3. 通过 `FilterHighlightStrategy` 控制过滤行为

**过滤高亮策略**（`TreeFilterHighlightStrategy`，可组合）：

| 策略 | 说明 |
|---|---|
| `HighlightedMatch` | 高亮匹配文本 |
| `HighlightedWhole` | 高亮整个匹配节点 |
| `BoldedMatch` | 加粗匹配文本 |
| `ExpandPath` | 自动展开匹配节点的祖先路径 |
| `HideUnMatched` | 隐藏未匹配的节点 |
| `All` | 以上全部（默认） |

### 异步加载（DataLoader）

通过 `DataLoader` 属性设置异步数据加载器：
- 必须使用 `ItemsSource` 绑定数据（不支持手动添加 `TreeViewItem` 模式）
- 展开非叶子节点时自动调用 `ITreeItemNodeLoader.LoadAsync()`
- 加载过程中节点显示旋转 Loading 图标
- 设置 `TreeItemNode.IsLeaf = true` 的节点不会触发加载

### 连接线（ShowLine）

设置 `IsShowLine="True"` 启用节点间连接线：
- 连接线通过 `TreeViewItem.Render()` 自绘，使用 `BorderBrush` 颜色
- `IsSwitcherRotation` 控制展开/折叠图标是旋转动画还是替换图标
- 当 `IsShowLine="True"` 时建议设置 `IsSwitcherRotation="False"`，使用展开/折叠两个不同图标

### 空数据指示器

当 TreeView 无数据（`Items` 为空或过滤无结果）时：
- `IsShowEmptyIndicator="True"`（默认启用）自动显示空状态指示器
- 可通过 `EmptyIndicator` / `EmptyIndicatorTemplate` 自定义空状态内容

---

## 与 Ant Design 规范的对齐程度

| 特性 | Ant Design (React) | AtomUI (Avalonia) | 对齐情况 |
|---|---|---|---|
| 展开/折叠 | ✅ 基础能力 | ✅ `IsExpanded` + 动画 | ✅ 完全对齐 |
| 复选框 `checkable` | ✅ `checkable` 属性 | ✅ `ToggleType=CheckBox` | ✅ 完全对齐 |
| 父子联动 `checkStrictly` | ✅ `checkStrictly` | ✅ `IsCheckStrictly` | ✅ 完全对齐 |
| 拖拽 `draggable` | ✅ `draggable` | ✅ `IsDraggable` | ✅ 完全对齐 |
| 连接线 `showLine` | ✅ `showLine` | ✅ `IsShowLine` | ✅ 完全对齐 |
| 节点图标 `showIcon` | ✅ `showIcon` | ✅ `IsShowIcon` | ✅ 完全对齐 |
| 默认展开 `defaultExpandAll` | ✅ `defaultExpandAll` | ✅ `IsDefaultExpandAll` | ✅ 完全对齐 |
| 默认展开键 `defaultExpandedKeys` | ✅ 按 key 指定 | ✅ `DefaultExpandedPaths` | ✅ 对齐（使用路径而非 key） |
| 默认选中键 `defaultSelectedKeys` | ✅ 按 key 指定 | ✅ `DefaultSelectedPaths` | ✅ 对齐（使用路径而非 key） |
| 默认勾选键 `defaultCheckedKeys` | ✅ 按 key 指定 | ✅ `DefaultCheckedPaths` | ✅ 对齐（使用路径而非 key） |
| 异步加载 `loadData` | ✅ 回调函数 | ✅ `DataLoader` + `ITreeItemNodeLoader` | ✅ 对齐（接口模式） |
| 搜索过滤 | ✅ `filterTreeNode` | ✅ `Filter` + `FilterValue` | ✅ 完全对齐 |
| 自定义开关图标 `switcherIcon` | ✅ ReactNode | ✅ `SwitcherExpandIcon` 等 | ✅ 对齐 |
| Block 节点 `blockNode` | ✅ `blockNode` | ✅ `NodeHoverMode=Block` | ✅ 对齐 |
| 虚拟滚动 `virtual` | ✅ 虚拟列表 | ❌ 暂未支持 | ⚠️ 待支持 |
| 目录模式 `Tree.DirectoryTree` | ✅ 独立组件 | ❌ 暂未提供 | ⚠️ 可通过样式模拟 |

---

## 继承关系

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ItemsControl
        └── Avalonia.Controls.SelectingItemsControl
              └── Avalonia.Controls.TreeView
                    └── AtomUI.Desktop.Controls.TreeView
                          ├── implements IMotionAwareControl
                          └── implements IFormItemAware
```

`TreeView` 通过 `using AvaloniaTreeView = Avalonia.Controls.TreeView;` 别名引用 Avalonia 原生 TreeView，避免类名冲突。

**各层级职责划分：**

| 层级 | 提供的能力 |
|---|---|
| `ItemsControl` | Items 集合管理、ItemsSource 数据绑定、ItemTemplate 模板化 |
| `SelectingItemsControl` | 选择模型（SelectedItem / SelectedItems / SelectionMode）、选择事件 |
| `Avalonia.Controls.TreeView` | 树形容器化管理、`TreeViewItem` 容器创建、展开/折叠基础逻辑 |
| `AtomUI.Desktop.Controls.TreeView` | Ant Design 视觉体系（连接线/图标/悬浮模式）、CheckBox/Radio 勾选、拖拽排序、搜索过滤、异步加载、默认路径、Design Token 集成、展开折叠动画、空数据指示器、表单集成 |

**实现的共享接口：**

| 接口 | 定义位置 | 作用 |
|---|---|---|
| `IMotionAwareControl` | `AtomUI.Controls.Shared` | 支持 `IsMotionEnabled` 动画开关 |
| `IFormItemAware` | `AtomUI.Controls.Shared` | 可作为 `FormItem` 的子控件参与表单验证 |

---

## 关联类型

### TreeViewItem

`TreeViewItem` 是 TreeView 的节点容器控件，继承自 `Avalonia.Controls.TreeViewItem`，实现了 `IRadioButton` 和 `ITreeItemNode` 接口。

```
Avalonia.Controls.Primitives.TemplatedControl
  └── Avalonia.Controls.ContentControl (HeaderedItemsControl)
        └── Avalonia.Controls.TreeViewItem
              └── AtomUI.Desktop.Controls.TreeViewItem
                    ├── implements IRadioButton
                    └── implements ITreeItemNode
```

每个 TreeViewItem 代表树中的一个节点，支持：
- 标题内容（`Header`）
- 图标（`Icon`）
- 三态勾选（`IsChecked`）
- 叶子节点标识（`IsLeaf`）
- 加载状态（`IsLoading`）
- 节点值（`Value`）
- 指示器启用（`IsIndicatorEnabled`）
- 节点唯一标识（`ItemKey`）

### ITreeItemNode

树节点数据接口，用于数据绑定模式下描述节点数据。继承自 `ITreeNode<ITreeItemNode>`。

```csharp
public interface ITreeItemNode : ITreeNode<ITreeItemNode>
{
    bool? IsChecked { get; set; }
    bool IsSelected { get; set; }
    bool IsExpanded { get; set; }
    bool IsIndicatorEnabled { get; set; }
    string? GroupName { get; }
    bool IsLeaf { get; }
    object? Value { get; set; }
    bool IsEnabled { get; set; }
    void UpdateParentNode(ITreeItemNode? parentNode);
}
```

### TreeItemNode

`ITreeItemNode` 的默认实现（`record` 类型），支持声明式初始化和父子关系自动维护。当向 `Children` 集合添加子节点时，会自动设置 `ParentNode`。

### ITreeItemNodeLoader

异步数据加载器接口，用于展开节点时按需加载子节点数据。

```csharp
public interface ITreeItemNodeLoader
{
    Task<TreeItemLoadResult> LoadAsync(ITreeItemNode targetTreeItem, CancellationToken token);
}
```

### ITreeItemFilter

自定义过滤器接口，用于控制树节点的搜索匹配逻辑。

```csharp
public interface ITreeItemFilter
{
    bool Filter(TreeView treeView, TreeViewItem treeViewItem, object? filterValue);
}
```

### FloatableTreeView

`TreeView` 的子类，额外支持 `TreeViewFlyout` 弹出层集成，用于下拉树选择器（如 `TreeSelect`）等场景。提供 `IsOpen`、`Opened`、`Closed` 等弹出状态管理。

---

## 源码位置

| 层级 | 文件路径 | 说明 |
|---|---|---|
| 控件类（主文件） | `src/AtomUI.Desktop.Controls/TreeView/TreeView.cs` | TreeView 主类 |
| 控件类（过滤） | `src/AtomUI.Desktop.Controls/TreeView/TreeView.Filter.cs` | 搜索过滤相关逻辑 |
| 控件类（异步加载） | `src/AtomUI.Desktop.Controls/TreeView/TreeView.AsyncItemDataLoad.cs` | 异步加载逻辑 |
| 控件类（拖拽） | `src/AtomUI.Desktop.Controls/TreeView/TreeView.DragAndDrop.cs` | 拖拽排序逻辑 |
| 节点控件 | `src/AtomUI.Desktop.Controls/TreeView/TreeViewItem.cs` | 树节点容器控件 |
| 节点数据接口 | `src/AtomUI.Desktop.Controls/TreeView/ITreeItemNode.cs` | 节点数据接口定义 |
| 节点数据实现 | `src/AtomUI.Desktop.Controls/TreeView/TreeItemNode.cs` | 节点数据默认实现 |
| 浮动树 | `src/AtomUI.Desktop.Controls/TreeView/FloatableTreeView.cs` | 弹出层集成 |
| 过滤接口 | `src/AtomUI.Desktop.Controls/TreeView/ITreeItemFilter.cs` | 自定义过滤器接口 |
| 伪类常量 | `src/AtomUI.Desktop.Controls/TreeView/TreeViewPseudoClass.cs` | 伪类定义 |
| Token 定义 | `src/AtomUI.Desktop.Controls/TreeView/TreeViewToken.cs` | 组件级 Design Token |
| 异步加载接口 | `src/AtomUI.Desktop.Controls/TreeView/DataLoad/ITreeItemNodeLoader.cs` | 数据加载器接口 |
| 加载结果 | `src/AtomUI.Desktop.Controls/TreeView/DataLoad/TreeItemLoadResult.cs` | 加载结果模型 |
| 事件参数 | `src/AtomUI.Desktop.Controls/TreeView/EventArgs/` | 各类事件参数定义 |
| 主题模板 | `src/AtomUI.Desktop.Controls/TreeView/Themes/TreeViewTheme.axaml` | ControlTheme AXAML |
| 主题代码 | `src/AtomUI.Desktop.Controls/TreeView/Themes/TreeViewTheme.cs` | 主题 code-behind |
| 模板常量 | `src/AtomUI.Desktop.Controls/TreeView/Themes/TreeViewThemeConstants.cs` | 模板部件名称常量 |
| Gallery 示例 | `controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml` | 使用范例 |

---

## 模板结构

TreeView 的模板由三层组成：

### TreeView 模板

```
Border#Frame                                ← 主框架（承载背景、边框）
└── ScrollViewer                            ← 滚动容器
    └── ItemsPresenter                      ← 节点列表
```

### TreeViewItem 模板

```
Panel                                       ← 节点根面板
├── TreeViewItemHeader (Header)             ← 节点标题区域
│   ├── NodeSwitcherButton (PART_NodeSwitcherButton) ← 展开/折叠按钮
│   ├── CheckBox / RadioButton               ← 勾选控件（由 ToggleType 决定）
│   ├── IconPresenter (PART_IconPresenter)   ← 节点图标
│   └── ContentPresenter                     ← 标题内容
└── BaseMotionActor (PART_ItemsPresenterMotionActor) ← 子节点容器（带动画）
    └── ItemsPresenter                       ← 子节点列表
```

### 模板部件常量

| 常量名 | 值 | 说明 |
|---|---|---|
| `TreeViewItemThemeConstants.ItemsPresenterMotionActorPart` | `"PART_ItemsPresenterMotionActor"` | 子节点动画容器 |
| `TreeViewItemHeaderThemeConstants.IconPresenterPart` | `"PART_IconPresenter"` | 图标展示器 |
| `TreeViewItemHeaderThemeConstants.NodeSwitcherButtonPart` | `"PART_NodeSwitcherButton"` | 展开/折叠按钮 |
| `TreeViewItemHeaderThemeConstants.HeaderContentFramePart` | `"PART_HeaderContentFrame"` | 标题内容框架 |
