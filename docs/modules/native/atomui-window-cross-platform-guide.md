# AtomUI Window 跨平台定制完整指南

> 面向团队成员和 AI 模型的 AtomUI Window 跨平台窗口定制技术路径与陷阱手册。
> 基于 Avalonia 12 + AtomUI V6 实际实现总结。

---

## 目录

1. [总览：平台后端技术路径一图速记](#1-总览平台后端技术路径一图速记)
2. [核心概念](#2-核心概念)
3. [macOS](#3-macos)
4. [Linux (X11)](#4-linux-x11)
5. [Windows](#5-windows)
7. [AtomUI Window 架构：IsCsdEnabled 双模板机制](#7-atomui-window-架构iscsdenabled-双模板机制)
8. [跨平台陷阱速查表](#8-跨平台陷阱速查表)
9. [关键源码索引](#9-关键源码索引)

---

## 1. 总览：平台后端技术路径一图速记

```
平台              CSD 模式    标题栏由谁画         阴影由谁画        Resize 由谁提供     圆角
─────────────────────────────────────────────────────────────────────────────────────────
macOS             ❌ 关闭     AtomUI 自绘模板      OS 原生           OS 原生             OS 原生
Linux (X11)       ✅ 开启     Avalonia CSD         Avalonia BoxShadow Avalonia ResizeGrips AtomUI 自绘
Linux (Wayland)   由协商决定   Avalonia CSD / SSD   CSD: BoxShadow     CSD: ResizeGrips    CSD: AtomUI
Windows 11+        ✅ 开启     Avalonia CSD         Windows compositor  Avalonia Win32       DWM 原生
Windows 10         ✅ 开启     Avalonia CSD         DWM redirection      Avalonia Win32       DWM 原生
```

### 决策树

```
OperatingSystem.IsWindows()?
  ├─ Win10 → IsCsdEnabled=true + RedirectionSurface
  └─ Win11+ → IsCsdEnabled=true + WinUIComposition（带 DirectComposition/RedirectionSurface 回退）
OperatingSystem.IsMacOS()?
  └─ 永远 → IsCsdEnabled=false → 非 CSD 模板 + NSWindow 原生标题栏布局
OperatingSystem.IsLinux()?
  ├─ X11 → EnableDrawnDecorations + extend hint 决定 managed decorations
  └─ Wayland → xdg-decoration 协商决定 CSD/SSD，运行期可能变化
```

对应代码（`Window.cs`）：

```csharp
private void ConfigureCsdStatus()
{
    if (OperatingSystem.IsMacOS())
        IsCsdEnabled = false;
    else if (OperatingSystem.IsLinux())
        IsCsdEnabled = PlatformImpl?.NeedsManagedDecorations == true;
    else if (OperatingSystem.IsWindows())
        IsCsdEnabled = true;
}
```

---

## 2. 核心概念

### 2.1 CSD vs 非 CSD

- **CSD (Client-Side Decorations)**：Avalonia 12 的 `WindowDrawnDecorations` 机制。通过 `ExtendClientAreaToDecorationsHint=true` 激活，框架接管标题栏/边框/阴影的渲染。
- **非 CSD**：不设 `ExtendClientAreaToDecorationsHint`，Avalonia 不创建 `WindowDrawnDecorations`，标题栏由 Window 自己的 ControlTemplate 提供。

### 2.2 IsCsdEnabled 属性

AtomUI 自定义的 `DirectProperty<Window, bool>`。Windows 固定为 `true`；Linux 下它来自当前
`IWindowImpl.NeedsManagedDecorations`，并在 `DrawnDecorationsRequestChanged` 后重新读取，
驱动 `WindowTheme.axaml` 中的模板切换：

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

### 2.4 Windows 消息所有权

Avalonia Win32 独占 `WM_NCCALCSIZE`、resize hit-test 和 DWM non-client frame。AtomUI 不修改
客户区或窗口几何。

AtomUI 唯一保留的窗口消息接入位于 `CaptionButtonGroup`：对最大化按钮命中区域返回
`HTMAXBUTTON`，用于 Windows 11 Snap Layout。该 hook 不处理 resize 边缘。

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
| 把原生 Wayland 当成 XWayland | Wayland 的 `IWindowImpl.Handle` 为 `null`，不能调用 Xlib/XCB 或读取 XID |
| 最大化/全屏时忘记重置 input region | 框架把 ShadowThickness 归零，需重置为整个客户区 |

### 原生 Wayland（Avalonia 12.1）

`Avalonia.Wayland` 是独立后端，不经过 XWayland。`UseAtomUIPlatformDetect()` 在
`WAYLAND_DISPLAY` 非空时选择它；Avalonia 自带的 `UsePlatformDetect()` 在 Linux 仍只加载 X11。

Wayland 后端的源码约束：

- `WindowImplBase.Handle => null`，`Move(PixelPoint)` 和 `Activate()` 是 no-op。
- `NeedsManagedDecorations` 来自 `zxdg_toplevel_decoration_v1` 协商；CSD 时请求
  `TitleBar | Border | Shadow | ResizeGrips`。
- `SetShadowExtents` 会进入 persistent `WSurface`，并在下一次 buffer commit 前更新
  `xdg_surface.set_window_geometry`。
- UI 线程只能调用 `WXdgTopLevelProxy`。真实 `WSurface/WlSurface` 属于 `AvaloniaWayland` worker；
  协议状态通过 `WaylandWorkerClient.PostWithCommit` 排队。
- `WindowDecorations=None/TitleBar` 会锁定 sticky CSD，不能用它表示合成器协商出的 SSD。

因此，X11 SHAPE、`_GTK_FRAME_EXTENTS`、绝对窗口位置和 XID 初始化几何都只能留在 X11 manager。
Wayland input region 在 12.1 没有公开 API；AtomUI 当前越过 proxy 的反射实现不符合上游线程契约，
只能视为待替换的内部适配，不能作为通用 Native API 示例。

---

## 5. Windows

### 技术路径

| 维度 | Windows 11+ | Windows 10 |
| --- | --- | --- |
| IsCsdEnabled | `true` | `true` |
| WindowDrawnDecorations | 创建 | 创建 |
| 合成模式 | WinUIComposition，带回退 | RedirectionSurface |
| 非客户区与 resize | Avalonia Win32 | Avalonia Win32 |
| AtomUI WndProc | 仅 `HTMAXBUTTON` | 仅 `HTMAXBUTTON` |

### 为什么 Windows 10 使用 RedirectionSurface

Avalonia 12.1 的 WinUIComposition 在 Windows 10 上可能让合成表面提交落后于 live-resize
消息。拖动左边缘或上边缘时，原生窗口的对向边界已经稳定，但上一帧 scene size 仍被提交，
表现为右边缘或下边缘剧烈抖动。

`RedirectionSurface` 让 DWM redirection bitmap 与 HWND resize 走同一条系统路径，避免该错帧。
Windows 11 保持 WinUIComposition 优先，以保留高刷新率、透明和 backdrop 能力。

### 为什么必须使用 Avalonia CSD

旧 AtomUI Windows chrome 同时处理 `WM_NCCALCSIZE`、resize hit-test、DWM frame 和
`SWP_FRAMECHANGED`。Avalonia 12.1 也会管理 extended-client 模式下的 `WS_CAPTION` 与边框，
两套实现争夺非客户区所有权时，会出现黑边和系统标题栏按钮闪现。

Windows 现在固定 `IsCsdEnabled=true`，并保持
`ExtendClientAreaToDecorationsHint=true`。AtomUI 不再挂载 Windows chrome manager，
不再修改窗口几何。

### 合成模式配置

`AtomUI.Core` 不直接引用可选的 `Avalonia.Win32` 程序集。现有平台反射边界只负责创建
`Win32PlatformOptions`，策略集中在 `GetWin32CompositionModeNames()`：

```csharp
return OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000)
    ? ["WinUIComposition", "DirectComposition", "RedirectionSurface"]
    : ["RedirectionSurface"];
```

### Snap Layout

`CaptionButtonGroup` 继续对最大化按钮返回 `HTMAXBUTTON`。这只描述按钮命中区域，
不处理 `WM_NCCALCSIZE` 或 resize 边缘，因此不破坏单一几何所有权。

### 维护约束

- 不要重新引入 AtomUI 自定义 `WM_NCCALCSIZE`。
- 不要手动返回 `HTLEFT/HTTOP/...`。
- 不要用 `DwmExtendFrameIntoClientArea` 或 `SWP_FRAMECHANGED` 修补 CSD。
- 上游修复 Win10 WinUIComposition 后，必须重新做快速拖动左/上边缘的实机验证，才能恢复。

完整根因和验证矩阵见
[Windows live resize 与窗口装饰方案](windows-live-resize-scheme.md).

---

## 7. AtomUI Window 架构：IsCsdEnabled 双模板机制

### WindowTheme.axaml 模板结构

```xml
<!-- IsCsdEnabled=False 模板（macOS / Linux 非 CSD） -->
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

<!-- IsCsdEnabled=True 模板（Windows / Linux CSD） -->
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
<!-- 仅 X11 非 managed-decoration 场景：隐藏原生 WM 标题栏 -->
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

上面的 `WindowDecorations=None` 不能原样用于 Wayland SSD。Wayland 收到 `None` 后会设置 sticky CSD，
销毁 decoration object，并永久回到 CSD；Wayland SSD 必须保持 `WindowDecorations=Full`。

---

## 8. 跨平台陷阱速查表

| # | 陷阱 | 影响平台 | 现象 | 解决 |
| --- | --- | --- | --- | --- |
| 1 | Win10 使用 WinUIComposition | Win10 | 对向边缘在 live resize 时抖动 | 使用 RedirectionSurface |
| 2 | AtomUI 再处理 WM_NCCALCSIZE | Windows | 黑边、原生标题栏按钮闪现 | 非客户区完全交给 Avalonia CSD |
| 3 | 删除 HTMAXBUTTON hook | Win11 | Snap Layout 不再出现 | 只保留最大化按钮命中测试 |
| 4 | macOS 写 WindowDrawnDecorations 主题 | macOS | 不生效 | 使用非 CSD 模板和 NSWindow 原生按钮 |
| 5 | X11 不启用 drawn decorations | X11 | 原生标题栏仍存在 | 配置 EnableDrawnDecorations |
| 6 | X11 阴影不做 SHAPE 裁剪 | X11 | 阴影区域拦截鼠标 | 更新 input region |
| 7 | 把原生 Wayland 当 XWayland | Wayland | XID/native 调用失败 | 按 HandleDescriptor 分流 |
| 8 | UI 线程直接调用 WlSurface | Wayland | 破坏 worker 排序 | 通过 worker proxy 提交 |
| 9 | Wayland SSD 设置 Decorations=None | Wayland | 永久锁定 sticky CSD | SSD 保持 Full |
| 10 | 用 OS 编译宏决定运行时模板 | 全平台 | 后端变化后模板不更新 | 使用 IsCsdEnabled |

---

## 9. 关键源码索引

### AtomUI 源码

| 文件 | 作用 |
|------|------|
| `src/AtomUI.Desktop.Controls/Window/Window.cs` | 核心窗口类：CSD 状态与跨平台 chrome manager 生命周期 |
| `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml` | 双模板 ControlTheme：CSD / 非 CSD 模板切换 |
| `src/AtomUI.Core/AppBuilderExtensions.cs` | Windows 10/11 合成模式策略 |

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
| `Avalonia.Wayland/WindowImpl.cs` | CSD/SSD、sticky CSD、shadow extents、Wayland 能力限制 |
| `Avalonia.Wayland/Server/WaylandWorkerClient.cs` | UI→Wayland worker proxy 与 commit 编排 |
| `Avalonia.Wayland/Server/Persistent/WSurface.cs` | persistent surface、window geometry 与 commit 生命周期 |

### 相关文档

| 文档 | 内容 |
|------|------|
| `docs/modules/native/windows-live-resize-scheme.md` | Windows live resize、合成后端与 CSD 单一所有权方案 |
| `docs/modules/native/window-drawn-decorations.md` | Avalonia 12 WindowDrawnDecorations 完整使用指南 |
