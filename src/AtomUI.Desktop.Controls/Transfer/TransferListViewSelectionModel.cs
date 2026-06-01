using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Controls.Data;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Data;

namespace AtomUI.Desktop.Controls;

internal class TransferListViewSelectionModel : SelectionModel<object?>, ISelectionModel
{
    private IList? _writableSelectedItems;
    private int _ignoreModelChanges;
    private bool _ignoreSelectedItemsChanges;
    private bool _skipSyncFromSelectedItems;
    private bool _isResetting;
    private ListCollectionView? _collectionView;

    IEnumerable? ISelectionModel.Source 
    {
        get => Source;
        set => SetListSource(value);
    }
    
    public new IEnumerable? Source
    {
        get => base.Source;
        set => SetListSource(value);
    }
    
    public TransferListViewSelectionModel()
    {
        SelectionChanged += OnSelectionChanged;
        SourceReset      += OnSourceReset;
    }

    [AllowNull]
    public IList WritableSelectedItems
    {
        get
        {
            if (_writableSelectedItems is null)
            {
                _writableSelectedItems = new AvaloniaList<object?>();
                SubscribeToSelectedItems();
            }

            return _writableSelectedItems;
        }
        set
        {
            value ??= new AvaloniaList<object?>();

            if (value.IsFixedSize)
            {
                throw new NotSupportedException("Cannot assign fixed size selection to SelectedItems.");
            }

            if (_writableSelectedItems != value)
            {
                UnsubscribeFromSelectedItems();
                _writableSelectedItems = value;
                SyncFromSelectedItems();
                SubscribeToSelectedItems();
                    
                RaisePropertyChanged(nameof(WritableSelectedItems));
            }
        }
    }

    internal void Update(IEnumerable? source, Optional<IList?> selectedItems)
    {
        var previousSource                = Source;
        var previousWritableSelectedItems = _writableSelectedItems;

        base.OnSourceCollectionChangeStarted();
            
        try
        {
            _skipSyncFromSelectedItems = true;
            SetListSource(source);
            if (selectedItems.HasValue)
            {
                WritableSelectedItems = selectedItems.Value;
            }
        }
        finally 
        { 
            _skipSyncFromSelectedItems = false;
        }

        // We skipped the sync from WritableSelectedItems before; do it now that both
        // the source and WritableSelectedItems are updated.
        if (previousWritableSelectedItems != _writableSelectedItems)
        {
            base.OnSourceCollectionChangeFinished();
            SyncFromSelectedItems();
        }
        else if (previousSource != Source)
        {
            SyncFromSelectedItems();
            base.OnSourceCollectionChangeFinished();
        }
        else
        {
            base.OnSourceCollectionChangeFinished();
        }
    }

    protected void SetListSource(IEnumerable? value)
    {
        var collectionView = value as ListCollectionView;
        if (collectionView != null)
        {
            if (_collectionView == collectionView)
            {
                return;
            }

            
        }
        else
        {
            if (Source == value)
            {
                return;
            }
        }

        var shouldSyncFromSelectedItems = Source is object && value is object;
        if (shouldSyncFromSelectedItems)
        {
            _ = WritableSelectedItems;
        }

        try
        {
            _ignoreSelectedItemsChanges = true;
            ++_ignoreModelChanges;
            base.Source = value;
            _collectionView = collectionView;
        }
        finally
        {
            --_ignoreModelChanges;
            _ignoreSelectedItemsChanges = false;
        }

        if (!shouldSyncFromSelectedItems)
        {
            SyncToSelectedItems();
        }
        else
        {
            SyncFromSelectedItems();
        }
    }

    private void SyncToSelectedItems()
    {
        if (_writableSelectedItems is object &&
            !SequenceEqual(_writableSelectedItems, base.SelectedItems))
        {
            try
            {
                _ignoreSelectedItemsChanges = true;
                _writableSelectedItems.Clear();

                var selectedItems = base.SelectedItems;
                for (var i = 0; i < selectedItems.Count; i++)
                {
                    _writableSelectedItems.Add(selectedItems[i]);
                }
            }
            finally
            {
                _ignoreSelectedItemsChanges = false;
            }
        }
    }

