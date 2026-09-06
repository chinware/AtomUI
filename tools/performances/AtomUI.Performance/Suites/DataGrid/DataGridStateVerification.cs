using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.DataGridPerformanceSupport;

internal static class DataGridStateVerifier
{
    private const int VerificationVisibleRowCount = 12;
    private const int VerificationScrollCycleCount = 10_000;
    private const int ViewportSupersessionSampleCount = 20;
    private static readonly TimeSpan ViewportSourceDelay = TimeSpan.FromMilliseconds(180);

    internal static bool Run()
    {
        var failures = new List<string>();
        VerifyMillionRowLocalProjection(failures);
        VerifyLongRemotePageRequest(failures);
        VerifyViewportSupersessionLatency(failures);
        VerifyRangeVirtualizationPlateau(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("DataGrid state verification passed.");
            return true;
        }

        Console.Error.WriteLine("DataGrid state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyMillionRowLocalProjection(ICollection<string> failures)
    {
        using var source = DataGridLocalSource.Create(
            new MillionLocalRowList(1_000_000),
            MillionLocalRowDescriptor,
            new DataGridLocalSourceOptions
            {
                PreferredRangeSize = 64,
                MaximumRangeSize = 64,
                MaximumProjectionCount = 2
            });
        var query = DataGridQuery.Empty.WithSorts(
            [new DataGridSort(AgeField, DataGridSortDirection.Descending)]);
        var result = source.FetchAsync(
                CreateFetchRequest(
                    query,
                    new DataGridRange(0, 64)),
                CancellationToken.None)
            .AsTask()
            .GetAwaiter()
            .GetResult();

        Expect(result.TotalDataCount == 1_000_000,
            $"Million-row local Source should report the complete total. Actual: {result.TotalDataCount}.",
            failures);
        Expect(result.Entries.Length == 64,
            $"Million-row local Source should return only the requested range. Actual: {result.Entries.Length}.",
            failures);
        Expect(source.CachedProjectionCount <= 2,
            $"Local projection cache should remain bounded. Actual: {source.CachedProjectionCount}.",
            failures);
        Expect(result.Entries.All(entry => entry.Item is MillionLocalRow),
            "Million-row local Source should project the typed rows without a second full object list.",
            failures);

        Console.WriteLine(
            $"DataGrid local million: returned={result.Entries.Length}, projections={source.CachedProjectionCount}, total={result.TotalDataCount}.");
    }

    private static void VerifyLongRemotePageRequest(ICollection<string> failures)
    {
        const long totalDataCount = 10_000_000_000;
        using var source = new VerificationRemoteDataGridSource(totalDataCount);
        var pageRequest = new DataGridPageRequest(9_000_000_000, 1_000);
        var result = source.FetchAsync(
                CreateFetchRequest(
                    DataGridQuery.Empty,
                    new DataGridRange(960, 40),
                    pageRequest),
                CancellationToken.None)
            .AsTask()
            .GetAwaiter()
            .GetResult();

        Expect(result.TotalDataCount == totalDataCount,
            $"Remote Source should preserve long totals. Actual: {result.TotalDataCount}.",
            failures);
        Expect(result.WindowDataCount == pageRequest.DataCount,
            $"Remote page request count should remain exact. Actual: {result.WindowDataCount}.",
            failures);
        Expect(result.Entries.Length == 40,
            $"Remote Source should return only the requested page range. Actual: {result.Entries.Length}.",
            failures);
        Expect(result.Entries[0].DataIndex == 9_000_000_960,
            $"Remote DataIndex should remain global and long-valued. Actual: {result.Entries[0].DataIndex}.",
            failures);
        Expect(source.MaximumReturnedRangeCount == 40,
            $"Remote Source must not allocate a total-sized result. Maximum returned: {source.MaximumReturnedRangeCount}.",
            failures);

        Console.WriteLine(
            $"DataGrid long remote: returned={result.Entries.Length}, window={result.WindowDataCount}, total={result.TotalDataCount}.");
    }

    private static void VerifyRangeVirtualizationPlateau(ICollection<string> failures)
    {
        using var source = new VerificationRemoteDataGridSource(1_000_000);
        var grid = CreateVerificationDataGrid(source);
        using var realized = RealizeControl(grid);

        WaitForRangeIdle(grid, failures, "initial viewport");
        Expect(grid.LoadState == DataGridLoadState.Ready,
            $"Remote grid should reach Ready. Actual: {grid.LoadState}.",
            failures);
        Expect(grid.TotalItemCount == 1_000_000,
            $"Remote grid should expose the million-row logical total. Actual: {grid.TotalItemCount}.",
            failures);
        Expect(grid.RangeWindowDataCount == 1_000_000,
            $"Continuous remote window should contain one million logical slots. Actual: {grid.RangeWindowDataCount}.",
            failures);
        Expect(source.InvalidatedSubscriberCount == 1,
            $"One realized DataGrid should own exactly one Source subscription. Actual: {source.InvalidatedSubscriberCount}.",
            failures);

        var realizedCells = grid.GetVisualDescendants().OfType<DataGridCell>().ToArray();
        Expect(realizedCells.Length > 0,
            "The performance verifier should realize DataGrid cells.",
            failures);
        Expect(realizedCells.All(cell => !cell.HasLongLivedSortSubscription),
            "Every realized DataGridCell must have zero long-lived sort subscriptions.",
            failures);

        var requestsBeforeNoOp = source.RequestCount;
        grid.Query = new DataGridQuery([], [], []);
        Dispatcher.UIThread.RunJobs();
        Expect(source.RequestCount == requestsBeforeNoOp,
            $"An equal immutable Query must issue zero requests. Before: {requestsBeforeNoOp}; after: {source.RequestCount}.",
            failures);

        var requestsBeforeLayout = source.RequestCount;
        RefreshLayout(realized.Window);
        RefreshLayout(realized.Window);
        Expect(source.RequestCount == requestsBeforeLayout,
            $"Measure/Arrange/container work must issue zero Source fetches. Before: {requestsBeforeLayout}; after: {source.RequestCount}.",
            failures);

        RangeStateSnapshot? plateau = null;
        var maximumRealizedRows = 0;
        for (var cycle = 0; cycle < VerificationScrollCycleCount; cycle++)
        {
            var firstVisibleSlot = 1 + (int)(((long)cycle * 7_919) % 999_800);
            var direction = cycle == 0 || firstVisibleSlot >=
                            1 + (int)(((long)(cycle - 1) * 7_919) % 999_800)
                ? 1
                : -1;
            grid.RequestRangeViewport(
                firstVisibleSlot,
                VerificationVisibleRowCount,
                direction);
            Dispatcher.UIThread.RunJobs();
            WaitForRangeIdle(grid, failures, $"scroll cycle {cycle + 1}");

            if ((cycle & 31) == 0)
            {
                realized.Window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();
            }

            maximumRealizedRows = Math.Max(
                maximumRealizedRows,
                CountRealizedRows(grid));
            if (cycle == 999)
            {
                plateau = CaptureRangeState(grid);
            }
        }

        RefreshLayout(realized.Window);
        WaitForRangeIdle(grid, failures, "final viewport");
        var final = CaptureRangeState(grid);
        plateau ??= final;

        Expect(final.CacheCount <= 16,
            $"Range cache must remain at its fixed capacity. Actual: {final.CacheCount}.",
            failures);
        Expect(final.CacheCount <= plateau.CacheCount,
            $"Range cache must plateau after warmup. At 1,000: {plateau.CacheCount}; at 10,000: {final.CacheCount}.",
            failures);
        Expect(plateau.PinCount <= 4 && final.PinCount <= 4,
            $"Range pins must remain viewport-bounded at both samples. At 1,000: {plateau.PinCount}; at 10,000: {final.PinCount}.",
            failures);
        Expect(final.MeasuredHeightCount <= 256 &&
               final.MeasuredHeightCount <= plateau.MeasuredHeightCount + 64,
            $"Measured heights must plateau with retained range blocks. At 1,000: {plateau.MeasuredHeightCount}; at 10,000: {final.MeasuredHeightCount}.",
            failures);
        Expect(maximumRealizedRows <= VerificationVisibleRowCount + 2,
            $"Realized rows must stay within visible plus edit/drag pins. Maximum: {maximumRealizedRows}.",
            failures);
        Expect(source.PeakConcurrentRequestCount <= 2,
            $"Range fetch concurrency must not exceed two. Actual: {source.PeakConcurrentRequestCount}.",
            failures);
        Expect(grid.RangeStaleCommitCount == 0,
            $"Stale result commits must be zero. Actual: {grid.RangeStaleCommitCount}.",
            failures);
        Expect(source.MaximumReturnedRangeCount <= source.Schema.MaximumRangeSize,
            $"Remote requests must stay range-bounded. Maximum returned: {source.MaximumReturnedRangeCount}.",
            failures);
        Expect(final.ViewportCommitCount >= VerificationScrollCycleCount,
            $"All requested final viewports should commit. Actual commits: {final.ViewportCommitCount}.",
            failures);

        Console.WriteLine(
            "DataGrid range plateau: " +
            $"cycles={VerificationScrollCycleCount}, requests={source.RequestCount}, generated={source.GeneratedRowCount}, " +
            $"maxRange={source.MaximumReturnedRangeCount}, maxConcurrent={source.PeakConcurrentRequestCount}, " +
            $"realizedMax={maximumRealizedRows}, cache={final.CacheCount}, pins={final.PinCount}, " +
            $"heights={final.MeasuredHeightCount}, staleCommits={grid.RangeStaleCommitCount}.");

        realized.Window.Content = null;
        Dispatcher.UIThread.RunJobs();
        Expect(source.InvalidatedSubscriberCount == 0,
            $"Detaching the DataGrid must release its Source subscription. Actual: {source.InvalidatedSubscriberCount}.",
            failures);
        Expect(source.ActiveRequestCount == 0 &&
               grid.RangeActiveRequestCount == 0 &&
               grid.RangeInFlightBlockCount == 0,
            "Detaching the DataGrid must leave no active or in-flight range requests.",
            failures);
        Expect(grid.RangeCacheCount == 0 &&
               grid.RangeCachePinCount == 0 &&
               grid.RangeMeasuredHeightCount == 0,
            "Detaching the DataGrid must release cache, pins, and measured-height state.",
            failures);
    }

    private static void VerifyViewportSupersessionLatency(ICollection<string> failures)
    {
        using var source = new DelayedVerificationDataGridSource(
            totalDataCount: 1_000_000,
            ViewportSourceDelay);
        var grid = CreateVerificationDataGrid(source);
        using var realized = RealizeControl(grid);
        WaitForCondition(
            () => grid.LoadState == DataGridLoadState.Ready,
            TimeSpan.FromSeconds(5),
            "delayed Source initial viewport",
            failures);

        var elapsed = new double[ViewportSupersessionSampleCount];
        var commitBaseline = grid.RangeViewportCommitCount;
        for (var sample = 0; sample < elapsed.Length; sample++)
        {
            var sampleBase = 10_000 + sample * 20_000;
            for (var intent = 0; intent < 8; intent++)
            {
                grid.RequestRangeViewport(
                    sampleBase + intent * 128,
                    VerificationVisibleRowCount,
                    scrollDirection: 1);
                Dispatcher.UIThread.RunJobs();
            }

            var finalTarget = sampleBase + 1_024;
            var started = Stopwatch.GetTimestamp();
            grid.RequestRangeViewport(
                finalTarget,
                VerificationVisibleRowCount,
                scrollDirection: 1);
            Dispatcher.UIThread.RunJobs();
            WaitForCondition(
                () => grid.LoadState == DataGridLoadState.Ready &&
                      grid.DisplayData.FirstScrollingSlot == finalTarget,
                TimeSpan.FromSeconds(5),
                $"viewport supersession sample {sample + 1}",
                failures);
            elapsed[sample] = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
        }

        Array.Sort(elapsed);
        var mean = elapsed.Average();
        var median = Percentile(elapsed, 0.50);
        var p95 = Percentile(elapsed, 0.95);
        Expect(p95 < 600,
            $"Final viewport P95 should remain below 600 ms for a 180 ms cooperative Source. Actual: {p95:F3} ms.",
            failures);
        Expect(source.PeakConcurrentRequestCount <= 2,
            $"Delayed Source concurrency must not exceed two. Actual: {source.PeakConcurrentRequestCount}.",
            failures);
        Expect(grid.RangeViewportCommitCount - commitBaseline >= ViewportSupersessionSampleCount,
            $"Every final viewport should commit. Expected at least {ViewportSupersessionSampleCount}; actual: {grid.RangeViewportCommitCount - commitBaseline}.",
            failures);
        Expect(grid.RangeStaleCommitCount == 0,
            $"Delayed supersession must produce zero stale commits. Actual: {grid.RangeStaleCommitCount}.",
            failures);

        Console.WriteLine(
            "DataGrid viewport supersession: " +
            $"samples={ViewportSupersessionSampleCount}, sourceDelayMs={ViewportSourceDelay.TotalMilliseconds:F0}, " +
            $"meanMs={mean:F3}, medianMs={median:F3}, p95Ms={p95:F3}, " +
            $"requests={source.RequestCount}, canceled={source.CanceledRequestCount}, " +
            $"maxConcurrent={source.PeakConcurrentRequestCount}, staleCommits={grid.RangeStaleCommitCount}.");

        realized.Window.Content = null;
        Dispatcher.UIThread.RunJobs();
        WaitForCondition(
            () => source.ActiveRequestCount == 0 &&
                  grid.RangeActiveRequestCount == 0 &&
                  grid.RangeInFlightBlockCount == 0,
            TimeSpan.FromSeconds(5),
            "delayed Source detach cleanup",
            failures);
        Expect(source.InvalidatedSubscriberCount == 0,
            $"Delayed Source detach must release its invalidation subscription. Actual: {source.InvalidatedSubscriberCount}.",
            failures);
    }

    private static DataGrid CreateVerificationDataGrid(IDataGridSource source)
    {
        var grid = new DataGrid
        {
            Width = 720,
            Height = 300,
            AutoGenerateColumns = false,
            ItemsSource = source
        };
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            FieldId = NameField,
            Binding = new Binding(nameof(VerificationRemoteRow.Name))
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Age",
            FieldId = AgeField,
            Binding = new Binding(nameof(VerificationRemoteRow.Age))
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Address",
            FieldId = AddressField,
            Binding = new Binding(nameof(VerificationRemoteRow.Address))
        });
        return grid;
    }

