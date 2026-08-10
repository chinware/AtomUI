# Windows Live Resize 与窗口装饰架构

本文档定义 AtomUI 在 Windows 上的 live resize、合成模式和窗口装饰所有权。该契约跨越应用启动配置、
`AtomUI.Desktop.Controls.Window`、Window 主题和 `AtomUI.Native`，具体使用方式见
[跨平台 Window 定制指南](../../../guides/windowing/cross-platform-window.md)。

## 平台策略

| 系统 | 渲染模式 | 合成模式 | 窗口装饰 |
|---|---|---|---|
| Windows 10 | `AngleEgl`，失败时 `Software` | `RedirectionSurface` | Avalonia CSD |
| Windows 11+ | `AngleEgl`，失败时 `Software` | `RedirectionSurface` | Avalonia CSD |

`RedirectionSurface` 是 AtomUI 的 Windows 默认合成策略。Window Control 不读取或切换合成后端；后端只在
`WithAtomUIDefaultOptions()` 的平台启动配置中确定。

## 单一所有权

AtomUI 在 Windows 上保持两个单一所有者：

- Avalonia Win32 拥有非客户区、resize hit-test、CSD、窗口状态和窗口几何。
- Avalonia 启动选项拥有渲染与合成后端选择。

AtomUI Window 负责 Control 状态、主题、标题栏内容和最小布局约束，不重新实现 Win32 chrome。AtomUI 不处理
`WM_NCCALCSIZE`，不返回边缘 resize hit-test，不扩展 DWM frame，也不通过 frame refresh 强制重算非客户区。

这条边界不禁止窄范围 Native 能力。`AtomUI.Native` 可以封装 Windows 公共 API 无法表达的 sizing boundary、
整窗鼠标穿透或明确的原生生命周期信号，但必须由上层显式启用和释放，且不能接管 caption、CSD 或 Snap Layout。

## 配置链路

```text
AppBuilder
  -> WithAtomUIDefaultOptions()
  -> Win32PlatformOptions
  -> CompositionMode = RedirectionSurface
  -> Avalonia Win32 platform initialization

AtomUI Window
  -> IsCsdEnabled = true
  -> ExtendClientAreaToDecorationsHint = true
  -> WindowDecorations = Full
  -> WindowDrawnDecorations / DWM owned by Avalonia Win32
```

启动配置直接使用 Avalonia 的强类型公开 API，不通过 `Type.GetType`、`Enum.Parse`、反射泛型调用或字符串枚举名
发现 Win32 options。API 变化必须在编译期暴露，NativeAOT 不依赖动态保留规则。

## Window 与 CSD

Windows Window 主题保持：

```xml
<Setter Property="ExtendClientAreaToDecorationsHint" Value="True" />
<Setter Property="WindowDecorations" Value="Full" />
```

`WindowDrawnDecorations`、resize grips、非客户区和窗口状态切换由 Avalonia 管理。AtomUI 不创建第二套边框、阴影、
caption hit-test 或 resize grip。

Windows CSD Window 的最小高度必须为装饰层和内容层保留稳定的非零空间。标题栏手动移动状态必须在以下路径清理：

- 普通 pointer release。
- pointer capture lost。
- 调用原生 `BeginMoveDrag` 之前。
- Window detach、关闭或模板重建。

同一次 pointer 序列不能从 resize 状态错误切换到 move 状态。

## 标题栏按钮

标题栏按钮使用 Avalonia `WindowDecorationProperties.ElementRole` 声明角色：

- `MinimizeButton`
- `MaximizeButton`
- `CloseButton`
- `FullScreenButton`
- `DecorationsElement`

AtomUI 不注册自定义 WndProc 返回 `HTMAXBUTTON`，不通过反射修改 pointer 状态，也不复制 Snap Layout 的命中逻辑。

## Live Resize 诊断边界

Live resize 问题分为两类，修复路径不能混用。

