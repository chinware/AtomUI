using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

internal class ZoomInMotion : AbstractMotion
{
    public ZoomInMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.2);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomOutMotion : AbstractMotion
{
    public ZoomOutMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.2);
    }
    
    protected override void NotifyCompleted(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomBigInMotion : AbstractMotion
{
    public ZoomBigInMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.85);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomBigOutMotion : AbstractMotion
{
    public ZoomBigOutMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.85);
    }
    
    protected override void NotifyCompleted(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomUpInMotion : AbstractMotion
{
    public ZoomUpInMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomUpOutMotion : AbstractMotion
{
    public ZoomUpOutMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }
    
    protected override void NotifyCompleted(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomLeftInMotion : AbstractMotion
{
    public ZoomLeftInMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomLeftOutMotion : AbstractMotion
{
    public ZoomLeftOutMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }
    
    protected override void NotifyCompleted(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomRightInMotion : AbstractMotion
{
    public ZoomRightInMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomRightOutMotion : AbstractMotion
{
    public ZoomRightOutMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }
    
    protected override void NotifyCompleted(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}

internal class ZoomDownInMotion : AbstractMotion
{
    public ZoomDownInMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }
}

internal class ZoomDownOutMotion : AbstractMotion
{
    public ZoomDownOutMotion(TimeSpan duration,
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

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleTransform(0.8);
    }
    
    protected override void NotifyCompleted(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = null;
    }
}