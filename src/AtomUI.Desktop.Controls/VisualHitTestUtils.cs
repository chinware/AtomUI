using Avalonia;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal static class VisualHitTestUtils
{
    public static bool ContainsSelfOrDescendantAt(this Visual container, Point point)
    {
        foreach (var visual in container.GetVisualsAt(point))
        {
            if (container == visual || container.IsVisualAncestorOf(visual))
            {
                return true;
            }
        }

        return false;
    }
}
