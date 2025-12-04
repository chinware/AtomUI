using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using AtomUI.Native.Linux;

namespace AtomUI.Native;

internal static partial class WindowExtensions
{
    [SupportedOSPlatform("linux")]
    public static void SetWindowIgnoreMouseEventsLinux(IntPtr handle, bool flag)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle");
        }

        // 转换为X11窗口ID（uint类型）
        uint windowId = (uint)handle.ToInt64();

        // 连接到X服务器
        IntPtr connection = WindowUtilsInterop.xcb_connect(string.Empty, IntPtr.Zero);
        if (connection == IntPtr.Zero || WindowUtilsInterop.xcb_connection_has_error(connection) != 0)
        {
            throw new InvalidOperationException("XCB connection error");
        }

        try
        {
            // 检查形状扩展是否可用
            if (!IsShapeExtensionAvailable(connection))
            {
                throw new InvalidOperationException("SHAPE extension is unavailable");
            }

            WindowUtilsInterop.xcb_void_cookie_t cookie;

            if (flag)
            {
                // 启用鼠标事件穿透：设置输入形状为空
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
            }
            else
            {
                // 禁用鼠标事件穿透：恢复默认输入形状（整个窗口）
                IntPtr geomReplyPtr = GetWindowGeometry(connection, windowId);
                if (geomReplyPtr == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Failed to retrieve window geometry information");
                }

                try
                {
                    var geometry = Marshal.PtrToStructure<WindowUtilsInterop.xcb_get_geometry_reply_t>(geomReplyPtr);

                    // 创建覆盖整个窗口的矩形
                    var rect = new WindowUtilsInterop.xcb_rectangle_t
                    {
                        X      = 0,
                        Y      = 0,
                        Width  = geometry.Width,
                        Height = geometry.Height
                    };

                    // 分配非托管内存存储矩形
                    IntPtr rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                    try
                    {
                        Marshal.StructureToPtr(rect, rectPtr, false);

                        // 设置输入形状为整个窗口
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

            // 检查操作是否出错
            IntPtr errorPtr = WindowUtilsInterop.xcb_request_check(connection, cookie);
            if (errorPtr != IntPtr.Zero)
            {
                var error = Marshal.PtrToStructure<WindowUtilsInterop.xcb_generic_error_t>(errorPtr);
                Marshal.FreeHGlobal(errorPtr);
                Console.Error.WriteLine($"Shape manipulation error: {error.ErrorCode}");
            }

            // 刷新请求
            WindowUtilsInterop.xcb_flush(connection);
        }
        finally
        {
            WindowUtilsInterop.xcb_disconnect(connection);
        }
    }

    [SupportedOSPlatform("linux")]
    public static bool IsWindowIgnoreMouseEventsLinux(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle");
        }

        uint windowId = (uint)handle.ToInt64();

        IntPtr connection = WindowUtilsInterop.xcb_connect(string.Empty, IntPtr.Zero);
        if (connection == IntPtr.Zero || WindowUtilsInterop.xcb_connection_has_error(connection) != 0)
        {
            throw new InvalidOperationException("XCB connection error");
        }

        try
        {
            // 检查形状扩展是否可用
            if (!IsShapeExtensionAvailable(connection))
            {
                throw new InvalidOperationException("SHAPE extension is unavailable");
            }

            // 查询窗口的输入形状
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
                // 获取矩形数量
                int numRects = WindowUtilsInterop.xcb_shape_get_rectangles_rectangles_length(rectsReplyPtr);
                // 如果矩形数量为0，表示输入区域为空（鼠标穿透）
                return numRects == 0;
            }
            finally
            {
                Marshal.FreeHGlobal(rectsReplyPtr);
            }
        }
        finally
        {
            WindowUtilsInterop.xcb_disconnect(connection);
        }
    }

    [SupportedOSPlatform("linux")]
    private static bool IsShapeExtensionAvailable(IntPtr connection)
    {
        uint   shapeCookie   = WindowUtilsInterop.xcb_shape_query_version(connection);
        IntPtr errorPtr      = IntPtr.Zero;
        IntPtr shapeReplyPtr = WindowUtilsInterop.xcb_shape_query_version_reply(connection, shapeCookie, errorPtr);

        if (errorPtr != IntPtr.Zero)
        {
            var error = Marshal.PtrToStructure<WindowUtilsInterop.xcb_generic_error_t>(errorPtr);
            Console.Error.WriteLine($"SHAPE extension error: {error.ErrorCode}");
            Marshal.FreeHGlobal(errorPtr);
            return false;
        }

        if (shapeReplyPtr == IntPtr.Zero)
        {
            return false;
        }

        Marshal.FreeHGlobal(shapeReplyPtr);
        return true;
    }

    [SupportedOSPlatform("linux")]
    private static IntPtr GetWindowGeometry(IntPtr connection, uint windowId)
    {
        uint geomCookie = WindowUtilsInterop.xcb_get_geometry(connection, windowId);
        return WindowUtilsInterop.xcb_get_geometry_reply(connection, geomCookie, IntPtr.Zero);
    }
}