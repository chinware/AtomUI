# 过滤使用指南

本文说明如何为 AtomUI Control 启用过滤、选择匹配方式和接入自定义数据。公共过滤模型、状态所有权和兼容性由
[过滤系统架构](../../architecture/systems/control-infrastructure/filtering.md) 定义。

## 启用过滤

大多数支持过滤的 Control 使用以下属性：

| 属性 | 作用 |
| --- | --- |
| `IsFilterEnabled` | 显示搜索 UI 并启用 Control 的过滤路径 |
| `Filter` | `IValueFilter?` 谓词；未设置时由 Control 提供默认匹配 |
| `FilterValue` | 当前搜索条件，通常由内部搜索输入更新 |
| `FilterValueSelector` | 把 item 转换成供谓词比较的值 |

### Select

```xml
<atom:Select IsFilterEnabled="True"
             OptionsSource="{Binding Users}" />
```

Select 默认使用 Contains 过滤，并从 option 的 Header 或配置的过滤字段中取值。

### ComboBox

```xml
<atom:ComboBox IsEditable="True"
               IsFilterEnabled="True"
               ItemsSource="{Binding Cities}"
               DisplayMemberBinding="{Binding Name}" />
```

ComboBox 只有在 `IsEditable && IsFilterEnabled` 时过滤候选项。过滤视图不会修改原始 `ItemsSource`、选中项或原始索引。

## 选择匹配方式

`ValueFilterFactory.BuildFilter(...)` 提供 StartsWith、Contains 和 Equals，以及 culture/ordinal、大小写敏感/不敏感组合。

```csharp
myAutoComplete.Filter = ValueFilterFactory.BuildFilter(
    ValueFilterMode.StartsWithCaseSensitive);
```

只希望显示搜索框时设置 `IsFilterEnabled=True` 即可。`Filter` 和 `IsFilterEnabled` 是正交契约：Filter 不自动打开
搜索 UI，null Filter 也不自动关闭搜索 UI。

## 自定义取值

复杂对象可以通过 `FilterValueSelector` 投影多个业务字段：

```csharp
myListView.FilterValueSelector = value =>
{
    if (value is User user)
    {
        return $"{user.Name} {user.Department} {user.Email}";
    }

    return value?.ToString();
};
```

Selector 只负责 item 到可比较值的转换，不执行谓词，也不修改 item。

## 自定义谓词

```csharp
public sealed class PinyinFilter : IValueFilter
{
    public ValueFilterMode Mode => ValueFilterMode.Custom;

    public bool Filter(object? value, object? filterValue)
    {
        var text = value?.ToString();
        var keyword = filterValue?.ToString();
        return !string.IsNullOrEmpty(text) &&
               !string.IsNullOrEmpty(keyword) &&
               PinyinHelper.Contains(text, keyword);
    }
}
```

```csharp
myCascader.Filter = new PinyinFilter();
```

谓词只接收 `(value, filterValue)`。Control、item owner、Visual 或节点路径不能作为隐式第三个输入；层级信息应由
Selector 显式编码到 value。

## 层级和双栏 Control

- TreeView 和 TreeSelect 使用 `FilterStrategy` 决定父链展开、隐藏和高亮。
- Cascader、ListBox 和 ListView 可以使用 `FilterHighlightStrategy` 和 `FilterHighlightForeground`。
- Transfer 使用左右独立的过滤值，但共享 Filter 和 FilterValueSelector。

```xml
<atom:ListTransfer IsFilterEnabled="True"
                   FilterValueSelector="{x:Static local:MyViewModel.RecordSelector}"
                   ItemsSource="{Binding SourceItems}"
                   TargetItemsSource="{Binding TargetItems}" />
```

## 新 Control 接入

1. 注册 `Filter`、`FilterValue`、`FilterValueSelector` 和 `IsFilterEnabled`。
2. 使用 `SetCurrentValue` 提供默认 Filter 与 Selector，不覆盖用户 local value。
3. 过滤时先通过 Selector 得到 value，再调用 `Filter.Filter(value, filterValue)`。
4. 搜索 UI 的可见性由 `IsFilterEnabled` 驱动。
5. 特殊有效状态使用 internal effective property，不改变公共 `IsFilterEnabled` 语义。
6. 数据源替换、item recycle 和过滤结束后释放临时视图、binding 与高亮状态。

```csharp
private bool Matches(object? item)
{
    if (Filter is null || FilterValue is null)
    {
        return true;
    }

    var selector = FilterValueSelector ?? DefaultSelector;
    return Filter.Filter(selector(item), FilterValue);
}
```

## 默认取值行为

| Control | 默认取值 |
| --- | --- |
| AutoComplete | option Value 或字符串本身 |
| ComboBox | TextSearch、DisplayMemberBinding、ComboBoxItem.Content 或 `ToString()` |
| Mentions | option Content |
| Select | Header 或 Value，取决于 FilterBy |
| Cascader | 已构建的路径文本 |
| TreeView / TreeSelect | item Header |
| ListBox | item Content 或 header |
| Transfer | 用户 Selector 或 `ToString()` fallback |

Upload 的 `Accepts` 是输入源约束，Pagination 是分页，两者不属于候选项过滤系统。

## 源码入口

- `src/AtomUI.Controls.Shared/Utils/IValueFilter.cs`
- `src/AtomUI.Controls.Shared/Utils/ValueFilterMode.cs`
- `src/AtomUI.Controls.Shared/Utils/ValueFilterFactory.cs`
- `src/AtomUI.Desktop.Controls/AutoComplete/AbstractAutoComplete.cs`
- `src/AtomUI.Desktop.Controls/ComboBox/ComboBox.cs`
- `src/AtomUI.Desktop.Controls/Cascader/CascaderView.Filter.cs`
- `src/AtomUI.Desktop.Controls/Transfer/AbstractTransfer.cs`
