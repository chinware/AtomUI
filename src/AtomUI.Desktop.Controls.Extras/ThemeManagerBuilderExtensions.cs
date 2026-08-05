using AtomUI.Generated.AtomUI_Desktop_Controls_Extras;
namespace AtomUI.Desktop.Controls;

public static class ExtrasThemeManagerBuilderExtensions
{
    public static IAtomUIBuilder UseDesktopExtras(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        GeneratedControlPackageRegistration.Register(
            builder.Theme,
            new AtomUIExtrasThemesProvider());
        return builder;
    }
}
