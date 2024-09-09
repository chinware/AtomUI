using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
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

        host ??= TopLevel.GetTopLevel(target);
            
        if (host == null)
        {
            return null;
        }

        var layer = host.GetVisualChildren().FirstOrDefault(c => c is AtomLayer) as AtomLayer
                    ?? TryInject(host);
            
        return layer;
    }
    
    public static T? GetAdorner<T>(this Visual target) where T : Control
    {
        return target.GetLayer()?.GetAdorner<T>(target);
    }

    public static IEnumerable<Control> GetAdorners(this Visual target)
    {
        return target.GetLayer()?.GetAdorners(target) ?? [];
    }

    public static void AddAdorner(this Visual target, Control adorner)
    {
        target.GetLayer()?.AddAdorner(target, adorner);
    }

    public static void RemoveAdorner<T>(this Visual target) where T : Control
    {
        target.GetLayer()?.RemoveAdorner<T>(target);
    }

    public static void RemoveAdorner(this Visual target, Control adorner)
    {
        target.GetLayer()?.RemoveAdorner(adorner);
    }

    public static void BeginRemovingAdorner(this Visual target, Control adorner, int millisecondsToConfirm, Func<bool> confirm)
    {
        target.GetLayer()?.BeginRemovingAdorner(adorner, millisecondsToConfirm, confirm);
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
        ((ISetLogicalParent)layer).SetParent(host);
        layer.Host = host;

        if (host is ScrollContentPresenter presenter)
        {
            layer[!AtomLayer.HostOffsetProperty] = presenter[!ScrollContentPresenter.OffsetProperty];
        }

        return true;
    }
}