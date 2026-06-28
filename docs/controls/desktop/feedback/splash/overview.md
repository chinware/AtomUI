# Splash 桌面版架构设计

本文档定义 `Splash` 桌面版的设计定位、公共契约、状态模型、视觉主题关系和兼容边界。通用控件研发约束见 [控件研发标准](../../../../engineering/control-development-guidelines.md)，内部实现原理见 [Splash 桌面版实现原理](implementation.md)，Splash Token 的专项设计见 [Splash Token 设计](token.md)，设计和契约变化记录见 [Splash Changelog](changelog.md)。

## 1. 控件定位

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls.Extras` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Feedback/Splash` |
| 控件状态 | Design |

Splash 是 AtomUI 桌面控件体系中的应用启动反馈控件，用于在主窗口准备完成前展示品牌、启动状态、确定或不确定进度、错误信息和可选补充内容。

Splash 不负责应用依赖注入、主窗口创建、启动异常吞掉、重试策略或业务步骤调度。这些职责应由应用启动代码、组合 ViewModel、`SplashService` 调用方或业务层承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls.Extras/Splash`

## 2. 设计语言

Splash 的设计语言围绕“启动中但应用尚不可交互”的产品语义组织。它应比普通 `Spin` 更具品牌表达和窗口宿主语义，但不承担 `Modal` 的流程阻断和动作确认职责。

| 维度 | 含义 | Splash 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | 显示桌面应用启动、加载和启动失败状态。 |
| 内容承载 | 用户数据、展示内容、模板或操作入口如何进入控件。 | `Logo`、`Title`、`Subtitle`、`Message`、`Detail`、`Content`、`Footer`。 |
| 状态反馈 | public API、服务状态和模板绑定如何形成用户可感知反馈。 | `Status`、`Progress`、`IsIndeterminate`、`IsMotionEnabled`。 |
| 宿主语义 | 启动窗口、静态 API 和实例服务如何协作。 | `SplashWindow` 负责窗口，`SplashService` 负责编排，`Splash` 静态 API 做薄封装。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Splash Token + ControlTheme。 |

## 3. API 与契约模型

Splash 的公共契约由视觉控件、启动窗口、实例服务、静态便利入口和主题资源共同组成。维护时应先确认这些契约是否已经被源码、Gallery 示例或文档暴露。

核心 public surface 按语义分组维护：

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 品牌与标题 | `Logo`、`LogoTemplate`、`Title`、`Subtitle` | 定义启动页品牌识别和主标题区域。 |
| 状态文本 | `Message`、`Detail` | 表达当前启动阶段、错误详情或补充说明。 |
| 进度状态 | `Progress`、`IsIndeterminate`、`Status` | 表达确定进度、不确定加载、成功和错误状态。 |
| 内容扩展 | `Content`、`ContentTemplate`、`Footer`、`FooterTemplate` | 承载自定义启动内容、版本信息、版权信息或业务操作入口。 |
| 动效与窗口 | `IsMotionEnabled`、`MinimumShowDuration`、`CloseDelay`、`FadeOutDuration`、`Topmost` | 控制启动展示时长、关闭节奏和窗口级视觉体验。 |
| 服务入口 | `ISplashService`、`SplashService`、`Splash.DefaultService`、`Splash.ShowAsync()`、`Splash.CloseAsync()` | 提供实例 API 和静态 API，静态 API 委托给可替换服务实例。 |

主要公开类型与枚举：

- 类型：`Splash`、`SplashWindow`、`SplashService`、`SplashOptions`、`SplashController`、`ISplashService`。
- 枚举：`SplashStatus`。

稳定 template part：

| Template Part | 类型 | 职责 |
| --- | --- | --- |
| `PART_RootLayout` | `Panel` | 承载启动页根布局、背景、圆角和阴影边界。 |
| `PART_LogoPresenter` | `ContentPresenter` | 展示 `Logo` 和 `LogoTemplate`。 |
| `PART_TitleBlock` | `TextBlock` | 展示主标题。 |
| `PART_SubtitleBlock` | `TextBlock` | 展示副标题。 |
| `PART_MessageBlock` | `TextBlock` | 展示当前状态消息。 |
| `PART_DetailBlock` | `TextBlock` | 展示详细信息或错误详情。 |
| `PART_ProgressBar` | `ProgressBar` | 展示确定进度。 |
| `PART_Spin` | `Spin` | 展示不确定加载状态。 |
| `PART_ContentPresenter` | `ContentPresenter` | 展示扩展内容。 |
| `PART_FooterPresenter` | `ContentPresenter` | 展示底部内容或启动失败操作入口。 |

控件专属伪类：

| 伪类 | 含义 |
| --- | --- |
| `:loading` | `Status` 为 `Loading`。 |
| `:success` | `Status` 为 `Success`。 |
| `:error` | `Status` 为 `Error`。 |
| `:indeterminate` | `IsIndeterminate` 为 `true`。 |
| `:determinate` | `Progress` 有有效值且 `IsIndeterminate` 为 `false`。 |

## 4. 行为与状态模型

Splash 的状态流按以下路径收敛：

```text
Splash visual API / SplashService API / Splash static API
  -> SplashController state
  -> Splash instance properties
  -> pseudo-class / template binding
  -> ControlTheme selector / ProgressBar / Spin / TextBlock
  -> SplashWindow visible behavior
