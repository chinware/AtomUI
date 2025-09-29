using Avalonia.Animation;
using Avalonia.Media;

namespace AtomUI.Animations;

internal class SolidColorBrushTransition : InterpolatingTransitionBase<IBrush?>
{
    protected override IBrush? Interpolate(double progress, IBrush? from, IBrush? to)
    {
        if (from is ISolidColorBrush fromBrush && to is ISolidColorBrush toBrush)
        {
            return InterpolateUtils.SolidColorBrushInterpolate(progress, fromBrush, toBrush);
        }
        return InterpolateUtils.BrushInterpolate(progress, from, to);
    }
}