# Dialog API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### DialogHostType

对话框宿主类型，决定对话框的渲染方式。

| 值 | 说明 |
|---|---|
| `Overlay` | 在当前窗口的 Overlay 层中渲染（默认） |
| `Window` | 在独立的原生窗口中渲染 |

### DialogHorizontalAnchor

对话框水平起始位置锚点。

| 值 | 说明 |
|---|---|
| `Left` | 靠左对齐 |
| `Center` | 水平居中 |
| `Right` | 靠右对齐 |
| `Custom` | 自定义（使用 `HorizontalOffset`） |

### DialogVerticalAnchor

对话框垂直起始位置锚点。

| 值 | 说明 |
|---|---|
| `Top` | 靠顶对齐 |
| `Center` | 垂直居中 |
| `Bottom` | 靠底对齐 |
| `Custom` | 自定义（使用 `VerticalOffset`） |

### DialogStandardButton（Flags 枚举）

标准按钮标记，可通过位运算或逗号组合。

| 值 | 角色 | 说明 |
|---|---|---|
| `NoButton` | — | 无按钮 |
| `Ok` | AcceptRole | "确定"按钮 |
| `Open` | AcceptRole | "打开"按钮 |
| `Save` | AcceptRole | "保存"按钮 |
| `Cancel` | RejectRole | "取消"按钮 |
| `Close` | RejectRole | "关闭"按钮 |
| `Discard` | DestructiveRole | "放弃"按钮 |
| `Apply` | ApplyRole | "应用"按钮 |
| `Reset` | ResetRole | "重置"按钮 |
| `Reload` | ResetRole | "重新加载"按钮 |
| `RestoreDefaults` | ResetRole | "恢复默认"按钮 |
| `Help` | HelpRole | "帮助"按钮 |
| `SaveAll` | AcceptRole | "全部保存"按钮 |
| `Yes` | YesRole | "是"按钮 |
| `YesToAll` | YesRole | "全部是"按钮 |
| `No` | NoRole | "否"按钮 |
| `NoToAll` | NoRole | "全部否"按钮 |
| `Abort` | RejectRole | "中止"按钮 |
| `Retry` | AcceptRole | "重试"按钮 |
| `Ignore` | AcceptRole | "忽略"按钮 |

### DialogButtonRole

按钮角色，决定点击后的对话框行为。

| 值 | 说明 |
|---|---|
| `AcceptRole` | 接受角色（点击后触发 Accept） |
| `RejectRole` | 拒绝角色（点击后触发 Reject） |
| `DestructiveRole` | 破坏性操作角色 |
| `ActionRole` | 动作角色（不自动关闭对话框） |
| `HelpRole` | 帮助角色 |
| `YesRole` | "是"角色（点击后触发 Accept） |
| `NoRole` | "否"角色（点击后触发 Reject） |
| `ApplyRole` | 应用角色（点击后触发 Accept） |
| `ResetRole` | 重置角色（点击后触发 Accept） |
| `CustomRole` | 自定义角色 |

### DialogCode

对话框结果代码。

| 值 | 说明 |
|---|---|
| `Accepted` | 对话框被接受 |
| `Rejected` | 对话框被拒绝 |

---

## 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Title` | `string?` | `null` | 对话框标题 |
| `TitleIcon` | `PathIcon?` | `null` | 标题栏图标 |
| `Content` | `object?` | `null` | 对话框内容（支持任意控件），标记 `[Content]` 可在 AXAML 中直接作为子元素 |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容数据模板 |
| `IsOpen` | `bool` | `false` | 对话框是否打开。设置为 `true` 时自动调用 `OpenAsync()`，设置为 `false` 时触发 `Done(Result)` |
| `IsModal` | `bool` | `true` | 是否为模态对话框。模态时 `OpenAsync()` 在关闭后才完成 Task；非模态时打开后立即完成 |
| `IsClosable` | `bool` | `true` | 是否显示关闭按钮 |
| `IsResizable` | `bool` | `false` | 是否可调整大小 |
| `IsMaximizable` | `bool` | `false` | 是否可最大化 |
| `IsMinimizable` | `bool` | `true` | 是否可最小化（仅 `DialogHostType.Window` 宿主有效；模态对话框自动禁用） |
| `IsDragMovable` | `bool` | `false` | 是否可拖拽移动 |
| `IsLightDismissEnabled` | `bool` | `false` | 点击遮罩/外部区域是否关闭（仅 Overlay 宿主有效） |
| `IsLoading` | `bool` | `false` | 整体加载状态，内容区域显示加载遮罩 |
| `IsConfirmLoading` | `bool` | `false` | 确认加载状态。为 `true` 时阻止所有关闭请求（`NotifyClose` 直接返回），防止用户在异步操作完成前关闭 |
| `IsFooterVisible` | `bool` | `true` | 是否显示底部按钮区域 |
| `DialogHostType` | `DialogHostType` | `Overlay` | 宿主类型 |
| `StandardButtons` | `DialogStandardButtons` | `NoButton` | 标准按钮配置（通过 `AddOwner` 从 `DialogButtonBox` 共享） |
| `DefaultStandardButton` | `DialogStandardButton` | `NoButton` | 默认确认按钮（视觉高亮，支持 Enter 键触发） |
| `EscapeStandardButton` | `DialogStandardButton` | `NoButton` | Escape 键触发的按钮 |
| `HorizontalStartupLocation` | `DialogHorizontalAnchor` | `Custom` | 水平起始位置锚点 |
| `VerticalStartupLocation` | `DialogVerticalAnchor` | `Custom` | 垂直起始位置锚点 |
| `HorizontalOffset` | `Dimension?` | `null` | 水平偏移（像素或百分比），当 `HorizontalStartupLocation == Custom` 时生效 |
| `VerticalOffset` | `Dimension?` | `null` | 垂直偏移（像素或百分比），当 `VerticalStartupLocation == Custom` 时生效 |
| `PlacementTarget` | `Control?` | `null` | 定位参考控件。未设置时自动查找逻辑父控件 |
| `CustomDialogPlacementCallback` | `CustomDialogPlacementCallback?` | `null` | 自定义定位回调 |
| `OverlayDismissEventPassThrough` | `bool` | `false` | 遮罩关闭时是否将指针事件穿透给下层控件 |
| `OverlayInputPassThroughElement` | `IInputElement?` | `null` | 输入穿透元素 |
| `InheritsTransform` | `bool` | `false` | 是否继承父元素变换（缩放等） |
| `Topmost` | `bool` | `false` | 是否置顶 |
| `Result` | `object?` | `null` | 对话框结果值。`Accept()` 设为 `DialogCode.Accepted`，`Reject()` 设为 `DialogCode.Rejected`，`Done(result)` 设为自定义值 |
| `IsMotionEnabled` | `bool` | 跟随全局 Token | 是否启用打开/关闭动画（共享属性，通过 `AddOwner` 注册） |

