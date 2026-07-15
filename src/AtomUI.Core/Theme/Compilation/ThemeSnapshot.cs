using AtomUI.Theme.Resources;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Compilation;

internal sealed class ThemeSnapshot
{
    internal ThemeSnapshot(
        string id,
        long version,
        IReadOnlyList<ThemeAlgorithm> algorithms,
        bool isDark,
        DesignToken sharedToken,
        IReadOnlyDictionary<object, object?> sharedResources,
        IReadOnlyDictionary<ComponentTokenIdentity, ComponentThemeSnapshot> components)
    {
        Id              = id;
        Version         = version;
        Algorithms      = algorithms;
        IsDark          = isDark;
        SharedToken     = sharedToken;
        SharedResources = sharedResources;
        Components      = components;
    }

    public string Id { get; }
    public long Version { get; }
    public IReadOnlyList<ThemeAlgorithm> Algorithms { get; }
    public bool IsDark { get; }
    public DesignToken SharedToken { get; }
    public IReadOnlyDictionary<object, object?> SharedResources { get; }
    public IReadOnlyDictionary<ComponentTokenIdentity, ComponentThemeSnapshot> Components { get; }
}
