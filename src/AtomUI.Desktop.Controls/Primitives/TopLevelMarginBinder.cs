using System.Reactive.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal static class TopLevelMarginBinder
{
    internal static IDisposable? BindWindowFrameGeometry(
        Control host,
        Action<Thickness, CornerRadius> applyGeometry)
    {
        if (host is Window window)
        {
            // DrawerContainer hosts both the surface and its mask. Keep the
            // visible frame border inside that root so the mask covers it;
            // only FrameShadowThickness is a transparent buffer outside the
            // window's visible client frame.
            return window.GetObservable(Window.FrameShadowThicknessProperty)
                         .CombineLatest(
                             window.GetObservable(TemplatedControl.CornerRadiusProperty),
                             static (frameShadowThickness, cornerRadius) =>
                                 (frameShadowThickness, cornerRadius))
                         .Subscribe(value => applyGeometry(
                             value.frameShadowThickness,
                             value.cornerRadius));
        }

        applyGeometry(default, default);
        return null;
    }

    internal static Size GetVisibleFrameSize(Control host)
    {
        var size = host.Bounds.Size;
        if (host is not Window window)
        {
            return size;
        }

        return WindowVisualLayerClip.CalculateClipBounds(
            size,
            Add(window.FrameShadowThickness, window.VisibleFrameBorderThickness)).Size;
    }

    internal static IDisposable? BindHostMargin(TopLevel topLevel, Action<Thickness> applyMargin)
    {
        if (RuntimePlatform.Features.SupportsWindowChrome && topLevel is Window window)
        {
            return window.GetObservable(Window.IsCsdEnabledProperty)
                         .CombineLatest(
                             window.GetObservable(Avalonia.Controls.Window.WindowDecorationMarginProperty),
                             window.GetObservable(Window.FrameShadowThicknessProperty),
                             window.GetObservable(Window.VisibleFrameBorderThicknessProperty),
                             static (isCsd, windowDecorationMargin, frameShadowThickness, visibleFrameBorderThickness) =>
                                 Add(
                                     isCsd ? windowDecorationMargin : frameShadowThickness,
                                     visibleFrameBorderThickness))
                         .Subscribe(applyMargin);
        }

        applyMargin(topLevel.InsetsManager?.SafeAreaPadding ?? default);
        return null;
    }

    private static Thickness Add(Thickness first, Thickness second)
    {
        return new Thickness(
            first.Left + second.Left,
            first.Top + second.Top,
            first.Right + second.Right,
            first.Bottom + second.Bottom);
    }
}
