using AtomUI.Generated.AtomUI_Desktop_Controls_Extras;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public static class ExtrasThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseDesktopExtras(this IThemeManagerBuilder themeManagerBuilder)
    {
        GeneratedControlPackageRegistration.Register(
            themeManagerBuilder,
            new AtomUIExtrasThemesProvider());
        return themeManagerBuilder;
    }
}
