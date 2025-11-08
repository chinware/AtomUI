using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;

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
    #endregion
    
    private IArrowAwareShadowMaskInfoProvider? _popupArrowDecoratedBox;
    private Border? _maskRenderer;
    private CompositeDisposable? _bindingDisposables;

    static OverlayPopupContent()
    {
        AffectsRender<PopupBuddyLayer>(MaskShadowsContentCornerRadiusProperty);
        AffectsMeasure<PopupBuddyLayer>(ArrowIndicatorLayoutBoundsProperty, IsShowArrowProperty);
        AffectsArrange<PopupBuddyLayer>(ArrowSizeProperty, ArrowDirectionProperty);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _maskRenderer = e.NameScope.Find<Border>(OverlayPopupContentThemeConstants.MaskShadowsPart);
        ConfigureMarginForShadows();
        ConfigureShadowInfo();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == BoxShadowProperty ||
                change.Property == ContentProperty)
            {
                ConfigureMarginForShadows();
                ConfigureShadowInfo();
            }
        }
    }
    
    private void ConfigureMarginForShadows()
    {
        var thickness = BoxShadow.Thickness();
        if (Content is IArrowAwareShadowMaskInfoProvider arrowAwareShadowMaskInfoProvider)
        {
            if (arrowAwareShadowMaskInfoProvider.IsShowArrow())
            {
                var arrowPosition = arrowAwareShadowMaskInfoProvider.GetArrowPosition();
                var direction     = ArrowDecoratedBox.GetDirection(arrowPosition);
                var delta         = arrowAwareShadowMaskInfoProvider.GetArrowIndicatorBounds().Height + 0.5;
                if (direction == Direction.Bottom)
                {
                    thickness = new Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom + delta);
                }
                else if (direction == Direction.Top)
                {
                    thickness = new Thickness(thickness.Left, thickness.Top + delta, thickness.Right, thickness.Bottom);
                }
                else if (direction == Direction.Left)
                {
                    thickness = new Thickness(thickness.Left + delta, thickness.Top, thickness.Right, thickness.Bottom);
                }
                else
                {
                    thickness = new Thickness(thickness.Left, thickness.Top, thickness.Right + delta, thickness.Bottom);
                }
            }
        }

        SetCurrentValue(EffectiveContentMarginProperty, thickness);
    }
    
    private void ConfigureShadowInfo()
    {
        if (Content is IArrowAwareShadowMaskInfoProvider arrowAwareShadowMaskInfoProvider)
        {
            _bindingDisposables?.Dispose();
            _bindingDisposables = new CompositeDisposable();
            var arrowDecoratedBox = arrowAwareShadowMaskInfoProvider.GetArrowDecoratedBox();
            _popupArrowDecoratedBox = arrowDecoratedBox;
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

    internal Point DeltaOffset()
    {
        var shadowThickness = BoxShadow.Thickness();
        return new Point(shadowThickness.Left, shadowThickness.Top);
    }

}