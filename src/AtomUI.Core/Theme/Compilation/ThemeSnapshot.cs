using System.Collections.ObjectModel;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;
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
        IReadOnlyDictionary<ControlTokenIdentity, ControlThemeSnapshot> controls,
        IReadOnlyDictionary<ControlTokenIdentity, ControlTokenConfigInfo>? controlConfigs = null,
        IReadOnlyDictionary<string, string>? sharedConfig = null)
    {
        Id              = id;
        Version         = version;
        Algorithms      = algorithms;
        IsDark          = isDark;
        SharedTokenCore = sharedToken;
        SharedResources = sharedResources;
        Controls       = controls;
        ControlConfigs = CopyControlConfigs(controlConfigs ??
                                             new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>());
        SharedConfig   = CopySharedConfig(sharedConfig ?? new Dictionary<string, string>());
        Resources       = BuildResourceMap(sharedResources, controls);
    }

    public string Id { get; }
    public long Version { get; }
    public IReadOnlyList<ThemeAlgorithm> Algorithms { get; }
    public bool IsDark { get; }
    public DesignToken SharedToken => DesignTokenClone.DeepClone(SharedTokenCore);
    public IReadOnlyDictionary<object, object?> SharedResources { get; }
    public IReadOnlyDictionary<ControlTokenIdentity, ControlThemeSnapshot> Controls { get; }
    public IReadOnlyDictionary<ControlTokenIdentity, ControlTokenConfigInfo> ControlConfigs { get; }
    internal DesignToken SharedTokenCore { get; }
    internal IReadOnlyDictionary<string, string> SharedConfig { get; }
    internal IReadOnlyDictionary<object, object?> Resources { get; }

    private static IReadOnlyDictionary<object, object?> BuildResourceMap(
        IReadOnlyDictionary<object, object?> sharedResources,
        IReadOnlyDictionary<ControlTokenIdentity, ControlThemeSnapshot> controls)
    {
        var resources = new Dictionary<object, object?>(sharedResources.Count);
        foreach (var resource in sharedResources)
        {
            resources.Add(resource.Key, resource.Value);
        }

        foreach (var control in controls.Values)
        {
            foreach (var resource in control.ControlResources)
            {
                resources[resource.Key] = resource.Value;
            }
        }

        return new ReadOnlyDictionary<object, object?>(resources);
    }

    private static IReadOnlyDictionary<ControlTokenIdentity, ControlTokenConfigInfo> CopyControlConfigs(
        IReadOnlyDictionary<ControlTokenIdentity, ControlTokenConfigInfo> controlConfigs)
    {
        var copy = new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>(controlConfigs.Count);
        foreach (var (identity, config) in controlConfigs)
        {
            copy.Add(identity, config.CloneImmutable());
        }

        return new ReadOnlyDictionary<ControlTokenIdentity, ControlTokenConfigInfo>(copy);
    }

    private static IReadOnlyDictionary<string, string> CopySharedConfig(
        IReadOnlyDictionary<string, string> sharedConfig)
    {
        return new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(sharedConfig, StringComparer.Ordinal));
    }
}
