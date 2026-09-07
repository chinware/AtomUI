# 图片加载公共契约

> 状态：当前架构。本文与 [统一图片加载系统架构](overview.md) 共同定义引擎 public surface；控件 surface 由
> [统一图片加载控件 API](control-apis.md)单独拥有。

Shared 的公开图片类型、Builder 扩展和 Application 扩展位于 `AtomUI.Controls` namespace。目录名 `ImageLoading/` 只表达
源码 ownership，不额外制造 `AtomUI.Controls.ImageLoading` public namespace。

## 来源、请求和状态模型

所有加载型控件只接收 `ImageSource`。它是封闭的抽象基类：外部程序集不能调用其构造函数，也不能注入未注册的 source kind；
所有具体来源均为不可变 sealed 类型。

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

诊断使用的 `ImageSourceKind` 固定为 `Http`、`File`、`Asset`、`StorageFile`、`Bytes`、`Stream` 和 `Borrowed`。
字符串转换只接受 `http`、`https`、`avares`、`file` 和平台绝对路径；未知 scheme 不回退为文件路径。
`Parse()` 对无效输入抛出 `FormatException`，具体来源构造器对无效参数抛出 `ArgumentException`，`TryParse()` 返回 false；
`UnsupportedScheme` 保留给合法 source kind 在当前平台不可执行等加载期错误。

`StreamImageSource` 只接受可重复打开的工厂，每次调用必须返回新的可读流，loader 拥有并关闭该次返回流。
`FileImageSource` 在构造时捕获绝对规范路径；`BytesImageSource` 防御性复制输入及公开读取结果，避免调用方修改破坏内容身份。
`BorrowedImageSource` 始终是 borrowed：loader、缓存和控件均不销毁调用方提供的 `IImage`，也不把它写入可释放 decoded cache。
对象引用身份始终参与 borrowed source identity，两个不同 `IImage` 不能仅因显示名相同而合并。
borrowed image 不执行重采样或格式校验，request 的 decode width/height 对它无效，result 报告调用方图像的实际尺寸。

`FileImageSource` 默认采用 `ImageFileValidationMode.Metadata`，比较当前打开文件可观察的 identity/creation、长度和修改令牌；
必须识别“字节已变但可观察元数据保持不变”的场景时使用 `ContentHash`，它会在每次普通读取时重新读取并计算内容摘要。
`StorageFileImageSource` 和 `StreamImageSource` 的 revision 是调用方承诺：revision 不变时内容不变；无法承诺时必须省略。

每次请求的跨来源配置集中在一个对象中：

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
```

控件通过替换完整 `RequestOptions` 修改请求，不依赖对象内部 property change。`Headers` 在请求开始时做防御性不可变快照；
调用方随后修改原 dictionary 不会影响已经开始的请求。认证 header 存在而 `CachePartition` 为空时，请求禁止共享读取、共享写入、请求合并和
持久化，并把本次有效 `CacheStorage` 收紧为 `None`；控件不能覆盖这条安全降级。

应用级容量、并发、安全上限和扩展点通过 `UseImageLoading()` 配置，而不是散落在控件属性上：

```csharp
builder.UseImageLoading(options =>
{
    options.MaxConcurrentDownloads = 6;
    options.MaxConcurrentDecodes = 4;
    options.EncodedMemoryCacheBytes = 128 * 1024 * 1024;
    options.DecodedMemoryCacheBytes = 256 * 1024 * 1024;
});
```

当前配置面完整收敛为：

```csharp
public sealed class ImageLoadingOptionsBuilder
{
    public int MaxConcurrentDownloads { get; set; }
    public int MaxConcurrentDecodes { get; set; }

    public long EncodedMemoryCacheBytes { get; set; }
    public int EncodedMemoryCacheEntries { get; set; }
    public long DecodedMemoryCacheBytes { get; set; }
    public int DecodedMemoryCacheEntries { get; set; }

    public long MaxResponseBytes { get; set; }
    public int MaxImageWidth { get; set; }
    public int MaxImageHeight { get; set; }
    public long MaxImagePixelCount { get; set; }
    public long MaxDecodedImageBytes { get; set; }
    public TimeSpan DefaultRequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRedirects { get; set; } = 8;

    public bool IsPersistentCacheEnabled { get; set; }
    public string? PersistentCacheDirectory { get; set; }
    public long PersistentCacheBytes { get; set; }
    public int PersistentCacheEntries { get; set; }
    public bool AllowAuthenticatedPersistentCache { get; set; }

