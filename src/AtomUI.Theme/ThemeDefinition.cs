using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme;

public class ThemeDefinition
{
    public string Id { get; }
    public string DisplayName { get; set; }
    public IList<string> Algorithms { get; set; }
    public IDictionary<string, ControlTokenConfigInfo> ControlTokens { get; set; }
    public IDictionary<string, string> GlobalTokens { get; set; }

    public ThemeDefinition(string id, string? displayName = null)
    {
        Id            = id;
        Algorithms    = new List<string>();
        ControlTokens = new Dictionary<string, ControlTokenConfigInfo>();
        GlobalTokens  = new Dictionary<string, string>();
        DisplayName   = displayName ?? id;
    }

    public void Reset()
    {
        Algorithms.Clear();
        ControlTokens.Clear();
        GlobalTokens.Clear();
        DisplayName = string.Empty;
    }
}