```

状态维护规则：

- `Splash` 视觉控件只持有可展示状态，不创建主窗口、不关闭应用、不吞异常。
- `SplashWindow` 只持有窗口级状态和关闭动效，不解释业务启动步骤。
- `SplashService` 是实例 API 的状态 owner，同一个服务实例一次只管理一个 `CurrentWindow`。
- `Splash` 静态 API 只委托给 `Splash.DefaultService`，不直接持有窗口或视觉节点。
- `Progress` 为 `null` 或 `IsIndeterminate=true` 时显示不确定加载；`Progress` 有效且 `IsIndeterminate=false` 时显示确定进度。
- `Status=Error` 时保留窗口，等待调用方决定重试、退出或显示补充内容。
- `CloseAsync()` 必须幂等；重复调用、取消或窗口已关闭都不能留下不可释放窗口引用。
- 所有服务 API 写入 UI 状态时必须回到 UI thread。

## 5. 视觉与主题模型

Splash 的视觉模型由 `Splash` 控件模板、`SplashWindow` 宿主主题、SharedToken 和 `SplashToken` 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `SplashTheme.axaml` | 定义启动页视觉控件模板、状态 selector、ProgressBar/Spin 组合和内容区域。 |
| `SplashWindowTheme.axaml` | 定义桌面启动窗口宿主、无标题栏、不可调整大小、圆角和阴影边界。 |
| `SplashThemes.axaml` | 聚合 Splash 控件家族主题资源，保证包级引入顺序稳定。 |

Splash 使用 `SplashToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 `Status`、`Progress`、`IsIndeterminate`、启动步骤或异常对象。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 视觉结构优先使用 AXAML、`TemplateBinding`、selector、`Spin`、`ProgressBar` 和 `ContentPresenter` 表达。
- 不把状态显示逻辑改成 C# 动态创建视觉，除非 AXAML 无法表达且生命周期 owner 明确。
- Light/Dark 主题应保持品牌区域、进度区域、错误状态和窗口表面的对比度。

## 6. 控件家族或集成关系

Splash 属于 `AtomUI.Desktop.Controls.Extras` 中的稳定补充控件。它与 `Spin`、`ProgressBar`、`Window` 和 `Result` 共享反馈、进度和窗口主题能力，但保持启动页职责边界。

主要协作类型：

- `Splash`：视觉控件，承载品牌、文本、进度、状态和扩展内容。
- `SplashWindow`：桌面宿主窗口，承载 `Splash` 并管理显示、淡出和关闭节奏。
- `SplashController`：状态写入入口，供 `SplashWindow` 和服务统一更新 visual state。
- `ISplashService`：实例服务契约，适合应用启动代码、测试和依赖注入场景。
- `SplashService`：默认实例服务，实现 show/update/close 编排。
- `SplashOptions`：启动窗口和初始内容配置对象。
- `SplashToken`：组件 Token scope，负责从全局 token 派生启动页视觉变量。

集成关系：

- `UseDesktopExtras()` 注册 `SplashToken` 和 `AtomUIExtrasThemesProvider`，不由 `UseDesktopControls()` 默认加载。
- `SplashService` 不绑定特定 DI 容器，不自动注册全局服务。
- 静态 API 使用 `Splash.DefaultService`，用户可以替换为自定义 `ISplashService`。
- 多窗口或多个启动流程使用独立 `SplashService` 实例；静态 API 只管理一个默认启动流。

## 7. 兼容性不变量

维护 Splash 时必须保持以下不变量：

- `Splash` 视觉控件不接管应用生命周期，不创建主窗口，不吞业务异常。
- `SplashService` 不替调用方决定错误后退出、重试或继续。
- 静态 API 不绕过 `ISplashService`，不直接持有窗口或模板节点。
- `CloseAsync()` 保持幂等，最短展示时间和关闭延迟不会导致窗口引用泄漏。
- `Status`、`Progress`、`IsIndeterminate` 的优先级稳定，Gallery 和用户 XAML 可依赖。
- Template part、伪类、ControlTheme key、Token 名称和资源 key 不擅自变更。
- 不引入运行时反射扫描作为 API、Token、服务或窗口发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

