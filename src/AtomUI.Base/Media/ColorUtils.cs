using AtomUI.Utils;
using Avalonia.Media;

namespace AtomUI.Media;

public enum WCAG2Level
{
    AA,
    AAA
}

public enum WCAG2Size
{
    Large,
    Small
}

public class WCAG2Parms
{
    public WCAG2Level Level { get; set; } = WCAG2Level.AA;
    public WCAG2Size Size { get; set; } = WCAG2Size.Small;
}

public class WCAG2FallbackParms : WCAG2Parms
{
    public bool IncludeFallbackColors { get; set; }
}

public static class ColorUtils
{
    public static Color FromRgbF(double alpha, double red, double green, double blue)
    {
        return Color.FromArgb((byte)Math.Round(alpha * 255d),
            (byte)Math.Round(red * 255d),
            (byte)Math.Round(green * 255d),
            (byte)Math.Round(blue * 255d));
    }

    public static Color FromRgbF(double red, double green, double blue)
    {
        return FromRgbF(1d, red, green, blue);
    }

    public static Color TransparentColor()
    {
        return Color.FromArgb(0, 255, 255, 255);
    }

    public static Color Desaturate(string color, int amount = 10)
    {
        return Color.Parse(color).Desaturate(amount);
    }

    public static Color Saturate(string color, int amount = 10)
    {
        return Color.Parse(color).Saturate(amount);
    }

    public static Color Lighten(string color, int amount = 10)
    {
        return Color.Parse(color).Lighten(amount);
    }

    public static Color Brighten(string color, int amount = 10)
    {
        return Color.Parse(color).Brighten(amount);
    }

    public static Color Darken(string color, int amount = 10)
    {
        return Color.Parse(color).Darken(amount);
    }

    public static Color Spin(string color, int amount = 10)
    {
        return Color.Parse(color).Spin(amount);
    }

    public static Color OnBackground(in Color frontColor, in Color backgroundColor)
    {
        var fr = frontColor.GetRedF();
        var fg = frontColor.GetGreenF();
        var fb = frontColor.GetBlueF();
        var fa = frontColor.GetAlphaF();

        var br = backgroundColor.GetRedF();
        var bg = backgroundColor.GetGreenF();
        var bb = backgroundColor.GetBlueF();
        var ba = backgroundColor.GetAlphaF();

        var alpha = fa + ba * (1 - fa);

        var nr = (fr * fa + br * ba * (1 - fa)) / alpha;
        var ng = (fg * fa + bg * ba * (1 - fa)) / alpha;
        var nb = (fb * fa + bb * ba * (1 - fa)) / alpha;
        var na = alpha;

        return FromRgbF(na, nr, ng, nb);
    }

    public static bool IsStableColor(int color)
    {
        return color >= 0 && color <= 255;
    }

    public static bool IsStableColor(double color)
    {
        return MathUtils.GreaterThanOrClose(color, 0.0d) && MathUtils.LessThanOrClose(color, 255.0d);
    }

    public static Color AlphaColor(in Color frontColor, in Color backgroundColor)
    {
        var fR          = frontColor.R;
        var fG          = frontColor.G;
        var fB          = frontColor.B;
        var originAlpha = frontColor.GetAlphaF();
        if (originAlpha < 1d)
        {
            return frontColor;
        }

        var bR = backgroundColor.R;
        var bG = backgroundColor.G;
        var bB = backgroundColor.B;

        for (var fA = 0.01d; fA <= 1.0d; fA += 0.01d)
        {
            var r = Math.Round((fR - bR * (1d - fA)) / fA);
            var g = Math.Round((fG - bG * (1d - fA)) / fA);
            var b = Math.Round((fB - bB * (1d - fA)) / fA);
            if (IsStableColor(r) && IsStableColor(g) && IsStableColor(b))
            {
                return Color.FromArgb((byte)(Math.Round(fA * 100d) / 100d * 255), (byte)r, (byte)g, (byte)b);
            }
        }

        // fallback
        /* istanbul ignore next */
        return Color.FromArgb(255, fR, fG, fB);
    }

    public static Color ParseCssRgbColor(string colorExpr)
    {
        if (TryParseCssRgbColor(colorExpr, out var color))
        {
            return color;
        }

        throw new FormatException($"Invalid color string: '{colorExpr}'.");
    }

