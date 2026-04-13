# Message API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### MessageType

消息类型枚举，定义消息的语义和默认图标。

| 值 | 说明 |
|---|---|
| `Information` | 信息提示（默认），蓝色 `InfoCircleFilled` 图标 |
| `Success` | 成功提示，绿色 `CheckCircleFilled` 图标 |
| `Warning` | 警告提示，黄色 `ExclamationCircleFilled` 图标 |
| `Error` | 错误提示，红色 `CloseCircleFilled` 图标 |
| `Loading` | 加载中提示，蓝色旋转 `LoadingOutlined` 图标 |

### NotificationPosition（来自 `AtomUI.Desktop.Controls`）

消息位置枚举，用于 `WindowMessageManager.Position`。

| 值 | 说明 |
|---|---|
| `TopLeft` | 左上角 |
| `TopRight` | 右上角（默认） |
| `BottomLeft` | 左下角 |
| `BottomRight` | 右下角 |
| `TopCenter` | 顶部居中 |
| `BottomCenter` | 底部居中 |

---

## IMessage 接口

消息数据契约接口，定义了全局提示消息所需的基本信息。

```csharp
public interface IMessage
{
    string Content { get; }
    PathIcon? Icon { get; }
    MessageType Type { get; }
    TimeSpan Expiration { get; }
    Action? OnClose { get; }
}
```

| 属性 | 类型 | 说明 |
|---|---|---|
| `Content` | `string` | 消息文本内容 |
| `Icon` | `PathIcon?` | 自定义图标，为 `null` 时使用 `Type` 对应的默认图标 |
| `Type` | `MessageType` | 消息类型 |
| `Expiration` | `TimeSpan` | 自动关闭时间，`TimeSpan.Zero` 表示不自动关闭 |
| `OnClose` | `Action?` | 消息关闭回调 |

---

## Message 类（数据模型）

`Message` 是 `IMessage` 的默认实现，同时实现了 `INotifyPropertyChanged`，支持 `Content` 和 `Icon` 的动态更新。

### 构造函数

```csharp
public Message(
    string content,
    MessageType type = MessageType.Information,
    PathIcon? icon = null,
    TimeSpan? expiration = null,
    Action? onClose = null)
```

| 参数 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `content` | `string` | — | 消息文本内容（必填） |
| `type` | `MessageType` | `MessageType.Information` | 消息类型 |
| `icon` | `PathIcon?` | `null` | 自定义图标，为 `null` 时使用默认图标 |
| `expiration` | `TimeSpan?` | `null`（实际 5 秒） | 自动关闭时间 |
| `onClose` | `Action?` | `null` | 消息关闭时执行的回调 |

### 属性

| 属性名 | 类型 | 默认值 | 可写 | 说明 |
|---|---|---|---|---|
| `Content` | `string` | 构造函数传入 | ✅ | 消息文本，修改后触发 `PropertyChanged` |
| `Icon` | `PathIcon?` | `null` | ✅ | 自定义图标，修改后触发 `PropertyChanged` |
| `Type` | `MessageType` | `MessageType.Information` | ✅ | 消息类型 |
| `Expiration` | `TimeSpan` | `TimeSpan.FromSeconds(5)` | ✅ | 自动关闭时间 |
| `OnClose` | `Action?` | `null` | ✅ | 关闭回调 |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `PropertyChanged` | `PropertyChangedEventHandler` | 属性变更通知（`Content`、`Icon` 变更时触发） |

---

## IMessageManager 接口

消息管理器契约接口。

```csharp
public interface IMessageManager
{
    void Show(IMessage message, string[]? classes = null);
}
```

| 方法 | 参数 | 说明 |
|---|---|---|
| `Show` | `message`: 消息数据; `classes`: 可选样式类名数组 | 显示一条全局提示消息 |

---

## MessageCard 类（视觉控件）

