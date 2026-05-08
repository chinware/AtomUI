# Dialog Window/Overlay implementation plan for release/6.0

## 背景与目标

`release/5.0` 的 `Dialog` 是一套完整的弹窗系统：外部通过 `Dialog`、`DialogOptions`、`ShowDialog*` 静态 API、`IDialog`、`IDialogHostProvider` 等接口使用；内部根据 `DialogHostType` 分为 `Window Dialog` 与 `Overlay Dialog` 两种承载方式。

`release/6.0` 已引入新的 `Window` 架构与新的 `Popup`/Overlay 架构：

- `Window` 模板内使用 `VisualLayerManager`、`FullscreenPopoverLayer`、新的标题栏和新的 resizer。
- `Popup` 移除了旧的 `PopupBuddyLayer`/`OverlayPopupContent`，改为 Avalonia 12 的 PopupHost/OverlayPopupHost 路线，并通过 `PopupMotionActor`、`ShadowsAwareContainer`、`ShouldUseOverlayLayer` 统一动画与阴影。

本计划的目标是基于 `release/6.0` 的新架构重新完成：

1. `Window Dialog`
2. `Overlay Dialog`

同时保持 `release/5.0` 的外部接口、交互语义和 UI 效果不变。

重要原则：以 `release/5.0` 的对外 API、行为语义和 UI 效果为基线，在 `release/6.0` 新 `Window` 和新 `Popup` 架构上重新实现 Dialog。

## 必须保持不变的对外行为

### Public API

需要保持或恢复以下 API：

- `Dialog`
- `DialogOptions`
- `DialogHostType`
- `DialogPlacement` 中的 `DialogHorizontalAnchor`、`DialogVerticalAnchor`
- `DialogStandardButton` / `DialogStandardButtons`
- `DialogButton`
- `DialogButtonRole`
- `IDialog`
- `IDialogActionResult`
- `IDialogAwareDataContext`
- `IDialogHost`
- `IDialogHostProvider`
- `Dialog.ShowDialog*`
- `Dialog.ShowDialogModal*`
- `Dialog.ShowDialogAsync*`
- `Dialog.ShowDialogModalAsync*`

为了“接口不变”，需要提供这些静态 API。内部实现不再依赖 `GlobalDialogManager`：Window 类型基于新 `Window`，Overlay 类型基于新 `Popup` 自己的 OverlayLayer。

### 行为语义

以下行为需要与 `release/5.0` 保持一致：

- `DialogHostType.Window` 使用独立窗口承载。
- `DialogHostType.Overlay` 在当前 `TopLevel` 的 overlay 层内承载。
- `IsModal=true` 时，异步打开应等待 Dialog 关闭后完成。
- `IsModal=false` 时，异步打开应在 Host 显示后完成，关闭结果通过回调或事件获取。
- `Open()` 仍通过局部 Dispatcher frame 实现同步等待。
- `Accept()` 设置 `Result = DialogCode.Accepted` 并触发关闭。
- `Reject()` 设置 `Result = DialogCode.Rejected` 并触发关闭。
- `Done(object? result)` 设置结果并关闭。
- `Closing` 可取消关闭。
- `IsConfirmLoading=true` 时禁止确认关闭，并同步到确认类按钮 loading 状态。
- `IDialogAwareDataContext.NotifyAttachedToDialog/NotifyDetachedFromDialog/NotifyClosed` 生命周期保持。
- `CustomButtons` 打开前后动态增删保持同步。
- `ButtonsConfigure` 在按钮同步后仍可配置按钮。
- light dismiss、遮罩事件穿透、父窗口关闭时关闭 Dialog 的语义保持。

### UI 效果

需要保持 Ant Design 5.0 Dialog 视觉效果：

- header/content/footer padding、间距、字体、背景和阴影继续由 `DialogToken` 控制。
- Overlay Dialog 保持圆角、阴影、modal mask、标题栏、关闭按钮、最大化按钮、resize handle、拖拽移动。
- Window Dialog 保持与 AtomUI Window 一致的新标题栏、系统平台适配和窗口阴影，同时保留 Dialog footer/button box。
- `DialogButtonBox` 的按钮布局、本地化文本、默认按钮、Esc 按钮、确认按钮 loading 不变。

## 架构取舍

实现时需要遵循以下取舍：

