using AtomUI.IconPkg;
using Avalonia.Input;

namespace AtomUI.Controls;

public class MenuItemData : IMenuItemData
{
    public string? ItemKey { get; set; }
    public object? Header { get; set; }
    public Icon? Icon { get; set; }
    public bool IsEnabled { get; set; } = true;
    public KeyGesture? InputGesture { get; set; }
    public IList<IMenuItemData> Children { get; set; } = [];
}