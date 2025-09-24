using AtomUI.Fonts.AlibabaSans;
using AtomUI.Theme.Language;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Data;

namespace AtomUI.Theme;

public static class AtomUIExtensions
{
    public static AppBuilder UseAtomUI(this AppBuilder builder, Action<IThemeManagerBuilder>? themeConfigureAction = null)
    {
        var themeManagerBuilder = new ThemeManagerBuilder(builder);
        themeManagerBuilder.WithDefaultLanguageVariant(LanguageVariant.en_US);
        themeManagerBuilder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID);
            
        themeConfigureAction?.Invoke(themeManagerBuilder);
        
        builder.AfterSetup(_ =>
        {
            var themeManager = themeManagerBuilder.Build();
            themeManager.Configure();
            var defaultFontFamily = themeManagerBuilder.FontFamily;
            themeManager.ThemeLoaded += (sender, args) =>
            {
                if (defaultFontFamily != null && args.Theme != null)
                {
                    var loadedTheme = args.Theme;
                    loadedTheme.ThemeResource[SharedTokenKey.FontFamily] = defaultFontFamily;
                }
            };
            ThemeManager.Current     =  themeManager;
            themeManager.NotifyInitialized();
            var application = builder.Instance as AtomApplication;
            if (application == null)
            {
                throw new AtomUIBootstrapException("Application is not null and its type must inherit from AtomUI.Controls.Application.");
            }
            application.AttachThemeManager(themeManager);
            application.SetValue(AtomApplication.RequestedLanguageProperty, themeManagerBuilder.LanguageVariant, BindingPriority.Template);
            themeManagerBuilder = null;
        });
        return builder.WithAlibabaSansFont();
    }
}