namespace AtomUI.Theme.Definitions;

public sealed class ThemeDefinitionResolveResult
{
    public ThemeDefinitionResolveResult(
        IReadOnlyList<IThemeDefinitionSource> sources,
        IReadOnlyList<ThemeDiagnostic> diagnostics)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(diagnostics);

        var sourceCopy = sources.ToArray();
        if (sourceCopy.Any(static source => source is null))
        {
            throw new ArgumentException("Theme definition sources cannot contain null entries.", nameof(sources));
        }

        var diagnosticCopy = diagnostics.ToArray();
        if (diagnosticCopy.Any(static diagnostic => diagnostic is null))
        {
            throw new ArgumentException("Theme diagnostics cannot contain null entries.", nameof(diagnostics));
        }

        Sources     = Array.AsReadOnly(sourceCopy);
        Diagnostics = Array.AsReadOnly(diagnosticCopy);
    }

    public IReadOnlyList<IThemeDefinitionSource> Sources { get; }
    public IReadOnlyList<ThemeDiagnostic> Diagnostics { get; }
    public bool Success => Diagnostics.All(static diagnostic =>
        diagnostic.Severity != ThemeDiagnosticSeverity.Error);
}
