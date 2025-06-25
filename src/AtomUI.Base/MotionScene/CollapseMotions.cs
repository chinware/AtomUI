using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.MotionScene;

internal class CollapseMotion : AbstractMotion
{
    public Direction Direction { get; }

    public CollapseMotion(Direction direction,
                          TimeSpan? duration = null,
                          Easing? easing = null,
                          FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
    {
        Direction = direction;
    }

    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        if (Direction == Direction.Left)
        {
            RenderTransformOrigin = new RelativePoint(1, 0.5, RelativeUnit.Relative);
        }
        else if (Direction == Direction.Right)
        {
            RenderTransformOrigin = new RelativePoint(0, 0.5, RelativeUnit.Relative);
        }
        else if (Direction == Direction.Top)
        {
            RenderTransformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
        }
        else
        {
            RenderTransformOrigin = new RelativePoint(0.5, 0.0, RelativeUnit.Relative);
        }
    }

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        var isHorizontal = Direction == Direction.Left || Direction == Direction.Right;
        actor.Opacity         = 1.0;
        if (isHorizontal)
        {
            actor.MotionTransform = BuildScaleXTransform(1.0);
        }
        else
        {
            actor.MotionTransform = BuildScaleYTransform(1.0);
        }
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        var isHorizontal = Direction == Direction.Left || Direction == Direction.Right;
        actor.Opacity         = 0.0;
        if (isHorizontal)
        {
            actor.MotionTransform = BuildScaleXTransform(0.01);
        }
        else
        {
            actor.MotionTransform = BuildScaleYTransform(0.01);
        }
    }
}

internal class ExpandMotion : AbstractMotion
{
    public Direction Direction { get; }

    public ExpandMotion(Direction direction,
                        TimeSpan? duration = null,
                        Easing? easing = null,
                        FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
    {
        Direction = direction;
    }
    
    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        if (Direction == Direction.Left)
        {
            RenderTransformOrigin = new RelativePoint(1.0, 0.5, RelativeUnit.Relative);
        }
        else if (Direction == Direction.Right)
        {
            RenderTransformOrigin = new RelativePoint(0.0, 0.5, RelativeUnit.Relative);
        }
        else if (Direction == Direction.Top)
        {
            RenderTransformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
        }
        else
        {
            RenderTransformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
        }

    }

    protected override void ConfigureMotionStartValue(MotionActorControl actor)
    {
        var isHorizontal = Direction == Direction.Left || Direction == Direction.Right;
        actor.Opacity         = 0.0;
        if (isHorizontal)
        {
            actor.MotionTransform = BuildScaleXTransform(0.01);
        }
        else
        {
            actor.MotionTransform = BuildScaleYTransform(0.01);
        }
    }

    protected override void ConfigureMotionEndValue(MotionActorControl actor)
    {
        var isHorizontal = Direction == Direction.Left || Direction == Direction.Right;
        actor.Opacity         = 1.0;
        if (isHorizontal)
        {
            actor.MotionTransform = BuildScaleXTransform(1.0);
        }
        else
        {
            actor.MotionTransform = BuildScaleYTransform(1.0);
        }
    }
}