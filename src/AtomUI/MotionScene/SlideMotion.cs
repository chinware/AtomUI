using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace AtomUI.MotionScene;

public class SlideUpInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public SlideUpInMotion(MotionActor actor)
      : base(actor)
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
   
   public SlideUpOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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
   
   public SlideDownInMotion(MotionActor actor)
      : base(actor)
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
         StartValue = new ScaleTransform(1, 0.8),
         EndValue = new ScaleTransform(1, 1),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public override void NotifyPreStart()
   {
      var motionEntity = GetMotionEntity();
      _renderTransformBackup = motionEntity.RenderTransformOrigin;
      motionEntity.RenderTransformOrigin = RelativePoint.BottomRight;
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class SlideDownOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public SlideDownOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public override void NotifyPreStart()
   {
      var motionEntity = GetMotionEntity();
      _renderTransformBackup = motionEntity.RenderTransformOrigin;
      motionEntity.RenderTransformOrigin = RelativePoint.BottomRight;
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class SlideLeftInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public SlideLeftInMotion(MotionActor actor)
      : base(actor)
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
   
   public SlideLeftOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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
   
   public SlideRightInMotion(MotionActor actor)
      : base(actor)
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

   public override void NotifyPreStart()
   {
      var motionEntity = GetMotionEntity();
      _renderTransformBackup = motionEntity.RenderTransformOrigin;
      motionEntity.RenderTransformOrigin = new RelativePoint(1, 0, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class SlideRightOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public SlideRightOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public override void NotifyPreStart()
   {
      var motionEntity = GetMotionEntity();
      _renderTransformBackup = motionEntity.RenderTransformOrigin;
      motionEntity.RenderTransformOrigin = new RelativePoint(1, 0, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}