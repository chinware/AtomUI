using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class NotificationMoveDownInMotion : AbstractMotion
{
    public double Offset { get; }

    public NotificationMoveDownInMotion(double offset,
                                        TimeSpan duration,
                                        Easing? easing = null,
                                        FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new QuinticEaseOut(), fillMode)
    {
        Offset     = offset;
        SpiritType = MotionSpiritType.Animation;
    }

    protected override void ConfigureAnimation()
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
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
                Value    = 0.8
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, Offset)
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
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
    
    protected override void NotifyPreStart(BaseMotionActor actor)
    {
        base.NotifyPreStart(actor);
        actor.Opacity = 0.0;
    }
}

internal class NotificationMoveDownOutMotion : AbstractMotion
{
    public double Offset { get; }

    public NotificationMoveDownOutMotion(double offset,
                                         TimeSpan duration,
                                         Easing? easing = null,
                                         FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new QuinticEaseIn(), fillMode)
    {
        Offset     = offset;
        SpiritType = MotionSpiritType.Animation;
    }

    protected override void ConfigureAnimation()
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };

            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.2)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.2
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 0.2, 0.0, Offset / 4)
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, 0.0, Offset)
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

internal class NotificationMoveUpInMotion : AbstractMotion
{
    public double Offset { get; }

    public NotificationMoveUpInMotion(double offset,
                                      TimeSpan duration,
                                      Easing? easing = null,
                                      FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new QuinticEaseOut(), fillMode)
    {
        Offset     = offset;
        SpiritType = MotionSpiritType.Animation;
    }

    protected override void ConfigureAnimation()
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
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
                Value    = 0.8
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, -Offset)
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
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
    
    protected override void NotifyPreStart(BaseMotionActor actor)
    {
        base.NotifyPreStart(actor);
        actor.Opacity = 0.0;
    }
}

internal class NotificationMoveUpOutMotion : AbstractMotion
{
    public double Offset { get; }

    public NotificationMoveUpOutMotion(double offset,
                                       TimeSpan duration,
                                       Easing? easing = null,
                                       FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new QuinticEaseIn(), fillMode)
    {
        Offset     = offset;
        SpiritType = MotionSpiritType.Animation;
    }

    protected override void ConfigureAnimation()
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };

            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.2)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.2
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 0.2, 0.0, -Offset / 4)
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, 0.0, -Offset)
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

internal class NotificationMoveLeftInMotion : AbstractMotion
{
    public double Offset { get; }
    public bool IsTop { get; }

    public NotificationMoveLeftInMotion(double offset,
                                        TimeSpan duration,
                                        Easing? easing = null,
                                        FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new QuinticEaseOut(), fillMode)
    {
        Offset     = offset;
        SpiritType = MotionSpiritType.Animation;
    }

    protected override void ConfigureAnimation()
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, -Offset, 0.0)
            };

            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.5)
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, -Offset, 0.0)
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
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
    
    protected override void NotifyPreStart(BaseMotionActor actor)
    {
        base.NotifyPreStart(actor);
        actor.Opacity = 0.0;
    }
}

internal class NotificationMoveLeftOutMotion : AbstractMotion
{
    public double Offset { get; }

    public NotificationMoveLeftOutMotion(double offset,
                                         TimeSpan duration,
                                         Easing? easing = null,
                                         FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new QuinticEaseIn(), fillMode)
    {
        Offset     = offset;
        SpiritType = MotionSpiritType.Animation;
    }

    protected override void ConfigureAnimation()
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };
            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.5)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.6
            };
            middleFrame.Setters.Add(opacitySetter);
            var transformSetter = new Setter
            {
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, -Offset, 0.0)
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, -Offset, 0.0)
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

internal class NotificationMoveRightInMotion : AbstractMotion
{
    public double Offset { get; }

    public NotificationMoveRightInMotion(double offset,
                                         TimeSpan duration,
                                         Easing? easing = null,
                                         FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new QuinticEaseOut(), fillMode)
    {
        Offset     = offset;
        SpiritType = MotionSpiritType.Animation;
    }

    protected override void ConfigureAnimation()
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 0.01, Offset, 0.0)
            };
            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.5)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.0
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, Offset, 0.0)
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
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
    
    protected override void NotifyPreStart(BaseMotionActor actor)
    {
        base.NotifyPreStart(actor);
        actor.Opacity = 0.0;
    }
}

internal class NotificationMoveRightOutMotion : AbstractMotion
{
    public double Offset { get; }

    public NotificationMoveRightOutMotion(double offset,
                                          TimeSpan duration,
                                          Easing? easing = null,
                                          FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new QuinticEaseIn(), fillMode)
    {
        Offset     = offset;
        SpiritType = MotionSpiritType.Animation;
    }

    protected override void ConfigureAnimation()
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, 0.0, 0.0)
            };
            startFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(startFrame);

        var middleFrame = new KeyFrame
        {
            Cue = new Cue(0.5)
        };
        {
            var opacitySetter = new Setter
            {
                Property = Visual.OpacityProperty,
                Value    = 0.6
            };
            middleFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 1.0, Offset, 0.0)
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
                Property = BaseMotionActor.MotionTransformOperationsProperty,
                Value    = BuildTranslateScaleAndTransform(1.0, 0.0, Offset, 0.0)
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