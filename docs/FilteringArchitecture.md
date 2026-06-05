# AtomUI 过滤体系架构与使用指南

> 适用于 `AtomUI.Desktop.Controls` 下所有支持"在一组候选项里按输入筛选"的控件：AutoComplete / Mentions / Select / Cascader / TreeSelect / TreeView / Transfer / ListBox / ListView。

## 1. 设计目标

AtomUI 把"过滤"抽象成统一体系，面向这几个诉求：

- **一套契约覆盖所有控件**：过滤谓词、过滤值、选择器三件套完全同构
- **样式层零侵入**：`IsFilterEnabled` 作为"用户意图开关"由 ControlTheme 消费，不做隐式派生
- **层级控件不妥协**：Cascader / TreeSelect / TreeView 的父链、路径、高亮策略这些真正有差异的能力原状保留
- **不要抽象化过度**：统一的是"谓词 + 值选择"，其它行为差异交给各控件自己处理

## 2. 核心契约

过滤能力由三件东西构成。

### 2.1 `IValueFilter` —— 过滤谓词

```csharp
namespace AtomUI.Controls.Utils;

public interface IValueFilter
{
    ValueFilterMode Mode => ValueFilterMode.Custom;
    bool Filter(object? value, object? filterValue);
}
```

- `value` 是从数据项中抽出的可比较值（字符串、数值、任意类型都可以）
- `filterValue` 是当前过滤条件（通常是搜索框文本）
- 返回 `true` 表示命中

`Mode` 属性让某些控件（比如 AutoComplete）能基于过滤类型做分支行为（前缀匹配时的最优候选选择），所以框架内置实现都老老实实填写了。

### 2.2 `DefaultFilterValueSelector` —— 值选择器

```csharp
public delegate object? DefaultFilterValueSelector(object? value);
```

职责：把数据项（item 原始对象）转成可以丢给 `IValueFilter.Filter` 比对的那个值。

- 对纯字符串列表，selector 直接返回自身
- 对 `ISelectOption`、`ICascaderItemInfo`、`TreeViewItem` 这类结构化数据，selector 负责抽 `Header`、`Content`、`Path` 等
- 对复杂业务对象，selector 可以是用户自定义 lambda，返回最有意义的那个字段或者拼接后的字符串

参数类型是 `object?`，自定义 selector 的 lambda 内部需要做空检查。

### 2.3 `ValueFilterMode` + `ValueFilterFactory` —— 内置谓词

```csharp
public enum ValueFilterMode
{
    None, Custom,
    StartsWith, StartsWithCaseSensitive, StartsWithOrdinal, StartsWithOrdinalCaseSensitive,
    Contains,   ContainsCaseSensitive,   ContainsOrdinal,   ContainsOrdinalCaseSensitive,
    Equals,     EqualsCaseSensitive,     EqualsOrdinal,     EqualsOrdinalCaseSensitive,
}

public static class ValueFilterFactory
{
    public static IValueFilter? BuildFilter(ValueFilterMode mode);
}
```

三种匹配策略（StartsWith / Contains / Equals）× 四种比较规则（文化敏感/Ordinal × 大小写敏感/不敏感），共 12 个内置 `IValueFilter`，绝大多数搜索场景都不需要写自定义实现。

## 3. 控件 API 总览

全部 9 个控件对外暴露的过滤相关属性命名一致：

| 属性 | 类型 | 作用 |
|---|---|---|
| `Filter` | `IValueFilter?` | 过滤谓词。`null` 代表不过滤；控件通常在 `OnInitialized` 里给默认值（一般是 `Contains`） |
| `FilterValue` | `object?` / `string?` | 当前过滤条件。通常由控件内部搜索框通过 `SetCurrentValue` 写入，也可由外部绑定 |
| `FilterValueSelector` | `DefaultFilterValueSelector?` | 值选择器。`null` 时控件用自己的默认 selector |
| `IsFilterEnabled` | `bool` | 用户意图开关。是否显示搜索 UI / 启用过滤路径 |

层级 / 高亮相关属性（只有部分控件有）：

| 属性 | 出现在 | 作用 |
|---|---|---|
| `FilterStrategy` | TreeSelect、TreeView | 树过滤策略（匹配部分高亮、展开路径、隐藏未匹配、显示整棵树等） |
| `FilterHighlightStrategy` | Cascader、ListBox、ListView | 文本高亮策略（全部、匹配部分、隐藏未匹配等） |
| `FilterHighlightForeground` | Cascader、TreeSelect、TreeView、ListBox、ListView | 高亮颜色 |
| `FilterResultCount` | Cascader、TreeView、ListBox | 过滤后命中的条目数 |
| `IsFiltering` | ListView、ListBox | "当前正在过滤中"的通知信号 |
| `SourceFilterValue` / `TargetFilterValue` | Transfer | 左右两栏独立的过滤条件（`FilterValue` 不存在于 Transfer） |

Transfer 的特殊性：它是双栏结构，过滤条件左右独立，但 `Filter` 和 `FilterValueSelector` 两栏共用。

## 4. 数据流

所有控件共用这条路径：

