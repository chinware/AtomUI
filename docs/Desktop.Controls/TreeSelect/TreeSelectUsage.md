# TreeSelect 使用文档

本文档介绍 AtomUI TreeSelect 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 TreeSelect，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // TreeSelect 控件
using AtomUI.Controls;            // SizeType, ITreeItemNode, TreeItemNode 等共享类型
```

---

## 1. 基本用法（单选）

最基本的 TreeSelect 使用方式——单选模式，点击节点即选中，弹窗自动关闭。

```xml
<atom:TreeSelect Name="BasicTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsDefaultExpandAll="True"
                 IsAllowClear="True"
                 IsFilterEnabled="True"
                 PlaceholderText="Please select" />
```

```csharp
// Code-behind：构建树形数据源
var treeNodes = new List<TreeItemNode>
{
    new TreeItemNode()
    {
        Header = "parent 1",
        Value  = "parent 1",
        Children = [
            new TreeItemNode()
            {
                Header = "parent 1-0",
                Value  = "parent 1-0",
                Children = [
                    new TreeItemNode() { Header = "leaf1", Value = "leaf1" },
                    new TreeItemNode() { Header = "leaf2", Value = "leaf2" },
                ]
            },
            new TreeItemNode()
            {
                Header = "parent 1-1",
                Value  = "parent 1-1",
                Children = [
                    new TreeItemNode() { Header = "leaf11", Value = "leaf11" }
                ]
            }
        ]
    }
};

BasicTreeSelect.ItemsSource = treeNodes;
```

**要点**：
- `IsDefaultExpandAll="True"` 使所有节点默认展开
- `IsAllowClear="True"` 在选中后显示清除按钮
- `IsFilterEnabled="True"` 启用搜索筛选功能

---

## 2. 多选模式

设置 `IsMultiple="True"` 启用多选模式，选中项以 Tag 形式展示在输入框中。

```xml
<atom:TreeSelect Name="MultiSelectionTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsDefaultExpandAll="True"
                 IsAllowClear="True"
                 IsFilterEnabled="True"
                 IsMultiple="True"
                 PlaceholderText="Please select" />
```

```csharp
vm.MultiSelectionTreeNodes = new List<TreeItemNode>
{
    new TreeItemNode()
    {
        Header = "parent 1",
        Value  = "parent 1",
        Children = [
            new TreeItemNode()
            {
                Header = "parent 1-0",
                Value  = "parent 1-0",
                Children = [
                    new TreeItemNode() { Header = "my leaf", Value = "leaf1" },
                    new TreeItemNode() { Header = "your leaf", Value = "leaf2" }
                ]
            },
            new TreeItemNode()
            {
                Header = "parent 1-1",
                Value  = "parent 1-1",
                Children = [
                    new TreeItemNode() { Header = "sss", Value = "sss" }
                ]
            }
        ]
    },
};
```

---

## 3. 通过 ItemsSource 绑定数据

使用 `ItemsSource` 属性绑定树形数据源是推荐的 MVVM 数据驱动方式：

```xml
<atom:TreeSelect Name="ItemsSourceTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsDefaultExpandAll="True"
                 IsAllowClear="True"
                 IsFilterEnabled="True"
                 PlaceholderText="Please select" />
```

```csharp
// 使用 ReactiveUI 绑定
this.OneWayBind(ViewModel, vm => vm.TreeNodes, v => v.ItemsSourceTreeSelect.ItemsSource)
    .DisposeWith(disposables);
```

```csharp
// ViewModel
vm.TreeNodes = new List<TreeItemNode>
{
    new TreeItemNode()
    {
        Header = "Node1",
        Value  = "0-0",
        Children = [
            new TreeItemNode() { Header = "Child Node1", Value = "0-0-1" },
            new TreeItemNode() { Header = "Child Node2", Value = "0-0-2" }
        ]
    },
    new TreeItemNode()
    {
        Header = "Node2",
        Value  = "0-1",
    }
};
```

---

## 4. 勾选模式（Checkable）

设置 `IsTreeCheckable="True"` 启用 Checkbox 勾选模式，支持父子联动。配合 `ShowCheckedStrategy` 控制展示策略。

```xml
<atom:TreeSelect Name="CheckableTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsDefaultExpandAll="True"
                 IsAllowClear="True"
                 IsFilterEnabled="True"
                 IsTreeCheckable="True"
                 PlaceholderText="Please select"
                 ShowCheckedStrategy="ShowParent" />
