using AtomUI.IconPkg;

namespace AtomUI.Controls;

public class TreeViewItemData : ITreeViewItemData
{
    public TreeNodeKey? ItemKey { get; set; }
    public object? Header { get; set; }
    public Icon? Icon { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool? IsChecked { get; set; } = false;
    public bool IsSelected { get; set; }
    public bool IsExpanded { get; set; }
    public bool IsIndicatorEnabled { get; set; } = true;
    public string? GroupName { get; set; }
    public IList<ITreeViewItemData> Children { get; set; } =  new List<ITreeViewItemData>();
}