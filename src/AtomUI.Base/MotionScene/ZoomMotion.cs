using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public class ZoomInMotion : AbstractMotion
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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(0.2),
            EndValue       = BuildScaleTransform(1),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}

public class ZoomOutMotion : AbstractMotion
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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(1),
            EndValue       = BuildScaleTransform(0.2),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}

public class ZoomBigInMotion : AbstractMotion
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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(0.8),
            EndValue       = BuildScaleTransform(1),
            MotionDuration = duration,
            MotionEasing   = easing
        };

        AddMotionConfig(config);
    }
}

public class ZoomBigOutMotion : AbstractMotion
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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(1),
            EndValue       = BuildScaleTransform(0.8),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}

public class ZoomUpInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public ZoomUpInMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(0.5, 0, RelativeUnit.Relative);
    }

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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(0.8),
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

public class ZoomUpOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public ZoomUpOutMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(0.5, 0, RelativeUnit.Relative);
    }

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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(1),
            EndValue       = BuildScaleTransform(0.8),
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

public class ZoomLeftInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public ZoomLeftInMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(0, 0.5, RelativeUnit.Relative);
    }

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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(0.8),
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

public class ZoomLeftOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public ZoomLeftOutMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(0, 0.5, RelativeUnit.Relative);
    }

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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(1),
            EndValue       = BuildScaleTransform(0.8),
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

public class ZoomRightInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public ZoomRightInMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(1, 0.5, RelativeUnit.Relative);
    }

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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(0.8),
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

public class ZoomRightOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public ZoomRightOutMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(1, 0.5, RelativeUnit.Relative);
    }

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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(1),
            EndValue       = BuildScaleTransform(0.8),
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

public class ZoomDownInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public ZoomDownInMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(0.5, 1, RelativeUnit.Relative);
    }

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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(0.8),
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

public class ZoomDownOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public ZoomDownOutMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(0.5, 1, RelativeUnit.Relative);
    }

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

    public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new CircularEaseInOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleTransform(1),
            EndValue       = BuildScaleTransform(0.8),
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