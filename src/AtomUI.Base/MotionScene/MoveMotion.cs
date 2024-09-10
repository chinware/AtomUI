using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public class MoveDownInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();
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

    public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            EndValue       = BuildTranslateTransform(0, 0),
            MotionDuration = duration,
            MotionEasing   = easing
        };

        AddMotionConfig(config);
    }

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionRenderTransformProperty)
        {
            config.StartValue = BuildTranslateTransform(0, -motionTarget.DesiredSize.Height);
        }
    }

    internal override Size CalculateSceneSize(Size motionTargetSize)
    {
        return motionTargetSize.WithHeight(motionTargetSize.Height * 2);
    }

    internal override Point CalculateScenePosition(Size motionTargetSize, Point motionTargetPosition)
    {
        return motionTargetPosition.WithY(motionTargetPosition.Y - motionTargetSize.Height);
    }
}

public class MoveDownOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();
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

    public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildTranslateTransform(0, 0),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionRenderTransformProperty)
        {
            config.EndValue = BuildTranslateTransform(0, -motionTarget.DesiredSize.Height);
        }
    }

    internal override Size CalculateSceneSize(Size motionTargetSize)
    {
        return motionTargetSize.WithHeight(motionTargetSize.Height * 2);
    }

    internal override Point CalculateScenePosition(Size motionTargetSize, Point motionTargetPosition)
    {
        return motionTargetPosition.WithY(motionTargetPosition.Y - motionTargetSize.Height);
    }
}

public class MoveLeftInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();
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

    public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();
        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            EndValue       = BuildTranslateTransform(0, 0),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionRenderTransformProperty)
        {
            config.StartValue = BuildTranslateTransform(-motionTarget.DesiredSize.Width, 0);
        }
    }

    internal override Size CalculateSceneSize(Size motionTargetSize)
    {
        return motionTargetSize.WithHeight(motionTargetSize.Width * 2);
    }

    internal override Point CalculateScenePosition(Size motionTargetSize, Point motionTargetPosition)
    {
        return motionTargetPosition.WithX(motionTargetPosition.X - motionTargetSize.Width);
    }
}

public class MoveLeftOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();
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

    public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();
        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildTranslateTransform(0, 0),
            MotionDuration = duration,
            MotionEasing   = easing
        };

        AddMotionConfig(config);
    }

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionRenderTransformProperty)
        {
            config.EndValue = BuildTranslateTransform(-motionTarget.DesiredSize.Width, 0);
        }
    }

    internal override Size CalculateSceneSize(Size motionTargetSize)
    {
        return motionTargetSize.WithHeight(motionTargetSize.Width * 2);
    }

    internal override Point CalculateScenePosition(Size motionTargetSize, Point motionTargetPosition)
    {
        return motionTargetPosition.WithX(motionTargetPosition.X - motionTargetSize.Width);
    }
}

public class MoveRightInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();
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

    public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();
        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            EndValue       = BuildTranslateTransform(0, 0),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionRenderTransformProperty)
        {
            config.StartValue = BuildTranslateTransform(motionTarget.DesiredSize.Width, 0);
        }
    }

    internal override Size CalculateSceneSize(Size motionTargetSize)
    {
        return motionTargetSize.WithHeight(motionTargetSize.Width * 2);
    }
}

public class MoveRightOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();
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

    public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();
        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildTranslateTransform(0, 0),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionRenderTransformProperty)
        {
            config.EndValue = BuildTranslateTransform(motionTarget.DesiredSize.Width, 0);
        }
    }

    internal override Size CalculateSceneSize(Size motionTargetSize)
    {
        return motionTargetSize.WithHeight(motionTargetSize.Width * 2);
    }
}

public class MoveUpInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();
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

    public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();
        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            EndValue       = BuildTranslateTransform(0, 0),
            MotionDuration = duration,
            MotionEasing   = easing
        };

        AddMotionConfig(config);
    }

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionRenderTransformProperty)
        {
            config.StartValue = BuildTranslateTransform(0, motionTarget.DesiredSize.Height);
        }
    }

    internal override Size CalculateSceneSize(Size motionTargetSize)
    {
        return motionTargetSize.WithHeight(motionTargetSize.Height * 2);
    }
}

public class MoveUpOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();
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

    public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildTranslateTransform(0, 0),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }

    protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
    {
        base.NotifyPreBuildTransition(config, motionTarget);
        if (config.Property == MotionRenderTransformProperty)
        {
            config.EndValue = BuildTranslateTransform(0, motionTarget.DesiredSize.Height);
        }
    }

    internal override Size CalculateSceneSize(Size motionTargetSize)
    {
        return motionTargetSize.WithHeight(motionTargetSize.Height * 2);
    }
}