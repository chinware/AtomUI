using System.Collections;
using System.Collections.Specialized;

namespace AtomUI.Controls.Data;

internal partial class ListCollectionView : IListCollectionEntryView
{
    private readonly ListCollectionEntryStore _entryStore = new();
    private readonly List<ListCollectionEntry> _logicalEntries = [];
    private readonly List<ListCollectionViewNode> _viewNodes = [];
    private readonly Dictionary<long, int> _viewIndexByEntryId = [];

    int IListCollectionEntryView.SourceEntryCount => _entryStore.Entries.Count;
    IReadOnlyList<ListCollectionEntry> IListCollectionEntryView.LogicalEntries => _logicalEntries;

    bool IListCollectionEntryView.TryGetSourceEntry(int sourceIndex, out ListCollectionEntry? entry)
        => TryGetSourceEntry(sourceIndex, out entry);

    bool IListCollectionEntryView.TryGetViewNode(int viewIndex, out ListCollectionViewNode? node)
        => TryGetViewNode(viewIndex, out node);

    bool IListCollectionEntryView.TryGetSourceIndex(long entryId, out int sourceIndex)
        => TryGetSourceIndex(entryId, out sourceIndex);

    bool IListCollectionEntryView.TryGetViewIndex(long entryId, out int viewIndex)
        => TryGetViewIndex(entryId, out viewIndex);

    event EventHandler<ListCollectionEntryChangeEventArgs>? IListCollectionEntryView.EntryChangePrepared
    {
        add => EntryChangePrepared += value;
        remove => EntryChangePrepared -= value;
    }

    event EventHandler? IListCollectionEntryView.EntryChangeCommitted
    {
        add => EntryChangeCommitted += value;
        remove => EntryChangeCommitted -= value;
    }

    event EventHandler? IListCollectionEntryView.ProjectionCommitted
    {
        add => ProjectionCommitted += value;
        remove => ProjectionCommitted -= value;
    }

    private void InitializeEntryProjection()
    {
        _entryStore.Reset(_sourceCollection);
        _logicalEntries.Clear();
        _viewNodes.Clear();
        _viewIndexByEntryId.Clear();
    }

    private void RebuildUngroupedProjection()
    {
        RebuildEntryProjection();
    }

    private void RebuildEntryProjection()
    {
        var visibleEntries = BuildVisibleEntries();

        if (!IsGrouping)
        {
            _logicalEntries.Clear();
            _logicalEntries.AddRange(visibleEntries);
            _viewNodes.Clear();
            _viewIndexByEntryId.Clear();
            for (var index = 0; index < visibleEntries.Count; index++)
            {
                var entry = visibleEntries[index];
                _viewNodes.Add(ListCollectionViewNode.ForEntry(entry));
                _viewIndexByEntryId[entry.Id] = index;
            }
            return;
        }

        var outputItems = new List<object?>(Count);
        for (var index = 0; index < Count; index++)
        {
            outputItems.Add(GetItemAt(index));
        }

        _logicalEntries.Clear();
        _viewNodes.Clear();
        _viewIndexByEntryId.Clear();

        var remainingEntries = visibleEntries.ToList();
        foreach (var item in outputItems)
        {
            if (item is GroupListItemData groupHeader && groupHeader.IsGroupItem)
            {
                _viewNodes.Add(ListCollectionViewNode.ForGroupHeader(groupHeader));
                continue;
            }

            var entryIndex = FindEntryIndex(remainingEntries, item);
            if (entryIndex < 0)
            {
                continue;
            }

            var entry = remainingEntries[entryIndex];
            remainingEntries.RemoveAt(entryIndex);
            _logicalEntries.Add(entry);
            _viewNodes.Add(ListCollectionViewNode.ForEntry(entry));
            _viewIndexByEntryId[entry.Id] = _viewNodes.Count - 1;
        }
    }

