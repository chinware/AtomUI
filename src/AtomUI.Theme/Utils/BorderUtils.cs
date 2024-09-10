using AtomUI.Utils;
using Avalonia;

namespace AtomUI.Theme.Utils;

public static class BorderUtils
{
    public static Thickness BuildRenderScaleAwareThickness(in Thickness borderThickness, double renderScaling)
    {
        if (MathUtils.AreClose(renderScaling, Math.Floor(renderScaling)))
        {
            renderScaling = 1.0d; // 这种情况很清晰
        }

        return new Thickness(borderThickness.Left / renderScaling,
            borderThickness.Top / renderScaling,
            borderThickness.Right / renderScaling,
            borderThickness.Bottom / renderScaling);
    }
}