using System.Collections.ObjectModel;
using AtomUI.Controls.Data;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class ListViewSelectionModel : IListViewSelection
{
    private readonly HashSet<long> _selectedEntryIds = [];
    private IListCollectionEntryView? _view;
    private long? _anchorEntryId;
    private long? _activeEntryId;
    private SelectionMode _mode = SelectionMode.Single;
    private int _batchDepth;
    private ListViewSelectionSnapshot? _batchOldState;
    private bool _batchChanged;
    private ListViewSelectionChange? _pendingEntryChange;

    public ListViewSelectionModel(IListCollectionEntryView? view = null)
    {
        _view = view;
    }

    public SelectionMode Mode
    {
        get => _mode;
        set => _mode = value;
    }

    public int AnchorIndex => ResolveSourceIndex(_anchorEntryId);
    public int SelectedIndex => ResolveSourceIndex(_activeEntryId);
    public IReadOnlyList<int> SelectedIndexes => BuildSnapshot().SelectedIndexes;
    public object? SelectedItem => BuildSnapshot().SelectedItem;
    public IReadOnlyList<object?> SelectedItems => BuildSnapshot().SelectedItems;

    internal IReadOnlyCollection<long> SelectedEntryIds => new ReadOnlyCollection<long>(_selectedEntryIds.ToArray());

    internal void AttachView(IListCollectionEntryView? view)
    {
        _view = view;
        RemoveStaleEntries();
    }

    public bool IsSelected(int sourceIndex)
        => TryGetEntry(sourceIndex, out var entry) && _selectedEntryIds.Contains(entry!.Id);

    public void Select(int sourceIndex)
    {
        if (!TryGetEntry(sourceIndex, out var entry) || !IsEnabled(entry!.Item))
        {
            return;
        }

        Mutate(() =>
        {
            if (HasAllFlags(_mode, SelectionMode.Toggle) && HasAllFlags(_mode, SelectionMode.Multiple))
            {
                if (!_selectedEntryIds.Remove(entry.Id))
                {
                    _selectedEntryIds.Add(entry.Id);
                }
            }
            else
            {
                if (!HasAllFlags(_mode, SelectionMode.Multiple))
                {
                    _selectedEntryIds.Clear();
                }

                _selectedEntryIds.Add(entry.Id);
            }

            _anchorEntryId = entry.Id;
            _activeEntryId = entry.Id;
        });
    }

    public void Deselect(int sourceIndex)
    {
        if (!TryGetEntry(sourceIndex, out var entry))
        {
            return;
        }

        Mutate(() =>
        {
            if (_selectedEntryIds.Remove(entry!.Id))
            {
                if (_anchorEntryId == entry.Id)
                {
                    _anchorEntryId = null;
                }

                if (_activeEntryId == entry.Id)
                {
                    _activeEntryId = null;
                }
            }

            ApplyAlwaysSelectedFallback();
        });
    }

    public void Clear()
    {
        Mutate(() =>
        {
            _selectedEntryIds.Clear();
            _anchorEntryId = null;
            _activeEntryId = null;
            ApplyAlwaysSelectedFallback();
        });
    }

    public void SelectAll()
    {
        Mutate(() =>
        {
            foreach (var entry in _view?.LogicalEntries ?? Array.Empty<ListCollectionEntry>())
            {
                if (IsEnabled(entry.Item))
                {
                    _selectedEntryIds.Add(entry.Id);
                }
            }

            var first = _view?.LogicalEntries.FirstOrDefault(entry => IsEnabled(entry.Item));
            if (first is not null)
            {
                _anchorEntryId ??= first.Id;
                _activeEntryId ??= first.Id;
            }
        });
    }

    internal void SelectRangeToEntry(long entryId, bool preserveExisting)
    {
        if (_view is null || !_view.TryGetSourceIndex(entryId, out _) || !TryGetLogicalIndex(entryId, out var targetIndex))
        {
            return;
        }

        var anchorId = _anchorEntryId;
        if (anchorId is null || !TryGetLogicalIndex(anchorId.Value, out var anchorIndex))
        {
            Select(ResolveSourceIndex(entryId));
            return;
        }

        Mutate(() =>
        {
            if (!preserveExisting)
            {
                _selectedEntryIds.Clear();
            }

            var start = Math.Min(anchorIndex, targetIndex);
            var end = Math.Max(anchorIndex, targetIndex);
            for (var index = start; index <= end; index++)
            {
                var entry = _view.LogicalEntries[index];
                if (IsEnabled(entry.Item))
                {
                    _selectedEntryIds.Add(entry.Id);
                }
            }

            _activeEntryId = entryId;
        });
    }

    internal IDisposable BeginSelectionBatchUpdate() => new BatchScope(this);

    public void BeginBatchUpdate() => _batchDepth++;

    public void EndBatchUpdate()
    {
        if (_batchDepth == 0)
        {
            return;
        }

        _batchDepth--;
        if (_batchDepth == 0 && _batchChanged && _batchOldState is not null)
        {
            var oldState = _batchOldState;
            _batchOldState = null;
            _batchChanged = false;
            Publish(BuildChange(oldState, BuildSnapshot()));
        }
    }

    public void SelectRange(int start, int end)
    {
        if (!TryGetEntry(start, out var startEntry) || !TryGetEntry(end, out var endEntry))
        {
            return;
        }

        SelectRangeToEntry(endEntry!.Id, preserveExisting: false);
        _anchorEntryId = startEntry!.Id;
    }

    public void DeselectRange(int start, int end)
    {
        if (_view is null)
        {
            return;
        }

        var first = Math.Min(start, end);
        var last = Math.Max(start, end);
        using var batch = BeginSelectionBatchUpdate();
        for (var index = first; index <= last; index++)
        {
            Deselect(index);
        }
    }

    internal void OnEntryChangePrepared(ListCollectionEntryChangeSet changeSet)
    {
        var oldState = BuildSnapshot(changeSet);
        foreach (var oldEntry in changeSet.OldEntries)
        {
            if (changeSet.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                (changeSet.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace &&
                 changeSet.NewEntries.All(entry => entry.Id != oldEntry.Id)))
            {
                _selectedEntryIds.Remove(oldEntry.Id);
                if (_anchorEntryId == oldEntry.Id)
                {
                    _anchorEntryId = null;
                }

                if (_activeEntryId == oldEntry.Id)
                {
                    _activeEntryId = null;
                }
            }
        }

        ApplyAlwaysSelectedFallback();

        var newState = BuildSnapshot();
        if (SnapshotsEqual(oldState, newState))
        {
            _pendingEntryChange = null;
            return;
        }

        var change = BuildChange(oldState, newState);
        if (_batchDepth > 0)
        {
            _batchOldState ??= oldState;
            _batchChanged = true;
            return;
        }

        _pendingEntryChange = change;
    }

    internal void OnEntryChangeCommitted()
    {
        if (_batchDepth > 0 || _pendingEntryChange is not { } change)
        {
            return;
        }

        _pendingEntryChange = null;
        Publish(change);
    }

    internal event EventHandler<ListViewSelectionChange>? Changed;

    private void Mutate(Action action)
    {
        var oldState = BuildSnapshot();
        action();
        var newState = BuildSnapshot();
        if (SnapshotsEqual(oldState, newState))
        {
            return;
        }

        var change = BuildChange(oldState, newState);
        if (_batchDepth > 0)
        {
            _batchOldState ??= oldState;
            _batchChanged = true;
            return;
        }

        Publish(change);
    }

    private void Publish(ListViewSelectionChange change)
    {
        Changed?.Invoke(this, change);
    }

    private ListViewSelectionChange BuildChange(ListViewSelectionSnapshot oldState, ListViewSelectionSnapshot newState)
    {
        var oldIds = oldState.EntryIds.ToHashSet();
        var newIds = newState.EntryIds.ToHashSet();
        var deselected = oldState.SelectedIndexes
            .Where((_, index) => !newIds.Contains(oldState.EntryIds[index]))
            .ToArray();
        var deselectedItems = oldState.SelectedItems
            .Where((_, index) => !newIds.Contains(oldState.EntryIds[index]))
            .ToArray();
        var selected = newState.SelectedIndexes
            .Where((_, index) => !oldIds.Contains(newState.EntryIds[index]))
            .ToArray();
        var selectedItems = newState.SelectedItems
            .Where((_, index) => !oldIds.Contains(newState.EntryIds[index]))
            .ToArray();

        return new ListViewSelectionChange(
            oldState,
            newState,
            deselected,
            deselectedItems,
            selected,
            selectedItems,
            deselected.Length != 0 || selected.Length != 0);
    }

    private ListViewSelectionSnapshot BuildSnapshot(ListCollectionEntryChangeSet? previousChange = null)
    {
        var selected = _selectedEntryIds
            .Select(id => TryResolveSelectedEntry(id, previousChange, out var selectedEntry)
                ? selectedEntry
                : default)
            .Where(value => value.Id != 0)
            .OrderBy(value => value.Index)
            .ToArray();
        var indexes = selected.Select(value => value.Index).ToArray();
        var items = selected.Select(value => value.Item).ToArray();
        var activeIndex = ResolveEntryIndex(_activeEntryId, previousChange, out var activeItem);
        var anchorIndex = ResolveEntryIndex(_anchorEntryId, previousChange, out _);

        return new ListViewSelectionSnapshot(
            selected.Select(value => value.Id).ToArray(),
            activeIndex,
            anchorIndex,
            indexes,
            activeItem,
            items);
    }

    private bool TryResolveSelectedEntry(
        long entryId,
        ListCollectionEntryChangeSet? previousChange,
        out (long Id, int Index, object? Item) selectedEntry)
    {
        if (_view?.TryGetSourceIndex(entryId, out var sourceIndex) == true &&
            _view.TryGetSourceEntry(sourceIndex, out var entry))
        {
            var item = entry!.Item;
            var index = sourceIndex;
            if (previousChange is not null &&
                TryGetChangedEntry(previousChange, entryId, out var oldIndex, out var oldItem))
            {
                if (previousChange.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
                {
                    index = oldIndex;
                }
                else if (previousChange.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    item = oldItem;
                }
            }

            selectedEntry = (entryId, index, item);
            return true;
        }

        if (previousChange is not null && TryGetChangedEntry(previousChange, entryId, out var removedIndex, out var removedItem))
        {
            selectedEntry = (entryId, removedIndex, removedItem);
            return true;
        }

        selectedEntry = default;
        return false;
    }

    private int ResolveEntryIndex(long? entryId, ListCollectionEntryChangeSet? previousChange, out object? item)
    {
        item = null;
        if (entryId is null)
        {
            return -1;
        }

        var id = entryId.Value;

        if (_view?.TryGetSourceIndex(id, out var sourceIndex) == true &&
            _view.TryGetSourceEntry(sourceIndex, out var entry))
        {
            item = entry!.Item;
            if (previousChange is not null && TryGetChangedEntry(previousChange, id, out var oldIndex, out var oldItem))
            {
                if (previousChange.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
                {
                    sourceIndex = oldIndex;
                }
                else if (previousChange.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    item = oldItem;
                }
            }

            return sourceIndex;
        }

        if (previousChange is not null && TryGetChangedEntry(previousChange, id, out var removedIndex, out var removedItem))
        {
            item = removedItem;
            return removedIndex;
        }

        return -1;
    }

    private static bool TryGetChangedEntry(
        ListCollectionEntryChangeSet changeSet,
        long entryId,
        out int sourceIndex,
        out object? item)
    {
        for (var offset = 0; offset < changeSet.OldEntries.Count; offset++)
        {
            var oldEntry = changeSet.OldEntries[offset];
            if (oldEntry.Id != entryId)
            {
                continue;
            }

            sourceIndex = changeSet.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset
                ? offset
                : changeSet.OldStartingIndex + offset;
            item = oldEntry.Item;
            return sourceIndex >= 0;
        }

        sourceIndex = -1;
        item = null;
        return false;
    }

    private int ResolveSourceIndex(long? entryId)
        => entryId is not null && _view?.TryGetSourceIndex(entryId.Value, out var index) == true ? index : -1;

    private bool TryGetEntry(int sourceIndex, out ListCollectionEntry? entry)
    {
        if (_view?.TryGetSourceEntry(sourceIndex, out entry) == true)
        {
            return true;
        }

        entry = null;
        return false;
    }

    private bool TryGetLogicalIndex(long entryId, out int index)
    {
        if (_view is not null)
        {
            for (index = 0; index < _view.LogicalEntries.Count; index++)
            {
                if (_view.LogicalEntries[index].Id == entryId)
                {
                    return true;
                }
            }
        }

        index = -1;
        return false;
    }

    private void RemoveStaleEntries()
    {
        _selectedEntryIds.RemoveWhere(id => _view?.TryGetSourceIndex(id, out _) != true);
        if (_anchorEntryId is not null && _view?.TryGetSourceIndex(_anchorEntryId.Value, out _) != true)
        {
            _anchorEntryId = null;
        }

        if (_activeEntryId is not null && _view?.TryGetSourceIndex(_activeEntryId.Value, out _) != true)
        {
            _activeEntryId = null;
        }
    }

    private void ApplyAlwaysSelectedFallback()
    {
        if (!HasAllFlags(_mode, SelectionMode.AlwaysSelected) || _selectedEntryIds.Count != 0)
        {
            return;
        }

        var first = _view?.LogicalEntries.FirstOrDefault(entry => IsEnabled(entry.Item));
        if (first is not null)
        {
            _selectedEntryIds.Add(first.Id);
            _anchorEntryId = first.Id;
            _activeEntryId = first.Id;
        }
    }

    private static bool IsEnabled(object? item)
        => item is not IListItemData data || data.IsEnabled;

    private static bool HasAllFlags(SelectionMode value, SelectionMode flags)
        => (value & flags) == flags;

    private static bool SnapshotsEqual(ListViewSelectionSnapshot oldState, ListViewSelectionSnapshot newState)
        => oldState.EntryIds.SequenceEqual(newState.EntryIds) &&
           oldState.SelectedIndex == newState.SelectedIndex &&
           oldState.AnchorIndex == newState.AnchorIndex;

    private sealed class BatchScope : IDisposable
    {
        private ListViewSelectionModel? _owner;

        public BatchScope(ListViewSelectionModel owner)
        {
            _owner = owner;
            owner._batchDepth++;
        }

        public void Dispose()
        {
            if (_owner is not { } owner)
            {
                return;
            }

            _owner = null;
            if (--owner._batchDepth == 0 && owner._batchChanged && owner._batchOldState is not null)
            {
                var oldState = owner._batchOldState;
                owner._batchOldState = null;
                owner._batchChanged = false;
                owner.Publish(owner.BuildChange(oldState, owner.BuildSnapshot()));
            }
        }
    }
}

internal sealed class ListViewSelectionSnapshot
{
    public IReadOnlyList<long> EntryIds { get; }
    public int SelectedIndex { get; }
    public int AnchorIndex { get; }
    public IReadOnlyList<int> SelectedIndexes { get; }
    public object? SelectedItem { get; }
    public IReadOnlyList<object?> SelectedItems { get; }

    public ListViewSelectionSnapshot(
        IReadOnlyList<long> entryIds,
        int selectedIndex,
        int anchorIndex,
        IReadOnlyList<int> selectedIndexes,
        object? selectedItem,
        IReadOnlyList<object?> selectedItems)
    {
        EntryIds        = entryIds;
        SelectedIndex   = selectedIndex;
        AnchorIndex     = anchorIndex;
        SelectedIndexes = selectedIndexes;
        SelectedItem    = selectedItem;
        SelectedItems   = selectedItems;
    }
}

internal sealed class ListViewSelectionChange
{
    public ListViewSelectionSnapshot OldState { get; }
    public ListViewSelectionSnapshot NewState { get; }
    public IReadOnlyList<int> DeselectedIndexes { get; }
    public IReadOnlyList<object?> DeselectedItems { get; }
    public IReadOnlyList<int> SelectedIndexes { get; }
    public IReadOnlyList<object?> SelectedItems { get; }
    public bool HasBusinessSelectionChange { get; }

    public ListViewSelectionChange(
        ListViewSelectionSnapshot oldState,
        ListViewSelectionSnapshot newState,
        IReadOnlyList<int> deselectedIndexes,
        IReadOnlyList<object?> deselectedItems,
        IReadOnlyList<int> selectedIndexes,
        IReadOnlyList<object?> selectedItems,
        bool hasBusinessSelectionChange)
    {
        OldState                   = oldState;
        NewState                   = newState;
        DeselectedIndexes          = deselectedIndexes;
        DeselectedItems            = deselectedItems;
        SelectedIndexes            = selectedIndexes;
        SelectedItems              = selectedItems;
        HasBusinessSelectionChange = hasBusinessSelectionChange;
    }
}
