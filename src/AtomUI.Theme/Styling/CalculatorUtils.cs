using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Theme.Styling;

internal struct RadiusInfo
{
    public double BorderRadiusXS { get; set; }
    public double BorderRadiusSM { get; set; }
    public double BorderRadiusLG { get; set; }
    public double BorderRadius { get; set; }
    public double BorderRadiusOuter { get; set; }
}


internal struct FontSizeInfo
{
    public double Size { get; set; }
    public double LineHeight { get; set; }
}


internal static class CalculatorUtils
{
    public static StyleMapDesignToken GenerateStyleMapToken(SeedDesignToken seedToken)
    {
        var motionUnit   = seedToken.MotionUnit;
        var motionBase   = seedToken.MotionBase;
        var borderRadius = seedToken.BorderRadius.TopLeft;
        var lineWidth    = seedToken.LineWidth;

        var radiusInfo = CalculateRadius(borderRadius);

        var token = new StyleMapDesignToken
        {
            // motion
            MotionDurationFast     = TimeSpan.FromMilliseconds(motionBase + motionUnit),
            MotionDurationMid      = TimeSpan.FromMilliseconds(motionBase + motionUnit * 2),
            MotionDurationSlow     = TimeSpan.FromMilliseconds(motionBase + motionUnit * 3),
            MotionDurationVerySlow = TimeSpan.FromMilliseconds(motionBase + motionUnit * 8),

            // line
            LineWidthBold = lineWidth + 1,

            // radius
            BorderRadiusXS    = new CornerRadius(radiusInfo.BorderRadiusXS),
            BorderRadiusSM    = new CornerRadius(radiusInfo.BorderRadiusSM),
            BorderRadiusLG    = new CornerRadius(radiusInfo.BorderRadiusLG),
            BorderRadiusOuter = new CornerRadius(radiusInfo.BorderRadiusOuter),
            BorderThickness   = new Thickness(lineWidth)
        };
        return token;
    }

    public static SizeMapDesignToken GenerateSizeMapToken(SeedDesignToken seedToken)
    {
        var sizeUnit = seedToken.SizeUnit;
        var sizeStep = seedToken.SizeStep;

        var sizeMapToken = new SizeMapDesignToken
        {
            SizeXXL = sizeUnit * (sizeStep + 8), // 48
            SizeXL  = sizeUnit * (sizeStep + 4), // 32
            SizeLG  = sizeUnit * (sizeStep + 2), // 24
            SizeMD  = sizeUnit * (sizeStep + 1), // 20
            SizeMS  = sizeUnit * sizeStep,       // 16
            Size    = sizeUnit * sizeStep,       // 16
            SizeSM  = sizeUnit * (sizeStep - 1), // 12
            SizeXS  = sizeUnit * (sizeStep - 2), // 8
            SizeXXS = sizeUnit * (sizeStep - 3)  // 4
        };
        return sizeMapToken;
    }

    public static FontMapDesignToken GenerateFontMapToken(double fontSize)
    {
        var           fontSizePairs = CalculateFontSize(fontSize);
        IList<double> fontSizes     = fontSizePairs.Select(item => item.Size).ToList();
        IList<double> lineHeights   = fontSizePairs.Select(item => item.LineHeight).ToList();

        var fontSizeMD   = fontSizes[1];
        var fontSizeSM   = fontSizes[0];
        var fontSizeLG   = fontSizes[2];
        var lineHeight   = lineHeights[1];
        var lineHeightSM = lineHeights[0];
        var lineHeightLG = lineHeights[2];

        var fontMapToken = new FontMapDesignToken
        {
            FontSizeSM = fontSizeSM,
            FontSize   = fontSizeMD,
            FontSizeLG = fontSizeLG,
            FontSizeXL = fontSizes[3],

            FontSizeHeading1 = fontSizes[6],
            FontSizeHeading2 = fontSizes[5],
            FontSizeHeading3 = fontSizes[4],
            FontSizeHeading4 = fontSizes[3],
            FontSizeHeading5 = fontSizes[2],

            LineHeight   = lineHeight,
            LineHeightLG = lineHeightLG,
            LineHeightSM = lineHeightSM,

            FontHeight   = Math.Round(lineHeight   * fontSizeMD),
            FontHeightLG = Math.Round(lineHeightLG * fontSizeLG),
            FontHeightSM = Math.Round(lineHeightSM * fontSizeSM),

            LineHeightHeading1 = lineHeights[6],
            LineHeightHeading2 = lineHeights[5],
            LineHeightHeading3 = lineHeights[4],
            LineHeightHeading4 = lineHeights[3],
            LineHeightHeading5 = lineHeights[2]
        };
        return fontMapToken;
    }

    public static HeightMapDesignToken GenerateControlHeightMapToken(SeedDesignToken seedToken)
    {
        var controlHeight = seedToken.ControlHeight;
        var token = new HeightMapDesignToken
        {
            ControlHeightSM = controlHeight * 0.75,
            ControlHeightXS = controlHeight * 0.5,
            ControlHeightLG = controlHeight * 1.25
        };
        return token;
    }

    public static double CalculateLineHeight(double fontSize)
    {
        return (fontSize + 8) / fontSize;
    }

    public static RadiusInfo CalculateRadius(double radiusBase)
    {
        var radiusLG    = radiusBase;
        var radiusSM    = radiusBase;
        var radiusXS    = radiusBase;
        var radiusOuter = radiusBase;

        // radiusLG
        if (radiusBase < 6 && radiusBase >= 5)
            radiusLG = radiusBase + 1;
        else if (radiusBase < 16 && radiusBase >= 6)
            radiusLG                        = radiusBase + 2;
        else if (radiusBase >= 16) radiusLG = 16;

        // radiusSM
        if (radiusBase < 7 && radiusBase >= 5)
            radiusSM = 4;
        else if (radiusBase < 8 && radiusBase >= 7)
            radiusSM = 5;
        else if (radiusBase < 14 && radiusBase >= 8)
            radiusSM = 6;
        else if (radiusBase < 16 && radiusBase >= 14)
            radiusSM                        = 7;
        else if (radiusBase >= 16) radiusSM = 8;

        // radiusXS
        if (radiusBase < 6 && radiusBase >= 2)
            radiusXS                       = 1;
        else if (radiusBase >= 6) radiusXS = 2;

        // radiusOuter
        if (radiusBase > 4 && radiusBase < 8)
            radiusOuter                       = 4;
        else if (radiusBase >= 8) radiusOuter = 6;

        return new RadiusInfo
        {
            BorderRadius      = radiusBase,
            BorderRadiusXS    = radiusXS,
            BorderRadiusSM    = radiusSM,
            BorderRadiusLG    = radiusLG,
            BorderRadiusOuter = radiusOuter
        };
    }

    public static IList<FontSizeInfo> CalculateFontSize(double baseValue)
    {
        var fontSizes = new List<double>(10);
        for (var index = 0; index < 10; ++index)
        {
            var i        = index - 1;
            var baseSize = baseValue * Math.Pow(2.71828, i / 5.0);
            var intSize  = index > 1 ? Math.Floor(baseSize) : Math.Ceiling(baseSize);

            // Convert to even
            fontSizes.Add((int)(Math.Floor(intSize / 2.0d) * 2));
        }

        fontSizes[1] = baseValue;
        var results = new List<FontSizeInfo>();
        foreach (var size in fontSizes)
            results.Add(new FontSizeInfo
            {
                Size       = size,
                LineHeight = CalculateLineHeight(size)
            });

        return results;
    }
}