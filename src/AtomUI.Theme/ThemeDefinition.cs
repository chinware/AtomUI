using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme;

internal class ThemeDefinition
{
    public string Id { get; }
    public string DisplayName { get; set; }
    public bool IsDefault { get; set; }
    public ISet<ThemeAlgorithm> Algorithms { get; set; }
    public IDictionary<string, ControlTokenConfigInfo> ControlTokens { get; set; }
    public IDictionary<string, string> SharedTokens { get; set; }

    public ThemeDefinition(string id, string? displayName = null)
    {
        Id            = id;
        Algorithms    = new HashSet<ThemeAlgorithm>();
        ControlTokens = new Dictionary<string, ControlTokenConfigInfo>();
        SharedTokens  = new Dictionary<string, string>();
        DisplayName   = displayName ?? id;
    }

    public void Reset()
    {
        Algorithms.Clear();
        ControlTokens.Clear();
        SharedTokens.Clear();
        DisplayName = string.Empty;
    }

    internal ThemeDefinition Clone()
    {
        var cloned = new ThemeDefinition(Id, DisplayName);
        cloned.IsDefault = IsDefault;
        foreach (var algorithm in Algorithms)
        {
            cloned.Algorithms.Add(algorithm);
        }

        foreach (var controlTokenConfigInfo in ControlTokens)
        {
            cloned.ControlTokens.Add(controlTokenConfigInfo.Key, controlTokenConfigInfo.Value.Clone());
        }
        
        foreach (var sharedToken in SharedTokens)
        {
            cloned.SharedTokens.Add(sharedToken);
        }
        return cloned;
    }
}