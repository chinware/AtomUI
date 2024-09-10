using System.Collections.Specialized;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Media;

public static class PenUtils
{
    /// <summary>
    /// Smart reuse and update pen properties.
    /// </summary>
    /// <param name="pen">Old pen to modify.</param>
    /// <param name="brush">The brush used to draw.</param>
    /// <param name="thickness">The stroke thickness.</param>
    /// <param name="strokeDashArray">The stroke dask array.</param>
    /// <param name="strokeDaskOffset">The stroke dask offset.</param>
    /// <param name="lineCap">The line cap.</param>
    /// <param name="lineJoin">The line join.</param>
    /// <param name="miterLimit">The miter limit.</param>
    /// <returns>If a new instance was created and visual invalidation required.</returns>
    internal static bool TryModifyOrCreate(ref IPen? pen,
                                           IBrush? brush,
                                           double thickness,
                                           IList<double>? strokeDashArray = null,
                                           double strokeDaskOffset = default,
                                           PenLineCap lineCap = PenLineCap.Flat,
                                           PenLineJoin lineJoin = PenLineJoin.Miter,
                                           double miterLimit = 10.0)
    {
        var previousPen = pen;
        if (brush is null)
        {
            pen = null;
            return previousPen is not null;
        }

        IDashStyle? dashStyle = null;
        if (strokeDashArray is { Count: > 0 })

            // strokeDashArray can be IList (instead of AvaloniaList) in future
            // So, if it supports notification - create a mutable DashStyle
        {
            dashStyle = strokeDashArray is INotifyCollectionChanged
                ? new DashStyle(strokeDashArray, strokeDaskOffset)
                : new ImmutableDashStyle(strokeDashArray, strokeDaskOffset);
        }

        if (brush is IImmutableBrush immutableBrush && dashStyle is null or ImmutableDashStyle)
        {
            pen = new ImmutablePen(
                immutableBrush,
                thickness,
                (ImmutableDashStyle?)dashStyle,
                lineCap,
                lineJoin,
                miterLimit);

            return true;
        }

        var mutablePen = previousPen as Pen ?? new Pen();
        mutablePen.Brush      = brush;
        mutablePen.Thickness  = thickness;
        mutablePen.LineCap    = lineCap;
        mutablePen.LineJoin   = lineJoin;
        mutablePen.DashStyle  = dashStyle;
        mutablePen.MiterLimit = miterLimit;

        pen = mutablePen;
        return !Equals(previousPen, pen);
    }
}