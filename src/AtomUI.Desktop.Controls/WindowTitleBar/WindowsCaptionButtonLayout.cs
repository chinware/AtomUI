using Avalonia;

namespace AtomUI.Desktop.Controls;

internal static class WindowsCaptionButtonLayout
{
    internal static Size NormalizeMeasureConstraint(Size availableSize)
    {
        var width = availableSize.Width;
        if (double.IsInfinity(width))
        {
            width = availableSize.Height;
        }

        return new Size(width, availableSize.Height);
    }

    internal static Size ResolveDesiredSize(Size measureConstraint, Size measuredSize)
    {
        var availableMinSize = Math.Min(measureConstraint.Width, measureConstraint.Height);
        var measuredMinSize = Math.Min(measuredSize.Width, measuredSize.Height);
        var finalSize = Math.Max(measuredMinSize, availableMinSize);
        return new Size(finalSize, finalSize);
    }
}
