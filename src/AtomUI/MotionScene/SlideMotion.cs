using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;

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
         StartValue = 0d,
         EndValue = 1d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 0.8),
         EndValue = new ScaleTransform(1, 1),
         MotionDuration = duration,
         MotionEasing = easing
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
         StartValue = 1d,
         EndValue = 0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseIn();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(1, 0.8),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class SlideDownInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;

   public SlideDownInMotion()
   {
      MotionRenderTransformOrigin = RelativePoint.BottomRight;
   }
   
   public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseOut();
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = 0d,
         EndValue = 1d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 0.8),
         EndValue = new ScaleTransform(1, 1),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public override void NotifyConfigMotionTarget(Control motionTarget)
   {
      base.NotifyConfigMotionTarget(motionTarget);
      _renderTransformBackup = motionTarget.RenderTransformOrigin;
      motionTarget.RenderTransformOrigin = MotionRenderTransformOrigin;
   }

   public override void NotifyRestoreMotionTarget(Control motionTarget)
   {
      base.NotifyRestoreMotionTarget(motionTarget);
      motionTarget.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class SlideDownOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;

   public SlideDownOutMotion()
   {
      MotionRenderTransformOrigin = RelativePoint.BottomRight;
   }
   
   public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseIn();
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = 1d,
         EndValue = 0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseIn();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(1, 0.8),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public override void NotifyConfigMotionTarget(Control motionTarget)
   {
      base.NotifyConfigMotionTarget(motionTarget);
      _renderTransformBackup = motionTarget.RenderTransformOrigin;
      motionTarget.RenderTransformOrigin = MotionRenderTransformOrigin;
   }

   public override void NotifyRestoreMotionTarget(Control motionTarget)
   {
      base.NotifyRestoreMotionTarget(motionTarget);
      motionTarget.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class SlideLeftInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public SlideLeftInMotion()
   {}
   
   public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseOut();
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = 0d,
         EndValue = 1d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(0.8, 1),
         EndValue = new ScaleTransform(1, 1),
         MotionDuration = duration,
         MotionEasing = easing
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
         StartValue = 1d,
         EndValue = 0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseIn();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(0.8, 1),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class SlideRightInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;

   public SlideRightInMotion()
   {
      MotionRenderTransformOrigin = new RelativePoint(1, 0, RelativeUnit.Relative);
   }
   
   public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseOut();
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = 0d,
         EndValue = 1d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(0.8, 1),
         EndValue = new ScaleTransform(1, 1),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
   
   public override void NotifyConfigMotionTarget(Control motionTarget)
   {
      base.NotifyConfigMotionTarget(motionTarget);
      _renderTransformBackup = motionTarget.RenderTransformOrigin;
      motionTarget.RenderTransformOrigin = MotionRenderTransformOrigin;
   }

   public override void NotifyRestoreMotionTarget(Control motionTarget)
   {
      base.NotifyRestoreMotionTarget(motionTarget);
      motionTarget.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class SlideRightOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;

   public SlideRightOutMotion()
   {
      MotionRenderTransformOrigin = new RelativePoint(1, 0, RelativeUnit.Relative);
   }
   
   public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseIn();
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = 1d,
         EndValue = 0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new QuinticEaseIn();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(0.8, 1),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public override void NotifyConfigMotionTarget(Control motionTarget)
   {
      base.NotifyConfigMotionTarget(motionTarget);
      _renderTransformBackup = motionTarget.RenderTransformOrigin;
      motionTarget.RenderTransformOrigin = MotionRenderTransformOrigin;
   }

   public override void NotifyRestoreMotionTarget(Control motionTarget)
   {
      base.NotifyRestoreMotionTarget(motionTarget);
      motionTarget.RenderTransformOrigin = _renderTransformBackup;
   }
}