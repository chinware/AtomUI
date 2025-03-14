using Avalonia;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Utils;

public static class VisualExtensions
{
    public static T? FindChildOfType<T>(this Visual? visual) where T : class
    {
        if (visual == null)
        {
            return null;
        }
           
        IEnumerable<Visual> visualChildren = visual.GetVisualChildren();
        foreach (var child in visualChildren)
        {
            if (child is T target)
            {
                return target;
            }
        }
        return null;
    }
    
    public static bool HasChild(this Visual? visual, Visual searchedChild)
    {
        if (visual == null)
        {
            return false;
        }
           
        IEnumerable<Visual> visualChildren = visual.GetVisualChildren();
        foreach (var child in visualChildren)
        {
            if (child == searchedChild)
            {
                return true;
            }
        }
        return false;
    }
}