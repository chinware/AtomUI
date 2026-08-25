namespace AtomUI.Controls;

internal sealed class ImageRequestScheduler : IDisposable
{
    private readonly PriorityScheduler _downloadScheduler;
    private readonly PriorityScheduler _localReadScheduler;
    private readonly PriorityScheduler _decodeScheduler;

    internal ImageRequestScheduler(
        int maxConcurrentDownloads,
        int maxConcurrentLocalReads,
        int maxConcurrentDecodes,
        Func<DateTimeOffset>? clock = null)
    {
        _downloadScheduler = new PriorityScheduler(maxConcurrentDownloads, clock);
        _localReadScheduler = new PriorityScheduler(maxConcurrentLocalReads, clock);
        _decodeScheduler = new PriorityScheduler(maxConcurrentDecodes, clock);
    }

    internal int ActiveReads => _downloadScheduler.ActiveCount + _localReadScheduler.ActiveCount;

    internal int QueuedReads => _downloadScheduler.QueuedCount + _localReadScheduler.QueuedCount;

    internal int ActiveDecodes => _decodeScheduler.ActiveCount;

    internal int QueuedDecodes => _decodeScheduler.QueuedCount;

    internal Task<T> ScheduleReadAsync<T>(
        ImageLoadSourceKind sourceKind,
        Func<CancellationToken, Task<T>> action,
        Func<ImageRequestPriority> priority,
        CancellationToken cancellationToken)
    {
        var scheduler = sourceKind == ImageLoadSourceKind.Http
            ? _downloadScheduler
            : _localReadScheduler;
        return scheduler.ScheduleAsync(action, priority, cancellationToken);
    }

    internal Task<T> ScheduleDecodeAsync<T>(
        Func<CancellationToken, Task<T>> action,
        Func<ImageRequestPriority> priority,
        CancellationToken cancellationToken)
    {
        return _decodeScheduler.ScheduleAsync(action, priority, cancellationToken);
    }

    public void Dispose()
    {
        _downloadScheduler.Dispose();
        _localReadScheduler.Dispose();
        _decodeScheduler.Dispose();
    }

    private sealed class PriorityScheduler : IDisposable
    {
        private readonly object _gate = new();
        private readonly List<IWorkItem> _queue = [];
        private readonly SemaphoreSlim _signal = new(0);
        private readonly CancellationTokenSource _disposeCancellation = new();
        private readonly Task[] _workers;
        private readonly Func<DateTimeOffset> _clock;
        private long _sequence;
        private int _activeCount;
        private bool _disposed;

        internal PriorityScheduler(int concurrency, Func<DateTimeOffset>? clock)
        {
            _clock = clock ?? GetUtcNow;
            _workers = Enumerable.Range(0, concurrency)
                .Select(_ => Task.Run(WorkerAsync))
                .ToArray();
        }

        internal int ActiveCount => Volatile.Read(ref _activeCount);

        internal int QueuedCount
        {
            get { lock (_gate) return _queue.Count; }
        }

        internal Task<T> ScheduleAsync<T>(
            Func<CancellationToken, Task<T>> action,
            Func<ImageRequestPriority> priority,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(action);
            ArgumentNullException.ThrowIfNull(priority);
            var item = new WorkItem<T>(
                action,
                priority,
                Interlocked.Increment(ref _sequence),
                _clock(),
                cancellationToken);
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                _queue.Add(item);
                _signal.Release();
            }
            return item.Task;
        }

