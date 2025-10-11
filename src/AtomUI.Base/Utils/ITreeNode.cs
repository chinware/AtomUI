namespace AtomUI.Utils;

public interface ITreeNode
{
    string? ItemKey { get; }
    object? Header { get; set; }
    IList<ITreeNode> Children { get; }
}