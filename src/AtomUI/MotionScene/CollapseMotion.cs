using AtomUI.Utils;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.Media;

public class CollapseMotion : AbstractMotion
{
   public CollapseMotion(Control target)
      : base(target)
   {}

   public MotionConfig? HeightConfig => GetMotionConfig(MotionHeightProperty);
   public MotionConfig? WidthConfig => GetMotionConfig(MotionWidthProperty);

   public void ConfigureHeight(double originHeight, TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
      var config = new MotionConfig(MotionHeightProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = originHeight,
         EndValue = 0,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureOpacity(double originOpacity, TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
      originOpacity = Math.Clamp(originOpacity, 0, 1);
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = originOpacity,
         EndValue = 0,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
}