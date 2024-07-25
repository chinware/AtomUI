using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaSeparator = Avalonia.Controls.Separator;

public class MenuSeparator : AvaloniaSeparator
{
   public override void Render(DrawingContext context)
   {
      var linePen = new Pen(BorderBrush);
      var offsetY = Bounds.Height / 2.0;
      context.DrawLine(linePen, new Point(0, offsetY), new Point(Bounds.Right, offsetY));
   }
}