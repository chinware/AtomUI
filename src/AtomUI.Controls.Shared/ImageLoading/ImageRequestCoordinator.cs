namespace AtomUI.Controls;

internal sealed class ImageRequestCoordinator : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<ImageEncodedOperationKey, SharedOperation<ImageEncodedContent>> _encoded = [];
    private readonly Dictionary<ImageDecodedOperationKey, SharedOperation<ImageDecodedCacheEntry>> _decoded = [];
    private bool _disposed;

    internal Task<ImageEncodedContent> GetEncodedAsync(
        ImageEncodedOperationKey key,
        ImageRequestPriority priority,
        Func<SharedOperationContext, CancellationToken, Task<ImageEncodedContent>> factory,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken)
    {
        return JoinAsync(
            _encoded,
            key,
            priority,
            factory,
            static value => value,
            progress,
            cancellationToken,
            null);
    }

    internal Task<TResult> GetDecodedAsync<TResult>(
        ImageDecodedOperationKey key,
        ImageRequestPriority priority,
        Func<SharedOperationContext, CancellationToken, Task<ImageDecodedCacheEntry>> factory,
        Func<ImageDecodedCacheEntry, TResult> resultSelector,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken)
    {
        return JoinAsync(
            _decoded,
            key,
            priority,
            factory,
            resultSelector,
            progress,
            cancellationToken,
            entry => entry.ReleaseOperation());
    }

    internal void Cancel(string? partitionHash)
    {
        SharedOperationBase[] operations;
        lock (_gate)
        {
            operations = _encoded
                .Where(pair => partitionHash is null || pair.Key.CacheKey.PartitionHash == partitionHash)
                .Select(pair => (SharedOperationBase)pair.Value)
                .Concat(_decoded
                    .Where(pair => partitionHash is null || pair.Key.CacheKey.EncodedKey.PartitionHash == partitionHash)
                    .Select(pair => (SharedOperationBase)pair.Value))
                .Distinct()
                .ToArray();
        }
        foreach (var operation in operations)
        {
            operation.Cancel();
        }
    }

    public void Dispose()
    {
        SharedOperationBase[] operations;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            operations = _encoded.Values.Cast<SharedOperationBase>()
                .Concat(_decoded.Values)
                .Distinct()
                .ToArray();
            _encoded.Clear();
            _decoded.Clear();
        }
        foreach (var operation in operations)
        {
            operation.Cancel();
        }
        try
        {
            Task.WhenAll(operations.Select(operation => operation.Completion))
                .GetAwaiter()
                .GetResult();
        }
        catch
        {
            // In-flight failures are delivered to their waiters; disposal only waits for cleanup.
        }
    }

    private Task<TResult> JoinAsync<TKey, TValue, TResult>(
        Dictionary<TKey, SharedOperation<TValue>> operations,
        TKey key,
        ImageRequestPriority priority,
        Func<SharedOperationContext, CancellationToken, Task<TValue>> factory,
        Func<TValue, TResult> resultSelector,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken,
        Action<TValue>? releaseValue)
        where TKey : notnull
    {
        SharedOperation<TValue> operation;
        long waiterId = 0;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (operations.TryGetValue(key, out operation!) &&
                !operation.TryAddWaiter(priority, progress, out waiterId))
            {
                operations.Remove(key);
                operation = null!;
            }
            if (operation is null)
            {
                operation = new SharedOperation<TValue>(factory, releaseValue);
                operations.Add(key, operation);
                operation.Completed += (_, _) =>
                {
                    lock (_gate)
                    {
                        if (operations.TryGetValue(key, out var current) && ReferenceEquals(current, operation))
                        {
                            operations.Remove(key);
                        }
                    }
                };
                if (!operation.TryAddWaiter(priority, progress, out waiterId))
                {
                    throw new InvalidOperationException("A new image operation rejected its first waiter.");
                }
                operation.Start();
                return WaitAsync(operation, waiterId, resultSelector, cancellationToken);
            }
        }
        return WaitAsync(operation, waiterId, resultSelector, cancellationToken);
    }

    private static async Task<TResult> WaitAsync<TValue, TResult>(
        SharedOperation<TValue> operation,
        long waiterId,
        Func<TValue, TResult> resultSelector,
        CancellationToken cancellationToken)
    {
        try
        {
            var value = await operation.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            return resultSelector(value);
        }
        finally
        {
            operation.RemoveWaiter(waiterId);
        }
    }

    internal sealed class SharedOperationContext
    {
        private readonly object _gate = new();
        private readonly Dictionary<long, IProgress<ImageLoadProgress>> _progress = [];
        private int _priority = (int)ImageRequestPriority.Preload;

        internal ImageRequestPriority Priority => (ImageRequestPriority)Volatile.Read(ref _priority);

        internal void SetPriority(ImageRequestPriority priority)
        {
            Volatile.Write(ref _priority, (int)priority);
        }

        internal void AddProgress(long waiterId, IProgress<ImageLoadProgress>? progress)
        {
            if (progress is null)
            {
                return;
            }
            lock (_gate)
            {
                _progress[waiterId] = progress;
            }
        }

        internal void RemoveProgress(long waiterId)
        {
            lock (_gate)
            {
                _progress.Remove(waiterId);
            }
        }

        internal void Report(ImageLoadProgress progress)
        {
            IProgress<ImageLoadProgress>[] reporters;
            lock (_gate)
            {
                reporters = _progress.Values.ToArray();
            }
            foreach (var reporter in reporters)
            {
                reporter.Report(progress);
            }
        }
    }

    private abstract class SharedOperationBase
    {
        internal abstract Task Completion { get; }

        internal abstract void Cancel();
    }

    private sealed class SharedOperation<T> : SharedOperationBase
    {
        private readonly object _gate = new();
        private readonly Func<SharedOperationContext, CancellationToken, Task<T>> _factory;
        private readonly Action<T>? _releaseValue;
        private readonly CancellationTokenSource _cancellation = new();
        private readonly Dictionary<long, ImageRequestPriority> _waiters = [];
        private readonly SharedOperationContext _context = new();
        private Task<T>? _task;
        private T? _completedValue;
        private bool _hasCompletedValue;
        private bool _released;
        private bool _completed;
        private bool _cancelRequested;
        private bool _cancellationDisposed;
        private long _nextWaiterId;

        internal SharedOperation(
            Func<SharedOperationContext, CancellationToken, Task<T>> factory,
            Action<T>? releaseValue)
        {
            _factory = factory;
            _releaseValue = releaseValue;
        }

        internal event EventHandler? Completed;

        internal Task<T> Task => _task ?? throw new InvalidOperationException("Operation has not started.");

        internal override Task Completion => Task;

        internal void Start()
        {
            _task = RunAsync();
        }

        internal bool TryAddWaiter(
            ImageRequestPriority priority,
            IProgress<ImageLoadProgress>? progress,
            out long waiterId)
        {
            lock (_gate)
            {
                if (_completed)
                {
                    waiterId = 0;
                    return false;
                }
                waiterId = ++_nextWaiterId;
                _waiters.Add(waiterId, priority);
                _context.AddProgress(waiterId, progress);
                UpdatePriorityCore();
                return true;
            }
        }

        internal void RemoveWaiter(long waiterId)
        {
            var cancel = false;
            var release = false;
            var disposeCancellation = false;
            lock (_gate)
            {
                _waiters.Remove(waiterId);
                _context.RemoveProgress(waiterId);
                UpdatePriorityCore();
                if (_waiters.Count == 0)
                {
                    cancel = !_completed && !_cancelRequested;
                    _cancelRequested |= cancel;
                    release = _hasCompletedValue && !_released;
                    _released |= release;
                    disposeCancellation = _completed && !_cancellationDisposed;
                    _cancellationDisposed |= disposeCancellation;
                }
            }
            if (cancel)
            {
                _cancellation.Cancel();
            }
            if (release)
            {
                _releaseValue?.Invoke(_completedValue!);
            }
            if (disposeCancellation)
            {
                _cancellation.Dispose();
            }
        }

        internal override void Cancel()
        {
            var cancel = false;
            lock (_gate)
            {
                cancel = !_completed && !_cancelRequested;
                _cancelRequested |= cancel;
            }
            if (cancel)
            {
                _cancellation.Cancel();
            }
        }

        private async Task<T> RunAsync()
        {
            try
            {
                var value = await _factory(_context, _cancellation.Token).ConfigureAwait(false);
                var release = false;
                lock (_gate)
                {
                    _completedValue = value;
                    _hasCompletedValue = true;
                    release = _waiters.Count == 0 && !_released;
                    _released |= release;
                }
                if (release)
                {
                    _releaseValue?.Invoke(value);
                }
                return value;
            }
            finally
            {
                var release = false;
                var disposeCancellation = false;
                lock (_gate)
                {
                    _completed = true;
                    release = _hasCompletedValue && _waiters.Count == 0 && !_released;
                    _released |= release;
                    disposeCancellation = _waiters.Count == 0 && !_cancellationDisposed;
                    _cancellationDisposed |= disposeCancellation;
                }
                if (release)
                {
                    _releaseValue?.Invoke(_completedValue!);
                }
                Completed?.Invoke(this, EventArgs.Empty);
                if (disposeCancellation)
                {
                    _cancellation.Dispose();
                }
            }
        }

        private void UpdatePriorityCore()
        {
            _context.SetPriority(_waiters.Count == 0
                ? ImageRequestPriority.Preload
                : _waiters.Values.Min());
        }
    }
}
