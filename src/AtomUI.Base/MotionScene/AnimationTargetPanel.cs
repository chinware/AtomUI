using Avalonia;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

internal class AnimationTargetPanel : Panel
{
   public bool InAnimation { get; set; }

   private Size _cacheMeasureSize = default;

   protected override Size MeasureOverride(Size availableSize)
   {
      if (InAnimation && _cacheMeasureSize != default) {
         return _cacheMeasureSize;
      }
      _cacheMeasureSize = base.MeasureOverride(availableSize);
      return _cacheMeasureSize;
   }
}