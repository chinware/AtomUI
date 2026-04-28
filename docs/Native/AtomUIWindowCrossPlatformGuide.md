# AtomUI Window 跨平台定制完整指南

> 面向团队成员和 AI 模型的 AtomUI Window 跨平台窗口定制技术路径与陷阱手册。
> 基于 Avalonia 12 + AtomUI V6 实际实现总结。

---

## 目录

1. [总览：四平台技术路径一图速记](#1-总览四平台技术路径一图速记)
2. [核心概念](#2-核心概念)
3. [macOS](#3-macos)
4. [Linux (X11)](#4-linux-x11)
5. [Windows 11 (build ≥ 22000)](#5-windows-11-build--22000)
6. [Windows 10 (build < 22000)](#6-windows-10-build--22000)
7. [AtomUI Window 架构：IsCsdEnabled 双模板机制](#7-atomui-window-架构iscsdenabled-双模板机制)
8. [跨平台陷阱速查表](#8-跨平台陷阱速查表)
9. [关键源码索引](#9-关键源码索引)

---

## 1. 总览：四平台技术路径一图速记

```
平台              CSD 模式    标题栏由谁画         阴影由谁画        Resize 由谁提供     圆角
─────────────────────────────────────────────────────────────────────────────────────────
macOS             ❌ 关闭     AtomUI 自绘模板      OS 原生           OS 原生             OS 原生
Linux (X11)       ✅ 开启     AtomUI 自绘模板      Avalonia BoxShadow Avalonia ResizeGrips AtomUI 自绘
Windows 11        ❌ 关闭     AtomUI 自绘模板      DWM (手动启用)    WndProc Hook        DWM 原生 (DWMWCP_ROUND)
Windows 10        ❌ 关闭     AtomUI 自绘模板      DWM (手动启用)    WndProc Hook        无 (DWM 不支持)
```

### 决策树

```
OperatingSystem.IsWindows()?
  └─ 全版本 → IsCsdEnabled=false → 非 CSD 模板 + WndProc Hook + 手动 DWM 阴影 (Win11 额外设置圆角)
OperatingSystem.IsMacOS()?
  └─ 永远 → IsCsdEnabled=false → 非 CSD 模板 + NSWindow 原生标题栏布局
OperatingSystem.IsLinux()?
  └─ EnableDrawnDecorations? → IsCsdEnabled=该值 → CSD 模板 + 自绘阴影 + SHAPE 点击穿透
```

对应代码（`Window.cs`）：

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

---

## 2. 核心概念

### 2.1 CSD vs 非 CSD

- **CSD (Client-Side Decorations)**：Avalonia 12 的 `WindowDrawnDecorations` 机制。通过 `ExtendClientAreaToDecorationsHint=true` 激活，框架接管标题栏/边框/阴影的渲染。
- **非 CSD**：不设 `ExtendClientAreaToDecorationsHint`，Avalonia 不创建 `WindowDrawnDecorations`，标题栏由 Window 自己的 ControlTemplate 提供。

### 2.2 IsCsdEnabled 属性

AtomUI 自定义的 `DirectProperty<Window, bool>`，在构造函数中根据平台设置，驱动 `WindowTheme.axaml` 中的模板切换：

- `IsCsdEnabled=True` → CSD 模板（设置 `ExtendClientAreaToDecorationsHint=True`）
- `IsCsdEnabled=False` → 非 CSD 模板（不设 `ExtendClientAreaToDecorationsHint`，自带标题栏区域）

### 2.3 Avalonia 12 WindowDrawnDecorations 激活链路

```
ExtendClientAreaToDecorationsHint = true
  → WindowImpl._isClientAreaExtended = true
    → NeedsManagedDecorations = true
      → ComputeDecorationParts() 返回非 null
        → TopLevelHost.UpdateDrawnDecorations() 创建 WindowDrawnDecorations
```

不设 `ExtendClientAreaToDecorationsHint` → 整条链路不触发 → WindowDrawnDecorations 不存在。

### 2.4 WndProc 调用链（Windows 平台）

```
WndProcMessageHandler()
  → WndProcHookCallback    ← Win32Properties.AddWndProcHookCallback() 注册，在 Avalonia 之前执行
    → WndProc()
      → CustomCaptionProc()  ← 仅当 _isClientAreaExtended=true 时调用
        → AppWndProc()       ← WM_NCCALCSIZE / WM_SIZE 在这里处理
```

---

## 3. macOS

### 技术路径

| 维度 | 说明 |
|------|------|
| IsCsdEnabled | `false` |
| WindowDrawnDecorations | **永不创建**（`NeedsManagedDecorations` 硬编码 `false`） |
| 标题栏 | AtomUI 非 CSD 模板自绘，NSWindow 原生红绿灯按钮通过 `MacStandardWindowButtons` 定位 |
| 阴影 | OS 原生，无需任何干预 |
| 圆角 | OS 原生 |
| Resize | OS 原生 |

### 关键实现

1. **红绿灯按钮定位**：通过 `MacStandardWindowButtons.SetStandardWindowButtonsLayout()` 设置偏移和间距
2. **标题栏高度同步**：`ExtendClientAreaTitleBarHeightHint` 告诉 OS 标题栏高度，红绿灯按钮据此垂直居中
3. **标题栏内容左偏移**：`TitleBarOffsetMargin` 根据红绿灯按钮宽度计算，避免自绘内容与红绿灯重叠

```csharp
// Window.cs - ConfigureMacOsWindow()
MacStandardWindowButtons.SetStandardWindowButtonsLayout(this, titleBarHeight, offsetX, null, effectSpacing);
var offset = this.GetRecommendedTitleBarContentLeftMargin(effectSpacing) ?? 0;
TitleBarOffsetMargin = new Thickness(offset, 0, 0, 0);
```

### 陷阱

| 陷阱 | 说明 |
|------|------|
| 写 WindowDrawnDecorations 主题期望 macOS 生效 | 死代码，`NeedsManagedDecorations` 硬编码 false，永远不实例化 |
| 不设 `ExtendClientAreaTitleBarHeightHint` | 红绿灯按钮垂直位置错误 |
| 忘记处理 `OnSizeChanged` | 窗口大小变化后红绿灯位置不更新 |
| 忘记处理 `WindowState` 变化 | 全屏/最大化切换后红绿灯配置不更新 |

---

## 4. Linux (X11)

### 技术路径

| 维度 | 说明 |
|------|------|
| IsCsdEnabled | 取决于 `X11PlatformOptions.EnableDrawnDecorations` |
| WindowDrawnDecorations | ✅ 完整创建（TitleBar + Border + Shadow + ResizeGrips 四件套） |
| 标题栏 | Avalonia WindowDrawnDecorations 渲染 |
| 阴影 | Avalonia `BoxShadow` 自绘 |
| 圆角 | AtomUI 自绘（通过 `CornerRadius`） |
| Resize | Avalonia `ResizeGripLayer` |

### 激活双门

必须同时满足两个条件：

1. `X11PlatformOptions.EnableDrawnDecorations = true`（Program.cs 中配置）
2. `ExtendClientAreaToDecorationsHint = true`（由 `IsCsdEnabled=true` 的 Style 自动设置）

缺任一条件 → 原生 WM 标题栏，自定义装饰不生效。

### 阴影点击穿透（SHAPE 扩展）

X11 上 Avalonia 的 OS 窗口矩形包含阴影 buffer，默认 input region = 整个矩形 → 阴影区域会拦截鼠标事件。

解决方案：通过 `XShapeCombineRectangles` 把 input region 收窄到可见窗体 + 窄 resize 带。

AtomUI 实现：`Window` 构造函数中调用 `this.AttachClickThroughShadow()`，监听 `FrameShadowThickness` 变化自动重算。

```csharp
// Window.cs 构造函数
if (OperatingSystem.IsLinux())
{
    this.AttachClickThroughShadow(
        new HashSet<AvaloniaProperty> { FrameShadowThicknessProperty },
        () => FrameShadowThickness);
}
```

### 非 CSD 模式（IsCsdEnabled=false）

当 `EnableDrawnDecorations=false` 时，Linux 走非 CSD 模板：
- `WindowDecorations` 设为 `None`（隐藏原生 WM 标题栏）
- AtomUI 自绘标题栏 + BoxShadow 阴影
- 需要 SHAPE 点击穿透

### 陷阱

| 陷阱 | 说明 |
|------|------|
| 没设 `EnableDrawnDecorations=true` | X11 上原生标题栏还在，自定义装饰不生效 |
| `EnableDrawnDecorations` 编译警告 | 标了 `[Experimental("AVALONIA_X11_CSD")]`，需 `<NoWarn>AVALONIA_X11_CSD</NoWarn>` |
| ShadowThickness 太大不做 SHAPE 裁剪 | 阴影区变 resize 光标，用户体验差 |
| Wayland 担心兼容性 | 实际走 XWayland，X11 SHAPE 仍可用，无需额外处理 |
| 最大化/全屏时忘记重置 input region | 框架把 ShadowThickness 归零，需重置为整个客户区 |

---

## 5. Windows（全版本统一方案）

### 技术路径

| 维度 | Win11 (build ≥ 22000) | Win10 (build < 22000) |
|------|----------------------|----------------------|
| IsCsdEnabled | `false` | `false` |
| WindowDrawnDecorations | **不创建** | **不创建** |
| 标题栏 | AtomUI 非 CSD 模板自绘 | AtomUI 非 CSD 模板自绘 |
| 阴影 | DWM（手动启用） | DWM（手动启用） |
| 圆角 | DWM 原生（`DWMWCP_ROUND`） | 无（DWM 不支持） |
| Resize | WndProc Hook 拦截 `WM_NCHITTEST` | WndProc Hook 拦截 `WM_NCHITTEST` |

### 为什么全 Windows 不能走 CSD

Avalonia 12 CSD 在 Windows 上有两个严重 resize bug（Win10 和 Win11 均存在）：

1. **右边闪烁**：从右边缘往左拉缩小窗口到最小宽度后继续拉，窗口右半部分闪烁
2. **左边推窗口**：从左边缘往右拉到最小宽度后，窗口整体被推着往右走

根因：
- `ExtendClientArea()` 在每次 `WM_SIZE` 状态变化时被调用，重置 `DWMNCRP_DISABLED`，与 DWM 阴影恢复产生时序竞争
- Avalonia 的 `WM_NCCALCSIZE` 处理在 `_isClientAreaExtended=true` 时做复杂边框计算，constrained resize 时坐标出错
- WindowDrawnDecorations 三层布局增加 resize 时的布局复杂度

曾尝试在 Win11 上保持 CSD 启用并通过 WndProc hook 拦截 WM_NCCALCSIZE，但因 Avalonia 内部 `_isClientAreaExtended=true` 状态与实际窗口行为不匹配，闪烁反而更严重。

### 完整实现方案

Win32 原生逻辑集中在 `AtomUI.Native` 项目中：

| 层 | 文件 | 职责 |
|---|------|------|
| 常量 | `AtomUI.Native/Windows/WindowUtils.Interop.cs` | `WM_NCCALCSIZE`、`WM_NCHITTEST`、`HT*` 系列、DWM 属性 |
| Native 逻辑 | `AtomUI.Native/Windows/WindowUtils.Windows.cs` | `ApplyDwmShadow()`、`HandleNcCalcSize()`、`HitTestBorder()` |
| 窗口控件 | `AtomUI.Desktop.Controls/Window/Window.cs` | CSD 状态判断、WndProc hook 注册、WindowState 处理 |

#### 5.1 禁用 CSD

`ConfigureCsdStatus()` 中全 Windows 设 `IsCsdEnabled=false`，`WindowTheme.axaml` 不设 `ExtendClientAreaToDecorationsHint` → Avalonia 不调用 `ExtendClientArea()` → 整条 CSD 链路不触发。

#### 5.2 注册 WndProc Hook

在 `OnOpened()` 中注册（构造函数时 HWND 尚未创建），通过 `Win32Properties.AddWndProcHookCallback()` 在 Avalonia WndProc 之前拦截消息：

```csharp
Win32Properties.AddWndProcHookCallback(this, WinWndProcHook);
ApplyWinDwmShadow();

// 强制 Windows 重新发送 WM_NCCALCSIZE，让 hook 生效
WindowUtilsInterop.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
    SWP_FRAMECHANGED | SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE);
```

#### 5.3 WM_NCCALCSIZE — 去掉原生标题栏

拦截 `WM_NCCALCSIZE`，不修改 rect → client area = window area → 原生标题栏消失。

最大化时需要补偿边框：Windows 最大化窗口会超出屏幕边缘一个边框厚度来隐藏 resize 边框，不补偿会导致标题栏顶部被裁剪。

```csharp
// WindowUtilsWindows.HandleNcCalcSize(hWnd, lParam)
var style = GetWindowLongPtr(hWnd, GWL_STYLE);
if ((style & WS_MAXIMIZE) != 0)
{
    var borderX = GetSystemMetrics(SM_CXSIZEFRAME) + GetSystemMetrics(SM_CXPADDEDBORDER);
    var borderY = GetSystemMetrics(SM_CYSIZEFRAME) + GetSystemMetrics(SM_CXPADDEDBORDER);
    nccsp.rgrc0.left += borderX;
    nccsp.rgrc0.top += borderY;
    nccsp.rgrc0.right -= borderX;
    nccsp.rgrc0.bottom -= borderY;
}
```

#### 5.4 WM_NCHITTEST — Resize 边框

`WindowUtilsWindows.HitTestBorder(hWnd, lParam, borderWidth)` 在窗口边缘 4 DIP 范围内返回对应的 HT 值（HTLEFT/HTRIGHT/HTTOP/HTBOTTOM 及四角）。

最大化和全屏时不处理（不需要 resize 边框）。

标题栏拖拽由 `WindowTitleBar` 的 `BeginMoveDrag()` 处理，不需要返回 `HTCAPTION`。

#### 5.5 DWM 阴影 + Win11 圆角

`WindowUtilsWindows.ApplyDwmShadow(hwnd)` 手动启用 DWM NC 渲染 + 扩展 DWM 帧：

```csharp
// 启用 DWM NC 渲染
DwmSetWindowAttribute(hwnd, DWMWA_NCRENDERING_POLICY, &DWMNCRP_ENABLED, sizeof(int));

// 扩展 DWM 帧到整个窗口 → 触发阴影
DwmExtendFrameIntoClientArea(hwnd, ref new MARGINS { -1, -1, -1, -1 });

// Win11: 保留原生圆角
if (Environment.OSVersion.Version.Build >= 22000)
    DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, &DWMWCP_ROUND, sizeof(int));
```

因为不走 `ExtendClientAreaToDecorationsHint`，Avalonia 永远不会覆盖这个设置。

#### 5.6 全屏状态处理

| 状态 | DWM 处理 |
|------|----------|
| 全屏 | 不做 DWM 操作（避免干扰 OS 全屏动画），仅标记 `_wasFullScreen=true` |
| 退出全屏 | 用 `Dispatcher.UIThread.Post(ApplyWinDwmShadow, DispatcherPriority.Send)` 延迟恢复。因为 Avalonia 的 `SetFullScreen(false)` 会调用 `UpdateWindowProperties(forceChanges: true)` 重置 margins |
| 最大化 | DWM 阴影自动隐藏（OS 行为），WM_NCCALCSIZE 补偿边框 |

### 陷阱

| 陷阱 | 说明 |
|------|------|
| Hook 注册太晚 | 窗口 Show() 时第一个 WM_NCCALCSIZE 已处理，原生标题栏闪现。必须在注册后调用 `SetWindowPos(SWP_FRAMECHANGED)` 强制重发 |
| 退出全屏后阴影消失 | Avalonia `UpdateWindowProperties(forceChanges: true)` 重置 DWM margins。必须用 `Dispatcher.UIThread.Post` 延迟恢复 |
| 最大化时标题栏被裁剪 | Windows 最大化窗口超出屏幕边缘。WM_NCCALCSIZE 中必须检测 `WS_MAXIMIZE` 并 inset 边框厚度 |
| 试图在 Windows 上走 CSD | 会触发闪烁和推窗口 bug，这是 Avalonia 12 CSD 与 Windows DWM 的根本冲突，Win10 和 Win11 均存在 |
| 用 `Dispatcher.UIThread.Post(ApplyDwmSystemShadow)` 在 CSD 模式下修复阴影 | 时序竞争，`ExtendClientArea()` 在每次 WM_SIZE 都会重置，Post hack 不可靠 |

---

## 7. AtomUI Window 架构：IsCsdEnabled 双模板机制

### WindowTheme.axaml 模板结构

```xml
<!-- IsCsdEnabled=False 模板（macOS / Win10 / Linux 非 CSD） -->
<Style Selector="^[IsCsdEnabled=False]">
    <!-- 不设 ExtendClientAreaToDecorationsHint -->
    <!-- 自带标题栏区域、WindowFrame 背景、BoxShadow 阴影 -->
    <Panel>
        <Border Name="WindowFrame" Background="..." CornerRadius="..." BoxShadow="..."
                Margin="{TemplateBinding FrameShadowThickness}">
            <!-- WindowFrameLayer -->
        </Border>
        <VisualLayerManager Margin="{TemplateBinding FrameShadowThickness}">
            <DockPanel>
                <Panel DockPanel.Dock="Top"> <!-- 标题栏 --> </Panel>
                <Panel> <!-- 内容区 --> </Panel>
            </DockPanel>
        </VisualLayerManager>
    </Panel>
</Style>

<!-- IsCsdEnabled=True 模板（Linux CSD） -->
<Style Selector="^[IsCsdEnabled=True]">
    <Setter Property="ExtendClientAreaToDecorationsHint" Value="True" />
    <!-- 无自绘标题栏（由 WindowDrawnDecorations 提供） -->
    <!-- 无 BoxShadow（由 DWM 或 Avalonia CSD 提供） -->
    <Panel>
        <Border Name="WindowFullScreenFrame" IsVisible="False"> <!-- 全屏背景 --> </Border>
        <VisualLayerManager>
            <Panel Margin="{Binding $parent[Window].WindowDecorationMargin}">
                <!-- 内容区 -->
            </Panel>
        </VisualLayerManager>
    </Panel>
</Style>
```

### 平台特定样式

```xml
<!-- Linux 非 CSD：隐藏原生 WM 标题栏 -->
<Style Selector="^[OsType=Linux][IsCsdEnabled=False]">
    <Setter Property="WindowDecorations" Value="None" />
</Style>

<!-- Linux CSD：保持 Full 装饰 -->
<Style Selector="^[OsType=Linux][IsCsdEnabled=True]">
    <Setter Property="WindowDecorations" Value="Full" />
</Style>

<!-- Linux：自绘阴影和圆角 -->
<Style Selector="^[OsType=Linux]">
    <Setter Property="FrameShadow" Value="{atom:WindowTokenResource FrameShadows}" />
    <Setter Property="CornerRadius" Value="{atom:WindowTokenResource CornerRadius}" />
</Style>

<!-- 最大化/全屏：去掉阴影和圆角 -->
<Style Selector="^[OsType=Linux][WindowState=Maximized]">
    <Setter Property="FrameShadow" Value="0 0 0 0 #00000000" />
    <Setter Property="CornerRadius" Value="0" />
</Style>
```

---

## 8. 跨平台陷阱速查表

| # | 陷阱 | 影响平台 | 现象 | 解决 |
|---|------|----------|------|------|
| 1 | 全局设 `ExtendClientAreaToDecorationsHint=True` | Win10 | 闪烁 + 推窗口 | 移到 `[IsCsdEnabled=True]` 条件下 |
| 2 | macOS 写 WindowDrawnDecorations 主题 | macOS | 不生效（死代码） | macOS 走非 CSD 模板 + NSWindow 原生 |
| 3 | 依赖 `:has-shadow` / `:has-border` 伪类 | Windows | 永远 false | 用无条件选择器写兜底样式 |
| 4 | X11 不设 `EnableDrawnDecorations` | Linux | 原生标题栏还在 | Program.cs 配置 `X11PlatformOptions` |
| 5 | 大阴影不做 SHAPE 裁剪 | Linux | 阴影区拦截鼠标 | `AttachClickThroughShadow()` |
| 6 | Win10 WndProc Hook 注册后不发 `SWP_FRAMECHANGED` | Win10 | 首次打开原生标题栏闪现 | `SetWindowPos` 强制重发 WM_NCCALCSIZE |
| 7 | Win10 退出全屏不延迟恢复 DWM | Win10 | 阴影消失 | `Dispatcher.UIThread.Post` |
| 8 | Win10 最大化不补偿边框 | Win10 | 标题栏顶部被裁剪 | WM_NCCALCSIZE 检测 WS_MAXIMIZE 并 inset |
| 9 | 用 OS 嗅探 (`#if MACOS`) 切标题栏 | 全平台 | 不响应运行时变化 | 用 `IsCsdEnabled` 属性驱动模板切换 |
| 10 | `GetVisualDescendants<WindowDrawnDecorations>()` | 全平台 | 永远找不到 | 它是 StyledElement 不是 Visual |
| 11 | CSD 模板里也画标题栏 | Linux | 双标题栏 | CSD 模板不含标题栏，由 WindowDrawnDecorations 提供 |
| 12 | Win10 上试图用 Post hack 修复 CSD 阴影 | Win10 | 时序竞争不可靠 | 完全绕过 CSD，用纯 Win32 方案 |

---

## 9. 关键源码索引

### AtomUI 源码

| 文件 | 作用 |
|------|------|
| `src/AtomUI.Desktop.Controls/Window/Window.cs` | 核心窗口类：`ConfigureCsdStatus()`、Win10 WndProc Hook、DWM 阴影、macOS 配置 |
| `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml` | 双模板 ControlTheme：CSD / 非 CSD 模板切换 |
| `src/AtomUI.Native/Windows/WindowUtils.Interop.cs` | Win32 P/Invoke：DWM API、GetWindowRect、SetWindowPos、GetSystemMetrics |

### Avalonia 12 参考源码

| 文件 | 关键内容 |
|------|----------|
| `Avalonia.Win32/WindowImpl.cs` | `NeedsManagedDecorations`、`RequestedDrawnDecorations`、`ExtendClientArea()` |
| `Avalonia.Win32/WindowImpl.AppWndProc.cs` | WM_NCCALCSIZE / WM_SIZE 处理 |
| `Avalonia.Win32/WindowImpl.CustomCaptionProc.cs` | WM_NCHITTEST、HitTestNCA |
| `Avalonia.Win32/WindowImpl.WndProc.cs` | WndProc 分发链 |
| `Avalonia.Controls/Platform/Win32Properties.cs` | `AddWndProcHookCallback()` API |
| `Avalonia.Controls/TopLevelHost.Decorations.cs` | WindowDrawnDecorations 创建、ResizeGripLayer |
| `Avalonia.Controls/Window.cs` | `ComputeDecorationParts()`、`WindowDecorationMargin` |
| `Avalonia.X11/X11Window.cs` | `EnableDrawnDecorations` gating、`RequestedDrawnDecorations` 四件套 |
| `Avalonia.Native/WindowImpl.cs` | `NeedsManagedDecorations => false`（macOS 硬编码） |

### 相关文档

| 文档 | 内容 |
|------|------|
| `docs/Native/WindowsDisableCsdShadowScheme.md` | Windows 禁用 CSD + 纯 Win32 DWM 阴影方案详细设计 |
| `docs/Native/WindowDrawnDecorations.md` | Avalonia 12 WindowDrawnDecorations 完整使用指南 |
