namespace AtomUI.Theme.TokenSystem;

internal class ControlTokenConfigInfo
{
    public bool EnableAlgorithm { get; set; } = false;
    
    public string TokenId { get; set; } = string.Empty;
    
    public IDictionary<string, string> Tokens { get; set; }
    public IDictionary<string, string> SharedTokens { get; set; }

    public ControlTokenConfigInfo() : this(0, 0)
    {
    }

    private ControlTokenConfigInfo(int tokenCapacity, int sharedTokenCapacity)
    {
        Tokens       = new Dictionary<string, string>(tokenCapacity);
        SharedTokens = new Dictionary<string, string>(sharedTokenCapacity);
    }

    internal ControlTokenConfigInfo Clone()
    {
        var cloned = new ControlTokenConfigInfo(Tokens.Count, SharedTokens.Count)
        {
            EnableAlgorithm = EnableAlgorithm,
            TokenId         = TokenId
        };

        foreach (var token in Tokens)
        {
            cloned.Tokens.Add(token.Key, token.Value);
        }

        foreach (var sharedToken in SharedTokens)
        {
            cloned.SharedTokens.Add(sharedToken.Key, sharedToken.Value);
        }
        return cloned;
    }
}
