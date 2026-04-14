# TreeView 使用文档

本文档介绍 AtomUI TreeView 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 TreeView，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // TreeView, TreeViewItem, TreeItemNode 等
using AtomUI.Controls;            // ItemToggleType, TreeNodePath, EntityKey 等共享类型
```

---

## 1. 基本用法（直接声明节点）

最简单的方式是在 AXAML 中直接声明 `TreeViewItem` 节点：

```xml
<atom:TreeView ToggleType="CheckBox">
    <atom:TreeViewItem Header="parent 1" ItemKey="0-0">
        <atom:TreeViewItem Header="parent 1-0" ItemKey="0-0-0">
            <atom:TreeViewItem Header="leaf" ItemKey="0-0-0-0" IsEnabled="False" />
            <atom:TreeViewItem Header="leaf" ItemKey="0-0-0-1" />
        </atom:TreeViewItem>
        <atom:TreeViewItem Header="parent 1-1" ItemKey="0-0-1">
            <atom:TreeViewItem Header="sss" ItemKey="0-0-1-0">
                <atom:TreeViewItem Header="ccc" ItemKey="0-0-1-0-0" />
            </atom:TreeViewItem>
        </atom:TreeViewItem>
    </atom:TreeViewItem>
</atom:TreeView>
```

**要点**：
- `ToggleType="CheckBox"` 启用复选框勾选功能
- `ItemKey` 用于唯一标识节点，配合 `DefaultExpandedPaths` 等路径功能使用
- `IsEnabled="False"` 可禁用特定节点

> 📖 参考：Gallery 中 "Basic" 示例。

---

## 2. 通过数据模板生成（ItemsSource + TreeDataTemplate）

推荐的 MVVM 数据驱动方式，使用 `ItemsSource` 绑定数据：

```xml
<atom:TreeView Name="BasicTplTree" ToggleType="CheckBox">
    <atom:TreeView.ItemTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:TreeView.ItemTemplate>
</atom:TreeView>
```

```csharp
// Code-behind
BasicTplTree.ItemsSource = new List<TreeItemNode>
{
    new TreeItemNode()
    {
        Header = "parent 1",
        ItemKey = "0-0",
        Children = [
            new TreeItemNode()
            {
                Header = "parent 1-0",
                ItemKey = "0-0-0",
                Children = [
                    new TreeItemNode() { Header = "leaf 1", ItemKey = "0-0-0-0", IsEnabled = false },
                    new TreeItemNode() { Header = "leaf 2", ItemKey = "0-0-0-1" }
                ]
            },
            new TreeItemNode()
            {
                Header = "parent 1-1",
                ItemKey = "0-0-1",
                Children = [
                    new TreeItemNode() { Header = "sss", ItemKey = "0-0-1-0" }
                ]
            }
        ]
    }
};
```

**要点**：
- `TreeDataTemplate` 的 `ItemsSource` 绑定到 `Children` 属性实现递归层级展示
- `TreeItemNode` 是 `ITreeItemNode` 的默认实现，支持 `Header`、`ItemKey`、`Children`、`Icon` 等属性
- 使用 ReactiveUI 绑定：`this.OneWayBind(ViewModel, vm => vm.TreeNodes, v => v.BasicTplTree.ItemsSource)`

> 📖 参考：Gallery 中 "Generate by template" 示例。

---

## 3. Block 悬浮模式

设置 `NodeHoverMode="Block"` 使节点悬浮效果扩展到整行：

```xml
<atom:TreeView ToggleType="CheckBox"
               IsDefaultExpandAll="True"
               NodeHoverMode="Block">
    <atom:TreeViewItem Header="parent">
        <atom:TreeViewItem Header="child 1" IsEnabled="False" />
        <atom:TreeViewItem Header="child 2" IsIndicatorEnabled="False" />
    </atom:TreeViewItem>
</atom:TreeView>
```

**要点**：
- `NodeHoverMode="Block"` 高亮整个节点行
- `IsIndicatorEnabled="False"` 禁用该节点的勾选指示器（CheckBox 不显示）

> 📖 参考：Gallery 中 "Block Node" 示例。

---

## 4. 显示连接线和图标

通过 `IsShowLine`、`IsShowIcon`、`IsShowLeafIcon`、`IsSwitcherRotation` 属性控制树形结构的视觉展示：

```xml
<atom:TreeView IsShowLine="{Binding ShowLineSwitchChecked}"
               IsShowIcon="{Binding ShowIconSwitchChecked}"
               IsShowLeafIcon="{Binding ShowLeafIconSwitchChecked}"
               NodeHoverMode="{Binding TreeViewNodeHoverMode}"
               IsSwitcherRotation="{Binding ShowLineSwitchChecked,
                                    Converter={x:Static BoolConverters.Not}}">
    <atom:TreeViewItem Header="parent 1"
                       Icon="{antdicons:AntDesignIconProvider Kind=CarryOutOutlined}"
                       IsExpanded="True">
        <atom:TreeViewItem Header="parent 1-0"
                           Icon="{antdicons:AntDesignIconProvider Kind=CarryOutOutlined}"
                           IsExpanded="True">
            <atom:TreeViewItem Header="leaf 1"
                               Icon="{antdicons:AntDesignIconProvider Kind=CarryOutOutlined}" />
        </atom:TreeViewItem>
    </atom:TreeViewItem>