- 可从 5.0 拿回 API、按钮系统、Token、Localization 和视觉模板结构。
- 不直接拿回 5.0 的旧 `GlobalDialogManager`、旧 `DialogLayer`、旧 `ManagedDialogPositioner` 作为最终架构。
- 以 6.0 的 `Window` 和新 `Popup`/OverlayPopupHost 作为底座。
- Window Dialog 不复制或修改 Window 的复杂 ControlTheme，直接使用新 `Window` 的原生模板能力，通过设置 Window `Content` / `ContentTemplate` 承载 Dialog 自己的内容包装控件。

因此实现重点是将 5.0 的 API 和视觉行为重新映射到 6.0 的 Window/Popup 架构。

## 总体架构

### 组件关系

```text
Dialog
├── public API / lifecycle / events / result
├── static ShowDialog* APIs
├── DialogOpenState
├── Relay bindings to host
│
├── Window Dialog
│   └── DialogHost : AtomUI.Desktop.Controls.Window
│       ├── uses release/6.0 Window chrome/titlebar/resizer
│       ├── owns DialogButtonBox footer
│       └── relays close/minimize/maximize/footer/loading/buttons
│
└── Overlay Dialog
    └── OverlayDialogHost : ContentControl
        ├── hosted as Popup content
        ├── Popup.ShouldUseOverlayLayer = true
        ├── optional OverlayDialogMask
        ├── OverlayDialogHeader
        ├── DialogButtonBox footer
        ├── OverlayDialogResizer
        └── drag/maximize/restore/z-index/motion
```

### Host 创建策略

`Dialog.OpenAsync` 根据 `DialogHostType` 分发：

```text
DialogHostType.Window
    TopLevel parent = TopLevel.GetTopLevel(PlacementTarget)
    DialogHost host = CreateDialogHost(parent, this)
    host.SetChild(ContentControl)
    relay Dialog properties
    host.Show() or ShowDialog-like modal loop

DialogHostType.Overlay
    Popup popup = CreateOverlayPopup(PlacementTarget or topLevel)
    popup.ShouldUseOverlayLayer = true
    OverlayDialogHost host = CreateOverlayDialogHost(popup, this)
    host is assigned as popup child/content
    relay Dialog properties
    popup.IsOpen = true
```

`Dialog` 自身继续不参与 measure/arrange，仍作为逻辑控制器存在。

## Window Dialog 设计

### 承载类

保留：

```csharp
internal class DialogHost : Window, IDialogHost, IStyleHost, IMotionAwareControl
```

但需要明确它是基于 `release/6.0` 新 `Window` 的 Dialog 特化，而不是 5.0 旧 Window 模板副本。

### Window 属性同步

`Dialog` 打开 Window Host 时需要同步：

- `Title`
- `TitleIcon` 到 Window 的 `Icon`/`Logo` 或 DialogHost 自定义 title content
- `Content`
- `ContentTemplate`
- `Width`
- `Height`
- `MinWidth`
- `MinHeight`
- `MaxWidth`
- `MaxHeight`
- `Topmost`
- `IsResizable`
- `IsClosable`
- `IsMaximizable`
- `IsMinimizable`
- `IsModal`
- `IsMotionEnabled`
- `IsLoading`
- `IsConfirmLoading`
- `StandardButtons`
- `DefaultStandardButton`
- `EscapeStandardButton`
- `IsFooterVisible`

`IsModal=true` 时：

- `EffectiveMinimizable=false`
- Window host 不显示 minimize caption button
- 关闭通过 Dialog `Closing` 事件统一拦截

### Window Content 包装策略

Window 的 ControlTheme 处理跨平台 CSD、标题栏、阴影、resizer、`VisualLayerManager`、`FullscreenPopoverLayer` 等复杂逻辑。Window Dialog 不应复制、覆盖或修改 Window 的 ControlTheme。

首选方案：`DialogHost` 继续继承 release/6.0 的 `Window`，并直接使用 Window 原有 `ControlTheme`。Dialog 专属 UI 通过设置 Window 的 `Content` 和 `ContentTemplate` 实现。

建议新增内部内容包装控件，例如：

```text
DialogWindowContent
├── Skeleton loading wrapper
├── user content presenter
└── DialogButtonBox footer
```

`DialogHost.SetChild(Control? control)` 不应直接把用户内容设置给 Window `Content`，而应：

1. 保存用户内容和 `ContentTemplate`。
2. 创建/更新 `DialogWindowContent`。
3. 将 `DialogWindowContent` 设置为 Window `Content`。
4. 由 `DialogWindowContent` 内部承载用户内容、loading skeleton、footer 和 button box。

