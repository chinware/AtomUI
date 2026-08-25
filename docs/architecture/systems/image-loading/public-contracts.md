# 图片加载公共契约

> 状态：当前架构。本文与 [统一图片加载系统架构](overview.md) 共同定义引擎 public surface；控件 surface 由
> [统一图片加载控件 API](control-apis.md)单独拥有。

Shared 的公开图片类型、Builder 扩展和 Application 扩展位于 `AtomUI.Controls` namespace。目录名 `ImageLoading/` 只表达
源码 ownership，不额外制造 `AtomUI.Controls.ImageLoading` public namespace。

## 来源、请求和状态模型

所有加载型控件只接收 `ImageLoadSource`。该类型是封闭的来源值对象，不允许应用通过继承注入未注册的 source kind。

```csharp
[TypeConverter(typeof(ImageLoadSourceConverter))]
public sealed class ImageLoadSource
{
    public ImageLoadSourceKind Kind { get; }
    public string? DisplayName { get; }

    public static ImageLoadSource Parse(string source);
    public static bool TryParse(string? source, out ImageLoadSource? result);
    public static ImageLoadSource FromUri(string uri);
    public static ImageLoadSource FromUri(Uri uri);
    public static ImageLoadSource FromFile(string path);
    public static ImageLoadSource FromAsset(Uri uri);
    public static ImageLoadSource FromStorageFile(
        IStorageFile storageFile,
        string? cacheKey = null,
        string? version = null);
    public static ImageLoadSource FromBytes(
        ReadOnlyMemory<byte> bytes,
        string? cacheKey = null,
        string? version = null);
    public static ImageLoadSource FromStream(
        Func<CancellationToken, ValueTask<Stream>> openStream,
        string? cacheKey = null,
        string? version = null,
        string? displayName = null);
    public static ImageLoadSource FromImage(IImage image, string? cacheKey = null);
}
```

`ImageLoadSourceKind` 固定为 `Http`、`File`、`Asset`、`StorageFile`、`Bytes`、`Stream` 和 `Image`。
字符串转换只接受 `http`、`https`、`avares`、`file` 和平台绝对路径；未知 scheme 不回退为文件路径。
`Parse()`/`FromUri()` 对无效输入抛出 `FormatException`/`ArgumentException`，`TryParse()` 返回 false；
`UnsupportedScheme` 保留给合法 source kind 在当前平台不可执行等加载期错误。

`FromStream` 只接受可重复打开的工厂。每次调用必须返回新的可读流，loader 拥有并关闭该次返回流。
`FromFile` 在构造时捕获绝对规范路径；`FromBytes` 防御性复制输入，避免调用方后续修改破坏缓存身份。
`FromImage` 始终是 borrowed：loader、缓存和控件均不销毁调用方提供的 `IImage`，也不把它写入可释放 decoded cache。
其可选 `cacheKey` 只参与诊断/业务 variant，对象引用身份仍是 key 的必要部分，两个不同 `IImage` 不能仅因字符串相同而合并。
borrowed image 不执行重采样或格式校验，request 的 decode width/height 对它无效，result 报告调用方图像的实际尺寸。

每次请求的跨来源配置集中在一个对象中：

```csharp
public sealed record ImageRequestOptions
{
    public ImageCacheMode CacheMode { get; init; } = ImageCacheMode.Default;
    public string? CachePartition { get; init; }
    public string? Variant { get; init; }
    public TimeSpan? Timeout { get; init; }
    public IReadOnlyDictionary<string, string>? Headers { get; init; }
}
```

控件通过替换完整 `RequestOptions` 修改请求，不依赖对象内部 property change。`Headers` 按请求开始时再做防御性不可变快照，
调用方不得在提交后修改底层 dictionary。认证 header 存在而 `CachePartition` 为空时，请求强制按 `NoStore`
执行；控件不能覆盖这条安全降级。

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

    public void AddAuthenticationHeaderName(string name);
    public void AllowHttpOrigin(string origin);
    public void AllowCredentialForwardingOrigin(string origin);
}
```

`MaxConcurrentDownloads` 只限制 HTTP/HTTPS 下载。Asset、File、Storage、Bytes 和 Stream 使用独立的内部有界读取池，
因此慢网络请求不会阻塞本地图片，同时大量本地 source 也不会形成无限并发。

空 HTTP origin allowlist 表示应用未启用 host 收紧；一旦添加任一 origin，只有 allowlist 成员可以请求。credential forwarding
allowlist 只影响跨 origin redirect，不能允许 HTTPS downgrade。封闭 source kind 对应的 reader 与 codec registry 由 AtomUI
包显式静态注册；reader/codec 替换不属于应用 public plug-in API。
所有标量在 `UseAtomUI()` 构建阶段验证并冻结；控件运行后不能修改全局容量。重复调用 `UseImageLoading()` 合并到同一
应用注册，service factory 只注册一次；reader 按 source kind 唯一注册，codec 按稳定 Id 和 Version 去重，冲突属于启动错误。

`UseCommonControls()` 调用同一注册入口并增加受信任 `avares` SVG codec，因此
`UseDesktopControls()` 的应用无需额外注册。应用可以在同一个 `UseAtomUI()` configure callback 内显式调用
`UseImageLoading()` 覆盖默认容量；默认注册不得覆盖用户已经写入的显式值。完整链路见
[启动与注册链路](../../foundations/startup-and-registration.md)。

稳定枚举如下：

```text
ImageLoadState       Idle | Loading | Loaded | Failed
ImageCacheMode       Default | Reload | NoStore | CacheOnly
ImageDecodeMode      Auto | Original | Explicit
ImageRequestPriority Critical | High | Normal | Low | Preload
ImageCacheSource     None | DecodedMemory | EncodedMemory | Persistent | Revalidated | Network | Local
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
`InvalidImageData`、`DimensionLimitExceeded`、`PixelLimitExceeded`、`DecodedByteLimitExceeded`、
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
    public ImageLoadRequest(ImageLoadSource source);

    public ImageLoadSource Source { get; }
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
`ImageLoadError`。结果同时提供原始像素尺寸、实际解码尺寸、媒体类型、`ImageCacheSource` 和阶段耗时。
调用方不得直接 dispose `ImageLoadResult.Image`。

普通加载失败以失败 `ImageLoadResult` 返回，不因 404、格式错误或解码失败抛异常；参数错误、调用方取消和已销毁 loader
分别使用标准异常。每次成功调用返回独立结果租约，即使它们指向同一个 decoded cache entry；释放必须幂等。borrowed
`IImage` 的结果租约只控制使用期，不销毁原对象。

`Application.GetImageLoader()` 返回当前应用注册的 loader；`TryGetImageLoader()` 用于宿主和测试中的显式探测。
控件 public API 不提供 `Loader` 属性，避免单个控件绕过应用级调度、缓存和销毁边界。

控件如何消费这些契约见 [统一图片加载控件 API](control-apis.md)。
