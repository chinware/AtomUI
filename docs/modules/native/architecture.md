# AtomUI.Native 架构

## 定位

`AtomUI.Native` 是 AtomUI 的内部原生调用层。它只负责在上层已经确定平台和窗口后端后执行原生操作，
不负责窗口主题、CSD/SSD 策略、Avalonia 私有对象发现或控件生命周期。

```text
AtomUI.Desktop.Controls
  - 选择 Window chrome manager
  - 管理 Window 事件、属性和模板生命周期
  - 维护 Avalonia private API reflection boundary
        |
        v
AtomUI.Native
  - 校验原生 handle / 参数
  - 调用 Win32、Objective-C、Xlib/XCB 或 NWayland
  - 管理原生资源和错误边界
```

所有成员目前都是 `internal`，通过 `InternalsVisibleTo` 向 `AtomUI.Core`、
`AtomUI.Desktop.Controls` 和预留的 `AtomUI.Mobile.Controls` 开放。它是实现包，不是公共 Native SDK。

## 当前依赖

| 依赖 | 用途 | 影响 |
|---|---|---|
| `Avalonia` | `Window`、`WindowBase`、geometry 和 platform handle | AtomUI 基础依赖 |
| `NWayland 0.11.0` | `WlSurface`、`WlCompositor`、`WlRegion` | 会进入所有引用 Native 的依赖图 |

`Avalonia.Wayland 12.1.0` 同样依赖 NWayland 0.11.0。类型版本当前一致，但这不是稳定的跨包 ABI
保证；升级 Avalonia.Wayland 时必须同步核对 NWayland 版本。

## 目录和职责

```text
src/AtomUI.Native/
├── AtomUI.Native.csproj
├── WindowExtensions.cs
├── Windows/
│   ├── WindowUtils.Interop.cs
│   └── WindowUtils.Windows.cs
├── MacOS/
│   ├── WindowUtils.Interop.cs
│   └── WindowUtils.MacOS.cs
└── Linux/
    ├── WindowUtils.Interop.cs
    ├── WindowUtils.Linux.cs
    ├── XcbConnectionHolder.cs
    ├── ClickThroughShadowExtensions.cs
    └── WaylandWindowUtils.cs
```

### Windows

- `SetWindowIgnoreMouseEventsWindows()` 修改 `WS_EX_TRANSPARENT/WS_EX_LAYERED`。
- `ApplyDwmShadow()` 和 `ForceWinNonClientFrameChanged()` 恢复 DWM non-client frame。
- `WinWndProcHook()` 处理 `WM_NCCALCSIZE` 和 `WM_NCHITTEST`。

### macOS

- Objective-C runtime 调用 `NSWindow.ignoresMouseEvents`。
- 查询和重排 standard window buttons。
- x64 的 32-byte `NSRect` 返回走 `objc_msgSend_stret`，其他架构走 `objc_msgSend`。

### Linux/X11

- Xlib 设置 map 前初始 geometry 和 `WM_NORMAL_HINTS`。
- `_GTK_FRAME_EXTENTS` 告知 Mutter/KWin CSD 阴影范围。
- 进程级独立 XCB 连接复用 SHAPE input-region 请求。

X11 能力必须以 `IPlatformHandle.HandleDescriptor == "XID"` 为前置条件，不能只判断 Linux OS。

### Linux/Wayland

`WaylandWindowUtils` 只接受已经解析出的 NWayland `WlSurface/WlCompositor`，创建 region 并调用
`SetInputRegion()`。它不应该知道 Avalonia 私有字段或查找 `WindowImpl`。

但 Avalonia 12.1 的真实 surface 属于专用 Wayland worker。当前调用方从 UI 线程越过
`WXdgTopLevelProxy` 直接调用真实 `WlSurface`，不符合上游线程和重连契约。`DynamicDependency` 只保证
reflection metadata 不被裁剪，不能解决该问题。正确边界应由 Avalonia persistent surface/proxy 提供
input-region 方法，并通过 `WaylandWorkerClient.PostWithCommit` 下发。

## 设计约束

1. 平台判断和后端判断是两层条件。Linux 不等于 X11，非 X11 也不等于 Wayland。
2. Avalonia private reflection 留在 `AtomUI.Desktop.Controls/Reflection` 或对应 Window 适配边界，
   Native 只接收已经确定的原生对象。
3. 原生资源必须有明确 owner 和释放点。临时 region 在请求入队后销毁；长期连接要记录进程生命周期策略。
4. 原生调用失败不能让 Window manager 的更新队列永久卡死；异步/延迟状态更新需要 `try/finally` 恢复标志。
5. AOT 要同时满足 metadata 保留和真实 publish；`DynamicDependency`、普通 build、源码字符串测试分别只能
   覆盖一部分风险。
6. Native 层不修改 Avalonia control properties，不订阅 Window 事件，不决定 resize band 或主题 token。

## 已知结构问题

### 平台依赖扩散

`AtomUI.Native` 被 `AtomUI.Core` 引用，因此 NWayland 会出现在 Browser 共享依赖闭包；
`AtomUI.Desktop.Controls` 又直接引用 `Avalonia.Desktop` 和 `Avalonia.Wayland`。Browser 当前通过 WASM
内部 item 过滤部分 desktop assembly，这只是构建补偿，不是清晰的依赖边界。

长期建议把 desktop bootstrap 与具体后端拆成可选 assembly，例如 X11、Wayland、Win32、macOS backend
包；Browser 只引用平台无关 controls，不应先引入桌面包再从 bundle 中删除 assembly。

### API 命名和校验

`WindowExtensions` 同时承担公共路由、Win32 hook、X11 geometry 和单位转换，职责过宽。后续应按能力拆成
`Win32WindowNative`、`X11WindowNative`、`MacWindowNative` 和 Wayland protocol adapter，并让每个入口
统一校验 handle descriptor、零句柄、尺寸范围和平台。

### 测试层级

当前 Native 相关测试主要读取源码并断言字符串。这能防止某些旧代码回归，但不能验证 ABI、线程、资源释放、
协议顺序或真实 NativeAOT。至少需要：

- 纯计算和参数校验单元测试。
- 可替换 native façade 的调用顺序/错误路径测试。
- X11/Wayland 集成测试或最小 smoke app。
- Linux/Windows/macOS 的 NativeAOT publish matrix。

## 事实源

Avalonia 行为以 `/workspace/projects/ReferenceProjects/Avalonia` 的 `12.1.0` tag
（commit `a21b9f573172f705a944dcc8aad7f036b9986f39`）为准，重点文件：

- `src/Avalonia.Wayland/WindowImpl.cs`
- `src/Avalonia.Wayland/WindowImplBase.cs`
- `src/Avalonia.Wayland/Server/WaylandWorkerClient.cs`
- `src/Avalonia.Wayland/Server/Persistent/WSurface.cs`
- `src/Avalonia.Controls/TopLevelHost.Decorations.cs`
- `src/Avalonia.X11/X11Window.cs`