---

## 只读属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `Host` | `IDialogHost?` | 当前宿主实例（对话框打开后可访问） |
| `IsPointerOverDialog` | `bool` | 指针是否位于对话框宿主之上 |

---

## 公共集合与回调

| 属性名 | 类型 | 说明 |
|---|---|---|
| `CustomButtons` | `AvaloniaList<DialogButton>` | 自定义按钮集合。对话框打开后修改此集合会实时同步到宿主 |
| `ButtonsConfigure` | `Action<IReadOnlyList<DialogButton>>?` | 按钮创建后的配置回调，可用于统一修改按钮属性（如禁用全部按钮） |
| `DependencyResolver` | `IAvaloniaDependencyResolver?` | Avalonia 依赖注入解析器 |

---

## 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Opened` | `EventHandler?` | 对话框打开后触发 |
| `Closing` | `EventHandler<CancelEventArgs>?` | 关闭前触发，设置 `Cancel = true` 可阻止关闭 |
| `Closed` | `EventHandler?` | 关闭后触发（宿主清理完成后） |
| `Accepted` | `EventHandler?` | `Result` 为 `DialogCode.Accepted` 时触发 |
| `Rejected` | `EventHandler?` | `Result` 为 `DialogCode.Rejected` 时触发 |
| `Finished` | `EventHandler<DialogFinishedEventArgs>?` | 对话框完成时触发（在 `Accepted`/`Rejected` 之后），包含 `Result` |
| `ButtonClicked` | `EventHandler<DialogButtonClickedEventArgs>?` | 底部按钮被点击时触发。设置 `Handled = true` 可阻止默认的 Accept/Reject 行为，常用于异步关闭场景 |

---

## 公共方法

| 方法 | 返回值 | 说明 |
|---|---|---|
| `Open()` | `object?` | 同步打开，创建新的消息循环等待对话框关闭。返回 `Result` |
| `OpenAsync()` | `Task` | 异步打开。模态时 Task 在关闭后完成（可 `await` 后读取 `Result`）；非模态时打开后立即完成 |
| `Accept()` | `void` | 设置 `Result = DialogCode.Accepted` 并关闭 |
| `Reject()` | `void` | 设置 `Result = DialogCode.Rejected` 并关闭 |
| `Done(object?)` | `void` | 设置自定义 `Result` 并关闭 |
| `Done()` | `void` | 直接关闭（不修改 `Result`） |
| `IsInsideDialog(Visual)` | `bool` | 判断指定的可视元素是否位于当前对话框宿主内部 |

---

## 静态方法

### 同步方法

| 方法签名 | 返回值 | 说明 |
|---|---|---|
| `ShowDialog<TView, TViewModel>(TViewModel?, DialogOptions?, TopLevel?)` | `object?` | 同步打开（泛型视图 + ViewModel） |
| `ShowDialogModal<TView, TViewModel>(TViewModel?, DialogOptions?, TopLevel?)` | `object?` | 同步模态打开（泛型视图 + ViewModel） |
| `ShowDialog(Control, object?, DialogOptions?, TopLevel?)` | `object?` | 同步打开（直接传入内容控件） |
| `ShowDialogModal(Control, object?, DialogOptions?, TopLevel?)` | `object?` | 同步模态打开（直接传入内容控件） |

### 异步方法

