# Window

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Window 是 AtomUI 桌面控件体系中的桌面窗口控件，用于提供 AtomUI 自绘窗口、平台窗口能力和主题集成入口。

Window 不负责普通内容控件、Dialog 弹层或业务路由容器。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Window`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `未独立 Gallery 页面；以 `docs/controls/desktop/window/window` 源文档为准` |
| 状态 | Stable |

## 何时使用

Window 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Window 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Window 是 AtomUI 桌面控件体系中的桌面窗口控件，用于提供 AtomUI 自绘窗口、平台窗口能力和主题集成入口。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `ContentFrameBackground`、`ContentFrameLayer`、`ContentFrameLayerOpacity`、`ContentFrameLayerTemplate`、`IsTitleBarVisible`、`LogoTemplate`、`TitleBarFrameBackground`、`TitleBarFrameLayer` 等 11 项。`TitleBarFrameLayer` 表示标题栏背景或装饰层，不作为按钮、菜单、搜索框等交互控件入口。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Window Token + ControlTheme。 |

## 公共 API

Window 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ContentFrameBackground`、`ContentFrameLayer`、`ContentFrameLayerOpacity`、`ContentFrameLayerTemplate`、`IsTitleBarVisible`、`LogoTemplate`、`LeftAddOn`、`LeftAddOnTemplate`、`RightAddOn`、`RightAddOnTemplate`、`TitleBarFrameBackground`、`TitleBarFrameLayer`、`TitleBarFrameLayerOpacity`、`TitleBarFrameLayerTemplate` 等 | 定义控件展示内容、输入数据、模板或业务对象入口；`LeftAddOn` 和 `RightAddOn` 用于默认标题栏中的交互内容，`TitleBarFrameLayer` 仍只表示标题栏背景或装饰层。 |
| 选择与集合 | `ViewModel` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsMinimizeCaptionButtonVisible`、`IsMaximizeCaptionButtonVisible`、`IsCloseCaptionButtonVisible`、`IsFullScreenCaptionButtonVisible`、`IsPinCaptionButtonVisible`、`IsMoveEnabled` | 表达 managed caption button 呈现、窗口移动和用户可观察状态；visibility 不替代窗口 capability。 |
| 弹层与窗口 | `WindowFrameLayer`、`WindowFrameLayerOpacity` | 控制 popup、flyout、dialog、window 或 overlay 宿主协作。 |
| 其他稳定入口 | `Logo`、`LogoVisibility`、`MediaBreakPoint`、`OsType`、`OsVersion`、`TitleAlignment` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要 public 类型与枚举：

- 类型：`Window`、`ReactiveWindow<TViewModel>`、`MacStandardWindowButtons`。
- 枚举：无。

Window 模板还使用 `FullscreenPopoverLayer`、`WindowResizer`、`WindowVisualLayerClip` 等 internal
协作类型。它们会影响主题和可观察窗口行为，但不是用户可直接依赖的 public API。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_CloseButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_ContentPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_FullScreenButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_FullscreenPopoverLayer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_OverlayWrapper` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_PopoverBorder` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_PopoverCloseButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_PopoverFullScreenButton` | `?` | 承载用户触发入口、导航或关闭动作。 |
| `PART_RootLayout` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_TitleBar` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_TitleBarPresenter` | `?` | 展示用户内容、文本、图标或模板化数据。 |
| `PART_TransparencyFallback` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_UnderlayWrapper` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_VisualLayerManager` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |
| `PART_WindowFrame` | `?` | 承载根视觉、边框、背景或尺寸基线。 |
| `PART_WindowResizer` | `?` | 稳定模板协作入口，重命名前必须同步主题和实现。 |

当前未抽取到控件专属伪类；主题主要依赖 Avalonia 标准伪类、模板绑定和内部 StyledProperty。

## 事件与命令

Window 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。
当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

Gallery 目录 `未独立 Gallery 页面；以 `docs/controls/desktop/window/window` 源文档为准` 当前不存在；请检查控件文档中的 Gallery 页面元数据。

## 状态模型

