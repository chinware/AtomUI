using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public class StepsProgressBar : AbstractLineProgress
{
   public static readonly StyledProperty<LinePercentAlignment> PercentPositionProperty =
      AvaloniaProperty.Register<ProgressBar, LinePercentAlignment>(nameof(PercentPosition));
   
   public LinePercentAlignment PercentPosition
   {
      get => GetValue(PercentPositionProperty);
      set => SetValue(PercentPositionProperty, value);
   }
   
   public void SetStepsStrokeColor(List<Color> colors)
   {
      
   }
   
   protected override void RenderGroove(DrawingContext context)
   {
      
   }

   protected override void RenderIndicatorBar(DrawingContext context)
   {
      
   }

   protected override void CalculateStrokeThickness() { }
   
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
}