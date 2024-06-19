using AtomUI.ColorSystem;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Utils;

public static class AnimationUtils
{
   public static Animation RunAnimation<ValueType>(AvaloniaProperty targetProperty,
                                                   ValueType startValue,
                                                   ValueType endValue,
                                                   TimeSpan duration,
                                                   Easing? easing = null)
   {
      if (easing is null) {
         easing = new LinearEasing();
      }

      var animation = new Animation
      {
         Duration = duration,
         Easing = easing,
         FillMode = FillMode.Backward,
         Children =
         {
            new KeyFrame
            {
               Setters = { new Setter(targetProperty, startValue), }, KeyTime = TimeSpan.FromMilliseconds(0)
            },
            new KeyFrame
            {
               Setters = { new Setter(targetProperty, endValue), }, KeyTime = duration
            }
         }
      };
      return animation;
   }
   
   public static Color ColorInterpolate(Color fromColor, Color toColor, double progress)
   {
      double a1 = fromColor.GetAlphaF();
      double r1 = fromColor.GetRedF() * a1;
      double g1 = fromColor.GetGreenF() * a1;
      double b1 = fromColor.GetBlueF() * a1;
      
      double a2 = toColor.GetAlphaF();
      double r2 = toColor.GetRedF() * a2;
      double g2 = toColor.GetGreenF() * a2;
      double b2 = toColor.GetBlueF() * a2;

      double r = DoubleInterpolate(r1, r2, progress);
      double g = DoubleInterpolate(g1, g2, progress);
      double b = DoubleInterpolate(b1, b2, progress);
      double a = DoubleInterpolate(a1, a2, progress);
      
      // 处理接近完全透明的情况
      if (a < 1e-5) {
         // 在这种情况下，我们可以选择直接使用目标颜色的RGB分量
         r = toColor.GetRedF();
         g = toColor.GetGreenF();
         b = toColor.GetBlueF();
      } else {
         // 如果alpha不为零，反预乘alpha
         r /= a;
         g /= a;
         b /= a;
      }

      // 防止颜色分量超出范围
      r = NumberUtils.Clamp(r, 0.0f, 1.0f);
      g = NumberUtils.Clamp(g, 0.0f, 1.0f);
      b = NumberUtils.Clamp(b, 0.0f, 1.0f);
      a = NumberUtils.Clamp(a, 0.0f, 1.0f);
      
      return ColorUtils.FromRgbF(a, r, g, b);
   }
   
   public static double DoubleInterpolate(double oldValue, double newValue, double progress)
   {
      return ((newValue - oldValue) * progress) + oldValue;
   }

   public static ITransition CreateTransition<T>(AvaloniaProperty targetProperty, 
                                                 string durationResourceKey = GlobalResourceKey.MotionDurationMid, 
                                                 Easing? easing = null)
      where T : TransitionBase, new()
   {
      easing ??= new LinearEasing();
      var transition = new T()
      {
         Property = targetProperty,
         Easing = easing
      };
      var application = Application.Current;
      if (application is not null) {
         transition.Bind(TransitionBase.DurationProperty,application.GetResourceObservable(durationResourceKey));
      }
      return transition;
   }
}