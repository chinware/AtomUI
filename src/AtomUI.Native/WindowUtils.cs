using AtomUI.Native.Interop;
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

    public static void MoveWindow(this WindowBase window, int x, int y)
    {
        var windowHandle = window.PlatformImpl?.Handle?.Handle;
        if (windowHandle is not null)
        {
            WindowUtilsImpl.AtomUIMoveWindow(windowHandle.Value, x, y);
        }
    }
}