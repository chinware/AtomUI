using Avalonia;

namespace AtomUI.Animations;

internal class NotifiableThicknessTransition : AbstractNotifiableTransition<Thickness>
{
    protected override Thickness Interpolate(double progress, Thickness from, Thickness to)
    {
        var result = InterpolateUtils.ThicknessInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}