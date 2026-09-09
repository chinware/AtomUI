# ImagePreviewer

> 生成产物：由源文档生成，不要手工编辑。修改内容请回到控件文档、源码 public surface、Token 类型或生成数据、Gallery ShowCase 或源码结构。

## 概述

`ImagePreviewer` 提供单封面入口与完整图片预览；`ImageGroupPreviewer` 提供多封面入口。两者共享同一套 immutable item、当前项、
预加载、标题、窗口或 overlay、加载状态和结果租约模型。

Previewer 是应用级图片加载系统的消费者，不拥有网络栈、文件读取器、codec、缓存或全局并发调度器。所有来源解析、内容验证、
请求合并、缓存读写和解码都进入当前 `Application` 的 `IImageLoader`；Previewer 只拥有控件 entry、waiter、显示状态和结果 lease。

## 包与命名空间

| 项 | 值 |
| --- | --- |
| NuGet 包 | `AtomUI.Desktop.Controls` |
| .NET 命名空间 | `AtomUI.Desktop.Controls` |
| AXAML 命名空间 | `https://atomui.net` |
| Gallery 页面 | `controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer` |
| 状态 | Stable |

## 何时使用

ImagePreviewer 以“页面封面 -> 沉浸预览”的两阶段体验表达图片查看。关闭态封面提供稳定尺寸和加载反馈；打开态聚焦当前完整图，
相邻项只作为低优先级预加载。单封面与多封面只改变入口布局，不改变当前项、标题、缩放、导航、来源验证和加载语义。

Loading、Failed 和 Loaded 都是明确状态。封面 Skeleton、viewer Spin 和错误内容只呈现状态，不拥有加载行为。Desktop native dialog
与 Browser overlay 共享同一 item/entry 模型，平台差异只位于宿主能力和标题栏组合。

加载架构遵守以下原则：

- 来源地址不是内容身份；路径或 URI 相同不等于内容未变化。
- 普通读取先验证来源，再决定能否复用内容和解码结果；不能根据旧来源键直接返回 decoded image。
- `ImageSourceKey -> ImageSourceVersion -> ImageContentId -> ImageDecodeKey` 是唯一身份链。
- 缓存读取策略与缓存写入位置正交，Reload 只改变一次请求的读取策略。
- Previewer 的集合、宿主和 entry 生命周期不等于 Application cache 生命周期。
- Full 与 Thumbnail 通道的 generation、取消、状态、尺寸桶和结果 lease 完全隔离。
- borrowed image 的所有权始终属于调用方，loader、cache 和 Previewer 都不能销毁它。

## 公共 API

### 3.1 封闭图片来源模型

所有加载型入口只接收 `ImageSource`。`ImageSource` 是 AtomUI 定义的封闭来源层次；基类不提供外部继承入口，每个具体来源均为
不可变 sealed 类型：

```csharp
[TypeConverter(typeof(ImageSourceConverter))]
public abstract class ImageSource
{
    public string? DisplayName { get; }

    public static ImageSource Parse(string source);
    public static bool TryParse(string? source, out ImageSource? result);
}

public sealed class HttpImageSource : ImageSource
{
    public HttpImageSource(Uri uri);
    public Uri Uri { get; }
}

public sealed class FileImageSource : ImageSource
{
    public FileImageSource(
        string path,
        ImageFileValidationMode validation = ImageFileValidationMode.Metadata);

    public string Path { get; }
    public ImageFileValidationMode Validation { get; }
}

public sealed class AssetImageSource : ImageSource
{
    public AssetImageSource(Uri uri);
    public Uri Uri { get; }
}

public sealed class StorageFileImageSource : ImageSource
{
    public StorageFileImageSource(IStorageFile file, string? revision = null);
    public IStorageFile File { get; }
    public string? Revision { get; }
}

public sealed class BytesImageSource : ImageSource
{
    public BytesImageSource(ReadOnlyMemory<byte> bytes, string? displayName = null);
    public ReadOnlyMemory<byte> Bytes { get; }
}

public sealed class StreamImageSource : ImageSource
{
    public StreamImageSource(
        Func<CancellationToken, ValueTask<Stream>> openStream,
        string? identity = null,
        string? revision = null,
        string? displayName = null);
    public Func<CancellationToken, ValueTask<Stream>> OpenStream { get; }
    public string? Identity { get; }
    public string? Revision { get; }
}

public sealed class BorrowedImageSource : ImageSource
{
    public BorrowedImageSource(IImage image, string? displayName = null);
    public IImage Image { get; }
}
```

