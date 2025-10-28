using System;
using Avalonia;

namespace AtomUI.Controls;

/// <summary>
/// Coordinate abstraction for orientation-agnostic layout calculations
/// U = main axis, V = cross axis
/// </summary>
internal readonly struct Uv
{
    public readonly double U;
    public readonly double V;

    public Uv(double u, double v)
    {
        U = u;
        V = v;
    }

    public static Uv FromSize(double width, double height, bool isColumn)
    {
        return isColumn
            ? new Uv(height, width)
            : new Uv(width, height);
    }

    public static Uv FromSize(Size size, bool isColumn)
    {
        return FromSize(size.Width, size.Height, isColumn);
    }

    public static Size ToSize(Uv uv, bool isColumn)
    {
        return isColumn
            ? new Size(uv.V, uv.U)
            : new Size(uv.U, uv.V);
    }

    public static Point ToPoint(Uv uv, bool isColumn)
    {
        return isColumn
            ? new Point(uv.V, uv.U)
            : new Point(uv.U, uv.V);
    }
}