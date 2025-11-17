using Avalonia;

namespace AtomUI.Animations;

internal class NotifiableSizeTransition : AbstractNotifiableTransition<Size>
{
    protected override Size Interpolate(double progress, Size from, Size to)
    {
        var result = InterpolateUtils.SizeInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}