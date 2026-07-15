namespace AtomUI.Theme.Definitions;

internal enum ThemeDiagnosticSeverity
{
    Warning,
    Error
}

internal sealed record ThemeDefinitionDiagnostic(
    string Code,
    ThemeDiagnosticSeverity Severity,
    string FilePath,
    int Line,
    int Column,
    string Path,
    string Message);