Window 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- open/close 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。
- 默认标题栏的交互内容通过 `LeftAddOn`、`RightAddOn` 及其模板属性承载；`TitleBarFrameLayer` 只表达标题栏背景、遮罩或装饰视觉，不保证内部控件获得 pointer、focus、keyboard 或 command 事件。

Caption button 的 requested visibility 与窗口 capability 分离：Minimize、Maximize 和 Close 默认请求显示，FullScreen 和 Pin 默认隐藏。设置 visibility 为 `false` 只隐藏 AtomUI managed button，不修改 `CanMinimize`、`CanMaximize`、`WindowState`、`Topmost` 或其他窗口操作入口；capability 为 `false` 时对应 managed button 保持隐藏。完整模型见 [WindowTitleBar Caption Button 配置设计](../window-title-bar/caption-button-configuration-design.md)。

Window 为逻辑树内每个 `WindowTitleBar` 定义相同的宿主上下文投影，包括 caption 配置、窗口能力、WindowState、active state、Topmost、平台/CSD 输入和窗口操作命令。投影由 Window 创建为可释放 lease，由各标题栏实例分别持有；Window 不以单例 binding 容器限制一个窗口只能接入一个标题栏。

## 主题与 Design Token

Window 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `FullscreenPopoverLayerTheme.axaml` | 定义 macOS 全屏标题栏 popover 的固定模板、caption buttons 和标题展示。 |
| `WindowDrawnDecorationsTheme.axaml` | 定义 Avalonia drawn decorations overlay 下的标题栏、caption buttons、shadow 和 visible frame 裁剪结构；该 overlay 不承载 Dialog、Drawer 或其他业务 presentation。 |
| `WindowResizerTheme.axaml` | 定义 managed resize grip 的八向命中区域。 |
| `WindowTheme.axaml` | 定义普通 Window 模板、标题栏、内容 frame、visual layer、overlay host、fullscreen popover 和 managed resizer。 |

