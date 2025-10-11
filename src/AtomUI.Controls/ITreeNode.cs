using AtomUI.IconPkg;

namespace AtomUI.Controls;

public interface ITreeNode
{
    string? ItemKey { get; }
    object? Header { get; }
    Icon? Icon { get; }
    IList<ITreeNode> Children { get; }
}