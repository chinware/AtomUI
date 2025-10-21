using AtomUI.Theme.Language;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Data;
using Avalonia.Styling;

namespace AtomUI.Theme;

public static class ApplicationExtensions
{
    public static Application UseAtomUI(this Application application, Action<IThemeManagerBuilder>? themeConfigureAction = null)
    {
        var themeManagerBuilder = new ThemeManagerBuilder();
        themeManagerBuilder.WithDefaultLanguageVariant(LanguageVariant.en_US);
        themeManagerBuilder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID);
        themeConfigureAction?.Invoke(themeManagerBuilder);
        
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
        AvaloniaLocator.CurrentMutable.BindToSelf(themeManager);
        themeManager.NotifyInitialized();
        application.RequestedThemeVariant = new ThemeVariant(themeManager.DefaultThemeId, null);
        themeManager.SetValue(ThemeManager.LanguageVariantProperty, themeManagerBuilder.LanguageVariant, BindingPriority.Template);
        themeManager.AttachApplication(application);
        return application;
    }
}