包装控件可以是内部 `TemplatedControl` 或轻量 `ContentControl`：

- 若需要 token style 和 template selector，使用 `TemplatedControl`。
- 若实现足够简单，可使用 `ContentControl` + 专用 `ControlTheme`。

包装控件需要暴露或接收：

- `DialogContent`
- `DialogContentTemplate`
- `IsLoading`
- `IsConfirmLoading`
- `IsFooterVisible`
- `IsEffectiveFooterVisible`
- `StandardButtons`
- `DefaultStandardButton`
- `EscapeStandardButton`
- `CustomButtons`
- `IsMotionEnabled`

它的模板只负责 Dialog 内容区：

```xml
<DockPanel LastChildFill="True">
    <Border Name="FooterFrame" DockPanel.Dock="Bottom"
            IsVisible="{TemplateBinding IsEffectiveFooterVisible}">
        <atom:DialogButtonBox ... />
    </Border>
    <Border Name="ContentFrame" Padding="{TemplateBinding Padding}">
        <atom:Skeleton IsLoading="{TemplateBinding IsLoading}" ... />
    </Border>
</DockPanel>
```

这样 Window Dialog 可以完整复用新 Window 的 ControlTheme，并把 Dialog 的 header/footer/content 逻辑限制在 Window content 区域内，避免后续 Window 模板变化导致 DialogHost 模板同步成本。

`DialogHostTheme.axaml` 应尽量保持很薄：

- 只 `BasedOn` 新 `WindowTheme`。
- 设置 DialogHost 默认属性，例如 `SizeToContent`、`Padding`、caption button 可见性。
- 不定义完整 `ControlTemplate`。
- 不选择 Window 内部复杂 template part，除非该 part 是稳定公开常量。

### Dialog 定位抽象

不要使用 `release/5.0` 的 `ManagedDialogPositioner`。它为旧 Popup/DialogLayer 场景处理了大量 anchor、gravity、screen constraint、flip 等复杂逻辑，但 Dialog 当前只需要 Ant Design Dialog 的启动位置和 offset 语义。Window 类型已经可以直接设置窗口位置；Popup 类型固定使用 Dialog 自己的 `CustomPopupPlacementCallback`，不考虑通用 `Placement`，因为 Dialog 的 Overlay 场景里 placement 没有实际意义。

建议抽象一个轻量定位接口，统一 Window Dialog 和 Overlay Dialog 的定位输入，但分别适配不同宿主：

```csharp
internal interface IDialogPlacementController
{
    void Attach(Dialog dialog, IDialogHost host, TopLevel topLevel, Control placementTarget);
    void Detach();
    void UpdatePlacement();
    void NotifyHostMeasured(Size hostSize, Rect ownerBounds);
}
```

或使用更小的计算型接口：

```csharp
internal interface IDialogPlacementCalculator
{
    Point Calculate(Dialog dialog, Size hostSize, Size ownerSize);
}
```

推荐组合：

- `DialogPlacementCalculator`：只负责根据 `HorizontalStartupLocation`、`VerticalStartupLocation`、`HorizontalOffset`、`VerticalOffset` 计算 logical offset。
- `WindowDialogPlacementController`：把 logical offset 转成 screen/device position，并设置 `DialogHost.Position`。
- `PopupDialogPlacementController`：维护 Dialog offset，并安装 Dialog 专用 `CustomPopupPlacementCallback`；callback 根据当前 offset 返回固定 top-left placement。

这样定位策略只写一份，宿主适配逻辑分开，避免把 Window 和 Popup 坐标系混在一起。

### Window Dialog 定位

Window Dialog 直接设置 Window 位置，不使用 `ManagedDialogPositioner`：

- startup location 只负责初始位置。
- `HorizontalStartupLocation` / `VerticalStartupLocation` 计算 `OffsetX` / `OffsetY`。
- Window host 测量完成后调用 `NotifyDialogHostMeasured(Size, Rect)`。
- `Rect` 来源：
  - 优先使用 owner 所在 screen working area。
  - 无 owner screen 时使用 primary screen working area。
- 计算完成后设置 `DialogHost.Position`。

`WindowDialogPlacementController.UpdatePlacement()`：

```text
Window host:
    logical point = owner screen top-left + OffsetX/Y
    device point = logical point * DesktopScaling
    host.Position = device point
```

