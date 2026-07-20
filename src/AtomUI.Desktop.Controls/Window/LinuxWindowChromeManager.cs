using System.Runtime.Versioning;
using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

internal enum LinuxWindowingBackend
{
    Other,
    X11,
    Wayland
}

[SupportedOSPlatform("linux")]
internal abstract class LinuxWindowChromeManager : IWindowChromeManager
{
    private readonly Window _window;
    private bool _initialShowStatePrepared;
    private bool _frameGeometryUpdateQueued;
    private bool _hasPendingFrameShadowThickness;
    private Thickness _pendingFrameShadowThickness;
    private bool _hasPendingTitleBarHeightHint;
    private double _pendingTitleBarHeightHint;

    protected LinuxWindowChromeManager(Window window)
    {
        _window = window;
    }

    protected Window Window => _window;

    public virtual bool UsesCustomResizer => !Window.IsCsdEnabled;

    public static LinuxWindowChromeManager Attach(Window window)
    {
        var configuredPlatform = AvaloniaLocator.Current.GetService<AtomUIWindowingPlatformOptions>()?.Platform;
        var platformImpl       = window.PlatformImpl;
        var backend = ResolveBackend(
            configuredPlatform,
            platformImpl?.Handle?.HandleDescriptor,
            platformImpl?.GetType().Assembly.GetName().Name);
        LinuxWindowChromeManager manager = backend switch
        {
            LinuxWindowingBackend.X11 => new X11WindowChromeManager(window),
            LinuxWindowingBackend.Wayland => new WaylandWindowChromeManager(window),
            _ => new OtherLinuxWindowChromeManager(window)
        };
        manager.Attach();
        return manager;
    }

    internal static LinuxWindowingBackend ResolveBackend(
        AtomUIWindowingPlatform? configuredPlatform,
        string? handleDescriptor,
        string? platformAssemblyName)
    {
        if (configuredPlatform is AtomUIWindowingPlatform.X11)
        {
            return LinuxWindowingBackend.X11;
        }

        if (configuredPlatform is AtomUIWindowingPlatform.Wayland)
        {
            return LinuxWindowingBackend.Wayland;
        }

        if (string.Equals(handleDescriptor, "XID", StringComparison.Ordinal))
        {
            return LinuxWindowingBackend.X11;
        }

        return string.Equals(platformAssemblyName, "Avalonia.Wayland", StringComparison.Ordinal)
            ? LinuxWindowingBackend.Wayland
            : LinuxWindowingBackend.Other;
    }

    private void Attach()
    {
        _window.ScalingChanged += HandleScalingChanged;
        if (_window.PlatformImpl is { } platformImpl)
        {
            platformImpl.DrawnDecorationsRequestChanged += HandleDrawnDecorationsRequestChanged;
        }

        AttachPlatformHooks();
    }

    protected virtual void AttachPlatformHooks()
    {
    }

    public Action? PrepareInitialShowState()
    {
        if (_initialShowStatePrepared || _window.IsVisible || _window.PlatformImpl is null)
        {
            return null;
        }

        _initialShowStatePrepared = true;
        _window.PreparePlatformChromeInitialShowLayout();
        EnsureResizeMinimumSize();
        return PrepareInitialPlatformShowState();
    }

    protected virtual Action? PrepareInitialPlatformShowState()
    {
        return null;
    }

    public void HandleFrameShadowChanged(BoxShadows frameShadow)
    {
        var thickness = frameShadow.Thickness();
        if (!ShouldDeferFrameGeometryUpdate())
        {
            ApplyFrameShadowThickness(thickness);
            return;
        }

        _pendingFrameShadowThickness    = thickness;
        _hasPendingFrameShadowThickness = true;
        RequestFrameGeometryUpdate();
    }

    public void ConfigureTitleBarHeightHint(double height)
    {
        if (!ShouldDeferCsdTitleBarHeightHint())
        {
            ApplyTitleBarHeightHint(height);
            return;
        }

        _pendingTitleBarHeightHint    = height;
        _hasPendingTitleBarHeightHint = true;
        RequestFrameGeometryUpdate();
    }

    public void HandlePropertyChanged(AvaloniaProperty property)
    {
        if (property == AvaloniaWindow.WindowStateProperty ||
            property == AvaloniaWindow.CanResizeProperty ||
            property == AvaloniaWindow.WindowDecorationMarginProperty ||
            property == Window.FrameShadowThicknessProperty ||
            property == Window.TitleBarHeightProperty ||
            property == Window.IsCsdEnabledProperty)
        {
            RequestFrameGeometryUpdate();
        }
    }

