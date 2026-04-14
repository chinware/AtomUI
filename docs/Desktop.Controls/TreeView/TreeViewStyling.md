# TreeView 自定义样式指南

TreeView 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 TreeView 的公共属性来控制外观和行为：

```xml
<!-- 基本带复选框的树 -->
<atom:TreeView ToggleType="CheckBox">
    <atom:TreeViewItem Header="parent 1" ItemKey="0-0">
        <atom:TreeViewItem Header="child 1" ItemKey="0-0-0" />
        <atom:TreeViewItem Header="child 2" ItemKey="0-0-1" />
    </atom:TreeViewItem>
</atom:TreeView>

<!-- 显示连接线 + 图标 -->
<atom:TreeView IsShowLine="True"
               IsShowIcon="True"
               IsSwitcherRotation="False">
    <atom:TreeViewItem Header="parent 1"
                       Icon="{antdicons:AntDesignIconProvider Kind=CarryOutOutlined}">
        <atom:TreeViewItem Header="leaf"
                           Icon="{antdicons:AntDesignIconProvider Kind=CarryOutOutlined}" />
    </atom:TreeViewItem>
</atom:TreeView>

<!-- Block 悬浮模式 -->
<atom:TreeView NodeHoverMode="Block"
               IsDefaultExpandAll="True"
               ToggleType="CheckBox">
    <atom:TreeViewItem Header="parent">
        <atom:TreeViewItem Header="child 1" IsEnabled="False" />
        <atom:TreeViewItem Header="child 2" IsIndicatorEnabled="False" />
    </atom:TreeViewItem>
</atom:TreeView>

<!-- 拖拽排序 -->
<atom:TreeView IsDraggable="True" NodeHoverMode="Block">
    <atom:TreeViewItem Header="Item 1">
        <atom:TreeViewItem Header="Sub Item 1" />
        <atom:TreeViewItem Header="Sub Item 2" />
    </atom:TreeViewItem>
    <atom:TreeViewItem Header="Item 2" />
</atom:TreeView>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 TreeView 和 TreeViewItem 进行全局或局部样式覆盖：

### 全局设置连接线和展开行为

```xml
<Window.Styles>
    <Style Selector="atom|TreeView">
        <Setter Property="IsShowLine" Value="True" />
        <Setter Property="IsDefaultExpandAll" Value="True" />
    </Style>
</Window.Styles>
```

### 自定义选中节点样式

```xml
<Style Selector="atom|TreeViewItem:selected">
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

### 自定义勾选模式下的节点

```xml
<!-- 勾选模式下禁用的节点降低透明度 -->
<Style Selector="atom|TreeViewItem:toggle:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

### 根据悬浮模式定制背景

```xml
<!-- Block 模式下增加额外内边距 -->
<Style Selector="atom|TreeView[NodeHoverMode=Block]">
    <Setter Property="Padding" Value="4" />
</Style>
```

---

## 3. 自定义展开/折叠图标

通过 `SwitcherRotationIcon`、`SwitcherExpandIcon`、`SwitcherCollapseIcon` 属性自定义展开/折叠图标：

### 旋转图标模式

```xml
<atom:TreeView IsSwitcherRotation="True">
    <atom:TreeView.SwitcherRotationIcon>
        <atom:IconTemplate>
            <antdicons:RightOutlined />
        </atom:IconTemplate>
    </atom:TreeView.SwitcherRotationIcon>
    <atom:TreeViewItem Header="parent" ItemKey="0-0">
        <atom:TreeViewItem Header="child 1" ItemKey="0-0-0" />
    </atom:TreeViewItem>
</atom:TreeView>
```

### 替换图标模式

```xml
<atom:TreeView IsSwitcherRotation="False">
    <atom:TreeView.SwitcherExpandIcon>
        <atom:IconTemplate>
            <antdicons:PlusSquareOutlined />
        </atom:IconTemplate>
    </atom:TreeView.SwitcherExpandIcon>
    <atom:TreeView.SwitcherCollapseIcon>
        <atom:IconTemplate>
            <antdicons:MinusSquareOutlined />
        </atom:IconTemplate>
    </atom:TreeView.SwitcherCollapseIcon>
    <!-- 节点... -->
