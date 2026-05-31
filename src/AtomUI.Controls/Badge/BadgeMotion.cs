using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace AtomUI.Controls.Commons;

internal static class BadgeMotionTransforms
{
    internal static readonly ITransform ScaleNearZero = BuildScaleTransform(0.01);
    internal static readonly ITransform ScaleFull     = BuildScaleTransform(1.0);

    private static ITransform BuildScaleTransform(double scale)
    {
        var builder = TransformOperations.CreateBuilder(1);
        builder.AppendScale(scale, scale);
        return builder.Build();
    }
}

internal class BadgeZoomBadgeInMotion : AbstractMotion
{
    public BadgeZoomBadgeInMotion(TimeSpan duration,
                                  Easing? easing = null,
                                  FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new ExponentialEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BadgeMotionTransforms.ScaleNearZero;
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BadgeMotionTransforms.ScaleFull;
    }
}

internal class BadgeZoomBadgeOutMotion : AbstractMotion
{
    public BadgeZoomBadgeOutMotion(TimeSpan duration,
                                   Easing? easing = null,
                                   FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new ExponentialEaseIn(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BadgeMotionTransforms.ScaleFull;
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BadgeMotionTransforms.ScaleNearZero;
    }
}
