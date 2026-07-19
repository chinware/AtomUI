namespace AtomUI.Theme.Configuration;

public sealed class ControlThemeConfigBuilder
{
    private ControlAlgorithmMode _algorithm;
    private List<string>? _algorithms;
    private readonly Dictionary<string, string> _tokens = new(StringComparer.Ordinal);

    public ControlThemeConfigBuilder WithAlgorithm(ControlAlgorithmMode algorithm)
    {
        _algorithm = algorithm;
        return this;
    }

    public ControlThemeConfigBuilder WithAlgorithms(params string[] algorithms)
    {
        ArgumentNullException.ThrowIfNull(algorithms);
        _algorithms = new List<string>(algorithms);
        return this;
    }

    public ControlThemeConfigBuilder WithToken(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);
        _tokens[name] = value;
        return this;
    }

    public ControlThemeConfig Build()
    {
        return new ControlThemeConfig(_algorithm, _algorithms, _tokens);
    }
}
