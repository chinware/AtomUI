using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Commons;

public enum DotBadgeStatus
{
    Default,
    Success,
    Processing,
    Error,
    Warning
}

public abstract class AbstractDotBadge : Control, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> DotColorProperty =
        AvaloniaProperty.Register<AbstractDotBadge, string?>(
            nameof(DotColor));

    public static readonly StyledProperty<DotBadgeStatus?> StatusProperty =
        AvaloniaProperty.Register<AbstractDotBadge, DotBadgeStatus?>(
            nameof(Status));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<AbstractDotBadge, string?>(nameof(Text));

    public static readonly StyledProperty<Control?> DecoratedTargetProperty =
        AvaloniaProperty.Register<AbstractDotBadge, Control?>(nameof(DecoratedTarget));

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<AbstractDotBadge, Point>(nameof(Offset));

    public static readonly StyledProperty<bool> BadgeIsVisibleProperty =
        AvaloniaProperty.Register<AbstractDotBadge, bool>(nameof(BadgeIsVisible));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractDotBadge>();

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
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    private protected AbstractDotBadgeAdorner? _dotBadgeAdorner;
    private protected AdornerLayer? _adornerLayer;

    static AbstractDotBadge()
    {
        AffectsMeasure<AbstractDotBadge>(DecoratedTargetProperty, TextProperty);
        AffectsRender<AbstractDotBadge>(DotColorProperty, StatusProperty);
        HorizontalAlignmentProperty.OverrideDefaultValue<AbstractDotBadge>(HorizontalAlignment.Left);
        VerticalAlignmentProperty.OverrideDefaultValue<AbstractDotBadge>(VerticalAlignment.Top);
    }

    public AbstractDotBadge()
    {
        this.ConfigureMotionBindingStyle();
    }

    private protected abstract AbstractDotBadgeAdorner CreateDotBadgeAdorner();

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
        else
        {
            IsVisible = true;
        }
    }

    private void HideAdorner(bool enableMotion)
    {
        // 这里需要抛出异常吗？
        if (_dotBadgeAdorner is null)
        {
            return;
        }

        _dotBadgeAdorner.DetachFromTargetAsync(_adornerLayer, enableMotion);
        if (!enableMotion)
        {
            if (DecoratedTarget is null)
            {
                IsVisible = false;
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DecoratedTarget is null)
        {
            CreateDotBadgeAdorner();
        }
        else
        {
            PrepareAdorner();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HideAdorner(false);
    }

    private protected virtual void SetupTokenBindings()
    {
        if (_dotBadgeAdorner is not null)
        {
            _dotBadgeAdorner[!AbstractDotBadgeAdorner.StatusProperty] = this[!StatusProperty];
            _dotBadgeAdorner[!AbstractDotBadgeAdorner.TextProperty]   = this[!TextProperty];
            _dotBadgeAdorner[!AbstractDotBadgeAdorner.OffsetProperty] = this[!OffsetProperty];
            _dotBadgeAdorner[!IsMotionEnabledProperty]                = this[!IsMotionEnabledProperty];
        }
    }

    private protected virtual void HandleDecoratedTargetChanged()
    {
        if (_dotBadgeAdorner is not null)
        {
            if (DecoratedTarget is null)
            {
                _dotBadgeAdorner.IsAdornerMode = false;
                _dotBadgeAdorner.SetLogicalParent(this);
                VisualChildren.Add(_dotBadgeAdorner);
                LogicalChildren.Add(_dotBadgeAdorner);
            }
            else
            {
                _dotBadgeAdorner.IsAdornerMode = true;
                DecoratedTarget.SetLogicalParent(this);
                VisualChildren.Add(DecoratedTarget);
                LogicalChildren.Add(DecoratedTarget);
            }
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
                PrepareAdorner();
            }
            else
            {
                HideAdorner(IsMotionEnabled && IsLoaded);
            }
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == DecoratedTargetProperty)
            {
                HandleDecoratedTargetChanged();
            }

            if (change.Property == DotColorProperty)
            {
                ConfigureDotColor(change.GetNewValue<string>());
            }
        }
    }

    private protected virtual void ConfigureDotColor(string colorStr)
    {
        _dotBadgeAdorner!.BadgeDotColor = BadgeColorUtils.CalculateColor(colorStr);
    }
    
}