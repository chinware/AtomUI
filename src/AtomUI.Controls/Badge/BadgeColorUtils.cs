using AtomUI.Theme.Palette;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Controls.Commons;

internal static class BadgeColorUtils
{
    private const int MaxColorCacheSize = 128;
    private static readonly Dictionary<string, IBrush?> ColorCache = new(StringComparer.OrdinalIgnoreCase);

    internal static IBrush? CalculateColor(string? colorStr)
    {
        if (string.IsNullOrEmpty(colorStr))
        {
            return null;
        }

        colorStr = colorStr.Trim();
        if (ColorCache.TryGetValue(colorStr, out var cachedBrush))
        {
            return cachedBrush;
        }

        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            if (presetColor.Type.ToString().Equals(colorStr, StringComparison.OrdinalIgnoreCase))
            {
                var brush = new ImmutableSolidColorBrush(presetColor.Color());
                CacheBrush(colorStr, brush);
                return brush;
            }
        }

        if (Color.TryParse(colorStr, out var color))
        {
            var brush = new ImmutableSolidColorBrush(color);
            CacheBrush(colorStr, brush);
            return brush;
        }

        CacheBrush(colorStr, null);
        return null;
    }

    private static void CacheBrush(string colorStr, IBrush? brush)
    {
        if (ColorCache.Count < MaxColorCacheSize)
        {
            ColorCache[colorStr] = brush;
        }
    }
}
