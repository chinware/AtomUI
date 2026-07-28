# ImagePreviewer 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `ImagePreviewer` / `ImageGroupPreviewer` | 图片预览控件根语义区域，承载 public API、图片来源、当前项、封面索引和主题入口。 | `Source`、`Sources`、`FallbackSource`、`CurrentIndex`、`CoverIndex`、`IsOpen` | `ImagePreviewerToken` | stable |
| `cover` | `ImagePreviewerCover` | 普通页面中的封面展示区域，承载 `CoverIndex` 对应图片、mask、loading 和 error 内容。 | `CoverIndex`、`CoverIndicatorContent`、`CoverWidth`、`CoverHeight`、`IsShowCoverMask`、`LoadingContent`、`ErrorContent` | `MaskBgColor`、`CoverImageWidth` | stable |
| `viewer` | `ImageViewer` / `PART_ImageViewerScene` / `PART_ImageRenderer` | 预览宿主中的图片场景和渲染区域，承载缩放、旋转、翻转和拖拽坐标空间。 | `ImageScaleStep`、`ImageMinScale`、`ImageMaxScale`、`Stretch`、`Transform` | `DialogMinWidth`、`DialogMinHeight` | stable |
| `title` | `ImagePreviewerTitleBar` / `PART_TitleLayout` / `PART_IconPresenter` | 预览窗口标题区域，承载 effective title 和显式标题图标。 | `PreviewTitle`、`PreviewTitleIcon`、`PreviewTitleResolver` | `TitleBarBackgroundColor`、`WindowTitleBarToken.LogoAndTitleSpacing` | template-stable |
| `toolbar` | `ImagePreviewToolbar` / `ImagePreviewFloatToolbar` | 预览操作区域，承载上一张、下一张、缩放、fit-to-window、翻转和旋转动作。 | toolbar request events、`CurrentIndex`、`Count` | `ToolbarBoxShadow`、`ToolbarBgColor` | stable |
| `host` | `ImagePreviewerDialog` / `ImagePreviewerOverlayHost` | 预览宿主区域，承载窗口化或 overlay 打开、关闭、modal、topmost 和释放语义。 | `IsOpen`、`IsDialogModal`、`IsDialogTopmost`、`IsModal` | `DialogMinWidth`、`DialogMinHeight` | internal-observable |

## Abstract AXAML Structure

来源：`src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerTheme.axaml`

