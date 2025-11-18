using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.Desktop.Controls.Badge;

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
        actor.MotionTransform = BuildScaleTransform(0.01);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
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
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.01);
    }
}