    private static DataGridFetchRequest CreateFetchRequest(
        DataGridQuery query,
        DataGridRange range,
        DataGridPageRequest? pageRequest = null) =>
        new(
            query,
            pageRequest,
            DataGridGroupExpansion.AllExpanded,
            range,
            expectedSnapshot: null,
            queryRevision: 1,
            dataGeneration: 1);

    private static void WaitForRangeIdle(
        DataGrid grid,
        ICollection<string> failures,
        string operation)
    {
        for (var attempt = 0; attempt < 2_000; attempt++)
        {
            Dispatcher.UIThread.RunJobs();
            if (!grid.HasPendingRangeViewport &&
                grid.RangeActiveRequestCount == 0 &&
                grid.RangeInFlightBlockCount == 0)
            {
                return;
            }
            Thread.Yield();
        }

        failures.Add(
            $"DataGrid did not become idle after {operation}: pending={grid.HasPendingRangeViewport}, " +
            $"active={grid.RangeActiveRequestCount}, inFlight={grid.RangeInFlightBlockCount}.");
    }

    private static void WaitForCondition(
        Func<bool> condition,
        TimeSpan timeout,
        string operation,
        ICollection<string> failures)
    {
        var started = Stopwatch.GetTimestamp();
        while (!condition())
        {
            Dispatcher.UIThread.RunJobs();
            if (Stopwatch.GetElapsedTime(started) >= timeout)
            {
                failures.Add($"DataGrid did not complete {operation} within {timeout.TotalSeconds:F1} seconds.");
                return;
            }
            Thread.Yield();
        }
    }

