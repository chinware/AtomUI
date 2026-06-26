# Window 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

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
                        <ContentPresenter Name="PART_ContentPresenter" />
                    </Border>
                </Panel>
            </DockPanel>
        </Border>
    </VisualLayerManager>
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
           -> Panel (template-stable)
              -> StackPanel (template-stable)
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
        -> FullscreenPopoverLayer#PART_FullscreenPopoverLayer (template-stable)
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
        -> VisualLayerManager#PART_VisualLayerManager (template-stable)
           -> Panel (template-stable)
              -> ContentPresenter#ContentFrameLayer (internal-observable)
              -> Border#ContentFrame (template-stable)
                 -> ContentPresenter#PART_ContentPresenter (template-stable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Window` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `FullscreenPopoverLayer` | control theme | `FullscreenPopoverLayerTheme.axaml` | Window | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PopoverBorder` | template node (Border) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `FullscreenPopoverLayerTheme.axaml` | FullscreenPopoverLayer | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
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
| `ContentPresenter` | template node (ContentPresenter) | `WindowTheme.axaml` | Window | `TitleBar`, `TitleBarOffsetMargin` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentFrameLayer` | template node (ContentPresenter) | `WindowTheme.axaml` | Window | `ContentFrameBackground`, `ContentFrameLayer`, `ContentFrameLayerOpacity`, `ContentFrameLayerTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ContentFrame` | template node (Border) | `WindowTheme.axaml` | Window | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `Padding`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ContentPresenter` | template node (ContentPresenter) | `WindowTheme.axaml` | Window | `Content`, `ContentTemplate`, `HorizontalContentAlignment`, `VerticalContentAlignment` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FullscreenPopoverLayer` | template node (FullscreenPopoverLayer) | `WindowTheme.axaml` | Window | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_WindowResizer` | template node (WindowResizer) | `WindowTheme.axaml` | Window | `FrameShadowThickness` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `WindowFullScreenFrame` | template node (Border) | `WindowTheme.axaml` | Window | `Background` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `ContentFrameBackground`、`ContentFrameLayer`、`ContentFrameLayerOpacity`、`ContentFrameLayerTemplate`、`IsTitleBarVisible`、`LogoTemplate`、`TitleBarFrameBackground`、`TitleBarFrameLayer`、`TitleBarFrameLayerOpacity`、`TitleBarFrameLayerTemplate` 等 11 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `ViewModel` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsCloseCaptionButtonVisible`、`IsFullScreenCaptionButtonVisible`、`IsMoveEnabled`、`IsPinCaptionButtonVisible` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 弹层与窗口 | `WindowFrameLayer`、`WindowFrameLayerOpacity` | 控制 popup、flyout、dialog、window 或 overlay 宿主协作。 |
| 其他稳定入口 | `Logo`、`LogoVisibility`、`MediaBreakPoint`、`OsType`、`OsVersion` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | open/close。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | Window Token + ControlTheme。 |

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

## Theme and Token Boundaries

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

Token 边界：

Window Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `WindowToken`，scope id 为 `Window`，源码位于 `src/AtomUI.Desktop.Controls/Window/WindowToken.cs`。

## Customization Boundaries

维护 Window 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 Window 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
