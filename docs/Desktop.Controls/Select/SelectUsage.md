# Select 使用文档

本文档介绍 AtomUI Select 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Select，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Select, SelectOption, ISelectOption
using AtomUI.Controls;            // SizeType, InputControlStyleVariant 等共享类型
```

---

## 1. 基本用法

最基本的 Select 用法——单选模式，声明式定义选项：

```xml
<atom:Select Mode="Single" PlaceholderText="Please select" Width="120"
             DefaultValues="lucy">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
    <atom:SelectOption Header="Yiminghe" Content="yiminghe" />
    <atom:SelectOption Header="Disabled" IsEnabled="False" Content="disabled" />
</atom:Select>
```

**说明**：
- `Header` 是显示文本，`Content` 是选项的值
- `DefaultValues` 通过 `Content` 值匹配来设置默认选中项
- `IsEnabled="False"` 可禁用单个选项

### 禁用整个 Select

```xml
<atom:Select Mode="Single" PlaceholderText="Please select" IsEnabled="False" Width="120"
             DefaultValues="lucy">
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>
```

### 允许清空

```xml
<atom:Select Mode="Single" PlaceholderText="Please select" IsAllowClear="True" Width="120">
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>
```

---

## 2. 搜索过滤

通过 `IsFilterEnabled="True"` 启用展开时的搜索过滤：

```xml
<atom:Select Mode="Single" PlaceholderText="Please select" IsFilterEnabled="True">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
    <atom:SelectOption Header="Tom" Content="tom" />
</atom:Select>
```

**默认行为**：按 `Header` 属性进行模糊匹配。

### 自定义搜索过滤

使用内置过滤器或自定义 `IValueFilter` 实现：

```xml
<atom:Select Name="CustomSearchSelect"
             Mode="Single"
             PlaceholderText="Select a person"
             IsFilterEnabled="True"
             Filter="{atom:ValueFilterProvider Contains}">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
    <atom:SelectOption Header="Tom" Content="tom" />
</atom:Select>
```

在 code-behind 中设置自定义过滤器：

```csharp
public class CustomFilter : IValueFilter
{
    public bool Filter(object? value, object? filterValue)
    {
        if (value is string valueStr && filterValue is string filterValueStr)
        {
            // 区分大小写的包含匹配
            return valueStr.Contains(filterValueStr, StringComparison.Ordinal);
        }
        return false;
    }
}

// 在构造函数或初始化中设置
CustomSearchSelect.Filter = new CustomFilter();
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Custom Search" 示例。

---

## 3. 多选模式

通过 `Mode="Multiple"` 启用多选，已选项以标签形式展示：

```xml
<atom:Select Name="MultiSelect1"
             Mode="Multiple"
             PlaceholderText="Select a person"
             IsAllowClear="True"
             HorizontalAlignment="Stretch"
             IsFilterEnabled="True"
             DefaultValues="a10, c12" />
```

**说明**：
- 多选模式下，`DefaultValues` 支持逗号分隔的多个值
- 每个已选项显示为可关闭的标签（Tag）
- 点击标签上的关闭按钮可取消选中
- `Backspace` / `Delete` 键可删除最后一个标签

### 禁用多选 Select

```xml
<atom:Select Name="MultiSelect2"
             Mode="Multiple"
             PlaceholderText="Select a person"
             IsAllowClear="True"
             HorizontalAlignment="Stretch"
             DefaultValues="a10, c12"
             IsEnabled="False" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "multiple selection" 示例。

---

## 4. 标签模式（Tags）

通过 `Mode="Tags"` 允许用户从列表选择或自由输入创建新标签：

```xml
<atom:Select Name="TagsModeSelect"
             Mode="Tags"
             PlaceholderText="Tags Mode"
             HorizontalAlignment="Stretch"
             IsFilterEnabled="False" />
```

**Tags 模式特殊行为**：
- 搜索功能自动启用（即使 `IsFilterEnabled="False"`）
- 当输入内容无匹配项时，自动创建一个新的临时选项
- 选中后临时选项变为正式选项；取消选中时自动移除

---

## 5. 三种尺寸

通过 `SizeType` 属性设置尺寸：

```xml
<!-- 大号 -->
<atom:Select Mode="Single"
             SizeType="Large"
             PlaceholderText="Select a person"
             DefaultValues="a10"
             Width="200" />

