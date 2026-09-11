namespace AtomUI.Controls;

public readonly record struct ImageLoaderSnapshot(
    int ActiveReads,
    int QueuedReads,
    int ActiveDecodes,
    int QueuedDecodes,
    int EncodedCacheEntries,
    long EncodedCacheBytes,
    int DecodedCacheEntries,
    long DecodedCacheBytes,
    long CacheHits,
    long CacheMisses,
    long CanceledLoads,
    long FailedLoads);
