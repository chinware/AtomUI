using System.Collections.Immutable;

namespace AtomUI.Desktop.Controls;

public readonly struct DataGridRange : IEquatable<DataGridRange>
{
    public DataGridRange(int startIndex, int count)
    {
        if (startIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        _ = checked(startIndex + count);
        StartIndex = startIndex;
        Count = count;
    }

    public int StartIndex { get; }

    public int Count { get; }

    public int EndExclusive => checked(StartIndex + Count);

    public bool Equals(DataGridRange other) =>
        StartIndex == other.StartIndex && Count == other.Count;

    public override bool Equals(object? obj) => obj is DataGridRange other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(StartIndex, Count);

    public static bool operator ==(DataGridRange left, DataGridRange right) => left.Equals(right);

    public static bool operator !=(DataGridRange left, DataGridRange right) => !left.Equals(right);
}

public readonly struct DataGridPageRequest : IEquatable<DataGridPageRequest>
{
    public DataGridPageRequest(long dataStartIndex, int dataCount)
    {
        if (dataStartIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dataStartIndex));
        }
        if (dataCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dataCount));
        }
        _ = checked(dataStartIndex + dataCount);
        DataStartIndex = dataStartIndex;
        DataCount = dataCount;
    }

    public long DataStartIndex { get; }

    public int DataCount { get; }

    public bool Equals(DataGridPageRequest other) =>
        DataStartIndex == other.DataStartIndex && DataCount == other.DataCount;

    public override bool Equals(object? obj) => obj is DataGridPageRequest other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(DataStartIndex, DataCount);

    public static bool operator ==(DataGridPageRequest left, DataGridPageRequest right) => left.Equals(right);

    public static bool operator !=(DataGridPageRequest left, DataGridPageRequest right) => !left.Equals(right);
}

public sealed class DataGridGroupExpansion : IEquatable<DataGridGroupExpansion>
{
    private readonly int _structuralHashCode;

    public static DataGridGroupExpansion AllExpanded { get; } = new(default);

    public DataGridGroupExpansion(ImmutableArray<DataGridGroupKey> collapsedGroups)
    {
        if (collapsedGroups.IsDefaultOrEmpty)
        {
            CollapsedGroups = ImmutableArray<DataGridGroupKey>.Empty;
            _structuralHashCode = 0;
            return;
        }

        var sorted = collapsedGroups.ToArray();
        foreach (var key in sorted)
        {
            if (!key.IsValid)
            {
                throw new ArgumentException(
                    "Collapsed group identities must be valid.", nameof(collapsedGroups));
            }
        }
        Array.Sort(sorted, static (left, right) =>
            StringComparer.Ordinal.Compare(left.Value, right.Value));

        var builder = ImmutableArray.CreateBuilder<DataGridGroupKey>(sorted.Length);
        DataGridGroupKey? previous = null;
        foreach (var key in sorted)
        {
            if (previous is null || previous.Value != key)
            {
                builder.Add(key);
                previous = key;
            }
        }
        CollapsedGroups = builder.ToImmutable();
        _structuralHashCode = ComputeStructuralHashCode(CollapsedGroups);
    }

    public ImmutableArray<DataGridGroupKey> CollapsedGroups { get; }

    public DataGridGroupExpansion Collapse(DataGridGroupKey key)
    {
        ValidateKey(key);
        var index = Find(key);
        if (index >= 0)
        {
            return this;
        }
        var insertionIndex = ~index;
        var builder = ImmutableArray.CreateBuilder<DataGridGroupKey>(CollapsedGroups.Length + 1);
        for (var current = 0; current < insertionIndex; current++)
        {
            builder.Add(CollapsedGroups[current]);
        }
        builder.Add(key);
        for (var current = insertionIndex; current < CollapsedGroups.Length; current++)
        {
            builder.Add(CollapsedGroups[current]);
        }
        return new DataGridGroupExpansion(builder.MoveToImmutable());
    }

    public DataGridGroupExpansion Expand(DataGridGroupKey key)
    {
        ValidateKey(key);
        var index = Find(key);
        if (index < 0)
        {
            return this;
        }
        var builder = ImmutableArray.CreateBuilder<DataGridGroupKey>(CollapsedGroups.Length - 1);
        for (var current = 0; current < CollapsedGroups.Length; current++)
        {
            if (current != index)
            {
                builder.Add(CollapsedGroups[current]);
            }
        }
        return builder.Count == 0
            ? AllExpanded
            : new DataGridGroupExpansion(builder.MoveToImmutable());
    }

    public bool Equals(DataGridGroupExpansion? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (other is null ||
            _structuralHashCode != other._structuralHashCode ||
            CollapsedGroups.Length != other.CollapsedGroups.Length)
        {
            return false;
        }
        for (var index = 0; index < CollapsedGroups.Length; index++)
        {
            if (CollapsedGroups[index] != other.CollapsedGroups[index])
            {
                return false;
            }
        }
        return true;
    }

    public override bool Equals(object? obj) => obj is DataGridGroupExpansion other && Equals(other);

    public override int GetHashCode() => _structuralHashCode;