### 窗口外边缘错帧

拖动左边缘或上边缘时，对向边缘出现位置回跳，属于窗口 surface 与几何提交的同步问题。AtomUI 通过统一的
`RedirectionSurface` 平台策略处理，不在 Window 模板、布局或 Win32 消息层增加补丁。

### 内容区域抖动

窗口外框稳定但内容帧抖动时，先记录 GPU、驱动、Windows build、render mode 和 Avalonia 日志，并使用受支持的
驱动环境复测。该现象不能转化为 GPU 厂商判断、驱动版本分支、UI 线程渲染或软件渲染默认值。

## 禁止做法

- 在 AtomUI 中处理 `WM_NCCALCSIZE`。
- 手动返回 `HTLEFT`、`HTTOP`、`HTRIGHT`、`HTBOTTOM` 等 resize hit-test。
- 调用 `DwmExtendFrameIntoClientArea` 修补 CSD 阴影。
- 使用 `SWP_FRAMECHANGED`、延时、重试或强制刷新掩盖时序问题。
- 默认开启 `ShouldRenderOnUIThread`、`Software` 或 `Wgl` 规避单机环境问题。
- 使用 WndProc hook 重复实现公开的 caption element roles。
- 把 Native sizing helper 扩展成第二套 Window chrome 或 caption/resize hit-test。
- 由单个 Window 实例改变全局渲染或合成后端。

## 兼容性不变量

- Windows 10 和 Windows 11+ 默认使用 `RedirectionSurface`。
- Windows 默认 render modes 为 `AngleEgl` 和 `Software` fallback，`ShouldRenderOnUIThread=false`。
- Win32 非客户区、resize hit-test、CSD 和 Snap Layout 只有一个所有者。
- Window 主题始终通过 Avalonia CSD 表达 Windows 装饰。
- Caption button 只使用公开 element role。
- Native helper 必须显式 opt-in、可释放，并保持窄范围职责。
- 启动配置不依赖反射或运行时类型发现。

## 验证矩阵

### 自动验证

1. Windows 10 和 Windows 11+ options 只包含 `RedirectionSurface`。
2. Render modes 保持 `AngleEgl`、`Software`，且 `ShouldRenderOnUIThread=false`。
3. Caption buttons 使用公开 `ElementRole`，不存在自定义 caption WndProc 注册。
4. Window 主题保持 Avalonia CSD，不存在平行 Windows chrome manager。
5. Desktop tests、Browser build 和 NativeAOT publish 不依赖反射发现 Win32 options。
6. Windows CSD 最小高度为装饰层和内容层保留非零布局空间。
7. 标题栏拖动状态不会跨 release、capture lost、detach 或 `BeginMoveDrag` 保留。

### Windows 10 实机验证

1. 快速来回拖动左边缘，右边缘保持固定。
2. 快速来回拖动上边缘，下边缘保持固定。
3. Resize 期间没有黑框、透明条或原生标题栏按钮闪现。
4. 最大化、还原、全屏、焦点切换和退出全屏后装饰正常。
5. 缩到最小高度后继续 resize 不产生旧帧叠加或错误窗口移动。

### Windows 11+ 实机验证

1. 最大化按钮 Snap Layout 正常。
2. 左边缘和上边缘快速拖动时对向边缘稳定。
3. 透明 Popup 和独立窗口 Popup 没有黑边或闪烁回归。
4. 最大化、还原、全屏和退出全屏后装饰正常。

改变合成模式、render mode、CSD 所有权或 Native helper 边界时，必须重新完成自动验证和两个 Windows 实机矩阵。

## 实现入口

- `src/AtomUI.Core/AppBuilderExtensions.cs`
- `src/AtomUI.Desktop.Controls/Window/Window.cs`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowDrawnDecorationsTheme.axaml`
- `src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/CaptionButtonGroupTheme.axaml`
- `src/AtomUI.Native/Windows/`