    public SvgImageLoadingOptionsBuilder Svg { get; }

    public void AddAuthenticationHeaderName(string name);
    public void AllowHttpOrigin(string origin);
    public void AllowCredentialForwardingOrigin(string origin);
}
```

SVG 配置只调整有界资源预算，不允许应用放开脚本、DTD 或外部资源：

```csharp
public sealed class SvgImageLoadingOptionsBuilder
{
    public long MaxDocumentBytes { get; set; } = 4L * 1024 * 1024;
    public long MaxXmlCharacters { get; set; } = 8_000_000;
    public int MaxElementCount { get; set; } = 20_000;
    public int MaxAttributeCount { get; set; } = 100_000;
    public int MaxElementDepth { get; set; } = 256;
    public int MaxReferenceDepth { get; set; } = 64;
    public long MaxPathDataCharacters { get; set; } = 2_000_000;
    public int MaxEmbeddedImageCount { get; set; } = 16;
    public long MaxEmbeddedImageBytes { get; set; } = 8L * 1024 * 1024;
}
```

`MaxConcurrentDownloads` 只限制 HTTP/HTTPS 下载。Asset、File、Storage、Bytes 和 Stream 使用独立的内部有界读取池，
因此慢网络请求不会阻塞本地图片，同时大量本地 source 也不会形成无限并发。

空 HTTP origin allowlist 表示应用未启用 host 收紧；一旦添加任一 origin，只有 allowlist 成员可以请求。credential forwarding
allowlist 只影响跨 origin redirect，不能允许 HTTPS downgrade。封闭 source kind 对应的 reader 与 codec registry 由 AtomUI
包显式静态注册；reader/codec 替换不属于应用 public plug-in API。
所有标量在 `UseAtomUI()` 构建阶段验证并冻结；控件运行后不能修改全局容量。重复调用 `UseImageLoading()` 合并到同一
应用注册，service factory 只注册一次；reader 按 source kind 唯一注册，codec 按稳定 Id 和 Version 去重，冲突属于启动错误。

`UseCommonControls()` 调用同一注册入口并增加统一 `SvgImageCodec`，因此 HTTP、File、Asset、Storage、Bytes 和 Stream 中
通过安全验证的静态 SVG 都使用同一个 codec；renderer 不自行打开 URL/File。完整安全子集见
[网络 SVG 加载设计](network-svg.md)。因此
`UseDesktopControls()` 的应用无需额外注册。应用可以在同一个 `UseAtomUI()` configure callback 内显式调用
`UseImageLoading()` 覆盖默认容量；默认注册不得覆盖用户已经写入的显式值。完整链路见
[启动与注册链路](../../foundations/startup-and-registration.md)。

稳定枚举如下：

```text
ImageLoadState       Idle | Loading | Loaded | Failed
ImageCacheReadPolicy ValidateSource | RefreshSource | PreferCache | CacheOnly
ImageCacheStoragePolicy None | Memory | MemoryAndDisk
ImageDecodeMode      Auto | Original | Explicit
ImageRequestPriority Critical | High | Normal | Low | Preload
ImageLoadOrigin      Borrowed | DecodedMemory | EncodedMemory | Persistent | Network | Local
ImageSourceValidation NotRequired | Current | Revalidated | Unverified
```

Loader 的进度是阶段化快照，不以不可靠的百分比冒充确定进度：

```csharp
public readonly record struct ImageLoadProgress(
    ImageLoadStage Stage,
    long BytesReceived,
    long? TotalBytes,
    double? Fraction);
```

`ImageLoadStage` 固定为 `Resolving`、`CacheLookup`、`Queued`、`Reading`、`Downloading`、`Validating` 和
`Decoding`。只有读取/下载总长度已知且大于零时 `Fraction` 才有值；缓存命中和解码阶段允许没有字节进度。控件只发布当前
generation 的进度。

`ImageLoadErrorCode` 至少覆盖 `InvalidSource`、`UnsupportedScheme`、`AccessDenied`、`NotFound`、
`NetworkFailure`、`Timeout`、`HttpStatus`、`TooManyRedirects`、`CacheMiss`、`ResponseTooLarge`、
`RedirectBlocked`、`OriginNotAllowed`、`ContentTypeMismatch`、`UnsupportedFormat`、`UnsafeVectorContent`、
`InvalidImageData`、`VectorComplexityLimitExceeded`、`EmbeddedResourceLimitExceeded`、
`DimensionLimitExceeded`、`PixelLimitExceeded`、`DecodedByteLimitExceeded`、
`AnimationNotSupported` 和 `DecodeFailed`。调用方取消仍抛出
`OperationCanceledException`；loader 已销毁属于编程错误，抛出 `ObjectDisposedException`，不伪装成图片失败。

`ImageLoadError` 是不可变值，至少包含 `Code`、安全的用户可显示 `Message`、可空 HTTP status、来源显示名和内部
`Exception`。异常不得进入缓存键、事件参数字符串化或日志默认消息；URI user-info、认证 header、Cookie 和
`CachePartition` 原值不得出现在错误文本中。

## Loader 与结果租约

```csharp
public interface IImageLoader
{
    ValueTask<ImageLoadResult> LoadAsync(
        ImageLoadRequest request,
        CancellationToken cancellationToken = default);

