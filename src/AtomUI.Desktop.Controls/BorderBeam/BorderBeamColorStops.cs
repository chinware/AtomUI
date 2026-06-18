using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal readonly record struct BorderBeamResolvedColorStop(Color Color, double Offset);

internal static class BorderBeamColorStops
{
    private static readonly Color Transparent = Colors.Transparent;

    public static IEnumerable<BorderBeamResolvedColorStop> Normalize(
        IEnumerable<BorderBeamColorStop>? colorStops,
        Color defaultStartColor,
        Color defaultEndColor,
        double maxVisibleStopPercent)
    {
        var maxVisibleOffset = NormalizeVisibleOffset(maxVisibleStopPercent);
        var normalizedStops = NormalizeExplicitStops(colorStops, maxVisibleOffset);

        if (normalizedStops.Count == 0)
        {
            normalizedStops.Add(new BorderBeamResolvedColorStop(defaultStartColor, 0d));
            normalizedStops.Add(new BorderBeamResolvedColorStop(defaultEndColor, maxVisibleOffset));
        }

        normalizedStops.Sort(static (left, right) => left.Offset.CompareTo(right.Offset));
        if (normalizedStops[^1].Offset < maxVisibleOffset)
        {
            normalizedStops.Add(new BorderBeamResolvedColorStop(normalizedStops[^1].Color, maxVisibleOffset));
        }

        normalizedStops.Add(new BorderBeamResolvedColorStop(Transparent, 1d));
        return normalizedStops;
    }

    public static IEnumerable<BorderBeamResolvedColorStop> Normalize(
        IEnumerable<BorderBeamColorStop>? colorStops,
        Color? color,
        Color defaultStartColor,
        Color defaultEndColor,
        double maxVisibleStopPercent)
    {
        if (colorStops?.Any() == true)
        {
            return Normalize(colorStops, defaultStartColor, defaultEndColor, maxVisibleStopPercent);
        }

        if (color is { } value)
        {
            return Normalize(
                new[]
                {
                    new BorderBeamColorStop { Color = value, Percent = 0 },
                    new BorderBeamColorStop { Color = value, Percent = 100 }
                },
                defaultStartColor,
                defaultEndColor,
                maxVisibleStopPercent);
        }

        return Normalize(colorStops, defaultStartColor, defaultEndColor, maxVisibleStopPercent);
    }

    private static List<BorderBeamResolvedColorStop> NormalizeExplicitStops(
        IEnumerable<BorderBeamColorStop>? colorStops,
        double maxVisibleOffset)
    {
        var normalizedStops = new List<BorderBeamResolvedColorStop>();
        if (colorStops is null)
        {
            return normalizedStops;
        }

        foreach (var colorStop in colorStops)
        {
            normalizedStops.Add(new BorderBeamResolvedColorStop(
                colorStop.Color,
                Math.Round(NormalizePercent(colorStop.Percent) * maxVisibleOffset, 10)));
        }

        return normalizedStops;
    }

    private static double NormalizeVisibleOffset(double maxVisibleStopPercent)
    {
        return NormalizePercent(maxVisibleStopPercent);
    }

    private static double NormalizePercent(double percent)
    {
        if (double.IsNaN(percent) || double.IsInfinity(percent))
        {
            return 0d;
        }

        return Math.Clamp(percent, 0d, 100d) / 100d;
    }
}
