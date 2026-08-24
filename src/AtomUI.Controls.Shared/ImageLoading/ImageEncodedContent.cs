namespace AtomUI.Controls;

internal sealed record ImageEncodedContent(
    byte[] Bytes,
    string? MediaType,
    ImageCacheSource CacheSource,
    DateTimeOffset StoredAt,
    DateTimeOffset? FreshUntil = null,
    string? ETag = null,
    DateTimeOffset? LastModified = null,
    bool NoStore = false,
    bool NoCache = false,
    bool MustRevalidate = false,
    bool IsRemote = false,
    bool IsTrustedAsset = false,
    string? SourceVersion = null,
    string[]? VaryHeaders = null,
    string? VaryDigest = null,
    int SecurityPolicyVersion = 1,
    DateTimeOffset? ResponseDate = null,
    TimeSpan? ResponseAge = null,
    DateTimeOffset? Expires = null,
    TimeSpan? MaxAge = null,
    bool IsPrivate = false)
{
    internal long Size => Bytes.LongLength;

    internal bool IsFresh(DateTimeOffset now) =>
        !NoCache && FreshUntil is not null && FreshUntil.Value > now;

    internal ImageEncodedContent WithCacheSource(ImageCacheSource source) => this with { CacheSource = source };
}
