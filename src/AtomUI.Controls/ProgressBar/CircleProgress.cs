using AtomUI.Media;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public class CircleProgress : AbstractCircleProgress
{
   private Rect _currentGrooveRect;
   protected override void RenderGroove(DrawingContext context)
   {
      var controlRect = new Rect(new Point(0, 0), DesiredSize); 
      _currentGrooveRect = GetProgressBarRect(controlRect).Deflate(StrokeThickness / 2);
      _currentGrooveRect = new Rect(_currentGrooveRect.Position, new Size(Math.Floor(_currentGrooveRect.Size.Width), 
                                                                          Math.Floor(_currentGrooveRect.Size.Height)));
      var pen = new Pen(GrooveBrush, StrokeThickness);
      context.DrawEllipse(null, pen, _currentGrooveRect);
   }

   protected override void RenderIndicatorBar(DrawingContext context)
   {
      var pen = new Pen(IndicatorBarBrush, StrokeThickness)
      {
         LineCap = StrokeLineCap
      };
      double startAngle = -90;
      context.DrawArc(pen, _currentGrooveRect, startAngle, IndicatorAngle);

      if (!double.IsNaN(SuccessThreshold)) {
         var successPen = new Pen(SuccessThresholdBrush, StrokeThickness)
         {
            LineCap = StrokeLineCap
         };
         context.DrawArc(successPen, _currentGrooveRect, startAngle, CalculateAngle(SuccessThreshold));
      }
   }
   
   protected override void NotifyUpdateProgress()
   {
      base.NotifyUpdateProgress();
      IndicatorAngle = CalculateAngle(Value);
   }

   private double CalculateAngle(double value)
   {
      return 360 * value / (Maximum - Minimum);
   }
}