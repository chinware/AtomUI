using AtomUI.Controls;
using AtomUI.Desktop.Controls.Badge;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class CountBadgeAdorner : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<IBrush?> BadgeColorProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, IBrush?>(
            nameof(BadgeColor));

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, Point>(
            nameof(Offset));

    public static readonly StyledProperty<int> OverflowCountProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, int>(nameof(OverflowCount));

    public static readonly StyledProperty<CountBadgeSize> SizeProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, CountBadgeSize>(
            nameof(Size));
    
    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<CountBadgeAdorner>();

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

    internal static readonly DirectProperty<CountBadgeAdorner, bool> IsAdornerModeProperty =
        AvaloniaProperty.RegisterDirect<CountBadgeAdorner, bool>(
            nameof(IsAdornerMode),
            o => o.IsAdornerMode,
            (o, v) => o.IsAdornerMode = v);

    internal static readonly DirectProperty<CountBadgeAdorner, BoxShadows> BoxShadowProperty =
        AvaloniaProperty.RegisterDirect<CountBadgeAdorner, BoxShadows>(
            nameof(BoxShadow),
            o => o.BoxShadow,
            (o, v) => o.BoxShadow = v);

    internal static readonly DirectProperty<CountBadgeAdorner, string?> CountTextProperty =
        AvaloniaProperty.RegisterDirect<CountBadgeAdorner, string?>(
            nameof(CountText),
            o => o.CountText,
            (o, v) => o.CountText = v);

    internal static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, int>(
            nameof(Count));

    internal static readonly StyledProperty<IBrush?> BadgeShadowColorProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, IBrush?>(
            nameof(BadgeShadowColor));

    internal static readonly StyledProperty<double> BadgeShadowSizeProperty =
        AvaloniaProperty.Register<CountBadgeAdorner, double>(
            nameof(BadgeShadowSize));

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CountBadgeAdorner>();

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
    private bool _needInitialHide;

    static CountBadgeAdorner()
    {
        AffectsMeasure<CountBadgeAdorner>(OverflowCountProperty,
            SizeProperty,
            CountTextProperty,
            IsAdornerModeProperty,
            CornerRadiusProperty);
        AffectsRender<CountBadgeAdorner>(BadgeColorProperty, OffsetProperty);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicatorMotionActor = e.NameScope.Get<BaseMotionActor>(CountBadgeAdornerThemeConstants.IndicatorMotionActorPart);
        if (_needInitialHide)
        {
            _indicatorMotionActor.IsVisible = false;
            _needInitialHide                = false;
        }
        BuildBoxShadow();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        BuildCountText();
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
        if (this.IsAttachedToVisualTree())
        {
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

    private void ApplyShowMotion()
    {
        if (_indicatorMotionActor is not null)
        {
            _indicatorMotionActor.IsVisible = false;
            var motion = new BadgeZoomBadgeInMotion(MotionDuration);
            motion.Run(_indicatorMotionActor, () => { _indicatorMotionActor.IsVisible = true; });
        }
    }

    private void ApplyHideMotion(Action completedAction)
    {
        if (_indicatorMotionActor is not null)
        {
            var motion = new BadgeZoomBadgeOutMotion(MotionDuration);
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource = new CancellationTokenSource();
            motion.Run(_indicatorMotionActor,  null, completedAction);
        }
        else
        {
            _needInitialHide = true;
        }
    }

    internal void ApplyToTarget(AdornerLayer? adornerLayer, Control adorned)
    {
        if (adornerLayer is not null)
        {
            adornerLayer.Children.Remove(this);

            AdornerLayer.SetAdornedElement(this, adorned);
            AdornerLayer.SetIsClipEnabled(this, false);
            adornerLayer.Children.Add(this);
        }
        
        if (IsMotionEnabled)
        {
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource = new CancellationTokenSource();
            ApplyShowMotion();
        }
        else
        {
            if (_indicatorMotionActor != null)
            {
                _indicatorMotionActor.IsVisible = true;
            }
        }
    }

    internal void DetachFromTarget(AdornerLayer? adornerLayer, bool enableMotion = true)
    {
        if (enableMotion)
        {
            ApplyHideMotion(() =>
            {
                if (adornerLayer is not null)
                {
                    adornerLayer.Children.Remove(this);
                }
            });
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