```

```csharp
vm.CheckableTreeNodes = new List<TreeItemNode>
{
    new TreeItemNode()
    {
        Header = "Node1",
        ItemKey = "0-0",
        Children = [
            new TreeItemNode() { Header = "Child Node1", Value = "0-0-0" }
        ]
    },
    new TreeItemNode()
    {
        Header = "Node2",
        ItemKey = "0-1",
        Children = [
            new TreeItemNode() { Header = "Child Node3", Value = "0-1-0" },
            new TreeItemNode()
            {
                Header = "Child Node4",
                Value  = "0-1-1",
                Children = [
                    new TreeItemNode() { Header = "Child Node6", Value = "0-1-1-0" },
                    new TreeItemNode() { Header = "Child Node7", Value = "0-1-1-1" },
                ]
            },
            new TreeItemNode() { Header = "Child Node5", Value = "0-1-2" }
        ]
    }
};
```

**勾选策略说明**：
- `ShowCheckedStrategy="All"`：展示所有勾选节点
- `ShowCheckedStrategy="ShowParent"`：当所有子节点都勾选时仅展示父节点
- `ShowCheckedStrategy="ShowChild"`：仅展示叶子节点

---

## 5. 异步加载

通过 `DataLoader` 属性提供异步加载器，实现展开节点时按需加载子节点：

```xml
<atom:TreeSelect Name="AsyncLoadTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsAllowClear="True"
                 PlaceholderText="Please select" />
```

```csharp
// 初始化根节点
vm.AsyncLoadTreeNodes = new List<TreeItemNode>
{
    new TreeItemNode() { Header = "Expand to load", ItemKey = "0" },
    new TreeItemNode() { Header = "Expand to load", ItemKey = "1" },
    new TreeItemNode() { Header = "Tree Node", ItemKey = "2", IsLeaf = true }
};

// 绑定异步加载器
vm.AsyncLoadTreeNodeLoader = new TreeItemDataLoader();
this.OneWayBind(ViewModel, vm => vm.AsyncLoadTreeNodeLoader, v => v.AsyncLoadTreeSelect.DataLoader)
    .DisposeWith(disposables);
```

```csharp
// 自定义加载器实现 ITreeItemNodeLoader 接口
public class TreeItemDataLoader : ITreeItemNodeLoader
{
    public async Task<IList<ITreeItemNode>> LoadChildrenAsync(ITreeItemNode parentNode, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken); // 模拟异步加载
        return new List<ITreeItemNode>
        {
            new TreeItemNode() { Header = $"Child of {parentNode.Header}", Value = "child-1" },
            new TreeItemNode() { Header = $"Another child", Value = "child-2", IsLeaf = true }
        };
    }
}
```

**要点**：非叶子节点展开时自动调用 `DataLoader`，加载过程中显示旋转图标。设置 `IsLeaf = true` 的节点不会触发加载。

---

## 6. 弹窗位置

通过 `Placement` 属性和 `IsPopupMatchSelectWidth` 属性控制弹窗弹出位置：

```xml
<atom:TreeSelect Name="PlacementTreeSelect"
                 IsAllowClear="True"
                 PlaceholderText="Please select"
                 IsDefaultExpandAll="True"
                 IsFilterEnabled="True"
                 IsPopupMatchSelectWidth="False"
                 Placement="{Binding Placement}" />
