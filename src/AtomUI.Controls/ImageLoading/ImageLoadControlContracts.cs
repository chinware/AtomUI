namespace AtomUI.Controls;

public interface IImageLoadControl
{
    ImageLoadSource? Source { get; set; }

    ImageLoadSource? FallbackSource { get; set; }

    ImageRequestOptions? RequestOptions { get; set; }

    ImageLoadState LoadState { get; }

    ImageLoadError? LoadError { get; }

    ImageLoadProgress? LoadProgress { get; }

    bool IsLoading { get; }

    bool IsLoaded { get; }

    bool IsFailed { get; }

    event EventHandler<ImageOpenedEventArgs>? ImageOpened;

    event EventHandler<ImageFailedEventArgs>? ImageFailed;

    void Reload();
}

public sealed class ImageOpenedEventArgs : EventArgs
{
    internal ImageOpenedEventArgs(
        ImageLoadSource source,
        bool isFallback,
        ImageCacheSource cacheSource,
        int pixelWidth,
        int pixelHeight)
    {
        Source = source;
        IsFallback = isFallback;
        CacheSource = cacheSource;
        PixelWidth = pixelWidth;
        PixelHeight = pixelHeight;
    }

    public ImageLoadSource Source { get; }

    public bool IsFallback { get; }

    public ImageCacheSource CacheSource { get; }

    public int PixelWidth { get; }

    public int PixelHeight { get; }
}

public sealed class ImageFailedEventArgs : EventArgs
{
    internal ImageFailedEventArgs(ImageLoadSource source, bool isFallback, ImageLoadError error)
    {
        Source = source;
        IsFallback = isFallback;
        Error = error;
    }

    public ImageLoadSource Source { get; }

    public bool IsFallback { get; }

    public ImageLoadError Error { get; }
}
