using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Commons;

public enum CountBadgeSize
{
    Default,
    Small
}

public abstract class AbstractCountBadge : Control, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> BadgeColorProperty =
        AvaloniaProperty.Register<AbstractCountBadge, string?>(
            nameof(BadgeColor));

    public static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<AbstractCountBadge, int>(nameof(Count),
            coerce: (o, v) => Math.Max(0, v));

    public static readonly StyledProperty<Control?> DecoratedTargetProperty =
        AvaloniaProperty.Register<AbstractCountBadge, Control?>(nameof(DecoratedTarget));

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<AbstractCountBadge, Point>(nameof(Offset));

    public static readonly StyledProperty<int> OverflowCountProperty =
        AvaloniaProperty.Register<AbstractCountBadge, int>(nameof(OverflowCount), 99,
            coerce: (o, v) => Math.Max(0, v));

    public static readonly StyledProperty<bool> IsShowZeroProperty =
        AvaloniaProperty.Register<AbstractCountBadge, bool>(nameof(IsShowZero));

    public static readonly StyledProperty<CountBadgeSize> SizeProperty =
        AvaloniaProperty.Register<AbstractCountBadge, CountBadgeSize>(nameof(Size));

    public static readonly StyledProperty<bool> BadgeIsVisibleProperty =
        AvaloniaProperty.Register<AbstractCountBadge, bool>(nameof(BadgeIsVisible), true);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractCountBadge>();

    public string? BadgeColor
    {
        get => GetValue(BadgeColorProperty);
        set => SetValue(BadgeColorProperty, value);
    }

    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
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

    public int OverflowCount
    {
        get => GetValue(OverflowCountProperty);
        set => SetValue(OverflowCountProperty, value);
    }

    public bool IsShowZero
    {
        get => GetValue(IsShowZeroProperty);
        set => SetValue(IsShowZeroProperty, value);
    }

    public CountBadgeSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
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

    private protected AbstractCountBadgeAdorner? _badgeAdorner;
    private protected AdornerLayer? _adornerLayer;

    static AbstractCountBadge()
    {
        AffectsMeasure<AbstractCountBadge>(DecoratedTargetProperty,
            CountProperty,
            OverflowCountProperty,
            SizeProperty);
        AffectsRender<AbstractCountBadge>(BadgeColorProperty, OffsetProperty);
    }

    public AbstractCountBadge()
    {
        this.ConfigureMotionBindingStyle();
    }

    private protected abstract AbstractCountBadgeAdorner CreateBadgeAdorner();

    private void PrepareAdorner()
    {
        var badgeAdorner = CreateBadgeAdorner();
        if (DecoratedTarget is not null)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            // 这里需要抛出异常吗？
            if (_adornerLayer == null)
            {
                return;
            }

            badgeAdorner.ApplyToTarget(_adornerLayer, this);
        }
        else
        {
            IsVisible = true;
            badgeAdorner.ApplyToTarget(null, this);
        }
    }

    private void HideAdorner(bool enableMotion)
    {
        // 这里需要抛出异常吗？
        if (_badgeAdorner is null)
        {
            return;
        }

        _badgeAdorner.DetachFromTargetAsync(_adornerLayer, enableMotion);
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
        if (BadgeIsVisible)
        {
            PrepareAdorner();
        }
        if (DecoratedTarget is null)
        {
            CreateBadgeAdorner();
        }

        SetupShowZero();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HideAdorner(false);
    }

    private protected virtual void SetupTokenBindings()
    {
        if (_badgeAdorner is not null)
        {
            _badgeAdorner[!AbstractCountBadgeAdorner.OffsetProperty]        = this[!OffsetProperty];
            _badgeAdorner[!AbstractCountBadgeAdorner.SizeProperty]          = this[!SizeProperty];
            _badgeAdorner[!AbstractCountBadgeAdorner.OverflowCountProperty] = this[!OverflowCountProperty];
            _badgeAdorner[!AbstractCountBadgeAdorner.CountProperty]         = this[!CountProperty];
            _badgeAdorner[!AbstractCountBadgeAdorner.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
        }
    }

    private protected virtual void HandleDecoratedTargetChanged()
    {
        if (_badgeAdorner is not null)
        {
            if (DecoratedTarget is null)
            {
                _badgeAdorner.SetLogicalParent(this);
                VisualChildren.Add(_badgeAdorner);
                LogicalChildren.Add(_badgeAdorner);
                _badgeAdorner.IsAdornerMode = false;
            }
            else if (DecoratedTarget is not null)
            {
                _badgeAdorner.IsAdornerMode = true;
                DecoratedTarget.SetLogicalParent(this);
                VisualChildren.Add(DecoratedTarget);
                LogicalChildren.Add(DecoratedTarget);
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BadgeIsVisibleProperty)
        {
            if (BadgeIsVisible)
            {
                SetupShowZero();
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

            if (change.Property == BadgeColorProperty)
            {
                ConfigureDotColor(change.GetNewValue<string>());
            }
        }

        if (change.Property == CountProperty ||
            change.Property == IsShowZeroProperty)
        {
            SetupShowZero();
        }
    }

    private void SetupShowZero()
    {
        if (Count == 0 && !IsShowZero)
        {
            BadgeIsVisible = false;
        }
        else if (Count > 0)
        {
            BadgeIsVisible = true;
        }
    }
    
    private protected virtual void ConfigureDotColor(string colorStr)
    {
        _badgeAdorner!.BadgeColor = BadgeColorUtils.CalculateColor(colorStr);
    }
}