# Windows live resize 与窗口装饰架构

## 结论

AtomUI 在 Windows 上坚持两个单一所有者：

- Avalonia Win32 独占窗口非客户区、resize hit-test、CSD 和窗口几何。
- Avalonia 启动选项独占渲染与合成后端选择，Window 控件不感知后端细节。

当前平台策略如下：

| 系统 | 渲染模式 | 合成模式 | 窗口装饰 |
| --- | --- | --- | --- |
| Windows 10 | `AngleEgl`，失败时 `Software` | `RedirectionSurface` | Avalonia CSD |
| Windows 11+ | `AngleEgl`，失败时 `Software` | `RedirectionSurface` | Avalonia CSD |

Windows 10 继续使用已经实机验证稳定的 `RedirectionSurface`。Windows 11 在 Avalonia 12.1.0
下也能观察到左边缘和上边缘 live resize 时的帧不同步，因此默认同样收敛到
`RedirectionSurface`。这是平台合成模式策略，不是 Window 控件内的消息补丁。AtomUI 不处理
`WM_NCCALCSIZE`，不返回 resize hit-test，不扩展 DWM frame，也不通过 `SWP_FRAMECHANGED`
强制重算非客户区。

## Avalonia 12.0.5 到 12.1.0 的事实

以下结论来自 Avalonia 仓库 `12.0.5` 与 `12.1.0` 标签的源码差异。

### WinUI surface 的尺寸来源发生变化

`WinUiCompositedWindowSurface` 在 12.0.5 中从原生窗口信息读取尺寸和缩放：

```csharp
var size = _window.WindowInfo.Size;
var scale = _window.WindowInfo.Scaling;
```

12.1.0 改为从当前 render scene 读取：

```csharp
var size = sceneInfo.Size;
var scale = sceneInfo.Scaling;
```

12.1.0 还会根据 scene transparency 创建不同 alpha mode 的 composition drawing surface。
这使 WinUI surface 的尺寸提交更直接地依赖 composition scene 与 render tick 的时序。

### Windows 10 和 Windows 11 的回调行为本来就不同

`WinUiCompositorConnection` 明确记录：`RequestCommitAsync` 的完成回调在 Windows 10 的
`DispatchMessage()` 中触发，在 Windows 11 的 `GetMessage()` 中触发。该文件在 12.0.5 与
12.1.0 之间没有变化，因此不能把问题描述为“12.1 修改了消息循环”。更准确的结论是：

1. 操作系统原有的回调差异一直存在；
2. 12.1.0 改变了 WinUI drawing surface 对 scene size 的依赖；
3. 在测试机的 Windows 10 live resize 中，两者组合后出现了可见错帧；
4. `RedirectionSurface` 实机验证可以消除外边缘错帧。

后续 Windows 11 实机视频显示，同类错帧也会出现在 Windows 11 上。由于该现象仍发生在
Avalonia Win32 CSD 拥有非客户区和 resize hit-test 的路径内，AtomUI 不重新接管窗口消息，
而是把 Windows 11 默认合成模式也切到 `RedirectionSurface`。

### Avalonia 的非客户区所有权没有迁移给 AtomUI

`WindowImpl.AppWndProc.cs` 中 `WM_NCCALCSIZE` 的核心处理在两个标签间没有本质变化。
12.1.0 在 managed decorations 方向新增了请求变化通知和 shadow extents 同步，但没有要求
控件库重新实现 Win32 chrome。

开发过程中曾尝试增加 AtomUI WndProc、DWM frame 和 non-client refresh。这是中间实验，
不是 Avalonia 12.1 迁移要求。它与 Avalonia CSD 形成重复所有权，会引入黑边、原生标题栏按钮
闪现和新的 resize 回归，最终实现必须删除这条路径。

## 两类抖动必须分开诊断

### 窗口外边缘错帧

现象是拖动左边缘或上边缘时，对向的右边缘或下边缘来回跳动。实机验证表明，Windows 10
选择 `RedirectionSurface` 后外边缘稳定；Windows 11 也采用同一保守路径缓解该类错帧。
因此该问题由平台合成模式策略解决，而不是由 AtomUI Window 模板或布局系统处理。

### 窗口内容区域抖动

测试机 Intel HD Graphics 630 使用旧驱动 `31.0.101.2111` 时，在 `AngleEgl +
RedirectionSurface` 下仍有内容区抖动。升级 Intel 官方驱动到 `31.0.101.2141` 后，用户实测
内容区域不再抖动。

这是运行环境结论，不应转化为 AtomUI 的 GPU 厂商判断、驱动版本分支、UI 线程渲染或软件渲染
默认值。遇到“外框稳定但内容抖动”时，应先记录 GPU、驱动、Windows build 和 Avalonia 日志，
再用最新 OEM/芯片厂商驱动复测。

## 实现边界

### 启动配置

`WithAtomUIDefaultOptions()` 强类型创建 `Win32PlatformOptions`：

```csharp
CompositionMode = [Win32CompositionMode.RedirectionSurface];
```

这里直接引用 Avalonia 的公开类型，不使用 `Type.GetType`、`Enum.Parse`、反射调用泛型
`AppBuilder.With<T>()` 或字符串形式的枚举名。这样 API 变化会在编译期暴露，并且 NativeAOT
不依赖动态保留规则。

### Window 与 CSD

Windows 固定 `IsCsdEnabled=true`，主题保持：

```xml
<Setter Property="ExtendClientAreaToDecorationsHint" Value="True" />
<Setter Property="WindowDecorations" Value="Full" />
```