`BytesImageSource` 防御性复制输入；`StreamImageSource` 的工厂每次必须返回新的可读流；`BorrowedImageSource` 只借用调用方的
`IImage`。字符串转换只接受 `http`、`https`、`avares`、`file` 和平台绝对路径，未知 scheme 不回退为文件路径。

文件来源提供两种验证强度：

```csharp
public enum ImageFileValidationMode
{
    Metadata,
    ContentHash
}
```

- `Metadata` 是默认值，使用已打开句柄可观察的文件 identity、长度和修改/变化令牌判断内容是否变化。
- `ContentHash` 每次请求都读取正文并计算内容摘要，用于必须识别元数据保持不变的内容替换场景。

### 3.2 图片项模型

Public 数据输入只有 `ItemsSource: IEnumerable<ImagePreviewItem>?`。`ImagePreviewItem` 是不可变配置 record：

```csharp
public sealed record ImagePreviewItem
{
    public ImagePreviewItem(ImageSource source);

    public ImageSource Source { get; init; }
    public ImageSource? ThumbnailSource { get; init; }
    public ImageSource? FallbackSource { get; init; }
    public ImageRequestOptions? RequestOptions { get; init; }
    public string? Title { get; init; }
    public object? Tag { get; init; }
}
```

`Source` 是完整图来源。`ThumbnailSource` 是可选封面来源；为空时封面使用 `Source` 按封面尺寸解码。完整图加载顺序为
`Source -> FallbackSource`，缩略图加载顺序为 `ThumbnailSource -> Source -> FallbackSource`，同一通道内相同来源 identity
不重复请求。

`ImagePreviewItem` 不保存加载状态，不持有 cancellation token、图片或 Visual。相同 item 可以出现在多个控件中，各控件的
entry 状态互不影响；底层加载器仍可按内容身份共享请求和缓存。`Tag` 只供业务关联，加载器和标题解析器不通过反射读取它。

### 3.3 请求与缓存策略

缓存读取语义与成功结果的允许存储位置是两个独立维度：

```csharp
public sealed record ImageRequestOptions
{
    public ImageCacheReadPolicy CacheRead { get; init; } =
        ImageCacheReadPolicy.ValidateSource;

    public ImageCacheStoragePolicy CacheStorage { get; init; } =
        ImageCacheStoragePolicy.MemoryAndDisk;

    public string? CachePartition { get; init; }
    public string? Variant { get; init; }
    public TimeSpan? Timeout { get; init; }
    public IReadOnlyDictionary<string, string>? Headers { get; init; }
}

public enum ImageCacheReadPolicy
{
    ValidateSource,
    RefreshSource,
    PreferCache,
    CacheOnly
}

public enum ImageCacheStoragePolicy
{
    None,
    Memory,
    MemoryAndDisk
}
```

| `ImageCacheReadPolicy` | 已有来源映射 | 来源访问 | cache miss | 结果验证语义 |
| --- | --- | --- | --- | --- |
| `ValidateSource` | 仅在来源规则确认有效后复用 | 按 File、HTTP、Storage 等规则探测或重验证 | 访问来源 | `Current`、`Revalidated` 或 `NotRequired` |
| `RefreshSource` | 不把现有映射视为完成结果 | 强制访问来源；HTTP 可以使用 validator 发条件请求 | 访问来源 | `Current`、`Revalidated` 或 `NotRequired` |
| `PreferCache` | 映射和内容都存在时直接复用 | 仅 miss 时访问来源 | 访问来源 | 未验证命中为 `Unverified` |
| `CacheOnly` | 只使用已有映射和内容 | 禁止 | `CacheMiss` | 未验证命中为 `Unverified` |

`PreferCache` 不隐含后台刷新。一次 `LoadAsync` 不会在返回后静默替换结果；需要新内容时由调用方显式发起
`RefreshSource` 请求。`CacheStorage` 只决定本次成功结果允许写入 memory 或 disk；HTTP cache directive、安全策略和平台能力
只能进一步收紧，不能被 per-request 设置放宽。persistent cache 未启用时，`MemoryAndDisk` 的有效目标是 memory。

