using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.Threading;
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
    private const int MaxAdornerLayerRetryCount = 30;
    private bool _adornerLayerRetryScheduled;
    private int _adornerLayerRetryCount;

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
        var dotBadgeAdorner = CreateDotBadgeAdorner();
        if (DecoratedTarget is not null)
        {
            DetachChild(dotBadgeAdorner);
            AttachChild(DecoratedTarget);
            dotBadgeAdorner.IsAdornerMode = true;
            _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            // 这里需要抛出异常吗？
            if (_adornerLayer == null)
            {
                ScheduleAdornerLayerRetry();
                return;
            }

            _adornerLayerRetryCount     = 0;
            _adornerLayerRetryScheduled = false;
            dotBadgeAdorner.ApplyToTarget(_adornerLayer, this);
        } 
        else
        {
            DetachLayerAdorner();
            dotBadgeAdorner.IsAdornerMode = false;
            AttachChild(dotBadgeAdorner);
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

        if (DecoratedTarget is null)
        {
            if (enableMotion)
            {
                var dotBadgeAdorner = _dotBadgeAdorner;
                dotBadgeAdorner.DetachFromTargetAsync(null,
                    true,
                    () =>
                    {
                        if (_dotBadgeAdorner == dotBadgeAdorner)
                        {
                            DetachChild(dotBadgeAdorner);
                            IsVisible = false;
                        }
                    });
            }
            else
            {
                DetachChild(_dotBadgeAdorner);
                _dotBadgeAdorner.DetachFromTargetAsync(null, false);
            }
        }
        else
        {
            if (enableMotion)
            {
                var dotBadgeAdorner = _dotBadgeAdorner;
                var adornerLayer    = _adornerLayer;
                if (adornerLayer is null)
                {
                    dotBadgeAdorner.DetachFromTargetAsync(null,
                        true,
                        () =>
                        {
                            if (_dotBadgeAdorner == dotBadgeAdorner)
                            {
                                DetachChild(dotBadgeAdorner);
                            }
                        });
                }
                else
                {
                    dotBadgeAdorner.DetachFromTargetAsync(adornerLayer,
                        true,
                        () =>
                        {
                            if (_adornerLayer == adornerLayer)
                            {
                                _adornerLayer = null;
                            }
                        });
                }
            }
            else
            {
                DetachChild(_dotBadgeAdorner);
                _dotBadgeAdorner.DetachFromTargetAsync(_adornerLayer, false);
                _adornerLayer = null;
            }
        }

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
        _adornerLayerRetryCount = 0;
        if (BadgeIsVisible)
        {
            PrepareAdorner();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Loaded -= HandleAdornerLayerRetryLoaded;
        _adornerLayerRetryScheduled = false;
        _adornerLayerRetryCount     = 0;
        HideAdorner(false);
    }

    private void ScheduleAdornerLayerRetry()
    {
        if (_adornerLayerRetryScheduled ||
            !this.IsAttachedToVisualTree() ||
            _adornerLayerRetryCount >= MaxAdornerLayerRetryCount)
        {
            return;
        }

        _adornerLayerRetryScheduled = true;
        if (IsLoaded)
        {
            if (_adornerLayerRetryCount == 0)
            {
                Dispatcher.UIThread.Post(RetryPrepareAdorner, DispatcherPriority.Loaded);
            }
            else
            {
                DispatcherTimer.RunOnce(RetryPrepareAdorner, TimeSpan.FromMilliseconds(16));
            }
        }
        else
        {
            Loaded += HandleAdornerLayerRetryLoaded;
        }
    }

    private void HandleAdornerLayerRetryLoaded(object? sender, RoutedEventArgs e)
    {
        Loaded -= HandleAdornerLayerRetryLoaded;
        Dispatcher.UIThread.Post(RetryPrepareAdorner, DispatcherPriority.Loaded);
    }

    private void RetryPrepareAdorner()
    {
        if (!_adornerLayerRetryScheduled)
        {
            return;
        }

        _adornerLayerRetryScheduled = false;
        if (!this.IsAttachedToVisualTree() ||
            !BadgeIsVisible ||
            DecoratedTarget is null)
        {
            return;
        }

        _adornerLayerRetryCount++;
        var adornerLayer = AdornerLayer.GetAdornerLayer(this);
        if (adornerLayer is null)
        {
            ScheduleAdornerLayerRetry();
            return;
        }

        PrepareAdorner();
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
        if (_dotBadgeAdorner is null)
        {
            return;
        }

        _dotBadgeAdorner.DetachFromTargetAsync(_adornerLayer, false);
        _adornerLayer = null;
    }

    private void ResetDotBadgeAdorner()
    {
        if (_dotBadgeAdorner is null)
        {
            return;
        }

        DetachChild(_dotBadgeAdorner);
        DetachLayerAdorner();
        _dotBadgeAdorner = null;
    }

    private protected virtual void NotifyDecoratedTargetChanged(Control? oldDecoratedTarget = null)
    {
        if (_dotBadgeAdorner is not null)
        {
            DetachChild(oldDecoratedTarget);
            if (DecoratedTarget is null)
            {
                _dotBadgeAdorner.IsAdornerMode = false;
                DetachLayerAdorner();
                if (BadgeIsVisible)
                {
                    AttachChild(_dotBadgeAdorner);
                }
            }
            else
            {
                DetachChild(_dotBadgeAdorner);
                _dotBadgeAdorner.IsAdornerMode = true;
                AttachChild(DecoratedTarget);
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
                var oldDecoratedTarget = change.GetOldValue<Control?>();
                DetachChild(oldDecoratedTarget);
                if ((oldDecoratedTarget is null) != (DecoratedTarget is null))
                {
                    ResetDotBadgeAdorner();
                }
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

    private protected virtual void ConfigureDotColor(string colorStr)
    {
        _dotBadgeAdorner!.BadgeDotColor = BadgeColorUtils.CalculateColor(colorStr);
    }
    
}
