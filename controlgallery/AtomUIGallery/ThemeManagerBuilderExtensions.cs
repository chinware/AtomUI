using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUIGallery.Controls;

namespace AtomUIGallery;

public static class ThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseGalleryControls(this IThemeManagerBuilder themeManagerBuilder)
    {
        themeManagerBuilder.AddControlThemesProvider(new GalleryControlThemesProvider());
        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProviders(languageProvider);
        }
        return themeManagerBuilder;
    }
}