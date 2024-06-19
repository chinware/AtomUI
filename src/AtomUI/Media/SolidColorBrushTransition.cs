using AtomUI.Utils;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Media;

public class SolidColorBrushTransition : InterpolatingTransitionBase<IBrush?>
{
   protected override IBrush? Interpolate(double progress, IBrush? from, IBrush? to)
   {
      if (from is null || to is null) {
         return progress >= 0.5 ? to : from;
      }
      
      if (from is ISolidColorBrush fromBrush && to is ISolidColorBrush toBrush) {
         return new ImmutableSolidColorBrush(
            AnimationUtils.ColorInterpolate(fromBrush.Color, toBrush.Color, progress),
            DoubleInterpolate(progress, from.Opacity, to.Opacity));
      }
      // TODO 不知道这样返回是否合适
      return from;
   }

   private double DoubleInterpolate(double progress, double oldValue, double newValue)
   {
      return ((newValue - oldValue) * progress) + oldValue;
   }
}