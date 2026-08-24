namespace AtomUI.Controls;

public sealed class ImageLoadRequest
{
    private int _decodePixelWidth;
    private int _decodePixelHeight;

    public ImageLoadRequest(ImageLoadSource source)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public ImageLoadSource Source { get; }

    public ImageRequestOptions? Options { get; init; }

    public int DecodePixelWidth
    {
        get => _decodePixelWidth;
        init => _decodePixelWidth = value >= 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value));
    }

    public int DecodePixelHeight
    {
        get => _decodePixelHeight;
        init => _decodePixelHeight = value >= 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value));
    }

    public ImageRequestPriority Priority { get; init; } = ImageRequestPriority.Normal;

    public IProgress<ImageLoadProgress>? Progress { get; init; }
}