| 方法签名 | 返回值 | 说明 |
|---|---|---|
| `ShowDialogAsync<TView, TViewModel>(TViewModel?, DialogOptions?, Action<IDialogActionResult>?, TopLevel?)` | `Task` | 异步打开。非模态时通过 `closed` 回调获取结果 |
| `ShowDialogModalAsync<TView, TViewModel>(TViewModel?, DialogOptions?, TopLevel?)` | `Task<object?>` | 异步模态打开，`await` 后直接获取结果 |
| `ShowDialogAsync(Control, object?, DialogOptions?, Action<IDialogActionResult>?, TopLevel?)` | `Task` | 异步打开（内容控件版本）。通过 `closed` 回调获取结果 |
| `ShowDialogModalAsync(Control, object?, DialogOptions?, TopLevel?)` | `Task<object?>` | 异步模态打开（内容控件版本） |

> **提示**：非模态异步方法 `ShowDialogAsync` 提供 `Action<IDialogActionResult>? closed` 回调参数，在对话框关闭时调用，可通过 `IDialogActionResult.Result` 读取结果。模态异步方法 `ShowDialogModalAsync` 的 `Task` 在关闭后完成，可直接 `await` 获取返回值。

---

## 配置类

### DialogOptions

`DialogOptions` 是一个 `record` 类型，用于在静态 API 中配置对话框参数。

```csharp
public record DialogOptions
{
    public string? Title { get; init; }
    public PathIcon? TitleIcon { get; init; }
    public bool IsLightDismissEnabled { get; init; }       // 仅 Overlay 宿主有效
    public bool IsResizable { get; init; }
    public bool IsClosable { get; init; } = true;
    public bool IsMaximizable { get; init; }
    public bool IsMinimizable { get; init; } = true;       // 仅 Window 宿主有效
    public bool IsDragMovable { get; init; }
    public bool IsFooterVisible { get; init; } = true;
    public Control? PlacementTarget { get; init; }
    public Dimension? HorizontalOffset { get; init; }
    public Dimension? VerticalOffset { get; init; }
    public DialogHostType DialogHostType { get; init; } = DialogHostType.Overlay;
    public DialogStandardButtons StandardButtons { get; init; } = DialogStandardButton.NoButton;
    public DialogStandardButton DefaultStandardButton { get; init; }
    public DialogHorizontalAnchor HorizontalStartupLocation { get; init; } = DialogHorizontalAnchor.Custom;
    public DialogVerticalAnchor VerticalStartupLocation { get; init; } = DialogVerticalAnchor.Custom;
    public double Width { get; init; } = double.NaN;
    public double Height { get; init; } = double.NaN;
    public double MinWidth { get; init; } = 0d;
    public double MinHeight { get; init; } = 0d;
    public double MaxWidth { get; init; } = double.PositiveInfinity;
    public double MaxHeight { get; init; } = double.PositiveInfinity;
}
```

---

## 事件参数类

### DialogButtonClickedEventArgs

```csharp
public class DialogButtonClickedEventArgs : EventArgs
{
    public DialogButton SourceButton { get; }   // 被点击的按钮实例
    public bool Handled { get; set; }           // 设为 true 可阻止默认 Accept/Reject 行为
}
```

### DialogFinishedEventArgs

```csharp
public class DialogFinishedEventArgs : EventArgs
{
    public object? Result { get; }              // 对话框结果值
}
```

---

## 关键接口

### IDialog

```csharp
public interface IDialog
{
    string? Title { get; set; }
    object? Result { get; set; }
    event EventHandler? Closed;
    event EventHandler? Opened;
    event EventHandler<CancelEventArgs>? Closing;
    event EventHandler? Accepted;
    event EventHandler? Rejected;
    event EventHandler<DialogFinishedEventArgs>? Finished;
    event EventHandler<DialogButtonClickedEventArgs>? ButtonClicked;
    void Accept();
    void Reject();
    void Done(object? result);
    void Done();
}
```

### IDialogAwareDataContext

ViewModel 实现此接口后，当被设置为 Dialog 的 DataContext 时，会自动收到通知：

```csharp
public interface IDialogAwareDataContext
{
    void NotifyAttachedToDialog(IDialog dialog);    // DataContext 被关联到 Dialog 时调用
    void NotifyDetachedFromDialog() {}              // DataContext 被解除关联时调用（有默认空实现）
    void NotifyClosed() {}                          // Dialog 关闭后调用（有默认空实现）
}
```

### IDialogActionResult

非模态异步对话框关闭回调中的结果接口：

```csharp
public interface IDialogActionResult
{
    object? Result { get; }
}
```

### DialogButton

继承自 `Button`，用作对话框底部按钮。

| 属性名 | 类型 | 说明 |
|---|---|---|
| `StandardButtonType` | `DialogStandardButton?` | 对应的标准按钮类型 |
| `Role` | `DialogButtonRole` | 按钮角色（默认 `CustomRole`） |
| `IsDefaultConfirmButton` | `bool` | 是否为默认确认按钮（视觉高亮） |
| `IsDefaultEscapeButton` | `bool` | 是否为默认 Escape 按钮 |
