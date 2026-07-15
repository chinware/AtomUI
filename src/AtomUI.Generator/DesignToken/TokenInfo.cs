using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

internal class ControlTokenInfo
{
    public string ControlNamespace { get; set; }
    public string ControlName { get; set; }
    public string? ControlId { get; set; }
    public string? ResourceCatalog { get; set; }
    public string TokenKindType => $"{ControlName}Kind";
    public HashSet<TokenName> Tokens { get; }
    public List<Diagnostic> Diagnostics { get; }
    public bool IsValid => Diagnostics.Count == 0 && !string.IsNullOrWhiteSpace(ControlId);

    public ControlTokenInfo(string ns, string controlName, HashSet<TokenName> tokens)
    {
        ControlNamespace = ns;
        ControlName = controlName;
        Tokens      = tokens;
        Diagnostics = new List<Diagnostic>();
    }

    public ControlTokenInfo()
        : this(string.Empty, string.Empty, new HashSet<TokenName>())
    {
    }

    public void AddToken(TokenName tokenName)
    {
        Tokens.Add(tokenName);
    }

    public string GetFullyQualifiedTypeName()
    {
        return string.IsNullOrWhiteSpace(ControlNamespace)
            ? ControlName
            : $"{ControlNamespace}.{ControlName}";
    }
}

internal class TokenInfo
{
    public HashSet<TokenName> Tokens { get; private set; }
    public List<ControlTokenInfo> ControlTokenInfos { get; private set; }

    public TokenInfo()
    {
        Tokens            = new HashSet<TokenName>();
        ControlTokenInfos = new List<ControlTokenInfo>();
    }
}

internal record TokenName
{
    public string Name { get; }
    public string ResourceCatalog { get; }

    public TokenName(string name, string catalog)
    {
        Name            = name;
        ResourceCatalog = catalog;
    }
}
