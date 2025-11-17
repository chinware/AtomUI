using Avalonia.Media;

namespace AtomUI.Animations;

internal class NotifiableBoxShadowsTransition : AbstractNotifiableTransition<BoxShadows>
{
    protected override BoxShadows Interpolate(double progress, BoxShadows from, BoxShadows to)
    {
        var result = InterpolateUtils.BoxShadowsInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}