```xml
<PixelAlignedBorder>
    <ImagePreviewerCover />
</PixelAlignedBorder>
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
     -> PixelAlignedBorder (template-stable)
        -> Panel (template-stable)
           -> ImagePreviewRenderer (internal-observable)
           -> Border#PART_LoadingPresenter (template-stable)
              -> Panel (template-stable)
                 -> SkeletonImage (template-stable)
                 -> ContentPresenter (internal-observable)
           -> Border#PART_ErrorPresenter (template-stable)
              -> Panel (template-stable)
                 -> StackPanel#DefaultErrorLayout (template-stable)
                    -> PictureOutlined#DefaultErrorIcon (template-stable)
                    -> TextBlock#DefaultErrorText (template-stable)
                 -> ContentPresenter (internal-observable)
           -> Border#Mask (template-stable)
              -> ContentPresenter#MaskContentPresenter (internal-observable)
  -> ImagePreviewerDialog (control theme, ImagePreviewerDialogTheme.axaml)
  -> ImagePreviewer (control theme, ImagePreviewerTheme.axaml)
     -> PixelAlignedBorder (template-stable)
        -> ImagePreviewerCover (internal-observable)
  -> ImagePreviewerOverlayHost (control theme, ImagePreviewerThemes.axaml)
     -> Panel (template-stable)
        -> ContentPresenter (internal-observable)
        -> IconButton#PART_CloseButton (template-stable)
  -> ImagePreviewerTitleBar (control theme, ImagePreviewerTitleBarTheme.axaml)
     -> Border#Frame (template-stable)
        -> WindowTitleBarLayoutPanel (template-stable)
           -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel#PART_TitleLayout (template-stable)
              -> IconPresenter#PART_IconPresenter (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
     -> Border#Frame (template-stable)
        -> WindowTitleBarLayoutPanel (template-stable)
           -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel#PART_TitleLayout (template-stable)
              -> IconPresenter#PART_IconPresenter (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
     -> Border#Frame (template-stable)
        -> WindowTitleBarLayoutPanel (template-stable)
           -> ContentPresenter#PART_LeftAddOn (template-stable)
           -> DockPanel#PART_TitleLayout (template-stable)
              -> IconPresenter#PART_IconPresenter (template-stable)
              -> ContentPresenter#PART_ContentPresenter (template-stable)
           -> StackPanel (template-stable)
              -> ContentPresenter#PART_RightAddOn (template-stable)
              -> CaptionButtonGroup#PART_CaptionButtonGroup (template-stable)
  -> ImageViewer (control theme, ImageViewerTheme.axaml)
     -> Panel (template-stable)
        -> Canvas#PART_ImageViewerScene (template-stable)
           -> ImagePreviewRenderer#PART_ImageRenderer (template-stable)
        -> Border#PART_LoadingPresenter (template-stable)
           -> Panel (template-stable)
              -> Spin (template-stable)
              -> ContentPresenter (internal-observable)
        -> Border#PART_ErrorPresenter (template-stable)
           -> Panel (template-stable)
              -> StackPanel#DefaultErrorLayout (template-stable)
                 -> PictureOutlined#DefaultErrorIcon (template-stable)
                 -> TextBlock#DefaultErrorText (template-stable)
              -> ContentPresenter (internal-observable)
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
| `Panel` | template node (Panel) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate`, `ErrorContent`, `ErrorContentTemplate`, `ImageSource`, `IsCoverMaskVisible` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingPresenter` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `IsLoading`, `LoadingContent`, `LoadingContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `LoadingContent`, `LoadingContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ErrorPresenter` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `ErrorContent`, `ErrorContentTemplate`, `IsFailed` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DefaultErrorLayout` | template node (StackPanel) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `ErrorContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DefaultErrorIcon` | template node (PictureOutlined) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DefaultErrorText` | template node (TextBlock) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Mask` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate`, `IsCoverMaskVisible`, `MaskOpacity` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `MaskContentPresenter` | template node (ContentPresenter) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `ImagePreviewerDialog` | control theme | `ImagePreviewerDialogTheme.axaml` | ImagePreviewer | 主题状态 / visual state | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| 契约组 | 代表成员 | 维护含义 |
| --- | --- | --- |
| 图片来源 | `Source`、`Sources`、`FallbackSource`、`IImagePreviewSource` | 统一表达单图、多图和失败兜底图片来源。`IImagePreviewSource` 是唯一来源契约，URI 场景通过 `UriImagePreviewSource` 显式进入来源集合，数据流场景通过 `StreamImagePreviewSource` 按需打开。 |
| 内容与数据 | `CoverIndicatorContent`、`CoverIndicatorContentTemplate`、`LoadingContent`、`LoadingContentTemplate`、`ErrorContent`、`ErrorContentTemplate`、`ImageMaxScale`、`ImageMinScale`、`ImageScaleStep`、`ImageTranslateX`、`ImageTranslateY` | 定义控件展示内容、输入数据、模板或业务对象入口。 |
| 选择与集合 | `Count`、`CurrentIndex` | 维护当前预览项、多图切换和集合状态；`CurrentIndex` 是控件级当前项索引，默认双向绑定，只决定打开预览后的当前图片。 |
| 封面展示 | `CoverIndex` | 只决定关闭态 `ImagePreviewer` 封面显示哪一张来源图片。它不参与打开行为、导航行为或 `CurrentIndex` 同步。 |
| 加载调度 | `MaxConcurrentLoads`、`PreloadCount` | 控制图片加载并发和打开预览后的邻近图片预加载窗口，避免大集合一次性加载全部图片。 |
| 预览标题 | `PreviewTitle`、`PreviewTitleIcon`、`PreviewTitleResolver`、`IImagePreviewTitleResolver`、`ImagePreviewTitleResolveContext` | 定义预览宿主标题和标题图标契约。显式标题非空时优先显示；显式标题为空时由 resolver 基于 current effective item 解析标题；`PreviewTitleIcon` 使用 `PathIcon?`，只在显式设置时显示，不继承应用或主窗口图标。 |
| 交互与状态 | `IsDialogModal`、`IsDialogTopmost`、`IsModal`、`IsMotionEnabled`、`IsOpen`、`IsShowCoverMask` | 表达用户可观察状态、可用性、清除、加载或反馈语义；`IsOpen` 默认双向绑定。 |
| 视觉与布局 | `CoverHeight`、`CoverWidth` | 影响尺寸、位置、颜色、形状、密度和模板视觉变量。 |
| 其他稳定入口 | `MaxScale`、`MinScale`、`ScaleStep`、`Stretch`、`Transform` | 保留为 public surface，变更前需确认 Gallery 和用户 XAML 依赖。 |

