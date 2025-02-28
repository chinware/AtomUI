using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Utils;

internal static class VisualAndLogicalUtils
{
    private static readonly PropertyInfo LogicalChildrenInfo;
    private static readonly PropertyInfo VisualChildrenInfo;
    private static readonly PropertyInfo TemplateParentInfo;

    static VisualAndLogicalUtils()
    {
        LogicalChildrenInfo =
            typeof(StyledElement).GetProperty("LogicalChildren", BindingFlags.Instance | BindingFlags.NonPublic)!;
        VisualChildrenInfo =
            typeof(Visual).GetProperty("VisualChildren", BindingFlags.Instance | BindingFlags.NonPublic)!;
        TemplateParentInfo =
            typeof(StyledElement).GetProperty("TemplatedParent", BindingFlags.Instance | BindingFlags.Public)!;
    }

    public static void SetLogicalParent(ILogical control, ILogical? parent)
    {
        ((ISetLogicalParent)control).SetParent(parent);
    }

    public static void AddToLogicalChildren(StyledElement parent, Control child)
    {
        var value = LogicalChildrenInfo.GetValue(parent);
        if (value is IAvaloniaList<ILogical> logicalChildren)
        {
            logicalChildren.Add(child);
        }
    }

    public static void InsertToLogicalChildren(StyledElement parent, int index, Control child)
    {
        var value = LogicalChildrenInfo.GetValue(parent);
        if (value is IAvaloniaList<ILogical> logicalChildren)
        {
            logicalChildren.Insert(index, child);
        }
    }

    public static void AddToVisualChildren(StyledElement parent, Control child)
    {
        var value = VisualChildrenInfo.GetValue(parent);
        if (value is IAvaloniaList<Visual> visualChildren)
        {
            visualChildren.Add(child);
        }
    }

    public static void InsertToVisualChildren(StyledElement parent, int index, Control child)
    {
        var value = VisualChildrenInfo.GetValue(parent);
        if (value is IAvaloniaList<Visual> visualChildren)
        {
            visualChildren.Insert(index, child);
        }
    }

    public static int IndexOfVisualChildren(StyledElement parent, Control child)
    {
        var value = VisualChildrenInfo.GetValue(parent);
        if (value is IAvaloniaList<Visual> visualChildren)
        {
            return visualChildren.IndexOf(child);
        }

        return -1;
    }

    public static void SetTemplateParent(StyledElement control, AvaloniaObject? templateParent)
    {
        TemplateParentInfo.SetValue(control, templateParent);
    }
}