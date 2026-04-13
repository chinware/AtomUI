# Notification 使用文档

本文档介绍 AtomUI Notification 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/NotificationShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Notification 相关控件，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;       // WindowNotificationManager, Notification, NotificationCard
using AtomUI.Icons.AntDesign;         // 自定义图标（如 SettingOutlined）
using Avalonia.Controls;              // TopLevel
```

---

## 1. 基本用法

Notification 是通过编程式 API 调用的（而非在 AXAML 中声明），需要先创建 `WindowNotificationManager` 实例并挂载到窗口。

### 初始化 NotificationManager

```csharp
// 在控件的 OnAttachedToVisualTree 中初始化
protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnAttachedToVisualTree(e);
    var topLevel = TopLevel.GetTopLevel(this);
    _notificationManager = new WindowNotificationManager(topLevel)
    {
        MaxItems = 3  // 最多同时显示 3 条
    };
}

// 在 OnDetachedFromVisualTree 中释放资源
protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _notificationManager?.Dispose();
}
```

### 弹出简单通知

```csharp
_notificationManager?.Show(new Notification(
    "Notification Title",
    "Hello, AtomUI/Avalonia!"
));
```

对应 AXAML 按钮触发：

```xml
<atom:Button ButtonType="Primary" Click="ShowSimpleNotification">
    Show Notification
</atom:Button>
```

```csharp
private void ShowSimpleNotification(object? sender, RoutedEventArgs e)
{
    _notificationManager?.Show(new Notification(
        "Notification Title",
        "Hello, AtomUI/Avalonia!"
    ));
}
```

---

## 2. 通知类型

AtomUI 提供四种通知类型，通过 `type` 参数设置：

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <atom:Button ButtonType="Default" Click="ShowSuccessNotification">
        Success
    </atom:Button>
    <atom:Button ButtonType="Default" Click="ShowInfoNotification">
        Info
    </atom:Button>
    <atom:Button ButtonType="Default" Click="ShowWarningNotification">
        Warning
    </atom:Button>
    <atom:Button ButtonType="Default" Click="ShowErrorNotification">
        Error
    </atom:Button>
</StackPanel>
```

```csharp
private void ShowSuccessNotification(object? sender, RoutedEventArgs e)
{
    _notificationManager?.Show(new Notification(
        type: NotificationType.Success,
        title: "Notification Title",
        content: "This is the content of the notification."
    ));
}

private void ShowInfoNotification(object? sender, RoutedEventArgs e)
{
    _notificationManager?.Show(new Notification(
        type: NotificationType.Information,
        title: "Notification Title",
        content: "This is the content of the notification."
    ));
}

private void ShowWarningNotification(object? sender, RoutedEventArgs e)
{
    _notificationManager?.Show(new Notification(
        type: NotificationType.Warning,
        title: "Notification Title",
        content: "This is the content of the notification."
    ));
}

private void ShowErrorNotification(object? sender, RoutedEventArgs e)
{
    _notificationManager?.Show(new Notification(
        type: NotificationType.Error,
        title: "Notification Title",
        content: "This is the content of the notification."
    ));
}
```

**使用场景指引**：
- **Information**：一般性信息通知、系统公告
- **Success**：操作成功的反馈、任务完成
- **Warning**：需要注意的警告信息
- **Error**：操作失败、系统错误

---

## 3. 永不自动关闭

设置 `expiration: TimeSpan.Zero` 使通知仅能通过关闭按钮手动关闭：

```csharp
private void ShowNeverCloseNotification(object? sender, RoutedEventArgs e)
{
    _notificationManager?.Show(new Notification(
        expiration: TimeSpan.Zero,
        title: "Notification Title",
        content: "I will never close automatically. This is a purposely very very long description that has many many characters and words."
    ));
}
```

---

## 4. 弹出位置

支持六种弹出位置，通过 `WindowNotificationManager.Position` 属性设置。每个位置需要一个独立的 Manager 实例：

```csharp
// 初始化不同位置的管理器
_topLeftManager = new WindowNotificationManager(topLevel)
{
    Position = NotificationPosition.TopLeft,
    MaxItems = 3
};

_topManager = new WindowNotificationManager(topLevel)
{
    Position = NotificationPosition.TopCenter,
    MaxItems = 3
};

_topRightManager = new WindowNotificationManager(topLevel)
{
    Position = NotificationPosition.TopRight,
    MaxItems = 3
};

_bottomLeftManager = new WindowNotificationManager(topLevel)
{
    Position = NotificationPosition.BottomLeft,
    MaxItems = 3
};

_bottomManager = new WindowNotificationManager(topLevel)
{
    Position = NotificationPosition.BottomCenter,
    MaxItems = 3
};

_bottomRightManager = new WindowNotificationManager(topLevel)
{
    Position = NotificationPosition.BottomRight,
    MaxItems = 3
};
```

在 AXAML 中提供位置选择按钮：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button ButtonType="Primary" Click="ShowTopNotification">
            Top
        </atom:Button>
        <atom:Button ButtonType="Primary" Click="ShowBottomNotification">
            Bottom
        </atom:Button>
    </StackPanel>
    <atom:Separator />
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button ButtonType="Primary" Click="ShowTopLeftNotification">
            TopLeft
        </atom:Button>
        <atom:Button ButtonType="Primary" Click="ShowTopRightNotification">
            TopRight
        </atom:Button>
    </StackPanel>
    <atom:Separator />
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:Button ButtonType="Primary" Click="ShowBottomLeftNotification">
            BottomLeft
        </atom:Button>
        <atom:Button ButtonType="Primary" Click="ShowBottomRightNotification">
            BottomRight
        </atom:Button>
    </StackPanel>
