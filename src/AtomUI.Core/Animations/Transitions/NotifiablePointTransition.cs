
using Avalonia;

namespace AtomUI.Animations;

internal class NotifiablePointTransition : AbstractNotifiableTransition<Point>
{
    protected override Point Interpolate(double progress, Point from, Point to)
    {
        var result = InterpolateUtils.PointInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}