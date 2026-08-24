using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

public sealed class ImagePreviewOpenedEventArgs : EventArgs
{
    internal ImagePreviewOpenedEventArgs(ImagePreviewItem item, int index, ImageCacheSource cacheSource)
    {
        Item = item;
        Index = index;
        CacheSource = cacheSource;
    }

    public ImagePreviewItem Item { get; }

    public int Index { get; }

    public ImageCacheSource CacheSource { get; }
}

public sealed class ImagePreviewFailedEventArgs : EventArgs
{
    internal ImagePreviewFailedEventArgs(ImagePreviewItem item, int index, ImageLoadError error)
    {
        Item = item;
        Index = index;
        Error = error;
    }

    public ImagePreviewItem Item { get; }

    public int Index { get; }

    public ImageLoadError Error { get; }
}
