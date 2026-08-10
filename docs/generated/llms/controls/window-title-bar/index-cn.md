# WindowTitleBar

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

`WindowTitleBar` 负责以下内容：

- 展示窗口 Logo、标题以及标题模板。
- 承载标题栏左右两侧的应用自定义内容。
- 为窗口拖动、双击最大化和系统 caption buttons 提供统一交互表面。
- 接收宿主窗口的平台、激活状态和窗口状态，并投影为稳定的模板状态。
- 在不同平台和窗口装饰模式下保持标题、原生按钮、managed buttons 与 add-on 互不覆盖。

`WindowTitleBar` 不是通用工具栏或导航栏。业务操作应放入 `LeftAddOn`、`RightAddOn` 或专用控件，并保留标题栏拖动区域和系统窗口操作的优先级。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `未提供独立页面；以本目录源文档、控件源码和回归测试为准。` |
| 状态 | Stable |

## 何时使用

标题栏由四类语义内容构成：

| 语义 | 内容 | 责任 |
| --- | --- | --- |
| Leading | Windows/Linux: `Logo + LeftAddOn`；macOS: `LeftAddOn` | 承载靠近起始侧的应用操作，并占用标题安全空间。Windows/Linux 中可见 Logo 位于物理最左侧，先于 `LeftAddOn`。 |
| Title | Windows/Linux: `Title`；macOS: `Logo + Title` | 承载标题内容并执行对齐和裁剪。macOS 保留连续 Logo/Title 标题组以配合原生窗口按钮安全区。 |
| Trailing | `RightAddOn + CaptionButtonGroup` | 承载结束侧应用操作和 managed window operations。 |
| Native chrome | 平台原生窗口按钮或 overlay | 不进入 visual tree，通过窗口边缘安全区参与布局。 |

三块 managed 区域与 native chrome 的完整几何关系由本文第 8 节和 [WindowTitleBar 实现原理](implementation.md) 定义。Windows/Linux 的 Logo 属于 Leading，占用左侧操作安全空间；macOS 的 Logo 属于 Title，以保持平台标题行为。左右 add-on 属于操作区，不参与标题中心计算。

控件家族的职责边界：

| 类型 | 可见性 | 职责 |
| --- | --- | --- |
| `WindowTitleBar` | public | 公共内容契约、宿主状态投影、交互入口和主题入口。 |
| `WindowTitleBarLogoVisibility` | public | 定义 Logo 的显示策略。 |
| `WindowTitleBarTitleAlignment` | public | 定义标题组的跨平台对齐语义。 |
| `CaptionButtonGroup` | internal | 把窗口能力和状态映射为关闭、最小化、最大化、全屏和置顶按钮。 |
| `CaptionButton` | internal | 承载 caption icon、checked icon 和按钮视觉状态。 |
| `WindowsCaptionButton` | internal | 提供 Windows 方形 caption button 尺寸和 hover 状态修正。 |

internal 类型服务于 AtomUI 内置主题，不属于应用可直接创建或继承的公共控件 API。

## 公共 API

### 3.1 标题内容

| API | 默认值 | 语义 |
| --- | --- | --- |
| `Logo` | `null` | Logo 内容对象。 |
| `LogoTemplate` | `null` | Logo 数据模板。 |
| `LogoVisibility` | `Auto` | Logo 显示策略。 |
| `Title` | `null` | 标题内容对象。 |
| `TitleTemplate` | `null` | 标题数据模板。 |
| `TitleAlignment` | `Auto` | 标题组对齐策略。 |

`Logo` 与 `Title` 都接受任意对象。字符串标题为空或仅包含空白时按“无标题内容”处理；非字符串对象按存在标题内容处理。

### 3.2 Add-on

| API | 默认值 | 语义 |
| --- | --- | --- |
| `LeftAddOn` | `null` | Leading 区域内容。 |
| `LeftAddOnTemplate` | `null` | Leading 内容模板。 |
| `RightAddOn` | `null` | Trailing 区域中位于 caption buttons 之前的内容。 |
| `RightAddOnTemplate` | `null` | Trailing 内容模板。 |

Add-on 可以包含可交互控件，也可以是 `null`、隐藏节点或当前没有孩子的容器。内置模板必须保留其命中测试能力，同时避免 Title 覆盖这些区域。Leading 或 Trailing 的实测宽度为零时，该区域不占用标题安全空间，也不产生 `HeaderHorizontalSpacing`；内容出现、隐藏或动态替换后由正常 measure invalidation 重新计算。

### 3.3 宿主状态

| API | 语义 |
| --- | --- |
| `IsWindowActive` | 表示宿主窗口是否激活，并驱动标题和 caption buttons 的 active/inactive 视觉。 |
| `IsMotionEnabled` | 控制标题栏及 caption button 的状态过渡。 |
| `OsType` | 只读平台标识，用于选择平台主题和布局 Strategy。 |
| `OsVersion` | 只读平台版本，用于受版本约束的平台能力判断。 |

