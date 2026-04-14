# TreeView API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### TreeItemHoverMode

节点悬浮高亮模式。

| 值 | 说明 |
|---|---|
| `Default` | 仅高亮节点文字区域（默认） |
| `Block` | 高亮整个节点行（包含缩进区域） |
| `WholeLine` | 高亮整行（从容器左到右） |

### TreeFilterHighlightStrategy（Flags 枚举）

搜索过滤高亮策略，支持组合使用。

| 值 | 说明 |
|---|---|
| `HighlightedMatch` (0x01) | 高亮匹配文本 |
| `HighlightedWhole` (0x02) | 高亮整个匹配节点 |
| `BoldedMatch` (0x04) | 加粗匹配文本 |
| `ExpandPath` (0x08) | 自动展开匹配节点的祖先路径 |
| `HideUnMatched` (0x10) | 隐藏未匹配的节点 |
| `All` | 以上全部策略组合（默认值） |

### ItemToggleType（来自 `AtomUI.Controls`）

节点勾选控件类型。

| 值 | 说明 |
|---|---|
| `None` | 不显示勾选控件（默认） |
| `CheckBox` | 显示 CheckBox 复选框 |
| `Radio` | 显示 Radio 单选按钮 |

---

## TreeView 公共属性

### 核心属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsAutoExpandParent` | `bool` | `true` | 是否自动展开父节点 |
| `IsDraggable` | `bool` | `false` | 是否启用拖拽排序 |
| `IsShowIcon` | `bool` | `false` | 是否显示节点图标 |
| `IsShowLine` | `bool` | `false` | 是否显示节点间连接线 |
| `IsDefaultExpandAll` | `bool` | `false` | 是否默认展开所有节点 |
| `IsShowLeafIcon` | `bool` | `false` | 是否显示叶子节点图标 |
| `IsSwitcherRotation` | `bool` | `true` | 展开/折叠图标是否使用旋转动画（`false` 时使用替换图标） |
| `IsSelectable` | `bool` | `true` | 节点是否可选中 |
| `IsCheckStrictly` | `bool` | `false` | 是否严格模式（`true` 时关闭父子联动勾选） |
| `NodeHoverMode` | `TreeItemHoverMode` | `Default` | 节点悬浮高亮模式 |
| `ToggleType` | `ItemToggleType` | `None` | 节点勾选控件类型（None/CheckBox/Radio） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用展开/折叠动画（共享属性） |

### 自定义图标属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `SwitcherExpandIcon` | `IconTemplate?` | `null` | 自定义展开图标模板 |
| `SwitcherCollapseIcon` | `IconTemplate?` | `null` | 自定义折叠图标模板 |
| `SwitcherRotationIcon` | `IconTemplate?` | `null` | 自定义旋转图标模板（`IsSwitcherRotation=True` 时生效） |
| `SwitcherLoadingIcon` | `IconTemplate?` | `null` | 自定义加载中图标模板 |
| `SwitcherLeafIcon` | `IconTemplate?` | `null` | 自定义叶子节点图标模板 |

### 默认路径属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `DefaultExpandedPaths` | `IList<TreeNodePath>?` | `null` | 默认展开的节点路径列表 |
| `DefaultSelectedPaths` | `IList<TreeNodePath>?` | `null` | 默认选中的节点路径列表 |
| `DefaultCheckedPaths` | `IList<TreeNodePath>?` | `null` | 默认勾选的节点路径列表 |

### 异步加载属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `DataLoader` | `ITreeItemNodeLoader?` | `null` | 异步数据加载器 |

### 搜索过滤属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Filter` | `ITreeItemFilter?` | `DefaultTreeItemFilter` | 自定义过滤器 |
| `FilterValue` | `object?` | `null` | 过滤关键词 |
| `FilterHighlightStrategy` | `TreeFilterHighlightStrategy` | `All` | 过滤高亮策略 |
| `FilterResultCount` | `int` | `0` | 过滤匹配结果数量（只读） |
| `FilterHighlightForeground` | `IBrush?` | `null` | 过滤高亮前景色（默认使用 Token 中的 `FilterHighlightColor`） |

### 动画属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `OpenMotion` | `AbstractMotion?` | `ExpandMotion` | 展开动画 |
| `CloseMotion` | `AbstractMotion?` | `CollapseMotion` | 折叠动画 |

### 空数据指示器属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `EmptyIndicator` | `object?` | `null` | 空数据指示器内容 |
| `EmptyIndicatorTemplate` | `IDataTemplate?` | `null` | 空数据指示器模板 |
| `IsShowEmptyIndicator` | `bool` | `true` | 是否在无数据时显示空状态指示器 |
| `EmptyIndicatorPadding` | `Thickness` | `default` | 空数据指示器内边距 |

