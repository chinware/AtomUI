# Mentions 自定义样式指南

Mentions 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Mentions 的公共属性来控制外观：

```xml
<!-- 不同样式变体 -->
<atom:Mentions StyleVariant="Outline" PlaceholderText="Outlined" />
<atom:Mentions StyleVariant="Filled" PlaceholderText="Filled" />
<atom:Mentions StyleVariant="Borderless" PlaceholderText="Borderless" />
<atom:Mentions StyleVariant="Underlined" PlaceholderText="Underlined" />

<!-- 不同验证状态 -->
<atom:Mentions Status="Error" DefaultValue="@afc163" />
<atom:Mentions Status="Warning" DefaultValue="@afc163" />

<!-- 弹窗位置 -->
<atom:Mentions Placement="Top" />
<atom:Mentions Placement="Bottom" />

<!-- 尺寸 -->
<atom:Mentions SizeType="Large" />
<atom:Mentions SizeType="Small" />

<!-- 允许清除 -->
<atom:Mentions IsAllowClear="True" />

<!-- 自动高度 -->
<atom:Mentions IsAutoSize="True" />

<!-- 多行 -->
<atom:Mentions Lines="3" />

<!-- 只读和禁用 -->
<atom:Mentions IsReadOnly="True" />
<atom:Mentions IsEnabled="False" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/MentionsShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Mentions 进行全局或局部样式覆盖：

### 全局统一样式

```xml
<Window.Styles>
    <!-- 所有 Mentions 使用统一样式变体 -->
    <Style Selector="atom|Mentions">
        <Setter Property="StyleVariant" Value="Filled" />
        <Setter Property="IsAllowClear" Value="True" />
    </Style>
</Window.Styles>
```

### 按验证状态定制

```xml
<!-- 错误状态的额外视觉提示 -->
<Style Selector="atom|Mentions[Status=Error]">
    <Setter Property="PlaceholderText" Value="请修正输入内容" />
</Style>
```

### 弹窗打开时的样式

```xml
<!-- 弹窗打开时的额外样式 -->
<Style Selector="atom|Mentions:candidateopen">
    <Setter Property="Opacity" Value="1" />
</Style>
```

---

## 3. 自定义候选项模板

通过 `OptionTemplate` 属性自定义候选项的显示模板：

```xml
<atom:Mentions>
    <atom:Mentions.OptionTemplate>
        <DataTemplate x:DataType="atom:IMentionOption">
            <StackPanel Orientation="Horizontal" Spacing="8">
                <Border Width="24" Height="24" CornerRadius="12"
                        Background="#1677ff">
                    <TextBlock Text="{Binding Header, 
                                Converter={x:Static StringConverters.InitialLetter}}"
                               Foreground="White"
                               HorizontalAlignment="Center"
                               VerticalAlignment="Center"
                               FontSize="12" />
                </Border>
                <TextBlock Text="{Binding Header}" VerticalAlignment="Center" />
            </StackPanel>
        </DataTemplate>
    </atom:Mentions.OptionTemplate>
</atom:Mentions>
```

默认模板为简单的 TextBlock：

```xml
<DataTemplate x:DataType="atom:IMentionOption">
    <TextBlock Text="{Binding Header}" TextTrimming="CharacterEllipsis" />
</DataTemplate>
```

---

## 4. 多触发前缀配合动态数据源

通过 `CandidateTriggered` 事件根据不同触发字符切换数据源：

```xml
<atom:Mentions TriggerPrefix="{Binding MentionTriggers}"
               CandidateTriggered="HandleCandidateTriggered"
               PlaceholderText="input @ to mention people, # to mention tag" />