    private void SyncFromSelectedItems()
    {
        if (_skipSyncFromSelectedItems || Source is null || _writableSelectedItems is null)
        {
            return;
        }

        try
        {
            ++_ignoreModelChanges;

            using (BatchUpdate())
            {
                Clear();

                for (var i = 0; i < _writableSelectedItems.Count; ++i)
                {
                    var index = IndexOf(Source, _writableSelectedItems[i]);

                    if (index != -1)
                    {
                        Select(index);
                    }
                    else
                    {
                        _writableSelectedItems.RemoveAt(i);
                        --i;
                    }
                }
            }
        }
        finally
        {
            --_ignoreModelChanges;
        }
    }

    private void SubscribeToSelectedItems()
    {
        if (_writableSelectedItems is INotifyCollectionChanged incc)
        {
            incc.CollectionChanged += OnSelectedItemsCollectionChanged;
        }
    }

    private void UnsubscribeFromSelectedItems()
    {
        if (_writableSelectedItems is INotifyCollectionChanged incc)
        {
            incc.CollectionChanged -= OnSelectedItemsCollectionChanged;
        }
    }

    private void OnSelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs e)
    {
        if (_ignoreModelChanges > 0)
        {
            return;
        }

        try
        {
            var items      = WritableSelectedItems;
            var deselected = e.DeselectedItems;
            var selected   = e.SelectedItems;

            _ignoreSelectedItemsChanges = true;

            for (var i = 0; i < deselected.Count; i++)
            {
                items.Remove(deselected[i]);
            }

            for (var i = 0; i < selected.Count; i++)
            {
                items.Add(selected[i]);
            }
        }
        finally
        {
            _ignoreSelectedItemsChanges = false;
        }
    }

    protected override void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            ++_ignoreModelChanges;
            _isResetting = true;
        }

        base.OnSourceCollectionChanged(e);
    }

    protected override void OnSourceCollectionChangeFinished()
    {
        base.OnSourceCollectionChangeFinished();

        if (_isResetting)
        {
            --_ignoreModelChanges;
            _isResetting = false;
        }
    }

    private void OnSourceReset(object? sender, EventArgs e) => SyncFromSelectedItems();

    private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_ignoreSelectedItemsChanges)
        {
            return;
        }

        if (_writableSelectedItems == null)
        {
            throw new AvaloniaInternalException("CollectionChanged raised but we don't have items.");
        }

        void Remove()
        {
            var oldItems = e.OldItems!;
            for (var i = 0; i < oldItems.Count; i++)
            {
                var index = IndexOf(Source, oldItems[i]);

                if (index != -1)
                {
                    Deselect(index);
                }
            }
        }

        try
        {
            using var operation = BatchUpdate();

            ++_ignoreModelChanges;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems!);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Remove();
                    Add(e.NewItems!);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    Add(_writableSelectedItems);
                    break;
            }
        }
        finally
        {
            --_ignoreModelChanges;
        }
    }

    private void Add(IList newItems)
    {
        for (var i = 0; i < newItems.Count; i++)
        {
            var index = IndexOf(Source, newItems[i]);

            if (index != -1)
            {
                Select(index);
            }
        }
    }

    private static int IndexOf(object? source, object? item)
    {
        if (source is IList l)
        {
            return l.IndexOf(item);
        }
        else if (source is ItemsSourceView v)
        {
            return v.IndexOf(item);
        }

        return -1;
    }

    private static bool SequenceEqual(IList first, IReadOnlyList<object?> second)
    {
        if (first.Count != second.Count)
        {
            return false;
        }

        var comparer = EqualityComparer<object?>.Default;
        for (var i = 0; i < first.Count; i++)
        {
            if (!comparer.Equals(first[i], second[i]))
            {
                return false;
            }
        }

        return true;
    }
    
}
