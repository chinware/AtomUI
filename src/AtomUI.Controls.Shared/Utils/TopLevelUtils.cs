using Avalonia.Controls;

namespace AtomUI.Controls.Utils;

internal static class TopLevelUtils
{
    public static double GetDesktopScaling(Control control)
    {
        var window = TopLevel.GetTopLevel(control) as WindowBase;
        return window?.DesktopScaling ?? 1.0;
    }
}