# Splash 桌面版实现原理

本文档描述 Splash 桌面版的内部实现范围、源码职责、状态流、生命周期、服务编排和维护规则。公共设计与 API 契约见 [Splash 桌面版架构设计](overview.md)，变化记录见 [Splash Changelog](changelog.md)。涉及组件 Token 的实现应同时阅读 [Splash Token 设计](token.md)。

## 1. 实现定位

本文档覆盖 `AtomUI.Desktop.Controls.Extras` 中 Splash 视觉控件、启动窗口、实例服务、静态 API、主题接入和 Gallery 可见维护边界。具体属性注册、默认值、动效细节和 AXAML selector 仍应直接阅读源码；本文只记录维护者必须理解的稳定结构和不变量。

## 2. 源码文件结构

主要源码文件：

- `src/AtomUI.Desktop.Controls.Extras/Splash/Splash.cs`
- `src/AtomUI.Desktop.Controls.Extras/Splash/Splash.StaticAPI.cs`
- `src/AtomUI.Desktop.Controls.Extras/Splash/SplashWindow.cs`
- `src/AtomUI.Desktop.Controls.Extras/Splash/SplashOptions.cs`
- `src/AtomUI.Desktop.Controls.Extras/Splash/SplashService.cs`
- `src/AtomUI.Desktop.Controls.Extras/Splash/ISplashService.cs`
- `src/AtomUI.Desktop.Controls.Extras/Splash/SplashStatus.cs`
- `src/AtomUI.Desktop.Controls.Extras/Splash/SplashToken.cs`
- `src/AtomUI.Desktop.Controls.Extras/Splash/SplashPseudoClass.cs`
- `src/AtomUI.Desktop.Controls.Extras/Splash/Themes/SplashTheme.axaml`
- `src/AtomUI.Desktop.Controls.Extras/Splash/Themes/SplashWindowTheme.axaml`
- `src/AtomUI.Desktop.Controls.Extras/Splash/Themes/SplashThemes.axaml`
- `src/AtomUI.Desktop.Controls.Extras/AtomUIExtrasThemesProvider.axaml`
- `src/AtomUI.Desktop.Controls.Extras/AtomUIExtrasThemesProvider.cs`
- `src/AtomUI.Desktop.Controls.Extras/ThemeManagerBuilderExtensions.cs`

职责边界：

- `Splash.cs` 保留视觉控件 public/protected API、状态写入方法、Avalonia 属性注册、伪类同步和主要模板生命周期入口。
- `Splash.StaticAPI.cs` 只放静态便利入口，所有逻辑委托给 `Splash.DefaultService`。
- `SplashWindow.cs` 负责窗口级 `Splash` 内容承载属性、展示时间记录、淡出关闭和关闭请求状态；`SplashWindowTheme.axaml` 负责透明无装饰窗口默认值、窗口模板、阴影宿主和内容承载边界。
- `SplashService.cs` 是启动编排 owner，负责创建窗口、创建或复用 `Splash` 实例、应用运行时 options、更新状态、关闭窗口和 UI thread 调度。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 3. 核心类职责

- `Splash`：视觉控件，承载品牌、文本、进度、状态、扩展内容、状态写入方法和静态 API 委托入口。
- `SplashWindow`：桌面窗口宿主，通过可空 `Splash` 属性承载内容控件，管理展示、最短展示时间、关闭延迟和淡出。
- `ISplashService`：实例服务契约，适合应用启动代码、测试和依赖注入场景。
- `SplashService`：默认服务实现，一次管理一个 `CurrentWindow`，通过 `CurrentWindow.Splash` 写入状态。
- `SplashOptions`：启动窗口、初始内容和关闭节奏配置对象。
- `SplashStatus`：视觉状态枚举，不表达业务启动结果对象。
- `SplashToken`：组件 Token scope，负责从全局 token 派生控件语义变量。
- `AtomUIExtrasThemesProvider`：Extras 包主题聚合入口。

核心协作规则：

