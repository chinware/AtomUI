using AtomUI.IconPkg;
using Avalonia.Input;

namespace AtomUI.Controls;

public interface IMenuItemData : ITreeNode<IMenuItemData>
{
    KeyGesture? InputGesture { get; }
}

public class MenuItemData : IMenuItemData
{
    public TreeNodeKey? ItemKey { get; set; }
    public object? Header { get; set; }
    public Icon? Icon { get; set; }
    public bool IsEnabled { get; set; } = true;
    public KeyGesture? InputGesture { get; set; }
    public IList<IMenuItemData> Children { get; set; } = [];
}