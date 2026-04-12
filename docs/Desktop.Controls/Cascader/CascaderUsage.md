# Cascader 使用文档

本文档介绍 AtomUI Cascader 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/CascaderShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Cascader，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;          // Cascader, CascaderOption, CascaderView
using AtomUI.Desktop.Controls.DataLoad; // ICascaderItemDataLoader
using AtomUI.Controls;                  // SizeType, TreeNodePath 等共享类型
```

---

## 1. 基本用法

最基本的级联选择器，用户逐级展开并选择叶子节点：

```xml
<atom:Cascader PlaceholderText="Please select"
               IsAllowClear="True">
    <atom:Cascader.OptionTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}"
                          x:DataType="atom:ICascaderOption">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:Cascader.OptionTemplate>
</atom:Cascader>
```

在 code-behind 中绑定数据源：

```csharp
var options = new List<ICascaderOption>
{
    new CascaderOption
    {
        Header = "Zhejiang", Value = "zhejiang",
        Children =
        {
            new CascaderOption
            {
                Header = "Hangzhou", Value = "hangzhou",
                Children =
                {
                    new CascaderOption { Header = "West Lake", Value = "xihu" },
                    new CascaderOption { Header = "Lingyin shi", Value = "lingyinshi" }
                }
            }
        }
    },
    new CascaderOption
    {
        Header = "Jiangsu", Value = "jiangsu",
        Children =
        {
            new CascaderOption
            {
                Header = "Nanjing", Value = "nanjing",
                Children =
                {
                    new CascaderOption { Header = "Zhong Hua Men", Value = "zhonghuamen" }
                }
            }
        }
    }
};

cascader.OptionsSource = options;
```

---

## 2. 使用 AXAML 直接声明选项

也可以在 AXAML 中直接使用 `CascaderOption` 声明层级数据（无需 OptionTemplate）：

```xml
<atom:CascaderView>
    <atom:CascaderOption Header="Zhejiang" Value="zhejiang">
        <atom:CascaderOption Header="Hangzhou" Value="hangzhou">
            <atom:CascaderOption Header="West Lake" Value="xihu" />
            <atom:CascaderOption Header="Lingyin shi" Value="lingyinshi" />
        </atom:CascaderOption>
    </atom:CascaderOption>
    <atom:CascaderOption Header="Jiangsu" Value="jiangsu">
        <atom:CascaderOption Header="Nanjing" Value="nanjing">
            <atom:CascaderOption Header="Zhong Hua Men" Value="zhonghuamen" />
        </atom:CascaderOption>
    </atom:CascaderOption>
</atom:CascaderView>
```

---

## 3. 默认选中值

通过 `DefaultSelectOptionPath` 设置初始选中值：

```csharp
// 通过 TreeNodePath 指定初始路径，段之间以 "/" 分隔
viewModel.DefaultSelectOptionPath = new TreeNodePath("zhejiang/hangzhou/xihu");
```

```xml
<atom:Cascader DefaultSelectOptionPath="{Binding DefaultSelectOptionPath}"
               PlaceholderText="Please select"
               IsAllowClear="True" />
```

路径中的每一段依次匹配选项的 `ItemKey` 或 `Value` 属性。

---

## 4. 悬浮展开

设置 `ExpandTrigger="Hover"` 后，鼠标悬浮在选项上即可展开子级，点击完成选择：

```xml
<atom:Cascader ExpandTrigger="Hover"
               PlaceholderText="Please select"
               IsAllowClear="True" />
```

---

## 5. 禁用选项

通过 `CascaderOption.IsEnabled` 禁用特定选项：

```csharp
new CascaderOption
{
    Header    = "Jiangsu",
    ItemKey   = "jiangsu",
    IsEnabled = false,  // 整个分支不可选
    Children = { ... }
}
```

也可以在子节点级别单独禁用：

```csharp
new CascaderOption
{
    Header    = "Hefang jie",
    ItemKey   = "hefangjie",
    IsEnabled = false  // 仅此选项不可选
}
```

---

## 6. 允许选择父节点

默认仅叶子节点可选。设置 `IsAllowSelectParent="True"` 后，任何层级的节点都可以被选中：

```xml
<atom:Cascader IsAllowSelectParent="True"
               PlaceholderText="Please select"
               IsAllowClear="True" />