`WindowDrawnDecorations`、resize grips、非客户区和窗口状态切换全部由 Avalonia 管理。

AtomUI 的 Windows CSD 窗口同时保证 `MinHeight` 不低于两个标题栏高度。一个标题栏高度用于
装饰层，另一个标题栏高度为内容层保留稳定的非零布局/合成表面，避免窗口缩到系统默认最小
track 高度时让内容层进入零高度。标题栏的手动移动状态会在普通点击释放、pointer capture 丢失
以及调用原生 `BeginMoveDrag` 前清空，禁止同一次指针序列从 resize 错误切换到 move。

### 标题栏按钮

Avalonia 12.1 提供 `WindowDecorationProperties.ElementRole`。AtomUI 的 Windows 标题栏按钮分别
声明 `MinimizeButton`、`MaximizeButton`、`CloseButton`、`FullScreenButton` 和
`DecorationsElement`，由 Avalonia Win32 把角色转换为正确的非客户区命中结果。

AtomUI 不再注册自定义 WndProc 来返回 `HTMAXBUTTON`，也不通过反射修改 `IsPointerOver`。

### AtomUI.Native

`AtomUI.Native` 继续承载 Avalonia 公共 API 无法表达的平台能力，例如 Windows 整窗鼠标穿透、
macOS standard window buttons 和 Linux input region。Windows live resize、CSD 所有权和合成
后端选择不下沉为 Native hack。

## 禁止重新引入

- 不要在 AtomUI 中处理 `WM_NCCALCSIZE`。
- 不要手动返回 `HTLEFT/HTTOP/HTRIGHT/HTBOTTOM` 等 resize hit-test。
- 不要调用 `DwmExtendFrameIntoClientArea` 修补 CSD 阴影。
- 不要用 `SWP_FRAMECHANGED`、延时、重试或强制刷新掩盖时序问题。
- 不要默认开启 `ShouldRenderOnUIThread`、`Software` 或 `Wgl` 来规避单机驱动问题。
- 不要用 WndProc hook 实现 Avalonia 12.1 已公开的 caption element roles。
- 不要在 Windows 默认配置恢复 `WinUIComposition` / `DirectComposition` 优先级，除非上游变化后完成同等实机矩阵。

## 验证矩阵

### 自动验证

1. Windows 10 选项只包含 `RedirectionSurface`。
2. Windows 11+ 选项只包含 `RedirectionSurface`。
3. 渲染模式保持 `AngleEgl`、`Software`，且 `ShouldRenderOnUIThread=false`。
4. Windows caption buttons 使用公开 `ElementRole`，不存在自定义 WndProc 注册。
5. Window 主题保持 Avalonia CSD，源码不存在旧 Windows chrome manager。
6. Desktop 测试、Browser 构建和 NativeAOT publish 不依赖运行时反射发现 Win32 options。
7. Windows CSD 最小高度始终大于标题栏高度，并为内容合成表面保留非零高度。
8. 标题栏拖动状态不会跨 pointer release/capture lost 保留，也不会在 `BeginMoveDrag` 后复用。

### Windows 10 实机验证

1. 快速来回拖动左边缘，右边缘保持固定，内容区域不回跳。
2. 快速来回拖动上边缘，下边缘保持固定，内容区域不回跳。
3. resize 期间没有黑框、透明条或原生标题栏按钮闪现。
4. 失去和恢复焦点时不出现额外黑边。
5. 最大化、还原、全屏和退出全屏后装饰正常。
6. 最小化、最大化和关闭按钮点击、hover 与按下状态正常。
7. 从上边缘向下缩到最小高度后继续移动指针，窗口底边和窗口位置保持不变；随后从左边缘
   resize 不出现旧帧叠加。

### Windows 11 实机验证

1. 最大化按钮 Snap Layout 正常。
2. 快速来回拖动左边缘，右边缘保持固定，内容区域不回跳。
3. 快速来回拖动上边缘，下边缘保持固定，内容区域不回跳。
4. 透明 Popup 和独立窗口 popup 路径没有黑边或闪烁回归。
5. 最大化、还原、全屏和退出全屏后装饰正常。

## 上游回访条件

只有同时满足以下条件，才评估恢复 Windows 默认配置中的 WinUIComposition / DirectComposition 优先级：

1. 上游改动明确覆盖 Windows live resize 的 scene/surface 同步；
2. Windows 10 和 Windows 11 的左边缘、上边缘快速拖动都通过实机验证；
3. Intel、AMD 至少各一套驱动环境通过；
4. 透明 Popup 和窗口装饰回归通过；
5. 删除平台分支后代码和测试确实更简单。

## 参考源码

AtomUI：

- `src/AtomUI.Core/AppBuilderExtensions.cs`
- `src/AtomUI.Core/AppBuilderExtensions.cs`
- `src/AtomUI.Desktop.Controls/Window/Window.cs`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowDrawnDecorationsTheme.axaml`
- `src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/CaptionButtonGroupTheme.axaml`

Avalonia `12.0.5` / `12.1.0`：

- `src/Windows/Avalonia.Win32/WinRT/Composition/WinUiCompositedWindowSurface.cs`
- `src/Windows/Avalonia.Win32/WinRT/Composition/WinUiCompositorConnection.cs`
- `src/Windows/Avalonia.Win32/WindowImpl.AppWndProc.cs`
- `src/Windows/Avalonia.Win32/WindowImpl.CustomCaptionProc.cs`
- `src/Avalonia.Controls/Chrome/WindowDecorationProperties.cs`