    public static bool TryParseCssRgbColor(string? colorExpr, out Color color)
    {
        color = default;
        if (string.IsNullOrEmpty(colorExpr))
        {
            return false;
        }

        if (colorExpr[0] == '#')
        {
            return Color.TryParse(colorExpr, out color);
        }

        var isRgba = colorExpr.StartsWith("rgba", StringComparison.InvariantCultureIgnoreCase);
        var isRgb  = false;
        if (!isRgba)
        {
            isRgb = colorExpr.StartsWith("rgb", StringComparison.InvariantCultureIgnoreCase);
        }

        if (isRgb || isRgba)
        {
            var leftParen  = colorExpr.IndexOf('(');
            var rightParen = colorExpr.IndexOf(')');
            if (leftParen == -1 || rightParen == -1)
            {
                return false;
            }

            var parts = new List<string>(colorExpr.Substring(leftParen + 1, rightParen - leftParen - 1)
                                                  .Split(',', StringSplitOptions.RemoveEmptyEntries));
            if (isRgb)
            {
                if (parts.Count != 3)
                {
                    return false;
                }

                parts.Add("255");
            }
            else
            {
                if (parts.Count != 4)
                {
                    return false;
                }
            }

            var rgbaValues = new List<int>();
            foreach (var part in parts)
            {
                if (int.TryParse(part, out var partValue))
                {
                    rgbaValues.Add(partValue);
                }
                else
                {
                    return false;
                }
            }

            color = Color.FromArgb((byte)rgbaValues[0], (byte)rgbaValues[1], (byte)rgbaValues[2], (byte)rgbaValues[3]);
            return true;
        }

        return false;
    }

    /// Readability Functions
    /// ---------------------
    /// <http:// www.w3.org/ TR/2008/ REC-WCAG20-20081211/# contrast-ratiodef ( WCAG Version 2)
    /// AKA ` contrast`
    /// Analyze the 2 colors and returns the color contrast defined by ( WCAG Version
    /// 2
    /// )
    public static double Readability(Color color1, Color color2)
    {
        return (Math.Max(color1.GetLuminance(), color2.GetLuminance()) + 0.05) /
               (Math.Min(color1.GetLuminance(), color2.GetLuminance()) + 0.05);
    }

    /// Ensure that foreground and background color combinations meet WCAG2 guidelines.
    /// The third argument is an object.
    /// the 'level' property states 'AA' or 'AAA' - if missing or invalid, it defaults to 'AA';
    /// the 'size' property states 'large' or 'small' - if missing or invalid, it defaults to 'small'.
    /// If the entire object is absent, isReadable defaults to {level:"AA",size:"small"}.
    /// 
    /// Example
    /// new Color().IsReadable('#000', '#111') => false
    /// new Color().IsReadable('#000', '#111', { level: 'AA', size: 'large' }) => false
    public static bool IsReadable(Color color1, Color color2, WCAG2Parms? wcag2 = null)
    {
        wcag2 ??= new WCAG2Parms();
        var readabilityLevel = Readability(color1, color2);
        if (wcag2.Level == WCAG2Level.AA)
        {
            if (wcag2.Size == WCAG2Size.Large)
            {
                return MathUtils.GreaterThanOrClose(readabilityLevel, 3);
            }

            return MathUtils.GreaterThanOrClose(readabilityLevel, 4.5);
        }

        if (wcag2.Level == WCAG2Level.AAA)
        {
            if (wcag2.Size == WCAG2Size.Large)
            {
                return MathUtils.GreaterThanOrClose(readabilityLevel, 4.5);
            }

            return MathUtils.GreaterThanOrClose(readabilityLevel, 7);
        }

        return false;
    }

    /// Given a base color and a list of possible foreground or background
    /// colors for that base, returns the most readable color.
    /// Optionally returns Black or White if the most readable color is unreadable.
    /// 
    /// @param baseColor - the base color.
    /// @param colorList - array of colors to pick the most readable one from.
    /// @param args - and object with extra arguments
    /// 
    /// Example
    /// new Color().mostReadable('#123', ['#124", "#125'], { includeFallbackColors: false }).toHexString(); // "#112255"
    /// new Color().mostReadable('#123', ['#124", "#125'],{ includeFallbackColors: true }).toHexString();  // "#ffffff"
    /// new Color().mostReadable('#a8015a', ["#faf3f3"], { includeFallbackColors:true, level: 'AAA', size: 'large' }).toHexString(); // "#faf3f3"
    /// new Color().mostReadable('#a8015a', ["#faf3f3"], { includeFallbackColors:true, level: 'AAA', size: 'small' }).toHexString(); // "#ffffff"
    public static Color? MostReadable(Color baseColor, List<Color> colorList, WCAG2FallbackParms? args = null)
    {
        args ??= new WCAG2FallbackParms
        {
            IncludeFallbackColors = false,
            Level                 = WCAG2Level.AA,
            Size                  = WCAG2Size.Small
        };
        Color? bestColor = null;
        var    bestScore = 0d;
        foreach (var color in colorList)
        {
            var score = Readability(baseColor, color);
            if (score > bestScore)
            {
                bestScore = score;
                bestColor = color;
            }
        }

        if (IsReadable(baseColor, bestColor!.Value, new WCAG2Parms { Level = args.Level, Size = args.Size }) ||
            !args.IncludeFallbackColors)
        {
            return bestColor;
        }

        args.IncludeFallbackColors = false;
        return MostReadable(baseColor, new List<Color>
        {
            Color.Parse("#fff"),
            Color.Parse("#000")
        }, args);
    }
    
