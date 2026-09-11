using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace AtomUI.Desktop.Controls;

public sealed class DataGridSelectionScope : IEquatable<DataGridSelectionScope>
{
    public DataGridSelectionScope(
        IDataGridSource source,
        DataGridQuery query,
        DataGridSnapshotId snapshot)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(query);
        if (!snapshot.IsValid)
        {
            throw new ArgumentException("The snapshot identity must be valid.", nameof(snapshot));
        }

        Source = source;
        Query = query;
        Snapshot = snapshot;
    }

    public IDataGridSource Source { get; }

    public DataGridQuery Query { get; }

    public DataGridSnapshotId Snapshot { get; }

    public bool Equals(DataGridSelectionScope? other) =>
        ReferenceEquals(this, other) ||
        other is not null &&
        ReferenceEquals(Source, other.Source) &&
        Query == other.Query &&
        Snapshot == other.Snapshot;

    public override bool Equals(object? obj) =>
        obj is DataGridSelectionScope other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(
        RuntimeHelpers.GetHashCode(Source),
        Query,
        Snapshot);
}

public readonly struct DataGridSelectionInterval : IEquatable<DataGridSelectionInterval>
{
    public DataGridSelectionInterval(
        long startIndex,
        long endIndexExclusive,
        DataGridSelectionScope scope)
    {
        if (startIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }
        if (endIndexExclusive <= startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(endIndexExclusive));
        }
        ArgumentNullException.ThrowIfNull(scope);

        StartIndex = startIndex;
        EndIndexExclusive = endIndexExclusive;
        Scope = scope;
    }

    public long StartIndex { get; }

    public long EndIndexExclusive { get; }

    public DataGridSelectionScope Scope { get; }

    public bool Contains(long dataIndex) =>
        dataIndex >= StartIndex && dataIndex < EndIndexExclusive;

    public bool Equals(DataGridSelectionInterval other) =>
        StartIndex == other.StartIndex &&
        EndIndexExclusive == other.EndIndexExclusive &&
        Equals(Scope, other.Scope);

    public override bool Equals(object? obj) =>
        obj is DataGridSelectionInterval other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(
        StartIndex,
        EndIndexExclusive,
        Scope);

    public static bool operator ==(
        DataGridSelectionInterval left,
        DataGridSelectionInterval right) => left.Equals(right);

    public static bool operator !=(
        DataGridSelectionInterval left,
        DataGridSelectionInterval right) => !left.Equals(right);
}

internal enum DataGridSelectionTransition
{
    Preserve,
    SortOrGroup,
    Filter,
    Invalidation
}

public sealed class DataGridSelectionState : IEquatable<DataGridSelectionState>
{
    public DataGridSelectionState(
        ImmutableArray<DataGridRowKey> explicitKeys,
        DataGridSelectionScope? allMatchingQuery,
        ImmutableArray<DataGridSelectionInterval> indexIntervals,
        ImmutableArray<DataGridRowKey> excludedKeys)
    {
        ExplicitKeys = NormalizeKeys(explicitKeys, nameof(explicitKeys));
        IndexIntervals = NormalizeIntervals(indexIntervals, allMatchingQuery);
        ExcludedKeys = NormalizeKeys(excludedKeys, nameof(excludedKeys));
        AllMatchingQuery = allMatchingQuery;
    }

    private DataGridSelectionState()
    {
        ExplicitKeys = ImmutableArray<DataGridRowKey>.Empty;
        IndexIntervals = ImmutableArray<DataGridSelectionInterval>.Empty;
        ExcludedKeys = ImmutableArray<DataGridRowKey>.Empty;
    }

    public static DataGridSelectionState Empty { get; } = new();

    public ImmutableArray<DataGridRowKey> ExplicitKeys { get; }

    public DataGridSelectionScope? AllMatchingQuery { get; }

    public ImmutableArray<DataGridSelectionInterval> IndexIntervals { get; }

    public ImmutableArray<DataGridRowKey> ExcludedKeys { get; }

    public bool IsEmpty =>
        ExplicitKeys.IsEmpty &&
        AllMatchingQuery is null &&
        IndexIntervals.IsEmpty;

    public bool Contains(
        DataGridRowKey key,
        long dataIndex,
        DataGridSelectionScope scope)
    {
        ValidateKey(key, nameof(key));
        ArgumentNullException.ThrowIfNull(scope);
        if (ContainsKey(ExcludedKeys, key))
        {
            return false;
        }
        if (ContainsKey(ExplicitKeys, key))
        {
            return true;
        }
        if (AllMatchingQuery?.Equals(scope) == true)
        {
            return true;
        }
        foreach (var interval in IndexIntervals)
        {
            if (interval.Scope.Equals(scope) && interval.Contains(dataIndex))
            {
                return true;
            }
        }
        return false;
    }

