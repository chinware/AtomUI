using AtomUI.Theme.Definitions;

namespace AtomUI.Theme.Catalog;

internal sealed record ThemeDescriptor
{
    internal ThemeDescriptor(
        string id,
        string definitionFilePath,
        bool isBuiltIn,
        int sourcePriority,
        ThemeDefinition? definition,
        IEnumerable<ThemeDefinitionDiagnostic> diagnostics)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(definitionFilePath);
        ArgumentNullException.ThrowIfNull(diagnostics);

        Id                 = id;
        DefinitionFilePath = definitionFilePath;
        IsBuiltIn          = isBuiltIn;
        SourcePriority     = sourcePriority;
        Definition         = definition;
        Diagnostics        = Array.AsReadOnly(diagnostics.ToArray());
    }

    public string Id { get; }
    public string DefinitionFilePath { get; }
    public bool IsBuiltIn { get; }
    public int SourcePriority { get; }
    public ThemeDefinition? Definition { get; }
    public IReadOnlyList<ThemeDefinitionDiagnostic> Diagnostics { get; }

    public bool IsAvailable =>
        Definition is not null &&
        Diagnostics.All(static diagnostic => diagnostic.Severity != ThemeDiagnosticSeverity.Error);

    internal ThemeDescriptor WithAdditionalDiagnostic(ThemeDefinitionDiagnostic diagnostic)
    {
        ArgumentNullException.ThrowIfNull(diagnostic);
        return new ThemeDescriptor(
            Id,
            DefinitionFilePath,
            IsBuiltIn,
            SourcePriority,
            Definition,
            Diagnostics.Append(diagnostic));
    }
}
