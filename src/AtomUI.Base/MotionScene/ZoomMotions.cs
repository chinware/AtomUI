using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.MotionScene;

internal class ZoomInMotion : AbstractMotion
{
    public ZoomInMotion(TimeSpan? duration = null,
                        Easing? easing = null,
                        FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.2);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomOutMotion : AbstractMotion
{
    public ZoomOutMotion(TimeSpan? duration = null,
                         Easing? easing = null,
                         FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.2);
    }
    
    protected override void NotifyCompleted(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomBigInMotion : AbstractMotion
{
    public ZoomBigInMotion(TimeSpan? duration = null,
                           Easing? easing = null,
                           FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }

    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.01;
        actor.MotionTransform = BuildScaleTransform(0.35);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomBigOutMotion : AbstractMotion
{
    public ZoomBigOutMotion(TimeSpan? duration = null,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = RelativePoint.Center;
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.01;
        actor.MotionTransform = BuildScaleTransform(0.85);
    }
    
    protected override void NotifyCompleted(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomUpInMotion : AbstractMotion
{
    public ZoomUpInMotion(TimeSpan? duration = null,
                          Easing? easing = null,
                          FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.5, 0.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomUpOutMotion : AbstractMotion
{
    public ZoomUpOutMotion(TimeSpan? duration = null,
                           Easing? easing = null,
                           FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.5, 0.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }
    
    protected override void NotifyCompleted(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomLeftInMotion : AbstractMotion
{
    public ZoomLeftInMotion(TimeSpan? duration = null,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.0, 0.5, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomLeftOutMotion : AbstractMotion
{
    public ZoomLeftOutMotion(TimeSpan? duration = null,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.0, 0.5, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }
    
    protected override void NotifyCompleted(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomRightInMotion : AbstractMotion
{
    public ZoomRightInMotion(TimeSpan? duration = null,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(1.0, 0.5, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomRightOutMotion : AbstractMotion
{
    public ZoomRightOutMotion(TimeSpan? duration = null,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(1.0, 0.5, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }
    
    protected override void NotifyCompleted(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomDownInMotion : AbstractMotion
{
    public ZoomDownInMotion(TimeSpan? duration = null,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomDownOutMotion : AbstractMotion
{
    public ZoomDownOutMotion(TimeSpan? duration = null,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }
    
    protected override void NotifyCompleted(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}