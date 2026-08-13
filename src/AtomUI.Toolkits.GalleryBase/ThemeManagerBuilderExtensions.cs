using AtomUI.Generated.AtomUIToolkitsGalleryBase;
using AtomUI.Registration;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.Controls;

namespace AtomUI.Toolkits.GalleryBase;

public static class ThemeManagerBuilderExtensions
{
    internal const string PackageId = "AtomUI.Toolkits.GalleryBase";

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

        var provider = new GalleryControlThemesProvider();
        if (AotTrimRegistration.IsEnabled)
        {
            AotTrimRegistrationPlanRegistry.ApplyPackage(
                builder,
                PackageId,
                provider);
        }
        else
        {
            GeneratedControlPackageRegistration.Register(builder.Theme, provider);
        }
        GeneratedLanguageModuleRegistration.Register(builder.Localization);

        return builder;
    }
}
