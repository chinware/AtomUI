# WindowTitleBar 桌面版架构设计

`WindowTitleBar` 是 `AtomUI.Desktop.Controls` 中用于构成桌面窗口标题栏的模板化控件。本文档定义控件的设计定位、公共契约、状态模型、模板语义和集成边界。内部实现与跨平台标题布局见 [WindowTitleBar 实现原理](implementation.md)，视觉变量见 [WindowTitleBar Token 设计](token.md)，契约变化见 [WindowTitleBar Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | 未提供独立页面；以本目录源文档、控件源码和回归测试为准。 |
| 控件状态 | Stable |

`WindowTitleBar` 负责以下内容：

- 展示窗口 Logo、标题以及标题模板。
- 承载标题栏左右两侧的应用自定义内容。
- 为窗口拖动、双击最大化和系统 caption buttons 提供统一交互表面。
- 接收宿主窗口的平台、激活状态和窗口状态，并投影为稳定的模板状态。
- 在不同平台和窗口装饰模式下保持标题、原生按钮、managed buttons 与 add-on 互不覆盖。

`WindowTitleBar` 不是通用工具栏或导航栏。业务操作应放入 `LeftAddOn`、`RightAddOn` 或专用控件，并保留标题栏拖动区域和系统窗口操作的优先级。

## 2. 设计语言

标题栏由四类语义内容构成：

| 语义 | 内容 | 责任 |
| --- | --- | --- |
| Leading | `LeftAddOn` | 承载靠近起始侧的应用操作，并占用标题安全空间。 |
| Title | `Logo + Title` | 作为连续标题组测量、对齐和裁剪。 |
| Trailing | `RightAddOn + CaptionButtonGroup` | 承载结束侧应用操作和 managed window operations。 |
| Native chrome | 平台原生窗口按钮或 overlay | 不进入 visual tree，通过窗口边缘安全区参与布局。 |

三块 managed 区域与 native chrome 的完整几何关系由本文第 8 节和 [WindowTitleBar 实现原理](implementation.md) 定义。Logo 属于 Title，不属于 Leading；左右 add-on 属于操作区，不参与标题组中心计算。

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

## 3. API 与契约模型

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

## 4. 行为与状态模型

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

## 5. 视觉与主题模型

`WindowTitleBarTheme` 是 `WindowTitleBar` 的 ControlTheme 入口。内置主题保留以下稳定 template part 与语义节点：

| 节点 | 类型 | 契约 |
| --- | --- | --- |
| `Frame` | `Border` | 绘制标题栏背景并提供完整可见 frame 的布局边界。 |
| `PART_Logo` | `ContentPresenter` | 展示有效 Logo。 |
| `PART_ContentPresenter` | `ContentPresenter` | 展示标题；字符串标题在安全宽度不足时使用字符省略号，且不参与命中测试。 |
| `PART_LeftAddOn` | `ContentPresenter` | 展示 Leading 内容。 |
| `PART_RightAddOn` | `ContentPresenter` | 展示 Trailing add-on。 |
| `PART_CaptionButtonGroup` | `CaptionButtonGroup` | 运行时连接宿主窗口操作。 |

`PART_CaptionButtonGroup` 是 `WindowTitleBar` 代码查找并 attach/detach 的协作 part。其内部 `PART_CloseButton`、`PART_MinimizeButton`、`PART_MaximizeButton`、`PART_FullScreenButton` 和 `PART_PinButton` 属于 `CaptionButtonGroup` 模板，不是 `WindowTitleBar` 的 public template part。

平台主题可以改变 caption button 外观和 native chrome 来源，但不得改变 Public API 语义、Title/Leading/Trailing 角色或窗口操作行为。应用替换完整 ControlTheme 时负责提供等价区域、裁剪和命中测试；internal caption 类型不作为定制 API。

视觉尺寸、间距、active/inactive 颜色和 caption button 状态颜色由 [WindowTitleBar Token 设计](token.md) 管理。标题对齐值、CSD 状态和窗口状态不是 Token。

## 6. 控件家族或集成关系

### 6.1 Window

`Window` 创建默认 `WindowTitleBar`，并把 `Title`、`Logo`、`LogoTemplate`、`LogoVisibility` 和 `TitleAlignment` 单向投影给标题栏。`NotifyCreateTitleBar` 和 `NotifyConfigureTitleBar` 是派生窗口替换标题栏类型与补充配置的 protected 扩展点。

`Window` 负责窗口移动、最大化/还原、原生 chrome metrics、CSD 状态和标题栏高度提示。`WindowTitleBar` 负责内容布局，不直接调用平台窗口 API。标题栏必须横跨完整可见窗口 frame；原生窗口按钮安全区作为布局输入传递，不能通过给整个标题栏添加单侧 Padding 或 Margin 来改变窗口中心。

### 6.2 ImagePreviewer

`ImagePreviewerTitleBar` 是 internal 派生标题栏。它使用显式预览图标替代 Logo，并把图片工具栏放入 Leading。预览图标与标题仍组成一个连续 Title 组，caption buttons 和右侧 add-on 仍属于 Trailing。

### 6.3 全屏标题宿主

`WindowDrawnDecorations` 和 `FullscreenPopoverLayer` 中的全屏标题宿主复用标题栏 Token、窗口操作语义和标题对齐模型。全屏宿主只计算当前实际可见的 native/managed 区域，不继承普通窗口状态下已经消失的原生按钮占位。

## 7. 兼容性不变量

- Public API 的类型、默认值、绑定语义和事件时序保持稳定。
- `Auto` Logo 规则和标题对齐的显式枚举语义保持稳定。
- `PART_CaptionButtonGroup`、内容 presenter 名称、ControlTheme key 和伪类保持稳定。
- Title 内容不参与命中测试；add-on 和 caption buttons 保持可交互。
- Logo 和 Title 始终作为连续 Title 组；add-on 不进入标题中心计算。
- CSD 开关只改变 chrome metrics 来源和可见操作区，不改变显式标题对齐含义。
- template reapply、逻辑树 detach 和窗口替换时释放旧订阅与 part handler。
- 平台选择和 Token 发现不依赖运行时反射或程序集扫描。

## 8. 专项模型

标题对齐使用 Leading、Title、Trailing 与 Native chrome 四类区域。`Auto` 只解析平台默认值；`Left`、`Center`、`WindowCenter` 和 `Right` 在所有平台保持同一几何语义。标题安全宽度由 managed operation、Padding 与 native chrome inset 共同约束，平台输入 owner、坐标系和共享公式见 [WindowTitleBar 实现原理](implementation.md)。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [WindowTitleBar 实现原理](implementation.md)
- [WindowTitleBar Token 设计](token.md)
- [WindowTitleBar Changelog](changelog.md)
- [Window 控件设计](../window/overview.md)

稳定行为由以下测试覆盖：

- `WindowTitleBarLogoVisibilityTests`：Logo 默认值、平台规则、全屏规则和 Window 投影。
- `WindowTitleBarLayoutPanelTests`：共享几何、条件间距、margin 单次计入、左右 add-on 动态内容、窄窗口和非法 metrics。
- `WindowTitleBarLayoutStrategyTests`：平台 `Auto`、CSD、WindowState 与 native inset 归一。
- `WindowTitleBarTokenTests`：Token 默认值、三平台 caption 视觉和 Windows edge layout。
- `ImagePreviewerTitleBarThemeTests`：派生标题栏的标题组、操作区和平台模板契约。

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `WindowTitleBar` | 承载公共内容契约、平台状态和标题栏主题入口。 | `Logo`、`Title`、`TitleAlignment` | `Height`、`TitleBarPadding`、标题字体与颜色 | public |
| `frame` | `Border#Frame` | 绘制标题栏背景并定义完整可见 frame。 | `Background`、`Padding` | `Height`、`TitleBarPadding` | template-stable |
| `leading` | `ContentPresenter#PART_LeftAddOn` | 承载起始侧应用操作并占用标题安全空间。 | `LeftAddOn`、`LeftAddOnTemplate` | `HeaderHorizontalSpacing` | template-stable |
| `title` | `PART_Logo` + `PART_ContentPresenter` | 将 Logo 与 Title 作为连续标题组展示、测量和裁剪。 | `Logo`、`LogoTemplate`、`LogoVisibility`、`Title`、`TitleTemplate` | `LogoSize`、`LogoAndTitleSpacing`、标题字体与颜色 | template-stable |
| `trailing` | `PART_RightAddOn` + `PART_CaptionButtonGroup` | 承载结束侧应用操作和 managed window operations。 | `RightAddOn`、`RightAddOnTemplate`；Window caption 配置 | `HeaderHorizontalSpacing`、caption button 尺寸、间距与状态颜色 | template-stable |
| `native-chrome` | 平台原生窗口按钮安全区 | 以逻辑像素 inset 约束标题安全空间，不进入 visual tree。 | 平台、CSD、WindowState | 不适用 | internal-observable |

LLMS 生成使用以下来源，不手工修改 `docs/AI/llms` 产物。

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + 源码 public surface + `token.md` | 生成 `controls/window-title-bar/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + Themes | 生成 `controls/window-title-bar/semantic-cn.md` |
| API 表 | 源码 public surface | 不在 `overview.md` 机械复制完整表 |
| Design Token 表 | `WindowTitleBarToken.cs` + `token.md` | 不手工维护第二份生成表 |
| 示例 | 无独立 Gallery 稳定示例 | 不把临时验收 Demo 纳入生成来源 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档 | 运行 LLMS `verify`、`git diff --check` 并检查相对链接。 |
| Public API 与状态 | 覆盖属性默认值、Logo 规则、窗口状态、事件时序和 Window 投影。 |
| Theme 与 Template | 检查三平台结构、稳定 part、命中测试、ImagePreviewer 与全屏宿主。 |
| 标题布局 | 覆盖四种显式对齐、平台 `Auto`、CSD/native inset、窄窗口退化和动态可见性。 |
| Token | 检查 Token 默认值、generated resource key、Light/Dark 和 active/inactive 状态。 |
