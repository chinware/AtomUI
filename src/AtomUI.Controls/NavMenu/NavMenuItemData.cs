using AtomUI.IconPkg;

namespace AtomUI.Controls;

public class NavMenuItemData : INavMenuItemData
{
    public string? ItemKey { get; set; }
    public object? Header { get; set; }
    public Icon? Icon { get; set; }
    public bool IsEnabled { get; set; } = true;
    public IList<INavMenuItemData> Children { get; set; } = [];
}