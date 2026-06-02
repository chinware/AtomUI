using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Controls.Commons;

[TemplatePart("PART_MotionActor", typeof(BaseMotionActor))]
internal abstract class AbstractDotBadgeAdorner : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<IBrush?> BadgeDotColorProperty =
        AvaloniaProperty.Register<AbstractDotBadgeAdorner, IBrush?>(
            nameof(BadgeDotColor));

    public static readonly DirectProperty<AbstractDotBadgeAdorner, DotBadgeStatus?> StatusProperty =
        AvaloniaProperty.RegisterDirect<AbstractDotBadgeAdorner, DotBadgeStatus?>(
            nameof(Status),
            o => o.Status,
            (o, v) => o.Status = v);
    
    public static readonly DirectProperty<AbstractDotBadgeAdorner, string?> TextProperty =
        AvaloniaProperty.RegisterDirect<AbstractDotBadgeAdorner, string?>(
            nameof(Text),
            o => o.Text,
            (o, v) => o.Text = v);
    
    public static readonly DirectProperty<AbstractDotBadgeAdorner, bool> IsAdornerModeProperty =
        AvaloniaProperty.RegisterDirect<AbstractDotBadgeAdorner, bool>(
            nameof(IsAdornerMode),
            o => o.IsAdornerMode,
            (o, v) => o.IsAdornerMode = v);
    
    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<AbstractDotBadgeAdorner, Point>(
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
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<AbstractDotBadgeAdorner>();
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractDotBadgeAdorner>();
    
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
    private Action? _pendingShowReadyAction;
    private bool _pendingShowMotion;
    private bool _showMotionStarted;

    static AbstractDotBadgeAdorner()
    {
        AffectsMeasure<AbstractDotBadgeAdorner>(TextProperty, IsAdornerModeProperty);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicatorMotionActor = e.NameScope.Get<BaseMotionActor>(BaseMotionActor.MotionActorPart);
        PrepareIndicatorForShowMotion();
        TryStartPendingShowMotion();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        TryStartPendingShowMotion();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        CancelCurrentMotion();
        _pendingShowReadyAction = null;
        _pendingShowMotion      = false;
        _showMotionStarted      = false;
    }

    private void CancelCurrentMotion()
    {
        _motionCancellationTokenSource?.Cancel();
        _motionCancellationTokenSource?.Dispose();
        _motionCancellationTokenSource = null;
    }

    private void ResetCurrentMotion()
    {
        CancelCurrentMotion();
        _motionCancellationTokenSource = new CancellationTokenSource();
    }

    private void PrepareIndicatorForShowMotion()
    {
        if (!_pendingShowMotion || _indicatorMotionActor is null)
        {
            return;
        }

        _indicatorMotionActor.Opacity         = 0.0;
        _indicatorMotionActor.MotionTransform = BadgeMotionTransforms.ScaleNearZero;
        _indicatorMotionActor.IsVisible       = true;
    }

    private void ApplyShowReadyAction()
    {
        var action = _pendingShowReadyAction;
        _pendingShowReadyAction = null;
        action?.Invoke();
    }

    private void ShowIndicatorWithoutMotion()
    {
        if (_indicatorMotionActor is null)
        {
            return;
        }

        _indicatorMotionActor.Opacity         = 1.0;
        _indicatorMotionActor.MotionTransform = null;
        _indicatorMotionActor.IsVisible       = true;
    }

    private void RequestShowMotion(Action? onShowReady)
    {
        ResetCurrentMotion();
        _pendingShowReadyAction = onShowReady;
        _pendingShowMotion      = true;
        _showMotionStarted      = false;
        PrepareIndicatorForShowMotion();
        ApplyShowReadyAction();
        TryStartPendingShowMotion();
    }

    private void TryStartPendingShowMotion()
    {
        if (!_pendingShowMotion ||
            _showMotionStarted ||
            !IsLoaded ||
            _indicatorMotionActor is null ||
            _motionCancellationTokenSource is null)
        {
            return;
        }

        _showMotionStarted = true;
        var motionCancellationTokenSource = _motionCancellationTokenSource;
        Dispatcher.UIThread.Post(
            () => _ = RunPendingShowMotionAsync(motionCancellationTokenSource),
            DispatcherPriority.Loaded);
    }

    private async Task RunPendingShowMotionAsync(CancellationTokenSource motionCancellationTokenSource)
    {
        if (_motionCancellationTokenSource != motionCancellationTokenSource ||
            motionCancellationTokenSource.IsCancellationRequested ||
            _indicatorMotionActor is null)
        {
            return;
        }

        try
        {
            await ApplyShowMotionAsync(motionCancellationTokenSource.Token);
        }
        catch (OperationCanceledException) when (motionCancellationTokenSource.IsCancellationRequested)
        {
        }
        finally
        {
            if (_motionCancellationTokenSource == motionCancellationTokenSource)
            {
                _showMotionStarted = false;
                if (!motionCancellationTokenSource.IsCancellationRequested)
                {
                    _pendingShowMotion = false;
                }
            }
        }
    }

    private async Task ApplyShowMotionAsync(CancellationToken cancellationToken)
    {
        if (_indicatorMotionActor is not null)
        {
            var motion = new BadgeZoomBadgeInMotion(MotionDuration, null, FillMode.Forward);
            try
            {
                await motion.RunAsync(_indicatorMotionActor,
                    () => _indicatorMotionActor.IsVisible = true,
                    cancellationToken);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _indicatorMotionActor.Opacity         = 1.0;
                _indicatorMotionActor.MotionTransform = null;
                _indicatorMotionActor.IsVisible       = true;
            }
        }
    }

    private async Task ApplyHideMotionAsync(AdornerLayer? adornerLayer,
                                            Action? onDetachCompleted,
                                            CancellationToken cancellationToken)
    {
        try
        {
            if (_indicatorMotionActor is not null)
            {
                var motion = new BadgeZoomBadgeOutMotion(MotionDuration, null, FillMode.Forward);
                await motion.RunAsync(_indicatorMotionActor, cancellationToken: cancellationToken);
                _indicatorMotionActor.IsVisible = false;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        adornerLayer?.Children.Remove(this);
        onDetachCompleted?.Invoke();
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

    internal void ApplyToTarget(AdornerLayer? adornerLayer, Control adorned, Action? onShowReady = null)
    {
        if (adornerLayer is not null)
        {
            adornerLayer.Children.Remove(this);

            AdornerLayer.SetAdornedElement(this, adorned);
            AdornerLayer.SetIsClipEnabled(this, true);
            adornerLayer.Children.Add(this);
        }
        
        if (IsMotionEnabled)
        {
            RequestShowMotion(onShowReady);
        }
        else
        {
            onShowReady?.Invoke();
            _pendingShowMotion      = false;
            _showMotionStarted      = false;
            _pendingShowReadyAction = null;
            ShowIndicatorWithoutMotion();
        }
    }

    internal void DetachFromTargetAsync(AdornerLayer? adornerLayer,
                                        bool enableMotion = true,
                                        Action? onDetachCompleted = null)
    {
        _pendingShowMotion      = false;
        _showMotionStarted      = false;
        _pendingShowReadyAction = null;
        if (enableMotion)
        {
            ResetCurrentMotion();
            var token = _motionCancellationTokenSource!.Token;
            Dispatcher.InvokeAsync(() => ApplyHideMotionAsync(adornerLayer, onDetachCompleted, token));
        }
        else
        {
            CancelCurrentMotion();
            adornerLayer?.Children.Remove(this);
            onDetachCompleted?.Invoke();
        }
    }
}
