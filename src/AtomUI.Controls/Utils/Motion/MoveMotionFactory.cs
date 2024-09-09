using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls.Utils;

public static partial class MotionFactory
{
    public static MotionConfig BuildMoveDownInMotion(double offset, TimeSpan duration, Easing? easing = null,
        FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseOut();
        var           animations      = new List<IAnimation>();
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

            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = offset
            };
            startFrame.Setters.Add(translateYSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 0.0
            };
            startFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.6)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.1
            };
            middleFrame.Setters.Add(opacitySetter);
            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = offset / 4
            };
            middleFrame.Setters.Add(translateYSetter);
            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(scaleYSetter);
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
            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = 0.0
            };
            endFrame.Setters.Add(translateYSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildMoveDownOutMotion(double offset, TimeSpan duration, Easing? easing = null,
        FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseIn();
        var           animations      = new List<IAnimation>();
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

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            startFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.35)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.0
            };
            middleFrame.Setters.Add(opacitySetter);
            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = offset / 2
            };
            middleFrame.Setters.Add(translateYSetter);
            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(scaleYSetter);
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
            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = offset
            };
            endFrame.Setters.Add(translateYSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 0.0
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildMoveUpInMotion(double offset, TimeSpan duration, Easing? easing = null,
        FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseOut();
        var           animations      = new List<IAnimation>();
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

            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = -offset
            };
            startFrame.Setters.Add(translateYSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 0.0
            };
            startFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.6)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.1
            };
            middleFrame.Setters.Add(opacitySetter);
            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = -offset / 4
            };
            middleFrame.Setters.Add(translateYSetter);
            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(scaleYSetter);
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
            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = 0.0
            };
            endFrame.Setters.Add(translateYSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildMoveUpOutMotion(double offset, TimeSpan duration, Easing? easing = null,
        FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseIn();
        var           animations      = new List<IAnimation>();
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

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            startFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.35)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.0
            };
            middleFrame.Setters.Add(opacitySetter);
            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = -offset / 2
            };
            middleFrame.Setters.Add(translateYSetter);
            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(scaleYSetter);
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
            var translateYSetter = new Setter
            {
                Property = TranslateTransform.YProperty,
                Value    = -offset
            };
            endFrame.Setters.Add(translateYSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 0.0
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildMoveLeftInMotion(double offset, TimeSpan duration, Easing? easing = null,
        FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseOut();
        var           animations      = new List<IAnimation>();
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

            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = -offset
            };
            startFrame.Setters.Add(translateXSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 0.0
            };
            startFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.7)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.0
            };
            middleFrame.Setters.Add(opacitySetter);
            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = -offset
            };
            middleFrame.Setters.Add(translateXSetter);
            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(scaleYSetter);
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
            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = 0.0
            };
            endFrame.Setters.Add(translateXSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildMoveLeftOutMotion(double offset, TimeSpan duration, Easing? easing = null,
        FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseIn();
        var           animations      = new List<IAnimation>();
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

            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = 0.0
            };
            startFrame.Setters.Add(translateXSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            startFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.75)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(opacitySetter);
            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = -offset
            };
            middleFrame.Setters.Add(translateXSetter);
            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(scaleYSetter);
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
            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = -offset
            };
            endFrame.Setters.Add(translateXSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 0.0
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildMoveRightInMotion(double offset, TimeSpan duration, Easing? easing = null,
        FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseOut();
        var           animations      = new List<IAnimation>();
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

            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = offset
            };
            startFrame.Setters.Add(translateXSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 0.0
            };
            startFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.7)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.0
            };
            middleFrame.Setters.Add(opacitySetter);
            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = offset
            };
            middleFrame.Setters.Add(translateXSetter);
            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(scaleYSetter);
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
            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = 0.0
            };
            endFrame.Setters.Add(translateXSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }

    public static MotionConfig BuildMoveRightOutMotion(double offset, TimeSpan duration, Easing? easing = null,
        FillMode fillMode = FillMode.None)
    {
        easing ??= new QuinticEaseIn();
        var           animations      = new List<IAnimation>();
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

            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = 0.0
            };
            startFrame.Setters.Add(translateXSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            startFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.75)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(opacitySetter);
            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = offset
            };
            middleFrame.Setters.Add(translateXSetter);
            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 1.0
            };
            middleFrame.Setters.Add(scaleYSetter);
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
            var translateXSetter = new Setter
            {
                Property = TranslateTransform.XProperty,
                Value    = offset
            };
            endFrame.Setters.Add(translateXSetter);

            var scaleYSetter = new Setter
            {
                Property = ScaleTransform.ScaleYProperty,
                Value    = 0.0
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        animations.Add(animation);
        return new MotionConfig(transformOrigin, animations);
    }
}