Window 类型不需要 placement callback，也不需要复杂避让逻辑。后续如果要支持 owner window 移动时跟随，只在 `WindowDialogPlacementController` 中订阅 owner position/size 变化即可。

### Window Dialog 关闭

`DialogHost.OnClosing` 必须拦截用户关闭：

- 非程序关闭或 caption close button 关闭时取消 `WindowClosingEventArgs`。
- 转发到 `Dialog.NotifyDialogHostCloseRequest()`。
- 由 `Dialog.NotifyClose()` 统一触发 `Closing`、`Accepted`、`Rejected`、`Finished`、`Closed`。

程序关闭时由 `DialogOpenState.Dispose()` 调用 `host.Close(callback)`。

## Overlay Dialog 设计

### Popup OverlayLayer 承载策略

Overlay Dialog 应直接使用 `Popup` 自己的 OverlayLayer，不引入 `ScopeAwareOverlayLayer`。

```text
Dialog
└── Popup
    ├── ShouldUseOverlayLayer = true
    ├── PlacementTarget = Dialog.PlacementTarget or TopLevel root/content
    ├── CustomPopupPlacementCallback = Dialog-specific callback
    └── Child/Content = OverlayDialogHost
```

这样 Overlay Dialog 的层级、PopupHost、OverlayPopupHost、motion/shadow 基础设施都由新 Popup 架构负责。

`Dialog` 不应直接查找或注入 overlay layer。它只创建和配置 `Popup`，由 `Popup` 内部决定使用 OverlayPopupHost 还是 PopupRoot。

### Overlay Host 结构

承载类：

```csharp
internal class OverlayDialogHost : ContentControl, IDialogHost, IMotionAwareControl
```

内部职责：

- 作为 `Popup` 的内容控件存在。
- 通过持有 owner `Popup` 或由 `DialogOpenState` 管理 popup open/close。
- 管理 `OverlayDialogMask`
- 管理 `OverlayDialogHeader`
- 管理 `DialogButtonBox`
- 管理 `OverlayDialogResizer`
- 管理拖拽、最大化、还原
- 管理 z-index
- 管理打开/关闭动画
- 管理 footer 自动显隐

### Overlay mask

`IsModal=true`：

- 在 `OverlayDialogHost` 内显示 mask。
- Popup 内容容器应按 owner `TopLevel.ClientSize` 或 placement target root bounds 拉满，使 mask 覆盖当前 Popup OverlayLayer 可视区域。
- mask 拦截 pointer wheel 和 pointer press。
- `IsLightDismissEnabled=true` 时点击 mask 触发 `Dialog.NotifyClose()`。

`IsModal=false`：

- 不显示阻塞 mask。
- host 自身保留阴影。
- 点击外部不关闭，除非显式配置 light dismiss。

### Overlay 事件穿透

保持 5.0 的语义：

- `OverlayDismissEventPassThrough=true` 时，mask 点击关闭后将 pointer event 转发给 `OverlayInputPassThroughElement` 或原始命中元素。
- 如果 release/6.0/Avalonia 12 的 raw input API 不再允许旧方式直接复用，需要实现局部兼容层，不改变公开属性。

### Overlay 定位与尺寸

Overlay Dialog 不使用旧 `ManagedDialogPositioner`。使用 Popup owner 的可用尺寸：

- `TopLevel.ClientSize`
- 或 `PlacementTarget` 所在 visual root/client bounds
- 作为 `NotifyDialogHostMeasured` 的 bounds

通用 `DialogPlacementCalculator` 定位规则：

- `HorizontalStartupLocation.Left` -> `OffsetX=0`
- `HorizontalStartupLocation.Center` -> `(ownerWidth - hostWidth) / 2`
- `HorizontalStartupLocation.Right` -> `ownerWidth - hostWidth`
- `HorizontalStartupLocation.Custom` -> `HorizontalOffset.Resolve(ownerWidth)`
- vertical 同理

拖拽时：

- 更新 `OffsetX` / `OffsetY`，由 `OverlayDialogHost` 在自己的 full-overlay panel 内重新 arrange frame。
- 不回写 `HorizontalStartupLocation` / `VerticalStartupLocation`。

Popup Dialog 固定使用自己的 `CustomPopupPlacementCallback`：

- 不使用 Popup 的普通 `Placement` 语义。
- 要考虑 Dialog offset：接口实现负责设置/更新 offset。
- callback 只处理 Dialog 的 top-left 位置。
- callback 根据当前 offset 定位 Dialog。
- callback 不实现 5.0 `ManagedDialogPositioner` 那种复杂 flip/constraint 算法。

