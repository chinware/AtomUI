using AtomUI.Native.Interop;
using Avalonia.Controls;

namespace AtomUI.Native;

internal static class WindowUtils
{
    public static void SetWindowIgnoreMouseEvents(WindowBase window, bool flag)
    {
        var windowHandle = window.PlatformImpl?.Handle?.Handle;
        if (windowHandle is not null)

        {
            WindowUtilsImpl.AtomUISetWindowIgnoresMouseEvents(windowHandle.Value, flag);
        }
    }
}