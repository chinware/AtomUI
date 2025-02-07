using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme;

public interface IThemeConfigProvider
{
    DesignToken SharedToken { get; }
    Dictionary<string, IControlDesignToken> ControlTokens { get; }
    IList<string> Algorithms { get; }
    bool IsDarkMode { get; }
}