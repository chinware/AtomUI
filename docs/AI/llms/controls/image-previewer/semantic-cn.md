# ImagePreviewer 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、Gallery API / Token 表、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `ImagePreviewer` | 数据展示控件根语义区域，承载 public API、数据状态和主题入口。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `item` | `条目或容器区域` | 承载集合项、单元格、标签、时间节点、卡片或展示单元。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `header` | `标题或头部区域` | 承载标题、字段名、列头、操作入口或摘要信息。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `content` | `内容区域` | 承载主体内容、媒体、文本、空状态、加载状态或详情区域。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |
| `motion` | `动效或浮层区域` | 表达展开收起、轮播、tooltip、tour、预览或虚拟化反馈。 | 见 API 与契约模型 | 见视觉与主题模型 | stable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerTheme.axaml`

```xml
<Border>
    <ImagePreviewerCover />
</Border>
```

## Composition Model

该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。

### 控件角色图

```text
ImagePreviewer
  -> ImagePreviewFloatToolbar (control theme, ImagePreviewFloatToolbarTheme.axaml)
     -> StackPanel#RootLayout (template-stable)
        -> Border#IndicatorFrame (template-stable)
           -> TextBlock (template-stable)
        -> Border#ActionFrame (template-stable)
           -> StackPanel (template-stable)
              -> IconButton#PART_ScaleDownButton (template-stable)
              -> IconButton#PART_ScaleUpButton (template-stable)
              -> ToggleIconButton#PART_FitToWindowButton (template-stable)
              -> IconButton#PART_HorizontalFlipButton (template-stable)
              -> IconButton#PART_VerticalFlipButton (template-stable)
              -> IconButton#PART_RotateLeftButton (template-stable)
              -> IconButton#PART_RotateRightButton (template-stable)
  -> ImagePreviewNavButton (control theme, ImagePreviewNavButtonTheme.axaml)
  -> ImagePreviewToolbar (control theme, ImagePreviewToolbarTheme.axaml)
     -> StackPanel#RootLayout (template-stable)
        -> IconButton#PART_PreviousButton (template-stable)
        -> IconButton#PART_NextButton (template-stable)
        -> IconButton#PART_ScaleDownButton (template-stable)
        -> IconButton#PART_ScaleUpButton (template-stable)
        -> ToggleIconButton#PART_FitToWindowButton (template-stable)
        -> IconButton#PART_HorizontalFlipButton (template-stable)
        -> IconButton#PART_VerticalFlipButton (template-stable)
        -> IconButton#PART_RotateLeftButton (template-stable)
        -> IconButton#PART_RotateRightButton (template-stable)
  -> ImagePreviewerCover (control theme, ImagePreviewerCoverTheme.axaml)
     -> Border (template-stable)
        -> Panel (template-stable)
           -> ImagePreviewRenderer (internal-observable)
           -> Border#Mask (template-stable)
              -> ContentPresenter#MaskContentPresenter (internal-observable)
  -> ImagePreviewerDialog (control theme, ImagePreviewerDialogTheme.axaml)
  -> ImagePreviewer (control theme, ImagePreviewerTheme.axaml)
     -> Border (template-stable)
        -> ImagePreviewerCover (internal-observable)
  -> ImagePreviewerOverlayHost (control theme, ImagePreviewerThemes.axaml)
     -> Panel (template-stable)
        -> ContentPresenter (internal-observable)
        -> IconButton#PART_CloseButton (template-stable)
  -> ImagePreviewerTitleBar (control theme, ImagePreviewerTitleBarTheme.axaml)
  -> ImageViewer (control theme, ImageViewerTheme.axaml)
     -> Panel (template-stable)
        -> Canvas#PART_ImageViewerScene (template-stable)
           -> ImagePreviewRenderer#PART_ImageRenderer (template-stable)
        -> ImagePreviewNavButton#PART_PreviousButton (template-stable)
        -> ImagePreviewNavButton#PART_NextButton (template-stable)
        -> ImagePreviewFloatToolbar (internal-observable)
        -> MediaBreakPointIndicator#{x:Static atom:MediaBreakPointIndicator.MediaQueryIndicatorName} (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `ImagePreviewer` | public control | `源文档 + public API` | 用户代码 / 控件宿主 | public API | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ImagePreviewFloatToolbar` | control theme | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewer | `Background`, `CornerRadius`, `IndicatorText`, `IsImageFitToWindow`, `IsMultiImages`, `IsScaleDownEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (StackPanel) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `Background`, `CornerRadius`, `IndicatorText`, `IsImageFitToWindow`, `IsMultiImages`, `IsScaleDownEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `IndicatorFrame` | template node (Border) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `Background`, `CornerRadius`, `IndicatorText`, `IsMultiImages` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ActionFrame` | template node (Border) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `Background`, `CornerRadius`, `IsImageFitToWindow`, `IsScaleDownEnabled`, `IsScaleUpEnabled`, `Padding` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `StackPanel` | template node (StackPanel) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `IsImageFitToWindow`, `IsScaleDownEnabled`, `IsScaleUpEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScaleDownButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `IsScaleDownEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScaleUpButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `IsScaleUpEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FitToWindowButton` | template node (ToggleIconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | `IsImageFitToWindow` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HorizontalFlipButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_VerticalFlipButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RotateLeftButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RotateRightButton` | template node (IconButton) | `ImagePreviewFloatToolbarTheme.axaml` | ImagePreviewFloatToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ImagePreviewNavButton` | control theme | `ImagePreviewNavButtonTheme.axaml` | ImagePreviewer | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ImagePreviewToolbar` | control theme | `ImagePreviewToolbarTheme.axaml` | ImagePreviewer | `IsFirstImage`, `IsImageFitToWindow`, `IsLastImage`, `IsMultiImages`, `IsScaleDownEnabled`, `IsScaleUpEnabled` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `RootLayout` | template node (StackPanel) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsFirstImage`, `IsImageFitToWindow`, `IsLastImage`, `IsMultiImages`, `IsScaleDownEnabled`, `IsScaleUpEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_PreviousButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsFirstImage`, `IsMultiImages` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_NextButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsLastImage`, `IsMultiImages` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScaleDownButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsScaleDownEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_ScaleUpButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsScaleUpEnabled` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_FitToWindowButton` | template node (ToggleIconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | `IsImageFitToWindow` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_HorizontalFlipButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_VerticalFlipButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RotateLeftButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_RotateRightButton` | template node (IconButton) | `ImagePreviewToolbarTheme.axaml` | ImagePreviewToolbar | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ImagePreviewerCover` | control theme | `ImagePreviewerCoverTheme.axaml` | ImagePreviewer | `Background`, `BorderBrush`, `BorderThickness`, `Content`, `ContentTemplate`, `CornerRadius` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate`, `ImageSource`, `IsShowCoverMask`, `MaskOpacity` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Mask` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate`, `IsShowCoverMask`, `MaskOpacity` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `MaskContentPresenter` | template node (ContentPresenter) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ImagePreviewerDialog` | control theme | `ImagePreviewerDialogTheme.axaml` | ImagePreviewer | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ImagePreviewer` | control theme | `ImagePreviewerTheme.axaml` | 用户代码 / 控件宿主 | `Background`, `BorderBrush`, `BorderThickness`, `CornerRadius`, `CoverIndicatorContent`, `CoverIndicatorContentTemplate` | public | 用户可直接使用 public 控件；可作为示例和 API 入口。 |
