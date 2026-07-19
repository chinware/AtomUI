using AtomUI.Theme;
using AtomUI.Theme.Language;
using Avalonia;

namespace AtomUI.Controls;

public static class ApplicationExtensions
{
    public static IThemeManager? GetThemeManager(this Application application)
    {
        return AvaloniaLocator.Current.GetService(typeof(ThemeManager)) as IThemeManager;
    }

    public static LanguageVariant? GetLanguageVariant(this Application application)
    {
        return application.GetLanguageManager()?.LanguageVariant;
    }

    public static void SetLanguageVariant(this Application application, LanguageVariant variant)
    {
        var languageManager = application.GetLanguageManager();
        if (languageManager != null)
        {
            languageManager.LanguageVariant = variant;
        }
    }

    public static ILanguageManager? GetLanguageManager(this Application application)
    {
        return AvaloniaLocator.Current.GetService(typeof(ThemeManager)) as ILanguageManager;
    }
}
