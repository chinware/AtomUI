using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

using AvaloniaSeparator = Avalonia.Controls.Separator;

public class MenuSeparator : AvaloniaSeparator, IControlSharedTokenResourcesHost
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
    
    #region 内部属性定义
    
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => MenuToken.ID;
    
    #endregion

    static MenuSeparator()
    {
        AffectsRender<MenuSeparator>(LineWidthProperty);
    }
    
    public MenuSeparator()
    {
        this.RegisterResources();
    }

    public override void Render(DrawingContext context)
    {
        var renderScaling = VisualRoot?.RenderScaling ?? 1.0d;
        var linePen       = new Pen(BorderBrush, LineWidth / renderScaling);
        var offsetY       = Bounds.Height / 2.0;
        context.DrawLine(linePen, new Point(0, offsetY), new Point(Bounds.Right, offsetY));
    }
}