| `ImagePreviewerOverlayHost` | control theme | `ImagePreviewerThemes.axaml` | ImagePreviewer | `Content` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `Panel` | template node (Panel) | `ImagePreviewerThemes.axaml` | ImagePreviewerOverlayHost | `Content` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `ImagePreviewerThemes.axaml` | ImagePreviewerOverlayHost | `Content` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_CloseButton` | template node (IconButton) | `ImagePreviewerThemes.axaml` | ImagePreviewerOverlayHost | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ImagePreviewerTitleBar` | control theme | `ImagePreviewerTitleBarTheme.axaml` | ImagePreviewer | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 内容与数据 | `CoverImageSrc`、`CoverIndicatorContent`、`CoverIndicatorContentTemplate`、`FallbackImageSrc`、`ImageMaxScale`、`ImageMinScale`、`ImageScaleStep`、`ImageSource`、`ImageTranslateX`、`ImageTranslateY` 等 14 项 | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `Count`、`CurrentIndex` | 维护选择、展开、过滤、分页、分组或集合状态。 |
| 交互与状态 | `IsDialogModal`、`IsDialogTopmost`、`IsModal`、`IsMotionEnabled`、`IsOpen`、`IsShowCoverMask` | 表达用户可观察状态、可用性、清除、加载或反馈语义。 |
| 视觉与布局 | `CoverHeight`、`CoverWidth` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `MaxScale`、`MinScale`、`ScaleStep`、`Stretch`、`Transform` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | selection/checked/active、open/close、collection/filter、input/value、motion。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | ImagePreviewer Token + ControlTheme。 |

## State Flow

ImagePreviewer 的状态流按以下路径收敛：

```text
Public API / inherited command / item source / user input
  -> 控件实例状态
  -> effective state / pseudo-class / template property
  -> ControlTheme selector / presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- selection/checked/active、open/close、collection/filter、input/value、motion 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放。

## Theme and Token Boundaries

ImagePreviewer 的视觉模型由控件模板、ControlTheme、SharedToken 和必要的组件 Token 共同构成。

| 主题文件 | 职责 |
| --- | --- |
| `ImageGroupPreviewerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewFloatToolbarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewNavButtonTheme.axaml` | 定义局部操作入口、按钮或 handle 的状态视觉。 |
| `ImagePreviewToolbarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewerCoverTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewerDialogTheme.axaml` | 定义弹层、窗口或 overlay 宿主视觉。 |
| `ImagePreviewerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImagePreviewerThemes.axaml` | 聚合控件家族主题资源，保证包级引入顺序稳定。 |
| `ImagePreviewerTitleBarTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |
| `ImageViewerTheme.axaml` | 提供控件模板、selector、资源绑定和状态视觉。 |

ImagePreviewer 使用 `ImagePreviewerToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 selection/checked/active、open/close、collection/filter、input/value、motion 运行时状态。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、selected、expanded、loading、filter、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

ImagePreviewer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ImagePreviewerToken`，scope id 为 `ImagePreviewer`，源码位于 `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs`。

## Customization Boundaries

维护 ImagePreviewer 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 ImagePreviewer 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 文档、Gallery API 表、Token 表与源码契约的一致性。
