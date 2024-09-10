using Avalonia.Controls;

namespace AtomUI.Theme.Styling;

public static class ControlStateUtils
{
    public static void InitCommonState(Control control, ref ControlStyleState state)
    {
        state = ControlStyleState.None;
        if (control.IsEnabled)
        {
            state |= ControlStyleState.Enabled;
        }

        if (control.IsFocused)
        {
            state |= ControlStyleState.HasFocus;
        }

        if (control.IsPointerOver)
        {
            state |= ControlStyleState.MouseOver;
        }
    }
}