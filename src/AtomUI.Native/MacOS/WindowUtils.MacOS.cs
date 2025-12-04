using System.Drawing;
using System.Runtime.Versioning;
using Avalonia.Controls;

namespace AtomUI.Native;

internal static partial class WindowExtensions
{
    [SupportedOSPlatform("macos")]
    public static void SetWindowIgnoreMouseEventsMacOS(IntPtr handle, bool flag)
    {
        
    }
    
    [SupportedOSPlatform("macos")]
    public static bool IsWindowIgnoreMouseEventsMacOS(IntPtr handle)
    {
        return false;
    }
    
    [SupportedOSPlatform("macos")]
    public static void SetMacOSOptionButtonsPosition(this WindowBase window, double x, double y, double spacing = 10.0)
    {
    }

    [SupportedOSPlatform("macos")]
    public static Size GetMacOSOptionsSize(this WindowBase window, double spacing = 10.0)
    {
        return new Size((int)window.Width, (int)window.Height);
    }

    [SupportedOSPlatform("macos")]
    public static void SetMacOSWindowClosable(this WindowBase window, bool flag)
    {
    }
}