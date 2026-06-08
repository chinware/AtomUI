using Avalonia;

namespace AtomUI.Fonts.AlibabaPuHuiTi;

public static class AppBuilderExtension
{
    public static AppBuilder WithAlibabaPuHuiTiFont(this AppBuilder appBuilder)
    {
        return appBuilder.ConfigureFonts(fontManager =>
        {
            fontManager.AddFontCollection(new AlibabaPuHuiTiFontCollection());
        });
    }
}
