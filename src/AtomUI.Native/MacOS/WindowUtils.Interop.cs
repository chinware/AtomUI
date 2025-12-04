using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace AtomUI.Native.MacOS;

[SupportedOSPlatform("macos")]
internal static class WindowUtilsInterop
{
    // 结构体定义
    [StructLayout(LayoutKind.Sequential)]
    public struct CGPoint
    {
        public double X;
        public double Y;

        public CGPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct CGSize
    {
        public double Width;
        public double Height;

        public CGSize(double width, double height)
        {
            Width  = width;
            Height = height;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct NSRect
    {
        public CGPoint Origin;
        public CGSize Size;

        public NSRect(CGPoint origin, CGSize size)
        {
            Origin = origin;
            Size   = size;
        }

        public NSRect(double x, double y, double width, double height)
        {
            Origin = new CGPoint(x, y);
            Size   = new CGSize(width, height);
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct NSPoint
    {
        public double X;
        public double Y;

        public NSPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NSSize
    {
        public double Width;
        public double Height;

        public NSSize(double width, double height)
        {
            Width  = width;
            Height = height;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct NSRectFull
    {
        public NSPoint Origin;
        public NSSize Size;

        public NSRectFull(NSPoint origin, NSSize size)
        {
            Origin = origin;
            Size   = size;
        }

        public NSRectFull(double x, double y, double width, double height)
        {
            Origin = new NSPoint(x, y);
            Size   = new NSSize(width, height);
        }
    }
    
    // 按钮类型枚举 - 完整枚举项
    public enum WindowButtonType : long
    {
        CloseButton = 0,
        MinimizeButton = 1,
        ZoomButton = 2,
        ToolbarButton = 3,
        DocumentIconButton = 4,
        DocumentVersionsButton = 5,
        FullScreenButton = 7
    }

    // NSWindowStyleMask 完整枚举
    [Flags]
    public enum NSWindowStyleMask : ulong
    {
        Borderless = 0,
        Titled = 1 << 0,
        Closable = 1 << 1,
        Miniaturizable = 1 << 2,
        Resizable = 1 << 3,
        TexturedBackground = 1 << 8,
        UnifiedTitleAndToolbar = 1 << 12,
        FullScreen = 1 << 14,
        FullSizeContentView = 1 << 15,
        UtilityWindow = 1 << 4,
        DocModalWindow = 1 << 6,
        NonactivatingPanel = 1 << 7,
        HUDWindow = 1 << 13
    }
    
    // NSWindowButton 完整枚举
    public enum NSWindowButton : long
    {
        CloseButton = 0,
        MiniaturizeButton = 1,
        ZoomButton = 2,
        ToolbarButton = 3,
        DocumentIconButton = 4,
        DocumentVersionsButton = 5,
        FullScreenButton = 7
    }
    
    // NSBackingStoreType 枚举
    public enum NSBackingStoreType : ulong
    {
        Buffered = 2
    }
    
    // NSWindowCollectionBehavior 枚举
    [Flags]
    public enum NSWindowCollectionBehavior : ulong
    {
        Default = 0,
        CanJoinAllSpaces = 1 << 0,
        MoveToActiveSpace = 1 << 1,
        Managed = 1 << 2,
        Transient = 1 << 3,
        Stationary = 1 << 4,
        ParticipatesInCycle = 1 << 5,
        IgnoresCycle = 1 << 6,
        FullScreenPrimary = 1 << 7,
        FullScreenAuxiliary = 1 << 8,
        FullScreenNone = 1 << 9,
        FullScreenAllowsTiling = 1 << 11,
        FullScreenDisallowsTiling = 1 << 12
    }
    
    // NSWindowLevel 枚举
    public enum NSWindowLevel : int
    {
        Normal = 0,
        Floating = 3,
        ModalPanel = 8,
        MainMenu = 24,
        StatusBar = 25,
        PopUpMenu = 101,
        ScreenSaver = 1000
    }
    
    // Objective-C 运行时函数
    [DllImport("/usr/lib/libobjc.A.dylib")]
    public static extern IntPtr objc_getClass(string className);

    [DllImport("/usr/lib/libobjc.A.dylib")]
    public static extern IntPtr sel_registerName(string selectorName);

    // objc_msgSend 函数的不同重载
    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr objc_msgSend_intptr(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern IntPtr objc_msgSend_intptr_long(IntPtr receiver, IntPtr selector, long arg);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend_void(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend_void_bool(IntPtr receiver, IntPtr selector, bool arg);
    
    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern bool objc_msgSend_bool(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend_void_rect(IntPtr receiver, IntPtr selector, NSRectFull rect);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern NSRectFull objc_msgSend_rect(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern ulong objc_msgSend_ulong(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend_void_ulong(IntPtr receiver, IntPtr selector, ulong arg);
    
    // Selector字符串缓存
    public static readonly IntPtr SetIgnoresMouseEventsSelector = sel_registerName("setIgnoresMouseEvents:");
    public static readonly IntPtr IgnoresMouseEventsSelector = sel_registerName("ignoresMouseEvents");
    public static readonly IntPtr StandardWindowButtonSelector = sel_registerName("standardWindowButton:");
    public static readonly IntPtr FrameSelector = sel_registerName("frame");
    public static readonly IntPtr SetFrameSelector = sel_registerName("setFrame:");
    public static readonly IntPtr SuperviewSelector = sel_registerName("superview");
    public static readonly IntPtr SetNeedsDisplaySelector = sel_registerName("setNeedsDisplay:");
    public static readonly IntPtr StyleMaskSelector = sel_registerName("styleMask");
    public static readonly IntPtr SetStyleMaskSelector = sel_registerName("setStyleMask:");
    public static readonly IntPtr SetEnabledSelector = sel_registerName("setEnabled:");
    
    // 辅助方法
    public static IntPtr GetStandardWindowButton(IntPtr window, long buttonType)
    {
        return objc_msgSend_intptr_long(window, StandardWindowButtonSelector, buttonType);
    }
    
    public static NSRectFull GetFrame(IntPtr view)
    {
        return objc_msgSend_rect(view, FrameSelector);
    }

    public static void SetFrame(IntPtr view, NSRectFull frame)
    {
        objc_msgSend_void_rect(view, SetFrameSelector, frame);
    }
    
    public static IntPtr GetSuperview(IntPtr view)
    {
        return objc_msgSend_intptr(view, SuperviewSelector);
    }

    public static void SetNeedsDisplay(IntPtr view, bool flag)
    {
        objc_msgSend_void_bool(view, SetNeedsDisplaySelector, flag);
    }

    public static ulong GetStyleMask(IntPtr window)
    {
        return objc_msgSend_ulong(window, StyleMaskSelector);
    }
    
    public static void SetEnabled(IntPtr control, bool enabled)
    {
        objc_msgSend_void_bool(control, SetEnabledSelector, enabled);
    }
}