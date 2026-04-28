using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.Primitives;

internal class VisualLayerManagerUtils
{
    internal const int ScopeAwareAdornerLayerZIndex = int.MaxValue - 1000;
    internal const int ScopeAwareOverlayZIndex = int.MaxValue - 990;
    
    internal static T? FindLayer<T>(VisualLayerManager visualLayerManager) where T : class
    {
        var layers = visualLayerManager.GetLayers();
        foreach (var layer in layers)
        {
            if (layer is T match)
            {
                return match;
            }
        }
        return null;
    }
}