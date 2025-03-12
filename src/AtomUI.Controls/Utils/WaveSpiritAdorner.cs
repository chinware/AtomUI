using System.Diagnostics;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Controls.Utils;

internal class WaveSpiritAdorner : Control, IDisposable
{
    public static Dictionary<Control, WaveSpiritAdorner> _adornerCache;
    public bool IsPlaying { get; private set; }

    private double _lastWaveOpacity;
    private Size _lastWaveSize;
    private double _lastWaveRadiusSize;
    private Control? _targetControl;
    private CancellationTokenSource? _cancellationTokenSource;

    public event EventHandler? PlayFinished;

    static WaveSpiritAdorner()
    {
        AffectsRender<WaveSpiritAdorner>(
            LastWaveSizeProperty,
            LastWaveRadiusProperty,
            LastWaveOpacityProperty);
        _adornerCache = new Dictionary<Control, WaveSpiritAdorner>();
    }

    public TimeSpan SizeMotionDuration
    {
        get => _wavePainter.SizeMotionDuration;
        set => _wavePainter.SizeMotionDuration = value;
    }

    public TimeSpan OpacityMotionDuration
    {
        get => _wavePainter.OpacityMotionDuration;
        set => _wavePainter.OpacityMotionDuration = value;
    }

    public Easing SizeEasingCurve
    {
        get => _wavePainter.SizeEasingCurve;
        set => _wavePainter.SizeEasingCurve = value;
    }

    public Easing OpacityEasingCurve
    {
        get => _wavePainter.OpacityEasingCurve;
        set => _wavePainter.OpacityEasingCurve = value;
    }

    public double OriginOpacity
    {
        get => _wavePainter.OriginOpacity;
        set => _wavePainter.OriginOpacity = value;
    }

    public double WaveRange
    {
        get => _wavePainter.WaveRange;
        set => _wavePainter.WaveRange = value;
    }

    public Color WaveColor
    {
        get => _wavePainter.WaveColor;
        set => _wavePainter.WaveColor = value;
    }

    protected static readonly DirectProperty<WaveSpiritAdorner, double> LastWaveOpacityProperty
        = AvaloniaProperty.RegisterDirect<WaveSpiritAdorner, double>(
            nameof(LastWaveOpacity),
            o => o.LastWaveOpacity,
            (o, v) => o.LastWaveOpacity = v);

    protected double LastWaveOpacity
    {
        get => _lastWaveOpacity;
        set => SetAndRaise(LastWaveOpacityProperty, ref _lastWaveOpacity, value);
    }

    protected static readonly DirectProperty<WaveSpiritAdorner, Size> LastWaveSizeProperty
        = AvaloniaProperty.RegisterDirect<WaveSpiritAdorner, Size>(
            nameof(LastWaveSize),
            o => o.LastWaveSize,
            (o, v) => o.LastWaveSize = v);

    protected Size LastWaveSize
    {
        get => _lastWaveSize;
        set => SetAndRaise(LastWaveSizeProperty, ref _lastWaveSize, value);
    }

    protected static readonly DirectProperty<WaveSpiritAdorner, double> LastWaveRadiusProperty
        = AvaloniaProperty.RegisterDirect<WaveSpiritAdorner, double>(
            nameof(LastWaveRadius),
            o => o.LastWaveRadius,
            (o, v) => o.LastWaveRadius = v);

    protected double LastWaveRadius
    {
        get => _lastWaveRadiusSize;
        set => SetAndRaise(LastWaveRadiusProperty, ref _lastWaveRadiusSize, value);
    }

    private readonly AbstractWavePainter _wavePainter;

    public WaveSpiritAdorner(Control targetControl, WaveType waveType, Color? waveColor = null)
    {
        _targetControl                 =  targetControl;
        _targetControl.PropertyChanged += HandleTargetControlChanged;
        var sharedToken  = TokenFinderUtils.FindSharedToken(_targetControl!);
        var themeVariant = TokenFinderUtils.FindThemeVariant(_targetControl);

        CornerRadius? borderRadius = null;
        // 获取相关值
        {
            if (_targetControl.TryGetResource(SharedTokenKey.BorderRadius, themeVariant, out var value))
            {
                if (value is CornerRadius cornerRadius)
                {
                    borderRadius = cornerRadius;
                }
            }
        }

        borderRadius ??= sharedToken.BorderRadius;

        TimeSpan? motionDurationSlow = null;
        {
            if (_targetControl.TryGetResource(SharedTokenKey.MotionDurationSlow, themeVariant, out var value))
            {
                if (value is TimeSpan timeSpan)
                {
                    motionDurationSlow = timeSpan;
                }
            }
        }
        motionDurationSlow ??= sharedToken.MotionDurationSlow;

        double? waveStartOpacity = null;
        {
            if (_targetControl.TryGetResource(SharedTokenKey.WaveStartOpacity, themeVariant, out var value))
            {
                if (value is double dvalue)
                {
                    waveStartOpacity = dvalue;
                }
            }
        }
        waveStartOpacity ??= sharedToken.WaveStartOpacity;

        double? waveAnimationRange = null;
        {
            if (_targetControl.TryGetResource(SharedTokenKey.WaveAnimationRange, themeVariant, out var value))
            {
                if (value is double dvalue)
                {
                    waveAnimationRange = dvalue;
                }
            }
        }
        waveAnimationRange ??= sharedToken.WaveAnimationRange;

        if (waveType == WaveType.CircleWave)
        {
            _wavePainter = new CircleWavePainter();
        }
        else if (waveType == WaveType.PillWave)
        {
            _wavePainter = new PillWavePainter();
        }
        else
        {
            var roundWavePainter = new RoundRectWavePainter();
            roundWavePainter.CornerRadius = borderRadius.Value;
            _wavePainter                  = roundWavePainter;
        }

        _wavePainter.SizeEasingCurve       = new CubicEaseOut();
        _wavePainter.OpacityEasingCurve    = new CubicEaseOut();
        _wavePainter.OriginOpacity         = Math.Clamp(waveStartOpacity.Value, 0.0, 1.0);
        _wavePainter.SizeMotionDuration    = motionDurationSlow.Value;
        _wavePainter.OpacityMotionDuration = motionDurationSlow.Value.Add(TimeSpan.FromMilliseconds(50));
        _wavePainter.WaveRange             = Math.Min(waveAnimationRange.Value, 8);
        if (waveColor is not null)
        {
            _wavePainter.WaveColor = waveColor.Value;
        }
        else
        {
            Color? colorPrimary = null;
            {
                if (_targetControl.TryGetResource(SharedTokenKey.ColorPrimary, themeVariant, out var value))
                {
                    if (value is Color color)
                    {
                        colorPrimary = color;
                    }
                }
            }
            colorPrimary           ??= sharedToken.ColorPrimary;
            _wavePainter.WaveColor =   colorPrimary.Value;
        }

        LayoutUpdated += HandleLayoutUpdated;
    }

