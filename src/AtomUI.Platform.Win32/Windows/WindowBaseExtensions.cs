using AtomUI.Platform.Win32.Interop;
using Avalonia.Controls;

namespace AtomUI.Platform.Windows;

internal static class WindowBaseExtensions
{
    private const WindowStyles WindowStateMask = (WindowStyles.WS_MAXIMIZE | WindowStyles.WS_MINIMIZE);
    
    private static WindowStyles GetWindowStateStyles(this WindowBase window)
    {
        return window.GetWindowStyle() & WindowStateMask;
    }
    
    private static WindowStyles GetWindowStyle(this WindowBase window)
    {
        var hwnd = window.PlatformImpl!.Handle!.Handle;
        return (WindowStyles)Win32API.GetWindowLong(hwnd, (int)WindowLongParam.GWL_STYLE);
    }
    
    private static void SetWindowStyle(this WindowBase window, WindowStyles style)
    {
        var hwnd = window.PlatformImpl!.Handle!.Handle;
        Win32API.SetWindowLong(hwnd, (int)WindowLongParam.GWL_STYLE, (uint)style);
    }
    
    private static WindowStyles GetWindowExtendedStyle(this WindowBase window)
    {
        var hwnd = window.PlatformImpl!.Handle!.Handle;
        return (WindowStyles)Win32API.GetWindowLong(hwnd, (int)WindowLongParam.GWL_EXSTYLE);
    }
    
    private static void SetWindowExtendedStyle(this WindowBase window, WindowStyles style)
    {
        var hwnd = window.PlatformImpl!.Handle!.Handle;
        Win32API.SetWindowLong(hwnd, (int)WindowLongParam.GWL_EXSTYLE, (uint)style);
    }
    
    public static void SetTransparentForMouseEvents(this WindowBase window, bool flag)
    {
        var styles = window.GetWindowExtendedStyle();
        // 不是确定这样处理是否合适
        if (flag)
        {
            styles |= WindowStyles.WS_EX_TRANSPARENT | WindowStyles.WS_EX_LAYERED;
        }
        else
        {
            styles &= ~(WindowStyles.WS_EX_TRANSPARENT | WindowStyles.WS_EX_LAYERED);
        }

        window.SetWindowExtendedStyle(styles);
    }
    
    public static bool IsTransparentForMouseEvents(this WindowBase window)
    {
        var styles = window.GetWindowExtendedStyle();
        return (styles & WindowStyles.WS_EX_TRANSPARENT) != 0;
    }
}