```

> 注意：在非叶子节点上，单击会选中但不关闭下拉面板（需要双击才关闭），以便用户可以继续查看子级。

---

## 7. 多选模式

设置 `IsMultiple="True"` 启用多选，选中项以标签形式展示在输入框中：

```xml
<atom:Cascader IsMultiple="True"
               ShowCheckedStrategy="ShowParent"
               PlaceholderText="Please select"
               HorizontalAlignment="Stretch"
               IsAllowClear="True" />
```

多选模式下还可以禁用特定选项的复选框：

```csharp
new CascaderOption
{
    Header            = "Toy Fish",
    Value             = "fish",
    IsCheckBoxEnabled = false  // 此选项的复选框不可操作
}
```

---

## 8. 选中策略

多选模式下，`ShowCheckedStrategy` 控制标签的展示方式：

```xml
<!-- All：显示所有选中的节点 -->
<atom:Cascader IsMultiple="True"
               ShowCheckedStrategy="All"
               IsResponsiveTagMode="True"
               HorizontalAlignment="Stretch" />

<!-- ShowParent：父节点所有子节点选中时，只显示父节点 -->
<atom:Cascader IsMultiple="True"
               ShowCheckedStrategy="ShowParent"
               IsResponsiveTagMode="True"
               HorizontalAlignment="Stretch" />
```

---

## 9. 搜索过滤

设置 `IsFilterEnabled="True"` 启用输入搜索功能：

```xml
<atom:Cascader IsFilterEnabled="True"
               PlaceholderText="Please select"
               IsAllowClear="True" />
```

搜索时，下拉面板切换为过滤结果列表，匹配的关键词高亮显示（默认红色）。

---

## 10. 异步数据加载

实现 `ICascaderItemDataLoader` 接口，在展开节点时动态加载子级数据：

```csharp
public class CascaderItemDataLoader : ICascaderItemDataLoader
{
    public async Task<CascaderItemLoadResult> LoadAsync(
        ICascaderOption targetCascaderItem, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(600), token);
        var children = new List<CascaderOption>
        {
            new() { Header = $"{targetCascaderItem.Value} Dynamic 1", IsLeaf = true },
            new() { Header = $"{targetCascaderItem.Value} Dynamic 2", IsLeaf = true }
        };
        return new CascaderItemLoadResult { IsSuccess = true, Data = children };
    }
}
```

```xml
<atom:Cascader DataLoader="{Binding AsyncCascaderNodeLoader}"
               PlaceholderText="Please select"
               IsAllowClear="True" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/ViewModels/DataEntry/CascaderViewModel.cs` 中的 `CascaderItemDataLoader` 实现。

---

## 11. 自定义图标

### 自定义后缀图标

```xml
<atom:Cascader SuffixIcon="{antdicons:AntDesignIconProvider SmileOutlined}"
               PlaceholderText="Please select" />
```

### 自定义展开箭头

```xml
<atom:Cascader PlaceholderText="Please select">
    <atom:Cascader.ExpandIcon>
        <atom:IconTemplate>
            <antdicons:SmileOutlined />
        </atom:IconTemplate>
    </atom:Cascader.ExpandIcon>
</atom:Cascader>
```

### 自定义前缀内容

```xml
<atom:Cascader PlaceholderText="Please select">
    <atom:Cascader.ContentLeftAddOn>
        <antdicons:SmileOutlined />
    </atom:Cascader.ContentLeftAddOn>