## Pseudo Classes

| 状态反馈 | public API、内部状态和伪类如何形成用户可感知反馈。 | current item、open/close、image loading、loaded/failed、fallback、motion。 |
| 主题语义 | ControlTheme、SharedToken、组件 Token 和模板绑定如何表达视觉。 | ImagePreviewer Token + ControlTheme。 |

## State Flow

ImagePreviewer 的状态流按以下路径收敛：

```text
Public API / IImagePreviewSource / ImageSourceUri / inherited command / user input
  -> 控件实例状态
  -> ImagePreviewItem state / effective state / pseudo-class / template property
  -> ControlTheme selector / loading presenter / error presenter / renderer
  -> Gallery 可观察行为
```

状态维护规则：

- Disabled 或不可交互状态优先屏蔽 pointer、keyboard、motion 和提交类反馈。
- `IImagePreviewSource` 是用户输入层，`ImagePreviewItem` 是控件内部图片项状态 owner，`LoadedImageSource` 是加载完成结果。三者不能混用职责。
- 图片项状态按 `Pending -> Loading -> Loaded/Failed` 收敛。单项加载失败只影响该项自身；`FallbackSource` 只在当前来源集合全部失败时作为整组兜底。
- current item、open/close、image loading、loaded/failed、fallback、motion 状态由控件实例或明确的数据 owner 推导，不能在 template part 之间双向竞争。
- `IsOpen` 与 `CurrentIndex` 是默认 `TwoWay` 的受控状态；dialog 和 overlay 只能消费或回写这条 public 状态路径，不能保留独立打开状态或当前项状态。
- `CurrentIndex` 是弹出 dialog 和 overlay host 共享的当前项状态。普通封面只消费 `CoverIndex`，不得反向改写 `CurrentIndex`。
- `CoverIndex` 从当前来源集合中选择关闭态封面。显示层可以对越界 `CoverIndex` 做有效范围 clamp 以稳定渲染，但不能静默修改用户设置的 public 属性值。
- 预览标题由单一 effective title 算法生成：非空白显式标题优先；显式标题为空时，使用 resolver 基于 current effective item 解析标题；解析不到标题时标题区域保持空态。
- 模板重套用时必须把 public API 对应状态回放到新的 part、伪类和主题变量。
- 集合、弹层、异步、动效或窗口相关状态必须能处理 reset、close、cancel、detach 和 owner 释放；过期异步加载结果不能回写新来源。
- 关闭态只加载封面所需图片；打开态加载当前图片、`PreloadCount` 定义的邻近图片，并保留或补加载 `CoverIndex` 对应封面，保证非模态预览切换时页面封面不消失。控件不得因为 `Sources` 包含大量来源而一次性加载全部图片。

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

ImagePreviewer 使用 `ImagePreviewerToken` 作为组件 Token scope。Token 只表达组件视觉语义，不承载 current item、open/close、image loading、loaded/failed、fallback 或 motion 运行时状态。

