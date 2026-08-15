using AtomUI.Generated.AtomUIDesktopControlsExtras;
using AtomUI.Registration;
namespace AtomUI.Desktop.Controls;

public static class ExtrasThemeManagerBuilderExtensions
{
    internal const string PackageId = "AtomUI.Desktop.Controls.Extras";

    [ControlPackageRegistrationEntry]
    public static IAtomUIBuilder UseDesktopExtras(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var provider = new AtomUIExtrasThemesProvider();
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
        return builder;
    }
}
