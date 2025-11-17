using Avalonia;
using Avalonia.Media;

namespace AtomUI.Media;

public static class BoxShadowExtensions
{
    public static Thickness Thickness(this BoxShadow boxShadow)
    {
        var offsetX      = boxShadow.OffsetX;
        var offsetY      = boxShadow.OffsetY;
        var blurRadius   = boxShadow.Blur;
        var spreadRadius = boxShadow.Spread;

        var value  = Math.Max(blurRadius + spreadRadius, 0.0); // 可以正负抵消
        var left   = Math.Max(value - offsetX, 0.0);
        var right  = Math.Max(value + offsetX, 0.0);
        var top    = Math.Max(value - offsetY, 0.0);
        var bottom = Math.Max(value + offsetY, 0.0);
        return new Thickness(left, top, right, bottom);
    }

    public static Thickness Thickness(this BoxShadows boxShadows)
    {
        double leftThickness   = 0;
        double topThickness    = 0;
        double rightThickness  = 0;
        double bottomThickness = 0;
        foreach (var shadow in boxShadows)
        {
            var thickness = shadow.Thickness();
            leftThickness   = Math.Max(leftThickness, thickness.Left);
            topThickness    = Math.Max(topThickness, thickness.Top);
            rightThickness  = Math.Max(rightThickness, thickness.Right);
            bottomThickness = Math.Max(bottomThickness, thickness.Bottom);
        }

        return new Thickness(leftThickness, topThickness, rightThickness, bottomThickness);
    }
}