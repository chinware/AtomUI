using Avalonia.Media;

namespace AtomUI.Animations;

internal class NotifiableTransformOperationsTransition : AbstractNotifiableTransition<ITransform>
{
    protected override ITransform Interpolate(double progress, ITransform from, ITransform to)
    {
        var result = InterpolateUtils.TransformOperationsInterpolate(progress, from, to);
        if (CheckCompletedStatus(progress))
        {
            NotifyTransitionCompleted(true);
        }
        return result;
    }
}