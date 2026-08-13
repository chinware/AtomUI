using AtomUI.Generated.AtomUIDesktopControlsColorPicker;
using AtomUI.Registration;
namespace AtomUI.Desktop.Controls;

public static class ColorPickerThemeManagerBuilderExtensions
{
    internal const string PackageId = "AtomUI.Desktop.Controls.ColorPicker";

    public static IAtomUIBuilder UseDesktopColorPicker(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var provider = new AtomUIColorPickerThemesProvider();
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
