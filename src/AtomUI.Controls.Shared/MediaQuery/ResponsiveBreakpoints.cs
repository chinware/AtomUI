namespace AtomUI.Controls;

internal static class ResponsiveBreakpoints
{
    public static readonly MediaBreakPoint[] Ascending =
    [
        MediaBreakPoint.ExtraSmall,
        MediaBreakPoint.Small,
        MediaBreakPoint.Medium,
        MediaBreakPoint.Large,
        MediaBreakPoint.ExtraLarge,
        MediaBreakPoint.ExtraExtraLarge,
        MediaBreakPoint.ExtraExtraExtraLarge
    ];

    public static readonly MediaBreakPoint[] Descending =
    [
        MediaBreakPoint.ExtraExtraExtraLarge,
        MediaBreakPoint.ExtraExtraLarge,
        MediaBreakPoint.ExtraLarge,
        MediaBreakPoint.Large,
        MediaBreakPoint.Medium,
        MediaBreakPoint.Small,
        MediaBreakPoint.ExtraSmall
    ];

    public static bool TryParse(ReadOnlySpan<char> input, out MediaBreakPoint breakPoint)
    {
        var normalized = input.Trim();
        if (normalized.Equals("xs".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            breakPoint = MediaBreakPoint.ExtraSmall;
            return true;
        }
        if (normalized.Equals("sm".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            breakPoint = MediaBreakPoint.Small;
            return true;
        }
        if (normalized.Equals("md".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            breakPoint = MediaBreakPoint.Medium;
            return true;
        }
        if (normalized.Equals("lg".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            breakPoint = MediaBreakPoint.Large;
            return true;
        }
        if (normalized.Equals("xl".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            breakPoint = MediaBreakPoint.ExtraLarge;
            return true;
        }
        if (normalized.Equals("xxl".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            breakPoint = MediaBreakPoint.ExtraExtraLarge;
            return true;
        }
        if (normalized.Equals("xxxl".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            breakPoint = MediaBreakPoint.ExtraExtraExtraLarge;
            return true;
        }

        breakPoint = default;
        return false;
    }
}
