using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

public sealed class ListViewSelectionChangedEventArgs : RoutedEventArgs
{
    public IReadOnlyList<int> DeselectedIndexes { get; }
    public IReadOnlyList<object?> DeselectedItems { get; }
    public IReadOnlyList<int> SelectedIndexes { get; }
    public IReadOnlyList<object?> SelectedItems { get; }

    public ListViewSelectionChangedEventArgs(
        RoutedEvent routedEvent,
        IReadOnlyList<int> deselectedIndexes,
        IReadOnlyList<object?> deselectedItems,
        IReadOnlyList<int> selectedIndexes,
        IReadOnlyList<object?> selectedItems)
        : base(routedEvent)
    {
        DeselectedIndexes = deselectedIndexes;
        DeselectedItems   = deselectedItems;
        SelectedIndexes   = selectedIndexes;
        SelectedItems     = selectedItems;
    }
}