```

```csharp
private void HandleCandidateTriggered(object? sender, MentionCandidateTriggeredEventArgs e)
{
    if (sender is Mentions mentions)
    {
        if (e.TriggerChar == "@")
        {
            mentions.OptionsSource = new[]
            {
                new MentionOption { Header = "afc163", Value = "afc163" },
                new MentionOption { Header = "zombieJ", Value = "zombieJ" },
            };
        }
        else if (e.TriggerChar == "#")
        {
            mentions.OptionsSource = new[]
            {
                new MentionOption { Header = "1.0", Value = "1.0" },
                new MentionOption { Header = "2.0", Value = "2.0" },
            };
        }
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/MentionsShowCase.axaml.cs` 中 "Customize Trigger Token" 示例。

---

## 5. 异步加载候选项

通过实现 `IMentionOptionsAsyncLoader` 接口实现异步加载：

```xml
<atom:Mentions OptionsAsyncLoader="{Binding MentionOptionAsyncLoader}" />
```

```csharp
public class MentionOptionsAsyncLoader : IMentionOptionsAsyncLoader
{
    public async Task<MentionOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
    {
        // 模拟网络请求延迟
        await Task.Delay(TimeSpan.FromMilliseconds(600), token);

        var options = new List<IMentionOption>
        {
            new MentionOption { Header = "User1", Value = "user1" },
            new MentionOption { Header = "User2", Value = "user2" },
        };

        return new MentionOptionsLoadResult
        {
            StatusCode = RpcStatusCode.Success,
            Data = options
        };
    }
}
```

加载过程中弹窗内会自动显示 `Spin` 加载指示器。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/ViewModels/DataEntry/MentionsViewModel.cs` 中的 `MentionOptionsAsyncLoader` 实现。

---

## 6. 在表单中使用

Mentions 实现了 `IFormItemAware` 接口，可直接参与 Form 验证：

```xml
<atom:Form>
    <atom:FormItem Label="评论" Required="True">
        <atom:Mentions PlaceholderText="@someone" />
    </atom:FormItem>
</atom:Form>
```

---

## 7. 控制弹窗行为

```xml
<!-- 禁用弹窗动画 -->
<atom:Mentions IsMotionEnabled="False" />

<!-- 限制显示候选项数量 -->
<atom:Mentions DisplayCandidateCount="5" />

<!-- 弹窗延迟打开 -->
<atom:Mentions MinimumPopulateDelay="00:00:00.300" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Mentions` 语法引用 `atom` XML 命名空间下的 `Mentions` 类型，其中 `|` 是命名空间分隔符。

### 按样式变体选择

| 选择器 | 说明 |
|---|---|
| `atom\|Mentions` | 匹配所有 Mentions 实例 |
| `atom\|Mentions[StyleVariant=Outline]` | 匹配 Outline 样式变体 |
| `atom\|Mentions[StyleVariant=Filled]` | 匹配 Filled 样式变体 |
| `atom\|Mentions[StyleVariant=Borderless]` | 匹配 Borderless 样式变体 |
| `atom\|Mentions[StyleVariant=Underlined]` | 匹配 Underlined 样式变体 |

### 按验证状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|Mentions[Status=Error]` | 匹配错误状态 |
| `atom\|Mentions[Status=Warning]` | 匹配警告状态 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Mentions[SizeType=Large]` | 匹配大号 |
| `atom\|Mentions[SizeType=Small]` | 匹配小号 |

### 按伪类选择

| 选择器 | 说明 |
|---|---|
| `atom\|Mentions:candidateopen` | 匹配候选弹窗打开状态 |
| `atom\|Mentions:pointerover` | 鼠标悬浮 |
| `atom\|Mentions:disabled` | 禁用状态 |
| `atom\|Mentions:focus` | 获得焦点 |
| `atom\|Mentions:focus-visible` | 键盘焦点 |

### 模板内部选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Mentions /template/ atom\|MentionTextArea#PART_TextArea` | 访问内部文本输入区域 |
| `atom\|Mentions /template/ atom\|Popup#PART_Popup` | 访问候选弹窗 |
| `atom\|Mentions /template/ Border#PopupFrame` | 访问弹窗框架 |
| `atom\|Mentions /template/ atom\|CandidateList#PART_CandidateList` | 访问候选列表 |
| `atom\|Mentions /template/ atom\|Spin#LoadingIndicator` | 访问加载指示器 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Mentions[StyleVariant=Outline]:candidateopen` | Outline 样式 + 弹窗打开 |
| `atom\|Mentions[Status=Error]:not(:disabled)` | 非禁用的错误状态 |
| `atom\|Mentions[SizeType=Large][StyleVariant=Filled]` | 大号填充样式 |
