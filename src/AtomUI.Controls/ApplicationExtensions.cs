using AtomUI.Theme;
using Avalonia;

namespace AtomUI.Controls;

public static class ApplicationExtensions
{
    public static IThemeManager? GetThemeManager(this Application application)
    {
        return AvaloniaLocator.Current.GetService<ThemeManager>();
    }
}