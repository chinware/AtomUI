using System.Globalization;

namespace AtomUI.Controls;

public record GridGutterInfo
{
    public double ExtraSmall { get; init; }
    public double Small { get; init; }
    public double Medium { get; init; }
    public double Large { get; init; }
    public double ExtraLarge { get; init; }
    public double ExtraExtraLarge { get; init; }
    public double ExtraExtraExtraLarge { get; init; }

    public GridGutterInfo()
        : this(0)
    {
    }

    public GridGutterInfo(double value)
    {
        ExtraSmall      = value;
        Small           = value;
        Medium          = value;
        Large           = value;
        ExtraLarge      = value;
        ExtraExtraLarge = value;
        ExtraExtraExtraLarge = value;
    }

    public GridGutterInfo(double extraSmall, double small, double medium, double large, double extraLarge, double extraExtraLarge)
        : this(extraSmall, small, medium, large, extraLarge, extraExtraLarge, extraExtraLarge)
    {
    }

    public GridGutterInfo(double extraSmall,
                          double small,
                          double medium,
                          double large,
                          double extraLarge,
                          double extraExtraLarge,
                          double extraExtraExtraLarge)
    {
        ExtraSmall      = extraSmall;
        Small           = small;
        Medium          = medium;
        Large           = large;
        ExtraLarge      = extraLarge;
        ExtraExtraLarge = extraExtraLarge;
        ExtraExtraExtraLarge = extraExtraExtraLarge;
    }

    public static GridGutterInfo Parse(string input)
    {
        return Parse(input.AsSpan());
    }

    internal static GridGutterInfo Parse(ReadOnlySpan<char> input)
    {
        var trimmed = input.Trim();
        if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var single))
        {
            ValidateGutterValue(single);
            return new GridGutterInfo(single);
        }

        var responsive = ResponsiveDouble.Parse(trimmed.ToString());
        return new GridGutterInfo(
            responsive.Resolve(MediaBreakPoint.ExtraSmall, 0),
            responsive.Resolve(MediaBreakPoint.Small, 0),
            responsive.Resolve(MediaBreakPoint.Medium, 0),
            responsive.Resolve(MediaBreakPoint.Large, 0),
            responsive.Resolve(MediaBreakPoint.ExtraLarge, 0),
            responsive.Resolve(MediaBreakPoint.ExtraExtraLarge, 0),
            responsive.Resolve(MediaBreakPoint.ExtraExtraExtraLarge, 0));
    }

    internal double GetValue(MediaBreakPoint breakPoint)
    {
        return breakPoint switch
        {
            MediaBreakPoint.ExtraSmall => ExtraSmall,
            MediaBreakPoint.Small => Small,
            MediaBreakPoint.Medium => Medium,
            MediaBreakPoint.Large => Large,
            MediaBreakPoint.ExtraLarge => ExtraLarge,
            MediaBreakPoint.ExtraExtraLarge => ExtraExtraLarge,
            _ => ExtraExtraExtraLarge
        };
    }

    private static void ValidateGutterValue(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
        {
            throw new FormatException($"Gutter value must be >= 0, got {value}.");
        }
    }
}
