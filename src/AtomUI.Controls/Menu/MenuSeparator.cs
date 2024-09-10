using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaSeparator = Avalonia.Controls.Separator;

public class MenuSeparator : AvaloniaSeparator
{
    public static readonly StyledProperty<double> LineWidthProperty =
        AvaloniaProperty.Register<MenuSeparator, double>(nameof(LineWidth), 1);

    private bool _initialized;

    public double LineWidth
    {
        get => GetValue(LineWidthProperty);
        set => SetValue(LineWidthProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (!_initialized)
        {
            TokenResourceBinder.CreateTokenBinding(this, LineWidthProperty, GlobalTokenResourceKey.LineWidth,
                BindingPriority.Template,
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