</atom:TreeView>
```

**要点**：
- `IsShowLine="True"` 启用节点间连接线
- `IsShowIcon="True"` 显示节点图标
- `IsShowLeafIcon="True"` 显示叶子节点图标
- 当启用连接线时，建议将 `IsSwitcherRotation` 设为 `False`，使用展开/折叠两个不同图标

> 📖 参考：Gallery 中 "Tree with line" 示例。

---

## 5. 拖拽排序

设置 `IsDraggable="True"` 启用拖拽功能：

```xml
<atom:TreeView IsDraggable="True" NodeHoverMode="Block">
    <atom:TreeViewItem Header="0-0">
        <atom:TreeViewItem Header="0-0-0">
            <atom:TreeViewItem Header="0-0-0-0" />
            <atom:TreeViewItem Header="0-0-0-1" />
        </atom:TreeViewItem>
        <atom:TreeViewItem Header="0-0-1">
            <atom:TreeViewItem Header="0-0-1-0" />
        </atom:TreeViewItem>
    </atom:TreeViewItem>
    <atom:TreeViewItem Header="0-1">
        <atom:TreeViewItem Header="0-1-0" />
    </atom:TreeViewItem>
</atom:TreeView>
```

**要点**：
- 拖拽时显示预览浮层和位置指示线
- 支持拖到节点上方、下方或内部（作为子节点）
- 不允许将父节点拖入自身子节点中

> 📖 参考：Gallery 中 "draggable" 示例。

---

## 6. 自定义展开/折叠图标

通过 `SwitcherRotationIcon` 或 `SwitcherExpandIcon`/`SwitcherCollapseIcon` 属性自定义图标：

```xml
<atom:TreeView IsSwitcherRotation="True">
    <atom:TreeView.SwitcherRotationIcon>
        <atom:IconTemplate>
            <antdicons:RightOutlined />
        </atom:IconTemplate>
    </atom:TreeView.SwitcherRotationIcon>
    <atom:TreeViewItem Header="parent 1" ItemKey="0-0">
        <atom:TreeViewItem Header="parent 1-0" ItemKey="0-0-0">
            <atom:TreeViewItem Header="leaf" ItemKey="0-0-0-0" />
        </atom:TreeViewItem>
    </atom:TreeViewItem>
</atom:TreeView>
```

> 📖 参考：Gallery 中 "Customize collapse/expand icon" 示例。

---

## 7. 异步加载

通过 `DataLoader` 属性提供异步加载器，实现展开节点时按需加载子节点：

```xml
<atom:TreeView Name="AsyncLoadTree">
    <atom:TreeView.ItemTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:TreeView.ItemTemplate>
</atom:TreeView>
```

```csharp
// 初始化根节点
AsyncLoadTree.ItemsSource = new List<TreeItemNode>
{
    new TreeItemNode() { Header = "Expand to load", ItemKey = "0" },
    new TreeItemNode() { Header = "Expand to load", ItemKey = "1" },
    new TreeItemNode() { Header = "Tree Node", ItemKey = "2", IsLeaf = true }
};

// 设置异步加载器
AsyncLoadTree.DataLoader = new TreeItemDataLoader();
```

```csharp
// 自定义加载器实现 ITreeItemNodeLoader 接口
public class TreeItemDataLoader : ITreeItemNodeLoader
{
    public async Task<TreeItemLoadResult> LoadAsync(ITreeItemNode targetTreeItem, CancellationToken token)
    {
        await Task.Delay(1000, token); // 模拟异步加载
        return new TreeItemLoadResult
        {
            IsSuccess = true,
            Data = new List<ITreeItemNode>
            {
                new TreeItemNode() { Header = "Child 1", ItemKey = "child-1" },
                new TreeItemNode() { Header = "Child 2", ItemKey = "child-2", IsLeaf = true }
            }.AsReadOnly()
        };
    }
}
```

**要点**：
- 必须使用 `ItemsSource` 绑定数据（不支持直接声明 `TreeViewItem` 模式）
- 非叶子节点展开时自动调用 `DataLoader`，加载过程中显示旋转图标
- 设置 `IsLeaf = true` 的节点不会触发加载

> 📖 参考：Gallery 中 "load data asynchronously" 示例。

---

## 8. 搜索过滤

通过设置 `FilterValue` 属性触发树节点搜索过滤：

### 对 ItemsSource 数据源搜索

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:SearchEdit PlaceholderText="Search"
                     SearchButtonClick="HandleFilterItemsSourceTreeClicked" />
    <atom:TreeView Name="SearchTreeViewByItemsSource">
        <atom:TreeView.ItemTemplate>
            <TreeDataTemplate ItemsSource="{Binding Children}" x:DataType="atom:ITreeItemNode">
                <atom:TextBlock Text="{Binding Header}" />
            </TreeDataTemplate>
        </atom:TreeView.ItemTemplate>
    </atom:TreeView>
</StackPanel>
```

