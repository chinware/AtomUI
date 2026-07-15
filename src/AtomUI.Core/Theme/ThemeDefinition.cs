using System.Collections.ObjectModel;

namespace AtomUI.Theme;

internal sealed class ThemeDefinition
{
    public string Id { get; }
    public string DisplayName { get; }
    public bool IsDefault { get; }
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
        var algorithmCopies = new List<ThemeAlgorithm>(algorithms);
        var sharedTokenCopies = new Dictionary<string, string>(sharedTokens, StringComparer.Ordinal);
        var controlTokenCopies = CopyControlTokens(controlTokens);

        Algorithms    = algorithmCopies.AsReadOnly();
        SharedTokens  = new ReadOnlyDictionary<string, string>(sharedTokenCopies);
        ControlTokens = new ReadOnlyDictionary<string, ThemeControlTokenDefinition>(controlTokenCopies);
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
