using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaSeparator = Avalonia.Controls.Separator;

public class MenuSeparator : AvaloniaSeparator,
                             IControlSharedTokenResourcesHost,
                             IResourceBindingManager
{
    #region 公共属性定义
    public static readonly StyledProperty<double> LineWidthProperty =
        AvaloniaProperty.Register<MenuSeparator, double>(nameof(LineWidth), 1);

    public double LineWidth
    {
        get => GetValue(LineWidthProperty);
        set => SetValue(LineWidthProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义
    
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => MenuToken.ID;
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }
    
    #endregion
    
    private CompositeDisposable? _resourceBindingsDisposable;

    public MenuSeparator()
    {
        this.RegisterResources();
    }

    public override void Render(DrawingContext context)
    {
        var linePen = new Pen(BorderBrush, LineWidth);
        var offsetY = Bounds.Height / 2.0;
        context.DrawLine(linePen, new Point(0, offsetY), new Point(Bounds.Right, offsetY));
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, LineWidthProperty, SharedTokenKey.LineWidth,
            BindingPriority.Template,
            new RenderScaleAwareDoubleConfigure(this)));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }
}