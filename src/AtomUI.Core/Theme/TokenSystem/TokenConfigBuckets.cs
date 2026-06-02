namespace AtomUI.Theme.TokenSystem;

internal struct TokenConfigBuckets
{
    private static readonly IDictionary<string, string> s_emptyConfig =
        new Dictionary<string, string>(0);

    private Dictionary<string, string>? _seed;
    private Dictionary<string, string>? _map;
    private Dictionary<string, string>? _alias;

    public IDictionary<string, string> Seed => _seed ?? s_emptyConfig;

    public IDictionary<string, string> Map => _map ?? s_emptyConfig;

    public IDictionary<string, string> Alias => _alias ?? s_emptyConfig;

    public void Add(DesignTokenKind kind, string key, string value)
    {
        switch (kind)
        {
            case DesignTokenKind.Seed:
                (_seed ??= new Dictionary<string, string>()).Add(key, value);
                break;
            case DesignTokenKind.Map:
                (_map ??= new Dictionary<string, string>()).Add(key, value);
                break;
            case DesignTokenKind.Alias:
                (_alias ??= new Dictionary<string, string>()).Add(key, value);
                break;
        }
    }

    public void AddByTokenName(string key,
                               string value,
                               IReadOnlySet<string> seedTokenKeys,
                               IReadOnlySet<string> mapTokenKeys,
                               IReadOnlySet<string> aliasTokenKeys)
    {
        if (seedTokenKeys.Contains(key))
        {
            Add(DesignTokenKind.Seed, key, value);
        }
        else if (mapTokenKeys.Contains(key))
        {
            Add(DesignTokenKind.Map, key, value);
        }
        else if (aliasTokenKeys.Contains(key))
        {
            Add(DesignTokenKind.Alias, key, value);
        }
    }
}
