using System.Runtime.CompilerServices;

namespace AtomUI.Desktop.Controls;

internal readonly struct DataGridRangeBlockIdentity : IEquatable<DataGridRangeBlockIdentity>
{
    public DataGridRangeBlockIdentity(
        object sourceIdentity,
        long queryRevision,
        long dataGeneration,
        DataGridSnapshotId snapshot,
        int blockStartIndex)
    {
        ArgumentNullException.ThrowIfNull(sourceIdentity);
        if (queryRevision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(queryRevision));
        }
        if (dataGeneration < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dataGeneration));
        }
        if (!snapshot.IsValid)
        {
            throw new ArgumentException("The snapshot identity must be valid.", nameof(snapshot));
        }
        if (blockStartIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockStartIndex));
        }

        SourceIdentity = sourceIdentity;
        QueryRevision = queryRevision;
        DataGeneration = dataGeneration;
        Snapshot = snapshot;
        BlockStartIndex = blockStartIndex;
    }

    public object SourceIdentity { get; }

    public long QueryRevision { get; }

    public long DataGeneration { get; }

    public DataGridSnapshotId Snapshot { get; }

    public int BlockStartIndex { get; }

    public bool Equals(DataGridRangeBlockIdentity other) =>
        ReferenceEquals(SourceIdentity, other.SourceIdentity) &&
        QueryRevision == other.QueryRevision &&
        DataGeneration == other.DataGeneration &&
        Snapshot == other.Snapshot &&
        BlockStartIndex == other.BlockStartIndex;

    public override bool Equals(object? obj) =>
        obj is DataGridRangeBlockIdentity other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(
        RuntimeHelpers.GetHashCode(SourceIdentity),
        QueryRevision,
        DataGeneration,
        Snapshot,
        BlockStartIndex);

    public static bool operator ==(
        DataGridRangeBlockIdentity left,
        DataGridRangeBlockIdentity right) => left.Equals(right);

    public static bool operator !=(
        DataGridRangeBlockIdentity left,
        DataGridRangeBlockIdentity right) => !left.Equals(right);

    public bool IsSameGeneration(DataGridRangeBlockIdentity other) =>
        ReferenceEquals(SourceIdentity, other.SourceIdentity) &&
        QueryRevision == other.QueryRevision &&
        DataGeneration == other.DataGeneration;

    public bool IsSameSnapshot(DataGridRangeBlockIdentity other) =>
        IsSameGeneration(other) && Snapshot == other.Snapshot;
}

internal enum DataGridRangeBlockPinReason
{
    Applied,
    Realized,
    Edit,
    Drag
}

internal sealed class DataGridRangeBlockCache
{
    private readonly object _gate = new();
    private readonly int _capacity;
    private readonly Dictionary<DataGridRangeBlockIdentity, CacheEntry> _entries;
    private readonly LinkedList<DataGridRangeBlockIdentity> _passiveLru = new();
    private long _epoch;
    private int _totalPinCount;