```
  [用户输入搜索文本]
        │
        ▼
  FilterValue ─────► 触发 OnPropertyChanged
        │                │
        │                ▼
        │         for each item in Items:
        │             object? selectedValue
        │                 = FilterValueSelector?.Invoke(item) ?? 控件内置默认抽值
        │             if Filter.Filter(selectedValue, FilterValue) → 保留 / 高亮
        │             else → 隐藏（依 HighlightStrategy 而定）
        │
        ▼
  IsFilterEnabled    ControlTheme 依此切换搜索框可见性 / 布局
```

关键点：

- **Filter 调用永远是两元**（`value`, `filterValue`），不带 owner、不带 item 对象本身
- **层级信息由 selector 负责编码**：例如 Cascader 默认 selector 取 `ICascaderItemInfo.Path`，里头已经是 `"华东 / 上海"` 这样的全路径字符串，谓词只看字符串就够
- **IsFilterEnabled 和 Filter 没有派生关系**：用户设 `IsFilterEnabled=True` 表示"我要过滤 UI"，默认谓词 `Filter` 只在控件初始化时 fallback 到 `Contains`；两者解耦

## 5. 使用示例

### 5.1 最常见场景：可搜索的 Select

```xml
<atom:Select IsFilterEnabled="True"
             OptionsSource="{Binding Users}" />
```

不需要写任何 C#。`IsFilterEnabled=True` 打开搜索框，`Filter` 默认用 `Contains`，selector 默认取 `ISelectOption.Header`。

### 5.2 自定义匹配策略

```xml
<atom:AutoComplete IsFilterEnabled="True"
                   Filter="{x:Static util:ValueFilterFactory.BuildFilter(util:ValueFilterMode.StartsWithCaseSensitive)}"
                   OptionsSource="{Binding Suggestions}" />
```

或者在 code-behind：

```csharp
myAutoComplete.Filter = ValueFilterFactory.BuildFilter(ValueFilterMode.StartsWithCaseSensitive);
```

### 5.3 自定义值选择器

场景：ListView 绑定了复杂业务对象，希望按多个字段拼接后匹配。

```csharp
myListView.FilterValueSelector = record =>
{
    if (record is User user)
    {
        return $"{user.Name} {user.Department} {user.Email}";
    }
    return record?.ToString();
};
```

`record` 是 `object?`，务必做空检查。

### 5.4 完全自定义谓词

当内置 12 种匹配策略都不满足（比如要做模糊拼音匹配、正则、或者跨字段加权）：

```csharp
public sealed class PinyinFilter : IValueFilter
{
    public ValueFilterMode Mode => ValueFilterMode.Custom;

    public bool Filter(object? value, object? filterValue)
    {
        var text = value?.ToString();
        var kw   = filterValue?.ToString();
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(kw)) return false;

        return PinyinHelper.Contains(text, kw);
    }
}

// 使用
myCascader.Filter = new PinyinFilter();
```

### 5.5 Transfer 双栏过滤

```xml
<atom:ListTransfer IsFilterEnabled="True"
                   FilterValueSelector="{x:Static local:MyViewModel.RecordSelector}"
                   ItemsSource="{Binding SourceItems}"
                   TargetItemsSource="{Binding TargetItems}" />
```

`SourceFilterValue` / `TargetFilterValue` 是 internal 的，由 ControlTheme 内部搜索框驱动；外部不需要手工绑它们。

## 6. 各控件的默认 Selector

控件内置的默认 selector 在 `OnInitialized` 里自动装上。用户可以随时覆盖。

| 控件 | 默认 selector 行为 |
|---|---|
| AutoComplete | 取 `IAutoCompleteOption.Value` / 字符串项原样 |
| Mentions | 取 `IMentionOption.Content` |
| Select | `HeaderFilterPropertySelector` 或 `ValueFilterPropertySelector`（取决于 `FilterBy` 配置） |
| Cascader | `(value) => (value as ICascaderItemInfo)?.Path`，`Path` 在 `CascaderView` 内部沿 `ICascaderOption.ParentNode` 预先拼好，例如 `"华东 / 上海"` |
| TreeView / TreeSelect | `(value) => (value as TreeViewItem)?.Header?.ToString()`，能看到 `Header` 为 `ITreeItemNode` 时走 `Header.Header` |
| ListBox | 按 `IListItemData.Content` 或 string header 取 |
| ListView | 随列定义，没有硬编码默认 |
| Transfer | 没有默认 selector，依赖用户提供或直接 `item.ToString()` |

## 7. IsFilterEnabled 的定位

`IsFilterEnabled` 在这套体系里是**用户意图开关**，不是"Filter 非空的派生信号"。这条语义决定了几件事：

1. **ControlTheme 直接消费**：`^[IsFilterEnabled=True]` 伪类 selector 和 `TemplateBinding IsFilterEnabled` 在 axaml 里正常使用，不需要绑到别的派生属性
2. **Select 的 `IsEffectiveFilterEnabled`** 是 internal DirectProperty，Select 在 Tags 模式下强制为 true，非 Tags 模式下等于 `IsFilterEnabled`；它是 Select 内部对"实际生效状态"的二次派生，不对外
3. **Filter == null 不会自动关闭搜索框**：`IsFilterEnabled=True` + `Filter=null` 的组合合法，行为是搜索框显示但过滤谓词被控件回落到默认 `Contains`
4. **Filter 非空也不会自动打开搜索框**：只设 `Filter` 而不设 `IsFilterEnabled=True` 时，搜索 UI 不显示，过滤路径不走