</atom:TreeView>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml` 中 "Customize collapse/expand icon" 示例。

---

## 4. 通过 TreeDataTemplate 数据绑定

使用 `ItemsSource` + `TreeDataTemplate` 实现 MVVM 数据驱动：

```xml
<atom:TreeView Name="MyTreeView" ToggleType="CheckBox">
    <atom:TreeView.ItemTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:TreeView.ItemTemplate>
</atom:TreeView>
```

```csharp
// Code-behind 或 ViewModel
myTreeView.ItemsSource = new List<TreeItemNode>
{
    new TreeItemNode()
    {
        Header = "parent 1",
        ItemKey = "0-0",
        Children = [
            new TreeItemNode() { Header = "leaf 1", ItemKey = "0-0-0" },
            new TreeItemNode() { Header = "leaf 2", ItemKey = "0-0-1" }
        ]
    }
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml` 中 "Generate by template" 示例。

---

## 5. 搜索过滤示例

通过配合 `SearchEdit` 实现树节点搜索：

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

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml` 中 "Searchable" 示例。

---

## 6. 异步加载示例

通过 `DataLoader` 实现展开时异步加载子节点：

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
// 初始化数据
AsyncLoadTree.ItemsSource = new List<TreeItemNode>
{
    new TreeItemNode() { Header = "Expand to load", ItemKey = "0" },
    new TreeItemNode() { Header = "Expand to load", ItemKey = "1" },
    new TreeItemNode() { Header = "Tree Node", ItemKey = "2", IsLeaf = true }
};

// 设置加载器
AsyncLoadTree.DataLoader = new TreeItemDataLoader();
```

```csharp
// 自定义加载器实现
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

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml` 中 "load data asynchronously" 示例。

---

## 7. 默认展开和选中路径

通过路径指定初始展开、选中和勾选状态：

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

// 默认勾选路径
myTreeView.DefaultCheckedPaths = new List<TreeNodePath>
{
    new TreeNodePath("0-0/0-0-1/0-0-1-1")
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/TreeViewShowCase.axaml.cs` 中 `InitBasicTreeViewData` 方法。

---

## 8. 控制动画行为

```xml
<!-- 禁用展开/折叠动画 -->
<atom:TreeView IsMotionEnabled="False">
    <!-- 节点... -->
</atom:TreeView>
```

```csharp
// 代码中全部展开（不带动画）
myTreeView.ExpandAll(motionEnabled: false);

// 代码中全部展开（带动画）
myTreeView.ExpandAll(motionEnabled: true);
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|TreeView` 语法引用 `atom` XML 命名空间下的类型，其中 `|` 是命名空间分隔符。

### TreeView 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TreeView` | 匹配所有 AtomUI TreeView 实例 |
| `atom\|TreeView:draggable` | 匹配启用了拖拽功能的 TreeView |
| `atom\|TreeView[NodeHoverMode=Block]` | 匹配 Block 悬浮模式的 TreeView |
| `atom\|TreeView[NodeHoverMode=WholeLine]` | 匹配 WholeLine 悬浮模式的 TreeView |
| `atom\|TreeView[IsShowLine=True]` | 匹配显示连接线的 TreeView |
| `atom\|TreeView:disabled` | 匹配禁用状态的 TreeView |

### TreeViewItem 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|TreeViewItem` | 匹配所有树节点 |
| `atom\|TreeViewItem:selected` | 匹配选中的节点 |
| `atom\|TreeViewItem:expanded` | 匹配展开的节点 |
| `atom\|TreeViewItem:checked` | 匹配勾选的节点 |
| `atom\|TreeViewItem:toggle` | 匹配 CheckBox 勾选模式的节点 |
| `atom\|TreeViewItem:radio` | 匹配 Radio 单选模式的节点 |
| `atom\|TreeViewItem:disabled` | 匹配禁用的节点 |
| `atom\|TreeViewItem:pointerover` | 匹配鼠标悬浮的节点 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|TreeViewItem:selected:not(:disabled)` | 非禁用状态下已选中的节点 |
| `atom\|TreeViewItem:toggle:checked` | CheckBox 模式下已勾选的节点 |
| `atom\|TreeView[IsShowLine=True] atom\|TreeViewItem:expanded` | 显示连接线的 TreeView 中展开的节点 |
| `atom\|TreeViewItem /template/ atom\|TreeViewItemHeader` | 访问节点模板内的标题区域 |
