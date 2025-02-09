using System.Diagnostics;
using AtomUI.Theme.TokenSystem;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme.Utils;

internal static class TokenFinderUtils
{
    public static DesignToken FindSharedToken(Control control)
    {
        DesignToken? token   = null;
        var          current = control;
        while (current != null)
        {
            if (current is IThemeConfigProvider configProvider)
            {
                token = configProvider.SharedToken;
                break;
            }

            current = (current as IStyleHost).StylingParent as Control;
        }

        if (token is null)
        {
            var theme = ThemeManager.Current.ActivatedTheme;
            Debug.Assert(theme != null);
            token = theme.SharedToken;
        }
        return token;
    }

    public static IControlDesignToken? FindControlToken(Control control, string tokenId, string? catalog = null)
    {
        IControlDesignToken? token   = null;
        var                  current = control;
        while (current != null)
        {
            if (current is IThemeConfigProvider configProvider)
            {
                token = configProvider.GetControlToken(tokenId, catalog);
                if (token is not null)
                {
                    break;
                }
            }

            current = (current as IStyleHost).StylingParent as Control;
        }

        if (token is null)
        {
            var theme = ThemeManager.Current.ActivatedTheme;
            if (theme is not null)
            {
                token = theme.GetControlToken(tokenId, catalog);
            }
        }

        return token;
    }

    public static ThemeVariant FindThemeVariant(Control control)
    {
        ThemeVariant? themeVariant = null;
        var           current      = control;
        while (current != null)
        {
            if (current is IThemeConfigProvider configProvider)
            {
                themeVariant = configProvider.ThemeVariant;
                break;
            }

            current = (current as IStyleHost).StylingParent as Control;
        }

        if (themeVariant is null)
        {
            var theme = ThemeManager.Current.ActivatedTheme;
            Debug.Assert(theme != null);
            themeVariant = theme.ThemeVariant;
        }
        return themeVariant;
    }
}