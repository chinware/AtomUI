using AtomUI.Theme.TokenSystem;
using Avalonia.Styling;

namespace AtomUI.Theme;

public interface IThemeConfigProvider
{
    DesignToken SharedToken { get; }
    Dictionary<string, IControlDesignToken> ControlTokens { get; } 
    IControlDesignToken? GetControlToken(string tokenId, string? catalog = null);
    List<string> Algorithms { get; }
    bool IsDarkMode { get; }
    ThemeVariant ThemeVariant { get; }
}