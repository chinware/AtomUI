using AtomUI;
using AtomUI.Generated.AtomUIGallery;
using AtomUI.Theme;
using AtomUI.Theme.Definitions;
using AtomUI.Toolkits.GalleryBase;

namespace AtomUIGallery;

public static class ThemeManagerBuilderExtensions
{
    public static IAtomUIBuilder UseGalleryControls(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        GeneratedLanguageModuleRegistration.Register(builder.Localization);
        builder.UseGalleryBase(AtomUIGalleryModule.Configure);
        builder.Theme.AddThemeDefinitionResolver(
            new AvaloniaAssetThemeDefinitionResolver(
                "AtomUIGallery.BuiltInThemes",
                [
                    new Uri("avares://AtomUIGallery/Assets/Themes/PolarGreen.theme.xml"),
                    new Uri("avares://AtomUIGallery/Assets/Themes/SunsetOrange.theme.xml"),
                    new Uri("avares://AtomUIGallery/Assets/Themes/GoldenPurple.theme.xml"),
                    new Uri("avares://AtomUIGallery/Assets/Themes/Magenta.theme.xml")
                ],
                typeof(ThemeManagerBuilderExtensions).Assembly.GetName().Version?.ToString() ?? "0"));

        return builder;
    }
}
