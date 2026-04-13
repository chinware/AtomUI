# Notification API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### NotificationType

通知类型枚举，定义通知的语义类别和对应图标/颜色。

| 值 | 说明 |
|---|---|
| `Information` | 信息类通知（默认），蓝色图标 |
| `Success` | 成功类通知，绿色图标 |
| `Warning` | 警告类通知，黄色图标 |
| `Error` | 错误类通知，红色图标 |

### NotificationPosition

通知弹出位置枚举，控制通知在窗口中的出现位置。

| 值 | 说明 |
|---|---|
| `TopLeft` | 左上角 |
| `TopRight` | 右上角（默认） |
| `TopCenter` | 顶部居中 |
| `BottomLeft` | 左下角 |
| `BottomRight` | 右下角 |
| `BottomCenter` | 底部居中 |

---

## INotification 接口

通知数据契约，定义通知所需的全部信息。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Title` | `string` | 通知标题 |
| `Content` | `object?` | 通知正文内容 |
| `Icon` | `PathIcon?` | 自定义图标，为 `null` 时使用类型对应的默认图标 |
| `Type` | `NotificationType` | 通知类型 |
| `Expiration` | `TimeSpan` | 自动关闭时间，`TimeSpan.Zero` 表示不自动关闭 |
| `ShowProgress` | `bool` | 是否显示倒计时进度条 |
| `OnClick` | `Action?` | 通知被点击时的回调 |
| `OnClose` | `Action?` | 通知被关闭时的回调 |

---

## INotificationManager 接口

通知管理器契约。

### 方法

| 方法 | 签名 | 说明 |
|---|---|---|
| `Show` | `void Show(INotification notification, string[]? classes = null)` | 弹出一条通知。可选传入样式类名数组 |

---

## Notification 类（数据模型）

`INotification` 的默认实现，同时实现 `INotifyPropertyChanged` 支持属性变更通知。

### 构造函数

```csharp
public Notification(
    string title,
    object? content,
    NotificationType type = NotificationType.Information,
    PathIcon? icon = null,
    TimeSpan? expiration = null,
    bool showProgress = false,
    Action? onClick = null,
    Action? onClose = null)
```

| 参数 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `title` | `string` | — | 通知标题（必填） |
| `content` | `object?` | — | 通知正文内容 |
| `type` | `NotificationType` | `Information` | 通知类型 |
| `icon` | `PathIcon?` | `null` | 自定义图标 |
| `expiration` | `TimeSpan?` | `null`（实际为 5s） | 自动关闭时间，`null` 默认 5 秒，`TimeSpan.Zero` 不自动关闭 |
| `showProgress` | `bool` | `false` | 是否显示进度条 |
| `onClick` | `Action?` | `null` | 点击回调 |
| `onClose` | `Action?` | `null` | 关闭回调 |

### 公共属性

| 属性名 | 类型 | 说明 | 可写 |
|---|---|---|---|
| `Title` | `string` | 通知标题 | ✅ 触发 PropertyChanged |
| `Content` | `object?` | 通知正文内容 | ✅ 触发 PropertyChanged |
| `Icon` | `PathIcon?` | 自定义图标 | ✅ 触发 PropertyChanged |
| `ShowProgress` | `bool` | 是否显示进度条 | ✅ 触发 PropertyChanged |
| `Type` | `NotificationType` | 通知类型 | ✅ |
| `Expiration` | `TimeSpan` | 自动关闭时间 | ✅ |
| `OnClick` | `Action?` | 点击回调 | ✅ |
| `OnClose` | `Action?` | 关闭回调 | ✅ |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `PropertyChanged` | `PropertyChangedEventHandler?` | 属性变更通知（实现 `INotifyPropertyChanged`） |

---

## WindowNotificationManager 类

通知管理器控件，负责通知的生命周期管理和视觉容器。通过 `AdornerLayer` 挂载到窗口顶层。

### 构造函数

```csharp
// 无参构造，需后续手动挂载
public WindowNotificationManager()

// 传入 TopLevel 自动挂载到其 AdornerLayer
public WindowNotificationManager(TopLevel? host)
```

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Position` | `NotificationPosition` | `NotificationPosition.TopRight` | 通知弹出位置 |
| `MaxItems` | `int` | `5` | 最大同时可见通知数，超出时自动关闭最早的通知 |
| `IsPauseOnHover` | `bool` | `true` | 鼠标悬停时是否暂停自动关闭计时 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用出入动画（共享属性） |

### 方法

