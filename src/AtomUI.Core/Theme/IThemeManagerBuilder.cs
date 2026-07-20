using System.Globalization;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Language;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using Avalonia.Media;

namespace AtomUI.Theme;

public interface IThemeManagerBuilder
{
    void AddThemeDefinitionResolver(IThemeDefinitionResolver resolver);
    void AddControlToken(ControlTokenDescriptor descriptor);
    void AddControlThemesProvider(IControlThemesProvider controlThemesProvider);
    void AddLanguageProviders(LanguageProvider languageProvider);
    void AddInitializer(Action<IThemeManager> initializer);
    void WithInitialTheme(string themeId, ThemeConfig? config = null);
    void WithFollowSystemThemes(ThemeRequest light, ThemeRequest dark);
    void WithApplicationId(string applicationId);
    void UseUserThemeDirectory();
    void UseUserThemeDirectory(string directory);
    void WithDefaultFontFamily(FontFamily fontFamily);
    void WithDefaultFontFamily(string fontFamily);
    void WithDefaultCultureInfo(CultureInfo cultureInfo);
    void WithDefaultLanguageVariant(LanguageVariant languageVariant);
}
