# ImagePreviewer

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

`ImagePreviewer` 提供单封面入口与完整图片预览；`ImageGroupPreviewer` 提供多封面入口。两者共享同一个 immutable item 集合、
当前项、预加载、标题、窗口/overlay 和图片状态模型。

Previewer 不拥有网络栈、缓存或并发调度器。所有请求进入当前 `Application` 的 `IImageLoader`。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer` |
| 状态 | Stable |

## 何时使用

ImagePreviewer 以“页面封面 -> 沉浸预览”的两阶段体验表达图片查看。关闭态封面提供稳定的尺寸和加载反馈；打开态聚焦当前完整图，
相邻项仅作为低优先级预加载。单封面与多封面只改变入口布局，不改变当前项、标题、缩放、导航和加载语义。

Loading、Failed 和 Loaded 都是明确状态。封面 Skeleton、viewer Spin 和错误内容只呈现状态，不拥有加载行为。Desktop native dialog
与 Browser overlay 共享同一 item/entry 模型，平台差异只位于宿主能力和标题栏组合。

## 公共 API

### 3.1 图片项模型

Public 数据输入只有 `ItemsSource: IEnumerable<ImagePreviewItem>?`。`ImagePreviewItem` 是不可变配置 record：

```csharp
public sealed record ImagePreviewItem
{
    public ImagePreviewItem(ImageLoadSource source);

