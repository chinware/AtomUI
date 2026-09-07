namespace AtomUI.Controls;

internal sealed record ImageSourceSnapshot(
    ImageSourceKey SourceKey,
    ImageSourceVersion SourceVersion,
    ImageContentId ContentId,
    long CommitGeneration,
    DateTimeOffset StoredAt,
    DateTimeOffset? FreshUntil = null,
    string? ETag = null,
    DateTimeOffset? LastModified = null,
    bool NoCache = false,
    bool MustRevalidate = false,
    bool IsRemote = false,
    bool IsTrustedAsset = false,
    string[]? VaryHeaders = null,
    string? VaryDigest = null,
    DateTimeOffset? ResponseDate = null,
    TimeSpan? ResponseAge = null,
    DateTimeOffset? Expires = null,
    TimeSpan? MaxAge = null,
    bool IsPrivate = false,
    int SecurityPolicyVersion = 0)
{
    internal bool IsFresh(DateTimeOffset now) =>
        !NoCache && FreshUntil is not null && FreshUntil.Value > now;
}

internal readonly record struct ImageCacheEpoch(long Global, long Partition);
