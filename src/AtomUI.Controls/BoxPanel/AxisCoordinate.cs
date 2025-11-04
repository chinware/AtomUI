using Avalonia;

namespace AtomUI.Controls;

/// <summary>
/// Coordinate abstraction for orientation-agnostic layout calculations
/// MainAxis = main axis (U in traditional flexbox terminology)
/// CrossAxis = cross axis (V in traditional flexbox terminology)
/// </summary>
internal readonly struct AxisCoordinate
{
    public readonly double MainAxis;
    public readonly double CrossAxis;

    public AxisCoordinate(double mainAxis, double crossAxis)
    {
        MainAxis = mainAxis;
        CrossAxis = crossAxis;
    }

    public static AxisCoordinate FromSize(double width, double height, bool isColumn)
    {
        return isColumn
            ? new AxisCoordinate(height, width)
            : new AxisCoordinate(width, height);
    }

    public static AxisCoordinate FromSize(Size size, bool isColumn)
    {
        return FromSize(size.Width, size.Height, isColumn);
    }

    public static Size ToSize(AxisCoordinate axisCoord, bool isColumn)
    {
        return isColumn
            ? new Size(axisCoord.CrossAxis, axisCoord.MainAxis)
            : new Size(axisCoord.MainAxis, axisCoord.CrossAxis);
    }

    public static Point ToPoint(AxisCoordinate axisCoord, bool isColumn)
    {
        return isColumn
            ? new Point(axisCoord.CrossAxis, axisCoord.MainAxis)
            : new Point(axisCoord.MainAxis, axisCoord.CrossAxis);
    }
}