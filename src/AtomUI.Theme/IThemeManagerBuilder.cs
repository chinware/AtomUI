using System.Globalization;
using AtomUI.Theme.Language;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Theme;

public interface IThemeManagerBuilder
{
    IList<Type> ControlDesignTokens { get; }
    IList<IThemeAssetPathProvider> ThemeAssetPathProviders { get; }
    IList<IControlThemesProvider> ControlThemesProviders { get; }
    IList<LanguageProvider> LanguageProviders { get; }
    IList<EventHandler> InitializedHandlers { get; }
    LanguageVariant LanguageVariant { get; }
    string ThemeId { get; }
    AppBuilder AppBuilder { get; }
    
    void AddControlToken(Type tokenType);
    void AddControlThemesProvider(IThemeAssetPathProvider themeAssetPathProvider);
    void AddControlThemesProvider(IControlThemesProvider controlThemesProvider);
    void AddLanguageProviders(LanguageProvider languageProvider);
    
    void WithDefaultTheme(string themeId);
    void WithDefaultFontFamily(FontFamily fontFamily);
    void WithDefaultFontFamily(string fontFamily);
    void WithDefaultCultureInfo(CultureInfo cultureInfo);
    void WithDefaultLanguageVariant(LanguageVariant languageVariant);
    void WithThemeVariantCalculatorFactory(IThemeVariantCalculatorFactory factory);
}