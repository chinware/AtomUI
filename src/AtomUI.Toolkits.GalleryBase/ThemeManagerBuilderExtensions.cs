using AtomUI.Generated.AtomUI_Toolkits_GalleryBase;
using AtomUI.Theme;
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

        GeneratedControlPackageRegistration.Register(
            themeManagerBuilder,
            new GalleryControlThemesProvider());

        return themeManagerBuilder;
    }
}
