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
        AvaloniaProperty.Register<AbstractDotBadge, bool>(nameof(BadgeIsVisible), true);
    
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
        if (!BadgeIsVisible)
        {
            return;
        }

        if (DecoratedTarget is not null)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            // 这里需要抛出异常吗？
            if (adornerLayer == null)
            {
                return;
            }

            _adornerLayer = adornerLayer;
            var dotBadgeAdorner = CreateDotBadgeAdorner();
            DetachDotBadgeAdornerFromControl();
            dotBadgeAdorner.ApplyToTarget(adornerLayer, this);
        } 
        else
        {
            IsVisible = true;
            CreateDotBadgeAdorner();
            EnsureDotBadgeAdornerAttached();
        }
    }

    private void HideAdorner(bool enableMotion)
    {
        // 这里需要抛出异常吗？
        if (_dotBadgeAdorner is null)
        {
            return;
        }

        var adornerLayer = _adornerLayer;
        _adornerLayer = null;
        var shouldAnimate = enableMotion && DecoratedTarget is not null;
        _dotBadgeAdorner.DetachFromTargetAsync(adornerLayer, shouldAnimate);
        if (!shouldAnimate)
        {
            if (DecoratedTarget is null)
            {
                DetachDotBadgeAdornerFromControl();
                IsVisible = false;
            }
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

    private protected virtual void NotifyDecoratedTargetChanged()
    {
        if (_dotBadgeAdorner is not null)
        {
            if (DecoratedTarget is null)
            {
                _dotBadgeAdorner.IsAdornerMode = false;
                EnsureDotBadgeAdornerAttached();
            }
            else
            {
                DetachDotBadgeAdornerFromControl();
                _dotBadgeAdorner.IsAdornerMode = true;
                EnsureDecoratedTargetAttached();
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
                if (change.GetOldValue<Control?>() is { } oldTarget)
                {
                    DetachControlChild(oldTarget);
                }
                HideAdorner(false);
                EnsureDecoratedTargetAttached();
                NotifyDecoratedTargetChanged();
                if (BadgeIsVisible)
                {
                    PrepareAdorner();
                }
            }

            if (change.Property == DotColorProperty)
            {
                ConfigureDotColor(change.GetNewValue<string>());
            }
        }
    }

    private protected virtual void ConfigureDotColor(string? colorStr)
    {
        if (_dotBadgeAdorner is not null)
        {
            _dotBadgeAdorner.BadgeDotColor = BadgeColorUtils.CalculateColor(colorStr);
        }
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

    private void EnsureDotBadgeAdornerAttached()
    {
        if (_dotBadgeAdorner is null)
        {
            return;
        }

        DetachDecoratedTarget();
        if (!VisualChildren.Contains(_dotBadgeAdorner))
        {
            _dotBadgeAdorner.SetLogicalParent(this);
            VisualChildren.Add(_dotBadgeAdorner);
            LogicalChildren.Add(_dotBadgeAdorner);
        }
    }

    private void DetachDotBadgeAdornerFromControl()
    {
        if (_dotBadgeAdorner is null)
        {
            return;
        }

        VisualChildren.Remove(_dotBadgeAdorner);
        LogicalChildren.Remove(_dotBadgeAdorner);
        _dotBadgeAdorner.SetLogicalParent(null);
    }

    private void DetachControlChild(Control child)
    {
        VisualChildren.Remove(child);
        LogicalChildren.Remove(child);
        child.SetLogicalParent(null);
    }
    
}
