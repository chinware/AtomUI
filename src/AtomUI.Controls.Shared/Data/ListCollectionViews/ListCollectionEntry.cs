namespace AtomUI.Controls.Data;

/// <summary>
/// Represents one occurrence of an item in a source collection.
/// </summary>
internal sealed class ListCollectionEntry
{
    public long Id { get; }
    public object? Item { get; set; }

    public ListCollectionEntry(long id, object? item)
    {
        Id   = id;
        Item = item;
    }
}

internal readonly struct ListCollectionEntrySnapshot
{
    public long Id { get; }
    public object? Item { get; }

    public ListCollectionEntrySnapshot(long id, object? item)
    {
        Id   = id;
        Item = item;
    }
}
