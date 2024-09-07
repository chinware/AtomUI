using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Primitives;

public static class AtomLayerExtension
{
    public static AtomLayer? GetLayer(this Visual? visual)
    {
        if (visual == null)
        {
            return null;
        }
        
        var host = visual.FindAncestorOfType<ScrollContentPresenter>(true)?.Content as Visual
                   ?? TopLevel.GetTopLevel(visual);
            
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
        layer.ParentHost = host;

        return true;
    }
}