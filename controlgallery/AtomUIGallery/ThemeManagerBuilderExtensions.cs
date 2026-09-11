using AtomUI;
using AtomUI.Generated.AtomUIGallery;
using AtomUI.Theme;
using AtomUI.Theme.Definitions;

namespace AtomUIGallery;

public static class ThemeManagerBuilderExtensions
{
    /// <summary>
    /// Registers Gallery-level services shared by all hosts. The GalleryBase control package
    /// entry (UseGalleryBase) must be invoked directly by the application project so the
    /// linked-registration plan can see it.
    /// </summary>
    public static IAtomUIBuilder UseGalleryControls(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        GeneratedLanguageModuleRegistration.Register(builder.Localization);
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
