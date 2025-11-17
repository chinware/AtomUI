using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls.Utils;

internal static class ILogicalExtensions
{
    public static void SetLogicalParent(this ILogical child, ILogical? parent)
    {
        ((ISetLogicalParent)child).SetParent(parent);
    }
}