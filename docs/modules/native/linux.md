# AtomUI.Native Linux 平台 API

## 范围

AtomUI 在 Linux 上有两条完全不同的窗口后端路径：

| 后端 | Avalonia 包 | 原生对象 | AtomUI 可使用的能力 |
|---|---|---|---|
| X11 / XWayland | `Avalonia.X11` | `IPlatformHandle`，descriptor 为 `XID` | Xlib 初始几何、`_GTK_FRAME_EXTENTS`、XCB SHAPE input region |
| 原生 Wayland | `Avalonia.Wayland` | `IWindowImpl.Handle == null` | xdg-shell move/resize、CSD/SSD、surface geometry；无公开 input-region API |

不得只用 `OperatingSystem.IsLinux()` 决定调用 X11 API。所有 Xlib/XCB 路径还必须验证窗口后端为
X11；原生 Wayland、Linux framebuffer 和 headless 后端都不是 X11。

本文以 AtomUI 当前公开的平台契约、后端隔离规则和验证条件为准。Avalonia 内部类型仅用于解释集成边界，
不能作为 AtomUI 可直接调用的稳定 API。

## X11 能力

### 初始窗口几何

`WindowExtensions.ConfigureLinuxInitialWindowGeometry()` 将 Avalonia DIP 尺寸转成 device pixel，
再调用 `WindowUtilsLinux.ConfigureInitialWindowGeometry()`：

1. `XOpenDisplay()` 打开独立 Xlib 连接。
2. `XSetWMNormalHints()` 写入 position、size、min/max size 和 resize increment。
3. `XMoveResizeWindow()` 在 map 前设置窗口位置和大小。
4. `XFlush()` 后关闭该次 Xlib 连接。

该逻辑依赖 XID 和全局屏幕坐标，只能由 `X11WindowChromeManager` 调用。Wayland surface 没有全局位置，
`Avalonia.Wayland.WindowImpl.Move(PixelPoint)` 明确是 no-op。

### CSD frame extents

`WindowExtensions.SetLinuxX11CsdFrameExtents()` 先验证 platform handle descriptor 为 `XID`，然后由
`WindowUtilsLinux.SetX11CsdFrameExtents()` 写入 `_GTK_FRAME_EXTENTS`。

属性格式为四个 32-bit CARDINAL，顺序是 `left, right, top, bottom`，单位为 device pixel。AtomUI 在
normal 状态写入阴影 extents；最大化和全屏写入全零，避免 Mutter/KWin 继续按旧阴影范围做放置和吸附。

### SHAPE input region

`ClickThroughShadowExtensions` 用 XCB SHAPE 的 `XCB_SHAPE_SK_INPUT` 把输入区限制为一个矩形，目的
是让视觉阴影区域的点击穿透到下方窗口，同时保留窄 resize band。

当前实现的真实生命周期：

1. `XcbConnectionHolder` 懒创建一条进程级独立 XCB 连接。
2. 首次初始化时查询并缓存 SHAPE 扩展是否可用。
3. 所有请求在 `XcbConnectionHolder.SyncRoot` 下串行执行。
4. `xcb_shape_rectangles_checked()` 替换 input region，随后检查请求并 flush。
5. 该连接在进程生命周期内复用；不是每次调用都 connect/disconnect。

`ClickThroughShadowExtensions` 监听 `Bounds`、`WindowDecorationMargin`、`WindowState`、
`TransparencyLevelHint` 以及调用方给出的额外属性。坐标先按 `RenderScaling` 从 DIP 转成 device pixel。
最大化或全屏时 input region 恢复为整个 surface。

这不是“整窗忽略鼠标”的 API。Linux 版本的 `SetWindowIgnoreMouseEventsLinux()`、
`IsWindowIgnoreMouseEventsLinux()`、`xcb_get_geometry()` 和 shape-query 旧实现已经删除；文档和新代码都
不得继续引用这些成员。

## Wayland 能力与线程模型

Avalonia Wayland 后端使用专用 `AvaloniaWayland` worker 线程：

```text
UI thread WindowImpl
  -> WXdgTopLevelProxy
  -> WaylandWorkerClient.Marshaller
  -> PostWithCommit / PostOob
  -> worker-thread WSurface / WlSurface
```

源码中的关键约束：

