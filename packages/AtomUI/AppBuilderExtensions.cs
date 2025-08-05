using System.Globalization;
using AtomUI.Utils;
using Avalonia;

namespace AtomUI.Theme;

public static class AtomUIExtensions
{
    public static AppBuilder UseAtomUI(this AppBuilder builder, Action<ThemeManagerBuilder>? themeConfigureAction = null)
    {
        var themeManagerBuilder = new ThemeManagerBuilder(builder);
        themeManagerBuilder.WithDefaultCultureInfo(new CultureInfo(LanguageCode.en_US));
        themeManagerBuilder.WithDefaultTheme(ThemeManager.DEFAULT_THEME_ID);
            
        themeConfigureAction?.Invoke(themeManagerBuilder);
        
        builder.AfterSetup(_ =>
        {
            var themeManager = themeManagerBuilder.Build();
            themeManager.Configure();
            ThemeManager.Current     = themeManager;
            themeManager.CultureInfo = themeManagerBuilder.CultureInfo;
            themeManager.LoadTheme(themeManagerBuilder.ThemeId);
            themeManager.SetActiveTheme(themeManagerBuilder.ThemeId);
            builder.Instance!.Styles.Add(themeManager);
            themeManager.NotifyInitialized();
            themeManagerBuilder = null;
        });
        return builder.WithInterFont();
    }
}