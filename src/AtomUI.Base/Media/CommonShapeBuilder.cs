using AtomUI.Utils;
using Avalonia;
using Avalonia.Media;
using Avalonia.Utilities;

namespace AtomUI.Media;

public static class CommonShapeBuilder
{
    public static Geometry BuildCheckMark(Size size)
    {
        var       checkMark = new StreamGeometry();
        using var context   = checkMark.Open();

        var startPoint = new Point(size.Width * 0.25, size.Height * 0.5);
        var midPoint   = new Point(size.Width * 0.4, size.Height * 0.7);
        var endPoint   = new Point(size.Width * 0.7, size.Height * 0.3);

        context.BeginFigure(startPoint, true);
        context.LineTo(midPoint);
        context.LineTo(endPoint);
        context.EndFigure(false);

        return checkMark;
    }

    /// <summary>
    ///     生成一个以矩形中点为圆心，以宽和高最小的一半为半径的且指定角度的圆弧
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="startAngle"></param>
    /// <param name="sweepAngle"></param>
    /// <returns></returns>
    public static Geometry BuildArc(Rect rect, double startAngle, double sweepAngle)
    {
        if (MathUtils.IsZero(sweepAngle))
        {
            return new StreamGeometry();
        }

        var angle1 = MathUtilities.Deg2Rad(startAngle);
        var angle2 = angle1 + MathUtilities.Deg2Rad(sweepAngle);

        startAngle = Math.Min(angle1, angle2);
        sweepAngle = Math.Max(angle1, angle2);

        var normStart = RadToNormRad(startAngle);
        var normEnd   = RadToNormRad(sweepAngle);

        if (MathUtils.AreClose(normStart, normEnd) && !MathUtils.AreClose(startAngle, sweepAngle))
        {
            // Complete ring.
            return new EllipseGeometry(rect);
        }

        // Partial arc.
        var deflatedRect = rect;

        var centerX = rect.Center.X;
        var centerY = rect.Center.Y;

        var radiusX = deflatedRect.Width / 2;
        var radiusY = deflatedRect.Height / 2;

        var angleGap = RadToNormRad(sweepAngle - startAngle);

        var startPoint = GetRingPoint(radiusX, radiusY, centerX, centerY, startAngle);
        var endPoint   = GetRingPoint(radiusX, radiusY, centerX, centerY, sweepAngle);

        var arcGeometry = new StreamGeometry();

        using (var context = arcGeometry.Open())
        {
            context.BeginFigure(startPoint, false);
            context.ArcTo(
                endPoint,
                new Size(radiusX, radiusY),
                angleGap,
                angleGap >= Math.PI,
                SweepDirection.Clockwise);
            context.EndFigure(false);
        }

        return arcGeometry;
    }

    private static double RadToNormRad(double inAngle)
    {
        return (inAngle % (Math.PI * 2) + Math.PI * 2) % (Math.PI * 2);
    }

    private static Point GetRingPoint(double radiusX, double radiusY, double centerX, double centerY, double angle)
    {
        return new Point(radiusX * Math.Cos(angle) + centerX, radiusY * Math.Sin(angle) + centerY);
    }

    public static Geometry BuildArrow(double size, double radius)
    {
        var       geometryStream = new StreamGeometry();
        using var context        = geometryStream.Open();

        // 假设 p1 是原始圆角顶点，p2 和 p3 是原始底边的顶点
        var p1 = new Point(size / 2.0, size / 2.0 + 1);
        var p2 = new Point(0, size);
        var p3 = new Point(size, size);
        // 假设 r 是圆角的半径
        var controlPoint = new Point(p1.X, p1.Y - radius);
        // 移动到调整后的底边左顶点
        context.BeginFigure(p2, true);
        // 绘制左边的直线到圆角的起始点
        context.LineTo(new Point(p1.X - radius, p1.Y));
        // 绘制圆角
        context.QuadraticBezierTo(controlPoint, new Point(p1.X + radius, p1.Y));
        // 绘制右边的直线到调整后的底边右顶点
        context.LineTo(p3);
        // 绘制底边，闭合路径
        context.LineTo(p2);
        context.EndFigure(true);

        return geometryStream;
    }
}