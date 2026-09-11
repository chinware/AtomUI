namespace AtomUI.Controls;

public sealed record ImageRequestOptions
{
    public ImageCacheReadPolicy CacheRead { get; init; } = ImageCacheReadPolicy.ValidateSource;

    public ImageCacheStoragePolicy CacheStorage { get; init; } = ImageCacheStoragePolicy.MemoryAndDisk;

    public string? CachePartition { get; init; }

    public string? Variant { get; init; }

    public TimeSpan? Timeout { get; init; }

    public IReadOnlyDictionary<string, string>? Headers { get; init; }
}
