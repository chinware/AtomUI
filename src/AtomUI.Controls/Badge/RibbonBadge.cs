using AtomUI.Data;
using AtomUI.Theme.Palette;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum RibbonBadgePlacement
{
    Start,
    End
}

public class RibbonBadge : Control
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> RibbonColorProperty
        = AvaloniaProperty.Register<RibbonBadge, string?>(nameof(RibbonColor));

    public static readonly StyledProperty<Control?> DecoratedTargetProperty =
        AvaloniaProperty.Register<RibbonBadge, Control?>(nameof(DecoratedTarget));

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<RibbonBadge, Point>(nameof(Offset));

    public static readonly StyledProperty<string?> TextProperty
        = AvaloniaProperty.Register<RibbonBadge, string?>(nameof(Text));

    public static readonly StyledProperty<RibbonBadgePlacement> PlacementProperty
        = AvaloniaProperty.Register<RibbonBadge, RibbonBadgePlacement>(
            nameof(Text),
            RibbonBadgePlacement.End);

    public static readonly StyledProperty<bool> BadgeIsVisibleProperty =
        AvaloniaProperty.Register<RibbonBadge, bool>(nameof(BadgeIsVisible));

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
    
    static RibbonBadge()
    {
        AffectsMeasure<RibbonBadge>(DecoratedTargetProperty,
            TextProperty);
        AffectsRender<RibbonBadge>(RibbonColorProperty, PlacementProperty);
    }
    
    private RibbonBadgeAdorner? _ribbonBadgeAdorner;
    private AdornerLayer? _adornerLayer;

    private void HandleDecoratedTargetChanged()
    {
        if (_ribbonBadgeAdorner is not null)
        {
            if (DecoratedTarget is null)
            {
                VisualChildren.Add(_ribbonBadgeAdorner);
                ((ISetLogicalParent)_ribbonBadgeAdorner).SetParent(this);
                _ribbonBadgeAdorner.IsAdornerMode = false;
            }
            else if (DecoratedTarget is not null)
            {
                _ribbonBadgeAdorner.IsAdornerMode = true;
                VisualChildren.Add(DecoratedTarget);
                ((ISetLogicalParent)DecoratedTarget).SetParent(this);
            }
        }
    }

    private void SetupRibbonColor(string colorStr)
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

    private void SetupTokenBindings()
    {
        if (_ribbonBadgeAdorner is not null)
        {
            BindUtils.RelayBind(this, TextProperty, _ribbonBadgeAdorner, RibbonBadgeAdorner.TextProperty);
            BindUtils.RelayBind(this, OffsetProperty, _ribbonBadgeAdorner, RibbonBadgeAdorner.OffsetProperty);
            BindUtils.RelayBind(this, PlacementProperty, _ribbonBadgeAdorner, RibbonBadgeAdorner.PlacementProperty);
        }
    }

    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        if (DecoratedTarget is null)
        {
            CreateBadgeAdorner();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsVisibleProperty ||
            e.Property == BadgeIsVisibleProperty)
        {
            var badgeIsVisible = e.GetNewValue<bool>();
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

        if (VisualRoot is not null)
        {
            if (e.Property == DecoratedTargetProperty)
            {
                HandleDecoratedTargetChanged();
            }

            if (e.Property == RibbonColorProperty)
            {
                SetupRibbonColor(e.GetNewValue<string>());
            }
        }
    }

    private RibbonBadgeAdorner CreateBadgeAdorner()
    {
        if (_ribbonBadgeAdorner is null)
        {
            _ribbonBadgeAdorner = new RibbonBadgeAdorner();
            SetupTokenBindings();
            HandleDecoratedTargetChanged();
            if (RibbonColor is not null)
            {
                SetupRibbonColor(RibbonColor);
            }
        }

        return _ribbonBadgeAdorner;
    }

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
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HideAdorner();
    }
}