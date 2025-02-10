using AtomUI.Controls.Badge;
using AtomUI.MotionScene;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class DotBadgeAdorner : TemplatedControl
{
    public static readonly StyledProperty<IBrush?> BadgeDotColorProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, IBrush?>(
            nameof(BadgeDotColor));

    public static readonly DirectProperty<DotBadgeAdorner, DotBadgeStatus?> StatusProperty =
        AvaloniaProperty.RegisterDirect<DotBadgeAdorner, DotBadgeStatus?>(
            nameof(Status),
            o => o.Status,
            (o, v) => o.Status = v);

    internal IBrush? BadgeDotColor
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

    public static readonly DirectProperty<DotBadgeAdorner, string?> TextProperty =
        AvaloniaProperty.RegisterDirect<DotBadgeAdorner, string?>(
            nameof(Text),
            o => o.Text,
            (o, v) => o.Text = v);

    private string? _text;

    public string? Text
    {
        get => _text;
        set => SetAndRaise(TextProperty, ref _text, value);
    }

    public static readonly DirectProperty<DotBadgeAdorner, bool> IsAdornerModeProperty =
        AvaloniaProperty.RegisterDirect<DotBadgeAdorner, bool>(
            nameof(IsAdornerMode),
            o => o.IsAdornerMode,
            (o, v) => o.IsAdornerMode = v);

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, Point>(
            nameof(Offset));

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

    #region 内部属性定义

    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, TimeSpan>(
            nameof(MotionDuration));

    internal TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    #endregion

    private MotionActorControl? _indicatorMotionActor;
    private CancellationTokenSource? _motionCancellationTokenSource;

    static DotBadgeAdorner()
    {
        AffectsMeasure<DotBadge>(TextProperty, IsAdornerModeProperty);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, SharedTokenKey.MotionDurationMid);
        SetupBadgeColor();
        _indicatorMotionActor = e.NameScope.Get<MotionActorControl>(DotBadgeAdornerTheme.IndicatorMotionActorPart);
    }

    private void ApplyShowMotion()
    {
        if (_indicatorMotionActor is not null)
        {
            _indicatorMotionActor.IsVisible = false;
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource = new CancellationTokenSource();
            var motion = new BadgeZoomBadgeInMotion(MotionDuration, null, FillMode.Forward);
            MotionInvoker.Invoke(_indicatorMotionActor, motion, () => _indicatorMotionActor.IsVisible = true,
                null, _motionCancellationTokenSource.Token);
        }
    }

    private void ApplyHideMotion(Action completedAction)
    {
        if (_indicatorMotionActor is not null)
        {
            _motionCancellationTokenSource?.Cancel();
            _motionCancellationTokenSource = new CancellationTokenSource();
            var motion = new BadgeZoomBadgeOutMotion(MotionDuration, null, FillMode.Forward);
            MotionInvoker.Invoke(_indicatorMotionActor, motion, null, () => completedAction(),
                _motionCancellationTokenSource.Token);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (VisualRoot is not null)
        {
            if (change.Property == StatusProperty)
            {
                SetupBadgeColor();
            }
        }
    }

    private void SetupBadgeColor()
    {
        if (Status is not null)
        {
            if (Status == DotBadgeStatus.Error)
            {
                TokenResourceBinder.CreateTokenBinding(this, BadgeDotColorProperty,
                    SharedTokenKey.ColorError);
            }
            else if (Status == DotBadgeStatus.Success)
            {
                TokenResourceBinder.CreateTokenBinding(this, BadgeDotColorProperty,
                    SharedTokenKey.ColorSuccess);
            }
            else if (Status == DotBadgeStatus.Warning)
            {
                TokenResourceBinder.CreateTokenBinding(this, BadgeDotColorProperty,
                    SharedTokenKey.ColorWarning);
            }
            else if (Status == DotBadgeStatus.Processing)
            {
                TokenResourceBinder.CreateTokenBinding(this, BadgeDotColorProperty,
                    SharedTokenKey.ColorInfo);
            }
            else if (Status == DotBadgeStatus.Default)
            {
                TokenResourceBinder.CreateTokenBinding(this, BadgeDotColorProperty,
                    SharedTokenKey.ColorTextPlaceholder);
            }
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

        _motionCancellationTokenSource?.Cancel();
        _motionCancellationTokenSource = new CancellationTokenSource();

        ApplyShowMotion();
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