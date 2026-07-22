using System.Collections;
using System.Collections.Specialized;

namespace AtomUI.Controls.Data;

/// <summary>
/// Owns source-order entry identity for one collection-view lifecycle.
/// </summary>
internal sealed class ListCollectionEntryStore
{
    private readonly List<ListCollectionEntry> _entries = [];
    private readonly Dictionary<long, int> _sourceIndexById = [];
    private long _nextId;

    public IReadOnlyList<ListCollectionEntry> Entries => _entries;

    public bool TryGetEntry(int sourceIndex, out ListCollectionEntry? entry)
    {
        if ((uint)sourceIndex < (uint)_entries.Count)
        {
            entry = _entries[sourceIndex];
            return true;
        }

        entry = null;
        return false;
    }

    public bool TryGetSourceIndex(long entryId, out int sourceIndex)
        => _sourceIndexById.TryGetValue(entryId, out sourceIndex);

    public ListCollectionEntryChangeSet Add(int newStartingIndex, IList newItems)
    {
        ArgumentNullException.ThrowIfNull(newItems);
        ValidateInsertIndex(newStartingIndex);

        var entries = CreateEntries(newItems);
        _entries.InsertRange(newStartingIndex, entries);
        ReindexFrom(newStartingIndex);
        return new ListCollectionEntryChangeSet(
            NotifyCollectionChangedAction.Add,
            -1,
            newStartingIndex,
            [],
            entries,
            startsNewLifecycle: false);
    }

    public ListCollectionEntryChangeSet Remove(int oldStartingIndex, int count)
    {
        ValidateRange(oldStartingIndex, count);

        var oldEntries = Snapshot(oldStartingIndex, count);
        _entries.RemoveRange(oldStartingIndex, count);
        foreach (var entry in oldEntries)
        {
            _sourceIndexById.Remove(entry.Id);
        }

        ReindexFrom(oldStartingIndex);
        return new ListCollectionEntryChangeSet(
            NotifyCollectionChangedAction.Remove,
            oldStartingIndex,
            -1,
            oldEntries,
            [],
            startsNewLifecycle: false);
    }

    public ListCollectionEntryChangeSet Move(int oldStartingIndex, int newStartingIndex, int count)
    {
        ValidateRange(oldStartingIndex, count);
        if (newStartingIndex < 0 || newStartingIndex > _entries.Count - count)
        {
            throw new InvalidOperationException("The move destination is outside the source entry range.");
        }

        var moved = _entries.GetRange(oldStartingIndex, count);
        var snapshots = moved
            .Select(static entry => new ListCollectionEntrySnapshot(entry.Id, entry.Item))
            .ToArray();
        _entries.RemoveRange(oldStartingIndex, count);
        _entries.InsertRange(newStartingIndex, moved);
        ReindexFrom(Math.Min(oldStartingIndex, newStartingIndex));

        return new ListCollectionEntryChangeSet(
            NotifyCollectionChangedAction.Move,
            oldStartingIndex,
            newStartingIndex,
            snapshots,
            moved,
            startsNewLifecycle: false);
    }

    public ListCollectionEntryChangeSet Replace(
        int startingIndex,
        IList oldItems,
        IList newItems)
    {
        ArgumentNullException.ThrowIfNull(oldItems);
        ArgumentNullException.ThrowIfNull(newItems);
        ValidateRange(startingIndex, oldItems.Count);

        if (oldItems.Count == newItems.Count)
        {
            var oldEntries = Snapshot(startingIndex, oldItems.Count);
            var newEntries = new List<ListCollectionEntry>(newItems.Count);
            for (var offset = 0; offset < newItems.Count; offset++)
            {
                var entry = _entries[startingIndex + offset];
                entry.Item = newItems[offset];
                newEntries.Add(entry);
            }

            return new ListCollectionEntryChangeSet(
                NotifyCollectionChangedAction.Replace,
                startingIndex,
                startingIndex,
                oldEntries,
                newEntries,
                startsNewLifecycle: false);
        }

        var oldSnapshots = Snapshot(startingIndex, oldItems.Count);
        _entries.RemoveRange(startingIndex, oldItems.Count);
        foreach (var entry in oldSnapshots)
        {
            _sourceIndexById.Remove(entry.Id);
        }

        var inserted = CreateEntries(newItems);
        _entries.InsertRange(startingIndex, inserted);
        ReindexFrom(startingIndex);
        return new ListCollectionEntryChangeSet(
            NotifyCollectionChangedAction.Replace,
            startingIndex,
            startingIndex,
            oldSnapshots,
            inserted,
            startsNewLifecycle: false);
    }

    public ListCollectionEntryChangeSet Reset(IEnumerable source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var items = source.Cast<object?>().ToArray();
        var oldEntries = _entries
            .Select(static entry => new ListCollectionEntrySnapshot(entry.Id, entry.Item))
            .ToArray();

        _entries.Clear();
        _sourceIndexById.Clear();
        var newEntries = CreateEntries(items);
        _entries.AddRange(newEntries);
        ReindexFrom(0);

        return new ListCollectionEntryChangeSet(
            NotifyCollectionChangedAction.Reset,
            -1,
            -1,
            oldEntries,
            newEntries,
            startsNewLifecycle: true);
    }

    public ListCollectionEntryChangeSet Reset(params object?[] source)
        => Reset((IEnumerable)source);

    private List<ListCollectionEntry> CreateEntries(IList items)
    {
        var entries = new List<ListCollectionEntry>(items.Count);
        for (var index = 0; index < items.Count; index++)
        {
            entries.Add(new ListCollectionEntry(++_nextId, items[index]));
        }

        return entries;
    }

    private List<ListCollectionEntrySnapshot> Snapshot(int startingIndex, int count)
    {
        var snapshots = new List<ListCollectionEntrySnapshot>(count);
        for (var index = startingIndex; index < startingIndex + count; index++)
        {
            var entry = _entries[index];
            snapshots.Add(new ListCollectionEntrySnapshot(entry.Id, entry.Item));
        }

        return snapshots;
    }

    private void ValidateInsertIndex(int index)
    {
        if (index < 0 || index > _entries.Count)
        {
            throw new InvalidOperationException("The insertion index is outside the source entry range.");
        }
    }

    private void ValidateRange(int startingIndex, int count)
    {
        if (startingIndex < 0 || count < 0 || startingIndex > _entries.Count - count)
        {
            throw new InvalidOperationException("The source entry range is invalid.");
        }
    }

    private void ReindexFrom(int sourceIndex)
    {
        for (var index = Math.Max(0, sourceIndex); index < _entries.Count; index++)
        {
            _sourceIndexById[_entries[index].Id] = index;
        }
    }
}
