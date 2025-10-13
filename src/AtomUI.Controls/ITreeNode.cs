using AtomUI.IconPkg;

namespace AtomUI.Controls;

public interface ITreeNode<TChild>
    where TChild : class, ITreeNode<TChild>
{
    string? ItemKey { get; }
    object? Header { get; }
    Icon? Icon { get; }
    bool IsEnabled { get; }
    IList<TChild> Children { get; }
}