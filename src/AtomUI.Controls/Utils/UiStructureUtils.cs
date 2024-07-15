using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Utils;

internal static class UiStructureUtils
{
   private static readonly MethodInfo SetVisualParentMethodInfo;
   private static readonly PropertyInfo LogicalChildrenInfo;

   static UiStructureUtils()
   {
      SetVisualParentMethodInfo = typeof(Visual).GetMethod("SetVisualParent", BindingFlags.Instance | BindingFlags.NonPublic)!;
      LogicalChildrenInfo = typeof(StyledElement).GetProperty("LogicalChildren", BindingFlags.Instance | BindingFlags.NonPublic)!;
   }

   public static void SetVisualParent(Visual control, Control? parent)
   {
      SetVisualParentMethodInfo.Invoke(control, new object?[] { parent });
   }

   public static void SetLogicalParent(ILogical control, Control? parent)
   {
      ((ISetLogicalParent)control).SetParent(parent);
   }
   
   public static void ClearVisualParentRecursive(Visual control, Control? parent)
   {
      SetVisualParent(control, null);
      foreach (var child in control.GetVisualChildren()) {
         ClearVisualParentRecursive(child, parent);
      }
   }
   
   public static void ClearLogicalParentRecursive(ILogical control, Control? parent)
   {
      ((ISetLogicalParent)control).SetParent(parent);
      foreach (var child in control.GetLogicalChildren()) {
         ClearLogicalParentRecursive(child, parent);
      }
   }

   public static void AddToLogicalChildren(StyledElement parent, Control child)
   {
      var value = LogicalChildrenInfo.GetValue(parent);
      if (value is IAvaloniaList<ILogical> logicalChildren) {
         logicalChildren.Add(child);
      }
   }
}