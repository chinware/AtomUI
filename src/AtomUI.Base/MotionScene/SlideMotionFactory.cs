using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

internal static partial class MotionFactory
{
    public static MotionConfig BuildSlideUpInMotion(TimeSpan duration, Easing? easing = null,
                                                     FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseOut();
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
            var scaleYSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleYTransform(0.01) // // 不知道为啥设置成 0.0, 子元素渲染不正常
            };
            startFrame.Setters.Add(scaleYSetter);
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
            var scaleYSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleYTransform(1.0)
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildSlideUpOutMotion(TimeSpan duration, Easing? easing = null,
                                                      FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseIn();
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

            var scaleYSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleYTransform(1.0)
            };
            startFrame.Setters.Add(scaleYSetter);
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
            var scaleYSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleYTransform(0.0)
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildSlideDownInMotion(TimeSpan duration, Easing? easing = null,
                                                       FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseOut();
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

            var scaleYSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleYTransform(0.01)
            };
            startFrame.Setters.Add(scaleYSetter);
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
                Value    = 0.2
            };
            endFrame.Setters.Add(opacitySetter);
            var scaleYSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleYTransform(1.0)
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildSlideDownOutMotion(TimeSpan duration, Easing? easing = null,
                                                        FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseIn();
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

            var scaleYSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleYTransform(1.0)
            };
            startFrame.Setters.Add(scaleYSetter);
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
            var scaleYSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleYTransform(0.0)
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildSlideLeftInMotion(TimeSpan duration, Easing? easing = null,
                                                       FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseOut();
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

            var scaleXSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleXTransform(0.01)
            };
            startFrame.Setters.Add(scaleXSetter);
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
            var scaleXSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleXTransform(1.0)
            };
            endFrame.Setters.Add(scaleXSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildSlideLeftOutMotion(TimeSpan duration, Easing? easing = null,
                                                       FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseIn();
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

            var scaleXSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleXTransform(1.0)
            };
            startFrame.Setters.Add(scaleXSetter);
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
            var scaleXSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleXTransform(0.0)
            };
            endFrame.Setters.Add(scaleXSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildSlideRightInMotion(TimeSpan duration, Easing? easing = null,
                                                        FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseOut();
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

            var scaleXSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleXTransform(0.01)
            };
            startFrame.Setters.Add(scaleXSetter);
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
            var scaleXSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleXTransform(1.0)
            };
            endFrame.Setters.Add(scaleXSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(1.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildSlideRightOutMotion(TimeSpan duration, Easing? easing = null,
                                                         FillMode fillMode = FillMode.None)
    {
        easing ??= new CubicEaseIn();
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

            var scaleXSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleXTransform(1.0)
            };
            startFrame.Setters.Add(scaleXSetter);
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
            var scaleXSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleXTransform(0.0)
            };
            endFrame.Setters.Add(scaleXSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(1.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }
}