## 8. 专项模型

### 8.1 启动编排模型

启动编排以 `SplashService` 为 owner：

```text
Application startup code
  -> ISplashService.ShowAsync(options)
  -> SplashWindow.Show()
  -> ISplashService.SetMessage / SetProgress / SetError
  -> main window Show()
  -> ISplashService.CloseAsync()
```

应用启动代码仍然拥有主窗口、业务异常和退出策略。Splash 只展示状态并管理自己的窗口生命周期。

### 8.2 静态与实例 API 模型

实例 API 是主要扩展点，静态 API 是低门槛入口。静态 API 位于 `Splash` 类型上：

```text
Splash.ShowAsync(...)
Splash.SetMessage(...)
Splash.CloseAsync(...)
```

这些静态方法只委托给 `Splash.DefaultService`。`Splash.DefaultService` 可替换，但替换行为必须发生在启动流开始前。

### 8.3 进度与状态模型

`SplashStatus` 只表达视觉状态：

- `Loading`：启动中，默认状态。
- `Success`：启动完成或准备关闭前的成功反馈。
- `Error`：启动失败，窗口保持显示。

确定进度和不确定进度通过 `Progress` 与 `IsIndeterminate` 归一。错误详情使用 `Detail` 或自定义 `Content` 展示，不把异常对象写入控件 API。

### 8.4 窗口宿主模型

`SplashWindow` 默认不显示任务栏、不显示标题栏、不可调整大小并居中显示。关闭动效只影响 Splash 自身窗口，不改变主窗口的显示、激活或 owner 关系。

## 9. 文档导航、LLMS 导出与验证策略

关联文档：

- [Splash 桌面版实现原理](implementation.md)
- [Splash Token 设计](token.md)
- [Splash Changelog](changelog.md)

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Splash` | 启动反馈控件根语义区域，承载 public API、状态和主题入口。 | `Logo`、`Title`、`Status`、`Progress` | `SplashToken` | stable |
| `host` | `SplashWindow` | 承载独立桌面启动窗口和关闭动效。 | `MinimumShowDuration`、`CloseDelay`、`FadeOutDuration` | `WindowWidth`、`WindowMinHeight` | stable |
| `brand` | `PART_LogoPresenter`、`PART_TitleBlock`、`PART_SubtitleBlock` | 展示品牌和应用身份。 | `Logo`、`LogoTemplate`、`Title`、`Subtitle` | `LogoSize`、`TitleFontSize` | template-stable |
| `status` | `PART_MessageBlock`、`PART_DetailBlock` | 展示启动阶段、错误详情或补充说明。 | `Message`、`Detail`、`Status` | `MessageFontSize`、`DetailFontSize` | template-stable |
| `progress` | `PART_ProgressBar`、`PART_Spin` | 展示确定或不确定进度。 | `Progress`、`IsIndeterminate` | `ProgressMarginTop`、`IndicatorSize` | template-stable |
| `content` | `PART_ContentPresenter`、`PART_FooterPresenter` | 承载自定义内容和底部区域。 | `Content`、`ContentTemplate`、`Footer`、`FooterTemplate` | `ContentGap`、`FooterMarginTop` | template-stable |

LLMS 导出来源：

| LLMS 内容 | 来源 | 说明 |
| --- | --- | --- |
| 单控件完整文档 | `overview.md` + Gallery API / Token / ShowCase | 生成 `controls/splash/index-cn.md` |
| 单控件语义文档 | `overview.md` + `implementation.md` + theme/template 信息 | 生成 `controls/splash/semantic-cn.md` |
| API 表 | Gallery ApiDataGrid 或源码 public surface | 不在 `overview.md` 中复制完整 API 表 |
| Design Token 表 | Gallery DesignTokenDataGrid、Token 类型或第 5 节主题模型 | 不在生成产物中手工维护第二份 Token 表 |
| 示例 | Gallery ShowCase + source snippet catalog | 只引用稳定示例 |
| 源码索引 | `implementation.md` | 用于定位控件源码、主题和测试 |

验证策略：

| 改动类型 | 验证要求 |
| --- | --- |
| 文档改动 | 运行 `git diff --check`，检查相对链接存在。 |
| Public API | 覆盖视觉控件默认值、服务默认值、静态 API 委托和窗口默认值。 |
| 状态模型 | 覆盖 `Loading`、`Success`、`Error`、确定进度、不确定进度、重复关闭和取消。 |
| AXAML/Theme | 检查 template part、伪类、资源 key、Light/Dark 主题和窗口宿主主题。 |
| Token | 检查 TokenKind、AXAML token resource、Gallery Token 表和文档同步。 |
| Gallery | 走查基础启动页、确定进度、不确定进度、自定义内容和错误状态示例。 |
