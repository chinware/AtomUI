using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace AtomUI.Native.Linux;

[SupportedOSPlatform("linux")]
internal static class WindowUtilsInterop
{
    #region XCB基本类型和常量
        
    // 基础句柄类型
    internal struct xcb_connection_t { }
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct xcb_window_t { public uint Id; }
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct xcb_void_cookie_t { public uint Sequence; }
    
    // 矩形结构
    [StructLayout(LayoutKind.Sequential)]
    internal struct xcb_rectangle_t
    {
        public short X;
        public short Y;
        public ushort Width;
        public ushort Height;
    }
    
    // 枚举类型（对应C中的typedef enum）
    internal enum xcb_shape_op_t : byte
    {
        XCB_SHAPE_SO_SET = 0,
        XCB_SHAPE_SO_UNION = 1,
        XCB_SHAPE_SO_INTERSECT = 2,
        XCB_SHAPE_SO_SUBTRACT = 3,
        XCB_SHAPE_SO_INVERT = 4
    }
    
    internal enum xcb_shape_kind_t : byte
    {
        XCB_SHAPE_SK_BOUNDING = 0,
        XCB_SHAPE_SK_CLIP = 1,
        XCB_SHAPE_SK_INPUT = 2
    }
    
    internal enum xcb_clip_ordering_t : byte
    {
        XCB_CLIP_ORDERING_UNSORTED = 0,
        XCB_CLIP_ORDERING_Y_SORTED = 1,
        XCB_CLIP_ORDERING_YX_SORTED = 2,
        XCB_CLIP_ORDERING_YX_BANDED = 3
    }
    
    // 错误处理
    [StructLayout(LayoutKind.Sequential)]
    internal struct xcb_generic_error_t
    {
        public byte ResponseType;
        public byte ErrorCode;
        public ushort Sequence;
        public uint ResourceId;
        public ushort MinorCode;
        public byte MajorCode;
        public byte Pad0;
        public uint FullSequence;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct xcb_get_geometry_reply_t
    {
        public byte ResponseType;
        public byte Depth;
        public ushort Sequence;
        public uint Length;
        public short X;
        public short Y;
        public ushort Width;
        public ushort Height;
        public ushort BorderWidth;
        public byte Pad0;
        public byte Pad1;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct xcb_shape_get_rectangles_reply_t
    {
        public byte ResponseType;
        public byte Ordering;
        public ushort Sequence;
        public uint Length;
        public uint RectanglesLen;
        public uint Pad0;
        public uint Pad1;
        public uint Pad2;
    }
    
    #endregion
        
    #region XCB核心函数P/Invoke声明
        
    [DllImport("libxcb.so.1", EntryPoint = "xcb_connect")]
    internal static extern IntPtr xcb_connect(string display, IntPtr screen);
        
    [DllImport("libxcb.so.1", EntryPoint = "xcb_connection_has_error")]
    internal static extern int xcb_connection_has_error(IntPtr connection);
        
    [DllImport("libxcb.so.1", EntryPoint = "xcb_disconnect")]
    internal static extern void xcb_disconnect(IntPtr connection);
        
    [DllImport("libxcb.so.1", EntryPoint = "xcb_flush")]
    internal static extern int xcb_flush(IntPtr connection);
        
    [DllImport("libxcb.so.1", EntryPoint = "xcb_request_check")]
    internal static extern IntPtr xcb_request_check(IntPtr connection, xcb_void_cookie_t cookie);
        
    #endregion
        
    #region 窗口几何函数
        
    [DllImport("libxcb.so.1", EntryPoint = "xcb_get_geometry")]
    internal static extern uint xcb_get_geometry(IntPtr connection, uint window);
        
    [DllImport("libxcb.so.1", EntryPoint = "xcb_get_geometry_reply")]
    internal static extern IntPtr xcb_get_geometry_reply(IntPtr connection, uint cookie, IntPtr error);
        
    #endregion
        
    #region 形状扩展函数
        
    // 查询形状扩展版本
    [DllImport("libxcb-shape.so.0", EntryPoint = "xcb_shape_query_version")]
    internal static extern uint xcb_shape_query_version(IntPtr connection);
        
    [StructLayout(LayoutKind.Sequential)]
    internal struct xcb_shape_query_version_reply_t
    {
        public byte ResponseType;
        public byte Pad0;
        public ushort Sequence;
        public uint Length;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public uint Pad1;
    }
        
    [DllImport("libxcb-shape.so.0", EntryPoint = "xcb_shape_query_version_reply")]
    internal static extern IntPtr xcb_shape_query_version_reply(
        IntPtr connection, uint cookie, IntPtr error);
        
    [DllImport("libxcb-shape.so.0", EntryPoint = "xcb_shape_rectangles_checked")]
    internal static extern xcb_void_cookie_t xcb_shape_rectangles_checked(
        IntPtr c,                          // xcb_connection_t*
        xcb_shape_op_t operation,          // xcb_shape_op_t (实际是byte)
        xcb_shape_kind_t destination_kind, // xcb_shape_kind_t (实际是byte)
        byte ordering,                     // uint8_t
        uint destination_window,           // xcb_window_t (底层是uint32_t)
        short x_offset,                    // int16_t
        short y_offset,                    // int16_t
        uint rectangles_len,               // uint32_t
        IntPtr rectangles                  // const xcb_rectangle_t* (指针)
    );
        
    // 查询形状矩形
    [DllImport("libxcb-shape.so.0", EntryPoint = "xcb_shape_get_rectangles")]
    internal static extern uint xcb_shape_get_rectangles(
        IntPtr connection, uint window, xcb_shape_kind_t shapeKind);
        
    [DllImport("libxcb-shape.so.0", EntryPoint = "xcb_shape_get_rectangles_reply")]
    internal static extern IntPtr xcb_shape_get_rectangles_reply(
        IntPtr connection, uint cookie, IntPtr error);
        
    [DllImport("libxcb-shape.so.0", EntryPoint = "xcb_shape_get_rectangles_rectangles_length")]
    internal static extern int xcb_shape_get_rectangles_rectangles_length(IntPtr reply);
        
    #endregion
}