- `WaylandWorkerClient.CreateTopLevelHandle()` 只向 UI 线程暴露 `WXdgTopLevelProxy`，注释明确说明真实
  `WXdgTopLevel` 是 worker-thread state。
- 普通 surface 状态由 `PostWithCommit` 排队，与 compositor commit 保持顺序；move/resize 等需要输入
  serial 的请求使用 OOB 路径。
- `WSurface.OnBeforeNewBufferAttached()` 在 attach/commit 前统一应用 frame callback、ack configure、
  window geometry、viewport 和 buffer scale。
- `WindowImpl.SetShadowExtents()` 通过 proxy 更新 persistent surface；`WSurface` 再根据 shadow extents
  调用 `xdg_surface.set_window_geometry`。

Wayland 核心协议虽然有 `wl_surface.set_input_region`，但当前公开框架 API 不能表达该能力。AtomUI 当前
`WaylandWindowReflectionExtensions` 从私有 proxy 中取出真实 target，并由 `WaylandWindowUtils` 直接调用
NWayland。这绕过了 worker marshalling 和重连模型，只能作为当前技术债记录，不能视为线程安全或受
Avalonia 支持的 Native 扩展点。后续正确实现应进入 Avalonia persistent surface/proxy，并通过
`PostWithCommit` 下发。

## Wayland CSD/SSD

Wayland 的 `NeedsManagedDecorations` 来自 xdg-decoration 协商：

- Client-side mode：请求 `TitleBar | Border | Shadow | ResizeGrips`。
- Server-side mode：`NeedsManagedDecorations=false`，由 compositor 提供 SSD。
- 协商结果变化时触发 `DrawnDecorationsRequestChanged`。

`WindowDecorations` 在 Wayland 上不是可逆的 CSD/SSD toggle。当前后端的
`SetWindowDecorations(None/TitleBar)` 会设置 `_csdSticky=true` 并销毁 decoration object，之后设置
`Full` 也不会恢复 SSD。因此，AtomUI 不能在 `IsCsdEnabled=False` 时无条件设置
`WindowDecorations=None`；这会把刚协商出的 SSD 立即永久切回 CSD。

## 依赖和边界

`AtomUI.Native` 与 `Avalonia.Wayland` 共享 NWayland 类型，必须在依赖升级时验证最终解析版本和 ABI 兼容性。
`NWayland` 因而会进入所有引用 `AtomUI.Native` 的依赖图，包括 Browser 的共享项目闭包。Browser 项目
目前通过内部 WASM item 过滤桌面 assembly，这能消除已知 P/Invoke collector warning，但不能消除
restore 图中的桌面依赖，也不是理想的模块边界。

长期结构应把 X11、Wayland、Win32、macOS 后端能力放到可选平台 assembly，避免
`AtomUI.Desktop.Controls`/Browser 为单个平台实现携带所有桌面后端和协议依赖。平台 manager 负责
Avalonia 控件生命周期和状态机；`AtomUI.Native` 只负责已经确定后端之后的原生调用，不负责主题、
reflection discovery 或窗口策略。

## 实现与升级检查入口

### AtomUI

- `src/AtomUI.Native/Linux/WindowUtils.Interop.cs`
- `src/AtomUI.Native/Linux/WindowUtils.Linux.cs`
- `src/AtomUI.Native/Linux/XcbConnectionHolder.cs`
- `src/AtomUI.Native/Linux/ClickThroughShadowExtensions.cs`
- `src/AtomUI.Native/Linux/WaylandWindowUtils.cs`
- `src/AtomUI.Native/WindowExtensions.cs`
- `src/AtomUI.Desktop.Controls/Window/X11WindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/WaylandWindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/WaylandWindowReflectionExtensions.cs`

### 框架集成验证项

- Wayland 窗口只向 UI 线程暴露 proxy，真实协议对象留在 worker 线程。
- 普通 surface 状态通过 commit 队列提交，不能从 UI 线程直接操作协议对象。
- CSD/SSD 协商、shadow extents 与 window geometry 保持同一状态传播链。
- X11 路径继续以 `XID` descriptor 为门槛，不能退化为 Linux OS 判断。

框架升级时应针对当前依赖重新验证这些行为，不在长期文档中固化外部文件路径、行号或源码快照。
