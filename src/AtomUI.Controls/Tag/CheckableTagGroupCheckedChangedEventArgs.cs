using System.Collections;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class CheckableTagGroupCheckedChangedEventArgs : RoutedEventArgs
{
    public CheckableTagGroupCheckedChangedEventArgs(
        RoutedEvent routedEvent,
        bool isMultiple,
        object? oldCheckedItem,
        object? newCheckedItem,
        IList addedItems,
        IList removedItems)
        : base(routedEvent)
    {
        IsMultiple    = isMultiple;
        OldCheckedItem = oldCheckedItem;
        NewCheckedItem = newCheckedItem;
        AddedItems    = addedItems;
        RemovedItems  = removedItems;
    }

    public bool IsMultiple { get; }

    public object? OldCheckedItem { get; }

    public object? NewCheckedItem { get; }

    public IList AddedItems { get; }

    public IList RemovedItems { get; }
}
