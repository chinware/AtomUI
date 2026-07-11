using System.Runtime.Versioning;
using AtomUI.Native;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

[SupportedOSPlatform("linux")]
internal sealed class X11WindowChromeManager : LinuxWindowChromeManager
{
    private const double InitialScreenMargin = 48;

    private static readonly ISet<AvaloniaProperty> s_shadowInputRegionAffectsProperties =
        new HashSet<AvaloniaProperty>
        {
            AvaloniaWindow.CanResizeProperty,
            Window.FrameShadowThicknessProperty,
            Window.IsCsdEnabledProperty
        };

    public X11WindowChromeManager(Window window)
        : base(window)
    {
    }

    protected override void AttachPlatformHooks()
    {
        Window.AttachClickThroughShadow(
            s_shadowInputRegionAffectsProperties,
            () => Window.FrameShadowThickness,
            ClickThroughShadowExtensions.DefaultResizeBand);
    }

    protected override Action? PrepareInitialPlatformShowState()
    {
        ApplyX11CsdFrameExtents();
        var screen     = Window.Screens.ScreenFromPoint(Window.Position) ?? Window.Screens.Primary;
        var clientSize = SynchronizeInitialClientSize(screen);
        if (clientSize is null)
        {
            return null;
        }

        var originalStartupLocation = Window.WindowStartupLocation;
        if (TryGetInitialStartupPosition(clientSize.Value, screen, out var position))
        {
            Window.SetCurrentValue(AvaloniaWindow.WindowStartupLocationProperty, WindowStartupLocation.Manual);
            Window.Position = position;
            Window.ConfigureLinuxInitialWindowGeometry(position, clientSize.Value, screen?.Scaling ?? 1);
            return () => Window.SetCurrentValue(
                AvaloniaWindow.WindowStartupLocationProperty,
                originalStartupLocation);
        }

        Window.ConfigureLinuxInitialWindowGeometry(Window.Position, clientSize.Value, screen?.Scaling ?? 1);
        return null;
    }

    protected override void UpdatePlatformFrameGeometry()
    {
        ApplyX11CsdFrameExtents();
    }

    private Size? SynchronizeInitialClientSize(Screen? screen)
    {
        if (Window.SizeToContent != SizeToContent.Manual)
        {
            return null;
        }

        if (Window.WindowState is WindowState.Minimized or WindowState.Maximized or WindowState.FullScreen)
        {
            return null;
        }

        if (!TryGetExplicitInitialClientSize(out var clientSize))
        {
            return null;
        }

        clientSize = ConstrainInitialClientSizeToScreen(clientSize, screen);

        if (!MathUtils.AreClose(Window.Width, clientSize.Width))
        {
            Window.Width = clientSize.Width;
        }

        if (!MathUtils.AreClose(Window.Height, clientSize.Height))
        {
            Window.Height = clientSize.Height;
        }

        if (Window.ClientSize != clientSize)
        {
            Window.SetPlatformChromeClientSize(clientSize);
        }

        return clientSize;
    }

    private Size ConstrainInitialClientSizeToScreen(Size clientSize, Screen? screen)
    {
        if (screen is null || screen.Scaling <= 0)
        {
            return clientSize;
        }

        var workingArea = screen.WorkingArea;
        var maxWidth    = Math.Max(1, workingArea.Width / screen.Scaling - InitialScreenMargin * 2);
        var maxHeight   = Math.Max(1, workingArea.Height / screen.Scaling - InitialScreenMargin * 2);

        return new Size(
            ClampToWindowSizeConstraint(clientSize.Width, Window.MinWidth, Math.Min(Window.MaxWidth, maxWidth)),
            ClampToWindowSizeConstraint(clientSize.Height, Window.MinHeight, Math.Min(Window.MaxHeight, maxHeight)));
    }

    private bool TryGetInitialStartupPosition(Size clientSize, Screen? screen, out PixelPoint position)
    {
        position = default;

        if (Window.WindowStartupLocation is not (WindowStartupLocation.CenterScreen or WindowStartupLocation.CenterOwner))
        {
            return false;
        }

        if (screen is null)
        {
            return false;
        }

        var rect      = new PixelRect(PixelSize.FromSize(clientSize, screen.Scaling));
        var childRect = screen.WorkingArea.CenterRect(rect);
        if (Window.Screens.ScreenFromPoint(childRect.Position) is null)
        {
            childRect = ApplyScreenConstraint(screen, childRect, rect);
        }

        position = childRect.Position;
        return true;
    }

    private static PixelRect ApplyScreenConstraint(Screen screen, PixelRect childRect, PixelRect rect)
    {
        var constraint = screen.WorkingArea;
        var maxX       = constraint.Right - rect.Width;
        var maxY       = constraint.Bottom - rect.Height;

        if (constraint.X <= maxX)
        {
            childRect = childRect.WithX(Math.Clamp(childRect.X, constraint.X, maxX));
        }

        if (constraint.Y <= maxY)
        {
            childRect = childRect.WithY(Math.Clamp(childRect.Y, constraint.Y, maxY));
        }

        return childRect;
    }

    private bool TryGetExplicitInitialClientSize(out Size clientSize)
    {
        clientSize = default;

        if (!IsUsableExplicitSize(Window.Width) || !IsUsableExplicitSize(Window.Height))
        {
            return false;
        }

        clientSize = new Size(
            ClampToWindowSizeConstraint(Window.Width, Window.MinWidth, Window.MaxWidth),
            ClampToWindowSizeConstraint(Window.Height, Window.MinHeight, Window.MaxHeight));
        return true;
    }

    private static bool IsUsableExplicitSize(double value)
    {
        return double.IsFinite(value) && value > 0;
    }

    private static double ClampToWindowSizeConstraint(double value, double min, double max)
    {
        var effectiveMin = double.IsFinite(min) ? min : 0;
        var effectiveMax = double.IsFinite(max) ? max : double.PositiveInfinity;
        if (effectiveMax < effectiveMin)
        {
            effectiveMax = effectiveMin;
        }

        return Math.Min(Math.Max(value, effectiveMin), effectiveMax);
    }

    private void ApplyX11CsdFrameExtents()
    {
        var frameExtents = Window.WindowState is WindowState.Normal ? Window.FrameShadowThickness : default;
        Window.SetLinuxX11CsdFrameExtents(frameExtents);
    }
}
