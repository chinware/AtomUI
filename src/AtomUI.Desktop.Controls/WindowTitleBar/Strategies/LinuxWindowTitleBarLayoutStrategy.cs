using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class LinuxWindowTitleBarLayoutStrategy : IWindowTitleBarLayoutStrategy
{
    public static readonly LinuxWindowTitleBarLayoutStrategy Instance = new();

    public WindowTitleBarTitleAlignment AutoAlignment => WindowTitleBarTitleAlignment.Left;

    private LinuxWindowTitleBarLayoutStrategy()
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