        public void Dispose()
        {
            List<IWorkItem> queued;
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }
                _disposed = true;
                queued = [.. _queue];
                _queue.Clear();
            }
            _disposeCancellation.Cancel();
            foreach (var item in queued)
            {
                item.Cancel();
            }
            _signal.Release(_workers.Length);
            try
            {
                Task.WhenAll(_workers).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
            }
            _disposeCancellation.Dispose();
            _signal.Dispose();
        }

        private async Task WorkerAsync()
        {
            while (true)
            {
                try
                {
                    await _signal.WaitAsync(_disposeCancellation.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                IWorkItem? item;
                lock (_gate)
                {
                    if (_disposed)
                    {
                        return;
                    }
                    item = TakeNextCore();
                }
                if (item is null)
                {
                    continue;
                }

                Interlocked.Increment(ref _activeCount);
                try
                {
                    await item.ExecuteAsync(_disposeCancellation.Token).ConfigureAwait(false);
                }
                finally
                {
                    Interlocked.Decrement(ref _activeCount);
                }
            }
        }

        private IWorkItem? TakeNextCore()
        {
            IWorkItem? selected = null;
            var selectedIndex = -1;
            for (var index = 0; index < _queue.Count; index++)
            {
                var candidate = _queue[index];
                if (candidate.IsCanceled)
                {
                    _queue.RemoveAt(index--);
                    candidate.Cancel();
                    continue;
                }
                if (selected is null || Compare(candidate, selected, _clock()) < 0)
                {
                    selected = candidate;
                    selectedIndex = index;
                }
            }
            if (selectedIndex >= 0)
            {
                _queue.RemoveAt(selectedIndex);
            }
            return selected;
        }

        private static int Compare(IWorkItem first, IWorkItem second, DateTimeOffset now)
        {
            var firstPriority = EffectivePriority(first.Priority, first.EnqueuedAt, now);
            var secondPriority = EffectivePriority(second.Priority, second.EnqueuedAt, now);
            var comparison = firstPriority.CompareTo(secondPriority);
            return comparison != 0 ? comparison : first.Sequence.CompareTo(second.Sequence);
        }

        private static int EffectivePriority(
            ImageRequestPriority priority,
            DateTimeOffset enqueuedAt,
            DateTimeOffset now)
        {
            var baseValue = (int)priority;
            if (priority is ImageRequestPriority.Normal or ImageRequestPriority.Low)
            {
                var agedLevels = (int)((now - enqueuedAt).TotalSeconds / 2);
                baseValue = Math.Max((int)ImageRequestPriority.High, baseValue - agedLevels);
            }
            return baseValue;
        }

        private static DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;

        private interface IWorkItem
        {
            ImageRequestPriority Priority { get; }
            DateTimeOffset EnqueuedAt { get; }
            long Sequence { get; }
            bool IsCanceled { get; }
            Task ExecuteAsync(CancellationToken schedulerCancellation);
            void Cancel();
        }

        private sealed class WorkItem<T> : IWorkItem
        {
            private readonly Func<CancellationToken, Task<T>> _action;
            private readonly Func<ImageRequestPriority> _priority;
            private readonly CancellationToken _cancellationToken;
            private readonly TaskCompletionSource<T> _completion =
                new(TaskCreationOptions.RunContinuationsAsynchronously);

            internal WorkItem(
                Func<CancellationToken, Task<T>> action,
                Func<ImageRequestPriority> priority,
                long sequence,
                DateTimeOffset enqueuedAt,
                CancellationToken cancellationToken)
            {
                _action = action;
                _priority = priority;
                _cancellationToken = cancellationToken;
                Sequence = sequence;
                EnqueuedAt = enqueuedAt;
            }

            public ImageRequestPriority Priority => _priority();
            public DateTimeOffset EnqueuedAt { get; }
            public long Sequence { get; }
            public bool IsCanceled => _cancellationToken.IsCancellationRequested;
            internal Task<T> Task => _completion.Task;

            public async Task ExecuteAsync(CancellationToken schedulerCancellation)
            {
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                    schedulerCancellation,
                    _cancellationToken);
                try
                {
                    _completion.TrySetResult(await _action(linked.Token).ConfigureAwait(false));
                }
                catch (OperationCanceledException) when (linked.IsCancellationRequested)
                {
                    _completion.TrySetCanceled(_cancellationToken.IsCancellationRequested
                        ? _cancellationToken
                        : schedulerCancellation);
                }
                catch (Exception exception)
                {
                    _completion.TrySetException(exception);
                }
            }

            public void Cancel()
            {
                _completion.TrySetCanceled(_cancellationToken);
            }
        }
    }
}
