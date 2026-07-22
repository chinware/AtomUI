namespace AtomUI.Desktop.Controls;

public interface IListViewSelection
{
    int AnchorIndex { get; }
    int SelectedIndex { get; }
    IReadOnlyList<int> SelectedIndexes { get; }
    object? SelectedItem { get; }
    IReadOnlyList<object?> SelectedItems { get; }

    bool IsSelected(int sourceIndex);
    void Select(int sourceIndex);
    void Deselect(int sourceIndex);
    void SelectRange(int startSourceIndex, int endSourceIndex);
    void Clear();
    void SelectAll();
}
