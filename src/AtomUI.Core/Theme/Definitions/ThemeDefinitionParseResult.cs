namespace AtomUI.Theme.Definitions;

internal sealed record ThemeDefinitionParseResult
{
    public ThemeDefinition? Definition { get; }
    public IReadOnlyList<ThemeDefinitionDiagnostic> Diagnostics { get; }

    public bool Success =>
        Definition is not null &&
        Diagnostics.All(static diagnostic => diagnostic.Severity != ThemeDiagnosticSeverity.Error);

    public ThemeDefinitionParseResult(
        ThemeDefinition? definition,
        IEnumerable<ThemeDefinitionDiagnostic> diagnostics)
    {
        Definition  = definition;
        Diagnostics = Array.AsReadOnly(diagnostics.ToArray());
    }
}