    public DataGridRangeBlockCache(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }
        _capacity = capacity;
        _entries = new Dictionary<DataGridRangeBlockIdentity, CacheEntry>(capacity);
    }

    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _entries.Count;
            }
        }
    }

    public int TotalPinCount
    {
        get
        {
            lock (_gate)
            {
                return _totalPinCount;
            }
        }
    }

    public bool TryAdd(object sourceIdentity, DataGridValidatedRangeBlock block)
    {
        ArgumentNullException.ThrowIfNull(sourceIdentity);
        ArgumentNullException.ThrowIfNull(block);
        var identity = block.CreateCacheIdentity(sourceIdentity);

        lock (_gate)
        {
            if (_entries.ContainsKey(identity))
            {
                return false;
            }
            foreach (var pair in _entries)
            {
                if (identity.IsSameGeneration(pair.Key) && identity.Snapshot != pair.Key.Snapshot)
                {
                    return false;
                }
                if (identity.IsSameSnapshot(pair.Key))
                {
                    ValidateCompatibleBlocks(block, pair.Value.Block);
                }
            }

            if (_entries.Count == _capacity && !EvictPassiveCore())
            {
                return false;
            }

            var node = _passiveLru.AddFirst(identity);
            _entries.Add(identity, new CacheEntry(block, node));
            return true;
        }
    }

    public bool TryGetBlock(
        DataGridRangeBlockIdentity identity,
        out DataGridValidatedRangeBlock? block)
    {
        lock (_gate)
        {
            if (!_entries.TryGetValue(identity, out var entry))
            {
                block = null;
                return false;
            }
            if (entry.TotalPinCount == 0)
            {
                _passiveLru.Remove(entry.PassiveNode);
                _passiveLru.AddFirst(entry.PassiveNode);
            }
            block = entry.Block;
            return true;
        }
    }

    public bool Contains(DataGridRangeBlockIdentity identity)
    {
        lock (_gate)
        {
            return _entries.ContainsKey(identity);
        }
    }

    public IDisposable AcquirePin(
        DataGridRangeBlockIdentity identity,
        DataGridRangeBlockPinReason reason)
    {
        ValidateReason(reason);
        lock (_gate)
        {
            if (!_entries.TryGetValue(identity, out var entry))
            {
                throw new KeyNotFoundException(
                    $"Range block starting at {identity.BlockStartIndex} is not cached.");
            }
            if (entry.TotalPinCount == 0)
            {
                _passiveLru.Remove(entry.PassiveNode);
            }
            entry.Increment(reason);
            _totalPinCount = checked(_totalPinCount + 1);
            return new PinToken(this, _epoch, identity, reason);
        }
    }

    public int GetPinCount(
        DataGridRangeBlockIdentity identity,
        DataGridRangeBlockPinReason reason)
    {
        ValidateReason(reason);
        lock (_gate)
        {
            return _entries.TryGetValue(identity, out var entry)
                ? entry.GetCount(reason)
                : 0;
        }
    }

    public bool EvictPassive()
    {
        lock (_gate)
        {
            return EvictPassiveCore();
        }
    }

    public void ClearPassive()
    {
        lock (_gate)
        {
            while (_passiveLru.Last is { } node)
            {
                _passiveLru.Remove(node);
                _entries.Remove(node.Value);
            }
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _epoch++;
            _totalPinCount = 0;
            _entries.Clear();
            _passiveLru.Clear();
        }
    }

    private bool EvictPassiveCore()
    {
        var node = _passiveLru.Last;
        if (node is null)
        {
            return false;
        }
        _passiveLru.Remove(node);
        return _entries.Remove(node.Value);
    }

    private void ReleasePin(
        long epoch,
        DataGridRangeBlockIdentity identity,
        DataGridRangeBlockPinReason reason)
    {
        lock (_gate)
        {
            if (epoch != _epoch || !_entries.TryGetValue(identity, out var entry))
            {
                return;
            }
            entry.Decrement(reason);
            _totalPinCount--;
            if (entry.TotalPinCount == 0)
            {
                _passiveLru.AddFirst(entry.PassiveNode);
            }
        }
    }

    private static void ValidateCompatibleBlocks(
        DataGridValidatedRangeBlock left,
        DataGridValidatedRangeBlock right)
    {
        if (left.Identity.TotalDataCount != right.Identity.TotalDataCount ||
            left.Identity.WindowDataCount != right.Identity.WindowDataCount ||
            left.Identity.TotalEntryCount != right.Identity.TotalEntryCount)
        {
            throw new DataGridSourceContractException(
                "Cached blocks from one snapshot report different totals.");
        }

        var leftEnd = checked(left.Range.StartIndex + left.Entries.Length);
        var rightEnd = checked(right.Range.StartIndex + right.Entries.Length);
        if (left.Entries.Length > 0 && right.Entries.Length > 0 &&
            left.Range.StartIndex < rightEnd && right.Range.StartIndex < leftEnd)
        {
            throw new DataGridSourceContractException(
                "Cached display-entry ranges overlap within one snapshot.");
        }

        foreach (var leftEntry in left.Entries)
        {
            foreach (var rightEntry in right.Entries)
            {
                if (leftEntry.Kind == DataGridSourceEntryKind.Data &&
                    rightEntry.Kind == DataGridSourceEntryKind.Data &&
                    leftEntry.RowKey == rightEntry.RowKey)
                {
                    throw new DataGridSourceContractException(
                        $"Row key '{leftEntry.RowKey}' occurs in multiple cached blocks.");
                }
                if (leftEntry.Kind == DataGridSourceEntryKind.GroupHeader &&
                    rightEntry.Kind == DataGridSourceEntryKind.GroupHeader &&
                    leftEntry.Group!.Key == rightEntry.Group!.Key)
                {
                    throw new DataGridSourceContractException(
                        $"Group key '{leftEntry.Group.Key}' occurs in multiple cached blocks.");
                }
            }
        }

        if (TryGetDataBounds(left, out var leftFirst, out var leftLast) &&
            TryGetDataBounds(right, out var rightFirst, out var rightLast))
        {
            if (left.Range.StartIndex < right.Range.StartIndex &&
                (leftLast.WindowDataIndex >= rightFirst.WindowDataIndex ||
                 leftLast.DataIndex >= rightFirst.DataIndex))
            {
                throw new DataGridSourceContractException(
                    "Data indices do not increase across cached display ranges.");
            }
            if (right.Range.StartIndex < left.Range.StartIndex &&
                (rightLast.WindowDataIndex >= leftFirst.WindowDataIndex ||
                 rightLast.DataIndex >= leftFirst.DataIndex))
            {
                throw new DataGridSourceContractException(
                    "Data indices do not increase across cached display ranges.");
            }
        }
    }

    private static bool TryGetDataBounds(
        DataGridValidatedRangeBlock block,
        out DataGridSourceEntry first,
        out DataGridSourceEntry last)
    {
        first = default;
        last = default;
        var found = false;
        foreach (var entry in block.Entries)
        {
            if (entry.Kind != DataGridSourceEntryKind.Data)
            {
                continue;
            }
            if (!found)
            {
                first = entry;
                found = true;
            }
            last = entry;
        }
        return found;
    }

    private static void ValidateReason(DataGridRangeBlockPinReason reason)
    {
        if (reason is < DataGridRangeBlockPinReason.Applied or > DataGridRangeBlockPinReason.Drag)
        {
            throw new ArgumentOutOfRangeException(nameof(reason));
        }
    }

    private sealed class CacheEntry
    {
        private int _appliedPins;
        private int _realizedPins;
        private int _editPins;
        private int _dragPins;

        public CacheEntry(
            DataGridValidatedRangeBlock block,
            LinkedListNode<DataGridRangeBlockIdentity> passiveNode)
        {
            Block = block;
            PassiveNode = passiveNode;
        }

        public DataGridValidatedRangeBlock Block { get; }

        public LinkedListNode<DataGridRangeBlockIdentity> PassiveNode { get; }

        public int TotalPinCount => _appliedPins + _realizedPins + _editPins + _dragPins;

        public int GetCount(DataGridRangeBlockPinReason reason) => reason switch
        {
            DataGridRangeBlockPinReason.Applied => _appliedPins,
            DataGridRangeBlockPinReason.Realized => _realizedPins,
            DataGridRangeBlockPinReason.Edit => _editPins,
            DataGridRangeBlockPinReason.Drag => _dragPins,
            _ => 0
        };

        public void Increment(DataGridRangeBlockPinReason reason)
        {
            switch (reason)
            {
                case DataGridRangeBlockPinReason.Applied:
                    _appliedPins = checked(_appliedPins + 1);
                    break;
                case DataGridRangeBlockPinReason.Realized:
                    _realizedPins = checked(_realizedPins + 1);
                    break;
                case DataGridRangeBlockPinReason.Edit:
                    _editPins = checked(_editPins + 1);
                    break;
                case DataGridRangeBlockPinReason.Drag:
                    _dragPins = checked(_dragPins + 1);
                    break;
            }
        }

        public void Decrement(DataGridRangeBlockPinReason reason)
        {
            switch (reason)
            {
                case DataGridRangeBlockPinReason.Applied:
                    Decrement(ref _appliedPins, reason);
                    break;
                case DataGridRangeBlockPinReason.Realized:
                    Decrement(ref _realizedPins, reason);
                    break;
                case DataGridRangeBlockPinReason.Edit:
                    Decrement(ref _editPins, reason);
                    break;
                case DataGridRangeBlockPinReason.Drag:
                    Decrement(ref _dragPins, reason);
                    break;
            }
        }

        private static void Decrement(ref int count, DataGridRangeBlockPinReason reason)
        {
            if (count <= 0)
            {
                throw new InvalidOperationException(
                    $"Pin reason '{reason}' was released more times than it was acquired.");
            }
            count--;
        }
    }

    private sealed class PinToken : IDisposable
    {
        private DataGridRangeBlockCache? _cache;
        private readonly long _epoch;
        private readonly DataGridRangeBlockIdentity _identity;
        private readonly DataGridRangeBlockPinReason _reason;

        public PinToken(
            DataGridRangeBlockCache cache,
            long epoch,
            DataGridRangeBlockIdentity identity,
            DataGridRangeBlockPinReason reason)
        {
            _cache = cache;
            _epoch = epoch;
            _identity = identity;
            _reason = reason;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _cache, null)?.ReleasePin(_epoch, _identity, _reason);
        }
    }
}
