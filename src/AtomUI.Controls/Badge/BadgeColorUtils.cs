using AtomUI.Theme.Palette;
using Avalonia.Media;

namespace AtomUI.Controls.Commons;

internal static class BadgeColorUtils
{
    private static readonly (string Name, Color Color)[] PresetColorEntries =
    {
        (nameof(PresetColorType.Red), Color.FromRgb(0xF5, 0x22, 0x2D)),
        (nameof(PresetColorType.Volcano), Color.FromRgb(0xFA, 0x54, 0x1C)),
        (nameof(PresetColorType.Orange), Color.FromRgb(0xFA, 0x8C, 0x16)),
        (nameof(PresetColorType.Gold), Color.FromRgb(0xFA, 0xAD, 0x14)),
        (nameof(PresetColorType.Yellow), Color.FromRgb(0xFA, 0xDB, 0x14)),
        (nameof(PresetColorType.Lime), Color.FromRgb(0xA0, 0xD9, 0x11)),
        (nameof(PresetColorType.Green), Color.FromRgb(0x52, 0xC4, 0x1A)),
        (nameof(PresetColorType.Cyan), Color.FromRgb(0x13, 0xC2, 0xC2)),
        (nameof(PresetColorType.Blue), Color.FromRgb(0x16, 0x77, 0xFF)),
        (nameof(PresetColorType.GeekBlue), Color.FromRgb(0x2F, 0x54, 0xEB)),
        (nameof(PresetColorType.Purple), Color.FromRgb(0x72, 0x2E, 0xD1)),
        (nameof(PresetColorType.Pink), Color.FromRgb(0xEB, 0x2F, 0x96)),
        (nameof(PresetColorType.Magenta), Color.FromRgb(0xEB, 0x2F, 0x96)),
        (nameof(PresetColorType.Grey), Color.FromRgb(0x66, 0x66, 0x66))
    };

    internal static IBrush? CalculateColor(string? colorStr)
    {
        if (string.IsNullOrEmpty(colorStr))
        {
            return null;
        }
        var colorSpan = colorStr.AsSpan().Trim();

        if (TryGetPresetColor(colorSpan, out var presetColor))
        {
            return new SolidColorBrush(presetColor);
        }

        if (Color.TryParse(colorSpan, out var color))
        {
            return new SolidColorBrush(color);
        }
        return null;
    }

    internal static bool TryGetPresetColor(ReadOnlySpan<char> colorSpan, out Color color)
    {
        foreach (var entry in PresetColorEntries)
        {
            if (entry.Name.AsSpan().Equals(colorSpan, StringComparison.OrdinalIgnoreCase))
            {
                color = entry.Color;
                return true;
            }
        }

        color = default;
        return false;
    }
}
