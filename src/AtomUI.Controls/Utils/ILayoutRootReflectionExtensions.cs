using System.Diagnostics;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Layout;

namespace AtomUI.Controls.Utils;

internal static class ILayoutRootReflectionExtensions
{
    #region 反射信息定义
    private static readonly Lazy<PropertyInfo> LayoutManagerPropertyInfo = new Lazy<PropertyInfo>(() => 
        typeof(ILayoutRoot).GetPropertyInfoOrThrow("LayoutManager",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion
    
    public static ILayoutManager GetLayoutManager(this ILayoutRoot visualRoot)
    {
        var layoutManager = LayoutManagerPropertyInfo.Value.GetValue(visualRoot) as ILayoutManager;
        Debug.Assert(layoutManager != null);
        return layoutManager;
    }
}