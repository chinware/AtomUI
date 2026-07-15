using AtomUI.Theme;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Data;

namespace AtomUI;

public static class ApplicationExtensions
{
    public static Application UseAtomUI(this Application application, Action<IThemeManagerBuilder>? themeConfigureAction = null)
    {
        var themeManagerBuilder = new ThemeManagerBuilder();
        themeManagerBuilder.WithDefaultLanguageVariant(LanguageVariant.en_US);
        themeConfigureAction?.Invoke(themeManagerBuilder);
        
        var themeManager = themeManagerBuilder.Build();
        var defaultFontFamily = themeManagerBuilder.FontFamily;
        themeManager.FontFamily = defaultFontFamily;
        themeManager.Configure();
        AvaloniaLocator.CurrentMutable.BindToSelf(themeManager);
        themeManager.NotifyInitialized();
        themeManager.SetValue(ThemeManager.LanguageVariantProperty, themeManagerBuilder.LanguageVariant, BindingPriority.Template);
        themeManager.AttachApplication(application);
        return application;
    }
}
