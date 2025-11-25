using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

internal class HeadTextButton : AvaloniaButton
{

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<HeadTextButton>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty,
                        SharedTokenKey.MotionDurationFast)
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
}