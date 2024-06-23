using AtomUI.Utils;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.Media;

public class FadeInMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public FadeInMotion(Control target)
      : base(target)
   {}

   public void ConfigureOpacity(double originOpacity, TimeSpan duration, Easing? easing = null)
   {
      easing ??= new LinearEasing();
      originOpacity = NumberUtils.Clamp(originOpacity, 0, 1);
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = originOpacity,
         EndValue = 1.0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}

public class FadeOutMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   
   public FadeOutMotion(Control target)
      : base(target)
   {}
   
   public void ConfigureOpacity(double originOpacity, TimeSpan duration, Easing? easing = null)
   {
      easing ??= new LinearEasing();
      originOpacity = NumberUtils.Clamp(originOpacity, 0, 1);
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = originOpacity,
         EndValue = 0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}