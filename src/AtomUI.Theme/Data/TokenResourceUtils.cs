using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme.Data;

public static class TokenResourceUtils
{
    public static object? FindTokenResource(Control control, TokenResourceKey resourceKey,
        ThemeVariant? themeVariant = null)
    {
        if (themeVariant is null) themeVariant = (control as IThemeVariantHost).ActualThemeVariant;

        if (control.TryFindResource(resourceKey, themeVariant, out var value)) return value;

        return AvaloniaProperty.UnsetValue;
    }

    public static object? FindGlobalTokenResource(TokenResourceKey resourceKey, ThemeVariant? themeVariant = null)
    {
        var application = Application.Current;
        if (application is null) return null;
        if (themeVariant is null) themeVariant = (application as IThemeVariantHost).ActualThemeVariant;
        if (application.TryFindResource(resourceKey, themeVariant, out var value)) return value;

        return AvaloniaProperty.UnsetValue;
    }
}