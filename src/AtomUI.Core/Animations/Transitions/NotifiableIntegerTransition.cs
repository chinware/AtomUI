namespace AtomUI.Animations;

internal class NotifiableIntegerTransition : AbstractNotifiableTransition<int>
{
    protected override int Interpolate(double progress, int from, int to)
    {
        var result = InterpolateUtils.IntegerInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}