using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Generated.AtomUI_Controls;

namespace AtomUI.Controls;

internal static class ThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseCommonControls(this IThemeManagerBuilder themeManagerBuilder)
    {
        foreach (var descriptor in GeneratedThemeSchema.GetControls())
        {
            themeManagerBuilder.AddControlToken(descriptor);
        }
        themeManagerBuilder.AddControlThemesProvider(RuntimePlatform.Features.SupportsNativeWindow
            ? new CommonControlThemesProvider()
            : new BrowserCommonControlThemesProvider());

        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProviders(languageProvider);
        }

        return themeManagerBuilder;
    }
}
