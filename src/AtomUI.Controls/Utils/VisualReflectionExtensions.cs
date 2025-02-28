using System.Diagnostics;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Utils;

internal static class VisualReflectionExtensions
{
    #region 反射信息定义

    private static readonly Lazy<MethodInfo> SetVisualParentMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(Visual).GetMethodInfoOrThrow("SetVisualParent",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    private static readonly Lazy<PropertyInfo> VisualChildrenPropertyInfo = new Lazy<PropertyInfo>(() =>
        typeof(Visual).GetPropertyInfoOrThrow("VisualChildren",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion
    
    public static void SetVisualParent(this Visual visual, Control? parent)
    {
        SetVisualParentMethodInfo.Value.Invoke(visual, [parent]);
    }
    
    public static void ClearVisualParentRecursive(this Visual visual)
    {
        visual.SetVisualParent(null);
        foreach (var child in visual.GetVisualChildren())
        {
            child.ClearVisualParentRecursive();
        }
    }

    /// <summary>
    /// 直接获取 IAvaloniaList
    /// </summary>
    /// <param name="visual"></param>
    /// <returns></returns>
    public static IAvaloniaList<Visual> GetVisualChildrenList(this Visual visual)
    {
        var children = VisualChildrenPropertyInfo.Value.GetValue(visual) as IAvaloniaList<Visual>;
        Debug.Assert(children != null);
        return children;
    }
    
    public static int IndexOfVisualChildren(this Visual visual, Visual child)
    {
        var children = visual.GetVisualChildrenList();
        return children.IndexOf(child);
    }
    
    public static void AddToVisualChildren(this Visual visual, Visual child)
    {
        var children = visual.GetVisualChildrenList();
        children.Add(child);
    }

    public static void InsertToVisualChildren(this Visual visual, int index, Control child)
    {
        var children = visual.GetVisualChildrenList();
        children.Insert(index, child);
    }
}