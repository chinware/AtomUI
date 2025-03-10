using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Controls.Utils;

internal static class TopLevelReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(TopLevel))]
    private static readonly Lazy<PropertyInfo> LayoutManagerPropertyInfo = new Lazy<PropertyInfo>(() => 
        typeof(TopLevel).GetPropertyInfoOrThrow("LayoutManager",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion

    public static ILayoutManager GetLayoutManager(this TopLevel topLevel)
    {
        var value = LayoutManagerPropertyInfo.Value.GetValue(topLevel, null) as  ILayoutManager;
        Debug.Assert(value != null);
        return value;
    }
}