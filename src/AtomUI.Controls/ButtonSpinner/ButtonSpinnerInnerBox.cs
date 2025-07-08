using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

[TemplatePart(ButtonSpinnerThemeConstants.SpinnerHandlePart, typeof(ContentPresenter))]
internal class ButtonSpinnerInnerBox : AddOnDecoratedInnerBox, 
                                       ICustomHitTest,
                                       IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> SpinnerContentProperty =
        AvaloniaProperty.Register<ButtonSpinnerInnerBox, object?>(nameof(SpinnerContent));

    public object? SpinnerContent
    {
        get => GetValue(SpinnerContentProperty);
        set => SetValue(SpinnerContentProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<ButtonSpinnerInnerBox, Location> ButtonSpinnerLocationProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, Location>(nameof(ButtonSpinnerLocation),
            o => o.ButtonSpinnerLocation,
            (o, v) => o.ButtonSpinnerLocation = v);

    internal static readonly DirectProperty<ButtonSpinnerInnerBox, bool> ShowButtonSpinnerProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, bool>(nameof(ShowButtonSpinner),
            o => o.ShowButtonSpinner,
            (o, v) => o.ShowButtonSpinner = v);

    internal static readonly DirectProperty<ButtonSpinnerInnerBox, Thickness> SpinnerBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, Thickness>(nameof(SpinnerBorderThickness),
            o => o.SpinnerBorderThickness,
            (o, v) => o.SpinnerBorderThickness = v);

    internal static readonly DirectProperty<ButtonSpinnerInnerBox, IBrush?> SpinnerBorderBrushProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, IBrush?>(nameof(SpinnerBorderBrush),
            o => o.SpinnerBorderBrush,
            (o, v) => o.SpinnerBorderBrush = v);

    internal static readonly DirectProperty<ButtonSpinnerInnerBox, double> SpinnerHandleWidthTokenProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, double>(nameof(SpinnerHandleWidthToken),
            o => o.SpinnerHandleWidthToken,
            (o, v) => o.SpinnerHandleWidthToken = v);

    private bool _showButtonSpinner;

    internal bool ShowButtonSpinner
    {
        get => _showButtonSpinner;
        set => SetAndRaise(ShowButtonSpinnerProperty, ref _showButtonSpinner, value);
    }

    private Location _buttonSpinnerLocation;

    internal Location ButtonSpinnerLocation
    {
        get => _buttonSpinnerLocation;
        set => SetAndRaise(ButtonSpinnerLocationProperty, ref _buttonSpinnerLocation, value);
    }

    private Thickness _spinnerBorderThickness;

    internal Thickness SpinnerBorderThickness
    {
        get => _spinnerBorderThickness;
        set => SetAndRaise(SpinnerBorderThicknessProperty, ref _spinnerBorderThickness, value);
    }

    private IBrush? _spinnerBorderBrush;

    internal IBrush? SpinnerBorderBrush
    {
        get => _spinnerBorderBrush;
        set => SetAndRaise(SpinnerBorderBrushProperty, ref _spinnerBorderBrush, value);
    }

    private double _spinnerHandleWidthToken;

    internal double SpinnerHandleWidthToken
    {
        get => _spinnerHandleWidthToken;
        set => SetAndRaise(SpinnerHandleWidthTokenProperty, ref _spinnerHandleWidthToken, value);
    }
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }

    #endregion
    
    private CompositeDisposable? _resourceBindingsDisposable;
    private ContentPresenter? _handleContentPresenter;

    protected override void BuildEffectiveInnerBoxPadding()
    {
        if (ShowButtonSpinner)
        {
            var padding = _spinnerHandleWidthToken + InnerBoxPadding.Right;
            if (ButtonSpinnerLocation == Location.Right)
            {
                EffectiveInnerBoxPadding = new Thickness(InnerBoxPadding.Left, InnerBoxPadding.Top, padding,
                    InnerBoxPadding.Bottom);
            }
            else
            {
                EffectiveInnerBoxPadding = new Thickness(padding, InnerBoxPadding.Top, InnerBoxPadding.Right,
                    InnerBoxPadding.Bottom);
            }
        }
        else
        {
            EffectiveInnerBoxPadding = InnerBoxPadding;
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, SpinnerBorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _handleContentPresenter = e.NameScope.Find<ContentPresenter>(ButtonSpinnerThemeConstants.SpinnerHandlePart);
    }

    public override void Render(DrawingContext context)
    {
        if (_handleContentPresenter is not null)
        {
            var handleOffset  = _handleContentPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
            var handleOffsetY = handleOffset.Y + Bounds.Height / 2;
            using var optionState = context.PushRenderOptions(new RenderOptions
            {
                EdgeMode = EdgeMode.Aliased
            });
            {
                // 画竖线
                var startPoint = new Point(handleOffset.X, 0);
                var endPoint   = new Point(handleOffset.X, Bounds.Height);
                context.DrawLine(new Pen(SpinnerBorderBrush, SpinnerBorderThickness.Left), startPoint, endPoint);
            }
            {
                // 画竖线
                var startPoint = new Point(handleOffset.X, handleOffsetY);
                var endPoint   = new Point(Bounds.Width, handleOffsetY);
                context.DrawLine(new Pen(SpinnerBorderBrush, SpinnerBorderThickness.Left), startPoint, endPoint);
            }
        }
    }
}