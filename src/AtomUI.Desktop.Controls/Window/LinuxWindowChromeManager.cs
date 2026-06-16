using System.Runtime.Versioning;
using AtomUI.Media;
using AtomUI.Native;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

[SupportedOSPlatform("linux")]
internal sealed class LinuxWindowChromeManager : IWindowChromeManager
{
    private const double InitialScreenMargin = 48;

    private static readonly ISet<AvaloniaProperty> s_shadowInputRegionAffectsProperties =
        new HashSet<AvaloniaProperty>
        {
            AvaloniaWindow.CanResizeProperty,
            Window.FrameShadowThicknessProperty,
            Window.IsCsdEnabledProperty
        };

    private readonly Window _window;
    private bool _initialShowStatePrepared;
    private bool _frameGeometryUpdateQueued;
    private bool _hasPendingFrameShadowThickness;
    private Thickness _pendingFrameShadowThickness;
    private bool _hasPendingTitleBarHeightHint;
    private double _pendingTitleBarHeightHint;

    private LinuxWindowChromeManager(Window window)
    {
        _window = window;
    }

    public static LinuxWindowChromeManager Attach(Window window)
    {
        var manager = new LinuxWindowChromeManager(window);
        manager.Attach();
        return manager;
    }

    private void Attach()
    {
        _window.ScalingChanged += HandleScalingChanged;
        _window.AttachClickThroughShadow(
            s_shadowInputRegionAffectsProperties,
            () => _window.FrameShadowThickness,
            ClickThroughShadowExtensions.DefaultResizeBand);
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
        ApplyX11CsdFrameExtents();
        var screen     = _window.Screens.ScreenFromPoint(_window.Position) ?? _window.Screens.Primary;
        var clientSize = SynchronizeInitialClientSize(screen);
        if (clientSize is null)
        {
            return null;
        }

        var originalStartupLocation = _window.WindowStartupLocation;
        if (TryGetInitialStartupPosition(clientSize.Value, screen, out var position))
        {
            _window.SetCurrentValue(AvaloniaWindow.WindowStartupLocationProperty, WindowStartupLocation.Manual);
            _window.Position = position;
            _window.ConfigureLinuxInitialWindowGeometry(position, clientSize.Value, screen?.Scaling ?? 1);
            return () => _window.SetCurrentValue(AvaloniaWindow.WindowStartupLocationProperty, originalStartupLocation);
        }

        _window.ConfigureLinuxInitialWindowGeometry(_window.Position, clientSize.Value, screen?.Scaling ?? 1);
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
        ApplyX11CsdFrameExtents();
    }

    private void HandleScalingChanged(object? sender, EventArgs e)
    {
        RequestFrameGeometryUpdate();
    }

    private Size? SynchronizeInitialClientSize(Screen? screen)
    {
        if (_window.SizeToContent != SizeToContent.Manual)
        {
            return null;
        }

        if (_window.WindowState is WindowState.Minimized or WindowState.Maximized or WindowState.FullScreen)
        {
            return null;
        }

        if (!TryGetExplicitInitialClientSize(out var clientSize))
        {
            return null;
        }

        clientSize = ConstrainInitialClientSizeToScreen(clientSize, screen);

        if (!MathUtils.AreClose(_window.Width, clientSize.Width))
        {
            _window.Width = clientSize.Width;
        }

        if (!MathUtils.AreClose(_window.Height, clientSize.Height))
        {
            _window.Height = clientSize.Height;
        }

        if (_window.ClientSize != clientSize)
        {
            _window.SetPlatformChromeClientSize(clientSize);
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
            ClampToWindowSizeConstraint(clientSize.Width, _window.MinWidth, Math.Min(_window.MaxWidth, maxWidth)),
            ClampToWindowSizeConstraint(clientSize.Height, _window.MinHeight, Math.Min(_window.MaxHeight, maxHeight)));
    }

    private bool TryGetInitialStartupPosition(Size clientSize, Screen? screen, out PixelPoint position)
    {
        position = default;

        if (_window.WindowStartupLocation is not (WindowStartupLocation.CenterScreen or WindowStartupLocation.CenterOwner))
        {
            return false;
        }

        if (screen is null)
        {
            return false;
        }

        var rect      = new PixelRect(PixelSize.FromSize(clientSize, screen.Scaling));
        var childRect = screen.WorkingArea.CenterRect(rect);
        if (_window.Screens.ScreenFromPoint(childRect.Position) is null)
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

        if (!IsUsableExplicitSize(_window.Width) || !IsUsableExplicitSize(_window.Height))
        {
            return false;
        }

        clientSize = new Size(
            ClampToWindowSizeConstraint(_window.Width, _window.MinWidth, _window.MaxWidth),
            ClampToWindowSizeConstraint(_window.Height, _window.MinHeight, _window.MaxHeight));
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

    private bool ShouldDeferFrameGeometryUpdate()
    {
        return _window.IsVisible && _window.PlatformImpl is not null;
    }

    private bool ShouldDeferCsdTitleBarHeightHint()
    {
        return _window.IsCsdEnabled && ShouldDeferFrameGeometryUpdate();
    }

    private void RequestFrameGeometryUpdate()
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

    private void ApplyX11CsdFrameExtents()
    {
        var frameExtents = _window.WindowState is WindowState.Normal ? _window.FrameShadowThickness : default;
        _window.SetLinuxX11CsdFrameExtents(frameExtents);
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
