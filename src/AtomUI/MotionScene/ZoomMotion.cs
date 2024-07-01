using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace AtomUI.MotionScene;

public class ZoomInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public ZoomInMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(0.2, 0.2),
         EndValue = new ScaleTransform(1, 1),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class ZoomOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public ZoomOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(0.2, 0.2),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class ZoomBigInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public ZoomBigInMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(0.8, 0.8),
         EndValue = new ScaleTransform(1, 1),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class ZoomBigOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public ZoomBigOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(0.8, 0.8),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class ZoomUpInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public ZoomUpInMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(0.8, 0.8),
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
      motionEntity.RenderTransformOrigin = new RelativePoint(0.5, 0, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class ZoomUpOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public ZoomUpOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(0.8, 0.8),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public override void NotifyPreStart()
   {
      var motionEntity = GetMotionEntity();
      _renderTransformBackup = motionEntity.RenderTransformOrigin;
      motionEntity.RenderTransformOrigin = new RelativePoint(0.5, 0, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class ZoomLeftInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public ZoomLeftInMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(0.8, 0.8),
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
      motionEntity.RenderTransformOrigin = new RelativePoint(0, 0.5, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class ZoomLeftOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public ZoomLeftOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(0.8, 0.8),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public override void NotifyPreStart()
   {
      var motionEntity = GetMotionEntity();
      _renderTransformBackup = motionEntity.RenderTransformOrigin;
      motionEntity.RenderTransformOrigin = new RelativePoint(0, 0.5, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class ZoomRightInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public ZoomRightInMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(0.8, 0.8),
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
      motionEntity.RenderTransformOrigin = new RelativePoint(1, 0.5, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class ZoomRightOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public ZoomRightOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(0.8, 0.8),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public override void NotifyPreStart()
   {
      var motionEntity = GetMotionEntity();
      _renderTransformBackup = motionEntity.RenderTransformOrigin;
      motionEntity.RenderTransformOrigin = new RelativePoint(1, 0.5, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class ZoomDownInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public ZoomDownInMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(0.8, 0.8),
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
      motionEntity.RenderTransformOrigin = new RelativePoint(0.5, 1, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}

public class ZoomDownOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   private RelativePoint _renderTransformBackup;
   
   public ZoomDownOutMotion(MotionActor actor)
      : base(actor)
   {}
   
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

   public void ConfigureRenderTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new ScaleTransform(1, 1),
         EndValue = new ScaleTransform(0.8, 0.8),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public override void NotifyPreStart()
   {
      var motionEntity = GetMotionEntity();
      _renderTransformBackup = motionEntity.RenderTransformOrigin;
      motionEntity.RenderTransformOrigin = new RelativePoint(0.5, 1, RelativeUnit.Relative);
   }

   public override void NotifyStopped()
   {
      var motionEntity = GetMotionEntity();
      motionEntity.RenderTransformOrigin = _renderTransformBackup;
   }
}
