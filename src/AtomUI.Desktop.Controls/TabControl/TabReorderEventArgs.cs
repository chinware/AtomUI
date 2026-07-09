using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

public class TabReorderingEventArgs : RoutedEventArgs
{
    public TabReorderingEventArgs(RoutedEvent routedEvent, object? item, int oldIndex, int newIndex)
        : base(routedEvent)
    {
        Item     = item;
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }

    public object? Item { get; }

    public int OldIndex { get; }

    public int NewIndex { get; }

    public bool Cancel { get; set; }
}

public class TabReorderedEventArgs : RoutedEventArgs
{
    public TabReorderedEventArgs(RoutedEvent routedEvent, object? item, int oldIndex, int newIndex)
        : base(routedEvent)
    {
        Item     = item;
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }

    public object? Item { get; }

    public int OldIndex { get; }

    public int NewIndex { get; }
}
