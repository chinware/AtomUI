using System.Collections;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class CheckBoxGroupCheckedChangedEventArgs: RoutedEventArgs
{
    public CheckBoxGroupCheckedChangedEventArgs(RoutedEvent routedEvent, IList removedItems, IList addedItems)
        : base(routedEvent)
    {
        RemovedItems = removedItems;
        AddedItems   = addedItems;
    }
    
    public IList AddedItems { get; }
    
    public IList RemovedItems { get; }
}