using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

public class SlideUpInMotion : AbstractMotion
{
    public SlideUpInMotion(TimeSpan duration,
                           Easing? easing = null,
                           FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
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
                Value    = 0.0
            };
            startFrame.Setters.Add(opacitySetter);
            var scaleYSetter = new Setter
            {
                Property = MotionActorControl.MotionTransformProperty,
                Value    = BuildScaleYTransform(0.01) // 不知道为啥设置成 0.0, 子元素渲染不正常
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
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

public class SlideUpOutMotion : AbstractMotion
{
    public SlideUpOutMotion(TimeSpan duration,
                            Easing? easing = null,
                            FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
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
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

public class SlideDownInMotion : AbstractMotion
{
    public SlideDownInMotion(TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
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
        RenderTransformOrigin = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

public class SlideDownOutMotion : AbstractMotion
{
    public SlideDownOutMotion(TimeSpan duration,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
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
                Value    = BuildScaleYTransform(0.8)
            };
            endFrame.Setters.Add(scaleYSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

public class SlideLeftInMotion : AbstractMotion
{
    public SlideLeftInMotion(TimeSpan duration,
                             Easing? easing = null,
                             FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
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
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

public class SlideLeftOutMotion : AbstractMotion
{
    public SlideLeftOutMotion(TimeSpan duration,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
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
                Value    = BuildScaleXTransform(0.8)
            };
            endFrame.Setters.Add(scaleXSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

public class SlideRightInMotion : AbstractMotion
{
    public SlideRightInMotion(TimeSpan duration,
                              Easing? easing = null,
                              FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseOut(), fillMode)
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
        RenderTransformOrigin = new RelativePoint(1.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

public class SlideRightOutMotion : AbstractMotion
{
    public SlideRightOutMotion(TimeSpan duration,
                               Easing? easing = null,
                               FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new CubicEaseIn(), fillMode)
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
                Value    = BuildScaleXTransform(0.8)
            };
            endFrame.Setters.Add(scaleXSetter);
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(1.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}
