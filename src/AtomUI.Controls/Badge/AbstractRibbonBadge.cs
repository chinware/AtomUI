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
        AvaloniaProperty.Register<AbstractRibbonBadge, bool>(nameof(BadgeIsVisible));

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
                _ribbonBadgeAdorner.SetLogicalParent(this);
                _ribbonBadgeAdorner.IsAdornerMode = false;
                VisualChildren.Add(_ribbonBadgeAdorner);
                LogicalChildren.Add(_ribbonBadgeAdorner);
            }
            else if (DecoratedTarget is not null)
            {
                _ribbonBadgeAdorner.IsAdornerMode = true;
                DecoratedTarget.SetLogicalParent(this);
                VisualChildren.Add(DecoratedTarget);
                LogicalChildren.Add(DecoratedTarget);
            }
        }
    }

    private protected void SetupRibbonColor(string colorStr)
    {
        colorStr = colorStr.Trim().ToLower();

        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            if (presetColor.Type.ToString().ToLower() == colorStr)
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
                HandleDecoratedTargetChanged();
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
        if (_adornerLayer is null && DecoratedTarget is not null)
        {
            var ribbonBadgeAdorner = CreateBadgeAdorner();
            _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            // 这里需要抛出异常吗？
            if (_adornerLayer == null)
            {
                return;
            }

            AdornerLayer.SetAdornedElement(ribbonBadgeAdorner, this);
            AdornerLayer.SetIsClipEnabled(ribbonBadgeAdorner, false);
            _adornerLayer.Children.Add(ribbonBadgeAdorner);
        }
    }

    private void HideAdorner()
    {
        // 这里需要抛出异常吗？
        if (_adornerLayer is null || _ribbonBadgeAdorner is null)
        {
            return;
        }

        _adornerLayer.Children.Remove(_ribbonBadgeAdorner);
        _adornerLayer = null;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        PrepareAdorner();
        if (DecoratedTarget is null)
        {
            CreateBadgeAdorner();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HideAdorner();
    }
}