    private static double Percentile(double[] sorted, double percentile)
    {
        var index = (int)Math.Ceiling(sorted.Length * percentile) - 1;
        return sorted[Math.Clamp(index, 0, sorted.Length - 1)];
    }

    private static int CountRealizedRows(DataGrid grid) =>
        grid.GetVisualDescendants().OfType<DataGridRow>().Count();

    private static RangeStateSnapshot CaptureRangeState(DataGrid grid) =>
        new(
            grid.RangeCacheCount,
            grid.RangeCachePinCount,
            grid.RangeMeasuredHeightCount,
            grid.RangeViewportCommitCount);

    private static VerificationRealization RealizeControl(Control control)
    {
        var window = new AvaloniaWindow
        {
            Width = 900,
            Height = 640,
            Content = control,
            ShowInTaskbar = false
        };
        window.Show();
        RefreshLayout(window);
        return new VerificationRealization(window);
    }

    private static void RefreshLayout(AvaloniaWindow window)
    {
        Dispatcher.UIThread.RunJobs();
        window.Measure(new Size(1_280, 4_096));
        window.Arrange(new Rect(0, 0, 1_280, 4_096));
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Expect(
        bool condition,
        string failure,
        ICollection<string> failures)
    {
        if (!condition)
        {
            failures.Add(failure);
        }
    }

    private static readonly DataGridFieldId NameField = new("name");
    private static readonly DataGridFieldId AgeField = new("age");
    private static readonly DataGridFieldId AddressField = new("address");

    private static readonly DataGridLocalSourceDescriptor<MillionLocalRow> MillionLocalRowDescriptor =
        DataGridLocalSourceDescriptor.For<MillionLocalRow>(static row => DataGridRowKey.FromInt64(row.Id))
            .Field(AgeField, static row => row.Age);

    private readonly record struct MillionLocalRow(int Id, int Age);

    private sealed class MillionLocalRowList : IReadOnlyList<MillionLocalRow>
    {
        public MillionLocalRowList(int count)
        {
            Count = count;
        }

        public int Count { get; }

        public MillionLocalRow this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return new MillionLocalRow(index, index % 100);
            }
        }

