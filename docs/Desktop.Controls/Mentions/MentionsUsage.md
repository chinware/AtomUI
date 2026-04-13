# Mentions 使用文档

本文档介绍 AtomUI Mentions 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/MentionsShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Mentions，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;           // Mentions、MentionOption 等
using AtomUI.Desktop.Controls.DataLoad;  // IMentionOptionsAsyncLoader
using AtomUI.Controls;                   // SizeType、InputControlStatus 等共享类型
```

---

## 1. 基本用法

提供候选项数据源，输入 `@` 触发候选列表：

```xml
<atom:Mentions Name="BasicMentions"
               HorizontalAlignment="Stretch"
               DefaultValue="@afc163" />
```

```csharp
// Code-behind：设置候选项数据源
BasicMentions.OptionsSource = new List<IMentionOption>
{
    new MentionOption { Header = "afc163", Value = "afc163" },
    new MentionOption { Header = "zombieJ", Value = "zombieJ" },
    new MentionOption { Header = "yesmeck", Value = "yesmeck" }
};
```

或使用 ReactiveUI 绑定：

```csharp
// ViewModel
public class MentionsViewModel : ReactiveObject
{
    private List<IMentionOption>? _basicMentionOptions;
    public List<IMentionOption>? BasicMentionOptions
    {
        get => _basicMentionOptions;
        set => this.RaiseAndSetIfChanged(ref _basicMentionOptions, value);
    }
}

// View（WhenActivated）
this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v => v.BasicMentions.OptionsSource);
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/MentionsShowCase.axaml.cs`

---

## 2. 样式变体

通过 `StyleVariant` 属性设置四种样式变体：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:Mentions HorizontalAlignment="Stretch" StyleVariant="Outline"
                   PlaceholderText="Outlined" />
    <atom:Mentions HorizontalAlignment="Stretch" StyleVariant="Filled"
                   PlaceholderText="Filled" />
    <atom:Mentions HorizontalAlignment="Stretch" StyleVariant="Borderless"
                   PlaceholderText="Borderless" />
    <atom:Mentions HorizontalAlignment="Stretch" StyleVariant="Underlined"
                   PlaceholderText="Underlined" />
</StackPanel>
```

**使用场景指引**：
- **Outline**（默认）：最常用，适合表单场景
- **Filled**：有背景填充，适合需要更强视觉层次的场景
- **Borderless**：无边框，适合嵌入在其他控件内部
- **Underlined**：下划线风格，适合简洁的行内编辑

---

## 3. 异步加载候选项

通过实现 `IMentionOptionsAsyncLoader` 接口，从远程服务器异步加载候选项：

```xml
<atom:Mentions HorizontalAlignment="Stretch"
               OptionsAsyncLoader="{Binding MentionOptionAsyncLoader}" />
```

```csharp
public class MentionOptionsAsyncLoader : IMentionOptionsAsyncLoader
{
    public async Task<MentionOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
    {
        // 模拟网络请求
        await Task.Delay(TimeSpan.FromMilliseconds(600), token);

        if (token.IsCancellationRequested)
        {
            return new MentionOptionsLoadResult { StatusCode = RpcStatusCode.Cancelled };
        }

        var count = Random.Shared.Next(3, 8);
        var options = new List<IMentionOption>();
        for (int i = 0; i < count; i++)
        {
            var name = $"User{Random.Shared.Next(1000, 9999)}";
            options.Add(new MentionOption { Header = name, Value = name });
        }

        return new MentionOptionsLoadResult
        {
            StatusCode = RpcStatusCode.Success,
            Data = options
        };
    }
}
```

加载过程中，弹窗内会自动显示旋转的 `Spin` 加载指示器。

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/ViewModels/DataEntry/MentionsViewModel.cs`

---

## 4. 自定义触发前缀

通过 `TriggerPrefix` 属性配置多个触发字符，通过 `CandidateTriggered` 事件根据触发字符动态切换数据源：

```xml
<atom:Mentions HorizontalAlignment="Stretch"
               TriggerPrefix="{Binding MentionTriggers}"
               CandidateTriggered="HandleCandidateTriggered"
               PlaceholderText="input @ to mention people, # to mention tag" />
```

```csharp
// ViewModel
viewModel.MentionTriggers = ["@", "#"];

