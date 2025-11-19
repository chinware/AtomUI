using AtomUI.Fonts.AlibabaSans;
using AtomUI.Theme;
using Avalonia.Media;

public static class AlibabaSansThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseAlibabaSansFont(this IThemeManagerBuilder themeManagerBuilder)
    {
        FontManager.Current.AddFontCollection(new AlibabaSansFontCollection());
        return themeManagerBuilder;
    }
}