using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils; 
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class CarouselPageIndicator : ContentControl, ISelectable
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<CarouselPageIndicator>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CarouselPageIndicator>();
    
    public static readonly StyledProperty<bool> IsShowTransitionProgressProperty = 
        Carousel.IsShowTransitionProgressProperty.AddOwner<CarouselPageIndicator>();
    
    public static readonly StyledProperty<TimeSpan> AutoPlaySpeedProperty = 
        Carousel.AutoPlaySpeedProperty.AddOwner<CarouselPageIndicator>();
    
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsShowTransitionProgress
    {
        get => GetValue(IsShowTransitionProgressProperty);
        set => SetValue(IsShowTransitionProgressProperty, value);
    }
    
    public TimeSpan AutoPlaySpeed
    {
        get => GetValue(AutoPlaySpeedProperty);
        set => SetValue(AutoPlaySpeedProperty, value);
    }

    #endregion
    
    internal static readonly StyledProperty<double> FrameOpacityProperty =
        AvaloniaProperty.Register<CarouselPageIndicator, double>(nameof(FrameOpacity));
    
    internal static readonly DirectProperty<CarouselPageIndicator, double> ProgressValueProperty =
        AvaloniaProperty.RegisterDirect<CarouselPageIndicator, double>(
            nameof(ProgressValue),
            o => o.ProgressValue,
            (o, v) => o.ProgressValue = v);
    
    internal static readonly DirectProperty<CarouselPageIndicator, double> EffectiveProgressWidthProperty =
        AvaloniaProperty.RegisterDirect<CarouselPageIndicator, double>(
            nameof(EffectiveProgressWidth),
            o => o.EffectiveProgressWidth,
            (o, v) => o.EffectiveProgressWidth = v);
    
    internal double FrameOpacity
    {
        get => GetValue(FrameOpacityProperty);
        set => SetValue(FrameOpacityProperty, value);
    }

    private double _progressValue;

    internal double ProgressValue
    {
        get => _progressValue;
        set => SetAndRaise(ProgressValueProperty, ref _progressValue, value);
    }
    
    private double _effectiveProgressWidth;

    internal double EffectiveProgressWidth
    {
        get => _effectiveProgressWidth;
        set => SetAndRaise(EffectiveProgressWidthProperty, ref _effectiveProgressWidth, value);
    }
    
    private Animation? _animation;
    private CancellationTokenSource? _cancellationTokenSource;
    private Border? _frame;
    
    static CarouselPageIndicator()
    {
        SelectableMixin.Attach<CarouselPageIndicator>(IsSelectedProperty);
        PressedMixin.Attach<CarouselPageIndicator>();
        FocusableProperty.OverrideDefaultValue<CarouselPageIndicator>(true);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        SetCurrentValue(CornerRadiusProperty, new CornerRadius(e.NewSize.Height));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }

        if (change.Property == IsShowTransitionProgressProperty)
        {
            BuildProgressAnimation(true);
        }
        else if (change.Property == AutoPlaySpeedProperty)
        {
            ConfigureProgressAnimation();
        }
        else if (change.Property == IsSelectedProperty)
        {
            HandleSelectChanged();
        }
        else if (change.Property == ProgressValueProperty)
        {
            ConfigureProgressWidth();
        }
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<DoubleTransition>(WidthProperty),
                    TransitionUtils.CreateTransition<DoubleTransition>(FrameOpacityProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _frame = e.NameScope.Find<Border>(CarouselPageIndicatorThemeConstants.FramePart);
        if (IsShowTransitionProgress)
        {
            BuildProgressAnimation(false);
        }
    }

    private void BuildProgressAnimation(bool force = false)
    {
        if (force || _animation is null)
        {
            _cancellationTokenSource?.Cancel();
            _animation = new Animation
            {
                Easing         = new LinearEasing(),
                Duration       = AutoPlaySpeed,
                Children =
                {
                    new KeyFrame
                    {
                        Setters = { new Setter(ProgressValueProperty, 0.0) }, 
                        Cue     = new Cue(0.0d)
                    },
                    new KeyFrame
                    {
                        Setters = { new Setter(ProgressValueProperty, 1.0) }, 
                        Cue     = new Cue(1.0d)
                    }
                }
            };
            ConfigureProgressAnimation();
            _cancellationTokenSource = null;
        }
    }

    private void ConfigureProgressAnimation()
    {
        if (_animation != null)
        {
            _animation.Duration = AutoPlaySpeed;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }

    private void HandleSelectChanged()
    {
        if (IsSelected && IsShowTransitionProgress)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _animation?.RunAsync(this, _cancellationTokenSource.Token);
        }
        else
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
        }
    }

    private void ConfigureProgressWidth()
    {
        if (IsSelected && IsShowTransitionProgress)
        {
            var width = _frame?.Bounds.Width ?? 0.0;
            SetCurrentValue(EffectiveProgressWidthProperty, width * ProgressValue);
        }
     
    }
}