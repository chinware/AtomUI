using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Controls.Commons;

[TemplatePart("PART_MotionActor", typeof(BaseMotionActor))]
internal abstract class AbstractCountBadgeAdorner : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<IBrush?> BadgeColorProperty =
        AvaloniaProperty.Register<AbstractCountBadgeAdorner, IBrush?>(
            nameof(BadgeColor));

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<AbstractCountBadgeAdorner, Point>(
            nameof(Offset));

    public static readonly StyledProperty<int> OverflowCountProperty =
        AvaloniaProperty.Register<AbstractCountBadgeAdorner, int>(nameof(OverflowCount));

    public static readonly StyledProperty<CountBadgeSize> SizeProperty =
        AvaloniaProperty.Register<AbstractCountBadgeAdorner, CountBadgeSize>(
            nameof(Size));
    
    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<AbstractCountBadgeAdorner>();

    public IBrush? BadgeColor
    {
        get => GetValue(BadgeColorProperty);
        set => SetValue(BadgeColorProperty, value);
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

    public CountBadgeSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }
    
    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<AbstractCountBadgeAdorner, bool> IsAdornerModeProperty =
        AvaloniaProperty.RegisterDirect<AbstractCountBadgeAdorner, bool>(
            nameof(IsAdornerMode),
            o => o.IsAdornerMode,
            (o, v) => o.IsAdornerMode = v);

    internal static readonly DirectProperty<AbstractCountBadgeAdorner, BoxShadows> BoxShadowProperty =
        AvaloniaProperty.RegisterDirect<AbstractCountBadgeAdorner, BoxShadows>(
            nameof(BoxShadow),
            o => o.BoxShadow,
            (o, v) => o.BoxShadow = v);

    internal static readonly DirectProperty<AbstractCountBadgeAdorner, string?> CountTextProperty =
        AvaloniaProperty.RegisterDirect<AbstractCountBadgeAdorner, string?>(
            nameof(CountText),
            o => o.CountText,
            (o, v) => o.CountText = v);

    internal static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<AbstractCountBadgeAdorner, int>(
            nameof(Count));

    internal static readonly StyledProperty<IBrush?> BadgeShadowColorProperty =
        AvaloniaProperty.Register<AbstractCountBadgeAdorner, IBrush?>(
            nameof(BadgeShadowColor));

    internal static readonly StyledProperty<double> BadgeShadowSizeProperty =
        AvaloniaProperty.Register<AbstractCountBadgeAdorner, double>(
            nameof(BadgeShadowSize));

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractCountBadgeAdorner>();

    private bool _isAdornerMode;

    internal bool IsAdornerMode
    {
        get => _isAdornerMode;
        set => SetAndRaise(IsAdornerModeProperty, ref _isAdornerMode, value);
    }

    private BoxShadows _boxShadow;

    internal BoxShadows BoxShadow
    {
        get => _boxShadow;
        set => SetAndRaise(BoxShadowProperty, ref _boxShadow, value);
    }

    private string? _countText;

    internal string? CountText
    {
        get => _countText;
        set => SetAndRaise(CountTextProperty, ref _countText, value);
    }

    internal IBrush? BadgeShadowColor
    {
        get => GetValue(BadgeShadowColorProperty);
        set => SetValue(BadgeShadowColorProperty, value);
    }

    internal double BadgeShadowSize
    {
        get => GetValue(BadgeShadowSizeProperty);
        set => SetValue(BadgeShadowSizeProperty, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    private BaseMotionActor? _indicatorMotionActor;
    private CancellationTokenSource? _motionCancellationTokenSource;
    private Action? _pendingTemplateAction;

    static AbstractCountBadgeAdorner()
    {
        AffectsMeasure<AbstractCountBadgeAdorner>(OverflowCountProperty,
            SizeProperty,
            CountTextProperty,
            IsAdornerModeProperty,
            CornerRadiusProperty);
        AffectsRender<AbstractCountBadgeAdorner>(BadgeColorProperty, OffsetProperty);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicatorMotionActor = e.NameScope.Get<BaseMotionActor>(BaseMotionActor.MotionActorPart);
        _pendingTemplateAction?.Invoke();
        _pendingTemplateAction = null;
        BuildBoxShadow();
        BuildCountText();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _motionCancellationTokenSource?.Cancel();
        _motionCancellationTokenSource?.Dispose();
        _motionCancellationTokenSource = null;
    }

    private void BuildBoxShadow()
    {
        if (BadgeShadowColor is not null)
        {
            BoxShadow = new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 0,
                Blur    = 0,
                Spread  = BadgeShadowSize,
                Color   = ((ISolidColorBrush)BadgeShadowColor).Color
            });
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BadgeShadowSizeProperty ||
            change.Property == BadgeShadowColorProperty)
        {
            BuildBoxShadow();
        }

        if (change.Property == CountProperty || change.Property == OverflowCountProperty)
        {
            BuildCountText();
        }
    }

    private void BuildCountText()
    {
        CountText = Count > OverflowCount ? $"{OverflowCount}+" : $"{Count}";
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (IsAdornerMode && _indicatorMotionActor is not null)
        {
            var offsetX       = Offset.X;
            var offsetY       = Offset.Y;
            var indicatorSize = _indicatorMotionActor.DesiredSize;
            offsetX += finalSize.Width - indicatorSize.Width / 2;
            offsetY -= indicatorSize.Height / 2;
            _indicatorMotionActor.Arrange(new Rect(new Point(offsetX, offsetY), indicatorSize));
        }

        return size;
    }

    private async Task ApplyShowMotionAsync(Action? onShowReady = null)
    {
        if (_indicatorMotionActor is not null)
        {
            var motion = new BadgeZoomBadgeInMotion(MotionDuration);
            try
            {
                await motion.RunAsync(_indicatorMotionActor,
                    () =>
                    {
                        onShowReady?.Invoke();
                        _indicatorMotionActor.IsVisible = true;
                    },
                    _motionCancellationTokenSource?.Token ?? default);
            }
            catch (OperationCanceledException)
            {
                onShowReady?.Invoke();
                _indicatorMotionActor.Opacity         = 1.0;
                _indicatorMotionActor.MotionTransform = null;
                _indicatorMotionActor.IsVisible       = true;
            }
        }
        else
        {
            onShowReady?.Invoke();
        }
    }

    private async Task ApplyHideMotionAsync(AdornerLayer? adornerLayer, CancellationToken cancellationToken)
    {
        if (_indicatorMotionActor is not null)
        {
            var motion = new BadgeZoomBadgeOutMotion(MotionDuration);
            await motion.RunAsync(_indicatorMotionActor, cancellationToken: cancellationToken);
            adornerLayer?.Children.Remove(this);
        }
        else
        {
            _pendingTemplateAction = () => _indicatorMotionActor!.IsVisible = false;
            adornerLayer?.Children.Remove(this);
        }
    }

    internal void ApplyToTarget(AdornerLayer? adornerLayer, Control adorned, Action? onShowReady = null)
    {
        if (adornerLayer is not null)
        {
            adornerLayer.Children.Remove(this);

            AdornerLayer.SetAdornedElement(this, adorned);
            AdornerLayer.SetIsClipEnabled(this, false);
            adornerLayer.Children.Add(this);
        }

        if (IsMotionEnabled && IsLoaded)
        {
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource?.Dispose();
            _motionCancellationTokenSource = new CancellationTokenSource();
            if (_indicatorMotionActor != null)
            {
                _indicatorMotionActor.MotionTransform = new ScaleTransform(0.01, 0.01);
                Dispatcher.InvokeAsync(() => ApplyShowMotionAsync(onShowReady));
            }
            else
            {
                _pendingTemplateAction = () =>
                {
                    _indicatorMotionActor!.MotionTransform = new ScaleTransform(0.01, 0.01);
                    _motionCancellationTokenSource?.Cancel();
                    _motionCancellationTokenSource?.Dispose();
                    _motionCancellationTokenSource = new CancellationTokenSource();
                    Dispatcher.InvokeAsync(() => ApplyShowMotionAsync(onShowReady));
                };
                onShowReady?.Invoke();
            }
        }
        else
        {
            onShowReady?.Invoke();
            if (_indicatorMotionActor != null)
            {
                _indicatorMotionActor.Opacity         = 1.0;
                _indicatorMotionActor.MotionTransform = null;
                _indicatorMotionActor.IsVisible       = true;
            }
        }
    }

    internal void DetachFromTarget(AdornerLayer? adornerLayer, bool enableMotion = true)
    {
        if (enableMotion)
        {
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource?.Dispose();
            _motionCancellationTokenSource = new CancellationTokenSource();
            var token = _motionCancellationTokenSource.Token;
            Dispatcher.InvokeAsync(() => ApplyHideMotionAsync(adornerLayer, token));
        }
        else
        {
            if (adornerLayer is not null)
            {
                adornerLayer.Children.Remove(this);
            }
        }
    }
}