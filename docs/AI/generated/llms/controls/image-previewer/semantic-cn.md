# ImagePreviewer 语义结构

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## Semantic Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `AbstractImagePreviewer` | 归一 items、current、open state、宿主和加载策略 | `ItemsSource`、`CurrentIndex`、`IsOpen`、`PreloadCount` | SharedToken、ImagePreviewerToken | public |
| `cover` | `ImagePreviewer` / `PART_CoverItemsControl` | 展示单封面或 group 缩略图及 loading/error 状态 | Cover size/state、`ItemsPanel` | Cover size、mask、radius Token | public/template-stable |
| `host` | `ImagePreviewerDialog` / `ImagePreviewerOverlayHost` | 承载 Desktop window 或 Browser overlay | dialog、modal、topmost、title、motion API | Window、overlay、motion Token | internal-observable |
| `viewer` | `ImageViewer` / `PART_ImageViewerScene` | 当前项导航、fit、拖拽、缩放和旋转 | interaction、scale、CurrentIndex | Viewer background、toolbar Token | internal-observable |
| `renderer` | `PART_ImageRenderer` | 只渲染 entry 已持有的 `IImage` | current load state | 无独立加载 Token | template-stable |
| `loading` | `PART_LoadingPresenter` | 呈现 Skeleton 或 Spin，不拥有请求 | LoadingContent/Template | Loading、Skeleton、Spin Token | template-stable |
| `error` | `PART_ErrorPresenter` | 呈现最终失败内容，不改变状态机 | ErrorContent/Template | Error semantic Token | template-stable |

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
                 -> SkeletonImage#PART_LoadingSkeleton (template-stable)
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
| `Panel` | template node (Panel) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate`, `ErrorContent`, `ErrorContentTemplate`, `HasError`, `ImageSource` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingPresenter` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `LoadingContent`, `LoadingContentTemplate` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `PART_LoadingSkeleton` | template node (SkeletonImage) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `LoadingContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `ContentPresenter` | template node (ContentPresenter) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `LoadingContent`, `LoadingContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |
| `PART_ErrorPresenter` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `ErrorContent`, `ErrorContentTemplate`, `HasError` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DefaultErrorLayout` | template node (StackPanel) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `ErrorContent` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DefaultErrorIcon` | template node (PictureOutlined) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `DefaultErrorText` | template node (TextBlock) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | 主题状态 / visual state | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `Mask` | template node (Border) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate`, `IsCoverMaskVisible`, `MaskOpacity` | template-stable | 用于主题维护；变更需同步主题、实现和 LLMS。 |
| `MaskContentPresenter` | template node (ContentPresenter) | `ImagePreviewerCoverTheme.axaml` | ImagePreviewerCover | `Content`, `ContentTemplate` | internal-observable | 用于理解结构和状态流，不应指导用户代码直接依赖。 |

## Template Parts

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `AbstractImagePreviewer` | 归一 items、current、open state、宿主和加载策略 | `ItemsSource`、`CurrentIndex`、`IsOpen`、`PreloadCount` | SharedToken、ImagePreviewerToken | public |
| `cover` | `ImagePreviewer` / `PART_CoverItemsControl` | 展示单封面或 group 缩略图及 loading/error 状态 | Cover size/state、`ItemsPanel` | Cover size、mask、radius Token | public/template-stable |
| `host` | `ImagePreviewerDialog` / `ImagePreviewerOverlayHost` | 承载 Desktop window 或 Browser overlay | dialog、modal、topmost、title、motion API | Window、overlay、motion Token | internal-observable |
| `viewer` | `ImageViewer` / `PART_ImageViewerScene` | 当前项导航、fit、拖拽、缩放和旋转 | interaction、scale、CurrentIndex | Viewer background、toolbar Token | internal-observable |
| `renderer` | `PART_ImageRenderer` | 只渲染 entry 已持有的 `IImage` | current load state | 无独立加载 Token | template-stable |
| `loading` | `PART_LoadingPresenter` | 呈现 Skeleton 或 Spin，不拥有请求 | LoadingContent/Template | Loading、Skeleton、Spin Token | template-stable |
| `error` | `PART_ErrorPresenter` | 呈现最终失败内容，不改变状态机 | ErrorContent/Template | Error semantic Token | template-stable |

## Pseudo Classes

- Template Part、伪类、loading/error 门控、标题、导航、缩放和 Token 视觉语义保持稳定。
- 不允许同步 I/O、固定延迟、反射发现 reader/codec/serializer，或在 Previewer 内创建 `HttpClient`、cache 或 scheduler。
- SourceSnapshot、encoded content 和 decoded content 的共享范围都受 CachePartition 限制；安全分区之间不共享命中或诊断。
- borrowed `IImage` 始终由调用方拥有，任何 loader/cache/control 生命周期都不能 dispose 它。

## State Flow

### 4.1 内容身份与来源验证

内部身份链固定为：

```text
ImageSourceKey --validate--> ImageSourceVersion --maps-to--> ImageContentId
ImageContentId + ImageDecodeSpec ---------------------------> ImageDecodeKey
```

| 术语 | 定义 | 是否包含解码尺寸 |
| --- | --- | --- |
| `ImageSourceKey` | 规范来源地址或 identity、representation header 摘要、Variant、partition 摘要和 reader contract | 否 |
| `ImageSourceVersion` | 某次来源验证观察到的版本令牌 | 否 |
| `ImageContentId` | 对读取到的精确编码字节计算的 SHA-256 内容身份 | 否 |
| `ImageDecodeSpec` | codec、安全策略、目标物理像素、颜色和方向等影响输出的参数 | 按 codec 决定 |
| `ImageDecodeKey` | partition、ContentId 与 DecodeSpec 组成的解码身份 | 按 codec 决定 |
| `ImageSourceSnapshot` | SourceKey、SourceVersion、ContentId、freshness/validator 和提交 generation 的不可变映射 | 否 |

`Timeout`、Priority、progress callback、CacheRead 和 CacheStorage 不进入内容身份。它们只控制单个 waiter 的执行方式。

来源验证策略：

| Source | SourceVersion 或验证契约 | 跨请求来源映射 |
| --- | --- | --- |
| File | 已打开句柄的 file identity、长度和 modify/change stamp；严格模式读取哈希 | Desktop 可持久化 |
| HTTP | FreshUntil、ETag、Last-Modified 和 Vary | 服从响应缓存指令 |
| Asset | 当前 Application build/resource identity 内不可变 | 可持久化 |
| StorageFile | 显式 revision，或平台可观察 basic properties | 仅稳定 identity/revision 可持久化 |
| Bytes | 防御性副本直接计算 ContentId | 不建立来源持久映射 |
| Stream | 显式 identity + revision；否则每次打开并计算 ContentId | 仅稳定 identity/revision 可持久化 |
| Borrowed image | 对象 identity；不进入 encoded/decoded cache | 不持久化 |

File 的 metadata probe 和正文读取基于同一个已打开句柄，避免路径探测与正文读取之间的替换竞态。HTTP fresh snapshot 可直接
复用；过期或 `no-cache` snapshot 必须重验证，优先使用 ETag，其次 Last-Modified；`304` 延续原 ContentId，`200` 对新正文
重新计算 ContentId；`no-store` 不保留 source、encoded 或 decoded 条目。

调用方为 Stream 或 Storage 提供 revision，即承诺 revision 不变时内容不变；无法作出该承诺时必须省略 revision，让 loader
重新读取并以 ContentId 判断内容复用。

### 4.2 集合与双通道状态

`ItemsSource` 物化为控件内部的 `ImagePreviewEntry` 集合。每个 entry 拥有两个互相隔离的通道：

| 通道 | 内容 | 状态与资源 |
| --- | --- | --- |
| Full | 预览完整图 | FullState/Error/Progress、generation、waiter CTS、物理像素 bucket、`ImageLoadResult` lease |
| Thumbnail | 页面封面 | ThumbnailState/Error/Progress、generation、waiter CTS、独立 bucket 与 lease |

集合支持 enumerable replacement，以及 `INotifyCollectionChanged` 的 Add、Remove、Move、Replace 和 Reset。未变的 immutable item
可以复用 entry 与现有 lease；被移除或替换的 entry 立即取消并 dispose。detach 期间解除集合订阅，reattach 时重新物化当前集合，
补齐离线变更。

集合动作只影响 Previewer 自己的 entry、waiter 和 lease，绝不调用全局或分区 cache clear：

| 集合或宿主动作 | Previewer 行为 | Application cache |
| --- | --- | --- |
| Add | 创建 entry，按 open/cover/preload 状态请求 | 正常共享 |
| Remove / Replace | 取消并 dispose 旧 entry | 不清理 |
| Move | 移动 entry，保留其通道与 lease | 不清理 |
| Reset / Clear | 复用仍存在的 item，其余 entry 全部 dispose | 不清理 |
| ItemsSource replacement | 重新物化，释放不再使用的 entry | 不清理 |
| host close | 释放 Full waiter/lease，恢复关闭态 Thumbnail 策略 | 不清理 |
| detach | 释放 Full/Thumbnail waiter/lease 并退订集合 | 不清理 |

缓存正确性不依赖集合 Clear。相同路径的文件被替换后，`ValidateSource` 产生新的 SourceVersion；新字节产生新的 ContentId，进而
产生新的 DecodeKey。旧 decoded entry 不能作为新内容命中，只能保留为受预算约束的 LRU 条目直至驱逐。

### 4.3 加载优先级、尺寸与预加载

| 场景 | Source 顺序 | `ImageRequestPriority` | 默认 CacheRead |
| --- | --- | --- | --- |
| 当前完整图 | `Source -> FallbackSource` | `Critical` | item options；缺省为 `ValidateSource` |
| 单封面或 group 缩略图 | `ThumbnailSource -> Source -> FallbackSource` | `High` | item options |
| 相邻完整图预加载 | `Source -> FallbackSource` | `Preload` | item options |

打开态加载当前完整图和 `PreloadCount` 邻近窗口；离开窗口的未完成 Full waiter 被取消。邻项按与 CurrentIndex 的距离从近到远
提交。关闭宿主时释放所有 Full lease，再恢复关闭态封面请求；Thumbnail lease 保留供页面封面继续显示。

完整图解码目标来自 TopLevel ClientSize，封面目标来自 CoverWidth/CoverHeight 或实际 Bounds，均乘 render scaling 并向上量化
到 16 px bucket。Full 与 Thumbnail 分别按 bucket 保持请求幂等；布局、窗口尺寸或 DPI 变化产生更合适的 bucket 时保留当前图片并
异步升级，成功后原子替换。明确得到 0 x 0 时不发起无意义的封面请求。SVG 等尺寸无关 codec 不把显示尺寸写入 DecodeSpec。

### 4.4 Fallback、Reload 与结果事件

Fallback 是每个 item、每个通道的局部策略。一个 item 失败不能删除其他 item、替换 ItemsSource 或改变 CurrentIndex。取消不触发
fallback，也不提交 Failed。

`ReloadCurrent()`、`ReloadItem(index)` 和 `ReloadCover()` 使用 item 的 RequestOptions 副本，只把目标通道本次请求的
`CacheRead` 覆盖为 `RefreshSource`。调用方 options 不被修改；另一个通道和其他 item 的 generation、waiter、state 与 lease
不受影响。

只有当前 Full entry 从非 Loaded 进入 Loaded 时触发 `ImageOpened`，从非 Failed 进入 Failed 时触发 `ImageFailed`。预加载项和
封面状态不冒充当前项事件。

加载结果把“从哪里取得”和“来源是否已验证”分开表达：

```csharp
public enum ImageLoadOrigin
{
    Borrowed,
    DecodedMemory,
    EncodedMemory,
    Persistent,
    Network,
    Local
}

