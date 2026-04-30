using System.Collections.Frozen;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Theme.Palette;

public class PaletteInfo
{
    public Color Primary { get; set; }
    public IReadOnlyList<Color> ColorSequence { get; set; } = default!;
}

public static class PresetPalettes
{
    private static readonly FrozenDictionary<PresetPrimaryColor, PaletteInfo> sm_presetPalettes;
    private static readonly FrozenDictionary<PresetPrimaryColor, PaletteInfo> sm_presetDarkPalettes;

    static PresetPalettes()
    {
        var light = new Dictionary<PresetPrimaryColor, PaletteInfo>();
        var dark  = new Dictionary<PresetPrimaryColor, PaletteInfo>();
        InitPalettes(light, false);
        InitPalettes(dark, true);
        sm_presetPalettes     = light.ToFrozenDictionary();
        sm_presetDarkPalettes = dark.ToFrozenDictionary();
    }

    public static PaletteInfo GetPresetPalette(PresetPrimaryColor primaryColor, bool isDark = false)
    {
        if (isDark)
        {
            return sm_presetDarkPalettes[primaryColor];
        }

        return sm_presetPalettes[primaryColor];
    }

    public static IReadOnlyDictionary<PresetPrimaryColor, PaletteInfo> GetPresetPalettes(bool isDark = false)
    {
        if (isDark)
        {
            return sm_presetDarkPalettes;
        }

        return sm_presetPalettes;
    }

    private static void InitPalettes(Dictionary<PresetPrimaryColor, PaletteInfo> target, bool isDark)
    {
        var allColors = PresetPrimaryColor.AllColorTypes();
        foreach (var presetColor in allColors)
        {
            if (isDark)
            {
                var colorSequence = PaletteGenerator.GeneratePalette(presetColor.Color(),
                    new PaletteGenerateOption
                    {
                        ThemeVariant    = ThemeVariant.Dark,
                        BackgroundColor = Color.Parse("#141414")
                    });
                target[presetColor] = new PaletteInfo
                {
                    Primary       = colorSequence[5],
                    ColorSequence = colorSequence
                };
            }
            else
            {
                var colorSequence = PaletteGenerator.GeneratePalette(presetColor.Color());
                target[presetColor] = new PaletteInfo
                {
                    Primary       = colorSequence[5],
                    ColorSequence = colorSequence
                };
            }
        }
    }
}