<!-- 中号（默认） -->
<atom:Select Mode="Single"
             SizeType="Middle"
             PlaceholderText="Select a person"
             DefaultValues="a10"
             Width="200" />

<!-- 小号 -->
<atom:Select Mode="Single"
             SizeType="Small"
             PlaceholderText="Select a person"
             DefaultValues="a10"
             Width="200" />
```

尺寸也支持数据绑定，可在运行时动态切换：

```xml
<atom:Select Mode="Single"
             SizeType="{Binding SelectSizeType}"
             PlaceholderText="Select a person"
             Width="200" />
```

```csharp
public class SelectViewModel : ReactiveObject
{
    private SizeType _selectSizeType;
    public SizeType SelectSizeType
    {
        get => _selectSizeType;
        set => this.RaiseAndSetIfChanged(ref _selectSizeType, value);
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Sizes" 示例。

---

## 6. 自定义下拉选项

通过 `OptionTemplate` 自定义下拉选项的渲染方式：

```xml
<atom:Select Mode="Multiple" PlaceholderText="Please select"
             IsFilterEnabled="True"
             HorizontalAlignment="Stretch"
             IsAllowClear="True">
    <views:CustomOption Header="China" Content="china" Description="China (中国)" Emoji="🇨🇳" />
    <views:CustomOption Header="USA" Content="usa" Description="USA (美国)" Emoji="🇺🇸" />
    <views:CustomOption Header="Japan" Content="japan" Description="Japan (日本)" Emoji="🇯🇵" />
    <views:CustomOption Header="Korea" Content="korea" Description="Korea (韩国)" Emoji="🇰🇷" />
    <atom:Select.OptionTemplate>
        <DataTemplate x:DataType="views:CustomOption">
            <StackPanel Orientation="Horizontal" Spacing="5">
                <TextBlock Text="{Binding Emoji}" />
                <TextBlock Text="{Binding Description}" />
            </StackPanel>
        </DataTemplate>
    </atom:Select.OptionTemplate>
</atom:Select>
```

自定义选项类型：

```csharp
public record CustomOption : SelectOption
{
    public string? Description { get; init; }
    public string? Emoji { get; init; }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Custom dropdown options" 示例。

---

## 7. 选项分组

通过 `IsGroupEnabled="True"` 启用选项分组，每个选项通过 `Group` 属性指定所属分组：

```xml
<atom:Select Mode="Single" PlaceholderText="Please select"
             DefaultValues="lucy"
             Width="200"
             IsGroupEnabled="True"
             IsFilterEnabled="True">
    <atom:SelectOption Header="Jack" Content="jack" Group="Manager" />
    <atom:SelectOption Header="Lucy" Content="lucy" Group="Manager" />
    <atom:SelectOption Header="Chloe" Content="chloe" Group="engineer" />
    <atom:SelectOption Header="Lucas" Content="lucas" Group="engineer" />
</atom:Select>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Option Group" 示例。

---

## 8. 样式变体

通过 `StyleVariant` 属性设置三种样式变体：

```xml
<!-- Outlined（默认） -->
<atom:Select Mode="Single" PlaceholderText="Outline" HorizontalAlignment="Stretch">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>

<!-- Filled -->
<atom:Select Mode="Single" PlaceholderText="Filled" HorizontalAlignment="Stretch"
             StyleVariant="Filled">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>

<!-- Borderless -->
<atom:Select Mode="Single" PlaceholderText="Borderless" HorizontalAlignment="Stretch"
             StyleVariant="Borderless">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>
```

每种变体同时支持单选和多选模式。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Variants" 示例。

---

## 9. 隐藏已选选项

多选模式下，通过 `IsHideSelectedOptions="True"` 在下拉中隐藏已选项：

```xml
<atom:Select Mode="Multiple"
             PlaceholderText="Inserted are removed"
             IsHideSelectedOptions="True"
             IsFilterEnabled="True"
             HorizontalAlignment="Stretch">
    <atom:SelectOption Header="Apples" Content="Apples" />
    <atom:SelectOption Header="Nails" Content="Nails" />
    <atom:SelectOption Header="Bananas" Content="Bananas" />
    <atom:SelectOption Header="Helicopters" Content="Helicopters" />
</atom:Select>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Hide Already Selected" 示例。

---

## 10. 验证状态

通过 `Status` 属性设置 Error 或 Warning 状态：

```xml
<atom:Select Mode="Single" HorizontalAlignment="Stretch" Status="Error" />
<atom:Select Mode="Single" HorizontalAlignment="Stretch" Status="Warning" />
```

**说明**：Error 状态显示红色边框，Warning 状态显示橙色边框。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Status" 示例。

---

## 11. 最大选择数限制

通过 `MaxCount` 限制最大可选数量，配合 `IsShowMaxCountIndicator` 显示计数指示器：

```xml
<atom:Select Mode="Multiple"
             HorizontalAlignment="Stretch"
             PlaceholderText="Please select"
             MaxCount="3"
             IsShowMaxCountIndicator="True">
    <atom:SelectOption Header="Ava Swift" Content="Ava Swift" />
    <atom:SelectOption Header="Cole Reed" Content="Cole Reed" />
    <atom:SelectOption Header="Mia Blake" Content="Mia Blake" />
    <atom:SelectOption Header="Jake Stone" Content="Jake Stone" />
    <atom:SelectOption Header="Lily Lane" Content="Lily Lane" />
    <atom:SelectOption Header="Ryan Chase" Content="Ryan Chase" />
    <atom:SelectOption Header="Zoe Fox" Content="Zoe Fox" />
    <atom:SelectOption Header="Alex Grey" Content="Alex Grey" />
    <atom:SelectOption Header="Elle Blair" Content="Elle Blair" />
</atom:Select>
```

**说明**：当已选数量达到 `MaxCount` 后，未选选项将被禁用。指示器显示 `当前数/最大数`。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Max Count" 示例。

---

## 12. 响应式标签折叠

多选模式下处理大量已选标签的展示：

### 响应式自动折叠

```xml
<atom:Select Name="MaxTagSelect1"
             Mode="Multiple"
             PlaceholderText="Select Item..."
             HorizontalAlignment="Stretch"
             IsFilterEnabled="True"
             IsResponsiveTagMode="True" />
```

### 固定最大标签数

```xml
<atom:Select Name="MaxTagSelect3"
             Mode="Multiple"
             PlaceholderText="Select Item..."
             HorizontalAlignment="Stretch"
             IsFilterEnabled="True"
             MaxTagCount="5" />
```

**说明**：
- `IsResponsiveTagMode="True"`：根据控件实际宽度自动计算可见标签数量
- `MaxTagCount="5"`：固定最多显示 5 个标签，超出部分折叠为 `+N`

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Responsive maxTagCount" 示例。

---

## 13. 前缀和后缀

通过 `ContentLeftAddOn` 和 `ContentRightAddOn` 添加前后缀内容：

```xml
<!-- 文本前缀 -->
<atom:Select Mode="Single"
             PlaceholderText="Outline"
             DefaultValues="lucy"
             Width="200"
             ContentLeftAddOn="User">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>

<!-- 图标后缀 -->
<atom:Select Mode="Single" PlaceholderText="Please select"
             DefaultValues="lucy"
             IsFilterEnabled="True"
             Width="200"
             ContentRightAddOn="{antdicons:AntDesignIconProvider Kind=SmileOutlined}">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>
```

前缀和后缀同样支持多选模式：

```xml
<atom:Select Mode="Multiple"
             PlaceholderText="Outline"
             DefaultValues="lucy"
             Width="200"
             ContentLeftAddOn="User">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Prefix and Suffix" 示例。

---

## 14. 异步加载选项

通过 `OptionsLoader` 属性绑定 `ISelectOptionsAsyncLoader` 实现远程数据加载：

```csharp
// 实现异步加载器
public class SelectOptionsAsyncLoader : ISelectOptionsAsyncLoader
{
    public async Task<SelectOptionsLoadResult> LoadAsync(object? context, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(600), token); // 模拟网络请求
        var options = new List<ISelectOption>
        {
            new SelectOption { Header = "Jack", Content = "jack" },
            new SelectOption { Header = "Lucy", Content = "lucy" },
            new SelectOption { Header = "Yiminghe", Content = "yiminghe" },
            new SelectOption { Header = "Disabled", Content = "disabled", IsEnabled = false }
        };
        return new SelectOptionsLoadResult
        {
            Data = options,
            StatusCode = RpcStatusCode.Success
        };
    }
}
```

```xml
<atom:Select Name="AsyncLoadSelect"
             Mode="Single"
             PlaceholderText="Please select"
             Width="120"
             DefaultValues="lucy">
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>
```

在 code-behind 中绑定加载器：

```csharp
this.OneWayBind(viewModel, vm => vm.SelectOptionsAsyncLoader, v => v.AsyncLoadSelect.OptionsLoader)
    .DisposeWith(disposables);
```

**加载行为**：
- 首次点击打开时触发异步加载
- 加载期间显示旋转的 `LoadingOutlined` 图标
- 加载成功后自动填充选项并展开下拉
- 仅加载一次，后续点击直接展开

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/ViewModels/DataEntry/SelectViewModel.cs` 中的 `SelectOptionsAsyncLoader` 实现。

---

## 15. 数据绑定（OptionsSource）

通过 `OptionsSource` 属性绑定数据集合：

```csharp
// ViewModel
public class SelectViewModel : ReactiveObject
{
    private List<SelectOption>? _randomOptions;
    public List<SelectOption>? RandomOptions
    {
        get => _randomOptions;
        set => this.RaiseAndSetIfChanged(ref _randomOptions, value);
    }
}

// 初始化数据
var options = new List<SelectOption>();
for (var i = 10; i < 36; i++)
{
    var base36Str = ConvertToBase36(i);
    options.Add(new SelectOption
    {
        Header  = base36Str + i,
        Content = base36Str + i
    });
}
viewModel.RandomOptions = options;
```

在 code-behind 中绑定：

```csharp
this.OneWayBind(viewModel, vm => vm.RandomOptions, v => v.MultiSelect1.OptionsSource)
    .DisposeWith(disposables);
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml.cs`

---

## 16. 选中值绑定

### 单选模式

```csharp
// 通过 SelectedOption 获取当前选中项
this.OneWayBind(viewModel, vm => vm.DefaultSelectedOptions, v => v.DefaultSelectedSelect.SelectedOptions)
    .DisposeWith(disposables);
```

### 监听选择变更

```csharp
select.SelectionChanged += (sender, args) =>
{
    if (args.Mode == SelectMode.Single)
    {
        var selectedOption = args.NewValue as ISelectOption;
        // 处理单选变更...
    }
    else
    {
        var selectedOptions = args.NewValue as IList<ISelectOption>;
        // 处理多选变更...
    }
};
```

---

## 常见组合模式

### 表单内使用（带验证状态）

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Select Mode="Single"
                 PlaceholderText="请选择城市"
                 HorizontalAlignment="Stretch"
                 Status="Error">
        <atom:SelectOption Header="北京" Content="beijing" />
        <atom:SelectOption Header="上海" Content="shanghai" />
    </atom:Select>
</StackPanel>
```

### 联动选择

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Select Mode="Single" PlaceholderText="Province" Width="150">
        <atom:SelectOption Header="Zhejiang" Content="zhejiang" />
        <atom:SelectOption Header="Jiangsu" Content="jiangsu" />
    </atom:Select>
    <atom:Select Mode="Single" PlaceholderText="City" Width="150">
        <atom:SelectOption Header="Hangzhou" Content="hangzhou" />
        <atom:SelectOption Header="Ningbo" Content="ningbo" />
    </atom:Select>
</StackPanel>
```

### 标签录入

```xml
<atom:Select Mode="Tags"
             PlaceholderText="输入标签后回车"
             HorizontalAlignment="Stretch"
             IsAllowClear="True" />
```

### 分组搜索选择

```xml
<atom:Select Mode="Single"
             PlaceholderText="Search and select"
             Width="250"
             IsGroupEnabled="True"
             IsFilterEnabled="True"
             IsAllowClear="True">
    <atom:SelectOption Header="Jack" Content="jack" Group="Manager" />
    <atom:SelectOption Header="Lucy" Content="lucy" Group="Manager" />
    <atom:SelectOption Header="Chloe" Content="chloe" Group="Engineer" />
    <atom:SelectOption Header="Lucas" Content="lucas" Group="Engineer" />
</atom:Select>
```