public enum ImageSourceValidation
{
    NotRequired,
    Current,
    Revalidated,
    Unverified
}
```

成功结果至少包含 `Image`、`ContentId`、`Origin`、`SourceValidation`、原始/解码尺寸、media type 和阶段耗时。borrowed image 的
ContentId 为空，Origin 为 `Borrowed`，SourceValidation 为 `NotRequired`。结果 lease 独立于 cache membership；条目被驱逐时，
仍被控件持有的 image 在最后一个 lease 释放前保持有效。

### 4.5 切换显示、标题与宿主

`ImageSwitchMode` 决定目标图片切换时是否保留上一张图片。`Immediate` 在目标没有可显示图片时于同一 UI 更新周期清空旧图，并把
Idle/Loading 的待完成目标投影为 loading；`WaitForLoaded` 在有效保留帧存在时持续显示旧图，目标加载状态仍保持为 true，但 loading
presenter 由 `:loading:not(:has-image)` 门控而不覆盖旧图。失败呈现、事件时序、请求与租约语义不因模式变化；运行时切换模式立即重算
预览窗口与单封面状态。
完整显示矩阵和保留帧生命周期见 [ImagePreviewer 切换显示设计](switch-display-design.md)。

标题优先级固定为：

```text
宿主显式 Window.Title / PreviewTitle
  > ImagePreviewItem.Title
  > IImagePreviewTitleResolver
  > 空标题
