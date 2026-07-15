using System.Collections.ObjectModel;
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
        Resources       = BuildResourceMap(sharedResources, components);
    }

    public string Id { get; }
    public long Version { get; }
    public IReadOnlyList<ThemeAlgorithm> Algorithms { get; }
    public bool IsDark { get; }
    public DesignToken SharedToken { get; }
    public IReadOnlyDictionary<object, object?> SharedResources { get; }
    public IReadOnlyDictionary<ComponentTokenIdentity, ComponentThemeSnapshot> Components { get; }
    internal IReadOnlyDictionary<object, object?> Resources { get; }

    private static IReadOnlyDictionary<object, object?> BuildResourceMap(
        IReadOnlyDictionary<object, object?> sharedResources,
        IReadOnlyDictionary<ComponentTokenIdentity, ComponentThemeSnapshot> components)
    {
        var resources = new Dictionary<object, object?>(sharedResources.Count);
        foreach (var resource in sharedResources)
        {
            resources.Add(resource.Key, resource.Value);
        }

        foreach (var component in components.Values)
        {
            foreach (var resource in component.ControlResources)
            {
                resources[resource.Key] = resource.Value;
            }
        }

        return new ReadOnlyDictionary<object, object?>(resources);
    }
}
