# Select 自定义样式指南

Select 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Select 的公共属性来控制外观：

```xml
<!-- 不同选择模式 -->
<atom:Select Mode="Single" PlaceholderText="Please select">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>

<atom:Select Mode="Multiple" PlaceholderText="Select items" IsFilterEnabled="True">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>

<atom:Select Mode="Tags" PlaceholderText="Tags Mode">
    <atom:SelectOption Header="Tag1" Content="tag1" />
</atom:Select>

<!-- 不同样式变体 -->
<atom:Select StyleVariant="Outline" PlaceholderText="Outlined" />
<atom:Select StyleVariant="Filled" PlaceholderText="Filled" />
<atom:Select StyleVariant="Borderless" PlaceholderText="Borderless" />

<!-- 不同尺寸 -->
<atom:Select SizeType="Large" PlaceholderText="Large" />
<atom:Select SizeType="Middle" PlaceholderText="Middle" />
<atom:Select SizeType="Small" PlaceholderText="Small" />

<!-- 验证状态 -->
<atom:Select Status="Error" PlaceholderText="Error" />
<atom:Select Status="Warning" PlaceholderText="Warning" />

<!-- 允许清空 -->
<atom:Select IsAllowClear="True" PlaceholderText="Clearable" />

<!-- 搜索过滤 -->
<atom:Select IsFilterEnabled="True" PlaceholderText="Searchable" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Select 进行全局或局部样式覆盖：

### 全局统一样式

```xml
<Window.Styles>
    <Style Selector="atom|Select">
        <Setter Property="Margin" Value="5" />
        <Setter Property="HorizontalAlignment" Value="Stretch" />
    </Style>
</Window.Styles>
```

### 按模式定制

```xml
<!-- 多选模式下统一启用搜索 -->
<Style Selector="atom|Select[Mode=Multiple]">
    <Setter Property="IsFilterEnabled" Value="True" />
</Style>

<!-- 标签模式下设置最大选择数 -->
<Style Selector="atom|Select[Mode=Tags]">
    <Setter Property="MaxCount" Value="10" />
</Style>
```

### 按尺寸定制

```xml
<!-- 大号 Select 使用更大的字体 -->
<Style Selector="atom|Select[SizeType=Large]">
    <Setter Property="FontSize" Value="16" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 下拉打开时的边框颜色 -->
<Style Selector="atom|Select:dropdownopen">
    <!-- 通过 AddOnDecoratedBox 样式控制边框 -->
</Style>

<!-- 错误状态的自定义样式 -->
<Style Selector="atom|Select:error">
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<!-- 禁用状态 -->
<Style Selector="atom|Select:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Select 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomSelect" TargetType="atom:Select">
    <Setter Property="Template">
        <ControlTemplate>
            <!-- 自定义模板... -->
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:Select Theme="{StaticResource MyCustomSelect}" />
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的 AddOnDecoratedBox 装饰、下拉动画、候选列表等功能。建议优先使用 Style 覆盖。

---

## 4. 自定义选项渲染

通过 `OptionTemplate` 自定义下拉选项的展示方式：

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

自定义选项类型需继承 `SelectOption`：

```csharp
public record CustomOption : SelectOption
{
    public string? Description { get; init; }
    public string? Emoji { get; init; }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Custom dropdown options" 示例。

---

## 5. 自定义搜索过滤

通过实现 `IValueFilter` 接口自定义过滤逻辑：

```csharp
public class CustomFilter : IValueFilter
{
    public bool Filter(object? value, object? filterValue)
    {
        if (value is string valueStr && filterValue is string filterValueStr)
        {
            return valueStr.Contains(filterValueStr, StringComparison.Ordinal);
        }
        return false;
    }
}
```

```xml
<!-- 使用内置的 Contains 过滤器 -->
<atom:Select IsFilterEnabled="True"
             Filter="{atom:ValueFilterProvider Contains}">
    <atom:SelectOption Header="Jack" Content="jack" />
    <atom:SelectOption Header="Lucy" Content="lucy" />
</atom:Select>
```

或在 code-behind 中设置自定义过滤器：

```csharp
CustomSearchSelect.Filter = new CustomFilter();
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml.cs` 中的 `CustomFilter` 实现。

---

## 6. 前缀和后缀内容

通过 `ContentLeftAddOn` 和 `ContentRightAddOn` 添加内部附加内容：

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

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/SelectShowCase.axaml` 中 "Prefix and Suffix" 示例。

---

## 7. 控制动画行为

```xml
<!-- 禁用打开/关闭动画 -->
<atom:Select IsMotionEnabled="False" PlaceholderText="No animation">
    <atom:SelectOption Header="Jack" Content="jack" />
</atom:Select>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Select` 语法引用 `atom` XML 命名空间下的 `Select` 类型，其中 `|` 是命名空间分隔符。

### 按模式选择

| 选择器 | 说明 |
|---|---|
| `atom\|Select` | 匹配所有 Select 实例 |
| `atom\|Select[Mode=Single]` | 匹配单选模式 |
| `atom\|Select[Mode=Multiple]` | 匹配多选模式 |
| `atom\|Select[Mode=Tags]` | 匹配标签模式 |

### 按样式变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|Select[StyleVariant=Outline]` | 匹配 Outlined 变体 |
| `atom\|Select[StyleVariant=Filled]` | 匹配 Filled 变体 |
| `atom\|Select[StyleVariant=Borderless]` | 匹配 Borderless 变体 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Select[SizeType=Large]` | 匹配大号 Select |
| `atom\|Select[SizeType=Middle]` | 匹配中号 Select（默认） |
| `atom\|Select[SizeType=Small]` | 匹配小号 Select |

### 按状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|Select:dropdownopen` | 下拉面板打开状态 |
| `atom\|Select:error` | 错误验证状态 |
| `atom\|Select:warning` | 警告验证状态 |
| `atom\|Select:pointerover` | 鼠标悬浮状态 |
| `atom\|Select:disabled` | 禁用状态 |
| `atom\|Select:pressed` | 按下状态 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Select[Mode=Single]:not(:disabled)` | 非禁用的单选 Select |
| `atom\|Select[Mode=Multiple]:dropdownopen` | 多选模式且下拉打开 |
| `atom\|Select[SizeType=Large][Mode=Multiple]` | 大号多选 Select |
| `atom\|Select /template/ atom\|SelectAddOnDecoratedBox` | 访问模板内的装饰容器 |
| `atom\|Select /template/ Border#PopupFrame` | 访问模板内的弹出框架 |
