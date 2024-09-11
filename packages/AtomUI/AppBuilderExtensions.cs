using System.Globalization;
using AtomUI.Controls;
using AtomUI.Utils;
using Avalonia;

namespace AtomUI.Theme;

public static class AtomUIExtensions
{
    public static AppBuilder ConfigureAtomUI(this AppBuilder builder)
    {
        builder.AfterSetup(builder =>
        {
            var themeManager = ThemeManager.Current;
            themeManager.ConfigureAtomUIControls();
            themeManager.Configure();
        });
        return builder;
    }
    
    public static AppBuilder UseAtomUI(this AppBuilder builder)
    {
        builder.AfterSetup(builder =>
        {
            var themeManager = ThemeManager.Current;
            themeManager.LoadTheme(ThemeManager.DEFAULT_THEME_ID);
            themeManager.SetActiveTheme(ThemeManager.DEFAULT_THEME_ID);
            themeManager.CultureInfo = new CultureInfo(LanguageCode.en_US);
            builder.Instance!.Styles.Add(themeManager);
        });
        return builder;
    }
}