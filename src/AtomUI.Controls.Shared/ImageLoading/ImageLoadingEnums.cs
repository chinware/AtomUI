namespace AtomUI.Controls;

public enum ImageSourceKind
{
    Http,
    File,
    Asset,
    StorageFile,
    Bytes,
    Stream,
    Borrowed
}

public enum ImageFileValidationMode
{
    Metadata,
    ContentHash
}

public enum ImageLoadState
{
    Idle,
    Loading,
    Loaded,
    Failed
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
    VectorComplexityLimitExceeded,
    EmbeddedResourceLimitExceeded,
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
