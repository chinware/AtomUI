# ImagePreviewer 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `AbstractImagePreviewer` | 归一 ItemsSource、current、open state、宿主与加载策略。 | `ItemsSource`、`CurrentIndex`、`IsOpen`、`PreloadCount` | SharedToken、ImagePreviewerToken | public |
| `cover` | `ImagePreviewer` / `PART_CoverItemsControl` | 展示单封面或 group 缩略图及其 loading/error 状态。 | `CoverIndex`、cover size/state、`ItemsPanel` | Cover size、mask、radius Token | public/template-stable |
| `host` | `ImagePreviewerDialog` / `ImagePreviewerOverlayHost` | 承载 Desktop window 或 Browser overlay。 | dialog、modal、topmost、title、motion API | Window、overlay、motion Token | internal-observable |
| `viewer` | `ImageViewer` / `PART_ImageViewerScene` | 当前项导航、fit、拖拽、缩放和旋转。 | interaction、scale、CurrentIndex | Viewer background、toolbar Token | internal-observable |
| `renderer` | `PART_ImageRenderer` | 只渲染 entry 已持有的 `IImage`。 | current load state | 无独立加载 Token | template-stable |
| `loading` | `PART_LoadingPresenter` | 呈现 Skeleton 或 Spin，不拥有请求。 | LoadingContent/Template | Loading、Skeleton、Spin Token | template-stable |
| `error` | `PART_ErrorPresenter` | 呈现最终失败内容，不改变状态机。 | ErrorContent/Template | Error semantic Token | template-stable |

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
  -> ImagePreviewerOverlayHost (control theme, ImagePreviewerOverlayHostTheme.axaml)
     -> Panel (template-stable)
        -> ContentPresenter (internal-observable)
        -> IconButton#PART_CloseButton (template-stable)
  -> ImagePreviewer (control theme, ImagePreviewerTheme.axaml)
     -> PixelAlignedBorder (template-stable)
        -> ImagePreviewerCover (internal-observable)
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

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `AbstractImagePreviewer` | 归一 ItemsSource、current、open state、宿主与加载策略。 | `ItemsSource`、`CurrentIndex`、`IsOpen`、`PreloadCount` | SharedToken、ImagePreviewerToken | public |
| `cover` | `ImagePreviewer` / `PART_CoverItemsControl` | 展示单封面或 group 缩略图及其 loading/error 状态。 | `CoverIndex`、cover size/state、`ItemsPanel` | Cover size、mask、radius Token | public/template-stable |
| `host` | `ImagePreviewerDialog` / `ImagePreviewerOverlayHost` | 承载 Desktop window 或 Browser overlay。 | dialog、modal、topmost、title、motion API | Window、overlay、motion Token | internal-observable |
| `viewer` | `ImageViewer` / `PART_ImageViewerScene` | 当前项导航、fit、拖拽、缩放和旋转。 | interaction、scale、CurrentIndex | Viewer background、toolbar Token | internal-observable |
| `renderer` | `PART_ImageRenderer` | 只渲染 entry 已持有的 `IImage`。 | current load state | 无独立加载 Token | template-stable |
| `loading` | `PART_LoadingPresenter` | 呈现 Skeleton 或 Spin，不拥有请求。 | LoadingContent/Template | Loading、Skeleton、Spin Token | template-stable |
| `error` | `PART_ErrorPresenter` | 呈现最终失败内容，不改变状态机。 | ErrorContent/Template | Error semantic Token | template-stable |

## Pseudo Classes

源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。

## State Flow

### 4.1 集合与状态所有权

`ItemsSource` 每次物化为控件内部的 `ImagePreviewEntry` 集合。entry 持有两个独立通道：

| 通道 | 内容 | 状态与租约 |
| --- | --- | --- |
| Full | 预览完整图 | FullState/Error/Progress、generation、CTS、`ImageLoadResult` lease |
| Thumbnail | 页面封面 | ThumbnailState/Error/Progress、generation、CTS、独立 lease |

集合必须支持普通 enumerable replacement，以及 `INotifyCollectionChanged` 的 Add、Remove、Move、Replace 和 Reset。未变的
immutable item 复用 entry 与已有租约；被移除或替换的 entry 立即取消并 dispose。控件 detach 期间解除集合订阅，reattach 时
重新物化当前集合，补齐离线变更。

当前项和封面都按显示索引 clamp。Clamp 只用于选择 entry，不能把外部 TwoWay `CurrentIndex` 写回为边界值。

### 4.2 加载优先级与懒加载

Previewer 使用应用级调度预算，优先级固定为：

| 场景 | `ImageRequestPriority` |
| --- | --- |
| 当前完整图 | `Critical` |
| 单封面或 group 缩略图 | `High` |
| 邻近完整图预加载 | `Preload` |

打开态加载当前完整图和 `PreloadCount` 邻近窗口；离开窗口的未完成 Full waiter 被取消。邻项按与 CurrentIndex 的距离从近到远
提交。关闭宿主时释放所有 Full lease，再恢复关闭态封面请求；缩略图 lease 保留供页面封面继续显示。

Previewer 不公开控件级最大并发数。下载/读取与解码并发、请求提升、两级去重、缓存和 waiter-local timeout 都由
Application-scoped loader 统一控制。

