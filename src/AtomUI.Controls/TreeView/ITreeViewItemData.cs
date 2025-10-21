namespace AtomUI.Controls;

public interface ITreeViewItemData : ITreeNode<ITreeViewItemData>
{
    bool? IsChecked { get; }
    bool IsSelected { get; }
    bool IsExpanded { get; }
    bool IsIndicatorEnabled { get; }
    string? GroupName { get; }
}