- 视觉状态 owner 是 `Splash`，窗口生命周期 owner 是 `SplashWindow`，`Splash` 实例创建和编排状态 owner 是 `SplashService`。
- `Splash.DefaultService` 是静态 API 的唯一默认服务入口，静态方法不得绕过它操作窗口。
- `Splash` 状态写入方法不持有业务异常、主窗口或应用 lifetime。
- `SplashWindow` 不创建主窗口，不调用应用退出，不决定错误处理策略。
- `SplashWindow` 不在构造函数中创建窗口表面宿主或设置窗口壳层视觉默认值；这些结构由 `SplashWindowTheme.axaml` 表达。

## 4. 状态与数据流

Splash 的状态流遵循下面路径：

```text
Application startup code
  -> ISplashService / Splash static API
  -> SplashService
  -> SplashService creates or reuses Splash
  -> SplashWindow.Splash
  -> Splash properties
  -> pseudo-class / template binding
  -> SplashTheme visual nodes
```

源码中的状态入口按以下语义维护：

- 品牌与标题：`Logo`、`LogoTemplate`、`Title`、`Subtitle`。
- 状态文本：`Message`、`Detail`。
- 进度状态：`Progress`、`IsIndeterminate`、`Status`.
- 内容扩展：`Content`、`ContentTemplate`、`Footer`、`FooterTemplate`。
- 动效与窗口：`IsMotionEnabled`、`MinimumShowDuration`、`CloseDelay`、`FadeOutDuration`、`Topmost`。
- 服务入口：`ISplashService`、`SplashService`、`Splash.DefaultService`、静态方法。

维护要求：

- 外部设置的 Avalonia 属性必须在模板应用前后保持一致。
- `Status` 变化只同步伪类和模板绑定，不触发业务动作。
- `Progress` 值应归一到 `0..1` 区间；无进度使用 `null`。
- `IsIndeterminate` 优先于 `Progress` 的视觉展示。
- `SetError()` 不关闭窗口，调用方负责决定退出、重试或继续。
- overview.md 的 API 契约说明应与源码实际状态流一致。

### 组合结构模型

### 控件角色图

```text
SplashWindow
  -> SplashWindowTheme
     -> ShadowsAwareContainer#PART_SurfaceHost (template-stable)
        -> Splash
           -> Border#PART_RootLayout (template-stable)
              -> Border#PART_SurfaceLayout (template-stable)
                 -> ContentPresenter#PART_LogoPresenter (template-stable)
                 -> TextBlock#PART_TitleBlock (template-stable)
                 -> TextBlock#PART_SubtitleBlock (template-stable)
                 -> ContentPresenter#PART_ContentPresenter (template-stable)
                 -> Spin#PART_Spin (template-stable)
                 -> ProgressBar#PART_ProgressBar (template-stable)
                 -> TextBlock#PART_MessageBlock (template-stable)
                 -> TextBlock#PART_DetailBlock (template-stable)
                 -> ContentPresenter#PART_FooterPresenter (template-stable)

Splash static API
  -> ISplashService DefaultService (public)
     -> SplashService (public)
        -> SplashWindow (public)
           -> Splash (public)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Splash` | public control | `Splash.cs` / `SplashTheme.axaml` | 用户或 `SplashService` | 视觉 API、静态 API | public | 可作为用户 XAML 控件和静态入口。 |
| `SplashWindow` | public window | `SplashWindow.cs` / `SplashWindowTheme.axaml` | 用户或 `SplashService` | 窗口展示和关闭节奏 | public | 可直接使用，但不接管主窗口。 |
| `ISplashService` | public service contract | `ISplashService.cs` | 调用方 | show/update/close 编排 | public | 推荐测试和 DI 使用。 |
| `SplashService` | public service implementation | `SplashService.cs` | 调用方或 `Splash.DefaultService` | 默认服务行为 | public | 一次只管理一个启动流。 |
| `PART_SurfaceHost` | template part | `SplashWindowTheme.axaml` | `SplashWindow` template | 窗口表面阴影和阴影遮罩圆角 | template-stable | 使用 `ShadowsAwareContainer`，不通过 C# token bridge 绑定资源。 |
| `PART_RootLayout` | template part | `SplashTheme.axaml` | `Splash` template | Splash 内容裁剪边界 | template-stable | 主题维护可依赖名称，用户不直接操作。 |
| `PART_SurfaceLayout` | template part | `SplashTheme.axaml` | `Splash` template | 背景、内容 padding 和表面圆角裁剪 | template-stable | 与 `PART_SurfaceHost` 共用表面圆角 token。 |
| `PART_ProgressBar` | template part | `SplashTheme.axaml` | `Splash` template | `Progress` | template-stable | 表达确定进度，不承载业务任务。 |
| `PART_Spin` | template part | `SplashTheme.axaml` | `Splash` template | `IsIndeterminate` | template-stable | 表达不确定加载，不替代 Spin 控件文档。 |

