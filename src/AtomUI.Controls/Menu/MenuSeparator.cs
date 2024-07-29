using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaSeparator = Avalonia.Controls.Separator;

public class MenuSeparator : AvaloniaSeparator
{
   public static readonly StyledProperty<double> LineWidthProperty =
      AvaloniaProperty.Register<MenuSeparator, double>(nameof(LineWidth), 1);

   public double LineWidth
   {
      get => GetValue(LineWidthProperty);
      set => SetValue(LineWidthProperty, value);
   }

   private bool _initialized = false;

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      if (!_initialized) {
         TokenResourceBinder.CreateTokenBinding(this, LineWidthProperty, GlobalResourceKey.LineWidth, BindingPriority.Template,
                                      new RenderScaleAwareDoubleConfigure(this));
         _initialized = true;
      }
   }

   public override void Render(DrawingContext context)
   {
      var linePen = new Pen(BorderBrush, LineWidth);
      var offsetY = Bounds.Height / 2.0;
      context.DrawLine(linePen, new Point(0, offsetY), new Point(Bounds.Right, offsetY));
   }
}