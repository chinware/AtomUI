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
            return window.GetObservable(Window.FrameShadowThicknessProperty)
                         .CombineLatest(
                             window.GetObservable(Window.VisibleFrameBorderThicknessProperty),
                             window.GetObservable(TemplatedControl.CornerRadiusProperty),
                             static (frameShadowThickness, visibleFrameBorderThickness, cornerRadius) =>
                                 (frameShadowThickness, visibleFrameBorderThickness, cornerRadius))
                         .Subscribe(value => applyGeometry(
                             Add(value.frameShadowThickness, value.visibleFrameBorderThickness),
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
