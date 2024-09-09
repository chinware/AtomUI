using Avalonia.Animation.Easings;

namespace AtomUI.MotionScene;

public class FadeInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);

    public void ConfigureOpacity(double originOpacity, TimeSpan duration, Easing? easing = null)
    {
        easing        ??= new LinearEasing();
        originOpacity =   Math.Clamp(originOpacity, 0d, 1d);
        var config = new MotionConfig(MotionOpacityProperty)
        {
            TransitionKind = TransitionKind.Double,
            StartValue     = originOpacity,
            EndValue       = 1.0d,
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}


public class FadeOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);

    public void ConfigureOpacity(double originOpacity, TimeSpan duration, Easing? easing = null)
    {
        easing        ??= new LinearEasing();
        originOpacity =   Math.Clamp(originOpacity, 0d, 1d);
        var config = new MotionConfig(MotionOpacityProperty)
        {
            TransitionKind = TransitionKind.Double,
            StartValue     = originOpacity,
            EndValue       = 0d,
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}