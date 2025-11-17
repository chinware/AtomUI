using Avalonia.Input;

namespace AtomUI.Controls;

internal static class CalendarExtensions
{
    public static void GetMetaKeyState(KeyModifiers modifiers, out bool ctrl, out bool shift)
    {
        ctrl  = (modifiers & KeyModifiers.Control) == KeyModifiers.Control;
        shift = (modifiers & KeyModifiers.Shift) == KeyModifiers.Shift;
    }
}