    public ImageLoadSource Source { get; init; }
    public ImageLoadSource? ThumbnailSource { get; init; }
    public ImageLoadSource? FallbackSource { get; init; }
    public ImageRequestOptions? RequestOptions { get; init; }
    public string? Title { get; init; }
    public object? Tag { get; init; }
}
```

`Source` 是完整图来源。`ThumbnailSource` 是可选封面来源；为空时封面使用 `Source` 按封面尺寸解码。完整图加载顺序为
`Source -> FallbackSource`，缩略图加载顺序为 `ThumbnailSource -> Source -> FallbackSource`，相同 identity 不重复请求。

`ImagePreviewItem` 不保存加载状态、不持有 cancellation token、图片或 Visual。相同 item 可以出现在多个控件中，每个控件的
状态互不影响。`Tag` 只供业务关联，加载器和标题解析器不通过反射读取它。

### 3.2 AbstractImagePreviewer 公共契约

| 契约组 | 成员与默认值 | 语义 |
| --- | --- | --- |
| 集合 | `ItemsSource=null` | 唯一图片项输入 |
| 当前项 | `CurrentIndex=0`、`CurrentItem` | `CurrentIndex` 默认 TwoWay；显示时 clamp，但不静默改写外部值 |
| 封面 | `CoverIndex=0`、`CoverWidth=NaN`、`CoverHeight=NaN` | CoverIndex 非负并在显示时 clamp，与 CurrentIndex 独立 |
| 预加载 | `PreloadCount=1` | 当前项前后各预加载的完整图片数量 |
| 切换显示 | `ImageSwitchMode=Immediate` | 预览窗口与封面的切换显示策略：两种模式都保持显示连续性（无空白帧频闪）；`Immediate` 在目标超过 300ms 宽限仍无图时回退加载占位，`WaitForLoaded` 无限期保持上一张 |
| 当前加载状态 | `CurrentLoadState/Error/Progress`、`IsCurrentLoading/Loaded/Failed` | 当前完整图 entry 的只读投影 |
| 占位内容 | `LoadingContent/Template`、`ErrorContent/Template` | 只替换主题 presenter 内容，不改变状态机 |
| 打开状态 | `IsOpen=false` | 默认 TwoWay，统一驱动 native dialog 或 Browser overlay |
| 预览交互 | `IsImageMovable=true`、`ImageScaleStep=0.5`、`ImageMinScale=1`、`ImageMaxScale=50` | 拖拽、缩放、旋转和 fit-to-window 的边界 |
| 窗口 | `IsDialogModal=false`、`IsDialogTopmost=false` | Desktop native window 行为；Browser 使用 overlay |
| 标题 | `PreviewTitle`、`PreviewTitleIcon`、`PreviewTitleResolver` | 当前项标题和可选 PathIcon |
| 动效 | `IsMotionEnabled` | 控制主题动效，不改变加载与集合语义 |

命令和事件：

- `OpenDialog()` 打开当前宿主；无有效 item 时不打开。
- `ReloadCurrent()` 只 Reload 当前完整图通道。
- `ReloadItem(index)` 只 Reload 指定 entry 的完整图通道；越界抛出 `ArgumentOutOfRangeException`。
- `DialogOpening` 不作为 public surface；当前事件为 `DialogOpened`、可取消的 `DialogClosing` 和 `DialogClosed`。
- `ImageOpened`、`ImageFailed` 只投影当前完整图状态，事件参数包含 immutable item 与索引。

### 3.3 ImagePreviewer 与 ImageGroupPreviewer

单封面 `ImagePreviewer` 增加：

- `CoverIndicatorContent`、`CoverIndicatorContentTemplate`
- `IsShowCoverMask=true`
- `CoverLoadState/Error/Progress`
- `IsCoverLoading`、`IsCoverLoaded`、`IsCoverFailed`
- `ReloadCover()`，只 Reload 当前封面的缩略图通道

`ImageGroupPreviewer` 只增加 `ItemsPanel`。关闭态下它为每个 effective item 请求缩略图；点击某个封面时先把
`CurrentIndex` 设置为该项索引，再打开预览。

单封面控件的 CoverIndex 只决定关闭态封面。点击单封面只打开预览，不隐式把 CurrentIndex 同步为 CoverIndex。

稳定 template part 包括 `PART_CoverItemsControl`、`PART_ImageViewerScene`、`PART_ImageRenderer`、
`PART_LoadingPresenter`、`PART_ErrorPresenter`、`PART_PreviousButton`、`PART_NextButton`、`PART_TitleLayout`、
`PART_IconPresenter` 和 `PART_CloseButton`。

LLMS 语义区域：

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `AbstractImagePreviewer` | 归一 ItemsSource、current、open state、宿主与加载策略。 | `ItemsSource`、`CurrentIndex`、`IsOpen`、`PreloadCount` | SharedToken、ImagePreviewerToken | public |
| `cover` | `ImagePreviewer` / `PART_CoverItemsControl` | 展示单封面或 group 缩略图及其 loading/error 状态。 | `CoverIndex`、cover size/state、`ItemsPanel` | Cover size、mask、radius Token | public/template-stable |
| `host` | `ImagePreviewerDialog` / `ImagePreviewerOverlayHost` | 承载 Desktop window 或 Browser overlay。 | dialog、modal、topmost、title、motion API | Window、overlay、motion Token | internal-observable |
| `viewer` | `ImageViewer` / `PART_ImageViewerScene` | 当前项导航、fit、拖拽、缩放和旋转。 | interaction、scale、CurrentIndex | Viewer background、toolbar Token | internal-observable |
| `renderer` | `PART_ImageRenderer` | 只渲染 entry 已持有的 `IImage`。 | current load state | 无独立加载 Token | template-stable |
| `loading` | `PART_LoadingPresenter` | 呈现 Skeleton 或 Spin，不拥有请求。 | LoadingContent/Template | Loading、Skeleton、Spin Token | template-stable |
| `error` | `PART_ErrorPresenter` | 呈现最终失败内容，不改变状态机。 | ErrorContent/Template | Error semantic Token | template-stable |

## 事件与命令

命令和事件：
- `DialogOpening` 不作为 public surface；当前事件为 `DialogOpened`、可取消的 `DialogClosing` 和 `DialogClosed`。
- `ImageOpened`、`ImageFailed` 只投影当前完整图状态，事件参数包含 immutable item 与索引。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:36`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:ImagePreviewer Width="200"
```

### 远程图片加载

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:48`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:ImagePreviewer Width="200"
```

### 容错

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:60`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:ImagePreviewer Width="200"
```

### 20 张远程图片

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:72`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:ImagePreviewer Width="200"
```

## 状态模型

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

### 4.4 切换显示策略

`ImageSwitchMode` 决定切换目标图片时加载占位的回退时机。两种模式共享显示连续性：目标无图期间
保留上一张已加载图（含切换瞬间的 Idle 态，无空白帧/转圈频闪），保留帧由当前项完成或"最近完成的全图
加载"补充。`Immediate`（默认）在目标超过 300ms 宽限仍无图时回退加载占位；`WaitForLoaded` 无限期保持。
失败呈现、事件时序、加载请求与租约语义两种模式一致。显示矩阵、保留帧与宽限机制、宿主跟随集合增量
变更的规则见 [ImagePreviewer 切换显示设计](switch-display-design.md)。

### 4.5 标题契约

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

## 主题与 Design Token

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
Failed 时使用本地化默认错误内容或用户模板。viewer 的加载指示器由 `:loading:not(:has-image)` 伪类门控，只在无可显示图片时呈现，
与封面和 `AsyncImage` 的门控语义一致。

Token 来源：

ImagePreviewer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ImagePreviewerToken`，scope id 为 `ImagePreviewer`，源码位于 `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs`。

