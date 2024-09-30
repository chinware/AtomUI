using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

internal static partial class MotionFactory
{
    public static MotionConfigX BuildMoveDownInMotion(double offset, 
                                                     TimeSpan duration, 
                                                     Easing? easing = null,
                                                     FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseOut();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, 0.0, offset)
            };
            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.8)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.1
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, offset / 4)
            };
            middleFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(middleFrame);

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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfigX(transformOrigin, animations);
    }

    public static MotionConfigX BuildMoveDownOutMotion(double offset, 
                                                      TimeSpan duration, 
                                                      Easing? easing = null,
                                                      FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseIn();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };

            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.8)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.1
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, offset / 4)
            };
            middleFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(middleFrame);

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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, 0.0, offset)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfigX(transformOrigin, animations);
    }

    public static MotionConfigX BuildMoveUpInMotion(double offset, TimeSpan duration, Easing? easing = null,
                                                   FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseOut();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, 0.0, -offset)
            };
            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.8)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.1
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, -offset / 4)
            };
            middleFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(middleFrame);

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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfigX(transformOrigin, animations);
    }

    public static MotionConfigX BuildMoveUpOutMotion(double offset, TimeSpan duration, Easing? easing = null,
                                                    FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseIn();
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

            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = 0.0
            };
            startFrame.Setters.Add(translateYSetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };

            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.8)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.1
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, -offset / 4)
            };

            middleFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(middleFrame);

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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, 0.0, -offset)
            };

            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfigX(transformOrigin, animations);
    }

    public static MotionConfigX BuildMoveLeftInMotion(double offset, TimeSpan duration, Easing? easing = null,
                                                     FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseOut();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, -offset, 0.0)
            };

            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.8)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.1
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, -offset / 2, 0.0)
            };

            middleFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(middleFrame);

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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };

            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfigX(transformOrigin, animations);
    }

    public static MotionConfigX BuildMoveLeftOutMotion(double offset, TimeSpan duration, Easing? easing = null,
                                                      FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseIn();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };
            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.8)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.1
            };
            middleFrame.Setters.Add(opacitySetter);
            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, -offset / 2, 0.0)
            };
            middleFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(middleFrame);

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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, -offset, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfigX(transformOrigin, animations);
    }

    public static MotionConfigX BuildMoveRightInMotion(double offset, TimeSpan duration, Easing? easing = null,
                                                      FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseOut();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, offset, 0.0)
            };
            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.8)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.1
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, offset / 2, 0.0)
            };
            middleFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(middleFrame);

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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfigX(transformOrigin, animations);
    }

    public static MotionConfigX BuildMoveRightOutMotion(double offset, TimeSpan duration, Easing? easing = null,
                                                       FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseIn();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };
            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.8)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, offset / 2, 0.0)
            };
            middleFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(middleFrame);

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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, offset, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfigX(transformOrigin, animations);
    }
}