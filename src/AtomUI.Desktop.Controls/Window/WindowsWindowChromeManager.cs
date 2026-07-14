using System.Runtime.Versioning;
using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

[Flags]
internal enum WindowsDrawnDecorationParts
{
    None        = 0,
    Shadow      = 1,
    Border      = 2,
    TitleBar    = 4,
    ResizeGrips = 8,
    All         = Shadow | Border | TitleBar | ResizeGrips
}

[SupportedOSPlatform("windows10.0")]
internal sealed class WindowsWindowChromeManager : IWindowChromeManager
{
    private const uint WindowStyleCaption     = 0x00C00000;
    private const uint WindowStyleBorder      = 0x00800000;
    private const uint WindowStyleDialogFrame = 0x00400000;
    private const uint WindowStyleThickFrame  = 0x00040000;

    private const uint ExtendedWindowStyleDialogModalFrame = 0x00000001;
    private const uint ExtendedWindowStyleWindowEdge       = 0x00000100;
    private const uint ExtendedWindowStyleClientEdge       = 0x00000200;
    private const uint ExtendedWindowStyleStaticEdge       = 0x00020000;

    private static readonly uint s_nativeFrameStyleMask =
        WindowStyleCaption |
        WindowStyleBorder |
        WindowStyleDialogFrame |
        WindowStyleThickFrame;

    private static readonly uint s_nativeFrameExtendedStyleMask =
        ExtendedWindowStyleDialogModalFrame |
        ExtendedWindowStyleWindowEdge |
        ExtendedWindowStyleClientEdge |
        ExtendedWindowStyleStaticEdge;

    private readonly Window _window;

    private WindowsWindowChromeManager(Window window)
    {
        _window = window;
    }

    public static WindowsWindowChromeManager Attach(Window window)
    {
        var manager = new WindowsWindowChromeManager(window);
        manager.Attach();
        return manager;
    }

    internal static (uint style, uint exStyle) ApplyDrawnDecorationsWindowStyles(uint style, uint exStyle)
    {
        return (style & ~s_nativeFrameStyleMask, exStyle & ~s_nativeFrameExtendedStyleMask);
    }

    internal static WindowsDrawnDecorationParts ComputeDecorationParts(
        WindowDecorations windowDecorations,
        WindowState windowState,
        bool canResize)
    {
        var parts = WindowsDrawnDecorationParts.None;
        if (windowDecorations != WindowDecorations.None)
        {
            if (windowDecorations == WindowDecorations.Full)
            {
                parts |= WindowsDrawnDecorationParts.TitleBar;
            }

            parts |= WindowsDrawnDecorationParts.Shadow;
            parts |= WindowsDrawnDecorationParts.Border;

            if (canResize)
            {
                parts |= WindowsDrawnDecorationParts.ResizeGrips;
            }

            if (windowState == WindowState.FullScreen)
            {
                parts &= ~(WindowsDrawnDecorationParts.Shadow
                           | WindowsDrawnDecorationParts.Border
                           | WindowsDrawnDecorationParts.ResizeGrips
                           | WindowsDrawnDecorationParts.TitleBar);
            }
            else if (windowState == WindowState.Maximized)
            {
                parts &= ~(WindowsDrawnDecorationParts.Shadow
                           | WindowsDrawnDecorationParts.Border
                           | WindowsDrawnDecorationParts.ResizeGrips);
            }
        }

        return parts;
    }

    public Action? PrepareInitialShowState()
    {
        return null;
    }

    public void HandleFrameShadowChanged(BoxShadows frameShadow)
    {
        ApplyFrameShadowThickness(frameShadow.Thickness());
        UpdateFrameGeometry();
    }

    public void ConfigureTitleBarHeightHint(double height)
    {
        if (!MathUtils.AreClose(_window.ExtendClientAreaTitleBarHeightHint, height))
        {
            _window.SetCurrentValue(AvaloniaWindow.ExtendClientAreaTitleBarHeightHintProperty, height);
        }

        UpdateFrameGeometry();
    }

    public void HandlePropertyChanged(AvaloniaProperty property)
    {
        if (property == AvaloniaWindow.WindowStateProperty ||
            property == AvaloniaWindow.CanResizeProperty ||
            property == AvaloniaWindow.WindowDecorationsProperty ||
            property == AvaloniaWindow.WindowDecorationsThemeProperty ||
            property == AvaloniaWindow.ExtendClientAreaTitleBarHeightHintProperty ||
            property == Window.FrameShadowThicknessProperty ||
            property == Window.TitleBarHeightProperty ||
            property == Window.IsCsdEnabledProperty ||
            property == Window.IsWindowsDrawnDecorationsEnabledProperty)
        {
            UpdateFrameGeometry();
        }
    }

    public void UpdateFrameGeometry()
    {
        var parts = ComputeDecorationParts(_window.WindowDecorations, _window.WindowState, _window.CanResize);
        _window.TryUpdateDrawnDecorations(parts);
    }

    private void Attach()
    {
        _window.IsWindowsDrawnDecorationsEnabled = true;
        _window.ScalingChanged += HandleScalingChanged;
        _window.Closed += HandleClosed;

        Win32Properties.AddWindowStylesCallback(_window, ApplyDrawnDecorationsWindowStyles);
        ApplyFrameShadowThickness(_window.FrameShadow.Thickness());
        RefreshWindowStyles();
        UpdateFrameGeometry();
    }

    private void HandleScalingChanged(object? sender, EventArgs e)
    {
        UpdateFrameGeometry();
    }

    private void HandleClosed(object? sender, EventArgs e)
    {
        _window.ScalingChanged -= HandleScalingChanged;
        _window.Closed -= HandleClosed;
        Win32Properties.RemoveWindowStylesCallback(_window, ApplyDrawnDecorationsWindowStyles);
    }

    private void RefreshWindowStyles()
    {
        _window.PlatformImpl?.SetWindowDecorations(_window.WindowDecorations);
    }

    private void ApplyFrameShadowThickness(Thickness thickness)
    {
        if (!AreThicknessClose(_window.FrameShadowThickness, thickness))
        {
            _window.FrameShadowThickness = thickness;
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
