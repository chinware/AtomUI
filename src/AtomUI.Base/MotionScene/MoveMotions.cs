using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

internal class MoveDownInMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveDownInMotion(double offset,
                            TimeSpan duration,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new QuinticEaseOut(), fillMode)
    {
        Offset = offset;
    }

    protected override void Configure()
    {
        var animation = CreateAnimation();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, 0.0, Offset)
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, Offset / 4)
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
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

internal class MoveDownOutMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveDownOutMotion(double offset,
                             TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new QuinticEaseIn(), fillMode)
    {
        Offset = offset;
    }

    protected override void Configure()
    {
        var animation = CreateAnimation();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, Offset / 4)
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, 0.0, Offset)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

internal class MoveUpInMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveUpInMotion(double offset,
                          TimeSpan duration,
                          Easing? easing = null,
                          FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new QuinticEaseOut(), fillMode)
    {
        Offset = offset;
    }

    protected override void Configure()
    {
        var animation = CreateAnimation();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, 0.0, -Offset)
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, -Offset / 4)
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
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

internal class MoveUpOutMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveUpOutMotion(double offset,
                           TimeSpan duration,
                           Easing? easing = null,
                           FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new QuinticEaseIn(), fillMode)
    {
        Offset = offset;
    }

    protected override void Configure()
    {
        var animation = CreateAnimation();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, -Offset / 4)
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, 0.0, -Offset)
            };

            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

internal class MoveLeftInMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveLeftInMotion(double offset,
                            TimeSpan duration,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new QuinticEaseOut(), fillMode)
    {
        Offset = offset;
    }

    protected override void Configure()
    {
        var animation = CreateAnimation();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, -Offset, 0.0)
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, -Offset / 2, 0.0)
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
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

internal class MoveLeftOutMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveLeftOutMotion(double offset,
                             TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new QuinticEaseIn(), fillMode)
    {
        Offset = offset;
    }

    protected override void Configure()
    {
        var animation = CreateAnimation();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, -Offset / 2, 0.0)
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, -Offset, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

internal class MoveRightInMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveRightInMotion(double offset,
                             TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new QuinticEaseOut(), fillMode)
    {
        Offset = offset;
    }

    protected override void Configure()
    {
        var animation = CreateAnimation();

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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, Offset, 0.0)
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, Offset / 2, 0.0)
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
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

internal class MoveRightOutMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveRightOutMotion(double offset,
                              TimeSpan duration,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.None)
        : base(duration, easing ?? new QuinticEaseIn(), fillMode)
    {
        Offset = offset;
    }

    protected override void Configure()
    {
        var animation = CreateAnimation();
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
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, Offset / 2, 0.0)
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
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, Offset, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}