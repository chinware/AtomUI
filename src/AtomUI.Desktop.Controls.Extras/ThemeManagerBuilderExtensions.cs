using AtomUI.Generated.AtomUI_Desktop_Controls_Extras;
using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls;

public static class ExtrasThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseDesktopExtras(this IThemeManagerBuilder themeManagerBuilder)
    {
        foreach (var descriptor in GeneratedThemeSchema.GetControls())
        {
            themeManagerBuilder.AddControlToken(descriptor);
        }
        themeManagerBuilder.AddControlThemesProvider(new AtomUIExtrasThemesProvider());

        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProviders(languageProvider);
        }
        return themeManagerBuilder;
    }
}
