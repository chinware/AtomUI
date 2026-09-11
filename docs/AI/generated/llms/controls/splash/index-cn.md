# Splash

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

Splash 是 AtomUI 桌面控件体系中的应用启动反馈控件，用于在主窗口准备完成前展示品牌、启动状态、确定或不确定进度、错误信息和可选补充内容。

Splash 不负责应用依赖注入、主窗口创建、启动异常吞掉、重试策略或业务步骤调度。这些职责应由应用启动代码、组合 ViewModel、`SplashService` 调用方或业务层承担。

主要源码入口：

- `src/AtomUI.Desktop.Controls.Extras/Splash`

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls.Extras` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/Other/Splash` |
| 状态 | Preview |

## 何时使用

Splash 的设计语言围绕“启动中但应用尚不可交互”的产品语义组织。它应比普通 `Spin` 更具品牌表达和窗口宿主语义，但不承担 `Modal` 的流程阻断和动作确认职责。

| 维度 | 含义 | Splash 中的表达 |
| --- | --- | --- |
| 产品语义 | 控件在界面中承担的稳定职责。 | 显示桌面应用启动、加载和启动失败状态。 |
| 内容承载 | 用户数据、展示内容、模板或操作入口如何进入控件。 | `Logo`、`Title`、`Subtitle`、`Message`、`Detail`、`Content`、`Footer`。 |
| 状态反馈 | public API、服务状态和模板绑定如何形成用户可感知反馈。 | `Status`、`Progress`、`IsIndeterminate`、`IsMotionEnabled`。 |
| 宿主语义 | 启动窗口、静态 API 和实例服务如何协作。 | `SplashWindow` 负责窗口，`SplashService` 负责编排，`Splash` 静态 API 做薄封装。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Splash Token + ControlTheme。 |

## 公共 API

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

- 类型：`Splash`、`SplashWindow`、`SplashService`、`SplashOptions`、`ISplashService`。
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

## 事件与命令

Splash 的事件与命令以控件文档、源码 public surface 和 Avalonia 基类契约为准；生成器不从源码发明额外事件。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础

来源：`controlgallery/AtomUIGallery/ShowCases/Other/Splash/Views/SplashShowCase.axaml:49`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:Splash Classes="preview-splash"
             Logo="{Binding BasicLogo}"
             Title="AtomUI"
             Subtitle="桌面启动流程"
             Message="正在准备工作区"
             Detail="正在加载主题、语言资源和缓存状态。"
             IsIndeterminate="True"
             HorizontalAlignment="Left">
    <atom:Splash.LogoTemplate>
        <DataTemplate x:DataType="vm:SplashLogoInfo">
            <Border Width="52"
                    Height="52"
                    CornerRadius="14"
                    Background="{Binding Background}">
                <atom:TextBlock Text="{Binding Text}"
                                Foreground="{Binding Foreground}"
                                FontSize="20"
                                FontWeight="Bold"
                                HorizontalAlignment="Center"
                                VerticalAlignment="Center" />
            </Border>
        </DataTemplate>
    </atom:Splash.LogoTemplate>
</atom:Splash>
```

### 确定进度

来源：`controlgallery/AtomUIGallery/ShowCases/Other/Splash/Views/SplashShowCase.axaml:83`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:Splash Classes="preview-splash"
             Logo="{Binding BasicLogo}"
             Title="AtomUI"
             Subtitle="桌面启动流程"
             Message="正在加载模块"
             Detail="主题、图标和路由目录已就绪，正在初始化可选包。"
             Progress="{Binding ProgressValue}"
             IsIndeterminate="False"
             HorizontalAlignment="Left">
    <atom:Splash.LogoTemplate>
        <DataTemplate x:DataType="vm:SplashLogoInfo">
            <Border Width="52"
                    Height="52"
                    CornerRadius="14"
                    Background="{Binding Background}">
                <atom:TextBlock Text="{Binding Text}"
                                Foreground="{Binding Foreground}"
                                FontSize="20"
                                FontWeight="Bold"
                                HorizontalAlignment="Center"
                                VerticalAlignment="Center" />
            </Border>
        </DataTemplate>
    </atom:Splash.LogoTemplate>
</atom:Splash>
```

### 状态

来源：`controlgallery/AtomUIGallery/ShowCases/Other/Splash/Views/SplashShowCase.axaml:119`

Gallery key：`ExamplesContent` / item `2`

```axaml
<WrapPanel Orientation="Horizontal"
           ItemSpacing="20"
           LineSpacing="20">
    <atom:Splash Classes="status-splash"
                 Title="AtomUI"
                 Message="工作区已就绪"
                 Detail="满足最短展示时长后即可显示主窗口。"
                 Status="Success"
                 Progress="1"
                 IsIndeterminate="False"
                 Footer="Gallery 静态预览" />
    <atom:Splash Classes="status-splash"
                 Title="AtomUI"
                 Message="启动失败"
                 Detail="可通过 SetErrorAsync 在关闭前呈现阻塞型启动错误。"
                 Status="Error"
                 IsIndeterminate="False"
                 Footer="Gallery 静态预览" />
</WrapPanel>
```