### 勾选相关属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `CheckedItems` | `IList` | `AvaloniaList<object>` | 已勾选项集合（可读写） |

### 继承自 Avalonia.Controls.TreeView 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Items` | `ItemCollection` | — | 直接添加子节点 |
| `ItemsSource` | `IEnumerable?` | `null` | 数据绑定源 |
| `ItemTemplate` | `IDataTemplate?` | `null` | 项数据模板（推荐 `TreeDataTemplate`） |
| `SelectedItem` | `object?` | `null` | 当前选中项 |
| `SelectedItems` | `IList?` | — | 选中项集合（多选） |
| `SelectionMode` | `SelectionMode` | `Single` | 选择模式 |

---

## TreeView 公共方法

| 方法名 | 签名 | 说明 |
|---|---|---|
| `ExpandAll` | `void ExpandAll(bool? motionEnabled = null)` | 展开所有节点。可选参数指定是否启用动画 |
| `CollapseAll` | `void CollapseAll(bool? motionEnabled = null)` | 折叠所有节点。可选参数指定是否启用动画 |
| `CheckedSubTree` | `void CheckedSubTree(TreeViewItem viewItem)` | 勾选指定节点及其所有子节点 |
| `UnCheckedSubTree` | `void UnCheckedSubTree(TreeViewItem viewItem)` | 取消勾选指定节点及其所有子节点 |
| `FilterTreeNode` | `void FilterTreeNode()` | 手动触发树节点过滤 |

---

## TreeView 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `CheckedItemsChanged` | `EventHandler<TreeViewCheckedItemsChangedEventArgs>` | 勾选项发生变化时触发 |
| `TreeItemLoaded` | `EventHandler<TreeViewItemLoadedEventArgs>` | 异步加载节点完成时触发 |
| `ItemExpanded` | `EventHandler<TreeItemExpandedEventArgs>` | 节点展开时触发 |
| `ItemCollapsed` | `EventHandler<TreeItemCollapsedEventArgs>` | 节点折叠时触发 |
| `ItemClicked` | `EventHandler<TreeItemClickedEventArgs>` | 节点被点击时触发 |
| `ItemContextMenuRequest` | `EventHandler<TreeItemContextMenuEventArgs>` | 节点右键菜单请求时触发 |
| `ItemDragStarted` | `EventHandler<TreeViewDragStartedEventArgs>` | 拖拽开始时触发 |
| `ItemDragCompleted` | `EventHandler<TreeViewDragCompletedEventArgs>` | 拖拽完成时触发 |
| `ItemDragEnter` | `EventHandler<TreeViewDragEnterEventArgs>` | 拖拽进入节点时触发 |
| `ItemDragLeave` | `EventHandler<TreeViewDragLeaveEventArgs>` | 拖拽离开节点时触发 |
| `ItemDragOver` | `EventHandler<TreeViewDragOverEventArgs>` | 拖拽经过节点时触发 |
| `ItemDropped` | `EventHandler<TreeViewDroppedEventArgs>` | 拖拽放下时触发 |

### 事件参数说明

**TreeViewCheckedItemsChangedEventArgs**：

| 属性 | 类型 | 说明 |
|---|---|---|
| `AddedItems` | `IList` | 新增的勾选项 |
| `RemovedItems` | `IList` | 移除的勾选项 |

**TreeViewDragStartedEventArgs / TreeViewDragCompletedEventArgs**：

| 属性 | 类型 | 说明 |
|---|---|---|
| `ViewItem` | `TreeViewItem` | 被拖拽的节点 |

**TreeViewDragEnterEventArgs / TreeViewDragLeaveEventArgs / TreeViewDragOverEventArgs**：

| 属性 | 类型 | 说明 |
|---|---|---|
| `DraggedViewItem` | `TreeViewItem` | 被拖拽的节点 |
| `DragOverViewItem` | `TreeViewItem` | 经过的目标节点 |

**TreeViewDroppedEventArgs**：

| 属性 | 类型 | 说明 |
|---|---|---|
| `DraggedViewItem` | `TreeViewItem` | 被拖拽的节点 |
| `DroppedItem` | `TreeViewItem?` | 放下目标节点 |
| `DropIndex` | `int` | 插入位置索引 |

**TreeItemExpandedEventArgs / TreeItemCollapsedEventArgs / TreeItemClickedEventArgs**：

| 属性 | 类型 | 说明 |
|---|---|---|
| `ViewItem` | `TreeViewItem` | 相关的节点 |

