namespace AtomUI.Animations;

internal class NotifiableBoolTransition : AbstractNotifiableTransition<bool>
{
    protected override bool Interpolate(double progress, bool from, bool to)
    {
        var result = InterpolateUtils.BoolInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}