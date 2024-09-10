using Avalonia.Media;

namespace AtomUI.Media;

public static class InterpolateUtils
{
    public static Color ColorInterpolate(Color fromColor, Color toColor, double progress)
    {
        var a1 = fromColor.GetAlphaF();
        var r1 = fromColor.GetRedF() * a1;
        var g1 = fromColor.GetGreenF() * a1;
        var b1 = fromColor.GetBlueF() * a1;

        var a2 = toColor.GetAlphaF();
        var r2 = toColor.GetRedF() * a2;
        var g2 = toColor.GetGreenF() * a2;
        var b2 = toColor.GetBlueF() * a2;

        var r = DoubleInterpolate(r1, r2, progress);
        var g = DoubleInterpolate(g1, g2, progress);
        var b = DoubleInterpolate(b1, b2, progress);
        var a = DoubleInterpolate(a1, a2, progress);

        // 处理接近完全透明的情况
        if (a < 1e-5)
        {
            // 在这种情况下，我们可以选择直接使用目标颜色的RGB分量
            r = toColor.GetRedF();
            g = toColor.GetGreenF();
            b = toColor.GetBlueF();
        }
        else
        {
            // 如果alpha不为零，反预乘alpha
            r /= a;
            g /= a;
            b /= a;
        }

        // 防止颜色分量超出范围
        r = Math.Clamp(r, 0.0f, 1.0f);
        g = Math.Clamp(g, 0.0f, 1.0f);
        b = Math.Clamp(b, 0.0f, 1.0f);
        a = Math.Clamp(a, 0.0f, 1.0f);

        return ColorUtils.FromRgbF(a, r, g, b);
    }

    public static double DoubleInterpolate(double oldValue, double newValue, double progress)
    {
        return (newValue - oldValue) * progress + oldValue;
    }
}