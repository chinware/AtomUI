namespace AtomUI.Theme;

public class TokenSetter
{
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string? Catalog { get; set; }

    public TokenSetter()
    {
    }

    public TokenSetter(string? catalog, string key, string value)
    {
        Catalog = catalog;
        Key     = key;
        Value   = value;
    }
}