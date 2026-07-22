using System.Collections.Specialized;

namespace AtomUI.Controls.Data;

internal sealed class ListCollectionEntryChangeSet
{
    public NotifyCollectionChangedAction Action { get; }
    public int OldStartingIndex { get; }
    public int NewStartingIndex { get; }
    public IReadOnlyList<ListCollectionEntrySnapshot> OldEntries { get; }
    public IReadOnlyList<ListCollectionEntry> NewEntries { get; }
    public bool StartsNewLifecycle { get; }

    public ListCollectionEntryChangeSet(
        NotifyCollectionChangedAction action,
        int oldStartingIndex,
        int newStartingIndex,
        IReadOnlyList<ListCollectionEntrySnapshot> oldEntries,
        IReadOnlyList<ListCollectionEntry> newEntries,
        bool startsNewLifecycle)
    {
        Action           = action;
        OldStartingIndex = oldStartingIndex;
        NewStartingIndex = newStartingIndex;
        OldEntries       = oldEntries;
        NewEntries       = newEntries;
        StartsNewLifecycle = startsNewLifecycle;
    }
}

internal sealed class ListCollectionEntryChangeEventArgs : EventArgs
{
    public ListCollectionEntryChangeSet ChangeSet { get; }

    public ListCollectionEntryChangeEventArgs(ListCollectionEntryChangeSet changeSet)
    {
        ChangeSet = changeSet;
    }
}
