using Avalonia;
using Avalonia.Layout;

namespace AtomUI.Utils;

internal static class BorderUtils
{
    public static Thickness BuildRenderScaleAwareThickness(Layoutable owner, Thickness borderThickness)
    {
        if (!owner.UseLayoutRounding)
        {
            return borderThickness;
        }

        var scale = LayoutHelper.GetLayoutScale(owner);
        return BuildRenderScaleAwareThickness(borderThickness, scale);
    }

    public static double BuildRenderScaleAwareThickness(Layoutable owner, double thickness)
    {
        if (!owner.UseLayoutRounding)
        {
            return thickness;
        }

        var scale = LayoutHelper.GetLayoutScale(owner);
        return BuildRenderScaleAwareThickness(thickness, scale);
    }

    public static Thickness BuildRenderScaleAwareThickness(in Thickness borderThickness, double renderScaling)
    {
        if (MathUtils.AreClose(renderScaling, 0.0) ||
            MathUtils.AreClose(renderScaling, Math.Floor(renderScaling)))
        {
            return borderThickness;
        }

        return new Thickness(
            borderThickness.Left / renderScaling,
            borderThickness.Top / renderScaling,
            borderThickness.Right / renderScaling,
            borderThickness.Bottom / renderScaling);
    }

    public static double BuildRenderScaleAwareThickness(double thickness, double renderScaling)
    {
        if (MathUtils.AreClose(renderScaling, 0.0) ||
            MathUtils.AreClose(renderScaling, Math.Floor(renderScaling)))
        {
            return thickness;
        }

        return thickness / renderScaling;
    }
}
