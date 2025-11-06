using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Primitives;

internal class DialogLayer : Canvas
{
    protected override bool BypassFlowDirectionPolicies => true;
    
    public Size AvailableSize { get; private set; }

    static DialogLayer()
    {
        ClipToBoundsProperty.OverrideDefaultValue<DialogLayer>(true);
    }

    public DialogLayer()
    {
        Children.CollectionChanged += HandleDialogChanged;
    }

    private void HandleDialogChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems != null)
            {
                foreach (Control child in e.NewItems)
                {
                    if (child is OverlayDialogHost overlayDialogHost)
                    {
                        overlayDialogHost.HeaderPressed += HandleOverlayDialogClicked;
                    }
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null)
            {
                foreach (Control child in e.OldItems)
                {
                    if (child is OverlayDialogHost overlayDialogHost)
                    {
                        overlayDialogHost.HeaderPressed -= HandleOverlayDialogClicked;
                    }
                }
            }
        }
    }

    private void HandleOverlayDialogClicked(object? sender, EventArgs e)
    {
        var maxZIndex = 0;
        foreach (Control child in Children)
        {
            if (child is OverlayDialogHost overlayDialogHost)
            {
                maxZIndex = Math.Max(maxZIndex, overlayDialogHost.ZIndex);
            }
        }
        
        if (sender is OverlayDialogHost clickedOverlayDialogHost)
        {
            clickedOverlayDialogHost.NotifyChangeZIndex(maxZIndex + 2);
        }
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