using Avalonia;
using Avalonia.Animation;

namespace AtomUI.Media;

public class PixelPointTransition : InterpolatingTransitionBase<PixelPoint>
{
    protected override PixelPoint Interpolate(double progress, PixelPoint oldValue, PixelPoint newValue)
    {
        var delta = newValue - oldValue;
        return new PixelPoint((int)Math.Round(delta.X * progress + oldValue.X),
            (int)Math.Round(delta.Y                   * progress + oldValue.Y));
    }
}