// Code-behind
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
                new MentionOption { Header = "yesmeck", Value = "yesmeck" }
            };
        }
        else if (e.TriggerChar == "#")
        {
            mentions.OptionsSource = new[]
            {
                new MentionOption { Header = "1.0", Value = "1.0" },
                new MentionOption { Header = "2.0", Value = "2.0" },
                new MentionOption { Header = "3.0", Value = "3.0" }
            };
        }
    }
}
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/MentionsShowCase.axaml` 中 "Customize Trigger Token" 示例。

---

## 5. 禁用和只读

```xml
<!-- 禁用 -->
<atom:Mentions IsEnabled="False" PlaceholderText="this is disabled Mentions" />

<!-- 只读 -->
<atom:Mentions IsReadOnly="True" PlaceholderText="this is readOnly Mentions" />
```

禁用状态下控件变为灰色调，不响应任何交互。只读状态下文本不可编辑，但内容仍可选择复制。

---

## 6. 弹窗位置

通过 `Placement` 属性控制候选弹窗的弹出方向：

```xml
<!-- 候选列表在上方弹出 -->
<atom:Mentions Placement="Top" />

<!-- 候选列表在下方弹出（默认） -->
<atom:Mentions Placement="Bottom" />
```

---

## 7. 验证状态

通过 `Status` 属性设置验证状态：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <!-- 错误状态 -->
    <atom:Mentions Status="Error" DefaultValue="@afc163" />
    
    <!-- 警告状态 -->
    <atom:Mentions Status="Warning" DefaultValue="@afc163" />
</StackPanel>
```

验证状态会改变边框颜色：Error 为红色系，Warning 为橙色系。

---

## 8. 自动高度

通过 `IsAutoSize` 属性启用输入区域高度自适应：

```xml
<atom:Mentions IsAutoSize="True" />
```

配合 `MinLines` / `MaxLines` 限制高度范围：

```xml
<atom:Mentions IsAutoSize="True" MinLines="2" MaxLines="6" />
```

---

## 9. 允许清除

```xml
<!-- 单行可清除 -->
<atom:Mentions IsAllowClear="True" />

<!-- 多行可清除 -->
<atom:Mentions IsAllowClear="True" Lines="3" />
```

启用后，当输入框有内容时，右侧会显示清除按钮（默认为 `CloseCircleFilled` 图标），点击一键清空。

---

## 10. MVVM 数据绑定

```xml
<atom:Mentions Value="{Binding CommentText, Mode=TwoWay}"
               OptionsSource="{Binding MentionOptions}"
               PlaceholderText="输入评论..." />
```

```csharp
public class CommentViewModel : ReactiveObject
{
    [Reactive]
    public string? CommentText { get; set; }

    [Reactive]
    public List<IMentionOption>? MentionOptions { get; set; }
}
```

---

## 常见组合模式

### 评论框

```xml
<StackPanel Spacing="8">
    <atom:Mentions Value="{Binding CommentText, Mode=TwoWay}"
                   OptionsSource="{Binding TeamMembers}"
                   IsAutoSize="True"
                   MinLines="3"
                   MaxLines="8"
                   PlaceholderText="输入评论，使用 @ 提及团队成员..." />
    <atom:Button ButtonType="Primary"
                 HorizontalAlignment="Right"
                 Command="{Binding SubmitCommentCommand}">
        发表评论
    </atom:Button>
</StackPanel>
```

### 标签输入

```xml
<atom:Mentions TriggerPrefix="{x:Array Type=x:String}"
               PlaceholderText="输入 # 添加标签"
               Split=" ">
    <!-- TriggerPrefix 设为 ["#"] -->
</atom:Mentions>
```

### 表单集成

```xml
<atom:Form>
    <atom:FormItem Label="提及" Required="True"
                   HelpText="使用 @ 提及相关人员">
        <atom:Mentions OptionsSource="{Binding TeamMembers}"
                       Status="{Binding MentionStatus}" />
    </atom:FormItem>
</atom:Form>
```

---

## 注意事项

1. **数据源必须实现 `IMentionOption`**：候选项必须是 `IMentionOption` 接口的实现，推荐使用内置的 `MentionOption` record。

2. **触发字符检测**：触发字符的检测基于光标位置向前扫描，直到遇到空白字符或控制字符为止。因此触发字符后的连续非空白输入都会作为过滤文本。

3. **窗口失活自动关闭**：当应用窗口失去焦点时，候选弹窗会自动关闭。

4. **异步加载取消**：当用户在异步加载过程中继续输入新的触发字符，前一次加载会自动取消（通过 `CancellationToken`）。

5. **`OptionsSource` 支持动态更新**：如果 `OptionsSource` 实现了 `INotifyCollectionChanged`（如 `ObservableCollection`），控件会自动响应集合变更。
