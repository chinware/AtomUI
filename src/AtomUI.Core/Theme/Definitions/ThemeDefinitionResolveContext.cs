namespace AtomUI.Theme.Definitions;

public sealed class ThemeDefinitionResolveContext
{
    public ThemeDefinitionResolveContext(
        string applicationId,
        string applicationDataRoot,
        bool isReload,
        long generation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationId);
        ArgumentNullException.ThrowIfNull(applicationDataRoot);
        ArgumentOutOfRangeException.ThrowIfNegative(generation);

        ApplicationId       = applicationId;
        ApplicationDataRoot = applicationDataRoot;
        IsReload            = isReload;
        Generation          = generation;
    }

    public string ApplicationId { get; }
    public string ApplicationDataRoot { get; }
    public bool IsReload { get; }
    public long Generation { get; }
}
