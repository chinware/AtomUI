using System.Collections.Immutable;

namespace AtomUI.Desktop.Controls;

internal readonly struct DataGridDesiredViewport : IEquatable<DataGridDesiredViewport>
{
    public DataGridDesiredViewport(int firstVisibleIndex, int visibleCount, int scrollDirection)
    {
        if (firstVisibleIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(firstVisibleIndex));
        }
        if (visibleCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(visibleCount));
        }
        if (scrollDirection is < -1 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(scrollDirection));
        }
        _ = checked(firstVisibleIndex + visibleCount);
        FirstVisibleIndex = firstVisibleIndex;
        VisibleCount = visibleCount;
        ScrollDirection = scrollDirection;
    }

    public int FirstVisibleIndex { get; }

    public int VisibleCount { get; }

    public int ScrollDirection { get; }

    public bool Equals(DataGridDesiredViewport other) =>
        FirstVisibleIndex == other.FirstVisibleIndex &&
        VisibleCount == other.VisibleCount &&
        ScrollDirection == other.ScrollDirection;

    public override bool Equals(object? obj) =>
        obj is DataGridDesiredViewport other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(FirstVisibleIndex, VisibleCount, ScrollDirection);
}

internal sealed class DataGridGeneration
{
    internal DataGridGeneration(
        IDataGridSource source,
        DataGridSourceSchema schema,
        DataGridQuery query,
        DataGridPageRequest? pageRequest,
        DataGridGroupExpansion groupExpansion,
        long queryRevision,
        long dataGeneration,
        long epoch,
        int snapshotExpiryRestarts,
        DataGridPresentationSnapshot? fallback,
        CancellationToken cancellationToken,
        DataGridViewportAnchor? viewportAnchor = null)
    {
        Source = source;
        Schema = schema;
        Query = query;
        PageRequest = pageRequest;
        GroupExpansion = groupExpansion;
        QueryRevision = queryRevision;
        DataGeneration = dataGeneration;
        Epoch = epoch;
        SnapshotExpiryRestarts = snapshotExpiryRestarts;
        Fallback = fallback;
        CancellationToken = cancellationToken;
        ViewportAnchor = viewportAnchor;
    }

    public IDataGridSource Source { get; }

    public DataGridSourceSchema Schema { get; }

    public DataGridQuery Query { get; }

    public DataGridPageRequest? PageRequest { get; }

    public DataGridGroupExpansion GroupExpansion { get; }

    public long QueryRevision { get; }

    public long DataGeneration { get; }

    public long Epoch { get; }

    public int SnapshotExpiryRestarts { get; }

    internal DataGridPresentationSnapshot? Fallback { get; set; }

    internal CancellationToken CancellationToken { get; }

    internal DataGridViewportAnchor? ViewportAnchor { get; set; }

    internal DataGridSourceResultIdentity? ResultIdentity { get; set; }
}

internal sealed class DataGridPresentationSnapshot : IDisposable
{
    private ImmutableArray<IDisposable> _pins;
    private int _isDisposed;

    public DataGridPresentationSnapshot(
        DataGridGeneration generation,
        DataGridDesiredViewport committedViewport,
        ImmutableArray<DataGridValidatedRangeBlock> blocks,
        ImmutableArray<IDisposable> pins,
        DataGridSourceResultIdentity identity)
    {
        Source = generation.Source;
        Query = generation.Query;
        PageRequest = generation.PageRequest;
        GroupExpansion = generation.GroupExpansion;
        QueryRevision = generation.QueryRevision;
        DataGeneration = generation.DataGeneration;
        Epoch = generation.Epoch;
        CommittedViewport = committedViewport;
        Blocks = blocks;
        _pins = pins;
        Snapshot = identity.Snapshot;
        TotalEntryCount = identity.TotalEntryCount;
        WindowDataCount = identity.WindowDataCount;
        TotalDataCount = identity.TotalDataCount;
    }

    public IDataGridSource Source { get; }

    public DataGridQuery Query { get; }

    public DataGridPageRequest? PageRequest { get; }

    public DataGridGroupExpansion GroupExpansion { get; }

    public long QueryRevision { get; }

    public long DataGeneration { get; }

    public long Epoch { get; }

    public DataGridDesiredViewport CommittedViewport { get; }

    public ImmutableArray<DataGridValidatedRangeBlock> Blocks { get; }

    public DataGridSnapshotId Snapshot { get; }

    public int TotalEntryCount { get; }

    public int WindowDataCount { get; }

    public long TotalDataCount { get; }

    public DataGridSourceEntry EntryAt(int displayIndex)
    {
        if (displayIndex < 0 || displayIndex >= TotalEntryCount)
        {
            throw new ArgumentOutOfRangeException(nameof(displayIndex));
        }
        foreach (var block in Blocks)
        {
            var offset = displayIndex - block.Range.StartIndex;
            if ((uint)offset < (uint)block.Entries.Length)
            {
                return block.Entries[offset];
            }
        }
        throw new InvalidOperationException(
            $"Display entry {displayIndex} is not present in the committed viewport blocks.");
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
        {
            return;
        }
        foreach (var pin in _pins)
        {
            pin.Dispose();
        }
        _pins = ImmutableArray<IDisposable>.Empty;
    }
}

internal enum DataGridPresentationTransitionKind
{
    Committed,
    RolledBack,
    Redirected,
    Failed,
    Superseded
}

internal readonly struct DataGridPresentationTransition
{
    public DataGridPresentationTransition(
        DataGridPresentationTransitionKind kind,
        DataGridPresentationSnapshot? snapshot,
        Exception? error,
        DataGridPageRequest? redirectedPageRequest = null)
    {
        Kind = kind;
        Snapshot = snapshot;
        Error = error;
        RedirectedPageRequest = redirectedPageRequest;
    }

    public DataGridPresentationTransitionKind Kind { get; }

    public DataGridPresentationSnapshot? Snapshot { get; }

    public Exception? Error { get; }

    public DataGridPageRequest? RedirectedPageRequest { get; }

    public static DataGridPresentationTransition Superseded() =>
        new(DataGridPresentationTransitionKind.Superseded, null, null);

    public static DataGridPresentationTransition Redirected(DataGridPageRequest pageRequest) =>
        new(DataGridPresentationTransitionKind.Redirected, null, null, pageRequest);
}
