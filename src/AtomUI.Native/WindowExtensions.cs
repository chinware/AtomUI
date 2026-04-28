using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using AtomUI.Native.Windows;
using Avalonia.Controls;

namespace AtomUI.Native;

internal static class WindowExtensions
{
    public static void SetWindowIgnoreMouseEvents(this WindowBase window, bool flag)
    {
        var handle = window.PlatformImpl?.Handle?.Handle;
        Debug.Assert(handle is not null);
        if (OperatingSystem.IsWindows())
        {
            WindowUtilsWindows.SetWindowIgnoreMouseEventsWindows(handle.Value, flag);
        }
        else if (OperatingSystem.IsMacOS())
        {
            WindowUtilsMacOS.SetWindowIgnoreMouseEventsMacOS(handle.Value, flag);
        }
        else if (OperatingSystem.IsLinux())
        {
            WindowUtilsLinux.SetWindowIgnoreMouseEventsLinux(handle.Value, flag);
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
            return WindowUtilsWindows.IsWindowIgnoreMouseEventsWindows(handle.Value);
        }
        if (OperatingSystem.IsMacOS())
        {
            return WindowUtilsMacOS.IsWindowIgnoreMouseEventsMacOS(handle.Value);
        }
        if (OperatingSystem.IsLinux())
        {
            return WindowUtilsLinux.IsWindowIgnoreMouseEventsLinux(handle.Value);
        }
        throw new PlatformNotSupportedException($"Unsupported platform: {RuntimeInformation.OSDescription}");
    }

    /// <summary>
    /// 获取系统默认的标题栏高度。
    /// 当 ExtendClientAreaTitleBarHeightHint 为 -1 时，可以使用此方法获取实际的标题栏高度。
    /// </summary>
    /// <returns>标题栏高度，如果无法获取则返回 <c>null</c>。</returns>
    public static double? GetSystemTitleBarHeight(this Window window)
    {
        if (OperatingSystem.IsWindows())
        {
            return WindowUtilsWindows.GetSystemTitleBarHeightWindows(window);
        }
        if (OperatingSystem.IsMacOS())
        {
            return WindowUtilsMacOS.GetSystemTitleBarHeightMacOS(window);
        }
        if (OperatingSystem.IsLinux())
        {
            return WindowUtilsLinux.GetSystemTitleBarHeightLinux(window);
        }
        return null;
    }
    
    [SupportedOSPlatform("windows")]
    public static void InitializeWinWindow(this Window window)
    {
        Win32Properties.AddWndProcHookCallback(window, window.WinWndProcHook);
        window.ApplyWinDwmShadow();

        var hwnd = window.TryGetPlatformHandle()!.Handle;
        WindowUtilsInterop.SetWindowPos(hwnd, IntPtr.Zero,
            0, 0, 0, 0,
            WindowUtilsInterop.SWP_FRAMECHANGED |
            WindowUtilsInterop.SWP_NOSIZE |
            WindowUtilsInterop.SWP_NOMOVE |
            WindowUtilsInterop.SWP_NOZORDER |
            WindowUtilsInterop.SWP_NOACTIVATE);
    }

    [SupportedOSPlatform("windows")]
    public static void ApplyWinDwmShadow(this Window window)
    {
        var handle = window.TryGetPlatformHandle();
        if (handle is null || handle.HandleDescriptor != "HWND")
        {
            return;
        }
        WindowUtilsWindows.ApplyDwmShadow(handle.Handle);
    }
    
    [SupportedOSPlatform("windows")]
    public static IntPtr WinWndProcHook(this Window window, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WindowUtilsInterop.WM_NCCALCSIZE when wParam != IntPtr.Zero:
                WindowUtilsWindows.HandleNcCalcSize(hWnd, lParam);
                handled = true;
                return IntPtr.Zero;

            case WindowUtilsInterop.WM_NCHITTEST:
                return window.HandleNcHitTest(hWnd, lParam, ref handled);
        }
        return IntPtr.Zero;
    }
    
    [SupportedOSPlatform("windows")]
    private static IntPtr HandleNcHitTest(this Window window, IntPtr hWnd, IntPtr lParam, ref bool handled)
    {
        if (window.WindowState is WindowState.FullScreen or WindowState.Maximized)
            return IntPtr.Zero;

        var borderWidth = (int)(4 * window.RenderScaling);
        var hitTest     = WindowUtilsWindows.HitTestBorder(hWnd, lParam, borderWidth);
        if (hitTest != 0)
        {
            handled = true;
            return (IntPtr)hitTest;
        }

        return IntPtr.Zero;
    }

    /// <summary>
    /// 窗体输入区域 (X11 SHAPE input region) 控制扩展。
    /// 将窗体的输入区域设置为指定矩形（device pixel 坐标），矩形外的鼠标事件将穿透到下层窗口。
    /// 主要用于 <c>WindowDrawnDecorations</c> 阴影区域的点击穿透处理。
    /// </summary>
    [SupportedOSPlatform("linux")]
    public static void SetWindowInputRectangle(this Window window, int x, int y, int width, int height)
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        var handle = window.PlatformImpl?.Handle?.Handle;
        Debug.Assert(handle is not null);
        WindowUtilsLinux.SetInputRectangle(handle.Value, x, y, width, height);
    }

    /// <summary>
    /// 将窗体的输入区域重置为整个客户区（device pixel 坐标）。
    /// </summary>
    [SupportedOSPlatform("linux")]
    public static void ResetWindowInputRegion(this Window window, int width, int height)
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        var handle = window.PlatformImpl?.Handle?.Handle;
        Debug.Assert(handle is not null);
        WindowUtilsLinux.ResetInputRegion(handle.Value, width, height);
    }
}
