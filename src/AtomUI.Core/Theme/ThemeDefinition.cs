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
        : this(id, displayName, 0, 0, 0)
    {
    }

    private ThemeDefinition(string id,
                            string? displayName,
                            int algorithmCapacity,
                            int controlTokenCapacity,
                            int sharedTokenCapacity)
    {
        Id            = id;
        Algorithms    = new HashSet<ThemeAlgorithm>(algorithmCapacity);
        ControlTokens = new Dictionary<string, ControlTokenConfigInfo>(controlTokenCapacity);
        SharedTokens  = new Dictionary<string, string>(sharedTokenCapacity);
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
        var cloned = new ThemeDefinition(Id,
                                         DisplayName,
                                         Algorithms.Count,
                                         ControlTokens.Count,
                                         SharedTokens.Count)
        {
            IsDefault = IsDefault
        };

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
            cloned.SharedTokens.Add(sharedToken.Key, sharedToken.Value);
        }
        return cloned;
    }
}
