# Windows 禁用 CSD + 纯 Win32 DWM 阴影方案

## 背景

Avalonia 12 的 CSD（Client-Side Decorations）机制通过 `ExtendClientAreaToDecorationsHint=True` 启用 WindowDrawnDecorations，在 Windows 上存在两个严重的 resize bug：

1. **右边闪烁**：从右边缘往左拉缩小窗口到最小宽度后继续拉，窗口右半部分偶尔闪烁
2. **左边推窗口**：从左边缘往右拉到最小宽度后，窗口整体被推着往右走

这两个 bug 在 Win10 和 Win11 上均存在。

### 根因分析

两个 bug 都源于 Avalonia 12 CSD 机制与 DWM 的冲突：

- `ExtendClientArea()` 在每次 `WM_SIZE` 状态变化时被调用，重置 `DWMNCRP_DISABLED`，与 DWM 阴影设置产生时序竞争
- Avalonia 的 `WM_NCCALCSIZE` 处理（`AppWndProc` 59-131行）在 `_isClientAreaExtended=true` 时做复杂的 `AdjustWindowRectExForDpi` 边框计算，在 constrained resize 时坐标计算与 DWM 帧渲染冲突
- WindowDrawnDecorations 的三层布局（Underlay/Overlay/FullscreenPopover）增加了 resize 时的布局复杂度

曾尝试在 Win11 上保持 CSD 启用并通过 WndProc hook 拦截 WM_NCCALCSIZE，但因 Avalonia 内部 `_isClientAreaExtended=true` 状态与实际窗口行为不匹配，闪烁反而更严重。

---

## 方案概述

**在全部 Windows 版本上完全绕过 Avalonia 的 CSD 机制**，改用纯 Win32 方式实现无标题栏 + DWM 阴影：

1. `IsCsdEnabled = false` → 使用非 CSD 模板（无 WindowDrawnDecorations）
2. 不设 `ExtendClientAreaToDecorationsHint` → Avalonia 不调用 `ExtendClientArea()`，不设 `DWMNCRP_DISABLED`
3. 通过 `Win32Properties.AddWndProcHookCallback()` 拦截 `WM_NCCALCSIZE` 去掉原生标题栏
4. 通过 `WM_NCHITTEST` 提供 resize 边框
5. 手动调用 DWM API 开启阴影
6. **Win11 额外**：通过 `DWMWA_WINDOW_CORNER_PREFERENCE = DWMWCP_ROUND` 保留原生圆角

---

## Avalonia 12 关键链路

### WndProc 调用链

```
WndProcMessageHandler()
  → WndProcHookCallback    ← 我们的 hook 点，在 Avalonia 之前执行
    → WndProc()
      → CustomCaptionProc()  ← 仅当 _isClientAreaExtended=true 时调用
        → AppWndProc()       ← WM_NCCALCSIZE / WM_SIZE 在这里处理
```

### WindowDrawnDecorations 创建链路

```
ExtendClientAreaToDecorationsHint = true
  → WindowImpl._isClientAreaExtended = true
    → NeedsManagedDecorations = true
      → ComputeDecorationParts() 返回非 null
        → TopLevelHost.UpdateDrawnDecorations() 创建 WindowDrawnDecorations
```

不设 `ExtendClientAreaToDecorationsHint` → `_isClientAreaExtended = false` → `NeedsManagedDecorations = false` → WindowDrawnDecorations 不创建。

### ExtendClientArea() 调用时机

```
WM_SIZE (状态变化) → UpdateWindowProperties() → ExtendClientArea() → DWMNCRP_DISABLED
WM_SHOWWINDOW      → OnShowHideMessage()      → ExtendClientArea() → DWMNCRP_DISABLED
SetExtendClientAreaToDecorationsHint()          → ExtendClientArea() → DWMNCRP_DISABLED
```

不设 `ExtendClientAreaToDecorationsHint` → `_isClientAreaExtended = false` → 以上所有路径都不会调用 `ExtendClientArea()`。

---

## 具体实现

### 代码分层

Win32 原生逻辑集中在 `AtomUI.Native` 项目中，`Window.cs` 只负责 Avalonia 层面的判断和调用：

