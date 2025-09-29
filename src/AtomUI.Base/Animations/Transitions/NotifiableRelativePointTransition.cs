using Avalonia;

namespace AtomUI.Animations;

internal class NotifiableRelativePointTransition : AbstractNotifiableTransition<RelativePoint>
{
    protected override RelativePoint Interpolate(double progress, RelativePoint from, RelativePoint to)
    {
        var result = InterpolateUtils.PointInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}