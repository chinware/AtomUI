using AtomUI.Theme.Palette;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Compilation;

internal static class DesignTokenClone
{
    internal static DesignToken DeepClone(DesignToken source)
    {
        var clone = (DesignToken)source.Clone();
        var palettes = new Dictionary<PresetPrimaryColor, ColorMap>(source.ColorPalettes.Count);
        foreach (var palette in source.ColorPalettes)
        {
            palettes.Add(palette.Key, CloneColorMap(palette.Value));
        }

        clone.ColorPalettes = palettes;
        return clone;
    }

    private static ColorMap CloneColorMap(ColorMap source)
    {
        return new ColorMap
        {
            Color1  = source.Color1,
            Color2  = source.Color2,
            Color3  = source.Color3,
            Color4  = source.Color4,
            Color5  = source.Color5,
            Color6  = source.Color6,
            Color7  = source.Color7,
            Color8  = source.Color8,
            Color9  = source.Color9,
            Color10 = source.Color10
        };
    }
}
