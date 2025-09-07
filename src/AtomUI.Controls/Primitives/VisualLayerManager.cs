using AvaloniaVisualLayerManager = Avalonia.Controls.Primitives.VisualLayerManager;

namespace AtomUI.Controls.Primitives;

public class VisualLayerManager : AvaloniaVisualLayerManager
{
    private const int DialogZIndex = int.MaxValue - 1000;
    
    private T? FindLayer<T>() where T : class
    {
        var layers = this.GetLayers();
        foreach (var layer in layers)
        {
            if (layer is T match)
            {
                return match;
            }
        }
        return null;
    }
    
    public DialogLayer DialogLayer
    {
        get
        {
            var dialogLayer = FindLayer<DialogLayer>();
            if (dialogLayer == null)
            {
                dialogLayer = new DialogLayer();
                this.AddLayer(dialogLayer, DialogZIndex);
            }
            return dialogLayer;
        }
    }
}