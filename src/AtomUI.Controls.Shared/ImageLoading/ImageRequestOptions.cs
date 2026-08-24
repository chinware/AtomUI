namespace AtomUI.Controls;

public sealed record ImageRequestOptions
{
    public ImageCacheMode CacheMode { get; init; } = ImageCacheMode.Default;

    public string? CachePartition { get; init; }

    public string? Variant { get; init; }

    public TimeSpan? Timeout { get; init; }

    public IReadOnlyDictionary<string, string>? Headers { get; init; }
}
