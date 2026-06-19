using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.Controls;

namespace AtomUI.Toolkits.GalleryBase;

public static class ThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseGalleryBase(this IThemeManagerBuilder themeManagerBuilder,
                                                      Action<GalleryBaseOptions>? configure = null)
    {
        if (configure is not null)
        {
            var options = new GalleryBaseOptions();
            configure(options);
            GalleryBaseConfigurationProvider.SetCurrent(options.BuildConfiguration());
        }

        var controlTokenTypes = ControlTokenTypePool.GetTokenTypes();
        foreach (var controlTokenRegistration in controlTokenTypes)
        {
            themeManagerBuilder.AddControlToken(controlTokenRegistration.TokenType);
        }

        themeManagerBuilder.AddControlThemesProvider(new GalleryControlThemesProvider());

        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProviders(languageProvider);
        }

        return themeManagerBuilder;
    }
}