    public static bool operator ==(DataGridGroupExpansion? left, DataGridGroupExpansion? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(DataGridGroupExpansion? left, DataGridGroupExpansion? right) => !(left == right);

    private int Find(DataGridGroupKey key)
    {
        var low = 0;
        var high = CollapsedGroups.Length - 1;
        while (low <= high)
        {
            var middle = low + ((high - low) >> 1);
            var comparison = StringComparer.Ordinal.Compare(CollapsedGroups[middle].Value, key.Value);
            if (comparison == 0)
            {
                return middle;
            }
            if (comparison < 0)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }
        return ~low;
    }

    private static void ValidateKey(DataGridGroupKey key)
    {
        if (!key.IsValid)
        {
            throw new ArgumentException("The group identity must be valid.", nameof(key));
        }
    }

    private static int ComputeStructuralHashCode(ImmutableArray<DataGridGroupKey> keys)
    {
        var hash = new HashCode();
        foreach (var key in keys)
        {
            hash.Add(key);
        }
        return hash.ToHashCode();
    }
}

public enum DataGridSourceEntryKind
{
    Data,
    GroupHeader
}

public sealed class DataGridGroupEntry
{
    public DataGridGroupEntry(
        DataGridGroupKey key,
        DataGridFieldId field,
        DataGridScalar value,
        int level,
        long leafCount)
    {
        if (!key.IsValid)
        {
            throw new ArgumentException("The group identity must be valid.", nameof(key));
        }
        if (!field.IsValid)
        {
            throw new ArgumentException("The field identity must be valid.", nameof(field));
        }
        if (level < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(level));
        }
        if (leafCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(leafCount));
        }

        Key = key;
        Field = field;
        Value = value;
        Level = level;
        LeafCount = leafCount;
    }

    public DataGridGroupKey Key { get; }

    public DataGridFieldId Field { get; }

    public DataGridScalar Value { get; }

    public int Level { get; }

    public long LeafCount { get; }
}

public readonly struct DataGridSourceEntry
{
    private DataGridSourceEntry(
        DataGridSourceEntryKind kind,
        DataGridRowKey rowKey,
        object? item,
        DataGridGroupEntry? group,
        int windowDataIndex,
        long dataIndex)
    {
        Kind = kind;
        RowKey = rowKey;
        Item = item;
        Group = group;
        WindowDataIndex = windowDataIndex;
        DataIndex = dataIndex;
    }

    public DataGridSourceEntryKind Kind { get; }

    public DataGridRowKey RowKey { get; }

    public object? Item { get; }

    public DataGridGroupEntry? Group { get; }

    public int WindowDataIndex { get; }

    public long DataIndex { get; }

    public static DataGridSourceEntry CreateData(
        DataGridRowKey rowKey,
        object? item,
        int windowDataIndex,
        long dataIndex)
    {
        if (!rowKey.IsValid)
        {
            throw new ArgumentException("The row key must be valid.", nameof(rowKey));
        }
        if (windowDataIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(windowDataIndex));
        }
        if (dataIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dataIndex));
        }
        return new DataGridSourceEntry(
            DataGridSourceEntryKind.Data,
            rowKey,
            item,
            null,
            windowDataIndex,
            dataIndex);
    }

    public static DataGridSourceEntry CreateGroupHeader(DataGridGroupEntry group)
    {
        ArgumentNullException.ThrowIfNull(group);
        return new DataGridSourceEntry(
            DataGridSourceEntryKind.GroupHeader,
            default,
            null,
            group,
            -1,
            -1);
    }
}

public readonly struct DataGridFetchRequest
{
    public DataGridFetchRequest(
        DataGridQuery query,
        DataGridPageRequest? pageRequest,
        DataGridGroupExpansion groupExpansion,
        DataGridRange range,
        DataGridSnapshotId? expectedSnapshot,
        long queryRevision,
        long dataGeneration)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(groupExpansion);
        if (range.Count <= 0)
        {
            throw new ArgumentException("The range must be initialized and non-empty.", nameof(range));
        }
        if (expectedSnapshot is { IsValid: false })
        {
            throw new ArgumentException("The expected snapshot identity must be valid.", nameof(expectedSnapshot));
        }
        if (queryRevision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(queryRevision));
        }
        if (dataGeneration < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dataGeneration));
        }
        if (query.Groups.IsEmpty && !groupExpansion.CollapsedGroups.IsEmpty)
        {
            throw new ArgumentException(
                "Collapsed groups require at least one query group.", nameof(groupExpansion));
        }

        Query = query;
        PageRequest = pageRequest;
        GroupExpansion = groupExpansion;
        Range = range;
        ExpectedSnapshot = expectedSnapshot;
        QueryRevision = queryRevision;
        DataGeneration = dataGeneration;
    }

    public DataGridQuery Query { get; }

    public DataGridPageRequest? PageRequest { get; }

    public DataGridGroupExpansion GroupExpansion { get; }

    public DataGridRange Range { get; }

    public DataGridSnapshotId? ExpectedSnapshot { get; }

    public long QueryRevision { get; }

    public long DataGeneration { get; }
}

public sealed class DataGridRangeResult
{
    public DataGridRangeResult(
        int startIndex,
        ImmutableArray<DataGridSourceEntry> entries,
        int totalEntryCount,
        int windowDataCount,
        long totalDataCount,
        DataGridSnapshotId snapshot)
    {
        if (startIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }
        if (totalEntryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalEntryCount));
        }
        if (windowDataCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(windowDataCount));
        }
        if (totalDataCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalDataCount));
        }
        if (!snapshot.IsValid)
        {
            throw new ArgumentException("The snapshot identity must be valid.", nameof(snapshot));
        }

        StartIndex = startIndex;
        Entries = entries.IsDefault ? ImmutableArray<DataGridSourceEntry>.Empty : entries;
        TotalEntryCount = totalEntryCount;
        WindowDataCount = windowDataCount;
        TotalDataCount = totalDataCount;
        Snapshot = snapshot;
    }

    public int StartIndex { get; }

    public ImmutableArray<DataGridSourceEntry> Entries { get; }

    public int TotalEntryCount { get; }

    public int WindowDataCount { get; }

    public long TotalDataCount { get; }

    public DataGridSnapshotId Snapshot { get; }
}