### Logo、内容与页脚

来源：`controlgallery/AtomUIGallery/ShowCases/Other/Splash/Views/SplashShowCase.axaml:149`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:Splash Width="420"
             MinHeight="300"
             Logo="{Binding ComposedLogo}"
             Title="AtomUI Gallery"
             Subtitle="桌面启动流程"
             Message="正在加载模块"
             Detail="主题、图标和路由目录已就绪，正在初始化可选包。"
             Progress="{Binding ProgressValue}"
             IsIndeterminate="False"
             Footer="{Binding ComposedFooter}"
             HorizontalAlignment="Left">
    <StackPanel Orientation="Horizontal"
                Spacing="8"
                HorizontalAlignment="Center">
        <atom:Tag Text="核心"
                  TagColor="success" />
        <atom:Tag Text="主题"
                  TagColor="processing" />
        <atom:Tag Text="Gallery"
                  TagColor="warning" />
    </StackPanel>
    <atom:Splash.LogoTemplate>
        <DataTemplate x:DataType="vm:SplashLogoInfo">
            <Border Width="56"
                    Height="56"
                    CornerRadius="18"
                    Background="{Binding Background}">
                <atom:TextBlock Text="{Binding Text}"
                                Foreground="{Binding Foreground}"
                                FontSize="18"
                                FontWeight="Bold"
                                HorizontalAlignment="Center"
                                VerticalAlignment="Center" />
            </Border>
        </DataTemplate>
    </atom:Splash.LogoTemplate>
    <atom:Splash.FooterTemplate>
        <DataTemplate x:DataType="vm:SplashFooterInfo">
            <StackPanel Orientation="Horizontal"
                        Spacing="8"
                        VerticalAlignment="Center">
                <atom:Tag Text="{Binding Version}"
                          TagColor="geekblue" />
                <atom:TextBlock Text="{Binding Description}"
                                Foreground="{atom:SharedTokenResource ColorTextTertiary}"
                                VerticalAlignment="Center" />
            </StackPanel>
        </DataTemplate>
    </atom:Splash.FooterTemplate>
</atom:Splash>
```

## 状态模型

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

## 主题与 Design Token

Splash 的视觉模型由 `Splash` 控件模板、`SplashWindow` 宿主主题、SharedToken 和 `SplashToken` 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `SplashTheme.axaml` | 定义启动页视觉控件模板、状态 selector、ProgressBar/Spin 组合和内容区域。 |
| `SplashWindowTheme.axaml` | 定义桌面启动窗口宿主、透明无装饰窗口模板、阴影宿主和内容承载边界。 |

Splash 使用独立的 Control identity 和 `SplashToken` Own Token scope。Token 只表达组件视觉语义，不承载 `Status`、`Progress`、`IsIndeterminate`、启动步骤或异常对象。`SplashTheme.axaml` 通过 `SplashTokenResource` 统一读取 Splash 的 Effective Global Token 和 Own Token：标题读取 Global Token `ColorTextHeading`，普通消息读取 Global Token `ColorText`，副标题和详情读取 Own Token `SubtleForeground`。默认没有 Splash Control 级覆盖时，Effective Global Token 回退到当前主题的全局结果，因此默认 Light/Dark 视觉不变。
`SplashWindow` 使用 `{x:Type atom:SplashWindow}` 作为隐式 `ControlTheme` key；窗口模板必须保持透明内容宿主，避免默认 Window 背景破坏 Splash 表面圆角。
`SplashWindowTheme.axaml` 直接使用 `ShadowsAwareContainer#PART_SurfaceHost` 承载 `Splash`，由 `SurfaceBoxShadow` 控制窗口表面阴影，由 `SurfaceCornerRadius` 控制阴影遮罩圆角。`SplashTheme.axaml` 内部的 `PART_RootLayout` 和 `PART_SurfaceLayout` 继续负责背景、内容圆角和裁剪。

资源覆盖边界：

