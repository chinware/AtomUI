# MessageBox API 参考

## 命名空间

```csharp
namespace AtomUI.Desktop.Controls;
```

AXAML 命名空间前缀：`atom`（`xmlns:atom="https://atomui.net"`）

---

## 枚举类型

### MessageBoxStyle

消息弹框样式枚举，控制图标和默认按钮组合。

| 值 | 说明 |
|---|---|
| `Normal` | 无图标，仅 OK 按钮 |
| `Confirm` | 黄色 `ExclamationCircleFilled` 图标，OK + Cancel 按钮 |
| `Information` | 蓝色 `InfoCircleFilled` 图标，仅 OK 按钮 |
| `Success` | 绿色 `CheckCircleFilled` 图标，仅 OK 按钮 |
| `Warning` | 黄色 `ExclamationCircleFilled` 图标，仅 OK 按钮 |
| `Error` | 红色 `CloseCircleFilled` 图标，仅 OK 按钮 |

### MessageBoxOkButtonStyle

确认按钮样式枚举。

| 值 | 说明 |
|---|---|
| `Default` | 默认样式按钮（白色背景 + 边框） |
| `Primary` | 主按钮样式（蓝色实心背景） |

### DialogHostType（来自 `AtomUI.Desktop.Controls`）

弹窗宿主类型枚举。

| 值 | 说明 |
|---|---|
| `Window` | 独立原生窗口 |
| `Overlay` | 页面内浮层覆盖（默认） |

---

## MessageBox 类

### 公共属性（StyledProperty）

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `Title` | `string?` | `null` | 弹框标题 |
| `Icon` | `PathIcon?` | `null` | 自定义图标（为 null 时根据 Style 自动设置） |
| `Content` | `object?` | `null` | 弹框正文内容（[Content] 标记，支持任意控件） |
| `ContentTemplate` | `IDataTemplate?` | `null` | 内容模板 |
| `Style` | `MessageBoxStyle` | `MessageBoxStyle.Information` | 消息样式，控制图标和按钮组合 |
| `OkButtonStyle` | `MessageBoxOkButtonStyle` | `MessageBoxOkButtonStyle.Primary` | 确认按钮样式 |
| `OkButtonText` | `string?` | `null` | 自定义确认按钮文本 |
| `CancelButtonText` | `string?` | `null` | 自定义取消按钮文本 |
| `HostType` | `DialogHostType` | `DialogHostType.Overlay` | 宿主模式 |
| `IsOpen` | `bool` | `false` | 弹框是否打开（支持双向绑定） |
| `IsModal` | `bool` | `false` | 是否为模态弹框 |
| `IsLoading` | `bool` | `false` | 整体加载状态 |
| `IsConfirmLoading` | `bool` | `false` | 确认按钮加载状态 |
| `IsLightDismissEnabled` | `bool` | `false` | 是否允许点击遮罩关闭（仅 Overlay 模式） |
| `IsDragMovable` | `bool` | `false` | 是否允许拖拽标题栏移动 |
| `IsCenterOnStartup` | `bool` | `true` | 是否初始居中显示 |
| `HorizontalOffset` | `Dimension?` | `null` | 水平偏移量 |
| `VerticalOffset` | `Dimension?` | `null` | 垂直偏移量 |
| `PlacementTarget` | `Control?` | `null` | 放置目标控件（[ResolveByName] 标记） |
| `Result` | `object?` | `null` | 弹框操作结果（支持双向绑定） |
| `IsMotionEnabled` | `bool` | 跟随全局 Token `EnableMotion` | 是否启用动画（共享属性） |

### CLR 属性

| 属性名 | 类型 | 说明 |
|---|---|---|
| `ButtonsConfigure` | `Action<IReadOnlyList<DialogButton>>?` | 按钮配置回调，可自定义标准按钮的文本和行为 |
| `CustomButtons` | `AvaloniaList<DialogButton>` | 自定义按钮集合 |

### 公共方法

