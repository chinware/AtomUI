using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Primitives;

public class ScopeAwareOverlayLayer : Canvas
{
    public static readonly StyledProperty<Visual?> LayerHostProperty = AvaloniaProperty
        .Register<ScopeAwareOverlayLayer, Visual?>(nameof(LayerHost));

    public Visual? LayerHost
    {
        get => GetValue(LayerHostProperty);
        set => SetValue(LayerHostProperty, value);
    }
    
    protected override bool BypassFlowDirectionPolicies => true;
    
    public Size AvailableSize { get; private set; }
    public bool Injecting { get; private set; }
    
    public static ScopeAwareOverlayLayer? GetLayer(Visual visual)
    {
        Layoutable? layerHost = visual.FindAncestorOfType<ScopeAwareOverlayLayerPanel>(true);
        layerHost ??= visual.FindAncestorOfType<VisualLayerManager>();
        layerHost ??= TopLevel.GetTopLevel(visual);

        if (layerHost == null)
        {
            return null;
        }

        var layer =
            layerHost.GetVisualChildren().FirstOrDefault(c => c is ScopeAwareOverlayLayer) as ScopeAwareOverlayLayer;
        layer ??= InjectLayer(layerHost);

        return layer;
    }
    
    private static ScopeAwareOverlayLayer InjectLayer(Layoutable layerHost)
    {
        var layer = FindLayer(layerHost);
        if (layer != null)
        {
            return layer;
        }
        layer = new ScopeAwareOverlayLayer
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
            ZIndex              = VisualLayerManagerUtils.ScopeAwareOverlayZIndex
        };
        try
        {
            layer.Injecting = true;
            layer.LayerHost = layerHost;
            if (layerHost is VisualLayerManager visualLayerManager)
            {
                visualLayerManager.AddLayer(layer, visualLayerManager.ZIndex);
            }
            else if (layerHost is ScopeAwareOverlayLayerPanel scopeAwareOverlayLayerPanel)
            {
                scopeAwareOverlayLayerPanel.Children.Add(layer);
            }

            return layer;
        }
        finally
        {
            layer.Injecting = false;
        }
    }
     
    public static ScopeAwareOverlayLayer? FindLayer(Layoutable layerHost)
    {
        ScopeAwareOverlayLayer? layer = null;
        if (layerHost is ScopeAwareOverlayLayerPanel scopeAwareOverlayLayerPanel)
        {
            layer = scopeAwareOverlayLayerPanel.FindChildOfType<ScopeAwareOverlayLayer>();
        }
        else if (layerHost is VisualLayerManager visualLayerManager)
        {
            layer = visualLayerManager.FindChildOfType<ScopeAwareOverlayLayer>();
        }
        return layer;
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        // We are saving it here since child controls might need to know the entire size of the overlay
        // and Bounds won't be updated in time
        AvailableSize = finalSize;
        return base.ArrangeOverride(finalSize);
    }
}