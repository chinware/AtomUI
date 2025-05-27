using System.Globalization;
using Avalonia;

namespace AtomUI.Theme;

public interface IThemeManagerBuilder
{
    IList<Type> ControlDesignTokens { get; }
    IList<BaseControlTheme> ControlThemes { get; }
    IList<AbstractLanguageProvider> LanguageProviders { get; }
    IList<EventHandler> InitializedHandlers { get; }
    CultureInfo CultureInfo { get; }
    string ThemeId { get; }
    AppBuilder AppBuilder { get; }
    
    void AddControlToken(Type tokenType);
    void AddControlTheme(BaseControlTheme controlTheme);
    void AddLanguageProviders(AbstractLanguageProvider languageProvider);
    
    void UseTheme(string themeId);
    void UseCultureInfo(CultureInfo cultureInfo);
}