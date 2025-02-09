namespace AtomUI.Theme;

public class ControlTokenInfoSetter
{
    public bool EnableAlgorithm { get; set; } = false;
    
    public List<TokenSetter> Setters { get; set; } = new ();

    public string? Catalog { get; set; }
    public string TokenId { get; set; } = string.Empty;

    public ControlTokenInfoSetter()
    {
    }

    public ControlTokenInfoSetter(string tokenId, string? catalog)
    {
        Catalog = catalog;
        TokenId = tokenId;
    }
}