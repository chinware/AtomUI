using Avalonia;

namespace AtomUI.Animations;

internal class NotifiableVectorTransition : AbstractNotifiableTransition<Vector>
{
    protected override Vector Interpolate(double progress, Vector from, Vector to)
    {
        var result = InterpolateUtils.VectorInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}