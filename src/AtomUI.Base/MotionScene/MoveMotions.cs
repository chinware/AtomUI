using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

public class MoveDownInMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveDownInMotion(double offset,
                            TimeSpan duration,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
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
                Value    = 0.1
            };
            startFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, Offset)
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

    internal override Size CalculateSceneSize(Size actorSize)
    {
        return actorSize.WithHeight(actorSize.Height * 2);
    }

    internal override Point CalculateScenePosition(Size actorSize, Point actorPosition)
    {
        return actorPosition.WithY(actorPosition.Y + actorSize.Height);
    }
}

public class MoveDownOutMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveDownOutMotion(double offset,
                             TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
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
                Value    = 0.1
            };
            endFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, Offset)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        Animations.Add(animation);
    }

    internal override Size CalculateSceneSize(Size actorSize)
    {
        return actorSize.WithHeight(actorSize.Height * 2);
    }
}

public class MoveUpInMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveUpInMotion(double offset,
                          TimeSpan duration,
                          Easing? easing = null,
                          FillMode fillMode = FillMode.Forward)
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
                Value    = 0.1
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

    internal override Size CalculateSceneSize(Size actorSize)
    {
        return actorSize.WithHeight(actorSize.Height * 2);
    }

    internal override Point CalculateScenePosition(Size actorSize, Point actorPosition)
    {
        return actorPosition.WithY(actorPosition.Y - actorSize.Height);
    }
}

public class MoveUpOutMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveUpOutMotion(double offset,
                           TimeSpan duration,
                           Easing? easing = null,
                           FillMode fillMode = FillMode.Forward)
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
                Value    = 0.1
            };
            endFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, -Offset)
            };

            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        Animations.Add(animation);
    }

    internal override Size CalculateSceneSize(Size actorSize)
    {
        return actorSize.WithHeight(actorSize.Height * 2);
    }
}

public class MoveLeftInMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveLeftInMotion(double offset,
                            TimeSpan duration,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
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
                Value    = 0.1
            };
            startFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, -Offset, 0.0)
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

    internal override Size CalculateSceneSize(Size actorSize)
    {
        return actorSize.WithWidth(actorSize.Width * 2);
    }

    internal override Point CalculateScenePosition(Size actorSize, Point actorPosition)
    {
        return actorPosition.WithX(actorPosition.X - actorSize.Width);
    }
}

public class MoveLeftOutMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveLeftOutMotion(double offset,
                             TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
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
                Value    = 0.1
            };
            endFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, -Offset, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        Animations.Add(animation);
    }

    internal override Size CalculateSceneSize(Size actorSize)
    {
        return actorSize.WithHeight(actorSize.Width * 2);
    }
}

public class MoveRightInMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveRightInMotion(double offset,
                             TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
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
                Value    = 0.1
            };
            startFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, Offset, 0.0)
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

    internal override Size CalculateSceneSize(Size actorSize)
    {
        return actorSize.WithHeight(actorSize.Width * 2);
    }

    internal override Point CalculateScenePosition(Size actorSize, Point actorPosition)
    {
        return actorPosition.WithY(actorPosition.X + actorSize.Height);
    }
}

public class MoveRightOutMotion : AbstractMotion
{
    public double Offset { get; }

    public MoveRightOutMotion(double offset,
                              TimeSpan duration,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.Forward)
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
                Value    = 0.1
            };
            endFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, Offset, 0.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

        Animations.Add(animation);
    }

    internal override Size CalculateSceneSize(Size actorSize)
    {
        return actorSize.WithHeight(actorSize.Width * 2);
    }
}