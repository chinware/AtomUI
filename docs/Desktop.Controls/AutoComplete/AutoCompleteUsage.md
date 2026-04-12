# AutoComplete 使用文档

本文档介绍 AtomUI AutoComplete 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 AutoComplete，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // AutoComplete 控件、AutoCompleteOption 等
using AtomUI.Controls;            // SizeType 等共享类型
```

---

## 1. 基本用法

最简单的 AutoComplete，配合异步加载器提供候选数据：

```xml
<atom:AutoComplete Name="BasicAutoComplete"
                   PlaceholderText="input here"
                   Width="200" />
```

```csharp
// Code-behind：设置异步加载器
BasicAutoComplete.OptionsAsyncLoader = new BasicOptionsAsyncLoader();
```

异步加载器实现示例：

```csharp
public class BasicOptionsAsyncLoader : ICompleteOptionsAsyncLoader
{
    public async Task<CompleteOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(200)); // 模拟网络延迟
        List<IAutoCompleteOption> data = [];
        if (!string.IsNullOrWhiteSpace(context))
        {
            data.Add(new AutoCompleteOption { Header = context, Content = context });
            data.Add(new AutoCompleteOption
            {
                Header  = string.Concat(Enumerable.Repeat(context, 2)),
                Content = string.Concat(Enumerable.Repeat(context, 2)),
            });
            data.Add(new AutoCompleteOption
            {
                Header  = string.Concat(Enumerable.Repeat(context, 3)),
                Content = string.Concat(Enumerable.Repeat(context, 3)),
            });
        }
        return new CompleteOptionsLoadResult
        {
            StatusCode = RpcStatusCode.Success,
            Data = data
        };
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/ViewModels/DataEntry/AutoCompleteViewModel.cs`

---

## 2. 静态数据源

使用 `OptionsSource` 设置静态候选列表，配合内置过滤器自动筛选：

```xml
<atom:AutoComplete Name="FilterAutoComplete"
                   PlaceholderText="try to type `b`"
                   Width="300"
                   IsAllowClear="True"
                   Filter="{atom:ValueFilterProvider Contains}" />
```

```csharp
// Code-behind
FilterAutoComplete.OptionsSource = new List<IAutoCompleteOption>
{
    new AutoCompleteOption { Header = "Burns Bay Road", Content = "Burns Bay Road" },
    new AutoCompleteOption { Header = "Downing Street", Content = "Downing Street" },
    new AutoCompleteOption { Header = "Wall Street", Content = "Wall Street" },
};
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` 中 "Non-case-sensitive AutoComplete" 示例。

---

## 3. 自定义邮箱补全

常见的邮箱自动补全场景，根据输入自动拼接邮箱后缀：

```xml
<atom:AutoComplete Name="CustomizedAutoComplete"
                   PlaceholderText="input here"
                   Width="200" />
```

```csharp
public class CustomLabelOptionsAsyncLoader : ICompleteOptionsAsyncLoader
{
    private readonly List<string> _suffixes = ["gmail.com", "163.com", "qq.com"];
    
    public async Task<CompleteOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        List<IAutoCompleteOption> data = [];

        if (!string.IsNullOrWhiteSpace(context) && !context.Contains('@'))
        {
            foreach (var suffix in _suffixes)
            {
                var value = $"{context}@{suffix}";
                data.Add(new AutoCompleteOption { Header = value, Content = value });
            }
        }
        
        return new CompleteOptionsLoadResult { Data = data };
    }
}

// 绑定
CustomizedAutoComplete.OptionsAsyncLoader = new CustomLabelOptionsAsyncLoader();
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` 中 "Customized" 示例。

---

## 4. 搜索型自动补全（AutoCompleteSearchEdit）

带搜索按钮的自动补全，适用于搜索引擎式联想场景。支持自定义候选项模板：

```xml
<atom:AutoCompleteSearchEdit Name="SearchAutoComplete"
                             PlaceholderText="input here"
                             Width="300"
                             SizeType="Large"
                             SearchButtonStyle="Primary">
    <atom:AutoCompleteSearchEdit.OptionTemplate>
        <DataTemplate x:DataType="viewModels:CustomAutoCompleteOption">
            <DockPanel LastChildFill="True">
                <TextBlock Text="results"
                           DockPanel.Dock="Right"
                           Margin="5 0 0 0" />
                <TextBlock Text="{Binding ResultCount}"
                           DockPanel.Dock="Right" />
                <TextBlock Text="{Binding Header}"
                           TextTrimming="CharacterEllipsis" />
            </DockPanel>
        </DataTemplate>
    </atom:AutoCompleteSearchEdit.OptionTemplate>
