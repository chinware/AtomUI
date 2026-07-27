using AtomUI.Media;
using AtomUI.Native;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

internal sealed class MacOSWindowChromeManager : IWindowChromeManager
{
    private readonly Window _window;
    private bool _initialShowStatePrepared;

    private MacOSWindowChromeManager(Window window)
    {
        _window = window;
    }

    internal static MacOSWindowChromeManager Attach(Window window)
    {
        return new MacOSWindowChromeManager(window);
    }

    public bool UsesCustomResizer => false;

    public Action? PrepareInitialShowState()
    {
        if (_initialShowStatePrepared || _window.IsVisible)
        {
            return null;
        }

        _initialShowStatePrepared = true;
        _window.PreparePlatformChromeInitialShowLayout();
        UpdateFrameGeometry();
        return null;
    }

    public void HandleFrameShadowChanged(BoxShadows frameShadow)
    {
        _window.FrameShadowThickness = frameShadow.Thickness();
    }

    public void ConfigureTitleBarHeightHint(double height)
    {
        _window.SetCurrentValue(AvaloniaWindow.ExtendClientAreaTitleBarHeightHintProperty, height);
    }

    public void HandlePropertyChanged(AvaloniaProperty property)
    {
        if (property == AvaloniaWindow.WindowStateProperty ||
            property == AvaloniaWindow.CanResizeProperty ||
            property == AvaloniaWindow.WindowDecorationMarginProperty ||
            property == Window.FrameShadowThicknessProperty ||
            property == Window.IsCsdEnabledProperty)
        {
            UpdateFrameGeometry();
        }
    }

    public void UpdateFrameGeometry()
    {
        if (_window.IsCsdEnabled)
        {
            if (OperatingSystem.IsMacOS())
            {
                _window.SetMacOsResizeIndicatorVisible(true);
            }
            return;
        }

        if (OperatingSystem.IsMacOS())
        {
            _window.SetMacOsResizeIndicatorVisible(false);
        }
    }
}
