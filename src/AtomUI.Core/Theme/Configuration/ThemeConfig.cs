using System.Collections.ObjectModel;
using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Configuration;

public sealed class ThemeConfig
{
    internal ThemeConfig(
        bool inherit,
        IReadOnlyList<string>? algorithms,
        IReadOnlyDictionary<string, string> tokens,
        IReadOnlyDictionary<ControlTokenIdentity, ControlThemeConfig> controls)
    {
        Inherit    = inherit;
        Algorithms = algorithms is null ? null : Array.AsReadOnly(algorithms.ToArray());
        Tokens     = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(tokens, StringComparer.Ordinal));
        Controls   = new ReadOnlyDictionary<ControlTokenIdentity, ControlThemeConfig>(
            new Dictionary<ControlTokenIdentity, ControlThemeConfig>(controls));
    }

    public bool Inherit { get; }

    public IReadOnlyList<string>? Algorithms { get; }

    public IReadOnlyDictionary<string, string> Tokens { get; }

    public IReadOnlyDictionary<ControlTokenIdentity, ControlThemeConfig> Controls { get; }
}
