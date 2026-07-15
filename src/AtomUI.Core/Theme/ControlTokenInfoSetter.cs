using Avalonia;
using Avalonia.Collections;
using Avalonia.Metadata;

namespace AtomUI.Theme;

public class ControlTokenInfoSetter : AvaloniaObject
{
    public static readonly StyledProperty<bool> EnableAlgorithmProperty =
        AvaloniaProperty.Register<ControlTokenInfoSetter, bool>(nameof(EnableAlgorithm));

    public static readonly StyledProperty<AvaloniaList<TokenSetter>> SettersProperty =
        AvaloniaProperty.Register<ControlTokenInfoSetter, AvaloniaList<TokenSetter>>(nameof(Setters));

    public static readonly StyledProperty<string> TokenIdProperty =
        AvaloniaProperty.Register<ControlTokenInfoSetter, string>(nameof(TokenId), string.Empty);

    public bool EnableAlgorithm
    {
        get => GetValue(EnableAlgorithmProperty);
        set => SetValue(EnableAlgorithmProperty, value);
    }
    
    [Content]
    public AvaloniaList<TokenSetter> Setters
    {
        get => GetValue(SettersProperty);
        set => SetValue(SettersProperty, value);
    }
    
    public string TokenId
    {
        get => GetValue(TokenIdProperty);
        set => SetValue(TokenIdProperty, value);
    }

    public ControlTokenInfoSetter()
    {
        Setters = new AvaloniaList<TokenSetter>();
    }

    public ControlTokenInfoSetter(string tokenId)
        : this()
    {
        TokenId = tokenId;
    }
}
