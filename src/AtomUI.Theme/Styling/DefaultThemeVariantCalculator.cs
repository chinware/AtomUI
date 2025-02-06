﻿using AtomUI.Media;
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

    public override void Calculate(DesignToken designToken)
    {
        if (designToken.ColorBgBase.HasValue)
        {
            _colorBgBase = designToken.ColorBgBase.Value;
        }

        if (designToken.ColorTextBase.HasValue)
        {
            _colorTextBase = designToken.ColorTextBase.Value;
        }

        SetupColorPalettes(designToken);
        CalculateColorMapTokenValues(designToken);

        CalculatorUtils.CalculateFontMapTokenValues(designToken);
        CalculatorUtils.CalculateSizeMapTokenValues(designToken);
        CalculatorUtils.CalculateControlHeightMapTokenValues(designToken);
        CalculatorUtils.CalculateStyleMapTokenValues(designToken);
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

    protected override void CalculateNeutralColorPalettes(Color? bgBaseColor, Color? textBaseColor, DesignToken designToken)
    {
        var colorBgBase   = bgBaseColor ?? _colorBgBase;
        var colorTextBase = textBaseColor ?? _colorTextBase;

        designToken.ColorText           = AlphaColor(colorTextBase, 0.88);
        designToken.ColorTextSecondary  = AlphaColor(colorTextBase, 0.65);
        designToken.ColorTextTertiary   = AlphaColor(colorTextBase, 0.45);
        designToken.ColorTextQuaternary = AlphaColor(colorTextBase, 0.25);

        designToken.ColorFill           = AlphaColor(colorTextBase, 0.15);
        designToken.ColorFillSecondary  = AlphaColor(colorTextBase, 0.06);
        designToken.ColorFillTertiary   = AlphaColor(colorTextBase, 0.04);
        designToken.ColorFillQuaternary = AlphaColor(colorTextBase, 0.02);

        designToken.ColorBgLayout    = SolidColor(colorBgBase, 4);
        designToken.ColorBgContainer = SolidColor(colorBgBase, 0);
        designToken.ColorBgElevated  = SolidColor(colorBgBase, 0);
        designToken.ColorBgSpotlight = AlphaColor(colorTextBase, 0.85);
        designToken.ColorBgBlur      = ColorUtils.TransparentColor();

        designToken.ColorBorder          = SolidColor(colorBgBase, 15);
        designToken.ColorBorderSecondary = SolidColor(colorBgBase, 6);
    }

    private Color AlphaColor(in Color baseColor, double alpha)
    {
        return Color.FromArgb((byte)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);
    }

    private Color SolidColor(Color baseColor, int brightness)
    {
        return baseColor.Darken(brightness);
    }

    private void SetupColorPalettes(DesignToken designToken)
    {
        // 生成所有预置颜色的色系
        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            var colors   = PaletteGenerator.GeneratePalette(designToken.GetPresetPrimaryColor(presetColor.Type).Color());
            var colorMap = ColorMap.FromColors(colors);
            designToken.SetColorPalette(presetColor, colorMap);
        }
    }
}