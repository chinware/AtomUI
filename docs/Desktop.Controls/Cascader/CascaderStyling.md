# Cascader 自定义样式指南

Cascader 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Cascader 的公共属性来控制外观和行为：

```xml
<!-- 基本用法 -->
<atom:Cascader PlaceholderText="Please select" IsAllowClear="True" />

<!-- 不同尺寸 -->
<atom:Cascader SizeType="Large" PlaceholderText="Large" />
<atom:Cascader SizeType="Middle" PlaceholderText="Middle" />
<atom:Cascader SizeType="Small" PlaceholderText="Small" />

<!-- 不同样式变体 -->
<atom:Cascader StyleVariant="Outline" PlaceholderText="Outlined" />
<atom:Cascader StyleVariant="Filled" PlaceholderText="Filled" />
<atom:Cascader StyleVariant="Borderless" PlaceholderText="Borderless" />
<atom:Cascader StyleVariant="Underlined" PlaceholderText="Underlined" />

<!-- 状态反馈 -->
<atom:Cascader Status="Error" PlaceholderText="Error" />
<atom:Cascader Status="Warning" PlaceholderText="Warning" />

<!-- 多选模式 -->
<atom:Cascader IsMultiple="True" ShowCheckedStrategy="ShowParent"
               PlaceholderText="Please select" />

<!-- 悬浮展开 -->
<atom:Cascader ExpandTrigger="Hover" PlaceholderText="Hover to expand" />

<!-- 可搜索 -->
<atom:Cascader IsFilterEnabled="True" PlaceholderText="Search..." />

<!-- 允许选择父节点 -->
<atom:Cascader IsAllowSelectParent="True" PlaceholderText="Any level" />

<!-- 弹出位置 -->
<atom:Cascader PopupPlacement="TopEdgeAlignedLeft" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/CascaderShowCase.axaml`

---

## 2. 自定义图标

Cascader 支持自定义后缀图标、展开箭头图标和加载图标：

```xml
<!-- 自定义后缀图标 -->
<atom:Cascader SuffixIcon="{antdicons:AntDesignIconProvider SmileOutlined}"
               PlaceholderText="Please select" />

<!-- 自定义展开箭头图标 -->
<atom:Cascader PlaceholderText="Please select">
    <atom:Cascader.ExpandIcon>
        <atom:IconTemplate>
            <antdicons:SmileOutlined />
        </atom:IconTemplate>
    </atom:Cascader.ExpandIcon>
</atom:Cascader>

<!-- 自定义前缀内容 -->
<atom:Cascader PlaceholderText="Please select">
    <atom:Cascader.ContentLeftAddOn>
        <antdicons:SmileOutlined />
    </atom:Cascader.ContentLeftAddOn>
</atom:Cascader>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/CascaderShowCase.axaml` 中 "Prefix and Suffix" 示例。

---

## 3. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Cascader 进行全局或局部样式覆盖：

### 全局统一样式

```xml
<Window.Styles>
    <Style Selector="atom|Cascader">
        <Setter Property="Margin" Value="0,5" />
        <Setter Property="IsAllowClear" Value="True" />
    </Style>
</Window.Styles>
```

### 按尺寸定制

```xml
<Style Selector="atom|Cascader[SizeType=Large]">
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

### 按状态定制

```xml
<!-- 禁用态自定义 -->
<Style Selector="atom|Cascader:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

---

## 4. 自定义选项模板

通过 `OptionTemplate` 属性自定义选项的渲染方式。默认的模板是一个 `TreeDataTemplate`：

```xml
<atom:Cascader>
    <atom:Cascader.OptionTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}"
                          x:DataType="atom:ICascaderOption">
            <atom:TextBlock Text="{Binding Header}" />
        </TreeDataTemplate>
    </atom:Cascader.OptionTemplate>
</atom:Cascader>
```

你可以扩展模板添加图标或其他内容：

```xml
<atom:Cascader>
    <atom:Cascader.OptionTemplate>
        <TreeDataTemplate ItemsSource="{Binding Children}"
                          x:DataType="atom:CascaderOption">
            <StackPanel Orientation="Horizontal" Spacing="4">
                <ContentPresenter Content="{Binding Icon}"
                                  IsVisible="{Binding Icon, Converter={x:Static ObjectConverters.IsNotNull}}" />
                <atom:TextBlock Text="{Binding Header}" />
            </StackPanel>
        </TreeDataTemplate>
    </atom:Cascader.OptionTemplate>
</atom:Cascader>
```

