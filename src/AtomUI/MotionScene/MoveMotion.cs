using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;

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
         StartValue = 0d,
         EndValue = 1d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         EndValue = new TranslateTransform(0, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      
      AddMotionConfig(config);
   }

   protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
   {
      base.NotifyPreBuildTransition(config, motionTarget);
      if (config.Property == MotionRenderTransformProperty) {
         config.StartValue = new TranslateTransform(0, motionTarget.DesiredSize.Height);
      }
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
         StartValue = 1d,
         EndValue = 0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new TranslateTransform(0, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
   
   protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
   {
      base.NotifyPreBuildTransition(config, motionTarget);
      if (config.Property == MotionRenderTransformProperty) {
         config.EndValue = new TranslateTransform(0, motionTarget.DesiredSize.Height);
      }
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
         StartValue = 0d,
         EndValue = 1d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         EndValue = new TranslateTransform(0, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
   
   protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
   {
      base.NotifyPreBuildTransition(config, motionTarget);
      if (config.Property == MotionRenderTransformProperty) {
         config.StartValue = new TranslateTransform(-motionTarget.DesiredSize.Width, 0);
      }
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
         StartValue = 1d,
         EndValue = 0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new TranslateTransform(0, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      
      AddMotionConfig(config);
   }
   
   protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
   {
      base.NotifyPreBuildTransition(config, motionTarget);
      if (config.Property == MotionRenderTransformProperty) {
         config.EndValue = new TranslateTransform(-motionTarget.DesiredSize.Width, 0);
      }
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
         StartValue = 0d,
         EndValue = 1d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         EndValue = new TranslateTransform(0, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
   
   protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
   {
      base.NotifyPreBuildTransition(config, motionTarget);
      if (config.Property == MotionRenderTransformProperty) {
         config.StartValue = new TranslateTransform(motionTarget.DesiredSize.Width, 0);
      }
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
         StartValue = 1d,
         EndValue = 0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new TranslateTransform(0, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
   
   protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
   {
      base.NotifyPreBuildTransition(config, motionTarget);
      if (config.Property == MotionRenderTransformProperty) {
         config.EndValue = new TranslateTransform(motionTarget.DesiredSize.Width, 0);
      }
   }
}
