namespace AtomUI.Controls.Data;

internal interface IListCollectionEntryView
{
    int SourceEntryCount { get; }
    IReadOnlyList<ListCollectionEntry> LogicalEntries { get; }

    bool TryGetSourceEntry(int sourceIndex, out ListCollectionEntry? entry);
    bool TryGetViewNode(int viewIndex, out ListCollectionViewNode? node);
    bool TryGetSourceIndex(long entryId, out int sourceIndex);
    bool TryGetViewIndex(long entryId, out int viewIndex);

    event EventHandler<ListCollectionEntryChangeEventArgs>? EntryChangePrepared;
    event EventHandler? EntryChangeCommitted;
    event EventHandler? ProjectionCommitted;
}
