using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Media;

public static class DrawingContextExtensions
{
   public static void DrawPilledRect(this DrawingContext context, IBrush? brush, IPen? pen, Rect rect,
                                     Orientation orientation = Orientation.Horizontal)
   {
      double pillRadius;
      if (orientation == Orientation.Horizontal) {
         pillRadius = rect.Height / 2;
      } else {
         pillRadius = rect.Width / 2;
      }
      context.DrawRectangle(brush, pen, rect, pillRadius, pillRadius);
   }
}