```text
PopupDialogPlacementController.UpdatePlacement()
    offset = DialogPlacementCalculator.Calculate(...)
    popup.InvalidatePosition()

DialogCustomPopupPlacementCallback(ownerSize, hostSize, current offset)
    returns top-left custom placement from offset
```

最大化时：

- 保存 `_originPosition` 和 `_latestSize`。
- `WindowState=Maximized` 后 dialog frame 填满 popup owner 可视区域。
- `CornerRadius=0`。
- resizer 隐藏。

还原时：

- 恢复最大化前位置和尺寸。

### Overlay 与新 Popup 架构的关系

Overlay Dialog 不应复活旧 `PopupBuddyLayer` 或旧 `DialogLayer`。

Overlay Dialog 直接使用新 Popup 架构：

- `Popup.ShouldUseOverlayLayer=true`
- `Popup.PlacementTarget = Dialog.PlacementTarget ?? topLevel/root`
- `Popup` 负责 OverlayLayer/OverlayPopupHost 生命周期。
- `OverlayDialogHost` 负责 Dialog mask、frame、drag、resize、maximize/restore。
- `PopupMotionActor`/`MotionActor` 的动画调度模式可以复用，但 Dialog 当前也可继续使用 host transitions，前提是视觉不变。
- `ShadowsAwareContainer` 的阴影测量思路可以参考，但 Dialog 当前可继续通过 `ShadowFrame` + token shadow 实现，避免影响 UI。

Popup 配置必须满足：

- `Popup.ShouldUseOverlayLayer=true`
- `Popup.IsLightDismissEnabled=false`，light dismiss 由 Dialog mask 统一处理。
- `Popup.StaysOpen=true` 或等价设置，避免 Popup 自己绕过 Dialog 生命周期关闭。
- `Popup` close/cancel 事件必须转发到 `Dialog.NotifyClose()`，不能直接销毁。
- Popup content 的 root 尺寸应覆盖 owner 可视区域，以支持 modal mask。
- Popup 定位固定通过 Dialog 专用 `CustomPopupPlacementCallback` 实现：controller 设置 offset，callback 根据 offset 定位；不使用通用 `Placement`，不使用 `ManagedDialogPositioner`。

## Dialog.OpenAsync 实现计划

### 打开流程

`OpenAsync` 应补齐为：

```text
if already open/opening -> return
resolve placementTarget
resolve topLevel
create host by DialogHostType
set child/content
relay properties and token bindings
subscribe parent closed/detached/raw input if needed
create DialogOpenState
set IsOpen=true under ignore scope
host.Show()
raise Opened
if IsModal:
    await close task
else:
    return after show
```

需要内部增加 close completion：

```csharp
TaskCompletionSource<object?> _closeTcs
```

或放入 `DialogOpenState`。

### 关闭流程

`NotifyClose` 应：

1. 检查 `IsConfirmLoading` 和 `_closing`。
2. 触发 `Closing`，允许取消。
3. 根据 `Result` 触发 `Accepted` / `Rejected`。
4. 触发 `Finished`。
5. dispose open state：
   - 从 overlay layer 移除 mask 和 host，播放关闭动画。
   - 或关闭 Window host。
   - 解除所有 bindings/subscriptions。
6. 设置 `IsOpen=false`。
7. 触发 `Closed`。
8. 通知 `IDialogAwareDataContext.NotifyClosed()`。
9. 完成 modal await 的 task。

### 属性 relay

`Dialog` 到 host 使用 `BindUtils.RelayBind`，避免手动同步遗漏。

Window Host 和 Overlay Host 共同 relay：

- title/icon
- content/template
- size
- modal
- resizable
- closable
- maximizable
- minimizable/effective minimizable
- drag movable
- motion
- loading
- confirm loading
- footer visible
- standard buttons
- default/escape button
- topmost

Overlay-only：

- light dismiss
- overlay event pass-through
- overlay input pass-through element
- offset x/y

Window-only：

- Window titlebar caption button visibility
- owner/parent relation
- startup position

## Static API 恢复计划

恢复 `Dialog.StaticAPI.cs`，但内部实现改为：

```text
FindDialogPlacementRoot(topLevel)
    topLevel ?? Window.GetMainWindow()
    throw if null

CreateDialog(content, dataContext, options)
    same mapping as release/5.0

ShowDialog(...)
    dialog.PlacementTarget = options.PlacementTarget ?? topLevel/root content
    return dialog.Open()

ShowDialogAsync(...)
    attach Closed callback
    await dialog.OpenAsync(...)
```

