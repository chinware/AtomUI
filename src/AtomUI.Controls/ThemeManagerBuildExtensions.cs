using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Controls;

internal static class ThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseCommonControls(this IThemeManagerBuilder themeManagerBuilder)
    {
        var controlTokenTypes = ControlTokenTypePool.GetTokenTypes();
        foreach (var controlType in controlTokenTypes)
        {
            themeManagerBuilder.AddControlToken(controlType);
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
