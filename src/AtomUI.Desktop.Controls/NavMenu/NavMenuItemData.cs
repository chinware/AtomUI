using AtomUI.IconPkg;

namespace AtomUI.Controls;

public interface INavMenuItemData : ITreeNode<INavMenuItemData>
{
}

public record NavMenuItemData : INavMenuItemData
{
    public ITreeNode<INavMenuItemData>? ParentNode { get; internal set; }
    public TreeNodeKey? ItemKey { get; init; }
    public object? Header { get; init; }
    public Icon? Icon { get; init; }
    public bool IsEnabled { get; init; } = true;

    private IList<INavMenuItemData> _children = [];
    public IList<INavMenuItemData> Children
    {
        get => _children;
        init
        {
            _children = value;
            foreach (var child in _children)
            {
                if (child is NavMenuItemData item)
                {
                    item.ParentNode = this;
                }
            }
        }
    }
}