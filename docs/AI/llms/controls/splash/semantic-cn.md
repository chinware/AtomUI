# Splash 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

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

来源：`src/AtomUI.Desktop.Controls.Extras/Splash/Themes/SplashTheme.axaml`

```xml
<Border Name="PART_RootLayout">
    <Border Name="PART_SurfaceLayout">
        <StackPanel Name="PART_ContentLayout">
            <ContentPresenter Name="PART_LogoPresenter" />
            <TextBlock Name="PART_TitleBlock" />
            <TextBlock Name="PART_SubtitleBlock" />
            <ContentPresenter Name="PART_ContentPresenter" />
            <Panel Name="PART_ProgressLayout">
                <Spin Name="PART_Spin" />
                <ProgressBar Name="PART_ProgressBar" />
            </Panel>
            <TextBlock Name="PART_MessageBlock" />
            <TextBlock Name="PART_DetailBlock" />
            <ContentPresenter Name="PART_FooterPresenter" />
        </StackPanel>
    </Border>
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Splash
  -> Splash (control theme, SplashTheme.axaml)
     -> Border#PART_RootLayout (template-stable)
        -> Border#PART_SurfaceLayout (template-stable)
           -> StackPanel#PART_ContentLayout (template-stable)
              -> ContentPresenter#PART_LogoPresenter (template-stable)
              -> TextBlock#PART_TitleBlock (template-stable)
              -> TextBlock#PART_SubtitleBlock (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
              -> Panel#PART_ProgressLayout (template-stable)
                 -> Spin#PART_Spin (template-stable)
                 -> ProgressBar#PART_ProgressBar (template-stable)
              -> TextBlock#PART_MessageBlock (template-stable)
              -> TextBlock#PART_DetailBlock (template-stable)
              -> ContentPresenter#PART_FooterPresenter (template-stable)
  -> SplashWindow (control theme, SplashWindowTheme.axaml)
     -> ShadowsAwareContainer#PART_SurfaceHost (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Splash` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Splash` | control theme | `SplashTheme.axaml` | 用户代码 / 控件宿主 | `Content`, `ContentTemplate`, `Detail`, `Footer`, `FooterTemplate`, `IsProgressBarVisible` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_RootLayout` | template node (Border) | `SplashTheme.axaml` | Splash | `Content`, `ContentTemplate`, `Detail`, `Footer`, `FooterTemplate`, `IsProgressBarVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SurfaceLayout` | template node (Border) | `SplashTheme.axaml` | Splash | `Content`, `ContentTemplate`, `Detail`, `Footer`, `FooterTemplate`, `IsProgressBarVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentLayout` | template node (StackPanel) | `SplashTheme.axaml` | Splash | `Content`, `ContentTemplate`, `Detail`, `Footer`, `FooterTemplate`, `IsProgressBarVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LogoPresenter` | template node (ContentPresenter) | `SplashTheme.axaml` | Splash | `Logo`, `LogoTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_TitleBlock` | template node (TextBlock) | `SplashTheme.axaml` | Splash | `Title` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_SubtitleBlock` | template node (TextBlock) | `SplashTheme.axaml` | Splash | `Subtitle` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `SplashTheme.axaml` | Splash | `Content`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ProgressLayout` | template node (Panel) | `SplashTheme.axaml` | Splash | `IsProgressBarVisible`, `IsSpinVisible`, `ProgressValue` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_Spin` | template node (Spin) | `SplashTheme.axaml` | Splash | `IsSpinVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ProgressBar` | template node (ProgressBar) | `SplashTheme.axaml` | Splash | `IsProgressBarVisible`, `ProgressValue` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_MessageBlock` | template node (TextBlock) | `SplashTheme.axaml` | Splash | `Message` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_DetailBlock` | template node (TextBlock) | `SplashTheme.axaml` | Splash | `Detail` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FooterPresenter` | template node (ContentPresenter) | `SplashTheme.axaml` | Splash | `Footer`, `FooterTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `SplashWindow` | control theme | `SplashWindowTheme.axaml` | 用户代码 / 控件宿主 | `Splash` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `PART_SurfaceHost` | template node (ShadowsAwareContainer) | `SplashWindowTheme.axaml` | SplashWindow | `Splash` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

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
  -> Splash instance properties
  -> pseudo-class / template binding
  -> ControlTheme selector / ProgressBar / Spin / TextBlock
  -> SplashWindow visible behavior
```

状态维护规则：

- `Splash` 视觉控件只持有可展示状态，不创建主窗口、不关闭应用、不吞异常。
- `Splash` 本体提供 `SetMessage`、`SetProgress`、`SetStatus` 和 `SetError` 状态写入方法。
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
| `SplashWindowTheme.axaml` | 定义桌面启动窗口宿主、透明无装饰窗口模板、阴影宿主和内容承载边界。 |

Splash 使用 `SplashToken` 作为控件 Token scope。Token 只表达组件视觉语义，不承载 `Status`、`Progress`、`IsIndeterminate`、启动步骤或异常对象。
`SplashWindow` 使用 `{x:Type atom:SplashWindow}` 作为隐式 `ControlTheme` key；窗口模板必须保持透明内容宿主，避免默认 Window 背景破坏 Splash 表面圆角。
`SplashWindowTheme.axaml` 直接使用 `ShadowsAwareContainer#PART_SurfaceHost` 承载 `Splash`，由 `SurfaceBoxShadow` 控制窗口表面阴影，由 `SurfaceCornerRadius` 控制阴影遮罩圆角。`SplashTheme.axaml` 内部的 `PART_RootLayout` 和 `PART_SurfaceLayout` 继续负责背景、内容圆角和裁剪。

资源覆盖边界：

- 同时影响窗口阴影宿主和 Splash 内容表面的视觉资源，应写入 `SplashWindow.Resources`。
- 只影响 `Splash` 内部模板的资源，可以写入 `Splash.Resources`。
- 不通过 C# `TokenResourceBinder` 在窗口宿主和 Splash 之间桥接 `SurfaceBoxShadow`、`SurfaceCornerRadius` 等模板可表达关系。

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
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