预览窗口标题栏使用 `ImagePreviewer.PreviewTitleIcon` 作为标题图标来源。`PreviewTitleIcon` 是 `PathIcon?` 契约，表示只属于 ImagePreviewer 预览窗口标题的显式图标；未设置时标题栏不显示图标，也不从 `Window.Icon`、`Window.Logo`、应用图标或主窗口图标回退。`ImagePreviewerTitleBarTheme` 将 `PART_IconPresenter` 和标题内容放入共享布局的 Title 角色，图片工具栏放入 Leading，右侧 add-on 与 caption buttons 放入 Trailing。图标位于标题左侧，仅当图标和标题都有效时使用 `WindowTitleBarToken.LogoAndTitleSpacing`。预览 dialog 的 `TitleAlignment` 默认值覆盖为 `WindowCenter`，因此 Windows、Linux 和 macOS 都默认以完整窗口水平中心作为标题基准；显式设置 `TitleAlignment` 时仍通过 `ImagePreviewerTitleBar` 投射，并继续由 `WindowTitleBarLayoutPanel` 处理左右安全区裁剪。三个平台模板都通过 `NativeChromeInsets` 避让实际与客户区重叠的原生按钮，不使用外层单侧 Padding/Margin 缩窄完整 frame。

预览 dialog 的图片查看层必须绘制与 dialog `Background` 一致的实心背景，用于覆盖 Windows CSD maximize/restore 期间可能短暂暴露的通用窗口 underlay 背景。窗口状态或尺寸变化期间，dialog 还会短暂关闭查看层的图片 transform/translate 过渡，避免外层窗口 resize 与内层图片居中动画叠加成闪动；overlay host 的查看层保持透明，由 overlay 模板自身背景承载遮罩语义。

加载视觉遵循以下规则：

- 封面加载态使用图片 Skeleton 占位，保持封面尺寸稳定，不显示 hover mask。
- 预览层加载态使用居中 Spin，缩放、旋转、拖拽和 fit-to-window 在当前图片未加载完成前禁用。
- loading 视觉允许短暂延迟显示以避免本地文件或 `avares://` 资源快速完成造成闪烁；延迟只影响视觉，不影响 `ImagePreviewItemState`。
- `LoadingContent` / `LoadingContentTemplate` 替换默认加载内容，`ErrorContent` / `ErrorContentTemplate` 替换默认失败内容；替换内容不得重新定义 `Pending -> Loading -> Loaded/Failed` 状态机。
- 封面 loading 和 failed 状态必须使用稳定占位尺寸。尺寸解析优先使用显式 `CoverWidth` / `CoverHeight`，其次使用控件布局约束中的有效宽高，最后使用 `ImagePreviewerToken.CoverImageWidth` 作为兜底基准。没有图片自然尺寸时，失败态不能由错误文案撑开成窄条。
- 默认失败态使用图片失败占位视觉：图标、简短本地化文案和低干扰背景共同表达失败。失败文案来自 ImagePreviewer 控件语言资源，主题中不得硬编码英文 `Image load failed`。

主题维护规则：

- 不删除或重命名已经稳定的 ControlTheme key、template part、伪类和资源 key。
- 不把可由 AXAML 表达的模板状态迁移为 C# 动态创建视觉。
- 不把 hover、pressed、current item、loading、failed、fallback、popup open 等运行时状态写入 Token。
- Browser 或平台特化主题必须保持同一 API 的语义一致。

Token 边界：

