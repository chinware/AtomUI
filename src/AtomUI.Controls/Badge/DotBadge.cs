using AtomUI.Data;
using AtomUI.Theme.Palette;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum DotBadgeStatus
{
    Default,
    Success,
    Processing,
    Error,
    Warning
}

public class DotBadge : Control
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> DotColorProperty
        = AvaloniaProperty.Register<DotBadge, string?>(
            nameof(DotColor));

    public static readonly StyledProperty<DotBadgeStatus?> StatusProperty
        = AvaloniaProperty.Register<DotBadge, DotBadgeStatus?>(
            nameof(Status));

    public static readonly StyledProperty<string?> TextProperty
        = AvaloniaProperty.Register<DotBadge, string?>(
            nameof(Text));

    public static readonly StyledProperty<Control?> DecoratedTargetProperty =
        AvaloniaProperty.Register<DotBadge, Control?>(nameof(DecoratedTarget));

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<DotBadge, Point>(nameof(Offset));

    public static readonly StyledProperty<bool> BadgeIsVisibleProperty =
        AvaloniaProperty.Register<DotBadge, bool>(nameof(BadgeIsVisible));

    public string? DotColor
    {
        get => GetValue(DotColorProperty);
        set => SetValue(DotColorProperty, value);
    }

    public DotBadgeStatus? Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    [Content]
    public Control? DecoratedTarget
    {
        get => GetValue(DecoratedTargetProperty);
        set => SetValue(DecoratedTargetProperty, value);
    }

    public Point Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public bool BadgeIsVisible
    {
        get => GetValue(BadgeIsVisibleProperty);
        set => SetValue(BadgeIsVisibleProperty, value);
    }

    #endregion

    private DotBadgeAdorner? _dotBadgeAdorner;
    private AdornerLayer? _adornerLayer;

    static DotBadge()
    {
        AffectsMeasure<DotBadge>(DecoratedTargetProperty, TextProperty);
        AffectsRender<DotBadge>(DotColorProperty, StatusProperty);
        HorizontalAlignmentProperty.OverrideDefaultValue<DotBadge>(HorizontalAlignment.Left);
        VerticalAlignmentProperty.OverrideDefaultValue<DotBadge>(VerticalAlignment.Top);
    }

    private DotBadgeAdorner CreateDotBadgeAdorner()
    {
        if (_dotBadgeAdorner is null)
        {
            _dotBadgeAdorner = new DotBadgeAdorner();
            SetupTokenBindings();
            HandleDecoratedTargetChanged();
            if (DotColor is not null)
            {
                SetupDotColor(DotColor);
            }
        }

        return _dotBadgeAdorner;
    }

    private void PrepareAdorner()
    {
        if (DecoratedTarget is not null)
        {
            var dotBadgeAdorner = CreateDotBadgeAdorner();
            _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            // 这里需要抛出异常吗？
            if (_adornerLayer == null)
            {
                return;
            }

            dotBadgeAdorner.ApplyToTarget(_adornerLayer, this);
        }
    }
    
    private void HideAdorner()
    {
        // 这里需要抛出异常吗？
        if ( _dotBadgeAdorner is null)
        {
            return;
        }
        _dotBadgeAdorner.DetachFromTarget(_adornerLayer);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HideAdorner();
    }
    
    private void SetupTokenBindings()
    {
        if (_dotBadgeAdorner is not null)
        {
            BindUtils.RelayBind(this, StatusProperty, _dotBadgeAdorner, DotBadgeAdorner.StatusProperty);
            BindUtils.RelayBind(this, TextProperty, _dotBadgeAdorner, DotBadgeAdorner.TextProperty);
            BindUtils.RelayBind(this, OffsetProperty, _dotBadgeAdorner, DotBadgeAdorner.OffsetProperty);
        }
    }

    private void HandleDecoratedTargetChanged()
    {
        if (_dotBadgeAdorner is not null)
        {
            if (DecoratedTarget is null)
            {
                _dotBadgeAdorner.IsAdornerMode = false;
                ((ISetLogicalParent)_dotBadgeAdorner).SetParent(this);
                VisualChildren.Add(_dotBadgeAdorner);
            }
            else
            {
                _dotBadgeAdorner.IsAdornerMode = true;
                VisualChildren.Add(DecoratedTarget);
                ((ISetLogicalParent)DecoratedTarget).SetParent(this);
            }
        }
    }

    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        if (DecoratedTarget is null)
        {
            CreateDotBadgeAdorner();
        }
        else
        {
            PrepareAdorner();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsVisibleProperty)
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
        else if (e.Property == BadgeIsVisibleProperty)
        {
            if (BadgeIsVisible)
            {
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

            if (e.Property == DotColorProperty)
            {
                SetupDotColor(e.GetNewValue<string>());
            }
        }
    }

    private void SetupDotColor(string colorStr)
    {
        colorStr = colorStr.Trim().ToLower();

        foreach (var presetColor in PresetPrimaryColor.AllColorTypes())
        {
            if (presetColor.Type.ToString().ToLower() == colorStr)
            {
                _dotBadgeAdorner!.BadgeDotColor = new SolidColorBrush(presetColor.Color());
                return;
            }
        }

        if (Color.TryParse(colorStr, out var color))
        {
            _dotBadgeAdorner!.BadgeDotColor = new SolidColorBrush(color);
        }
    }
    
}