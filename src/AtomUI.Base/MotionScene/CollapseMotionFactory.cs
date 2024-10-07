using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

internal static partial class MotionFactory
{
    public static MotionConfig BuildCollapseMotion(Direction direction, TimeSpan duration, Easing? easing = null,
                                                   FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseOut();
        var           animations      = new List<Animation>();
        RelativePoint transformOrigin = default;
        var           isHorizontal    = direction == Direction.Left || direction == Direction.Right;
        var animation = new Animation
        {
            Duration = duration,
            Easing   = easing,
            FillMode = fillMode
        };

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

        if (direction == Direction.Left)
        {
            transformOrigin = new RelativePoint(1, 0.5, RelativeUnit.Relative);
        }
        else if (direction == Direction.Right)
        {
            transformOrigin = new RelativePoint(0, 0.5, RelativeUnit.Relative);
        }
        else if (direction == Direction.Top)
        {
            transformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
        }
        else
        {
            transformOrigin = new RelativePoint(0.5, 0.0, RelativeUnit.Relative);
        }

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildExpandMotion(Direction direction, TimeSpan duration, Easing? easing = null,
                                                 FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseIn();
        var           animations      = new List<Animation>();
        RelativePoint transformOrigin = default;
        var           isHorizontal    = direction == Direction.Left || direction == Direction.Right;
        var animation = new Animation
        {
            Duration = duration,
            Easing   = easing,
            FillMode = fillMode
        };

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

        if (direction == Direction.Left)
        {
            transformOrigin = new RelativePoint(1.0, 0.5, RelativeUnit.Relative);
        }
        else if (direction == Direction.Right)
        {
            transformOrigin = new RelativePoint(0.0, 0.5, RelativeUnit.Relative);
        }
        else if (direction == Direction.Top)
        {
            transformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
        }
        else
        {
            transformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
        }

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }
}