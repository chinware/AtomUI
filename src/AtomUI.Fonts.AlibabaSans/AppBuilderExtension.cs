using Avalonia;

namespace AtomUI.Fonts.AlibabaSans;

public static class AppBuilderExtension
{
    public static AppBuilder WithAlibabaSansFont(this AppBuilder appBuilder)
    {
        return appBuilder.ConfigureFonts(fontManager =>
        {
            fontManager.AddFontCollection(new AlibabaSansFontCollection());
        });
    }
}