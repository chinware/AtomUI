using Avalonia;
using Avalonia.Metadata;

namespace AtomUI.Theme;

public class TokenSetter : AvaloniaObject
{
    public static readonly StyledProperty<string> KeyProperty =
        AvaloniaProperty.Register<TokenSetter, string>(nameof(Key), string.Empty);

    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<TokenSetter, string>(nameof(Value), string.Empty);

    public static readonly StyledProperty<string?> CatalogProperty =
        AvaloniaProperty.Register<TokenSetter, string?>(nameof(Catalog));

    public string Key
    {
        get => GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }
    
    [Content]
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string? Catalog
    {
        get => GetValue(CatalogProperty);
        set => SetValue(CatalogProperty, value);
    }

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

public class ControlTokenSetter : TokenSetter 
{}