`ImageSourceValidation.Current` 表示内容满足该来源选择的验证契约，不表示所有来源都完成了逐字节证明。File 的
`Metadata` 模式以文件 identity 和 metadata token 为契约；需要逐字节证明时使用 `ContentHash`。

所有调用方提供、可能影响 HTTP 表示的 header 都以名称和不可逆值摘要进入来源 identity；header 原值不进入可打印 key、
持久文件名或默认 diagnostics。存在认证 header 而未提供 `CachePartition` 时，请求禁止共享、禁止读取共享缓存并强制不存储。

### 3.4 AbstractImagePreviewer 公共契约

| 契约组 | 成员与默认值 | 语义 |
| --- | --- | --- |
| 集合 | `ItemsSource=null` | 唯一图片项输入 |
| 当前项 | `CurrentIndex=0`、`CurrentItem` | `CurrentIndex` 默认 TwoWay；显示时 clamp，但不静默改写外部值 |
| 封面 | `CoverIndex=0`、`CoverWidth=NaN`、`CoverHeight=NaN` | CoverIndex 非负并在显示时 clamp，与 CurrentIndex 独立 |
| 预加载 | `PreloadCount=1` | 当前项前后各预加载的完整图片数量 |
| 切换显示 | `ImageSwitchMode=Immediate` | `Immediate` 在目标无图时同周期清空旧图并报告 loading；`WaitForLoaded` 在有效保留帧存在时持续显示旧图直到目标完成或失败 |
| 当前加载状态 | `CurrentLoadState/Error/Progress`、`IsCurrentLoading/Loaded/Failed` | 当前完整图 entry 的只读投影 |
| 占位内容 | `LoadingContent/Template`、`ErrorContent/Template` | 只替换主题 presenter 内容，不改变状态机 |
| 打开状态 | `IsOpen=false` | 默认 TwoWay，统一驱动 native dialog 或 Browser overlay |
| 预览交互 | `IsImageMovable=true`、`ImageScaleStep=0.5`、`ImageMinScale=1`、`ImageMaxScale=50` | 拖拽、缩放、旋转和 fit-to-window 的边界 |
| 窗口 | `IsDialogModal=false`、`IsDialogTopmost=false` | Desktop native window 行为；Browser 使用 overlay |
| 标题 | `PreviewTitle`、`PreviewTitleIcon`、`PreviewTitleResolver` | 当前项标题和可选 PathIcon |
| 动效 | `IsMotionEnabled` | 控制主题动效，不改变加载与集合语义 |

命令和事件：

- `OpenDialog()` 打开当前宿主；无有效 item 时不打开。
- `ReloadCurrent()` 只刷新当前完整图通道。
- `ReloadItem(index)` 只刷新指定 entry 的完整图通道；越界抛出 `ArgumentOutOfRangeException`。
- `DialogOpened`、可取消的 `DialogClosing` 和 `DialogClosed` 描述宿主生命周期。
- `ImageOpened`、`ImageFailed` 只投影当前完整图状态，事件参数包含 immutable item 与索引。

### 3.5 ImagePreviewer 与 ImageGroupPreviewer

单封面 `ImagePreviewer` 增加：

- `CoverIndicatorContent`、`CoverIndicatorContentTemplate`；
- `IsShowCoverMask=true`；
- `CoverLoadState/Error/Progress`；
- `IsCoverLoading`、`IsCoverLoaded`、`IsCoverFailed`；
- `ReloadCover()`，只刷新当前封面的 Thumbnail 通道。

`ImageGroupPreviewer` 只增加 `ItemsPanel`。关闭态下它为每个 effective item 请求缩略图；点击某个封面时先把
`CurrentIndex` 设置为该项索引，再打开预览。单封面控件的 CoverIndex 只决定关闭态封面；点击单封面只打开预览，不隐式把
CurrentIndex 同步为 CoverIndex。

稳定 template part 契约：

