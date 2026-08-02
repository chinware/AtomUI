using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Definitions;

internal sealed class ThemeDefinitionLoadCache : IDisposable
{
    private const int MaxEntriesPerKind = 256;
    private readonly object _gate = new();
    private readonly Dictionary<ThemeSourceCacheKey, ThemeSourceReadCacheEntry> _reads = new();
    private readonly Dictionary<ThemeBindingCacheKey, ThemeBindingCacheEntry> _bindings = new();
    private readonly Queue<ThemeSourceCacheKey> _readOrder = new();
    private readonly Queue<ThemeBindingCacheKey> _bindingOrder = new();
    private bool _disposed;

    internal bool TryGetRead(
        ThemeSourceCacheKey key,
        out ThemeSourceReadCacheEntry? entry)
    {
        lock (_gate)
        {
            if (_disposed)
            {
                entry = null;
                return false;
            }

            return _reads.TryGetValue(key, out entry);
        }
    }

    internal void StoreRead(
        ThemeSourceCacheKey key,
        ThemeSourceReadCacheEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        lock (_gate)
        {
            if (!_disposed)
            {
                if (!_reads.ContainsKey(key))
                {
                    _readOrder.Enqueue(key);
                }
                _reads[key] = entry;
                Trim(_reads, _readOrder);
            }
        }
    }

    internal bool TryGetBinding(
        ThemeBindingCacheKey key,
        out ThemeBindingCacheEntry? entry)
    {
        lock (_gate)
        {
            if (_disposed)
            {
                entry = null;
                return false;
            }

            return _bindings.TryGetValue(key, out entry);
        }
    }

    internal void StoreBinding(
        ThemeBindingCacheKey key,
        ThemeBindingCacheEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        lock (_gate)
        {
            if (!_disposed)
            {
                if (!_bindings.ContainsKey(key))
                {
                    _bindingOrder.Enqueue(key);
                }
                _bindings[key] = entry;
                Trim(_bindings, _bindingOrder);
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
            _reads.Clear();
            _bindings.Clear();
            _readOrder.Clear();
            _bindingOrder.Clear();
        }
    }

    private static void Trim<TKey, TValue>(
        Dictionary<TKey, TValue> values,
        Queue<TKey> order)
        where TKey : notnull
    {
        while (values.Count > MaxEntriesPerKind && order.TryDequeue(out var oldest))
        {
            values.Remove(oldest);
        }
    }
}

internal readonly record struct ThemeSourceCacheKey(
    string SourceIdentity,
    string SourceRevision);

internal readonly record struct ThemeBindingCacheKey(
    string SourceIdentity,
    string ContentDigest,
    ThemeSchemaRevision RegistryRevision);

internal sealed record ThemeSourceReadCacheEntry(
    ThemeDocument Document,
    string ContentDigest,
    IReadOnlyList<ThemeDiagnostic> Diagnostics);

internal sealed record ThemeBindingCacheEntry(
    BoundThemeDefinition Definition,
    IReadOnlyList<ThemeDiagnostic> Diagnostics);
