using System.Reflection;
using Avalonia.Layout;

namespace AtomUI.Controls.Utils;

internal class LayoutUtils
{
   private static readonly PropertyInfo LayoutManagerPropertyInfo;
   
   static LayoutUtils()
   {
      LayoutManagerPropertyInfo = typeof(ILayoutRoot).GetProperty("LayoutManager", BindingFlags.Instance | BindingFlags.NonPublic)!;
   }

   public static ILayoutManager GetLayoutManager(ILayoutRoot visualRoot)
   {
      return (LayoutManagerPropertyInfo.GetValue(visualRoot) as ILayoutManager)!;
   }
}