using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Data;

public static class TokenResourceUtils
{
    public static object? FindTokenResource<TTokenKind>(Control control, TTokenKind resourceKey,
                                                        ThemeVariant? themeVariant = null)
        where TTokenKind : Enum
    {
        if (themeVariant is null)
        {
            themeVariant = control.ActualThemeVariant;
        }

        if (control.TryFindResource(resourceKey, themeVariant, out var value))
        {
            return value;
        }

        return AvaloniaProperty.UnsetValue;
    }

    public static object? FindGlobalTokenResource<TTokenKind>(TTokenKind resourceKey, ThemeVariant? themeVariant = null)
        where TTokenKind : Enum
    {
        var application = Application.Current;
        if (application is null)
        {
            return null;
        }

        if (themeVariant is null)
        {
            themeVariant = (application as IThemeVariantHost).ActualThemeVariant;
        }

        if (application.TryFindResource(resourceKey, themeVariant, out var value))
        {
            return value;
        }

        return AvaloniaProperty.UnsetValue;
    }
}
