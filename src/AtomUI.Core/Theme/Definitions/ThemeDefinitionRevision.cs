namespace AtomUI.Theme.Definitions;

internal readonly record struct ThemeDefinitionRevision
{
    internal ThemeDefinitionRevision(
        string sourceIdentity,
        string sourceRevision,
        string contentDigest)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceIdentity);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceRevision);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentDigest);

        SourceIdentity = sourceIdentity;
        SourceRevision = sourceRevision;
        ContentDigest  = contentDigest;
    }

    internal string SourceIdentity { get; }
    internal string SourceRevision { get; }
    internal string ContentDigest { get; }
}