</StackPanel>
```

```csharp
private void ShowTopNotification(object? sender, RoutedEventArgs e)
{
    _topManager?.Show(new Notification("Notification Top", "Hello, AtomUI/Avalonia!"));
}

private void ShowBottomNotification(object? sender, RoutedEventArgs e)
{
    _bottomManager?.Show(new Notification("Notification Bottom", "Hello, AtomUI/Avalonia!"));
}

private void ShowTopLeftNotification(object? sender, RoutedEventArgs e)
{
    _topLeftManager?.Show(new Notification("Notification TopLeft", "Hello, AtomUI/Avalonia!"));
}

private void ShowTopRightNotification(object? sender, RoutedEventArgs e)
{
    _topRightManager?.Show(new Notification("Notification TopRight", "Hello, AtomUI/Avalonia!"));
}

private void ShowBottomLeftNotification(object? sender, RoutedEventArgs e)
{
    _bottomLeftManager?.Show(new Notification("Notification BottomLeft", "Hello, AtomUI/Avalonia!"));
}

private void ShowBottomRightNotification(object? sender, RoutedEventArgs e)
{
    _bottomRightManager?.Show(new Notification("Notification BottomRight", "Hello, AtomUI/Avalonia!"));
}
```

> ⚠️ **注意**：不同位置的通知需要使用不同的 `WindowNotificationManager` 实例。不要在同一个 Manager 上频繁切换 `Position` 属性。

---

## 5. 自定义图标

通过 `icon` 参数替换默认的类型图标：

```csharp
private void ShowCustomIconNotification(object? sender, RoutedEventArgs e)
{
    _notificationManager?.Show(new Notification(
        "Notification Title",
        "This is the content of the notification.",
        icon: new SettingOutlined()
    ));
}
```

可以使用 `AtomUI.Icons.AntDesign` 中的任意图标：

```csharp
using AtomUI.Icons.AntDesign;

// 一些常用的图标
icon: new SettingOutlined()           // 设置图标
icon: new BellOutlined()              // 铃铛图标
icon: new MailOutlined()              // 邮件图标
icon: new CalendarOutlined()          // 日历图标
```

---

## 6. 带进度条的通知

设置 `showProgress: true` 在通知底部显示倒计时进度条：

```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <atom:OptionButtonGroup Name="HoverOptionGroup" ButtonStyle="Outline">
        <atom:OptionButton IsChecked="True">Pause on hover</atom:OptionButton>
        <atom:OptionButton>Don&apos;t pause on hover</atom:OptionButton>
    </atom:OptionButtonGroup>
    <atom:Button ButtonType="Primary" Click="ShowProgressNotification">
        Show Notification
    </atom:Button>
</StackPanel>
```

```csharp
private void ShowProgressNotification(object? sender, RoutedEventArgs e)
{
    _notificationManager?.Show(new Notification(
        type: NotificationType.Information,
        title: "Notification Title",
        content: "This is the content of the notification.",
        showProgress: true
    ));
}
```

### 控制悬停暂停

配合 `OptionButtonGroup` 切换悬停暂停行为：

```csharp
private void HandleHoverOptionGroupCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
{
    if (_notificationManager is not null)
    {
        _notificationManager.IsPauseOnHover = args.Index == 0;
    }
}
```

---

## 7. 带回调的通知

通过 `onClick` 和 `onClose` 参数设置交互回调：

```csharp
_notificationManager?.Show(new Notification(
    title: "可点击的通知",
    content: "点击通知区域执行操作",
    onClick: () =>
    {
        // 用户点击通知时执行的逻辑
        Console.WriteLine("Notification clicked!");
    },
    onClose: () =>
    {
        // 通知关闭时执行的逻辑（无论自动关闭还是手动关闭）
        Console.WriteLine("Notification closed!");
    }
));
```

---

## 8. 资源管理

`WindowNotificationManager` 实现了 `IDisposable`，使用完毕后务必释放资源：

```csharp
protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    base.OnDetachedFromVisualTree(e);
    _basicManager?.Dispose();
    _topLeftManager?.Dispose();
    _topManager?.Dispose();
    _topRightManager?.Dispose();
    _bottomLeftManager?.Dispose();
    _bottomManager?.Dispose();
    _bottomRightManager?.Dispose();
}
```

---

## 常见组合模式

### 异步操作完成通知

```csharp
public async Task SubmitForm()
{
    try
    {
        await _repository.SaveAsync();
        _notificationManager?.Show(new Notification(
            type: NotificationType.Success,
            title: "提交成功",
            content: "表单数据已成功保存"
        ));
    }
    catch (Exception ex)
    {
        _notificationManager?.Show(new Notification(
            type: NotificationType.Error,
            title: "提交失败",
            content: $"保存数据时发生错误：{ex.Message}",
            expiration: TimeSpan.Zero  // 错误通知不自动关闭
        ));
    }
}
```

### 后台任务进度通知

```csharp
_notificationManager?.Show(new Notification(
    type: NotificationType.Information,
    title: "文件上传中",
    content: "正在上传文件，请稍候...",
    showProgress: true,
    expiration: TimeSpan.FromSeconds(10)
));
```

### 系统级重要通知

```csharp
_notificationManager?.Show(new Notification(
    type: NotificationType.Warning,
    title: "系统维护通知",
    content: "系统将于今晚 22:00 进行维护，届时服务将暂停约 30 分钟。",
    expiration: TimeSpan.Zero,  // 重要通知不自动关闭
    onClose: () => { /* 记录用户已阅读 */ }
));
```
