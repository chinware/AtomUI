using System.Runtime.Versioning;
using AtomUI.Native.Windows;
using Avalonia.Controls;

namespace AtomUI.Native;

[SupportedOSPlatform("windows")]
internal static class WindowUtilsWindows
{
    private const int Windows10DarkFrameMinimumBuild = 17763;
    private const int Windows10DarkFrame20H1AttributeBuild = 18985;

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

    public static void SetWindowFrameDarkModeWindows(IntPtr handle, bool isDarkMode)
    {
        if (handle == IntPtr.Zero ||
            !OperatingSystem.IsWindowsVersionAtLeast(10, 0, Windows10DarkFrameMinimumBuild) ||
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            return;
        }

        var value = isDarkMode ? 1 : 0;
        if (!TrySetWindowFrameDarkMode(handle, SelectDarkModeAttribute(), value) &&
            !TrySetWindowFrameDarkMode(handle, SelectFallbackDarkModeAttribute(), value))
        {
            return;
        }

        ReapplyActiveNonClientFrame(handle);
    }

    private static int SelectDarkModeAttribute()
    {
        return Environment.OSVersion.Version.Build >= Windows10DarkFrame20H1AttributeBuild
            ? WindowUtilsInterop.DWMWA_USE_IMMERSIVE_DARK_MODE
            : WindowUtilsInterop.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
    }

    private static int SelectFallbackDarkModeAttribute()
    {
        return Environment.OSVersion.Version.Build >= Windows10DarkFrame20H1AttributeBuild
            ? WindowUtilsInterop.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1
            : WindowUtilsInterop.DWMWA_USE_IMMERSIVE_DARK_MODE;
    }

    private static bool TrySetWindowFrameDarkMode(IntPtr handle, int attribute, int value)
    {
        return WindowUtilsInterop.DwmSetWindowAttribute(
            handle,
            attribute,
            ref value,
            sizeof(int)) == WindowUtilsInterop.S_OK;
    }

    private static void ReapplyActiveNonClientFrame(IntPtr handle)
    {
        // Avalonia suppresses default non-client activation handling for CSD windows.
        // Let Windows repaint the active non-client frame once so the Win10 DWM
        // dark-frame attribute is reflected immediately instead of after focus changes.
        WindowUtilsInterop.DefWindowProc(
            handle,
            WindowUtilsInterop.WM_NCACTIVATE,
            IntPtr.Zero,
            IntPtr.Zero);
        WindowUtilsInterop.DefWindowProc(
            handle,
            WindowUtilsInterop.WM_NCACTIVATE,
            new IntPtr(1),
            IntPtr.Zero);
    }
}