## AOT 与裁剪注意事项

- ItemsSource、entry、host 和 loader 之间没有 Visual -> business item 的反向长期引用。
- 显示 tracker 重算热路径零分配；目标项与保留帧订阅严格经单一变更通道配对，`Immediate` 模式不持有保留帧（额外持有恒为 0），
  `WaitForLoaded` 至多持有 1 个保留帧 entry 及其解码图租约。
- entry Dispose 的重置通知使用静态缓存 EventArgs，无订阅者时零开销。
- 任何 cancellation、result dispose 或 event callback 都不在 Shared coordinator/cache lock 内由 Previewer执行。
- 不进行同步 HTTP/File I/O，不使用固定延迟，不创建私有 cache/scheduler。
- Public data model、AXAML property 和 title resolver 都不依赖反射扫描。
- Browser overlay 与 Desktop dialog 共用同一 entry/load model；平台差异只位于宿主能力。
- Application dispose 统一取消底层 loader；控件仍负责尽快取消 waiter 和释放自身租约。

## 源码索引

`src/AtomUI.Desktop.Controls/ImagePreviewer/` 的稳定职责如下：

| 类型 | 职责 |
| --- | --- |
| `AbstractImagePreviewer` | public API、ItemsSource 物化、current entry、预加载策略和 dialog/overlay 生命周期 |
| `ImagePreviewItem` | immutable public 配置，包含 Source、Thumbnail、Fallback、Options、Title 和 Tag |
| `ImagePreviewEntry` | internal Full/Thumbnail 状态、generation、取消、结果租约 owner；Dispose 前广播重置通知 |
| `ImagePreviewDisplayTracker` | internal 宿主显示状态机：目标项与保留帧跟踪、显示图三值解析（见[切换显示设计](switch-display-design.md)） |
| `ImagePreviewer` | 单封面选择、状态投影和 `ReloadCover()` |
| `ImageGroupPreviewer` | 多封面 ItemsControl、点击索引和关闭态缩略图请求 |
| `ImagePreviewerDialog` | Desktop native window、标题算法、CurrentIndex relay 和 viewer 组合 |
| `ImagePreviewerOverlayHost` | Browser/无原生窗口平台的 overlay 宿主 |
| `ImageViewer` | 导航、变换、fit-to-window 和 loading/error 状态呈现 |
| `ImagePreviewRenderer` | 只渲染 entry 已持有的 `IImage` |
| `IImagePreviewTitleResolver` | 无 I/O 的标题解析扩展点 |

底层 `IImageLoader`、Source、Request、Result、scheduler、coordinator、cache、transport 和 codec 位于
`AtomUI.Controls.Shared/ImageLoading/`。Previewer 目录不保留这些类型的副本。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/image-previewer/overview.md`
- 实现文档：`docs/controls/desktop/data-display/image-previewer/implementation.md`
- Token 文档：`docs/controls/desktop/data-display/image-previewer/token.md`
- 变更记录：`docs/controls/desktop/data-display/image-previewer/changelog.md`
- 语义结构：`./semantic-cn.md`
