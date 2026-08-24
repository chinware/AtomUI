namespace AtomUI.Controls;

public enum ImageLoadSourceKind
{
    Http,
    File,
    Asset,
    StorageFile,
    Bytes,
    Stream,
    Image
}

public enum ImageLoadState
{
    Idle,
    Loading,
    Loaded,
    Failed
}

public enum ImageCacheMode
{
    Default,
    Reload,
    NoStore,
    CacheOnly
}

public enum ImageDecodeMode
{
    Auto,
    Original,
    Explicit
}

public enum ImageRequestPriority
{
    Critical,
    High,
    Normal,
    Low,
    Preload
}

public enum ImageCacheSource
{
    None,
    DecodedMemory,
    EncodedMemory,
    Persistent,
    Revalidated,
    Network,
    Local
}

public enum ImageLoadStage
{
    Resolving,
    CacheLookup,
    Queued,
    Reading,
    Downloading,
    Validating,
    Decoding
}

public enum ImageLoadErrorCode
{
    InvalidSource,
    UnsupportedScheme,
    AccessDenied,
    NotFound,
    NetworkFailure,
    Timeout,
    HttpStatus,
    TooManyRedirects,
    CacheMiss,
    ResponseTooLarge,
    RedirectBlocked,
    OriginNotAllowed,
    ContentTypeMismatch,
    UnsupportedFormat,
    UnsafeVectorContent,
    InvalidImageData,
    DimensionLimitExceeded,
    PixelLimitExceeded,
    DecodedByteLimitExceeded,
    AnimationNotSupported,
    DecodeFailed
}

public enum ImageLoaderEventKind
{
    Started,
    CacheHit,
    Completed,
    Failed,
    Canceled
}
