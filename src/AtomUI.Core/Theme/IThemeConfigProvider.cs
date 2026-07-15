using AtomUI.Theme.TokenSystem;
using Avalonia.Collections;
using Avalonia.Styling;

namespace AtomUI.Theme;

public interface IThemeConfigProvider
{
    DesignToken SharedToken { get; }
    Dictionary<string, IControlDesignToken> ControlTokens { get; } 
    IControlDesignToken? GetControlToken(string tokenId);
    AvaloniaList<string> Algorithms { get; }
    bool Inherit { get; }
    bool IsDarkMode { get; }
    ThemeVariant ThemeVariant { get; }
}
