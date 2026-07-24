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

Windows 10 继续使用已经实机验证稳定的 `RedirectionSurface`。Windows 11 实机验证也能观察到
左边缘和上边缘 live resize 时的帧不同步，因此默认同样收敛到
`RedirectionSurface`。这是平台合成模式策略，不是 Window 控件内的消息补丁。AtomUI 不处理
`WM_NCCALCSIZE`，不返回 resize hit-test，不扩展 DWM frame，也不通过 `SWP_FRAMECHANGED`
强制重算非客户区。

## 机制与决策依据

本节记录 AtomUI 选择合成模式时必须保持的机制结论，不把某次框架版本差异或源码快照作为长期架构依据。

### WinUI surface 的尺寸提交

当前 WinUI composition surface 从 render scene 读取尺寸和缩放：

```csharp
var size = sceneInfo.Size;
var scale = sceneInfo.Scaling;
```

surface 还会根据 scene transparency 选择 alpha mode。这使尺寸提交依赖 composition scene 与 render tick
的时序；AtomUI 的平台策略必须通过实机 live resize 验证这种时序，而不能从外部实现细节推导稳定性。

### Windows 10 和 Windows 11 的回调行为本来就不同

`WinUiCompositorConnection` 明确记录：`RequestCommitAsync` 的完成回调在 Windows 10 的
`DispatchMessage()` 中触发，在 Windows 11 的 `GetMessage()` 中触发。不能把观察到的错帧简单归因于
某次框架升级；AtomUI 维护的是以下可验证结论：

1. 操作系统原有的回调差异一直存在；
2. WinUI drawing surface 的提交依赖 scene size；
3. 在测试机的 Windows 10 live resize 中，两者组合后出现了可见错帧；
4. `RedirectionSurface` 实机验证可以消除外边缘错帧。

后续 Windows 11 实机视频显示，同类错帧也会出现在 Windows 11 上。由于该现象仍发生在
Avalonia Win32 CSD 拥有非客户区和 resize hit-test 的路径内，AtomUI 不重新接管窗口消息，
而是把 Windows 11 默认合成模式也切到 `RedirectionSurface`。

### Avalonia 的非客户区所有权没有迁移给 AtomUI

Avalonia Win32 负责 `WM_NCCALCSIZE`、managed decorations、shadow extents 和非客户区状态同步。
AtomUI 不重新实现 Win32 chrome，也不根据外部源码差异改变所有权边界。

开发过程中曾尝试增加 AtomUI WndProc、DWM frame 和 non-client refresh。这是中间实验，
不是框架集成要求。它与 Avalonia CSD 形成重复所有权，会引入黑边、原生标题栏按钮
闪现和新的 resize 回归，最终实现必须删除这条路径。

这条边界不等于禁止所有 Win32 消息封装。若某个 host 需要修正 Windows 原生 sizing boundary，例如
`WM_GETMINMAXINFO` 中 track size 与 CSD client/frame 差值不一致，相关消息结构体和 hook 生命周期应封装在
`AtomUI.Native`，由上层 host 显式启用并释放。该能力不能处理 `WM_NCCALCSIZE`、`WM_NCHITTEST`、
caption hit-test、DWM frame 或 Snap Layout。

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

AtomUI 使用 Avalonia 公开的 `WindowDecorationProperties.ElementRole`。Windows 标题栏按钮分别
声明 `MinimizeButton`、`MaximizeButton`、`CloseButton`、`FullScreenButton` 和
`DecorationsElement`，由 Avalonia Win32 把角色转换为正确的非客户区命中结果。

AtomUI 不再注册自定义 WndProc 来返回 `HTMAXBUTTON`，也不通过反射修改 `IsPointerOver`。

### AtomUI.Native

`AtomUI.Native` 继续承载 Avalonia 公共 API 无法表达的平台能力，例如 Windows 整窗鼠标穿透、
Windows sizing helper、macOS standard window buttons 和 Linux input region。Windows live resize 的合成
后端选择、CSD 所有权和 Window chrome 策略不下沉为 Native hack；Native 只封装上层明确启用的底层能力。

## 禁止重新引入

- 不要在 AtomUI 中处理 `WM_NCCALCSIZE`。
- 不要手动返回 `HTLEFT/HTTOP/HTRIGHT/HTBOTTOM` 等 resize hit-test。
- 不要调用 `DwmExtendFrameIntoClientArea` 修补 CSD 阴影。
- 不要用 `SWP_FRAMECHANGED`、延时、重试或强制刷新掩盖时序问题。
- 不要默认开启 `ShouldRenderOnUIThread`、`Software` 或 `Wgl` 来规避单机驱动问题。
- 不要用 WndProc hook 重复实现公开的 caption element roles。
- 不要把 Native sizing helper 扩展成第二套 Window chrome、caption hit-test 或 resize hit-test。
- 不要在 Windows 默认配置恢复 `WinUIComposition` / `DirectComposition` 优先级，除非上游变化后完成同等实机矩阵。

## 验证矩阵

### 自动验证

1. Windows 10 选项只包含 `RedirectionSurface`。
2. Windows 11+ 选项只包含 `RedirectionSurface`。
3. 渲染模式保持 `AngleEgl`、`Software`，且 `ShouldRenderOnUIThread=false`。
4. Windows caption buttons 使用公开 `ElementRole`，不存在自定义 caption WndProc 注册。
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

## 实现与升级检查入口

AtomUI：

- `src/AtomUI.Core/AppBuilderExtensions.cs`
- `src/AtomUI.Core/AppBuilderExtensions.cs`
- `src/AtomUI.Desktop.Controls/Window/Window.cs`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowDrawnDecorationsTheme.axaml`
- `src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/CaptionButtonGroupTheme.axaml`

升级 Avalonia 或调整 Windows 合成策略时，应重新验证 WinUI surface 的 scene-size 提交、compositor
回调时机、Win32 非客户区所有权和 caption element roles。外部类型和源码只能用于当次调查，不能把
特定版本、tag、文件路径或行号固化为本架构的事实来源。
