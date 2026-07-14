using System.Runtime.Versioning;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

[SupportedOSPlatform("windows10.0")]
internal static class WindowsInactiveFramePolicy
{
    private const int Windows11InitialBuild = 22000;
    private const uint WindowMessageNonClientActivate = 0x0086;

    internal static void Apply(Window window)
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, Windows11InitialBuild))
        {
            return;
        }

        Win32Properties.AddWndProcHookCallback(window, HandleWindowMessage);
    }

    internal static IntPtr HandleWindowMessage(
        IntPtr hWnd,
        uint message,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        if (message == WindowMessageNonClientActivate && wParam == IntPtr.Zero)
        {
            handled = true;
            return new IntPtr(1);
        }

        return IntPtr.Zero;
    }
}
