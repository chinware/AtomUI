using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal static class MediaQueryHost
{
    internal static IMediaBreakAwareControl? FindOwner(Visual visual)
    {
        foreach (var ancestor in visual.GetSelfAndVisualAncestors())
        {
            if (ancestor is IMediaBreakAwareControl owner)
            {
                return owner;
            }
        }

        return TopLevel.GetTopLevel(visual) as IMediaBreakAwareControl;
    }
}