```

默认 resolver 返回 `Item.Source.DisplayName`。resolver 上下文只包含 immutable item、显示用 CurrentIndex 和 Count，不发起 I/O。
`PreviewTitleIcon` 只显示在预览标题栏，不投射到普通 Window icon。

Desktop 支持 native window 时使用 `ImagePreviewerDialog`；Browser 等无 native window 平台使用
`ImagePreviewerOverlayHost`。两种宿主共享 ItemsSource、CurrentIndex TwoWay、交互、占位、动效、加载和关闭语义。

## Theme and Token Boundaries

稳定主题节点包括：

- `PART_CoverItemsControl`：group 封面集合；
- `PART_ImageViewerScene`、`PART_ImageRenderer`：预览坐标空间与图片 renderer；
- `PART_LoadingPresenter`、`PART_ErrorPresenter`：封面和 viewer 的状态占位；
- `PART_PreviousButton`、`PART_NextButton` 与 toolbar 操作按钮；
- `PART_TitleLayout`、`PART_IconPresenter`：dialog 标题与图标；
- `PART_CloseButton`：overlay 关闭入口。

Renderer 只消费 entry 已提交的 `IImage`，不得自行打开 Source 或访问 loader/cache。Loading 时封面使用稳定尺寸 Skeleton，viewer
使用居中 Spin；Failed 时使用本地化默认错误内容或用户模板。viewer 的加载指示器由 `:loading:not(:has-image)` 门控，只在无可
显示图片时呈现。Loading/error 自定义模板只替换内容，不能拥有请求、entry 或结果 lease。

本设计不改变现有缩放、拖拽、旋转、导航、标题栏、封面 mask、Token 和 Light/Dark 视觉契约。Token 的来源、计算和消费位置见
[ImagePreviewer Token 设计](token.md)。

Token 边界：

ImagePreviewer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ImagePreviewerToken`，scope id 为 `ImagePreviewer`，源码位于 `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs`。