    internal DataGridSelectionState WithKey(
        DataGridRowKey key,
        long dataIndex,
        DataGridSelectionScope scope,
        bool isSelected,
        bool single)
    {
        ValidateKey(key, nameof(key));
        if (single)
        {
            if (isSelected)
            {
                return new DataGridSelectionState([key], null, [], []);
            }

            return ContainsKey(ExplicitKeys, key)
                ? Empty
                : this;
        }

        if (isSelected)
        {
            var alreadyCovered = AllMatchingQuery?.Equals(scope) == true ||
                                 IsInInterval(dataIndex, scope);
            var explicitKeys = alreadyCovered || ContainsKey(ExplicitKeys, key)
                ? ExplicitKeys
                : ExplicitKeys.Add(key);
            return new DataGridSelectionState(
                explicitKeys,
                AllMatchingQuery,
                IndexIntervals,
                RemoveKey(ExcludedKeys, key));
        }

        var withoutExplicit = RemoveKey(ExplicitKeys, key);
        var covered = AllMatchingQuery?.Equals(scope) == true ||
                      IsInInterval(dataIndex, scope);
        var exclusions = covered && !ContainsKey(ExcludedKeys, key)
            ? ExcludedKeys.Add(key)
            : ExcludedKeys;
        return new DataGridSelectionState(
            withoutExplicit,
            AllMatchingQuery,
            IndexIntervals,
            exclusions);
    }

    internal DataGridSelectionState WithAllMatching(DataGridSelectionScope scope) =>
        new(
            ExplicitKeys,
            scope,
            ImmutableArray<DataGridSelectionInterval>.Empty,
            ImmutableArray<DataGridRowKey>.Empty);

    internal DataGridSelectionState WithInterval(
        long startIndex,
        long endIndexExclusive,
        DataGridSelectionScope scope) =>
        new(
            ExplicitKeys,
            AllMatchingQuery,
            IndexIntervals.Add(new DataGridSelectionInterval(
                startIndex,
                endIndexExclusive,
                scope)),
            ExcludedKeys);

    internal DataGridSelectionState NormalizeForMode(DataGridSelectionMode mode)
    {
        if (mode == DataGridSelectionMode.None)
        {
            return Empty;
        }
        if (mode == DataGridSelectionMode.Extended || IsEmpty)
        {
            return this;
        }
        return ExplicitKeys.IsEmpty
            ? Empty
            : new DataGridSelectionState([ExplicitKeys[0]], null, [], []);
    }

    internal DataGridSelectionState TransitionTo(
        DataGridSelectionScope nextScope,
        DataGridSelectionTransition transition)
    {
        ArgumentNullException.ThrowIfNull(nextScope);
        if (transition == DataGridSelectionTransition.Filter)
        {
            return ExplicitKeys.IsEmpty
                ? Empty
                : new DataGridSelectionState(ExplicitKeys, null, [], []);
        }

        var allMatching = AllMatchingQuery is null ? null : nextScope;
        var intervals = transition is DataGridSelectionTransition.SortOrGroup or
            DataGridSelectionTransition.Invalidation
            ? ImmutableArray<DataGridSelectionInterval>.Empty
            : RebindIntervals(IndexIntervals, nextScope);
        var exclusions = intervals.IsEmpty && allMatching is null
            ? ImmutableArray<DataGridRowKey>.Empty
            : ExcludedKeys;
        if (ExplicitKeys.IsEmpty && allMatching is null && intervals.IsEmpty)
        {
            return Empty;
        }
        return new DataGridSelectionState(
            ExplicitKeys,
            allMatching,
            intervals,
            exclusions);
    }

    public bool Equals(DataGridSelectionState? other) =>
        ReferenceEquals(this, other) ||
        other is not null &&
        Equals(AllMatchingQuery, other.AllMatchingQuery) &&
        SequenceEqual(ExplicitKeys, other.ExplicitKeys) &&
        SequenceEqual(IndexIntervals, other.IndexIntervals) &&
        SequenceEqual(ExcludedKeys, other.ExcludedKeys);