## 5. 生命周期与模板接入

生命周期规则：

- `Splash` 构造阶段只注册 token scope 和默认状态，不依赖 template part。
- `Splash.OnApplyTemplate` 只获取稳定 template part 并回放伪类状态；可由 AXAML 表达的绑定留在主题中。
- `SplashWindow` 构造阶段不创建 `Splash`，不应用 `SplashOptions`，不设置透明窗口、任务栏、尺寸策略、内容宿主或阴影宿主。
- `SplashWindow.Splash` 是可空 `StyledProperty<Splash?>`，默认值为 `null`；窗口模板只通过 `TemplateBinding` 承载该属性。
- `SplashWindow.MinimumShowDuration`、`SplashWindow.CloseDelay`、`SplashWindow.FadeOutDuration` 是 `StyledProperty<TimeSpan>`，用于窗口关闭调度；`SplashWindow.IsCloseRequested` 是只读运行时 `DirectProperty`。
- `SplashWindowTheme.axaml` 负责 `Background`、`TransparencyBackgroundFallback`、`TransparencyLevelHint`、`ExtendClientAreaToDecorationsHint`、`WindowDecorations`、`ShowInTaskbar`、`CanResize`、`SizeToContent`、`WindowStartupLocation`、`Topmost` 和 `PART_SurfaceHost`。
- `SplashWindowTheme.axaml` 必须保持 `ExtendClientAreaToDecorationsHint=False`、`WindowDecorations=None`、`CanResize=False` 和 `WindowStartupLocation=CenterScreen`；SplashWindow 不提供标题栏或客户区拖动，默认显示在屏幕正中央。
- `SplashWindowTheme.axaml` 中的 `PART_SurfaceHost` 直接使用 `{atom:SplashTokenResource SurfaceBoxShadow}` 和 `{atom:SplashTokenResource SurfaceCornerRadius}`；不得为这两个资源再引入 C# `TokenResourceBinder` 桥接对象。
- `SplashWindow.Show()` 记录展示开始时间，供 `MinimumShowDuration` 使用。
- `SplashWindow.CloseAsync()` 先等待最短展示时间和 `CloseDelay`，再执行淡出，最后关闭窗口。
- `SplashWindow.Show()` 和 `SplashWindow.Show(owner)` 必须共享同一套 shown 状态记录；服务使用 owned window 显示时，关闭调度仍要基于真实展示时间，并且 `CloseAsync()` 必须真正关闭窗口。
- `SplashService.ShowAsync()` 创建新窗口前必须处理已有窗口；重复 show 不应产生不可追踪窗口。
- `SplashService.ShowAsync()` 默认先创建未应用调用方 options 的 `SplashWindow`，再由服务创建或复用 `SplashWindow.Splash`，并统一把 `SplashOptions` 写入窗口和 `Splash` 控件。
- `SplashService.ShowAsync()` 在 desktop lifetime 中必须优先把 `SplashWindow` 作为当前可见主窗口的 owned window 显示；点击过 SplashWindow 后再关闭时，平台输入、激活和 pointer tracking 应回到主窗口。找不到可见主窗口时才退回普通 `Show()`。
- `SplashService.CloseAsync()` 必须在成功、取消和异常路径释放 `CurrentWindow` 和订阅。
- 控件卸载、窗口关闭、服务替换或取消时必须释放事件订阅和任务引用。

Template part 接入点：

