using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public class SlideUpInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuinticEaseOut();
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
        easing ??= new QuinticEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleYTransform(0.8),
            EndValue       = BuildScaleYTransform(1.0),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}


public class SlideUpOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuinticEaseIn();
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
        easing ??= new QuinticEaseIn();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.Double,
            StartValue     = BuildScaleYTransform(1.0),
            EndValue       = BuildScaleYTransform(0.8),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}


public class SlideDownInMotion : AbstractMotion
{
    public SlideDownInMotion()
    {
        MotionRenderTransformOrigin = RelativePoint.BottomRight;
    }

    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuinticEaseOut();
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
        easing ??= new QuinticEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleYTransform(0.8),
            EndValue       = BuildScaleYTransform(1.0),
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


public class SlideDownOutMotion : AbstractMotion
{
    public SlideDownOutMotion()
    {
        MotionRenderTransformOrigin = RelativePoint.BottomRight;
    }

    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuinticEaseIn();
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
        easing ??= new QuinticEaseIn();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleYTransform(1.0),
            EndValue       = BuildScaleYTransform(0.8),
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


public class SlideLeftInMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuinticEaseOut();
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
        easing ??= new QuinticEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleXTransform(0.8),
            EndValue       = BuildScaleXTransform(1.0),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}


public class SlideLeftOutMotion : AbstractMotion
{
    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuinticEaseIn();
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
        easing ??= new QuinticEaseIn();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.Double,
            StartValue     = BuildScaleXTransform(1.0),
            EndValue       = BuildScaleXTransform(0.8),
            MotionDuration = duration,
            MotionEasing   = easing
        };
        AddMotionConfig(config);
    }
}


public class SlideRightInMotion : AbstractMotion
{
    public SlideRightInMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(1, 0, RelativeUnit.Relative);
    }

    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuinticEaseOut();
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
        easing ??= new QuinticEaseOut();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleXTransform(0.8),
            EndValue       = BuildScaleXTransform(1.0),
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


public class SlideRightOutMotion : AbstractMotion
{
    public SlideRightOutMotion()
    {
        MotionRenderTransformOrigin = new RelativePoint(1, 0, RelativeUnit.Relative);
    }

    public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
    public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);

    public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
    {
        easing ??= new QuinticEaseIn();
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
        easing ??= new QuinticEaseIn();

        var config = new MotionConfig(MotionRenderTransformProperty)
        {
            TransitionKind = TransitionKind.TransformOperations,
            StartValue     = BuildScaleXTransform(1.0),
            EndValue       = BuildScaleXTransform(0.8),
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