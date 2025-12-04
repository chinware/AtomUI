using System.Runtime.Versioning;
using AtomUI.Native.Windows;

namespace AtomUI.Native;

internal static partial class WindowExtensions
{
    [SupportedOSPlatform("windows")]
    public static void SetWindowIgnoreMouseEventsWindows(IntPtr handle, bool flag)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle");
        }

        var styles = WindowUtilsInterop.GetWindowLongPtr(handle, WindowUtilsInterop.GWL_EXSTYLE);
        if (flag)
        {
            styles |= (WindowUtilsInterop.WS_EX_TRANSPARENT | WindowUtilsInterop.WS_EX_LAYERED);
        }
        else
        {
            styles &= ~(WindowUtilsInterop.WS_EX_TRANSPARENT | WindowUtilsInterop.WS_EX_LAYERED);
        }

        WindowUtilsInterop.SetWindowLongPtr(handle, WindowUtilsInterop.GWL_EXSTYLE, styles);
    }
    
    [SupportedOSPlatform("windows")]
    public static bool IsWindowIgnoreMouseEventsWindows(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle");
        }
        var styles = WindowUtilsInterop.GetWindowLongPtr(handle, WindowUtilsInterop.GWL_EXSTYLE);
        return (styles & WindowUtilsInterop.WS_EX_TRANSPARENT) != 0;
    }
}