---

## TreeView 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:draggable` | `StdPseudoClass.Draggable` | `IsDraggable == true` |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |
| `:focus` | 获得焦点 |

---

## TreeViewItem 公共属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Icon` | `PathIcon?` | `null` | 节点自定义图标 |
| `IsChecked` | `bool?` | `false` | 三态勾选状态（`true` / `false` / `null` 表示半选） |
| `IsLeaf` | `bool` | 自动推断 | 是否为叶子节点（只读，由子节点数和 DataLoader 决定） |
| `IsLoading` | `bool` | `false` | 是否处于异步加载中 |
| `Value` | `object?` | `null` | 节点绑定值 |
| `GroupName` | `string?` | `null` | Radio 模式下的分组名称 |
| `IsIndicatorEnabled` | `bool` | `true` | 是否启用勾选指示器（`false` 时节点不可勾选） |
| `ItemKey` | `EntityKey?` | `null` | 节点唯一标识键 |

### 继承自 Avalonia.Controls.TreeViewItem 的常用属性

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Header` | `object?` | `null` | 节点标题内容 |
| `HeaderTemplate` | `IDataTemplate?` | `null` | 标题模板 |
| `IsExpanded` | `bool` | `false` | 是否展开 |
| `IsSelected` | `bool` | `false` | 是否选中 |
| `Items` | `ItemCollection` | — | 子节点集合 |
| `Level` | `int` | 自动计算 | 节点层级（只读） |
| `IsEnabled` | `bool` | `true` | 是否启用 |

---

## TreeViewItem 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Click` | `EventHandler<RoutedEventArgs>` | 节点被点击时触发（路由事件，冒泡） |
| `ContextMenuRequest` | `EventHandler<RoutedEventArgs>` | 右键菜单请求事件（路由事件，冒泡+隧道） |

---

## TreeViewItem 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:toggle` | `TreeViewPseudoClass.NodeToggleTypeCheckBox` | `ToggleType == CheckBox` |
| `:radio` | `TreeViewPseudoClass.NodeToggleTypeRadio` | `ToggleType == Radio` |
| `:checked` | `StdPseudoClass.Checked` | `IsChecked == true` |

### 继承的标准伪类

| 伪类 | 触发条件 |
|---|---|
| `:selected` | `IsSelected == true` |
| `:expanded` | `IsExpanded == true` |
| `:pointerover` | 鼠标悬浮 |
| `:disabled` | `IsEnabled == false` |

---

## 关联数据类型

### TreeItemNode（record）

`ITreeItemNode` 的默认实现，推荐用于 `ItemsSource` 数据绑定。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Header` | `object?` | `null` | 节点标题 |
| `ItemKey` | `EntityKey?` | `null` | 节点唯一标识 |
| `Icon` | `PathIcon?` | `null` | 节点图标 |
| `IsEnabled` | `bool` | `true` | 是否启用 |
| `IsChecked` | `bool?` | `false` | 勾选状态 |
| `IsSelected` | `bool` | `false` | 选中状态 |
| `IsExpanded` | `bool` | `false` | 展开状态 |
| `IsIndicatorEnabled` | `bool` | `true` | 是否可勾选 |
| `GroupName` | `string?` | `null` | Radio 分组名 |
| `IsLeaf` | `bool` | `false` | 是否叶子节点 |
| `Value` | `object?` | `null` | 节点值 |
| `Children` | `IList<ITreeItemNode>` | `[]` | 子节点列表 |
| `ParentNode` | `ITreeNode<ITreeItemNode>?` | 自动设置 | 父节点引用（只读） |

### TreeNodePath

节点路径，用于 `DefaultExpandedPaths` / `DefaultSelectedPaths` / `DefaultCheckedPaths`。

```csharp
// 路径格式：使用 "/" 分隔的 ItemKey 值
var path = new TreeNodePath("0-0/0-0-0/0-0-0-1");
```

### TreeItemLoadResult

异步加载结果模型。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `IsSuccess` | `bool` | `true` | 是否加载成功 |
| `StatusCode` | `RpcStatusCode` | `Unknown` | 状态码 |
| `UserFriendlyMessage` | `string?` | `null` | 用户友好的错误消息 |
| `Data` | `IReadOnlyList<ITreeItemNode>?` | `null` | 加载的子节点数据 |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

控制展开/折叠动画是否启用。

### IFormItemAware

可作为 `FormItem` 的子控件参与表单验证流程。选择模式为 `Multiple` 时，表单值为 `SelectedItems`；否则为 `SelectedItem`。