### 对直接声明的节点搜索

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:SearchEdit PlaceholderText="Search"
                     SearchButtonClick="HandleFilterTreeClicked" />
    <atom:TreeView Name="SearchTreeView">
        <atom:TreeViewItem Header="0-0">
            <atom:TreeViewItem Header="0-0-0">
                <atom:TreeViewItem Header="0-0-0-0" />
            </atom:TreeViewItem>
        </atom:TreeViewItem>
    </atom:TreeView>
</StackPanel>
```

```csharp
private void HandleFilterTreeClicked(object? sender, RoutedEventArgs e)
{
    if (sender is SearchEdit searchEdit)
    {
        SearchTreeView.FilterValue = searchEdit.Text?.Trim();
    }
}
```

**要点**：
- 默认使用 `DefaultTreeItemFilter`，按节点 `Header` 文本匹配
- `FilterHighlightStrategy` 控制过滤行为（高亮匹配/隐藏未匹配/展开路径等）
- 将 `FilterValue` 设为 `null` 或空字符串可清除过滤

> 📖 参考：Gallery 中 "Searchable" 和 "Searchable by TreeDataTemplate" 示例。

---

## 9. 默认展开、选中和勾选

通过路径属性指定初始状态：

```csharp
// 默认展开路径
myTreeView.DefaultExpandedPaths = new List<TreeNodePath>
{
    new TreeNodePath("0-0/0-0-0"),
    new TreeNodePath("0-0/0-0-1/0-0-1-1")
};

// 默认选中路径
myTreeView.DefaultSelectedPaths = new List<TreeNodePath>
{
    new TreeNodePath("0-0/0-0-1")
};

// 默认勾选路径（需配合 ToggleType="CheckBox"）
myTreeView.DefaultCheckedPaths = new List<TreeNodePath>
{
    new TreeNodePath("0-0/0-0-1/0-0-1-1")
};
```

**路径格式**：使用 `/` 分隔的 `ItemKey` 值，从根节点到目标节点的完整路径。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml.cs` 中 `InitBasicTreeViewData` 方法。

---

## 10. 获取选中和勾选数据

### 获取选中节点

```csharp
// 单选模式
object? selected = myTreeView.SelectedItem;

// 多选模式
IList? selectedItems = myTreeView.SelectedItems;
```

### 获取勾选节点

```csharp
// 获取所有勾选项
IList checkedItems = myTreeView.CheckedItems;
```

### 监听勾选变化

```csharp
myTreeView.CheckedItemsChanged += (sender, args) =>
{
    foreach (var added in args.AddedItems)
    {
        Console.WriteLine($"Checked: {added}");
    }
    foreach (var removed in args.RemovedItems)
    {
        Console.WriteLine($"Unchecked: {removed}");
    }
};
```

---

## 11. 程序化控制

### 展开/折叠所有节点

```csharp
// 展开全部（不带动画）
myTreeView.ExpandAll(motionEnabled: false);

// 展开全部（带动画）
myTreeView.ExpandAll(motionEnabled: true);

// 折叠全部
myTreeView.CollapseAll(motionEnabled: false);
```

### 勾选/取消勾选子树

```csharp
// 勾选指定节点及其所有子节点
if (myTreeView.ContainerFromIndex(0) is TreeViewItem item)
{
    myTreeView.CheckedSubTree(item);
}

// 取消勾选
myTreeView.UnCheckedSubTree(item);
```

---

## 常见组合模式

### 文件浏览器风格

```xml
<atom:TreeView IsShowIcon="True"
               IsShowLine="True"
               IsSwitcherRotation="False"
               NodeHoverMode="WholeLine">
    <!-- 节点... -->
</atom:TreeView>
```

### 权限配置风格

```xml
<atom:TreeView ToggleType="CheckBox"
               IsDefaultExpandAll="True"
               NodeHoverMode="Block">
    <!-- 节点... -->
</atom:TreeView>
```

### 搜索 + 异步加载

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:SearchEdit PlaceholderText="Search"
                     SearchButtonClick="HandleSearch" />
    <atom:TreeView Name="MyTree"
                   DataLoader="{Binding MyDataLoader}">
        <atom:TreeView.ItemTemplate>
            <TreeDataTemplate ItemsSource="{Binding Children}">
                <atom:TextBlock Text="{Binding Header}" />
            </TreeDataTemplate>
        </atom:TreeView.ItemTemplate>
    </atom:TreeView>
</StackPanel>
```
