using Avalonia;
using Avalonia.Animation.Easings;

namespace AtomUI.MotionScene;

public class MoveDownInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? OffsetConfig => GetMotionConfig(MotionRenderOffsetProperty);
   
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

   public void ConfigureOffset(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var target = GetMotionActor()?.Entity!;
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(target.Bounds.X, target.Bounds.Bottom),
         EndValue = new Point(target.Bounds.X, 0),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class MoveDownOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? OffsetConfig => GetMotionConfig(MotionRenderOffsetProperty);
   
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

   public void ConfigureOffset(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(motionEntity.Bounds.X, 0),
         EndValue = new Point(motionEntity.Bounds.X, motionEntity.Bounds.Bottom),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class MoveLeftInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? OffsetConfig => GetMotionConfig(MotionRenderOffsetProperty);
   
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

   public void ConfigureOffset(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(motionEntity.Bounds.X - motionEntity.Bounds.Width, motionEntity.Bounds.Y),
         EndValue = new Point(motionEntity.Bounds.X, motionEntity.Bounds.Y),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class MoveLeftOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? OffsetConfig => GetMotionConfig(MotionRenderOffsetProperty);
   
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

   public void ConfigureOffset(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(motionEntity.Bounds.X, motionEntity.Bounds.Y),
         EndValue = new Point(motionEntity.Bounds.X - motionEntity.Bounds.Width, motionEntity.Bounds.Y),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class MoveRightInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? OffsetConfig => GetMotionConfig(MotionRenderOffsetProperty);
   
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

   public void ConfigureOffset(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(motionEntity.Bounds.Right, motionEntity.Bounds.Y),
         EndValue = new Point(motionEntity.Bounds.X, motionEntity.Bounds.Y),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class MoveRightOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? OffsetConfig => GetMotionConfig(MotionRenderOffsetProperty);
   
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

   public void ConfigureOffset(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CircularEaseInOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(motionEntity.Bounds.X, motionEntity.Bounds.Y),
         EndValue = new Point(motionEntity.Bounds.Right, motionEntity.Bounds.Y),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}
