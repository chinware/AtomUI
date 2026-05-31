using AtomUI.Theme.Palette;
using Avalonia.Media;

namespace AtomUI.Controls.Commons;

internal static class BadgeColorUtils
{
    internal static IBrush? CalculateColor(string? colorStr)
    {
        if (string.IsNullOrEmpty(colorStr))
        {
            return null;
        }
        var colorSpan = colorStr.AsSpan().Trim();

        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            if (presetColor.Type.ToString().AsSpan().Equals(colorSpan, StringComparison.OrdinalIgnoreCase))
            {
                return new SolidColorBrush(presetColor.Color());
            }
        }

        if (Color.TryParse(colorSpan, out var color))
        {
            return new SolidColorBrush(color);
        }
        return null;
    }
}
