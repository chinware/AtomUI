using Avalonia.Media;

namespace AtomUI.Controls;

public sealed class ImageLoadResult : IDisposable
{
    private IDisposable? _lease;

    internal ImageLoadResult(
        IImage image,
        IDisposable lease,
        int originalPixelWidth,
        int originalPixelHeight,
        int decodedPixelWidth,
        int decodedPixelHeight,
        string? mediaType,
        ImageLoadOrigin origin,
        ImageSourceValidation sourceValidation,
        string? contentId,
        IReadOnlyDictionary<ImageLoadStage, TimeSpan>? stageDurations = null)
    {
        Image = image ?? throw new ArgumentNullException(nameof(image));
        _lease = lease ?? throw new ArgumentNullException(nameof(lease));
        OriginalPixelWidth = originalPixelWidth;
        OriginalPixelHeight = originalPixelHeight;
        DecodedPixelWidth = decodedPixelWidth;
        DecodedPixelHeight = decodedPixelHeight;
        MediaType = mediaType;
        Origin = origin;
        SourceValidation = sourceValidation;
        ContentId = contentId;
        StageDurations = stageDurations ?? EmptyDurations;
    }

    internal ImageLoadResult(
        ImageLoadError error,
        IReadOnlyDictionary<ImageLoadStage, TimeSpan>? stageDurations = null)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
        StageDurations = stageDurations ?? EmptyDurations;
    }

    public bool IsSuccess => Image is not null;

    public IImage? Image { get; }

    public ImageLoadError? Error { get; }

    public int OriginalPixelWidth { get; }

    public int OriginalPixelHeight { get; }

    public int DecodedPixelWidth { get; }

    public int DecodedPixelHeight { get; }

    public string? MediaType { get; }

    public ImageLoadOrigin Origin { get; }

    public ImageSourceValidation SourceValidation { get; }

    public string? ContentId { get; }

    public IReadOnlyDictionary<ImageLoadStage, TimeSpan> StageDurations { get; }

    public void Dispose()
    {
        Interlocked.Exchange(ref _lease, null)?.Dispose();
    }

    private static IReadOnlyDictionary<ImageLoadStage, TimeSpan> EmptyDurations { get; } =
        new Dictionary<ImageLoadStage, TimeSpan>();
}
