using AtomUI.Icons.AntDesign;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace AtomUI.Controls.Commons;

using AvaScrollViewer = Avalonia.Controls.ScrollViewer;

public abstract class AbstractBackTopFloatButton : AbstractFloatButton
{
    #region 公共属性定义

    public static readonly StyledProperty<TimeSpan> ToTopDurationProperty =
        AvaloniaProperty.Register<AbstractBackTopFloatButton, TimeSpan>(nameof(ToTopDuration), TimeSpan.FromMilliseconds(450));

    public static readonly StyledProperty<AvaScrollViewer?> TargetProperty =
        AvaloniaProperty.Register<AbstractBackTopFloatButton, AvaScrollViewer?>(nameof(Target));

    public static readonly StyledProperty<double> VisibilityHeightProperty =
        AvaloniaProperty.Register<AbstractBackTopFloatButton, double>(nameof(VisibilityHeight), 400d);

    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<AbstractBackTopFloatButton>();

    public TimeSpan ToTopDuration
    {
        get => GetValue(ToTopDurationProperty);
        set => SetValue(ToTopDurationProperty, value);
    }

    public AvaScrollViewer? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public double VisibilityHeight
    {
        get => GetValue(VisibilityHeightProperty);
        set => SetValue(VisibilityHeightProperty, value);
    }

    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<AbstractBackTopFloatButton, bool>(nameof(IsActive));

    internal bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    #endregion

    private BaseMotionActor? _motionActor;
    private bool _showAnimating;
    private bool _hideAnimating;
    private CancellationTokenSource? _cancellationTokenSource;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (Target != null)
        {
            Target.ScrollChanged -= HandleScrollChanged;
            Target.ScrollChanged += HandleScrollChanged;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (Target != null)
        {
            Target?.ScrollChanged -= HandleScrollChanged;
        }
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _motionActor = e.NameScope.Find<BaseMotionActor>(BaseMotionActor.MotionActorPart);
        if (_motionActor != null)
        {
            _motionActor.SetCurrentValue(IsVisibleProperty, IsActive);
        }
    }

    protected override void ConfigureDefaultIcon()
    {
        if (Icon == null)
        {
            SetCurrentValue(IconProperty, new VerticalAlignTopOutlined());
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TargetProperty)
        {
            ConfigureTarget(change.OldValue as AvaScrollViewer, change.NewValue as AvaScrollViewer);
        }
        else if (change.Property == IsActiveProperty)
        {
            if (IsActive)
            {
                Dispatcher.InvokeAsync(ApplyShowMotionAsync);
            }
            else
            {
                Dispatcher.InvokeAsync(ApplyHideMotionAsync);
            }
        }
    }

    private void ConfigureTarget(AvaScrollViewer? oldScrollViewer, AvaScrollViewer? newScrollViewer)
    {
        if (oldScrollViewer != null)
        {
            oldScrollViewer.ScrollChanged -= HandleScrollChanged;
        }

        if (newScrollViewer != null)
        {
            newScrollViewer.ScrollChanged += HandleScrollChanged;
        }
    }

    private void HandleScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is AvaScrollViewer scrollViewer)
        {
            IsActive = scrollViewer.Offset.Y >= VisibilityHeight;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (Target != null)
        {
            IsActive = Target.Offset.Y >= VisibilityHeight;
        }
    }

    private async Task ApplyShowMotionAsync()
    {
        if (_motionActor is not null)
        {
            if (IsMotionEnabled)
            {
                if (_showAnimating)
                {
                    return;
                }

                _showAnimating = true;
                _motionActor.SetCurrentValue(IsVisibleProperty, false);
                var motion = new FadeInMotion(MotionDuration, new QuadraticEaseOut());
                await motion.RunAsync(_motionActor,
                    () => { _motionActor.SetCurrentValue(IsVisibleProperty, true); });
                _showAnimating = false;
            }
            else
            {
                _motionActor.SetCurrentValue(IsVisibleProperty, true);
            }
        }
    }

    private async Task ApplyHideMotionAsync()
    {
        if (_motionActor is not null)
        {
            if (IsMotionEnabled)
            {
                if (_hideAnimating)
                {
                    return;
                }

                _hideAnimating = true;
                var motion =
                    new FadeOutMotion(MotionDuration, new QuadraticEaseOut());
                await motion.RunAsync(_motionActor);
                _hideAnimating = false;
                _motionActor.SetCurrentValue(IsVisibleProperty, false);
            }
            else
            {
                _motionActor.SetCurrentValue(IsVisibleProperty, false);
            }
        }
    }

    protected override void OnClick()
    {
        if (IsEffectivelyEnabled)
        {
            if (Target != null)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var currentOffset = Target.Offset;
                var animation = new Animation
                {
                    Easing   = new CubicEaseInOut(),
                    Duration = ToTopDuration,
                    Children =
                    {
                        new KeyFrame
                        {
                            Setters = { new Setter(AvaScrollViewer.OffsetProperty, currentOffset) }, Cue = new Cue(0.0d)
                        },
                        new KeyFrame
                        {
                            Setters = { new Setter(AvaScrollViewer.OffsetProperty, new Vector(currentOffset.X, 0)) }, Cue = new Cue(1.0d)
                        }
                    }
                };
                animation.RunAsync(Target, _cancellationTokenSource.Token);
            }
        }
        base.OnClick();
    }
}
