using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace AtomUI.Controls.Utils;

internal class MotionTransformOptionsAnimator : InterpolatingAnimator<TransformOperations>
{
    public override TransformOperations Interpolate(double progress, TransformOperations oldValue, TransformOperations newValue)
    {
        var oldTransform = EnsureOperations(oldValue);
        var newTransform = EnsureOperations(newValue);

        return TransformOperations.Interpolate(oldTransform, newTransform, progress);
    }

    internal static TransformOperations EnsureOperations(ITransform value)
    {
        return value as TransformOperations ?? TransformOperations.Identity;
    }
}