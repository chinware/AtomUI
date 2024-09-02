using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;

// ReSharper disable SuggestBaseTypeForParameter

namespace AtomUI.Controls.Primitives
{
    public static class AtomLayerExtension
    {
        public static AtomLayer? GetAxLayer(this Visual? visual)
        {
            return visual == null ? null : AtomLayer.GetLayer(visual);
        }
    }

    public class AtomLayer : Panel
    {
        private AtomLayer()
        {
            
        }
        
        public static AtomLayer? GetLayer(Visual visual)
        {
            if (visual.GetVisualRoot() is not TopLevel topLevel)
            {
                return null;
            }
            
            var layer = topLevel.GetVisualChildren().FirstOrDefault(c => c is AtomLayer) as AtomLayer;
            layer ??= TryInject(topLevel);
            return layer;
        }

        private static AtomLayer TryInject(TopLevel topLevel)
        {
            var layer = new AtomLayer();
            
            if (topLevel.IsLoaded == false)
            {
                topLevel.Loaded += (sender, args) =>
                {
                    InjectCore(topLevel, layer);
                };
            }
            else
            {
                InjectCore(topLevel, layer);
            }

            return layer;
        }

        private static void InjectCore(TopLevel topLevel, AtomLayer layer)
        {
            if (topLevel.GetVisualChildren() is not IList<Visual> visualChildren)
            {
                return;
            }
            if (visualChildren.Any(c => c is AtomLayer))
            {
                return;
            }

            layer.HorizontalAlignment = HorizontalAlignment.Stretch;
            layer.VerticalAlignment   = VerticalAlignment.Stretch;
            layer.InheritanceParent   = topLevel;

            visualChildren.Add(layer);
            topLevel.InvalidateMeasure();
        }
    }
}
