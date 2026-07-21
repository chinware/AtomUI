using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

[SupportedOSPlatform("linux")]
internal sealed class WaylandWindowChromeManager : LinuxWindowChromeManager
{
    private const double ManagedResizeGripScale = 1.0 / 3.0;

    private Thickness _managedResizeGripThickness;
    private bool _hasManagedResizeGrip;

    public WaylandWindowChromeManager(Window window)
        : base(window)
    {
        // Temporary workaround for Avalonia's Wayland fractional-scale resize path.
        // Avalonia 12.1 can quantize the logical root size, viewport destination, and
        // physical render buffer independently.  During a live resize this can leave
        // the buffer one physical pixel short, which exposes a seam at the right or
        // bottom edge of full-window overlays.  UseLayoutRounding is inherited from
        // the Window; disabling it here prevents the affected root layout path from
        // producing the fractional logical sizes that no longer match Avalonia's
        // Wayland buffer/viewport rounding, without changing Windows, macOS, or X11.
        //
        // TODO(Avalonia upgrade): Re-check Avalonia's upstream Wayland fractional-scale
        // resize issue before changing AvaloniaVersion and remove this workaround once
        // buffer allocation, viewport destination, and window geometry share one
        // quantization rule:
        // https://github.com/AvaloniaUI/Avalonia/issues?q=is%3Aissue+wayland+fractional+scale
        Window.UseLayoutRounding = false;
    }

    public override bool UsesCustomResizer => true;

    protected override void AttachPlatformHooks()
    {
        Window.PropertyChanged += HandleWindowPropertyChanged;
    }

    protected override void UpdatePlatformFrameGeometry()
    {
        UpdateWaylandShadowExtents();

        _hasManagedResizeGrip = Window.TryTakeOverManagedResizeGrip(
            ManagedResizeGripScale,
            out _managedResizeGripThickness);
        if (_hasManagedResizeGrip)
        {
            Window.ConfigureManagedResizeGrip(_managedResizeGripThickness);
        }

        UpdateWaylandInputRegion();
    }

    private void UpdateWaylandShadowExtents()
    {
        var shadowExtents = Window.IsCsdEnabled && Window.WindowState == WindowState.Normal
            ? NormalizeWaylandShadowExtents(Window.FrameShadowThickness)
            : default;
        Window.PlatformImpl?.SetShadowExtents(shadowExtents);
    }

    private void HandleWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Visual.BoundsProperty)
        {
            RequestFrameGeometryUpdate();
        }
    }

    private void UpdateWaylandInputRegion()
    {
        var region = CalculateInputRegion(
            Window.ClientSize,
            Window.FrameShadowThickness,
            _managedResizeGripThickness,
            Window.CanResize,
            Window.IsCsdEnabled && _hasManagedResizeGrip,
            Window.WindowState);

        Window.TrySetWaylandInputRectangle(region.X, region.Y, region.Width, region.Height);
    }

    internal static PixelRect CalculateInputRegion(
        Size surfaceSize,
        Thickness shadowThickness,
        Thickness resizeGripThickness,
        bool canResize,
        bool isCsdEnabled,
        WindowState windowState)
    {
        var surfaceWidth  = Math.Max(1, (int)Math.Round(surfaceSize.Width));
        var surfaceHeight = Math.Max(1, (int)Math.Round(surfaceSize.Height));
        if (!isCsdEnabled || windowState != WindowState.Normal)
        {
            return new PixelRect(0, 0, surfaceWidth, surfaceHeight);
        }

        var effectiveGrip = canResize ? resizeGripThickness : default;
        var insetLeft     = Math.Max(0, shadowThickness.Left - effectiveGrip.Left);
        var insetTop      = Math.Max(0, shadowThickness.Top - effectiveGrip.Top);
        var insetRight    = Math.Max(0, shadowThickness.Right - effectiveGrip.Right);
        var insetBottom   = Math.Max(0, shadowThickness.Bottom - effectiveGrip.Bottom);

        var left   = Math.Clamp((int)Math.Ceiling(insetLeft), 0, surfaceWidth - 1);
        var top    = Math.Clamp((int)Math.Ceiling(insetTop), 0, surfaceHeight - 1);
        var right  = Math.Clamp((int)Math.Floor(surfaceWidth - insetRight), left + 1, surfaceWidth);
        var bottom = Math.Clamp((int)Math.Floor(surfaceHeight - insetBottom), top + 1, surfaceHeight);
        return new PixelRect(left, top, right - left, bottom - top);
    }

    internal static Thickness NormalizeWaylandShadowExtents(Thickness shadowExtents)
    {
        return new Thickness(
            NormalizeWaylandShadowExtent(shadowExtents.Left),
            NormalizeWaylandShadowExtent(shadowExtents.Top),
            NormalizeWaylandShadowExtent(shadowExtents.Right),
            NormalizeWaylandShadowExtent(shadowExtents.Bottom));
    }

    private static double NormalizeWaylandShadowExtent(double value)
    {
        return Math.Max(0, Math.Round(value, MidpointRounding.AwayFromZero));
    }
}
