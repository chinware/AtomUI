using System.Runtime.Versioning;
using AtomUI.Native.MacOS;
using Avalonia.Controls;

namespace AtomUI.Native;

/// <summary>单个 standard window button 在其 superview 中的矩形 (Cocoa 坐标，原点左下)。</summary>
internal readonly record struct ButtonFrame(double X, double Y, double Width, double Height);

[SupportedOSPlatform("macos")]
internal static class WindowUtilsMacOS
{
    public static void SetWindowIgnoreMouseEventsMacOS(IntPtr handle, bool flag)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle");
        }
        WindowUtilsInterop.objc_msgSend_void_bool(handle, WindowUtilsInterop.SetIgnoresMouseEventsSelector, flag);
    }
    
    public static bool IsWindowIgnoreMouseEventsMacOS(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle");
        }
        return WindowUtilsInterop.objc_msgSend_bool(handle, WindowUtilsInterop.IgnoresMouseEventsSelector);
    }

    // ------------------------------ MacStandardWindowButtons API ------------------------------

    private static IntPtr GetNSWindow(Window window)
    {
        var handle = window.TryGetPlatformHandle();
        return handle?.Handle ?? IntPtr.Zero;
    }
    
    private static IntPtr GetButton(IntPtr ns, WindowUtilsInterop.NSWindowButton which)
    {
        if (ns == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }
        return WindowUtilsInterop.GetStandardWindowButton(ns, (long)which);
    }

    /// <summary>
    /// 读取某个 standard window button 的当前 frame (Cocoa 坐标，原点在 superview 左下)。
    /// </summary>
    /// <returns>非 macOS、无 NSWindow 句柄或按钮不存在时返回 <c>null</c>。</returns>
    public static ButtonFrame? GetStandardWindowButtonFrame(this Window window, WindowUtilsInterop.NSWindowButton button)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return null;
        }

        var ns  = GetNSWindow(window);
        var btn = GetButton(ns, button);
        if (btn == IntPtr.Zero)
        {
            return null;
        }

        var f = WindowUtilsInterop.GetFrame(btn);
        return new ButtonFrame(f.Origin.X, f.Origin.Y, f.Size.Width, f.Size.Height);
    }

    /// <summary>
    /// 获取 macOS 系统默认的标题栏高度。
    /// 通过 superview 高度和 contentView 高度的差值来计算。
    /// </summary>
    /// <returns>非 macOS 或无窗口句柄时返回 <c>null</c>。</returns>
    public static double? GetSystemTitleBarHeightMacOS(Window window)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return null;
        }

        var ns = GetNSWindow(window);
        if (ns == IntPtr.Zero)
        {
            return null;
        }

        // 获取 window 的 frame (包含标题栏)
        var windowFrame = WindowUtilsInterop.GetFrame(ns);

        // 获取 contentLayoutRect：这是不包含标题栏的内容区域
        var selContentLayoutRect = WindowUtilsInterop.sel_registerName("contentLayoutRect");
        var contentRect = WindowUtilsInterop.GetRect(ns, selContentLayoutRect);

        // 标题栏高度 = 窗口总高度 - 内容布局区域高度
        var titleBarHeight = windowFrame.Size.Height - contentRect.Size.Height;
        return titleBarHeight > 0 ? titleBarHeight : null;
    }

    /// <summary>
    /// 获取 Standard window buttons 的原始位置（未调整前的默认位置）。
    /// 返回 close 按钮的 X 和 Y 坐标 (Cocoa 坐标，原点左下)。
    /// </summary>
    /// <returns>非 macOS、无 NSWindow 句柄或按钮不存在时返回 <c>null</c>。</returns>
    public static (double X, double Y) GetStandardWindowButtonsOriginalPosition(this Window window)
    {
        var closeFrame = window.GetStandardWindowButtonFrame(WindowUtilsInterop.NSWindowButton.CloseButton);
        if (closeFrame is not { } cf)
        {
            return (0, 0);
        }

        return (cf.X, cf.Y);
    }

    /// <summary>
    /// 获取 Standard window buttons 占用的总宽度（从最左边的按钮到最右边的按钮）。
    /// 用于计算自定义标题栏需要为按钮预留的左侧 margin。
    /// </summary>
    /// <param name="window">目标窗口。</param>
    /// <param name="spacing">按钮之间的间距，默认为 20pt（AppKit 原生值）。</param>
    /// <returns>按钮占用的总宽度（包括间距），如果无法获取则返回 <c>null</c>。</returns>
    public static double? GetStandardWindowButtonsTotalWidth(this Window window, double spacing = 20.0)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return null;
        }

        var closeFrame = window.GetStandardWindowButtonFrame(WindowUtilsInterop.NSWindowButton.CloseButton);
        var miniaturizeFrame = window.GetStandardWindowButtonFrame(WindowUtilsInterop.NSWindowButton.MiniaturizeButton);
        var zoomFrame = window.GetStandardWindowButtonFrame(WindowUtilsInterop.NSWindowButton.ZoomButton);

        if (closeFrame is not { } cf || miniaturizeFrame is not { } mf || zoomFrame is not { } zf)
        {
            return null;
        }

        // 计算从最左边按钮的左边缘到最右边按钮的右边缘的总宽度
        // 通常顺序是：Close, Miniaturize, Zoom
        double leftMost = Math.Min(Math.Min(cf.X, mf.X), zf.X);
        double rightMost = Math.Max(Math.Max(cf.X + cf.Width, mf.X + mf.Width), zf.X + zf.Width);

        return rightMost - leftMost;
    }

    /// <summary>
    /// 获取自定义标题栏内容应该从左边缘偏移多少距离，以避免与 Standard window buttons 重叠。
    /// 这个值包括了按钮的宽度、按钮之间的间距，以及按钮右侧的额外间距。
    /// </summary>
    /// <param name="window">目标窗口。</param>
    /// <param name="spacing">按钮之间的间距，默认为 20pt。</param>
    /// <param name="extraMargin">按钮右侧的额外间距，默认为 8pt。</param>
    /// <returns>建议的左侧 margin 值，如果无法获取则返回 <c>null</c>。</returns>
    public static double? GetRecommendedTitleBarContentLeftMargin(this Window window, double spacing = 20.0, double extraMargin = 8.0)
    {
        var closeFrame = window.GetStandardWindowButtonFrame(WindowUtilsInterop.NSWindowButton.CloseButton);
        var zoomFrame = window.GetStandardWindowButtonFrame(WindowUtilsInterop.NSWindowButton.ZoomButton);

        if (closeFrame is not { } cf || zoomFrame is not { } zf)
        {
            return null;
        }

        // 找到最右边按钮的右边缘
        double rightEdge = Math.Max(cf.X + cf.Width, zf.X + zf.Width);

        // 加上额外的间距
        return rightEdge + extraMargin;
    }

    /// <summary>
    /// 读取按钮 superview (约为 NSThemeFrame) 的高度，近似等于窗口总高度 (含标题栏)。
    /// 用于 Cocoa 底部原点 Y 与 Avalonia 顶部原点 Y 的换算：
    /// <c>cocoaY = superHeight - topY - buttonHeight</c>。
    /// </summary>
    /// <returns>非 macOS 或无窗口句柄时返回 <c>null</c>。</returns>
    public static double? GetStandardWindowButtonSuperviewHeight(this Window window)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return null;
        }

        var ns  = GetNSWindow(window);
        var btn = GetButton(ns, WindowUtilsInterop.NSWindowButton.CloseButton);
        if (btn == IntPtr.Zero)
        {
            return null;
        }

        var super = WindowUtilsInterop.GetSuperview(btn);
        if (super == IntPtr.Zero)
        {
            return null;
        }

        return WindowUtilsInterop.GetFrame(super).Size.Height;
    }

    /// <summary>
    /// 把某个 standard window button 移到 (<paramref name="x"/>, <paramref name="y"/>)，
    /// 坐标为 Cocoa 原生语义 (原点左下)。
    /// 非 macOS、全屏状态或按钮不存在时直接 no-op。
    /// </summary>
    /// <remarks>
    /// 典型用法（顶部原点换算）：
    /// <code>
    /// var superH = window.GetStandardWindowButtonSuperviewHeight();
    /// var frame  = window.GetStandardWindowButtonFrame(WindowUtilsInterop.NSWindowButton.CloseButton);
    /// if (superH is double h &amp;&amp; frame is { } f)
    ///     window.SetStandardWindowButtonOrigin(WindowUtilsInterop.NSWindowButton.CloseButton,
    ///         x: 12, y: h - topY - f.Height);
    /// </code>
    /// </remarks>
    public static void SetStandardWindowButtonOrigin(this Window window, WindowUtilsInterop.NSWindowButton button, double x, double y)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return;
        }
        if (window.WindowState == WindowState.FullScreen)
        {
            return;
        }

        var ns  = GetNSWindow(window);
        var btn = GetButton(ns, button);
        if (btn == IntPtr.Zero)
        {
            return;
        }

        WindowUtilsInterop.SetFrameOrigin(btn, new WindowUtilsInterop.CGPoint(x, y));
    }

    /// <summary>
    /// 把三个 standard window buttons 按一行布局：close 放到 (<paramref name="x"/>, <paramref name="y"/>)，
    /// miniaturize 放到 (x + spacing, y)，zoom 放到 (x + 2·spacing, y)。
    /// 坐标为 Cocoa 原生语义 (原点左下)。非 macOS、全屏或无 NSWindow 时 no-op。
    /// </summary>
    /// <param name="window">目标窗口。</param>
    /// <param name="x">close 按钮距窗口左边缘的 X (points)。</param>
    /// <param name="y">所有按钮共用的 Y，Cocoa 坐标 (从窗口底部向上量)。</param>
    /// <param name="spacing">相邻按钮左边缘间距，AppKit 原生为 20 pt。</param>
    /// <remarks>
    /// 内部等价于：
    /// <code>
    /// window.SetStandardWindowButtonOrigin(NSWindowButton.CloseButton,       x,               y);
    /// window.SetStandardWindowButtonOrigin(NSWindowButton.MiniaturizeButton, x +     spacing, y);
    /// window.SetStandardWindowButtonOrigin(NSWindowButton.ZoomButton,        x + 2 * spacing, y);
    /// </code>
    /// </remarks>
    public static void SetStandardWindowButtonsLayout(this Window window, double x, double y, double spacing = 20.0)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return;
        }
        if (window.WindowState == WindowState.FullScreen)
        {
            return;
        }

        for (int i = 0; i <= 2; i++)
        {
            window.SetStandardWindowButtonOrigin((WindowUtilsInterop.NSWindowButton)i, x + spacing * i, y);
        }

        // 移动按钮后，需要刷新 superview 的 tracking areas 和显示，
        // 否则鼠标 hover 事件仍然在旧位置触发
        var ns = GetNSWindow(window);
        var closeBtn = GetButton(ns, WindowUtilsInterop.NSWindowButton.CloseButton);
        if (closeBtn != IntPtr.Zero)
        {
            var superview = WindowUtilsInterop.GetSuperview(closeBtn);
            WindowUtilsInterop.UpdateTrackingAreas(superview);
            WindowUtilsInterop.SetNeedsDisplay(superview, true);
        }
    }

    /// <summary>
    /// 把三个 standard window buttons 垂直居中到高度为 <paramref name="titleBarHeight"/>
    /// 的扩展标题栏里。本方法是 <see cref="SetStandardWindowButtonsLayout"/> 上的封装 —— 自动读 superview 高度和
    /// close 按钮尺寸，算出居中的 Y 后委托给 <see cref="SetStandardWindowButtonsLayout"/>。
    /// </summary>
    /// <param name="window">目标窗口。</param>
    /// <param name="titleBarHeight">扩展后的标题栏高度 (即 <c>ExtendClientAreaTitleBarHeightHint</c>)。</param>
    /// <param name="leftInset">
    /// 可选：close 按钮距窗口左边缘的 X 像素偏移。<c>null</c> 时沿用 close 当前的 X
    /// (即 AppKit 默认的约 7–8 pt)。
    /// </param>
    /// <param name="spacing">
    /// 可选：相邻按钮左边缘间距。<c>null</c> 时使用 AppKit 原生值 20 pt。
    /// </param>
    public static void CenterStandardWindowButtons(this Window window, double titleBarHeight, double? leftInset = null,
                                                   double? spacing = null)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return;
        }
        if (window.WindowState == WindowState.FullScreen)
        {
            return;
        }

        var superH     = window.GetStandardWindowButtonSuperviewHeight();
        var closeFrame = window.GetStandardWindowButtonFrame(WindowUtilsInterop.NSWindowButton.CloseButton);
        if (superH is not { } sh || closeFrame is not { } cf)
        {
            return;
        }

        double x = leftInset ?? cf.X;
        double y = sh - (titleBarHeight + cf.Height) / 2.0;
        window.SetStandardWindowButtonsLayout(x, y, spacing ?? 20.0);
    }

    public static double? CalculateVerticalCenter(this Window window, double titleBarHeight)
    {
        var superH     = window.GetStandardWindowButtonSuperviewHeight();
        var closeFrame = window.GetStandardWindowButtonFrame(WindowUtilsInterop.NSWindowButton.CloseButton);
        if (superH is not { } sh || closeFrame is not { } cf)
        {
            return null;
        }
        return sh - (titleBarHeight + cf.Height) / 2.0;
    }
}