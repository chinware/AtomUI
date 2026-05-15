using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

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
    private Panel? _rootLayout;
    private Border? _frame;
    private Border? _progress;
    
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
        ConfigureProgressWidth();
        SyncProgressBorderProperties();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsShowTransitionProgressProperty)
        {
            UpdateProgressState(forceAnimation: true);
        }
        else if (change.Property == AutoPlaySpeedProperty)
        {
            ConfigureProgressAnimation();
        }
        else if (change.Property == IsSelectedProperty)
        {
            UpdateProgressState(forceAnimation: false);
        }
        else if (change.Property == ProgressValueProperty)
        {
            ConfigureProgressWidth();
        }
        else if (change.Property == EffectiveProgressWidthProperty ||
                 change.Property == CornerRadiusProperty ||
                 change.Property == HeightProperty ||
                 change.Property == BackgroundProperty)
        {
            SyncProgressBorderProperties();
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ReleaseProgressBorder();
        base.OnApplyTemplate(e);
        _rootLayout = e.NameScope.Find<Panel>("PART_RootLayout");
        _frame      = e.NameScope.Find<Border>("PART_Frame");
        UpdateProgressState(forceAnimation: false);
    }

    private void UpdateProgressState(bool forceAnimation)
    {
        if (!IsShowTransitionProgress || !IsSelected)
        {
            ClearProgressAnimation();
            ReleaseProgressBorder();
            return;
        }

        if (_rootLayout is null)
        {
            return;
        }

        EnsureProgressBorder();
        BuildProgressAnimation(forceAnimation);
        HandleSelectChanged();
    }

    private void BuildProgressAnimation(bool force = false)
    {
        if (!IsShowTransitionProgress)
        {
            ClearProgressAnimation();
            return;
        }

        if (force || _animation is null)
        {
            StopProgressAnimation();
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
        StopProgressAnimation();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateProgressState(forceAnimation: false);
    }

    private void StopProgressAnimation()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private void ClearProgressAnimation()
    {
        StopProgressAnimation();
        _animation = null;
        SetCurrentValue(ProgressValueProperty, 0.0);
        SetCurrentValue(EffectiveProgressWidthProperty, 0.0);
    }

    private void HandleSelectChanged()
    {
        StopProgressAnimation();
        if (IsSelected && IsShowTransitionProgress)
        {
            if (_animation is null)
            {
                BuildProgressAnimation();
            }
            _cancellationTokenSource = new CancellationTokenSource();
            _animation?.RunAsync(this, _cancellationTokenSource.Token);
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

    private void EnsureProgressBorder()
    {
        if (_progress is not null || _rootLayout is null)
        {
            SyncProgressBorderProperties();
            return;
        }

        _progress = new Border
        {
            Name                = "Progress",
            HorizontalAlignment = HorizontalAlignment.Left
        };
        _progress.SetValue(IsVisibleProperty, false, BindingPriority.Template);
        _progress.SetTemplatedParent(this);
        SyncProgressBorderProperties();
        _rootLayout.Children.Add(_progress);
    }

    private void ReleaseProgressBorder()
    {
        if (_progress is null)
        {
            return;
        }

        if (_progress.GetVisualParent() is Panel panel)
        {
            panel.Children.Remove(_progress);
        }
        else
        {
            _rootLayout?.Children.Remove(_progress);
        }

        _progress.SetTemplatedParent(null);
        _progress = null;
    }

    private void SyncProgressBorderProperties()
    {
        if (_progress is null)
        {
            return;
        }

        _progress.SetValue(CornerRadiusProperty, CornerRadius, BindingPriority.Template);
        _progress.SetValue(HeightProperty, Height, BindingPriority.Template);
        _progress.SetValue(WidthProperty, EffectiveProgressWidth, BindingPriority.Template);
        _progress.SetValue(BackgroundProperty, Background, BindingPriority.Template);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.Dispatcher.Post(this.EnableTransitions);
    }
}
