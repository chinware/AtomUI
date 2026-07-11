using System.Reactive.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal static class TopLevelMarginBinder
{
    internal static IDisposable? BindCsdHostGeometry(
        Control host,
        Action<bool, Thickness, CornerRadius> applyGeometry)
    {
        if (host is Window window)
        {
            return window.GetObservable(Window.IsCsdEnabledProperty)
                         .CombineLatest(
                             window.GetObservable(Window.FrameShadowThicknessProperty),
                             window.GetObservable(TemplatedControl.CornerRadiusProperty),
                             static (isCsd, frameShadowThickness, cornerRadius) =>
                                 (isCsd,
                                     margin: isCsd ? frameShadowThickness : default,
                                     cornerRadius: isCsd ? cornerRadius : default))
                         .Subscribe(value => applyGeometry(
                             value.isCsd,
                             value.margin,
                             value.cornerRadius));
        }

        applyGeometry(false, default, default);
        return null;
    }

    internal static Size GetCsdContentSize(Control host)
    {
        var size = host.Bounds.Size;
        if (host is not Window { IsCsdEnabled: true } window)
        {
            return size;
        }

        var margin = window.FrameShadowThickness;
        return new Size(
            Math.Max(0, size.Width - margin.Left - margin.Right),
            Math.Max(0, size.Height - margin.Top - margin.Bottom));
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
