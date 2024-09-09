using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Theme.Data;

internal class RenderScaleAwareThicknessConfigure
{
    private readonly WeakReference<Control> _control;
    private readonly Func<Thickness, Thickness>? _postProcessor;

    public RenderScaleAwareThicknessConfigure(Control target, Func<Thickness, Thickness>? postProcessor = null)
    {
        _control       = new WeakReference<Control>(target);
        _postProcessor = postProcessor;
    }

    public object? Configure(object? value)
    {
        if (value is Thickness thickness)
        {
            var renderScaling = 1d;
            if (_control.TryGetTarget(out var target))
            {
                var visualRoot                            = target.GetVisualRoot();
                if (visualRoot is not null) renderScaling = visualRoot.RenderScaling;
            }

            if (MathUtils.AreClose(renderScaling, Math.Floor(renderScaling))) renderScaling = 1.0d; // 这种情况很清晰
            var result = BorderUtils.BuildRenderScaleAwareThickness(thickness, renderScaling);
            if (_postProcessor is not null) return _postProcessor(result);

            return result;
        }

        return value;
    }

    public static implicit operator Func<object?, object?>(RenderScaleAwareThicknessConfigure configure)
    {
        return configure.Configure;
    }
}