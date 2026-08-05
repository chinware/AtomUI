using AtomUI.Generated.AtomUI_Toolkits_GalleryBase;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.Controls;

namespace AtomUI.Toolkits.GalleryBase;

public static class ThemeManagerBuilderExtensions
{
    public static IAtomUIBuilder UseGalleryBase(this IAtomUIBuilder builder,
                                                Action<GalleryBaseOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (configure is not null)
        {
            var options = new GalleryBaseOptions();
            configure(options);
            GalleryBaseConfigurationProvider.SetCurrent(options.BuildConfiguration());
        }

        GeneratedControlPackageRegistration.Register(
            builder.Theme,
            new GalleryControlThemesProvider());

        return builder;
    }
}
