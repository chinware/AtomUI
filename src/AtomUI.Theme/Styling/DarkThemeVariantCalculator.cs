using AtomUI.Media;
using AtomUI.Theme.Palette;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Theme.Styling;

public class DarkThemeVariantCalculator : AbstractThemeVariantCalculator
{
    public const string ID = "DarkAlgorithm";

    public DarkThemeVariantCalculator(IThemeVariantCalculator calculator)
        : base(calculator)
    {
        _colorBgBase   = Color.FromRgb(0, 0, 0);
        _colorTextBase = Color.FromRgb(255, 255, 255);
    }

    public override void Calculate(GlobalToken globalToken)
    {
        _compositeGenerator!.Calculate(globalToken);

        if (globalToken.ColorBgBase.HasValue)
        {
            _colorBgBase = globalToken.ColorBgBase.Value;
        }

        if (globalToken.ColorTextBase.HasValue)
        {
            _colorTextBase = globalToken.ColorTextBase.Value;
        }

        // Dark tokens
        SetupColorPalettes(globalToken);
        CalculateColorMapTokenValues(globalToken);
    }

    protected override ColorMap GenerateColorPalettes(Color baseColor)
    {
        var colors = PaletteGenerator.GeneratePalette(baseColor, new PaletteGenerateOption
        {
            ThemeVariant = ThemeVariant.Dark
        });
        var colorMap = new ColorMap
        {
            Color1  = colors[0],
            Color2  = colors[1],
            Color3  = colors[2],
            Color4  = colors[3],
            Color5  = colors[6],
            Color6  = colors[5],
            Color7  = colors[4],
            Color8  = colors[6],
            Color9  = colors[5],
            Color10 = colors[4]
            // Color8 = colors[9],
            // Color9 = colors[8],
            // Color10 = colors[7],
        };
        return colorMap;
    }

    protected override void CalculateNeutralColorPalettes(Color? bgBaseColor, Color? textBaseColor, 
        GlobalToken globalToken)
    {
        var colorBgBase   = bgBaseColor ?? _colorBgBase;
        var colorTextBase = textBaseColor ?? _colorTextBase;

        globalToken.ColorText           = AlphaColor(colorTextBase, 0.85);
        globalToken.ColorTextSecondary  = AlphaColor(colorTextBase, 0.65);
        globalToken.ColorTextTertiary   = AlphaColor(colorTextBase, 0.45);
        globalToken.ColorTextQuaternary = AlphaColor(colorTextBase, 0.25);

        globalToken.ColorFill           = AlphaColor(colorTextBase, 0.18);
        globalToken.ColorFillSecondary  = AlphaColor(colorTextBase, 0.12);
        globalToken.ColorFillTertiary   = AlphaColor(colorTextBase, 0.08);
        globalToken.ColorFillQuaternary = AlphaColor(colorTextBase, 0.04);

        globalToken.ColorBgElevated  = SolidColor(colorBgBase, 12);
        globalToken.ColorBgContainer = SolidColor(colorBgBase, 8);
        globalToken.ColorBgLayout    = SolidColor(colorBgBase, 0);
        globalToken.ColorBgSpotlight = SolidColor(colorBgBase, 26);
        globalToken.ColorBgBlur      = AlphaColor(colorTextBase, 0.04);

        globalToken.ColorBorder          = SolidColor(colorBgBase, 26);
        globalToken.ColorBorderSecondary = SolidColor(colorBgBase, 19);
    }

    private Color AlphaColor(in Color baseColor, double alpha)
    {
        return Color.FromArgb((byte)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);
    }

    private Color SolidColor(Color baseColor, int brightness)
    {
        return baseColor.Lighten(brightness);
    }

    private void SetupColorPalettes(GlobalToken globalToken)
    {
        // 生成所有预置颜色的色系
        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            var colors = PaletteGenerator.GeneratePalette(globalToken.GetPresetPrimaryColor(presetColor.Type).Color(),
                new PaletteGenerateOption
                {
                    ThemeVariant = ThemeVariant.Dark
                });
            var colorMap = ColorMap.FromColors(colors);
            globalToken.SetColorPalette(presetColor, colorMap);
        }
    }
}