using System.Collections.Immutable;

namespace AtomUI.Desktop.Controls.Tests.DataGrid;

internal sealed class TestDataGridSource<T> : IDataGridSource
{
    private readonly IReadOnlyList<T> _items;
    private readonly Func<T, int, DataGridRowKey> _rowKey;
    private readonly string _identity = $"test-{Guid.NewGuid():N}";
    private int _version;

    public TestDataGridSource(
        IReadOnlyList<T> items,
        DataGridSourceSchema? schema = null,
        Func<T, int, DataGridRowKey>? rowKey = null)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items = items;
        _rowKey = rowKey ?? (static (_, index) => DataGridRowKey.FromInt64(index + 1L));
        Schema = schema ?? new DataGridSourceSchema(
            typeof(T),
            ImmutableArray<DataGridFieldSchema>.Empty,
            preferredRangeSize: 32,
            maximumRangeSize: 128);
    }

    public DataGridSourceSchema Schema { get; }

    public event EventHandler? Invalidated;

    public void Invalidate()
    {
        Interlocked.Increment(ref _version);
        Invalidated?.Invoke(this, EventArgs.Empty);
    }

    public ValueTask<DataGridRangeResult> FetchAsync(
        DataGridFetchRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        DataGridSourceContractValidator.ValidateRequest(request, Schema);
        var snapshot = new DataGridSnapshotId($"{_identity}-{Volatile.Read(ref _version)}");
        if (request.ExpectedSnapshot is { } expected && expected != snapshot)
        {
            throw new DataGridSnapshotExpiredException(
                $"Snapshot '{expected}' does not match '{snapshot}'.");
        }

        var start = Math.Min(request.Range.StartIndex, _items.Count);
        var count = Math.Min(request.Range.Count, _items.Count - start);
        var entries = ImmutableArray.CreateBuilder<DataGridSourceEntry>(count);
        for (var offset = 0; offset < count; offset++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var index = start + offset;
            var item = _items[index];
            entries.Add(DataGridSourceEntry.CreateData(
                _rowKey(item, index),
                item,
                index,
                index));
        }

        return ValueTask.FromResult(new DataGridRangeResult(
            start,
            entries.MoveToImmutable(),
            _items.Count,
            _items.Count,
            _items.Count,
            snapshot));
    }
}
