using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class OverlayPopupContent : ContentControl
{
    #region 公共属性定义

    public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
        AvaloniaProperty.Register<OverlayPopupContent, BoxShadows>(nameof(BoxShadow));
    
    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<OverlayPopupContent, Thickness> EffectiveContentMarginProperty =
        AvaloniaProperty.RegisterDirect<OverlayPopupContent, Thickness>(nameof(EffectiveContentMargin),
            o => o.EffectiveContentMargin,
            (o, v) => o.EffectiveContentMargin = v);
    
    internal static readonly DirectProperty<OverlayPopupContent, CornerRadius> MaskShadowsContentCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<OverlayPopupContent, CornerRadius>(
            nameof(MaskShadowsContentCornerRadius),
            o => o.MaskShadowsContentCornerRadius,
            (o, v) => o.MaskShadowsContentCornerRadius = v);
    
    internal static readonly DirectProperty<OverlayPopupContent, Rect> ArrowIndicatorLayoutBoundsProperty =
        AvaloniaProperty.RegisterDirect<OverlayPopupContent, Rect>(
            nameof(ArrowIndicatorLayoutBounds),
            o => o.ArrowIndicatorLayoutBounds,
            (o, v) => o.ArrowIndicatorLayoutBounds = v);
    
    internal static readonly StyledProperty<Direction> ArrowDirectionProperty = 
        ArrowDecoratedBox.ArrowDirectionProperty.AddOwner<OverlayPopupContent>();
    
    internal static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<OverlayPopupContent>();
    
    internal static readonly StyledProperty<double> ArrowSizeProperty =
        ArrowDecoratedBox.ArrowSizeProperty.AddOwner<OverlayPopupContent>();
    
    internal static readonly DirectProperty<OverlayPopupContent, bool> IsFlippedProperty =
        AvaloniaProperty.RegisterDirect<OverlayPopupContent, bool>(nameof(IsFlipped),
            o => o.IsFlipped,
            (o, v) => o.IsFlipped = v);

    private Thickness _effectiveContentMargin;

    internal Thickness EffectiveContentMargin
    {
        get => _effectiveContentMargin;
        set => SetAndRaise(EffectiveContentMarginProperty, ref _effectiveContentMargin, value);
    }
    
    private CornerRadius _maskShadowsContentCornerRadius;

    internal CornerRadius MaskShadowsContentCornerRadius
    {
        get => _maskShadowsContentCornerRadius;
        set => SetAndRaise(MaskShadowsContentCornerRadiusProperty, ref _maskShadowsContentCornerRadius, value);
    }
    
    private Rect _arrowIndicatorLayoutBounds;

    internal Rect ArrowIndicatorLayoutBounds
    {
        get => _arrowIndicatorLayoutBounds;
        set => SetAndRaise(ArrowIndicatorLayoutBoundsProperty, ref _arrowIndicatorLayoutBounds, value);
    }
    
    internal Direction ArrowDirection
    {
        get => GetValue(ArrowDirectionProperty);
        set => SetValue(ArrowDirectionProperty, value);
    }
    
    internal bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }
    
    internal double ArrowSize
    {
        get => GetValue(ArrowSizeProperty);
        set => SetValue(ArrowSizeProperty, value);
    }
    
    private bool _isFlipped;

    public bool IsFlipped
    {
        get => _isFlipped;
        private set => SetAndRaise(IsFlippedProperty, ref _isFlipped, value);
    }
    
    #endregion
    
    private Border? _maskRenderer;
    private CompositeDisposable? _bindingDisposables;

    static OverlayPopupContent()
    {
        AffectsRender<OverlayPopupContent>(MaskShadowsContentCornerRadiusProperty);
        AffectsMeasure<OverlayPopupContent>(IsShowArrowProperty, ArrowSizeProperty);
        AffectsArrange<OverlayPopupContent>(ArrowDirectionProperty, IsFlippedProperty);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _maskRenderer = e.NameScope.Find<Border>(OverlayPopupContentThemeConstants.MaskShadowsPart);
        ConfigureShadowInfo();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BoxShadowProperty ||
            change.Property == ContentProperty)
        {
            ConfigureShadowInfo();
        }
    }
    
    private void ConfigureShadowInfo()
    {
        if (Content is IArrowAwareShadowMaskInfoProvider arrowAwareShadowMaskInfoProvider)
        {
            var arrowDecoratedBox = arrowAwareShadowMaskInfoProvider.GetArrowDecoratedBox();
            _bindingDisposables?.Dispose();
            _bindingDisposables = new CompositeDisposable();
            _bindingDisposables?.Add(BindUtils.RelayBind(arrowDecoratedBox, ArrowDecoratedBox.CornerRadiusProperty, this, MaskShadowsContentCornerRadiusProperty));
            _bindingDisposables?.Add(BindUtils.RelayBind(arrowDecoratedBox, ArrowDecoratedBox.ArrowIndicatorLayoutBoundsProperty, this, ArrowIndicatorLayoutBoundsProperty));
            _bindingDisposables?.Add(BindUtils.RelayBind(arrowDecoratedBox, ArrowDecoratedBox.ArrowSizeProperty, this, ArrowSizeProperty));
            _bindingDisposables?.Add(BindUtils.RelayBind(arrowDecoratedBox, ArrowDecoratedBox.ArrowDirectionProperty, this, ArrowDirectionProperty));
            _bindingDisposables?.Add(BindUtils.RelayBind(arrowDecoratedBox, ArrowDecoratedBox.IsShowArrowProperty, this, IsShowArrowProperty));
        }
        else if (Content is Border bordered)
        {
            MaskShadowsContentCornerRadius = bordered.CornerRadius;
        }
        else if (Content is TemplatedControl templatedControl)
        {
            MaskShadowsContentCornerRadius = templatedControl.CornerRadius;
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size         = base.ArrangeOverride(finalSize);
        var targetBounds = _maskRenderer?.Bounds ?? default;
        var offsetX      = 0;
        var offsetY      = 0;
        var width        = finalSize.Width;
        var height       = finalSize.Height;
        var arrowBounds  = ArrowIndicatorLayoutBounds;
        if (IsShowArrow)
        {
            var effectiveDirection = ArrowDirection;
            if (effectiveDirection == Direction.Top)
            {
                targetBounds = new Rect(offsetX, offsetY + arrowBounds.Height, width, height - arrowBounds.Height);
            }
            else if (effectiveDirection == Direction.Bottom)
            {
                targetBounds = targetBounds.WithHeight(height - arrowBounds.Height);
            }
            else if (effectiveDirection == Direction.Left)
            {
                targetBounds = targetBounds.WithX(arrowBounds.Width).WithWidth(width - arrowBounds.Width);
            }
            else if (effectiveDirection == Direction.Right)
            {
                targetBounds = targetBounds.WithWidth(width - arrowBounds.Width);
            }
        }
        _maskRenderer?.Arrange(targetBounds);
        
        return size;
    }
}