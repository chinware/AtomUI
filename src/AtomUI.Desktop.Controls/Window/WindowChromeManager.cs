using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal interface IWindowChromeManager
{
    Action? PrepareInitialShowState();

    void HandleFrameShadowChanged(BoxShadows frameShadow);

    void ConfigureTitleBarHeightHint(double height);

    void HandlePropertyChanged(AvaloniaProperty property);

    void UpdateFrameGeometry();
}

internal static class WindowChromeManager
{
    public static IWindowChromeManager? Attach(Window window)
    {
        if (OperatingSystem.IsWindows())
        {
            return WindowsWindowChromeManager.Attach(window);
        }

        if (OperatingSystem.IsLinux())
        {
            return LinuxWindowChromeManager.Attach(window);
        }

        return null;
    }
}
