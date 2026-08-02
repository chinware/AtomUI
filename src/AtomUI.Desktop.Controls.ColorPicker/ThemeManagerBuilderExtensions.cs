using AtomUI.Generated.AtomUI_Desktop_Controls_ColorPicker;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public static class ColorPickerThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseDesktopColorPicker(this IThemeManagerBuilder themeManagerBuilder)
    {
        GeneratedControlPackageRegistration.Register(
            themeManagerBuilder,
            new AtomUIColorPickerThemesProvider());
        return themeManagerBuilder;
    }
}
