using AtomUI.IconPkg;

namespace AtomUI.Controls;

public class NavMenuTreeNode : ITreeNode
{
    public string? ItemKey { get; set; }
    public object? Header { get; set; }
    public Icon? Icon { get; set; }
    public IList<ITreeNode> Children { get; set; } = [];
}