# Message 使用文档

本文档介绍 AtomUI Message 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/MessageShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Message，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Message, MessageCard, WindowMessageManager
using Avalonia.Controls;          // TopLevel
```

---

## 1. 初始化 WindowMessageManager

Message 系统使用命令式 API（而非 AXAML 声明式），需要先在代码中初始化 `WindowMessageManager`：

```csharp
public partial class MyView : UserControl
{
    private WindowMessageManager? _messageManager;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var topLevel = TopLevel.GetTopLevel(this);
        _messageManager = new WindowMessageManager(topLevel)
        {
            MaxItems = 10  // 最多同时显示 10 条消息
        };
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _messageManager?.Dispose();  // 必须释放资源
        _messageManager = null;
    }
}
```

**关键要点**：
- `WindowMessageManager` 需要一个 `TopLevel` 实例作为宿主，它会自动挂载到该窗口的 `AdornerLayer` 上
- 在 `OnDetachedFromVisualTree` 中必须调用 `Dispose()` 释放资源，避免内存泄漏
- 一个 `TopLevel` 通常只需要一个 `WindowMessageManager` 实例

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/MessageShowCase.axaml.cs`

---

## 2. 基本用法

最简单的使用方式——显示一条默认类型的消息：

```xml
<atom:Button ButtonType="Primary" Click="ShowSimpleMessage">
    Display normal message
</atom:Button>
```

```csharp
private void ShowSimpleMessage(object? sender, RoutedEventArgs e)
{
    _messageManager?.Show(new Message("Hello, AtomUI/Avalonia!"));
}
```

默认使用 `MessageType.Information` 类型，显示蓝色 `InfoCircleFilled` 图标，5 秒后自动关闭。

---

## 3. 不同消息类型

通过 `MessageType` 参数设置不同的消息类型，每种类型有对应的图标和颜色：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Button ButtonType="Default" Click="ShowSuccessMessage">Success</atom:Button>
    <atom:Button ButtonType="Default" Click="ShowInfoMessage">Info</atom:Button>
    <atom:Button ButtonType="Default" Click="ShowWarningMessage">Warning</atom:Button>
    <atom:Button ButtonType="Default" Click="ShowErrorMessage">Error</atom:Button>
</StackPanel>
```

```csharp
private void ShowSuccessMessage(object? sender, RoutedEventArgs e)
{
    _messageManager?.Show(new Message(
        type: MessageType.Success,
        content: "This is a success message."
    ));
}

private void ShowInfoMessage(object? sender, RoutedEventArgs e)
{
    _messageManager?.Show(new Message(
        type: MessageType.Information,
        content: "This is a information message."
    ));
}

private void ShowWarningMessage(object? sender, RoutedEventArgs e)
{
    _messageManager?.Show(new Message(
        type: MessageType.Warning,
        content: "This is a warning message."
    ));
}

private void ShowErrorMessage(object? sender, RoutedEventArgs e)
{
    _messageManager?.Show(new Message(
        type: MessageType.Error,
        content: "This is a error message."
    ));
}
```

**类型对应关系**：
- `Information` → 蓝色 `InfoCircleFilled` 图标
- `Success` → 绿色 `CheckCircleFilled` 图标
- `Warning` → 黄色 `ExclamationCircleFilled` 图标
- `Error` → 红色 `CloseCircleFilled` 图标

---

## 4. Loading 消息

Loading 类型消息显示旋转的加载图标，适用于异步操作进行中的提示：

```xml
<atom:Button ButtonType="Default" Click="ShowLoadingMessage">
    Display a loading indicator
</atom:Button>
```

```csharp
private void ShowLoadingMessage(object? sender, RoutedEventArgs e)
{
    _messageManager?.Show(new Message(
        type: MessageType.Loading,
        content: "Action in progress..."
    ));
}
```

Loading 消息的 `LoadingOutlined` 图标会自动播放旋转动画。

---

## 5. 自定义持续时间

通过 `expiration` 参数控制消息的显示时长：

```csharp
// 10 秒后关闭
_messageManager?.Show(new Message(
    content: "This message will last 10 seconds",
    expiration: TimeSpan.FromSeconds(10)
));

// 永不自动关闭（需要手动关闭）
_messageManager?.Show(new Message(
    content: "This message won't auto-close",
    expiration: TimeSpan.Zero
));

// 快速消失（1 秒）
_messageManager?.Show(new Message(
    content: "Quick flash",
    type: MessageType.Success,
    expiration: TimeSpan.FromSeconds(1)
));
```

---

## 6. 关闭回调（链式消息）

通过 `onClose` 参数在消息关闭后执行回调，可以实现链式消息——一条消息关闭后自动显示下一条：

```xml
<atom:Button ButtonType="Default" Click="ShowSequentialMessage">
    Display sequential messages