```

```csharp
// ViewModel 中动态切换弹窗位置
public void HandlePlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
{
    if (args.Index == 0)
        vm.Placement = SelectPopupPlacement.TopEdgeAlignedLeft;
    else if (args.Index == 1)
        vm.Placement = SelectPopupPlacement.TopEdgeAlignedRight;
    else if (args.Index == 2)
        vm.Placement = SelectPopupPlacement.BottomEdgeAlignedLeft;
    else
        vm.Placement = SelectPopupPlacement.BottomEdgeAlignedRight;
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml` 中 "Placement" 示例。

---

## 7. 显示树线

通过 `IsShowLine`、`IsShowIcon`、`IsShowLeafIcon`、`IsSwitcherRotation` 属性控制树形结构的视觉展示：

```xml
<atom:ToggleSwitch OnContent="ShowIcon" OffContent="ShowIcon" Name="ShowIconSwitch" />
<atom:ToggleSwitch OnContent="TreeLine" OffContent="TreeLine" IsChecked="True"
                   Name="ShowTreeLineSwitch" />
<atom:ToggleSwitch OnContent="ShowLeafIcon" OffContent="ShowLeafIcon" Name="ShowLeafIconSwitch" />

<atom:TreeSelect Name="ShowTreeLineTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsAllowClear="True"
                 PlaceholderText="Please select"
                 IsShowIcon="{Binding #ShowIconSwitch.IsChecked}"
                 IsShowLeafIcon="{Binding #ShowLeafIconSwitch.IsChecked}"
                 IsShowLine="{Binding #ShowTreeLineSwitch.IsChecked}"
                 IsSwitcherRotation="{Binding #ShowTreeLineSwitch.IsChecked,
                                      Converter={x:Static BoolConverters.Not}}" />