    public static HsvColor ToHsv(
        double r,
        double g,
        double b,
        double a = 1.0)
    {
        // Note: Conversion code is originally based on the C++ in WinUI (licensed MIT)
        // https://github.com/microsoft/microsoft-ui-xaml/blob/main/dev/Common/ColorConversion.cpp
        // This was used because it is the best documented and likely most optimized for performance
        // Alpha support was added

        double hue;
        double saturation;
        double value;

        double max = r >= g ? (r >= b ? r : b) : (g >= b ? g : b);
        double min = r <= g ? (r <= b ? r : b) : (g <= b ? g : b);

        // The value, a number between 0 and 1, is the largest of R, G, and B (divided by 255).
        // Conceptually speaking, it represents how much color is present.
        // If at least one of R, G, B is 255, then there exists as much color as there can be.
        // If RGB = (0, 0, 0), then there exists no color at all - a value of zero corresponds
        // to black (i.e., the absence of any color).
        value = max;

        // The "chroma" of the color is a value directly proportional to the extent to which
        // the color diverges from greyscale.  If, for example, we have RGB = (255, 255, 0),
        // then the chroma is maximized - this is a pure yellow, no gray of any kind.
        // On the other hand, if we have RGB = (128, 128, 128), then the chroma being zero
        // implies that this color is pure greyscale, with no actual hue to be found.
        var chroma = max - min;

        // If the chrome is zero, then hue is technically undefined - a greyscale color
        // has no hue.  For the sake of convenience, we'll just set hue to zero, since
        // it will be unused in this circumstance.  Since the color is purely gray,
        // saturation is also equal to zero - you can think of saturation as basically
        // a measure of hue intensity, such that no hue at all corresponds to a
        // nonexistent intensity.
        if (chroma == 0)
        {
            hue        = 0.0;
            saturation = 0.0;
        }
        else
        {
            // In this block, hue is properly defined, so we'll extract both hue
            // and saturation information from the RGB color.

            // Hue can be thought of as a cyclical thing, between 0 degrees and 360 degrees.
            // A hue of 0 degrees is red; 120 degrees is green; 240 degrees is blue; and 360 is back to red.
            // Every other hue is somewhere between either red and green, green and blue, and blue and red,
            // so every other hue can be thought of as an angle on this color wheel.
            // These if/else statements determines where on this color wheel our color lies.
            if (r == max)
            {
                // If the red channel is the most pronounced channel, then we exist
                // somewhere between (-60, 60) on the color wheel - i.e., the section around 0 degrees
                // where red dominates.  We figure out where in that section we are exactly
                // by considering whether the green or the blue channel is greater - by subtracting green from blue,
                // then if green is greater, we'll nudge ourselves closer to 60, whereas if blue is greater, then
                // we'll nudge ourselves closer to -60.  We then divide by chroma (which will actually make the result larger,
                // since chroma is a value between 0 and 1) to normalize the value to ensure that we get the right hue
                // even if we're very close to greyscale.
                hue = 60 * (g - b) / chroma;
            }
            else if (g == max)
            {
                // We do the exact same for the case where the green channel is the most pronounced channel,
                // only this time we want to see if we should tilt towards the blue direction or the red direction.
                // We add 120 to center our value in the green third of the color wheel.
                hue = 120 + (60 * (b - r) / chroma);
            }
            else // blue == max
            {
                // And we also do the exact same for the case where the blue channel is the most pronounced channel,
                // only this time we want to see if we should tilt towards the red direction or the green direction.
                // We add 240 to center our value in the blue third of the color wheel.
                hue = 240 + (60 * (r - g) / chroma);
            }

            // Since we want to work within the range [0, 360), we'll add 360 to any value less than zero -
            // this will bump red values from within -60 to -1 to 300 to 359.  The hue is the same at both values.
            if (hue < 0.0)
            {
                hue += 360.0;
            }

            // The saturation, our final HSV axis, can be thought of as a value between 0 and 1 indicating how intense our color is.
            // To find it, we divide the chroma - the distance between the minimum and the maximum RGB channels - by the maximum channel (i.e., the value).
            // This effectively normalizes the chroma - if the maximum is 0.5 and the minimum is 0, the saturation will be (0.5 - 0) / 0.5 = 1,
            // meaning that although this color is not as bright as it can be, the dark color is as intense as it possibly could be.
            // If, on the other hand, the maximum is 0.5 and the minimum is 0.25, then the saturation will be (0.5 - 0.25) / 0.5 = 0.5,
            // meaning that this color is partially washed out.
            // A saturation value of 0 corresponds to a greyscale color, one in which the color is *completely* washed out and there is no actual hue.
            saturation = chroma / value;
        }

        return new HsvColor(a, hue, saturation, value);
    }
}