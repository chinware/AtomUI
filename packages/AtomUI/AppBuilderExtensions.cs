using Avalonia;

namespace AtomUI.Theme;

public static class AtomUIExtensions
{
    public static ThemeManagerBuilder CreateThemeManagerBuilder(this AppBuilder builder)
    {
        return new ThemeManagerBuilder(builder);
    }

    public static AppBuilder UseAtomUI(this AppBuilder builder, ThemeManagerBuilder themeManagerBuilder)
    {
        builder.AfterSetup(builder =>
        {
            var themeManager = themeManagerBuilder.Build();
            themeManager.Configure();
            ThemeManager.Current     = themeManager;
            themeManager.CultureInfo = themeManagerBuilder.CultureInfo;
            themeManager.LoadTheme(themeManagerBuilder.ThemeId);
            themeManager.SetActiveTheme(themeManagerBuilder.ThemeId);
            builder.Instance!.Styles.Add(themeManager);
            themeManager.NotifyInitialized();
        });
        return builder;
    }
}