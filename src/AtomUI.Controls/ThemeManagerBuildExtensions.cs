using AtomUI.Generated.AtomUI_Controls;

namespace AtomUI.Controls;

internal static class ThemeManagerBuilderExtensions
{
    public static IAtomUIBuilder UseCommonControls(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        GeneratedControlPackageRegistration.Register(
            builder.Theme,
            RuntimePlatform.Features.SupportsNativeWindow
                ? new CommonControlThemesProvider()
                : new BrowserCommonControlThemesProvider());
        GeneratedLanguageModuleRegistration.Register(builder.Localization);

        return builder;
    }
}
