namespace AtomUI.Theme.Definitions;

internal enum ThemeDefinitionDiagnosticSeverity
{
    Warning,
    Error
}

internal sealed record ThemeDefinitionDiagnostic(
    string Code,
    ThemeDefinitionDiagnosticSeverity Severity,
    string FilePath,
    int Line,
    int Column,
    string Path,
    string Message);
