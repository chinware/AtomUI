using AtomUI.Theme;
using AtomUI.Theme.Language;
using Avalonia;

namespace AtomUI.Controls;

public static class ApplicationExtensions
{
    public static IThemeManager? GetThemeManager(this Application application)
    {
        return AvaloniaLocator.Current.GetService<ThemeManager>();
    }

    public static LanguageVariant? GetLanguageVariant(this Application application)
    {
        var themeManager = application.GetThemeManager();
        return themeManager?.LanguageVariant;
    }

    public static void SetLanguageVariant(this Application application, LanguageVariant variant)
    {
        var themeManager = application.GetThemeManager();
        if (themeManager != null)
        {
            themeManager.LanguageVariant = variant;
        }
    }

    public static bool IsMotionEnabled(this Application application)
    {
        var themeManager = application.GetThemeManager();
        return themeManager?.IsMotionEnabled ?? false;
    }
    
    public static void SetMotionEnabled(this Application application, bool enabled)
    {
        var themeManager = application.GetThemeManager();
        if (themeManager != null)
        {
            themeManager.IsMotionEnabled = enabled;
        }
    }

    public static bool IsWaveSpiritEnabled(this Application application)
    {
        var themeManager = application.GetThemeManager();
        return themeManager?.IsWaveSpiritEnabled ?? false;
    }

    public static void SetWaveSpiritEnabled(this Application application, bool enabled)
    {
        var themeManager = application.GetThemeManager();
        if (themeManager != null)
        {
            themeManager.IsWaveSpiritEnabled = enabled;
        }
    }

    public static bool IsDarkThemeMode(this Application application)
    {
        var themeManager = application.GetThemeManager();
        return themeManager?.IsDarkThemeMode ?? false;
    }

    public static void SetDarkThemeMode(this Application application, bool enabled)
    {
        var themeManager = application.GetThemeManager();
        if (themeManager != null)
        {
            themeManager.IsDarkThemeMode = enabled;
        }
    }

    public static bool IsCompactThemeMode(this Application application)
    {
        var themeManager = application.GetThemeManager();
        return themeManager?.IsCompactThemeMode ?? false;
    }

    public static void SetCompactThemeMode(this Application application, bool enabled)
    {
        var themeManager = application.GetThemeManager();
        if (themeManager != null)
        {
            themeManager.IsCompactThemeMode = enabled;
        }
    }
}