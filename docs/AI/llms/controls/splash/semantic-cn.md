# Splash 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Splash` | 启动反馈控件根语义区域，承载 public API、状态和主题入口。 | `Logo`、`Title`、`Status`、`Progress` | `SplashToken` | stable |
| `host` | `SplashWindow` | 承载独立桌面启动窗口和关闭动效。 | `MinimumShowDuration`、`CloseDelay`、`FadeOutDuration` | `WindowWidth`、`WindowMinHeight` | stable |
| `brand` | `PART_LogoPresenter`、`PART_TitleBlock`、`PART_SubtitleBlock` | 展示品牌和应用身份。 | `Logo`、`LogoTemplate`、`Title`、`Subtitle` | `LogoSize`、`TitleFontSize` | template-stable |
| `status` | `PART_MessageBlock`、`PART_DetailBlock` | 展示启动阶段、错误详情或补充说明。 | `Message`、`Detail`、`Status` | `MessageFontSize`、`DetailFontSize` | template-stable |
| `progress` | `PART_ProgressBar`、`PART_Spin` | 展示确定或不确定进度。 | `Progress`、`IsIndeterminate` | `ProgressMarginTop`、`IndicatorSize` | template-stable |
| `content` | `PART_ContentPresenter`、`PART_FooterPresenter` | 承载自定义内容和底部区域。 | `Content`、`ContentTemplate`、`Footer`、`FooterTemplate` | `ContentGap`、`FooterMarginTop` | template-stable |

## Abstract AXAML Structure

未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。

## Composition Model

该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。

## Template Parts

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

## Pseudo Classes

控件专属伪类：

| 伪类 | 含义 |
| --- | --- |
| `:loading` | `Status` 为 `Loading`。 |
| `:success` | `Status` 为 `Success`。 |
| `:error` | `Status` 为 `Error`。 |
| `:indeterminate` | `IsIndeterminate` 为 `true`。 |
| `:determinate` | `Progress` 有有效值且 `IsIndeterminate` 为 `false`。 |

## State Flow

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

## Theme and Token Boundaries

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

Token 边界：

Splash Token 只表达组件级视觉变量，例如窗口尺寸、内容间距、品牌尺寸、文字规格、进度区域间距、圆角、阴影和状态色。Token 不承载启动步骤、`Status`、`Progress`、`IsIndeterminate`、异常对象、主窗口引用或服务状态。

当前 Token scope：

- `SplashToken`，scope id 为 `Splash`，源码位于 `src/AtomUI.Desktop.Controls.Extras/Splash/SplashToken.cs`。

## Customization Boundaries

维护 Splash 时必须保持以下不变量：

- `Splash` 视觉控件不接管应用生命周期，不创建主窗口，不吞业务异常。
- `SplashService` 不替调用方决定错误后退出、重试或继续。
- 静态 API 不绕过 `ISplashService`，不直接持有窗口或模板节点。
- `CloseAsync()` 保持幂等，最短展示时间和关闭延迟不会导致窗口引用泄漏。
- `Status`、`Progress`、`IsIndeterminate` 的优先级稳定，Gallery 和用户 XAML 可依赖。
- Template part、伪类、ControlTheme key、Token 名称和资源 key 不擅自变更。
- 不引入运行时反射扫描作为 API、Token、服务或窗口发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Splash 时不得破坏：

- 视觉控件、窗口宿主、实例服务和静态 API 的职责边界。
- Public API、默认值、服务委托路径和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- `CloseAsync()` 幂等、最短展示时间、关闭延迟和引用释放路径。
- Light/Dark、不同 DPI、不同平台窗口系统下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