| 层 | 文件 | 职责 |
|---|------|------|
| 常量 | `AtomUI.Native/Windows/WindowUtils.Interop.cs` | Win32 常量（`WM_NCCALCSIZE`、`WM_NCHITTEST`、`HT*` 系列）、P/Invoke 声明 |
| Native 逻辑 | `AtomUI.Native/Windows/WindowUtils.Windows.cs` | `ApplyDwmShadow()`、`HandleNcCalcSize()`、`HitTestBorder()` |
| 窗口控件 | `AtomUI.Desktop.Controls/Window/Window.cs` | CSD 状态判断、WndProc hook 注册、WindowState 处理 |

### 1. ConfigureCsdStatus — 全 Windows 禁用 CSD

**文件**：`src/AtomUI.Desktop.Controls/Window/Window.cs`

```csharp
private void ConfigureCsdStatus()
{
    if (OperatingSystem.IsMacOS())
        IsCsdEnabled = false;
    else if (OperatingSystem.IsLinux())
        IsCsdEnabled = AvaloniaLocator.Current.GetService<X11PlatformOptions>()?.EnableDrawnDecorations == true;
    else if (OperatingSystem.IsWindows())
        IsCsdEnabled = false;
}
```

### 2. OnOpened — 注册 Hook + DWM 阴影

```csharp
protected override void OnOpened(EventArgs e)
{
    base.OnOpened(e);
    if (OperatingSystem.IsMacOS())
        ConfigureMacOsWindow();

    if (OperatingSystem.IsWindows())
    {
        Win32Properties.AddWndProcHookCallback(this, WinWndProcHook);
        ApplyWinDwmShadow();

        // 强制重新发送 WM_NCCALCSIZE，使 hook 生效
        var hwnd = TryGetPlatformHandle()!.Handle;
        WindowUtilsInterop.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
            WindowUtilsInterop.SWP_FRAMECHANGED |
            WindowUtilsInterop.SWP_NOSIZE | WindowUtilsInterop.SWP_NOMOVE |
            WindowUtilsInterop.SWP_NOZORDER | WindowUtilsInterop.SWP_NOACTIVATE);
    }
}
```

注意：Hook 必须在 `OnOpened` 中注册，因为构造函数执行时 Win32 HWND 尚未创建。

### 3. WndProc Hook — WM_NCCALCSIZE + WM_NCHITTEST

```csharp
[SupportedOSPlatform("windows")]
private IntPtr WinWndProcHook(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
{
    switch (msg)
    {
        case WindowUtilsInterop.WM_NCCALCSIZE when wParam != IntPtr.Zero:
            WindowUtilsWindows.HandleNcCalcSize(hWnd, lParam);
            handled = true;
            return IntPtr.Zero;

        case WindowUtilsInterop.WM_NCHITTEST:
            return HandleNcHitTest(hWnd, lParam, ref handled);
    }
    return IntPtr.Zero;
}
```

`HandleNcCalcSize` 在最大化时修正边框（减去 `SM_CXSIZEFRAME + SM_CXPADDEDBORDER`），非最大化时不修改 `NCCALCSIZE_PARAMS`（client area = window area → 原生标题栏消失）。

```csharp
[SupportedOSPlatform("windows")]
private IntPtr HandleNcHitTest(IntPtr hWnd, IntPtr lParam, ref bool handled)
{
    if (WindowState is WindowState.FullScreen or WindowState.Maximized)
        return IntPtr.Zero;

    var borderWidth = (int)(4 * RenderScaling);
    var hitTest = WindowUtilsWindows.HitTestBorder(hWnd, lParam, borderWidth);
    if (hitTest != 0)
    {
        handled = true;
        return (IntPtr)hitTest;
    }
    return IntPtr.Zero;
}
```

`HitTestBorder` 检测鼠标是否在窗口四边四角的 4 DIP resize 区域内，返回对应的 `HT*` 值。标题栏拖拽由 `WindowTitleBar.BeginMoveDrag()` 处理，不需要返回 `HTCAPTION`。

### 4. DWM 阴影 + Win11 圆角

**文件**：`src/AtomUI.Native/Windows/WindowUtils.Windows.cs`