- `PART_SurfaceHost`：窗口表面阴影、阴影遮罩圆角和阴影测量扩展。
- `PART_RootLayout`：根视觉表面。
- `PART_SurfaceLayout`：背景、内容 padding 和圆角裁剪。
- `PART_LogoPresenter`：品牌图形。
- `PART_TitleBlock` / `PART_SubtitleBlock`：标题文本。
- `PART_MessageBlock` / `PART_DetailBlock`：状态文本。
- `PART_ProgressBar` / `PART_Spin`：进度反馈。
- `PART_ContentPresenter` / `PART_FooterPresenter`：扩展内容。

## 6. 交互与事件处理

Splash 的交互事件应从启动服务收敛到控件状态：

- 静态 API 调用必须委托给 `DefaultService`。
- 实例服务 API 必须回到 UI thread 更新视觉状态。
- 错误状态可以通过 `Footer` 承载重试、退出或查看日志按钮，但这些按钮的命令属于业务层。
- SplashWindow 默认不可调整大小，不提供标题栏按钮；关闭行为由调用方控制。
- 不通过全局输入捕获阻止主窗口交互；主窗口显示时序由应用启动代码控制。

当前设计不定义控件专属 public 事件；交互语义主要通过服务方法、窗口关闭任务、继承事件、属性变化和 Gallery 可观察行为体现。

## 7. 内部算法与关键流程

维护者需要重点关注以下流程：

- `SplashService.ShowAsync()` 的重复调用处理和窗口创建顺序。
- `SplashService.ShowAsync()` 的 owner 解析和 `Show(ownerWindow)` / `Show()` fallback 分支。
- `SplashService` 对 UI thread 的调度边界。
- `Splash.SetProgress`、`Splash.SetStatus`、`Splash.SetError` 对 `Progress`、`IsIndeterminate`、`Status` 的优先级归一。
- `SplashWindow.CloseAsync()` 的最短展示时间、关闭延迟、淡出和幂等。
- `Splash.DefaultService` 替换时的状态边界。
- Template part 重新应用时的伪类状态回放。
- 主题资源、Token 和 SharedToken 计算后的视觉更新。
- `SplashWindow.Resources` 与 `Splash.Resources` 的资源覆盖边界。

实现文档不逐行解释私有方法。若某个私有算法成为稳定维护入口，应在本节补充算法不变量，而不是把代码复述为说明书。

## 8. 资源、性能与 AOT 边界

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token、服务或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 不为模板稳定节点之间的 token 资源关系创建 `TokenResourceBinder` 桥接；`PART_SurfaceHost` 和 `Splash` 内部模板应通过相同的资源树解析 `SplashTokenResource`。
- 窗口级视觉覆盖写入 `SplashWindow.Resources`，确保 `PART_SurfaceHost` 与 `Splash` 内部模板都能解析；`Splash.Resources` 只用于仅影响 Splash 内部模板的覆盖。
- `SplashService` 中的延迟任务、淡出任务和取消 token 必须能取消或释放。
- 事件订阅必须与窗口或服务生命周期一致。
- `Splash.DefaultService` 替换不得保留旧服务窗口引用。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- Splash 不面向大集合和虚拟化场景，默认视觉树应保持短路径和低首次创建成本。
- 不为每次状态文本更新创建新的窗口、控件或动画对象。
- 动效应复用 Avalonia transition 和 AtomUI motion token，不手写高频 timer。

## 9. 维护不变量

维护 Splash 时不得破坏：

- 视觉控件、窗口宿主、实例服务和静态 API 的职责边界。
- Public API、默认值、服务委托路径和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- `CloseAsync()` 幂等、最短展示时间、关闭延迟和引用释放路径。
- Light/Dark、不同 DPI、不同平台窗口系统下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。

## 10. 测试与验证

推荐验证：

- 纯文档改动运行 `git diff --check` 并检查相对链接。
- 控件 API、主题或行为变更运行 `tests/AtomUI.Desktop.Controls.Tests` 中的 Splash 测试。
- 服务编排变更覆盖 `ShowAsync()`、重复 show、重复 close、取消、错误状态和 UI thread 调度。
- 窗口宿主变更走查 macOS、Windows、Linux 的显示、居中、任务栏、Topmost 和关闭动效。
- Gallery 示例或源码片段变更运行 `tests/AtomUIGallery.Tests`。
- AOT、生成器或动态数据路径变更按 Gallery NativeAOT 发布流程验证。