ImagePreviewer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ImagePreviewerToken`，scope id 为 `ImagePreviewer`，源码位于 `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs`。

## Customization Boundaries

维护 ImagePreviewer 时必须保持以下不变量：

- 不擅自新增、删除、重命名或改变已批准的 public/protected API、Avalonia 属性、事件和默认值。
- 不破坏 template part、伪类、ControlTheme key、Token 名称和资源 key。
- 不改变 Gallery 已展示的 XAML 用法、默认外观、交互顺序和状态优先级。
- Template part 重新应用、集合替换、弹层关闭、窗口失活和控件 detach 时必须释放旧订阅和资源宿主。
- 图片加载不得在 UI 线程执行网络 I/O，不得通过同步 `Stream` API 承载远程来源。
- `Source` / `Sources` / `FallbackSource`、URI 便利入口和数据流来源的解析、来源身份解析和取消语义必须一致。
- `CurrentIndex` 不得退化为弹层专属状态；dialog 和 overlay 必须消费同一当前项语义。封面由 `CoverIndex` 独立决定，只负责关闭态展示，不参与打开后的当前项行为。
- `MaxConcurrentLoads` 必须限制所有图片加载入口的实际并发，不能只限制预加载路径。
- `PreloadCount` 只扩大打开态当前图片附近的加载窗口，不改变 `CurrentIndex`、`CoverIndex`、`Count` 或导航语义。
- 预览标题不得使用过期图片项：标题必须随 current effective item、effective items、显式标题和 resolver 变化重新计算；非空白显式标题的优先级不能被默认 resolver 覆盖。
- 不通过隐藏延迟、强制刷新或吞异常掩盖状态同步问题。
- 不引入运行时反射扫描作为 API、Token 或数据路径发现机制。
- 文档只描述当前稳定设计；历史变化记录在 `changelog.md`。

维护不变量：

维护 ImagePreviewer 时不得破坏：

- Public API、默认值、事件顺序和 Gallery 可观察行为。
- Template part 名称、ControlTheme key、伪类和资源 key。
- 旧 template part、事件订阅、Popup/Flyout/Window host 和 collection view 的释放路径。
- 图片加载状态的单 owner：`IImagePreviewSource` 是来源，`ImagePreviewItem` 是状态，`LoadedImageSource` 是结果，`ImagePreviewRenderer` 只负责渲染。
- `Source`、`Sources`、`FallbackSource`、URI 便利入口和数据流来源的加载、取消、来源身份和失败处理一致性。
- 单图 `ImagePreviewer` 与 `ImageGroupPreviewer` 对 `CoverWidth` / `CoverHeight` 的消费一致性。远程图片 loading/failed 时不能因为没有图片自然尺寸而丢失封面高度。
- 默认 loading/error 视觉不能破坏自定义内容入口。用户设置 `LoadingContent` 或 `ErrorContent` 后，模板仍负责稳定尺寸、状态显隐和 mask 抑制。
- 默认失败文案必须走控件本地化资源，不允许在 `ImagePreviewerCoverTheme.axaml` 或 `ImageViewerTheme.axaml` 中硬编码英文字符串。
- `CurrentIndex` 的单一语义：dialog 和 overlay 使用同一 current effective item；普通封面只使用 `CoverIndex`。点击封面、封面加载状态和封面错误状态不得修改 `CurrentIndex`。
- `CoverIndex` 的单一语义：它只选择关闭态封面图片，不参与打开、导航、标题解析、fallback 批次判定或 selection 同步。
- `MaxConcurrentLoads` 的单一闸门语义：任何新增加载入口都必须接入同一调度器，不能形成第二套并发队列。
- `PreloadCount` 的窗口语义：只影响打开态邻近加载范围，不改变 public selection、封面索引或 `Count`。
- 预览标题的单一算法：非空白 `Window.Title` 或 `PreviewTitle` 优先，resolver 只在显式标题为空时运行；标题必须跟随 current effective item，不能保留旧集合项的文件名。
- 预览标题图标的单一路径：`PreviewTitleIcon` 只能进入 `ImagePreviewerDialog.TitleIcon`，再绑定到 `ImagePreviewerTitleBar.Icon` 和 `PART_IconPresenter`；不得转接 `Window.Icon`、`Window.Logo` 或右侧扩展区域来表达标题图标。
- 加载完成后必须重新计算 `ImageViewer` 的布局、居中、fit-to-window 和交互边界。
- Light/Dark、Browser/Desktop 和不同 SizeType 下的主题一致性。
- 控件文档、源码 public surface、Token 类型或生成数据与源码契约的一致性。
