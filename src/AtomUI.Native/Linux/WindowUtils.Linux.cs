using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using AtomUI.Native.Linux;
using Avalonia.Controls;

namespace AtomUI.Native;

[SupportedOSPlatform("linux")]
internal static class WindowUtilsLinux
{
    public static void SetWindowIgnoreMouseEventsLinux(IntPtr handle, bool flag)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle");
        }

        if (!XcbConnectionHolder.IsShapeSupported)
        {
            throw new InvalidOperationException("SHAPE extension is unavailable");
        }

        uint windowId = (uint)handle.ToInt64();
        IntPtr connection = XcbConnectionHolder.Connection;

        WindowUtilsInterop.xcb_void_cookie_t cookie;

        if (flag)
        {
            // 启用鼠标事件穿透：设置输入形状为空
            lock (XcbConnectionHolder.SyncRoot)
            {
                cookie = WindowUtilsInterop.xcb_shape_rectangles_checked(
                    connection,
                    WindowUtilsInterop.xcb_shape_op_t.XCB_SHAPE_SO_SET,
                    WindowUtilsInterop.xcb_shape_kind_t.XCB_SHAPE_SK_INPUT,
                    (byte)WindowUtilsInterop.xcb_clip_ordering_t.XCB_CLIP_ORDERING_UNSORTED,
                    windowId,
                    0,
                    0,
                    0, // num_rects = 0 表示空区域
                    IntPtr.Zero // rects = nullptr
                );

                CheckRequest(connection, cookie);
                WindowUtilsInterop.xcb_flush(connection);
            }
        }
        else
        {
            // 禁用鼠标事件穿透：恢复默认输入形状（整个窗口）
            lock (XcbConnectionHolder.SyncRoot)
            {
                IntPtr geomReplyPtr = GetWindowGeometry(connection, windowId);
                if (geomReplyPtr == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Failed to retrieve window geometry information");
                }

                try
                {
                    var geometry = Marshal.PtrToStructure<WindowUtilsInterop.xcb_get_geometry_reply_t>(geomReplyPtr);

                    var rect = new WindowUtilsInterop.xcb_rectangle_t
                    {
                        X      = 0,
                        Y      = 0,
                        Width  = geometry.Width,
                        Height = geometry.Height
                    };

                    IntPtr rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                    try
                    {
                        Marshal.StructureToPtr(rect, rectPtr, false);

                        cookie = WindowUtilsInterop.xcb_shape_rectangles_checked(
                            connection,
                            WindowUtilsInterop.xcb_shape_op_t.XCB_SHAPE_SO_SET,
                            WindowUtilsInterop.xcb_shape_kind_t.XCB_SHAPE_SK_INPUT,
                            (byte)WindowUtilsInterop.xcb_clip_ordering_t.XCB_CLIP_ORDERING_UNSORTED,
                            windowId,
                            0,
                            0,
                            1,
                            rectPtr
                        );

                        CheckRequest(connection, cookie);
                        WindowUtilsInterop.xcb_flush(connection);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(rectPtr);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(geomReplyPtr);
                }
            }
        }
    }
    
    public static bool IsWindowIgnoreMouseEventsLinux(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle");
        }

        if (!XcbConnectionHolder.IsShapeSupported)
        {
            throw new InvalidOperationException("SHAPE extension is unavailable");
        }

        uint windowId = (uint)handle.ToInt64();
        IntPtr connection = XcbConnectionHolder.Connection;

        lock (XcbConnectionHolder.SyncRoot)
        {
            uint rectsCookie = WindowUtilsInterop.xcb_shape_get_rectangles(connection, windowId,
                WindowUtilsInterop.xcb_shape_kind_t.XCB_SHAPE_SK_INPUT);
            IntPtr errorPtr      = IntPtr.Zero;
            IntPtr rectsReplyPtr = WindowUtilsInterop.xcb_shape_get_rectangles_reply(connection, rectsCookie, errorPtr);

            if (errorPtr != IntPtr.Zero)
            {
                var error = Marshal.PtrToStructure<WindowUtilsInterop.xcb_generic_error_t>(errorPtr);
                Marshal.FreeHGlobal(errorPtr);
                throw new InvalidOperationException($"Shape lookup error: {error.ErrorCode}");
            }

            if (rectsReplyPtr == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                int numRects = WindowUtilsInterop.xcb_shape_get_rectangles_rectangles_length(rectsReplyPtr);
                // 矩形数量为 0 表示输入区域为空（鼠标穿透）
                return numRects == 0;
            }
            finally
            {
                Marshal.FreeHGlobal(rectsReplyPtr);
            }
        }
    }
    
    private static void CheckRequest(IntPtr connection, WindowUtilsInterop.xcb_void_cookie_t cookie)
    {
        IntPtr errorPtr = WindowUtilsInterop.xcb_request_check(connection, cookie);
        if (errorPtr != IntPtr.Zero)
        {
            var error = Marshal.PtrToStructure<WindowUtilsInterop.xcb_generic_error_t>(errorPtr);
            Marshal.FreeHGlobal(errorPtr);
            Console.Error.WriteLine($"Shape manipulation error: {error.ErrorCode}");
        }
    }
    
    private static IntPtr GetWindowGeometry(IntPtr connection, uint windowId)
    {
        uint geomCookie = WindowUtilsInterop.xcb_get_geometry(connection, windowId);
        return WindowUtilsInterop.xcb_get_geometry_reply(connection, geomCookie, IntPtr.Zero);
    }

    /// <summary>
    /// 获取 Linux 系统默认的标题栏高度。
    /// 使用 Avalonia 的 WindowDecorationMargin 属性获取标题栏高度。
    /// </summary>
    /// <returns>非 Linux 或无窗口句柄时返回 <c>null</c>。</returns>
    public static double? GetSystemTitleBarHeightLinux(Window window)
    {
        if (!OperatingSystem.IsLinux())
        {
            return null;
        }

        // 使用 Avalonia 提供的 WindowDecorationMargin 属性
        // Top 值就是标题栏高度
        var margin = window.WindowDecorationMargin;
        return margin.Top > 0 ? margin.Top : null;
    }
    
    /// <summary>
    /// 基于 XCB SHAPE 扩展的 X11 窗体输入区域操作。
    /// 将（视觉透明的）阴影缓冲区从输入区中裁掉，使阴影区域的鼠标事件穿透到下层窗口。
    /// 所有坐标均为 <b>device pixel</b>（X11 原生坐标），非 DIP。
    /// 内部连接由 <see cref="XcbConnectionHolder"/> 提供。
    /// </summary>
    /// <summary>X 服务器是否暴露 SHAPE 扩展且本地连接已建立。</summary>
    public static bool IsSupported => XcbConnectionHolder.IsShapeSupported;

    /// <summary>
    /// 将窗体的 X11 input region 替换为指定矩形。矩形外的像素事件将穿透到下层窗口（含跨进程）。
    /// </summary>
    public static void SetInputRectangle(IntPtr handle, int x, int y, int width, int height)
    {
        if (handle == IntPtr.Zero) return;
        if (width <= 0 || height <= 0) return;
        if (!IsSupported) return;

        uint windowId = (uint)handle.ToInt64();

        var rect = new WindowUtilsInterop.xcb_rectangle_t
        {
            X      = (short)Math.Clamp(x, short.MinValue, short.MaxValue),
            Y      = (short)Math.Clamp(y, short.MinValue, short.MaxValue),
            Width  = (ushort)Math.Min(width, ushort.MaxValue),
            Height = (ushort)Math.Min(height, ushort.MaxValue)
        };

        IntPtr rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
        try
        {
            Marshal.StructureToPtr(rect, rectPtr, false);

            lock (XcbConnectionHolder.SyncRoot)
            {
                var connection = XcbConnectionHolder.Connection;
                var cookie = WindowUtilsInterop.xcb_shape_rectangles_checked(
                    connection,
                    WindowUtilsInterop.xcb_shape_op_t.XCB_SHAPE_SO_SET,
                    WindowUtilsInterop.xcb_shape_kind_t.XCB_SHAPE_SK_INPUT,
                    (byte)WindowUtilsInterop.xcb_clip_ordering_t.XCB_CLIP_ORDERING_UNSORTED,
                    windowId,
                    0,
                    0,
                    1,
                    rectPtr
                );

                IntPtr errorPtr = WindowUtilsInterop.xcb_request_check(connection, cookie);
                if (errorPtr != IntPtr.Zero)
                {
                    var error = Marshal.PtrToStructure<WindowUtilsInterop.xcb_generic_error_t>(errorPtr);
                    Marshal.FreeHGlobal(errorPtr);
                    Console.Error.WriteLine($"Shape manipulation error: {error.ErrorCode}");
                }

                WindowUtilsInterop.xcb_flush(connection);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(rectPtr);
        }
    }

    /// <summary>
    /// 将窗体的输入区域重置为整个客户区。
    /// </summary>
    public static void ResetInputRegion(IntPtr handle, int width, int height)
    {
        SetInputRectangle(handle, 0, 0, width, height);
    }
}