Window 使用 `WindowToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 open/close 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

### 5.1 标题栏背景层与自定义 TitleBar 模型

Window 标题栏按职责拆分为背景/装饰层、默认标题栏层和自定义标题栏层三类稳定语义。该模型同时约束普通自绘模板和 Avalonia `WindowDrawnDecorations` CSD 模板：

| 语义层 | 代表入口 | 职责 | 命中语义 |
| --- | --- | --- | --- |
| 标题栏背景/装饰层 | `TitleBarFrameBackground` / `TitleBarFrameLayer` / `TitleBarFrameLayerTemplate` | 提供标题栏背景、遮罩、纹理、圆角、裁剪或装饰视觉。 | 不作为用户交互入口；CSD 下可处于标题栏拖拽 role 中。 |
| 默认标题栏层 | `WindowTitleBar` | 展示标题、Logo、`LeftAddOn`、`RightAddOn` 与 caption buttons，并在空白区域提供窗口拖拽语义。 | add-on 与 caption buttons 按普通 Avalonia client input 语义命中；空白区域保留标题栏交互。 |
| 自定义标题栏层 | `NotifyCreateTitleBar` / `NotifyConfigureTitleBar` 扩展点 | 承载需要替换默认标题栏组成或行为的派生窗口实现。 | 派生窗口负责其自定义标题栏的 client input 与空白区域拖拽策略。 |
| 内容区标题栏 | Window 内容逻辑树中的 public `WindowTitleBar` | 自动消费最近 Window 的 caption 状态和操作命令，并在空白区域提供窗口拖动和双击最大化/还原语义。 | 不提供标题栏高度提示，也不取得唯一 CSD chrome role。 |

维护标题栏模板时，不应把 `TitleBarFrameLayer` 提升为可交互覆盖层。需要向默认标题栏加入按钮、菜单或搜索框时，使用 `LeftAddOn` 或 `RightAddOn`；只有需要替换整个标题栏组成或行为时，才在派生 `Window` 中重写标题栏创建与配置扩展点。`Window.TitleBar` 是模板生命周期拥有的 internal 状态，不作为应用 API 公开。

Window 的 title-bar host projection 服务默认、派生和内容区标题栏，并由每个标题栏的 logical attach/detach 生命周期持有。该 lease 同时包含 caption 状态/命令投影和窗口拖动、双击请求的交互订阅。默认标题栏另行消费 Window facade：`Window.TitleAlignment` add-owner `WindowTitleBar.TitleAlignmentProperty`，`LeftAddOn`、`LeftAddOnTemplate`、`RightAddOn` 和 `RightAddOnTemplate` 同样 add-owner 对应标题栏属性，并以 `Template` 优先级单向投影。派生标题栏以 local value 提供的内置操作区优先于 Window facade。`NotifyConfigureTitleBar` 只扩展这组默认内容配置，不负责通用宿主发现或 caption command 接入。Window 提供内容、平台、CSD、WindowState 和原生 chrome 安全区，但不实现标题排列公式。

CSD 模式下，`IsTitleBarVisible=false` 只隐藏 AtomUI drawn title-bar frame、shadow 和 presenter，不把 `WindowDecorations` 从 `Full` 降级为 `BorderOnly`。内容区同时移除 Avalonia drawn title-bar 对顶部 decoration margin 的占位，但继续保留 frame 和 shadow margin，因此用户内容可以到达窗口顶部且不破坏可调整大小边框。窗口最小化、最大化和恢复继续通过 Avalonia `WindowState` 表达，由 Windows DWM、macOS AppKit 或 Linux 窗口管理器/合成器在平台支持范围内执行原生状态转换和动画；AtomUI 不伪造窗口缩放动画，也不为 caption button 建立平台专用状态旁路。macOS 非 CSD 且隐藏原生标题栏时仍可使用 `BorderOnly`，该分支不改变 CSD 契约。

默认标题栏的 add-on 可直接使用 AXAML 属性元素配置：

```xml
<atom:Window>
  <atom:Window.LeftAddOn>
    <Button Content="Back" />
  </atom:Window.LeftAddOn>
  <atom:Window.RightAddOn>
    <Button Content="Settings" />
  </atom:Window.RightAddOn>
</atom:Window>
```
完整协作模型见 [WindowTitleBar 实现原理](../window-title-bar/implementation.md)。

### 5.2 跨平台首帧主题表面模型

Windows、macOS 和 Linux 共用同一个首次显示主题契约：平台窗口进入可见状态前，`Window` 必须已经获得目标
`ThemeContext`、与该 context 一致的 `RequestedThemeVariant`，以及当前 Window Token scope 中的首帧背景。
该契约由 `Window` 的共享显示生命周期负责，不属于 Win32、AppKit、X11 或 Wayland chrome manager 的职责。

首次显示按以下所有权模型维护：

- `WindowTheme.axaml` 是窗口背景的唯一长期视觉所有者，`WindowToken.DefaultBackground` 是默认背景语义真源。
- `Show` 和 `ShowDialog` 的所有 AtomUI 入口必须先解析 owner 作用域并挂载可释放的 `ThemeContext` lease，再进入 Avalonia 的平台显示流程。
- 在正式 ControlTheme 接管前，`Window` 从已提交的当前作用域 Snapshot 同步读取一次默认背景，并以低于用户 local value 的优先级临时预热 `Background` 与 `TransparencyBackgroundFallback`。
- 首帧预热只覆盖同步显示临界区，不订阅资源变化；Avalonia 完成同步样式应用后立即释放临时值，由正式 ControlTheme 继续响应主题切换。
- 用户显式设置的 `Background` 或 `TransparencyBackgroundFallback` 始终优先，首帧预热不得改写或清除用户 local value。
- 平台 chrome manager 只处理窗口装饰、原生几何和平台能力投影，不得分别复制 ThemeContext、Token 查找或首帧背景算法。

如果上述 managed 状态在平台显示前已经正确，某个平台仍然暴露尚未提交内容的原生空白 surface，则该问题属于平台后端边界。此时应通过统一的平台能力接口提供最小后备实现，并分别验证对应后端；不得把平台消息、延时显示或透明度切换混入共享主题状态机。

Token 来源：

Window Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `WindowToken`，scope id 为 `Window`，源码位于 `src/AtomUI.Desktop.Controls/Window/WindowToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token 或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 异步加载、上传、弹层和窗口生命周期必须能取消或释放。
- 缓存对象必须与控件、窗口、弹层或数据 owner 生命周期一致。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- 控件应优先复用 Avalonia 原生虚拟化、模板绑定和资源系统。
- 避免为每次状态变化创建不必要的视觉对象、订阅或动画对象。
- TitleBar 宿主发现只使用逻辑祖先和强类型 AvaloniaProperty binding，不使用全局 Window registry、反射或字符串 binding path；状态变化复用现有 lease，template reapply 不重复创建 lease。
- 首次显示主题表面使用一次性 Snapshot 读取，不为同步 `Show` 临界区创建资源 observable 或订阅。
- 大集合控件必须保证 container recycle 后不会泄漏旧 item 状态。

