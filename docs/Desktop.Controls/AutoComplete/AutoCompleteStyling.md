# AutoComplete 自定义样式指南

AutoComplete 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 AutoComplete 的公共属性来控制外观：

```xml
<!-- 不同样式变体 -->
<atom:AutoComplete StyleVariant="Outline" PlaceholderText="Outline" Width="200" />
<atom:AutoComplete StyleVariant="Filled" PlaceholderText="Filled" Width="200" />
<atom:AutoComplete StyleVariant="Borderless" PlaceholderText="Borderless" Width="200" />
<atom:AutoComplete StyleVariant="Underlined" PlaceholderText="Underlined" Width="200" />

<!-- 不同尺寸 -->
<atom:AutoComplete SizeType="Large" PlaceholderText="Large" Width="200" />
<atom:AutoComplete SizeType="Middle" PlaceholderText="Middle" Width="200" />
<atom:AutoComplete SizeType="Small" PlaceholderText="Small" Width="200" />

<!-- 验证状态 -->
<atom:AutoComplete Status="Error" Width="200" />
<atom:AutoComplete Status="Warning" Width="200" />

<!-- 允许清除 + 自定义清除图标 -->
<atom:AutoComplete IsAllowClear="True" Width="200" />
<atom:AutoComplete IsAllowClear="True"
                   ClearIcon="{antdicons:AntDesignIconProvider CloseSquareFilled}"
                   Width="200" />

<!-- 搜索型 -->
<atom:AutoCompleteSearchEdit SizeType="Large" SearchButtonStyle="Primary"
                             PlaceholderText="input here" Width="300" />

<!-- 多行文本型 -->
<atom:AutoCompleteTextArea Lines="5" IsAllowClear="True"
                           PlaceholderText="input here" Width="300" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 AutoComplete 进行全局或局部样式覆盖：

### 全局统一宽度

```xml
<Window.Styles>
    <Style Selector="atom|AutoComplete">
        <Setter Property="Width" Value="250" />
        <Setter Property="Margin" Value="0 5" />
    </Style>
</Window.Styles>
```

### 按变体定制样式

```xml
<Style Selector="atom|AutoComplete[StyleVariant=Filled]">
    <Setter Property="Margin" Value="0 8" />
</Style>
```

### 按验证状态定制样式

```xml
<!-- Error 状态时增加粗体 -->
<Style Selector="atom|AutoComplete[Status=Error]">
    <Setter Property="FontWeight" Value="Bold" />
</Style>
```

### 使用伪类选择器

```xml
<!-- 候选列表打开时的自定义样式 -->
<Style Selector="atom|AutoComplete:candidateopen">
    <Setter Property="Opacity" Value="0.95" />
</Style>

<!-- 禁用状态 -->
<Style Selector="atom|AutoComplete:disabled">
    <Setter Property="Opacity" Value="0.5" />
</Style>
```

---

## 3. 自定义候选项模板

通过 `OptionTemplate` 属性自定义候选项的渲染方式：

### 简单文本模板（默认）

默认模板已内置，展示 `Header` 文本：

```xml
<atom:AutoComplete>
    <atom:AutoComplete.OptionTemplate>
        <DataTemplate x:DataType="atom:IAutoCompleteOption">
            <TextBlock Text="{Binding Header}" TextTrimming="CharacterEllipsis" />
        </DataTemplate>
    </atom:AutoComplete.OptionTemplate>
</atom:AutoComplete>
```

### 多列信息模板

```xml
<atom:AutoCompleteSearchEdit Width="300" SizeType="Large" SearchButtonStyle="Primary">
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

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` 中 "Lookup-Patterns" 示例。

---

## 4. 通过 ControlTheme 完全替换主题

如果需要彻底替换 AutoComplete 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomAutoComplete" TargetType="atom:AutoComplete">
    <Setter Property="Template">
        <ControlTemplate>
            <Panel>
                <!-- 自定义输入框 -->
                <atom:AutoCompleteLineEditBox Name="PART_TextBox"
                    PlaceholderText="{TemplateBinding PlaceholderText}"
                    SizeType="{TemplateBinding SizeType}" />
                <!-- 自定义弹出层 -->
                <atom:Popup Name="PART_Popup"
                    Placement="{TemplateBinding PopupPlacement}">
                    <Border>
                        <atom:CandidateList Name="PART_CandidateList"
                            SelectionMode="Single"
                            ItemTemplate="{TemplateBinding OptionTemplate}" />
                    </Border>
                </atom:Popup>
            </Panel>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:AutoComplete Theme="{StaticResource MyCustomAutoComplete}" />
```

