namespace AtomUI.Animations;

internal class NotifiableDoubleTransition : AbstractNotifiableTransition<double>
{
    protected override double Interpolate(double progress, double from, double to)
    {
        var result = InterpolateUtils.DoubleInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}