    public override bool Equals(object? obj) =>
        obj is DataGridSelectionState other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(AllMatchingQuery);
        foreach (var key in ExplicitKeys)
        {
            hash.Add(key);
        }
        foreach (var interval in IndexIntervals)
        {
            hash.Add(interval);
        }
        foreach (var key in ExcludedKeys)
        {
            hash.Add(key);
        }
        return hash.ToHashCode();
    }

    private bool IsInInterval(long dataIndex, DataGridSelectionScope scope)
    {
        foreach (var interval in IndexIntervals)
        {
            if (interval.Scope.Equals(scope) && interval.Contains(dataIndex))
            {
                return true;
            }
        }
        return false;
    }

    private static ImmutableArray<DataGridRowKey> NormalizeKeys(
        ImmutableArray<DataGridRowKey> values,
        string parameterName)
    {
        if (values.IsDefaultOrEmpty)
        {
            return ImmutableArray<DataGridRowKey>.Empty;
        }
        var copy = values.ToArray();
        foreach (var value in copy)
        {
            ValidateKey(value, parameterName);
        }
        Array.Sort(copy, DataGridRowKeyComparer.Instance);
        var builder = ImmutableArray.CreateBuilder<DataGridRowKey>(copy.Length);
        foreach (var value in copy)
        {
            if (builder.Count == 0 || builder[^1] != value)
            {
                builder.Add(value);
            }
        }
        return builder.ToImmutable();
    }

    private static ImmutableArray<DataGridSelectionInterval> NormalizeIntervals(
        ImmutableArray<DataGridSelectionInterval> intervals,
        DataGridSelectionScope? allMatching)
    {
        if (intervals.IsDefaultOrEmpty)
        {
            return ImmutableArray<DataGridSelectionInterval>.Empty;
        }
        var copy = intervals.ToArray();
        var scope = copy[0].Scope ??
                    throw new ArgumentException("Selection interval scope cannot be null.", nameof(intervals));
        if (allMatching is not null && !allMatching.Equals(scope))
        {
            throw new ArgumentException(
                "All selection expressions must share one scope.", nameof(intervals));
        }
        foreach (var interval in copy)
        {
            if (interval.Scope is null || !scope.Equals(interval.Scope))
            {
                throw new ArgumentException(
                    "All selection intervals must share one scope.", nameof(intervals));
            }
        }
        Array.Sort(copy, static (left, right) =>
        {
            var start = left.StartIndex.CompareTo(right.StartIndex);
            return start != 0 ? start : left.EndIndexExclusive.CompareTo(right.EndIndexExclusive);
        });

        var builder = ImmutableArray.CreateBuilder<DataGridSelectionInterval>(copy.Length);
        var current = copy[0];
        for (var index = 1; index < copy.Length; index++)
        {
            var next = copy[index];
            if (next.StartIndex <= current.EndIndexExclusive)
            {
                current = new DataGridSelectionInterval(
                    current.StartIndex,
                    Math.Max(current.EndIndexExclusive, next.EndIndexExclusive),
                    scope);
            }
            else
            {
                builder.Add(current);
                current = next;
            }
        }
        builder.Add(current);
        return builder.ToImmutable();
    }

    private static ImmutableArray<DataGridSelectionInterval> RebindIntervals(
        ImmutableArray<DataGridSelectionInterval> intervals,
        DataGridSelectionScope scope)
    {
        if (intervals.IsEmpty)
        {
            return intervals;
        }
        var builder = ImmutableArray.CreateBuilder<DataGridSelectionInterval>(intervals.Length);
        foreach (var interval in intervals)
        {
            builder.Add(new DataGridSelectionInterval(
                interval.StartIndex,
                interval.EndIndexExclusive,
                scope));
        }
        return builder.MoveToImmutable();
    }

    private static bool ContainsKey(
        ImmutableArray<DataGridRowKey> keys,
        DataGridRowKey key) => keys.BinarySearch(key, DataGridRowKeyComparer.Instance) >= 0;

    private static ImmutableArray<DataGridRowKey> RemoveKey(
        ImmutableArray<DataGridRowKey> keys,
        DataGridRowKey key)
    {
        var index = keys.BinarySearch(key, DataGridRowKeyComparer.Instance);
        return index < 0 ? keys : keys.RemoveAt(index);
    }

    private static void ValidateKey(DataGridRowKey key, string parameterName)
    {
        if (!key.IsValid)
        {
            throw new ArgumentException("Selection keys must be valid.", parameterName);
        }
    }

    private static bool SequenceEqual<T>(ImmutableArray<T> left, ImmutableArray<T> right)
        where T : IEquatable<T>
    {
        if (left.Length != right.Length)
        {
            return false;
        }
        for (var index = 0; index < left.Length; index++)
        {
            if (!left[index].Equals(right[index]))
            {
                return false;
            }
        }
        return true;
    }

    private sealed class DataGridRowKeyComparer : IComparer<DataGridRowKey>
    {
        internal static DataGridRowKeyComparer Instance { get; } = new();

        public int Compare(DataGridRowKey left, DataGridRowKey right) => left.CompareTo(right);
    }
}