> ⚠️ 注意：完全替换 ControlTheme 需要确保模板中包含 `PART_TextBox`、`PART_Popup`、`PART_CandidateList` 三个必需部件，否则控件行为将不正常。

---

## 5. 自定义过滤模式

### 在 AXAML 中使用内置过滤器

```xml
<!-- 包含匹配（不区分大小写） -->
<atom:AutoComplete Filter="{atom:ValueFilterProvider Contains}" />

<!-- 前缀匹配（区分大小写） -->
<atom:AutoComplete Filter="{atom:ValueFilterProvider StartsWithCaseSensitive}" />

<!-- 完全匹配 -->
<atom:AutoComplete Filter="{atom:ValueFilterProvider Equals}" />
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/AutoCompleteShowCase.axaml` 中 "Non-case-sensitive AutoComplete" 示例。

### 在代码中实现自定义过滤器

```csharp
public class PinyinFilter : IValueFilter
{
    public ValueFilterMode Mode => ValueFilterMode.Custom;
    
    public bool Filter(object? value, object? filterValue)
    {
        // 自定义拼音匹配逻辑
        var text = value?.ToString() ?? "";
        var keyword = filterValue?.ToString() ?? "";
        return PinyinHelper.Contains(text, keyword);
    }
}

// 使用
autoComplete.Filter = new PinyinFilter();
```

---

## 6. 控制动画行为

```xml
<!-- 禁用过渡动画（弹出/关闭不再有动画效果） -->
<atom:AutoComplete IsMotionEnabled="False" PlaceholderText="No Animation" Width="200" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|AutoComplete` 语法引用 `atom` XML 命名空间下的 `AutoComplete` 类型，其中 `|` 是命名空间分隔符。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|AutoComplete` | 匹配标准自动补全（内嵌 LineEdit） |
| `atom\|AutoCompleteSearchEdit` | 匹配搜索型自动补全（内嵌 SearchEdit） |
| `atom\|AutoCompleteTextArea` | 匹配多行文本自动补全（内嵌 TextArea） |
| `atom\|AbstractAutoComplete` | 匹配所有 AutoComplete 变体的基类（使用 `:is` 选择器） |

### 按样式变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|AutoComplete[StyleVariant=Outline]` | 匹配默认带边框样式 |
| `atom\|AutoComplete[StyleVariant=Filled]` | 匹配填充背景样式 |
| `atom\|AutoComplete[StyleVariant=Borderless]` | 匹配无边框样式 |
| `atom\|AutoComplete[StyleVariant=Underlined]` | 匹配下划线样式 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|AutoComplete[SizeType=Large]` | 匹配大号自动补全 |
| `atom\|AutoComplete[SizeType=Middle]` | 匹配中号自动补全（默认） |
| `atom\|AutoComplete[SizeType=Small]` | 匹配小号自动补全 |

### 按验证状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|AutoComplete[Status=Error]` | 匹配错误状态 |
| `atom\|AutoComplete[Status=Warning]` | 匹配警告状态 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|AutoComplete:candidateopen` | 匹配候选列表打开状态 |
| `atom\|AutoComplete:pointerover` | 匹配鼠标悬浮状态 |
| `atom\|AutoComplete:disabled` | 匹配禁用状态 |
| `atom\|AutoComplete:focus` | 匹配获得焦点状态 |
| `atom\|AutoComplete:focus-visible` | 匹配通过键盘获得焦点的状态 |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|AutoComplete[IsAllowClear=True]` | 匹配启用清除按钮的实例 |
| `atom\|AutoComplete[IsPopupMatchSelectWidth=True]` | 匹配弹出层等宽模式（默认） |
| `atom\|AutoComplete[IsPopupMatchSelectWidth=False]` | 匹配弹出层非等宽模式 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|AutoComplete[Status=Error]:not(:disabled)` | 非禁用状态的错误态自动补全 |
| `atom\|AutoComplete[SizeType=Large]:candidateopen` | 大号且候选列表打开的自动补全 |
| `atom\|AutoComplete /template/ atom\|Popup#PART_Popup` | 访问模板内的弹出层部件 |
| `atom\|AutoComplete /template/ Border#PopupFrame` | 访问模板内的弹出层框架 Border |
