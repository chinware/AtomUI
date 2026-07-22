using Avalonia;

namespace AtomUI.Desktop.Controls;

internal readonly record struct DialogSizeConstraints(
    double MinWidth,
    double MinHeight,
    double MaxWidth,
    double MaxHeight)
{
    internal static DialogSizeConstraints Resolve(
        Size structuralMinimum,
        Size requestedMinimum,
        Size requestedMaximum,
        Size capacity)
    {
        var capacityWidth  = NormalizeCapacity(capacity.Width);
        var capacityHeight = NormalizeCapacity(capacity.Height);
        var minWidth = Math.Min(
            Math.Max(NormalizeMinimum(structuralMinimum.Width), NormalizeMinimum(requestedMinimum.Width)),
            capacityWidth);
        var minHeight = Math.Min(
            Math.Max(NormalizeMinimum(structuralMinimum.Height), NormalizeMinimum(requestedMinimum.Height)),
            capacityHeight);
        var maxWidth = Math.Max(
            minWidth,
            Math.Min(NormalizeMaximum(requestedMaximum.Width, capacityWidth), capacityWidth));
        var maxHeight = Math.Max(
            minHeight,
            Math.Min(NormalizeMaximum(requestedMaximum.Height, capacityHeight), capacityHeight));

        return new DialogSizeConstraints(minWidth, minHeight, maxWidth, maxHeight);
    }

    internal Size Clamp(Size size)
    {
        return new Size(
            Math.Clamp(size.Width, MinWidth, MaxWidth),
            Math.Clamp(size.Height, MinHeight, MaxHeight));
    }

    private static double NormalizeMinimum(double value)
    {
        return double.IsFinite(value) ? Math.Max(0, value) : 0;
    }

    private static double NormalizeMaximum(double value, double capacity)
    {
        return double.IsFinite(value) ? Math.Max(0, value) : capacity;
    }

    private static double NormalizeCapacity(double value)
    {
        if (double.IsPositiveInfinity(value))
        {
            return value;
        }

        return double.IsFinite(value) ? Math.Max(0, value) : 0;
    }
}
