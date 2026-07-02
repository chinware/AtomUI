namespace AtomUI.Docs.LLMsGenerator.Config;

public sealed class LLMsGeneratorConfig
{
    public int SchemaVersion { get; init; }

    public string ProjectId { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string DefaultLanguage { get; init; } = string.Empty;

    public List<string> Languages { get; init; } = [];

    public string OutputRoot { get; init; } = string.Empty;

    public LLMsVisibilityConfig Visibility { get; init; } = new();

    public List<LLMsControlSetConfig> ControlSets { get; init; } = [];
}

public sealed class LLMsVisibilityConfig
{
    public string Default { get; init; } = "public";

    public List<string> Included { get; init; } = [];

    public List<string> Excluded { get; init; } = [];
}

public sealed class LLMsControlSetConfig
{
    public string Id { get; init; } = string.Empty;

    public string Platform { get; init; } = string.Empty;

    public string DocsRoot { get; init; } = string.Empty;

    public string GalleryRoot { get; init; } = string.Empty;

    public List<string> SourceRoots { get; init; } = [];

    public List<string> CategoryOrder { get; init; } = [];
}
