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
        colorStr = colorStr.Trim().ToLower();

        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            if (presetColor.Type.ToString().ToLower() == colorStr)
            {
                return new SolidColorBrush(presetColor.Color());
            }
        }

        if (Color.TryParse(colorStr, out var color))
        {
            return new SolidColorBrush(color);
        }
        return null;
    }
}