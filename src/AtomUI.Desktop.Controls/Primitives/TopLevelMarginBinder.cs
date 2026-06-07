using System.Reactive.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal static class TopLevelMarginBinder
{
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
