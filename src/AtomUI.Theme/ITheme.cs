using AtomUI.Theme.TokenSystem;
using Avalonia.Styling;

namespace AtomUI.Theme;

public interface ITheme
{
    public string Id { get; }
    public string DisplayName { get; }
    public bool IsLoaded { get; }
    public bool IsDarkMode { get; }
    public bool IsActivated { get; }
    public List<string> ThemeResourceKeys { get; }
    public IControlDesignToken? GetControlToken(string tokenId);
    public DesignToken SharedToken { get; }
    public ThemeVariant ThemeVariant { get; }
}