    private List<ListCollectionEntry> BuildVisibleEntries()
    {
        var entries = _entryStore.Entries
            .Where(entry => Filter is null || (entry.Item is not null && PassesFilter(entry.Item)))
            .ToList();

        if (!CheckFlag(CollectionViewFlags.IsDataSorted) && SortDescriptions.Count > 0)
        {
            var itemType = ItemType;
            if (itemType is not null)
            {
                IEnumerable<ListCollectionEntry> sequence = entries;
                IOrderedEnumerable<ListCollectionEntry>? ordered = null;
                foreach (var sort in SortDescriptions)
                {
                    (sort as ListSortDescription)?.Initialize(itemType, _dataMemberAccessorDescriptor);
                    ordered = ordered is null
                        ? sequence.OrderBy(entry => (object)entry.Item!, sort.Comparer)
                        : ordered.ThenBy(entry => (object)entry.Item!, sort.Comparer);
                    sequence = ordered;
                }

                entries = sequence.ToList();
            }
        }

        if (PageSize > 0)
        {
            entries = entries
                .Skip(Math.Max(0, PageIndex) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        return entries;
    }

    private static int FindEntryIndex(IReadOnlyList<ListCollectionEntry> entries, object? item)
    {
        for (var index = 0; index < entries.Count; index++)
        {
            var candidate = entries[index].Item;
            if (ReferenceEquals(candidate, item) ||
                (candidate is null && item is null) ||
                (candidate is not null && item is not null &&
                 candidate.GetType().IsValueType && item.GetType().IsValueType && candidate.Equals(item)))
            {
                return index;
            }
        }

        return -1;
    }

    internal bool TryGetSourceEntry(int sourceIndex, out ListCollectionEntry? entry)
        => _entryStore.TryGetEntry(sourceIndex, out entry);

    internal bool TryGetViewNode(int viewIndex, out ListCollectionViewNode? node)
    {
        if ((uint)viewIndex < (uint)_viewNodes.Count)
        {
            node = _viewNodes[viewIndex];
            return true;
        }

        node = null;
        return false;
    }

    internal bool TryGetSourceIndex(long entryId, out int sourceIndex)
        => _entryStore.TryGetSourceIndex(entryId, out sourceIndex);

    internal bool TryGetViewIndex(long entryId, out int viewIndex)
        => _viewIndexByEntryId.TryGetValue(entryId, out viewIndex);

    internal IReadOnlyList<ListCollectionEntry> LogicalEntries => _logicalEntries;

    internal event EventHandler<ListCollectionEntryChangeEventArgs>? EntryChangePrepared;
    internal event EventHandler? EntryChangeCommitted;
    internal event EventHandler? ProjectionCommitted;

    private bool TryProcessEntryCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        if (Filter is not null || SortDescriptions.Count > 0 || GroupDescriptions.Count > 0 || PageSize > 0 || IsGrouping)
        {
            return false;
        }

        ListCollectionEntryChangeSet change;
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems is null || args.NewStartingIndex < 0)
                {
                    throw new InvalidOperationException("An indexed Add notification is required.");
                }

                change = _entryStore.Add(args.NewStartingIndex, args.NewItems);
                InsertInternalRange(args.NewStartingIndex, args.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems is null || args.OldStartingIndex < 0)
                {
                    throw new InvalidOperationException("An indexed Remove notification is required.");
                }

                change = _entryStore.Remove(args.OldStartingIndex, args.OldItems.Count);
                RemoveInternalRange(args.OldStartingIndex, args.OldItems.Count);
                break;
            case NotifyCollectionChangedAction.Move:
                if (args.OldItems is null || args.OldStartingIndex < 0 || args.NewStartingIndex < 0)
                {
                    throw new InvalidOperationException("An indexed Move notification is required.");
                }

                change = _entryStore.Move(args.OldStartingIndex, args.NewStartingIndex, args.OldItems.Count);
                var movedItems = GetInternalRange(args.OldStartingIndex, args.OldItems.Count);
                RemoveInternalRange(args.OldStartingIndex, args.OldItems.Count);
                InsertInternalRange(args.NewStartingIndex, movedItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                if (args.OldItems is null || args.NewItems is null || args.OldStartingIndex < 0)
                {
                    throw new InvalidOperationException("An indexed Replace notification is required.");
                }

                change = _entryStore.Replace(args.OldStartingIndex, args.OldItems, args.NewItems);
                RemoveInternalRange(args.OldStartingIndex, args.OldItems.Count);
                InsertInternalRange(args.OldStartingIndex, args.NewItems);
                break;
            case NotifyCollectionChangedAction.Reset:
                change = _entryStore.Reset(_sourceCollection);
                RebuildEntryProjection();
                EntryChangePrepared?.Invoke(this, new ListCollectionEntryChangeEventArgs(change));
                try
                {
                    RefreshInternal();
                }
                finally
                {
                    EntryChangeCommitted?.Invoke(this, EventArgs.Empty);
                }

                return true;
            default:
                throw new InvalidOperationException($"Unsupported collection change action: {args.Action}.");
        }

        RebuildUngroupedProjection();
        EntryChangePrepared?.Invoke(this, new ListCollectionEntryChangeEventArgs(change));
        try
        {
            NotifyCollectionChangedEventArgs publicChange = args.Action switch
            {
                NotifyCollectionChangedAction.Add => new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, args.NewItems, args.NewStartingIndex),
                NotifyCollectionChangedAction.Remove => new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove, args.OldItems, args.OldStartingIndex),
                NotifyCollectionChangedAction.Move => new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Move, args.NewItems ?? args.OldItems, args.NewStartingIndex, args.OldStartingIndex),
                NotifyCollectionChangedAction.Replace => new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace, args.NewItems!, args.OldItems!, args.OldStartingIndex),
                _ => throw new InvalidOperationException(),
            };
            HandleCollectionChanged(publicChange);
            if (args.Action == NotifyCollectionChangedAction.Replace &&
                args.OldItems!.Count != args.NewItems!.Count)
            {
                NotifyPropertyChanged(nameof(ItemCount));
            }
        }
        finally
        {
            EntryChangeCommitted?.Invoke(this, EventArgs.Empty);
        }

        return true;
    }

    private List<object?> GetInternalRange(int index, int count)
    {
        var result = new List<object?>(count);
        for (var offset = 0; offset < count; offset++)
        {
            result.Add(_internalList[index + offset]);
        }

        return result;
    }

    private void RemoveInternalRange(int index, int count)
    {
        for (var offset = count - 1; offset >= 0; offset--)
        {
            _internalList.RemoveAt(index + offset);
        }
    }

    private void InsertInternalRange(int index, IList items)
    {
        for (var offset = 0; offset < items.Count; offset++)
        {
            _internalList.Insert(index + offset, items[offset]);
        }
    }
}
