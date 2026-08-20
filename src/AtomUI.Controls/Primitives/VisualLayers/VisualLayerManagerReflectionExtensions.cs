using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Primitives;

using AvaloniaVisualLayerManager = Avalonia.Controls.Primitives.VisualLayerManager;

internal static class VisualLayerManagerReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaVisualLayerManager))]
    private static readonly Lazy<MethodInfo> AddLayerMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(AvaloniaVisualLayerManager).GetMethodInfoOrThrow("AddLayer",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaVisualLayerManager))]
    private static readonly Lazy<FieldInfo> LayersFieldInfo = new Lazy<FieldInfo>(() => 
        typeof(AvaloniaVisualLayerManager).GetFieldInfoOrThrow("_layers",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AvaloniaVisualLayerManager))]
    private static readonly Lazy<PropertyInfo> PopupOverlayLayerPropertyInfo = new Lazy<PropertyInfo>(() =>
        typeof(AvaloniaVisualLayerManager).GetPropertyInfoOrThrow("PopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AvaloniaVisualLayerManager))]
    private static readonly Lazy<PropertyInfo> EnablePopupOverlayLayerPropertyInfo = new Lazy<PropertyInfo>(() =>
        typeof(AvaloniaVisualLayerManager).GetPropertyInfoOrThrow("EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion
    
    internal static void AddLayer(this AvaloniaVisualLayerManager visualLayerManager, Control layer, int zindex)
    {
        AddLayerMethodInfo.Value.Invoke(visualLayerManager, [layer, zindex]);
    }

    internal static List<Control> GetLayers(this AvaloniaVisualLayerManager visualLayerManager)
    {
        var layers = LayersFieldInfo.Value.GetValue(visualLayerManager) as List<Control>;
        Debug.Assert(layers != null);
        return layers;
    }

    internal static Control? GetPopupOverlayLayer(this AvaloniaVisualLayerManager visualLayerManager)
    {
        return PopupOverlayLayerPropertyInfo.Value.GetValue(visualLayerManager) as Control;
    }

    // 创建一个启用 popup overlay 能力的 VisualLayerManager 作用域(与 TopLevel 对其自身 VLM 的启用方式一致),
    // 使该作用域子树内的 popup/light-dismiss 解析到作用域自身的 popup overlay layer。
    internal static AvaloniaVisualLayerManager CreatePopupCapableScope()
    {
        var scope = new AvaloniaVisualLayerManager();
        EnablePopupOverlayLayerPropertyInfo.Value.SetValue(scope, true);
        return scope;
    }

    internal static Control? GetPopupOverlayLayer(this Visual visual)
    {
        foreach (var ancestor in visual.GetSelfAndVisualAncestors())
        {
            if (ancestor is AvaloniaVisualLayerManager visualLayerManager &&
                visualLayerManager.GetPopupOverlayLayer() is { } layer)
            {
                return layer;
            }
        }

        if (TopLevel.GetTopLevel(visual) is { } topLevel)
        {
            var visualLayerManager = FindFirstDescendantVisualLayerManager(topLevel);
            return visualLayerManager?.GetPopupOverlayLayer();
        }

        return null;
    }

    private static AvaloniaVisualLayerManager? FindFirstDescendantVisualLayerManager(Visual visual)
    {
        foreach (var descendant in visual.GetVisualDescendants())
        {
            if (descendant is AvaloniaVisualLayerManager visualLayerManager)
            {
                return visualLayerManager;
            }
        }

        return null;
    }
}