    ValueTask ClearCacheAsync(
        ImageCacheClearRequest request,
        CancellationToken cancellationToken = default);

    ImageLoaderSnapshot Snapshot { get; }
    event EventHandler<ImageLoaderEventArgs>? LoadEvent;
}
```

```csharp
public sealed record ImageCacheClearRequest
{
    public string? CachePartition { get; init; }
    public bool ClearDecodedMemory { get; init; } = true;
    public bool ClearEncodedMemory { get; init; } = true;
    public bool ClearPersistent { get; init; } = true;
    public bool CancelInFlight { get; init; } = true;
}
```

可空 `CachePartition` 选择全部或单个分区。清理递增目标 cache scope epoch，旧 operation 即使在取消竞态中成功，也不能把结果
重新插入已清理分区。已有结果租约继续有效，最后一个租约释放时再销毁图片。控件不暴露清缓存 API，登录/注销、租户切换和
运维入口通过应用级 `IImageLoader` 调用。

请求对象明确使用物理像素，不让 loader 依赖 Control 或 VisualTree：

```csharp
public sealed class ImageLoadRequest
{
    public ImageLoadRequest(ImageSource source);

    public ImageSource Source { get; }
    public ImageRequestOptions? Options { get; init; }
    public int DecodePixelWidth { get; init; }
    public int DecodePixelHeight { get; init; }
    public ImageRequestPriority Priority { get; init; } = ImageRequestPriority.Normal;
    public IProgress<ImageLoadProgress>? Progress { get; init; }
}
```

宽高都为零表示原始尺寸；任一非零时表示保持宽高比的最大物理像素边界。负数在入口抛出
`ArgumentOutOfRangeException`。`ImageDecodeMode.Auto` 和 layout-to-physical-pixel 换算属于控件层；loader 只消费已经解析的
像素目标。

`ImageRequestOptions.Timeout` 是单 waiter 的端到端期限，从 `LoadAsync()` 接纳请求开始，覆盖排队、读取/下载、校验和解码。
同一共享 operation 上不同 waiter 可以有不同 timeout；一个 waiter 超时只按取消该 waiter 处理，最后 waiter 离开才取消底层工作。
控件主 Source 和 FallbackSource 是两个连续 attempt，各自使用相同 timeout 配置。

`ImageLoadResult` 实现 `IDisposable`。成功结果的 `Image` 只在结果未释放期间有效；失败结果没有 Image，携带
`ImageLoadError`。成功结果同时提供原始像素尺寸、实际解码尺寸、媒体类型、`ImageLoadOrigin`、
`ImageSourceValidation`、SHA-256 `ContentId` 和阶段耗时。`Origin` 只回答字节/图片从何处交付，`SourceValidation` 独立回答
本次请求如何确认来源映射；例如 HTTP `304` 可以同时是 `DecodedMemory` 与 `Revalidated`。
调用方不得直接 dispose `ImageLoadResult.Image`。

普通加载失败以失败 `ImageLoadResult` 返回，不因 404、格式错误或解码失败抛异常；参数错误、调用方取消和已销毁 loader
分别使用标准异常。每次成功调用返回独立结果租约，即使它们指向同一个 decoded cache entry；释放必须幂等。borrowed
`IImage` 的结果租约只控制使用期，不销毁原对象。

`Application.GetImageLoader()` 返回当前应用注册的 loader；`TryGetImageLoader()` 用于宿主和测试中的显式探测。
控件 public API 不提供 `Loader` 属性，避免单个控件绕过应用级调度、缓存和销毁边界。

控件如何消费这些契约见 [统一图片加载控件 API](control-apis.md)。
