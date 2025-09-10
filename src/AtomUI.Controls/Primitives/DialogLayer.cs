using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Primitives;

public class DialogLayer : Canvas
{
    protected override bool BypassFlowDirectionPolicies => true;
    
    public Size AvailableSize { get; private set; }

    static DialogLayer()
    {
        ClipToBoundsProperty.OverrideDefaultValue<DialogLayer>(true);
    }
    
    public static DialogLayer? GetDialogLayer(Visual visual)
    {
        if (TopLevel.GetTopLevel(visual) is {} tl)
        {
            var layers = tl.GetVisualDescendants().OfType<VisualLayerManager>().FirstOrDefault();
            return layers?.DialogLayer;
        }

        return null;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (Control child in Children)
        {
            child.Measure(availableSize);
        }
        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        // We are saving it here since child controls might need to know the entire size of the overlay
        // and Bounds won't be updated in time
        AvailableSize = finalSize;
        return base.ArrangeOverride(finalSize);
    }
}