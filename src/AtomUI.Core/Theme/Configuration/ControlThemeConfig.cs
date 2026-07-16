namespace AtomUI.Theme.Configuration;

public sealed class ControlThemeConfig
{
    public ControlAlgorithmMode Algorithm { get; set; } = ControlAlgorithmMode.Unspecified;

    public IList<string>? Algorithms { get; set; }

    public IDictionary<string, string> Tokens { get; set; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
