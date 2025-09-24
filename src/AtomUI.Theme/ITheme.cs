using AtomUI.Theme.TokenSystem;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme;

public interface ITheme
{
    string Id { get; }
    string DisplayName { get; }
    bool IsLoaded { get; }
    bool IsDarkMode { get; }
    bool IsActivated { get; }
    bool IsPrimary { get; }
    bool IsBuiltIn { get; }
    List<string> ThemeResourceKeys { get; }
    IControlDesignToken? GetControlToken(string tokenId, string? catalog = null);
    DesignToken SharedToken { get; }
    ThemeVariant ThemeVariant { get; }
    IList<ThemeAlgorithm> Algorithms { get; }
    ResourceDictionary ThemeResource { get; }
}