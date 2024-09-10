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

    public override MapDesignToken Calculate(SeedDesignToken seedToken, MapDesignToken sourceToken)
    {
        var mergedMapToken = _compositeGenerator!.Calculate(seedToken, sourceToken);

        if (seedToken.ColorBgBase.HasValue)
        {
            _colorBgBase = seedToken.ColorBgBase.Value;
        }

        if (seedToken.ColorTextBase.HasValue)
        {
            _colorTextBase = seedToken.ColorTextBase.Value;
        }

        // Dark tokens
        SetupColorPalettes(seedToken, mergedMapToken);
        SetupColorToken(seedToken, mergedMapToken);

        return mergedMapToken;
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

    protected override ColorNeutralMapDesignToken GenerateNeutralColorPalettes(Color? bgBaseColor, Color? textBaseColor)
    {
        var colorBgBase   = bgBaseColor ?? _colorBgBase;
        var colorTextBase = textBaseColor ?? _colorTextBase;

        var colorNeutralToken = new ColorNeutralMapDesignToken
        {
            ColorText           = AlphaColor(colorTextBase, 0.85),
            ColorTextSecondary  = AlphaColor(colorTextBase, 0.65),
            ColorTextTertiary   = AlphaColor(colorTextBase, 0.45),
            ColorTextQuaternary = AlphaColor(colorTextBase, 0.25),

            ColorFill           = AlphaColor(colorTextBase, 0.18),
            ColorFillSecondary  = AlphaColor(colorTextBase, 0.12),
            ColorFillTertiary   = AlphaColor(colorTextBase, 0.08),
            ColorFillQuaternary = AlphaColor(colorTextBase, 0.04),

            ColorBgElevated  = SolidColor(colorBgBase, 12),
            ColorBgContainer = SolidColor(colorBgBase, 8),
            ColorBgLayout    = SolidColor(colorBgBase, 0),
            ColorBgSpotlight = SolidColor(colorBgBase, 26),
            ColorBgBlur      = AlphaColor(colorTextBase, 0.04),

            ColorBorder          = SolidColor(colorBgBase, 26),
            ColorBorderSecondary = SolidColor(colorBgBase, 19)
        };
        return colorNeutralToken;
    }

    private Color AlphaColor(in Color baseColor, double alpha)
    {
        return Color.FromArgb((byte)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);
    }

    private Color SolidColor(Color baseColor, int brightness)
    {
        return baseColor.Lighten(brightness);
    }

    private void SetupColorPalettes(SeedDesignToken seedToken, MapDesignToken sourceToken)
    {
        // 生成所有预置颜色的色系
        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            var colors = PaletteGenerator.GeneratePalette(seedToken.GetPresetPrimaryColor(presetColor.Type).Color(),
                new PaletteGenerateOption
                {
                    ThemeVariant = ThemeVariant.Dark
                });
            var colorMap = ColorMap.FromColors(colors);
            sourceToken.SetColorPalette(presetColor, colorMap);
        }
    }

    private void SetupColorToken(SeedDesignToken seedToken, MapDesignToken sourceToken)
    {
        var colorMapToken = GenerateColorMapToken(seedToken);
        sourceToken.ColorToken = colorMapToken;
    }
}