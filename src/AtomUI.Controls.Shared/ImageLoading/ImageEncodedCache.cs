namespace AtomUI.Controls;

internal sealed class ImageEncodedCache : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<ImageEncodedCacheKey, CacheItem> _items = [];
    private readonly LinkedList<ImageEncodedCacheKey> _lru = [];
    private readonly long _maxBytes;
    private readonly int _maxEntries;
    private long _bytes;
    private bool _disposed;

    internal ImageEncodedCache(long maxBytes, int maxEntries)
    {
        _maxBytes = maxBytes;
        _maxEntries = maxEntries;
    }

    internal int Count
    {
        get { lock (_gate) return _items.Count; }
    }

    internal long Bytes
    {
        get { lock (_gate) return _bytes; }
    }

    internal bool TryGet(ImageEncodedCacheKey key, out ImageEncodedContent? content)
    {
        lock (_gate)
        {
            ThrowIfDisposed();
            if (!_items.TryGetValue(key, out var item))
            {
                content = null;
                return false;
            }
            _lru.Remove(item.Node);
            _lru.AddFirst(item.Node);
            content = item.Content.WithCacheSource(ImageCacheSource.EncodedMemory);
            return true;
        }
    }

    internal void Set(ImageEncodedCacheKey key, ImageEncodedContent content)
    {
        if (content.NoStore ||
            content.SecurityPolicyVersion != ImageSecurityPolicy.Version ||
            content.Size > _maxBytes)
        {
            return;
        }

        lock (_gate)
        {
            ThrowIfDisposed();
            RemoveCore(key);
            var node = _lru.AddFirst(key);
            _items.Add(key, new CacheItem(content with { CacheSource = ImageCacheSource.None }, node));
            _bytes += content.Size;
            TrimCore();
        }
    }

    internal void Remove(ImageEncodedCacheKey key)
    {
        lock (_gate)
        {
            if (!_disposed)
            {
                RemoveCore(key);
            }
        }
    }

    internal void Clear(string? partitionHash)
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }
            if (partitionHash is null)
            {
                _items.Clear();
                _lru.Clear();
                _bytes = 0;
                return;
            }
            foreach (var key in _items.Keys.Where(key => key.PartitionHash == partitionHash).ToArray())
            {
                RemoveCore(key);
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _items.Clear();
            _lru.Clear();
            _bytes = 0;
        }
    }

    private void TrimCore()
    {
        while ((_bytes > _maxBytes || _items.Count > _maxEntries) && _lru.Last is { } node)
        {
            RemoveCore(node.Value);
        }
    }

    private void RemoveCore(ImageEncodedCacheKey key)
    {
        if (!_items.Remove(key, out var existing))
        {
            return;
        }
        _lru.Remove(existing.Node);
        _bytes -= existing.Content.Size;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private sealed record CacheItem(ImageEncodedContent Content, LinkedListNode<ImageEncodedCacheKey> Node);
}
