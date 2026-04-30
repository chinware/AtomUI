using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

using AvaloniaSeparator = Avalonia.Controls.Separator;

public class MenuSeparator : AvaloniaSeparator
{
    #region 公共属性定义
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<MenuSeparator, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<double> LineWidthProperty =
        AvaloniaProperty.Register<MenuSeparator, double>(nameof(LineWidth), 1);

    public double LineWidth
    {
        get => GetValue(LineWidthProperty);
        set => SetValue(LineWidthProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    #endregion

    static MenuSeparator()
    {
        AffectsRender<MenuSeparator>(LineWidthProperty);
    }

    public MenuSeparator()
    {
        this.RegisterTokenResourceScope(MenuToken.ScopeProvider);
    }

    public override void Render(DrawingContext context)
    {
        var renderScaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0d;
        var linePen       = new Pen((IBrush?)BorderBrush, LineWidth / renderScaling);
        var offsetY       = Bounds.Height / 2.0;
        context.DrawLine(linePen, new Point(0, offsetY), new Point(Bounds.Right, offsetY));
    }
}
