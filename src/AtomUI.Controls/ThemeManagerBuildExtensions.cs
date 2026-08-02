using AtomUI.Generated.AtomUI_Controls;
using AtomUI.Theme;

namespace AtomUI.Controls;

internal static class ThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseCommonControls(this IThemeManagerBuilder themeManagerBuilder)
    {
        GeneratedControlPackageRegistration.Register(
            themeManagerBuilder,
            RuntimePlatform.Features.SupportsNativeWindow
                ? new CommonControlThemesProvider()
                : new BrowserCommonControlThemesProvider());

        return themeManagerBuilder;
    }
}