---

## 5. 撑满父容器

```xml
<atom:Cascader HorizontalAlignment="Stretch"
               PlaceholderText="Full width cascader" />
```

当设置 `HorizontalAlignment="Stretch"` 时，Cascader 将不再使用 Token 中的固定宽度 `ControlWidth`，而是自动撑满父容器。

---

## 6. 自定义过滤器

实现 `ICascaderItemFilter` 接口来自定义过滤逻辑：

```csharp
public class MyCustomFilter : ICascaderItemFilter
{
    public bool Filter(CascaderView cascaderView, ICascaderItemInfo cascaderItemInfo, object? filterValue)
    {
        if (filterValue == null) return true;
        var keyword = filterValue.ToString()?.ToLower();
        return cascaderItemInfo.Path.ToLower().StartsWith(keyword ?? "");
    }
}
```

```xml
<atom:Cascader IsFilterEnabled="True"
               Filter="{Binding CustomFilter}" />
```

---

## 7. 异步加载器

实现 `ICascaderItemDataLoader` 接口来提供异步数据加载：

```csharp
public class MyCascaderDataLoader : ICascaderItemDataLoader
{
    public async Task<CascaderItemLoadResult> LoadAsync(
        ICascaderOption targetNode, CancellationToken token)
    {
        // 模拟网络请求
        await Task.Delay(500, token);
        var children = new List<CascaderOption>
        {
            new() { Header = "Dynamic Child 1", IsLeaf = true },
            new() { Header = "Dynamic Child 2", IsLeaf = true }
        };
        return new CascaderItemLoadResult { IsSuccess = true, Data = children };
    }
}
```

```xml
<atom:Cascader DataLoader="{Binding MyDataLoader}"
               PlaceholderText="Load on expand" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/CascaderShowCase.axaml` 中 "Load Options Lazily" 示例。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Cascader` 语法引用 `atom` XML 命名空间下的 `Cascader` 类型，其中 `|` 是命名空间分隔符。

### 按控件选择

| 选择器 | 说明 |
|---|---|
| `atom\|Cascader` | 匹配所有 Cascader 实例 |
| `atom\|CascaderView` | 匹配级联面板视图（弹出层内的面板） |
| `atom\|CascaderViewItem` | 匹配单个级联选项项 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Cascader[SizeType=Large]` | 大号 Cascader |
| `atom\|Cascader[SizeType=Middle]` | 中号 Cascader（默认） |
| `atom\|Cascader[SizeType=Small]` | 小号 Cascader |

### 按样式变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|Cascader[StyleVariant=Outline]` | 有边框样式 |
| `atom\|Cascader[StyleVariant=Filled]` | 填充背景样式 |
| `atom\|Cascader[StyleVariant=Borderless]` | 无边框样式 |
| `atom\|Cascader[StyleVariant=Underlined]` | 下划线样式 |

### 按模式选择

| 选择器 | 说明 |
|---|---|
| `atom\|Cascader[IsMultiple=True]` | 多选模式 |
| `atom\|Cascader[IsMultiple=False]` | 单选模式 |
| `atom\|Cascader[IsFilterEnabled=True]` | 启用搜索的 Cascader |

### 按状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|Cascader:disabled` | 禁用状态 |
| `atom\|Cascader:pointerover` | 鼠标悬浮状态 |
| `atom\|Cascader[IsDropDownOpen=True]` | 下拉面板打开状态 |
| `atom\|Cascader[Status=Error]` | 错误状态 |
| `atom\|Cascader[Status=Warning]` | 警告状态 |

### CascaderViewItem 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|CascaderViewItem:selected` | 选中的选项 |
| `atom\|CascaderViewItem:expanded` | 已展开的选项 |
| `atom\|CascaderViewItem:pointerover` | 鼠标悬浮的选项 |
| `atom\|CascaderViewItem:disabled` | 禁用的选项 |
| `atom\|CascaderViewItem:checked` | 多选模式下启用了复选框的选项 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Cascader[IsMultiple=True][SizeType=Large]` | 大号多选 Cascader |
| `atom\|Cascader /template/ Border#PopupFrame` | 访问弹出框架的 Border 部件 |
| `atom\|Cascader /template/ atom\|CascaderView#PART_CascaderView` | 访问模板内的级联面板视图 |
