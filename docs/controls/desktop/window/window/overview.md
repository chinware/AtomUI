# Window 桌面版架构设计

本文档定义 `Window` 桌面版的最新设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Window 桌面版实现原理](implementation.md)，Window Token 的专项设计见 [Window Token 设计](token.md)，设计和契约变化记录见 [Window Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | 未独立 Gallery 页面；以 `docs/controls/desktop/window/window` 源文档为准 |
| 控件状态 | Stable |

Window 是 AtomUI 桌面控件体系中的桌面窗口控件，用于提供 AtomUI 自绘窗口、平台窗口能力和主题集成入口。

Window 不负责普通内容控件、Dialog 弹层或业务路由容器。这些职责应由业务层、组合控件或更专用的 AtomUI 控件承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls/Window`

## 2. 设计语言

Window 的设计语言围绕控件职责、可观察状态和主题契约组织，而不是围绕模板节点组织。

| 维度 | 含义 | Window 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | Window 是 AtomUI 桌面控件体系中的桌面窗口控件，用于提供 AtomUI 自绘窗口、平台窗口能力和主题集成入口。 |
| 内容承载 | 用户数据、展示内容、集合项或操作入口如何进入控件。 | `ContentFrameBackground`、`ContentFrameLayer`、`ContentFrameLayerOpacity`、`ContentFrameLayerTemplate`、`IsTitleBarVisible`、`LogoTemplate`、`TitleBarFrameBackground`、`TitleBarFrameLayer` 等 11 项。`TitleBarFrameLayer` 表示标题栏背景或装饰层，不作为按钮、菜单、搜索框等交互控件入口。 |
| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Window Token + ControlTheme。 |

## 3. API 与契约模型

Window 的公共契约由 public/protected 类型成员、Avalonia 属性、事件、命令、template part、伪类、ControlTheme key 和资源 key 共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ContentFrameBackground`、`ContentFrameLayer`、`ContentFrameLayerOpacity`、`ContentFrameLayerTemplate`、`IsTitleBarVisible`、`LogoTemplate`、`TitleBarFrameBackground`、`TitleBarFrameLayer`、`TitleBarFrameLayerOpacity`、`TitleBarFrameLayerTemplate` 等 11 项 | 定义控件展示内容、输入数据、模板或业务对象入口；其中 `TitleBarFrameLayer` 是标题栏背景或装饰层，交互按钮、菜单、搜索框应通过自定义 `TitleBar` 承载。 |
| 选择与集合 | `ViewModel` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsCloseCaptionButtonVisible`、`IsFullScreenCaptionButtonVisible`、`IsMoveEnabled`、`IsPinCaptionButtonVisible` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 弹层与窗口 | `WindowFrameLayer`、`WindowFrameLayerOpacity` | 控制 popup、flyout、dialog、window 或 overlay 宿主协作。 |
| 其他稳定入口 | `Logo`、`LogoVisibility`、`MediaBreakPoint`、`OsType`、`OsVersion` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

当前没有抽取到控件专属 public 事件；交互通知主要来自继承事件、命令或 Gallery 可观察状态。

主要公开类型与枚举：

- 类型：`FullscreenPopoverLayer`、`MacStandardWindowButtons`、`ReactiveWindow`、`Window`、`WindowResizer`。
- 枚举：无。

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

## 4. 行为与状态模型

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
- 标题栏交互内容应由 `TitleBar` / `WindowTitleBar` 承载；`TitleBarFrameLayer` 只表达标题栏背景、遮罩或装饰视觉，不保证内部控件获得 pointer、focus、keyboard 或 command 事件。

## 5. 视觉与主题模型

Window 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `FullscreenPopoverLayerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `WindowDrawnDecorationsTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `WindowResizerTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `WindowTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `WindowThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |

Window 使用 `WindowToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 open/close 运行时状态。

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
| 默认标题栏层 | `TitleBar` / `WindowTitleBar` | 展示标题、Logo、caption buttons，并在空白区域提供窗口拖拽语义。 | 只处理标题栏默认交互和窗口操作。 |
| 自定义标题栏层 | `TitleBar` | 承载用户自定义标题栏布局、按钮、菜单、搜索框或其他交互控件。 | 用户控件按普通 Avalonia client input 语义命中；空白区域由自定义标题栏自行决定是否保留拖拽。 |

维护标题栏模板时，不应把 `TitleBarFrameLayer` 提升为可交互覆盖层。需要在标题栏放置按钮、菜单或搜索框时，应创建自定义 `WindowTitleBar` 或其他标题栏控件，并设置到 `Window.TitleBar`。

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

## 6. 控件家族或集成关系

Window 与同分类控件共享尺寸、状态、Token、Gallery 展示和验证规则。组合或派生控件应显式说明哪些 API 被继承、覆盖或不支持。

主要协作类型：

- `FullscreenPopoverLayer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `MacStandardWindowButtons`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `ReactiveWindow`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `Window`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `WindowResizer`：控件核心或内部协作类型，维护 public surface 与主题可观察行为。
- `WindowTheme`：ControlTheme 类型入口，连接主题资源和控件类型。
- `WindowToken`：组件 Token scope，负责从全局 token 派生控件语义变量。

集成关系：

- 与 ThemeManager、SharedToken、ControlTheme 和 Gallery ShowCase 的示例/API/Token 表保持一致。
- 涉及 ItemsSource、Popup、Flyout、Window、Form 或 CompactSpace 的路径必须保持生命周期释放和数据状态同步。
- 源码目录中的共享基类和内部协作类型形成维护边界，不能只修改桌面包装类而忽略共享状态 owner。

## 7. 兼容性不变量

维护 Window 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- `TitleBarFrameLayer` 是标题栏背景/装饰入口，不是标题栏用户交互入口；标题栏按钮、菜单、搜索框等交互内容必须通过 `TitleBar` 承载。
- Windows、macOS 和 Linux 共用同一套首次显示主题表面流程；平台可见前必须同步准备 ThemeContext、variant 和 Window Token 背景，正式显示后由 `WindowTheme` 单独持有长期主题状态。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 选择与当前项模型

Window 的当前项状态必须由单一 owner 推导。public 选择属性、集合项容器和伪类之间只能做单向同步，集合替换、清空和模板重套用时必须回放当前状态。

### 8.2 弹层与宿主模型

Window 涉及弹层、窗口或 overlay 宿主时，打开状态、取消事件、定位和宿主释放必须保持一致。重复打开、关闭、窗口失活和 template reapply 都必须释放旧宿主引用。

Window 对上层 overlay 发布三种不可混用的几何语义：

- 完整 layer bounds：保持 TopLevel client surface 坐标系，供 mask 和 layer 自身使用。
- visible frame bounds：完整 layer 只排除 `FrameShadowThickness`，仍包含 managed/drawn 标题栏，供 Dialog Surface、Drawer 和需要贴合可见窗口边缘的 overlay 使用。
- content bounds：在 visible frame 基础上排除标题栏和内容装饰，供普通 Window 内容或明确要求正文安全区的反馈使用。

`WindowVisualLayerClip` 是 visible frame 外轮廓计算真源。Window 的平台 manager 只负责把 X11、Wayland、Win32 和 macOS 原生能力投影为 `FrameShadowThickness`、CSD 状态和 drawn host，不允许上层控件再次按 OS 复制几何算法。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Window 桌面版实现原理](implementation.md)
- [Window Token 设计](token.md)
- [Window Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Window` | 窗口控件根语义区域，承载窗口 public API、平台状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `chrome` | `窗口装饰区域` | 承载标题栏、caption buttons、drag region、边框和阴影。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `窗口内容区域` | 承载业务内容、系统交互和布局边界。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `state` | `窗口状态区域` | 表达最大化、最小化、激活、失焦、resize 和平台能力。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/window/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/window/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖属性默认值、事件触发、命令和继承语义。 |
| 状态模型 | 覆盖 open/close、disabled、hover、pressed、focus 以及控件特有状态。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和 Browser 主题。 |
| 首次显示主题表面 | 覆盖根窗口和 owner 局部主题、Light/Dark、用户显式背景优先级、显示失败回滚，并在 Windows、macOS 和 Linux 实机检查首帧。 |
| Token | 检查 TokenKind、AXAML token resource、Gallery Token 表和文档同步。 |
| Gallery | 走查对应 ShowCase 示例、API 表和 Token 表入口。 |
