using AtomUI.Controls.Badge;
using AtomUI.Controls.Themes;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class DotBadgeAdorner : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<IBrush?> BadgeDotColorProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, IBrush?>(
            nameof(BadgeDotColor));

    public static readonly DirectProperty<DotBadgeAdorner, DotBadgeStatus?> StatusProperty =
        AvaloniaProperty.RegisterDirect<DotBadgeAdorner, DotBadgeStatus?>(
            nameof(Status),
            o => o.Status,
            (o, v) => o.Status = v);
    
    public static readonly DirectProperty<DotBadgeAdorner, string?> TextProperty =
        AvaloniaProperty.RegisterDirect<DotBadgeAdorner, string?>(
            nameof(Text),
            o => o.Text,
            (o, v) => o.Text = v);
    
    public static readonly DirectProperty<DotBadgeAdorner, bool> IsAdornerModeProperty =
        AvaloniaProperty.RegisterDirect<DotBadgeAdorner, bool>(
            nameof(IsAdornerMode),
            o => o.IsAdornerMode,
            (o, v) => o.IsAdornerMode = v);
    
    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, Point>(
            nameof(Offset));

    public IBrush? BadgeDotColor
    {
        get => GetValue(BadgeDotColorProperty);
        set => SetValue(BadgeDotColorProperty, value);
    }

    private DotBadgeStatus? _status;

    public DotBadgeStatus? Status
    {
        get => _status;
        set => SetAndRaise(StatusProperty, ref _status, value);
    }

    private string? _text;

    public string? Text
    {
        get => _text;
        set => SetAndRaise(TextProperty, ref _text, value);
    }

    private bool _isAdornerMode;

    public bool IsAdornerMode
    {
        get => _isAdornerMode;
        set => SetAndRaise(IsAdornerModeProperty, ref _isAdornerMode, value);
    }

    public Point Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    #endregion
   
    #region 内部属性定义

    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<DotBadgeAdorner>();
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DotBadgeAdorner>();
    
    internal TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    private BaseMotionActor? _indicatorMotionActor;
    private CancellationTokenSource? _motionCancellationTokenSource;

    static DotBadgeAdorner()
    {
        AffectsMeasure<DotBadge>(TextProperty, IsAdornerModeProperty);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _indicatorMotionActor = e.NameScope.Get<BaseMotionActor>(DotBadgeAdornerThemeConstants.IndicatorMotionActorPart);
    }

    private void ApplyShowMotion()
    {
        if (_indicatorMotionActor is not null)
        {
            _indicatorMotionActor.IsVisible = false;
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource = new CancellationTokenSource();
            var motion = new BadgeZoomBadgeInMotion(MotionDuration, null, FillMode.Forward);
            motion.Run(_indicatorMotionActor, () => _indicatorMotionActor.IsVisible = true);
        }
    }

    private void ApplyHideMotion(Action completedAction)
    {
        if (_indicatorMotionActor is not null)
        {
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource = new CancellationTokenSource();
            var motion = new BadgeZoomBadgeOutMotion(MotionDuration, null, FillMode.Forward);
            motion.Run(_indicatorMotionActor, null, completedAction);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (IsAdornerMode && _indicatorMotionActor is not null)
        {
            var offsetX = Offset.X;
            var offsetY = Offset.Y;
            var dotSize = _indicatorMotionActor.Bounds.Size;
            offsetX += dotSize.Width / 3;
            offsetY -= dotSize.Height / 3;
            _indicatorMotionActor.Arrange(new Rect(new Point(offsetX, offsetY), dotSize));
        }

        return size;
    }

    internal void ApplyToTarget(AdornerLayer? adornerLayer, Control adorned)
    {
        if (adornerLayer is null)
        {
            return;
        }

        adornerLayer.Children.Remove(this);

        AdornerLayer.SetAdornedElement(this, adorned);
        AdornerLayer.SetIsClipEnabled(this, false);
        adornerLayer.Children.Add(this);
        
        if (IsMotionEnabled)
        {
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource = new CancellationTokenSource();

            ApplyShowMotion();
        }
        else
        {
            if (_indicatorMotionActor is not null)
            {
                _indicatorMotionActor.IsVisible = true;
            }
        }
    }

    internal void DetachFromTarget(AdornerLayer? adornerLayer, bool enableMotion = true)
    {
        if (adornerLayer is null)
        {
            return;
        }

        if (enableMotion)
        {
            ApplyHideMotion(() => adornerLayer.Children.Remove(this));
        }
        else
        {
            adornerLayer.Children.Remove(this);
        }
    }
}