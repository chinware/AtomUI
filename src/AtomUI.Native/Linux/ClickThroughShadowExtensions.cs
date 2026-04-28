using System.Diagnostics;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Native;

/// <summary>
/// 高层助手：为启用 <c>WindowDrawnDecorations</c> 自绘阴影的窗体挂接点击穿透行为。
///
/// 通过 X11 SHAPE 扩展把（视觉上透明的）阴影缓冲区从窗体输入区中裁掉，
/// 使阴影区域的鼠标事件穿透到下层窗口；同时在可见边框内侧保留一条
/// 窄抓手带，保证用户仍可拖动窗体边缘 resize。
///
/// 仅在 Linux / X11（含 XWayland）下生效；Windows / macOS 上为 no-op。
/// </summary>
[SupportedOSPlatform("linux")]
internal static class ClickThroughShadowExtensions
{
    /// <summary>
    /// 默认在可见边框内侧保留的 resize 抓手带宽度（DIP）。
    /// 外圈 <c>shadowThickness - ResizeBand</c> 完全穿透。
    /// </summary>
    public const double DefaultResizeBand = 6.0;

    /// <summary>
    /// 为窗体挂接 X11 阴影点击穿透行为。
    /// </summary>
    /// <param name="window">目标窗体。</param>
    /// <param name="extraAffectsProperties">额外变动探测的属性列表，继承的 Window 可以使用这个新增额外的变动探测属性</param>
    /// <param name="shadowThicknessGetter">
    /// 每次重算时调用的委托，返回当前阴影缓冲厚度（DIP）。
    /// 典型实现：<c>() => window.FrameShadowThickness</c>。
    /// </param>
    /// <param name="resizeBand">
    /// 在可见边框内侧保留的 resize 抓手带宽度（DIP）。默认 <see cref="DefaultResizeBand"/>。
    /// </param>
    public static void AttachClickThroughShadow(this Window window,
                                                ISet<AvaloniaProperty> extraAffectsProperties,
                                                Func<Thickness> shadowThicknessGetter,
                                                double resizeBand = DefaultResizeBand)
    {
        Debug.Assert(OperatingSystem.IsLinux());

        void Reapply()
        {
            Apply(window, shadowThicknessGetter(), resizeBand);
        }

        window.Opened += (_, _) => Reapply();
        window.PropertyChanged += (_, e) =>
        {
            if (e.Property == Visual.BoundsProperty || 
                e.Property == Window.WindowDecorationMarginProperty || 
                e.Property == Window.WindowStateProperty ||
                e.Property == TopLevel.TransparencyLevelHintProperty ||
                extraAffectsProperties.Contains(e.Property))
            {
                Reapply();
            }
        };
    }

    private static void Apply(Window window, Thickness shadowThickness, double resizeBand)
    {
        // X11 后端的 PlatformHandle 描述符是 "XID"；其它后端跳过。
        var handle = window.TryGetPlatformHandle();
        if (handle is null || handle.HandleDescriptor != "XID")
        {
            return;
        }

        var size = window.ClientSize;
        if (size.Width <= 0 || size.Height <= 0)
        {
            return;
        }

        var scale = window.RenderScaling <= 0 ? 1.0 : window.RenderScaling;
        var fullW = (int)Math.Round(size.Width * scale);
        var fullH = (int)Math.Round(size.Height * scale);

        // 最大化 / 全屏：框架把 ShadowThickness 归零，整个客户区必须可点击。
        if (window.WindowState != WindowState.Normal)
        {
            window.ResetWindowInputRegion(fullW, fullH);
            return;
        }

        var insetLeft   = Math.Max(0, shadowThickness.Left - resizeBand);
        var insetTop    = Math.Max(0, shadowThickness.Top - resizeBand);
        var insetRight  = Math.Max(0, shadowThickness.Right - resizeBand);
        var insetBottom = Math.Max(0, shadowThickness.Bottom - resizeBand);

        if (insetLeft <= 0 && insetTop <= 0 && insetRight <= 0 && insetBottom <= 0)
        {
            window.ResetWindowInputRegion(fullW, fullH);
            return;
        }

        var x = (int)Math.Round(insetLeft * scale);
        var y = (int)Math.Round(insetTop * scale);
        var w = (int)Math.Round((size.Width - insetLeft - insetRight) * scale);
        var h = (int)Math.Round((size.Height - insetTop - insetBottom) * scale);

        window.SetWindowInputRectangle(x, y, w, h);
    }
}
