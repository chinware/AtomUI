using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.Utils;

internal static class VisualLayerManagerReflectionExtensions
{
    #region 反射信息定义
       
    private static readonly Lazy<MethodInfo> AddLayerMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(VisualLayerManager).GetMethodInfoOrThrow("AddLayer",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    #endregion
    
    public static void AddLayer(this VisualLayerManager visualLayerManager, Control layer, int zindex)
    {
        AddLayerMethodInfo.Value.Invoke(visualLayerManager, [layer, zindex]);
    }
}