不再要求 `GlobalDialogManager` 存在。

兼容点：

- 如果用户传入 AtomUI `Window` 或普通 Avalonia `TopLevel`，Overlay Dialog 都通过 `Popup` 自己解析 OverlayLayer。
- 如果 Popup 无法在当前 TopLevel 创建 overlay host，应抛出明确异常。

## 文件级实施清单

### 建议的实现策略

以 `release/5.0` 为行为基线，在 `release/6.0` 上实现：

1. 备份/对照 `release/5.0:src/AtomUI.Desktop.Controls/Dialog` 作为行为基线。
2. 保留可直接复用且无架构冲突的文件：
   - `DialogToken.cs`
   - `Localization/en_US.cs`
   - `Localization/zh_CN.cs`
   - `ButtonBox/*`，但需要按 6.0 Button/Theme API 验证
   - `DialogActionResult.cs`
   - `DialogFinishedEventArgs.cs`
   - `DialogManagerEventArgs.cs`
   - `DialogHostType.cs`
   - `DialogPlacement.cs`
   - `IDialog*.cs`
3. 实现以下核心文件：
   - `Dialog.cs`
   - `Dialog.StaticAPI.cs`
   - `WindowHost/DialogHost.cs`
   - `OverlayHost/OverlayDialogHost.cs`
   - `OverlayHost/OverlayDialogMask.cs`
   - `Themes/DialogHostTheme.axaml`
   - `Themes/OverlayDialogHostTheme.axaml`
4. 不恢复旧架构文件：
   - `GlobalDialogManager.cs`
   - `DialogPositioning/ManagedDialogPositioner.cs`
   - `DialogPositioning/IDialogPositioner.cs`
   - 旧 `DialogLayer` 相关实现
5. 新增轻量定位抽象：
   - `DialogPositioning/IDialogPlacementCalculator.cs`
   - `DialogPositioning/DialogPlacementCalculator.cs`
   - `DialogPositioning/WindowDialogPlacementController.cs`
   - `DialogPositioning/PopupDialogPlacementController.cs`
   - `DialogPositioning/DialogCustomPopupPlacementCallback.cs`

### Dialog 主流程

- `src/AtomUI.Desktop.Controls/Dialog/Dialog.cs`
  - 以 5.0 `Dialog` public surface 为基线重写。
  - 实现 Host 创建方法：`CreateDialogHost`、`CreateOverlayDialogHost`。
  - 实现 `Open()`、`OpenAsync()`、`NotifyClose()`、modal/non-modal close completion。
  - 实现 `UpdateHostSizing`，定位委托给轻量 `IDialogPlacementController`。
  - 实现 `Closed` 与 data context closed notification。

- `src/AtomUI.Desktop.Controls/Dialog/DialogPositioning/*`
  - 不恢复 5.0 复杂 positioner。
  - 新增轻量 `DialogPlacementCalculator` 计算 logical offset。
  - 新增 Window/Popup 两个 controller，分别设置 Window position 和 Dialog 专用 Popup placement callback。

- `src/AtomUI.Desktop.Controls/Dialog/Dialog.StaticAPI.cs`
  - 从 release/5.0 恢复静态 API 签名。
  - 改用 release/6.0 overlay/window root 解析。

- `src/AtomUI.Desktop.Controls/Dialog/IDialogHost.cs`
  - 保持 public surface 不破坏。
  - 如果必须新增内部方法，优先通过 internal extension 或具体 host 类型实现，避免破坏接口。

### Window Dialog

- `src/AtomUI.Desktop.Controls/Dialog/WindowHost/DialogHost.cs`
  - 对齐新 `Window` 标题栏和 caption button 属性。
  - 补齐 position 更新。
  - 确认 `Close(Action?)` 会执行 callback。
  - 确认 button/footer/loading relay。
  - 通过内部 `DialogWindowContent` 设置 Window `Content`，不直接修改 Window ControlTheme。

- `src/AtomUI.Desktop.Controls/Dialog/Themes/DialogHostTheme.axaml`
  - 保持薄主题，基于 release/6.0 `WindowTheme`。
  - 不复制 Window `ControlTemplate`。
  - 不插入 Window 内部 footer。
  - 只设置 Window/DialogHost 级默认属性。

