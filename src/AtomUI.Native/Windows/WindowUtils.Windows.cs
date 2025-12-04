using System.Runtime.Versioning;

namespace AtomUI.Native;

internal static partial class WindowExtensions
{
    [SupportedOSPlatform("windows")]
    public static void SetWindowIgnoreMouseEventsWindows(IntPtr handle, bool flag)
    {
        
    }
    
    [SupportedOSPlatform("windows")]
    public static bool IsWindowIgnoreMouseEventsWindows(IntPtr handle)
    {
        return false;
    }
}