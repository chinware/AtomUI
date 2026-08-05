using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AtomUI.Desktop.Controls;

internal sealed class NavMenuNodeChildrenView : IList<INavMenuNode>,
                                                IList,
                                                INotifyCollectionChanged,
                                                INotifyPropertyChanged
{
    private readonly IList<INavMenuEntry> _entries;
    private readonly INotifyCollectionChanged _rootNotifier;
    private readonly HashSet<INotifyCollectionChanged> _groupNotifiers = [];

    public NavMenuNodeChildrenView(IList<INavMenuEntry> entries)
    {
        _entries      = entries;
        _rootNotifier = (INotifyCollectionChanged)entries;
        _rootNotifier.CollectionChanged += HandleRootCollectionChanged;
        RebuildGroupSubscriptions();
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public int Count
    {
        get
        {
            var count = 0;
            CountNodes(_entries, ref count);
            return count;
        }
    }

    public bool IsReadOnly => false;

    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => this;

    public INavMenuNode this[int index]
    {
        get
        {
            if (!TryLocateNode(_entries, index, out _, out _, out var node))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return node;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (!TryLocateNode(_entries, index, out var owner, out var entryIndex, out _))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            owner[entryIndex] = value;
        }
    }

    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = GetNode(value);
    }

    public void Add(INavMenuNode item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _entries.Add(item);
    }

    int IList.Add(object? value)
    {
        Add(GetNode(value));
        return Count - 1;
    }

    public void Clear()
    {
        _entries.Clear();
    }

    public bool Contains(INavMenuNode item)
    {
        return IndexOf(item) >= 0;
    }

    bool IList.Contains(object? value)
    {
        return value is INavMenuNode node && Contains(node);
    }

    public void CopyTo(INavMenuNode[] array, int arrayIndex)
    {
        foreach (var node in this)
        {
            array[arrayIndex++] = node;
        }
    }

    void ICollection.CopyTo(Array array, int index)
    {
        foreach (var node in this)
        {
            array.SetValue(node, index++);
        }
    }

    public IEnumerator<INavMenuNode> GetEnumerator()
    {
        return EnumerateNodes(_entries).GetEnumerator();
    }

    public int IndexOf(INavMenuNode item)
    {
        var index = 0;
        foreach (var node in this)
        {
            if (ReferenceEquals(node, item))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    int IList.IndexOf(object? value)
    {
        return value is INavMenuNode node ? IndexOf(node) : -1;
    }

    public void Insert(int index, INavMenuNode item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (index == Count)
        {
            _entries.Add(item);
            return;
        }

        if (!TryLocateNode(_entries, index, out var owner, out var entryIndex, out _))
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        owner.Insert(entryIndex, item);
    }

    void IList.Insert(int index, object? value)
    {
        Insert(index, GetNode(value));
    }

    public bool Remove(INavMenuNode item)
    {
        if (!TryLocateNode(_entries, item, out var owner, out var entryIndex))
        {
            return false;
        }

        owner.RemoveAt(entryIndex);
        return true;
    }

    void IList.Remove(object? value)
    {
        if (value is INavMenuNode node)
        {
            Remove(node);
        }
    }

    public void RemoveAt(int index)
    {
        if (!TryLocateNode(_entries, index, out var owner, out var entryIndex, out _))
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        owner.RemoveAt(entryIndex);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static INavMenuNode GetNode(object? value)
    {
        return value as INavMenuNode ??
               throw new ArgumentException($"Expected {nameof(INavMenuNode)}.", nameof(value));
    }

    private void HandleRootCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildGroupSubscriptions();
        RaiseViewPropertiesChanged();

        if (CanForwardRootChange(e))
        {
            CollectionChanged?.Invoke(this, e);
        }
        else
        {
            RaiseReset();
        }
    }

    private void HandleGroupCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildGroupSubscriptions();
        RaiseViewPropertiesChanged();
        RaiseReset();
    }

    private bool CanForwardRootChange(NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset ||
            _entries.Any(entry => entry is not INavMenuNode))
        {
            return false;
        }

        return ContainsOnlyNodes(e.NewItems) && ContainsOnlyNodes(e.OldItems);
    }

    private static bool ContainsOnlyNodes(IList? items)
    {
        if (items is null)
        {
            return true;
        }

        foreach (var item in items)
        {
            if (item is not INavMenuNode)
            {
                return false;
            }
        }

        return true;
    }

    private void RebuildGroupSubscriptions()
    {
        foreach (var notifier in _groupNotifiers)
        {
            notifier.CollectionChanged -= HandleGroupCollectionChanged;
        }
        _groupNotifiers.Clear();

        SubscribeGroups(_entries);
    }

    private void SubscribeGroups(IEnumerable<INavMenuEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry is not NavMenuGroup group)
            {
                continue;
            }

            if (group.Entries is INotifyCollectionChanged notifier &&
                _groupNotifiers.Add(notifier))
            {
                notifier.CollectionChanged += HandleGroupCollectionChanged;
            }

            SubscribeGroups(group.Entries);
        }
    }

    private void RaiseViewPropertiesChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }

    private void RaiseReset()
    {
        CollectionChanged?.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private static IEnumerable<INavMenuNode> EnumerateNodes(IEnumerable<INavMenuEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry is INavMenuNode node)
            {
                yield return node;
            }
            else if (entry is NavMenuGroup group)
            {
                foreach (var groupedNode in EnumerateNodes(group.Entries))
                {
                    yield return groupedNode;
                }
            }
        }
    }

    private static void CountNodes(IEnumerable<INavMenuEntry> entries, ref int count)
    {
        foreach (var entry in entries)
        {
            if (entry is INavMenuNode)
            {
                count++;
            }
            else if (entry is NavMenuGroup group)
            {
                CountNodes(group.Entries, ref count);
            }
        }
    }

    private static bool TryLocateNode(IList<INavMenuEntry> entries,
                                      int targetIndex,
                                      out IList<INavMenuEntry> owner,
                                      out int entryIndex,
                                      out INavMenuNode node)
    {
        var currentIndex = 0;
        return TryLocateNode(entries, targetIndex, ref currentIndex, out owner, out entryIndex, out node);
    }

    private static bool TryLocateNode(IList<INavMenuEntry> entries,
                                      int targetIndex,
                                      ref int currentIndex,
                                      out IList<INavMenuEntry> owner,
                                      out int entryIndex,
                                      out INavMenuNode node)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            switch (entries[i])
            {
                case INavMenuNode currentNode:
                    if (currentIndex == targetIndex)
                    {
                        owner      = entries;
                        entryIndex = i;
                        node       = currentNode;
                        return true;
                    }

                    currentIndex++;
                    break;

                case NavMenuGroup group:
                    if (TryLocateNode(group.Entries,
                            targetIndex,
                            ref currentIndex,
                            out owner,
                            out entryIndex,
                            out node))
                    {
                        return true;
                    }
                    break;
            }
        }

        owner      = null!;
        entryIndex = -1;
        node       = null!;
        return false;
    }

    private static bool TryLocateNode(IList<INavMenuEntry> entries,
                                      INavMenuNode target,
                                      out IList<INavMenuEntry> owner,
                                      out int entryIndex)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            switch (entries[i])
            {
                case INavMenuNode node when ReferenceEquals(node, target):
                    owner      = entries;
                    entryIndex = i;
                    return true;

                case NavMenuGroup group when TryLocateNode(group.Entries, target, out owner, out entryIndex):
                    return true;
            }
        }

        owner      = null!;
        entryIndex = -1;
        return false;
    }
}
