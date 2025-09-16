using AtomUI.Controls.Utils; 
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class OverlayDialogMask : TemplatedControl
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<OverlayDialogMask>();
    
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<OverlayDialogMask, TimeSpan>(nameof(AnimationDuration));
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }
    
    private CancellationTokenSource? _cancellationTokenSource;

    public Task ShowAsync()
    {
        Task? task;
        Opacity = 1.0;
        if (IsMotionEnabled)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            task                     = Task.Delay(AnimationDuration * 1.2, _cancellationTokenSource.Token);
        }
        else
        {
            task = Task.CompletedTask;
        }
        return task;
    }

    public Task HideAsync()
    {
        Task? task;
        Opacity = 0.0;
        if (IsMotionEnabled)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            task                     = Task.Delay(AnimationDuration * 1.2, _cancellationTokenSource.Token);
        }
        else
        {
            task = Task.CompletedTask;
        }
        return task;
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

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                _cancellationTokenSource?.Cancel();
                Transitions = [
                    TransitionUtils.CreateTransition<DoubleTransition>(OpacityProperty)
                ];
            }
        }
        else
        {
            _cancellationTokenSource?.Cancel();
            Transitions = null;
        }
    }
    
}