| Template Part | AtomUI 节点 | 职责 | 稳定性 |
| --- | --- | --- | --- |
| `PART_CoverItemsControl` | `ImageGroupPreviewerTheme` | group 封面集合 | template-stable |
| `PART_ImageViewerScene` | `ImageViewerTheme` | 预览图片坐标空间 | template-stable |
| `PART_ImageRenderer` | `ImageViewerTheme` | 只渲染 entry 已持有的 `IImage` | template-stable |
| `PART_LoadingPresenter` | `ImagePreviewerCoverTheme` / `ImageViewerTheme` | 封面 Skeleton / viewer Spin 状态占位 | template-stable |
| `PART_ErrorPresenter` | `ImagePreviewerCoverTheme` / `ImageViewerTheme` | 失败内容占位 | template-stable |
| `PART_PreviousButton` / `PART_NextButton` | `ImageViewerTheme` | 上一张 / 下一张导航 | template-stable |
| `PART_TitleLayout` / `PART_IconPresenter` | `ImagePreviewerTitleBarTheme` | dialog 标题与图标 | template-stable |
| `PART_CloseButton` | `ImagePreviewerOverlayHostTheme` | overlay 关闭入口 | template-stable |

| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |
| --- | --- | --- | --- | --- | --- |
| `root` | `AbstractImagePreviewer` | 归一 items、current、open state、宿主和加载策略 | `ItemsSource`、`CurrentIndex`、`IsOpen`、`PreloadCount` | SharedToken、ImagePreviewerToken | public |
| `cover` | `ImagePreviewer` / `PART_CoverItemsControl` | 展示单封面或 group 缩略图及 loading/error 状态 | Cover size/state、`ItemsPanel` | Cover size、mask、radius Token | public/template-stable |
| `host` | `ImagePreviewerDialog` / `ImagePreviewerOverlayHost` | 承载 Desktop window 或 Browser overlay | dialog、modal、topmost、title、motion API | Window、overlay、motion Token | internal-observable |
| `viewer` | `ImageViewer` / `PART_ImageViewerScene` | 当前项导航、fit、拖拽、缩放和旋转 | interaction、scale、CurrentIndex | Viewer background、toolbar Token | internal-observable |
| `renderer` | `PART_ImageRenderer` | 只渲染 entry 已持有的 `IImage` | current load state | 无独立加载 Token | template-stable |
| `loading` | `PART_LoadingPresenter` | 呈现 Skeleton 或 Spin，不拥有请求 | LoadingContent/Template | Loading、Skeleton、Spin Token | template-stable |
| `error` | `PART_ErrorPresenter` | 呈现最终失败内容，不改变状态机 | ErrorContent/Template | Error semantic Token | template-stable |

