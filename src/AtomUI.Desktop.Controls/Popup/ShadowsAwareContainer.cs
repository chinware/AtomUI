using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class ShadowsAwareContainer : Decorator
{
    public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
        Border.BoxShadowProperty.AddOwner<ShadowsAwareContainer>();
    
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        Border.CornerRadiusProperty.AddOwner<ShadowsAwareContainer>();
    
    public static readonly StyledProperty<bool> IsArrowVisibleProperty =
        ArrowDecoratedBox.IsArrowVisibleProperty.AddOwner<ShadowsAwareContainer>();
    
    public static readonly StyledProperty<double> ArrowSizeProperty =
        ArrowDecoratedBox.ArrowSizeProperty.AddOwner<ShadowsAwareContainer>();
    
    public static readonly StyledProperty<Direction> ArrowDirectionProperty = 
        ArrowDecoratedBox.ArrowDirectionProperty.AddOwner<ShadowsAwareContainer>();
    
    public static readonly StyledProperty<bool> IsOverlayModeProperty = 
        AvaloniaProperty.Register<ShadowsAwareContainer, bool>(nameof (IsOverlayMode), false);
    
    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }
    
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
    
    public bool IsArrowVisible
    {
        get => GetValue(IsArrowVisibleProperty);
        set => SetValue(IsArrowVisibleProperty, value);
    }
    
    public double ArrowSize
    {
        get => GetValue(ArrowSizeProperty);
        set => SetValue(ArrowSizeProperty, value);
    }

    public Direction ArrowDirection
    {
        get => GetValue(ArrowDirectionProperty);
        set => SetValue(ArrowDirectionProperty, value);
    }
    
    public bool IsOverlayMode
    {
        get => GetValue(IsOverlayModeProperty);
        set => SetValue(IsOverlayModeProperty, value);
    }

    #region 内部属性定义
    internal static readonly DirectProperty<ShadowsAwareContainer, Rect> ArrowIndicatorLayoutBoundsProperty =
        AvaloniaProperty.RegisterDirect<ShadowsAwareContainer, Rect>(
            nameof(ArrowIndicatorLayoutBounds),
            o => o.ArrowIndicatorLayoutBounds,
            (o, v) => o.ArrowIndicatorLayoutBounds = v);
    
    private Rect _arrowIndicatorLayoutBounds;

    internal Rect ArrowIndicatorLayoutBounds
    {
        get => _arrowIndicatorLayoutBounds;
        set => SetAndRaise(ArrowIndicatorLayoutBoundsProperty, ref _arrowIndicatorLayoutBounds, value);
    }
    #endregion
    
    private Border? _shadowsRenderer;
    private IDisposable? _shadowsRenderDisposable;
    private IDisposable? _contentPresenterChildSubscription;

    private bool HasBoxShadow => BoxShadow.Count != 0;
    
    static ShadowsAwareContainer()
    {
        ClipToBoundsProperty.OverrideDefaultValue<ShadowsAwareContainer>(false);
        ChildProperty.Changed.AddClassHandler<ShadowsAwareContainer>((x, e) => x.ChildChanged(e));
        AffectsMeasure<ShadowsAwareContainer>(
            BoxShadowProperty, 
            ArrowDirectionProperty, 
            ArrowSizeProperty, 
            IsArrowVisibleProperty,
            IsOverlayModeProperty,
            ArrowIndicatorLayoutBoundsProperty);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        var popup = this.FindLogicalAncestorOfType<Popup>();
        if (popup != null)
        {
            _shadowsRenderDisposable?.Dispose();
            _shadowsRenderDisposable = BindUtils.RelayBind(popup, Popup.FrameShadowProperty, this, BoxShadowProperty);
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _shadowsRenderDisposable?.Dispose();
        _shadowsRenderDisposable = null;
        _contentPresenterChildSubscription?.Dispose();
        _contentPresenterChildSubscription = null;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size   = base.MeasureOverride(availableSize);
        var width  = size.Width;
        var height = size.Height;
        if (HasBoxShadow)
        {
            var shadowThickness = BoxShadow.Thickness();
            var effectiveShadowThickness = GetEffectiveShadowThickness(shadowThickness);

            if (!IsOverlayMode)
            {
                width  += effectiveShadowThickness.Left + effectiveShadowThickness.Right;
                height += effectiveShadowThickness.Top  + effectiveShadowThickness.Bottom;
            }
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var offsetX               = 0.0d;
        var offsetY               = 0.0d;
        if (!IsOverlayMode)
        {
            if (HasBoxShadow)
            {
                var shadowThickness = BoxShadow.Thickness();
                offsetX = shadowThickness.Left;
                offsetY = shadowThickness.Top;

                if (IsArrowVisible)
                {
                    if (ArrowDirection == Direction.Left)
                    {
                        offsetX = Math.Max(offsetX - ArrowIndicatorLayoutBounds.Width, 0);
                    }
                    else if (ArrowDirection == Direction.Top)
                    {
                        offsetY = Math.Max(offsetY - ArrowIndicatorLayoutBounds.Height, 0);
                    }
                }
            }
            if (Child != null)
            {
                Child.Arrange(new Rect(offsetX, offsetY, Child.DesiredSize.Width, Child.DesiredSize.Height));
            }
        }
        else
        {
            finalSize = base.ArrangeOverride(finalSize);
        }
        if (HasBoxShadow && _shadowsRenderer != null)
        {
            var shadowBounds = Child?.Bounds ?? default;
            if (IsArrowVisible)
            {
                var effectiveDirection = ArrowDirection;
                if (effectiveDirection == Direction.Top)
                {
                    shadowBounds = shadowBounds.WithY(ArrowIndicatorLayoutBounds.Height)
                                               .WithHeight(shadowBounds.Height - ArrowIndicatorLayoutBounds.Height);
                }
                else if (effectiveDirection == Direction.Bottom)
                {
                    shadowBounds = shadowBounds.WithHeight(shadowBounds.Height - ArrowIndicatorLayoutBounds.Height);
                }
                else if (effectiveDirection == Direction.Left)
                {
                    shadowBounds = shadowBounds.WithX(ArrowIndicatorLayoutBounds.Width)
                                               .WithWidth(shadowBounds.Width - ArrowIndicatorLayoutBounds.Width);
                }
                else
                {
                    shadowBounds = shadowBounds.WithWidth(shadowBounds.Width - ArrowIndicatorLayoutBounds.Width);
                }
            }
            _shadowsRenderer.Arrange(shadowBounds);
        }
        return finalSize;
    }

    private void ChildChanged(AvaloniaPropertyChangedEventArgs e)
    {
        EnsureShadowsRenderer();
        _contentPresenterChildSubscription?.Dispose();
        _contentPresenterChildSubscription = null;

        if (Child is ContentPresenter contentPresenter)
        {
            _contentPresenterChildSubscription = contentPresenter
                .GetObservable(ContentPresenter.ChildProperty)
                .Subscribe(PostConfigureShadowsInfo);

            PostConfigureShadowsInfo(contentPresenter.Child);
        }
        else if (Child != null)
        {
            ConfigureShadowsInfo(Child);
        }
    }

    private Thickness GetEffectiveShadowThickness(Thickness shadowThickness)
    {
        if (!IsArrowVisible)
        {
            return shadowThickness;
        }

        var left   = shadowThickness.Left;
        var top    = shadowThickness.Top;
        var right  = shadowThickness.Right;
        var bottom = shadowThickness.Bottom;
        
        switch (ArrowDirection)
        {
            case Direction.Left:
                left = Math.Max(left - ArrowIndicatorLayoutBounds.Width, 0);
                break;
            case Direction.Right:
                right = Math.Max(right - ArrowIndicatorLayoutBounds.Width, 0);
                break;
            case Direction.Top:
                top = Math.Max(top - ArrowIndicatorLayoutBounds.Height, 0);
                break;
            case Direction.Bottom:
                bottom = Math.Max(bottom - ArrowIndicatorLayoutBounds.Height, 0);
                break;
        }

        return new Thickness(left, top, right, bottom);
    }

    private void EnsureShadowsRenderer()
    {
        if (_shadowsRenderer != null)
        {
            return;
        }

        _shadowsRenderer                        = new Border();
        _shadowsRenderer[!BoxShadowProperty]    = this[!BoxShadowProperty];
        _shadowsRenderer[!CornerRadiusProperty] = this[!CornerRadiusProperty];
        ((ISetLogicalParent)_shadowsRenderer).SetParent(this);
        VisualChildren.Insert(0, _shadowsRenderer);
        LogicalChildren.Insert(0, _shadowsRenderer);
    }

    private void PostConfigureShadowsInfo(Control? child)
    {
        if (child != null)
        {
            ConfigureShadowsInfo(child);
        }
    }

    private void ConfigureShadowsInfo(Control child)
    {
        if (child is IArrowAwareShadowMaskInfoProvider arrowAwareShadowMaskInfoProvider)
        {
            var arrowDecoratedBox = arrowAwareShadowMaskInfoProvider.GetArrowDecoratedBox();
            this[!CornerRadiusProperty]               = arrowDecoratedBox[!CornerRadiusProperty];
            this[!ArrowSizeProperty]                  = arrowDecoratedBox[!ArrowSizeProperty];
            this[!ArrowIndicatorLayoutBoundsProperty] = arrowDecoratedBox[!ArrowDecoratedBox.ArrowIndicatorLayoutBoundsProperty];
            this[!ArrowDirectionProperty]             = arrowDecoratedBox[!ArrowDirectionProperty];
            this[!IsArrowVisibleProperty]                = arrowDecoratedBox[!IsArrowVisibleProperty];
        }
        else if (child is Border bordered)
        {
            SetCurrentValue(IsArrowVisibleProperty, false);
            this[!CornerRadiusProperty] = bordered[!CornerRadiusProperty];
        }
        else if (child is TemplatedControl templatedControl)
        {
            SetCurrentValue(IsArrowVisibleProperty, false);
            this[!CornerRadiusProperty] = templatedControl[!CornerRadiusProperty];
        }
    }
}
