namespace AtomUI.Theme.TokenSystem;

internal class ControlTokenConfigInfo
{
    public bool EnableAlgorithm { get; set; } = false;
    
    public string? Catalog { get; set; }
    
    public string TokenId { get; set; } = string.Empty;
    
    public IDictionary<string, string> Tokens { get; set; }
    public IDictionary<string, string> SharedTokens { get; set; }

    public ControlTokenConfigInfo()
    {
        Tokens       = new Dictionary<string, string>();
        SharedTokens = new Dictionary<string, string>();
    }

    internal ControlTokenConfigInfo Clone()
    {
        var cloned = new ControlTokenConfigInfo();
        cloned.EnableAlgorithm = EnableAlgorithm;
        cloned.Catalog         = Catalog;
        cloned.TokenId         = TokenId;
        foreach (var key in Tokens.Keys)
        {
            cloned.Tokens[key] = Tokens[key];
        }

        foreach (var key in SharedTokens.Keys)
        {
            cloned.SharedTokens[key] = SharedTokens[key];
        }
        return cloned;
    }
}