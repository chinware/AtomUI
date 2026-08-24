namespace AtomUI.Controls;

public sealed class ImageLoaderEventArgs : EventArgs
{
    internal ImageLoaderEventArgs(
        ImageLoaderEventKind kind,
        ImageLoadSourceKind sourceKind,
        ImageCacheSource cacheSource,
        ImageLoadErrorCode? errorCode,
        TimeSpan elapsed)
    {
        Kind = kind;
        SourceKind = sourceKind;
        CacheSource = cacheSource;
        ErrorCode = errorCode;
        Elapsed = elapsed;
    }

    public ImageLoaderEventKind Kind { get; }

    public ImageLoadSourceKind SourceKind { get; }

    public ImageCacheSource CacheSource { get; }

    public ImageLoadErrorCode? ErrorCode { get; }

    public TimeSpan Elapsed { get; }
}
