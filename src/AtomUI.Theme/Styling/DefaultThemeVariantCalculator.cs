using AtomUI.Media;
using AtomUI.Theme.Palette;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Theme.Styling;

public class DefaultThemeVariantCalculator : AbstractThemeVariantCalculator
{
    public const string ID = "DefaultAlgorithm";

    public DefaultThemeVariantCalculator()
        : base(null)
    {
        _colorBgBase   = Color.FromRgb(255, 255, 255);
        _colorTextBase = Color.FromRgb(0, 0, 0);
    }

    public override void Calculate(GlobalToken globalToken)
    {
        if (globalToken.ColorBgBase.HasValue)
        {
            _colorBgBase = globalToken.ColorBgBase.Value;
        }

        if (globalToken.ColorTextBase.HasValue)
        {
            _colorTextBase = globalToken.ColorTextBase.Value;
        }

        SetupColorPalettes(globalToken);
        CalculateColorMapTokenValues(globalToken);

        CalculatorUtils.CalculateFontMapTokenValues(globalToken);
        CalculatorUtils.CalculateSizeMapTokenValues(globalToken);
        CalculatorUtils.CalculateControlHeightMapTokenValues(globalToken);
        CalculatorUtils.CalculateStyleMapTokenValues(globalToken);
    }

    protected override ColorMap GenerateColorPalettes(Color baseColor)
    {
        var colors = PaletteGenerator.GeneratePalette(baseColor);
        var colorMap = new ColorMap
        {
            Color1  = colors[0],
            Color2  = colors[1],
            Color3  = colors[2],
            Color4  = colors[3],
            Color5  = colors[4],
            Color6  = colors[5],
            Color7  = colors[6],
            Color8  = colors[4],
            Color9  = colors[5],
            Color10 = colors[6]
            // Color8 = colors[7],
            // Color9 = colors[8],
            // Color10 = colors[9],
        };
        return colorMap;
    }

    protected override void CalculateNeutralColorPalettes(Color? bgBaseColor, Color? textBaseColor, GlobalToken globalToken)
    {
        var colorBgBase   = bgBaseColor ?? _colorBgBase;
        var colorTextBase = textBaseColor ?? _colorTextBase;

        globalToken.ColorText           = AlphaColor(colorTextBase, 0.88);
        globalToken.ColorTextSecondary  = AlphaColor(colorTextBase, 0.65);
        globalToken.ColorTextTertiary   = AlphaColor(colorTextBase, 0.45);
        globalToken.ColorTextQuaternary = AlphaColor(colorTextBase, 0.25);

        globalToken.ColorFill           = AlphaColor(colorTextBase, 0.15);
        globalToken.ColorFillSecondary  = AlphaColor(colorTextBase, 0.06);
        globalToken.ColorFillTertiary   = AlphaColor(colorTextBase, 0.04);
        globalToken.ColorFillQuaternary = AlphaColor(colorTextBase, 0.02);

        globalToken.ColorBgLayout    = SolidColor(colorBgBase, 4);
        globalToken.ColorBgContainer = SolidColor(colorBgBase, 0);
        globalToken.ColorBgElevated  = SolidColor(colorBgBase, 0);
        globalToken.ColorBgSpotlight = AlphaColor(colorTextBase, 0.85);
        globalToken.ColorBgBlur      = ColorUtils.TransparentColor();

        globalToken.ColorBorder          = SolidColor(colorBgBase, 15);
        globalToken.ColorBorderSecondary = SolidColor(colorBgBase, 6);
    }

    private Color AlphaColor(in Color baseColor, double alpha)
    {
        return Color.FromArgb((byte)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);
    }

    private Color SolidColor(Color baseColor, int brightness)
    {
        return baseColor.Darken(brightness);
    }

    private void SetupColorPalettes(GlobalToken globalToken)
    {
        // 生成所有预置颜色的色系
        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            var colors   = PaletteGenerator.GeneratePalette(globalToken.GetPresetPrimaryColor(presetColor.Type).Color());
            var colorMap = ColorMap.FromColors(colors);
            globalToken.SetColorPalette(presetColor, colorMap);
        }
    }
}