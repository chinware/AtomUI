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

}
