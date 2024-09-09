using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.MotionScene;

public class CollapseMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? HeightConfig => GetMotionConfig(MotionHeightProperty);

    /// <summary>
    ///     收起的方向，垂直还是水平方向
    /// </summary>
    public Orientation Orientation { get; set; } = Orientation.Vertical;

    public void ConfigureHeight(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CubicEaseInOut();
        var config = new MotionConfig(Orientation == Orientation.Vertical
            ? MotionHeightProperty
            : MotionWidthProperty)
        {
            TransitionKind = TransitionKind.Double,
            EndValue       = 0d,
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CubicEaseInOut();
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

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionHeightProperty)
        {
            if (!double.IsNaN(motionTarget.Height))
                config.StartValue = Math.Ceiling(motionTarget.Height);
            else
                config.StartValue = Math.Ceiling(motionTarget.DesiredSize.Height);
        }
        else if (config.Property == MotionWidthProperty)
        {
            if (!double.IsNaN(motionTarget.Width))
                config.StartValue = Math.Ceiling(motionTarget.Width);
            else
                config.StartValue = Math.Ceiling(motionTarget.DesiredSize.Width);
        }
    }
}


public class ExpandMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? HeightConfig => GetMotionConfig(MotionHeightProperty);

    /// <summary>
    ///     展开的方向，垂直还是水平方向
    /// </summary>
    public Orientation Orientation { get; set; } = Orientation.Vertical;

    public void ConfigureHeight(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CubicEaseInOut();
        var config = new MotionConfig(Orientation == Orientation.Vertical
            ? MotionHeightProperty
            : MotionWidthProperty)
        {
            TransitionKind = TransitionKind.Double,
            StartValue     = 0,
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CubicEaseInOut();
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

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionHeightProperty)
        {
            if (!double.IsNaN(motionTarget.Height))
                config.EndValue = Math.Ceiling(motionTarget.Height);
            else
                config.EndValue = Math.Ceiling(motionTarget.DesiredSize.Height);
        }
        else if (config.Property == MotionWidthProperty)
        {
            if (!double.IsNaN(motionTarget.Width))
                config.EndValue = Math.Ceiling(motionTarget.Width);
            else
                config.EndValue = Math.Ceiling(motionTarget.DesiredSize.Width);
        }
    }
}