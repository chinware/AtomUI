using AtomUI.Theme.Palette;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
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

    private void AttachChild(Control child)
    {
        if (child.GetVisualParent() == this)
        {
            return;
        }

        child.SetLogicalParent(this);
        VisualChildren.Add(child);
        LogicalChildren.Add(child);
    }

    private void DetachChild(Control? child)
    {
        if (child is null)
        {
            return;
        }

        if (child.GetVisualParent() == this)
        {
            VisualChildren.Remove(child);
        }

        if (LogicalChildren.Contains(child))
        {
            LogicalChildren.Remove(child);
        }

        child.SetLogicalParent(null);
    }

    private void DetachLayerAdorner()
    {
        if (_adornerLayer is null || _ribbonBadgeAdorner is null)
        {
            _adornerLayer = null;
            return;
        }

        _adornerLayer.Children.Remove(_ribbonBadgeAdorner);
        _adornerLayer = null;
    }

    private protected void HandleDecoratedTargetChanged(Control? oldDecoratedTarget = null)
    {
        if (_ribbonBadgeAdorner is not null)
        {
            DetachChild(oldDecoratedTarget);
            if (DecoratedTarget is null)
            {
                DetachLayerAdorner();
                _ribbonBadgeAdorner.IsAdornerMode = false;
                if (BadgeIsVisible)
                {
                    AttachChild(_ribbonBadgeAdorner);
                }
            }
            else if (DecoratedTarget is not null)
            {
                DetachChild(_ribbonBadgeAdorner);
                _ribbonBadgeAdorner.IsAdornerMode = true;
                AttachChild(DecoratedTarget);
            }
        }
    }

    private protected void SetupRibbonColor(string colorStr)
    {
        colorStr = colorStr.Trim();

        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            if (string.Equals(presetColor.Type.ToString(), colorStr, StringComparison.OrdinalIgnoreCase))
            {
                _ribbonBadgeAdorner!.RibbonColor = new SolidColorBrush(presetColor.Color());
                return;
            }
        }

        if (Color.TryParse(colorStr, out var color))
        {
            _ribbonBadgeAdorner!.RibbonColor = new SolidColorBrush(color);
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
                HandleDecoratedTargetChanged(change.GetOldValue<Control?>());
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
        var ribbonBadgeAdorner = CreateBadgeAdorner();
        if (DecoratedTarget is not null)
        {
            DetachChild(ribbonBadgeAdorner);
            AttachChild(DecoratedTarget);
            ribbonBadgeAdorner.IsAdornerMode = true;
            if (_adornerLayer is not null)
            {
                return;
            }

            _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            // 这里需要抛出异常吗？
            if (_adornerLayer == null)
            {
                return;
            }

            AdornerLayer.SetAdornedElement(ribbonBadgeAdorner, this);
            AdornerLayer.SetIsClipEnabled(ribbonBadgeAdorner, true);
            _adornerLayer.Children.Add(ribbonBadgeAdorner);
        }
        else
        {
            DetachLayerAdorner();
            ribbonBadgeAdorner.IsAdornerMode = false;
            AttachChild(ribbonBadgeAdorner);
            IsVisible = true;
        }
    }

    private void HideAdorner()
    {
        if (_ribbonBadgeAdorner is null)
        {
            return;
        }

        if (DecoratedTarget is null)
        {
            DetachChild(_ribbonBadgeAdorner);
            IsVisible = false;
        }
        else
        {
            DetachLayerAdorner();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
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
}
