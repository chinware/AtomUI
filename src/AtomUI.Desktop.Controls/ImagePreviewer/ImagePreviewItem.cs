using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

public sealed record ImagePreviewItem
{
    public ImagePreviewItem(ImageLoadSource source)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public ImageLoadSource Source { get; init; }

    public ImageLoadSource? ThumbnailSource { get; init; }

    public ImageLoadSource? FallbackSource { get; init; }

    public ImageRequestOptions? RequestOptions { get; init; }

    public string? Title { get; init; }

    public object? Tag { get; init; }
}
