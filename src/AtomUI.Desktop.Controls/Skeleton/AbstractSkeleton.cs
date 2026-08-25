using AtomUI.Controls;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public abstract class AbstractSkeleton : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<AbstractSkeleton, bool>(nameof(IsActive));
    
    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<AbstractSkeleton>();

    public static readonly StyledProperty<Easing?> MotionEasingCurveProperty =
        AvaloniaProperty.Register<AbstractSkeleton, Easing?>(nameof(MotionEasingCurve));
    
    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }
    
    public Easing? MotionEasingCurve
    {
        get => GetValue(MotionEasingCurveProperty);
        set => SetValue(MotionEasingCurveProperty, value);
    }

    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<IBrush?> LoadingBackgroundStartProperty =
        AvaloniaProperty.Register<AbstractSkeleton, IBrush?>(nameof(LoadingBackgroundStart));
    
    internal static readonly StyledProperty<IBrush?> LoadingBackgroundMiddleProperty =
        AvaloniaProperty.Register<AbstractSkeleton, IBrush?>(nameof(LoadingBackgroundMiddle));
    
    internal static readonly StyledProperty<IBrush?> LoadingBackgroundEndProperty =
        AvaloniaProperty.Register<AbstractSkeleton, IBrush?>(nameof(LoadingBackgroundEnd));
    
    internal static readonly StyledProperty<IBrush?> AnimationLayerFillProperty =
        AvaloniaProperty.Register<AbstractSkeleton, IBrush?>(nameof(AnimationLayerFill));
    
    internal IBrush? LoadingBackgroundStart
    {
        get => GetValue(LoadingBackgroundStartProperty);
        set => SetValue(LoadingBackgroundStartProperty, value);
    }
    
    internal IBrush? LoadingBackgroundMiddle
    {
        get => GetValue(LoadingBackgroundMiddleProperty);
        set => SetValue(LoadingBackgroundMiddleProperty, value);
    }
    
    internal IBrush? LoadingBackgroundEnd
    {
        get => GetValue(LoadingBackgroundEndProperty);
        set => SetValue(LoadingBackgroundEndProperty, value);
    }

    internal IBrush? AnimationLayerFill
    {
        get => GetValue(AnimationLayerFillProperty);
        set => SetValue(AnimationLayerFillProperty, value);
    }
    #endregion

    private static readonly Easing DefaultActiveMotionEasing = new SplineEasing
    {
        X1 = 0.25,
        Y1 = 0.1,
        X2 = 0.25,
        Y2 = 1.0
    };
    
    private Animation? _animation;
    private CancellationTokenSource? _animationCancellationTokenSource;
    private Control? _activeAnimationLayer;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsActiveProperty)
        {
            UpdateActiveAnimationState();
        }
        else if (change.Property == LoadingBackgroundStartProperty ||
                 change.Property == LoadingBackgroundMiddleProperty ||
                 change.Property == LoadingBackgroundEndProperty ||
                 change.Property == MotionDurationProperty ||
                 change.Property == MotionEasingCurveProperty)
        {
            RestartActiveAnimation();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateActiveAnimationState();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopActiveAnimation();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _activeAnimationLayer = e.NameScope.Find<Control>("PART_ActiveAnimationLayer");
        UpdateActiveAnimationState();
    }

    protected void StartActiveAnimation()
    {
        if (!ShouldRunActiveAnimation())
        {
            StopActiveAnimation();
            return;
        }

        if (_animationCancellationTokenSource is not null)
        {
            return;
        }

        _animation ??= BuildActiveAnimation();
        SetCurrentValue(AnimationLayerFillProperty, LoadingBackgroundStart);

        _animationCancellationTokenSource = new CancellationTokenSource();
        var cancellationTokenSource = _animationCancellationTokenSource;
        var animation               = _animation;
        Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                await animation.RunAsync(this, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
            {
            }
        });
    }

    protected void StopActiveAnimation()
    {
        _animationCancellationTokenSource?.Cancel();
        _animationCancellationTokenSource?.Dispose();
        _animationCancellationTokenSource = null;
    }

    private void UpdateActiveAnimationState()
    {
        if (ShouldRunActiveAnimation())
        {
            StartActiveAnimation();
        }
        else
        {
            StopActiveAnimation();
        }
    }

    private void RestartActiveAnimation()
    {
        var shouldRestart = _animationCancellationTokenSource is not null;
        StopActiveAnimation();
        _animation = null;
        if (shouldRestart)
        {
            StartActiveAnimation();
        }
    }

    private Animation BuildActiveAnimation()
    {
        return new Animation
        {
            Easing         = MotionEasingCurve ?? DefaultActiveMotionEasing,
            Duration       = MotionDuration,
            IterationCount = IterationCount.Infinite,
            PlaybackBehavior = PlaybackBehavior.OnlyIfVisible,
            Children =
            {
                new KeyFrame
                {
                    Setters = { new Setter(AnimationLayerFillProperty, LoadingBackgroundStart) },
                    Cue     = new Cue(0.0d)
                },
                new KeyFrame
                {
                    Setters = { new Setter(AnimationLayerFillProperty, LoadingBackgroundMiddle) },
                    Cue     = new Cue(0.5d)
                },
                new KeyFrame
                {
                    Setters = { new Setter(AnimationLayerFillProperty, LoadingBackgroundEnd) },
                    Cue     = new Cue(1.0d)
                }
            }
        };
    }

    private bool ShouldRunActiveAnimation()
    {
        return IsActive &&
               _activeAnimationLayer is not null &&
               MotionDuration > TimeSpan.Zero &&
               this.IsAttachedToVisualTree();
    }
}
