namespace AtomUI.Animations;

internal class NotifiableFloatTransition : AbstractNotifiableTransition<float>
{
    protected override float Interpolate(double progress, float from, float to)
    {
        var result = InterpolateUtils.FloatInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}