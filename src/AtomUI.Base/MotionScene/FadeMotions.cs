using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace AtomUI.MotionScene;

public class FadeInMotion : AbstractMotion
{
    public FadeInMotion(TimeSpan duration,
                        Easing? easing = null,
                        FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new LinearEasing(), fillMode)
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
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}

public class FadeOutMotion : AbstractMotion
{
    public FadeOutMotion(TimeSpan duration,
                         Easing? easing = null,
                         FillMode fillMode = FillMode.Forward)
        : base(duration, easing ?? new LinearEasing(), fillMode)
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
        }
        animation.Children.Add(endFrame);
        RenderTransformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

        Animations.Add(animation);
    }
}