`OsType` 和 `OsVersion` 由 AtomUI 平台感知基础设施写入。应用不通过 CLR setter 修改这两个值。

### 3.4 交互事件

`MaximizeWindowRequested` 表示标题栏收到有效的主按钮双击请求。`WindowTitleBar` 不直接修改 `WindowState`；宿主 `Window` 根据 `CanResize`、`CanMaximize` 和当前状态决定最大化或还原。

事件在对应的 `PointerReleased` 阶段发出，避免窗口同步 resize 破坏当前 pointer capture。pointer capture 丢失或释放条件不匹配时，请求被取消。

## 事件与命令

### 3.4 交互事件
事件在对应的 `PointerReleased` 阶段发出，避免窗口同步 resize 破坏当前 pointer capture。pointer capture 丢失或释放条件不匹配时，请求被取消。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

Gallery 目录 `未提供独立页面；以本目录源文档、控件源码和回归测试为准。` 当前不存在；请检查控件文档中的 Gallery 页面元数据。

## 状态模型

### 4.1 Logo 显示模型

`LogoVisibility` 的三个值具有以下语义：

| 值 | 规则 |
| --- | --- |
| `Always` | 存在 `Logo` 或 `LogoTemplate` 时显示。 |
| `Never` | 始终隐藏。 |
| `Auto` | 根据标题内容、平台和窗口状态计算。 |

`Auto` 的计算矩阵：

| 条件 | 结果 |
| --- | --- |
| 不存在 Logo 内容和 Logo 模板 | 隐藏。 |
| 存在有效标题内容 | 显示。 |
| 无标题内容且平台为 macOS | 隐藏。 |
| 无标题内容、平台不是 macOS、窗口非全屏 | 显示。 |
| 无标题内容、平台不是 macOS、窗口全屏 | 隐藏。 |

该模型只控制 Logo 的有效可见性，不修改 `Logo`、`Window.Icon` 或应用图标来源。

### 4.2 窗口状态

`WindowTitleBar` 从逻辑祖先 `Window` 接收状态并维护以下伪类：

| 伪类 | 条件 |
| --- | --- |
| `:active` | 宿主窗口处于激活状态。 |
| `:normal` | `WindowState.Normal`。 |
| `:minimized` | `WindowState.Minimized`。 |
| `:maximized` | `WindowState.Maximized`。 |
| `:fullscreen` | `WindowState.FullScreen`。 |

窗口激活状态同时写入 `IsWindowActive`，供模板中的内部协作控件使用。状态 owner 始终是宿主 `Window`；模板节点不反向维护第二份窗口状态。

### 4.3 Caption buttons

caption button 的公共配置属于宿主 `Window`：

- `CanMinimize` 和 `CanMaximize` 决定最小化、最大化按钮能力。
- `IsFullScreenCaptionButtonVisible`、`IsPinCaptionButtonVisible` 和 `IsCloseCaptionButtonVisible` 决定扩展按钮可见性。
- `Topmost`、`WindowState` 和平台 backend 决定 checked state 与有效可见性。

全屏时隐藏最小化和最大化按钮；最大化时隐藏进入全屏按钮。Wayland backend 不提供置顶按钮。按钮点击最终写入宿主窗口状态或调用关闭流程，状态不保存在按钮视觉中。

### 4.4 拖动和双击

`Window` 监听标题栏 pointer 事件并在移动距离超过拖动阈值后调用原生 `BeginMoveDrag`。`IsMoveEnabled=False` 或全屏状态禁止拖动。双击最大化与拖动共用标题栏输入表面，但 caption buttons 和 add-on 的已处理输入不应触发窗口拖动。

## 主题与 Design Token

`WindowTitleBarTheme` 是 `WindowTitleBar` 的 ControlTheme 入口。内置主题保留以下稳定 template part 与语义节点：

| 节点 | 类型 | 契约 |
| --- | --- | --- |
| `Frame` | `Border` | 绘制标题栏背景并提供完整可见 frame 的布局边界。 |
| `PART_Logo` | `ContentPresenter` | 展示有效 Logo；Windows/Linux 模板中位于 Leading 最左侧，macOS 模板中位于 Title 内容前。 |
| `PART_ContentPresenter` | `ContentPresenter` | 展示标题；字符串标题在安全宽度不足时使用字符省略号，且不参与命中测试。 |
| `PART_LeftAddOn` | `ContentPresenter` | 展示 Leading 内容。 |
| `PART_RightAddOn` | `ContentPresenter` | 展示 Trailing add-on。 |
| `PART_CaptionButtonGroup` | `CaptionButtonGroup` | 运行时连接宿主窗口操作。 |