| 方法 | 签名 | 说明 |
|---|---|---|
| `Show` | `void Show(INotification notification, string[]? classes = null)` | 弹出一条通知 |
| `Dispose` | `void Dispose()` | 释放资源，从 AdornerLayer 移除，停止所有定时器 |

### 实现的接口

| 接口 | 说明 |
|---|---|
| `INotificationManager` | 提供 `Show()` 方法 |
| `IMotionAwareControl` | 提供 `IsMotionEnabled` 属性 |
| `IDisposable` | 支持资源释放 |

### 模板部件

| 部件名 | 类型 | 说明 |
|---|---|---|
| `PART_Items` | `Panel` | 通知卡片容器（实际为 `ReversibleStackPanel`） |

---

## NotificationCard 类

单条通知的可视化控件，继承自 `ContentControl`。

### 公共属性（StyledProperty / DirectProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Title` | `string` | — | 通知标题文本 |
| `Content` | `object?` | `null` | 通知正文内容（继承自 `ContentControl`） |
| `Icon` | `PathIcon?` | `null` | 通知图标，`null` 时使用类型对应的默认图标 |
| `NotificationType` | `NotificationType` | `Information` | 通知类型，影响图标和颜色 |
| `Expiration` | `TimeSpan?` | `null` | 剩余关闭时间，`null` 为不自动关闭 |
| `IsShowProgress` | `bool` | `false` | 是否显示倒计时进度条 |
| `IsClosing` | `bool`（只读） | `false` | 是否正在关闭（退出动画播放中） |
| `IsClosed` | `bool` | `false` | 是否已关闭 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用出入动画 |

### 方法

| 方法 | 签名 | 说明 |
|---|---|---|
| `Close` | `void Close()` | 触发通知关闭（播放退出动画后移除） |

### 事件

| 事件名 | 类型 | 路由策略 | 说明 |
|---|---|---|---|
| `NotificationClosed` | `EventHandler<RoutedEventArgs>` | Bubble | 通知关闭完成后触发 |

### 模板部件

| 部件名 | 类型 | 说明 |
|---|---|---|
| `PART_CloseButton` | `IconButton` | 关闭按钮 |
| `BaseMotionActor.MotionActorPart` | `BaseMotionActor` | 动画执行器 |

---

## 伪类（Pseudo-Classes）

### NotificationCard 伪类

NotificationCard 支持以下伪类，可在样式选择器中使用：

#### 通知类型伪类（标准）

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:error` | `StdPseudoClass.Error` | `NotificationType == Error` |
| `:information` | `StdPseudoClass.Information` | `NotificationType == Information` |
| `:success` | `StdPseudoClass.Success` | `NotificationType == Success` |
| `:warning` | `StdPseudoClass.Warning` | `NotificationType == Warning` |

#### 位置伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:topleft` | `NotificationPseudoClass.TopLeft` | `Position == TopLeft` |
| `:topright` | `NotificationPseudoClass.TopRight` | `Position == TopRight` |
| `:topcenter` | `NotificationPseudoClass.TopCenter` | `Position == TopCenter` |
| `:bottomleft` | `NotificationPseudoClass.BottomLeft` | `Position == BottomLeft` |
| `:bottomright` | `NotificationPseudoClass.BottomRight` | `Position == BottomRight` |
| `:bottomcenter` | `NotificationPseudoClass.BottomCenter` | `Position == BottomCenter` |

### WindowNotificationManager 伪类

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:topleft` | `NotificationPseudoClass.TopLeft` | `Position == TopLeft` |
| `:topright` | `NotificationPseudoClass.TopRight` | `Position == TopRight` |
| `:topcenter` | `NotificationPseudoClass.TopCenter` | `Position == TopCenter` |
| `:bottomleft` | `NotificationPseudoClass.BottomLeft` | `Position == BottomLeft` |
| `:bottomright` | `NotificationPseudoClass.BottomRight` | `Position == BottomRight` |
| `:bottomcenter` | `NotificationPseudoClass.BottomCenter` | `Position == BottomCenter` |

---

## 实现的接口

### IMotionAwareControl

```csharp
public bool IsMotionEnabled { get; set; }
```

`WindowNotificationManager` 和 `NotificationCard` 均实现此接口，控制出入动画的启用/禁用。`IsMotionEnabled` 的默认值跟随全局 `SharedToken.EnableMotion`。

### IDisposable（仅 WindowNotificationManager）

```csharp
public void Dispose()
```

调用 `Dispose()` 后：
- 停止所有定时器（过期检测、清理队列）
- 清空通知列表和清理队列
- 从 `AdornerLayer` 移除自身
- 取消窗口事件订阅
