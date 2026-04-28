using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace AtomUI.Native.Windows;

[SupportedOSPlatform("windows")]
internal static class WindowUtilsInterop
{
    // 常量定义
    public const int GWL_STYLE = -16;
    public const int GWL_EXSTYLE = -20;
    public const long WS_EX_TRANSPARENT = 0x20L;
    public const long WS_EX_LAYERED = 0x80000L;

    // 窗口样式常量
    public const long WS_BORDER = 0x800000L;
    public const long WS_DLGFRAME = 0x400000L;
    public const long WS_CAPTION = WS_BORDER | WS_DLGFRAME;
    public const long WS_SYSMENU = 0x80000L;
    public const long WS_MINIMIZEBOX = 0x20000L;
    public const long WS_MAXIMIZEBOX = 0x10000L;
    public const long WS_MAXIMIZE = 0x01000000L;

    // WM 消息常量
    public const uint WM_NCCALCSIZE = 0x0083;
    public const uint WM_NCHITTEST = 0x0084;
    public const uint WM_SIZE = 0x0005;
    public const uint WM_CAPTURECHANGED = 0x0215;

    // Hit Test 返回值
    public const int HTTRANSPARENT = -1;
    public const int HTMAXBUTTON = 9;
    public const int HTLEFT = 10;
    public const int HTRIGHT = 11;
    public const int HTTOP = 12;
    public const int HTTOPLEFT = 13;
    public const int HTTOPRIGHT = 14;
    public const int HTBOTTOM = 15;
    public const int HTBOTTOMLEFT = 16;
    public const int HTBOTTOMRIGHT = 17;

    // GetSystemMetrics 常量
    public const int SM_CXSIZEFRAME = 32;
    public const int SM_CYSIZEFRAME = 33;
    public const int SM_CXPADDEDBORDER = 92;

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    // DWM 窗口属性
    public const int DWMWA_NCRENDERING_POLICY = 2;
    public const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;

    public const int DWMNCRP_USEWINDOWSTYLE = 0;
    public const int DWMNCRP_DISABLED = 1;
    public const int DWMNCRP_ENABLED = 2;

    public const int DWMWCP_DEFAULT = 0;
    public const int DWMWCP_ROUND = 2;

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NCCALCSIZE_PARAMS
    {
        public RECT rgrc0;
        public RECT rgrc1;
        public RECT rgrc2;
        public IntPtr lppos;
    }

    // SetWindowPos 常量
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOACTIVATE = 0x0010;
    public const uint SWP_FRAMECHANGED = 0x0020;

    // 使用正确的 Windows 类型
    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    public static extern long GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    public static extern long SetWindowLongPtr(IntPtr hWnd, int nIndex, long dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);

    [DllImport("dwmapi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

    [DllImport("dwmapi.dll")]
    public static extern unsafe int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, void* pvAttribute, int cbAttribute);

    public static short GetXLParam(IntPtr lParam) => unchecked((short)(int)((long)lParam & 0xFFFF));
    public static short GetYLParam(IntPtr lParam) => unchecked((short)(int)(((long)lParam >> 16) & 0xFFFF));
}