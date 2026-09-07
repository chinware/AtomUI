namespace AtomUI.Controls;

public sealed class ImageLoaderEventArgs : EventArgs
{
    internal ImageLoaderEventArgs(
        ImageLoaderEventKind kind,
        ImageSourceKind sourceKind,
        ImageLoadOrigin? origin,
        ImageLoadErrorCode? errorCode,
        TimeSpan elapsed)
    {
        Kind = kind;
        SourceKind = sourceKind;
        Origin = origin;
        ErrorCode = errorCode;
        Elapsed = elapsed;
    }

    public ImageLoaderEventKind Kind { get; }

    public ImageSourceKind SourceKind { get; }

    public ImageLoadOrigin? Origin { get; }

    public ImageLoadErrorCode? ErrorCode { get; }

    public TimeSpan Elapsed { get; }
}
