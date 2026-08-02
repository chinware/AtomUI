# Window 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `Window` | 窗口控件根语义区域，承载窗口 public API、平台状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `chrome` | `窗口装饰区域` | 承载标题栏、caption buttons、drag region、边框和阴影。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `窗口内容区域` | 承载业务内容、系统交互和布局边界。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `state` | `窗口状态区域` | 表达最大化、最小化、激活、失焦、resize 和平台能力。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml`

```xml
<Panel>
    <MediaBreakPointIndicator Name="{x:Static atom:MediaBreakPointIndicator.MediaQueryIndicatorName}" />
    <Border Name="PART_TransparencyFallback" />
    <Border Name="WindowFrame">
        <ContentPresenter Name="WindowFrameLayer" />
    </Border>
    <Panel />
    <WindowVisualLayerClip>
        <VisualLayerManager Name="PART_VisualLayerManager">
            <Border Name="WindowContentClip">
                <DockPanel>
                    <Panel Name="TitleBarPanel">
                        <ContentPresenter Name="TitleBarFrameLayer" />
                        <ContentPresenter />
                    </Panel>
                    <Panel>
                        <ContentPresenter Name="ContentFrameLayer" />
                        <Border Name="ContentFrame">
                        </Border>
                    </Panel>
                </DockPanel>
            </Border>
        </VisualLayerManager>
    </WindowVisualLayerClip>
    <FullscreenPopoverLayer Name="PART_FullscreenPopoverLayer" />
    <WindowResizer Name="PART_WindowResizer" />
</Panel>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
Window
  -> FullscreenPopoverLayer (control theme, FullscreenPopoverLayerTheme.axaml)
     -> Panel (template-stable)
        -> Border#PART_PopoverBorder (template-stable)
           -> WindowTitleBarLayoutPanel (template-stable)
              -> Panel (template-stable)
              -> DockPanel (template-stable)
                 -> ContentPresenter#FullscreenLogoPresenter (internal-observable)
                 -> TextBlock#FullscreenTitleText (template-stable)
              -> StackPanel#FullscreenCaptionButtonGroup (template-stable)
                 -> CaptionButton#PART_PopoverFullScreenButton (template-stable)
                 -> CaptionButton#PART_PopoverCloseButton (template-stable)
  -> WindowDrawnDecorations (control theme, WindowDrawnDecorationsTheme.axaml)
  -> WindowResizer (control theme, WindowResizerTheme.axaml)
     -> Panel#PART_RootLayout (template-stable)
        -> Border (template-stable)
        -> Border (template-stable)
        -> Border (template-stable)
        -> Border (template-stable)
        -> Border (template-stable)
        -> Border (template-stable)
        -> Border (template-stable)
        -> Border (template-stable)
  -> Window (control theme, WindowTheme.axaml)
     -> Panel (template-stable)
        -> MediaBreakPointIndicator#{x:Static atom:MediaBreakPointIndicator.MediaQueryIndicatorName} (internal-observable)
        -> Border#PART_TransparencyFallback (template-stable)
        -> Border#WindowFrame (template-stable)
           -> ContentPresenter#WindowFrameLayer (internal-observable)
        -> Panel (template-stable)
        -> WindowVisualLayerClip (template-stable)
           -> VisualLayerManager#PART_VisualLayerManager (template-stable)
              -> Border#WindowContentClip (template-stable)
                 -> DockPanel (template-stable)
                    -> Panel#TitleBarPanel (template-stable)
                       -> ContentPresenter#TitleBarFrameLayer (internal-observable)
                       -> ContentPresenter (internal-observable)
                    -> Panel (template-stable)
                       -> ContentPresenter#ContentFrameLayer (internal-observable)
                       -> Border#ContentFrame (template-stable)
                          -> ContentPresenter#PART_ContentPresenter (template-stable)
        -> FullscreenPopoverLayer#PART_FullscreenPopoverLayer (template-stable)
        -> WindowResizer#PART_WindowResizer (template-stable)
     -> Panel (template-stable)
        -> MediaBreakPointIndicator#{x:Static atom:MediaBreakPointIndicator.MediaQueryIndicatorName} (internal-observable)
        -> Border#PART_TransparencyFallback (template-stable)
        -> Border#WindowFrame (template-stable)
           -> ContentPresenter#WindowFrameLayer (internal-observable)
        -> Panel (template-stable)
        -> VisualLayerManager#PART_VisualLayerManager (template-stable)
           -> DockPanel (template-stable)
              -> Panel#TitleBarPanel (template-stable)
                 -> ContentPresenter#TitleBarFrameLayer (internal-observable)
                 -> ContentPresenter (internal-observable)
              -> Panel (template-stable)
                 -> ContentPresenter#ContentFrameLayer (internal-observable)
                 -> Border#ContentFrame (template-stable)
                    -> ContentPresenter#PART_ContentPresenter (template-stable)
     -> Panel (template-stable)
        -> MediaBreakPointIndicator#{x:Static atom:MediaBreakPointIndicator.MediaQueryIndicatorName} (internal-observable)
        -> Border#PART_TransparencyFallback (template-stable)
        -> Panel (template-stable)
        -> Border#WindowFullScreenFrame (template-stable)
        -> WindowVisualLayerClip (template-stable)
           -> VisualLayerManager#PART_VisualLayerManager (template-stable)
              -> Panel (template-stable)
                 -> ContentPresenter#ContentFrameLayer (internal-observable)
                 -> Border#ContentFrame (template-stable)
                    -> ContentPresenter#PART_ContentPresenter (template-stable)
        -> WindowResizer#PART_WindowResizer (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Window` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `FullscreenPopoverLayer` | control theme | `FullscreenPopoverLayerTheme.axaml` | Window | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PopoverBorder` | template node (Border) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowTitleBarLayoutPanel` | template node (WindowTitleBarLayoutPanel) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FullscreenLogoPresenter` | template node (ContentPresenter) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `FullscreenTitleText` | template node (TextBlock) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `FullscreenCaptionButtonGroup` | template node (StackPanel) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PopoverFullScreenButton` | template node (CaptionButton) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PopoverCloseButton` | template node (CaptionButton) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowDrawnDecorations` | control theme | `WindowDrawnDecorationsTheme.axaml` | Window | `DefaultTitleBarHeight`, `HasTitleBar`, `ShadowThickness`, `Title`, `TitleBarHeight` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowResizer` | control theme | `WindowResizerTheme.axaml` | Window | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_RootLayout` | template node (Panel) | `WindowResizerTheme.axaml` | WindowResizer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Window` | control theme | `WindowTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `Content`, `ContentFrameBackground`, `ContentFrameLayer`, `ContentFrameLayerOpacity`, `ContentFrameLayerTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `Panel` | template node (Panel) | `WindowTheme.axaml` | Window | `Background`, `Content`, `ContentFrameBackground`, `ContentFrameLayer`, `ContentFrameLayerOpacity`, `ContentFrameLayerTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `{x:Static atom:MediaBreakPointIndicator.MediaQueryIndicatorName}` | template node (MediaBreakPointIndicator) | `WindowTheme.axaml` | Window | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_TransparencyFallback` | template node (Border) | `WindowTheme.axaml` | Window | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowFrame` | template node (Border) | `WindowTheme.axaml` | Window | `Background`, `CornerRadius`, `FrameShadow`, `FrameShadowThickness`, `WindowFrameLayer`, `WindowFrameLayerOpacity` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowFrameLayer` | template node (ContentPresenter) | `WindowTheme.axaml` | Window | `CornerRadius`, `WindowFrameLayer`, `WindowFrameLayerOpacity`, `WindowFrameLayerTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_VisualLayerManager` | template node (VisualLayerManager) | `WindowTheme.axaml` | Window | `Content`, `ContentFrameBackground`, `ContentFrameLayer`, `ContentFrameLayerOpacity`, `ContentFrameLayerTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowContentClip` | template node (Border) | `WindowTheme.axaml` | Window | `Content`, `ContentFrameBackground`, `ContentFrameLayer`, `ContentFrameLayerOpacity`, `ContentFrameLayerTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DockPanel` | template node (DockPanel) | `WindowTheme.axaml` | Window | `Content`, `ContentFrameBackground`, `ContentFrameLayer`, `ContentFrameLayerOpacity`, `ContentFrameLayerTemplate`, `ContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TitleBarPanel` | template node (Panel) | `WindowTheme.axaml` | Window | `IsTitleBarVisible`, `TitleBar`, `TitleBarFrameBackground`, `TitleBarFrameLayer`, `TitleBarFrameLayerOpacity`, `TitleBarFrameLayerTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `TitleBarFrameLayer` | template node (ContentPresenter) | `WindowTheme.axaml` | Window | `TitleBarFrameBackground`, `TitleBarFrameLayer`, `TitleBarFrameLayerOpacity`, `TitleBarFrameLayerTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentPresenter` | template node (ContentPresenter) | `WindowTheme.axaml` | Window | `TitleBar` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentFrameLayer` | template node (ContentPresenter) | `WindowTheme.axaml` | Window | `ContentFrameBackground`, `ContentFrameLayer`, `ContentFrameLayerOpacity`, `ContentFrameLayerTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentFrame` | template node (Border) | `WindowTheme.axaml` | Window | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `Padding`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `WindowTheme.axaml` | Window | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FullscreenPopoverLayer` | template node (FullscreenPopoverLayer) | `WindowTheme.axaml` | Window | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WindowResizer` | template node (WindowResizer) | `WindowTheme.axaml` | Window | `FrameShadowThickness` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowFullScreenFrame` | template node (Border) | `WindowTheme.axaml` | Window | `Background` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ContentFrameBackground`、`ContentFrameLayer`、`ContentFrameLayerOpacity`、`ContentFrameLayerTemplate`、`IsTitleBarVisible`、`LogoTemplate`、`LeftAddOn`、`LeftAddOnTemplate`、`RightAddOn`、`RightAddOnTemplate`、`TitleBarFrameBackground`、`TitleBarFrameLayer`、`TitleBarFrameLayerOpacity`、`TitleBarFrameLayerTemplate` 等 | 定义控件展示内容、输入数据、模板或业务对象入口；`LeftAddOn` 和 `RightAddOn` 用于默认标题栏中的交互内容，`TitleBarFrameLayer` 仍只表示标题栏背景或装饰层。 |
| 选择与集合 | `ViewModel` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsCloseCaptionButtonVisible`、`IsFullScreenCaptionButtonVisible`、`IsMoveEnabled`、`IsPinCaptionButtonVisible` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 弹层与窗口 | `WindowFrameLayer`、`WindowFrameLayerOpacity` | 控制 popup、flyout、dialog、window 或 overlay 宿主协作。 |
| 其他稳定入口 | `Logo`、`LogoVisibility`、`MediaBreakPoint`、`OsType`、`OsVersion`、`TitleAlignment` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close。 |
| 主题语义 | ControlTheme、SharedToken、控件 Token 和模板绑定如何表达视觉。 | Window Token + ControlTheme。 |

