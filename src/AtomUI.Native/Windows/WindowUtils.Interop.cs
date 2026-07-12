using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace AtomUI.Native.Windows;

[SupportedOSPlatform("windows")]
internal static class WindowUtilsInterop
{
    // 常量定义
    public const int GWL_EXSTYLE = -20;
    public const long WS_EX_TRANSPARENT = 0x20L;
    public const long WS_EX_LAYERED = 0x80000L;

    // WM 消息常量
    public const uint WM_NCHITTEST = 0x0084;
    public const uint WM_CAPTURECHANGED = 0x0215;

    // Hit Test 返回值
    public const int HTMAXBUTTON = 9;

    // 使用正确的 Windows 类型
    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    public static extern long GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    public static extern long SetWindowLongPtr(IntPtr hWnd, int nIndex, long dwNewLong);

}
