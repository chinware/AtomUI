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

        return WindowVisualLayerClip.CalculateClipBounds(size, window.FrameShadowThickness).Size;
    }

    internal static IDisposable? BindHostMargin(TopLevel topLevel, Action<Thickness> applyMargin)
    {
        if (RuntimePlatform.Features.SupportsWindowChrome && topLevel is Window window)
        {
            return window.GetObservable(Window.IsCsdEnabledProperty)
                         .CombineLatest(
                             window.GetObservable(Avalonia.Controls.Window.WindowDecorationMarginProperty),
                             window.GetObservable(Window.FrameShadowThicknessProperty),
                             static (isCsd, windowDecorationMargin, frameShadowThickness) =>
                                 isCsd ? windowDecorationMargin : frameShadowThickness)
                         .Subscribe(applyMargin);
        }

        applyMargin(topLevel.InsetsManager?.SafeAreaPadding ?? default);
        return null;
    }
}