```

```csharp
// 数据中可以为节点指定图标
vm.ShowTreeLineTreeNodes = new List<TreeItemNode>
{
    new TreeItemNode()
    {
        Header = "parent 1",
        ItemKey = "parent 1",
        Children = [
            new TreeItemNode()
            {
                Header = "parent 1-0",
                Value  = "parent 1-0",
                Icon   = new CarryOutOutlined(),
                Children = [
                    new TreeItemNode()
                    {
                        Header  = "Leaf1",
                        ItemKey = "Leaf1",
                        IsLeaf  = true,
                        Icon    = new CarryOutOutlined(),
                    }
                ]
            }
        ]
    }
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml` 中 "Show Tree Line" 示例。

---

## 8. 样式变体

TreeSelect 支持四种样式变体，通过 `StyleVariant` 属性设置：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:TreeSelect HorizontalAlignment="Stretch"
                     IsAllowClear="True"
                     PlaceholderText="Outline (default)"
                     StyleVariant="Outline" />
    <atom:TreeSelect HorizontalAlignment="Stretch"
                     IsAllowClear="True"
                     PlaceholderText="Filled"
                     StyleVariant="Filled" />
    <atom:TreeSelect HorizontalAlignment="Stretch"
                     IsAllowClear="True"
                     PlaceholderText="Borderless"
                     StyleVariant="Borderless" />
    <atom:TreeSelect HorizontalAlignment="Stretch"
                     IsAllowClear="True"
                     PlaceholderText="Underlined"
                     StyleVariant="Underlined" />
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml` 中 "Variants" 示例。

---

## 9. 验证状态

通过 `Status` 属性设置 Error 或 Warning 状态，配合表单验证使用：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:TreeSelect HorizontalAlignment="Stretch"
                     IsAllowClear="True"
                     PlaceholderText="Error status"
                     Status="Error" />
    <atom:TreeSelect HorizontalAlignment="Stretch"
                     IsAllowClear="True"
                     PlaceholderText="Warning status"
                     Status="Warning" />
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml` 中 "Status" 示例。

---

## 10. 前缀附加内容

通过 `LeftAddOn` 和 `ContentLeftAddOn` 属性为 TreeSelect 添加前缀内容：

```xml
<!-- LeftAddOn：独立的左侧装饰区域 -->
<atom:TreeSelect Name="LeftAddTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsAllowClear="True"
                 PlaceholderText="Please select"
                 IsDefaultExpandAll="True"
                 LeftAddOn="Prefix"
                 IsFilterEnabled="True" />

<!-- ContentLeftAddOn：内容区域内的左侧前缀 -->
<atom:TreeSelect Name="ContentLeftAddTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsAllowClear="True"
                 PlaceholderText="Please select"
                 IsDefaultExpandAll="True"
                 ContentLeftAddOn="Prefix"
                 IsFilterEnabled="True" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml` 中 "Prefix and Suffix" 示例。

---

## 11. 最大选择数量

在多选/勾选模式下，通过 `MaxCount` 限制最大可选数量。达到上限后未选中节点自动禁用：

```xml
<!-- 多选模式 + MaxCount -->
<atom:TreeSelect Name="MaxSelectedTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsDefaultExpandAll="True"
                 IsMultiple="True"
                 IsAllowClear="True"
                 IsFilterEnabled="True"
                 PlaceholderText="Please select"
                 IsShowMaxCountIndicator="True"
                 MaxCount="3" />

<!-- 勾选模式 + MaxCount -->
<atom:TreeSelect Name="MaxCheckedTreeSelect"
                 HorizontalAlignment="Stretch"
                 IsDefaultExpandAll="True"
                 IsMultiple="True"
                 IsTreeCheckable="True"
                 IsAllowClear="True"
                 IsFilterEnabled="True"
                 PlaceholderText="Please select"
                 IsShowMaxCountIndicator="True"
                 MaxCount="3" />
```

**要点**：
- `IsShowMaxCountIndicator="True"` 在右侧显示 `已选/最大` 数量指示器
- 当选中数量达到 `MaxCount` 时，`TreeSelectTreeViewItem` 会通过 `IsMaxSelectReached` 属性将未选中/未勾选的节点禁用

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TreeSelectShowCase.axaml` 中 "Max Count" 示例。

---

## 12. 获取选中值

### 单选模式

```csharp
// 通过 SelectedItem 获取选中的节点
ITreeItemNode? selected = myTreeSelect.SelectedItem;
if (selected != null)
{
    Console.WriteLine($"Selected: {selected.Header} (Value: {selected.Value})");
}
```

### 多选/勾选模式

```csharp
// 通过 SelectedItems 获取所有选中的节点
IList<ITreeItemNode>? selectedItems = myTreeSelect.SelectedItems;
if (selectedItems != null)
{
    foreach (var item in selectedItems)
    {
        Console.WriteLine($"Selected: {item.Header}");
    }
}
```

### 清空选中项

```csharp
// 调用 Clear() 方法清空所有选中项
myTreeSelect.Clear();
```

---

## 常见组合模式

### 表单中使用 TreeSelect

```xml
<atom:FormItem Label="Department">
    <atom:TreeSelect IsAllowClear="True"
                     PlaceholderText="Select department"
                     HorizontalAlignment="Stretch" />
</atom:FormItem>
```

### 多选 + 搜索 + 最大数量限制

```xml
<atom:TreeSelect IsMultiple="True"
                 IsFilterEnabled="True"
                 IsAllowClear="True"
                 MaxCount="5"
                 IsShowMaxCountIndicator="True"
                 PlaceholderText="Select up to 5 items"
                 HorizontalAlignment="Stretch" />
```

### 勾选 + 仅展示叶子节点

```xml
<atom:TreeSelect IsTreeCheckable="True"
                 ShowCheckedStrategy="ShowChild"
                 IsAllowClear="True"
                 IsDefaultExpandAll="True"
                 PlaceholderText="Select leaf nodes"
                 HorizontalAlignment="Stretch" />
```

### 异步加载 + 搜索

```xml
<atom:TreeSelect IsFilterEnabled="True"
                 IsAllowClear="True"
                 DataLoader="{Binding MyDataLoader}"
                 PlaceholderText="Search and load"
                 HorizontalAlignment="Stretch" />
```
