using Avalonia.Animation;
using Avalonia.Media;

namespace AtomUI.Media;

public class ColorTransition : InterpolatingTransitionBase<Color>
{
   protected override Color Interpolate(double progress, Color from, Color to)
   {
      return InterpolateUtils.ColorInterpolate(from, to, progress);
   }
}