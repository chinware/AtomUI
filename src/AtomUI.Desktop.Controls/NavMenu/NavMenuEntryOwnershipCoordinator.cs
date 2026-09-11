using System.Collections;
using System.Collections.Specialized;
using Avalonia.Collections;

namespace AtomUI.Desktop.Controls;

internal sealed class NavMenuEntryOwnershipCoordinator
{
    private readonly object _owner;
    private readonly Func<IEnumerable> _rootEntriesAccessor;
    private readonly HashSet<INavMenuEntry> _ownedEntries = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<INotifyCollectionChanged, IDisposable> _sourceSubscriptions =
        new(ReferenceEqualityComparer.Instance);

    public NavMenuEntryOwnershipCoordinator(object owner, Func<IEnumerable> rootEntriesAccessor)
    {
        _owner = owner;
        _rootEntriesAccessor = rootEntriesAccessor;
    }

    public void Validate(IEnumerable rootEntries)
    {
        Process(rootEntries, false);
    }

    public void Synchronize()
    {
        Process(_rootEntriesAccessor(), true);
    }

    private void Process(IEnumerable rootEntries, bool apply)
    {
        var expectedOwners = new Dictionary<INavMenuEntry, object>(ReferenceEqualityComparer.Instance);
        var firstOccurrences = new Dictionary<INavMenuEntry, EntryOccurrence>(ReferenceEqualityComparer.Instance);
        var activeEntries = new HashSet<INavMenuEntry>(ReferenceEqualityComparer.Instance);
        var nestedSources = new HashSet<INotifyCollectionChanged>(ReferenceEqualityComparer.Instance);
        var pendingEntries = new Stack<PendingEntry>();
        InvalidOperationException? validationException = null;

        PushEntries(rootEntries, _owner, _owner, false, nestedSources, pendingEntries);
        while (pendingEntries.Count > 0)
        {
            var pendingEntry = pendingEntries.Pop();
            if (pendingEntry.IsExit)
            {
                activeEntries.Remove((INavMenuEntry)pendingEntry.Item!);
                continue;
            }

            INavMenuEntry? entry = null;
            try
            {
                entry = NavMenuEntryValidation.Validate(
                    pendingEntry.Item,
                    pendingEntry.ValidationOwner,
                    pendingEntry.Index);
            }
            catch (InvalidOperationException exception)
            {
                validationException ??= exception;
            }

            if (entry is null)
            {
                continue;
            }

            if (_owner is INavMenuEntry ownerEntry &&
                !ReferenceEquals(pendingEntry.ValidationOwner, _owner) &&
                NavMenuEntryOwnership.IsTracked(entry))
            {
                try
                {
                    NavMenuEntryGraph.ValidateInsertion(ownerEntry, entry);
                }
                catch (InvalidOperationException exception)
                {
                    validationException ??= exception;
                }
            }

            if (!activeEntries.Add(entry))
            {
                validationException ??= new InvalidOperationException(
                    $"{pendingEntry.ValidationOwner.GetType().Name} contains a NavMenu entry cycle involving " +
                    $"{entry.GetType().Name}.");
                continue;
            }

            if (NavMenuEntryOwnership.IsTracked(entry))
            {
                if (!expectedOwners.TryAdd(entry, pendingEntry.StructuralOwner))
                {
                    var firstOccurrence = firstOccurrences[entry];
                    validationException ??= ReferenceEquals(
                        firstOccurrence.Owner,
                        pendingEntry.ValidationOwner)
                        ? NavMenuEntryOwnership.CreateDuplicateException(
                            entry,
                            pendingEntry.ValidationOwner,
                            firstOccurrence.Index,
                            pendingEntry.Index)
                        : NavMenuEntryOwnership.CreateTreeDuplicateException(
                            entry,
                            _owner,
                            firstOccurrence.Owner,
                            pendingEntry.ValidationOwner);
                }
                else
                {
                    firstOccurrences.Add(
                        entry,
                        new EntryOccurrence(pendingEntry.ValidationOwner, pendingEntry.Index));
                }
            }

            pendingEntries.Push(PendingEntry.Exit(entry));
            switch (entry)
            {
                case NavMenuNode:
                case NavMenuGroup:
                    break;

                case INavMenuNode customNode:
                    PushEntries(
                        customNode.Entries,
                        pendingEntry.StructuralOwner,
                        customNode,
                        true,
                        nestedSources,
                        pendingEntries);
                    break;
            }
        }

        var currentOwnedEntries = new HashSet<INavMenuEntry>(ReferenceEqualityComparer.Instance);
        foreach (var (entry, expectedOwner) in expectedOwners)
        {
            if (ReferenceEquals(expectedOwner, _owner))
            {
                currentOwnedEntries.Add(entry);
            }

            if (NavMenuEntryOwnership.TryGetOwner(entry, out var currentOwner) &&
                !ReferenceEquals(currentOwner, expectedOwner))
            {
                validationException ??= NavMenuEntryOwnership.CreateAlreadyAttachedException(
                    entry,
                    expectedOwner,
                    currentOwner!);
            }
        }

        if (!apply)
        {
            if (validationException is not null)
            {
                throw validationException;
            }

            return;
        }

        foreach (var entry in _ownedEntries)
        {
            if (!currentOwnedEntries.Contains(entry))
            {
                NavMenuEntryOwnership.Detach(entry, _owner);
            }
        }

        _ownedEntries.IntersectWith(currentOwnedEntries);
        ReconcileSubscriptions(nestedSources);

        if (validationException is not null)
        {
            throw validationException;
        }

        foreach (var (entry, expectedOwner) in expectedOwners)
        {
            if (!NavMenuEntryOwnership.TryGetOwner(entry, out _))
            {
                NavMenuEntryOwnership.Attach(entry, expectedOwner);
            }
        }

        _ownedEntries.Clear();
        _ownedEntries.UnionWith(currentOwnedEntries);
    }

    private void ReconcileSubscriptions(HashSet<INotifyCollectionChanged> currentSources)
    {
        foreach (var source in _sourceSubscriptions.Keys.ToArray())
        {
            if (!currentSources.Contains(source))
            {
                _sourceSubscriptions.Remove(source, out var subscription);
                subscription?.Dispose();
            }
        }

        foreach (var source in currentSources)
        {
            if (!_sourceSubscriptions.ContainsKey(source))
            {
                _sourceSubscriptions.Add(source, source.WeakSubscribe(HandleNestedSourceChanged));
            }
        }
    }

    private void HandleNestedSourceChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Synchronize();
    }

    private static void PushEntries(IEnumerable entries,
                                    object structuralOwner,
                                    object validationOwner,
                                    bool trackSource,
                                    HashSet<INotifyCollectionChanged> nestedSources,
                                    Stack<PendingEntry> pendingEntries)
    {
        if (trackSource && entries is INotifyCollectionChanged notifyCollectionChanged)
        {
            nestedSources.Add(notifyCollectionChanged);
        }

        var bufferedEntries = entries.Cast<object?>().ToArray();
        for (var index = bufferedEntries.Length - 1; index >= 0; index--)
        {
            pendingEntries.Push(new PendingEntry(
                bufferedEntries[index],
                structuralOwner,
                validationOwner,
                index,
                false));
        }
    }

    private readonly record struct PendingEntry(object? Item,
                                                object StructuralOwner,
                                                object ValidationOwner,
                                                int Index,
                                                bool IsExit)
    {
        public static PendingEntry Exit(INavMenuEntry entry)
        {
            return new PendingEntry(entry, entry, entry, -1, true);
        }
    }

    private readonly record struct EntryOccurrence(object Owner, int Index);
}
