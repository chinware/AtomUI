using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Animations;

public class SolidColorBrushTransition : InterpolatingTransitionBase<ISolidColorBrush?>
{
    protected override ISolidColorBrush? Interpolate(double progress, ISolidColorBrush? from, ISolidColorBrush? to)
    {
        return InterpolateUtils.SolidColorBrushInterpolate(progress, from, to);
    }
}