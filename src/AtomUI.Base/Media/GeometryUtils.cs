using Avalonia;

namespace AtomUI.Media;

public static class GeometryUtils
{
    public static Rect CalculateScaledRect(in Rect rect, double scaleFactorX, double scaleFactorY, in Point origin)
    {
        // 计算原点相对于矩形左上角的偏移量
        var originalOffsetX = origin.X - rect.X;
        var originalOffsetY = origin.Y - rect.Y;

        // 计算缩放后的偏移量
        var scaledOffsetX = originalOffsetX * scaleFactorX;
        var scaledOffsetY = originalOffsetY * scaleFactorY;

        // 计算偏移量的变化
        var offsetXChange = scaledOffsetX - originalOffsetX;
        var offsetYChange = scaledOffsetY - originalOffsetY;

        // 计算新的矩形位置
        var newX = rect.X - offsetXChange;
        var newY = rect.Y - offsetYChange;

        // 计算新的矩形宽度和高度
        var newWidth  = rect.Width  * scaleFactorX;
        var newHeight = rect.Height * scaleFactorY;

        return new Rect(newX, newY, newWidth, newHeight);
    }

    public static double CornerRadiusScalarValue(CornerRadius cornerRadius, bool maxWhenNotUniform = true)
    {
        if (cornerRadius.IsUniform) return cornerRadius.TopLeft;

        return maxWhenNotUniform
            ? Math.Max(cornerRadius.TopLeft,
                Math.Max(cornerRadius.TopRight, Math.Max(cornerRadius.BottomLeft, cornerRadius.BottomRight)))
            : Math.Min(cornerRadius.TopLeft,
                Math.Min(cornerRadius.TopRight, Math.Min(cornerRadius.BottomLeft, cornerRadius.BottomRight)));
    }
}