## Customization Boundaries

- `ImageSource` 来源层次、`ImageCacheReadPolicy` 和 `ImageCacheStoragePolicy` 是唯一公共模型，不保留旧名称或兼容 shim。
- 普通加载默认 `CacheRead=ValidateSource`、`CacheStorage=MemoryAndDisk`；Reload 只对单次目标通道使用 `RefreshSource`。
- 来源变化不依赖集合 Clear、控件重建或手工 cache clear 才能被识别。
- Previewer 的 Add/Remove/Replace/Move/Reset/Clear、host close 和 detach 不能清除 Application cache。
- 关闭 dialog/overlay 释放宿主 binding、订阅、logical parent 和全部 Full lease；detach 释放 Full/Thumbnail waiter 与 lease。
- TopLevel resize 或 render scaling 变化重新计算物理像素 bucket；相同 bucket 不重复读取，不同 bucket 异步升级。
- source replacement、collection remove/reset、旧 generation 和已关闭 host 都不能回写当前 entry。
- Full 与 Thumbnail 通道保持取消、状态、错误、进度、尺寸和 lease 隔离。
- 当前项和封面按显示索引 clamp，但不静默改写外部 TwoWay CurrentIndex。
- Template Part、伪类、loading/error 门控、标题、导航、缩放和 Token 视觉语义保持稳定。
- 不允许同步 I/O、固定延迟、反射发现 reader/codec/serializer，或在 Previewer 内创建 `HttpClient`、cache 或 scheduler。
- SourceSnapshot、encoded content 和 decoded content 的共享范围都受 CachePartition 限制；安全分区之间不共享命中或诊断。
- borrowed `IImage` 始终由调用方拥有，任何 loader/cache/control 生命周期都不能 dispose 它。

