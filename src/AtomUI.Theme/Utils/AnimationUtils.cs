using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme.Utils;

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

   public static ITransition CreateTransition<T>(AvaloniaProperty targetProperty, 
                                                 TokenResourceKey? durationResourceKey = null, 
                                                 Easing? easing = null)
      where T : TransitionBase, new()
   {
      easing ??= new LinearEasing();
      durationResourceKey ??= GlobalResourceKey.MotionDurationMid;
      var transition = new T()
      {
         Property = targetProperty,
         Easing = easing,
         
      };
      var application = Application.Current;
      if (application is not null) {
         transition.Bind(TransitionBase.DurationProperty,application.GetResourceObservable(durationResourceKey));
      }
      return transition;
   }
}