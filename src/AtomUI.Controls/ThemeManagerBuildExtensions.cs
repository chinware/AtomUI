using AtomUI.Generated.AtomUIControls;

namespace AtomUI.Controls;

internal static class ThemeManagerBuilderExtensions
{
    internal const string PackageId = "AtomUI.Controls.Common";

    public static IAtomUIBuilder UseCommonControls(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.UseImageLoading();
        builder.AddImageCodec(static () => new SvgImageCodec());
        GeneratedControlPackageRegistration.Register(
            builder.Theme,
            RuntimePlatform.Features.SupportsNativeWindow
                ? new CommonControlThemesProvider()
                : new BrowserCommonControlThemesProvider());
        GeneratedLanguageModuleRegistration.Register(builder.Localization);

        return builder;
    }
}
