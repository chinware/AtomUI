using AtomUI;
using AtomUI.Fonts.AlibabaSans;
using Avalonia.Media;

public static class AlibabaSansThemeManagerBuilderExtensions
{
    public static IAtomUIBuilder UseAlibabaSansFont(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        FontManager.Current.AddFontCollection(new AlibabaSansFontCollection());
        return builder;
    }
}
