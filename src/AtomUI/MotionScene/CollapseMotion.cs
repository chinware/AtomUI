using AtomUI.Utils;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public class CollapseMotion : AbstractMotion
{
   public CollapseMotion(MotionActor actor)
      : base(actor)
   {}

   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? HeightConfig => GetMotionConfig(MotionHeightProperty);

   public void ConfigureHeight(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionHeightProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = motionEntity.Bounds.Height,
         EndValue = 0,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
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
}

public class ExpandMotion : AbstractMotion
{
   public ExpandMotion(MotionActor actor)
      : base(actor)
   {}

   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? HeightConfig => GetMotionConfig(MotionHeightProperty);

   public void ConfigureHeight(double originHeight, TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
      var motionEntity = GetMotionEntity();
      var config = new MotionConfig(MotionHeightProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = 0,
         EndValue = motionEntity.Bounds.Height,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureOpacity(double originOpacity, TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
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
}