维护不变量：

- public `ImagePreviewItem` 保持 immutable，只保存配置；所有可变加载状态只属于 internal entry。
- Full 与 Thumbnail 的 generation、CTS、state、progress 和 lease 相互独立，Reload 一个通道不能干扰另一个通道。
- Full 与 Thumbnail 分别记录活动/已提交物理像素 bucket；布局或 DPI 变化后不得继续放大较小 bucket 的旧结果。
- 同尺寸桶下的优先级提升必须采纳在途请求，不得重启；取消（调用方或共享操作内部）必须良性归位
  （保留旧图或回 Idle），不得产生 Failed 提交或 `ImageFailed`。
- `ImageLoader` 的取消分类：非超时、非销毁的 `OperationCanceledException` 一律按取消交付，禁止上报为源失败。
- current、cover 和 collection clamp 只决定显示 entry，不静默改写外部 TwoWay 索引。
- 所有请求进入 Application-scoped `IImageLoader`；Previewer 不增加本地 semaphore、cache、transport 或 codec。
- 普通请求必须先按 `ImageCacheReadPolicy` 解析或验证 SourceSnapshot，再按 ContentId/DecodeSpec 查询 decoded store；不存在
  SourceKey 直返 decoded image 的 fast path。
- 相同路径或 URI 的来源发生变化时必须形成新的 SourceVersion 和 ContentId；正确性不依赖 collection Clear 或手工 cache clear。
- Add/Remove/Replace/Move/Reset/Clear、host close 和 detach 只释放控件 waiter/lease，不清理 Application cache。
- `ImageCacheReadPolicy` 与 `ImageCacheStoragePolicy` 是正交契约；Reload 只覆盖单次请求的 CacheRead，不修改 item options。
- `CacheStorage=None` 可以读取既有 cache，但不能把 persistent 命中提升到 memory store；任何进入 memory 的 persistent 内容必须先
  通过当前安全策略校验。
- source/decode single-flight、source commit generation 和 cache epoch 必须阻止重复工作、snapshot 回滚和清理后回填。
- host close 释放 Full leases，detach 释放 Full/Thumbnail leases；旧 entry、旧 generation 和已关闭 host 都不能回写。
- dialog 与 overlay 必须共用 `ImagePreviewDisplayTracker`，不允许在任一宿主内复制目标项/保留帧跟踪；目标项与保留帧
  订阅只在 `SetCurrentItem` / `SetRetained` 内成对变更；宿主必须订阅有效集合增量变更（索引不变但 entry 更换时重配），
  宿主关闭必须退订集合并清空 tracker。
- 显示解析必须把“存在目标但尚无图且未失败”的 Idle/Loading 状态投影为 loading；`Immediate` 同周期清空旧图且不持有保留帧，
  `WaitForLoaded` 至多持有 1 个保留帧，并以 tracker 会话标识与会话内单调目标序号约束“当前项完成 / 最近完成全图加载”的两级来源；
  乱序完成、纯预加载和旧宿主会话都不能改变当前显示。
- `ImageSwitchMode` 运行时变化必须立即刷新打开态 tracker 与单封面状态，不能等待下一次索引、集合或加载通知。
- entry `Dispose()` 必须先广播 Full/Thumbnail 重置通知再清空 subscriber；显示持有者不得在通知后继续引用已释放位图。
- Full/Thumbnail 卸载与失败提交必须先断开旧 result，再发布一致状态，最后释放旧 lease；禁止暴露“Failed + 旧图”或
  “lease 已释放 + 显示仍持有旧图”的中间状态。
- viewer 加载指示器只在无显示图时呈现（`:loading:not(:has-image)` 门控）；封面 mask 只由 `IsShowCoverMask` 决定，
  与加载/失败状态解耦；错误呈现仍绑定 `IsCurrentImageFailed`。
- renderer、loading presenter 和 error presenter 只消费状态，不发起 I/O 或拥有结果。
- native dialog 与 Browser overlay 必须共享 item、current、navigation、loading 和关闭语义。
