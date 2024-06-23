using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.Media;

public class MoveDownInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? OffsetConfig => GetMotionConfig(MotionRenderOffsetProperty);
   
   public MoveDownInMotion(Control target)
      : base(target)
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
      var target = GetControlTarget()!;
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
   
   public MoveDownOutMotion(Control target)
      : base(target)
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
      var target = GetControlTarget()!;
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(target.Bounds.X, 0),
         EndValue = new Point(target.Bounds.X, target.Bounds.Bottom),
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
   
   public MoveLeftInMotion(Control target)
      : base(target)
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
      var target = GetControlTarget()!;
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(target.Bounds.X - target.Bounds.Width, target.Bounds.Y),
         EndValue = new Point(target.Bounds.X, target.Bounds.Y),
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
   
   public MoveLeftOutMotion(Control target)
      : base(target)
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
      var target = GetControlTarget()!;
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(target.Bounds.X, target.Bounds.Y),
         EndValue = new Point(target.Bounds.X - target.Bounds.Width, target.Bounds.Y),
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
   
   public MoveRightInMotion(Control target)
      : base(target)
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
      var target = GetControlTarget()!;
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(target.Bounds.Right, target.Bounds.Y),
         EndValue = new Point(target.Bounds.X, target.Bounds.Y),
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
   
   public MoveRightOutMotion(Control target)
      : base(target)
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
      var target = GetControlTarget()!;
      var config = new MotionConfig(MotionRenderOffsetProperty)
      {
         TransitionKind = TransitionKind.Point,
         StartValue = new Point(target.Bounds.X, target.Bounds.Y),
         EndValue = new Point(target.Bounds.Right, target.Bounds.Y),
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}
