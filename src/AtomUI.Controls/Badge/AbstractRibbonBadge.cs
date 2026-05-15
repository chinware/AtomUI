using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Commons;

public enum RibbonBadgePlacement
{
    Start,
    End
}

public abstract class AbstractRibbonBadge : Control
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> RibbonColorProperty =
        AvaloniaProperty.Register<AbstractRibbonBadge, string?>(nameof(RibbonColor));

    public static readonly StyledProperty<Control?> DecoratedTargetProperty =
        AvaloniaProperty.Register<AbstractRibbonBadge, Control?>(nameof(DecoratedTarget));

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<AbstractRibbonBadge, Point>(nameof(Offset));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<AbstractRibbonBadge, string?>(nameof(Text));

    public static readonly StyledProperty<RibbonBadgePlacement> PlacementProperty =
        AvaloniaProperty.Register<AbstractRibbonBadge, RibbonBadgePlacement>(
            nameof(Placement),
            RibbonBadgePlacement.End);

    public static readonly StyledProperty<bool> BadgeIsVisibleProperty =
        AvaloniaProperty.Register<AbstractRibbonBadge, bool>(nameof(BadgeIsVisible), true);

    [Content]
    public Control? DecoratedTarget
    {
        get => GetValue(DecoratedTargetProperty);
        set => SetValue(DecoratedTargetProperty, value);
    }

    public string? RibbonColor
    {
        get => GetValue(RibbonColorProperty);
        set => SetValue(RibbonColorProperty, value);
    }

    public Point Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public RibbonBadgePlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public bool BadgeIsVisible
    {
        get => GetValue(BadgeIsVisibleProperty);
        set => SetValue(BadgeIsVisibleProperty, value);
    }

    #endregion
    
    static AbstractRibbonBadge()
    {
        AffectsMeasure<AbstractRibbonBadge>(DecoratedTargetProperty,
            TextProperty);
        AffectsRender<AbstractRibbonBadge>(RibbonColorProperty, PlacementProperty);
    }
    
    private protected AbstractRibbonBadgeAdorner? _ribbonBadgeAdorner;
    private protected AdornerLayer? _adornerLayer;

    private protected void HandleDecoratedTargetChanged()
    {
        if (_ribbonBadgeAdorner is not null)
        {
            if (DecoratedTarget is null)
            {
                _ribbonBadgeAdorner.IsAdornerMode = false;
                EnsureRibbonBadgeAdornerAttached();
            }
            else if (DecoratedTarget is not null)
            {
                DetachRibbonBadgeAdornerFromControl();
                _ribbonBadgeAdorner.IsAdornerMode = true;
                EnsureDecoratedTargetAttached();
            }
        }
    }

    private protected void SetupRibbonColor(string? colorStr)
    {
        if (_ribbonBadgeAdorner is not null)
        {
            _ribbonBadgeAdorner.RibbonColor = BadgeColorUtils.CalculateColor(colorStr);
        }
    }

    private protected virtual void SetupTokenBindings()
    {
        if (_ribbonBadgeAdorner is not null)
        {
            _ribbonBadgeAdorner[!AbstractRibbonBadgeAdorner.TextProperty]      = this[!TextProperty];
            _ribbonBadgeAdorner[!AbstractRibbonBadgeAdorner.OffsetProperty]    = this[!OffsetProperty];
            _ribbonBadgeAdorner[!AbstractRibbonBadgeAdorner.PlacementProperty] = this[!PlacementProperty];
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsVisibleProperty ||
            change.Property == BadgeIsVisibleProperty)
        {
            var badgeIsVisible = change.GetNewValue<bool>();
            if (badgeIsVisible)
            {
                if (_adornerLayer is not null)
                {
                    return;
                }

                PrepareAdorner();
            }
            else
            {
                HideAdorner();
            }
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == DecoratedTargetProperty)
            {
                if (change.GetOldValue<Control?>() is { } oldTarget)
                {
                    DetachControlChild(oldTarget);
                }
                HideAdorner();
                EnsureDecoratedTargetAttached();
                HandleDecoratedTargetChanged();
                if (BadgeIsVisible)
                {
                    PrepareAdorner();
                }
            }

            if (change.Property == RibbonColorProperty)
            {
                SetupRibbonColor(change.GetNewValue<string>());
            }
        }
    }

    private protected abstract AbstractRibbonBadgeAdorner CreateBadgeAdorner();

    private void PrepareAdorner()
    {
        if (!BadgeIsVisible)
        {
            return;
        }

        if (_adornerLayer is null && DecoratedTarget is not null)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            // 这里需要抛出异常吗？
            if (adornerLayer == null)
            {
                return;
            }

            _adornerLayer = adornerLayer;
            var ribbonBadgeAdorner = CreateBadgeAdorner();
            DetachRibbonBadgeAdornerFromControl();
            AdornerLayer.SetAdornedElement(ribbonBadgeAdorner, this);
            AdornerLayer.SetIsClipEnabled(ribbonBadgeAdorner, true);
            adornerLayer.Children.Add(ribbonBadgeAdorner);
        }
        else if (DecoratedTarget is null)
        {
            CreateBadgeAdorner();
            EnsureRibbonBadgeAdornerAttached();
            IsVisible = true;
        }
    }

    private void HideAdorner()
    {
        // 这里需要抛出异常吗？
        if (_ribbonBadgeAdorner is null)
        {
            return;
        }

        if (_adornerLayer is not null)
        {
            _adornerLayer.Children.Remove(_ribbonBadgeAdorner);
            AdornerLayer.SetAdornedElement(_ribbonBadgeAdorner, null);
            _adornerLayer = null;
        }
        else if (DecoratedTarget is null)
        {
            DetachRibbonBadgeAdornerFromControl();
            IsVisible = false;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsureDecoratedTargetAttached();
        if (BadgeIsVisible)
        {
            PrepareAdorner();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HideAdorner();
    }

    private void EnsureDecoratedTargetAttached()
    {
        if (DecoratedTarget is null)
        {
            return;
        }

        if (!VisualChildren.Contains(DecoratedTarget))
        {
            DecoratedTarget.SetLogicalParent(this);
            VisualChildren.Add(DecoratedTarget);
            LogicalChildren.Add(DecoratedTarget);
        }
    }

    private void DetachDecoratedTarget()
    {
        if (DecoratedTarget is not null)
        {
            DetachControlChild(DecoratedTarget);
        }
    }

    private void EnsureRibbonBadgeAdornerAttached()
    {
        if (_ribbonBadgeAdorner is null)
        {
            return;
        }

        DetachDecoratedTarget();
        if (!VisualChildren.Contains(_ribbonBadgeAdorner))
        {
            _ribbonBadgeAdorner.SetLogicalParent(this);
            VisualChildren.Add(_ribbonBadgeAdorner);
            LogicalChildren.Add(_ribbonBadgeAdorner);
        }
    }

    private void DetachRibbonBadgeAdornerFromControl()
    {
        if (_ribbonBadgeAdorner is null)
        {
            return;
        }

        VisualChildren.Remove(_ribbonBadgeAdorner);
        LogicalChildren.Remove(_ribbonBadgeAdorner);
        _ribbonBadgeAdorner.SetLogicalParent(null);
    }

    private void DetachControlChild(Control child)
    {
        VisualChildren.Remove(child);
        LogicalChildren.Remove(child);
        child.SetLogicalParent(null);
    }
}