        public IEnumerator<MillionLocalRow> GetEnumerator()
        {
            for (var index = 0; index < Count; index++)
            {
                yield return this[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed record RangeStateSnapshot(
        int CacheCount,
        int PinCount,
        int MeasuredHeightCount,
        int ViewportCommitCount);

    private sealed class VerificationRealization : IDisposable
    {
        public VerificationRealization(AvaloniaWindow window)
        {
            Window = window;
        }

        public AvaloniaWindow Window { get; }

        public void Dispose()
        {
            Window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private sealed class VerificationRemoteRow
    {
        public long Id { get; init; }
        public string? Name { get; init; }
        public int Age { get; init; }
        public string? Address { get; init; }
    }

    private sealed class VerificationRemoteDataGridSource : IDataGridSource, IDisposable
    {
        private static readonly DataGridSnapshotId Snapshot = new("verification-remote-v1");
        private readonly object _gate = new();
        private readonly long _totalDataCount;
        private EventHandler? _invalidated;
        private int _requestCount;
        private int _generatedRowCount;
        private int _maximumReturnedRangeCount;
        private int _activeRequestCount;
        private int _peakConcurrentRequestCount;
        private bool _isDisposed;

        public VerificationRemoteDataGridSource(long totalDataCount)
        {
            _totalDataCount = totalDataCount;
            Schema = new DataGridSourceSchema(
                typeof(VerificationRemoteRow),
                [
                    new DataGridFieldSchema(
                        NameField,
                        typeof(string),
                        DataGridSortDirections.None,
                        [],
                        canGroup: false),
                    new DataGridFieldSchema(
                        AgeField,
                        typeof(int),
                        DataGridSortDirections.None,
                        [],
                        canGroup: false),
                    new DataGridFieldSchema(
                        AddressField,
                        typeof(string),
                        DataGridSortDirections.None,
                        [],
                        canGroup: false)
                ],
                preferredRangeSize: 64,
                maximumRangeSize: 64);
        }

        public DataGridSourceSchema Schema { get; }

        public int RequestCount => Volatile.Read(ref _requestCount);

        public int GeneratedRowCount => Volatile.Read(ref _generatedRowCount);

        public int MaximumReturnedRangeCount => Volatile.Read(ref _maximumReturnedRangeCount);

        public int ActiveRequestCount => Volatile.Read(ref _activeRequestCount);

        public int PeakConcurrentRequestCount => Volatile.Read(ref _peakConcurrentRequestCount);

        public int InvalidatedSubscriberCount
        {
            get
            {
                lock (_gate)
                {
                    return _invalidated?.GetInvocationList().Length ?? 0;
                }
            }
        }

        public event EventHandler? Invalidated
        {
            add
            {
                lock (_gate)
                {
                    ThrowIfDisposed();
                    _invalidated += value;
                }
            }
            remove
            {
                lock (_gate)
                {
                    _invalidated -= value;
                }
            }
        }

        public ValueTask<DataGridRangeResult> FetchAsync(
            DataGridFetchRequest request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (_gate)
            {
                ThrowIfDisposed();
            }

            Interlocked.Increment(ref _requestCount);
            var active = Interlocked.Increment(ref _activeRequestCount);
            RecordMaximum(ref _peakConcurrentRequestCount, active);
            try
            {
                if (request.ExpectedSnapshot is { } expectedSnapshot && expectedSnapshot != Snapshot)
                {
                    throw new DataGridSnapshotExpiredException(
                        $"Snapshot '{expectedSnapshot}' is not current.");
                }

                var pageStart = request.PageRequest?.DataStartIndex ?? 0;
                var availableDataCount = Math.Max(0L, _totalDataCount - pageStart);
                var requestedWindowCount = request.PageRequest?.DataCount ??
                                           (int)Math.Min(int.MaxValue, _totalDataCount);
                var windowDataCount = (int)Math.Min(requestedWindowCount, availableDataCount);
                var start = Math.Min(request.Range.StartIndex, windowDataCount);
                var count = Math.Min(request.Range.Count, windowDataCount - start);
                var entries = ImmutableArray.CreateBuilder<DataGridSourceEntry>(count);
                for (var offset = 0; offset < count; offset++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var windowIndex = start + offset;
                    var dataIndex = checked(pageStart + windowIndex);
                    entries.Add(DataGridSourceEntry.CreateData(
                        DataGridRowKey.FromInt64(dataIndex + 1),
                        new VerificationRemoteRow
                        {
                            Id = dataIndex,
                            Name = $"Remote {dataIndex + 1}",
                            Age = 18 + (int)(dataIndex % 48),
                            Address = $"Shard {dataIndex % 32:D2}"
                        },
                        windowIndex,
                        dataIndex));
                }

                Interlocked.Add(ref _generatedRowCount, count);
                RecordMaximum(ref _maximumReturnedRangeCount, count);
                return new ValueTask<DataGridRangeResult>(new DataGridRangeResult(
                    start,
                    entries.MoveToImmutable(),
                    windowDataCount,
                    windowDataCount,
                    _totalDataCount,
                    Snapshot));
            }
            finally
            {
                Interlocked.Decrement(ref _activeRequestCount);
            }
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _isDisposed = true;
                _invalidated = null;
            }
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
        }

        private static void RecordMaximum(ref int target, int candidate)
        {
            var observed = Volatile.Read(ref target);
            while (candidate > observed)
            {
                var previous = Interlocked.CompareExchange(ref target, candidate, observed);
                if (previous == observed)
                {
                    return;
                }
                observed = previous;
            }
        }
    }

    private sealed class DelayedVerificationDataGridSource : IDataGridSource, IDisposable
    {
        private static readonly DataGridSnapshotId Snapshot = new("verification-delayed-v1");
        private readonly object _gate = new();
        private readonly long _totalDataCount;
        private readonly TimeSpan _delay;
        private EventHandler? _invalidated;
        private int _requestCount;
        private int _canceledRequestCount;
        private int _activeRequestCount;
        private int _peakConcurrentRequestCount;
        private bool _isDisposed;

        public DelayedVerificationDataGridSource(long totalDataCount, TimeSpan delay)
        {
            _totalDataCount = totalDataCount;
            _delay = delay;
            Schema = new DataGridSourceSchema(
                typeof(VerificationRemoteRow),
                [
                    new DataGridFieldSchema(
                        NameField,
                        typeof(string),
                        DataGridSortDirections.None,
                        [],
                        canGroup: false),
                    new DataGridFieldSchema(
                        AgeField,
                        typeof(int),
                        DataGridSortDirections.None,
                        [],
                        canGroup: false),
                    new DataGridFieldSchema(
                        AddressField,
                        typeof(string),
                        DataGridSortDirections.None,
                        [],
                        canGroup: false)
                ],
                preferredRangeSize: 64,
                maximumRangeSize: 64);
        }

        public DataGridSourceSchema Schema { get; }

        public int RequestCount => Volatile.Read(ref _requestCount);

        public int CanceledRequestCount => Volatile.Read(ref _canceledRequestCount);

        public int ActiveRequestCount => Volatile.Read(ref _activeRequestCount);

        public int PeakConcurrentRequestCount => Volatile.Read(ref _peakConcurrentRequestCount);

        public int InvalidatedSubscriberCount
        {
            get
            {
                lock (_gate)
                {
                    return _invalidated?.GetInvocationList().Length ?? 0;
                }
            }
        }

        public event EventHandler? Invalidated
        {
            add
            {
                lock (_gate)
                {
                    ObjectDisposedException.ThrowIf(_isDisposed, this);
                    _invalidated += value;
                }
            }
            remove
            {
                lock (_gate)
                {
                    _invalidated -= value;
                }
            }
        }

        public async ValueTask<DataGridRangeResult> FetchAsync(
            DataGridFetchRequest request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Interlocked.Increment(ref _requestCount);
            var active = Interlocked.Increment(ref _activeRequestCount);
            RecordMaximum(ref _peakConcurrentRequestCount, active);
            try
            {
                await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                if (request.ExpectedSnapshot is { } expectedSnapshot && expectedSnapshot != Snapshot)
                {
                    throw new DataGridSnapshotExpiredException(
                        $"Snapshot '{expectedSnapshot}' is not current.");
                }

                var windowDataCount = (int)Math.Min(int.MaxValue, _totalDataCount);
                var start = Math.Min(request.Range.StartIndex, windowDataCount);
                var count = Math.Min(request.Range.Count, windowDataCount - start);
                var entries = ImmutableArray.CreateBuilder<DataGridSourceEntry>(count);
                for (var offset = 0; offset < count; offset++)
                {
                    var dataIndex = start + offset;
                    entries.Add(DataGridSourceEntry.CreateData(
                        DataGridRowKey.FromInt64(dataIndex + 1L),
                        new VerificationRemoteRow
                        {
                            Id = dataIndex,
                            Name = $"Delayed {dataIndex + 1}",
                            Age = 18 + dataIndex % 48,
                            Address = $"Shard {dataIndex % 32:D2}"
                        },
                        dataIndex,
                        dataIndex));
                }
                return new DataGridRangeResult(
                    start,
                    entries.MoveToImmutable(),
                    windowDataCount,
                    windowDataCount,
                    _totalDataCount,
                    Snapshot);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Interlocked.Increment(ref _canceledRequestCount);
                throw;
            }
            finally
            {
                Interlocked.Decrement(ref _activeRequestCount);
            }
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _isDisposed = true;
                _invalidated = null;
            }
        }

        private static void RecordMaximum(ref int target, int candidate)
        {
            var observed = Volatile.Read(ref target);
            while (candidate > observed)
            {
                var previous = Interlocked.CompareExchange(ref target, candidate, observed);
                if (previous == observed)
                {
                    return;
                }
                observed = previous;
            }
        }
    }
}