```csharp
public static unsafe void ApplyDwmShadow(IntPtr hwnd)
{
    // 启用 DWM NC 渲染 → 触发阴影
    var policy = WindowUtilsInterop.DWMNCRP_ENABLED;
    WindowUtilsInterop.DwmSetWindowAttribute(hwnd,
        WindowUtilsInterop.DWMWA_NCRENDERING_POLICY, &policy, sizeof(int));

    // 扩展 DWM 帧到整个窗口
    var margins = new WindowUtilsInterop.MARGINS
    {
        cxLeftWidth = -1, cxRightWidth = -1,
        cyTopHeight = -1, cyBottomHeight = -1
    };
    WindowUtilsInterop.DwmExtendFrameIntoClientArea(hwnd, ref margins);

    // Win11: 保留原生圆角
    if (Environment.OSVersion.Version.Build >= 22000)
    {
        var round = WindowUtilsInterop.DWMWCP_ROUND;
        WindowUtilsInterop.DwmSetWindowAttribute(hwnd,
            WindowUtilsInterop.DWMWA_WINDOW_CORNER_PREFERENCE, &round, sizeof(int));
    }
}
```

因为不走 `ExtendClientAreaToDecorationsHint`，Avalonia 永远不会覆盖 `DWMNCRP_ENABLED` 设置。

### 5. 全屏处理

```csharp
[SupportedOSPlatform("windows")]
private void UpdateWinDwmForWindowState()
{
    if (WindowState == WindowState.FullScreen)
    {
        _wasFullScreen = true;
        return;
    }
    if (_wasFullScreen)
    {
        _wasFullScreen = false;
        Dispatcher.UIThread.Post(ApplyWinDwmShadow, DispatcherPriority.Send);
    }
}
```

进入全屏时不做 DWM 操作（避免干扰 OS 全屏动画）。退出全屏时，Avalonia 的 `UpdateWindowProperties(forceChanges: true)` 会重置 DWM margins 为 0，所以需要 Post 重新应用。

---

## 为什么能解决问题

| 问题 | 旧方案的根因 | 新方案如何解决 |
|------|-------------|---------------|
| 右边闪烁 | `ExtendClientArea()` 在 WM_SIZE 中重置 DWMNCRP_DISABLED，与 Post hack 时序冲突；WindowDrawnDecorations 三层布局在 constrained resize 时与 DWM 帧渲染冲突 | `ExtendClientArea()` 永远不被调用；WindowDrawnDecorations 不存在；DWM 设置一次性生效 |
| 左边推窗口 | Avalonia CSD 的 WM_NCCALCSIZE 处理做复杂边框计算，constrained resize 时坐标出错 | 我们的 hook 在 Avalonia 之前拦截 WM_NCCALCSIZE，直接返回 0，Avalonia 的复杂计算不执行 |

## 涉及的文件

| 文件 | 改动 |
|------|------|
| `src/AtomUI.Native/Windows/WindowUtils.Interop.cs` | Win32 常量定义（`WM_NCCALCSIZE`、`WM_NCHITTEST`、`HT*` 系列、DWM 属性、`DWMWCP_ROUND`） |
| `src/AtomUI.Native/Windows/WindowUtils.Windows.cs` | `ApplyDwmShadow()`、`HandleNcCalcSize()`、`HitTestBorder()` |
| `src/AtomUI.Desktop.Controls/Window/Window.cs` | `ConfigureCsdStatus()` 全 Windows 设 false；`WinWndProcHook`、`HandleNcHitTest`、`ApplyWinDwmShadow`、`UpdateWinDwmForWindowState` |
| `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml` | `ExtendClientAreaToDecorationsHint` 仅在 `[IsCsdEnabled=True]` 条件下设置 |

## 已知限制

- **FullscreenPopover**：非 CSD 模板没有全屏弹出层。Windows 上进入全屏后，顶部悬停显示关闭/退出按钮的功能暂不可用。后续可在非 CSD 模板中单独实现。
- **全屏切换偶尔顿卡**：退出全屏时约 1 秒的随机顿卡，为 Avalonia 内部 `SetFullScreen()` → `UpdateWindowProperties()` 的预存问题，非本方案引入。

## 参考

- Avalonia 12 源码：`.referenceprojects/Avalonia/src/Windows/Avalonia.Win32/WindowImpl.cs`
- `Win32Properties.AddWndProcHookCallback()`：`.referenceprojects/Avalonia/src/Avalonia.Controls/Platform/Win32Properties.cs`
