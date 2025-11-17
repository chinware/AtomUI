using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Primitives;

public static class AtomLayerExtension
{
    public static AtomLayer? GetLayer(this Visual? target)
    {
        if (target == null)
        {
            return null;
        }

        var host   = target.FindAncestorOfType<ScrollContentPresenter>() as Visual;
        var anchor = AtomLayer.GetBoundsAnchor(target);
        if (anchor != null && host != null)
        {
            while (host != null && host.IsVisualAncestorOf(anchor) == false)
            {
                host = host.FindAncestorOfType<ScrollContentPresenter>();
            }
        }

        host ??= target.FindAncestorOfType<VisualLayerManager>();

        host ??= TopLevel.GetTopLevel(target);

        if (host == null)
        {
            return null;
        }

        var layer = host.GetVisualChildren().FirstOrDefault(c => c is AtomLayer) as AtomLayer
                    ?? TryInject(host);

        return layer;
    }

    private static AtomLayer? TryInject(Visual host)
    {
        var layer = new AtomLayer
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch
        };

        return InjectCore(host, layer) ? layer : null;
    }

    private static bool InjectCore(Visual host, AtomLayer layer)
    {
        if (host.GetVisualChildren() is not IList<Visual> visualChildren)
        {
            return false;
        }

        if (visualChildren.Any(c => c is AtomLayer))
        {
            return false;
        }

        visualChildren.Add(layer);
        layer.SetLogicalParent(host);
        layer.Host = host;

        if (host is ScrollContentPresenter presenter)
        {
            layer[!AtomLayer.HostOffsetProperty] = presenter[!ScrollContentPresenter.OffsetProperty];
        }

        return true;
    }
}