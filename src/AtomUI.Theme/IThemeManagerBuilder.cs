using System.Globalization;
using Avalonia;

namespace AtomUI.Theme;

public interface IThemeManagerBuilder
{
    IList<Type> ControlDesignTokens { get; }
    IList<IControlThemesProvider> ControlThemesProviders { get; }
    IList<AbstractLanguageProvider> LanguageProviders { get; }
    IList<EventHandler> InitializedHandlers { get; }
    CultureInfo CultureInfo { get; }
    string ThemeId { get; }
    AppBuilder AppBuilder { get; }
    
    void AddControlToken(Type tokenType);
    void AddControlThemesProvider(IControlThemesProvider controlThemesProvider);
    void AddLanguageProviders(AbstractLanguageProvider languageProvider);
    
    void WithDefaultTheme(string themeId);
    void WithDefaultCultureInfo(CultureInfo cultureInfo);
}