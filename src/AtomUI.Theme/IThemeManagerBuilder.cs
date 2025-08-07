using System.Globalization;
using AtomUI.Theme.Language;
using Avalonia;

namespace AtomUI.Theme;

public interface IThemeManagerBuilder
{
    IList<Type> ControlDesignTokens { get; }
    IList<IThemeAssetPathProvider> ThemeAssetPathProviders { get; }
    IList<IControlThemesProvider> ControlThemesProviders { get; }
    IList<AbstractLanguageProvider> LanguageProviders { get; }
    IList<EventHandler> InitializedHandlers { get; }
    LanguageVariant LanguageVariant { get; }
    string ThemeId { get; }
    AppBuilder AppBuilder { get; }
    
    void AddControlToken(Type tokenType);
    void AddControlThemesProvider(IThemeAssetPathProvider themeAssetPathProvider);
    void AddControlThemesProvider(IControlThemesProvider controlThemesProvider);
    void AddLanguageProviders(AbstractLanguageProvider languageProvider);
    
    void WithDefaultTheme(string themeId);
    void WithDefaultCultureInfo(CultureInfo cultureInfo);
    void WithDefaultLanguageVariant(LanguageVariant languageVariant);
}