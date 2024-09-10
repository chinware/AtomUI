using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.Controls.Badge;

internal class CountBadgeZoomBadgeIn : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public CountBadgeZoomBadgeIn()
    {
        MotionRenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
    }

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new ExponentialEaseOut();
        var config = new MotionConfig(MotionOpacityProperty)
        {
            TransitionKind = TransitionKind.Double,
            StartValue     = 0d,
            EndValue       = 1d,
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new BackEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(0),
            EndValue       = BuildScaleTransform(1),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    internal override void NotifyConfigMotionTarget(Control motionTarget)
    {
        base.NotifyConfigMotionTarget(motionTarget);
        motionTarget.RenderTransformOrigin = MotionRenderTransformOrigin;
    }
}

internal class CountBadgeZoomBadgeOut : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public CountBadgeZoomBadgeOut()
    {
        MotionRenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
    }

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new ExponentialEaseIn();
        var config = new MotionConfig(MotionOpacityProperty)
        {
            TransitionKind = TransitionKind.Double,
            StartValue     = 1d,
            EndValue       = 0d,
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new ExponentialEaseIn();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(1),
            EndValue       = BuildScaleTransform(0),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    internal override void NotifyConfigMotionTarget(Control motionTarget)
    {
        base.NotifyConfigMotionTarget(motionTarget);
        motionTarget.RenderTransformOrigin = MotionRenderTransformOrigin;
    }
}

internal class CountBadgeNoWrapperZoomBadgeIn : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuarticEaseOut();
        var config = new MotionConfig(MotionOpacityProperty)
        {
            TransitionKind = TransitionKind.Double,
            StartValue     = 0d,
            EndValue       = 1d,
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuarticEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(0),
            EndValue       = BuildScaleTransform(1),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}

internal class CountBadgeNoWrapperZoomBadgeOut : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseIn();
        var config = new MotionConfig(MotionOpacityProperty)
        {
            TransitionKind = TransitionKind.Double,
            StartValue     = 1d,
            EndValue       = 0d,
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseIn();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(1),
            EndValue       = BuildScaleTransform(0),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}