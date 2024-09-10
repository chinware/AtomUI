namespace AtomUI.Generator;

public class ControlTokenInfo
{
    public ControlTokenInfo(string controlName, HashSet<TokenName> tokens)
    {
        ControlName = controlName;
        Tokens      = tokens;
    }

    public ControlTokenInfo()
        : this(string.Empty, new HashSet<TokenName>())
    {
    }

    public string? ControlNamespace { get; set; }
    public string ControlName { get; set; }
    public HashSet<TokenName> Tokens { get; }

    public void AddToken(TokenName tokenName)
    {
        Tokens.Add(tokenName);
    }
}

public class TokenInfo
{
    public TokenInfo()
    {
        Tokens            = new HashSet<TokenName>();
        ControlTokenInfos = new List<ControlTokenInfo>();
    }

    public HashSet<TokenName> Tokens { get; private set; }
    public List<ControlTokenInfo> ControlTokenInfos { get; private set; }
}

public record TokenName
{
    public TokenName(string name, string catalog)
    {
        Name            = name;
        ResourceCatalog = catalog;
    }

    public string Name { get; }
    public string ResourceCatalog { get; }
}