namespace AtomUI.Theme.Compilation;

internal sealed class BoundedLruCache<TKey, TValue>
    where TKey : notnull
{
    private readonly object _gate = new();
    private readonly int _entryLimit;
    private readonly long _retainedBytesLimit;
    private readonly Func<TValue, long> _retainedBytesEstimator;
    private readonly Dictionary<TKey, LinkedListNode<CacheEntry>> _entries = new();
    private readonly LinkedList<CacheEntry> _lru = new();
    private readonly Dictionary<TKey, Task<TValue>> _inFlight = new();
    private long _retainedBytes;

    internal BoundedLruCache(
        int entryLimit,
        long retainedBytesLimit,
        Func<TValue, long> retainedBytesEstimator)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(entryLimit, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(retainedBytesLimit, 1);
        ArgumentNullException.ThrowIfNull(retainedBytesEstimator);

        _entryLimit             = entryLimit;
        _retainedBytesLimit     = retainedBytesLimit;
        _retainedBytesEstimator = retainedBytesEstimator;
    }

    internal int Count
    {
        get
        {
            lock (_gate)
            {
                return _entries.Count;
            }
        }
    }

    internal long RetainedBytes
    {
        get
        {
            lock (_gate)
            {
                return _retainedBytes;
            }
        }
    }

    internal int InFlightCount
    {
        get
        {
            lock (_gate)
            {
                return _inFlight.Count;
            }
        }
    }

    internal ValueTask<TValue> GetOrCreateAsync(
        TKey key,
        Func<CancellationToken, ValueTask<TValue>> factory)
    {
        return GetOrCreateAsync(key, factory, CancellationToken.None);
    }

    internal ValueTask<TValue> GetOrCreateAsync(
        TKey key,
        Func<CancellationToken, ValueTask<TValue>> factory,
        CancellationToken callerCancellation)
    {
        ArgumentNullException.ThrowIfNull(factory);

        Task<TValue> production;
        TaskCompletionSource<TValue>? completion = null;
        lock (_gate)
        {
            if (_entries.TryGetValue(key, out var node))
            {
                Touch(node);
                return ValueTask.FromResult(node.Value.Value);
            }

            if (!_inFlight.TryGetValue(key, out production!))
            {
                completion = new TaskCompletionSource<TValue>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
                production = completion.Task;
                _inFlight.Add(key, production);
            }
        }

        if (completion is not null)
        {
            _ = ProduceAsync(key, factory, completion);
        }

        return callerCancellation.CanBeCanceled
            ? new ValueTask<TValue>(production.WaitAsync(callerCancellation))
            : new ValueTask<TValue>(production);
    }

    private async Task ProduceAsync(
        TKey key,
        Func<CancellationToken, ValueTask<TValue>> factory,
        TaskCompletionSource<TValue> completion)
    {
        try
        {
            var value = await factory(CancellationToken.None).ConfigureAwait(false);
            var retainedBytes = _retainedBytesEstimator(value);
            ArgumentOutOfRangeException.ThrowIfNegative(retainedBytes);

            lock (_gate)
            {
                _inFlight.Remove(key);
                if (retainedBytes <= _retainedBytesLimit)
                {
                    Add(key, value, retainedBytes);
                }
            }

            completion.TrySetResult(value);
        }
        catch (OperationCanceledException exception)
        {
            RemoveInFlight(key);
            completion.TrySetCanceled(exception.CancellationToken);
        }
        catch (Exception exception)
        {
            RemoveInFlight(key);
            completion.TrySetException(exception);
        }
    }

    private void RemoveInFlight(TKey key)
    {
        lock (_gate)
        {
            _inFlight.Remove(key);
        }
    }

    private void Add(TKey key, TValue value, long retainedBytes)
    {
        var node = _lru.AddFirst(new CacheEntry(key, value, retainedBytes));
        _entries.Add(key, node);
        _retainedBytes += retainedBytes;

        while (_entries.Count > _entryLimit || _retainedBytes > _retainedBytesLimit)
        {
            var oldest = _lru.Last!;
            _lru.RemoveLast();
            _entries.Remove(oldest.Value.Key);
            _retainedBytes -= oldest.Value.RetainedBytes;
        }
    }

    private void Touch(LinkedListNode<CacheEntry> node)
    {
        if (!ReferenceEquals(node, _lru.First))
        {
            _lru.Remove(node);
            _lru.AddFirst(node);
        }
    }

    private sealed record CacheEntry(TKey Key, TValue Value, long RetainedBytes);
}
