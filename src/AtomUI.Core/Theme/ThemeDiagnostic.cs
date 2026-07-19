namespace AtomUI.Theme;

public enum ThemeDiagnosticSeverity : byte
{
    Warning,
    Error
}

public sealed record ThemeDiagnostic(
    string Code,
    ThemeDiagnosticSeverity Severity,
    string Source,
    string Path,
    string Message);