`MessageCard` 是单条消息的视觉呈现控件，继承自 `TemplatedControl`。

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `MessageType` | `MessageType` | `MessageType.Information` | 消息类型，控制图标和颜色 |
| `Message` | `string` | `""` | 消息文本内容 |
| `Icon` | `PathIcon?` | `null` | 消息图标，为 `null` 时根据 `MessageType` 自动设置默认图标 |
| `IsClosed` | `bool` | `false` | 消息是否已关闭 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用入场/出场动画（共享属性） |

### 只读属性（DirectProperty）

| 属性名 | 类型 | 说明 |
|---|---|---|
| `IsClosing` | `bool` | 消息是否正在关闭中（出场动画播放期间为 `true`） |

### 内部属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `OpenCloseMotionDuration` | `TimeSpan` | 入场/出场动画时长（由主题设置，默认为 `SharedToken.MotionDurationMid`） |

### 公共方法

| 方法 | 签名 | 说明 |
|---|---|---|
| `Close()` | `void Close()` | 触发消息关闭流程（播放出场动画后关闭） |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `MessageClosed` | `EventHandler<RoutedEventArgs>` | 消息关闭完成后触发的路由事件（Bubble 路由策略） |

### 伪类（Pseudo-Classes）

MessageCard 支持以下伪类，可在样式选择器中使用：

| 伪类 | 常量 | 触发条件 |
|---|---|---|
| `:information` | `MessageCardPseudoClass.Information` | `MessageType == Information` |
| `:success` | `MessageCardPseudoClass.Success` | `MessageType == Success` |
| `:warning` | `MessageCardPseudoClass.Warning` | `MessageType == Warning` |
| `:error` | `MessageCardPseudoClass.Error` | `MessageType == Error` |
| `:loading` | `MessageCardPseudoClass.Loading` | `MessageType == Loading` |

---

## WindowMessageManager 类（容器管理器）

`WindowMessageManager` 是全局消息容器，挂载在 `TopLevel` 的 `AdornerLayer` 上，管理消息的显示和生命周期。

### 构造函数

```csharp
public WindowMessageManager(TopLevel? host)
```

| 参数 | 类型 | 说明 |
|---|---|---|
| `host` | `TopLevel?` | 承载消息的顶层窗口。传入后自动挂载到该窗口的 AdornerLayer |

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Position` | `NotificationPosition` | `NotificationPosition.TopRight` | 消息显示位置 |
| `MaxItems` | `int` | `5` | 同时可见的最大消息数量 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用动画（传递给子 MessageCard） |

### 公共方法

| 方法 | 签名 | 说明 |
|---|---|---|
| `Show` | `void Show(IMessage message, string[]? classes = null)` | 显示一条消息，可附加样式类 |
| `Dispose` | `void Dispose()` | 释放资源，从 AdornerLayer 卸载，清理事件订阅 |

### 实现的接口

| 接口 | 作用 |
|---|---|
| `IMessageManager` | 消息管理器契约，提供 `Show` 方法 |
| `IMotionAwareControl` | 动画控制，`IsMotionEnabled` 属性传递给子 MessageCard |
| `IDisposable` | 资源释放，卸载时清理 AdornerLayer 引用和事件订阅 |

---

## 常量

### MessageCardPseudoClass

```csharp
public static class MessageCardPseudoClass
{
    public const string Error = ":error";
    public const string Information = ":information";
    public const string Success = ":success";
    public const string Warning = ":warning";
    public const string Loading = ":loading";
}
```

### MessageCardThemeConstants（内部）

```csharp
internal class MessageCardThemeConstants
{
    public const string FramePart = "PART_Frame";
    public const string IconContentPart = "PART_IconContent";
    public const string HeaderContainerPart = "PART_HeaderContainer";
    public const string MessagePart = "PART_Message";
}
```

### WindowMessageManagerThemeConstants（内部）

```csharp
internal class WindowMessageManagerThemeConstants
{
    public const string ItemsPart = "PART_Items";
}
```
