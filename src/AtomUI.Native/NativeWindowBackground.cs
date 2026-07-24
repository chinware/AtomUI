using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Native;

internal interface INativeWindowBackgroundHook : IDisposable
{
    void SetBackgroundColor(Color? color);

    void PaintClientArea();
}

internal static class NativeWindowBackground
{
    public static INativeWindowBackgroundHook? TryAttachWindowsBackgroundHook(Window window)
    {
        return OperatingSystem.IsWindows()
            ? new Windows.WindowsBackgroundHook(window)
            : null;
    }
}