Semantic Part 公开契约（`root`、`image`、`cover` 与 `popup.*` 分组）见 [Semantic Parts](#semantic-parts) 章节。`PART_*`
名称继续只服务控件代码查找，与 Semantic Part 承担不同职责。

## 事件与命令

命令和事件：
- `ImageOpened`、`ImageFailed` 只投影当前完整图状态，事件参数包含 immutable item 与索引。

## 使用示例

稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。

### 基础用法

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:134`

Gallery key：`ExamplesContent` / item `0`

```axaml
<atom:ImagePreviewer Width="200"
```

### 远程图片加载

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:146`

Gallery key：`ExamplesContent` / item `1`

```axaml
<atom:ImagePreviewer Width="200"
```

### 容错

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:158`

Gallery key：`ExamplesContent` / item `2`

```axaml
<atom:ImagePreviewer Width="200"
```

### 20 张远程图片

来源：`controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml:170`

Gallery key：`ExamplesContent` / item `3`

```axaml
<atom:ImagePreviewer Width="200"
```

## 状态模型

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

## 主题与 Design Token

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

Token 来源：

ImagePreviewer Token 只表达组件级视觉变量，例如尺寸、间距、颜色、圆角、阴影、图标尺寸和弹层边界。Token 不承载运行时选择、展开、加载、错误、上传任务、过滤条件或业务状态。

当前 Token scope：

- `ImagePreviewerToken`，scope id 为 `ImagePreviewer`，源码位于 `src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerToken.cs`。

## AOT 与裁剪注意事项

- ItemsSource、entry、host 和 loader 之间没有 Visual -> business item 的反向长期引用。
- Previewer collection、host 和 entry 生命周期只管理 waiter 与 result lease；任何 Add/Remove/Replace/Move/Reset/Clear、host close
  或 detach 都不能触发 Application cache clear。
- cache membership、source/decode operation reference 和 result lease 是三个独立持有关系；LRU 驱逐先移除 membership，活动引用
  归零后再释放 owned image。
- 显示 tracker 重算热路径零分配；目标项与保留帧订阅严格经单一变更通道配对，`Immediate` 模式不持有保留帧（额外持有恒为 0），
  `WaitForLoaded` 至多持有 1 个保留帧 entry 及其解码图租约。
- entry Dispose 的重置通知使用静态缓存 EventArgs，无订阅者时零开销。
- 任何 cancellation、result dispose 或 event callback 都不在 Shared coordinator/cache lock 内由 Previewer执行。
- 不进行同步 HTTP/File I/O，不使用固定延迟，不创建私有 cache/scheduler。
- `ImageSource` reader、validator、codec 和 metadata serializer 使用静态注册或 source-generated serializer；Public data model、
  AXAML property 和 title resolver 都不依赖反射扫描。
- Browser overlay 与 Desktop dialog 共用同一 entry/load model；平台差异只位于宿主能力。
- Application dispose 统一取消底层 loader；控件仍负责尽快取消 waiter 和释放自身租约。
- borrowed `IImage` 始终由调用方拥有，loader、cache、entry 和 Application dispose 都不能销毁它。
- Semantic marker 使用静态 `Classes.semantic-*="True"`，编译为模板初始化的一次 `Classes.Set`，不建立 Binding、selector
  activator 或持久订阅；`popup.*` 部件不引入 VisualTree 搜索、运行时注册或反射发现。
- descriptor、marker 常量和生成 Semantic Style 全部由生成器静态产生；宿主只引用生成的类常量，不解析 route 字符串。

## 源码索引

`src/AtomUI.Desktop.Controls/ImagePreviewer/` 的稳定职责如下：

| 类型 | 职责 |
| --- | --- |
| `AbstractImagePreviewer` | public API、ItemsSource 物化、current entry、预加载策略和 dialog/overlay 生命周期 |
| `ImagePreviewItem` | immutable public 配置，包含 `ImageSource` 类型的 Source、Thumbnail、Fallback，以及 Options、Title 和 Tag |
| `ImagePreviewEntry` | internal Full/Thumbnail 状态、generation、取消、结果租约 owner；Dispose 前广播重置通知 |
| `ImagePreviewDisplayTracker` | internal 宿主显示状态机：目标项、tracker 会话标识、会话内单调目标序号与 WaitForLoaded 保留帧跟踪、显示图三值解析（见[切换显示设计](switch-display-design.md)） |
| `ImagePreviewer` | 单封面选择、状态投影和 `ReloadCover()` |
| `ImageGroupPreviewer` | 多封面 ItemsControl、点击索引和关闭态缩略图请求 |
| `ImagePreviewer.SemanticParts.cs` | `ImagePreviewer` owner 的 `[SemanticPart]` 声明（partial），生成 descriptor、marker 常量与强类型 Semantic Style |
| `ImageGroupPreviewer.SemanticParts.cs` | `ImageGroupPreviewer` owner 的 `[SemanticPart]` 声明（partial） |
| `ImagePreviewerDialog` | Desktop native window、标题算法、CurrentIndex relay 和 viewer 组合 |
| `ImagePreviewerOverlayHost` | Browser/无原生窗口平台的 overlay 宿主 |
| `ImageViewer` | 导航、变换、fit-to-window 和 loading/error 状态呈现 |
| `ImagePreviewRenderer` | 只渲染 entry 已持有的 `IImage` |
| `IImagePreviewTitleResolver` | 无 I/O 的标题解析扩展点 |

底层 `IImageLoader`、`ImageSource`、Request、Result、source snapshot、encoded/decoded store、scheduler、coordinator、transport
和 codec 位于 `AtomUI.Controls.Shared/ImageLoading/`。Previewer 目录不保留这些类型的副本。

## 相关文档

- 源设计文档：`docs/controls/desktop/data-display/image-previewer/overview.md`
- 实现文档：`docs/controls/desktop/data-display/image-previewer/implementation.md`
- Semantic Part 文档：`docs/controls/desktop/data-display/image-previewer/semantic-part.md`
- Token 文档：`docs/controls/desktop/data-display/image-previewer/token.md`
- 变更记录：`docs/controls/desktop/data-display/image-previewer/changelog.md`
- 语义结构：`./semantic-cn.md`
