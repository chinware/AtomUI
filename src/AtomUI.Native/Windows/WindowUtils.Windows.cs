using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using AtomUI.Native.Windows;
using Avalonia.Controls;

namespace AtomUI.Native;

[SupportedOSPlatform("windows")]
internal static class WindowUtilsWindows
{
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
    
    public static bool IsWindowIgnoreMouseEventsWindows(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle");
        }
        var styles = WindowUtilsInterop.GetWindowLongPtr(handle, WindowUtilsInterop.GWL_EXSTYLE);
        return (styles & WindowUtilsInterop.WS_EX_TRANSPARENT) != 0;
    }

    /// <summary>
    /// 获取 Windows 系统默认的标题栏高度。
    /// 使用 Avalonia 的 WindowDecorationMargin 属性获取标题栏高度。
    /// </summary>
    /// <returns>非 Windows 或无窗口句柄时返回 <c>null</c>。</returns>
    public static double? GetSystemTitleBarHeightWindows(Window window)
    {
        if (!OperatingSystem.IsWindows())
        {
            return null;
        }

        // 使用 Avalonia 提供的 WindowDecorationMargin 属性
        // Top 值就是标题栏高度
        var margin = window.WindowDecorationMargin;
        return margin.Top > 0 ? margin.Top : null;
    }

    public static unsafe void ApplyDwmShadow(IntPtr hwnd)
    {
        var policy = WindowUtilsInterop.DWMNCRP_ENABLED;
        WindowUtilsInterop.DwmSetWindowAttribute(hwnd,
            WindowUtilsInterop.DWMWA_NCRENDERING_POLICY, &policy, sizeof(int));

        var margins = new WindowUtilsInterop.MARGINS
        {
            cxLeftWidth = -1,
            cxRightWidth = -1,
            cyTopHeight = -1,
            cyBottomHeight = -1
        };
        WindowUtilsInterop.DwmExtendFrameIntoClientArea(hwnd, ref margins);

        if (Environment.OSVersion.Version.Build >= 22000)
        {
            var round = WindowUtilsInterop.DWMWCP_ROUND;
            WindowUtilsInterop.DwmSetWindowAttribute(hwnd,
                WindowUtilsInterop.DWMWA_WINDOW_CORNER_PREFERENCE, &round, sizeof(int));
        }
    }

    public static void HandleNcCalcSize(IntPtr hWnd, IntPtr lParam)
    {
        var style = WindowUtilsInterop.GetWindowLongPtr(hWnd, WindowUtilsInterop.GWL_STYLE);
        if ((style & WindowUtilsInterop.WS_MAXIMIZE) != 0)
        {
            var nccsp = Marshal.PtrToStructure<WindowUtilsInterop.NCCALCSIZE_PARAMS>(lParam);
            var borderX = WindowUtilsInterop.GetSystemMetrics(WindowUtilsInterop.SM_CXSIZEFRAME)
                        + WindowUtilsInterop.GetSystemMetrics(WindowUtilsInterop.SM_CXPADDEDBORDER);
            var borderY = WindowUtilsInterop.GetSystemMetrics(WindowUtilsInterop.SM_CYSIZEFRAME)
                        + WindowUtilsInterop.GetSystemMetrics(WindowUtilsInterop.SM_CXPADDEDBORDER);
            nccsp.rgrc0.left += borderX;
            nccsp.rgrc0.top += borderY;
            nccsp.rgrc0.right -= borderX;
            nccsp.rgrc0.bottom -= borderY;
            Marshal.StructureToPtr(nccsp, lParam, false);
        }
    }

    public static int HitTestBorder(IntPtr hWnd, IntPtr lParam, int borderWidth)
    {
        var screenX = WindowUtilsInterop.GetXLParam(lParam);
        var screenY = WindowUtilsInterop.GetYLParam(lParam);

        WindowUtilsInterop.GetWindowRect(hWnd, out var rc);

        var left   = screenX - rc.left;
        var right  = rc.right - screenX;
        var top    = screenY - rc.top;
        var bottom = rc.bottom - screenY;

        if (top <= borderWidth && left <= borderWidth)    return WindowUtilsInterop.HTTOPLEFT;
        if (top <= borderWidth && right <= borderWidth)   return WindowUtilsInterop.HTTOPRIGHT;
        if (bottom <= borderWidth && left <= borderWidth) return WindowUtilsInterop.HTBOTTOMLEFT;
        if (bottom <= borderWidth && right <= borderWidth)return WindowUtilsInterop.HTBOTTOMRIGHT;
        if (top <= borderWidth)                           return WindowUtilsInterop.HTTOP;
        if (bottom <= borderWidth)                        return WindowUtilsInterop.HTBOTTOM;
        if (left <= borderWidth)                          return WindowUtilsInterop.HTLEFT;
        if (right <= borderWidth)                         return WindowUtilsInterop.HTRIGHT;

        return 0;
    }
}