using System.Collections.ObjectModel;
using AtomUI.Theme.Algorithms;

namespace AtomUI.Theme.Configuration;

public sealed class ControlThemeConfig
{
    internal ControlThemeConfig(
        ControlAlgorithmMode algorithm,
        IReadOnlyList<ThemeAlgorithm>? algorithms,
        IReadOnlyDictionary<string, string> tokens)
    {
        Algorithm  = algorithm;
        Algorithms = algorithms is null ? null : Array.AsReadOnly(algorithms.ToArray());
        Tokens     = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(tokens, StringComparer.Ordinal));
    }

    public ControlAlgorithmMode Algorithm { get; }

    public IReadOnlyList<ThemeAlgorithm>? Algorithms { get; }

    public IReadOnlyDictionary<string, string> Tokens { get; }
}
