using System.Collections.Generic;

namespace AtomUI.Desktop.Controls;

internal static class ListViewSelectionUtils
{
    public static object?[] CopySelectionItems(IReadOnlyList<object?> items)
    {
        if (items.Count == 0)
        {
            return [];
        }

        var result = new object?[items.Count];
        for (var i = 0; i < items.Count; ++i)
        {
            result[i] = items[i];
        }

        return result;
    }
}
