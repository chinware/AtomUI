using System.Collections.ObjectModel;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme;

internal sealed class ThemeDefinition
{
    private readonly List<ThemeAlgorithm> _algorithms;
    private readonly Dictionary<string, ThemeControlTokenDefinition> _controlTokens;
    private readonly Dictionary<string, string> _sharedTokens;

    public string Id { get; }
    public string DisplayName { get; private set; }
    public bool IsDefault { get; private set; }
    public IReadOnlyList<ThemeAlgorithm> Algorithms { get; }
    public IReadOnlyDictionary<string, ThemeControlTokenDefinition> ControlTokens { get; }
    public IReadOnlyDictionary<string, string> SharedTokens { get; }

    internal ThemeDefinition(string id, string? displayName = null)
        : this(
            id,
            displayName ?? id,
            false,
            Array.Empty<ThemeAlgorithm>(),
            new Dictionary<string, string>(),
            new Dictionary<string, ThemeControlTokenDefinition>())
    {
    }

    internal ThemeDefinition(
        string id,
        string displayName,
        bool isDefault,
        IEnumerable<ThemeAlgorithm> algorithms,
        IReadOnlyDictionary<string, string> sharedTokens,
        IReadOnlyDictionary<string, ThemeControlTokenDefinition> controlTokens)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(algorithms);
        ArgumentNullException.ThrowIfNull(sharedTokens);
        ArgumentNullException.ThrowIfNull(controlTokens);

        Id             = id;
        DisplayName    = displayName;
        IsDefault      = isDefault;
        _algorithms    = new List<ThemeAlgorithm>(algorithms);
        _sharedTokens  = new Dictionary<string, string>(sharedTokens, StringComparer.Ordinal);
        _controlTokens = CopyControlTokens(controlTokens);

        Algorithms    = _algorithms.AsReadOnly();
        SharedTokens  = new ReadOnlyDictionary<string, string>(_sharedTokens);
        ControlTokens = new ReadOnlyDictionary<string, ThemeControlTokenDefinition>(_controlTokens);
    }

    internal ThemeDefinition Clone()
    {
        return new ThemeDefinition(
            Id,
            DisplayName,
            IsDefault,
            Algorithms,
            SharedTokens,
            ControlTokens);
    }

    // Temporary migration boundary for ThemeDefinitionReader; Task 5 removes these methods.
    internal void LegacySetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    internal void LegacySetIsDefault(bool isDefault)
    {
        IsDefault = isDefault;
    }

    internal void LegacyReplaceAlgorithms(IEnumerable<ThemeAlgorithm> algorithms)
    {
        _algorithms.Clear();
        _algorithms.AddRange(algorithms);
    }

    internal void LegacyClearSharedTokens()
    {
        _sharedTokens.Clear();
    }

    internal void LegacyAddSharedToken(string name, string value)
    {
        _sharedTokens.Add(name, value);
    }

    internal void LegacyClearControlTokens()
    {
        _controlTokens.Clear();
    }

    internal void LegacyAddControlToken(string id, ControlTokenConfigInfo config)
    {
        _controlTokens.Add(
            id,
            new ThemeControlTokenDefinition(
                config.TokenId,
                config.EnableAlgorithm,
                config.Tokens,
                config.SharedTokens));
    }

    private static Dictionary<string, ThemeControlTokenDefinition> CopyControlTokens(
        IReadOnlyDictionary<string, ThemeControlTokenDefinition> controlTokens)
    {
        var copies = new Dictionary<string, ThemeControlTokenDefinition>(
            controlTokens.Count,
            StringComparer.Ordinal);
        foreach (var entry in controlTokens)
        {
            copies.Add(
                entry.Key,
                new ThemeControlTokenDefinition(
                    entry.Value.TokenId,
                    entry.Value.EnableAlgorithm,
                    entry.Value.Tokens,
                    entry.Value.SharedTokens));
        }

        return copies;
    }
}

internal sealed class ThemeControlTokenDefinition
{
    public string TokenId { get; }
    public bool EnableAlgorithm { get; }
    public IDictionary<string, string> Tokens { get; }
    public IDictionary<string, string> SharedTokens { get; }

    internal ThemeControlTokenDefinition(
        string tokenId,
        bool enableAlgorithm,
        IEnumerable<KeyValuePair<string, string>> tokens,
        IEnumerable<KeyValuePair<string, string>> sharedTokens)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenId);
        ArgumentNullException.ThrowIfNull(tokens);
        ArgumentNullException.ThrowIfNull(sharedTokens);

        TokenId         = tokenId;
        EnableAlgorithm = enableAlgorithm;
        Tokens = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(tokens, StringComparer.Ordinal));
        SharedTokens = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(sharedTokens, StringComparer.Ordinal));
    }
}
