# Transfer 使用文档

本文档介绍 AtomUI Transfer 控件的使用方式与常见场景。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/TransferShowCase.axaml`

---

## 前置准备

### AXAML 命名空间

```xml
xmlns:atom="https://atomui.net"
```

### C# 命名空间

```csharp
using AtomUI.Desktop.Controls;
```

---

## 基本用法

最基本的穿梭框，提供数据源和目标键，即可完成双栏穿梭操作：

```xml
<atom:ListTransfer Name="BasicListTransfer"
                   SourceTitle="Source"
                   TargetTitle="Target" />
```

在代码后置中设置数据源：

```csharp
var items = new List<ListItemData>();
for (var i = 0; i < 20; i++)
{
    items.Add(new ListItemData
    {
        ItemKey = new EntityKey(i),
        Content = $"content{i + 1}"
    });
}
BasicListTransfer.ItemsSource = items;
BasicListTransfer.TargetKeys = new List<EntityKey>
{
    new(1), new(3), new(5)
};
```

---

## 单向穿梭

设置 `IsOneWay="True"` 后，只有"转移到目标"按钮可见，目标面板以删除按钮替代选择框：

```xml
<atom:ListTransfer Name="OneWayTransferList"
                   SourceTitle="Source"
                   TargetTitle="Target"
                   IsOneWay="True"
                   IsEnabled="{Binding OneWayTransferEnabled}" />
<atom:ToggleSwitch IsChecked="{Binding OneWayTransferEnabled, Mode=TwoWay}"
                   OnContent="disable"
                   OffContent="enable" />
```

---

## 带搜索的穿梭框

启用 `IsFilterEnabled` 后在面板头部下方显示搜索框。可通过 `FilterValueSelector` 指定搜索匹配的数据字段：

```xml
<atom:ListTransfer Name="SearchTransferList"
                   IsFilterEnabled="True"
                   FilterPlaceholderText="Search here"
                   FilterValueSelector="{Binding TransferFilterValueSelector}" />
```

---

## 高级用法

自定义穿梭按钮文本、列表宽度、页脚内容和列表项模板：

```xml
<atom:ListTransfer ToTargetButtonText="to right"
                   ToSourceButtonText="to left"
                   ListWidth="300"
                   Name="AdvanceTransfer">
    <atom:ListTransfer.SourceViewFooter>
        <atom:Button SizeType="Small" Click="ReloadAdvancedTransferItems">
            Left button reload
        </atom:Button>
    </atom:ListTransfer.SourceViewFooter>
    <atom:ListTransfer.TargetViewFooter>
        <atom:Button SizeType="Small" Click="ReloadAdvancedTransferItems">
            Right button reload
        </atom:Button>
    </atom:ListTransfer.TargetViewFooter>
    <atom:ListTransfer.ItemTemplate>
        <DataTemplate x:DataType="viewModels:SearchCaseItemData">
            <TextBlock TextTrimming="CharacterEllipsis">
                <Run Text="{Binding Content}" />
                <Run Text="-" />
                <Run Text="{Binding Description}" />
            </TextBlock>
        </DataTemplate>
    </atom:ListTransfer.ItemTemplate>
</atom:ListTransfer>
```

---

## 分页穿梭框

当数据量很大时，设置 `PageSize` 启用分页显示：

```xml
<atom:ListTransfer Name="PaginationTransferList"
                   IsOneWay="{Binding PaginationIsOneWay}"
                   PageSize="10"
                   ListHeight="370" />
<atom:ToggleSwitch IsChecked="{Binding PaginationIsOneWay, Mode=TwoWay}"
                   OnContent="one way"
                   OffContent="one way" />
```

> 启用分页后列表宽度自动使用 `ListWidthLG`（250px），选择操作下拉菜单会增加"选择当页"、"反选当页"等选项。

---

## 树形穿梭框

使用 `TreeTransfer` 在源面板展示树形结构数据：

```xml
<atom:TreeTransfer Name="TreeTransfer"
                   IsStretchView="True"
                   IsOneWay="{Binding TreeTransferIsOneWay}">
    <atom:TreeTransfer.SourceView>
        <atom:TransferTreeView IsDefaultExpandAll="True" />
    </atom:TreeTransfer.SourceView>
</atom:TreeTransfer>
<atom:ToggleSwitch IsChecked="{Binding TreeTransferIsOneWay, Mode=TwoWay}"
                   OnContent="one way"
                   OffContent="one way" />
```

树形穿梭的数据项需实现 `ITreeItemNode` 接口（含 `Children` 属性和 `Header` 属性），穿梭后目标面板以扁平列表展示已选节点。

---

## 验证状态

Transfer 支持 `Status` 属性显示验证状态边框：

```xml
<!-- Error 状态 —— 红色边框 -->
<atom:ListTransfer Status="Error" />

<!-- Warning 状态 —— 黄色边框 -->
<atom:ListTransfer Status="Warning" />
```

---

## 事件处理

### SelectionChanged 事件

穿梭操作完成后触发，可获取源面板和目标面板的键集合：

```csharp
transfer.SelectionChanged += (sender, args) =>
{
    var sourceKeys = args.SourceSelectedKeys;
    var targetKeys = args.TargetSelectedKeys;
    // 处理穿梭后的逻辑
};
```

---

## 数据模型

### 列表穿梭数据项

列表穿梭的数据项需实现 `IItemKey` 接口。最简单的方式是使用内置的 `ListItemData`：

```csharp
var item = new ListItemData
{
    ItemKey = new EntityKey(1),
    Content = "Item 1"
};
```

### 自定义数据模型

也可以自定义数据模型，只要实现 `IItemKey` 接口即可：

```csharp
public class MyTransferItem : IItemKey
{
    public EntityKey? ItemKey { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}
```

### 树形穿梭数据项

树形穿梭需实现 `ITreeItemNode` 接口，包含 `Children` 子节点集合和 `Header` 展示文本。

---

## 常见问题

### Q: 如何控制面板宽度？

通过 `ListWidth` 属性设置固定宽度，或设置 `IsStretchView="True"` 让面板填满可用空间。

### Q: 分页模式下列表变宽了？

这是设计行为——启用分页后自动使用 `ListWidthLG`（250px）以容纳分页控件。

### Q: 如何自定义过滤逻辑？

实现 `IValueFilter` 接口并赋值给 `Filter` 属性即可替换默认的 `Contains` 过滤器。

### Q: TreeTransfer 的目标面板为什么是扁平列表？

与 Ant Design 的行为一致——树形穿梭的目标面板扁平化展示已选叶节点，源面板通过遮罩（灰显）表示已转移的节点。
