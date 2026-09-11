using AtomUI;
using Avalonia.Media;

namespace AtomUI.Fonts.AlibabaPuHuiTi;

public static class AlibabaPuHuiTiThemeManagerBuilderExtensions
{
    public static IAtomUIBuilder UseAlibabaPuHuiTiFont(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        FontManager.Current.AddFontCollection(new AlibabaPuHuiTiFontCollection());
        return builder;
    }
}
