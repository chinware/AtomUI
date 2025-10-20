using AtomUI.IconPkg;

namespace AtomUI.Controls;

public class TreeViewItemData : ITreeViewItemData
{
    public TreeNodeKey? ItemKey { get; set; }
    public object? Header { get; set; }
    public Icon? Icon { get; set; }
    public bool IsEnabled { get; set; }
    public bool? IsChecked { get; set; }
    public bool IsSelected { get; set; }
    public bool IsExpanded { get; set; }
    public string? GroupName { get; set; }
    
    public IList<ITreeViewItemData> Children { get; } =  new List<ITreeViewItemData>();
}