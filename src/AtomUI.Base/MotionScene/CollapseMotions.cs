using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

internal class CollapseMotion : AbstractMotion
{
    public Direction Direction { get; }

    public CollapseMotion(Direction direction,
                          TimeSpan duration,
                          Easing? easing = null,
                          FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
    {
        Direction = direction;
    }

    protected override void Configure()
    {
        var isHorizontal = Direction == Direction.Left || Direction == Direction.Right;
        var animation    = CreateAnimation();

        var startFrame = new KeyFrame
        {
            Cue = new Cue(0.0)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 1.0
            };
            startFrame.Setters.Add(opacitySetter);
            if (isHorizontal)
            {
                var scaleXSetter = new Setter
                {
                    Property = MotionActorControl.MotionTransformProperty,
                    Value    = BuildScaleXTransform(1.0)
                };
                startFrame.Setters.Add(scaleXSetter);
            }
            else
            {
                var scaleYSetter = new Setter
                {
                    Property = MotionActorControl.MotionTransformProperty,
                    Value    = BuildScaleYTransform(1.0)
                };
                startFrame.Setters.Add(scaleYSetter);
            }
        }
        animation.Children.Add(startFrame);

        var endFrame = new KeyFrame
        {
            Cue = new Cue(1.0)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.0
            };
            endFrame.Setters.Add(opacitySetter);
            if (isHorizontal)
            {
                var scaleXSetter = new Setter
                {
                    Property = MotionActorControl.MotionTransformProperty,
                    Value    = BuildScaleXTransform(0.0)
                };
                endFrame.Setters.Add(scaleXSetter);
            }
            else
            {
                var scaleYSetter = new Setter
                {
                    Property = MotionActorControl.MotionTransformProperty,
                    Value    = BuildScaleYTransform(0.0)
                };
                endFrame.Setters.Add(scaleYSetter);
            }
        }
        animation.Children.Add(endFrame);

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

        Animations.Add(animation);
    }
}

internal class ExpandMotion : AbstractMotion
{
    public Direction Direction { get; }

    public ExpandMotion(Direction direction,
                        TimeSpan duration,
                        Easing? easing = null,
                        FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
    {
        Direction = direction;
    }

    protected override void Configure()
    {
        var isHorizontal = Direction == Direction.Left || Direction == Direction.Right;
        var animation    = CreateAnimation();
        var startFrame = new KeyFrame
        {
            Cue = new Cue(0.0)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.0
            };
            startFrame.Setters.Add(opacitySetter);
            if (isHorizontal)
            {
                var scaleXSetter = new Setter
                {
                    Property = MotionActorControl.MotionTransformProperty,
                    Value    = BuildScaleXTransform(0.01)
                };
                startFrame.Setters.Add(scaleXSetter);
            }
            else
            {
                var scaleYSetter = new Setter
                {
                    Property = MotionActorControl.MotionTransformProperty,
                    Value    = BuildScaleYTransform(0.01)
                };
                startFrame.Setters.Add(scaleYSetter);
            }
        }
        animation.Children.Add(startFrame);

        var endFrame = new KeyFrame
        {
            Cue = new Cue(1.0)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 1.0
            };
            endFrame.Setters.Add(opacitySetter);
            if (isHorizontal)
            {
                var scaleXSetter = new Setter
                {
                    Property = MotionActorControl.MotionTransformProperty,
                    Value    = BuildScaleXTransform(1.0)
                };
                endFrame.Setters.Add(scaleXSetter);
            }
            else
            {
                var scaleYSetter = new Setter
                {
                    Property = MotionActorControl.MotionTransformProperty,
                    Value    = BuildScaleYTransform(1.0)
                };
                endFrame.Setters.Add(scaleYSetter);
            }
        }

        animation.Children.Add(endFrame);

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

        Animations.Add(animation);
    }
}