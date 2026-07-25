using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia;
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
    public static void SetWindowsCsdFrameDarkMode(this WindowBase window, bool isDarkMode)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var handle = window.TryGetPlatformHandle();
        if (handle is null || handle.Handle == IntPtr.Zero)
        {
            return;
        }

        WindowUtilsWindows.SetWindowFrameDarkModeWindows(handle.Handle, isDarkMode);
    }

    [SupportedOSPlatform("macos")]
    public static void SetMacOsResizeIndicatorVisible(this Window window, bool isVisible)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return;
        }

        WindowUtilsMacOS.SetResizeIndicatorVisible(window, isVisible);
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

    [SupportedOSPlatform("linux")]
    public static void ConfigureLinuxInitialWindowGeometry(
        this Window window,
        PixelPoint position,
        Size clientSize,
        double scaling)
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        var handle = window.PlatformImpl?.Handle?.Handle;
        Debug.Assert(handle is not null);

        scaling = Math.Max(1, scaling);
        var width    = ToPixelLength(clientSize.Width, scaling);
        var height   = ToPixelLength(clientSize.Height, scaling);
        var minWidth = ToPixelLength(window.MinWidth, scaling);
        var minHeight = ToPixelLength(window.MinHeight, scaling);
        var maxWidth = ToOptionalPixelLength(window.MaxWidth, scaling);
        var maxHeight = ToOptionalPixelLength(window.MaxHeight, scaling);

        WindowUtilsLinux.ConfigureInitialWindowGeometry(
            handle.Value,
            position.X,
            position.Y,
            width,
            height,
            minWidth,
            minHeight,
            maxWidth,
            maxHeight);
    }

    [SupportedOSPlatform("linux")]
    public static void SetLinuxX11CsdFrameExtents(this Window window, Thickness frameExtents)
    {
        if (!OperatingSystem.IsLinux())
        {
            return;
        }

        var handle = window.TryGetPlatformHandle();
        if (handle is null || handle.HandleDescriptor != "XID")
        {
            return;
        }

        var scaling = window.RenderScaling <= 0 ? 1.0 : window.RenderScaling;
        WindowUtilsLinux.SetX11CsdFrameExtents(
            handle.Handle,
            ToPixelMargin(frameExtents.Left, scaling),
            ToPixelMargin(frameExtents.Top, scaling),
            ToPixelMargin(frameExtents.Right, scaling),
            ToPixelMargin(frameExtents.Bottom, scaling));
    }

    private static int ToPixelLength(double value, double scaling)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            return 1;
        }
        return Math.Max(1, (int)(value * scaling));
    }

    private static int? ToOptionalPixelLength(double value, double scaling)
    {
        if (!double.IsFinite(value) || value <= 0 || value > 100_000)
        {
            return null;
        }
        return ToPixelLength(value, scaling);
    }

    private static int ToPixelMargin(double value, double scaling)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            return 0;
        }
        return Math.Max(0, (int)Math.Ceiling(value * scaling));
    }
}
