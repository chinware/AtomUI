using AtomUI.IconPkg;
using Avalonia.Input;

namespace AtomUI.Controls;

public interface IMenuItemData : ITreeNode<IMenuItemData>
{
    KeyGesture? InputGesture { get; }
}

public class MenuItemData : IMenuItemData
{
    public TreeNodeKey? ItemKey { get; init; }
    public object? Header { get; init; }
    public Icon? Icon { get; init; }
    public bool IsEnabled { get; init; } = true;
    public KeyGesture? InputGesture { get; init; }
    public IList<IMenuItemData> Children { get; init; } = [];
}