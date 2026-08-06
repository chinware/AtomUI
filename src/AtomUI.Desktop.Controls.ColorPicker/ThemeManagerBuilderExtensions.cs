using AtomUI.Generated.AtomUI_Desktop_Controls_ColorPicker;
namespace AtomUI.Desktop.Controls;

public static class ColorPickerThemeManagerBuilderExtensions
{
    public static IAtomUIBuilder UseDesktopColorPicker(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        GeneratedControlPackageRegistration.Register(
            builder.Theme,
            new AtomUIColorPickerThemesProvider());
        GeneratedLanguageModuleRegistration.Register(builder.Localization);
        return builder;
    }
}
