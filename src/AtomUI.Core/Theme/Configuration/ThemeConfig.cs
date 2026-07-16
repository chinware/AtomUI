using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Configuration;

public sealed class ThemeConfig
{
    public bool Inherit { get; set; } = true;

    public IList<string>? Algorithms { get; set; }

    public IDictionary<string, string> Tokens { get; set; } =
        new Dictionary<string, string>(StringComparer.Ordinal);

    public IDictionary<ControlTokenIdentity, ControlThemeConfig> Controls { get; set; } =
        new Dictionary<ControlTokenIdentity, ControlThemeConfig>();
}
