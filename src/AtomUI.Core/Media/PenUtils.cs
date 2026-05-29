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
                                          IReadOnlyList<double>? strokeDashArray = null,
                                          double strokeDaskOffset = 0.0,
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

        if (IsSamePen(previousPen,
                brush,
                thickness,
                strokeDashArray,
                strokeDaskOffset,
                lineCap,
                lineJoin,
                miterLimit))
        {
            return false;
        }

        IDashStyle? dashStyle = null;
        if (strokeDashArray is { Count: > 0 })
        {
            // strokeDashArray can be IList (instead of AvaloniaList) in future
            // So, if it supports notification - create a mutable DashStyle
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

    private static bool IsSamePen(IPen? pen,
                                  IBrush brush,
                                  double thickness,
                                  IReadOnlyList<double>? strokeDashArray,
                                  double strokeDaskOffset,
                                  PenLineCap lineCap,
                                  PenLineJoin lineJoin,
                                  double miterLimit)
    {
        return pen is not null &&
               Equals(pen.Brush, brush) &&
               pen.Thickness.Equals(thickness) &&
               pen.LineCap == lineCap &&
               pen.LineJoin == lineJoin &&
               pen.MiterLimit.Equals(miterLimit) &&
               IsSameDashStyle(pen.DashStyle, strokeDashArray, strokeDaskOffset);
    }

    private static bool IsSameDashStyle(IDashStyle? dashStyle,
                                        IReadOnlyList<double>? strokeDashArray,
                                        double strokeDaskOffset)
    {
        if (strokeDashArray is not { Count: > 0 })
        {
            return dashStyle is null || dashStyle.Dashes is not { Count: > 0 };
        }

        if (dashStyle is null ||
            !dashStyle.Offset.Equals(strokeDaskOffset) ||
            dashStyle.Dashes is not { } dashes ||
            dashes.Count != strokeDashArray.Count)
        {
            return false;
        }

        for (var i = 0; i < strokeDashArray.Count; i++)
        {
            if (!dashes[i].Equals(strokeDashArray[i]))
            {
                return false;
            }
        }

        return true;
    }
}