完整图解码目标来自 TopLevel ClientSize，封面目标来自 CoverWidth/CoverHeight 或实际 Bounds，均乘 render scaling 并向上量化
到 16 px bucket。Full 与 Thumbnail 分别按物理像素 bucket 保持请求幂等；初始占位布局、最终 Arrange、窗口尺寸或 DPI 变化产生
更合适的 bucket 时会保留当前图片并异步升级，成功后原子替换，避免把低分辨率封面长期放大。明确得到 0 x 0 时不发起无意义的
封面请求；直接 loader 请求的显式非法目标由统一错误契约处理。

### 4.3 Fallback、Reload 与事件

Fallback 是每个 item 的局部策略，不是整个集合的替换策略。一个 item 失败不能删除其他 item、替换 ItemsSource 或改变
CurrentIndex。Full 与 Thumbnail 通道分别按自己的来源顺序尝试 fallback。

Reload 使用 item 的 RequestOptions 副本并把 `CacheMode` 改为 `Reload`：HTTP 使用条件重验证，File/Asset/Storage/Bytes/Stream
强制重读。Reload 只增加目标通道的 generation；同一 item 的另一个通道以及其他 item 不受影响。

只有当前 Full entry 从非 Loaded 进入 Loaded 时触发 `ImageOpened`，从非 Failed 进入 Failed 时触发 `ImageFailed`。预加载项和
封面状态不冒充当前项事件。

### 4.4 标题契约

标题优先级固定为：

```text
宿主显式 Window.Title / PreviewTitle
  > ImagePreviewItem.Title
  > IImagePreviewTitleResolver
  > 空标题
```

`PreviewTitle` 被转接到 dialog 的 `Window.Title`。当显式标题为空时，宿主先读取当前 item 的 `Title`，再调用 resolver。
resolver 上下文只包含 immutable `ImagePreviewItem`、显示用 CurrentIndex 和 Count，不发起 I/O。

默认 resolver 返回 `Item.Source.DisplayName`。`PreviewTitleIcon` 只显示在预览标题栏，不投射到普通 Window icon。

## Theme and Token Boundaries

Desktop 支持 native window 时使用 `ImagePreviewerDialog`；Browser 等无 native window 平台使用
`ImagePreviewerOverlayHost`。两种宿主共享 ItemsSource、CurrentIndex TwoWay、交互、占位、动效和 modal 语义。

稳定主题节点包括：

- `PART_CoverItemsControl`：group 封面集合。
- `PART_ImageViewerScene`、`PART_ImageRenderer`：预览坐标空间与图片 renderer。
- `PART_LoadingPresenter`、`PART_ErrorPresenter`：封面和 viewer 的状态占位。
- `PART_PreviousButton`、`PART_NextButton` 与 toolbar 操作按钮。
- `PART_TitleLayout`、`PART_IconPresenter`：dialog 标题与图标。
- `PART_CloseButton`：overlay 关闭入口。

Renderer 只消费 entry 中的 `IImage`，不得自行打开 Source。Loading 时封面使用稳定尺寸 Skeleton 语义，viewer 使用居中 Spin；
Failed 时使用本地化默认错误内容或用户模板。

Token 边界：

ImagePreviewer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ImagePreviewerToken`，scope id 为 `ImagePreviewer`，源码位于 `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs`。

## Customization Boundaries

- 关闭 dialog/overlay：释放 host bindings、事件订阅、logical parent 和全部 Full lease，保留关闭态需要的 Thumbnail lease。
- detach：解除集合订阅，取消 Full/Thumbnail waiter 并释放全部 lease。
- TopLevel resize 或 render scaling 变化：重新计算当前物理像素 bucket，相同 bucket 不重复读取，不同 bucket 触发替换请求。
- reattach：重新物化集合并按 IsOpen 选择当前加载策略。
- source replacement、collection remove/reset：旧 entry 不能回写或继续保留图片。
- 当前 API 不包含旧 source 接口、单/多 source 属性、私有 loader 或私有 scheduler；不提供兼容 shim。
- 不允许同步 I/O、固定延迟、反射发现 loader/codec，或在 Previewer 内创建 `HttpClient`。

维护不变量：

- public `ImagePreviewItem` 保持 immutable，只保存配置；所有可变加载状态只属于 internal entry。
- Full 与 Thumbnail 的 generation、CTS、state、progress 和 lease 相互独立，Reload 一个通道不能干扰另一个通道。
- Full 与 Thumbnail 分别记录活动/已提交物理像素 bucket；布局或 DPI 变化后不得继续放大较小 bucket 的旧结果。
- current、cover 和 collection clamp 只决定显示 entry，不静默改写外部 TwoWay 索引。
- 所有请求进入 Application-scoped `IImageLoader`；Previewer 不增加本地 semaphore、cache、transport 或 codec。
- host close 释放 Full leases，detach 释放 Full/Thumbnail leases；旧 entry、旧 generation 和已关闭 host 都不能回写。
- renderer、loading presenter 和 error presenter 只消费状态，不发起 I/O 或拥有结果。
- native dialog 与 Browser overlay 必须共享 item、current、navigation、loading 和关闭语义。