</atom:AutoCompleteSearchEdit>
```

自定义选项数据模型：

```csharp
public record CustomAutoCompleteOption : AutoCompleteOption
{
    public int ResultCount { get; set; }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` 中 "Lookup-Patterns - Uncertain Category" 示例。

---

## 5. 多行文本自动补全（AutoCompleteTextArea）

使用 TextArea 作为输入框的自动补全，弹出层位置跟随光标：

```xml
<atom:AutoCompleteTextArea Name="TextAreaAutoComplete"
                           PlaceholderText="input here"
                           Width="300"
                           Lines="5"
                           IsAllowClear="True" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` 中 "TextArea type auto-completion" 示例。

---

## 6. 样式变体

通过 `StyleVariant` 属性切换四种视觉变体：

```xml
<StackPanel Spacing="10">
    <atom:AutoComplete Width="200" StyleVariant="Outline"
                       PlaceholderText="Outline" />
    <atom:AutoComplete Width="200" StyleVariant="Filled"
                       PlaceholderText="Filled" />
    <atom:AutoComplete Width="200" StyleVariant="Borderless"
                       PlaceholderText="Borderless" />
    <atom:AutoComplete Width="200" StyleVariant="Underlined"
                       PlaceholderText="Underlined" />
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` 中 "Variants" 示例。

---

## 7. 验证状态

通过 `Status` 属性展示错误或警告状态的视觉反馈：

```xml
<StackPanel Spacing="10">
    <atom:AutoComplete Width="200" Status="Error" />
    <atom:AutoComplete Width="200" Status="Warning" />
</StackPanel>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` 中 "Status" 示例。

---

## 8. 清除按钮

### 默认清除按钮

```xml
<atom:AutoComplete Width="200" IsAllowClear="True"
                   PlaceholderText="Clearable" />
```

### 自定义清除图标

```xml
<atom:AutoComplete Width="200"
                   IsAllowClear="True"
                   PlaceholderText="Customized clear icon"
                   ClearIcon="{antdicons:AntDesignIconProvider CloseSquareFilled}" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` 中 "Customize clear button" 示例。

---

## 9. 使用 ReactiveUI 进行 MVVM 绑定

Gallery 示例使用 ReactiveUI 进行数据绑定，是推荐的 MVVM 模式：

### ViewModel

```csharp
public class AutoCompleteViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }
    public string UrlPathSegment { get; } = "AutoComplete";
    
    private ICompleteOptionsAsyncLoader? _basicOptionsAsyncLoader;
    public ICompleteOptionsAsyncLoader? BasicOptionsAsyncLoader
    {
        get => _basicOptionsAsyncLoader;
        set => this.RaiseAndSetIfChanged(ref _basicOptionsAsyncLoader, value);
    }
    
    private List<IAutoCompleteOption>? _filterCaseOptions;
    public List<IAutoCompleteOption>? FilterCaseOptions
    {
        get => _filterCaseOptions;
        set => this.RaiseAndSetIfChanged(ref _filterCaseOptions, value);
    }
    
    public AutoCompleteViewModel(IScreen screen)
    {
        HostScreen = screen;
        Activator  = new ViewModelActivator();
    }
}
```

### View（ReactiveUI 绑定）

```csharp
public partial class AutoCompleteShowCase : ReactiveUserControl<AutoCompleteViewModel>
{
    public AutoCompleteShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is AutoCompleteViewModel viewModel)
            {
                viewModel.BasicOptionsAsyncLoader = new BasicOptionsAsyncLoader();
                
                this.OneWayBind(viewModel,
                    vm => vm.BasicOptionsAsyncLoader,
                    v => v.BasicAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);
                
                this.OneWayBind(viewModel,
                    vm => vm.FilterCaseOptions,
                    v => v.FilterAutoComplete.OptionsSource)
                    .DisposeWith(disposables);
                
                Disposable.Create(() =>
                {
                    viewModel.BasicOptionsAsyncLoader = null;
                    viewModel.FilterCaseOptions = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml.cs`

---

## 10. 禁用状态

```xml
<atom:AutoComplete Width="200" IsEnabled="False"
                   PlaceholderText="Disabled" />
```

---

## 11. 控制动画行为

```xml
<!-- 禁用弹出/关闭动画 -->
<atom:AutoComplete IsMotionEnabled="False"
                   PlaceholderText="No Animation" Width="200" />
```

---

## 12. 值变更事件监听

```xml
<atom:AutoComplete Name="MyAutoComplete"
                   ValueChanged="HandleValueChanged"
                   SelectionChanged="HandleSelectionChanged"
                   Width="200" />
```

```csharp
private void HandleValueChanged(object? sender, CompleteValueChangedEventArgs e)
{
    Console.WriteLine($"Value changed to: {e.Value}");
}

private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    if (e.AddedItems.Count > 0 && e.AddedItems[0] is IAutoCompleteOption option)
    {
        Console.WriteLine($"Selected: {option.Header}");
    }
}
```

---

## 常见组合模式

### 表单中的地址补全

```xml
<atom:FormItem Label="Address">
    <atom:AutoComplete PlaceholderText="Enter your address"
                       Width="300"
                       IsAllowClear="True" />
</atom:FormItem>
```

### 搜索框联想

```xml
<atom:AutoCompleteSearchEdit PlaceholderText="Search..."
                             Width="400"
                             SizeType="Large"
                             SearchButtonStyle="Primary" />
```

### 评论框中的 @提及

```xml
<atom:AutoCompleteTextArea PlaceholderText="Type @ to mention someone"
                           Width="400"
                           Lines="4"
                           IsAutoSize="True" />
```
