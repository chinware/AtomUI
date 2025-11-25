using Avalonia.Controls;

namespace AtomUI.Controls;

public interface ITreeNode<TChild>
    where TChild : class, ITreeNode<TChild>
{
    ITreeNode<TChild>? ParentNode { get; }
    TreeNodeKey? ItemKey { get; }
    object? Header { get; }
    PathIcon? Icon { get; }
    bool IsEnabled { get; }
    IList<TChild> Children { get; }
}