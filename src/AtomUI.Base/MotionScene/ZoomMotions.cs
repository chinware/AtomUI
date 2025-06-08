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

public class ZoomOutMotion : AbstractMotion
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
}

public class ZoomBigInMotion : AbstractMotion
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
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);
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

public class ZoomBigOutMotion : AbstractMotion
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
        actor.MotionTransform = BuildScaleTransform(0.85);
    }
}

public class ZoomUpInMotion : AbstractMotion
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

public class ZoomUpOutMotion : AbstractMotion
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
}

public class ZoomLeftInMotion : AbstractMotion
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

public class ZoomLeftOutMotion : AbstractMotion
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
}

public class ZoomRightInMotion : AbstractMotion
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

public class ZoomRightOutMotion : AbstractMotion
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
}

public class ZoomDownInMotion : AbstractMotion
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

public class ZoomDownOutMotion : AbstractMotion
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
}