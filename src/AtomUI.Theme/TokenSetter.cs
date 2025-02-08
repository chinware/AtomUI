namespace AtomUI.Theme;

public class TokenSetter
{
    public string Key { get; set; } = string.Empty;

    public object? Value { get; set; }

    public string? Catalog { get; set; }

    public TokenSetter()
    {
    }

    public TokenSetter(string? catalog, string key, object? value)
    {
        Catalog = catalog;
        Key     = key;
        Value   = value;
    }
}