</atom:Cascader>
```

---

## 12. 样式变体

Cascader 支持四种样式变体：

```xml
<atom:Cascader StyleVariant="Outline" PlaceholderText="Outlined (默认)" />
<atom:Cascader StyleVariant="Filled" PlaceholderText="Filled" />
<atom:Cascader StyleVariant="Borderless" PlaceholderText="Borderless" />
<atom:Cascader StyleVariant="Underlined" PlaceholderText="Underlined" />
```

---

## 13. 弹出位置

通过 `PopupPlacement` 控制下拉面板的弹出位置：

```xml
<atom:Cascader PopupPlacement="TopEdgeAlignedLeft" />
<atom:Cascader PopupPlacement="TopEdgeAlignedRight" />
<atom:Cascader PopupPlacement="BottomEdgeAlignedLeft" />   <!-- 默认 -->
<atom:Cascader PopupPlacement="BottomEdgeAlignedRight" />
```

---

## 14. 状态反馈

Cascader 支持 Error 和 Warning 两种状态：

```xml
<atom:Cascader Status="Error" PlaceholderText="Error state" />
<atom:Cascader Status="Warning" PlaceholderText="Warning state" />
```

---

## 15. 三种尺寸

```xml
<atom:Cascader SizeType="Large" PlaceholderText="Large" />
<atom:Cascader SizeType="Middle" PlaceholderText="Middle (默认)" />
<atom:Cascader SizeType="Small" PlaceholderText="Small" />
```

---

## 16. 禁用状态

```xml
<atom:Cascader IsEnabled="False" PlaceholderText="Disabled" />
```

---

## 17. CascaderView 独立使用

`CascaderView` 可以脱离 `Cascader` 独立使用，作为一个嵌入式的级联面板：

```xml
<!-- 单选面板 -->
<atom:CascaderView>
    <atom:CascaderOption Header="Zhejiang" Value="zhejiang">
        <atom:CascaderOption Header="Hangzhou" Value="hangzhou">
            <atom:CascaderOption Header="West Lake" Value="xihu" />
        </atom:CascaderOption>
    </atom:CascaderOption>
</atom:CascaderView>

<!-- 多选面板（带复选框） -->
<atom:CascaderView IsCheckable="True">
    <atom:CascaderOption Header="Zhejiang" Value="zhejiang">
        <atom:CascaderOption Header="Hangzhou" Value="hangzhou">
            <atom:CascaderOption Header="West Lake" Value="xihu" />
        </atom:CascaderOption>
    </atom:CascaderOption>
</atom:CascaderView>
```

### CascaderView 默认展开路径

```xml
<atom:CascaderView DefaultExpandedPath="{Binding DefaultExpandPath}">
    <atom:CascaderOption Header="Jiangsu" ItemKey="jiangsu">
        <atom:CascaderOption Header="Nanjing" ItemKey="nanjing">
            <atom:CascaderOption Header="Zhong Hua Men" ItemKey="zhonghuamen" />
        </atom:CascaderOption>
    </atom:CascaderOption>
</atom:CascaderView>
```

```csharp
viewModel.DefaultExpandPath = new TreeNodePath("jiangsu/nanjing/zhonghuamen");
```

### CascaderView 搜索过滤

```xml
<StackPanel Spacing="10">
    <atom:SearchEdit PlaceholderText="Search"
                     SearchButtonClick="HandleFilterClicked" />
    <atom:CascaderView Name="SearchCascaderView">
        <!-- 选项 -->
    </atom:CascaderView>
</StackPanel>
```

```csharp
private void HandleFilterClicked(object? sender, RoutedEventArgs e)
{
    if (sender is SearchEdit searchEdit)
    {
        SearchCascaderView.FilterValue = searchEdit.Text?.Trim();
    }
}
```

---

## 18. MVVM 数据绑定

使用 ReactiveUI 进行完整的 MVVM 绑定：

```csharp
// ViewModel
public class MyCascaderViewModel : ReactiveObject
{
    private List<ICascaderOption>? _options;
    public List<ICascaderOption>? Options
    {
        get => _options;
        set => this.RaiseAndSetIfChanged(ref _options, value);
    }
}
```

```csharp
// View (code-behind)
this.OneWayBind(viewModel, vm => vm.Options, v => v.MyCascader.OptionsSource)
    .DisposeWith(disposables);
```

---

## 常见组合模式

### 省市区选择器

```xml
<atom:Cascader PlaceholderText="请选择省/市/区"
               IsAllowClear="True"
               OptionsSource="{Binding RegionOptions}">
    <atom:Cascader.OptionTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}"
                          x:DataType="atom:ICascaderOption">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:Cascader.OptionTemplate>
</atom:Cascader>
```

### 可搜索的多选分类器

```xml
<atom:Cascader IsMultiple="True"
               IsFilterEnabled="True"
               IsResponsiveTagMode="True"
               ShowCheckedStrategy="ShowChild"
               HorizontalAlignment="Stretch"
               PlaceholderText="搜索并选择分类..."
               IsAllowClear="True" />
```

### 动态加载的组织架构选择

```xml
<atom:Cascader DataLoader="{Binding OrgDataLoader}"
               IsAllowSelectParent="True"
               PlaceholderText="选择部门"
               IsAllowClear="True" />
```
