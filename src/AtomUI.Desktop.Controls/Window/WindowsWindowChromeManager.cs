using System.Runtime.Versioning;
using AtomUI.Media;
using AtomUI.Native;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

[SupportedOSPlatform("windows")]
internal sealed class WindowsWindowChromeManager : IWindowChromeManager
{
    private readonly Window _window;
    private Win32Properties.CustomWndProcHookCallback? _wndProcHookCallback;
    private bool _wndProcHookRegistered;
    private bool _frameRefreshQueued;
    private bool _wasFullScreen;

    private WindowsWindowChromeManager(Window window)
    {
        _window = window;
    }

    public static WindowsWindowChromeManager Attach(Window window)
    {
        return new WindowsWindowChromeManager(window);
    }

    public Action? PrepareInitialShowState()
    {
        return null;
    }

    public void HandleFrameShadowChanged(BoxShadows frameShadow)
    {
        _window.FrameShadowThickness = frameShadow.Thickness();
    }

    public void ConfigureTitleBarHeightHint(double height)
    {
    }

    public void HandlePropertyChanged(AvaloniaProperty property)
    {
        if (property == AvaloniaWindow.WindowStateProperty)
        {
            HandleWindowStateChanged();
        }
        else if (property == AvaloniaWindow.IsVisibleProperty && _window.IsVisible)
        {
            RequestFrameRefresh(DispatcherPriority.Loaded);
        }
    }

    public void UpdateFrameGeometry()
    {
        ApplyFrameRefresh();
    }

    private void HandleWindowStateChanged()
    {
        if (_window.WindowState == WindowState.FullScreen)
        {
            _wasFullScreen = true;
            return;
        }

        RequestFrameRefresh(_wasFullScreen ? DispatcherPriority.Send : DispatcherPriority.Loaded);
        _wasFullScreen = false;
    }

    private void RequestFrameRefresh(DispatcherPriority priority)
    {
        EnsureWndProcHookRegistered();
        if (!_window.IsVisible || _window.PlatformImpl is null || _frameRefreshQueued)
        {
            return;
        }

        _frameRefreshQueued = true;
        _window.Dispatcher.Post(ApplyPendingFrameRefresh, priority);
    }

    private void ApplyPendingFrameRefresh()
    {
        _frameRefreshQueued = false;
        ApplyFrameRefresh();
    }

    private void ApplyFrameRefresh()
    {
        EnsureWndProcHookRegistered();
        if (!_window.IsVisible || _window.PlatformImpl is null)
        {
            return;
        }

        _window.ApplyWinDwmShadow();
        _window.ForceWinNonClientFrameChanged();
    }

    private void EnsureWndProcHookRegistered()
    {
        if (_wndProcHookRegistered || _window.PlatformImpl is null)
        {
            return;
        }

        _wndProcHookCallback = _window.WinWndProcHook;
        Win32Properties.AddWndProcHookCallback(_window, _wndProcHookCallback);
        _wndProcHookRegistered = true;
    }
}
