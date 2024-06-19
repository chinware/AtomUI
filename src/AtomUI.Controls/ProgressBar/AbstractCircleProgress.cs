using Avalonia;

namespace AtomUI.Controls;

public abstract class AbstractCircleProgress : AbstractProgressBar
{
   protected override SizeType CalculateEffectiveSizeType(double size)
   {
      return SizeType.Large;
   }
   
   protected override Rect GetProgressBarRect(Rect controlRect)
   {
      return new Rect();
   }

   protected override Rect GetExtraInfoRect(Rect controlRect)
   {
      return new Rect();
   }
   
   protected override void CalculateStrokeThickness()
   {
      
   }
   
   protected override void CreateCompletedIcons()
   {
      
   }
}