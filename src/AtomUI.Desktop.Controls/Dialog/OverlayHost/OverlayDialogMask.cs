using AtomUI.Controls;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

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

    private readonly OverlayLayer _dialogLayer;

    public OverlayDialogMask(OverlayLayer dialogLayer, Dialog dialog)
    {
        _dialogLayer = dialogLayer;
        this[!IsMotionEnabledProperty] = dialog[!IsMotionEnabledProperty];
    }

    public void Show(OverlayDialogHost maskTarget)
    {
        var dialogHostIndex = _dialogLayer.Children.IndexOf(maskTarget);
        if (dialogHostIndex == -1)
        {
            return;
        }
        _dialogLayer.SizeChanged += HandleDialogLayerSizeChanged;
        _dialogLayer.Children.Insert(dialogHostIndex, this);
        ConfigureMaskSize(_dialogLayer.Bounds.Size);
        Dispatcher.Post(() =>
        {
            Opacity = 1.0;
        });
    }

    public void Hide()
    {
        Opacity = 0.0;
        DispatcherTimer.RunOnce(() =>
        {
            _dialogLayer.Children.Remove(this);
            _dialogLayer.SizeChanged -= HandleDialogLayerSizeChanged;
        }, AnimationDuration);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ConfigureTransitions(false);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty ||
                change.Property == AnimationDurationProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<DoubleTransition>(OpacityProperty, AnimationDuration)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    private void HandleDialogLayerSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        ConfigureMaskSize(e.NewSize);
    }

    private void ConfigureMaskSize(Size size)
    {
        Width  = size.Width;
        Height = size.Height;
    }
}