using System.Globalization;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Language;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using Avalonia.Media;

namespace AtomUI.Theme;

public interface IThemeManagerBuilder
{
    void AddControlToken(ControlTokenDescriptor descriptor);
    void AddControlThemesProvider(IControlThemesProvider controlThemesProvider);
    void AddLanguageProviders(LanguageProvider languageProvider);
    void AddInitializer(Action<IThemeManager> initializer);
    void WithInitialTheme(string themeId, ThemeConfig? config = null);
    void WithFollowSystemThemes(ThemeRequest light, ThemeRequest dark);
    void WithDefaultFontFamily(FontFamily fontFamily);
    void WithDefaultFontFamily(string fontFamily);
    void WithDefaultCultureInfo(CultureInfo cultureInfo);
    void WithDefaultLanguageVariant(LanguageVariant languageVariant);
}
