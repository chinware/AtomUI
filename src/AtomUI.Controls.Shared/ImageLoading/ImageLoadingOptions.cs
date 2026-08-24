using System.Collections.Frozen;

namespace AtomUI.Controls;

internal sealed record ImageLoadingOptions(
    int MaxConcurrentDownloads,
    int MaxConcurrentDecodes,
    long EncodedMemoryCacheBytes,
    int EncodedMemoryCacheEntries,
    long DecodedMemoryCacheBytes,
    int DecodedMemoryCacheEntries,
    long MaxResponseBytes,
    int MaxImageWidth,
    int MaxImageHeight,
    long MaxImagePixelCount,
    long MaxDecodedImageBytes,
    TimeSpan DefaultRequestTimeout,
    int MaxRedirects,
    bool IsPersistentCacheEnabled,
    string? PersistentCacheDirectory,
    long PersistentCacheBytes,
    int PersistentCacheEntries,
    bool AllowAuthenticatedPersistentCache,
    FrozenSet<string> AuthenticationHeaderNames,
    FrozenSet<string> AllowedHttpOrigins,
    FrozenSet<string> CredentialForwardingOrigins);
