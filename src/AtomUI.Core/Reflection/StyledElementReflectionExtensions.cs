using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Reflection;

internal static class StyledElementReflectionExtensions
{
    #region 反射信息定义
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(StyledElement))]
    private static readonly Lazy<PropertyInfo> LogicalChildrenPropertyInfo = new Lazy<PropertyInfo>(() =>
        typeof(StyledElement).GetPropertyInfoOrThrow("LogicalChildren",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(StyledElement))]
    private static readonly Lazy<PropertyInfo> TemplatedParentPropertyInfo = new Lazy<PropertyInfo>(() =>
        typeof(StyledElement).GetPropertyInfoOrThrow("TemplatedParent",
            BindingFlags.Instance | BindingFlags.Public));
    
    #endregion
    
    public static void AddToLogicalChildren(this StyledElement styledElement, ILogical child)
    {
        var children = styledElement.GetLogicalChildrenList();
        children.Add(child);
    }

    public static void InsertToLogicalChildren(this StyledElement styledElement, int index, Control child)
    {
        var children = styledElement.GetLogicalChildrenList();
        children.Insert(index, child);
    }
    
    public static IAvaloniaList<ILogical> GetLogicalChildrenList(this StyledElement styledElement)
    {
        var children = LogicalChildrenPropertyInfo.Value.GetValue(styledElement) as IAvaloniaList<ILogical>;
        Debug.Assert(children != null);
        return children;
    }
    
    public static void SetTemplatedParent(this StyledElement styledElement, AvaloniaObject? templateParent)
    {
        TemplatedParentPropertyInfo.Value.SetValue(styledElement, templateParent);
    }
    
    public static void SetTemplatedParentRecursive(this StyledElement styledElement, AvaloniaObject? templateParent)
    {
        SetTemplatedParent(styledElement, templateParent);
        if (styledElement is Visual visual)
        {
            foreach (var child in visual.GetVisualChildren())
            {
                if (child is StyledElement styledChild)
                {
                    SetTemplatedParentRecursive(styledChild, templateParent);
                }
            }
        }
    }
}