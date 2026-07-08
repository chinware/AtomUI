using AtomUI.Controls.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls.Primitives;

public class DashedBorder : Decorator
{
    #region 公共属性定义
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
            AvaloniaProperty.Register<DashedBorder, IBrush?>(nameof(Background));
    
    public static readonly StyledProperty<BackgroundSizing> BackgroundSizingProperty =
        AvaloniaProperty.Register<DashedBorder, BackgroundSizing>(
            nameof(BackgroundSizing),
            BackgroundSizing.CenterBorder);
    
    public static readonly StyledProperty<IBrush?> BorderBrushProperty =
        AvaloniaProperty.Register<DashedBorder, IBrush?>(nameof(BorderBrush));
        
    public static readonly StyledProperty<Thickness> BorderThicknessProperty =
        AvaloniaProperty.Register<DashedBorder, Thickness>(nameof(BorderThickness));
        
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<DashedBorder, CornerRadius>(nameof(CornerRadius));
        
    public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
        AvaloniaProperty.Register<DashedBorder, BoxShadows>(nameof(BoxShadow));
    
    public static readonly StyledProperty<IReadOnlyList<double>?> StrokeDashArrayProperty =
        AvaloniaProperty.Register<DashedBorder, IReadOnlyList<double>?>(nameof(StrokeDashArray));
    
    public static readonly StyledProperty<double> StrokeDaskOffsetProperty =
        AvaloniaProperty.Register<DashedBorder, double>(nameof(StrokeDaskOffset), 0.0);
    
    /// <summary>
    /// Gets or sets a brush with which to paint the background.
    /// </summary>
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets how the background is drawn relative to the border.
    /// </summary>
    public BackgroundSizing BackgroundSizing
    {
        get => GetValue(BackgroundSizingProperty);
        set => SetValue(BackgroundSizingProperty, value);
    }

    /// <summary>
    /// Gets or sets a brush with which to paint the border.
    /// </summary>
    public IBrush? BorderBrush
    {
        get => GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of the border.
    /// </summary>
    public Thickness BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the radius of the border rounded corners.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the box shadow effect parameters
    /// </summary>
    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }
    
    public IReadOnlyList<double>? StrokeDashArray
    {
        get => GetValue(StrokeDashArrayProperty);
        set => SetValue(StrokeDashArrayProperty, value);
    }

    public double StrokeDaskOffset
    {
        get => GetValue(StrokeDaskOffsetProperty);
        set => SetValue(StrokeDaskOffsetProperty, value);
    }
    
    #endregion
    
    private readonly BorderRenderHelper _borderRenderHelper = new BorderRenderHelper();
    private Thickness? _layoutThickness;
    private double _layoutScale;

    private Thickness LayoutThickness
    {
        get
        {
            VerifyLayoutScale();
            _layoutThickness ??= BorderUtils.BuildLayoutRoundedThickness(this, BorderThickness);
            return _layoutThickness.Value;
        }
    }

    static DashedBorder()
    {
        AffectsRender<DashedBorder>(
            BackgroundProperty,
            BackgroundSizingProperty,
            BorderBrushProperty,
            BorderThicknessProperty,
            CornerRadiusProperty,
            BoxShadowProperty,
            StrokeDashArrayProperty,
            StrokeDaskOffsetProperty,
            UseLayoutRoundingProperty);
        AffectsMeasure<DashedBorder>(BorderThicknessProperty);
    }

    public sealed override void Render(DrawingContext context)
    {
        _borderRenderHelper.Render(
            context,
            Bounds.Size,
            LayoutThickness,
            CornerRadius,
            BackgroundSizing,
            Background,
            BorderBrush,
            StrokeDashArray,
            StrokeDaskOffset,
            BoxShadow);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BorderThicknessProperty ||
            change.Property == UseLayoutRoundingProperty)
        {
            _layoutThickness = null;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return LayoutHelper.MeasureChild(Child, availableSize, Padding, BorderThickness);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return LayoutHelper.ArrangeChild(Child, finalSize, Padding, BorderThickness);
    }

    private void VerifyLayoutScale()
    {
        var currentScale = LayoutHelper.GetLayoutScale(this);
        if (MathUtils.AreClose(currentScale, _layoutScale))
        {
            return;
        }

        _layoutScale     = currentScale;
        _layoutThickness = null;
    }
}