    public WaveType WaveType => _wavePainter.WaveType;

    private void HandleTargetControlChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == BoundsProperty)
        {
            AdjustWaveAdorner();
        }
    }

    private void AdjustWaveAdorner()
    {
        // 必须要成功
        var waveGeometryProvider = _targetControl as IWaveAdornerInfoProvider;
        Debug.Assert(waveGeometryProvider != null);
        var waveGeometry = waveGeometryProvider.WaveGeometry();
        // 设置 painter
        if (_wavePainter is RoundRectWavePainter roundRectWavePainter)
        {
            roundRectWavePainter.OriginPoint  = waveGeometry.TopLeft;
            roundRectWavePainter.OriginSize   = waveGeometry.Size;
            roundRectWavePainter.CornerRadius = waveGeometryProvider.WaveBorderRadius();
        }
        else if (_wavePainter is PillWavePainter pillWavePainter)
        {
            pillWavePainter.OriginPoint = waveGeometry.TopLeft;
            pillWavePainter.OriginSize  = waveGeometry.Size;
        }
        else if (_wavePainter is CircleWavePainter circleWavePainter)
        {
            circleWavePainter.OriginPoint  = waveGeometry.Center;
            circleWavePainter.OriginRadius = waveGeometry.Width / 2;
        }
    }

    public sealed override void Render(DrawingContext context)
    {
        // TODO 有时候会被合成器触发渲染
        if (!IsPlaying)
        {
            return;
        }

        object currentSize;
        if (_wavePainter.WaveType == WaveType.CircleWave)
        {
            currentSize = LastWaveRadius;
        }
        else
        {
            currentSize = LastWaveSize;
        }

        _wavePainter.Paint(context, currentSize, LastWaveOpacity);
    }

    protected override void ArrangeCore(Rect finalRect)
    {
        base.ArrangeCore(finalRect);
        AdjustWaveAdorner();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _cancellationTokenSource?.Cancel();
        IsPlaying = false;
    }

    private void HandleLayoutUpdated(object? sender, EventArgs args)
    {
        RunWaveAnimation();
    }

    private void RunWaveAnimation()
    {
        if (IsPlaying)
        {
            return;
        }

        var              sizeAnimation    = new Animation();
        var              opacityAnimation = new Animation();
        AvaloniaProperty targetProperty;
        if (_wavePainter.WaveType == WaveType.CircleWave)
        {
            targetProperty = LastWaveRadiusProperty;
        }
        else
        {
            targetProperty = LastWaveSizeProperty;
        }

        _wavePainter.NotifyBuildSizeAnimation(sizeAnimation, targetProperty);
        _wavePainter.NotifyBuildOpacityAnimation(opacityAnimation, LastWaveOpacityProperty);

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        var sizeAnimationTask    = sizeAnimation.RunAsync(this, _cancellationTokenSource.Token);
        var opacityAnimationTask = opacityAnimation.RunAsync(this, _cancellationTokenSource.Token);
        IsPlaying = true;
        var adorner = this;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await Task.WhenAll(sizeAnimationTask, opacityAnimationTask);
            IsPlaying = false;
            PlayFinished?.Invoke(adorner, EventArgs.Empty);
        });
    }

    public static void ShowWaveAdorner(Control target, WaveType waveType, Color? waveColor = null)
    {
        if (_adornerCache.TryGetValue(target, out var adorner))
        {
            // 回调会清除项
            return;
        }

        var adornerLayer = AdornerLayer.GetAdornerLayer(target);
        if (adornerLayer == null)
        {
            return;
        }

        adorner = new WaveSpiritAdorner(target, waveType, waveColor);

        AdornerLayer.SetAdornedElement(adorner, target);
        AdornerLayer.SetIsClipEnabled(adorner, false);
        adornerLayer.Children.Add(adorner);

        // TODO 如果动画没完成，target 就被删除了呢？
        _adornerCache[target] = adorner;

        adorner.PlayFinished += (sender, args) =>
        {
            adornerLayer.Children.Remove(adorner);
            _adornerCache.Remove(target);
            adorner.Dispose();
        };
    }

    public void Dispose()
    {
        if (_targetControl is not null)
        {
            _targetControl.PropertyChanged -= HandleTargetControlChanged;
        }
    }
}