- `src/AtomUI.Desktop.Controls/Dialog/WindowHost/DialogWindowContent.cs`
  - 新增内部内容包装控件。
  - 承载用户内容、`Skeleton` loading、footer 和 `DialogButtonBox`。
  - 向 `DialogHost` 回调按钮点击和按钮同步事件。

- `src/AtomUI.Desktop.Controls/Dialog/Themes/DialogWindowContentTheme.axaml`
  - 新增包装控件主题。
  - 使用 `DialogTokenResource ContentPadding`、`FooterPadding`、`FooterMarginTop`、`FooterBg`。
  - 视觉效果与 5.0 Window Dialog content/footer 一致。

### Overlay Dialog

- `src/AtomUI.Desktop.Controls/Dialog/OverlayHost/OverlayDialogHost.cs`
  - 改为作为 `Popup` 内容控件承载。
  - 补齐 show/close 动画与 popup open/close 协调。
  - 补齐 drag/maximize/restore resize 边界。
  - 补齐 z-index 更新。
  - 补齐 footer/loading/button 同步。

- `src/AtomUI.Desktop.Controls/Dialog/OverlayHost/OverlayDialogPopup.cs` 或 Dialog 内部 popup factory
  - 新增或封装 Overlay Dialog 专用 Popup 创建逻辑。
  - 设置 `ShouldUseOverlayLayer=true`。
  - 设置 placement target、placement、stays open/light dismiss 策略。
  - 将 `OverlayDialogHost` 作为 popup content。

- `src/AtomUI.Desktop.Controls/Dialog/OverlayHost/OverlayDialogMask.cs`
  - 确认 modal mask 覆盖 Popup owner 可视区域。
  - 确认 light dismiss 和 pointer wheel 拦截。

- `src/AtomUI.Desktop.Controls/Dialog/OverlayHost/OverlayDialogResizer.cs`
  - 确认 resize 事件与 `WindowState=Maximized` 互斥。

- `src/AtomUI.Desktop.Controls/Dialog/Themes/OverlayDialogHostTheme.axaml`
  - 保持现有视觉。
  - 必要时引入 `MotionActor`，但不改变 UI。

### ButtonBox / Token / Localization

- `src/AtomUI.Desktop.Controls/Dialog/ButtonBox/DialogButtonBox.cs`
  - 保持 release/5.0 行为。
  - 验证标准按钮顺序、角色分组、默认按钮和 escape 按钮。

- `src/AtomUI.Desktop.Controls/Dialog/DialogToken.cs`
  - 保持现有 token，不硬编码颜色和尺寸。

- `src/AtomUI.Desktop.Controls/Dialog/Localization/*.cs`
  - 保持按钮文本本地化。

## 实施阶段

### 阶段 1：恢复外部 API 与打开/关闭骨架

目标：

- 以 release/5.0 为基线重建 `Dialog.cs` 和 `Dialog.StaticAPI.cs`。
- 实现 `Dialog.OpenAsync` 的 Host 创建分发。
- 补齐 `DialogOpenState` close completion。
- 能打开和关闭最小 Window Dialog / Overlay Dialog。

验收：

- `ShowDialog` 可同步返回 `DialogCode.Accepted/Rejected`。
- `ShowDialogAsync` 非 modal 打开后返回，并可通过 closed callback 拿到 result。
- `ShowDialogModalAsync` 等待关闭后返回 result。

### 阶段 2：Window Dialog 接入新 Window

目标：

- `DialogHost` 完整使用 release/6.0 新 Window 行为。
- 新增 `DialogWindowContent` 包装控件，使用 Window `Content` / `ContentTemplate` 承载 Dialog 内容和 footer。
- `DialogHostTheme.axaml` 保持薄主题，不复制 Window `ControlTemplate`。
- 标题栏、caption buttons、resize、平台外观正常。

验收：

- Window Dialog 与普通 `Window` 的标题栏风格一致。
- footer 按钮显示、隐藏、loading 正常。
- modal Window 不显示 minimize。
- 关闭按钮触发 Dialog 生命周期，而不是绕过 `Closing`。

### 阶段 3：Overlay Dialog 接入 Popup OverlayLayer

目标：

- Overlay Dialog 不依赖 `GlobalDialogManager`。
- 通过 `Popup.ShouldUseOverlayLayer=true` 使用 Popup 自己的 OverlayLayer。
- 支持 mask、light dismiss、drag、resize、maximize/restore、z-index。

