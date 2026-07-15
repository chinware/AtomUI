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
        IReadOnlyDictionary<ComponentTokenIdentity, ComponentThemeSnapshot> components,
        IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo>? componentConfigs = null)
    {
        Id              = id;
        Version         = version;
        Algorithms      = algorithms;
        IsDark          = isDark;
        SharedToken     = sharedToken;
        SharedResources = sharedResources;
        Components      = components;
        ComponentConfigs = CopyComponentConfigs(componentConfigs ??
                                                 new Dictionary<ComponentTokenIdentity, ControlTokenConfigInfo>());
        Resources       = BuildResourceMap(sharedResources, components);
    }

    public string Id { get; }
    public long Version { get; }
    public IReadOnlyList<ThemeAlgorithm> Algorithms { get; }
    public bool IsDark { get; }
    public DesignToken SharedToken { get; }
    public IReadOnlyDictionary<object, object?> SharedResources { get; }
    public IReadOnlyDictionary<ComponentTokenIdentity, ComponentThemeSnapshot> Components { get; }
    public IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> ComponentConfigs { get; }
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

    private static IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> CopyComponentConfigs(
        IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> componentConfigs)
    {
        var copy = new Dictionary<ComponentTokenIdentity, ControlTokenConfigInfo>(componentConfigs.Count);
        foreach (var (identity, config) in componentConfigs)
        {
            copy.Add(identity, config.CloneImmutable());
        }

        return new ReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo>(copy);
    }
}