## State Flow

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

## Theme and Token Boundaries

Window 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的控件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `FullscreenPopoverLayerTheme.axaml` | 定义 macOS 全屏标题栏 popover 的固定模板、caption buttons 和标题展示。 |
| `WindowDrawnDecorationsTheme.axaml` | 定义 Avalonia drawn decorations overlay 下的标题栏、内容、Dialog/Drawer host 和 visible frame 裁剪结构。 |
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

维护标题栏模板时，不应把 `TitleBarFrameLayer` 提升为可交互覆盖层。需要向默认标题栏加入按钮、菜单或搜索框时，使用 `LeftAddOn` 或 `RightAddOn`；只有需要替换整个标题栏组成或行为时，才在派生 `Window` 中重写标题栏创建与配置扩展点。`Window.TitleBar` 是模板生命周期拥有的 internal 状态，不作为应用 API 公开。

`Window.TitleAlignment` add-owner `WindowTitleBar.TitleAlignmentProperty`，并把配置单向投影给默认或派生
`WindowTitleBar`。`LeftAddOn`、`LeftAddOnTemplate`、`RightAddOn` 和 `RightAddOnTemplate` 同样 add-owner 对应标题栏属性，并以 `Template` 优先级单向投影。派生标题栏以 local value 提供的内置操作区优先于 Window facade。Window 只提供内容、平台、CSD、WindowState 和原生 chrome 安全区，不实现标题排列公式。

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

Token 边界：

Window Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `WindowToken`，scope id 为 `Window`，源码位于 `src/AtomUI.Desktop.Controls/Window/WindowToken.cs`。

## Customization Boundaries

维护 Window 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- `TitleBarFrameLayer` 是标题栏背景/装饰入口，不是标题栏用户交互入口；默认标题栏按钮、菜单、搜索框等交互内容必须通过 `LeftAddOn` 或 `RightAddOn` 承载。
- Windows、macOS 和 Linux 共用同一套首次显示主题表面流程；平台可见前必须同步准备 ThemeContext、variant 和 Window Token 背景，正式显示后由 `WindowTheme` 单独持有长期主题状态。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Window 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- `TitleBarFrameLayer` 的背景/装饰层语义，以及标题栏交互内容必须通过 `TitleBar` 承载的职责边界。
- 上层 Dialog/Drawer 不按 OS 或 CSD 状态复制 Window frame 几何，而是消费 Window 发布的 `FrameShadowThickness` 和实际 drawn host 能力。
- 所有桌面平台共用 Window 首次显示主题表面准备流程，`WindowTheme` 是显示完成后的唯一长期背景所有者。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
