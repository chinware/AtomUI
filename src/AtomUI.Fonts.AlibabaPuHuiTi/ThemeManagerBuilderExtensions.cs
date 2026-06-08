using AtomUI.Theme;
using Avalonia.Media;

namespace AtomUI.Fonts.AlibabaPuHuiTi;

public static class AlibabaPuHuiTiThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseAlibabaPuHuiTiFont(this IThemeManagerBuilder themeManagerBuilder)
    {
        FontManager.Current.AddFontCollection(new AlibabaPuHuiTiFontCollection());
        return themeManagerBuilder;
    }
}
