using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Rendering;

namespace AtomUI.Controls;

internal class DataGridRowExpander : ToggleButton,
                                     IResourceBindingManager,
                                     ICustomHitTest
{
    
    internal static readonly DirectProperty<DataGridRowExpander, double> IndicatorThicknessProperty =
        AvaloniaProperty.RegisterDirect<DataGridRowExpander, double>(
            nameof(IndicatorThickness),
            o => o.IndicatorThickness,
            (o, v) => o.IndicatorThickness = v);
    
    internal double IndicatorThickness
    {
        get => _indicatorThickness;
        set => SetAndRaise(IndicatorThicknessProperty, ref _indicatorThickness, value);
    }
    private double _indicatorThickness;
    
    private CompositeDisposable? _resourceBindingsDisposable;
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;

    private Rectangle? _horizontalIndicator;
    private Rectangle? _verticalIndicator;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BorderThicknessProperty)
        {
            IndicatorThickness = BorderThickness.Left;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _verticalIndicator   = e.NameScope.Find<Rectangle>(DataGridRowExpanderThemeConstants.VerticalIndicatorPart);
        _horizontalIndicator = e.NameScope.Find<Rectangle>(DataGridRowExpanderThemeConstants.HorizontalIndicatorPart);
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}