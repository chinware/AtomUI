using Avalonia;
using Avalonia.Animation;
using Avalonia.Media.Transformation;

namespace AtomUI.MotionScene;

internal record MotionConfigX
{
    public RelativePoint RenderTransformOrigin { get; }
    public IList<Animation> Animations { get; }

    public MotionConfigX(RelativePoint renderTransformOrigin, IList<Animation> animations)
    {
        RenderTransformOrigin = renderTransformOrigin;
        Animations            = animations;
    }
}

internal static partial class MotionFactory
{
    public static TransformOperations BuildScaleTransform(double scaleX, double scaleY)
    {
        var builder = new TransformOperations.Builder(1);
        builder.AppendScale(scaleX, scaleY);
        return builder.Build();
    }

    public static TransformOperations BuildScaleTransform(double scale)
    {
        return BuildScaleTransform(scale, scale);
    }

    public static TransformOperations BuildScaleXTransform(double scale)
    {
        return BuildScaleTransform(scale, 1.0);
    }

    public static TransformOperations BuildScaleYTransform(double scale)
    {
        return BuildScaleTransform(1.0, scale);
    }

    public static TransformOperations BuildTranslateTransform(double offsetX, double offsetY)
    {
        var builder = new TransformOperations.Builder(1);
        builder.AppendTranslate(offsetX, offsetY);
        return builder.Build();
    }

    public static TransformOperations BuildTranslateScaleAndTransform(double scaleX, double scaleY, double offsetX, double offsetY)
    {
        var builder = new TransformOperations.Builder(2);
        builder.AppendScale(scaleX, scaleY);
        builder.AppendTranslate(offsetX, offsetY);
        return builder.Build();
    }
}