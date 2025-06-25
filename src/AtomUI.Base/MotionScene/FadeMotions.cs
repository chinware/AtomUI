using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

internal class FadeInMotion : AbstractMotion
{
    public FadeInMotion(TimeSpan? duration = null,
                        Easing? easing = null,
                        FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new LinearEasing(), fillMode)
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
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 1.0;
    }
}

internal class FadeOutMotion : AbstractMotion
{
    public FadeOutMotion(TimeSpan? duration = null,
                         Easing? easing = null,
                         FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new LinearEasing(), fillMode)
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
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        actor.Opacity         = 0.0;
    }
}