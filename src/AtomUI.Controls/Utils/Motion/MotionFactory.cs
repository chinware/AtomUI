using Avalonia;
using Avalonia.Animation;
using Avalonia.Media.Transformation;

namespace AtomUI.Controls.Utils;

internal record MotionConfig
{
    public RelativePoint RenderTransformOrigin { get; }
    public IList<Animation> Animations { get; }

    public MotionConfig(RelativePoint renderTransformOrigin, IList<Animation> animations)
    {
        RenderTransformOrigin = renderTransformOrigin;
        Animations            = animations;
    }
}

internal static partial class MotionFactory
{
    static TransformOperations BuildScaleTransform(double scaleX, double scaleY)
    {
        var builder = new TransformOperations.Builder(1);
        builder.AppendScale(scaleX, scaleY);
        return builder.Build();
    }

    static TransformOperations BuildScaleTransform(double scale)
    {
        return BuildScaleTransform(scale, scale);
    }

    static TransformOperations BuildScaleXTransform(double scale)
    {
        return BuildScaleTransform(scale, 1.0);
    }

    static TransformOperations BuildScaleYTransform(double scale)
    {
        return BuildScaleTransform(1.0, scale);
    }

    static TransformOperations BuildTranslateTransform(double offsetX, double offsetY)
    {
        var builder = new TransformOperations.Builder(1);
        builder.AppendTranslate(offsetX, offsetY);
        return builder.Build();
    }
}