using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class ButtonSpinnerHandle : TemplatedControl
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ButtonSpinnerHandle>();
    
    public static readonly StyledProperty<Location> ButtonSpinnerLocationProperty =
        ButtonSpinner.ButtonSpinnerLocationProperty.AddOwner<ButtonSpinnerHandle>();
    
    public static readonly DirectProperty<ButtonSpinnerHandle, Thickness> SpinnerBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerHandle, Thickness>(nameof(SpinnerBorderThickness),
            o => o.SpinnerBorderThickness,
            (o, v) => o.SpinnerBorderThickness = v);
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public Location ButtonSpinnerLocation
    {
        get => GetValue(ButtonSpinnerLocationProperty);
        set => SetValue(ButtonSpinnerLocationProperty, value);
    }
    
    private Thickness _spinnerBorderThickness;

    internal Thickness SpinnerBorderThickness
    {
        get => _spinnerBorderThickness;
        set => SetAndRaise(SpinnerBorderThicknessProperty, ref _spinnerBorderThickness, value);
    }
    
    public IconButton? IncreaseButton { get; private set; }
    public IconButton? DecreaseButton { get; private set; }
    
    public event EventHandler? ButtonsCreated;
    
    private BorderRenderHelper _borderRenderHelper;
    private IDisposable? _borderThicknessDisposable;

    static ButtonSpinnerHandle()
    {
        AffectsRender<ButtonSpinnerHandle>(ButtonSpinnerLocationProperty);
    }
    
    public ButtonSpinnerHandle()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, SpinnerBorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderThicknessDisposable?.Dispose();
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        IncreaseButton          = e.NameScope.Find<IconButton>(ButtonSpinnerThemeConstants.IncreaseButtonPart);
        DecreaseButton          = e.NameScope.Find<IconButton>(ButtonSpinnerThemeConstants.DecreaseButtonPart);
        ButtonsCreated?.Invoke(this, EventArgs.Empty);
    }
    
    public override void Render(DrawingContext context)
    {
        var          lineWidth = SpinnerBorderThickness.Left;
        CornerRadius cornerRadius;
        if (ButtonSpinnerLocation == Location.Left)
        {
            cornerRadius = new CornerRadius(CornerRadius.TopLeft,
                0,
                0,
                CornerRadius.BottomLeft);
        }
        else
        {
            cornerRadius = new CornerRadius(0,
                CornerRadius.TopRight,
                CornerRadius.BottomRight,
                0);
        }
     
        {
            using var optionState = context.PushTransform(Matrix.CreateTranslation(lineWidth, lineWidth));
            _borderRenderHelper.Render(context, Bounds.Size.Deflate(new Thickness(lineWidth)),
                new Thickness(0), 
                cornerRadius, BackgroundSizing.OuterBorderEdge,
                Background, null, 
                new BoxShadows());
        }
        
        {
            var handleOffsetY = Bounds.Height / 2;
            using var optionState = context.PushRenderOptions(new RenderOptions
            {
                EdgeMode = EdgeMode.Aliased
            });
            {
                // 画竖线
                var startPoint = new Point(lineWidth / 2, lineWidth);
                var endPoint   = new Point(lineWidth / 2, Bounds.Height - lineWidth);
                context.DrawLine(new Pen(BorderBrush, lineWidth), startPoint, endPoint);
            }
            {
                // 画横线
                var startPoint = new Point(0, handleOffsetY); 
                var endPoint   = new Point(Bounds.Width - lineWidth, handleOffsetY);
                context.DrawLine(new Pen(BorderBrush, lineWidth), startPoint, endPoint);
            }
        }
    }
}