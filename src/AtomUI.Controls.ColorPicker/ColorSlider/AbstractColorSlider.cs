using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal abstract class AbstractColorSlider : RangeBase
{
    #region 公共属性定义

    public static readonly StyledProperty<List<GradientStop>?> GradientStopsProperty =
        AvaloniaProperty.Register<GradientColorSlider, List<GradientStop>?>(nameof(GradientStops));
    
    public static readonly StyledProperty<GradientStop?> ActivatedStopProperty =
        AvaloniaProperty.Register<GradientColorSlider, GradientStop?>(nameof(ActivatedStop));
    
    internal static readonly StyledProperty<IBrush?> TransparentBgIntervalColorProperty =
        AvaloniaProperty.Register<ColorSlider, IBrush?>(nameof(TransparentBgIntervalColor));
    
    internal static readonly StyledProperty<double> TransparentBgSizeProperty =
        AvaloniaProperty.Register<ColorSlider, double>(nameof(TransparentBgSize), 4.0);
    
    public List<GradientStop>? GradientStops
    {
        get => GetValue(GradientStopsProperty);
        set => SetValue(GradientStopsProperty, value);
    }
    
    public GradientStop? ActivatedStop
    {
        get => GetValue(ActivatedStopProperty);
        set => SetValue(ActivatedStopProperty, value);
    }
    
    internal IBrush? TransparentBgIntervalColor
    {
        get => GetValue(TransparentBgIntervalColorProperty);
        set => SetValue(TransparentBgIntervalColorProperty, value);
    }
    
    internal double TransparentBgSize
    {
        get => GetValue(TransparentBgSizeProperty);
        set => SetValue(TransparentBgSizeProperty, value);
    }

    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<ColorSlider, IBrush?> TransparentBgBrushProperty =
        AvaloniaProperty.RegisterDirect<ColorSlider, IBrush?>(
            nameof(TransparentBgBrush),
            o => o.TransparentBgBrush,
            (o, v) => o.TransparentBgBrush = v);
    
    private IBrush? _transparentBgBrush;

    internal IBrush? TransparentBgBrush
    {
        get => _transparentBgBrush;
        set => SetAndRaise(TransparentBgBrushProperty, ref _transparentBgBrush, value);
    }
    #endregion
    
    protected internal bool IsDragging;
    protected internal bool IsFocusEngaged;
    protected bool IgnorePropertyChanged = false;
    internal const double Tolerance = 0.0001;
    
    static AbstractColorSlider()
    {
        PressedMixin.Attach<AbstractColorSlider>();
        FocusableProperty.OverrideDefaultValue<AbstractColorSlider>(true);
    }
    
    protected void ConfigureCornerRadius()
    {
        CornerRadius = new CornerRadius(Height / 2);
    }
    
    protected override void UpdateDataValidation(
        AvaloniaProperty property,
        BindingValueType state,
        Exception? error)
    {
        if (property == ValueProperty)
        {
            DataValidationErrors.SetError(this, error);
        }
    }

    protected virtual void NotifyThumbDragStarted(VectorEventArgs e)
    {
        IsDragging = true;
    }
    
    protected virtual void NotifyThumbDragCompleted(VectorEventArgs e)
    {
        IsDragging = false;
    }
    
    protected void TrackReleased(object? sender, PointerReleasedEventArgs e)
    {
        IsDragging = false;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == HeightProperty ||
            change.Property == WidthProperty)
        {
            ConfigureCornerRadius();
        }
        
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == TransparentBgIntervalColorProperty ||
                change.Property == TransparentBgSizeProperty)
            {
                if (TransparentBgIntervalColor != null && TransparentBgIntervalColor is ISolidColorBrush solidColorBrush)
                {
                    TransparentBgBrush = TransparentBgBrushUtils.Build(TransparentBgSize, solidColorBrush.Color);
                }
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (TransparentBgIntervalColor != null && TransparentBgIntervalColor is ISolidColorBrush solidColorBrush)
        {
            TransparentBgBrush = TransparentBgBrushUtils.Build(TransparentBgSize, solidColorBrush.Color);
        }
    }
}