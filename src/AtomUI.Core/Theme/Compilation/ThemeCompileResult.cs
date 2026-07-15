using AtomUI.Theme.Definitions;

namespace AtomUI.Theme.Compilation;

internal sealed record ThemeCompileResult(
    ThemeSnapshot? Snapshot,
    IReadOnlyList<ThemeDefinitionDiagnostic> Diagnostics,
    Exception? Exception)
{
    public bool Success => Snapshot is not null && Exception is null;
}
