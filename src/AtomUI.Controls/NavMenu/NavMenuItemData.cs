using AtomUI.IconPkg;

namespace AtomUI.Controls;

public record NavMenuItemData : INavMenuItemData
{
    public TreeNodeKey? ItemKey { get; init; }
    public object? Header { get; init; }
    public Icon? Icon { get; init; }
    public bool IsEnabled { get; init; } = true;
    public IList<INavMenuItemData> Children { get; init; } = [];
}