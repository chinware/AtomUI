using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme;

public class ControlTokenInfo
{
    public bool EnableAlgorithm { get; set; } = false;
    
    public List<TokenSetter> Setters { get; set; } = new ();

    public string? Catalog { get; set; }
    public string TokenId { get; set; } = string.Empty;

    public ControlTokenInfo()
    {
    }

    public ControlTokenInfo(string tokenId, string? catalog)
    {
        Catalog = catalog;
        TokenId = tokenId;
    }
}