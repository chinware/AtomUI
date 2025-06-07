using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

internal class ZoomInMotion : AbstractMotion
{
    public ZoomInMotion(TimeSpan duration,
                        Easing? easing = null,
                        FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(0.01)
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
                Value    = BuildScaleTransform(1.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomOutMotion : AbstractMotion
{
    public ZoomOutMotion(TimeSpan duration,
                         Easing? easing = null,
                         FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(1.0)
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
                Value    = 0.1
            };
            endFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleTransform(0.01)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomBigInMotion : AbstractMotion
{
    public ZoomBigInMotion(TimeSpan duration,
                           Easing? easing = null,
                           FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(0.85)
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
                Value    = BuildScaleTransform(1.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomBigOutMotion : AbstractMotion
{
    public ZoomBigOutMotion(TimeSpan duration,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(1.0)
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
                Value    = 0.1
            };
            endFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleTransform(0.85)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomUpInMotion : AbstractMotion
{
    public ZoomUpInMotion(TimeSpan duration,
                          Easing? easing = null,
                          FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(0.01)
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
                Value    = BuildScaleTransform(1.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.5, 0.0, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomUpOutMotion : AbstractMotion
{
    public ZoomUpOutMotion(TimeSpan duration,
                           Easing? easing = null,
                           FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(1.0)
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
                Value    = 0.1
            };
            endFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleTransform(0.01)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.5, 0.0, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomLeftInMotion : AbstractMotion
{
    public ZoomLeftInMotion(TimeSpan duration,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(0.01)
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
                Value    = BuildScaleTransform(1.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.0, 0.5, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomLeftOutMotion : AbstractMotion
{
    public ZoomLeftOutMotion(TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(1.0)
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
                Value    = 0.1
            };
            endFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleTransform(0.01)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.0, 0.5, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomRightInMotion : AbstractMotion
{
    public ZoomRightInMotion(TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(0.01)
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
                Value    = BuildScaleTransform(1.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(1.0, 0.5, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomRightOutMotion : AbstractMotion
{
    public ZoomRightOutMotion(TimeSpan duration,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(1.0)
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
                Value    = 0.1
            };
            endFrame.Setters.Add(opacitySetter);

            var transformSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleTransform(0.01)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(1.0, 0.5, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomDownInMotion : AbstractMotion
{
    public ZoomDownInMotion(TimeSpan duration,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(0.01)
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
                Value    = BuildScaleTransform(1.0)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}

public class ZoomDownOutMotion : AbstractMotion
{
    public ZoomDownOutMotion(TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CircularEaseOut(), fillMode)
    {
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
                Value    = BuildScaleTransform(1.0)
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
                Value    = BuildScaleTransform(0.01)
            };
            endFrame.Setters.Add(transformSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);
        Animations.Add(animation);
    }
}