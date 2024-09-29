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
    public static void CalculateStyleMapTokenValues(GlobalToken globalToken)
    {
        var motionUnit   = globalToken.MotionUnit;
        var motionBase   = globalToken.MotionBase;
        var borderRadius = globalToken.BorderRadius.TopLeft;
        var lineWidth    = globalToken.LineWidth;

        var radiusInfo = CalculateRadius(borderRadius);

        // motion
        globalToken.MotionDurationFast     = TimeSpan.FromMilliseconds(motionBase + motionUnit);
        globalToken.MotionDurationMid      = TimeSpan.FromMilliseconds(motionBase + motionUnit * 2);
        globalToken.MotionDurationSlow     = TimeSpan.FromMilliseconds(motionBase + motionUnit * 3);
        globalToken.MotionDurationVerySlow = TimeSpan.FromMilliseconds(motionBase + motionUnit * 8);
        
        // line
        globalToken.LineWidthBold = lineWidth + 1;
        
        // radius
        globalToken.BorderRadiusXS    = new CornerRadius(radiusInfo.BorderRadiusXS);
        globalToken.BorderRadiusSM    = new CornerRadius(radiusInfo.BorderRadiusSM);
        globalToken.BorderRadiusLG    = new CornerRadius(radiusInfo.BorderRadiusLG);
        globalToken.BorderRadiusOuter = new CornerRadius(radiusInfo.BorderRadiusOuter);
        globalToken.BorderThickness   = new Thickness(lineWidth);
    }

    public static void CalculateSizeMapTokenValues(GlobalToken globalToken)
    {
        var sizeUnit = globalToken.SizeUnit;
        var sizeStep = globalToken.SizeStep;

        globalToken.SizeXXL = sizeUnit * (sizeStep + 8); // 48
        globalToken.SizeXL  = sizeUnit * (sizeStep + 4); // 32
        globalToken.SizeLG  = sizeUnit * (sizeStep + 2); // 24
        globalToken.SizeMD  = sizeUnit * (sizeStep + 1); // 20
        globalToken.SizeMS  = sizeUnit * sizeStep; // 16
        globalToken.Size    = sizeUnit * sizeStep; // 16
        globalToken.SizeSM  = sizeUnit * (sizeStep - 1); // 12
        globalToken.SizeXS  = sizeUnit * (sizeStep - 2); // 8
        globalToken.SizeXXS = sizeUnit * (sizeStep - 3); // 4
    }

    public static void CalculateFontMapTokenValues(GlobalToken globalToken)
    {
        var           fontSizePairs = CalculateFontSize(globalToken.FontSize); // Smaller size font-size as base
        IList<double> fontSizes     = fontSizePairs.Select(item => item.Size).ToList();
        IList<double> lineHeights   = fontSizePairs.Select(item => item.LineHeight).ToList();

        var fontSizeMD   = fontSizes[1];
        var fontSizeSM   = fontSizes[0];
        var fontSizeLG   = fontSizes[2];
        var lineHeight   = lineHeights[1];
        var lineHeightSM = lineHeights[0];
        var lineHeightLG = lineHeights[2];

        globalToken.FontSizeSM = fontSizeSM;
        globalToken.FontSize   = fontSizeMD;
        globalToken.FontSizeLG = fontSizeLG;
        globalToken.FontSizeXL = fontSizes[3];

        globalToken.FontSizeHeading1 = fontSizes[6];
        globalToken.FontSizeHeading2 = fontSizes[5];
        globalToken.FontSizeHeading3 = fontSizes[4];
        globalToken.FontSizeHeading4 = fontSizes[3];
        globalToken.FontSizeHeading5 = fontSizes[2];

        globalToken.LineHeight   = lineHeight;
        globalToken.LineHeightLG = lineHeightLG;
        globalToken.LineHeightSM = lineHeightSM;

        globalToken.FontHeight   = Math.Round(lineHeight * fontSizeMD);
        globalToken.FontHeightLG = Math.Round(lineHeightLG * fontSizeLG);
        globalToken.FontHeightSM = Math.Round(lineHeightSM * fontSizeSM);

        globalToken.LineHeightHeading1 = lineHeights[6];
        globalToken.LineHeightHeading2 = lineHeights[5];
        globalToken.LineHeightHeading3 = lineHeights[4];
        globalToken.LineHeightHeading4 = lineHeights[3];
        globalToken.LineHeightHeading5 = lineHeights[2];
    }

    public static void CalculateControlHeightMapTokenValues(GlobalToken globalToken)
    {
        var controlHeight = globalToken.ControlHeight;
        globalToken.ControlHeightSM = controlHeight * 0.75;
        globalToken.ControlHeightXS = controlHeight * 0.5;
        globalToken.ControlHeightLG = controlHeight * 1.25;
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
        {
            radiusLG = radiusBase + 1;
        }
        else if (radiusBase < 16 && radiusBase >= 6)
        {
            radiusLG = radiusBase + 2;
        }
        else if (radiusBase >= 16)
        {
            radiusLG = 16;
        }

        // radiusSM
        if (radiusBase < 7 && radiusBase >= 5)
        {
            radiusSM = 4;
        }
        else if (radiusBase < 8 && radiusBase >= 7)
        {
            radiusSM = 5;
        }
        else if (radiusBase < 14 && radiusBase >= 8)
        {
            radiusSM = 6;
        }
        else if (radiusBase < 16 && radiusBase >= 14)
        {
            radiusSM = 7;
        }
        else if (radiusBase >= 16)
        {
            radiusSM = 8;
        }

        // radiusXS
        if (radiusBase < 6 && radiusBase >= 2)
        {
            radiusXS = 1;
        }
        else if (radiusBase >= 6)
        {
            radiusXS = 2;
        }

        // radiusOuter
        if (radiusBase > 4 && radiusBase < 8)
        {
            radiusOuter = 4;
        }
        else if (radiusBase >= 8)
        {
            radiusOuter = 6;
        }

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
        {
            results.Add(new FontSizeInfo
            {
                Size       = size,
                LineHeight = CalculateLineHeight(size)
            });
        }

        return results;
    }
}