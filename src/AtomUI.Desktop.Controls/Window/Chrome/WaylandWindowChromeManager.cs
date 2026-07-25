using System.Runtime.Versioning;
using AtomUI.Native;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

[SupportedOSPlatform("linux")]
internal sealed class WaylandWindowChromeManager : AbstractLinuxWindowChromeManager
{
    private const double ManagedResizeGripScale = 1.0 / 3.0;

    private Thickness _managedResizeGripThickness;
    private bool _hasManagedResizeGrip;

    public WaylandWindowChromeManager(Window window)
        : base(window)
    {
    }

    public override bool UsesCustomResizer => true;

    protected override void AttachPlatformHooks()
    {
        Window.PropertyChanged += HandleWindowPropertyChanged;
    }

    protected override void UpdatePlatformFrameGeometry()
    {
        UpdateShadowExtents();
        UpdateMinMaxSizeHints();

        _hasManagedResizeGrip = Window.TryTakeOverManagedResizeGrip(
            ManagedResizeGripScale,
            out _managedResizeGripThickness);
        if (_hasManagedResizeGrip)
        {
            Window.ConfigureManagedResizeGrip(_managedResizeGripThickness);
        }

        UpdateInputRegion();
    }

    private void UpdateShadowExtents()
    {
        Window.PlatformImpl?.SetShadowExtents(ResolveShadowExtents());
    }

    private void UpdateMinMaxSizeHints()
    {
        var shadowExtents = ResolveShadowExtents();
        var minSize = CalculateGeometryConstraint(
            new Size(Window.MinWidth, Window.MinHeight),
            shadowExtents);
        var maxSize = CalculateGeometryConstraint(
            new Size(Window.MaxWidth, Window.MaxHeight),
            shadowExtents);

        Window.PlatformImpl?.SetMinMaxSize(minSize, maxSize);
    }

    private Thickness ResolveShadowExtents()
    {
        return Window.IsCsdEnabled && Window.WindowState == WindowState.Normal
            ? NormalizeShadowExtents(Window.FrameShadowThickness)
            : default;
    }

    private void HandleWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Visual.BoundsProperty)
        {
            RequestFrameGeometryUpdate();
        }
    }

    private void UpdateInputRegion()
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

    internal static Thickness NormalizeShadowExtents(Thickness shadowExtents)
    {
        return new Thickness(
            NormalizeShadowExtent(shadowExtents.Left),
            NormalizeShadowExtent(shadowExtents.Top),
            NormalizeShadowExtent(shadowExtents.Right),
            NormalizeShadowExtent(shadowExtents.Bottom));
    }

    internal static Size CalculateGeometryConstraint(
        Size clientConstraint,
        Thickness shadowExtents)
    {
        return new Size(
            CalculateGeometryConstraintAxis(
                clientConstraint.Width,
                shadowExtents.Left + shadowExtents.Right),
            CalculateGeometryConstraintAxis(
                clientConstraint.Height,
                shadowExtents.Top + shadowExtents.Bottom));
    }

    private static double NormalizeShadowExtent(double value)
    {
        return Math.Max(0, Math.Round(value, MidpointRounding.AwayFromZero));
    }

    private static double CalculateGeometryConstraintAxis(double value, double shadowExtent)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            return value;
        }

        return Math.Max(1, value - Math.Max(0, shadowExtent));
    }
}