这是有意为之的解耦：样式开关与谓词供应是正交的两件事。

## 8. 何时该写什么

- **只是想开一个搜索框**：`IsFilterEnabled="True"`，不碰 `Filter`
- **想换匹配策略**：设 `Filter = ValueFilterFactory.BuildFilter(...)`
- **想按复合字段过滤**：设 `FilterValueSelector = record => ...`
- **想完全自定义匹配**：实现 `IValueFilter`，设到 `Filter`
- **想要树过滤策略**：TreeView / TreeSelect 设 `FilterStrategy`
- **想要文本高亮策略**：Cascader / ListBox / ListView 设 `FilterHighlightStrategy`
- **想让自己写的新控件也加入这套体系**：参见下节

## 9. 新控件接入指引

如果你在 `AtomUI.Desktop.Controls` 里开发新的可过滤控件，遵循以下模板：

### 9.1 属性定义

```csharp
public static readonly StyledProperty<IValueFilter?> FilterProperty =
    AvaloniaProperty.Register<MyControl, IValueFilter?>(nameof(Filter));

public static readonly StyledProperty<object?> FilterValueProperty =
    AvaloniaProperty.Register<MyControl, object?>(nameof(FilterValue));

public static readonly StyledProperty<DefaultFilterValueSelector?> FilterValueSelectorProperty =
    AvaloniaProperty.Register<MyControl, DefaultFilterValueSelector?>(nameof(FilterValueSelector));

public static readonly StyledProperty<bool> IsFilterEnabledProperty =
    AvaloniaProperty.Register<MyControl, bool>(nameof(IsFilterEnabled));

public IValueFilter? Filter { get => GetValue(FilterProperty); set => SetValue(FilterProperty, value); }
public object? FilterValue { get => GetValue(FilterValueProperty); set => SetValue(FilterValueProperty, value); }
public DefaultFilterValueSelector? FilterValueSelector { get => GetValue(FilterValueSelectorProperty); set => SetValue(FilterValueSelectorProperty, value); }
public bool IsFilterEnabled { get => GetValue(IsFilterEnabledProperty); set => SetValue(IsFilterEnabledProperty, value); }
```

### 9.2 默认 Filter / Selector 配置

```csharp
protected override void OnInitialized()
{
    base.OnInitialized();
    if (Filter == null)
    {
        SetCurrentValue(FilterProperty, ValueFilterFactory.BuildFilter(ValueFilterMode.Contains));
    }
    if (FilterValueSelector == null)
    {
        SetCurrentValue(FilterValueSelectorProperty, DefaultSelector);
    }
}

internal static readonly DefaultFilterValueSelector DefaultSelector = value =>
{
    if (value is MyOption option) return option.Label;
    return value?.ToString();
};
```

### 9.3 过滤调用路径

```csharp
private bool DoFilter(object item)
{
    if (Filter == null || FilterValue == null) return true;
    var selector = FilterValueSelector ?? DefaultSelector;
    return Filter.Filter(selector(item), FilterValue);
}
```

**不要**传 `this` 或 `item` 本身给 `Filter.Filter`：统一契约只有 `(value, filterValue)` 两元，把 item → 可比较值的映射全部放在 selector 里。

### 9.4 样式层

- 搜索 UI 的可见性用 `IsFilterEnabled` 驱动（ControlTheme 里 `TemplateBinding` 或属性 selector）
- 如果是平铺文本高亮需求，暴露 `FilterHighlightStrategy` / `FilterHighlightForeground`，复用 `TextBlockHighlightStrategy` 枚举
- 如果是树结构过滤需求，暴露 `FilterStrategy` / `FilterHighlightForeground`，复用 `TreeFilterStrategy` 枚举

## 10. 不在体系内的控件

- **ComboBox**：定位为"纯选择"，不加过滤能力。需要可搜索下拉用 Select 或 AutoComplete
- **Upload**：`Accepts` 是文件类型约束，属于"输入源限制"而非"候选过滤"，不走本体系
- **Pagination**：只做分页，不参与过滤

## 11. 参考实现

源码位置（便于按需对照）：

- 核心契约：`src/AtomUI.Controls.Shared/Utils/IValueFilter.cs`、`ValueFilterMode.cs`、`ValueFilterFactory.cs`、`StringValueFilters.cs`
- 典型叶子控件：`src/AtomUI.Desktop.Controls/AutoComplete/AbstractAutoComplete.cs`
- 典型层级控件：`src/AtomUI.Desktop.Controls/Cascader/CascaderView.cs` + `CascaderView.Filter.cs`
- 典型容器控件：`src/AtomUI.Desktop.Controls/ListBox/ListBox.cs`
- Transfer 双栏：`src/AtomUI.Desktop.Controls/Transfer/AbstractTransfer.cs`
