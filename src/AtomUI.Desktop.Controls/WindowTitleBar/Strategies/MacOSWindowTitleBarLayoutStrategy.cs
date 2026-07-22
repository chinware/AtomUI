using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class MacOSWindowTitleBarLayoutStrategy : IWindowTitleBarLayoutStrategy
{
    public static readonly MacOSWindowTitleBarLayoutStrategy Instance = new();

    public WindowTitleBarTitleAlignment AutoAlignment => WindowTitleBarTitleAlignment.WindowCenter;

    private MacOSWindowTitleBarLayoutStrategy()
    {
    }

    public Thickness ResolveNativeChromeInsets(
        double width,
        Thickness reportedNativeChromeInsets,
        bool isCsdEnabled,
        WindowState windowState)
    {
        return WindowTitleBarNativeChromeInsets.Resolve(width, reportedNativeChromeInsets, windowState);
    }
}
