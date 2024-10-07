using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.Controls.Badge;

internal static class BadgeMotionFactory
{
    public static MotionConfig BuildBadgeZoomBadgeInMotion(TimeSpan duration, Easing? easing = null,
                                                                FillMode fillMode = FillMode.None)
    {
        easing ??= new ExponentialEaseOut();
        var           animations      = new List<Animation>();
        RelativePoint transformOrigin = default;
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

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = MotionFactory.BuildScaleTransform(0.0)
            };
            startFrame.Setters.Add(transformSetter);
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

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = MotionFactory.BuildScaleTransform(1.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildBadgeZoomBadgeOutMotion(TimeSpan duration, Easing? easing = null,
                                                                 FillMode fillMode = FillMode.None)
    {
        easing ??= new ExponentialEaseIn();
        var           animations      = new List<Animation>();
        RelativePoint transformOrigin = default;
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

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = MotionFactory.BuildScaleTransform(1.0)
            };
            startFrame.Setters.Add(transformSetter);
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

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = MotionFactory.BuildScaleTransform(0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildCountBadgeNoWrapperZoomBadgeInMotion(TimeSpan duration, Easing? easing = null,
                                                                         FillMode fillMode = FillMode.None)
    {
        easing ??= new CircularEaseOut();
        var           animations      = new List<Animation>();
        RelativePoint transformOrigin = default;
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

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = MotionFactory.BuildScaleTransform(0.0, 0.0)
            };
            startFrame.Setters.Add(transformSetter);
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

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = MotionFactory.BuildScaleTransform(1.0, 1.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildCountBadgeNoWrapperZoomBadgeOutMotion(TimeSpan duration, Easing? easing = null,
                                                                          FillMode fillMode = FillMode.None)
    {
        easing ??= new CircularEaseIn();
        var           animations      = new List<Animation>();
        RelativePoint transformOrigin = default;
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

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = MotionFactory.BuildScaleTransform(1.0, 1.0)
            };
            startFrame.Setters.Add(transformSetter);
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

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = MotionFactory.BuildScaleTransform(0.0, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }
}