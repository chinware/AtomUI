using AtomUI.Native.Interop;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Native;

internal static class WindowUtils
{
    public static void SetWindowIgnoreMouseEvents(this WindowBase window, bool flag)
    {
        var windowHandle = window.PlatformImpl?.Handle?.Handle;
        if (windowHandle is not null)

        {
            WindowUtilsImpl.AtomUISetWindowIgnoresMouseEvents(windowHandle.Value, flag);
        }
    }

    public static bool GetWindowIgnoreMouseEvents(this WindowBase window)
    {
        var windowHandle = window.PlatformImpl?.Handle?.Handle;
        if (windowHandle is not null)

        {
            return WindowUtilsImpl.AtomUIGetWindowIgnoresMouseEvents(windowHandle.Value);
        }

        return false;
    }
    
    public static void LockWindowBuddyLayer(this WindowBase window, WindowBase buddyWindow)
    {
        var windowHandle = window.PlatformImpl?.Handle?.Handle;
        var buddyHandle = buddyWindow.PlatformImpl?.Handle?.Handle;
        if (windowHandle is not null && buddyHandle is not null)
        {
            WindowUtilsImpl.AtomUILockWindowBuddyLayer(windowHandle.Value, buddyHandle.Value);
        }
    }

    public static void SetMacOSOptionButtonsPosition(this WindowBase window, double x, double y, double spacing = 10.0)
    {
        var windowHandle = window.PlatformImpl?.Handle?.Handle;
        if (windowHandle is not null)
        {
            WindowUtilsImpl.AtomUISetMacOSCaptionButtonsPosition(windowHandle.Value, x, y, spacing);
        }
    }
    
    public static Size GetMacOSOptionsSize(this WindowBase window, double spacing = 10.0)
    {
        var windowHandle = window.PlatformImpl?.Handle?.Handle;
        Size size = new Size();
        if (windowHandle is not null)
        {
            var result = WindowUtilsImpl.AtomUIMacOSCaptionsSize(windowHandle.Value, spacing);
            size = new Size(result.Width, result.Height);
        }
        return size;
    }
}