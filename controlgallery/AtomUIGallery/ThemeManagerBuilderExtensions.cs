using AtomUI.Theme;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Language;
using AtomUI.Toolkits.GalleryBase;

namespace AtomUIGallery;

public static class ThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseGalleryControls(this IThemeManagerBuilder themeManagerBuilder)
    {
        ArgumentNullException.ThrowIfNull(themeManagerBuilder);
        themeManagerBuilder.UseGalleryBase(AtomUIGalleryModule.Configure);
        themeManagerBuilder.AddThemeDefinitionResolver(
            new AvaloniaAssetThemeDefinitionResolver(
                "AtomUIGallery.BuiltInThemes",
                [
                    new Uri("avares://AtomUIGallery/Assets/Themes/PolarGreen.theme.xml"),
                    new Uri("avares://AtomUIGallery/Assets/Themes/SunsetOrange.theme.xml"),
                    new Uri("avares://AtomUIGallery/Assets/Themes/GoldenPurple.theme.xml"),
                    new Uri("avares://AtomUIGallery/Assets/Themes/Magenta.theme.xml")
                ],
                typeof(ThemeManagerBuilderExtensions).Assembly.GetName().Version?.ToString() ?? "0"));

        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProvider(languageProvider);
        }
        return themeManagerBuilder;
    }
}
