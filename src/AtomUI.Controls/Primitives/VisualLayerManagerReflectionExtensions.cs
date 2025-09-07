using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls;

namespace AtomUI.Controls.Primitives;

internal static class VisualLayerManagerReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(VisualLayerManager))]
    private static readonly Lazy<MethodInfo> AddLayerMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(VisualLayerManager).GetMethodInfoOrThrow("AddLayer",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(VisualLayerManager))]
    private static readonly Lazy<FieldInfo> LayersFieldInfo = new Lazy<FieldInfo>(() => 
        typeof(VisualLayerManager).GetFieldInfoOrThrow("_layers",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    #endregion
    
    internal static void AddLayer(this VisualLayerManager visualLayerManager, Control layer, int zindex)
    {
        AddLayerMethodInfo.Value.Invoke(visualLayerManager, [layer, zindex]);
    }

    internal static List<Control> GetLayers(this VisualLayerManager visualLayerManager)
    {
        var layers = LayersFieldInfo.Value.GetValue(visualLayerManager) as List<Control>;
        Debug.Assert(layers != null);
        return layers;
    }
}