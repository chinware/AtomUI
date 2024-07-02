using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace AtomUI.MotionScene;

public class MoveDownInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public MoveDownInMotion(MotionActor actor)
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

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var motionEntity = GetMotionActor().Entity;
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new TranslateTransform(0, motionEntity.Bounds.Bottom),
         EndValue = new TranslateTransform(0, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      
      AddMotionConfig(config);
   }
}

public class MoveDownOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public MoveDownOutMotion(MotionActor actor)
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

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var motionEntity = GetMotionEntity();
      
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new TranslateTransform(0, 0),
         EndValue = new TranslateTransform(0, motionEntity.Bounds.Bottom),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class MoveLeftInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public MoveLeftInMotion(MotionActor actor)
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

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new TranslateTransform(- motionEntity.Bounds.Width, 0),
         EndValue = new TranslateTransform(0, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class MoveLeftOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public MoveLeftOutMotion(MotionActor actor)
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

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var motionEntity = GetMotionEntity();
      
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new TranslateTransform(0, 0),
         EndValue = new TranslateTransform( - motionEntity.Bounds.Width, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      
      AddMotionConfig(config);
   }
}

public class MoveRightInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public MoveRightInMotion(MotionActor actor)
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

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new TranslateTransform(motionEntity.Bounds.Right, 0),
         EndValue = new TranslateTransform(0, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class MoveRightOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? RenderTransformConfig => GetMotionConfig(MotionRenderTransformProperty);
   
   public MoveRightOutMotion(MotionActor actor)
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

   public void ConfigureTransform(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionRenderTransformProperty)
      {
         TransitionKind = TransitionKind.TransformOperations,
         StartValue = new TranslateTransform(0, 0),
         EndValue = new TranslateTransform(motionEntity.Bounds.Right, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}
