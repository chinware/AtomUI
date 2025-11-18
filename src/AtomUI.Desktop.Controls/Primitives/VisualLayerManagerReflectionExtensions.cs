using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls.Primitives;

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
}