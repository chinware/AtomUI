using AtomUI.IconPkg;

namespace AtomUI.Controls;

public record TreeViewItemData : ITreeViewItemData
{
    public ITreeNode<ITreeViewItemData>? ParentNode { get; internal set; }
    public TreeNodeKey? ItemKey { get; init; }
    public object? Header { get; init; }
    public Icon? Icon { get; init; }
    public bool IsEnabled { get; init; } = true;
    public bool? IsChecked { get; init; } = false;
    public bool IsSelected { get; init; }
    public bool IsExpanded { get; init; }
    public bool IsIndicatorEnabled { get; init; } = true;
    public string? GroupName { get; init; }
    
    private IList<ITreeViewItemData> _children = [];
    public IList<ITreeViewItemData> Children
    {
        get => _children;
        init
        {
            _children = value;
            foreach (var child in _children)
            {
                if (child is TreeViewItemData item)
                {
                    item.ParentNode = this;
                }
            }
        }
    }
}