## 源码索引

主要源码文件：

- `src/AtomUI.Desktop.Controls/Window/Chrome/WindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/Chrome/AbstractLinuxWindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/Chrome/GenericLinuxWindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/Chrome/X11WindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/Chrome/WaylandWindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/Chrome/WindowsWindowChromeManager.cs`
- `src/AtomUI.Desktop.Controls/Window/MacStandardWindowButtons.cs`
- `src/AtomUI.Desktop.Controls/Window/MediaBreakPointThemeBootstrapper.cs`
- `src/AtomUI.Desktop.Controls/Window/ReactiveWindow.cs`
- `src/AtomUI.Desktop.Controls/Window/Themes/FullscreenPopoverLayerTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowDrawnDecorationsTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowResizerTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml`
- `src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.cs`
- `src/AtomUI.Desktop.Controls/Window/Utils/FullscreenPopoverLayer.cs`
- `src/AtomUI.Desktop.Controls/Window/Utils/WindowDrawnDecorationsReflectionExtensions.cs`
- `src/AtomUI.Desktop.Controls/Window/Utils/WindowResizer.cs`
- `src/AtomUI.Desktop.Controls/Window/Utils/WindowTitleBarShadowBackground.cs`
- `src/AtomUI.Desktop.Controls/Window/Utils/WindowVisualLayerClip.cs`
- `src/AtomUI.Desktop.Controls/Window/Window.cs`
- `src/AtomUI.Desktop.Controls/Window/WindowToken.cs`
- `src/AtomUI.Native/WindowExtensions.cs`
- `src/AtomUI.Native/Linux/WaylandWindowReflectionExtensions.cs`
- `src/AtomUI.Native/Linux/WaylandWindowUtils.cs`
- `src/AtomUI.Native/Linux/WindowUtils.Linux.cs`

职责边界：

- 控件主文件保留 public/protected API、Avalonia 属性注册、事件和主要生命周期入口。
- `Window/Chrome` 只承载平台 chrome manager：选择后端、订阅 Window/PlatformImpl 事件、合并 frame geometry 更新，并把 X11、Wayland、Windows 的原生能力投影为 Window 内部状态。
- `Window/Utils` 承载 Window 模板内部视觉 helper 和 Desktop drawn decorations 反射边界，例如 visible frame clip、managed resize grip、macOS 全屏 popover 与 `DynamicDependency` 标注；这些类型是 internal 协作对象，不是用户 API。
- `AtomUI.Native` 只执行已经确定后端之后的底层平台调用，例如 XCB input region、Xlib geometry、Wayland `wl_surface.set_input_region`。X11 shadow 输入区订阅策略和 resize band 仍属于 `X11WindowChromeManager`，不下沉到 Native。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/window/window/overview.md`
- 实现文档：`docs/controls/desktop/window/window/implementation.md`
- Token 文档：`docs/controls/desktop/window/window/token.md`
- 变更记录：`docs/controls/desktop/window/window/changelog.md`
- 语义结构：`./semantic-cn.md`
