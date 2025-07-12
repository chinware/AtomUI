using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

public class Window : AvaloniaWindow
{
    protected override Type StyleKeyOverride { get; } = typeof(Window);

    private WindowState? _originState;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == WindowStateProperty)
        {
            if (change.NewValue is WindowState newState)
            {
                if (newState == WindowState.FullScreen)
                {
                    _originState = change.GetOldValue<WindowState>();
                }
                else
                {
                    WindowState = _originState.HasValue ? _originState.Value : newState;;
                }
            }
        }
    }
}