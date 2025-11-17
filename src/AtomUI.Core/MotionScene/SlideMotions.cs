using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

public class SlideUpInMotion : AbstractMotion
{
    public SlideUpInMotion(TimeSpan? duration = null,
                           Easing? easing = null,
                           FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
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
        actor.MotionTransform = BuildScaleYTransform(0.8); // 不知道为啥设置成 0.0, 子元素渲染不正常;
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleYTransform(1.0);
    }
}

public class SlideUpOutMotion : AbstractMotion
{
    public SlideUpOutMotion(TimeSpan? duration = null,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
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
        actor.MotionTransform = BuildScaleYTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleYTransform(0.8);
    }
}

public class SlideDownInMotion : AbstractMotion
{
    public SlideDownInMotion(TimeSpan? duration = null,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
    {
    }

    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleYTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleYTransform(1.0);
    }
}

public class SlideDownOutMotion : AbstractMotion
{
    public SlideDownOutMotion(TimeSpan? duration = null,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
    {
    }

    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleYTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleYTransform(0.8);
    }
}

public class SlideLeftInMotion : AbstractMotion
{
    public SlideLeftInMotion(TimeSpan? duration = null,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
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
        actor.MotionTransform = BuildScaleXTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleXTransform(1.0);
    }
}

public class SlideLeftOutMotion : AbstractMotion
{
    public SlideLeftOutMotion(TimeSpan? duration = null,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
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
        actor.MotionTransform = BuildScaleXTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleXTransform(0.8);
    }
}

public class SlideRightInMotion : AbstractMotion
{
    public SlideRightInMotion(TimeSpan? duration = null,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
    {
    }

    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(1.0, 0.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleXTransform(0.8);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleXTransform(1.0);
    }
}

public class SlideRightOutMotion : AbstractMotion
{
    public SlideRightOutMotion(TimeSpan? duration = null,
                               Easing? easing = null,
                               FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
    {
    }

    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = new RelativePoint(1.0, 0.0, RelativeUnit.Relative);
    }

    protected override void ConfigureMotionStartValue(BaseMotionActor actor)
    {
        actor.Opacity         = 1.0;
        actor.MotionTransform = BuildScaleXTransform(1.0);
    }

    protected override void ConfigureMotionEndValue(BaseMotionActor actor)
    {
        actor.Opacity         = 0.0;
        actor.MotionTransform = BuildScaleXTransform(0.8);
    }
}