`PART_CaptionButtonGroup` 是 `WindowTitleBar` 代码查找并 attach/detach 的协作 part。其内部 `PART_CloseButton`、`PART_MinimizeButton`、`PART_MaximizeButton`、`PART_FullScreenButton` 和 `PART_PinButton` 属于 `CaptionButtonGroup` 模板，不是 `WindowTitleBar` 的 public template part。

平台主题可以改变 caption button 外观和 native chrome 来源，但不得改变 Public API 语义、Title/Leading/Trailing 角色或窗口操作行为。应用替换完整 ControlTheme 时负责提供等价区域、裁剪和命中测试；internal caption 类型不作为定制 API。

视觉尺寸、间距、active/inactive 颜色和 caption button 状态颜色由 [WindowTitleBar Token 设计](token.md) 管理。标题对齐值、CSD 状态和窗口状态不是 Token。

Token 来源：

`WindowTitleBarToken` 是 scope id 为 `WindowTitleBar` 的 internal control token，源码位于 `src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs`。它从 `SharedToken` 计算标题栏和 caption button 的视觉变量，并通过生成的 `WindowTitleBarTokenResource` key 供 AXAML 使用。

Token 负责尺寸、间距、字体和状态颜色，不负责以下运行时语义：

- `TitleAlignment`、Leading/Title/Trailing 角色和布局公式。
- CSD、native chrome insets、WindowState 和 backend 能力。
- Logo、标题、add-on 或 caption button 的有效可见性。
- pointer capture、拖动、checked state 和窗口操作。

## AOT 与裁剪注意事项

- 窗口订阅和 relay binding 都有明确的 attach/detach 或 apply/reapply 配对。
- 标题布局 Strategy 使用静态无状态实例；measure/arrange 不创建 Context、Plan、binding 或临时 Visual。
- TemplateBinding 和 selector 承担静态视觉投影，不在状态变化时重建模板节点。
- Logo 计算只在相关属性或 WindowState 变化时执行。
- native chrome metrics 缓存属于 Window/platform manager，不能复制到 Panel 或 Strategy。
- 平台 Strategy 使用封闭 `OsType` switch，不使用反射、程序集扫描、字符串类型发现或运行时 DI。
- Token 通过生成的静态资源入口消费；不反射枚举 public API 或 Token 属性。

## 源码索引

```text
src/AtomUI.Desktop.Controls/
├── Window/
│   ├── Window.cs
│   ├── MacStandardWindowButtons.cs
│   ├── Chrome/
│   │   ├── WindowChromeManager.cs
│   │   ├── AbstractLinuxWindowChromeManager.cs
│   │   ├── GenericLinuxWindowChromeManager.cs
│   │   ├── X11WindowChromeManager.cs
│   │   ├── WaylandWindowChromeManager.cs
│   │   └── WindowsWindowChromeManager.cs
│   ├── Utils/
│   │   ├── FullscreenPopoverLayer.cs
│   │   └── WindowVisualLayerClip.cs
│   └── Themes/
│       ├── WindowTheme.axaml
│       ├── WindowDrawnDecorationsTheme.axaml
│       └── FullscreenPopoverLayerTheme.axaml
├── WindowTitleBar/
│   ├── WindowTitleBar.cs
│   ├── WindowTitleBarLogoVisibility.cs
│   ├── WindowTitleBarTitleAlignment.cs
│   ├── WindowTitleBarLayoutPanel.cs
│   ├── CaptionButtonGroup.cs
│   ├── CaptionButton.cs
│   ├── WindowsCaptionButton.cs
│   ├── WindowTitleBarToken.cs
│   ├── Strategies/
│   │   ├── IWindowTitleBarLayoutStrategy.cs
│   │   ├── MacOSWindowTitleBarLayoutStrategy.cs
│   │   ├── WindowsWindowTitleBarLayoutStrategy.cs
│   │   └── LinuxWindowTitleBarLayoutStrategy.cs
│   └── Themes/
│       ├── WindowTitleBarTheme.axaml
│       ├── CaptionButtonGroupTheme.axaml
│       ├── CaptionButtonTheme.axaml
│       └── WindowsCaptionButtonTheme.axaml
└── ImagePreviewer/
    ├── ImagePreviewerTitleBar.cs
    └── Themes/ImagePreviewerTitleBarTheme.axaml
```

标题布局输入和中间结果使用标量、`Thickness`、`Rect` 与命名 tuple 传递，不建立独立 Context、Plan 或布局结果类型。

## 相关文档

- 源设计文档：`docs/controls/desktop/window/window-title-bar/overview.md`
- 实现文档：`docs/controls/desktop/window/window-title-bar/implementation.md`
- Token 文档：`docs/controls/desktop/window/window-title-bar/token.md`
- 变更记录：`docs/controls/desktop/window/window-title-bar/changelog.md`
- 语义结构：`./semantic-cn.md`
