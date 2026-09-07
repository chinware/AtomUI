namespace AtomUI.Controls;

public interface IImageLoadControl
{
    ImageSource? Source { get; set; }

    ImageSource? FallbackSource { get; set; }

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
        ImageSource source,
        bool isFallback,
        ImageLoadOrigin origin,
        int pixelWidth,
        int pixelHeight)
    {
        Source = source;
        IsFallback = isFallback;
        Origin = origin;
        PixelWidth = pixelWidth;
        PixelHeight = pixelHeight;
    }

    public ImageSource Source { get; }

    public bool IsFallback { get; }

    public ImageLoadOrigin Origin { get; }

    public int PixelWidth { get; }

    public int PixelHeight { get; }
}

public sealed class ImageFailedEventArgs : EventArgs
{
    internal ImageFailedEventArgs(ImageSource source, bool isFallback, ImageLoadError error)
    {
        Source = source;
        IsFallback = isFallback;
        Error = error;
    }

    public ImageSource Source { get; }

    public bool IsFallback { get; }

    public ImageLoadError Error { get; }
}
