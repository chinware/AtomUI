using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal static class ILogicalExtensions
{
    public static void SetLogicalParent(this ILogical child, ILogical? parent)
    {
        ((ISetLogicalParent)child).SetParent(parent);
    }
}