验收：

- Overlay Dialog 居中显示。
- overlay mask 覆盖当前 Popup owner 可视区域。
- resize/maximize/restore 视觉与 release/5.0 一致。
- 多个 overlay dialog 层级正确。

### 阶段 4：定位、尺寸和 transform 兼容

目标：

- 支持 `HorizontalStartupLocation` / `VerticalStartupLocation`。
- 支持 `HorizontalOffset` / `VerticalOffset`。
- 支持 Window 和 Overlay 的不同坐标系。
- 抽象轻量 `IDialogPlacementCalculator` / `IDialogPlacementController`。
- Window 类型直接设置 `Window.Position`。
- Popup 类型由 controller 设置 offset，并通过 Dialog 专用 `CustomPopupPlacementCallback` 根据 offset 定位，不考虑通用 `Placement`。
- 保留 `InheritsTransform` 对 host sizing 的影响。

验收：

- top/center/bottom、left/center/right、custom offset 都正确。
- owner window 移动或不同 screen working area 下 Window Dialog 初始位置正确。
- Overlay Dialog 不越出 Popup owner 可视区域的基本边界。
- 不引入 5.0 `ManagedDialogPositioner`。

### 阶段 5：行为回归

目标：

- ButtonBox 行为与 release/5.0 一致。
- Loading/ConfirmLoading 行为一致。
- DataContext notification 一致。
- light dismiss 和 event pass-through 一致。

验收：

- 标准按钮角色触发正确 result。
- `Closing` 可取消关闭。
- `IsConfirmLoading` 阻止确认关闭。
- `NotifyClosed` 在最终关闭后触发。

### 阶段 6：测试和 Gallery 验证

建议补充或执行：

- 单元测试：
  - `DialogStandardButtons.Count`
  - `DialogButtonBox` 标准按钮构建和 role 分组
  - `Closing` cancel
  - `Accept/Reject/Done` result

- 交互验证：
  - Overlay modal
  - Overlay non-modal
  - Overlay light dismiss
  - Overlay resize/maximize/restore
  - Window modal
  - Window non-modal
  - 自定义按钮
  - confirm loading
  - loading skeleton

- Gallery：
  - 如现有 Gallery 无 Dialog showcase，需要补一个最小 showcase，覆盖 Window/Overlay 两种 host。

## 风险与注意事项

1. **不要恢复旧 PopupBuddyLayer 架构**  
   release/6.0 已迁移到 Avalonia 12 的 PopupHost/OverlayPopupHost 方向，Dialog 不应重新引入旧 Popup buddy layer。

2. **不要恢复 GlobalDialogManager 依赖**  
   静态 API 可以恢复，但内部 Window Dialog 使用新 `Window`，Overlay Dialog 使用新 `Popup` 自己的 OverlayLayer。

3. **WindowTheme 模板复制需要谨慎**  
   Window 的 ControlTheme 很复杂，Window Dialog 不应复制或修改它。DialogHost 应复用 Window 原主题，把 Dialog 专属内容放进 Window `Content` 包装控件中。

4. **Overlay 坐标系与 Window 坐标系不同**  
   Window 使用 screen/device pixel position；Overlay 使用 layer logical coordinates。不要混用。

5. **不要恢复 ManagedDialogPositioner**  
   5.0 的 positioner 异常复杂，不适合新架构。Window 直接设置位置；Popup 由 controller 设置 offset，再由 Dialog 专用 `CustomPopupPlacementCallback` 根据 offset 定位。需要复用的是“启动位置和 offset 语义”，不是旧 positioner 实现。

6. **Modal async 语义容易回归**  
   `OpenAsync` 对 modal/non-modal 的完成时机必须严格与 release/5.0 保持一致。

7. **Avalonia 12 internal API**  
   `release/6.0` 已有若干 reflection extension。新增反射前必须确认是否已有可复用扩展，避免重复和脆弱依赖。

## 完成标准

完成后应满足：

- 5.0 Dialog 外部 API 源码兼容。
- Window Dialog 基于 release/6.0 新 `Window`。
- Overlay Dialog 基于 release/6.0 新 `Popup` OverlayLayer 架构。
- Dialog UI 与 release/5.0 效果一致。
- 不引入旧 `GlobalDialogManager`、旧 `DialogLayer`、旧 `PopupBuddyLayer`。
- 不破坏 `Popup`、`Window`、`Message`、`Notification`、`Tour` 等现有 overlay/layer 使用者。