    public void UpdateFrameGeometry()
    {
        EnsureResizeMinimumSize();
        UpdatePlatformFrameGeometry();
    }

    protected virtual void UpdatePlatformFrameGeometry()
    {
    }

    private void HandleScalingChanged(object? sender, EventArgs e)
    {
        RequestFrameGeometryUpdate();
    }

    protected void RequestFrameGeometryUpdate()
    {
        if (!ShouldDeferFrameGeometryUpdate())
        {
            ApplyPendingFrameGeometryInputs();
            UpdateFrameGeometry();
            return;
        }

        if (_frameGeometryUpdateQueued)
        {
            return;
        }

        _frameGeometryUpdateQueued = true;
        _window.Dispatcher.Post(ApplyPendingFrameGeometryUpdate, Avalonia.Threading.DispatcherPriority.Render);
    }

    private void HandleDrawnDecorationsRequestChanged()
    {
        _window.RefreshPlatformCsdStatus();
        RequestFrameGeometryUpdate();
    }

    private bool ShouldDeferFrameGeometryUpdate()
    {
        return _window.IsVisible && _window.PlatformImpl is not null;
    }

    private bool ShouldDeferCsdTitleBarHeightHint()
    {
        return _window.IsCsdEnabled && ShouldDeferFrameGeometryUpdate();
    }

    private void ApplyPendingFrameGeometryUpdate()
    {
        ApplyPendingFrameGeometryInputs();
        UpdateFrameGeometry();
        _frameGeometryUpdateQueued = false;
    }

    private void ApplyPendingFrameGeometryInputs()
    {
        if (_hasPendingFrameShadowThickness)
        {
            _hasPendingFrameShadowThickness = false;
            ApplyFrameShadowThickness(_pendingFrameShadowThickness);
        }

        if (_hasPendingTitleBarHeightHint)
        {
            _hasPendingTitleBarHeightHint = false;
            ApplyTitleBarHeightHint(_pendingTitleBarHeightHint);
        }
    }

    private void ApplyFrameShadowThickness(Thickness thickness)
    {
        if (!AreThicknessClose(_window.FrameShadowThickness, thickness))
        {
            _window.FrameShadowThickness = thickness;
        }
    }

    private void ApplyTitleBarHeightHint(double height)
    {
        if (!MathUtils.AreClose(_window.ExtendClientAreaTitleBarHeightHint, height))
        {
            _window.SetCurrentValue(AvaloniaWindow.ExtendClientAreaTitleBarHeightHintProperty, height);
        }
    }

    private void EnsureResizeMinimumSize()
    {
        var shadow       = _window.FrameShadowThickness;
        var cornerRadius = _window.CornerRadius;
        var maxCorner = Math.Max(
            Math.Max(cornerRadius.TopLeft, cornerRadius.TopRight),
            Math.Max(cornerRadius.BottomLeft, cornerRadius.BottomRight));

        const double frameBorder = 1;

        var horizontalChrome = shadow.Left + shadow.Right + frameBorder * 2;
        var titleBarWidth    = _window.TitleBar?.DesiredSize.Width ?? 0;
        var minResizeWidth = Math.Max(
            horizontalChrome + maxCorner * 2,
            horizontalChrome + titleBarWidth);

        var titleBarHeight = Math.Max(_window.TitleBar?.DesiredSize.Height ?? 0, _window.TitleBarHeight);
        var verticalChrome = shadow.Top + shadow.Bottom + frameBorder * 2;
        var minResizeHeight = Math.Max(
            verticalChrome + titleBarHeight + maxCorner * 2,
            verticalChrome + titleBarHeight * 3);

        if (_window.MinWidth < minResizeWidth)
        {
            _window.MinWidth = minResizeWidth;
        }

        if (_window.MinHeight < minResizeHeight)
        {
            _window.MinHeight = minResizeHeight;
        }
    }

    private static bool AreThicknessClose(Thickness left, Thickness right)
    {
        return MathUtils.AreClose(left.Left, right.Left) &&
               MathUtils.AreClose(left.Top, right.Top) &&
               MathUtils.AreClose(left.Right, right.Right) &&
               MathUtils.AreClose(left.Bottom, right.Bottom);
    }
}
