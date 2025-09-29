using Avalonia;

namespace AtomUI.Animations;

internal class NotifiableCornerRadiusTransition : AbstractNotifiableTransition<CornerRadius>
{
    protected override CornerRadius Interpolate(double progress, CornerRadius from, CornerRadius to)
    {
        var result = InterpolateUtils.CornerRadiusInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}