| 方法 | 签名 | 说明 |
|---|---|---|
| `Open` | `object? Open()` | 同步打开弹框，返回操作结果 |
| `OpenAsync` | `Task OpenAsync()` | 异步打开弹框 |
| `Cancel` | `void Cancel()` | 触发取消操作（调用内部 Dialog.Reject） |
| `Confirm` | `void Confirm()` | 触发确认操作（调用内部 Dialog.Accept） |

### 静态方法

| 方法 | 签名 | 说明 |
|---|---|---|
| `ShowMessageBox` | `static object? ShowMessageBox<TView, TViewModel>(TViewModel?, MessageBoxOptions?, TopLevel?)` | 同步显示（泛型版） |
| `ShowMessageBox` | `static object? ShowMessageBox(Control, object?, MessageBoxOptions?, TopLevel?)` | 同步显示（自定义内容版） |
| `ShowMessageBoxAsync` | `static Task ShowMessageBoxAsync<TView, TViewModel>(TViewModel?, MessageBoxOptions?, Action<IMessageBoxActionResult>?, TopLevel?)` | 异步显示，通过回调获取结果 |
| `ShowMessageBoxModalAsync` | `static Task<object?> ShowMessageBoxModalAsync<TView, TViewModel>(TViewModel?, MessageBoxOptions?, TopLevel?)` | 异步模态显示，返回结果 |
| `ShowMessageAsync` | `static Task ShowMessageAsync(Control, object?, MessageBoxOptions?, Action<IMessageBoxActionResult>?, TopLevel?)` | 异步显示自定义内容 |
| `ShowMessageModalAsync` | `static Task<object?> ShowMessageModalAsync(Control, object?, MessageBoxOptions?, TopLevel?)` | 异步模态显示自定义内容 |

### 事件

| 事件名 | 类型 | 说明 |
|---|---|---|
| `Opened` | `EventHandler` | 弹框打开后触发 |
| `Closed` | `EventHandler` | 弹框关闭后触发 |
| `Cancelled` | `EventHandler` | 用户取消操作后触发 |
| `Confirmed` | `EventHandler` | 用户确认操作后触发 |

---

## MessageBoxOptions record

静态 API 使用的选项配置。

| 属性名 | 类型 | 默认值 | 说明 |
|---|---|---|---|
| `HostType` | `DialogHostType` | `DialogHostType.Overlay` | 宿主模式 |
| `Title` | `string?` | `null` | 弹框标题 |
| `Icon` | `PathIcon?` | `null` | 自定义图标 |
| `Style` | `MessageBoxStyle` | `MessageBoxStyle.Information` | 消息样式 |
| `IsDragMovable` | `bool` | `false` | 是否可拖拽 |
| `PlacementTarget` | `Control?` | `null` | 放置目标 |
| `HorizontalOffset` | `Dimension?` | `null` | 水平偏移 |
| `VerticalOffset` | `Dimension?` | `null` | 垂直偏移 |
| `IsCenterOnStartup` | `bool` | `true` | 是否居中 |
| `IsLightDismissEnabled` | `bool` | `false` | 是否轻量关闭 |
| `IsLoading` | `bool` | `false` | 加载状态 |
| `IsConfirmLoading` | `bool` | `false` | 确认按钮加载 |
| `Width` | `double` | `double.NaN` | 弹框宽度 |
| `Height` | `double` | `double.NaN` | 弹框高度 |
| `MinWidth` | `double` | `0` | 最小宽度 |
| `MinHeight` | `double` | `0` | 最小高度 |
| `MaxWidth` | `double` | `∞` | 最大宽度 |
| `MaxHeight` | `double` | `∞` | 最大高度 |

---

## IMessageBoxActionResult 接口

操作结果契约接口。

```csharp
public interface IMessageBoxActionResult
{
    object? Result { get; }
}
```

---

## 常量

### MessageBoxThemeConstants（内部）

```csharp
internal static class MessageBoxThemeConstants
{
    public const string DialogPart = "PART_Dialog";
}
```
