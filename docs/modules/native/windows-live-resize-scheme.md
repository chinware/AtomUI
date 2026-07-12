# Windows live resize 与窗口装饰方案

## 结论

AtomUI 在 Windows 上采用单一窗口几何所有者：

| 系统 | 合成模式 | 窗口装饰 | resize 几何 |
| --- | --- | --- | --- |
| Windows 10 | `RedirectionSurface` | Avalonia CSD | Avalonia Win32 + DWM redirection bitmap |
| Windows 11+ | `WinUIComposition`，失败时依次回退 | Avalonia CSD | Avalonia Win32 + Windows compositor |

AtomUI 不再拦截 `WM_NCCALCSIZE`，不再手动扩展 DWM frame，也不再通过
`SWP_FRAMECHANGED` 强制重算非客户区。

## 用户可见问题

升级 Avalonia 12.1.0 后，Windows 10 上快速拖动主窗口左边缘或上边缘时，会出现：

- 对向的右边缘或下边缘剧烈抖动；
- 黑色边框短暂出现；
- 系统最小化、最大化和关闭按钮在 AtomUI 标题栏上闪现。

问题在慢速拖动时不一定明显，必须使用连续快速的 live resize 才能稳定观察。

## 根因

这是两个独立时序问题叠加后的结果。

### 1. Windows 10 的 WinUIComposition 提交落后于 HWND

Avalonia 12.1 调整了 `WinUiCompositorConnection` 的消息循环。Windows 10 上
`RequestCommitAsync` 的完成回调发生在 `DispatchMessage` 内部，render tick 与 live-resize
模态消息循环可能错开。此时原生窗口几何已经更新，但合成树仍提交上一帧的 scene size，
因此对向边缘显示旧表面并来回跳动。

实机采样中，`GetWindowRect`、client rect 和 DWM extended frame bounds 的对向边界始终固定，
而 WinUIComposition 的右侧像素仍交替变化，证明抖动发生在合成提交层，而不是 Win32 几何层。

Windows 10 使用 `RedirectionSurface` 后，DWM redirection bitmap 与 HWND resize 由同一路径管理，
对向边缘不再错帧。

### 2. AtomUI 旧 chrome 与 Avalonia 12.1 重复管理非客户区

旧实现通过自定义 WndProc hook 拦截 `WM_NCCALCSIZE`，手动返回 resize hit-test，并调用
`DwmExtendFrameIntoClientArea` 与 `SetWindowPos(SWP_FRAMECHANGED)`。Avalonia 12.1 又恢复了
Windows 10 extended-client 模式下的 `WS_CAPTION` 和边框处理，两套实现会在快速 resize 时争夺
非客户区所有权，表现为黑边和系统标题栏按钮闪现。

最终实现删除 AtomUI 的几何 hook，让 Avalonia CSD 独占窗口装饰与 resize hit-test。

## 实现边界

### 合成模式

`AppBuilderExtensions.WithWin32CompositionOptions()` 根据系统版本设置 Avalonia Win32 options：

```csharp
return OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000)
    ? ["WinUIComposition", "DirectComposition", "RedirectionSurface"]
    : ["RedirectionSurface"];
```

`AtomUI.Core` 不直接引用 `Avalonia.Win32`，继续通过现有的可选平台反射边界配置 options。

### 窗口装饰

Windows 固定使用 AtomUI 的 CSD 模板状态：

```csharp
else if (OperatingSystem.IsWindows())
{
    IsCsdEnabled = true;
}
```

`WindowTheme.axaml` 的默认值保持：

```xml
<Setter Property="ExtendClientAreaToDecorationsHint" Value="True" />
```

Windows 只覆盖 `TransparencyLevelHint=None`，不再关闭 extend-client-area。

### 保留的 Windows 消息 hook

`CaptionButtonGroup` 仍处理 `WM_NCHITTEST` 并对最大化按钮返回 `HTMAXBUTTON`，用于 Windows 11
Snap Layout。该 hook 只描述最大化按钮区域，不修改客户区、窗口尺寸或 resize 边缘。

## 禁止重新引入

- 不要在 AtomUI 中重新处理 `WM_NCCALCSIZE`。
- 不要手动返回 `HTLEFT/HTTOP/...`；resize hit-test 归 Avalonia。
- 不要调用 `DwmExtendFrameIntoClientArea(-1)` 修补 CSD 阴影。
- 不要通过 `SWP_FRAMECHANGED` 循环触发非客户区重算。
- 不要在 Windows 10 恢复 `WinUIComposition`，除非上游修复后完成相同的快速拖边实机验证。

## 验证

回归验证至少包含：

1. Windows 10 快速来回拖动左边缘，右边缘保持固定。
2. Windows 10 快速来回拖动上边缘，下边缘保持固定。
3. resize 期间没有黑色外框和系统标题栏按钮闪现。
4. Windows 11 最大化按钮仍可触发 Snap Layout。
5. 最大化、还原、全屏和退出全屏后窗口装饰正常。

相关源码：

- `src/AtomUI.Core/AppBuilderExtensions.cs`
- `src/AtomUI.Desktop.Controls/Window/Window.cs`
- `src/AtomUI.Desktop.Controls/Window/WindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml`
- `src/AtomUI.Desktop.Controls/WindowTitleBar/CaptionButtonGroup.cs`
- `tests/AtomUI.Desktop.Controls.Tests/Window/WindowResizeArtifactTests.cs`
