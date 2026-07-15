using AtomUI.Theme.Resources;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Compilation;

internal sealed record ThemeCompileRequest(
    string ThemeId,
    ThemeDefinition Definition,
    ThemeSnapshot? Parent,
    IReadOnlyList<ThemeAlgorithm> Algorithms,
    IReadOnlyDictionary<string, string> SharedOverrides,
    IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> ComponentOverrides,
    IReadOnlyList<ControlTokenRegistration> Registrations,
    IReadOnlyDictionary<string, string> RuntimeOverrides)
{
    internal ThemeSnapshotCacheKey CreateSnapshotCacheKey()
    {
        return ThemeSnapshotCacheKey.Create(this);
    }
}
