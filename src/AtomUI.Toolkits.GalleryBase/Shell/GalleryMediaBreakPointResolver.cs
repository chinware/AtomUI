using System.Globalization;
using AtomUI.Controls;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Toolkits.GalleryBase.Shell;

internal static class GalleryMediaBreakPointResolver
{
    public static MediaBreakPoint Resolve(double width)
    {
        return Resolve(width, null);
    }

    public static MediaBreakPoint Resolve(double width, Control? resourceHost)
    {
        if (width >= ResolveMin(resourceHost,
                                SharedTokenKind.ScreenXXXLMin,
                                MediaBreakPoint.ExtraExtraExtraLarge))
        {
            return MediaBreakPoint.ExtraExtraExtraLarge;
        }

        if (width >= ResolveMin(resourceHost,
                                SharedTokenKind.ScreenXXLMin,
                                MediaBreakPoint.ExtraExtraLarge))
        {
            return MediaBreakPoint.ExtraExtraLarge;
        }

        if (width >= ResolveMin(resourceHost,
                                SharedTokenKind.ScreenXLMin,
                                MediaBreakPoint.ExtraLarge))
        {
            return MediaBreakPoint.ExtraLarge;
        }

        if (width >= ResolveMin(resourceHost,
                                SharedTokenKind.ScreenLGMin,
                                MediaBreakPoint.Large))
        {
            return MediaBreakPoint.Large;
        }

        if (width >= ResolveMin(resourceHost,
                                SharedTokenKind.ScreenMDMin,
                                MediaBreakPoint.Medium))
        {
            return MediaBreakPoint.Medium;
        }

        if (width >= ResolveMin(resourceHost,
                                SharedTokenKind.ScreenSMMin,
                                MediaBreakPoint.Small))
        {
            return MediaBreakPoint.Small;
        }

        return MediaBreakPoint.ExtraSmall;
    }

    private static double ResolveMin(Control? resourceHost,
                                     SharedTokenKind token,
                                     MediaBreakPoint fallback)
    {
        if (TryResolveToken(resourceHost, token, out var value))
        {
            return value;
        }

        return (double)fallback;
    }

    private static bool TryResolveToken(Control? resourceHost,
                                        SharedTokenKind token,
                                        out double value)
    {
        var themeVariant = resourceHost?.ActualThemeVariant ?? Application.Current?.ActualThemeVariant;
        if (resourceHost?.TryGetResource(token, themeVariant, out var resourceValue) == true ||
            Application.Current?.TryGetResource(token, themeVariant, out resourceValue) == true)
        {
            value = Convert.ToDouble(resourceValue, CultureInfo.InvariantCulture);
            return true;
        }

        value = default;
        return false;
    }
}
