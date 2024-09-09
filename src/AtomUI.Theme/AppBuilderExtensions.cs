using System.Globalization;
using AtomUI.Utils;
using Avalonia;

namespace AtomUI.Theme;

public static class AtomUIExtensions
{
    public static AppBuilder UseAtomUI(this AppBuilder builder)
    {
        builder.AfterSetup(builder =>
        {
            var themeManager = ThemeManager.Current;
            themeManager.Initialize();
            themeManager.LoadTheme(ThemeManager.DEFAULT_THEME_ID);
            themeManager.SetActiveTheme(ThemeManager.DEFAULT_THEME_ID);
            themeManager.CultureInfo = new CultureInfo(LanguageCode.en_US);
            builder.Instance!.Styles.Add(themeManager);
        });
        return builder;
    }
}