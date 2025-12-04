using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace AtomUI.Native;

internal static partial class WindowExtensions
{
    public static void SetWindowIgnoreMouseEvents(this WindowBase window, bool flag)
    {
        var handle = window.PlatformImpl?.Handle?.Handle;
        Debug.Assert(handle is not null);
        if (OperatingSystem.IsWindows())
        {
            SetWindowIgnoreMouseEventsWindows(handle.Value, flag);
        }
        else if (OperatingSystem.IsMacOS())
        {
            SetWindowIgnoreMouseEventsMacOS(handle.Value, flag);
        }
        else if (OperatingSystem.IsLinux())
        {
            SetWindowIgnoreMouseEventsLinux(handle.Value, flag);
        }
        else
        {
            throw new PlatformNotSupportedException($"Unsupported platform: {RuntimeInformation.OSDescription}");
        }
    }

    public static bool IsWindowIgnoreMouseEvents(this WindowBase window)
    {
        var handle = window.PlatformImpl?.Handle?.Handle;
        Debug.Assert(handle is not null);
        if (OperatingSystem.IsWindows())
        {
            return IsWindowIgnoreMouseEventsWindows(handle.Value);
        }
        if (OperatingSystem.IsMacOS())
        {
            return IsWindowIgnoreMouseEventsMacOS(handle.Value);
        }
        if (OperatingSystem.IsLinux())
        {
            return IsWindowIgnoreMouseEventsLinux(handle.Value);
        }
        throw new PlatformNotSupportedException($"Unsupported platform: {RuntimeInformation.OSDescription}");
    }
}