- 同时影响窗口阴影宿主和 Splash 内容表面的视觉资源，应写入 `SplashWindow.Resources`。
- 只影响 `Splash` 内部模板的资源，可以写入 `Splash.Resources`。
- 应用需要完整的专用启动窗口视觉时，定义自己的 `SplashWindow` 和内部 Splash 子控件；子控件通过 `StyleKeyOverride` 复用标准 Splash Theme，并由自己的 AXAML `Styles` 维护专用模板视觉。窗口或页面不得进入 Splash 模板修改内部节点。
- 不通过 C# `TokenResourceBinder` 在窗口宿主和 Splash 之间桥接 `SurfaceBoxShadow`、`SurfaceCornerRadius` 等模板可表达关系。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 视觉结构优先使用 AXAML、`TemplateBinding`、selector、`Spin`、`ProgressBar` 和 `ContentPresenter` 表达。
- 不把状态显示逻辑改成 C# 动态创建视觉，除非 AXAML 无法表达且生命周期 owner 明确。
- Gallery 和业务代码不得从页面、父控件 Style 或窗口 ControlTheme 通过 `/template/`、C# `.Template()` 或 `PART_*` 名称进入 Splash 模板。专用 Splash 子控件可以通过 `StyleKeyOverride` 复用标准 Splash Theme，并在自己的 AXAML `Styles` 中进入自己的一层模板；该子控件承担模板契约所有权。
- Light/Dark 主题应保持品牌区域、进度区域、错误状态和窗口表面的对比度。

Token 来源：

Splash Token 只表达组件级视觉变量，例如窗口尺寸、内容间距、品牌尺寸、文字规格、进度区域间距、圆角、阴影和状态色。Token 不承载启动步骤、`Status`、`Progress`、`IsIndeterminate`、异常对象、主窗口引用或服务状态。

当前 Token scope：

- `SplashToken`，scope id 为 `Splash`，源码位于 `src/AtomUI.Desktop.Controls.Extras/Splash/SplashToken.cs`。

## AOT 与裁剪注意事项

资源和 AOT 约束：

- 不通过运行时反射扫描 public API、Token、服务或 Gallery 示例数据。
- 不把可静态声明的模板结构迁移到 C# 动态创建。
- 不为模板稳定节点之间的 token 资源关系创建 `TokenResourceBinder` 桥接；`PART_SurfaceHost` 和 `Splash` 内部模板应通过相同的资源树解析 `SplashTokenResource`。
- 窗口级视觉覆盖写入 `SplashWindow.Resources`，确保 `PART_SurfaceHost` 与 `Splash` 内部模板都能解析；`Splash.Resources` 只用于仅影响 Splash 内部模板的覆盖。
- Gallery 和业务代码不得从页面、父控件 Style、窗口 ControlTheme 或 C# 动态 Style 穿透 Splash 模板。需要完整专用视觉时定义专用 Splash 子控件，通过 `StyleKeyOverride` 复用标准 Theme，并把专用 selector 放在该子控件自己的 AXAML `Styles` 中；每个 selector 最多进入自己的一层模板，并继续遵守稳定 template part 和状态 selector 契约。
- `SplashService` 中的延迟任务、淡出任务和取消 token 必须能取消或释放。
- 事件订阅必须与窗口或服务生命周期一致。
- `Splash.DefaultService` 替换不得保留旧服务窗口引用。
- Source generator 生成文件不手工编辑；需要修改时改输入源或 generator。

性能边界：

- Splash 不面向大集合和虚拟化场景，默认视觉树应保持短路径和低首次创建成本。
- 不为每次状态文本更新创建新的窗口、控件或动画对象。
- 动效应复用 Avalonia transition 和 AtomUI motion token，不手写高频 timer。

## 源码索引

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
- `src/AtomUI.Desktop.Controls.Extras/AtomUIExtrasThemesProvider.cs`
- `src/AtomUI.Desktop.Controls.Extras/ThemeManagerBuilderExtensions.cs`

职责边界：

- `Splash.cs` 保留视觉控件 public/protected API、状态写入方法、Avalonia 属性注册、伪类同步和主要模板生命周期入口。
- `Splash.StaticAPI.cs` 只放静态便利入口，所有逻辑委托给 `Splash.DefaultService`。
- `SplashWindow.cs` 负责窗口级 `Splash` 内容承载属性、展示时间记录、淡出关闭和关闭请求状态；`SplashWindowTheme.axaml` 负责透明无装饰窗口默认值、窗口模板、阴影宿主和内容承载边界。
- `SplashService.cs` 是启动编排 owner，负责创建窗口、创建或复用 `Splash` 实例、应用运行时 options、更新状态、关闭窗口和 UI thread 调度。
- Theme 文件负责静态视觉结构、template part、selector 和资源绑定；`SplashTheme.axaml` 使用 `SplashTokenResource` 读取 Splash Effective Global Token 与 Own Token，不绕过 Control identity 直接读取同名 SharedToken。
- Token 文件只提供组件视觉变量，不保存实例状态。
- Gallery 文件只展示用法和示例，不作为运行时逻辑 owner。

## 相关文档

- 源设计文档：`docs/controls/desktop/feedback/splash/overview.md`
- 实现文档：`docs/controls/desktop/feedback/splash/implementation.md`
- Token 文档：`docs/controls/desktop/feedback/splash/token.md`
- 变更记录：`docs/controls/desktop/feedback/splash/changelog.md`
- 语义结构：`./semantic-cn.md`