</atom:Button>
```

```csharp
private void ShowSequentialMessage(object? sender, RoutedEventArgs e)
{
    _messageManager?.Show(new Message(
        type: MessageType.Loading,
        content: "Action in progress...",
        expiration: TimeSpan.FromSeconds(2.5),
        onClose: () =>
        {
            _messageManager?.Show(new Message(
                type: MessageType.Success,
                expiration: TimeSpan.FromSeconds(2.5),
                content: "Loading finished",
                onClose: () =>
                {
                    _messageManager?.Show(new Message(
                        type: MessageType.Information,
                        expiration: TimeSpan.FromSeconds(2.5),
                        content: "Loading finished"
                    ));
                }
            ));
        }
    ));
}
```

这个示例展示了典型的异步操作流程：**Loading → Success → Info**，通过 `onClose` 回调串联。

---

## 7. 自定义图标

通过 `icon` 参数使用自定义 Ant Design 图标替代默认图标：

```csharp
using AtomUI.Icons.AntDesign;

// 使用自定义图标
_messageManager?.Show(new Message(
    content: "This message has a custom icon",
    icon: new SmileFilled()
));
```

---

## 8. 动态更新消息内容

`Message` 实现了 `INotifyPropertyChanged`，可以在消息显示后动态更新内容：

```csharp
var message = new Message(
    content: "Uploading... 0%",
    type: MessageType.Loading,
    expiration: TimeSpan.Zero  // 不自动关闭
);

_messageManager?.Show(message);

// 后续更新内容
Dispatcher.UIThread.InvokeAsync(async () =>
{
    for (int i = 1; i <= 100; i++)
    {
        await Task.Delay(50);
        message.Content = $"Uploading... {i}%";
    }
    message.Content = "Upload complete!";
});
```

---

## 9. 附加样式类

`Show` 方法的 `classes` 参数允许为单条消息附加自定义样式类：

```csharp
// 附加自定义样式类
_messageManager?.Show(
    new Message("Important notification!", MessageType.Warning),
    classes: new[] { "important", "bold" }
);
```

在 AXAML 中定义对应的样式：

```xml
<Window.Styles>
    <Style Selector="atom|MessageCard.important /template/ Border#PART_Frame">
        <Setter Property="BorderBrush" Value="Orange" />
        <Setter Property="BorderThickness" Value="2" />
    </Style>
    <Style Selector="atom|MessageCard.bold /template/ SelectableTextBlock#PART_Message">
        <Setter Property="FontWeight" Value="Bold" />
    </Style>
</Window.Styles>
```

---

## 10. 控制最大显示数量

通过 `MaxItems` 属性限制同时显示的消息数量，超出时最早的消息会被自动关闭：

```csharp
_messageManager = new WindowMessageManager(topLevel)
{
    MaxItems = 3  // 最多同时显示 3 条
};
```

---

## 常见使用模式

### 表单提交反馈

```csharp
private async void OnSubmit(object? sender, RoutedEventArgs e)
{
    try
    {
        _messageManager?.Show(new Message(
            content: "Submitting...",
            type: MessageType.Loading,
            expiration: TimeSpan.Zero
        ));

        await _service.SubmitAsync();

        _messageManager?.Show(new Message(
            content: "Submit successful!",
            type: MessageType.Success
        ));
    }
    catch (Exception ex)
    {
        _messageManager?.Show(new Message(
            content: $"Submit failed: {ex.Message}",
            type: MessageType.Error,
            expiration: TimeSpan.FromSeconds(10)
        ));
    }
}
```

### MVVM 模式下的使用

```csharp
// ViewModel 中通过接口引用
public class MyViewModel : ReactiveObject
{
    private readonly IMessageManager _messageManager;

    public MyViewModel(IMessageManager messageManager)
    {
        _messageManager = messageManager;
    }

    public void ShowSuccess(string content)
    {
        _messageManager.Show(new Message(content, MessageType.Success));
    }
}
```

### 与操作确认结合

```csharp
private void OnDelete(object? sender, RoutedEventArgs e)
{
    // 先显示 Loading，操作完成后显示结果
    _messageManager?.Show(new Message(
        content: "Deleting...",
        type: MessageType.Loading,
        expiration: TimeSpan.FromSeconds(2),
        onClose: () =>
        {
            _messageManager?.Show(new Message(
                content: "Deleted successfully",
                type: MessageType.Success
            ));
        }
    ));
}
```
